### 2026-08-15 — 延迟毒伤必须区分挂入列表与当前状态广播

- Symptom: TucsonEgg 爆炸定向测试第一次得到 1 点额外 HP 损失；设置毒伤时间后，测试又把尚未到处理时刻的 `CurrentPoison` 当成未施毒。
- Root cause: Go 的零值 `TickAt` 会在同一世界 tick 立即处理 Green poison；修复后 Legacy-compatible poison 先进入列表，`CurrentPoison`/状态包要等下一次到期处理。测试混用了两个时序边界。
- Prevention: 所有延迟毒伤构造时显式设置 `TickAt = now + Tick`；命中时断言毒列表、到期时再断言伤害与状态包，不能用 `CurrentPoison` 替代挂入列表。
- Verification: TucsonEgg 爆炸现保持初始固定伤害、只新增 Green poison 列表；AI=128/129 定向测试和新增真实 SwampWarrior `net.Pipe` transcript 均通过。

### 2026-08-15 — 协议 helper 名称必须先从 Go 定义核对

- Symptom: Tucson net.Pipe transcript 首次编译使用不存在的 `protocol.ParseObjectAttackPayload`，包测试在实现行为验证前失败。
- Root cause: 看到请求侧 `ParseAttackPayload` 后按对称命名猜测服务端对象包 parser，未先检索协议包实际导出 API。
- Prevention: 新增 transcript 的每个 parser/helper 先用 `rg` 在当前 Go `internal/protocol` 定义中核对；若无 parser，直接与已核对的 wire builder 比较完整 payload，并立即运行包级最小编译。
- Verification: 改为比较 `ObjectAttackPayload(ObjectAttackInfo{...})` 的完整字节序列，`go test ./cmd/crystal-server -run TestSessionTucsonMageNormalAttackTranscript` 通过。

### 2026-08-13 — 战争颜色刷新与 BroadcastInfo 是两条独立协议副作用

- Symptom: Conquest 开始/结束只为 WarZone 变化的玩家发送对象刷新，遗漏其他地图的 Legacy `BroadcastInfo`；三人测试中的颜色包顺序还随 Go map 迭代随机变化。
- Root cause: 把 `RefreshNameColour` 的 self/object colour 包和 `StartWar`/`EndWar` 的全服逐玩家 `BroadcastInfo` 合并成一条局部通知路径，同时直接遍历无序玩家 map。
- Prevention: 分别建模颜色变化、全局逐 subject 的附近 `ObjectPlayer` 广播和 NPC 强制开战的额外 announcement 循环；所有可观察玩家遍历先按 ObjectID 排序，再用接收者矩阵锁定每条连接的顺序与重复次数。
- Verification: 参与地图与无关地图的双玩家矩阵均收到正确 `ObjectPlayer`，三玩家 START/STOP 精确包序连续运行 10 次稳定通过，真实会话仍保持颜色/聊天/响应/NPC 可见性顺序。

### 2026-08-13 — Hero 可见性必须按接收者矩阵拆分属性包

- Symptom: 初版 Hero 广播给所有观察者相同的 `ObjectHealth`/`ObjectMana`，并重复发送颜色包；Hero 名称颜色还直接沿用了内部 MediumOrchid，而 Legacy 的 viewer-relative 投影为 White。
- Root cause: 把 `Broadcast` 对象包、owner/group 属性可见性和 owner-only Mana 合并成一条通知路径，没有展开 `HumanObject.GetInfoEx(viewer)` 与召唤后的额外 owner 更新。
- Prevention: 明确矩阵：所有可见者收到 `ObjectHero` 和一次颜色；owner 与非零同组收到可见生命，owner 额外收到 Mana 及召唤后的第二次 Health；后续进入视野只发其当时有权看到的 Health，不补 Mana。对象名称颜色从 viewer-relative 投影生成。
- Verification: `TestGameWorldHeroSummonObserverPacketMatrix` 和 `TestRefreshStaticHeroVisibilityObserverMatrix` 分别锁定召唤时与后续入视野的 owner/group/普通观察者包序、数量、颜色和 Mana 边界。

### 2026-08-13 — Buff 协议对象必须使用运行时 ObjectID

- Symptom: 智能生物奖励和安全区 Buff 测试若使用持久化角色 Index 编码 `AddBuff`/`PauseBuff`，单角色 fixture 可能碰巧通过，但观察者会定位到错误世界对象。
- Root cause: 把数据库角色身份和当前 world 会话身份混用，没有在 wire transcript 中用不同数值锁定边界。
- Prevention: 所有 Object*、Buff 增删/暂停协议字段使用 `worldPlayer.ObjectID`；持久化 API 才使用 `SelectInfo.Index`。会话 fixture 必须断言 bootstrap 分配的 ObjectID 出现在 payload 中。
- Verification: WonderDrug `AddBuff` 与安全区 `PauseBuff` 会话测试都解析并断言真实运行时 ObjectID，同时验证 auth 与 JSON 持久化。

### 2026-08-13 — 地图领域通知与真实接收者矩阵必须同时验收

- Symptom: 智能生物召回/跨图/禁宠地图的领域状态正确，不代表 owner、旧地图观察者和目标地图观察者都收到正确的 Teleport/Remove/ObjectMonster/Health 顺序。
- Root cause: 只断言 world 对象坐标或通知总数，没有按每个接收者过滤领域通知并走真实会话投递。
- Prevention: 地图对象功能同时建立领域接收者矩阵和 net.Pipe 会话：分别断言 owner、旧观察者、新观察者的 packet ID、payload ObjectID 与顺序；禁用地图还要验证持久化解除召唤。
- Verification: 智能生物 visibility 测试覆盖第二观察者登录、移动、召回、跨图和禁宠地图，逐接收者断言通知并通过服务端整包测试。

### 2026-08-13 — 新功能 ordinal 必须套用当前基线的历史插入偏移

- Symptom: P11 首次把本地 Legacy enum 直接计数得到的钓鱼、觉醒和排行榜 ordinal 写入 Go，立即与当前 Go 的 `SendOutputMessage` 等常量冲突。
- Root cause: 忘记当前迁移 wire 基线在 `DeleteItem` 和 `TownRevive` 处各有一个历史插入；本地只读 C# 快照的后续枚举值必须整体加一，不能把原始计数直接用于当前基线。
- Prevention: 每批新增协议先定位目标包前后两个已迁移常量，机械应用已记录的方向插入偏移，再在同一 patch 中加入显式 ordinal 和方向唯一性测试；任何冲突都先修正基线计算，不能挪用空闲 ID。
- Verification: P11 生产常量改为 Server `ObjectEffect=125`、`FishingUpdate=201`、`NPCAwakening=225`、`Rankings=253`，Client `FishingCast=103`、`AwakeningNeedMaterials=112`、`GetRanking=136`；现有 ordinal 测试表在统一偏移循环前故意填写 C# 快照原值，`got` 才填写当前生产值。首次把当前值直接写进 `wants` 导致测试再次加一，现已修正并由完整唯一性测试锁定。

### 2026-08-12 — 公会 ordinal 必须锁定目标 wire 基线而非相似源码快照

- Symptom: 初次从本地 `Shared/Enums.cs` 计数得到公会 Client `79..83`、Server `84/166..171`，但当前迁移目标的精确 wire 基线实际整体高一位，并且需要包含 `GuildExpGain=171`。
- Root cause: 把工作区中的一个 enum 快照直接当成最终目标版本，未先与主线正在使用的当前协议基线交叉确认；同时只按用户最初列举包名收口，漏掉了夹在 `GuildInvite` 与 `GuildNameRequest` 之间、决定后者 ordinal 的 `GuildExpGain`。
- Prevention: 对版本可能漂移的协议枚举，同时核对完整成员序列、主线当前 wire 基线和相邻占位包；显式测试必须包含整个连续区间，不能只断言功能入口包。收到权威 ordinal 修正后，以其为当前迁移基线并检查相邻常量是否遗漏。
- Verification: 公会测试现显式断言 Client `80..84`、Server `ObjectGuildNameChanged=85` 与 `GuildNoticeChange..GuildNameRequest=167..172`，并覆盖 `GuildExpGain` UInt32 小端 payload；`go test ./internal/protocol -count=1` 通过。
- Strengthening after recurrence: 插入枚举成员会让插入点后的所有现有常量漂移，局部修目标包会制造跨功能 ID 冲突（本次为 `ObjectGuildNameChanged=85` 与旧 `GainExperience=85`）。今后发现插入点后必须机械审计该方向所有已定义常量，更新所有领域 ordinal 测试，并让唯一性测试枚举生产常量表中的每个已定义 ID；当前验证覆盖 175 个 Server 与 116 个 Client 常量，且锁定 Server `HealthChanged=77 → DeleteItem=80 → ObjectGuildNameChanged=85 → GainExperience=86`、Client `GroupInvite=62 → TownRevive=69 → EditGuildMember=80` 边界。

### 2026-08-11 — 时间值进入协议结构前必须显式转换为 .NET binary

- Symptom: 存储密码接线后编译失败，`time.Time` 被赋给协议结构的 `int64 StoragePasswordLastSet`。
- Root cause: auth 层使用语义化的 `time.Time`，而 Crystal wire payload 使用 `DateTime.ToBinary()` 的 64 位值，边界转换遗漏。
- Prevention: 领域层保留 `time.Time`，进入协议结构或 payload 时统一调用 `protocol.DotNetDateTimeBinary`，并为零时间保留 .NET `DateTime.MinValue` 的编码。
- Verification: 修正后重新运行完整 Go 测试、`go vet`、race 测试和构建。

### 2026-08-11 — Packet ordinal 必须从完整 enum 锁定

- Symptom: 对照怪物 packet 时发现已有 Go 的 NPC、Player、Storage 等常量因遗漏原版 enum 成员而整体偏移，测试虽然通过但真实客户端会把 packet 解释成别的类型。
- Root cause: 只按相邻名称或记忆手写 ordinal，没有从 `Shared/Enums.cs` 的完整 `ServerPacketIds`/`ClientPacketIds` 顺序核对。
- Prevention: 每条新增 packet 先从完整 enum 计算 ordinal；Go 协议包测试必须包含关键 ID 的显式 legacy ordinal 断言，禁止只使用 Go 常量自洽测试。
- Verification: 修正 `ObjectPlayer`、`NewMonsterInfo`、`NewNPCInfo`、`ObjectMonster`、`ObjectNPC`、NPC/Storage 以及 Monster/NPC 请求 IDs，并通过 `TestPacketIDsMatchLegacyEnums` 与全量 Go 测试。

### 2026-08-11 — 复合协议包必须覆盖外层字段

- Symptom: 地图信息测试能正确解析标题和地图内容，但对照 `NewMapInfo.WritePacket` 时发现 Go payload 遗漏了开头的 `MapIndex`；真实客户端会从错误的偏移读取标题。
- Root cause: 只验证了复用的 `ClientMapInfo.Save` 内容，没有把外层 packet 类自己的字段纳入 payload 对照。
- Prevention: 每个复合 packet 先分别列出 packet 外层字段和嵌套对象字段；测试从第一个字节开始解析完整 payload，并断言外层索引、长度和嵌套字段。
- Verification: `NewMapInfoPayload` 现在先写入 `MapIndex`，协议、world 和 net.Pipe 测试均从完整 payload 解析并通过。

### 2026-08-11 — 地图切换可见性必须双向刷新

- Symptom: 传送后的玩家会被发送给目标地图观察者，但当前玩家没有收到目标地图已有玩家对象。
- Root cause: 只实现了旧地图 `ObjectRemove` 和当前玩家向新地图广播，遗漏了目标地图已有对象的 `GetObjects` 对等行为。
- Prevention: 地图切换验收同时断言旧观察者收到移除、新观察者收到当前玩家对象、当前玩家收到新地图已有玩家对象，并在静态对象刷新前完成玩家对象同步。
- Verification: transition world 状态测试锁定 old/new map 集合，net.Pipe 路径发送双向 player packet；全量 Go 测试通过。

### 2026-08-11 — 协议类型命名必须先避开已有 packet 常量

- Symptom: 新增客户端魔法资料结构时，Go 编译器报告 `ClientMagic (constant) is not a type` 和同名类型重复声明。
- Root cause: `internal/protocol` 已经用 `ClientMagic` 表示客户端 packet ordinal，却在同一包中再次使用该标识符声明 wire 数据类型。
- Prevention: 新增协议领域类型前先检索同包常量、函数和类型名称；packet ID 保留 legacy 名称，数据结构使用 `ClientMagicInfo` 等不冲突的明确后缀。
- Verification: 重命名后定向协议/auth/world 测试、Go 全量测试、race、vet 和 build 全部通过。

### 2026-08-11 — world 通知的指针和值边界必须显式处理

- Symptom: Healing action 编译失败，`worldPlayer` 指针不能直接赋给 `worldNotification.Recipient` 的值类型字段。
- Root cause: 世界对象 map 保存 `*worldPlayer`，而通知快照刻意保存值副本；新增通知路径遗漏了显式解引用。
- Prevention: 读取对象 map 后先确认 API 需要指针还是快照值；进入通知、排序或跨锁返回值时统一使用显式 `*player` 副本，并在编译后检查接收者身份。
- Verification: 改为 `Recipient: *target` 后，Healing 定向测试、Go 全量测试、race、vet 和 build 均通过。

### 2026-08-11 — 系统提示文本也属于可观察协议行为

- Symptom: 地图禁止丢弃和 Owner 拒绝拾取的 Go 提示初稿语义相近但不等于原版默认本地化文本。
- Root cause: 只迁移了 Chat packet 类型，没有从 `Shared/Language.cs` 核对 `ServerTextKeys` 的默认字符串。
- Prevention: 对每个失败/提示分支同时对照 packet 类型、默认文本和参数；未迁移本地化表时先使用原版英文键值，不自行改写措辞。
- Verification: Go handler 已使用 `CanNotDrop` 和 `CannotPickupNotOwner` 的默认文本，并通过全量 Go 测试与差异检查。

### 2026-08-11 — Repair ordinal 与浮点费用必须从原版逐项核算

- Symptom: Repair 初稿沿用了摘要中的客户端 ordinal `55/57`，与原版 `MagicKey=57` 冲突；会话测试的 125% 费用也曾把 `75*1.25` 写成 94。
- Root cause: 依赖二手摘要和心算，没有从完整 `ClientPacketIds` 枚举以及 C# `float` 截断规则分别建立证据和计算表。
- Prevention: 新增 packet 先从完整 `Shared/Enums.cs` 计算 ordinal 并加入显式 legacy wants；涉及金额时按原版字段类型、运算顺序、截断点和每一步余额写出 transcript，再填写断言。
- Verification: 原版核对确认 `RepairItem=54`、`SRepairItem=56`；协议 ordinal/payload 测试与普通/特殊 Repair net.Pipe transcript 通过，费用断言分别为 93 和 375。

### 2026-08-11 — 断线广播不能被失效连接短路

- Symptom: 组队成员通过 net.Pipe 断开后，在线 leader 先收到 ObjectRemove，收不到应有的 DeleteGroup。
- Root cause: 广播按通知顺序写入时，第一个已断开成员的写操作报错，deliverWorldNotifications 立即返回，后续在线成员没有收到通知。
- Prevention: 广播遍历必须继续投递所有 recipient，只保存并返回第一个错误；新增断线场景要同时断言在线成员的协议包顺序和 world 清理结果。
- Verification: 修正广播后双会话测试稳定收到 DeleteGroup → ObjectRemove；普通测试、race、vet 和 build 全部通过。

### 2026-08-12 — 定时协议常量必须先查完整枚举

- Symptom: 启动状态机补任务定时包时曾使用不存在的 `protocol.ServerQuestExpired`，导致新增测试无法编译。
- Root cause: 按业务语义猜测了包名，没有先检索 legacy `ServerPacketIds` 和 Go 常量表；实际任务计时使用通用 `ServerSetTimer`。
- Prevention: 新增 transcript 常量前先用 `rg` 同时核对 legacy enum、packet class 和 Go 常量，并在同一修改中补显式 ordinal 断言；禁止从功能名称推导常量名。
- Verification: bootstrap helper 现在只接受已核实的 `ServerSetTimer`/`ServerChangeQuest`，协议和服务端全量门禁通过。

### 2026-08-12 — 地图切换包序列必须按触发路径拆分

- Symptom: Go 曾把地图坐标移动、NPC `ENTERMAP`、NPC `MOVE` 和 RequiredGroup 强制离开统一投递为 `MapChanged + UserLocation`，导致移动测试读取到后续 NPC 包，并掩盖 `ObjectTeleportIn` 的差异。
- Root cause: 复用了一个通用 transition helper，没有保留 legacy `CompleteMapMovement`、`Teleport(..., false)` 与默认 effectful `Teleport` 三条独立调用链。
- Prevention: transition 先标注触发类型，再分别锁定坐标移动/ENTERMAP 仅 `MapChanged`，MOVE/强制传送为 `MapChanged + ObjectTeleportIn`，TownRevive 使用自己的 `MapChanged` 路径；禁止用统一的 `UserLocation` 补包。
- Verification: 更新 movement、ENTERMAP、MOVE、RequiredGroup 和 Rental 顺序测试后，Go 全量普通/race 测试、vet、build 与差异检查通过。

### 2026-08-12 — 注销通知快照与投递时机必须分离

- Symptom: 公会注销状态包若在 `world.leave` 前立即投递，会先写向正在关闭且无人读取的 net.Pipe，阻塞后续在线成员；若在 leave 后才构造，又丢失注销者姓名并退化为角色数字 ID。
- Root cause: 把“从在线 world 读取姓名并生成通知”和“向剩余会话实际写包”绑定在同一步，没有保留快照边界。
- Prevention: 离线流程先在玩家仍在线时提交 auth 状态并构造包含姓名的通知快照，完成 world 移除后再投递；通知接收者列表排除注销者，持久化在状态提交后执行。
- Verification: 双成员 world 测试锁定仅另一成员收到 `GuildMemberChange(Status=0, Name=leader)`；公会会话测试、Go 全量普通/race 门禁通过。

### 2026-08-14 — 广播包必须按源 ObjectID 转发给 Observer

- Symptom: 复核 Observer forwarding 时发现把每个 `worldPlayer.Send` 回调都转发给该回调所属玩家的观察者，会把发送给附近接收者的源玩家动作包误投给错误观察者并可能重复。
- Root cause: 把“通知接收者的 Send”误当成“动作源的 Send”，没有区分 `notifyPlayers` 的 recipient 与产生 `ObjectTurn/Attack/RangeAttack/Magic` 的 source。
- Prevention: Observer forwarding 只由动作源路径显式提交带源 ObjectID 的 packet；需要复用 world notification 时先确认源包是否也会发给源连接，禁止在通用 recipient callback 中推断来源。
- Verification: 攻击、远程攻击改为 source `ObserverPacket`，移动和魔法保留 source path 转发；全仓普通/race 测试、vet/build 与 Observer transcript 均通过。

### 2026-08-18 — DragonWarrior 新 AI 必须核对方法接收者

- Symptom: DragonWarrior 首次包级编译失败，新增文件把 `elephantManAttackPowerLocked` 当作包级函数调用，Go 报 `undefined`；行为测试尚未执行。
- Root cause: 读取了该 helper 的签名，却遗漏了它属于 `gameWorld` 的方法接收者，在新 AI 文件中凭函数名复制调用。
- Prevention: 新 AI 接入现有 helper 前同时记录完整签名和接收者；新增文件完成后立即运行 `gofmt` 与 `go test ./cmd/crystal-server -run '^$' -count=1`。
- Verification: 三处调用已改为 `w.elephantManAttackPowerLocked`，格式化后的包级编译通过。

### 2026-08-18 — AntCommander 协议 helper 必须区分 Packet 与通知包装

- Symptom: AI=196 首次编译时，Dazed effect helper 返回 `worldNotification`，但调用点需要可复用的 `protocol.Packet`，`go test ./cmd/crystal-server` 报类型不匹配。
- Root cause: 把协议 payload 构造和接收者/范围通知包装合并在一个 helper 中，导致不同层级的值被交叉传递。
- Prevention: 可复用的 effect helper 只返回 `protocol.Packet`；每个调用点再显式创建私有通知或 `notifyPlayersLocked` 范围通知。
- Verification: 修正后 `gofmt`、AI=196 定向世界测试、认证 `net.Pipe` transcript 以及 `go test ./cmd/crystal-server` 均通过。

### 2026-08-18 — AI=210 WakeAll 的关联 ObjectShow 也必须纳入通知基线

- Symptom: 为验证 stoned scroll 的 WakeAll 新增关联 Zuma fixture 后，测试仍只期望自身一条 ObjectShow，实际观察者收到自身和关联对象各一条。
- Root cause: Legacy WakeAll 会唤醒 14 格内的其他 Zuma 后逐个广播 ObjectShow；测试只覆盖了自身 Wake，遗漏了共享唤醒的客户端可观察通知。
- Prevention: WakeAll 回归同时建立至少一个关联 stoned Zuma，断言所有唤醒状态、目标继承（无预存 Target 时保持零值）和观察者通知数量/顺序；新增失败后先更新基线再提交。
- Verification: AI=210 wake test 现断言两条 ObjectShow、关联对象保持零目标并成功唤醒；定向普通/race 与全量门禁均通过。

### 2026-08-18 — AI=218 SepArcher 目标对象类型决定通知接收者

- Symptom: SepArcher Monster/Hero 目标的定向测试按目标 ObjectID 查找 `ObjectMagic`，得到空包；Player 目标同一断言通过，延迟伤害 action 已正确排队。
- Root cause: `notifyPlayersLocked` 的可观察接收者是附近 Player；Legacy Monster/Hero 的 `Broadcast` 也不会把非 Player 目标当成网络连接，Monster/Hero 场景应通过其主人 Player 验证攻击包。
- Prevention: 编写跨 Player/owned-Monster/Hero 的协议 transcript 时，先区分“受伤对象”与“网络接收者”；攻击包按实际附近 Player（宠物/英雄使用 Owner）断言，HP/action 仍按目标对象断言。
- Verification: 将 Monster/Hero 包断言改为主人 Player 接收者后，AI=218 定向普通测试连续 5 次、定向 race 测试 3 次，以及排除两条已知 transcript 抖动的全仓普通/race 门禁通过。

### 2026-08-18 — SoulFireBall 接入要核对 Go receiver 与协议字段

- Symptom: SoulFireBall 首次目标包编译分别报告未定义的 `stoningStatueHeroMissLocked` 和不存在的 `MagicRequest.TargetY`。
- Root cause: 新增方法调用漏写 `w.`，测试夹具又凭 C#/坐标命名习惯猜测了协议字段；实际请求字段是 `LocationY`。
- Prevention: 新增方法在目标包编译前逐一核对 receiver；构造协议请求只使用已打开的 Go `protocol` 定义，不凭记忆补字段名。
- Verification: 修正 receiver 与字段后，SoulFireBall 目标测试编译并通过；随后普通/race 目标回归通过。

### 2026-08-18 — 新增快照字段要用旧版默认赋值更新既有 wire 基线

- Symptom: Full Go regression failed in the poison ObjectPlayer snapshot because the new serializer emitted `ElementOrbMax=200` while the old test expected the pre-migration zero value.
- Root cause: The test baseline predated the elemental fields; it was not compared against Legacy `PlayerObject.GetInfo`, which always assigns the last `OrbsExpList` value even with no active orb.
- Prevention: When extending a serialized snapshot, inspect the reference constructor's unconditional/default assignments and update exact-byte tests accordingly; do not suppress a reference field merely to preserve an older Go expectation.
- Verification: Legacy-only source inspection confirmed Player/Hero `ElementOrbMax` always uses the final orb threshold; the poison snapshot now expects 200 and its targeted regression passes.

### 2026-08-18 — BackStep 受阻仍向附近其他玩家广播距离 0

- Symptom: BackStep 完全受阻测试首次只预期一个观察者通知，实际收到两个 `ObjectBackStep`。
- Root cause: Legacy `HumanObject.BackStep` 在无可移动格时仍调用 `Broadcast`，因此包括占位阻挡玩家在内的所有附近非施法者都会收到距离 0 的包。
- Prevention: 位移/击退测试把阻挡对象也纳入广播接收者集合；分别断言移动距离、通知接收者和距离 0 的 wire payload，不把“未移动”误判为“无广播”。
- Verification: BackStep 成功、部分受阻和完全受阻定向测试均通过。

### 2026-08-18 — 隐身清理通知只发送实际存在的 buff

- Symptom: MoonLight 命中揭示测试收到两个连续 `ServerRemoveBuff`，虽然玩家只持有 MoonLight。
- Root cause: 清理逻辑遍历支持的 MoonLight/DarkBody 类型列表时没有记录实际命中的类型，向客户端无条件发送了两种移除包。
- Prevention: 状态删除与协议通知使用同一份 `removedTypes` 集合；测试同时断言 buff 状态和目标收件人的包顺序/数量。
- Verification: 修正后 MoonLight 世界、命中揭示和认证 `net.Pipe` 测试通过，当前只有 MoonLight 时只产生一个 RemoveBuff。

### 2026-08-18 — 前置施法通知必须在技能专属通知中追加

- Symptom: 复核 CrescentSlash 分支发现直接赋值练功通知会覆盖隐身施法路径先生成的 `RemoveBuff` 通知。
- Root cause: 把技能 helper 返回的通知当成该阶段唯一来源，忽略了 `HumanObject.Magic` 在 switch 前已经追加的通用前置副作用。
- Prevention: 分支 helper 只返回专属通知；接入 `magicAttack` 时统一使用 `append` 保留全局前置通知，并为 Hidden/RemoveBuff 组合路径保留顺序断言。
- Verification: 已改为追加语义，Go 包编译、CrescentSlash 世界/会话/race 定向测试通过，前置通知不会被覆盖。

### 2026-08-19 — 新增协议固定向量必须先按完整帧编码规则计算长度

- Symptom: `Server.UserName` 固定向量首次把长度字段写成 16，`TestVerifyVectors` 实际得到 18；载荷本身和解析都正确，但探针门禁失败。
- Root cause: 将长度字段误当成“包 ID 加载荷”而漏算 `Encode` 规定的 4 字节帧头；既有协议帧的长度是头部、ID 和载荷的总长度。
- Prevention: 每个新 wire vector 同时按 `Encode` 的 `HeaderSize + len(payload)` 计算长度，并优先用 ASCII/UTF-8 字符数与字节数分开核对；不要凭载荷字段总和手写首两个字节。
- Verification: 修正为 18 字节后，`TestVerifyVectors`、协议/探针/服务器定向测试全部通过。

### 2026-08-20 — BoneLord 阶段通知期望必须使用协议常量

- Symptom: BoneLord 阶段测试实际收到 `ObjectAttack + 8×ObjectMonster`，但期望数组中的后八项是 `0`，导致通知 ID 比较失败。
- Root cause: 用 `make([]int16, waveSize)` 只创建了长度，没有把每个元素填成 `protocol.ServerObjectMonster`。
- Prevention: 构造重复协议 ID 期望时显式循环填充协议常量，避免把零值当成合法包 ID；优先复用已有 `packetIDs`/逐包 helper。
- Verification: 修正填充后 `go test ./cmd/crystal-server -run 'BoneLord' -count=1` 通过。

### 2026-08-20 — Go 协议模型字段必须先核对再移植 Legacy 状态

- Symptom: RightGuard 搜索移植初次引用 `protocol.SelectInfo.GMGameMaster`，Go 编译失败，因为当前 `SelectInfo` 没有该 Legacy 字段。
- Root cause: 直接把 C# `PlayerObject` 状态字段当成 Go 协议模型已存在，未先检索 `internal/protocol` 和 `worldPlayer` 的可观察字段。
- Prevention: 移植每个 Legacy 状态条件前先核对 Go 数据模型；缺失字段不得凭空添加到本批，必须按现有 authoritative 状态决定是否可表达，并在 handoff 标注未建模边界。
- Verification: 删除不可表达的字段检查后，RightGuard/LeftGuard 定向测试、全模块测试、vet/build 和定向 race 通过；Legacy C# 文件仍未修改。

### 2026-08-20 — TrapRock 协议新增必须同步 ordinal、payload 和 movement gate

- Symptom: AI=47 需要 Legacy `InTrapRock` 单布尔包，Go 协议原先没有 ordinal/payload；被困玩家仍可能通过普通 movement capability 移动。
- Root cause: 只迁移了对象生命周期，没有把 MapObject setter 的 wire side effect 与 HumanObject.CanWalk 门禁作为同一状态边界。
- Prevention: 新增协议包时同时更新 Go ordinal 向量、编码/解析测试和实体 movement gate；TrapRock 目标状态只通过权威 world 字段和专用包同步。
- Verification: `InTrapRock` protocol round-trip、认证显形 transcript 和 movement gate 测试通过，完整 protocol 包测试保持绿色。

### 2026-08-20 — ThunderElement ObjectAttack 必须在全部目标伤害之后广播

- Symptom: ThunderElement 初版在逐目标伤害前广播 `ObjectAttack`，包序与 Legacy `CompleteAttack` 不同。
- Root cause: 把 admission 时的普通攻击通知模式直接复用到 impact-time AOE；Legacy 会先完成所有目标的 `Attacked` 回调，再发布攻击包。
- Prevention: 延迟 AOE resolver 先完成全部目标有效性检查和伤害，再追加一次 `ObjectAttack`；测试按每个目标的伤害/状态包后再读攻击包。
- Verification: `TestGameWorldThunderElementAdmissionMovesThenAttacksAndRescans` 的通知顺序断言通过，AI=49 定向普通/race 测试均通过。

### 2026-08-20 — GreatFoxSpirit effect 广播必须以 GreatFox 自身坐标为源

- Symptom: GreatFoxSpirit 远程攻击的 effect=8 对目标可见，但初版以目标坐标作为广播中心，远处观察者收到错误的可见性结果。
- Root cause: effect 通知沿用了目标状态更新的坐标，而 Legacy `SpellEffect` 是由 GreatFoxSpirit 在自身位置广播。
- Prevention: 区分攻击者源坐标与目标 payload；每个远程 effect 都以 GreatFox 自身地图/坐标调用 observer fan-out，并在会话 transcript 覆盖接收者矩阵。
- Verification: `TestSessionGreatFoxRangeAttackTranscript` 锁定 `ObjectRangeAttack -> ObjectEffect` 及源坐标，AI=50 定向普通/race 测试通过。


### 2026-08-23 — 新增登录 bootstrap 包必须覆盖所有手写消费器并先补测试 import

- Symptom: `BaseStatsInfo` 首次定向编译因新断言使用 `reflect.DeepEqual` 却遗漏 `reflect` import；修正后服务端整包测试又有多组 NPC/Guild/Conquest/Shop/Relationship 手写 bootstrap 消费器把合法的新 ordinal 163 当成意外包。
- Root cause: 只更新了生产包序和通用 `startGameBootstrapForTest`/mail helper，没有在首次整包运行前机械枚举所有独立的显式 expected 列表与 switch；测试补丁也未先复核新增符号对应的 import。
- Prevention: 任何共享 bootstrap 包变更先用相邻稳定包（本次为 `MentorUpdate`、active quest、visible object、`TimeOfDay`）检索全部测试消费器，按 Legacy FIFO 边界一次性更新；新增测试引用后立即运行包级只编译，再运行定向测试和整包测试。
- Verification: 所有手写消费者现将 `BaseStatsInfo` 固定在 active quest 之后、被动对象之前；协议/探针/服务端定向重复与 race、服务端整包、全仓普通/race、vet、build 和 diff 检查均退出 0。
- Strengthening after review: “五职业默认公式完整匹配”不能只比较每类 count、first 和 last；`base_stats_test.go` 现逐项比较全部 55 个 `BaseStat` 记录及九个 cap，避免中间项类型、公式、Gain 或 GainRate 漂移而测试仍通过。
- Strengthening after final gate: 强化公式测试后的定向普通/race、全仓普通、vet、build 均通过；完整 race 后续命中已归档的 `TestSessionDarkBodySpawnAndRecallTranscript` 装备 Buff reconciliation 竞争，栈未进入 BaseStats production/protocol/probe/tests，不能继续沿用“本次完整 race 退出 0”的旧快照。
- Strengthening after conditional-packet recurrence: persisted combat `SpellToggle` 被加在 `GuildBuffList` 后时，首轮组合定向测试无输出挂起；隔离 `TestSessionHalfMoonAttackTranscriptAndPersistence` 后在 30 秒超时栈中确认 client 正写攻击请求、server 正写未消费的 bootstrap toggle，形成 `net.Pipe` write/write deadlock。根因是通用 helper 在 `GuildBuffList` 返回后，已有 HalfMoon/Thrusting/DoubleSlash/FatalSword 与 observer 夹具仍假设 bootstrap 已结束。新增条件式 server-first 包时，除检索包 ID 消费器外，还必须机械检索所有会使条件成立的状态 seed API/字段；直接 `net.Pipe` 客户端必须在发请求前逐包 drain，带后台 reader 的客户端也必须在 barrier 前验证并消费。四个 warrior transcript、target observer bootstrap 与新全四-toggle transcript 已显式消费固定顺序；定向普通 `-count=10`、race `-count=3`、服务端整包、全仓普通/race、vet、build 均通过。

### 2026-08-23 — BaseStats 配置必须分离序列化字段、计算短路与 runtime authority

- Symptom: 自定义 BaseStats 首次定向测试把 `Gain == 0` 的记录误期望为同时清零 `GainRate/Max`，并把 Wizard Mana 公式在 level 10 的结果误算成 112 而非 129；恢复时还发现未提交实现只加载配置，却遗漏 `serveWithConfig`、player bootstrap packet 与 `heroCharacter` profile 传播。
- Root cause: 把 `BaseStat.Calculate` 的 `Gain == 0` 计算短路误当成 INI/wire 字段规范化，并把通用 Mana 公式套到 Wizard 特例；同时只审查了 loader/公式 helper，没有沿 `Settings -> HumanObject/HeroObject.RefreshLevelStats -> SendBaseStats` 的完整 authority 链检查所有消费者。
- Prevention: 自定义公式测试必须分别断言原始 profile/wire 字段和计算结果；逐项代入 Health/Mana/Weight/Stat 及 Warrior/Wizard/Taoist 分支；配置 profile 作为 runtime-only authority，在 player enter、Hero summon、equipment calculation 和对应 BaseStats packet 四个边界显式传播，并用 `json:"-"` 锁定不落盘。
- Verification: 修正两项测试期望后，五职业默认、custom INI 顺序/数字零 fallback、四公式/Max/Gain==0、player runtime/cap/authenticated bootstrap、Hero runtime/packet/authenticated summon 和 transient JSON 测试普通 `-count=10`、race `-count=3` 均通过；全仓普通重跑、完整 race、vet 和 build 退出 0。

### 2026-08-23 — Notice bootstrap 必须以登出时间 authority 保证只投递一次

- Symptom: Go 已保留 `SelectInfo.LastAccess` 和 `LastLoginDate` 字段，却没有在真实 StartGame/StopGame 生命周期更新它们；若只增加 `UpdateNotice` 包，旧公告会在每次重登重复发送。实现初稿还用 `c.NoticeLastUpdate.UTC()` 作为 loader reset，重复加载缺失文件时会错误保留旧时间。
- Root cause: 把账户导入字段视为静态兼容元数据，没有追踪 Legacy `PlayerObject` 构造、`StartGameSuccess`、`StopGame` 与 `CharacterInfo.ToSelectInfo` 的完整 authority 链；同时把时间 location 规范化误当成零值清理。
- Prevention: 条件 bootstrap 必须同时迁移 seed 的写入生命周期；公告 admission 使用非空 Message 且严格 `Notice.LastUpdate > LastLogoutDate`，StartGame 写 `LastLoginDate`，显式登出、observer takeover 和断线 cleanup 在 JSON/checkpoint 前写 `LastAccess`；optional loader 每次先显式清空 payload 与 timestamp，再处理 missing/empty 文件。协议必须锁定 ordinal、完整 payload、网络 probe 和 pre-item FIFO 边界。
- Verification: Go 配置测试锁定 BOM、CR/LF、`string.Compare`/`Split('=')[1]` 怪癖、missing/empty reset 和文件时间；ordinal 272 固定向量与 malformed parser 通过；authenticated `net.Pipe` 首登→公告→显式登出→JSON restart→不重复公告→断线 checkpoint transcript 普通 `-count=10`、race `-count=3` 通过，全仓普通/race、vet、build 和 Go probe vectors 均退出 0。

### 2026-08-23 — localized welcome 必须同步 loader、probe 与全部 StartGame 消费器

- Symptom: compact 后 review 发现未提交 welcome loader 直接对 Go struct `json.Unmarshal`，会拒绝 .NET `File.ReadAllText` 已去除的 UTF-8 BOM，并错误接受 Legacy `System.Text.Json` 默认不接受的小写根键 `text`；同时生产网络 probe 在 `StartGame` 后仍直接进入旧 bootstrap state machine，会把新增的合法 `Server.Chat(Hint)` 当成意外包。
- Root cause: 把“沿用现有 Chat ordinal/payload”误当成无需更新协议消费者，只机械修改了 server tests；配置层也只验证了字段值，没有对照文件解码与属性名匹配语义。
- Prevention: 每个 server-first bootstrap 变更必须机械枚举生产 probe 和所有发送 `ClientStartGame` 的手写 consumer；配置 JSON 必须分别核对 BOM、根属性大小写、未知 key、malformed/missing fallback 和环境/INI path authority，不能依赖 Go 默认 decoder 恰好可用。
- Verification: Go loader 现去除 UTF-8 BOM、精确读取 `Text` 并只覆盖 `GameName`/`Welcome`；网络 probe 强制消费并解析 post-StartGame Hint，测试使用非 ASCII payload 并拒绝非 Hint。config/server/probe focused 普通 `-count=10` 与 race `-count=3`、排除既有 OmaMage 的全仓普通/race、vet、build、probe vectors 与 diff check 均退出 0；无排除普通与最终完整 race 仅命中同一 OmaMage 随机边界且未报告数据竞争。所有 22 个直接 `ClientStartGame` 文件已机械枚举并覆盖。

### 2026-08-23 — TestServer bootstrap 必须分离配置、瞬态模式与管理员 authority

- Symptom: Go 已读取 `[General] TestServer` 并把它用于部分特殊命令，但成功 StartGame 仍缺少 Legacy 的两条 Hint，玩家也没有 `Chat("@GAMEMASTER")` 所建立的攻击目标免疫；直接把账户 `AdminAccount` 当作替代会错误扩大权限和持久化语义。
- Root cause: 把同名 GameMaster 概念合并：Legacy TestServer 分支只发送 `GameIsTestMode -> GameMasterMode` 并切换瞬态 `GMGameMaster`，而 `IsGM` 来自账户管理员 authority，只有后者才生成 GameMaster Buff/options 并开放管理命令。
- Prevention: StartGame 条件分支分别记录 packet FIFO、runtime-only flag、账户权限和 Buff 投影；TestServer 的非管理员模式只进入玩家/怪物 `IsAttackTarget` 门禁，不写回角色，也不凭空获得 admin capability。生产 probe 在不知道远端配置时只接受零条或完整两条 Hint，拒绝半对。
- Verification: Go localization 覆盖两个新 key 及 English fallback；authenticated `net.Pipe` 锁定 `StartGame -> Welcome -> TestMode Hint -> GameMasterMode Hint -> map/bootstrap`，并在 KeepAlive barrier 后回读 runtime flag；player/monster target tests 锁定模式开关，probe 接受 0/2 且拒绝 1。focused 普通 `-count=10`、race `-count=3`、服务端整包、全仓普通/race、vet、build、probe vectors 与 diff check 全部退出 0。

### 2026-08-23 — 管理员 GameMaster bootstrap 必须锁定完整重复包序与旧 consumer

- Symptom: 管理员 `IsGM`/GameMaster Buff 实现的定向测试通过后，服务端整包唯一失败为 `TestObserverAdminChatBypassesPermissionsAndGlobalGate`：通用 mail bootstrap helper 在 `GuildBuffList` 返回，合法的末尾 `AddBuff(GameMaster)` 未消费，随后被误当成 observer bootstrap；review 还发现生产 probe 虽把测试命名为“zero/one/two tail”，实现却会无限接受 AddBuff，并错误接受没有前置 restore 的单独 RemoveBuff。
- Root cause: 把 `GuildBuffList` 当成稳定 game-loop 屏障，没有按 Legacy `restore Buffs -> final UpdateGMBuff` 展开管理员条件包；probe 只校验单包类型，没有建模 fresh admin、stored admin、TestServer admin 与 revoked authority 四种精确状态机。
- Prevention: 对每种 authority/config/persisted-state 组合列出 owner/nearby 的完整包序：fresh admin 一个 final AddBuff，stored admin restore+final，TestServer admin early+restore+final，revoked non-admin restore+remove；新增条件 bootstrap 后机械更新所有会 seed AdminAccount 的旧 consumer，并用 KeepAlive 作为尾部屏障。probe 必须要求一个合法 option 值、已知 bit、最多两个 tail 包，且 RemoveBuff 只能紧跟一个 restore AddBuff。
- Verification: 旧 observer transcript 已显式消费并断言 private final GameMaster AddBuff；probe 新增 missing/unknown options、third AddBuff、standalone removal 与 add-after-removal 拒绝测试。GameMaster/required-group/observer focused 普通 `-count=10`、四包 focused race `-count=3`、服务端整包、全仓普通重跑、完整 race、vet、build、probe vectors、gofmt 与 diff check 均退出 0；首次全仓普通仅命中既有 `TestSessionHallucinationTranscript` 30 秒 pipe flake，隔离 `-count=10` 与后续服务端/全仓重跑均退出 0，未修改该无关模块。
- Strengthening after interactive `@HAIR`: 新 authenticated 管理员夹具再次在 `GuildBuffList` 后直接发送命令，导致合法 final `AddBuff(GameMaster)` 抢占静默命令的 KeepAlive 屏障；显式消费并断言该 tail 后定向普通/race 通过。所有复用 mail bootstrap helper 且 seed `AdminAccount` 的测试，仍必须把 helper 返回点视为条件包中间态而非 game-loop barrier。

### 2026-08-23 — 交互式 GameMaster mode 必须分离权限、瞬态状态与 Buff authority

- Symptom: Legacy `@GAMEMASTER` 同时允许管理员和 TestServer 普通账户，但只有管理员的 `UpdateGMBuff` 会生成/持久化 AddBuff；新 authenticated TestServer fixture 首次又因沿用 `AllowStartGame=false` 默认值而收到合法的 StartGame result 0，未到达目标命令。
- Root cause: 容易把全局 TestServer capability、runtime `GMGameMaster` 目标免疫、账户 `IsGM` authority 和 GameMaster durable Buff 合并；测试还把“TestServer 可执行命令”误推成“普通账户自动绕过 AllowStartGame”。
- Prevention: 命令切片显式列四层：`IsGM || TestServer` permission、先发 localized mode Hint、所有合法调用都切瞬态 target gate、仅 `IsGM` 才更新 option-preserving AddBuff/可见广播/persistence。普通 TestServer session fixture 必须单独开启 `AllowStartGame`，不得扩大生产 start gate 修测试。
- Verification: world tests 覆盖未授权、TestServer transient-only、管理员 Observer/Superman bit 保留和 `Hint -> owner -> nearby -> persist`；authenticated admin/TestServer transcripts 覆盖 enable/disable、无额外 tail、logout/relogin restore/final reset；修正 fixture 后 focused 普通 `-count=10`、race `-count=3`、服务端/全仓普通、完整 race、vet、build、probe vectors、gofmt 和 diff check 全部退出 0。

### 2026-08-23 — 交互式 Superman 必须统一 HumanObject vital 边界并保留怪癖

- Symptom: `@SUPERMAN` 的 Hint/Buff 测试已能通过，但 Go 仍有多条玩家 HP/MP 生产路径直接赋值；初次共享 helper 又只实现上下限和 GMNeverDie refill，遗漏了 `ChangeHP` 在负值时先走 Protection→`ChangeMP` 的完整重定向。只读 review 还把零 MP Plague 发包、治疗显示请求量和 `int32` 回绕按常规工程直觉标成缺陷。
- Root cause: Legacy 的 `GMNeverDie` 不是单一 combat gate，而位于 `HumanObject.ChangeHP`、`ChangeMP`、`SetHP` 的共享状态提交边界；Protection 又先于 GMNeverDie。与此同时，把“更合理的客户端显示/数值安全”误当成行为等价，会抹掉原版明确存在的通知和 unchecked 算术怪癖。
- Prevention: 交互命令必须分离 `IsGM || TestServer` permission、runtime-only Superman flag、localized Hint、管理员 GameMaster option projection 与持久化。所有玩家 vital 写入口按 `ChangeHP`/`ChangeMP`/`SetHP`/直接 Die 分类；共享 `ChangeHP` 自身负责 Protection redirect，damage resolver 只决定死亡与通知。Review finding 必须回读 Legacy 入口和项目编译设置后裁决。
- Verification: 两个 `luna_worker` 分别完成 Legacy 调用链和 Go 34 文件只读审查；主审确认 `Hint -> owner AddBuff -> nearby AddBuff -> persist`、TestServer transient-only、stored Buff restore 后 runtime final reset，以及 Plague/regen/unchecked 怪癖。world 与 authenticated `net.Pipe` tests 覆盖权限、option 保留、Protection、HP/MP refill、lethal monster/poison、logout/relogin；focused 普通 `-count=10`、race `-count=3`、服务端整包、全仓普通、完整 race 重跑、vet、build 与 probe vectors 均退出 0。首次全仓普通命中既有 YinDevilNode 通知 flake，首次完整 race 命中既有 OmaMage 随机边界；隔离/后续完整重跑通过，未修改无关模块。

### 2026-08-23 — Observer 必须按属性 setter 顺序并从三项 runtime mode 重建 GM options

- Symptom: 未验证补丁先发送 owner Hint、再广播 `ObjectRemove`/`ObjectPlayer`，并把 Observer 加进通用 player/monster target gate；主审改为从 runtime mode 重建 GameMaster options 后，既有 GameMaster/Superman 测试又因只 seed durable Buff 的 Observer/Superman bits 而失败（期望 7，实际分别为 1/5）。
- Root cause: 只读取 `Chat("@OBSERVER")` 的命令正文而没有展开 `MapObject.Observer` setter，误掉了 setter 在 Hint 前同步执行的可见性副作用；同时把“不可见且不阻挡”推成“不可攻击”。旧测试在 Go 尚无 Observer runtime 字段时还把持久 `Buff.Values` 当作 live flag 替身，掩盖了 Legacy `UpdateGMBuff` 每次只从 `GMGameMaster`、`GMNeverDie`、`Observer` 三项运行时字段重建 options 的边界。
- Prevention: 管理模式命令必须分别列出 permission、属性 setter 副作用、Hint、runtime flags、Buff projection 与持久化顺序；Observer 只影响 `GetInfo`/`BroadcastInfo` 和 `Blocking`，不得加入通用 `IsAttackTarget`。所有 option-preservation fixture 必须同时 seed 对应 live flag 与 durable projection，禁止从旧 `Buff.Values` 猜当前 runtime mode。
- Verification: world tests 锁定 `ObjectRemove/ObjectPlayer -> Hint -> owner AddBuff -> visible nearby AddBuff -> persist`、非阻挡但仍可作为 player/monster target、三项 runtime option composition 和后续 ObjectPlayer 抑制；authenticated `net.Pipe` 锁定管理员 enable/disable、TestServer-only 拒绝、join-time invisibility、logout persistence 与 relogin restore/final reset。修正 fixture 后 focused 普通/race、服务端整包、全仓普通/race、vet、build 和 probe vectors 均退出 0。

### 2026-08-23 — .NET `byte.TryParse` 命令参数不能用 Go `ParseUint` 近似

- Symptom: `@HAIR` 的普通数值、负数、越界和非法字符串测试均通过后，新增 `+8` 向量稳定得到 Hair=0，而 Legacy `byte.TryParse("+8", out value)` 接受默认 `NumberStyles.Integer` 的前导正号并得到 8。
- Root cause: 依据目标类型无符号就直接选择 `strconv.ParseUint`，没有对照 .NET 默认 TryParse 的 lexical grammar；两者对前导 `+` 的接受范围不同。
- Prevention: 迁移 .NET 数值 TryParse 时先列出符号、空白、范围、overflow、culture 和失败默认值；对 byte 命令 token 使用有符号解析后显式约束 `0..255`，失败保留 out 参数默认 0，而不是按 Go 目标类型猜 parser。
- Verification: `hair_command.go` 改用 `ParseInt(..., 16)` 加 `0..255` 门禁；domain tests 锁定 `+8`、255、负数、256 和非法值，管理员、TestServer 与未授权 authenticated session 锁定权限、静默包序及 live/auth 状态；focused 普通 `-count=10` 与 race `-count=3` 退出 0。
- Strengthening after read-only review: TestServer transcript 初稿以初始 Hair=0 发送非法值并仍断言 0，未授权 no-op 也能通过，无法证明配置权限分支真实执行；改用有效非零值 7，并让独立未授权 session 保持原值。管理员 JSON relogin 还必须消费 stored restore 与 final projection 两个 AddBuff，再以 KeepAlive 锁定没有残余 bootstrap tail。

### 2026-08-23 — 交互式 `@ALLOWTRADE` transcript 必须隔离 ticker、冷却和默认值伪证据

- Symptom: 未提交 session test 用真实 world ticker、source 端裸 `ReadFrame` 和无界 done 等待；最终 relogin 断言的 AllowTrade=false 又与角色默认值相同。主审补上 disabled gate 后，首次 focused 测试退出 1：第二次 `TradeRequest` 在 1ms `TradeDelay` 内被合法静默丢弃，测试等待 System chat 超时，而失败清理期间才出现临时目录已删除后的 SaveJSON 噪声。
- Root cause: 把“不常触发的全局 ticker”“很短的真实冷却”和“最终字段等于默认值”当成稳定测试环境；同时没有区分首个 assertion timeout 与 defer/临时目录清理后的次生日志。裸 socket/done 等待还会把缺包回归拖到全局测试超时。
- Prevention: 精确 `net.Pipe` transcript 在 bootstrap 后停止并等待 world ticker；双方都启动 reader，所有预期包和 session shutdown 都使用有界 channel。非冷却测试将 `TradeDelay` 显式设为零，而冷却语义由独立测试负责。持久化/relogin 必须以非默认正值收口，并在同一会话分别证明 enabled request、disabled rejection 和最终 re-enabled restore；失败归因以首个测试行和退出码为准，忽略 teardown 后的次生路径日志。
- Verification: transcript 现以 private System chat + KeepAlive barrier 锁定大小写/额外参数，验证 enabled invitation、refusal、disabled trade gate、live/auth/JSON 三层状态，并以最终 AllowTrade=true logout/reload/relogin 排除默认值伪通过；source/target/relogin reader、ticker stop 与 5s shutdown barrier 消除裸等待。最小两包编译、focused 首次修正重跑、普通 `-count=10`、race `-count=3` 及全部 Trade 普通 `-count=5`/race `-count=3` 均退出 0。

### 2026-08-24 — P2 scope-freeze 不能把局部 helper 门禁或未测生命周期写成 Complete

- Symptom: P2 有限清单把 storage-password hash/protocol 与 wrong-stage connection lifetime 合并为 Complete，并只按 `CanAccessStorageNpc` 的 key/object/map/range 列门禁；独立 review 发现真实 `PlayerObject.CallNPC` 还要求普通 NPC 的 range、visibility 与 script page authorization。清单还把 global sections 写成双向 export，并未明确 P3 tombstone mutation 与 P2 Login/Logout SelectInfo filtering 的 ownership 分界。
- Root cause: 从末端 helper 和 schema 字段反推完整生产入口，把“实现存在”误当成“生产 transcript 已证明”，且没有逐项拆开 parse/merge、writer、restart 与跨阶段 mutation/projection authority。
- Prevention: phase closure 的每个 Complete 行必须映射到实际测试；未测 wrong-stage/connection lifetime 必须留在 Ready。门禁从客户端 packet 追到 page producer、visibility、对象/range 和最终 consumer；跨阶段字段明确一个 mutation owner 和一个 projection consumer。Import parse/merge、account-only writer、global re-export 与多 store recovery 分开登记，禁止用 round-trip 一词掩盖空 section writer。
- Verification: final review `01a031c0-18ea-71e3-ba9f-b6cf96be57d4` 精确指出 storage visibility/page、StartGameBanned、delete projection、wrong-stage lifetime 和 global writer 五项；P2 candidate 已把普通 NPC authorization 路由到 `NPC-P7-ACCESS-GATE-001`，将 all-handler wrong-stage transcript 留在 Ready，明确 P3 ban/delete mutation 与 P2 Login/Logout projection，并把 global re-export/recovery留给 P12。Review 只读、未运行测试、两仓无 C# 变化。

### 2026-08-24 — duplicate-account takeover 必须分别证明 production wiring 与 claim identity

- Symptom: 初版 P2 双会话测试直接向两个 handler 注入共享 authority，并在旧会话 cleanup 后只用新连接 KeepAlive；它能证明物理连接存活，却不能证明 `serveListener` 为所有 accepted sessions 共享同一 authority，也不能发现旧 release 误删新 claim。
- Root cause: 把依赖注入 helper 当成 production wiring evidence，并把 socket 存活等同于 account-authority 仍登记。
- Prevention: listener-owned 跨会话状态需要一条真实 listener/accept 测试；claim replacement 还必须等待旧 cleanup 后发起第三次同账户登录，断言第二会话收到 reason 1、第三会话成功并保持可用。release 必须按 claim identity 比较，禁止只按 account key 删除。
- Verification: 新 production-listener transcript 锁定两次登录的 reason-1 takeover；三会话 `net.Pipe` transcript 在旧 cleanup 后再次替换第二 claim，定向普通 `-count=5` 退出 0。
- Strengthening after read-only review `01a031f8-b7cb-7c70-b729-bdb23b11b362`: 初版 production transcript 未逐项覆盖 NewAccount 0..8 与 ChangePassword 0..6，ChangePassword 的严格 ban-expiry 边界也只有静态代码；现已增加完整 TCP result-code 表和确定性 `Expiry > now`/equality service test。Reviewer 指出的 version-117 retained-gap counter 仍明确属于已登记 `PERSIST-P12-ACCOUNT-ID-001`，不得在本 P2 leaf 中过度声明。

### 2026-08-24 — P3 closure 必须从配置路由和 operator handler 两端枚举管理员 authority

- Symptom: 首轮 P3 Legacy 只读清单覆盖已登记的 ban/password/storage reset/removal，却遗漏 `Settings.GMPassword -> PlayerObject.Chat(@LOGIN)` 的瞬态提权，以及 AccountInfoForm 的空账户创建、元数据编辑、AdminAccount/RequirePasswordChange 切换。
- Root cause: 把 cross-phase finding 中列出的操作示例当成完整入口集，只沿已知 operator-account button 追踪，没有反向检查 P1 已路由到 P3 的配置键，也没有机械枚举 AccountInfoForm 的全部事件处理器。
- Prevention: scope-freeze 遇到“remaining administrator capabilities”时固定做双向闭包：从 matrix/config 路由符号追全部消费者；同时枚举 operator UI/control 的 handler 声明并逐个裁决可观察效果、动态会话影响和 phase owner。已有 runtime-admin 测试只能证明预置 authority 的消费者，不能替代 grant/revoke/GMPassword 生产入口。
- Verification: 主线程分别零退出定位 `GMPassword`、`GMLogin`、`@LOGIN`、AdminAccount assignments 与 AccountInfoForm handlers；候选 P3 registry 增加独立 `ADMIN-P3-AUTHORITY-001`，并扩展 `ADMIN-P3-ACCOUNT-OPS-001` 到 create/edit/toggle/ban/reset/delete/wipe，Go-only ledger 复核当前只有 fixture 注入、没有可达 operator/GM-login API。
- Strengthening after denominator review `01a0327e-55a1-7f63-9fb7-0b8bdcc061af`: 首版 registry 让 wire boundary、StartGame bootstrap 和 Start/Logout 三项重复拥有 result/location/runtime-GM 语义，并把 P12 完整 recovery 写成 operator leaf 前置依赖，形成 P3↔P12 伪循环。现已把 Complete 项严格限为 codec/dispatch 与 admission 后 payload，把 admission/result/location/logout 全归一个 Ready child；operator leaf 直接验 JSON/117，P12 只作为后续 multi-store 消费者。Reviewer 复读后以 11=4 Complete+7 Ready 接受，无重叠或循环。
- Strengthening after `CHAR-P3-CREATE-001` tracing: “JSON/117 reload” 初稿没有区分普通 retained-record 推导与 header gap。Go `legacyworld.parseAccountDatabase` 明确跳过 `NextCharacterID`，auth import 按最大 retained index 加一，117 writer也按 retained maximum 重建；因此高于所有记录的 Legacy header 会丢失。P3 只负责安装权威 counter 后的创建与 JSON 自有 counter，新增 `PERSIST-P12-CHARACTER-ID-001` 负责 117 header import/checkpoint/re-export/restart，禁止为满足本 leaf 越权改 importer/writer。
- Strengthening after creation review `01a032c8-61dd-7901-9e68-0879b2839245`: writer 沿用 Go helper 的 `Level=1` 并让完整 SelectInfo 测试自证该值；Legacy `CharacterInfo(NewCharacter)` 未赋 Level，`ToSelectInfo` 和 SelectScene 直接暴露零值。独立 reviewer 精确定位后已移除生产创建的 Level 赋值并把 wire/auth 期望改为 0。主线程一度又把 `if (++count >= 4)` 误读为第三个 existing 即拒绝；逐次代入与 reviewer 复核确认三 existing 可创建第四个，Go `active >= 4` 正确保留，未按错误直觉修改。
- Strengthening after disabled-name casing review: `LoadDisabledChars` 与请求检查分别调用 process-current-culture `ToUpper()`，所以土耳其文化下文件 `INDIGO` 与请求 `indigo` 并不相等；两端都用 invariant upper 会产生伪匹配。生产 auth 现在接收 `cfg.CurrentCulture.ToUpper` authority，并以 tr-TR production session 锁定该不对称，同时保留管理员短路。
- Strengthening after focused/full tests: auth 交叉账户重复名用例仍保留同名 DisabledChars，实际按 Legacy 先返回 result 1 而非 duplicate result 5；清空独立 gate 后重复名证据通过。将 production NewCharacter 修为 Level 0 后，通用 `CreateCharacter` 测试夹具也变成 0，三个既有战斗/Hero transcript 与跨 boundary StartGame 测试随之失败；生产入口和 test/bootstrap helper 必须分开：前者返回 Legacy constructor zero，后者保留历史 Level 1，跨 boundary 测试显式 seed 后续 PlayerObject level authority。三个归因测试 `-count=10`、创建 focused/repeated/race 均通过。
- Integration attribution: 首次全仓在上述真实回归修复前失败于 safe-zone/ElectricShock/Hero special-item，修复后下一次只复现既有 `TestSessionYinDevilNodeTranscript/42` 空通知 flake；该项独立 `-count=20` 通过，随后 fresh unexcluded `go test ./... -count=1 -timeout=20m` 全部通过，不能把前两次失败描述为 full pass。

### 2026-08-24 — Account operator surface 必须按 auth/live/persistence 三层拆分

- Symptom: P3 registry 只写“new bounded Go operator control”，容易让新 `internal/operator` 直接改 detached snapshot，既无法安全访问 auth 私有 map/crypto，也会用 `ReplaceAccounts` 重置 Hero/rental/guild 派生状态；Legacy 还允许多个空 AccountID、live ban reason-6 断线、storage unlock reset，以及删除/擦除不清理关联 authority 的怪癖。
- Root cause: 把 WinForms handler 当成普通 CRUD，没有区分 AccountList 对象身份、auth lock/盐哈希、当前 world session、副作用通知和 form-close persistence 时机。
- Prevention: auth 包拥有 targeted mutation 与空账户内部 key；transport-neutral operator 只编排 typed request/result；main 包拥有 live disconnect/storage notification、running gate 和 persistence callback；117 bridge 只复用既有 writer/loader。禁止用 whole-map replacement、duplicate-login claim 或新增客户端/HTTP route代替 operator semantics。
- Verification: Legacy read-only auditor `01a032f0-9125-75f0-b5cb-7c18fb9eafc9` 冻结 11 个 handler family；Go auditor `01a032f0-c025-71a0-b3a9-12039ca68c16` 确认无现有 surface，并裁决 `internal/auth` + thin `internal/operator` + bounded main wiring 的三层边界。两 agent 均未写文件或运行测试，双仓 C# 门禁为空。
- Strengthening after direct 117 wipe test design: Legacy `AccountList` 是 insertion-order list，`LoadAccounts` 只无条件保留物理首账户；Go 若继续让 `AccountsSnapshot` 按 map key 排序，wipe 后 117 reload 会保留字典序首账户而非 Legacy 首账户。Auth 必须维护独立内部 account order，rename 保持槽位、remove 删除槽位、create/load/import 依次追加，JSON 仍可保持既有 deterministic 编码，而 117 snapshot/operator refresh 使用权威顺序；测试必须故意让 insertion order 与 lexical order 相反。
- Strengthening after read-only review `01a0332a-7e79-7161-988c-127f7fda5def`: 独立审查发现 live AccountID rename 只改 auth key、storage reset 同步写 socket、effect 可能命中替换后的错误 claim、blank salts 为 nil、117 把零账户时间改成 checkpoint time，以及 cleanup 后可能重插 write lock。候选现以 auth-lock 内 session rekey + lock 外 world completion、world-player-owned immutable claim、异步有序 effect queue/五秒写门禁、24-byte 零 salts、原样账户时间和 claim-before-lock cleanup 修复，并用 live rename→创建→reason-1 takeover、同步 controller return、JSON/117 登录/hash/order/sentinel 与 focused race 锁定。`DateTime.MaxValue` 另按 unspecified-kind 原始 ticks 编码；wipe 后 stale global Hero 的 117 re-export 明确路由 P12，不在本 leaf 伪修复。
- Strengthening after final review `01a03392-8cbd-7050-9fb2-ba06d3a3b28a`: AccountID rename 不能只更新 auth/world；Go auction 的 seller/buyer 字符串是 Legacy `CharacterInfo` 对象引用的持久投影，必须按角色 index 同步。写 deadline 必须从 FIFO 排队前开始并在取得 turn 后重装同一绝对值；否则 takeover/operator effect 可永远等在早先 write 后。多个 `{Index:0, ID:""}` 行必须携带对象级 opaque ref，纯 index+ID 歧义时拒绝。主审另发现 remove 后 Legacy `CharacterList` 仍保留旧对象及其会话后续创建，Go 因此维护 process-lifetime character-account authority，防止重用 AccountID 后旧会话写入新账户或重复名字漏检。对应 auction/ref/global-name/queue-timeout tests、focused `-count=10`/race `-count=3`、fresh full/full-race/vet/build 全通过；同一 reviewer 复核后三项无残余 P0-P2 finding。

### 2026-08-24 — Admin authority 必须分离共享 flag、PlayerObject 快照与物化排名

- Symptom: 首版 `ADMIN-P3-AUTHORITY-001` 已实现 GMPassword/@LOGIN，却用当前 `AdminAccount` 动态重建排名与命令权限；这会让 operator grant/revoke 立刻改写已在线 PlayerObject，并让 password-promoted `IsGM` 仍无法执行管理命令。首版排名还在 Buff 前随 `IsGM` 过滤，早于 Legacy 的 `Buff -> Server log -> System -> RemoveRank`，且测试误锁 `MyRank=0`。
- Root cause: 把持久 AccountInfo flag、构造时 PlayerObject.IsGM 快照、已经物化的 RankTop/RankClass 和瞬态 password promotion 合并为一个动态查询；同时用复制出的 `worldPlayer` 外壳读取共享 Buff slice，focused race 捕获 backing-array 并发读写。
- Prevention: 配置按 Legacy source order 加载；PlayerObject 构造边界重新读取 AdminAccount，之后命令只查 world `IsGM`；auth 在 JSON/117/ReplaceAccounts 时冻结 admin 排名排除，operator patch 不改它，revoke 后仅在非 GM StartGame 重新加入所选角色；password promotion 先更新 Buff/持久化、再 Server/System，最后单独标记 overall removal，保留 class stale handoff 给 `RANK-P3-CHAR-LIFECYCLE-001`。测试读取 nested runtime slice 必须使用锁内 deep snapshot。
- Verification: Legacy auditor `01a033ad-ba3d-7d11-a168-16646911ff4b` 和 Go auditor `01a033ad-e287-7d13-b01a-bfcdb4e867d4` 冻结入口与架构；reviewer `01a033ca-f85f-76a3-8576-5de4c62a8605` 连续发现 rank materialization、construction race、MyRank 越界断言与 runtime-command gate，三轮修复后最终回报 `no findings`。Config/auth/server focused `-count=10`、focused race `-count=3`、fresh unexcluded full tests/full race/vet/build 均 exit 0；首次新增 operator focused race 的唯一失败栈进入测试对共享 Buff slice 的无锁读取，改用 `playerCharacterSnapshot` 后重跑通过。
- Strengthening during durable recovery: 不能把未提交 lesson 中的 `no findings` 当作已核验候选。主线程重新读取 `Envir.RemoveRank`/`GetRanking` 后发现候选仍把被移除 GM 的 overall `MyRank` 算成 0；Legacy 删除 `RankTop` 行但不清空该 `CharacterInfo.Rank[0]`。生产现于 post-System removal 冻结旧的一基 rank，overall 列表排除 GM 但向本人返回 stale `MyRank`，class 列表继续保留；首个修复测试仍把 requester index 写成 0，`-count=10` 如实失败并在改用真实 GM index 后通过。最终 focused 普通 `-count=10`、race `-count=3`、fresh unexcluded full tests/full race/vet/build 均 exit 0。
- Strengthening after read-only review `01a033fa-8785-7863-a391-d3426f306e58`: reviewer 继续发现 game-stage `@OBSERVE` 在 runtime `IsGM` 后又查持久 AdminAccount、world.leave 会过早恢复 RankTop、broken class pass 应把本人 `MyRank` 清零，以及缺少真实 accept-loop 入口。生产现由 caller 区分 game runtime authority 与 observer-stage persisted authority；auth 保存 process-local overall removal/stale rank 到下次非 GM StartGame，class listing 保留但 `MyRank=0`；实际 TCP `serveListener` 锁定配置密码。新增 password-promoted observe、logout gap/relogin restore、overall/class MyRank 与 accepted-session tests；focused `-count=10`/race `-count=3`、fresh full/full-race/vet/build 全通过，同一 reviewer 复读后回报 `no findings`。

### 2026-08-24 — Character delete 必须分离内存 mutation、回包与周期持久化

- Symptom: `CHAR-P3-BAN-DELETE-001` 候选保留了旧 Go 的 success 后即时 `SaveJSON`，只读 reviewer 指出 Legacy handler 仅 tombstone→RemoveRank→success，保存属于独立周期/关服流程；新增 no-immediate-save 测试首跑还因 `DeferredSaveHero` 超过真实角色名上限而先返回创建 result 1。
- Root cause: 把 Go 现有即时导出当成删除事务的一部分，并为持久化场景凭语义命名夹具而未先过真实创建门禁。
- Prevention: mutation leaf 逐层列出锁内状态、rank 副作用、网络顺序与持久化 authority；Legacy 未在 handler 内保存时不得新增同步落盘承诺。所有生产 session 夹具先用真实创建 API验证名称长度/字符集，再进入目标分支。
- Verification: 已移除删除 handler 的即时 `SaveJSON`，生产测试在 success 后确认内存 tombstone 已存在且 export 文件仍不存在；夹具改为合法 `DeferredHero` 后定向测试退出 0。JSON 显式 save/reload、普通 unique-index 117 round-trip 与 corrupt-index 的 P12 re-export 边界仍分开验收。
