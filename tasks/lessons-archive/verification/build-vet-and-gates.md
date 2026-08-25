### 2026-08-17 — AI=164 初次编译必须先核对桥接类型与 helper 签名

- Symptom: HornedArcher 首次包级编译同时报告 `ViewRange` 的 `byte` 到 `int32` 不匹配，以及 Monster friendly helper 的调用参数错误。
- Root cause: 新 AI 代码沿用了概念上的数值类型和相邻 AI 的 helper 形状，没有以当前 Go 结构定义逐项核对。
- Prevention: 新增 AI 先读取实际字段/helper 签名，完成最小 patch 后立即运行 `gofmt` 与 `go test ./cmd/crystal-server -run '^$'`。
- Verification: 将 `ViewRange` 显式转换为 `int32` 并修正 helper 调用后，包级编译通过，随后 AI=164 定向测试通过。

### 2026-08-15 — FlyingStatue 目标扫描的嵌套循环必须先通过包级编译

- Symptom: AI=136 首次 `gofmt`/包级编译在 `flying_statue.go` 约第 163 行报告缺少逗号和操作数，功能代码尚未进入行为测试。
- Root cause: 目标扫描补丁调整缩进时遗漏了 `for x`/`for y`/`for distance` 的闭合层级，且候选目标语句脱离了 `for x` 体。
- Prevention: 新增多层坐标扫描后立即用 `gofmt` 和 `go test <package> -run '^$'` 做语法/包级编译门禁，再开始编写行为断言；检查每层循环的缩进和闭合数。
- Verification: 补回循环闭合、格式化后 `go test ./cmd/crystal-server -run '^$'` 通过。

### 2026-08-15 — DigOut 发现探测不能复用隐藏状态攻击门禁

- Symptom: Armadillo/ArmadilloElder 的隐藏状态如果直接调用普通怪物目标门禁，`FindNearby(3)` 会把攻击者自身的 `Visible=false` 当成拒绝条件，导致附近玩家永远不能触发 `ObjectMonster`/`ObjectShow` reveal。
- Root cause: Legacy 的“发现附近目标”和“已显示后允许攻击/移动”是两个阶段；迁移时只复用了后者的 `IsAttackTarget` 投影，遗漏了发现阶段应忽略攻击者自身隐藏标志。
- Prevention: 对 DigOut 的 discovery probe 使用仅在探测期间将 `DigOutVisible` 投影为 true 的副本，保留玩家安全区、NoFight、死亡和目标隐藏/CoolEye/等级门禁；普通 AI 和真正攻击路径继续使用原始隐藏门禁。
- Verification: Armadillo world transcript 通过了初始隐藏、3 格发现和 `ObjectMonster -> ObjectShow` 顺序；真实 `net.Pipe` 登录 transcript 确认隐藏对象不在 bootstrap 中，手动 tick 后只收到 reveal 两包。

### 2026-08-13 — 特殊物品分支不能绕过通用使用门禁

- Symptom: Guild Skill Scroll 的 shape 10 分支最初直接进入公会逻辑，遗漏通用 `CanUseItem` 校验，也允许死亡角色使用。
- Root cause: 把特殊 shape 当成完整入口实现，只迁移了分支领域效果，没有先执行所有物品共享的角色状态和物品可用性门禁。
- Prevention: 每个特殊物品 dispatch 先列出并复用通用入口前置条件，再进入 shape 专属逻辑；至少覆盖死亡、无权限/不满足条件、定义缺失、成功消费和失败不消费。
- Verification: shape 10 现先调用 `canUseCharacterItem` 并拒绝死亡角色；无公会、非 leader、定义缺失、重复、成功消费等真实入口测试均通过。

### 2026-08-13 — 公会门禁必须以 auth 权威成员关系为准

- Symptom: 公会仓库 world 入口先看 session 中缓存的 `Character.GuildIndex`，会让已失效但尚未同步的公会投影走到安全区提示；Type 3 请求还可能在确认权威成员关系前消耗一次性列表状态。
- Root cause: 把连接局部投影当成了成员资格的授权源，并过早提交了 `GuildCanRequestItems=false` 副作用。
- Prevention: 公会授权先在 auth 的同一锁域内校验 guild 和 member，再检查安全区/权限；一次性状态只有在所有前置校验成功后才能消耗。world 快照只用于定位连接和投递包，不决定权威成员资格。
- Verification: 新增 stale world GuildIndex 回归测试，锁定金钱请求优先返回 NotPartOfGuild，失败的列表请求保留 `GuildCanRequestItems`；公会仓库定向测试通过。

### 2026-08-25 — admission bypass 重构必须同一编译步迁完全部调用点

- Symptom: 将 variadic admission flag 改成显式 `AfterAdmission` wrapper 时，先删除共享 helper 再迁其余两个消费者，包级 compile 报 `undefined: itemUseAdmissionAlreadyChecked`；首个可编译版本又只绕过 `CanUseItem`，仍把 death bool 传入 family helper，使 Hero/Shout 等 world commit 能在共享 runtime snapshot 之后重复 generic death 门禁。
- Root cause: 接口迁移按文件而非按声明和完整 caller 集分步，发送最小编译前留下了有意但不可编译的中间态；随后又把“已执行 common admission”误缩窄成仅 gender/class/requirement，没有把已经由 main 线性化的 generic death decision 纳入 bypass 契约。
- Prevention: 先以 `rg` 列出声明和全部消费者；同一补丁迁完声明、wrapper 与 caller，或保留旧 helper 到最后一个消费者切换完成后再删除。`AfterAdmission` API 不再接收已经裁决的 common/death 输入，原始 standalone wrapper 才执行完整门禁；family-specific effect checks 仍由 owning helper 保留。
- Verification: Hero/Shout/Mount/intelligent-creature wrapper 与 main caller 全部迁到显式 `AfterAdmission` API 后，`go test ./cmd/crystal-server -run '^$'` 重跑 exit 0；focused `-count=20` 与 focused race `-count=5` 均通过，且 due full test/vet/build/full-race 全部 exit 0。
- Strengthening after auth-first adoption: 将智能生物领养分支从 session/world-first 改成 latest-auth callback 后，旧外层 `index` 不再被使用，首个 package compile 报 `declared and not used: index`。根因是替换事务体时未同步缩窄短变量声明；修正为丢弃 slot 的 `_` 后 `go test ./cmd/crystal-server -run '^$'` exit 0。大块事务替换后必须立即按新消费者集合检查声明，而不能沿用旧分支变量。

### 2026-08-12 — Go 门禁前检查并清理可重建构建缓存

- Symptom: 新关系网络测试编译时出现 `no space left on device`，系统数据卷仅剩约 204 MiB，而 Go build cache 占用约 5.4 GiB。
- Root cause: 长期多批次 `go test`/race 构建缓存累积，门禁前未检查临时卷余量。
- Prevention: 大批次全量/race 门禁前用 `df -h` 和 `du -sh $(go env GOCACHE)` 检查空间；不足时仅执行 `go clean -cache -testcache` 清理可重建缓存，不删除项目或用户数据。
- Verification: 清理后数据卷恢复约 25 GiB，关系双会话测试重新编译并通过。
- Strengthening after recurrence: 本批普通测试先通过，但随后 `go test -race ./...` 因 Go build cache 达到约 12 GiB、系统临时卷仅剩约 303 MiB 而在构建阶段失败；门禁前必须同时检查可用空间、Go cache 和临时构建目录，不能只在历史失败后清理一次。
- Verification after recurrence: 仅执行 `go clean -cache -testcache` 后可用空间恢复约 12 GiB、Go cache 降至 8 KiB；重新运行 `go test -race ./...`、`go vet ./...` 和 `go build ./...` 均以退出码 0 完成。

### 2026-08-11 — .NET 工具必须单独标注未编译验证

- Symptom: 新增 `Crystal.LegacyWorldExport` 或修改 `Crystal.ProtocolProbe` 后尝试执行 .NET 构建，当前环境没有 `dotnet`，命令输出 `dotnet unavailable`。
- Root cause: 运行环境只具备 Go 工具链，不能把 .NET 项目静态检查当成真实编译验证。
- Prevention: 提交前先探测 `dotnet`/`csc`/`mcs`；若均不可用，记录 exporter 与 probe 的未验证边界，并在有 .NET 8 SDK 的环境补跑两者及现有客户端探针。
- Verification: Go 的 `test`、`race`、`vet`、`build` 与差异检查通过；本轮怪物掉落/default NPC exporter 仍因同一环境限制未编译，.NET exporter 和 ProtocolProbe 保留为待 SDK 环境验证项。

### 2026-08-12 — 地图门禁失败路径必须保持事务性

- Symptom: TownRevive 先恢复 HP、ENTERMAP 先清空 pending 目的地，再检查 RequiredGroup 时，失败请求会留下不可见的生命值或入口状态变化。
- Root cause: 业务动作的副作用早于目标地图资格校验提交，失败结果没有回滚。
- Prevention: 所有 RequiredGroup 入口先完成地图存在、坐标和组人数校验；成功后才写回 HP/坐标，ENTERMAP 只有非门禁失败时才消费 pending 目的地。
- Verification: RequiredGroup world 测试断言 TownRevive 不恢复 HP、ENTERMAP 保留 pending；全量 Go 门禁通过。

### 2026-08-12 — 并行功能线交接后必须先恢复包级编译绿色

- Symptom: Rental 与移动兼容线合入共享工作树后，`cmd/crystal-server` 留下未使用的 `oldDirection` 和尚未实现的 `deliverMovementMapTransition`，后续工作处在不可编译中间态，放大了“迁移没有进展”的感受。
- Root cause: 多条功能线同时触碰 `main.go`/`world.go`，交接时只记录了剩余事项，没有在每个写入边界立即执行最小包级编译；新的兼容分支继续叠加在未完成的整合点上。
- Prevention: 并行线优先按互斥文件或稳定接口拆分；任何功能线完成写入后，整合者先运行受影响包的仅编译门禁，编译未恢复绿色前不再叠加另一条共享文件修改；未完成 helper 必须与调用点在同一整合步落盘。
- Verification: 本次 `go test ./cmd/crystal-server -run '^$' -count=1` 在 0.1 秒内准确定位两个整合缺口；修复后必须以同一命令恢复绿色，再进入 Rental 定向及全量门禁。

### 2026-08-14 — 状态门禁必须收窄到原版 capability 边界

- Symptom: FrostCrunch 初版在普通宠物和 Conquest archer 的外层 AI tick 遇到 Frozen 就直接 `continue`，同时跳过了普通宠物驯服到期/跨图召回和 archer 搜索目标等不属于移动或攻击的生命周期工作。
- Root cause: 把“Frozen 时不能 Move/Attack”简化成“整个对象不处理”，没有沿 Legacy `MonsterObject.Process` 的顺序区分 tame lifecycle、`ProcessSearch`、recall 与最终 `CanMove`/`CanAttack` 门禁。
- Prevention: 每个控制状态先标注它约束的原版 capability；门禁只能放在对应动作提交点，外层对象 tick、到期清理、持久化、召回和搜索继续运行。新增状态测试必须同时覆盖“受禁动作不发生”和“非受禁生命周期仍推进”。
- Verification: Go 已把 Frozen 从普通宠物/archer 外层循环移到 follow/target/attack 动作点；回归测试确认宠物保留目标且不移动/攻击、Frozen 期间仍执行 tame 到期广播，archer 仍更新搜索/缓存目标但不创建投射物，相关聚焦测试通过。

### 2026-08-15 — 嵌套魔法分支修改后必须立即做语法门禁

- Symptom: 支援魔法分支初版漏掉嵌套 `if` 的闭合括号，包级编译在后续函数定义处才报告 `expected '('`。
- Root cause: 在已有长 `if/else if` 链中一次性插入两层局部逻辑，没有先验证嵌套边界。
- Prevention: 长分支新增后立即用 `gofmt` 和 `go test <affected-package> -run '^$' -count=1`；对每个局部 `if` 先明确闭合范围，再加入行为断言。
- Verification: 修正闭合边界后包级编译、支援魔法定向世界测试和真实 net.Pipe transcript 均通过。

### 2026-08-15 — Guard 的攻击方向门禁必须与 AI 攻击门禁分开迁移

- Symptom: ArcherGuard 已能按 Legacy 规则攻击红名玩家，但 Go 玩家攻击公共入口仍可能把 AI=113 当作普通可攻击怪物。
- Root cause: 只迁移了 `PlayerObject.IsAttackTarget(MonsterObject)` 的 PK 门禁，遗漏了 `Guard.IsAttackTarget` 对玩家和怪物攻击者恒为 `false` 的独立覆盖。
- Prevention: 迁移同一对象的双向战斗语义时，分别检查“对象主动攻击目标”和“玩家/宠物主动攻击对象”两套入口；特殊 AI 的攻击资格不能推导出其可被攻击资格。
- Verification: `playerCanAttackMonsterLocked` 现明确拒绝 AI=113，ArcherGuard 定向测试同时覆盖主动攻击和玩家近战入口，均确认玩家攻击不造成伤害。

### 2026-08-17 — 特殊石化门禁必须覆盖每个玩家攻击入口

- Symptom: EarthGolem 石化后普通玩家攻击已被拒绝，但区域攻击入口仍返回可攻击，能力门禁回归失败。
- Root cause: 只在单目标 `playerCanAttackMonsterLocked` 迁移了石化条件，遗漏独立的 `playerCanAreaAttackMonsterLocked` 分支。
- Prevention: 迁移不可攻击状态时建立入口矩阵，至少覆盖单体、区域、魔法、宠物/Hero、毒/Buff、推退和移动/怪物 AI；每个入口都用同一状态 fixture 断言拒绝且不产生副作用。
- Verification: EarthGolem 石化世界测试现覆盖单体/区域/移动/攻击/毒/Buff/推退门禁，并在服务端整包测试中通过。

### 2026-08-17 — 新增 AI 常量前必须搜索全包声明

- Symptom: WoodBox 初次接入时 `woodBoxMonsterAI` 重复声明，Go 包编译失败；修复后 WoodBox 定向测试通过。
- Root cause: 新 AI 常量补丁没有先搜索现有常量声明，导致把已存在的标识符再次加入常量区。
- Prevention: 新增 AI/协议常量前先用 `rg -n` 在 Go 包内搜索完整标识符，确认唯一声明位置；随后立即运行 `gofmt` 和 `go test ./cmd/crystal-server -run '^$'`，包级编译通过后再写行为测试。
- Verification: 删除重复声明并格式化后，包级空测试和 `TestWoodBox` 定向测试均通过。

### 2026-08-17 — ScalyBeast Type 0 延迟重验不能新增距离门禁

- Symptom: 用 Stomp 测试“同地图远距离目标仍命中”时，目标离开半径 2 后当然不会被 AOE 扫描，HP 未变化，错误地把测试失败归因于延迟重验距离规则。
- Root cause: Legacy Type 0 `CompleteAttack` 直接对已捕获且仍有效的 target 调用 `Attacked`；AOE Stomp 则另行调用当前位置的 `FindAllTargets`，两条命中路径不能混用。
- Prevention: 延迟重验无距离门禁必须使用 Type 0 direct-hit fixture，并只验证目标存在、可攻击、同地图和节点有效；Stomp 单独验证当前位置半径扫描。
- Verification: 回归已改为 Type 0、同地图但移到远处且置于安全区的目标，仍收到 MAC 命中并保持无 Paralysis；定向普通/race 回归通过。

### 2026-08-17 — AI=166 FloatingRock 延迟死亡重验不得新增距离门禁

- Symptom: FloatingRock 延迟死亡测试最初把目标移出攻击距离并期望命中被取消，但 Legacy `CompleteDeath` 仍会在目标存在、可攻击、同地图且 Node 有效时结算。
- Root cause: 将普通 AI 的攻击范围门禁错误复用到死亡后的 `FindAllTargets`/延迟 AC 路径；两条 Legacy 路径的重验条件不同。
- Prevention: 为每个延迟动作列出 admission 与 impact 的独立条件；死亡 AC 只按 Legacy 实际检查存在、攻击资格、地图和实体状态，不额外推导距离条件。
- Verification: 将回归夹具改为跨地图重验后，Player/owned-Monster/Hero 世界测试和认证 FloatingRock 转录均通过，跨距离但同地图目标不再作为错误拒绝证据。

### 2026-08-18 — SnowYeti 远程命中前必须建模每个 tick 的 MoveTo 回退

- Symptom: SnowYeti 远程命中测试在预期 `[5,1,2,10,3,10]` 后实际多消费一个 `Next(2)`，严格 transcript 失败。
- Root cause: Legacy SnowYeti 在远程 action lock 期间仍执行 `MoveTo`；攻击冷却阻挡 Walk 后，MoveTo 每个 ProcessTarget tick 仍消费无偏回退随机数。
- Prevention: 记录发起 tick 与命中 tick 的完整 ProcessTarget 前导；不要把 ActionTime 阻挡误判为没有 AI 随机消费。
- Verification: 远程序列更新为 `[5,1,2,10,3,10,2]`，世界测试、owned-Monster/Hero 测试和认证 session transcript 均通过。

### 2026-08-18 — AI=222 攻击状态 helper 的值/指针边界要在重构后立即编译

- Symptom: SepHighAssassin 将公共攻击准备逻辑拆成值参数 helper 后，包级测试编译报错，把 `worldMonster` 值再次解引用为指针。
- Root cause: 从原先接收 `*worldMonster` 的攻击函数抽出只读的 ready 阶段时，没有同步核对 helper 签名和调用点的所有权语义。
- Prevention: 拆分会写入计时器的攻击流程时，先标注 setup/ready helper 的值或指针契约；每次签名调整后立即运行 `go test ./cmd/crystal-server -run '^$'`，再进入行为测试。
- Verification: 删除值参数上的非法解引用后，AI=222 包级编译、定向测试连续 5 次通过；ready 阶段仍按 live attacker 值排队即时与延迟伤害。

### 2026-08-18 — Go 时间常量名称必须在目标编译前核对

- Symptom: `DelayedExplosion` 定向测试首次编译失败，测试代码把标准库的 `time.Nanosecond` 写成不存在的 `time.Nanoseconds`。
- Root cause: 手写纳秒边界表达式时凭复数形式猜测 API 名称，没有在保存精确时间边界前先使用已确认的标准库常量。
- Prevention: 新增时间边界测试只使用已核对的 `time.Nanosecond`/`time.Microsecond` 等标准库标识；第一次新增测试后立即运行目标包编译，失败时先记录并修正 API 名称。
- Verification: 改用 `time.Nanosecond` 后重新运行 `gofmt` 与 `TestDelayedExplosion` 定向编译/测试。

### 2026-08-18 — ExplosiveTrap 远离清理要保留同轮对象处理顺序

- Symptom: Go 的 ExplosiveTrap distance-cleanup 测试首次预期首个链接对象移除、其余两个留到下一轮；实际 caster 远离时三个对象都在同一次 world tick 消失。
- Root cause: Legacy `SpellObject.Process` 先检查施法者距离，再检查 fuse；首个对象 `Despawn` 会先把链接对象标记为 detonated，但当前对象循环随后仍按距离条件继续处理每个链接对象，因此同轮全部移除。
- Prevention: 迁移链接对象生命周期时，分别复核 `Process` 的条件顺序、`RemoveObject`/`Despawn` 的副作用和同轮迭代，而不是只模拟链接对象的下一次 expiry；确定性测试同时覆盖远离、引爆和过期边界。
- Verification: 直接按旧版顺序对照 `SpellObject.Process`/`Despawn` 后，Go distance-cleanup 断言改为同轮移除全部三格；ExplosiveTrap 普通与 race 定向测试通过。

### 2026-08-19 — 二进制写出器新增原始字节方法后先做定向编译

- Symptom: 初次运行 `go test ./internal/legacyaccount` 时，新增 `binary_write.go` 报告 `legacyEncoder` 缺少 `raw` 方法，批次测试未能编译。
- Root cause: 编码器调用了用于拼接已编码 `UserItem` 记录的 `raw` 写入入口，但实现补丁只先添加了带长度前缀的 `bytes` 入口。
- Prevention: 新增二进制 writer 时先列出 primitive/raw 两类写入 API，并在扩展测试前立即运行受影响包的 `gofmt` 与定向 `go test`。
- Verification: 补充 `raw` 方法后，`go test ./internal/legacyaccount ./cmd/crystal-legacy-account-import`、全仓编译、vet/build 均通过。

补充证据：一次用 `sed -n '/switch packet.ID/,/default:/p'` 统计 Go 客户端入口时，提前命中了嵌套 `default`，输出了大量伪缺失入口；改用整份 `main.go` 的 `protocol.Client*` 引用集合复核后结果为空。预防再强化为：协议覆盖审计不得用会被嵌套 switch 截断的范围表达式，必须用语法边界明确的提取或完整引用集合，并对结果做人工抽样。

### 2026-08-19 — Go 多返回值与坐标 helper 必须在首轮编译门禁中校验

- Symptom: ManectricClaw 初版把返回 `([]worldNotification, bool)` 的伤害 helper 直接作为单返回值返回，并把 `movePoint` 的两个返回值直接放入坐标数组，导致包编译失败。
- Root cause: 从相邻 AI 实现复制结构时只核对了业务调用形状，没有先核对 Go helper 的签名和多返回值解构规则。
- Prevention: 新增 resolver 后立即执行 `gofmt` 与 `go test <受影响包> -run '^$'`；所有多返回 helper 显式解构，所有坐标构造先绑定 `(x, y)` 再组装。
- Verification: 修正后包级编译门禁通过，随后 ManectricClaw 定向行为测试全部通过。

### 2026-08-19 — AI=87 首轮编译必须核对多返回 helper 与运行时投影字段

- Symptom: ManectricBlest 首轮包编译把 `damageManectricClawTargetLocked` 的双返回值直接用于 `append`，并假设 `StoredHero` 暴露 `Hidden` 字段；编译失败。
- Root cause: 复用相邻 AI 的伤害 helper 时只看了业务含义，没有逐项核对 Go 签名和当前 Hero 持久化/运行时投影结构。
- Prevention: 新增 AI 前先读取 helper 完整签名与所有目标结构字段；多返回调用显式解构，目标可见性只使用当前模型实际提供的字段，不从 Legacy 类型名推断 Go 字段。
- Verification: 修正为显式解构并移除不存在的 Hero 字段依赖后，包级 `go test ./cmd/crystal-server -run '^$'`、AI=87 普通/`-race` 定向测试、`go vet ./...` 和 `go build ./...` 均通过。

### 2026-08-19 — AI=89 首轮编译必须先做常量/多返回值门禁

- Symptom: IcePillar 首轮包编译因 `icePillarMonsterAI` 在新文件和集中 AI 常量块重复声明，以及把无返回值通知追加 helper 当作返回值使用而失败。
- Root cause: 新 AI 文件与现有集中常量/通知编码边界未逐项核对。
- Prevention: 新批次先搜索同名常量/helper，再立即 gofmt 和 `go test ./cmd/crystal-server -run '^$'`；无返回值 helper 只调用不赋值。
- Verification: 修正后包级编译门禁通过；AI=89 定向普通/race、全量测试、vet/build 作为批次验收。

### 2026-08-19 — AI=90/91 首轮门禁必须登记公共人口与 PoisonTarget 实参顺序

- Symptom: Troll 首轮编译把 `int32` 攻击距离与 `int` 局部变量、距离延迟与 `time.Duration` 混算；修正后定向测试仍未进入 Troll 分支，因为新 AI 没加入 `monsterAICommonPopulation`。继续对照 Legacy 又发现 TrollKing 的 `PoisonTarget(target, 1, Random.Next(MaxMC), Dazed, 1000)` 会先消耗 MaxMC duration，再计算 SC value。
- Root cause: 只在 dispatch 和 action resolver 中接入新 AI，没有把人口注册、强类型时间/距离表达式和 Legacy 调用点的参数求值顺序作为同一批次的入口契约核对。
- Prevention: 新 AI 开工时同时搜索并更新 population、初始化/ProcessAI dispatch、delayed-action resolver 三个入口；首轮用包级空测试编译；对每个 `PoisonTarget` 调用记录参数名称、求值顺序、外层/目标侧抗性和状态持续时间，使用 MC/SC 不同值的 fixture 锁定随机流。
- Verification: `go test ./cmd/crystal-server -run '^$'`、Troll 普通/定向全包测试、Troll `-race`、`go test ./...`、`go vet ./...` 与 `go build ./...` 均通过；TrollKing 测试验证 MaxMC duration、SC value、Dazed effect/chat 及双次命中顺序。

### 2026-08-19 — Go 门禁前必须检查可再生构建缓存占用

- Symptom: TrapHexagon session 测试首次启动在创建 Go `vet.cfg`/`importcfg` 时失败，错误是 `no space left on device`，尚未进入代码编译或测试断言。
- Root cause: 本机 Go build cache 达到约 66 GiB，根文件系统仅剩约 204 MiB。
- Prevention: 长时间 Go 门禁前先检查 `df -h` 和 `go env GOCACHE`；若仅是可再生缓存占满，使用 `go clean -cache -testcache` 后再判断实现状态，不要把环境错误记录为功能回归。
- Verification: 清理缓存后可用空间恢复到约 66 GiB，随后重跑 TrapHexagon session 测试。

### 2026-08-20 — EvilCentipede 的 FindAllTargets(false) 不能复用带可见性门禁的搜索 gate

- Symptom: 代码审查发现 EvilCentipede 延迟 AOE 初版复用了 `ancientBringerMonsterCanBeAttackedLocked`，会把隐藏但有 Owner 的 Monster 从 `FindAllTargets(..., false)` 结果中排除。
- Root cause: Legacy `MonsterObject.FindAllTargets` 只在 `needSight=true` 时检查 `Hidden`；Monster 的 `IsAttackTarget(MonsterObject)` 负责 ownership/combat gate，但不检查客户端可见性。
- Prevention: 对每个 Legacy `FindNearby`/`FindAllTargets` 调用分别保留 `needSight` 语义；延迟 `false` 扫描使用不含 `monsterClientVisible` 的 ownership/combat gate，并由单独测试覆盖隐藏 owned Monster。
- Verification: 回读 `MonsterObject.cs` 的 `FindAllTargets` 与 `IsAttackTarget`；Go resolver 改用 MAC 路径的 ownership/combat gate，EvilCentipede 定向世界测试（隐藏 CreeperPlant pet）通过。

### 2026-08-20 — AI=31/32 RightGuard 的 Shock 与目标门禁必须独立于通用 AI

- Symptom: 初版 RightGuard/LeftGuard 复用通用 `monsterCanAttackLocked` 与 BoneSpearman 目标 helper，遗漏了 Legacy `CanAttack` 不检查 `ShockTime`，并把普通 Guard 的目标规则混入了 Right/Left Guard。
- Root cause: 相邻 AI 都有八格或延迟攻击外观，但 Legacy 的 `RightGuard.ProcessTarget` 是独立 override；没有先逐行对照 `CanAttack`、`FindTarget` 和 `IsAttackTarget` 的调用边界。
- Prevention: 对每个 Monster 子类分别实现攻击时序、搜索期与延迟结算期目标门禁；把 Shock-in-range attack、Shock-out-of-range clear、Rage/Hallucination 野生怪物例外和 MAC/MACAgility 防御类型写入独立测试。
- Verification: AI=31/32 世界测试覆盖 Player/owned-Monster/Hero、野生怪物 Rage 门禁、Shock 边界、Right/Left 距离延迟与 delayed revalidation；普通定向、组合回归、三次 race、全仓库测试、vet/build 均通过。

### 2026-08-20 — 完整普通门禁必须同时保留退出码和失败摘要

- Symptom: AI=48 批次第一次 `go test ./...` 返回失败，但大量 `read pipe: EOF` 日志淹没了具体失败名；同一最终工作树随后用捕获退出码的重跑取得 `go_rc=0`。
- Root cause: 直接展示原始长输出无法稳定区分业务失败与会话清理日志，且没有在同一 shell 中保留 Go 命令的退出码和过滤摘要。
- Prevention: 长门禁用任务专用变量保存完整输出和退出码，再单独过滤 `--- FAIL`/`FAIL`/panic；失败名不明时不得直接归因到本批生产代码。
- Verification: 重跑用 `go_rc` 明确取得 0；AI=48 定向测试、race、vet/build 与全仓普通结果分别留存。
