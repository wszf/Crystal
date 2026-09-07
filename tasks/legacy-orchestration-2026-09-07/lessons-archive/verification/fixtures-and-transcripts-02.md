### 2026-08-15 — 新 AI 测试必须在仅编译门禁下清理临时变量

- Symptom: AI=121 SeedingsGeneral 定向测试首次包级仅编译因声明但未使用的 `impact` 变量失败，行为测试尚未执行。
- Root cause: 从多目标 transcript 测试复制了结果变量，但该断言只需要检查实体状态，没有同步删掉变量。
- Prevention: 新增测试后立即运行 `go test ./cmd/crystal-server -run '^$' -count=1`；若只读最终状态，直接调用 tick 而不绑定未使用结果，或对结果做明确包序断言。
- Verification: 删除未使用绑定后重新执行包级仅编译，随后运行 SeedingsGeneral 定向测试确认修复覆盖实际行为。

### 2026-08-15 — AOE 测试必须区分受伤状态与旁观者公开包

- Symptom: AI=121 SeedingsGeneral 定向测试把 Echo poison 误期望为额外移动/生命包，并把 Stomp 区域外玩家误期望为零包；实际 transcript 分别只含当前 Slow poison 包，以及区域内动作对旁观者广播的公开受击/中毒包。
- Root cause: 沿用了另一个 AI 的 poison 副作用包序，且把“区域外不扣 HP”错误简化成“区域外不接收广播”。
- Prevention: 新增多目标动作时先以实际 Go notify 路径取得完整接收者矩阵，分别断言每个实体的 HP/毒状态和每条连接的公开包；不同毒类型不能直接复用其他 AI 的包序期望。
- Verification: 将断言改为当前 SeedingsGeneral transcript，并保留区域外 HP/毒状态不变的语义断言后，重新运行定向测试验证。

### 2026-08-15 — 同格攻击 transcript 必须使用 Direction=0 哨兵

- Symptom: AI=121 同格远程边界测试把方向期望写成相邻目标的 2，实际 `DirectionFromPoint` wire 值为 0。
- Root cause: 测试只按攻击者面向目标的普通方向推导 payload，没有单独核对同格输入的旧版方向函数哨兵。
- Prevention: 协议 payload 测试覆盖同格、相邻和远距时逐 case 计算方向；同格不可从相邻方向复用期望值。
- Verification: 同格 case 改为 Direction=0 后，SeedingsGeneral 边界定向测试验证 same-cell 与 adjacent-fallback 均通过。

### 2026-08-15 — RestlessJar 测试夹具需匹配协议索引类型并清理未用 transcript

- Symptom: AI=122 RestlessJar 首次包级仅编译同时报告 `uint32` 到 `SelectInfo.Index` 的类型不匹配，以及 Spin/Tornado 未使用的 `impact` 变量。
- Root cause: 通用玩家 helper 复用了运行时 ObjectID 类型，没有按协议结构的 `int32` 字段转换；多余的 tick 结果也未在复制测试后清理。
- Prevention: 构造协议夹具时逐字段核对底层类型，新增测试立即运行包级仅编译；只验证实体状态时直接调用 tick，不保留未使用返回值。
- Verification: 修正索引为 `int32(id)` 并删除未使用绑定后，重新执行仅编译门禁确认 RestlessJar 行为测试可构建。

### 2026-08-15 — RestlessJar 多目标 Stomp transcript 必须包含先前目标的公开推退包

- Symptom: Stomp 状态已正确，但主目标 packet 断言只列自身命中的五包；实际先命中的邻居向主目标广播了 `ObjectStruck`、`DamageIndicator`、`ObjectPushed`，随后才是主目标自己的命中和 `Pushed`。
- Root cause: 按目标拆分了私有包，遗漏了每个区域目标动作对同一范围内接收者的公开广播和顺序。
- Prevention: 多目标伤害/推退先按目标处理顺序建立接收者矩阵，再分别断言主目标、邻居和旁观者的公开/私有包序。
- Verification: transcript 增加邻居的三条公开包后，RestlessJar Stomp 定向测试继续验证完整顺序。

### 2026-08-16 — 在线 session transcript 必须过滤允许插入的 TimeOfDay 包

- Symptom: 全仓普通测试中 `TestCraftSessionRejectsDeadFarWrongPageAndUnknownRecipe/out_of_range` 在第 3 次走路读取到 `ServerTimeOfDay`（ID 61）而非 `ServerUserLocation`；单独运行因时钟落点不同未复现。
- Root cause: 在线 session 的全局时钟 ticker 可在任意请求响应之间插入时间通知，测试 reader 把异步允许包当成了本次 walk 响应。
- Prevention: 有在线 ticker 的 transcript reader 对明确允许的 `ServerTimeOfDay` 循环过滤，再继续等待目标响应；其他包仍立即失败，禁止用固定 sleep 或忽略所有异常包掩盖协议错误。
- Verification: 将 Craft 距离移动、NPC 移除和 ignored-request reader 改为只过滤 TimeOfDay 后，单测重复与全仓普通/race 门禁验证插入通知不再造成假失败。

### 2026-08-15 — AI=130 CannibalTentacles 测试必须区分完整 tick 的后续 AI 副作用

- Symptom: CannibalTentacles 攻击命中测试只断言命中伤害包，但完整 `world.tick` 在 resolver 后继续执行怪物 AI，额外产生 `ObjectWalk`，导致 transcript 断言失败。
- Root cause: 混淆了隔离攻击动作解析器与完整世界 tick 的可观察阶段；命中处理完成不代表同一 tick 已结束。
- Prevention: 测试完整 tick 时按实际调度顺序把命中后的移动、毒伤和其他 AI 通知纳入接收者 transcript；若只验证 resolver，则直接调用隔离入口并明确不覆盖后续 tick 副作用。
- Verification: 补入命中后的移动包并按 map 中的权威实体断言后，AI=130 CannibalTentacles 定向测试通过。

### 2026-08-15 — 在线 AI transcript 的人工时钟必须领先会话维护 tick

- Symptom: TucsonGeneral 真实 `net.Pipe` transcript 在 race 下偶发没有 15 个岩石 spawn 通知，手工 `world.tick` 返回空结果；普通模式也可重复复现。
- Root cause: 停止 world ticker 不会停止 session 主循环在每轮读包前执行的 `world.tick(time.Now())`；注入即时到期的 AI 状态后，该维护 tick 可能先消费 Rage 并生成/发送岩石。
- Prevention: bootstrap 后停止后台 ticker，并发送 KeepAlive 读完响应确认会话已完成同步；注入状态时把 transcript 的 `base` 放到实时会话时钟之后，保证会话维护 tick 只能看到尚未到期的动作，再用人工时间推进完整生命周期。
- Verification: TucsonGeneral rage transcript 普通定向通过，race 连续 10 次通过；随后 GasToad/TucsonGeneral 定向普通/race、服务端整包普通/race 均通过。

### 2026-08-15 — Mantis 定向 transcript 必须包含同 tick 的旁观者状态包和目标 AC 随机值

- Symptom: AI=133 Mantis 定向测试先遗漏了 Dazed 首跳后旁观者收到的 `ServerObjectPoisoned`，并把 Monster 目标用 `Next(3)=0` 的 AC/伤害场景期望为 89 HP，实际为 90。
- Root cause: 完整 `world.tick` 在 AI 施毒后同一 tick 继续处理零时刻毒物，并按接收者广播毒包；测试又把 `monsterAIPowerLocked(10,12)` 的确定性 roll=0 误按包含上界 11 计算，而该调用得到 10。
- Prevention: AI transcript 按 tick 阶段和接收者矩阵生成包序；每个确定性随机源先列出调用上界及返回值，再逐项计算伤害、AC 和最终 HP，不能复用相邻用例的期望。
- Verification: 修正观察者包序为 `ObjectAttack -> ObjectEffect -> ServerObjectPoisoned`，Monster 目标期望为 90，并移除 300ms 命中阶段重复的 `ServerPoisoned` 期望；随后重新运行 Mantis 定向测试确认。

### 2026-08-15 — 新增 session transcript helper 必须先检查整个 Go package

- Symptom: AI=133 Mantis 真实会话测试首次包级编译因新增的 `packetIDs` helper 与现有 `awakening_test.go` 同包 helper 重名而失败。
- Root cause: 新测试只按当前文件判断辅助函数是否可用，没有检索 `cmd/crystal-server` 的整个 `main` test package；Go 的 `*_test.go` 文件共享同一 package 命名空间。
- Prevention: 新增测试 helper 前先用 `rg` 搜索整个 package 的声明与调用，优先复用现有 helper；新增后先运行 `go test ./cmd/crystal-server -run '^$' -count=1`，再运行行为 transcript。
- Verification: 删除重复 helper、复用现有 `packetIDs` 后，Mantis 会话测试进入运行阶段；随后用包级编译和定向 transcript 验证。

### 2026-08-15 — AssassinBird 测试复合字面量必须先通过语法门禁

- Symptom: 新增 AI=134 测试夹具执行 `gofmt` 时在 `Stats` 切片闭合处报告 `missing ',' before newline in composite literal`，行为测试尚未运行。
- Root cause: 多行 `worlddata.MonsterInfo` 复合字面量的最后一个切片元素后遗漏了 trailing comma，长测试 patch 没有在首次落盘后立即做语法门禁。
- Prevention: 新增或改写多行 Go 复合字面量后立即运行 `gofmt` 和受影响包的 `go test ... -run '^$'`；切片/映射闭合行逐项核对逗号与层级，再继续增加行为断言。
- Verification: 该错误在 `gofmt` 阶段被捕获且未运行服务端测试；补齐逗号后将重新执行包级编译和 AssassinBird 定向测试。

### 2026-08-15 — AssassinBird session 随机源必须避开 AI 初始化

- Symptom: AI=134 真实 `net.Pipe` transcript 在 bootstrap 阶段先消费 `Random.Next(3000)`，确定性夹具把它当作攻击分支而失败，随后 pipe 被关闭。
- Root cause: 测试在服务启动前安装了只允许攻击阶段上界的 `monsterAIRoll`；后台维护 tick/AI 初始化仍会先读取初始化搜索延迟和 CoolEye 随机值。
- Prevention: session fixture 启动前不注入攻击专用随机源；完成 bootstrap、停止 ticker 并用 KeepAlive barrier 同步后，再初始化确定性目标/时间并安装只覆盖实际攻击调用序列的随机源。初始化阶段若必须运行，使用非断言默认源。
- Verification: 失败发生在 bootstrap fixture、业务 transcript 尚未执行；调整随机源安装边界后将先运行包级编译，再重复 AssassinBird session transcript。

### 2026-08-16 — 人工 session 时钟必须固定全局光照状态

- Symptom: AssassinBird 真实 `net.Pipe` transcript 在攻击 tick 多收到一个 ID 61 (`ServerTimeOfDay`)，精确包序断言失败。
- Root cause: 测试把人工 `base` 设置为实时后一小时，跨过了 `lightSettingAt` 的小时边界；在线 world 的全局光照更新因此在战斗通知前广播了状态包。
- Prevention: 只要 session transcript 使用与实时维护 tick 不同的人工时钟，就在注入 `base` 后调用 `setLightClock` 固定同一光照值，再驱动 world tick；不能假设停止后台 ticker 会关闭在线光照副作用。
- Verification: 固定光照时钟后，AssassinBird session transcript 的通知 ID 和完整 payload 连续通过，包级编译保持通过。

### 2026-08-16 — 全量会话回归必须覆盖旧 transcript 的光照时钟稳定性

- Symptom: 服务端整包普通测试在 Mantis 和 Tucson transcript 中分别收到额外的 `ServerTimeOfDay`，虽然领域行为未变，精确包序仍失败。
- Root cause: 新增光照副作用后，旧会话夹具仍用 `time.Now().Add(time.Hour)` 作为人工推进时钟，当前小时变化可能跨越 Legacy 光照区间。
- Prevention: 每个使用人工 `base` 的在线会话夹具都必须在停止维护 ticker 后注入固定 `setLightClock`；整包回归不能只验证新功能的 session 测试。
- Verification: 将 Mantis/Tucson 受影响夹具同步固定光照后，先重跑各自 transcript，再重跑服务端整包普通、race、vet 和 build 门禁。

### 2026-08-17 — RhinoPriest session 与多目标 transcript 夹具必须隔离派生状态和独立 AI

- Symptom: AI=137 RhinoPriest session transcript 在减益命中后多收到一个 `ServerHealthChanged`；新增夹具还曾引用不存在的 `monsterStatMinMAC/MaxMAC`、遗漏 `worlddata` import，并把相邻 `(1,0)` 目标误判为远程；宠物/Hero 回归初版又发生 nil 宠物解引用，Hero 额外产生自身攻击包。
- Root cause: `StatsInitialized=true` 的真实会话在 DC/MC/SC 减益后会按角色等级刷新派生 MaxHP，人工 100 HP 超过新手上限；Monster stat 常量和 import 未先从当前 Go package 核对；RhinoPriest 的同格/相邻分支边界与 Legacy 显式距离判断未逐项列出；Hero runtime 与 Monster AI 会在同一 `world.tick` 独立推进，而双目标表中另一分支的指针为 nil。
- Prevention: session fixture 保留真实 stat refresh，但把合成角色等级提高到足以容纳人工 HP，并固定光照/时间；新增测试先用当前 package 的符号检索和包级编译门禁；按 Chebyshev 距离逐项标注近战、同格和远程；多目标测试先按 kind 选择非 nil ID，并把 Hero `ActionReadyAt` 置于人工时钟之后，避免无关 AI transcript 污染。
- Verification: RhinoPriest 世界攻击、毒物、减益、宠物 Monster、Hero 和真实 `net.Pipe` transcript 均通过；失败夹具修正后重新运行定向测试，完整门禁将在本批次末执行。

### 2026-08-17 — EarthGolem fixture 必须先核对运行时字段并隔离 Hero tick

- Symptom: AI=140 首次包级编译使用了不存在的 Monster MAC stat 名称和 `worldMonster/worldHero` 未定义的冷却字段；修正后 Hero 失效目标测试又收到运行时自动传回 owner 位置的对象包。
- Root cause: 测试夹具按 Legacy/其他实体的字段直觉拼接，没有先读取当前 Go struct 与 stat 常量；把 Hero 改图作为唯一失效条件，却遗漏 `tickHeroesLocked` 会在 owner 与 Hero 不同地图时自动 teleport。
- Prevention: 新增夹具先检索当前 package 的精确常量、struct 字段和 map key 语义并执行包级编译；失效目标 transcript 要隔离无关实体 tick（同步 owner 状态或冻结/移除独立运行实体），再断言目标失效无攻击包。
- Verification: EarthGolem Player/Monster/Hero 世界测试及真实 session transcript 均通过，包级 `go test -run '^$'` 先于行为测试通过。

### 2026-08-17 — TreeGuardian 测试必须锁定防御公式、Hero 基础敏捷和 value-map 回写

- Symptom: AI=141 首次定向断言把 AC/MAC 伤害按直觉算错；Hero 夹具遗漏等级基础敏捷的 `Random.Next(16)`；真实 session 第二次攻击因未回写 `world.monsters` 的 value 副本而没有产生攻击包。
- Root cause: 测试只看攻击力和手工 AC，没有沿实际 Player/Monster/Hero 命中路径计算防御；没有从 Hero class/level 的基础属性推导随机边界；修改 detached `worldMonster` 后把 map 当成引用容器使用。
- Prevention: 每个 AI 分支先分别固定攻击随机源、战斗随机源和目标层防御/敏捷公式；Hero transcript 覆盖其实际基础 stat roll；修改 `world.monsters` 中的实体后立即显式写回，并在下一 tick 从权威 map 回读状态。
- Verification: TreeGuardian 四分支/Fullmoon 世界测试、真实 `net.Pipe` 攻击与失效目标 transcript 均通过；普通/race 全仓测试、vet、build 和 diff 检查均通过。

### 2026-08-17 — TreeQueen transcript 必须按真实 recipient 与对象生命周期验收

- Symptom: AI=142 初始定向测试使用了错误的 recipient ID，并假定对象在 spawn tick 就完成后续命中/移除，导致 transcript 与实际通知和生命周期不符。
- Root cause: 测试按 fixture 中的猜测身份和单次 tick 推导网络行为，没有先确认真实接收者，也没有把召唤、延迟命中和过期移除拆成实际世界 tick。
- Prevention: 多对象 transcript 先从在线连接和通知接收者矩阵确定 recipient，再按每个对象的 spawn/impact/expiry 时刻逐 tick 断言；value-map 实体状态从权威 world map 回读。
- Verification: TreeQueen 世界测试和真实 `net.Pipe` transcript 已按实际 recipient、目标投影和有序生命周期修正并通过，普通/race/vet/build 门禁保持绿色。

### 2026-08-17 — TucsonMage bootstrap 完成后再注入攻击随机源

- Symptom: TucsonMage session fixture 的攻击随机源可能在 bootstrap 阶段先被 `Next(3000)` 消费，导致后续攻击 transcript 的随机序列漂移。
- Root cause: bootstrap、session 维护 tick 和攻击动作共享同一个确定性随机源，测试在连接尚未稳定前就安装了只覆盖攻击阶段的 roll 表。
- Prevention: bootstrap 阶段保留宽松随机源；完成 bootstrap 并用 KeepAlive barrier 同步后，停止维护 ticker，再安装攻击专用随机源和人工时钟，避免初始化消费混入攻击序列。
- Verification: TucsonMage 定向测试连续 5 次通过，随后服务端整包、全仓普通/race、vet 和 build 门禁均通过。

### 2026-08-17 — PeacockSpider fixtures must use the package's stat identifiers

- Symptom: The first PeacockSpider test fixture failed to compile because it used nonexistent `monsterStatMinMAC`/`monsterStatMinMC`/`monsterStatMinSC` identifiers.
- Root cause: The fixture copied the DC-specific `monsterStat*` naming pattern instead of checking the current Go package's shared `statMinMAC`/`statMinMC`/`statMinSC` constants.
- Prevention: Before adding a migrated monster fixture, search the current Go package for each stat constant and run `gofmt` plus `go test ./cmd/crystal-server -run '^$' -count=1` immediately after the first patch.
- Verification: The fixture now uses the authoritative shared stat constants; package compilation, PeacockSpider tests, and the full server test suite pass.

### 2026-08-17 — AI session transcript 的共享 world 读取必须持锁

- Symptom: AI=144 真实 `net.Pipe` transcript 在 `go test -race` 下读取 `world.monsterAttackActions[0]` 时报告与连接维护 tick 的数据竞态；普通测试未暴露问题。
- Root cause: 停止后台 ticker 不会停止连接 session loop 的维护 tick；夹具只在写入共享 AI 状态时持锁，却无锁读取仍由 session loop 访问的 world slice。
- Prevention: 在线 transcript 对共享 world 的每次读取都经过 `world.mu` 快照，写入也保持同一锁；KeepAlive 只提供会话阶段屏障，不等价于停止连接级 tick。
- Verification: action due/count 断言改为锁内快照后，AI=144 session race 定向测试连续 5 次通过，普通 transcript 仍锁定完整包序。

### 2026-08-17 — Halfmoon 多目标 transcript 必须区分自身包与范围广播

- Symptom: AI=146 Halfmoon 定向测试把某个玩家 recipient 收到的完整 impact 包序固定为自身四包，但四个相邻目标都在 16 格通知范围内，实际还包含其他目标的 `ObjectStruck`/伤害/生命广播。
- Root cause: 把单目标会话 transcript 的通知假设复用到多目标世界夹具，没有先按通知范围和每个命中目标拆分观察者可见包。
- Prevention: 多目标攻击断言先锁定动作目标顺序和各目标权威 HP，再只检查 recipient 必须存在的自身包，或为隔离观察者/按 object ID 过滤范围广播；不要把完整 recipient 列表当成单目标序列。
- Verification: 修正后 AI=146 四格 Halfmoon 世界测试验证了 PreviousDir 顺序、四个目标 HP 和隐藏目标命中；单目标 `net.Pipe` transcript 继续验证完整四包顺序。

### 2026-08-17 — AI=147 固定随机边界、目标投影和 session 时钟必须分别锁定

- Symptom: OmaMage 首次定向测试把固定 DC/MC 区间期望成大于 1 的随机上界；Player 目标的延迟动作使用协议兼容的 `TargetKind=0` 后未还原，导致命中静默丢弃；真实 session 重复运行还可能在手工 tick 外消费一次 AI fallback 抽样。
- Root cause: Legacy `GetAttackPower(min,max)` 对相等区间仍调用 `Random.Next(1)`；内部目标 kind 与线上的 Player 默认值没有在 resolver 边界归一化；停止 world ticker 不代表测试期间所有 session 调度都已冻结，且回调中直接 `Fatal` 会把后台抽样误报为业务失败。
- Prevention: 迁移固定区间时保留 unit-bound 随机调用；所有延迟动作 resolver 先把 `TargetKind=0` 还原为 Player，并用 delayed-hit 测试覆盖；真实 session 将 AI/search/action 时间置于手工时钟之后，随机回调只做同步记录并断言业务阶段的稳定序列，不在异步回调中终止测试。
- Verification: OmaMage 世界测试覆盖固定攻击边界、CanFly/冷却移动、延迟重验、Player/Monster/Hero 与两次独立毒物抽样；`net.Pipe` transcript 普通和重复 10 次、全量 `go test ./...`、`go test -race ./...`、`go vet ./...`、`go build ./...` 均通过。

### 2026-08-17 — AI=148 夹具必须遵循 Go 坐标类型与真实距离分支

- Symptom: OmaWitchDoctor 初次定向测试在 `gofmt` 前由 `int` 坐标传入 `int32` 字段而编译失败；修正后又把距离 1 的对角目标误标成远程，并在非直线延迟 tick 误断言“无通知”，遗漏了专用 `ProcessTarget` 的冷却追击 `ObjectWalk`。
- Root cause: 测试夹具按字面量直觉推导坐标和“远程=非相邻方向”，没有先核对 Go struct 的精确类型，也没有按 Legacy `CurrentLocation == Target` 或 `!InRange(...,1)` 的 Chebyshev 距离条件建分支表。
- Prevention: 新增坐标夹具先显式声明 `int32`；攻击矩阵用距离 0、1、>1 分别覆盖同格、近战和远程，再单独标注直线/非直线；延迟 tick 必须继续执行一次完整 AI 流程，把冷却期间的移动/无移动作为包级断言。
- Verification: AI=148 测试改用 `int32` 坐标、距离 2/3 的直线远程和距离 2/1 的非直线场景；世界测试、`net.Pipe` transcript 及普通/race 定向门禁均通过。

### 2026-08-17 — PowerBead transcript 的随机消费发生在延迟命中阶段

- Symptom: AI=149 `net.Pipe` transcript 在发出 `ObjectRangeAttack` 后立即断言固定 DC 的 `Random.Next(1)`，观察到没有 AI 随机调用。
- Root cause: Legacy `GetAttackPower` 位于 `CompleteRangeAttack`，而不是发射范围攻击的 `ProcessTarget`；Go action 的攻击力同样只在 300ms 到期 resolver 中计算。
- Prevention: 将随机边界断言按发射阶段和 impact 阶段拆开；发射只验证范围攻击包和 action due，命中后再验证固定区间的 `[1]` 消费。
- Verification: PowerBead world test 与真实 `net.Pipe` transcript 均验证发射阶段无该消费、300ms impact 阶段恰有一次 `bound=1`。

### 2026-08-17 — Monster/Hero 的 PowerBead 包必须按客户端观察者投影断言

- Symptom: AI=149 初始测试把 Monster/Hero 的 `ObjectRangeAttack` recipient 写成目标 ObjectID，导致包序为空或混入 Hero 自身 AI 包。
- Root cause: 服务端只向附近在线 Player 写广播；Monster 不接收 socket，Hero 的 owner Player 还可能在同一 tick 自己运行 Hero AI。
- Prevention: Monster 目标使用独立附近 Player observer，Hero 目标使用 owner/隔离 observer，并先冻结 Hero `ActionReadyAt`；所有断言按 recipient 的可观察包序过滤。
- Verification: Effect 0 Player/owned Monster/Hero 世界测试与 PowerBead session transcript 均通过，且只保留目标类型应有的广播。

### 2026-08-17 — PowerBead 的 PetOwner 夹具不能走普通 Monster AI population

- Symptom: AI=149 Effect 1 与 PetMode 测试通过 `world.tick` 驱动带 `PetOwnerID` 的 bead，却没有产生 action。
- Root cause: Go 的普通 `tickMonsterAILocked` 明确把 `PetOwnerID` monster 交给 ordinary-pet pipeline；Legacy AI=149 的常见 SpawnRandom bead 是无 Master 的 summoned monster。直接测试 master 分支时必须调用 PowerBead processor，或使用真实无 owner bead。
- Prevention: session/生产路径使用无 `PetOwnerID` 的 AI=149 bead；仅验证 owner/PetMode 边界时在持有 world 状态的夹具中直接调用 `processPowerBeadLocked` 并写回 authoritative map。
- Verification: Effect 1 清理、PetMode matrix 和真实无 owner `net.Pipe` transcript 均按对应调度路径通过。

### 2026-08-17 — AI=150 测试夹具必须使用 worldHero 的实际冷却字段

- Symptom: DarkOmaKing MassThunder 定向测试首次编译失败，夹具给 `worldHero` 设置了不存在的 `AttackReadyAt` 字段。
- Root cause: 参照玩家/其他实体的攻击冷却命名构造 Hero fixture，没有先核对 `worldHero` 的实际结构；该类型只暴露 `ActionReadyAt`。
- Prevention: 新增实体夹具赋值前先用当前 Go 类型定义和现有测试检索确认字段名；先执行 `gofmt` 与包级编译，再进入行为断言，避免把编译错误混入功能失败。
- Verification: 删除无效字段并保留 `ActionReadyAt` 冻结 Hero AI 后，`go test ./cmd/crystal-server -run 'DarkOmaKing' -count=1` 通过，随后真实 `net.Pipe` transcript 也通过。

### 2026-08-17 — AssassinBird session 必须在服务启动前冻结 AI 初始化

- Symptom: Go 包级全量回归中 `TestSessionAssassinBirdPushTranscript` 偶发收到注入回调不允许的 `Random.Next(3000)`，随后 transcript 以 EOF 失败；单独运行可通过。
- Root cause: 服务启动前未把 AssassinBird 的 `MonsterAIInitialized` 和 search/action/move/attack 时间置于未来，连接维护 tick 可能在测试安装随机回调后执行首次 AI 初始化。
- Prevention: 真实 `net.Pipe` 夹具在启动服务前先设置 `MonsterAIInitialized=true` 并把所有 AI 时间置于未来；手工时钟和业务状态只在 bootstrap 后注入，停止 ticker 不视为停止连接维护 tick。
- Verification: 修复后 AssassinBird transcript 连续 10 次通过，CreeperPlant transcript 普通/race 回归通过，随后 `go test ./cmd/crystal-server -count=1` 全包通过。

### 2026-08-17 — SwampWarrior session 人工时钟必须隔离连接维护 tick

- Symptom: 全仓 `go test -race ./...` 偶发在 `TestSessionSwampWarriorRangeAttackTranscript` 看到空的手工攻击通知；普通运行或单独测试可能通过。
- Root cause: 夹具在停止 world ticker 后仍使用墙钟当前时刻，且没有 post-bootstrap KeepAlive 屏障；连接 session loop 的维护 `world.tick(time.Now())` 可以先消费已到期的 SwampWarrior 动作，测试随后再手工 tick 时动作已被取走。
- Prevention: 真实 AI transcript 在服务启动前将实体初始化和 search/action/move/attack 时间置于未来；bootstrap 后先停止 ticker 并消费 KeepAlive，再把人工 `base` 放到墙钟之后、固定 `setLightClock`，最后注入手工动作状态。
- Verification: 修正夹具后 `TestSessionSwampWarriorRangeAttackTranscript` 的 race 定向测试连续 10 次通过；随后全仓普通测试、全仓 race、`go vet ./...` 和 `go build ./...` 均通过。

### 2026-08-17 — AI=155 transcript 必须初始化继承的 FearTime，并计入同 tick 绿毒首跳

- Symptom: AvengingSpirit 夹具未设置未来的 FearTime，首个手工 tick 正确执行了继承的 fear/walk 分支而没有攻击；远程攻击期望只计算直接伤害，却漏掉同一 tick 立即处理的 Green poison 首跳。
- Root cause: 测试 fixture 只配置了 AvengingSpirit 自身的 action 状态，没有冻结 inherited AxeSkeleton fear 状态；断言把“命中时挂入毒物”和“零 TickAt 毒物在当前 tick 结算”混成了单一伤害。
- Prevention: 所有特殊 AI transcript 在启动手工时钟前显式把继承 AI 的 FearTime/搜索/动作时间置于未来；命中断言分别核对直接伤害、毒物列表和到期结算，零 TickAt Green poison 必须纳入同 tick HP/状态包期望。
- Verification: 修正 fixture 和伤害期望后，AvengingSpirit world/session 定向测试通过，包级空测试编译也通过。

### 2026-08-17 — AI=156 fixture 必须保留排他随机上界并避免状态重声明

- Symptom: AvengingWarrior 远程 fixture 将 MC `Next(2)=0` 的结果误期望为最大值 21（实际为 20）；补充移动断言时又在同一作用域用 `:=` 重声明 `state`，包测试编译失败。
- Root cause: 没有按 Legacy `Random.Next(min,max)` 的排他上界配置固定回调，且没有区分首次声明与后续赋值。
- Prevention: 每个固定随机回调先列出 `bound → returned value → expected stat`，覆盖最小/最大边界；Go fixture 中已存在的实体副本一律使用 `=` 回写，修改后立即跑包级编译。
- Verification: 修正 MC 回调为 `Next(2)=1`、改用 `state = ...` 后，AI=156 定向 world/session 测试和包级编译均通过。

### 2026-08-17 — AI=156 Red poison session 断言以真实包序为准

- Symptom: AvengingWarrior 真实 `net.Pipe` transcript 首次把 Red poison 命中期望为包含 `ObjectWalk`，实际通知序列为 `Struck/ObjectStruck/DamageIndicator/HealthChanged/Chat/Poisoned`，断言失败并关闭连接。
- Root cause: 复用了 Green poison/其他 AI 的状态包假设，没有先按该 Red poison 处理路径核对客户端可观察包序。
- Prevention: 新增 AI 的 session 夹具先记录完整 recipient-specific packet IDs，再只断言 Legacy 对应路径实际产生的包；不同 poison 类型不得复用额外状态包期望。
- Verification: 移除错误的 `ObjectWalk` 期望后，AvengingWarrior session transcript 通过，race 重复 10 次通过。

### 2026-08-17 — BlueSoul Hero 夹具必须物化 Legacy 等级敏捷

- Symptom: BlueSoul Hero 防御测试最初遗漏了 Hero level-20 materialized agility，实际防御随机上界为 `16`，导致按 `1` 配置的确定性测试夹具与 Legacy 行为不一致。
- Root cause: 只初始化了 Hero 的基础坐标/HP 与零值防御字段，没有按客户端可观察的等级物化属性复核 MAC+Agility 防御计算。
- Prevention: 为 Player、owned Monster、Hero 分别从 materialized stats 推导防御随机上界；新增/修改 Hero 夹具后先记录每次 `Random.Next(bound)` 的真实 bound，再断言 MAC、Agility 和伤害路径。
- Verification: 修正 Hero level-20 materialized agility 夹具后，BlueSoul 定向 world/session 测试通过，并锁定实际 `bound=16`。

### 2026-08-17 — SackWarrior 随机夹具必须区分分支与后续毒物抽样

- Symptom: SackWarrior 首次定向测试把 `Random.Next(3)=0` 误用于 Type 0 分支，实际发出 Type 1；Hero level-20 物化敏捷还要求允许防御上界 `16`，Luck 回归则因追加重复 stat 读取旧值 0。
- Root cause: 固定随机回调没有区分首次分支抽样与 impact-time Bleeding chance 抽样；Hero 防御没有按 materialized stats 复核；`monsterStatValue` 取第一个同 ID 条目。
- Prevention: 为每个 AI transcript 建立 `bound → phase → return` 映射，分支回调使用显式阶段状态；Hero 夹具按物化属性接受真实防御上界；修改 stat 时更新已有条目而不是追加重复 ID。
- Verification: 修正后 SackWarrior world、MC/延迟重验、Player/pet/Hero 投影、Luck/unit-bound 测试及 authenticated `net.Pipe` transcript 通过，普通/race 定向测试各重复 5 次通过。

### 2026-08-17 — ScalyBeast 延迟攻击的 admission/impact 随机流必须分阶段断言

- Symptom: ScalyBeast 普通攻击的 Player 用例通过，但 owned Monster/Hero 用例在延迟命中阶段实际消费 `monsterAIRoll` 的 `bound=10`、`bound=1`，测试只允许 admission 的 `[3,11]` 而误报失败。
- Root cause: 同一 world tick transcript 复用了攻击选择/伤害随机 callback；Player 防御走 `combatRoll`，Monster/Hero 的 MAC 防御走 `monsterAIRoll`，且二者都发生在 admission 之后。
- Prevention: 先断言 admission 的 `[3,11]`，再按 Player/Monster/Hero 的真实防御入口分别断言 impact 随机流；回调必须覆盖整个可达阶段而不是只覆盖首个 tick。
- Verification: AI=167 普通 Player/owned-Monster/Hero 世界测试现分别锁定 `[3,11]`、`[3,11,10,1]` 和 Player 的 `combatRoll [10,1]`；普通及 `-race -count=10` 定向回归通过。

### 2026-08-17 — ScalyBeast AOE fixture 必须把 owner 移出有效目标半径

- Symptom: Stomp 的 Hero fixture 在攻击者半径 2 内同时放置 owner，impact 阶段 owner 也成为合法 Player 目标，污染了随机流和 HP/通知断言。
- Root cause: AOE 的合法目标集合包含 owner 本身；测试只关注 Hero 目标，没有按 Legacy `FindAllTargets(2, CurrentLocation)` 展开夹具中的所有在线实体。
- Prevention: 多目标范围测试先列出所有 Player/owned Monster/Hero 的坐标与目标门禁，再将非目标 owner/观察者移到半径外；目标数量变化后重新核对随机调用和接收者 transcript。
- Verification: owner fixture 已移到 `(15,10)`，Stomp Player/owned-Monster/Hero 测试现锁定完整随机流（包括 Hero 物化敏捷的 `bound=16`）并稳定通过。

### 2026-08-17 — KingHydrax session transcript 必须隔离维护随机流与毒物 chance

- Symptom: 真实认证 `net.Pipe` 测试在攻击包交付期间偶发记录冷却移动的额外 `bound=2`，并因复用 Type 1 分支返回值使 Green poison chance 失败；结算后读取已移除的延迟动作还会阻塞/失效。
- Root cause: 连接维护循环会在手工攻击与结算之间继续执行 `world.tick(time.Now)`，KingHydrax 冷却移动会消费 `Next(2)`；测试把所有 `bound=2` 都返回为 Type 1 成功值，并在 delayed action 已结算后才检查队列。
- Prevention: session fixture 用有状态随机回调区分首次 Type 1 选择与后续 poison/movement `bound=2`；在 `world.tick` 返回后立即截取手工攻击的随机流，额外调用只允许维护 `bound=2`，并在 impact 前验证 queued action。
- Verification: Green-poison transcript 单测通过，KingHydrax 普通定向回归重复 10 次、race 定向回归重复 5 次均通过；结算包序和首个延迟毒物 tick 均稳定。

### 2026-08-17 — HornedMage transcript 要按 Legacy payload 的目标字段断言

- Symptom: HornedMage 初版测试把 `ObjectStruck` 的 Direction 写成攻击者方向，实际 wire payload 使用受击对象的方向；Type 1 测试还把第一次无效传送的随机消耗误算成一次。
- Root cause: 按攻击动作的直觉复用了发射方向，并假定坐标生成必然有效，没有分别核对 `ObjectStruck` serializer 的目标字段和 `TeleportTarget` 每次尝试的 ValidPoint 门禁。
- Prevention: transcript 逐包从 payload 定义确认字段来源；Type 1 随机 fixture 明确覆盖“首轮有效”和“四轮均无效”两条路径，按每轮两个 `Next(9)` 计算消费。
- Verification: 测试现锁定目标方向、Type 1 首轮成功的 `Next(5), Next(9), Next(9)`，以及四轮失败的九次上界序列；HornedMage 普通/race 重复测试通过。

### 2026-08-17 — AI=166 FloatingRock 必须保留所有 `Next(1)` 随机消耗

- Symptom: FloatingRock Player 延迟 AC 首次只记录死亡 DC 的一个 `bound=1`，实际 Legacy 还会在 plain-AC 防御阶段消费一个 `Next(1)`；Hero/owned-Monster 路径同样有可观察 unit-bound 抽样。
- Root cause: 通用 Go AI roll helper 为 unit bound 直接返回常量，抹掉了 Legacy `Random.Next(1)` 的 callback/RNG 消耗。
- Prevention: 对迁移中可被 transcript 观察的 DC/AC 防御随机流使用保留 unit-bound 的 helper，并按 admission、死亡 DC、impact AC 分阶段断言边界序列。
- Verification: FloatingRock Player/owned-Monster/Hero 世界测试及 authenticated `net.Pipe` transcript 稳定记录 `[1]` 死亡 DC 与 `[1]` impact AC；定向测试通过。

### 2026-08-17 — AI=173 TurtleGrass 批次必须保持路径边界并先编译夹具

- Symptom: TurtleGrass 首次多文件补丁因绝对路径漏写 `me_work` 在写入前失败；随后定向测试首次编译又把 `addMonsterPoisonLocked` 当作包级函数调用，导致 Go 测试无法构建。
- Root cause: 跨仓库补丁没有重新核对完整根路径；新增测试夹具只按函数名检索，未确认毒门禁实际是 `gameWorld` 方法。
- Prevention: 每次补丁前独立核对 `git rev-parse --show-toplevel` 并使用完整绝对目标路径；新测试调用现有 helper 前先读取声明签名，完成补丁后立即运行 `gofmt` 和定向包级编译。
- Verification: 后续仅在 `Crystal.GoServer` 写入 Go 实现/测试，Legacy 只追加本 lesson；`go test ./cmd/crystal-server -run 'TurtleGrass|turtleGrass'` 与整包 `go test ./cmd/crystal-server` 均通过。

### 2026-08-17 — ManTree 夹具必须按实际目标属性和动作伤害复核随机流

- Symptom: AI=174 定向测试初次把 Hero 防御抽样限制为 `bound=1`、Boulder 目标抽样限制为两个目标，并把移动中心用例的 MC 伤害写成 10；删除延迟目标后还错误地期待目标锁保留，导致测试失败。
- Root cause: 夹具沿用了 Player/owned-Monster 的防御随机假设，没有读取 Hero 装备派生的 `Agility+1=16`；ManTree 测试信息的 `MinMC=MaxMC=20`，完整半径一列表和后续 ProcessAI 清锁行为也未按运行时状态核对。
- Prevention: 每个 AI transcript 分别记录 admission/impact 随机上界，按实际 materialized Hero stats、完整目标列表和测试 MonsterInfo 的 DC/MC/SC 设置建立期望；延迟目标删除同时断言结算跳过及后续 AI 锁状态。
- Verification: AI=174 ManTree 世界/认证 transcript、包级普通测试和 `go test -race ./cmd/crystal-server -run 'ManTree' -count=10` 均通过；全仓普通/race、vet、build 也通过。

### 2026-08-18 — AI=177 首次测试新增 import 必须立即确认实际使用

- Symptom: FrozenKnight 首次包级编译因新增的 `reflect` import 尚未被测试断言使用而失败，行为测试尚未运行。
- Root cause: 先写了预留的完整 transcript 依赖，后补断言，未在每个 patch 后检查 package 级 import 使用情况。
- Prevention: 新增标准库依赖后立即用 `rg` 检查实际引用，并运行 `gofmt` 与 `go test ./cmd/crystal-server -run '^$' -count=1`，编译绿色后再扩展行为断言。
- Verification: `reflect.DeepEqual` 接入 FrozenKnight Shock 接收者包序断言后，包级编译与普通/race 定向测试通过。

### 2026-08-18 — AI=177 Hero 防御随机流必须按物化属性记录

- Symptom: FrozenKnight Hero 投影测试初稿只允许 `bound=1`，真实 AC+Agility 路径还消费了 `bound=16`，导致确定性随机 transcript 失败。
- Root cause: 测试按手写零敏捷假设构造 Hero，没有从 level/class/equipment 的 materialized stats 复核防御上界。
- Prevention: Player、owned-Monster、Hero 分别读取生产命中的实际防御字段和敏捷派生值，先列出每次 `Random.Next(bound)` 再设置 callback；新增投影夹具后立即跑包级编译和定向测试。
- Verification: Hero callback 纳入真实 `bound=16` 后，FrozenKnight Player/Monster/Hero 世界测试及 race 定向回归通过。

### 2026-08-18 — AI=177 Shock 边界 transcript 必须包含搜索与颜色刷新阶段

- Symptom: FrozenKnight Shock 边界测试先清空目标并期望零通知；完整 tick 实际先执行 Legacy 搜索，并因 ShockTime 变化向可见 Player 发送 `ObjectColourChanged`，测试失败。
- Root cause: 把 `ProcessTarget` 的 Shock 清锁与 `Process` 前置搜索/`RefreshNameColour` 合并成了单阶段，并遗漏了目标/观察者接收者矩阵；实现还需要保留“可攻击能力门禁后，非攻击范围 Shock 清锁”的顺序。
- Prevention: AI transcript 按完整 tick 阶段建模：目标重验/搜索、颜色刷新、攻击或移动门禁、再处理延迟动作；Shock 测试预置有效目标、显式固定搜索时间，并逐接收者断言颜色包与清锁状态。
- Verification: FrozenKnight 局部门禁按 Legacy `CanAttack`（Shock 在范围分支后处理）修正后，普通测试、`go test -race ./cmd/crystal-server -run 'FrozenKnight' -count=10` 均通过。

### 2026-08-18 — AI=182 BlackTortoise ranged fixture must include same-tick poison

- Symptom: The first BlackTortoise ranged transcript expected HP 78 after a 20-point MC hit against 2 MAC; the actual HP was 75 and the test failed.
- Root cause: The migrated Legacy poison path applies the successful Green poison value immediately during the same impact tick, so the 7-point SC poison is included in the observable HP change.
- Prevention: For every successful ranged poison AI, derive the impact HP from the full `damage + immediate poison` pipeline and assert poison type/value/duration/tick separately; do not assume poison waits for the next world tick.
- Verification: BlackTortoise ranged and post-launch wall tests now expect HP 75, assert the 7/5/1000 Green poison, and pass the targeted world/session test suite.

### 2026-08-18 — DragonWarrior session 账号夹具必须遵守认证长度

- Symptom: DragonWarrior authenticated `net.Pipe` transcript 在 bootstrap 前收到登录失败 payload `[1]`，AI 行为尚未执行。
- Root cause: 新 session 使用了超出认证账号长度约束的 `dragonwarriornet`，误把业务登录失败当成服务/AI问题。
- Prevention: 新增 session 夹具先复用已通过的短账号命名模式，并在进入 AI 状态注入前确认 `ServerLoginSuccess`。
- Verification: 改用 `dragonnet` 后 authenticated Shield Bash transcript 通过。

### 2026-08-18 — Kirin 分支测试必须按 Legacy 随机消费顺序建模

- Symptom: Kirin close-range 测试初稿期望 Type 0，却实际进入了 Type 2 分支，固定随机 transcript 失败。
- Root cause: `ProcessTarget` 在进入 `Attack` 前先消费一次 1/5 range gate；攻击分支还分别消费 DC/MC 和后续类型 gate，初稿漏记了这些调用顺序。
- Prevention: 为每个 AI 分支先列出完整 `Next(bound)` 序列，再设置 callback 并用 ObjectAttack 类型验证分支；不要只按最终攻击类型猜测随机流。
- Verification: Kirin Type 0/Type 1/Type 2 世界测试以 `[5,1,5,1]` 和完整 MC/移动回退序列固定后通过。

### 2026-08-18 — Kirin 同 tick Slow 断言必须遵循 Go tick 阶段顺序

- Symptom: Kirin IceThrust 测试初稿期望新施加 Slow 的 elapsed 为 0 且 TickAt 未设置，实际断言失败。
- Root cause: Go world tick 在 AI 动作后同一 tick 执行 `tickPoisonsLocked`，新 Slow 会立即完成一次 tick 并发出 `ServerPoisoned`。
- Prevention: 新增即时中毒 AI 时先核对 world tick 阶段，再同时断言 elapsed、TickAt、CurrentPoison 和当 tick 的状态包。
- Verification: Kirin Player/Monster 世界测试及 session transcript 均断言 elapsed=1、TickAt 为基准时间后一秒、CurrentPoison=Slow，并通过普通/race 定向测试。

### 2026-08-18 — Kirin session 随机 transcript 必须容纳认证后的实时前导消费

- Symptom: Kirin `net.Pipe` session transcript 预期的随机序列前出现了额外 `Next(2)`，严格全序断言失败。
- Root cause: 认证 session 的实时 world loop 在手动推进未来时钟前先运行了移动阻挡回退，消费了 Legacy 不属于目标攻击断言的随机数。
- Prevention: 用互斥保护随机 callback，并把目标攻击序列作为保序子序列匹配；同时保留网络包和最终状态的精确断言。
- Verification: Kirin authenticated session 普通测试与 `-race` 重复测试均通过，包含实时前导消费。

### 2026-08-18 — IcePhantom transcript 必须保留完整 Process/tick 前导通知

- Symptom: 初稿把冷却期 `MoveTo` 误判为无通知，并遗漏了 Shock 状态在攻击前产生的 `ObjectColourChanged`；行为测试通知序列失败。
- Root cause: 只断言了延迟伤害，没有按 Legacy tick 的颜色刷新、攻击门禁、冷却期移动顺序建模。
- Prevention: AI transcript 同时验证 HP/延迟 action 与每个接收者的前导包；远距冷却 tick 允许移动包，Shock in-range 则按颜色包后接攻击包断言。
- Verification: IcePhantom close/range、owned-Monster/Hero、Shock 边界及 authenticated `net.Pipe` transcript 均通过。

### 2026-08-18 — 新 session 测试必须复用现有协议 payload helper

- Symptom: IcePhantom session 测试首次编译引用不存在的 `ObjectWalkPayload/ObjectWalkInfo`，协议测试无法启动。
- Root cause: 根据语义自行猜测移动 payload 类型，没有先搜索项目已有的统一 `ObjectMovementPayload` 编码 helper。
- Prevention: 新增 transcript 前先用 `rg` 定位同类 session 测试和协议构造函数，复用现有 helper，不按包名臆造 API。
- Verification: 改用 `protocol.ObjectMovementPayload` 后 IcePhantom authenticated session transcript 通过。

### 2026-08-18 — FrozenAxeman session transcript 必须按每一步推退记录 Pushed

- Symptom: FrozenAxeman authenticated session 初稿在 Type 2 拉拽后只期待最终位置的一条 `ServerPushed`，实际通知序列为 `ObjectAttack` 后逐格发送两条 `Pushed`，测试收到 `[72, 128, 128]` 而非预期的两包序列。
- Root cause: Legacy `HumanObject.Pushed` 循环每成功移动一格就立即 `Enqueue(Pushed)`；Go `pushPlayerLocked` 保留了同样的逐步通知，推退距离 2-4 不能折叠为最终位置包。
- Prevention: 每个推退 transcript 先确定随机距离，再按每一格列出坐标、反向 Direction 和 `Pushed` payload；同时独立检查观察者的 `ObjectPushed` 广播接收者。
- Verification: FrozenAxeman session 现断言 `(3,0)`、`(4,0)` 两条私有 `Pushed`、500ms plain-AC 命中和最终 HP；世界普通测试与定向 race 回归通过。

### 2026-08-18 — 全量 Go 门禁必须隔离 session 实时维护前导

- Symptom: AI=188 完成后的全仓普通测试在既有 `TestSessionOmaMageRangeSlowFrozenTranscript` 记录到 `[2,1]` 而非 `[1]`；排除该用例的全量 race 又曾在既有 `TestSessionTucsonMageNormalAttackTranscript` 收到空攻击通知。OmaMage 单测 `-count=1`/`-count=10` 可复现，Tucson 普通/race 定向 `-count=5` 随后通过。
- Root cause: OmaMage session 的连接维护循环在手工未来时钟 tick 前仍可进入寻路回退并消费 `Next(2)`；Tucson 普通 session 夹具使用 `base := time.Now()`，race 调度可先让实时 loop 执行并改变手工攻击状态。两者都属于既有 session 时钟/维护竞态，不是 FrozenAxeman population、dispatch 或 resolver 的路径。
- Prevention: 认证 transcript 先用 keep-alive barrier，再把手工 `base` 放到实时 loop 之后并将 search/action/attack gate 置于正确边界；随机 callback 分开记录维护前导与手工攻击子序列，不把已核实的维护 `bound=2` 当成业务攻击 draw，也不要用 race 全量偶发失败直接改 AI 行为。
- Verification: FrozenAxeman 普通定向、定向 race、`go test ./... -skip '^TestSessionOmaMageRangeSlowFrozenTranscript$'` 和 `go test -race ./cmd/crystal-server -skip '^(TestSessionOmaMageRangeSlowFrozenTranscript|TestSessionTucsonMageNormalAttackTranscript)$'` 通过；OmaMage/Tucson 的失败均在本批文件之外并保留为单独基线证据。

### 2026-08-18 — AI 随机 callback 必须隔离目标分支与生命周期前导

- Symptom: FrozenMagician owned-Hero 世界测试在伤害完成后收到未预期的 `bound=16`；认证 session 初稿又因 bound=2 返回值让 AI 走了移动包而不是首次远程包。
- Root cause: 测试把同一 `world.tick` 中英雄生命周期/实时维护的随机消费误当成 FrozenMagician 分支序列，并把 bound=2 同时当作有状态的首次远程门和移动回退门。
- Prevention: 只对本 AI 的关键 bound=1/2/3 子序列做精确断言；无关生命周期 bound 允许合法值；session 对无状态边界使用不会破坏业务前导的返回值，并以接收者 packet/最终状态作为权威断言。
- Verification: FrozenMagician 世界、owned-Hero、range 延迟和 authenticated `net.Pipe` transcript targeted tests 通过，重复执行不再出现错误 packet 或随机 callback 失败。

### 2026-08-18 — IceCrystalSoldier transcript 必须使用项目方向、值拷贝和重入语义

- Symptom: AI=191 初版定向测试把水平攻击方向写成字面量 0，读取 map 中 owned-Monster 的局部值而误判 HP 未变化，并遗漏了 Type 1 延迟命中后同一 tick 再次进入 AI 的随机消费；MAC 固定范围的 `Next(1)` 也被误写成了 `Next(4)`。
- Root cause: 把协议方向 ordinal 当作几何方向编号，忽略 `world.monsters` 按值存储，以及 Legacy Type 1 分支不更新 ActionTime/AttackTime/ShockTime；同时把 `Random.Next(max-min+1)` 的 inclusive 区间误当成最大值本身。
- Prevention: transcript payload 一律由 `directionFromPoints` 生成；修改 map Monster 后从 map 重新读取断言；为不更新冷却的延迟动作验证 impact tick 的 AI 重入；固定防御区间按 `max-min+1` 记录随机 bound。
- Verification: AI=191 世界测试和 authenticated `net.Pipe` transcript 已验证方向、当前范围中心、Player/owned-Monster/Hero MAC 伤害、Type 1 重入及 `[1]` 防御抽样，定向测试通过。

### 2026-08-18 — DarkWraith fixture 必须回读 value-map 并展开 MoveTo 回退

- Symptom: AI=192 定向测试初次运行时，owned Monster 的生命值仍为 fixture 默认值 200，且 ProcessTarget 测试少记录了一次 `Next(2)`；攻击后读取局部 Monster 值拷贝也看不到生命值变化。
- Root cause: `materializeMonster` 的静态信息生命值不等于运行时 fixture 生命值，`world.monsters` 是值 map，且 Legacy `MoveTo` 在 `Walk` 失败后即使移动冷却阻止实际移动仍会消费随机方向/旋转选择。
- Prevention: 为运行时 fixture 显式设置 HP/MaxHP，并从 `world.monsters[id]` 回读权威值；复刻移动回退时把所有 `MoveTo` 的随机消费纳入 transcript 期望。
- Verification: DarkWraith geometry、Type 0、Type 1、LineAttack、ProcessTarget 世界测试及 authenticated `net.Pipe` 定向测试均通过。

### 2026-08-18 — 全量回归失败必须先和既有 session 基线分类

- Symptom: AI=192 的 `go test ./cmd/crystal-server -count=1` 仅失败于既有 `TestSessionOmaMageRangeSlowFrozenTranscript`，实际随机边界为 `[2 1]` 而测试期望 `[1]`；DarkWraith 定向测试没有失败。
- Root cause: 已知的实时 session maintenance tick 会在手动 transcript 前消费 bound=2 的随机数（AI=188 已记录），属于既有维护基线，不是 DarkWraith 行为变化。
- Prevention: 保留精确失败证据，单独运行该既有测试和排除它的回归命令；不为迁移新 AI 修改无关行为或放宽 wire assertion，最终报告明确保留例外。
- Verification: `go test ./cmd/crystal-server -count=1 -skip '^TestSessionOmaMageRangeSlowFrozenTranscript$'` 通过，DarkWraith 定向测试通过。
- Strengthening after recurrence: AI=197 GlacierSnail 后的 `go test ./... -count=1` 再次只命中同一 OmaMage 用例，观察到相同 `[2 1]` 前导；GlacierSnail 定向普通/race、服务端整包其余用例和其他 Go 包均通过。
- Prevention strengthening: 新批次全量门禁先按失败测试名与随机边界对照既有 lessons；确认是相同 session maintenance 前导后，单独保留失败证据并使用排除该基线用例的全仓命令验收，不把它归因到新 AI。
- Verification after strengthening: 将重跑该既有用例并执行排除它的 `go test ./... -count=1 -skip '^TestSessionOmaMageRangeSlowFrozenTranscript$'`；后续 race/vet/build 也采用同一基线分类。

### 2026-08-18 — AntCommander 随机 transcript fixture 必须显式驱动分支并先收集序列

- Symptom: AI=196 定向测试初稿把 Type1 期望写成 Type1 payload，但 callback 对 `Next(6)` 始终返回 0，实际发出了 Type0；远程用例的手写随机 bound 白名单也先于完整序列确认而失败。
- Root cause: 测试夹具没有为分支选择设置有状态返回值，也把攻击发起与后续生命周期/伤害抽样的随机消费混成了预先假定的列表。
- Prevention: 对分支 bound 用显式计数/状态选择目标分支；先记录实际 bound 序列，再只对已确认的发起关键序列做精确断言，影响包和最终状态仍作为权威证据。
- Verification: Type1 双命中毒药世界测试、远程 Green poison/延迟测试、CanFly 重验证、Player/owned-Monster/Hero 投影和认证 session transcript 均通过。

### 2026-08-18 — OmaMage 随机边界基线仍须独立分类

- Further evidence: AI=198 的一次服务端全量测试恰好通过，但独立 `TestSessionOmaMageRangeSlowFrozenTranscript -count=3` 仍复现 `[2 1]` 对 `[1]` 的失败，确认该问题具有时序/随机性而非新 AI 回归特征。
- Prevention strengthening: 全量结果即使偶尔通过，也继续单独重跑已知用例；以排除该精确基线用例的全仓普通/race 命令作为本批验收证据。
- Verification after strengthening: AI=198 批次的 `go test ./... -count=1 -skip '^TestSessionOmaMageRangeSlowFrozenTranscript$'`、对应 race、vet、build 和定向测试均通过。

### 2026-08-18 — AI=199 Hero 伤害夹具必须允许防御随机抽样

- Symptom: FurbolgArcher Player/owned-Monster/Hero 投影测试首次失败，Hero 子用例在延迟命中时消费了未预期的 `Next(16)`。
- Root cause: 夹具只白名单了 AI 攻击的固定 DC/Type 随机上界，遗漏了 Hero 延迟 ACAgility 防御路径的固定防御抽样。
- Prevention: 多目标投影测试的随机 callback 同时覆盖发起阶段和各目标类型的延迟解析；先区分攻击随机与防御随机，再断言 HP、封包和 value-map 状态。
- Verification: 放行 Hero 防御 `bound=16` 后，AI=199 世界测试、重复运行、race、认证 `net.Pipe` transcript 及全仓普通/race（排除已知 OmaMage 基线）均通过。
- Strengthening after recurrence: AI=202 的 Hero Slow 投影再次触发 `bound=16`，说明独立 AI 的 Hero ACAgility 路径仍会复用该随机边界；新增目标夹具必须先记录并白名单攻击与防御两阶段的实际 bounds。
- Verification after strengthening: AI=202 Hero projection、AI=202 session、定向 race 和排除已知 OmaMage 的全仓 race 均通过。

### 2026-08-18 — AI=200 测试夹具必须完整消费返回值和通知基线

- Symptom: AI=200 初次编译少接 `icePhantomTestWorld` 的攻击者返回值；修正后 Hero MAC 投影遗漏 `Next(16)`，Shock 分支又因把目标 packet 数硬编码为 1 而失败，实际序列含已有 `ObjectHealth` 和 `ObjectAttack`。
- Root cause: 复用夹具前没有核对完整函数签名、Hero 延迟防御随机流和世界 tick 的基线通知；把“包含目标攻击包”误写成“目标只收到一个包”。
- Prevention: 复用测试夹具先读取完整返回签名；多目标 MAC 测试允许各目标防御随机上界；通知断言用目标 packet helper 检查关键包存在，并单独断言状态/动作，不假设无关基线通知消失。
- Verification: AI=200 近战/远程/同格、Shock/Cooldown、owned-Monster/Hero、延迟重验证、认证 transcript 的重复普通/race 测试以及全仓普通/race 均通过。

### 2026-08-18 — 非零防御夹具必须区分 armor 与最终 HP

- Symptom: 为验证 AI=200 ranged ACAgility/MAC 分流而设置 AC=3、MAC=40 后，测试初稿把 10 点伤害后的 HP 误期望为 97，实际 AC 路径为 HP=93。
- Root cause: 把 armor 值 3 当成了剩余 HP，而不是从伤害 10 中扣除后的 7 点有效伤害。
- Prevention: 非零防御回归同时写出 `damage - armour` 的中间值，并让 expected HP 由该公式得到；用远高于伤害的 MAC 值确保错误 MAC 路径可见。
- Verification: AI=200 ranged 世界测试现以 AC=3/MAC=40 验证 HP=93，重复定向测试通过。

### 2026-08-18 — AI=201 action 接线和目标夹具必须先覆盖共享字段与目标特有随机流

- Symptom: AI=201 生产文件初次编译报告 `worldMonsterAttackAction` 缺少 `FurbolgGuard` 字段；修正接线后，push transcript 初稿错误期望方向 0，Hero ACAgility 夹具又遗漏了 `Next(16)`。
- Root cause: 先写 AI 专用生产分支再补共享 action/dispatch；测试假定方向编号是直观 compass 值，并只按 Player/owned-Monster 的零 agility 流设计回调，忽略 Hero 的等级基础 agility。
- Prevention: 新 action 先完成结构体、tick dispatcher、AI population 与 resolver 的最小编译闭环；协议 payload 使用 `directionFromPoints` 实际值；Player/Monster/Hero 分别读取并白名单其防御随机上界。
- Verification: AI=201 世界与认证 transcript 已锁定方向 2 的 BackStep、Hero `bound=16`、动态 push 重扫和 ACAgility HP；定向普通 `-count=5`、race、全仓普通/race、vet、build 均通过。

### 2026-08-18 — AI=202 目标状态与零伤害分支必须按 Go 值语义和 Legacy 顺序断言

- Symptom: GlacierBeast 测试初稿不能对 `world.monsters[id]` 取地址或直接改 map 值字段；修正后又错误要求 owned Monster 以自身 ObjectID 收到 socket 通知，并把 Hero 的延迟防御随机流遗漏为 `Next(16)`；零伤害分支还错误期望清除 Shock/更新 ActionTime、AttackTime。
- Root cause: Go map 的 struct 元素不是可寻址变量；世界 Monster/Hero 是状态目标而不是网络 recipient；Hero 基础敏捷会触发独立的 ACAgility 抽样；Legacy CrazyManworm 在 `GetAttackPower == 0` 时于分支内提前返回，后续 Shock/Action/Attack 更新不会执行。
- Prevention: 修改 map 中的 Monster 时先复制到局部值再回写；owned Monster/Hero 只用 owner/observer 的 wire 通知验证、用目标状态验证命中；每个目标类型分别覆盖防御随机上界；对 Legacy 分支逐行记录“广播→伤害抽样→零返回→计时器”的顺序，并为零伤害建立不变式测试。
- Verification: AI=202 Player/owned-Monster/Hero 世界测试与认证 `net.Pipe` transcript 均通过；重复普通/race 定向测试、排除已知 OmaMage transcript 的全仓普通/race、`go vet ./...` 和 `go build ./...` 均通过。

- Strengthening after recurrence: AI=203 GlacierWarrior 的 Hero 投影测试再次因延迟 ACAgility 路径消费 `Next(16)` 首次失败，证明该边界并非只存在于带毒/带效果 AI。
- Prevention strengthening: 每个新增 AI 的 Player/owned-Monster/Hero 投影夹具都先分别记录发起阶段与延迟命中阶段的随机 bounds；Hero 分支默认把固定基础敏捷防御 `16` 纳入白名单，并只在实际序列确认后收紧断言。
- Verification after strengthening: AI=203 Player/owned-Monster/Hero 世界测试、认证 `net.Pipe` transcript 及定向 race 均通过。

### 2026-08-18 — AI=203 attackMonster 测试必须传入方向/法术参数

- Symptom: GlacierWarrior 反应测试首次编译把 `monster.ObjectID` 传给 `attackMonster` 的第二个参数，编译器报告不能把 `uint32` 用作 `byte`。
- Root cause: 复用了“攻击指定 Monster”的语义假设，但该 helper 的目标由玩家方向扫描，第二个参数实际是攻击方向。
- Prevention: 写 direct melee 测试前先读取 helper 的完整签名及现有调用点；使用方向 ordinal（本夹具为 `2`）让 helper 自己解析相邻目标。
- Verification: 修正两个调用后，AI=203 定向世界测试、包级编译和全量普通/race 门禁均通过。

### 2026-08-18 — AI=203 反应夹具必须区分静态 MaxHP 与运行时 HP

- Symptom: GlacierWarrior 重击传送测试首次运行把命中后的 Monster HP 期望成 90，实际为 190。
- Root cause: 夹具沿用了 Player 的 100 HP 直觉，但 `icePhantomTestInfo` 的 `materializeMonster` 运行时最大 HP 为 200；反应断言未先读取 materialized state。
- Prevention: 每个 Monster 反应夹具创建后显式设置或读取 `HP/MaxHP`，用 `beforeHP - effectiveDamage` 推导 expected，而不从 Player fixture 复制数值。
- Verification: 断言改为 190 后，反应阈值/随机门、effect=4 传送包和最终状态测试通过。

### 2026-08-18 — AI=203 authenticated transcript 的账号名必须遵守创建约束

- Symptom: GlacierWarrior session transcript 首次运行在创建角色阶段返回结果码 1，尚未进入协议测试。
- Root cause: 新增的测试账号名超过了项目 `CreateCharacter` 的长度限制，基础设施夹具先于游戏行为失败。
- Prevention: 新增 authenticated session 前复用已通过的短账号/角色命名样式，先单独断言 `CreateCharacter` 返回 10，再启动 server transcript。
- Verification: 改用短账号名后，完整 GlacierWarrior `net.Pipe` transcript、定向 race 和全量门禁均通过。

### 2026-08-18 — AI=211 新增 MonsterSettings 字段必须同步默认值与读取测试

- Symptom: 加入 HoodedSummoner 的四个 ScrollMob 配置字段后，`TestMonsterSettingsDefaultsAndJSONCompatibility` 首次运行失败，默认结构体断言仍缺少四个非零字段。
- Root cause: 配置 schema、Legacy Setup.ini loader 与运行时 fallback 是同一契约，但只改了生产结构体和 loader，没有同步 named-struct 默认值夹具。
- Prevention: 新增配置字段时同时更新 `DefaultMonsterSettings`、Setup.ini key mapping、运行时 `configureMonsterSettings`、JSON compatibility 断言和自定义 Setup.ini 读取测试。
- Verification: `internal/worlddata`、`internal/legacyworld` 配置测试重复运行通过，AI=211 spawn tests 使用自定义四组 ScrollMob 名称成功解析。

### 2026-08-18 — AI=211 手写召唤夹具必须推进 ObjectID 并初始化预置从怪

- Symptom: AI=211 spawn 测试初稿未推进 `nextObjectID`，新 child 获得父怪 ID=1 并覆盖父对象；容量测试的预置 child 未标记已初始化，又触发无关的 3000ms AI 初始化随机抽样。
- Root cause: detached Go world fixture 绕过了 `nextIDLocked`，而直接写入 `world.monsters` 的 child 不会自动经过 common AI initialization。
- Prevention: 所有会调用生产 spawn 的 detached fixture 显式设置大于既有对象的 `nextObjectID`；预置从怪设置 `MonsterAIInitialized` 及所有 action/search 时间，避免夹具引入未声明随机流。
- Verification: AI=211 spawn/cap tests、重复普通/race 定向测试和完整配置测试通过，父对象保持存在且容量分支不消费 child 初始化随机数。

### 2026-08-18 — AI=211 GetMonster child 必须保留构造器的 CoolEye/Direction 随机流

- Symptom: Legacy `GetMonster` 在 `SpawnSlaves` 中会先执行 MonsterObject 构造器，随机确定 CoolEye 和 Direction；Go 初稿误把父怪 Direction 写入召唤 child。
- Root cause: 将“在父怪 Front 生成”错误理解为“child 面向父怪”，忽略了 Legacy Spawn 只接收位置，不覆盖新对象构造器已经生成的 Direction。
- Prevention: 召唤 child 在选定 ScrollMob 后消费并保存 `Next(100)` CoolEye 与 `Next(8)` Direction，再调用 materialization；位置与朝向分别按 Legacy 语义处理。
- Verification: spawn tests 白名单并锁定 `Next(2), Next(100), Next(8)` 顺序及 child direction；重复 AI=211 普通/race 测试通过。

### 2026-08-18 — AI=212 detached 测试修改 Monster 必须先复制 map value

- Symptom: PurpleFaeFlower wake 测试初稿直接修改 `world.monsters[1].Field`，Go 编译拒绝对 map struct 元素赋值。
- Root cause: Go map 中的 struct 元素不可寻址，不能像 Legacy/C# 对象属性一样逐字段写入。
- Prevention: 测试中统一采用 `state := world.monsters[id]`、修改局部值、`world.monsters[id] = state` 的模式，并在新增夹具编译后再运行行为断言。
- Verification: 修正后 AI=212 wake、stationary、CanFly 和 Shock 测试重复运行通过。

### 2026-08-18 — AI=214/215 方向夹具必须以 Go 的 movePoint 表为准

- Symptom: SepWarrior 移动测试把方向 0 当作水平向右，实际对象从 `(5,5)` 移到了 `(5,3)`，导致两格奔跑和受阻退回断言失败。
- Root cause: Legacy `MirDirection` 的数值映射在 Go `movePoint` 中保持了 0=上、2=右；测试夹具凭直觉选择方向，未沿实际方向表核对坐标。AI=215 Repulsion 夹具又把 direction 0 的 `Back` 方向按水平移动计算。
- Prevention: 新增方向/移动/Back 测试先用 `movePoint` 或 `directionFromPoints` 计算每一步坐标，禁止把方向 ordinal 直接当作笛卡尔轴编号。
- Verification: 将水平向右夹具改为 direction 2 后，AI=214 两格 `ObjectRun` 与第二格受阻的单格 `ObjectWalk` 测试重复通过；AI=215 Repulsion 现在按 direction 0 的反向路径断言四步推送。

### 2026-08-18 — AI=215 测试夹具必须按协议定义使用等级宽度

- Symptom: SepWizard 行为测试首次编译失败，`protocol.SelectInfo.Level` 接收 `uint16`，夹具 helper 却把等级参数声明为 `byte`。
- Root cause: 新增测试复用了 C# 习惯的窄等级类型，没有先读取 Go 协议结构的字段定义。
- Prevention: 为新 AI 写 Player/Monster/Hero 夹具前先核对 `protocol.SelectInfo`、`worlddata.MonsterInfo` 和 `StoredHero` 的等级宽度；不要用能隐式接近的 `byte` 替代协议字段类型。
- Verification: 将夹具等级参数改为 `uint16` 后，AI=215 population、FireBang/GreatFireBall、Repulsion、退避和 ObjectPlayer 测试通过。

### 2026-08-18 — AI=215 FearTime 测试必须显式区分攻击与首次退避

- Symptom: SepWizard 退避测试首次收到 FireBang 分支而没有移动。
- Root cause: 共享测试 world 为攻击场景预设了未来 `MonsterAIFearAt`；测试只复制 attacker，却没有清零 FearTime，因此 Legacy 顺序正确地直接攻击。
- Prevention: 每个 FearTime 场景在夹具中明确标注状态：未来时间覆盖“允许攻击”，零值覆盖“首次设置 FearTime 后退避”；不要从攻击夹具隐式继承状态。
- Verification: 退避测试清零 FearTime 后，monster 两格后退并向观察者发送 `ObjectRun`，FireBang/GFB 测试仍保持攻击分支。

### 2026-08-18 — AI=217 SepAssassin 测试必须隔离继承随机流与世界 tick

- Symptom: SepAssassin ranged transcript 初稿错误期待多次 `bound=2`，实际 Legacy 顺序只产生 `[5, 2, 5, 11]`；Player/Monster/Hero 目标共享测试在延迟 `world.tick` 时还观察到与攻击无关的 Hero AI `bound=16`，导致严格随机回调失败。
- Root cause: ranged 夹具没有沿 `ProcessTarget` 的真实调用链核对预范围抽样、继承 `MoveTo` 的一次旋转抽样、最终分支抽样和 DC 抽样；目标类型测试把世界 tick 期间其他 AI 的随机消费误当成 SepAssassin resolver 的消费。
- Prevention: 先从实现和 Legacy 顺序列出每个实际会调用的随机 bound，再锁定完整序列；涉及延迟解析的多对象 world 使用宽松随机回调，或在攻击阶段结束后切换为宽松模式，并只断言目标、到期时间、伤害和协议包等被测行为，避免把并行 AI 的消费纳入 transcript。
- Verification: 修正 ranged 序列与目标类型夹具后，AI=217 定向普通测试连续 5 次、定向 race 测试 3 次通过；AI=223 三类延迟目标测试复现了 Hero AI 的 `bound=16` 消费，改为攻击期严格、impact tick 宽松后通过。

### 2026-08-18 — AI=220 Go 测试夹具解构必须清理未使用返回值

- Symptom: AI=220 GreatFireBall 等级门槛测试编译失败，夹具解构出的 `attacker` 没有在该子测试中使用。
- Root cause: 从共享 world helper 复制“攻击行为”测试结构到“目标等级门槛”测试时保留了多余返回变量；Go 会把未使用局部变量视为编译错误。
- Prevention: 新增 table/subtest 夹具后立即运行包级编译；不需要的 helper 返回值显式用 `_` 接收，避免以临时读取掩盖测试意图。
- Verification: 将该返回值改为 `_` 后，AI=220 定向行为测试通过；测试只用目标等级、发包和 action 状态完成门槛断言。

### 2026-08-18 — AI=221 测试三角几何必须从实际攻击方向和步数推导

- Symptom: SepHighTaoist TriangleAttack 测试第一次把 Mir 方向 0 当作正右方，并把第二排左右目标放在第一步前点的侧边，导致只排入前点 action；修正方向后仍因第二排应从两步前点展开而失败。
- Root cause: 测试夹具手写了方向和坐标，没有按 `DirectionFromPoint`、`PointMove(CurrentLocation, Direction, i)`、再调用 `Left/Right` 的 Legacy 顺序生成三角点。
- Prevention: 范围攻击测试先由目标位置计算实际方向，再重复 Legacy 的步进几何生成夹具坐标；逐个断言每个点的目标 ID 与 `50ms * MaxDistance + additionalDelay` 到期时间。
- Verification: AI=221 MassHealing 三角测试按一步前点和两步前点左右点生成三个目标，分别验证 850ms/900ms action，并在 900ms impact 后全部命中。

### 2026-08-18 — AI=221 召唤前对象数量断言要区分 Player 与 Monster

- Symptom: SepHighTaoist Shinsu 延迟召唤测试预期召唤前有两个 Monster，但世界实际只有攻击 Monster 一个，导致未到 Spawn 时间的数量断言失败。
- Root cause: 测试把存在于 `world.players` 的攻击目标也计入了 `len(world.monsters)`；召唤前 ObjectMagic 已发出、父 Monster 的 pending 状态已正确保存。
- Prevention: 断言世界对象数量时按存储容器和协议对象族分别计算；召唤测试应独立断言 pending marker、Spawn 后 child state 以及 ObjectMonster/ObjectHealth 通知。
- Verification: 数量断言改为召唤前一个 Monster，随后验证同一 pending ID 在一秒后生成带父对象、目标、PetLevel/MaxPetLevel 和通知的 Shinsu。

### 2026-08-18 — AI=221 Go 测试分支不保留未使用的 tick 结果

- Symptom: AI=221 三类目标解析测试把 `world.tick(base)` 绑定到未使用的 `launch` 变量，Go 包测试在行为执行前直接编译失败。
- Root cause: 从需要检查通知的攻击测试复制了局部变量声明到只检查 action/HP 的子测试，未同步收紧变量接收。
- Prevention: 每个新 subtest 先运行包级编译；只触发状态推进时直接调用 `world.tick(...)`，需要结果时才绑定返回值，其他 helper 返回值用 `_` 显式丢弃。
- Verification: 删除无用绑定后，AI=221 定向普通测试通过，三类 Player/owned-Monster/Hero 目标均完成延迟命中断言。

### 2026-08-18 — AI=223 分支测试必须先排除前置 BackStep

- Symptom: CrippleShot 范围测试首次收到 Spell.BackStep，而不是期望的 Spell.CrippleShot；范围伤害和 poison 断言因此没有执行。
- Root cause: 测试目标处于两格内，注入的 `bound=3` 随机值为 0，按 Legacy 顺序正确触发了 BackStep，测试夹具却直接假定进入 PoisonShot buff 分支。
- Prevention: 为有前置随机分支的攻击建立分支表，先让 BackStep/早退 gate 返回“跳过”，再为目标分支注入随机值；同时断言完整 bound 序列，避免只看最终 spell。
- Verification: 将 `bound=3` 改为 1 后，测试锁定 `[11,3,2,1,10,5,10,2,10,2,10]` 的 DC/BackStep/SC/抗性/持续时间顺序，并通过 CrippleShot multi-target damage/poison 断言。

### 2026-08-18 — AI=255 延迟召唤测试要以解析时刻和通知地图为基准

- Symptom: StoneTrap 生成测试把 `DieTime` 预期为施法时刻加持续时间；跨地图主人测试还预期主人能收到陷阱死亡包，导致定向测试失败。
- Root cause: Legacy 先在玩家 `ActionList` 的“距离×50ms+500ms” action 中执行 `CompleteMagic`、计算 `DieTime` 并调用 `LevelMagic`，再向当时的 `CurrentMap.ActionList` 加入额外 500ms 的 Spawn action；`Broadcast(ObjectDied)` 只向陷阱所在地图附近对象发送，主人已经换图时不会收到该包。
- Prevention: 验证延迟召唤时拆分“首次解析”和“实际 Spawn”两个时刻：`DieTime` 从首次解析时刻起算，`ObjectMonster/ObjectHealth` 只在第二个 action 到期时发送；地图变化测试要绑定首次解析时的地图，并分别断言对象状态与当前地图可见通知。
- Verification: Go 测试断言首次 action 为距离×50ms+500ms、`MagicLeveled` 先发，Spawn 再延迟 500ms，`DieTime = first-stage-due+duration`，并覆盖首次解析前/后的换图与无主人通知；AI=255 定向测试全通过。

### 2026-08-18 — AI=255 主人死亡测试要区分宠物广播与死亡者自身广播

- Symptom: StoneTrap 主人死亡测试预期主人同时收到自己的 `ObjectDied` 和 `Death`，定向测试实际只收到 StoneTrap 的 `ObjectDied` 与玩家自身的 `Death`。
- Root cause: Legacy `PlayerObject.Die` 的玩家 `Broadcast(ObjectDied)` 排除死亡玩家；宠物死亡广播仍可发送给主人，因此两个对象的接收者集合不同。
- Prevention: 验证主人死亡时按“宠物通知”和“死亡者自身通知”分别建立 recipient/packet 矩阵，不把同一地图广播误推断为发送给死亡者本人。
- Verification: 将断言收紧为主人收到 StoneTrap `ObjectDied -> player Death`；AI=255 主人死亡与登出移除定向测试通过。

### 2026-08-18 — AI=255 首包投影与地图夹具要使用实际存储布局

- Symptom: StoneTrap `Extra=false` 首包测试最初从已生成的最终对象重建包，无法观察 Spawned 前状态；CanFly 墙体测试还把墙写入 `y*height+x`，导致本应阻挡的路径仍被接受。
- Root cause: Legacy 首包与 Spawned 后快照的状态不同，必须检查实际通知；Go `mapdata.Map` 按 `x*height+y` 存储 cell，而不是二维数组常见的 `y*width+x`。
- Prevention: 包序列测试优先解析返回通知中的原始 payload；修改地图夹具前核对 `Map.index`，用坐标到存储索引的同一 helper/公式写入墙体。
- Verification: 首次 `ObjectMonster` 通知断言 `Extra=false`、后续快照断言 `Extra=true`，并用 `2*Height+1` 写入 (2,1) 墙；AI=255 定向测试通过。

### 2026-08-18 — TurnUndead 定向测试要隔离随机门与经验升级阈值

- Symptom: 首轮成功测试使用经验值 100，正好命中默认等级 50 的升级阈值，产生额外的 `HealthChanged/LevelChanged` 包；首个随机失败夹具只提供一个随机值，却实际进入了第二道随机门并越界崩溃。
- Root cause: Go 默认经验表缺失项按 100 处理，测试角色等级 50 的下一等级阈值也是 100；Legacy `TurnUndead` 先执行 `Next(2)`，只有第一门通过才执行 `Next(100)`，测试没有按分支提供完整随机序列。
- Prevention: 法术测试使用不会触发升级的最小经验值，或显式配置经验阈值；对有短路随机分支的路径按每个分支建立精确随机序列，并断言失败分支不产生延迟 action。
- Verification: 将目标经验改为 1、第一门失败目标等级改为 50，并覆盖 `[1]` 与 `[0,99]` 序列；TurnUndead 定向测试全通过。
- Additional symptom: 为地图夹具直接给 `world.mapRules[0].Fight` 赋值会在 Go 编译器中失败，因为 map 索引返回的是不可寻址结构副本。
- Additional root cause: Go map 中的结构字段不能通过链式索引修改，必须先复制值、修改字段，再写回 map。
- Strengthened prevention: 对 map-of-struct 夹具统一使用 `rules := map[key]`、修改、`map[key] = rules` 的写回模式。
- Additional verification: 改为写回 `worldMapRules` 副本后，TurnUndead 定向测试与 legacyworld/worlddata 测试通过。

### 2026-08-18 — Revelation 英雄准入测试要隔离世界 tick 的其他周期通知

- Symptom: Revelation 英雄目标按预期被 admission 接受，但用完整 `world.tick` 验证“CompleteMagic 对 Hero 无效果”时，测试仍收到英雄自身周期产生的对象通知，误判为 Revelation 成功。
- Root cause: 测试夹具把英雄 runtime 放入世界后，`tick` 同时推进了 `tickHeroesLocked`；该输出与 Revelation resolver 的目标类型分支无关。
- Prevention: 验证某个 delayed action 的“无效果”分支时，直接在世界锁内调用对应 resolver 并只断言其返回通知；只有需要验证完整调度顺序或跨系统副作用时才使用 `world.tick`，并过滤/断言已知的其他周期系统输出。
- Verification: 改为直接解析排队的 Revelation action 后，英雄准入仍成立、resolver 返回空通知；其余 Revelation 延迟广播、随机门和玩家目标定向测试通过。

### 2026-08-18 — EnergyRepulsor 夹具要遵守 Go 值语义、地图范围和通知矩阵

- Symptom: 新增 EnergyRepulsor 定向测试先因直接给 `world.mapRules[0].SafeZones` 赋值而编译失败；修正后又出现 nil `Stats` 写入 panic、把目标移到施法者外围之外导致“不推送”，以及 caster 通知断言漏掉 `ObjectPushed`/伤害包；本次 ExplosiveTrap 的 Player MAC 测试再次因直接写入未初始化的 `caster.Stats` panic。
- Root cause: Go map-of-struct 元素不可链式修改，手写 `worldPlayer` 不会自动初始化 map 字段；3x3 perimeter 只扫描 Chebyshev 距离 1，且立即效果通过现有广播通道在 `MagicLeveled` 前产生多类通知。
- Prevention: map-of-struct 夹具统一采用“取值—修改—写回”，显式初始化需要写入的 map；设置目标后重新核对扫描范围和字段 map；通知断言按接收者与完整顺序建立，而不是只断言最终升级包；凡是会写 `worldPlayer.Stats` 的新夹具，构造时直接使用 `make(protocol.ItemStats)`，并在目标测试编译前检查所有 map 写入点。
- Verification: EnergyRepulsor 三个定向测试及其 race 版本通过；本次为 ExplosiveTrap caster 显式初始化 `Stats` 后，Player MAC 定向测试通过；随后普通/race 目标门禁、`go test ./cmd/crystal-server` 普通/race、跳过两个既有 session 例外的 `go test ./...` 普通/race、`go vet ./...` 和 `go build ./...` 均通过。

### 2026-08-18 — Repulsion 家族测试标签要使用显式协议值映射

- Symptom: 共享 Repulsion/FireBurst 测试首次包编译失败，协议包没有假定的 `SpellName` helper；resolver 重构后旧文件还保留了未使用的 `protocol` import。
- Root cause: 测试为了生成 subtest 名称凭记忆假设了协议 API，重构只修改了调用点而没有同步清理文件级 import。
- Prevention: 新测试只依赖已核对的协议 API；没有名称 helper 时使用显式 `{name, spell}` 表，重命名 resolver 后立即运行 `gofmt` 与目标包编译，清理未使用 import。
- Verification: 修正标签表和 import 后，Repulsion/FireBurst 定向普通与 race 测试及完整门禁通过。

### 2026-08-18 — FireBurst 练习测试必须满足 spell-specific level gate

- Symptom: FireBurst 已正确推送 Player/Monster/Hero 且扣除 10 mana，但技能经验仍为 0。
- Root cause: Legacy `FireBurst` 的 `Level1` 是 33，测试共享 caster 只有 30 级；`levelMagicLocked` 正确拒绝了练习，行为本身没有失败。
- Prevention: 共享法术 resolver 测试除行为相同外，必须逐 spell 核对 catalogue 的 cost、Level1/2/3 和练习门；FireBurst 场景显式使用至少 33 级 caster。
- Verification: FireBurst 夹具提升至 33 级后，经验、mana、推送结果与 Repulsion 对照测试通过，普通/race 全仓库门禁、vet 和 build 均通过。

### 2026-08-18 — SoulFireBall 夹具要在 world.enter 后恢复派生属性并使用运行时 ID

- Symptom: SoulFireBall 目标测试第一次得到错误的伤害/护甲值，玩家和 Hero 断言还因按夹具原始 ID 查找而出现 nil/崩溃。
- Root cause: `world.enter` 会重建玩家派生战斗属性并分配运行时 ObjectID，覆盖了手写的 SC/MAC 和原始 ID；测试断言没有使用 enter 后的对象身份。
- Prevention: 任何经过 `world.enter` 的夹具都在 admission 前重新设置并检查派生 combat stats；对象断言按运行时 ID 或稳定业务字段查找，且先显式检查 nil 再读取字段。
- Verification: 恢复 enter 后的 SC/MAC、按运行时对象查找玩家/Hero 后，怪物/玩家/Hero 三目标、护符消耗和延迟命中测试通过。

### 2026-08-18 — FireWall 测试断言要读取 map 中的最终值并核对 MAC 算术

- Symptom: FireWall 首次行为测试把 `world.monsters[id]` 插入后保留的本地 struct 副本当成最终状态，且把伤害 14 减 MAC 2 误算为 10；测试报告的 HP 与实现状态不一致。
- Root cause: Go map 保存的是 value，伤害 resolver 更新的是 map entry；测试同时没有逐项核对 `damage - magic armour` 的结果。
- Prevention: 插入 map value 后，所有 mutation 断言都重新读取 `world.monsters[id]`/`world.players[id]`；每个 MAC 场景先写出明确算式，再断言目标 HP 和 map 中的值。
- Verification: 断言改为读取 map entry，MAC=3 得 89、MAC=2 得 88；FireWall 专项测试随后通过。

### 2026-08-18 — Go 专项测试夹具要避开移动目标格

- Symptom: Concentration walk 测试首次把 observer 放在玩家的下一步格，移动结果为 `Moved=false`，因此没有产生中断状态；实现没有错误。
- Root cause: 夹具同时要求 observer 可见和目标格可通行，却没有先检查 `occupiedLocked` 的阻挡语义。
- Prevention: 设计移动/击退 transcript 时先固定 actor 的候选路径，再把 observer 放在同一可见范围内的非目标格；失败时区分夹具阻挡和实现拒绝。
- Verification: observer 移到 `(5,2)` 后，Concentration walk、三秒恢复和 expiry 专项测试通过。

### 2026-08-18 — net.Pipe 专项夹具要先满足账号与角色长度约束

- Symptom: Concentration session 首次创建角色返回结果 1，缩短角色名后登录又收到失败包；协议断言尚未开始。
- Root cause: 新夹具手写了超出旧版约束的角色名和账号标识，没有复用现有 session 测试的短标识模式。
- Prevention: 新增真实会话测试时先用服务层创建/登录的最小合法账号与角色名跑 bootstrap，再加入法术 transcript 断言；任何非预期 bootstrap 包都先修夹具，不把它归因于法术实现。
- Verification: 改为 `concowner`/`concwatch` 与 `ConcOwner`/`ConcWatch` 后，Concentration `net.Pipe` cast、SetConcentration owner/observer 顺序和 JSON buff persistence 测试通过。

### 2026-08-18 — 新增 Go 测试 helper 要先搜索整个 package

- Symptom: 元素系统测试文件首次编译因 `containsInt16` 与现有 `support_buffs_test.go` helper 重名而失败。
- Root cause: 只在新测试文件内命名，没有先搜索 `cmd/crystal-server` 包级测试符号。
- Prevention: 新增测试 helper 前用 `rg -n '^func [A-Za-z0-9_]+' cmd/crystal-server --glob '*_test.go'` 检查包级名称；优先复用现有 helper，专用 helper 使用领域前缀。
- Verification: 删除重复 helper、复用现有 `containsInt16` 后，元素 Shot、Barrier、Gather 定向测试全部通过。

### 2026-08-18 — Go 测试断言要显式统一协议时长与伤害类型

- Symptom: Barrier duration regression test failed to compile when subtracting `int32` damage ticks from an `int64` buff duration.
- Root cause: The assertion copied runtime fields with different wire/domain widths into one arithmetic expression without an explicit conversion.
- Prevention: Before writing duration assertions, inspect the field declarations and convert damage-derived milliseconds to `int64`; keep the expected expression in the same unit and type as the stored field.
- Verification: Added `int64(...)` around the damage tick subtraction; Elemental Shot/Barrier/Gather/death tests all passed.

### 2026-08-18 — Go 测试夹具的多字段赋值要逐字段核对

- Symptom: BindingShot 释放测试首次编译报多变量赋值数量不匹配，`near.ObjectID, near.X, near.Y` 左侧三个字段却只提供了两个右值。
- Root cause: 复制怪物夹具后修改坐标时，遗漏了第三个坐标值，编译器才暴露了夹具结构错误。
- Prevention: 修改结构体夹具的多个字段时逐一对应字段和值；新增测试后先跑目标包编译/定向测试，再进入完整回归。
- Verification: 为 `ObjectID/X/Y` 提供完整三元组后，继续运行 BindingShot 定向测试。

### 2026-08-18 — FlashDash 测试的 MP 断言要按等级成本计算

- Symptom: FlashDash 定向测试首次把等级 2 的施法后 MP 写成 76，实际运行值为 84，导致新测试失败。
- Root cause: 只记住了基础消耗 12，遗漏了 `LevelCost=2` 按法术等级累加，实际消耗是 `12 + 2*2 = 16`。
- Prevention: 新法术测试断言资源值前，先从 Go 目录的 catalogue 和 `playerMagicCost` 同时核对基础消耗、等级消耗及等级；不要凭基础值推导等级施法结果。
- Verification: 修正为 MP 84 后，FlashDash 的移动、零位移、多目标和延迟命中定向测试全部通过。

### 2026-08-18 — 整包会话测试失败要先隔离复现

- Symptom: `go test ./internal/protocol ./cmd/crystal-server` 又一次在整包会话运行中让 Tucson 普通攻击用例收到空通知；单独以 `-run '^TestSessionTucsonMageNormalAttackTranscript$' -count=1 -v` 重跑仍通过。
- Root cause: 失败持续只出现在整包会话生命周期/时序环境，隔离重跑不可复现；具体触发点尚未确认，不能据此归因到 FlashDash 或本批次线技能代码。
- Prevention: 完整回归出现会话类失败时，保存失败用例和完整命令，立即精确隔离复现；隔离仍失败才阻断当前批次，并把整包运行视为独立的并发稳定性信号。
- Verification: 本次 Tucson 隔离用例通过；Lightning/HeavenlySword 定向世界与 `net.Pipe` 测试通过；该重复现象继续保留为整包稳定性关注项。

### 2026-08-18 — 会话转录夹具必须显式设置持久化生命值

- Symptom: Lightning `net.Pipe` 会话测试首次把施法者法力包的 HP 预期为 100，实际启动角色的默认 HP 为 18，测试在协议命中断言前失败。
- Root cause: 会话 bootstrap 从 auth 角色持久化属性初始化世界玩家；测试只覆盖了 MC/MP，没有覆盖 HP，因此沿用了角色创建默认值。
- Prevention: 会话转录需要固定战斗属性时，在 bootstrap 后逐字段设置 HP、MaxHP、Character.HP 及 MP/防御；不要把世界夹具的默认生命值假设带入真实会话断言。
- Verification: 将 Lightning 会话夹具改为显式 100 HP 后重新运行同一 `net.Pipe` 测试，继续验证法力、延迟命中和双方包序。

### 2026-08-18 — 会话位置夹具要核对 map/x/y/direction 参数顺序

- Symptom: Lightning 会话测试首次收到位置 `(0,2)`，而用例预期施法者位于 `(0,0)`；测试在命中转录前失败。
- Root cause: `UpdateCharacterMapRuntime` 的参数顺序是 `mapIndex, x, y, direction`，夹具把方向值写入了 y 参数。
- Prevention: 设置会话角色位置前先读取 helper 签名，并在 bootstrap 后断言实际 `UserLocation`；坐标和朝向不要依赖位置参数的直觉顺序。
- Verification: 调整为 `map=0, x=0, y=0, direction=2` 后重新运行 Lightning 会话测试。

### 2026-08-18 — 会话练习包要满足 Legacy 的技能等级门槛

- Symptom: Lightning 会话命中包顺序中没有预期的 `ServerMagicLeveled`，施法与伤害包均已正常送达。
- Root cause: `levelMagicLocked` 保留 Legacy 的等级门槛；Lightning 等级 0 需要角色等级 26，而 auth 创建角色默认等级为 1，所以命中后不练习也不发练习包。
- Prevention: 会话测试若断言技能练习，bootstrap 后显式设置满足该法术 `Level1` 的角色等级，并将“命中”和“练习包”分开验证。
- Verification: 将 Lightning 会话施法者等级设为 40 后重跑，继续验证目标受击与 `ServerMagicLeveled` 顺序。

### 2026-08-18 — HellFire 测试要分别核对等级伤害与逐格时序

- Symptom: HellFire 等级 3 测试首次把 MC=10 的伤害预期为 25，实际为 30；把目标移到第二格后又在首格 +500ms 立即断言命中，导致定向测试失败。
- Root cause: 只沿用了等级 0 的伤害心算，忽略 `playerMagicDamageForSpell` 的等级倍率；没有区分首个动作 +500ms 和后续每格 +100ms。
- Prevention: 法术测试断言伤害前按 catalogue 与 `playerMagicDamageForSpell` 逐级计算；链式动作测试把首格、第二格和最终动作分别绑定到 `500ms + n*100ms`。
- Verification: HellFire 等级 0/3、四格链、逐格重验证测试均通过，等级 3 伤害固定为 30。

### 2026-08-18 — net.Pipe 会话施法要先消费观察者的 ObjectMagic

- Symptom: HellFire 会话影响通知的 `net.Pipe` 写入在测试中超时；目标读取器把施法时的 `ServerObjectMagic` 当成伤害包，导致后续 `ServerHealthChanged` 没有读端。
- Root cause: 会话夹具只启动了目标的影响包读取，没有为施法广播启动并消费独立的 ObjectMagic 读取器。
- Prevention: 真实会话测试按阶段为每个观察者建立读取器：施法阶段先读 ObjectMagic，再在手动 tick 前建立影响包读取器；不要把 cast 和 impact 包合并到一个固定计数。
- Verification: HellFire 会话测试先断言目标 ObjectMagic，再断言双方影响包顺序、HP=82 与 +100ms 链动作，定向运行通过。

### 2026-08-18 — Go 测试不得直接修改 map 元素的嵌套字段

- Symptom: Teleport NoTeleport 定向测试首次编译报 `cannot assign to struct field world.mapRules[0].NoTeleport in map`。
- Root cause: Go map 索引返回结构体副本，测试夹具直接对副本的嵌套字段赋值。
- Prevention: 修改 map 中的结构体配置时先读到局部变量，修改字段后再写回 map；新增夹具先跑目标包最小编译。
- Verification: 按该模式改写 NoTeleport 夹具后，Teleport/Blink/StormEscape 四个定向测试全部通过。

### 2026-08-18 — net.Pipe 会话测试要先按实际通知包数建立读端

- Symptom: Blink 会话测试只为延迟影响阶段读取 3 个包，但 TemporalFlux 的 stat refresh 还会产生额外通知；`deliverWorldNotifications` 在第四个写入处阻塞，测试无输出直至超时。
- Root cause: 测试夹具假设地图迁移、ObjectEffect、AddBuff 是全部自有包，未计入同一延迟动作返回的健康/状态边界。
- Prevention: 先执行手动 world tick，按返回通知中 `NoSend` 过滤后的实际包数启动 `net.Pipe` 读取器；随后再断言关键包序，避免固定计数让写端反压。
- Verification: Blink 会话测试改为动态包数后通过，并保留 `MapChanged -> ObjectEffect -> AddBuff` 的关键顺序断言。

### 2026-08-18 — 新增会话测试前先查同包协议解析辅助与创建参数

- Symptom: SummonSkeleton 会话测试首次编译找不到 `protocol.ParseObjectMonsterPayload`，修正后又因把 `CreateCharacter` 的 class 参数设为 2 而得到结果码 2。
- Root cause: 假设协议包提供了尚未导出的 ObjectMonster 解析 API，并凭 Legacy 职业语义猜测测试 helper 接受任意 class 值，没有先查现有同包辅助和创建夹具约定。
- Prevention: 新会话断言先用 `rg` 查已有 parser/helper 并复用同包解析器；创建角色先复制已通过的 session fixture 参数，再逐步改变测试所需字段。
- Verification: 复用 `ordinaryPetTestParseObjectMonster`，将会话角色创建参数改为现有有效值后，SummonSkeleton 定向世界/会话测试通过。

### 2026-08-18 — 会话坐标断言要用实际方向映射验证

- Symptom: SummonSkeleton 延迟生成对象实际位于 `(6,5)`，测试按直觉把方向 2 预期成 `(5,6)`。
- Root cause: 没有先调用项目的 `movePoint` 方向映射，直接凭方向编号推断前方坐标。
- Prevention: 位置相关会话夹具统一使用 `movePoint` 计算预期，或先读取相同方向的已验证对象位置；不要把数字方向当作笛卡尔轴的固定约定。
- Verification: 修正对象位置断言为方向 2 的 `(6,5)` 后，SummonSkeleton `net.Pipe` 延迟对象/健康包测试通过。

### 2026-08-18 — 新召唤会话夹具要复用合法的短角色标识
- Symptom: SummonShinsu/SummonHolyDeva 的世界测试通过，但新增 `net.Pipe` 测试在 bootstrap 前得到角色创建结果码 1；缩短标识后又曾把对象名断言按旧的长构造名写错。
- Root cause: 会话角色名由法术显示名拼接而成，超过旧版长度约束；修正夹具后断言仍没有读取实际传入的短角色名。
- Prevention: 新会话测试先使用已验证长度的固定账号/角色名完成创建和登录，再以同一 `characterName` 构造对象名断言；bootstrap 失败时先修夹具，不归因于法术逻辑。
- Verification: 改用 `ShinsuSess`/`HolySess` 与短账号后，两种召唤的认证 `net.Pipe` 转录、护符 DeleteItem 数量、延迟对象/健康包顺序均通过。

### 2026-08-18 — 多次施法测试期望必须从当前运行状态推导

- Symptom: UltimateEnhancer ResetStatAndDuration 测试第二次施法仍固定断言 MP=972，实际第二次应在第一次结果上再扣 28，导致定向测试失败。
- Root cause: 辅助函数只按首次施法设计，没有把当前 caster.MP 作为第二次施法的输入状态。
- Prevention: 可重复施法的测试辅助在发包前从当前状态计算期望 MP、护符数量、冷却和技能经验；不要把首次调用的常量复用于后续调用。
- Verification: 辅助断言改为 `caster.MP - 28` 后，UltimateEnhancer 世界测试覆盖五职业、重置和过期均通过。

### 2026-08-18 — Go 世界夹具的派生属性要在 enter 后覆盖

- Symptom: Curse 定向测试预期 SC=10 产生 15 秒持续时间，首次实际得到 5 秒；同一原因也使重置测试的首个 Buff 只有 5 秒。
- Root cause: `world.enter` 会按角色基础/装备/Buff 重新计算派生属性，覆盖了入场前手工设置的 `MinSC`/`MaxSC`。
- Prevention: 需要固定运行时战斗属性的世界测试先 `enter`，再设置 `MinSC`、`MaxSC`、HP/MP 等字段；若断言公式，先从当前角色快照计算基线，不依赖构造体初始值。
- Verification: Curse 世界测试改为入场后设置 SC，15 秒持续时间、25 秒重置持续时间及玩家百分比属性均通过。

### 2026-08-18 — 延迟法术会话要按主循环的实际包序读取

- Symptom: Curse `net.Pipe` 测试首次在扣物品包处读到 `ServerUserLocation`，并因后续读端错位超时；修正读序后又没有收到 `ServerMagicLeveled`。
- Root cause: `BeforeMagicNotifications` 由主循环在 `UserLocation` 之后、`ServerMagic` 之前发送；另外 Curse 的等级 0 练习门槛为角色等级 40，等级 35 的会话夹具合法命中但不会练习。
- Prevention: 新增会话测试先读取 `main.go` 的写包顺序并按协议 ID 建立阶段读端；若断言练习包，角色等级必须满足 catalogue 的 `Level1`，并把效果命中与技能练习分开验证。
- Verification: Curse 认证会话现验证 `HealthChanged -> UserLocation -> DeleteItem -> Magic`、延迟目标 `Chat -> AddBuff`、施法者 `MagicLeveled` 及护符/Buff 持久化；等级 50 夹具连续通过。

### 2026-08-18 — 会话持久化断言必须使用登录后的派生生命/法力基线

- Symptom: Plague 会话的包序和实际命中均通过，但测试把目标血蓝固定写成 80/88，存档实际为等级属性派生值扣除伤害后的 519/121。
- Root cause: 会话登录会从角色等级和装备计算 MaxHP/MaxMP，不能复用裸世界夹具的 100/100 初始值。
- Prevention: 发起会话动作前在 world 锁内记录目标当前 HP/MP，持久化断言只比较预期的相对扣减；裸世界测试与认证夹具分别维护各自基线。
- Verification: Plague 会话测试改为 `targetStartHP-20` 与 `targetStartMP-12`，不再依赖派生属性的绝对常量。

### 2026-08-18 — Go 时间夹具必须完整填写 time.Date 参数

- Symptom: PoisonCloud 世界测试首次编译失败，`time.Date` 少传了纳秒参数。
- Root cause: 新增测试夹具从已有日期表达式手工复制时漏掉了 Go `time.Date` 的完整八参数签名。
- Prevention: 新增固定时间夹具统一使用 `year, month, day, hour, minute, second, nanosecond, location` 八个参数；提交前先运行新增测试包的编译/定向测试。
- Verification: 补齐纳秒参数后 `TestGameWorldPoisonCloud` 世界测试通过。

  Recurrence evidence: 下一批筛查时又把 Legacy 相对路径放进了 Go workdir，命令未读到任何对侧文件；该调用输出已作废。
  Strengthened prevention: 仓库切换时同时更换 `workdir` 与相对路径前缀，命令正文只允许当前仓库已确认存在的路径；跨仓库研究必须拆成两个独立调用，不能以“当前目录下不存在”代替切换仓库。


### 2026-08-23 — ThunderElement 会话转录读取世界状态必须持有 world 锁

- Symptom: 新增 AI=49 ThunderElement authenticated `net.Pipe` transcript 的普通定向测试通过，但 `-race` 报告测试 goroutine 读取 `monsterAttackActions`/玩家 HP 与服务端 tick goroutine 写入并发。
- Root cause: 会话 transcript 在手动 tick 后直接读取 world map、攻击动作队列和最终 HP，忽略了连接维护/世界 tick 仍可并发运行；`net.Pipe` 包序正确不等于无锁世界快照可直接读取。
- Prevention: 测试只通过 world 锁读取或复制 map/队列快照；手动 tick 返回值用于协议断言，最终领域状态另在锁内回读，不能直接访问共享实体或 slice。
- Verification: 增加锁内快照后，ThunderElement 会话测试普通 `-count=10`、定向 `-race -count=3` 及 AI=49/50 组合门禁通过；完整 race 失败栈未进入该测试。
