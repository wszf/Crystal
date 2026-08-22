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

### 2026-08-23 — BaseStats 配置必须分离序列化字段、计算短路与 runtime authority

- Symptom: 自定义 BaseStats 首次定向测试把 `Gain == 0` 的记录误期望为同时清零 `GainRate/Max`，并把 Wizard Mana 公式在 level 10 的结果误算成 112 而非 129；恢复时还发现未提交实现只加载配置，却遗漏 `serveWithConfig`、player bootstrap packet 与 `heroCharacter` profile 传播。
- Root cause: 把 `BaseStat.Calculate` 的 `Gain == 0` 计算短路误当成 INI/wire 字段规范化，并把通用 Mana 公式套到 Wizard 特例；同时只审查了 loader/公式 helper，没有沿 `Settings -> HumanObject/HeroObject.RefreshLevelStats -> SendBaseStats` 的完整 authority 链检查所有消费者。
- Prevention: 自定义公式测试必须分别断言原始 profile/wire 字段和计算结果；逐项代入 Health/Mana/Weight/Stat 及 Warrior/Wizard/Taoist 分支；配置 profile 作为 runtime-only authority，在 player enter、Hero summon、equipment calculation 和对应 BaseStats packet 四个边界显式传播，并用 `json:"-"` 锁定不落盘。
- Verification: 修正两项测试期望后，五职业默认、custom INI 顺序/数字零 fallback、四公式/Max/Gain==0、player runtime/cap/authenticated bootstrap、Hero runtime/packet/authenticated summon 和 transient JSON 测试普通 `-count=10`、race `-count=3` 均通过；全仓普通重跑、完整 race、vet 和 build 退出 0。
