# Crystal Go 迁移 Session 交接

最后更新：2026-08-21（Asia/Singapore）

## 迁移目标与硬边界

目标是把当前 Crystal 项目的服务端功能与客户端可观察行为 100% 迁移到独立、跨平台的 Go 项目。服务端、测试客户端、协议探针、Legacy 数据导入/导出器和其他迁移工具全部使用 Go。

- 原 Crystal 仓库和 Go 迁移仓库中的所有 `.cs` 文件都是只读对照基线，禁止新增、修改、重命名或删除。
- 每个功能批次必须同时覆盖领域状态、协议序列、持久化/重载和真实会话路径；仅存在 handler 不算迁移完成。
- 可以共享运行时和测试矩阵的功能应成组迁移，但不能牺牲行为等价、竞态安全和回归覆盖。
- 每批完成后更新迁移矩阵、运行质量门禁并 Git 提交；整体迁移未达到 100% 前不得标记最终 Goal 完成。

## 仓库快照

| 仓库 | 路径 | 分支 | 当前基线 | 交接前状态 |
|---|---|---|---|---|
| Legacy Crystal | `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal` | `master` | `853de2d0 chore: compact and archive project lessons` | 本次 `agents.md`、lessons、archive 与 handoff 更新待独立文档提交 |
| Go migration | `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer` | `main` | `60a97c9 feat(p5): complete Trainer AI` | AI=52/53 为 `e29e1f9`，Dragon session 验证为 `71323ce`，AI=54 为 `f53167c`，AI=55 为 `b374fd5`，AI=56 为 `60a97c9`；工作树干净 |

新 Session 应以实际 `git status --short --branch` 和 `git log -1 --oneline` 为准。本文件提交后，Legacy 仓库 HEAD 会比表中的交接前基线多一个文档提交。

迁移状态的权威明细位于 Go 仓库的 `docs/migration-matrix.md`；`README.md` 是当前实现能力的长说明。

## 当前 Goal 状态

本线程当前已恢复以下完整 Goal，状态为 active；不要因为某个批次完成而把
整体 100% 迁移误标为完成。后续 Session 继续沿用该 Goal：

> 将当前 Crystal 项目按现有功能与客户端可观察行为 100% 迁移到独立 Git 管理的跨平台 Go 项目：服务端、测试客户端、协议探针、导入导出及迁移工具全部使用 Go；两个仓库中的所有 C# 文件仅作只读基线；按可安全合并的功能批次实现、测试、更新迁移矩阵并 Git 提交，持续推进直至功能矩阵全部完成。

## 阶段进度

迁移矩阵共有 P0–P12 十三个阶段。目前是 2 个 Complete、11 个 In progress、0 个 Pending。该计数只表示阶段状态，不等于准确的代码或功能完成百分比。

| 阶段 | 状态 | 当前摘要 |
|---|---|---|
| P0 协议帧/连接基础 | Complete | 帧编码、版本、断开、KeepAlive 和 Go 固定向量已完成。 |
| P1 配置与生命周期 | In progress | INI/环境配置、版本哈希、连接限制、超时、包速率和优雅关闭已有覆盖；完整重启/日志生命周期待闭环。 |
| P2 账户与密码 | In progress | 登录、账户创建、改密、StoragePassword、导入账户和 SelectInfo 已迁移；直接二进制写回及完整 NPC 访问仍待完成。 |
| P3 角色与 StartGame | In progress | 角色列表/创建/删除、运行时字段、有效/无效出生点、基础属性与登出持久化已有覆盖，尚未按完整客户端启动流程宣告完成。 |
| P4 地图/移动/可见性 | In progress | 多版本地图、碰撞/门、玩家/NPC/怪物可见性、地图切换、普通/私聊及聊天物品链接授权展开和多项地图门禁已迁移；完整 bootstrap 仍待完成。 |
| P5 战斗/技能/怪物/掉落 | In progress | 已完成核心近战、远程、PvP 基础、多个单体/区域魔法、九个自增益 Buff、FrostCrunch 状态、基础属性、掉落树和持久化；最近批次新增玩家 TrapHexagon、SummonVampire/SummonToad/SummonSnakes、CannibalPlant、Guard、Tao Guard、Deer AI=1/2、Tree AI=3、EvilCentipede AI=14、WoomaTaurus AI=11、RedMoonEvil AI=13、Shinsu AI=18、BugBagMaggot AI=12/RootSpider AI=39/BombSpider AI=40、RightGuard/LeftGuard AI=31/32、MinotaurKing AI=33、FrostTiger AI=34、ThunderElement AI=49 GreatFoxSpirit AI=50、Dragon/EvilMir AI=52/53、DragonStatue AI=54、HumanWizard AI=55 以及 Trainer AI=56 的 Legacy admission、目标捕获、延迟/生命周期、AI/伤害/逃跑、静态/Observer 可见性、HumanWizard owner-MP/owner-appearance/inspection/persistence 和 net.Pipe transcript；Trainer 以确定性 world production-entry tests 覆盖玩家/Hero/owned-pet、AC/MAC、AttackBonus、owner-root、miss、poison/DPS、切换/超时、Healing ChangeHP 和 value-map writeback；剩余技能/Buff、飞行/墙体规则、高级 PvP/组队战斗、其他通用/特殊怪物 AI、持久重生状态及完整包序仍待迁移。 |
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
- `149c60d feat(p5): complete TrapRock AI`
- `8eefcf2 feat(p5): complete ThunderElement and GreatFoxSpirit AI`
- `e29e1f9 Migrate Dragon EvilMir and EvilMirBody`
- `71323ce test(p5): verify Dragon session compatibility`
- `f53167c feat(p5): complete DragonStatue AI`
- `b374fd5 feat(p5): complete HumanWizard AI`
- `60a97c9 feat(p5): complete Trainer AI`

## 最近完成的 P5 批次

Go 提交 `e29e1f9`/`71323ce` 完成 Dragon、EvilMir 与 EvilMirBody 的生产行为和真实会话验证；`f53167c` 完成 AI=54 `DragonStatue`，`b374fd5` 完成 AI=55 `HumanWizard`，`60a97c9` 完成 AI=56 `Trainer`：

- DragonStatue 保留 common population、出生/respawn 方向钳制、永久静止但继承 push/blocking，以及 Player/owned-Monster/Hero/Hallucination 目标搜索；
- DragonStatue 锁定 300ms 动作、500ms live-target 半径二 MAC 延迟攻击、逐目标 DC/Luck、Shock-after-queue、睡眠后已排队命中、Struck 无伤害与 SpellObject poison follow-ups；
- DragonStatue 的睡眠目标仍可见且可被选中，但致死只进入严格 15 分钟睡眠，不发送 `ObjectDied`，不掉落、不清理、不减少 respawn population，restart 后不持久化睡眠；
- HumanWizard 注册 common/ordinary-pet population，保留固定 Down 方向、六格 ThunderBolt、Fear/撤退、主人跟随、Player/Monster/Hero 投影与 300ms/500ms 延迟边界；
- HumanWizard 的伤害、毒伤和治疗通过 `ChangeHP` 转移给主人 MP；主人每秒扣 10 MP，耗尽后宠物以 `ObjectDied(Type=1)` 清理；owner class/gender/hair/light/weapon/armour/wing 外观通过 `ObjectPlayer` 投影，首次 Spawned 的 `Extra=false`；
- HumanWizard 的 Mirroring、inspection owner resolution、logout/relogin persistence 及真实认证 `net.Pipe` spawn/logout/relogin transcript 已覆盖。
- Trainer 保留 common population 但静态禁止移动、攻击和 roam；玩家、Hero、owned pet、延迟 ranged/magic 及 queued owned-monster 命中均走独立 AC/MAC 边界，命中不改变 HP/death/loot/普通 struck-health 副作用；Trainer chat、AttackBonus、miss、毒伤/DPS、owner-root、攻击者切换和严格 `>5s` 平均输出均由确定性 world tests 锁定。Hero 使用自身 ObjectID/AttackBonus 且不把 Trainer chat 泄露给 owner；链式 owned monster 可解析到根玩家；Healing 通过真实 `resolveMassHealingActionLocked` 的 `ChangeHP` 统计路径验证，未生成满血 Trainer 的伪 regen。

本批 Go 提交后工作树干净，未修改任何 `.cs` 文件。主 Agent 使用 `gpt-5.6-sol/high`，只读 review subagents 使用 `gpt-5.6-luna/max`；该模型拆分已写入 `agents.md`。

## 当前质量门禁

AI=55 HumanWizard/Mirroring 与 AI=56 Trainer 批次通过：

- `go test ./cmd/crystal-server -run 'Trainer|HumanWizard' -count=5 -timeout=300s`
- `go test -race ./cmd/crystal-server -run 'Trainer|HumanWizard' -count=5 -timeout=300s`
- `go test ./cmd/crystal-server -count=1 -timeout=600s`（本次通过；`TestSessionOmaMageRangeSlowFrozenTranscript` 单独 `-count=10` 亦通过，历史随机边界失败仍保留为基线证据）
- `go test ./... -count=1 -timeout=900s -skip '^TestSessionOmaMageRangeSlowFrozenTranscript$'`
- `go test -race ./... -count=1 -timeout=900s -skip '^TestSessionOmaMageRangeSlowFrozenTranscript$'`（仅既有 `TestGuildBuffSessionNewbieLoginReplacesStalePersistedBuff` 及共享 session fixture race；栈位于 `player_spell_buffs.go`/`intelligent_creature_items.go`，未进入 Trainer）
- `go vet ./...`
- `go build ./...`
- `gofmt`、`git diff --check` 与两仓 tracked/staged/untracked `.cs` 零变化门禁

无排除项的普通全仓测试本次通过；历史 `TestSessionOmaMageRangeSlowFrozenTranscript` 随机边界 `[2 1]`/期望 `[1]` 失败证据仍保留，不能据此宣告该测试已稳定。完整 race 的失败仍只在上述既有共享 session 路径，未进入 AI=56 代码；未修改无关模块掩盖失败。

## 建议的下一条迁移线

AI=56 已完成；下一条仍优先继续 P5，因为最近批次已经建立了稳定的魔法/Buff/延迟动作/状态生命周期基础设施：

1. 继续 P5 通用 monster AI，按 `docs/migration-matrix.md` 中下一个仍 pending
   的 AI/target sub-slice 排序；AI=55 之后逐项确认可生成入口、隐藏/移动/目标门禁和攻击
   resolver；RootSpider/BugBag 已在此前批次完成。
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
