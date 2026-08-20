# Crystal Go 迁移 Session 交接

最后更新：2026-08-20（Asia/Singapore）

## 迁移目标与硬边界

目标是把当前 Crystal 项目的服务端功能与客户端可观察行为 100% 迁移到独立、跨平台的 Go 项目。服务端、测试客户端、协议探针、Legacy 数据导入/导出器和其他迁移工具全部使用 Go。

- 原 Crystal 仓库和 Go 迁移仓库中的所有 `.cs` 文件都是只读对照基线，禁止新增、修改、重命名或删除。
- 每个功能批次必须同时覆盖领域状态、协议序列、持久化/重载和真实会话路径；仅存在 handler 不算迁移完成。
- 可以共享运行时和测试矩阵的功能应成组迁移，但不能牺牲行为等价、竞态安全和回归覆盖。
- 每批完成后更新迁移矩阵、运行质量门禁并 Git 提交；整体迁移未达到 100% 前不得标记最终 Goal 完成。

## 仓库快照

| 仓库 | 路径 | 分支 | 当前基线 | 交接前状态 |
|---|---|---|---|---|
| Legacy Crystal | `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal` | `master` | `959c7bef docs: record FrostTiger migration handoff`，本批增加 AI=36 交接更新 | 本次 handoff 与 lessons 更新随本批文档提交 |
| Go migration | `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer` | `main` | `7df022c feat(p5): complete Yimoogi AI` | AI=36 生产、测试、矩阵已提交；提交后干净 |

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
| P5 战斗/技能/怪物/掉落 | In progress | 已完成核心近战、远程、PvP 基础、多个单体/区域魔法、九个自增益 Buff、FrostCrunch 状态、基础属性、掉落树和持久化；最近批次新增玩家 TrapHexagon、SummonVampire/SummonToad/SummonSnakes、CannibalPlant、Guard、Tao Guard、Deer AI=1/2、Tree AI=3、EvilCentipede AI=14、WoomaTaurus AI=11、RedMoonEvil AI=13、Shinsu AI=18、BugBagMaggot AI=12/RootSpider AI=39/BombSpider AI=40、RightGuard/LeftGuard AI=31/32、MinotaurKing AI=33 以及 FrostTiger AI=34 的 Legacy admission、目标捕获、延迟/生命周期、AI/伤害/逃跑、静态/Observer 可见性和 net.Pipe transcript；剩余技能/Buff、飞行/墙体规则、高级 PvP/组队战斗、其他通用/特殊怪物 AI、持久重生状态及完整包序仍待迁移。 |
| P6 物品/装备/维修/强化/制作 | In progress | 背包、装备、Storage、Trade、Repair、Refine、Craft 和基础 Use/Delete/Drop/Pickup 已有完整功能簇；尚未对整个 P6 做 100% 等价收口。 |
| P7 NPC/商店/任务/脚本 | In progress | NPC 可见性、传送、核心脚本动作/控制流、商店/BuyBack、Quest 生命周期及回调已迁移；剩余脚本/商店动作和完整包序待完成。 |
| P8 Group/Hero/Pet/Mount/Social | Complete | Group、Hero、普通战斗宠物、Mount、好友/黑名单、婚姻和导师体系已完成并有领域、协议、会话及持久化证据。 |
| P9 Guild/War/Territory/Conquest | In progress | 公会核心、仓库、进度/Buff、战争、领地和完整 Conquest runtime/NPC/assets 子簇已迁移；P9 其余范围尚未统一收口。 |
| P10 Mail/Market/Auction/Rental/GameShop | In progress | 五个主要经济功能簇均已有协议、事务、在线通知、持久化和竞态测试，但阶段仍未宣告完整。 |
| P11 Fishing/Awakening/Ranking/Intelligent Creature/Misc | In progress | Fishing、Awakening、Ranking、Intelligent Creature 功能簇已迁移；聊天物品链接已按库存/解锁 Storage/已召唤 HeroInventory 授权并展开定义；`RequestUserName`/`UserName` 已补齐全局角色名查询、缺失静默和协议探针；共享 HarvestMonster carcass 生命周期与 Deer AI=1/2 逃跑移动已迁移；其他 miscellaneous 系统待补。 |
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
- `93b1d77 feat(p5): migrate Deer AI`
- `a25713a feat(p5): migrate EvilCentipede AI`
- `ff1cbd3 feat(p5): migrate RootSpider and BombSpider AI`
- `789022b feat(p5): migrate WoomaTaurus AI`
- `2215a6f feat(p5): migrate RedMoonEvil AI`
- `aa61479 feat(p5): migrate Shinsu AI`
- `fe56473 feat(p5): migrate KingScorpion AI`
- `9d15529 feat(p5): complete RightGuard and LeftGuard AI`
- `f04430f feat(p5): complete MinotaurKing AI`
- `7df022c feat(p5): complete Yimoogi AI`

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
- 保留最近击杀者五秒经验归属及组成员门禁；独立 Deer AI=1/2 逃跑移动已在随后批次补齐，不把共享 Harvest 生命周期与 Deer 专用 AI 混为同一实现边界。
- Go 世界测试覆盖皮肤计数、掉落/背包、无掉落、叠加线材、组门禁；认证 `net.Pipe` transcript 覆盖三次 Harvest、`UserLocation`/`ObjectHarvest`/领取与持久化。

Go 实现与迁移矩阵更新已提交为 `03dad46 feat(p11): migrate harvest lifecycle`；本批未修改任何 C# 文件。

## 本次完成的 P5 批次（Deer AI=1/2）

本批在共享 HarvestMonster 基础上实现 Legacy Deer 的独立 AI 分支：

- AI=1/2 已接入 common population；AI=2 保留构造时 1/7 逃跑选择、五次剥皮、质量步进和逃跑 Deer 的 `MoveSpeed - 300`，并在 Slow 到期恢复时重新应用该速度差。
- 逃跑分支保留继承的 ViewRange 搜索/目标重验、`DirectionFromPoint(Target, Current)` 直线逃跑、受阻后的 `NextDir`/`PreviousDir` 七次尝试，以及 ActionTime/MoveTime 冷却；AI=1 和非逃跑 AI=2 清除目标并执行继承式随机漫游。
- Go 世界测试覆盖目标发现、直线/受阻逃跑、被动与随机 `ObjectTurn` 行为、构造状态和 Slow 恢复；认证 `net.Pipe` transcript 锁定 `ObjectWalk` payload、坐标、方向、目标和运行时速度。

Go 实现与迁移矩阵更新已提交为 `93b1d77 feat(p5): migrate Deer AI`；本批未修改任何 C# 文件。

## 本次完成的 P5 批次（Tree AI=3）

本批实现 Legacy `Tree` 的静态生命周期与普通命中子集：

- AI=3 已接入 common population，物化时固定朝向 `Up`；运行时不移动、攻击、搜索、漫游或再生。
- 玩家、战士、Hero（按 Legacy 转换为 Owner）和通用 Monster 入口保留 `ACAgility` 严格门禁；敏捷未命中与护甲吸收静默返回，命中固定只扣 1 HP，不产生通用伤害指示、AttackBonus、暴击或中毒副作用，同时保留 HumanObject 命中后的武器耐久与 `GatherElement` 钩子。
- 保留 Tree 的经验归属、宠物/主人归属窗口、私有健康可见性，以及死亡时 `ObjectDied` 先于最终 `ObjectHealth` 的顺序；非 `ACAgility` 的近战变体、魔法和远程入口不产生伤害。
- Go 世界测试覆盖静态状态、命中/未命中/吸收、Monster 命中和死亡顺序；认证 `net.Pipe` transcript 锁定 `ObjectAttack`、`ObjectStruck`、`ObjectHealth` 的可观察顺序与最终 HP。尚未宣告其他特殊 Monster AI 作为 Tree 目标的全部入口完成。

Go 实现与迁移矩阵更新已提交为 `373dec2 feat(p5): migrate Tree AI`；本批未修改任何 C# 文件。

## 本次完成的 P5 批次（EvilCentipede AI=14）

本批实现 Legacy `EvilCentipede` 的独立隐藏、静止和区域攻击行为：

- AI=14 已接入 common population；物化时隐藏、不可移动，并通过 passive
  object、blocking、玩家单体/范围攻击及搜索门禁保持不可见，直到附近目标触发
  `FindNearby(3)` 显现。
- 隐藏状态每两秒检查，显现发送 `ObjectMonster -> ObjectShow`，设置 500ms
  cell readiness 和两秒 action delay；可见状态用七格保留范围检查，离开后发送
  `ObjectHide`、恢复 MaxHP、清空目标并进入三秒隐藏延迟；隐藏期间每次 AI 处理恢复
  MaxHP。
- 攻击保持当前方向，发送 `ObjectAttack`，500ms 后按 Legacy
  `FindAllTargets(7, CurrentLocation, false)` 的方形环和 Cell insertion order
  扫描 Player、owned Monster、Hero；伤害使用 MAC 而不计敏捷，Tree 的 MAC overload
  保持 no-op。
- 命中后保留 SC 值的 Green（chance 5、duration 15）与 Paralysis（chance 15、
  duration 5）毒状态、两秒 tick 以及 Monster/Human/Hero 的抵抗/机会顺序。Go
  世界测试覆盖隐藏目标 AOE、目标顺序、计时和 poison state，认证 `net.Pipe`
  transcript 覆盖显现、攻击、延迟命中和精确包序。
- Go 提交为 `a25713a feat(p5): migrate EvilCentipede AI`；本批未修改任何 C# 文件。

## 本次完成的 P5 批次（BugBagMaggot AI=12、RootSpider AI=39、BombSpider AI=40）

本批实现 Legacy 三个召唤/爆炸怪物 AI 的客户端可观察生命周期：

- BugBagMaggot 与 RootSpider 接入 common population，保持静止、同地图
  DataRange=16 目标门禁和 20 个存活从属上限；读取 Setup.ini 的
  `BugBatName`/`BombSpiderName`，发送 `ObjectAttack`，保留父怪 300ms action
  和 3 秒 attack 冷却。
- RootSpider 保留 Up/UpRight/Right 三个构造方向及 Back/DownRight/DownLeft
  召唤格；子怪在 500ms 延迟 Spawn 中出现，继承 Player Target，`Spawned`
  的两秒 ActionTime 仍可观察，并在后续 world tick 才开始处理。
- BombSpider 保留目标丢失、相邻不同格接触和五分钟超时死亡；死亡立即发
  `ObjectDied`，500ms 后按 `FindAllTargets(1, ..., false)` 的 cell/insertion
  顺序对 Player、owned Monster、Hero 做 ACAgility 爆炸，使用 Legacy
  DC/SC luck/unit-bound 随机、PoisonResistWeight/5 chance、Green 五秒和两秒
  首跳毒伤。
- Go 世界测试覆盖召唤位置、容量、following-tick、接触伤害和毒物状态；认证
  `net.Pipe` transcript 锁定 `ObjectAttack -> ObjectMonster -> ObjectDied ->
  delayed explosion/Chat`，并单独锁定首个 Green tick 的伤害和 `ServerPoisoned`。

Go 实现与迁移矩阵已提交为 `ff1cbd3 feat(p5): migrate RootSpider and BombSpider AI`；本批未修改任何 C# 文件。

## 本次完成的 P5 批次（WoomaTaurus AI=11）

本批实现 Legacy `WoomaTaurus` 的通用目标/攻击与专属状态子集：

- AI=11 接入 common population，继承 FlamingWooma 的相邻 `ObjectAttack`、
  300ms 延迟、MAC/Agility 命中和 AC/MAC 分离；父类计时与目标重验仍由共享
  Monster AI pipeline/resolver 处理。
- 保留每 10 秒一次的八邻格阻塞扫描；五格或以上阻塞时清空目标并从
  `WalkableCell` 取随机传送点，按 `ObjectTeleportOut -> ObjectRemove ->
  ObjectMonster -> ObjectTeleportIn` 发送旧/新视野包，普通野生怪不额外发
  `ObjectHealth`。
- 保留 HP 七阶段阈值、进入阶段时 8 秒的 `MoveSpeed=400`/
  `AttackSpeed=500` Mad 状态，以及严格到期后的原配置速度恢复。
- Go 世界测试覆盖攻击、五阻塞传送、视野包序与 Mad 计时；认证
  `net.Pipe` transcript 覆盖 `ObjectAttack` 和延迟
  `Struck/ObjectStruck/DamageIndicator/HealthChanged`。

Go 实现、测试和迁移矩阵已提交为 `789022b feat(p5): migrate WoomaTaurus AI`；本批未修改任何 C# 文件。

## 本次完成的 P5 批次（RedMoonEvil AI=13）

本批实现 Legacy `RedMoonEvil` 的静止多目标攻击行为：

- AI=13 接入 common population，物化时固定朝向 `Up`；route 与普通移动路径均跳过，独立分支不进入普通移动/再生处理。
- 对齐出生生命周期：通用 `Spawned` 的两秒 ActionTime 覆盖构造器初始 300ms；每次攻击后保留 300ms ActionTime 和 `AttackSpeed` 冷却。
- 按 `FindAllTargets(ViewRange, CurrentLocation)` 的 Legacy cell/insertion order 扫描同地图 Player、owned Monster、Hero；同格目标仍可选，隐藏目标遵守 CoolEye/等级可见性门禁。
- 一次攻击先发送 `ObjectAttack`，每个非零 DC 目标排队 300ms 的 ACAgility 延迟动作，并立即从攻击者位置广播 `ObjectEffect`，保留 `SpellEffect.RedMoonEvil` 值 4；延迟 resolver 覆盖玩家/Monster/Hero 并在 impact 时重验目标。
- Go 世界测试覆盖初始时序、静止、同格/隐藏/越界筛选、效果顺序、观察者包和 AC+Agility 延迟伤害；认证 `net.Pipe` transcript 覆盖 `ObjectAttack -> ObjectEffect -> Struck/ObjectStruck/DamageIndicator/HealthChanged`。

Go 实现、测试和迁移矩阵已提交为 `2215a6f feat(p5): migrate RedMoonEvil AI`；本批未修改任何 C# 文件。

## 本次完成的 P5 批次（ZumaMonster AI=15）

本批实现 Legacy `ZumaMonster` 的石化、唤醒和继承近战行为：

- AI=15 接入 common population；物化时保留 `MonsterObject` 的随机朝向与
  `Stoned=true`，`ObjectMonster.Extra` 在出生阶段正确投影；route 与普通 AI
  移动均遵守 `AvoidFireWall`。
- 对齐严格的两秒出生 ActionTime 门禁；`FindNearby(2)` 发现有效目标后发送
  `ObjectShow` 唤醒自身，并按 Legacy `WakeAll(14)` 唤醒同范围 Zuma、继承当前
  目标并保留唤醒后的 1 秒 ActionTime。
- 石化阶段拒绝移动、攻击、Push、毒和 Buff；苏醒后复用 inherited
  Player/owned-Monster/Hero 搜索与相邻 `ObjectAttack`，保留 300ms 延迟
  MAC+Agility 伤害、Shock 的近身攻击/远距移动分支、堆叠绕行和随机漫游。
- Go 世界测试覆盖出生/唤醒边界、`WakeAll(14)`、FireWall 阻断、Player/
  owned-Monster/Hero 目标和延迟 HP；认证 `net.Pipe` transcript 覆盖
  `ObjectMonster.Extra -> ObjectShow -> ObjectAttack -> Struck/ObjectStruck/
  DamageIndicator/HealthChanged` 包序。

Go 实现、测试和迁移矩阵已提交为 `61cfa48 feat(p5): migrate ZumaMonster AI`；本批未修改任何 C# 文件。

## 本次完成的 P5 批次（RedThunderZuma AI=16）

本批实现 Legacy `RedThunderZuma` 的共享石化生命周期与独有雷电攻击行为：

- AI=16 接入 Zuma common population；物化时保留随机朝向、`Stoned=true` 和
  `ObjectMonster.Extra` 投影，严格遵守两秒出生门禁、`FindNearby(2)` 唤醒和
  `WakeAll(14)` 传播，并继续遵守 `AvoidFireWall`、Shock 与石化状态门禁。
- 对齐 Legacy 的九格 Chebyshev 攻击范围（含边界）：相邻目标使用
  `ObjectAttack` 与 `MACAgility`，命中延迟 300ms；同格和非相邻远程目标使用
  `ObjectRangeAttack` 与 `MAC`，命中延迟 500ms，远程攻击冷却额外增加 500ms。
  `ProcessTarget` 保留范围内冷却与范围外移动的边界行为。
- 延迟结算在 Player、owned Monster、Hero 三类目标上重验地图、存活和攻击资格，
  并覆盖世界行为与认证 `net.Pipe` transcript 的包序、范围边界、冷却移动和命中时序。

Go 实现、测试和迁移矩阵已提交为 `9a1d215 feat(p5): migrate RedThunderZuma AI`；本批未修改任何 C# 文件。

## 本次完成的 P5 批次（ZumaTaurus AI=17）

本批实现 Legacy `ZumaTaurus` 的 HP 阶段召唤与继承 ZumaMonster 行为：

- AI=17 接入 Go common population；物化时固定 `MirDirection.DownLeft`、保留
  `Stoned=true`/`ObjectMonster.Extra`，并按 Legacy 构造器设置
  `AvoidFireWall=false`。共享两秒出生 ActionTime、`FindNearby(2)` 唤醒、
  `WakeAll(14)` 和唤醒后 1 秒门禁继续复用 ZumaMonster pipeline。
- 在继承 `ProcessAI` 前执行 HP 七阶段钩子，使用整数 `MaxHP/7` 除数；每次
  下降阶段最多召唤一波 8 个子怪，总 `SlaveList` 上限 40。达到上限时仍发送
  `ObjectAttack(Type=1)` 并更新父怪 300ms/AttackSpeed 计时。
- 增加 Legacy `Zuma1..Zuma7` 配置字段和默认名（`ZumaStatue`、
  `ZumaGuardian`、`ZumaArcher`、`WedgeMoth`、`ZumaArcher3`、
  `ZumaStatue3`、`ZumaGuardian3`）；子怪按 `Next(7)` 查找并复制当前目标，
  设置两秒 ActionTime。Front 无效时回落父怪格；子怪保留其自身
  `GetMonster` 构造方向，不继承父方向，野生子怪只发送 `ObjectMonster`，不发送
  归属/召唤 `ObjectHealth`。
- Go 世界测试覆盖阶段边界、单波次、40 上限、配置查找、合法/非法 Front、
  子怪状态和普通 MAC/Agility 近战；认证 `net.Pipe` transcript 覆盖
  `ObjectAttack(Type=1) -> 8*ObjectMonster -> ObjectShow -> ObjectAttack(Type=0)
  -> Struck/ObjectStruck/DamageIndicator/HealthChanged`。

Go 实现、测试和迁移矩阵已提交为 `2830708 feat(p5): migrate ZumaTaurus AI`；本批未修改任何 C# 文件。

## 本次完成的 P5 批次（Shinsu AI=18）

本批实现 Legacy `Shinsu` 的野生/高级道士召唤子怪行为，以及普通
`Spell.SummonShinsu` 宠物的 AI 专用路径：

- AI=18 接入 Go common population；物化时保留 Legacy 构造器随机方向，并在
  `Spawned` 投影 `Summoned`/`ObjectMonster.Extra=true`；对象图像按隐藏态 79、
  显形态 80 输出，客户端继续通过 `ObjectShow`/`ObjectHide` 切换图像。
- 对齐 `Mode`/`ModeTime`：有目标时延长 30 秒，严格遵守 `ActionTime`，显形/隐藏
  各锁定 1 秒；攻击仅在显形态可用。`InAttackRange` 使用 Legacy 两格奇偶形几何，
  `ObjectAttack` 后设置 300ms ActionTime 与独立 AttackSpeed 冷却。
- `LineAttack(damage, 2)` 按方向扫描两格，每格最多选一个合法 Player/Monster/Hero，
  以 Cell 插入顺序确定首个目标；命中延迟为 `500ms + 50ms*distance`，impact 重新
  验证地图、存活、归属/攻击模式和目标门禁，并使用 ACAgility 防御与 Legacy AI 随机流。
- 普通 Shinsu 宠物使用独立的 `PetActionAt`/`ShinsuAttackAt` 双冷却、主人方向捕获、
  目标/召回/模式门禁和 Player/Monster/Hero 延迟投影；击杀野怪接入宠物经验、任务、
  掉落与目标清理。高级道士 AI=18 子怪沿用 `OwnerObjectID` 和继承目标路径。
- Go 世界测试覆盖 79/80/79 图像、Show/Hide 边界、奇偶形范围、Cell 插入顺序、两格
  延迟伤害和普通宠物；认证 `net.Pipe` transcript 覆盖 bootstrap、显形、严格攻击门禁、
  `ObjectAttack`、600ms LineAttack impact 与玩家生命包序。

Go 实现、测试和迁移矩阵已提交为 `aa61479 feat(p5): migrate Shinsu AI`；本批未修改任何
C# 文件。

## 本次完成的 P5 批次（KingScorpion AI=19）

本批实现 Legacy `KingScorpion` 的野生与普通宠物行为：

- AI=19 已接入 Go common population；物化遵守 `MonsterObject` 构造器的随机方向，
  普通目标范围保持 Legacy 两格奇偶几何（相邻、同对角线和同 parity 的坐标）。
- 攻击先按两格末端 Cell 是否存在可攻击的 Monster/Player 决定
  `ObjectRangeAttack`，否则保留 `Random.Next(5)==0` 的范围分支；另一分支发送
  `ObjectAttack`。两种攻击均设置 300ms ActionTime 与独立 AttackSpeed 冷却。
- `LineAttack(damage, 2, 300)` 按 Cell 插入顺序每格最多排队一个 Player/Monster/Hero，
  使用 `distance*50ms + 300ms` 延迟；范围分支使用 MAC/Agility，普通分支使用
  AC/Agility，并在 impact 重新检查地图、归属、存活和攻击目标门禁。
- 普通 KingScorpion 宠物使用独立双冷却和主人目标门禁，支持 Player/Monster/Hero
  投影；击杀野怪后接入宠物经验、任务、掉落和目标清理。世界测试覆盖越界端点、
  Cell 顺序、两种伤害分支和普通宠物击杀，认证 `net.Pipe` transcript 覆盖 bootstrap、
  targetless `ObjectRangeAttack`、400ms 两格延迟和玩家生命包序。

Go 实现、测试和迁移矩阵已提交为 `fe56473 feat(p5): migrate KingScorpion AI`；本批未修改任何
C# 文件。

## 本次完成的 P5 批次（DarkDevil AI=20）

本批实现 Legacy `DarkDevil` 的野生目标搜索、动态范围和范围伤害行为：

- AI=20 已接入 Go common population；物化遵守 `MonsterObject` 基类构造器的随机
  方向。`InAttackRange` 严格保留 `_areaTime` 边界：冷却窗口内为一格，
  `Envir.Time > _areaTime` 后扩展为三格。
- 首次/范围窗口过期时发送无目标 `ObjectRangeAttack`，设置
  `2s + Random.Next(3)*1s` 的区域窗口、300ms ActionTime 和 AttackSpeed 冷却，
  并排入 500ms 延迟的 RangeDamage；活动窗口内沿用基类 `ObjectAttack` 近战，
  单位区间 DC 抽样和 300ms 延迟。
- 延迟范围命中按 impact 时怪物当前位置和方向前方两格重新投影，再以 Legacy
  Cell 插入顺序扫描半径一格；Player、owned Monster、Hero 通过隐藏/视野、地图、
  安全区、归属和存活门禁后使用 MACAgility。原目标与 impact 目标均重新验证，
  认证 transcript 覆盖 targetless 包、500ms 命中包序与玩家生命状态。

Go 实现、世界/Cell-order/目标种类测试、认证 `net.Pipe` transcript 和迁移矩阵已提交为
`cc7e0d1 feat(p5): migrate DarkDevil AI`；本批未修改任何 C# 文件。

随后对照 Legacy `FindTarget` 的 `Target ??=`/已有 Player 跳过语义，单独修正了 DarkDevil
周期搜索不应替换已有目标的行为；Go follow-up 提交为 `433c1e9 fix(p5): preserve
DarkDevil target search semantics`，定向世界测试已通过。

## 本次完成的 P5 批次（IncarnatedGhoul AI=21）

本批实现 Legacy `IncarnatedGhoul` 的基类目标搜索、近战、延迟命中与麻痹行为：

- AI=21 已接入 Go common population；物化遵守 `MonsterObject` 构造器的随机方向。
  `ProcessSearch`/`FindTarget` 保留 Legacy 的奇偶形 ViewRange 扫描、Cell 插入顺序、
  Monster/Hero 的 `Target ??=` 保留和已有 Player 目标跳过语义；相邻范围严格排除同格。
- `Attack` 发送 `ObjectAttack`，设置独立 300ms ActionTime 与 AttackSpeed 冷却，按
  DC 使用 Legacy inclusive/unit-bound/Luck 抽样排入延迟动作；impact 时重新检查地图、
  安全区、归属、存活和 `IsAttackTarget`，使用 AC+Agility 投影到 Player、owned Monster
  和 Hero。
- 正命中后按 Legacy `PoisonTarget(target, 15, 5, Paralysis, 1000)` 重新抽取 SC，
  保留初始抵抗、15 分之一机会和 Human/Hero `ApplyPoison` 的第二次抵抗；Go 明确设置
  首次毒 tick 为 impact 后 1 秒，并发送玩家 `You have been poisoned`。
- 世界测试覆盖玩家/宠物/Hero、Cell 顺序、目标重验证、毒值/时序/随机边界；认证
  `net.Pipe` transcript 覆盖 bootstrap、`ObjectAttack`、300ms impact、伤害/健康/聊天
  包序与最终 Paralysis 状态。

Go 实现、测试和迁移矩阵已提交为 `a0a2c2f feat(p5): migrate IncarnatedGhoul AI`；本批未修改任何
C# 文件。

## 本次完成的 P5 批次（IncarnatedZT AI=22）

本批实现 Legacy `IncarnatedZT` 在 `ZumaMonster` 基础生命周期上的特殊攻击行为：

- AI=22 已接入 Go common population，并复用 `ZumaMonster` 的唤醒、搜索、漫游和相邻
  目标生命周期；构造时保持 `Stoned=false` 与 `AvoidFireWall=false`，bootstrap 的
  `Extra` 和 FireWall 行为与 Legacy 子类构造器一致。
- 相邻攻击发送 `ObjectAttack`，使用 DC 的 inclusive/unit-bound/Luck 抽样，独立设置
  300ms ActionTime 与 AttackSpeed 冷却；impact 时重新验证地图、存活、归属、安全区和
  `IsAttackTarget`，延迟命中沿用 `MACAgility` 投影到 Player、owned Monster 与 Hero。
- 实际命中后执行 `PoisonTarget(target, 12, 5, Paralysis, 1000)`，保留 SC 值、初始
  PoisonResist、12 分之一机会以及 Human/Hero 的第二次抵抗，并固定首次毒 tick 为
  impact 后 1 秒；世界测试覆盖目标类型、随机边界、FireWall/未石化状态和延迟失效目标。
- 认证 `net.Pipe` transcript 覆盖未石化 bootstrap、`ObjectAttack`、MACAgility 延迟
  impact、玩家伤害/健康/聊天包序与最终 Paralysis 状态；AI=21 共享麻痹 helper 的
  回归测试保持通过。

Go 实现、测试和迁移矩阵已提交为 `0c3ca86 feat(p5): migrate IncarnatedZT AI`；本批未修改任何
C# 文件。

## 本次完成的 P5 批次（BoneFamiliar AI=23）

本批实现 Legacy `BoneFamiliar` 对基础 `MonsterObject` 的轻量特殊化：

- AI=23 已接入 Go inherited base AI population；构造器固定方向为
  `MirDirection.DownLeft`，`Spawned` 在首个对象包前将 `Summoned=true`，
  `GetInfo` 因而投影 `ObjectMonster.Extra=true`。
- Go `materializeMonster` 对 AI=23 固定方向 5 并预置 summoned 状态，普通
  `SummonSkeleton` 使用真实 AI=23 定义；保留 500ms 延迟召唤、主人对象 ID、
  `ObjectMonster` 名称/图像/方向和健康包序。
- 静态构造/投影测试、SummonSkeleton 世界测试及认证 `net.Pipe` transcript
  覆盖继承的基础 AI admission、固定方向、summoned 标志、master projection 和
  delayed visibility；AI=21/22 与 OrdinaryPet/Shinsu 回归保持通过。

Go 实现、测试和迁移矩阵已提交为 `f8e2a2c feat(p5): migrate BoneFamiliar AI`；本批未修改任何
C# 文件。

## 本次完成的 P5 批次（DigOutZombie AI=24）

本批实现 Legacy `DigOutZombie` 的隐藏挖出生命周期，并复用已迁移的 DigOut/洞穴基础设施：

- AI=24 已接入 Go common population；构造时保持隐藏、不可阻挡、不可被玩家或其他
  目标攻击，按 `FindNearby(3)` 每两秒严格检查一次；首次发现有效目标后发送
  `ObjectMonster` 再发送 `ObjectShow`，捕获当前位置和方向并恢复 inherited
  `MonsterObject` 基础搜索、移动与攻击流程。
- 显形后严格等待 1 秒才生成 `Spell.DigOutZombie`，使用挖出时捕获的坐标/方向、
  Value=1、五分钟有效期和两秒 tick；AI=124/125 Armadillo 的 500ms 洞穴延迟保持
  独立，避免共享 helper 改变既有行为。
- 世界测试覆盖隐藏 bootstrap/可见性/阻挡/攻击门禁、2 秒探测、严格 1 秒边界、
  洞穴 payload/生命周期和显形后的 inherited 基础攻击；认证 `net.Pipe` transcript
  覆盖隐藏启动、显形包序和 DigOutZombie 洞穴包。

Go 实现、测试和迁移矩阵已提交为 `682302c feat(p5): migrate DigOutZombie AI`；本批未修改任何
C# 文件。

## 本次完成的 P5 批次（RevivingZombie AI=25）

本批实现 Legacy `RevivingZombie` 的死亡复活状态机，并保留继承的普通
`MonsterObject.ProcessAI` 路径：

- AI=25 已接入 Go common population；构造时初始化 `LifeCount=rand.Intn(3)`，
  初始对象保持普通怪物的可见性与 `ObjectMonster.Extra=false`。
- 每次非最终死亡按 Legacy `Random.Next(20)` 记录 4–23 秒复活窗口；严格在
  截止时间之后以 75%/50% 阶段恢复，清除死亡/重生计时并发送
  `ObjectRevived(ObjectID, false)`。
- 当前 `RevivalCount` 参与经验缩放（100%/75%/50%）；内部复活不减少共享
  respawn group 的 active population，最终死亡才进入普通 despawn/respawn bookkeeping。
- 世界测试覆盖构造范围、精确截止边界、随机抽样、两次复活、最终死亡、经验
  缩放和复活后基础相邻攻击；认证 `net.Pipe` transcript 覆盖 `ObjectDied` →
  `ObjectRevived` 包序与 payload。

Go 实现、测试和迁移矩阵已提交为 `1a7c654 feat(p5): migrate RevivingZombie AI`；本批未修改任何
C# 文件。

## 本次完成的 P5 批次（ShamanZombie AI=26）

本批实现 Legacy `ShamanZombie` 的六格轴线/对角线范围攻击，并保留其基础
MonsterObject 目标搜索、移动、冷却和延迟重验证语义：

- AI=26 已接入 Go common population；目标搜索范围与六格攻击几何分离，支持
  Player、owned-Monster、Hero 的 Cell-order 首目标投影，并保留 Shock、safe-zone
  和攻击/行动冷却门禁。
- `ObjectRangeAttack` 只投影 ObjectID、当前位置和方向；命中伤害采用 Legacy
  inclusive DC/Luck 抽样，动作固定 300ms，直线每格追加 50ms，使用 MACAgility
  防御并在延迟命中时重新验证目标有效性。
- 世界测试覆盖范围边界、搜索后移动、Cell-order 三类目标、六格延迟和安全区
  取消命中；认证 `net.Pipe` transcript 覆盖真实 bootstrap、攻击 payload、
  600ms 命中包序、HP 和冷却状态。

Go 实现、测试和迁移矩阵已提交为 `de999b0 feat(p5): migrate ShamanZombie AI`；本批未修改任何
C# 文件。

## 本次完成的 P5 批次（Khazard AI=27）

本批实现 Legacy `Khazard` 的四格拉拽攻击，并保留其基础
`MonsterObject` 目标搜索、移动、攻击冷却和延迟命中语义：

- AI=27 已接入 Go common population；目标搜索与攻击几何分离，拒绝同格和不等边
  对角线，接受四格以内的轴线/等边对角线目标；相邻目标走基础 `ObjectAttack`，
  远端目标进入 `Range` 分支并受 `PullTime` 五秒冷却约束。
- 远端攻击逐格按 Legacy `MagicResistWeight` 检查，成功后以目标到攻击者的反向
  方向调用 Player、owned Monster、Hero 的 `Pushed` 投影，再发送不带 TargetID 的
  `ObjectRangeAttack`；ActionTime 固定 300ms，AttackTime 仍按 AttackSpeed。
- 相邻攻击释放 BindingShot、清除 Shock，发送基础 `ObjectAttack`，使用 inclusive
  DC/Luck/unit-bound 抽样，在 300ms 后用 AC+Agility 重新验证 Player、owned Monster
  和 Hero 目标；安全区、存活、地图和归属变化会取消延迟命中。
- 世界测试覆盖几何边界、逐格推送顺序、魔抗拒绝、PullTime/cooldown、Player/Monster/
  Hero 投影、inclusive DC 与延迟重验证；认证 `net.Pipe` transcript 覆盖真实
  bootstrap、三个 `ServerPushed` 后的 `ObjectRangeAttack` 包序和最终坐标/冷却。

Go 实现、测试和迁移矩阵已提交为 `becac1db47d4a60e2a37e94d14440c572db1a903`
（`feat(p5): migrate Khazard AI`）；本批未修改任何 C# 文件。

## 本次完成的 P5 批次（ToxicGhoul AI=28）

本批实现 Legacy `ToxicGhoul` 的相邻攻击、Green 中毒和 Effect=1 死亡爆发，
并将其接入共享 HarvestMonster/no-normal-loot 生命周期：

- AI=28 已加入 Go common population；物化时保留两次剥皮状态，普通死亡掉落
  被抑制，Harvest 路径继续复用既有 carcass/缓存掉落实现。
- 普通 AI 复用 Legacy 目标搜索/移动/冷却边界，支持 Player、owned Monster、
  Hero 投射；相邻攻击发送 `ObjectAttack`，300ms 后以 MACAgility 重新计算
  命中，命中后按 SC 值、一次 PoisonResist 和一次机会门施加 Green。
- Green poison 保留 Legacy 的 duration=5、tick=2s 和零初始 TickAt，
  因而 impact tick 会同时产生首个 Green 伤害与 `Poisoned` 状态；延迟动作
  在 impact 时重新验证目标，目标失效则静默取消。
- Effect=1 死亡只排入一个严格 1 秒后的动作；到期时重新扫描半径一的
  当前 Cell 顺序，对 Player/owned Monster/Hero 分别使用 AC+Agility，
  每个目标独立 DC/Green poison 抽样，并保留 Legacy 的首个伤害失败即返回
  语义。Effect=0 不排入死亡动作。
- 纯世界测试覆盖 Harvest/no-loot、攻击 payload/延迟、Player/Monster/Hero
  投射、魔抗、目标重验证、死亡当前扫描、Effect 分支、首个 miss 的早返和
  poison 状态；认证 `net.Pipe` transcript 覆盖真实登录、攻击、延迟伤害、
  首个 Green tick、健康包和 `Poisoned` 包序。

Go 实现、测试和迁移矩阵已提交为 `43017a1 feat(p5): migrate ToxicGhoul AI`；本批未修改任何
C# 文件。

## 本次完成的 P5 批次（BoneSpearman AI=29）

本批将 Legacy `BoneSpearman` 从普通玩家目标子切片收口为完整的三类目标
投影，并保留其继承的目标搜索、移动、冷却和延迟命中行为：

- AI=29 已接入 Go common population 与专用处理路径；保留 Legacy 两格 parity-shaped
  直线攻击几何、同格拒绝、方向计算、`ObjectAttack` 和 300ms ActionTime/AttackSpeed
  冷却，并按 inclusive DC 抽样。
- 目标搜索和直线逐格扫描均使用 Cell insertion order 的首个有效目标，支持 Player、
  owned Monster、Hero；搜索期 Hidden/Hallucination 过滤与已选目标行攻击期过滤保持
  Legacy 的阶段边界。
- 每个命中格按距离追加 50ms，再加 BoneSpearman 的 250ms 基础延迟；Player、owned
  Monster、Hero 分别走对应 AC+Agility/延迟重验证路径，无毒素副作用；Hero 使用包含
  agility 的 Legacy 防御公式。
- 确定性测试覆盖三类目标、插入顺序、几何边界、350ms 两格命中、死亡目标重验证；
  认证 `net.Pipe` transcript 覆盖 bootstrap、`ObjectAttack`、延迟
  `Struck/ObjectStruck/DamageIndicator/HealthChanged` 包序和最终 HP。

Go 实现、测试和迁移矩阵已提交为 `b9c2cb9 feat(p5): migrate BoneSpearman AI`；本批未修改任何
C# 文件。

## 本次完成的 P5 批次（BoneLord AI=30）

本批实现 Legacy `BoneLord` 的目标、远程/近战攻击、阶段召唤和客户端可观察
行为，并将 AI=30 接入 Go common population：

- 物化保留基础构造器的随机方向；目标搜索沿用 Cell insertion order 及 Player、owned
  Monster、Hero 投影。七格 Chebyshev 范围内，同格或非相邻目标发送
  `ObjectRangeAttack`，相邻目标发送 `ObjectAttack`；分别在 MACAgility 与 ACAgility
  路径延迟命中，并在 impact 时重新验证目标。
- 保留 300ms ActionTime 与 AttackSpeed 冷却；每个阶段使用整数 `HP/(MaxHP/3)`
  阈值，只触发一次召唤波。每波最多八只、活动子怪最多 40 只，按配置的
  `BoneMonster1..BoneMonster4` 随机选择定义，复制当前目标，使用 Front 或当前位置
  fallback，子怪两秒后才可行动，并以无归属的 `ObjectMonster` 广播。
- 子怪不挂接普通宠物所有权，也不因 BoneLord 死亡而被批量清除；父怪会清理死亡/缺失
  子怪 ID 后再计算活动容量，匹配 Legacy `SlaveList` 的可观察生命周期。
- 确定性世界测试覆盖三类目标、几何和延迟、阶段/配置/容量/目标复制/Front 投影；
  认证 `net.Pipe` transcript 覆盖 bootstrap、远程攻击、冷却期 `ObjectWalk`、延迟
  `Struck/ObjectStruck/DamageIndicator/HealthChanged` 包序和最终 HP。

Go 实现、测试和迁移矩阵已提交为 `8df9715 feat(p5): migrate BoneLord AI`；本批未修改任何
C# 文件。

本批新增并通过以下门禁：

- `go test ./cmd/crystal-server -run 'BoneLord' -count=1`
- `go test -race ./cmd/crystal-server -run 'BoneLord' -count=1 -timeout=120s`
- `go test ./... -count=1 -timeout=300s`
- `go vet ./...`
- `go build ./...`
- Go 仓库提交前 `git diff --check` 与 tracked/staged/untracked `.cs` 零变化检查。

## 本次完成的 P5 批次（SandWorm AI=35）

本批将 Legacy `SandWorm` 从已存在的玩家目标子切片收口为完整的三类目标、
harvest 生命周期和客户端可观察行为：

- AI=35 接入专用 common-population 处理路径；保留 `HarvestMonster` 的两次剥皮
  状态和普通死亡无 loot 行为，物化时保留基础构造器的随机方向。
- 目标搜索沿用 Legacy Cell 顺序并支持 Player、owned Monster、Hero；攻击保留
  `SpittingSpider` 的 parity-shaped 两格范围、Type 0 `ObjectAttack`、300ms
  ActionTime/AttackSpeed 冷却和每个 line cell 的首个有效目标。
- 使用 Legacy inclusive DC/Luck 攻击力；每格按距离增加 50ms，再加 300ms
  ACAgility 延迟命中。Player、owned Monster、Hero 均在 impact 时重验，且不产生
  Green poison。
- 确定性世界测试覆盖目标搜索/三类投影、几何、Cell insertion order、Luck/DC、
  harvest/no-loot、距离延迟和死亡目标取消；认证 `net.Pipe` transcript 覆盖
  bootstrap、`ObjectAttack`、延迟 `Struck/ObjectStruck/DamageIndicator/HealthChanged`
  包序和最终 HP。

Go 实现、测试和迁移矩阵已提交为 `a894f47 feat(p5): complete SandWorm AI`；本批未修改任何
C# 文件。

本批新增并通过以下门禁：

- `go test ./cmd/crystal-server -run 'SandWorm' -count=1 -timeout=120s`
- `go test -race ./cmd/crystal-server -run 'SandWorm' -count=1 -timeout=120s`
- `go test ./cmd/crystal-server -run 'SandWorm|BoneLord|BoneSpearman|ToxicGhoul' -count=1 -timeout=180s`
- `go test ./... -count=1 -timeout=300s`
- `go vet ./...`
- `go build ./...`
- Go 仓库提交前 `git diff --check` 与 tracked/staged/untracked `.cs` 零变化检查。

## 本次完成的 P5 批次（VenomSpider AI=100）

本批将 Legacy `VenomSpider` 从已有 Player 目标子切片收口为完整的三类目标与
客户端可观察行为：

- AI=100 接入专用 common-population 处理路径；物化保留 `MonsterObject` 构造器的
  随机 0–7 朝向，继承目标搜索、攻击/移动冷却和 `ShockTime` 清理语义。
- 目标搜索和 `LineAttack` 逐格使用 Cell insertion order 的首个有效目标，支持
  Player、owned Monster、Hero；保留两格 parity-shaped 几何、Type 0 `ObjectAttack`、
  300ms ActionTime、独立 AttackSpeed 冷却以及每格 50ms 距离延迟。
- 延迟命中使用 `MACAgility`，在 Player、owned Monster、Hero 三类目标上重新验证
  地图、存活、归属和攻击资格；实际命中后施加 Green poison（8/5/1000ms），保持
  Player/Hero 两次抵抗检查与 Monster 一次抵抗检查的 Legacy 顺序。
- Go 世界测试覆盖构造朝向、搜索、三类投影、几何、Cell 顺序、距离延迟、DC/Luck、
  poison 随机边界和死亡目标取消；认证 `net.Pipe` transcript 覆盖 bootstrap、
  `ObjectAttack`、400ms 命中、Green 首跳、包序和最终 HP。

Go 实现、测试和迁移矩阵已提交为 `7397c43 feat(p5): complete VenomSpider AI`；
本批未修改任何 C# 文件。

本批新增并通过以下门禁：

- `go test ./cmd/crystal-server -run 'VenomSpider' -count=1 -timeout=180s`
- `go test -race ./cmd/crystal-server -run 'VenomSpider' -count=1 -timeout=180s`
- `go test ./cmd/crystal-server -run 'VenomSpider|SandWorm|BoneSpearman|BoneLord|SpittingSpider|ToxicGhoul' -count=1 -timeout=240s`
- `go test ./... -count=1 -timeout=360s`
- `go vet ./...`
- `go build ./...`
- Go 仓库提交前 `git diff --check` 与 tracked/staged/untracked `.cs` 零变化检查。

## 本次完成的 P5 批次（BlackFoxman AI=44）

本批将 Legacy `BlackFoxman` 从已有 Player 目标子切片收口为完整的三类目标与
客户端可观察行为：

- AI=44 接入专用目标/搜索/移动处理路径；继承 `MonsterObject` 的两格
  parity-shaped 攻击范围、目标保留、ActionTime/AttackSpeed 冷却和 `ShockTime`
  清理语义，并在搜索时保存 Player、owned Monster、Hero 的 TargetKind。
- 近身分支保留 `Random.Next(3)>0` 的普通 `ObjectAttack` 与 300ms 延迟；非近身
  分支发送 Type=1 `ObjectAttack`，逐格按 Cell insertion order 捕获首个有效目标，
  每格增加 50ms 并叠加 250ms 线攻击延迟。Player、owned Monster、Hero 均使用
  ACAgility 目标解析，且 impact 时重新验证地图、存活、归属和攻击资格。
- 新增确定性世界测试覆盖三类目标、搜索、几何、Cell 顺序、近身/直线分支、延迟
  和死亡目标取消；认证 `net.Pipe` transcript 覆盖 bootstrap、Type=1 攻击包、
  250ms ActionTime、350ms 两格命中、`Struck/ObjectStruck/DamageIndicator/
  HealthChanged` 包序和最终 HP。

Go 实现、测试和迁移矩阵已提交为
`e61f523 feat(p5): complete BlackFoxman AI`；本批未修改任何 C# 文件。

本批新增并通过以下门禁：

- `go test ./cmd/crystal-server -run 'BlackFoxman' -count=1 -timeout=240s`
- `go test -race ./cmd/crystal-server -run 'BlackFoxman' -count=3 -timeout=600s`
- `go test ./cmd/crystal-server -run 'BlackFoxman|BoneSpearman|SandWorm|VenomSpider|SpittingSpider|ToxicGhoul' -count=1 -timeout=300s`
- `go test ./... -count=1 -timeout=600s`
- `go test -race ./... -count=1 -timeout=900s`
- `go vet ./...`
- `go build ./...`
- Go 仓库提交前 `git diff --check` 与 tracked/staged/untracked `.cs` 零变化检查。

## 本次完成的 P5 批次（HedgeKekTal AI=51）

本批将 Legacy `HedgeKekTal` 从已有 Player 目标子切片收口为完整的三类目标与
客户端可观察行为：

- AI=51 接入独立 `RightGuard.ProcessTarget` 兼容流程；继承 8 格攻击范围、搜索刷新、
  `ShockTime` 清理、攻击/行动冷却和攻击冷却期间的目标移动，并按 Legacy Cell
  insertion order 保留 Player、owned Monster、Hero 的 TargetKind。
- 近身分支发送 `ObjectAttack`，使用 AC+Agility，300ms 后结算；同格或非相邻目标
  发送 `ObjectRangeAttack`，使用普通 AC，按 Chebyshev 距离每格增加 50ms，并叠加
  Legacy 500ms 投射基准；远程 Hero 的普通 AC 也显式保留。
- 延迟动作在 impact 时重新验证地图、存活、归属和攻击资格；Player、owned Monster、
  Hero 分别写回正确的生命状态和协议通知，不产生额外 poison 副作用。
- Go 世界测试覆盖三类目标的近/远程投影、目标搜索、动作防御标记、延迟重验和死亡
  目标取消；认证 `net.Pipe` transcript 覆盖 bootstrap、KeepAlive barrier、远程攻击、
  冷却期 `ObjectWalk`、延迟 `Struck/ObjectStruck/DamageIndicator/HealthChanged`
  包序和最终状态。

Go 实现、测试和迁移矩阵已提交为
`1cc9f54 feat(p5): complete HedgeKekTal AI`；本批未修改任何 C# 文件。

本批新增并通过以下门禁：

- `go test ./cmd/crystal-server -run 'HedgeKekTal' -count=1`
- `go test -race ./cmd/crystal-server -run 'HedgeKekTal' -count=1 -timeout=300s`
- `go test ./cmd/crystal-server -count=1 -timeout=300s`
- `go test ./... -count=1 -timeout=600s`
- `go vet ./...`
- `go build ./...`
- Go 仓库提交前 `git diff --check` 与 tracked/staged/untracked `.cs` 零变化检查。

全包 `go test -race ./cmd/crystal-server -count=1 -timeout=600s` 本次仍复现既有
session ticker/Buff 共享 fixture 竞争（`player_spell_buffs.go:742` 与
`intelligent_creature_items.go:549`），并出现既有 Kirin transcript 的未同步随机流
竞争；失败栈未进入 HedgeKekTal，本批不将全包 race 记为通过。

## 本次完成的 P5 批次（CrazyManworm AI=66）

本批将 Legacy `CrazyManworm` 从已有 Player 目标子切片收口为完整的三类目标与
客户端可观察行为：

- AI=66 接入独立的 `MonsterObject` 目标/搜索/近战流程；保留继承的
  `CanAttack` 门禁、相邻攻击范围、`ShockTime` 清理与攻击/行动冷却，并按 Cell
  insertion order 保留 Player、owned Monster、Hero 的 TargetKind。
- 近战分支保留 `Random.Next(3)==0` 的 Type 1 MC 攻击与其余 Type 0 DC 攻击；两者
  都发送 `ObjectAttack`，使用 ACAgility，按 Legacy inclusive `GetAttackPower` 与
  Luck 随机边界计算，300ms 后结算。
- Go 延迟动作在 impact 时重新验证地图、存活、归属、目标类型和攻击范围；未来
  `ShockTime` 仍允许已锁定目标攻击，只有正伤害才清理 Shock 并推进 Action/Attack
  冷却，Player、owned Monster、Hero 分别写回正确的生命状态和协议通知。
- Go 世界测试覆盖三类目标、DC/MC 与 Type 0/1、搜索/范围、Shock、零伤害、延迟
  目标取消和目标投影；认证 `net.Pipe` transcript 覆盖 bootstrap、KeepAlive barrier、
  `ObjectAttack`、300ms 延迟 `Struck/ObjectStruck/DamageIndicator/HealthChanged`
  包序和最终 HP。

Go 实现、测试和迁移矩阵已提交为
`4213640 feat(p5): complete CrazyManworm AI`；本批未修改任何 C# 文件。

本批新增并通过以下门禁：

- `go test ./cmd/crystal-server -run 'CrazyManworm' -count=1`
- `go test -race ./cmd/crystal-server -run 'CrazyManworm' -count=3 -timeout=300s`
- `go test ./cmd/crystal-server -count=1 -timeout=300s`
- `go test ./... -count=1 -timeout=600s`
- `go vet ./...`
- `go build ./...`
- Go 仓库提交前 `git diff --check` 与 tracked/staged/untracked `.cs` 零变化检查。

全包 `go test -race ./cmd/crystal-server -count=1 -timeout=600s` 本次重跑仍失败于
既有 session 夹具竞争：`TestGuildBuffSessionNewbieLoginReplacesStalePersistedBuff`
（装备 buff 共享状态）、`TestSessionKirinIceThrustTranscript`（未同步随机流）和
`TestSessionBlinkTranscriptIncludesDelayedMapChangeEffectAndBuff`（map-transition
buff ticker）。race 栈未进入 CrazyManworm；本批不将全包 race 记为通过。

## 本次完成的 P5 批次（MutatedManworm AI=65）

本批将 Legacy `MutatedManworm` 从已有 Player 目标子切片收口为继承
`CrazyManworm` 生命周期的完整三类目标与客户端可观察行为：

- AI=65 接入完整的目标搜索、`CanAttack`、相邻近战和移动流程；保留 Player、owned
  Monster、Hero 的 TargetKind，并覆盖三类目标的存活、归属、地图和距离重验。
- 攻击保留 `Random.Next(3)==0` 的 Type 1 MC 与其余 Type 0 DC，使用 Legacy
  inclusive `GetAttackPower`/Luck 边界、ACAgility 防御类型和 300ms 延迟结算；零伤害
  不推进 Shock/Action/Attack 状态，正伤害才推进冷却。
- `Attacked` 反应保留“受到伤害大于自身 DC 且 `Random.Next(2)==0`”门槛；按
  `FindAllTargets(ViewRange, CurrentLocation)` 的 Cell insertion order 查找三类目标中
  MinDC 更低的最后一个候选，保留目标类型并以 Effect=4 传送到目标反向一格，同时
  重新验证旧目标与新目标的可见性。
- Go 世界测试覆盖三类目标、DC/MC、搜索/范围、Shock、零伤害、延迟目标取消和重验；
  认证 `net.Pipe` transcript 覆盖 bootstrap、KeepAlive barrier、物理
  `ObjectAttack`、300ms 延迟 `Struck/ObjectStruck/DamageIndicator/HealthChanged`
  包序和最终 HP。

Go 实现、测试和迁移矩阵已提交为
`d5dc961 feat(p5): complete MutatedManworm AI`；本批未修改任何 C# 文件。

本批新增并通过以下门禁：

- `go test ./cmd/crystal-server -run 'MutatedManworm|CrazyManworm|Mandrill' -count=1 -timeout=300s`
- `go test -race ./cmd/crystal-server -run 'MutatedManworm|CrazyManworm' -count=3 -timeout=300s`
- `go test ./cmd/crystal-server -count=1 -timeout=300s`
- `go test ./... -count=1 -timeout=600s`
- `go vet ./...`
- `go build ./...`
- Go 仓库提交前 `git diff --check` 与 tracked/staged/untracked `.cs` 零变化检查。

全包 `go test -race ./cmd/crystal-server -count=1 -timeout=600s` 本次重跑仍失败于既有
session 夹具竞争：`TestGuildBuffSessionNewbieLoginReplacesStalePersistedBuff`（共享
装备 buff 状态）和 `TestSessionKirinIceThrustTranscript`（未同步随机流）。race 栈未进入
MutatedManworm；本批不将全包 race 记为通过。

## 本次完成的 P5 批次（RightGuard/LeftGuard AI=31/32）

本批实现 Legacy `RightGuard`（AI=31）与 `LeftGuard`（AI=32）的完整三类目标和
客户端可观察攻击行为：

- AI=31/32 接入 Go common population 与独立 AI 分支；搜索保留 Legacy
  `FindTarget` 的 Cell insertion order、`Target ??=` 保留语义、八格同地图范围和
  Player/owned Monster/Hero 的显式 TargetKind。
- `CanAttack` 逐项匹配基类时序：严格要求 `AttackTime`/`ActionTime` 已到期但不把
  `ShockTime` 当作攻击门禁；近身在同格或相邻时发送 `ObjectAttack`，300ms 后按
  `MACAgility` 结算；非相邻或同格远程条件按 Legacy 发送 `ObjectRangeAttack`。
- RightGuard 的远程命中固定为 500ms；LeftGuard 增加 `50ms * ChebyshevDistance`，
  两者都用 MAC 且不做 Agility 命中检查；DC 使用 inclusive `GetAttackPower`/Luck
  边界，延迟 impact 重新验证存活、地图、归属和目标类型。
- 世界测试覆盖三类目标、搜索、野生怪物 Rage 门禁、Shock 边界、距离延迟、MAC 无
  Agility 和延迟目标取消；认证 `net.Pipe` transcript 覆盖 bootstrap、远程攻击、
  冷却期移动、`Struck/ObjectStruck/DamageIndicator/HealthChanged` 包序和最终 HP。

Go 实现、测试和矩阵已提交为
`9d15529 feat(p5): complete RightGuard and LeftGuard AI`；本批未修改任何 C# 文件。

本批新增并通过：

- `go test ./cmd/crystal-server -run 'RightLeftGuard' -count=1 -timeout=300s`
- `go test ./cmd/crystal-server -run 'RightLeftGuard|HedgeKekTal|Mandrill|CrazyManworm|MutatedManworm' -count=1 -timeout=300s`
- `go test -race ./cmd/crystal-server -run 'RightLeftGuard' -count=3 -timeout=300s`
- `go test ./... -count=1 -timeout=600s`
- `go vet ./...`
- `go build ./...`

完整 `go test -race ./cmd/crystal-server -count=1 -timeout=900s` 仍未全绿：重现
既有 `TestGuildBuffSessionNewbieLoginReplacesStalePersistedBuff` 与
`TestSessionKirinIceThrustTranscript` 数据竞争，并在并发调度下出现既有
`TestSessionOmaMageRangeSlowFrozenTranscript` 的随机边界断言；失败栈未进入
AI=31/32。OmaMage 单次 race 可通过但 `-count=5` 重现，普通测试通过，故保留为
既有全包 race 阻塞项，不修改本批生产代码。

## 本次完成的 P5 批次（MinotaurKing AI=33）

本批实现 Legacy `MinotaurKing` 的完整三类目标和客户端可观察范围攻击行为：

- AI=33 接入 Go common population 与独立 AI 分支；继承 RightGuard 的目标投影和
  搜索语义，但使用六格同地图攻击范围，并严格在 `CanAttack` 通过后才执行范围判断
  和移动，因此攻击冷却期间保留目标且不移动。
- 近身路径发送 `ObjectAttack`，300ms 后按 Legacy MAC+Agility 结算；远程路径发送
  `ObjectRangeAttack`，固定 500ms 后按 MAC 且不做 Agility 命中检查，攻击冷却额外
  保留 500ms 的 RightGuard 投射节奏。
- 延迟命中时重新验证当前锚点仍可攻击、仍在地图上；随后以锚点的当前位置执行
  Legacy `FindAllTargets(3, ...)` 方形扫描，保留 Cell insertion order，并投影
  Player/owned Monster/Hero。初始六格距离只在攻击创建时检查，锚点后来移远不会
  自动取消，死亡/不可攻击才会取消整次范围命中。
- 世界测试覆盖三类目标、近身/远程字段和时序、命中时加入范围目标、死亡锚点取消、
  冷却期间不移动；认证 `net.Pipe` transcript 覆盖 bootstrap、`ObjectRangeAttack`、
  延迟 `Struck/ObjectStruck/DamageIndicator/HealthChanged` 包序和最终 HP。

Go 实现、测试和矩阵已提交为
`f04430f feat(p5): complete MinotaurKing AI`；本批未修改任何 C# 文件。

本批新增并通过：

- `go test ./cmd/crystal-server -run 'TestGameWorldMinotaurKing|TestSessionMinotaurKing' -count=1`
- `go test -race ./cmd/crystal-server -run 'MinotaurKing' -count=3 -timeout=300s`
- `go test ./cmd/crystal-server -count=1 -timeout=600s`
- `go test ./... -count=1 -timeout=900s`
- `go vet ./...`
- `go build ./...`

完整 `go test -race ./cmd/crystal-server -count=1 -timeout=600s` 仍未全绿：本次实际
命中既有 `TestGuildBuffSessionNewbieLoginReplacesStalePersistedBuff`、
`TestSessionKirinIceThrustTranscript` 与
`TestSessionBlinkTranscriptIncludesDelayedMapChangeEffectAndBuff` 的数据竞争；race
栈分别落在装备 buff reconciliation、Kirin 随机回调和 Blink map-transition buff
ticker，未进入 AI=33 生产代码或会话夹具。该全包 race 继续作为既有门禁问题记录，不
修改本批生产代码；具体复盘已写入 `tasks/lessons.md`。

## 本次完成的 P5 批次（FrostTiger AI=34）

本批实现 Legacy `FrostTiger` 的坐下/唤醒、反击锁定和近远程攻击全路径：

- AI=34 接入 Go common population 与独立 AI 分支；`FindTarget()` 保持空实现，只有真实
  `Attacked(...)` 入口把 Player 或 Monster 写入目标，避免无目标自主搜索改变 Legacy 行为。
- 保留构造后的 `Random.Next(120000)` 两分钟坐下计时，以及目标处理时不倒退的
  `NewSitDownTime` 刷新。坐下时同时设置 `Hidden`，广播精确的 `ObjectSitDown` 字段；
  `ObjectMonster` 投影同步反映 `Hidden=true` 与 `Extra=true`，目标出现后先唤醒再处理攻击。
- 保留六格同地图范围、同格/远程分支、300ms 近身 `ObjectAttack` 与 MAC+Agility 命中，
  以及固定 500ms 远程 `ObjectRangeAttack`、MAC 不含 Agility 的延迟命中和额外
  `AttackSpeed` 冷却。Player/owned Monster/Hero 目标投影与延迟目标重验均覆盖。
- 远程攻击 admission 时保留 Effect=0 的五秒 Bleeding 或 Effect=1 的五秒 Slow，均为
  一秒 tick、零 value，并按 PoisonResist 与 `Random.Next(8)==0` 顺序执行；协议新增
  `ObjectSitDown` payload/ordinal 固定向量。
- 目标锁定接入普通近战、Warrior/技能/远程命中共用入口，以及 Monster 延迟动作入口，
  使空 `FindTarget()` 的 FrostTiger 在真实会话中仍能被攻击后反击。

Go 实现、测试和矩阵已提交为
`e9351b4 feat(p5): complete FrostTiger AI`；本批未修改任何 C# 文件。

本批新增并通过：

- `go test ./internal/protocol ./cmd/crystal-server -run 'ObjectSitDown|PacketIDsMatchLegacyEnums|FrostTiger|RightLeftGuard|MinotaurKing' -count=1 -timeout=300s`
- `go test ./cmd/crystal-server -count=1 -timeout=600s`
- `go test ./... -count=1 -timeout=900s`
- `go vet ./...`
- `go build ./...`
- `go test -race ./cmd/crystal-server -run 'FrostTiger' -count=3 -timeout=300s`

完整 `go test -race ./... -count=1 -timeout=900s` 仍未全绿：本次实际重现
`TestGuildBuffSessionNewbieLoginReplacesStalePersistedBuff`、
`TestSessionKirinIceThrustTranscript` 与
`TestSessionBlinkTranscriptIncludesDelayedMapChangeEffectAndBuff` 的既有数据竞争；
race 栈落在装备 buff reconciliation、Kirin transcript 随机回调和 Blink map-transition
buff ticker，未进入 AI=34 生产代码或会话夹具。该结果已记录到 `tasks/lessons.md`，不因
本批实现改动非相关生产路径。

## 本次完成的 P5 批次（Yimoogi AI=36）

本批实现 Legacy `Yimoogi` 的成对生命周期、目标同步和完整攻击/传送行为：

- AI=36 接入 Go common population 与独立 AI 分支；保留构造后的 dormant 状态、四秒后
  的 `ObjectAttack Type=2` 姐妹生成、前方点位优先/当前位置回退、子体两秒 action lock
  和 parent/sister 的目标同步。真实受击入口会在伤害解析前唤醒整对实例。
- 保留 Legacy 的 parity-shaped 两格近战几何、同格/非近战七格攻击选择、300ms
  MAC+Agility 近战命中和 500ms MAC-only 远程命中；远程 admission 保留 `ObjectRangeAttack`
  与 `AttackSpeed+500ms` 冷却。
- 远程四格内保留 `ObjectAttack Type=1` 的即时 Red poison；固定范围随机消耗覆盖 DC/SC
  的 inclusive 边界、单位 `Next(1)`、Player/Hero 两阶段抗性和 Monster 单阶段抗性。
- 低血量 parent 保留 40 次随机传送尝试、effect=1 的出入场可见性包，并在传送前位置生成
  两个 `WhiteSnake`；成对死亡掉落只允许最后死亡的一方产出共享掉落。
- Go 实现、确定性世界测试、三类目标投影测试、毒效随机顺序测试和认证 `net.Pipe`
  ranged transcript 已提交为 `7df022c feat(p5): complete Yimoogi AI`；本批未修改任何
  C# 文件。

本批新增并通过：

- `go test ./cmd/crystal-server -run 'Yimoogi' -count=1`
- `go test -race ./cmd/crystal-server -run 'Yimoogi' -count=3 -timeout=600s`
- `go test ./...`
- `go vet ./...`
- `go build ./...`

完整 `go test -race ./... -count=1 -timeout=900s` 仍未全绿：本次实际命中
`TestSessionDarkBodySpawnAndRecallTranscript`、
`TestGuildBuffSessionNewbieLoginReplacesStalePersistedBuff`、
`TestSessionKirinIceThrustTranscript` 与
`TestSessionBlinkTranscriptIncludesDelayedMapChangeEffectAndBuff` 的既有数据竞争；race
栈落在装备 buff reconciliation/读取、Kirin 会话 ticker 随机回调和 Blink map-transition
buff ticker，未进入 AI=36 生产代码或 Yimoogi 会话夹具。该全包 race 继续作为既有门禁问题
记录，不修改本批生产代码；具体复盘已写入 `tasks/lessons.md`。

## 本次完成的 P5 批次（CrystalSpider AI=37 / HolyDeva AI=38）

本批完成 Legacy `CrystalSpider` 与 `HolyDeva` 的野生怪物、普通宠物和协议可观察行为：

- AI=37 `CrystalSpider` 接入 common population，保留构造时随机方向、继承目标搜索/保留/
  移动生命周期和三格 parity-shaped 几何。相邻目标使用 `ObjectAttack Type=0`、300ms
  action 加 `AttackSpeed` 冷却；远程使用 `ObjectAttack Type=1`，逐格按 Cell insertion
  order 选择首个 Player/owned Monster，保留 MagicResist admission、每格 50ms 加 300ms
  延迟和 MAC+Agility 命中。成功命中后按 SC inclusive 值、PoisonResist 与
  `Random.Next(8)==0` 施加五秒 Green poison、两秒 tick，并覆盖 Player/Monster/Hero
  目标重验和投影。
- AI=38 `HolyDeva` 接入 common population 与 ordinary-pet runtime，保留构造时
  DownLeft 方向 5、Summoned/`ObjectMonster.Extra`、图片 117 和召唤时捕获 owner
  direction。野生目标使用含边界的六格范围、五秒 Fear/retreat 门禁；攻击广播
  `ObjectRangeAttack`（Type 0 默认），300ms action 加 `AttackSpeed` 冷却，固定 500ms
  后以 MAC-only 方式结算 Player/owned Monster/Hero 目标。普通宠物在超出范围时按
  PetMode 移动或保持攻击距离。
- 召唤入口、对象 materialization、普通宠物对象包、延迟 action resolver 和 migration
  matrix 均已接线；新增确定性世界测试覆盖构造/目标/几何/抗性/毒物/延迟命中/宠物范围
  攻击，认证 `SummonHolyDeva` transcript 同时确认 AI=38、图片 117 和方向 2 的 owner
  direction capture。

Go 实现、测试和矩阵已提交为
`f5dec14 feat(p5): complete CrystalSpider and HolyDeva AI`；本批未修改任何 C# 文件。

本批新增并通过：

- `go test ./cmd/crystal-server -run 'CrystalSpider|HolyDeva|SessionSummonShinsuAndHolyDevaTranscript' -count=1`
- `go test -race ./cmd/crystal-server -run 'CrystalSpider|HolyDeva|SummonShinsuAndHolyDeva' -count=3 -timeout=600s`
- `go vet ./...`
- `go build ./...`

本批收尾的全仓普通测试仍只命中既有
`TestSessionOmaMageRangeSlowFrozenTranscript`：会话维护循环在真实时钟下先消费
`monsterAIRollLocked(2)`，因此得到 `[2 1]` 而非夹具只针对手工 tick 期望的 `[1]`；调用栈
位于 `serveWithConfig -> processOmaMageLocked -> monsterAIMoveToLocked`，未进入
CrystalSpider/HolyDeva。完整 race 摘要仍命中既有 GuildBuff、Kirin 和 OmaMage 会话/共享
状态竞争，AI=37/38 定向 race 没有失败；上述分类和复现证据已写入 `tasks/lessons.md`，
不扩大本批范围修复无关生产代码。

## 本次完成的 P5 批次（YinDevilNode AI=41/42）

本批完成 Legacy `YinDevilNode` 的 AI=41/42 双变体：

- 两个 AI 均接入 common population，保留不可移动、不可转向、无漫游、七格
  `FindFriendsNearby` 门禁和自身排除；成功 admission 广播默认 `ObjectAttack`，写入
  300ms action lock、`AttackSpeed` cooldown，并在 500ms 延迟 action 中重新扫描对象。
- 命中阶段按 Legacy `FindAllTargets` 的 Cell/ring 顺序、`IsAttackTarget`、Hidden/CoolEye
  门禁和 `IsFriendlyTarget` 顺序执行；不把 Player/Hero/owned Monster 的协议可见性或
  普通 AI 目标规则错误扩大为节点友方。
- AI=41 使用目标等级 `Level/7+4` 的五秒隐形 `BlessedArmour` MaxAC；AI=42 使用相同值
  的五秒隐形 `UltimateEnhancer` MaxDC。Player/Monster/Hero 的可表达运行态均复用现有
  Buff 刷新与 `OperateTime=0` 语义，Monster/Hero 的 value-map/owner 状态按权威副本写回。
- 认证 transcript 同时覆盖 AI=41/42 的 ObjectAttack、延迟重扫和 `RageTime` 引起的
  `ObjectColourChanged` 视觉副作用；世界测试覆盖普通两阶段门禁和自身不算友方的边界。

Go 实现、测试和矩阵已提交为
`316583a feat(p5): complete YinDevilNode AI`；本批未修改任何 C# 文件。

本批新增并通过：

- `go test ./cmd/crystal-server -run 'YinDevilNode' -count=3 -timeout=180s`
- `go test -race ./cmd/crystal-server -run 'YinDevilNode' -count=5 -timeout=600s`
- `go test ./... -count=1 -timeout=900s`
- `go vet ./...`
- `go build ./...`

完整 `go test -race ./... -count=1 -timeout=900s` 仍命中既有
`TestGuildBuffSessionNewbieLoginReplacesStalePersistedBuff`、
`TestSessionKirinIceThrustTranscript` 与
`TestSessionOmaMageRangeSlowFrozenTranscript`；摘要和 lessons 已按实际 race 栈归因，
未出现 YinDevilNode 生产文件或测试栈，不修改无关生产代码。

## 本次完成的 P5 批次（OmaKing AI=43）

本批完成 Legacy `OmaKing` 的继承目标、近身/远程攻击和延迟结算：

- AI=43 接入 common population，保留构造时随机方向、继承 `FindTarget` 的目标保留/Cell
  顺序和 7 格 inclusive ranged gate；目标在 action/attack cooldown 期间仍按 Legacy
  `ProcessTarget` 继续移动。
- 近身 parity-shaped 两格分支按 `Next(3)>0` 选择，广播 `ObjectAttack Type=0`，先对一格
  范围执行 level-gap 与 MagicResist 门禁，再执行 3–5 格 push、八分之一 Paralysis
  admission，最后使用两格 `LineAttack` 生成逐格 50ms 加 300ms 的 ACAgility 延迟动作。
- 远程/同格分支广播 `ObjectAttack Type=1`，使用 inclusive Luck/DC/MC power，固定 500ms
  命中并追加 `AttackSpeed+500ms` 冷却；`CompleteAttack` 忽略原始 action 目标，按当前
  OmaKing 位置重新扫描七格 Player/owned Monster/Hero，并用 MAC 或 ACAgility 结算每个
  当前可攻击对象。
- Go 专用目标 gate 保留 Legacy wild-Monster 对 Player safe-zone/NoFight 的调用顺序，
  同时覆盖隐藏目标、目标重验、value-map Monster 写回、push 包序和会话 cooldown
  `ObjectWalk` 副作用。

Go 实现、测试和矩阵已提交为
`bd62ffa feat(p5): complete OmaKing AI`；本批未修改任何 C# 文件。

本批新增并通过：

- `go test ./cmd/crystal-server -run '^(TestGameWorldOmaKing|TestSessionOmaKingRangedAreaTranscript)$' -count=3 -timeout=240s`
- `go test -race ./cmd/crystal-server -run 'OmaKing' -count=5 -timeout=600s`
- `go test ./... -count=1 -timeout=900s`
- `go vet ./...`
- `go build ./...`

完整 `go test -race ./... -count=1 -timeout=900s` 本批实际命中
`TestSessionDarkBodySpawnAndRecallTranscript`、
`TestGuildBuffSessionNewbieLoginReplacesStalePersistedBuff` 与
`TestSessionKirinIceThrustTranscript`；race 栈属于既有智能生物、GuildBuff/装备共享状态
和 Kirin ticker/随机回调，未出现 OmaKing 文件或测试栈，具体分类已写入
`tasks/lessons.md`。

## 当前质量门禁

AI=36 本批新增并通过以上门禁；历史批次的累计门禁记录仍保留如下：

- `go test ./cmd/crystal-server -run 'ToxicGhoul' -count=1 -timeout=120s`
- `go test -race ./cmd/crystal-server -run 'ToxicGhoul' -count=5 -timeout=600s`
- `go test ./cmd/crystal-server -run 'Harvest|CaveMaggot|ShamanZombie|Khazard' -count=1 -timeout=180s`
- `go test ./cmd/crystal-server -count=1 -timeout=600s`
- `go test ./... -count=1 -timeout=600s`
- `go vet ./...`
- `go build ./...`
- Go 仓库提交前 `git diff --check` 与 tracked/staged/untracked `.cs` 零变化检查通过。

此前 Go HEAD `becac1db` 对应的历史源码已通过以下收尾门禁；DarkDevil follow-up 为前置提交
`433c1e9`，IncarnatedGhoul/IncarnatedZT/BoneFamiliar 为前置提交
`a0a2c2f`/`0c3ca86`/`f8e2a2c`：

- `go test ./... -count=1 -timeout=600s`（本次收尾重新运行并明确取得 `exit_code=0`）
- `go test ./cmd/crystal-server -run 'Tree' -count=1 -timeout=120s`
- `go test -race ./cmd/crystal-server -run 'Tree' -count=5 -timeout=600s`
- `go test -race ./cmd/crystal-server -run 'ArcherSummon|SummonSnakes' -count=1 -timeout=5m`
- `go test -race ./cmd/crystal-server -run 'CaveMaggot' -count=1 -timeout=5m`
- `go test -race ./cmd/crystal-server -run 'CannibalPlant' -count=1 -timeout=5m`
- `go test ./cmd/crystal-server -run 'CannibalPlant|CreeperPlant|WaterDragon|EvilCentipede' -count=1`
- `go test -race ./cmd/crystal-server -run 'EvilCentipede' -count=5 -timeout=600s`
- `go test ./cmd/crystal-server -run 'BugBagAndRootSpider|RootSpiderStops|RootSpiderChild|BombSpiderContact|SessionRootSpiderBombSpider' -count=1 -timeout=600s`
- `go test -race ./cmd/crystal-server -run 'RootSpider|BombSpider|BugBag' -count=5 -timeout=600s`
- `go test ./cmd/crystal-server -run 'WoomaTaurus' -count=1 -timeout=600s`
- `go test -race ./cmd/crystal-server -run 'WoomaTaurus|FlamingWooma' -count=5 -timeout=600s`
- `go test ./cmd/crystal-server -run 'RedMoonEvil' -count=1 -timeout=600s`
- `go test -race ./cmd/crystal-server -run 'RedMoonEvil|WoomaTaurus|FlamingWooma' -count=5 -timeout=600s`
- `go test ./cmd/crystal-server -run 'ZumaMonster|TestGameWorldBasicMonsterAI' -count=1 -timeout=600s`
- `go test ./cmd/crystal-server -run 'RedThunderZuma|ZumaMonster' -count=1 -timeout=180s -v`
- `go test -race ./cmd/crystal-server -run 'RedThunderZuma|ZumaMonster|WoomaTaurus|RedMoonEvil|FlamingWooma' -count=5 -timeout=600s`
- `go test -race ./cmd/crystal-server -run 'ZumaMonster|WoomaTaurus|RedMoonEvil|FlamingWooma' -count=5 -timeout=600s`
- `go test ./cmd/crystal-server -run 'ZumaMonster|RedThunderZuma|ZumaTaurus|TestGameWorldBasicMonsterAI|FurbolgCommander' -count=1 -timeout=600s`
- `go test -race ./cmd/crystal-server -run 'ZumaTaurus|RedThunderZuma|ZumaMonster|WoomaTaurus|RedMoonEvil|FlamingWooma' -count=5 -timeout=600s`
- `go test ./cmd/crystal-server -run 'Shinsu|SummonShinsu|SepHighTaoist' -count=1 -timeout=600s`
- `go test -race ./cmd/crystal-server -run 'Shinsu|SummonShinsu|SepHighTaoist' -count=5 -timeout=600s`
- `go test ./cmd/crystal-server -run 'KingScorpion|kingScorpion' -count=1 -timeout=600s`
- `go test -race ./cmd/crystal-server -run 'KingScorpion|kingScorpion' -count=5 -timeout=600s`
- `go test ./cmd/crystal-server -run 'IncarnatedGhoul|incarnatedGhoul' -count=1 -timeout=600s`
- `go test -race ./cmd/crystal-server -run 'IncarnatedGhoul|incarnatedGhoul' -count=3 -timeout=600s`
- `go test ./cmd/crystal-server -run 'IncarnatedZT|incarnatedZT' -count=1 -timeout=600s`
- `go test -race ./cmd/crystal-server -run 'IncarnatedZT|incarnatedZT' -count=3 -timeout=600s`
- `go test ./cmd/crystal-server -run 'DigOutZombie|digOutZombie|Armadillo' -count=1 -timeout=600s`
- `go test -race ./cmd/crystal-server -run 'DigOutZombie|digOutZombie|Armadillo' -count=3 -timeout=600s`
- `go test ./cmd/crystal-server -run 'BoneFamiliar|SummonSkeleton|summonSkeleton' -count=1 -timeout=600s`
- `go test ./cmd/crystal-server -run 'RevivingZombie' -count=1 -timeout=600s`
- `go test -race ./cmd/crystal-server -run 'RevivingZombie' -count=5 -timeout=600s`
- `go test ./cmd/crystal-server -run 'ShamanZombie' -count=1 -timeout=600s`
- `go test -race ./cmd/crystal-server -run 'ShamanZombie' -count=5 -timeout=600s`
- `go test ./cmd/crystal-server -run 'Khazard' -count=1 -timeout=600s`
- `go test -race ./cmd/crystal-server -run 'Khazard' -count=5 -timeout=600s`
- `go test -race ./cmd/crystal-server -run 'BoneFamiliar|SummonSkeleton|BasicMonsterAI|OrdinaryPet|Shinsu' -count=3 -timeout=600s`
- `go test -race ./cmd/crystal-server -run 'IncarnatedGhoul|IncarnatedZT|ZumaMonster|zumaMonster|RedThunderZuma|ZumaTaurus' -count=3 -timeout=600s`
- `go test ./... -count=1 -timeout=600s`
- `go vet ./...`
- `go build ./...`
- `go test ./cmd/crystal-server -run '^TestSessionFurbolgCommanderRangedTranscript$' -count=5 -timeout=120s`
- `go test ./internal/worlddata ./internal/legacyworld -count=1 -timeout=120s`
- `go vet ./...`
- `go build ./...`
- ArcherSummon/TrapHexagon 会话组合重复 3 次通过，含 net.Pipe keep-alive barrier 回归
- CannibalPlant/CreeperPlant 领域回归与 CannibalPlant `net.Pipe` 会话 transcript 通过
- Guard 领域/认证 session 测试与 `-race` 通过；FurbolgGuard session `-race -count=3` 通过
- Tao Guard route/目标门禁与认证 session 测试通过；Guard/Tao Guard session `-race -count=5` 通过
- Harvest 协议/世界/认证 session `-race -count=5` 通过，覆盖三次剥皮、缓存掉落、组门禁和持久化
- Deer 世界/认证 session 定向测试、`go test -race ./cmd/crystal-server -run 'Deer' -count=5` 通过，覆盖目标发现、直线/受阻逃跑、漫游和 `ObjectWalk` transcript
- `go test ./cmd/crystal-server -count=1 -timeout=600s` 通过
- `go test ./... -count=1 -timeout=600s` 在 WoomaTaurus 修复旧 AI=11 占位夹具后通过；包含 RootSpider/BombSpider/WoomaTaurus/RedMoonEvil 认证转录
- `go test ./... -count=1 -timeout=600s` 在 AI=17 批次最终重跑通过；期间收紧 FurbolgCommander 认证夹具，在 keep-alive barrier 后停止竞争 ticker 并直接调用 `processMonsterAILocked`，避免实时 MoveTo fallback 污染精确随机序列
- 两仓库 `git diff --check`
- 两仓库 tracked、staged、untracked `.cs` 零变化检查

全仓库 race 尚未作为本批门禁重跑；此前已知的共享 session Buff 读写竞争仍按
`tasks/lessons.md` 管理。本批定向 EvilCentipede race 连续 5 次通过。

文档交接不会修改 Go 源码。新 Session 开始时仍应先确认两个工作树干净；若环境或工具链变化，再选择与风险相称的门禁重跑。

## 建议的下一条迁移线

优先继续 P5，因为最近五个批次已经建立了稳定的魔法/Buff/延迟动作/状态生命周期基础设施：

1. 继续 P5 通用 monster AI，按 `docs/migration-matrix.md` 中下一个仍 pending
   的 AI/target sub-slice 排序，逐项确认可生成入口、隐藏/移动/目标门禁和攻击
   resolver；RootSpider/BugBag 已在本批完成。
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
