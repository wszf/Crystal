# Crystal Go 迁移 Session 交接

最后更新：2026-08-19（Asia/Singapore）

## 迁移目标与硬边界

目标是把当前 Crystal 项目的服务端功能与客户端可观察行为 100% 迁移到独立、跨平台的 Go 项目。服务端、测试客户端、协议探针、Legacy 数据导入/导出器和其他迁移工具全部使用 Go。

- 原 Crystal 仓库和 Go 迁移仓库中的所有 `.cs` 文件都是只读对照基线，禁止新增、修改、重命名或删除。
- 每个功能批次必须同时覆盖领域状态、协议序列、持久化/重载和真实会话路径；仅存在 handler 不算迁移完成。
- 可以共享运行时和测试矩阵的功能应成组迁移，但不能牺牲行为等价、竞态安全和回归覆盖。
- 每批完成后更新迁移矩阵、运行质量门禁并 Git 提交；整体迁移未达到 100% 前不得标记最终 Goal 完成。

## 仓库快照

| 仓库 | 路径 | 分支 | 当前基线 | 交接前状态 |
|---|---|---|---|---|
| Legacy Crystal | `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal` | `master` | `6491b0cb docs: record Tao Guard migration`，随后增加本次交接更新 | 干净 |
| Go migration | `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer` | `main` | `03dad46 feat(p11): migrate harvest lifecycle` | 干净 |

新 Session 应以实际 `git status --short --branch` 和 `git log -1 --oneline` 为准。本文件提交后，Legacy 仓库 HEAD 会比表中的交接前基线多一个文档提交。

迁移状态的权威明细位于 Go 仓库的 `docs/migration-matrix.md`；`README.md` 是当前实现能力的长说明。

## 当前 Goal 状态

本线程当前已恢复以下完整 Goal，状态为 active；不要因为某个批次完成而把
整体 100% 迁移误标为完成。后续 Session 继续沿用该 Goal：

> 将当前 Crystal 项目按现有功能与客户端可观察行为 100% 迁移到独立 Git 管理的跨平台 Go 项目：服务端、测试客户端、协议探针、导入导出及迁移工具全部使用 Go；两个仓库中的所有 C# 文件仅作只读基线；按可安全合并的功能批次实现、测试、更新迁移矩阵并 Git 提交，持续推进直至功能矩阵全部完成。

## 阶段进度

迁移矩阵共有 P0–P12 十三个阶段。目前是 2 个 Complete、10 个 In progress、1 个 Pending。该计数只表示阶段状态，不等于准确的代码或功能完成百分比。

| 阶段 | 状态 | 当前摘要 |
|---|---|---|
| P0 协议帧/连接基础 | Complete | 帧编码、版本、断开、KeepAlive 和 Go 固定向量已完成。 |
| P1 配置与生命周期 | In progress | INI/环境配置、版本哈希、连接限制、超时、包速率和优雅关闭已有覆盖；完整重启/日志生命周期待闭环。 |
| P2 账户与密码 | In progress | 登录、账户创建、改密、StoragePassword、导入账户和 SelectInfo 已迁移；直接二进制写回及完整 NPC 访问仍待完成。 |
| P3 角色与 StartGame | In progress | 角色列表/创建/删除、运行时字段、有效/无效出生点、基础属性与登出持久化已有覆盖，尚未按完整客户端启动流程宣告完成。 |
| P4 地图/移动/可见性 | In progress | 多版本地图、碰撞/门、玩家/NPC/怪物可见性、地图切换、普通/私聊及聊天物品链接授权展开和多项地图门禁已迁移；完整 bootstrap 仍待完成。 |
| P5 战斗/技能/怪物/掉落 | In progress | 已完成核心近战、远程、PvP 基础、多个单体/区域魔法、九个自增益 Buff、FrostCrunch 状态、基础属性、掉落树和持久化；最近批次新增玩家 TrapHexagon、SummonVampire/SummonToad/SummonSnakes、CannibalPlant、Guard 和 Tao Guard 的 Legacy admission、目标捕获、延迟/生命周期、AI/伤害、静态/Observer 可见性和 net.Pipe transcript；剩余技能/Buff、飞行/墙体规则、高级 PvP/组队战斗、通用怪物 AI、持久重生状态及完整包序仍待迁移。 |
| P6 物品/装备/维修/强化/制作 | In progress | 背包、装备、Storage、Trade、Repair、Refine、Craft 和基础 Use/Delete/Drop/Pickup 已有完整功能簇；尚未对整个 P6 做 100% 等价收口。 |
| P7 NPC/商店/任务/脚本 | In progress | NPC 可见性、传送、核心脚本动作/控制流、商店/BuyBack、Quest 生命周期及回调已迁移；剩余脚本/商店动作和完整包序待完成。 |
| P8 Group/Hero/Pet/Mount/Social | Complete | Group、Hero、普通战斗宠物、Mount、好友/黑名单、婚姻和导师体系已完成并有领域、协议、会话及持久化证据。 |
| P9 Guild/War/Territory/Conquest | In progress | 公会核心、仓库、进度/Buff、战争、领地和完整 Conquest runtime/NPC/assets 子簇已迁移；P9 其余范围尚未统一收口。 |
| P10 Mail/Market/Auction/Rental/GameShop | In progress | 五个主要经济功能簇均已有协议、事务、在线通知、持久化和竞态测试，但阶段仍未宣告完整。 |
| P11 Fishing/Awakening/Ranking/Intelligent Creature/Misc | In progress | Fishing、Awakening、Ranking、Intelligent Creature 功能簇已迁移；聊天物品链接已按库存/解锁 Storage/已召唤 HeroInventory 授权并展开定义；`RequestUserName`/`UserName` 已补齐全局角色名查询、缺失静默和协议探针；共享 HarvestMonster carcass 生命周期已迁移；其他 miscellaneous 系统待补。 |
| P12 恢复/备份/部署 | Pending | 最终 restart equivalence、生产化 smoke test、备份与部署尚未开始。 |

## 已交付的重要能力

- 纯 Go 迁移工具：`crystal-legacy-account-export`、`crystal-legacy-world-export`、`crystal-protocol-probe`。保留的 C# 工具仅作历史基线。
- Go 服务端已贯通账户/角色、地图与可见性、NPC/Quest、物品与经济、Group/Hero/Pet/Mount/Social、Guild/Conquest、Mail/Market/Auction/Rental/GameShop、Fishing/Awakening/Ranking/Intelligent Creature 等大量功能簇。
- 协议测试使用 Legacy ordinal、精确 payload 和固定向量；关键跨玩家功能使用真实 `net.Pipe` 多会话接收者矩阵。
- `RequestUserName`/`UserName` 已按 Legacy 的 UInt32 角色索引、全局离线角色查询、缺失无响应语义和 .NET 字符串布局接入服务端，并由协议向量、认证探针和 Game 阶段会话测试锁定。
- Legacy 数据可通过纯 Go 账户/世界导出器进入 Go JSON 权威状态，并覆盖重登与 restart 恢复路径。

关键里程碑提交：

- `9084e6c feat(p5): migrate immediate self-buff runtime`
- `f0c768c feat(p5): migrate self-buff combat runtime`
- `c9f8264 feat(p5): migrate FrostCrunch poison states`
- `4e4d811 feat(p5): migrate delayed player area magic`
- `0758293 feat(p9): migrate conquest boundary effects and archer AI`
- `1b055eb feat(p9): migrate conquest runtime`
- `16c8a8a feat(p8): complete group compatibility`
- `9b5bbed feat(p8): migrate heroes`
- `0cf4a44 feat(p8): migrate mounts and ordinary pets`
- `03f9b2b feat(p11): migrate intelligent creatures`
- `0e17233 feat(p11): migrate fishing awakening and rankings`
- `225e855 feat(p11): migrate user name lookup`
- `d7bc163 feat(p5): migrate TrapHexagon spell`
- `1126c84 feat(p5): migrate archer summon spells`
- `213b92e feat(p5): migrate CaveMaggot AI`
- `b747d76 feat(p5): migrate CannibalPlant AI`
- `ff39426 feat(migration): add Go legacy account exporter`
- `cb4b899 feat(migration): add Go legacy world exporter`
- `296f1c1 feat(migration): add Go protocol probe`
- `03dad46 feat(p11): migrate harvest lifecycle`

## 最近完成的 P5 批次

提交 `9084e6c` 完成了可一起验收的三个即时自增益技能：

- `ProtectionField`：Buff 10，立即应用，持续 `45 + 15*level` 秒，Min/Max AC 使用 midpoint-to-even 取整。
- `Rage`：Buff 11，立即应用，持续 `18 + 6*level` 秒，Min/Max DC 使用 midpoint-to-even 取整。
- `SwiftFeet`：Buff 4、visible，立即应用，持续 `25 + 5*level` 秒，冷却 `210 - 40*level` 秒；激活期间 Run 三格，到期恢复两格。
- 已固定 owner 的 `HealthChanged -> UserLocation -> AddBuff -> Magic` 顺序，以及 observer 的 visible/private 接收者差异。
- 已保留 Legacy 登录怪癖：SwiftFeet Buff/可见快照恢复，但服务端三格能力不会在登录时恢复，必须重新施法。
- 生产实现、协议 ordinal、领域公式/到期/移动、双会话包序、JSON 持久化/重登和迁移文档均已提交。

不要重做这一批，除非新测试发现真实回归。

## 本次完成的 P5 批次

本批实现玩家 `Spell.TrapHexagon`：

- 施法入口保留 Legacy 的两行 admission 扫描、目标锁定坐标、shape-0 Amulet 门禁与单件消耗；成功时先扣 MP、发 `DeleteItem`/`UserLocation`/`MagicLeveled`，再发 `ServerMagic`。
- 500ms 后按完整 3×3 扫描设置可攻击且等级差合法怪物的 `ShockTime`，清空 AI 目标，并按 Legacy 顺序生成八个 `ObjectSpell` 字段；0 级持续时间固定为 10 秒，精确到期 tick 仍保留，下一 tick 才移除。
- 字段接入普通静态可见性恢复/移除和 Observer 被动快照；领域测试覆盖边界、目标过滤、点位、生命周期，session 测试覆盖认证、持久护符数量、即时包序与延迟 impact transcript。
- Go 功能已提交为 `d7bc163 feat(p5): migrate TrapHexagon spell`；本交接文档随后单独提交，两个仓库的 C# 审计保持 tracked、staged、untracked 均为零。

## 本次完成的 P5 批次（ArcherSummon）

本批实现玩家 `Spell.SummonVampire`、`Spell.SummonToad`、`Spell.SummonSnakes`：

- 入口保留 Legacy 的目标/落点捕获、`CanFly`、NoPets 文本和最多两个直接宠物门禁；不消耗护符，首次延迟只练习，第二个 500ms map action 才生成宠物。
- 新增 `VampireSpider`、`SpittingToad`、`SnakeTotem`、`CharmedSnake` 配置名与 AI 运行时；覆盖召回目标定位、owner 外观/MasterObjectID、等级上限、等级缩放生命期、地图/距离/死亡/登出清理及 Totem 子蛇数量/父子生命周期。
- Vampire 保留近战吸血和死亡爆发，Toad 保留距离延迟范围伤害，CharmedSnake 保留延迟近战/麻痹；世界测试覆盖目标捕获、两段延迟、召回、NoPets/缺定义、Totem 子蛇和延迟伤害，认证 `net.Pipe` transcript 覆盖 Health/Location/Magic、MagicLeveled、ObjectMonster/ObjectHealth。
- 为避免 live tick 在下一次客户端写入前产生待发送通知而造成互等，既有 TrapHexagon `net.Pipe` keep-alive barrier 改为并发写入并持续消费服务端帧；ArcherSummon+TrapHexagon 组合重复回归通过。

Go 实现与矩阵更新已提交为 `1126c84 feat(p5): migrate archer summon spells`；本批未修改任何 C# 文件。

## 本次完成的 P5 批次（CaveMaggot AI=7）

本批实现 Legacy `CaveMaggot` 的普通玩家目标攻击子集：

- AI=7 已加入 Go common population；保持相邻目标的 `ObjectAttack`、300ms 延迟、`AttackSpeed` 冷却和 impact-time 目标校验。
- 延迟命中沿用 Legacy `DefenceType.MACAgility`：使用 MAC 而不是 AC，保留 MagicResist miss gate 与 Agility/Accuracy hit gate；命中造成实际伤害后按一次 PoisonResist、20 分之一机会施加 5 秒麻痹/1 秒 tick。
- 世界测试覆盖 AC/MAC 分离、MagicResist 阻断、延迟动作/中毒参数和包序；认证 `net.Pipe` transcript 覆盖登录后 AI 攻击、延迟伤害及 `Poisoned` 状态。

Go 实现与矩阵更新已提交为 `213b92e feat(p5): migrate CaveMaggot AI`；本批未修改任何 C# 文件。

## 本次完成的 P5 批次（CannibalPlant AI=5）

本批实现 Legacy `CannibalPlant` 的隐藏植物生命周期与 Player/owned
Monster/Hero 目标子集：

- AI=5 已加入 Go common population；初始隐藏，按 `FindNearby(3)` 每两秒检查，
  发现目标时按 `ObjectMonster -> ObjectShow` 显示，设置 500ms cell/1000ms
  action 延迟；无目标时发送 `ObjectHide`、恢复满 HP 并进入三秒隐藏延迟。
- 隐藏状态贯通 passive object、blocking、搜索、攻击及玩家单体/范围攻击门禁；
  `CanMove=false` 的不移动边界保留，普通相邻攻击使用 DC/AC-Agility、
  `ObjectAttack` 和 300ms 延迟，并在 impact 时重验目标。
- Player、owned Monster、Hero 投影及共享延迟 resolver 已覆盖；AI=153
  CreeperPlant 的五格远程分支保持不变，只共享隐藏植物门控。
- Go 世界测试覆盖可见性、FindNearby(3)、不可移动、AC/MAC 分离、目标投影和
  延迟动作；认证 `net.Pipe` transcript 锁定隐藏显示、相邻攻击、300ms 命中及
  `Struck/ObjectStruck/DamageIndicator/HealthChanged` 包序。

Go 实现与矩阵更新已提交为 `b747d76 feat(p5): migrate CannibalPlant AI`；本批未修改任何 C# 文件。

## 本次完成的 P5 批次（Guard AI=6、Tao Guard AI=58）

本批实现 Legacy `Guard`（AI=6）的可观察服务端行为，并与已迁移的
`ArcherGuard`（AI=113）共享目标侧 Guard 门禁：

- AI=6 已接入 common population；无 route 时保持静止，有 route 时保留
  Legacy 的 route-only 移动、寻路顺序、移动/动作/攻击计时和 Guard 的
  `CanMove`/`CanAttack` 边界。
- 目标搜索按 Legacy 的 ViewRange 距离环和 cell insertion order 覆盖
  Player、Monster、Hero；只接受同地图、存活、可见且 `PKPoints >= 200`
  的红名 Player，并保留 Guard 绕过 safe-zone/NoFight 的特殊门禁。
  Guard 对 Player 的单体/AOE 攻击、对 Guard 类目标的反向攻击均保持免疫。
- 攻击严格发送目标背向格的 `ObjectAttack`，随后发送 Guard 当前格的
  `ObjectTurn`；设置 500ms action、`AttackSpeed` cooldown，并在 300ms
  延迟 impact 重新验证目标。Player 使用普通 AC，Monster/Hero 使用
  Legacy 的非玩家致命伤害路径，含 IcePillar/TucsonEgg 特殊 resolver。
- 新增 SkyBlue 名称颜色、Monster/Hero/Player 领域测试和认证 `net.Pipe`
  transcript；FurbolgGuard transcript 同步隔离 live AI 与手动时钟，避免
  服务循环污染协议测试的随机流。

- AI=58 已复用 Guard 的 common population、route-only movement、ViewRange
  Player/Monster/Hero 目标和 SkyBlue 投影、红名 Player safe-zone/NoFight
  绕过及 Player 单体/AOE 免疫；保留 Legacy Tao Guard 的差异：owned
  Monster 只要主人 `AttackMode != Peace` 即可成为目标，而 AI=6/113 仍要求
  主人 `PKPoints >= 200`。普通域测试覆盖非 Peace/Peace 两侧和 AI=6 对照，
  认证 `net.Pipe` transcript 覆盖攻击包序与 300ms AC impact。

Go 实现与迁移矩阵更新已提交为 `bc2e919 feat(p5): migrate Tao Guard AI`；本批未修改任何 C# 文件。

## 本次完成的 P11 批次（HarvestMonster carcass lifecycle）

本批实现 Legacy 共享 `HarvestMonster` carcass 生命周期的客户端可观察子集：

- 固定当前线材 `ClientHarvest=49`、`ServerObjectHarvest=91`、`ServerObjectHarvested=92`，并按 Legacy 的 13 字节 `ObjectHarvest` payload 做 round-trip 与截断/尾随字节校验。
- AI `{1,2,4,5,7,9,28}` 的尸体进入 Harvest 路径；普通死亡掉落被抑制，Skeleton 状态投影贯通静态 `ObjectMonster` 与认证会话。
- 保留前方一格加一环扫描、方向/移动/动作冷却、两次剥皮后生成缓存掉落、后续 Harvest 领取、背包叠加线材快照、满包/无物品文本、QuestRequired 和 Meat 质量/耐久处理。
- 保留最近击杀者五秒经验归属及组成员门禁；未实现的独立 Deer AI=1/2 逃跑移动明确留在待迁移项，不把共享 Harvest 生命周期扩张为完整 Deer AI。
- Go 世界测试覆盖皮肤计数、掉落/背包、无掉落、叠加线材、组门禁；认证 `net.Pipe` transcript 覆盖三次 Harvest、`UserLocation`/`ObjectHarvest`/领取与持久化。

Go 实现与迁移矩阵更新已提交为 `03dad46 feat(p11): migrate harvest lifecycle`；本批未修改任何 C# 文件。

## 当前质量门禁

Go HEAD `03dad46` 对应本批源码已通过以下收尾门禁：

- `go test ./... -count=1 -timeout=600s`（本次收尾重新运行并明确取得 `exit_code=0`）
- `go test -race ./cmd/crystal-server -run 'ArcherSummon|SummonSnakes' -count=1 -timeout=5m`
- `go test -race ./cmd/crystal-server -run 'CaveMaggot' -count=1 -timeout=5m`
- `go test -race ./cmd/crystal-server -run 'CannibalPlant' -count=1 -timeout=5m`
- `go test ./internal/worlddata ./internal/legacyworld -count=1 -timeout=120s`
- `go vet ./...`
- `go build ./...`
- ArcherSummon/TrapHexagon 会话组合重复 3 次通过，含 net.Pipe keep-alive barrier 回归
- CannibalPlant/CreeperPlant 领域回归与 CannibalPlant `net.Pipe` 会话 transcript 通过
- Guard 领域/认证 session 测试与 `-race` 通过；FurbolgGuard session `-race -count=3` 通过
- Tao Guard route/目标门禁与认证 session 测试通过；Guard/Tao Guard session `-race -count=5` 通过
- Harvest 协议/世界/认证 session `-race -count=5` 通过，覆盖三次剥皮、缓存掉落、组门禁和持久化
- 两仓库 `git diff --check`
- 两仓库 tracked、staged、untracked `.cs` 零变化检查

全仓库 race 尚未作为本批门禁重跑；此前已知的共享 session Buff 读写竞争仍按
`tasks/lessons.md` 管理。本批定向 ArcherSummon race 通过。

文档交接不会修改 Go 源码。新 Session 开始时仍应先确认两个工作树干净；若环境或工具链变化，再选择与风险相称的门禁重跑。

## 建议的下一条迁移线

优先继续 P5，因为最近四个批次已经建立了稳定的魔法/Buff/延迟动作/状态生命周期基础设施：

1. 继续 P5 通用 monster AI，按 `docs/migration-matrix.md` 中尚未完成的 AI/target sub-slice 排序，逐项确认可生成入口、隐藏/移动/目标门禁和攻击 resolver。
2. 每批从一个可独立验证的 AI 行为簇开始，同时覆盖公式、冷却、延迟动作/目标重验、玩家/宠物/Hero/Monster 投影、包序和 net.Pipe transcript；不要仅凭怪物名称推断行为。
3. 对已完成的玩家法术差集继续保持机械校验；若发现真实主动入口缺口，再从 Legacy 施法入口、命中 resolver、Buff/Poison 创建点建立准确的 `spell -> effect -> side effects` 表后成批迁移。
4. 之后再处理 P5 的 fly/wall validation、高级 PvP/Group combat、persistent respawn state 和完整 packet-order closure。

如果下一 Session 决定切换阶段，应以 `docs/migration-matrix.md` 中明确写出的 pending 项为准，不要从 README 的概述自行推导缺口。

## 新 Session 启动清单

```sh
cd /Users/wszf/Dropbox/source_code/git_work/me_work/Crystal
cat AGENTS.md
cat tasks/lessons.md
cat tasks/migration-handoff.md
git status --short --branch
git log -1 --oneline

cd /Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer
git status --short --branch
git log -8 --oneline
sed -n '1,180p' docs/migration-matrix.md
go test ./cmd/crystal-server -run '^$' -count=1
```

随后在新 Session 创建上文完整 Goal，并从“建议的下一条迁移线”开始。不要修改任何 C# 文件。

## 每批提交前固定检查

Go 仓库：

先根据 `git status --short` 的完整清单，只对本批实际修改的 `.go` 文件运行 `gofmt -w`；不要把 C# 文件或猜测的 glob 传给 Go 工具。随后运行：

```sh
go test ./...
go test -race ./...
go vet ./...
go build ./...
git diff --check
git diff --name-only -- '*.cs'
git diff --cached --name-only -- '*.cs'
git ls-files --others --exclude-standard '*.cs'
git status --short
```

Legacy 仓库：

```sh
git diff --check
git diff --name-only -- '*.cs'
git diff --cached --name-only -- '*.cs'
git ls-files --others --exclude-standard '*.cs'
git status --short
```

暂存时必须以 `git status --short` 为完整清单，不能只依赖 `git diff --name-only`，因为后者不包含未跟踪的新 Go 文件。功能和文档提交后再次检查两个工作树，并记录提交哈希。
