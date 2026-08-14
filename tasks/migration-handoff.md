# Crystal Go 迁移 Session 交接

最后更新：2026-08-14（Asia/Singapore）

## 迁移目标与硬边界

目标是把当前 Crystal 项目的服务端功能与客户端可观察行为 100% 迁移到独立、跨平台的 Go 项目。服务端、测试客户端、协议探针、Legacy 数据导入/导出器和其他迁移工具全部使用 Go。

- 原 Crystal 仓库和 Go 迁移仓库中的所有 `.cs` 文件都是只读对照基线，禁止新增、修改、重命名或删除。
- 每个功能批次必须同时覆盖领域状态、协议序列、持久化/重载和真实会话路径；仅存在 handler 不算迁移完成。
- 可以共享运行时和测试矩阵的功能应成组迁移，但不能牺牲行为等价、竞态安全和回归覆盖。
- 每批完成后更新迁移矩阵、运行质量门禁并 Git 提交；整体迁移未达到 100% 前不得标记最终 Goal 完成。

## 仓库快照

| 仓库 | 路径 | 分支 | 当前基线 | 交接前状态 |
|---|---|---|---|---|
| Legacy Crystal | `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal` | `master` | `a0a3a0b8`，随后增加本交接文档提交 | 干净，较 `origin/master` ahead 84（交接提交前） |
| Go migration | `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer` | `main` | `9084e6c358da8ca3e076ae9e28297ca26abd7681` | 干净 |

新 Session 应以实际 `git status --short --branch` 和 `git log -1 --oneline` 为准。本文件提交后，Legacy 仓库 HEAD 会比表中的交接前基线多一个文档提交。

迁移状态的权威明细位于 Go 仓库的 `docs/migration-matrix.md`；`README.md` 是当前实现能力的长说明。

## 当前 Goal 状态

本线程的 Goal 记录被误操作为 `objective: resu`、`status: paused`，现有接口无法恢复或替换这个未完成记录。不要为了清理记录而把 100% 迁移误标为完成。切换到新 Session 后，应新建以下完整 Goal：

> 将当前 Crystal 项目按现有功能与客户端可观察行为 100% 迁移到独立 Git 管理的跨平台 Go 项目：服务端、测试客户端、协议探针、导入导出及迁移工具全部使用 Go；两个仓库中的所有 C# 文件仅作只读基线；按可安全合并的功能批次实现、测试、更新迁移矩阵并 Git 提交，持续推进直至功能矩阵全部完成。

## 阶段进度

迁移矩阵共有 P0–P12 十三个阶段。目前是 2 个 Complete、10 个 In progress、1 个 Pending。该计数只表示阶段状态，不等于准确的代码或功能完成百分比。

| 阶段 | 状态 | 当前摘要 |
|---|---|---|
| P0 协议帧/连接基础 | Complete | 帧编码、版本、断开、KeepAlive 和 Go 固定向量已完成。 |
| P1 配置与生命周期 | In progress | INI/环境配置、版本哈希、连接限制、超时、包速率和优雅关闭已有覆盖；完整重启/日志生命周期待闭环。 |
| P2 账户与密码 | In progress | 登录、账户创建、改密、StoragePassword、导入账户和 SelectInfo 已迁移；直接二进制写回及完整 NPC 访问仍待完成。 |
| P3 角色与 StartGame | In progress | 角色列表/创建/删除、运行时字段、有效/无效出生点、基础属性与登出持久化已有覆盖，尚未按完整客户端启动流程宣告完成。 |
| P4 地图/移动/可见性 | In progress | 多版本地图、碰撞/门、玩家/NPC/怪物可见性、地图切换、聊天和多项地图门禁已迁移；完整 bootstrap 仍待完成。 |
| P5 战斗/技能/怪物/掉落 | In progress | 已完成核心近战、远程、PvP 基础、多个单体/区域魔法、九个自增益 Buff、FrostCrunch 状态、基础属性、掉落树和持久化；剩余技能/Buff、飞行/墙体规则、高级 PvP/组队战斗、通用怪物 AI、持久重生状态及完整包序仍待迁移。 |
| P6 物品/装备/维修/强化/制作 | In progress | 背包、装备、Storage、Trade、Repair、Refine、Craft 和基础 Use/Delete/Drop/Pickup 已有完整功能簇；尚未对整个 P6 做 100% 等价收口。 |
| P7 NPC/商店/任务/脚本 | In progress | NPC 可见性、传送、核心脚本动作/控制流、商店/BuyBack、Quest 生命周期及回调已迁移；剩余脚本/商店动作和完整包序待完成。 |
| P8 Group/Hero/Pet/Mount/Social | Complete | Group、Hero、普通战斗宠物、Mount、好友/黑名单、婚姻和导师体系已完成并有领域、协议、会话及持久化证据。 |
| P9 Guild/War/Territory/Conquest | In progress | 公会核心、仓库、进度/Buff、战争、领地和完整 Conquest runtime/NPC/assets 子簇已迁移；P9 其余范围尚未统一收口。 |
| P10 Mail/Market/Auction/Rental/GameShop | In progress | 五个主要经济功能簇均已有协议、事务、在线通知、持久化和竞态测试，但阶段仍未宣告完整。 |
| P11 Fishing/Awakening/Ranking/Intelligent Creature/Misc | In progress | Fishing、Awakening、Ranking、Intelligent Creature 功能簇已迁移；其他 miscellaneous 系统待补。 |
| P12 恢复/备份/部署 | Pending | 最终 restart equivalence、生产化 smoke test、备份与部署尚未开始。 |

## 已交付的重要能力

- 纯 Go 迁移工具：`crystal-legacy-account-export`、`crystal-legacy-world-export`、`crystal-protocol-probe`。保留的 C# 工具仅作历史基线。
- Go 服务端已贯通账户/角色、地图与可见性、NPC/Quest、物品与经济、Group/Hero/Pet/Mount/Social、Guild/Conquest、Mail/Market/Auction/Rental/GameShop、Fishing/Awakening/Ranking/Intelligent Creature 等大量功能簇。
- 协议测试使用 Legacy ordinal、精确 payload 和固定向量；关键跨玩家功能使用真实 `net.Pipe` 多会话接收者矩阵。
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
- `ff39426 feat(migration): add Go legacy account exporter`
- `cb4b899 feat(migration): add Go legacy world exporter`
- `296f1c1 feat(migration): add Go protocol probe`

## 最近完成的 P5 批次

提交 `9084e6c` 完成了可一起验收的三个即时自增益技能：

- `ProtectionField`：Buff 10，立即应用，持续 `45 + 15*level` 秒，Min/Max AC 使用 midpoint-to-even 取整。
- `Rage`：Buff 11，立即应用，持续 `18 + 6*level` 秒，Min/Max DC 使用 midpoint-to-even 取整。
- `SwiftFeet`：Buff 4、visible，立即应用，持续 `25 + 5*level` 秒，冷却 `210 - 40*level` 秒；激活期间 Run 三格，到期恢复两格。
- 已固定 owner 的 `HealthChanged -> UserLocation -> AddBuff -> Magic` 顺序，以及 observer 的 visible/private 接收者差异。
- 已保留 Legacy 登录怪癖：SwiftFeet Buff/可见快照恢复，但服务端三格能力不会在登录时恢复，必须重新施法。
- 生产实现、协议 ordinal、领域公式/到期/移动、双会话包序、JSON 持久化/重登和迁移文档均已提交。

不要重做这一批，除非新测试发现真实回归。

## 当前质量门禁

Go HEAD `9084e6c` 对应源码已通过：

- `go test ./cmd/crystal-server -count=1 -timeout=600s`
- `go test ./... -count=1 -timeout=600s`（本次收尾重新运行并明确取得 `exit_code=0`）
- `go test -race ./... -count=1 -timeout=900s`
- `go vet ./...`
- `go build ./...`
- 两仓库 `git diff --check`
- 两仓库 tracked、staged、untracked `.cs` 零变化检查

文档交接不会修改 Go 源码。新 Session 开始时仍应先确认两个工作树干净；若环境或工具链变化，再选择与风险相称的门禁重跑。

## 建议的下一条迁移线

优先继续 P5，因为最近四个批次已经建立了稳定的魔法/Buff/延迟动作/状态生命周期基础设施：

1. 先机械枚举 109-entry spell catalogue 与 `worldSpellBehaviorSupported` 的差集，并从 Legacy 施法入口、命中 resolver、Buff/Poison 创建点建立准确的 `spell -> effect -> side effects` 表。
2. 从差集中选择共享运行时、协议顺序和测试矩阵的一组技能/Buff 一次迁移；不要再按单功能碎片化推进，也不要仅凭技能名称推断效果。
3. 每批同时覆盖公式、MP/冷却、ActionTime/SpellTime retry、目标/地图/友方门禁、owner/observer 包序、到期/死亡/重登以及 auth/world/JSON 终态。
4. 之后再处理 P5 的 fly/wall validation、高级 PvP/Group combat、通用 monster AI、persistent respawn state 和完整 packet-order closure。

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
