using Shared.Vfs;

static void Check(bool condition, string message)
{
    if (!condition) throw new Exception(message);
}

static void Reject(Action action, string message)
{
    try { action(); }
    catch (Exception ex) when (ex is IOException or InvalidDataException or System.Runtime.Serialization.SerializationException) { return; }
    throw new Exception(message);
}

// NRBF stream header followed by a boxed primitive or string, then MessageEnd.
// Fixtures use the old BinaryFormatter wire format, independently of the new writer.
static byte[] Legacy(object value)
{
    using var stream = new MemoryStream();
    using var writer = new BinaryWriter(stream);
    writer.Write((byte)0);
    writer.Write(1); writer.Write(-1); writer.Write(1); writer.Write(0);
    if (value is string text)
    {
        writer.Write((byte)6); writer.Write(1); writer.Write(text);
    }
    else
    {
        writer.Write((byte)4); writer.Write(1);
        writer.Write(value is bool ? "System.Boolean" : "System.Int64");
        writer.Write(1); writer.Write("m_value");
        writer.Write((byte)0); writer.Write((byte)(value is bool ? 1 : 9));
        if (value is bool flag) writer.Write(flag); else writer.Write((long)value);
    }
    writer.Write((byte)11);
    return stream.ToArray();
}

foreach (object value in new object[] { true, false, long.MinValue, long.MaxValue, 0L, "", "中文存档" })
{
    Check(Equals(value, Functions.DeserializeFromBytes(Legacy(value))), "Legacy value was lost.");
    Check(Equals(value, Functions.DeserializeFromBytes(Functions.SerializeToBytes(value))), "New value was lost.");
}
Check(Functions.DeserializeFromBytes(new byte[] { 0 }) == null, "Null value changed.");
Check(Functions.SerializeToBytes(true).SequenceEqual(new byte[] { 1, 1 }), "Existing bool format changed.");
Check(Functions.SerializeToBytes(42L).SequenceEqual(new byte[] { 2, 42, 0, 0, 0, 0, 0, 0, 0 }), "Existing long format changed.");
Reject(() => Functions.SerializeToBytes(42), "Unsupported values must not be silently lost.");
Reject(() => Functions.DeserializeFromBytes(new byte[] { 99 }), "Unknown tag accepted.");
Reject(() => Functions.DeserializeFromBytes(new byte[] { 2, 1 }), "Truncated long accepted.");
Reject(() => Functions.DeserializeFromBytes(new byte[] { 1, 1, 1 }), "Trailing data accepted.");
Reject(() => Functions.DeserializeFromBytes(Legacy(true)[..^1]), "Truncated legacy payload accepted.");
Console.WriteLine("PASS: legacy/current buff values, format stability, invalid data rejection");

var originalDirectory = Directory.GetCurrentDirectory();
var root = Path.Combine(Path.GetTempPath(), "crystal-vfs-" + Guid.NewGuid());
Directory.CreateDirectory(root);
try
{
    Directory.SetCurrentDirectory(root);
    Directory.CreateDirectory("Data/MixedCase");
    File.WriteAllText("Data/MixedCase/Existing.TXT", "existing");
    VfsManager.Initialize(root);
    Check(VfsManager.ReadAllText(@"data\mixedcase\existing.txt") == "existing", "Case-insensitive read failed.");
    VfsManager.WriteAllText(@"data\mixedcase\new.txt", "new");
    Check(File.Exists("Data/MixedCase/new.txt"), "New path was not normalized.");
    Check(!File.Exists(@"data\mixedcase\new.txt"), "Literal backslash file was created.");
    File.WriteAllText("Data/MixedCase/External.TXT", "external");
    Check(VfsManager.GetFilesMatching(@"data\mixedcase", "*.txt").Length == 3, "External file was omitted.");
    File.Delete("Data/MixedCase/Existing.TXT");
    Check(VfsManager.GetFilesMatching(@"data\mixedcase", "*.txt").Length == 2, "Deleted file is still listed.");
    Check(!VfsManager.FileExists(@"data\mixedcase\existing.txt"), "Deleted file still resolves as existing.");
    Directory.CreateDirectory(VfsManager.ResolveDir(@"data\mixedcase\NewDir"));
    Check(Directory.Exists("Data/MixedCase/NewDir"), "New directory path was not normalized.");
    Check(VfsManager.GetDirectoriesMatching("Data", "*", SearchOption.AllDirectories).Length == 2, "New directory was omitted.");
    // Resolving paths outside the indexed root must work too.
    Console.WriteLine("PASS: mixed-case reads, normalized writes, live file/directory listings");
}
finally
{
    Directory.SetCurrentDirectory(originalDirectory);
    Directory.Delete(root, recursive: true);
}
