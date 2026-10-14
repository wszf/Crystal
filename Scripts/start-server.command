#!/bin/bash
set -euo pipefail
repo_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
data_dir="${1:-$repo_root/Build/ServerDemo}"
if [[ ! -f "$data_dir/Server.MirDB" || ! -d "$data_dir/Maps" || ! -d "$data_dir/Envir" ]]; then
    echo "Missing Server.MirDB, Maps or Envir in: $data_dir" >&2
    echo "Usage: $0 /path/to/server-data" >&2
    exit 1
fi
dotnet build "$repo_root/Server.Headless/Server.Headless.csproj" -c Release --nologo
cd "$data_dir"
exec dotnet "$repo_root/Build/Server/Release/net10.0/Server.Headless.dll"
