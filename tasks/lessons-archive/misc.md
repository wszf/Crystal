### 2026-08-20 — 对照读取末尾路径失败时整条调用作废（再次强化）

- Symptom: 一次 Legacy Dragon 对照命令末尾误带 Go 的 `internal/worlddata/world.go`，读取以路径错误结束；该调用没有写入，前面的 Legacy 输出也不能作为源码证据。
- Root cause: 查看 Dragon 对照代码时复用了另一仓库的路径参数，没有把一次 shell 调用绑定到唯一仓库。
- Prevention: 每次调用只使用当前 `git rev-parse --show-toplevel` 下已核验的路径；切换仓库必须结束调用并重新核验根目录，失败调用的全部输出丢弃。
- Verification: 失败调用未改变文件；后续 Dragon 对照将拆成纯 Legacy 与纯 Go 两次调用。

### 2026-08-19 — Harvest 最后一层皮剥必须在同一次调用生成掉落

- Symptom: 初版 Go Harvest 把 `RemainingSkinCount` 从 1 减到 0 后也提前返回，导致 AI=9 尸体比 Legacy 多需要一次客户端 Harvest 才能生成/领取掉落。
- Root cause: Legacy 只有减完仍大于 0 时返回；减到 0 的最后一次调用会继续进入 `HarvestDrops` 生成分支。
- Prevention: 处理皮剥次数时仅在 decrement 后仍大于 0 才返回，并把边界测试固定为“第一次只减皮、第二次生成、第三次领取”。
- Verification: Go world 测试与 `net.Pipe` 会话转录均按三次 Harvest 通过，第二次检查缓存掉落，第三次收到 `GainedItem/ObjectHarvested`。

### 2026-08-17 — 工具编排失败不得作为源码证据

- Symptom: 一次只读对照编排把 `exec_command` 拼写成不存在的工具函数，调用在执行前失败且没有任何源码输出。
- Root cause: 手写工具名时没有沿用已确认的工具接口，也没有先做最小调用验证。
- Prevention: 工具编排只使用已声明的 `tools.exec_command`/`tools.apply_patch` 等名称；脚本失败时整条调用作废，不从错误文本推断代码内容。
- Verification: 本次失败发生在脚本执行阶段、无文件变化；后续继续使用已核验工具接口和独立仓库调用。

### 2026-08-15 — TucsonGeneral 岩石值必须保留 Random.Next 的排他上界

- Symptom: AI=131 岩石生命周期夹具把 15 个岩石的首跳/次跳生命分别期望为 850/700，定向测试实际得到 835。
- Root cause: Legacy 使用 `Random.Next(minDC, maxDC)`，上界排他；固定值 11 时每个岩石造成 11 点伤害，不能按包含 12 的区间计算。
- Prevention: 迁移随机区间时先核对调用 API 的上下界语义；岩石测试按 `15*11` 明确计算累计伤害，并单独覆盖边界值。
- Verification: 修正期望为 835/670 后重新运行 AI=131 定向测试，并在完整普通/race、vet、build 门禁中确认。

### 2026-08-13 — ControlPoints 结算必须组合 EndWar 与 TakeConquest 的提前返回

- Symptom: Go 的 ControlPoints `End` 无条件清空积分并把每个控制点归一到最终 owner；静态复核发现，Legacy 平局保留当前 owner 时会在 `TakeConquest` 的“winning guild 已拥有 Conquest”门禁提前返回，控制点积分和各点 owner 实际都保持不变。
- Root cause: 只按 `EndWar` 的“计算最终赢家后调用 TakeConquest”概括结算意图，没有把被调函数入口门禁、是否真正换 owner、积分重置和旗帜刷新组成完整可达路径。
- Prevention: 迁移跨函数结算时分别建立“owner 真正变化”和“owner 保持”状态表；只有实际通过 `TakeConquest` 并换 owner 才重置积分、重投全部控制点（包括 owner 已相同的点）和主旗帜，平局/不可接管路径保留运行状态。
- Verification: 领域测试现同时锁定平局保留积分/分点 owner，以及新赢家清空积分并为全部控制点生成刷新事件；控制点结束包序测试覆盖颜色 → 全部控制点 → 主 owner → `BroadcastInfo`，全量普通/race 测试通过。

### 2026-08-13 — Conquest Siege 的运行实体语义必须按实际 Gate 类型迁移

- Symptom: Siege 在修复时使用 Gate 方向公式，但初始加载和普通受击只按 Wall/默认方向处理，导致同一资产在修复前后显示不一致。
- Root cause: 依据数据库列表名 `Siege` 推断运行类型，忽略 Legacy `ConquestGuildSiegeInfo.Spawn` 实际严格创建 AI 72 `Gate` 并调用 `CheckDirection`。
- Prevention: 迁移多态资产时沿 Spawn 的实际构造类型决定运行行为；列表分类只决定持久字段和 NPC 动作，方向、受击和对象投影应复用真实实例类型的公式。

### 2026-08-13 — Go 条件中的短声明只能位于 if 初始化子句

- Symptom: 新增 Conquest 测试把 `got := ...` 写在 `if existingCondition || got := ...` 中，包级编译报 non-name on left side of `:=`。
- Root cause: 把 Go 的 `if init; condition` 语法误写成了布尔表达式内部声明。
- Prevention: 条件需要复用计算结果时，在 `if` 前单独声明，或严格写成 `if got := expression; conditionUsingGot { ... }`；短声明不得出现在 `&&`/`||` 表达式中。
- Verification: 结果变量改为条件前声明后，Conquest NPC 定向包测试编译并全部通过。

### 2026-08-13 — 定时业务必须沿调用链传递同一个确定时间

- Symptom: 自动黑石的名称时效最初使用 `time.Now()`，与 tick 测试传入的 `now` 不一致；给 fixture 名称加入 `[2h]` 后又不再匹配默认黑石配置名。
- Root cause: 创建 helper 在调用链中重新读取墙上时间，测试 fixture 也隐式依赖默认名称匹配，随机/时间/配置三类输入没有全部显式化。
- Prevention: 所有 tick 驱动的过期、随机和生产逻辑把同一个 `now`、roll 与配置名称封装进不可变上下文向下传递；带时效标记的物品 fixture 显式设置对应配置名，不依赖默认模糊匹配。
- Verification: 自动黑石使用 tick 的 `now` 计算 `[2h]` 到期，测试显式调用 `setIntelligentCreatureSettings`，确定性 `CreateDropItem` 与时效测试通过。
- Strengthening after recurrence: FlameSpear FearTime 定向 fixture 把外层断言时间与 helper 内部初始化时间错开，导致预期的 fear-move 分支实际进入远程攻击；同一测试的状态时间、触发时间和断言时间必须由一个明确的 `base` 派生，不能靠相近的 Unix 秒值。
- Verification after recurrence: 失败 transcript 只产生错误断言、没有源码写入；将 fear-move 使用 helper 同一 `base` 后，包级编译和 FlameSpear 定向测试通过。

### 2026-08-13 — 特殊物品迁移必须审计通用入口尾部副作用

- Symptom: 只实现 Strongbox/BlackStone/WonderDrug/Knapsack 等特殊分支时，容易遗漏通用 UseItem 尾部的二次响应、源物品消耗或无效果也消费等 Legacy 可观察行为。
- Root cause: 测试只调用领域 helper 或只关注奖励结果，没有覆盖生产 dispatch 返回后的通用处理链。
- Prevention: 每种特殊物品同时测试领域结果和真实生产入口；逐项列出分支内部与通用尾部各自的消费、持久化和响应包，尤其锁定 Strongbox 双 `UseItem`、FortuneCookie 无效果仍消费以及材料不足/不适用路径。
- Verification: 当前生产会话覆盖 BlackStone、Strongbox、WonderDrug、FortuneCookie、Knapsack 全部 shape，并断言完整包序列、源物品终态、auth 状态与磁盘重载。

### 2026-08-13 — 跨领域锁内禁止调用公会 authority

- Symptom: 公会战争/领地骨架最初在持有 `world.mu` 时调用 `GuildAuthority`，而 authority 的事务顺序是 authority → auth；这会与公会投影、会话初始化或生命周期线程形成反向锁序风险。
- Root cause: 把“刷新在线投影”写成了锁内查询权威状态，没有先切分 detached authority 快照与 world 写入阶段。
- Prevention: 公会 authority 调用必须在 `world.mu` 外完成；先取得敌对、战争或领地结果并释放 authority/auth 锁，再进入 world 锁合并投影和构造通知，网络投递与 SaveJSON 都放在所有锁之外。懒初始化同样先复制配置/定义，再在 world 锁外构造 authority。
- Verification: P9 接线改为 authority 查询/事务 → world 投影 → SaveJSON → 通知四阶段，服务端整包测试通过；race 门禁在提交前继续验证。

### 2026-08-13 — 地图未标记格与拆解满包必须保留 Legacy 怪癖

- Symptom: P11 兼容审查发现地图格 Go 零值会把未标记水域误当成 `FishingAttribute=0`；拆解奖励在满包时若按普通事务回滚，又会改变原版仍发送 `GainedItem`、扣金并删除原物品的行为。
- Root cause: Go 零值与 Legacy 构造默认值不同，并把异常但可观察的原版副作用误当成应修复的失败事务。
- Prevention: 所有未显式标记的地图格初始化 `FishingAttribute=-1`；拆解先保留发送奖励包的快照，若 `AddItem` 无法落位仍继续原版扣金/删除流程。迁移目标是有效路径和可观察怪癖等价，不自行修正 Legacy 行为。
- Verification: map fixture 覆盖默认 `-1` 和显式水域值；觉醒测试锁定满包时 `GainedItem -> LoseGold -> DeleteItem`、奖励不入包且原物品消失。

### 2026-08-13 — XOR 权限接口不能用 false 清除未设置位

- Symptom: 公会仓库无取回权限测试调用 `ChangeGuildRankOption(..., "false")` 后，Members rank 反而获得了 `CanRetrieveItem`。
- Root cause: Legacy 的 `"false"` 分支使用 XOR 切换权限位，而 Members rank 初始并没有该位；测试把切换操作误当成了幂等清除。
- Prevention: 测试权限关闭状态时优先直接断言默认 rank；若必须走变更接口，则先设为 true 再设为 false，并断言每一步位掩码。不能用 XOR 风格接口清除一个未经确认已设置的位。
- Verification: fixture 已改为直接验证 Members rank 初始不含 Retrieve 位，避免在目标行为前改变权限。

### 2026-08-12 — 单次异常排查不得变成固定汇报项

- Symptom: 用户只为确认一次异常而询问模型、Goal 或代码比对状态，后续迁移仍反复展示同类截图分析和内部状态。
- Root cause: 把一次性诊断误当成长期进度模板，没有在问题解释清楚后恢复到只汇报交付结果的沟通边界。
- Prevention: 一次性异常只回答当次；除非用户再次询问或出现新的真实异常，后续不主动汇报代码比对、Goal 数量、模型名称、内部调度或已解释过的原因。正常迁移仅在整批完成、真实阻塞或必须由用户决策时汇报。
- Verification: 本轮已停止重复发送该类状态，并继续直接完成关系功能批次。

### 2026-08-12 — 源码比对与子任务细节只作为内部过程

- Symptom: 源码逐段比对、子任务完成报告被频繁发送给用户，界面还因已完成代理未及时关闭而呈现持续活动，造成迁移反复汇报、模型来回切换的观感。
- Root cause: 把内部审计证据和每条并行线的原始输出当成了用户必须逐项接收的进度；同时没有在取得结果后立即关闭 completed 子代理，也没有区分线程跨轮次换模与同一时刻的模型路由。
- Prevention: 源码比对结果默认仅用于内部实现和验收；只在整批功能完成、需要用户决策、出现真实阻塞或长时间工作需简短报平安时汇报。取得子代理结果后立即关闭，并在解释模型活动前同时核对主线程当前模型和子线程生命周期。
- Verification: 本次审计确认主线程当前固定为 `gpt-5.6-sol`，Luna 仅存在于历史轮次/已关闭子线程；遗留的 Maxwell、Gibbs 已关闭，当前所有子线程均为 closed，后续不再逐项转发源码比对结果。
- Strengthening after correction: 用户对 Goal/模型状态的询问属于一次性故障排查，不得沉淀为每轮固定报告；除非用户再次询问或发现新的实际异常，后续不主动复述 Goal 数量、模型名称或子线程状态。
- Second strengthening after correction: 用户针对异常截图或某项内部状态的单次询问，只回答当次问题；不得把该分析扩展成持续汇报模板，也不得在后续迁移批次中重复发送相同的代码比对、调度状态或原因分析。验证方式是后续仅汇报整批迁移结果、真实阻塞和必须由用户决定的事项。
- Third strengthening after correction: 用户指出某类内部分析无需每次展示后，立即从所有后续进度模板中移除该项；即使内容更新或截图形式变化，也不能以“新结果”为由再次主动发送。除非用户明确重新询问，否则只在内部用于实现和验收。

### 2026-08-12 — GameShop 必须用不同的 GIndex 与 ItemInfo.Index 锁定怪癖

- Symptom: 既有测试一直令 `GIndex == ItemInfo.Index`，掩盖了原版库存读取用 Info.Index、写入用 GIndex、Stock 包再发 Info.Index，以及每封购买邮件消费两个 MailID 的行为。
- Root cause: fixture 使用相同索引让错误键路径退化成正常字典操作，同时只断言邮件存在，没有断言全局 ID 步进。
- Prevention: GameShop 回归 fixture 固定使用不同索引，分别断言查找键、持久化键、Stock 包键、重复购买覆盖语义和 MailID 2/4 步进；Quantity 1..99 必须先于商品查找校验。
- Verification: auth 与完整会话测试覆盖错位库存、非法数量静默、双 MailID、信用扣款和邮件 transcript。

### 2026-08-12 — 同一文件的格式化写入与读取验证必须串行

- Symptom: 本轮曾把 `gofmt -w` 与对相同文件的 `rg` 放进并行调用；另有跨仓库读取曾通过命令链组合，存在读取中间态和混淆失败来源的风险。
- Root cause: 只按降低延迟判断可并行性，没有把格式化视为写操作，也没有保持每个仓库、每个验证步骤的独立 workdir/退出状态。
- Prevention: apply_patch、gofmt 和任何生成操作均串行完成，随后再并行执行互不写入的读取；跨仓库命令各用独立绝对 workdir，禁止用 `&&`、`;` 或管道拼接多个验证步骤。
- Verification: 后续格式化、定向测试、全量 test/race/vet/build 和两个仓库的 diff/status 均用独立调用完成。

### 2026-08-12 — 原版物品价格必须按附加属性数值总量计算

- Symptom: 带多个或负值附加属性的 UsedGoods 买回价格可能与原版不同。
- Root cause: Go 价格辅助函数曾按附加属性键数量计算；原版 `Stats.Count` 是每个属性值绝对值之和。
- Prevention: 迁移包含 `Stats` 的物品公式时，先对照 `Stats.Count` 的实现及符号规则，再复用统一的数值总量辅助函数；不要把 map 长度当成属性强度。
- Verification: 增加附加属性买回价格回归测试，`{2, -3}`、数量 2、单价 1000 得到原版价格 3000。

### 2026-08-12 — NPC 商品响应面板与购买请求类型不是同一语义

- Symptom: 初版 `[BUYUSED]` transcript 使用 `PanelType.BuySub` 发送购买请求；静态对照客户端后确认真实客户端会一直发送 `PanelType.Buy`，原版服务端因此会拒绝该初版请求。
- Root cause: 把 `NPCGoods.Type`（控制客户端显示 BuySub 子面板）误当成 `ClientPackets.BuyItem.Type`（原版购买入口只接受 Buy）。
- Prevention: 每个双向协议功能分别核对服务端响应字段、客户端发送字段和服务端门控；不能从同名/相近 enum 值推断请求值。
- Verification: Go 服务端 `[BUYUSED]` 请求门控改为接受 `Buy`，返回仍保持 `BuySub`；net.Pipe transcript 和客户端源码对照均通过。

### 2026-08-11 — NPC 条件接线必须复用现有依赖并先核对语义字段

- Symptom: 新增 NPC 地图光照条件的服务端定向测试先因使用未导入的 `fmt` 编译失败；随后按相似字段接线时发现 `MAPLIGHT` 被错误映射到地图环境光字节。
- Root cause: 接线前没有先检查主文件已有的字符串转换依赖，也没有从原版 `Envir.AdjustLights`/`MAPLIGHT` 实现确认它比较的是全局 `LightSetting` 名称。
- Prevention: 新增运行时字段前先复用当前文件已有标准库依赖；每个条件从原版 parser、context 数据源和 evaluator 三处逐字段核对，禁止按同名/相似字段推断语义。
- Verification: 改用已有 `strconv`，并按 `Dawn/Day/Evening/Night` 的原版时间规则实现；纯条件、光照规则和 net.Pipe NPC 会话定向测试均通过。

### 2026-08-15 — 召唤 helper 返回值必须明确绑定语义

- Symptom: AI=97 定向测试首次通过召唤 helper 返回的 `worldMonster` 副本调用死亡逻辑，但副本的 `Dead`/`DespawnAt` 更新没有自动写回 `world.monsters`，后续 tick 仍把世界中的实体当作存活对象。
- Root cause: helper 同时返回“刚创建的快照”和持锁 world map 中的实体，调用者没有意识到返回值是 detached value，而 Go struct 不提供引用式回写。
- Prevention: 领域 helper 要么只返回不可变结果并在内部完成 map 提交，要么明确命名/文档化 detached contract；任何修改返回实体后都必须在同一锁边界显式写回权威 map，并增加后续 tick/重载断言。
- Verification: AI=97 测试已在死亡回调后显式写回 knight，再验证 despawn 不进入普通 respawn；定向、全包和 race 门禁通过。

### YYYY-MM-DD — Short title

- Symptom:
- Root cause:
- Prevention:
- Verification:
```

### 2026-08-11 — Go 多变量声明必须显式使用 var

- Symptom: 新增 Game 阶段位置状态后，`go test ./...` 在 `gameX, gameY int32` 处编译失败。
- Root cause: Go 的局部变量声明不能使用没有 `var` 或初始化表达式的裸类型声明。
- Prevention: 新增状态变量时统一使用 `var x, y int32`，并在提交前运行 `gofmt` 与 `go test ./...`。
- Verification: 修正后 `go test ./...` 和 `go vet ./...` 通过。

### 2026-08-11 — Go 类型化常量不能直接混用底层 byte 类型

- Symptom: 新增 `mapdata.CellAttribute` 常量时，`go test ./...` 报告 `byte` 常量不能用于 `CellAttribute` 常量声明。
- Root cause: 常量组中的底层类型被显式固定为 `byte`，随后又把它们直接赋给另一种命名类型。
- Prevention: 对领域类型常量直接使用 `CellAttribute = iota` 或显式类型转换，不保留无必要的中间类型常量。
- Verification: 修正后重新运行 `gofmt`、`go test ./...` 和 `go vet ./...`。

### 2026-08-11 — Go 复合字面量中不能插入局部语句

- Symptom: 角色运行时字段接入后，`gofmt`/编译在 `UserInfo{}` 内的 `hp, mp :=` 处报告缺少逗号和非法语法。
- Root cause: 局部变量声明和条件语句被放进了结构体复合字面量；复合字面量只能包含字段表达式。
- Prevention: 先在字面量外完成默认值和派生值计算，再把最终变量作为字段值传入。
- Verification: 修正后重新运行 `gofmt`、`go test ./...`、`go vet ./...` 和 `git diff --check`。

### 2026-08-11 — awk 枚举核对变量不要使用保留字

- Symptom: 用 awk 计算原版 packet enum ordinal 时，命令因 `in` 变量名触发语法错误而未执行。
- Root cause: `in` 是 awk 的语法关键字，不能作为普通变量名；这是核对脚本错误，不是项目源码错误。
- Prevention: 编写 awk 枚举脚本时使用 `inside` 等非保留变量名，并先用最小命令验证脚本能运行，再依赖其输出修改协议常量。
- Verification: 改用 `inside` 后成功核对 `MagicKey=57`、`Magic=58`、`NewMagic=117`、`Magic=120`、`MagicDelay=121`、`MagicCast=122`、`ObjectMagic=123`，随后以原版 enum 数值为依据实现测试。

### 2026-08-11 — 经验状态抽取后必须检查残留局部变量

- Symptom: 将经验升级逻辑抽取到共享 helper 时，初版 patch 在 world 状态写回处残留了已不存在的局部变量引用，静态复查在测试前发现。
- Root cause: 用宽泛上下文替换相似代码时命中了相邻的 CHANGELEVEL 状态写回，未立即检查 helper 调用后的变量来源。
- Prevention: 抽取状态计算函数后，立即逐行核对 result 的 Level、Experience、HP、MP 写回点，并在继续扩展功能前运行 gofmt、定向编译和单元测试。
- Verification: 修正为使用计算结果的 Level 后，经验表、世界状态、NPC parser、net.Pipe transcript 和 go test ./... 全部通过。

### 2026-08-11 — PvP 友方规则必须保留 legacy 的严格时间和空公会语义

- Symptom: 复核攻击模式迁移时发现 Red/Brown 在 `BrownTime` 恰好到期时被 Go 判为友方；EnemyGuild 对无公会施法者也被错误判为非友方。
- Root cause: 用 `!now.Before(...)` 近似了 legacy 的严格 `Envir.Time > BrownTime`，并把 `Guild.IsEnemy(null)` 的 false 语义简化成了双方必须有公会。
- Prevention: 对照原版条件逐运算符迁移（`Before`/`After` 保持严格边界），并为 null guild、同 guild、敌对 guild、非敌对 guild 分别建立 truth table 和测试。
- Verification: 增加 BrownTime 相等时刻、敌对公会、非敌对公会和无公会 ally 测试；Go 定向测试通过，提交前继续运行 race、vet、build 和 diff 检查。

### 2026-08-11 — 装备条件比较必须显式统一整数类型

- Symptom: 新增装备解析时，Go 编译器拒绝 `byte` 的 `RequiredAmount` 与 `uint16` 的角色等级直接比较。
- Root cause: 协议字段保留 legacy 的 `byte` 宽度，领域角色等级使用 `uint16`，边界比较遗漏了显式转换。
- Prevention: 从 wire 读取的窄整数进入等级/计数比较前统一转换到领域类型，并在新增条件分支后立即运行定向 `go test`。
- Verification: 将候选装备等级转换为 `uint16` 后，协议与服务端装备测试通过。

### 2026-08-11 — nil 物品格与空物品格必须区分

- Symptom: 会话装备请求返回失败；原因是持久化的 equipment slice 是非 nil 空数组，默认容量补齐函数只对 nil 生效。
- Root cause: legacy 14 格默认语义在 Go 中由 `nil` 触发，空 slice 表示一个真实的零长度格子，不能混同为缺省值。
- Prevention: 对需要默认容量的导入数据传递 nil 或显式补齐 14/46/40 格；测试同时覆盖 nil、空 slice 和正确容量三种输入。
- Verification: 会话 fixture 改为 nil equipment/quest grids 后，Equip/Remove/Logout transcript 与持久化断言通过。
- Strengthening after recurrence: auth 领域事务测试若要验证合法背包槽，也必须显式创建 Legacy 的 46 格 Inventory；新建角色的 nil/零长度 Inventory 不代表可用空背包。否则合法取出会被正确判成目标越界，容易被误判为事务实现问题。

### 2026-08-11 — FriendlyName 清洗顺序必须保持 legacy 顺序

- Symptom: 地面物品名称清洗初稿先移除方括号再移除尾部数字，`Drop Blade7[rare]` 会错误变成 `Drop Blade`。
- Root cause: 原版 `ItemInfo.FriendlyName` 明确先执行尾部数字正则，再移除方括号；相似的两个清洗步骤不可交换。
- Prevention: 迁移字符串派生字段时保留原方法的操作顺序，并用“数字在方括号前/后”两种 fixture 做差异断言。
- Verification: Go 实现改为 trailing-digits → bracketed-text，双会话 ObjectItem transcript 与全量测试通过。

### 2026-08-11 — NPC 物品堆叠终态必须逐步推导

- Symptom: 本轮 `GIVEITEM` 测试的背包终态曾两次误算，先后把 fresh stack 的协议数量、已有堆叠和空槽放置顺序混在一起。
- Root cause: 没有按每次 `AddItem` 的“先合并已有堆、再按物品类别找槽位”逐步记录操作前后状态，也没有把发送前 clone 与背包中的可变对象分开核算。
- Prevention: 物品 transcript 先列出每个 fresh item 的 UniqueID/Count、合并目标、剩余数量和最终槽位；wire clone 与库存终态分别断言，不能凭总数量推断槽位。
- Verification: 修正 `GIVEITEM/TAKEITEM` 单测和 net.Pipe 持久化断言后，定向测试与 Go 全量 test/race/vet/build 通过。

### 2026-08-11 — NPC 响应包不能作为动作执行完成屏障

- Symptom: `SET` 会话测试读到 `ServerNPCResponse` 后立即检查角色旗标，偶发看到旧值并误报迁移失败。
- Root cause: Go NPC handler 按 legacy transcript 先写响应包，再执行页面 Actions；`net.Pipe` 客户端读到响应时，服务端仍可能在执行 `SET`。
- Prevention: 会话测试在断言 NPC 动作副作用前发送并消费 `ClientKeepAlive`/`ServerKeepAlive`，把 keep-alive 返回作为当前请求处理完成的屏障。
- Verification: `SET` + `BREAK` + 下一页面 `CHECK` 会话测试改用 keep-alive 屏障后稳定通过。

- P7 source correction (2026-08-28): this historical conclusion described the
  then-current Go implementation, not Legacy authority. Current Legacy
  `NPCScript.ProcessSegment -> NPCSegment.Check -> Success/Failed` executes the
  selected action list and formats SAY text before `NPCScript.Response` enqueues
  `NPCResponse`; Go still sends most actions after the response. The symptom was
  treating a passing Go transcript as proof of Legacy order. Prevention is to
  trace the complete producer call chain before naming a packet a completion
  boundary. Verification: exact Legacy ranges `NPCScript.cs:895-917` and
  `NPCSegment.cs:2077-2967,4956-4974` were reread separately; P7 routes this
  ordering gap to `NPC-P7-CONTROL-FLOW-001` instead of preserving the old test.

### 2026-08-11 — NPC 链式页必须逐页执行特殊面板

- Symptom: GOTO/CALL 初版只在整条链最后按 active page 发送 shop/repair/refine 面板，链中间进入功能页时客户端只能收到文本，缺少对应功能包。
- Root cause: 控制流 job 队列只复用了页面文本和 actions，特殊页处理仍留在原请求末尾，没有随每个 job 的生命周期执行。
- Prevention: 把 NPC 页处理拆成“选择响应 → 执行动作/追加链 → 当前页特殊处理”，每个链式 job 都按自身 page key 跑 shop、repair、refine、storage 和 collect 分支，并分别锁定包序列。
- Verification: 新增 GOTO 到 refine 页和自循环上限的 net.Pipe 测试；现有 shop、repair、refine、storage 测试和控制流测试均通过。

### 2026-08-11 — 同一连接重新登录必须清空可见对象缓存

- Symptom: 任务登出后在同一连接重新 StartGame，服务器先发送任务状态，但没有重新发送 NPC 对象；会话验收按启动包序列阻塞。
- Root cause: logout 已移除 world player，却保留了 session 层 `visibleNPCs`/`visibleMonsters` 集合，下一次刷新把仍存在的静态对象误判为客户端已拥有。
- Prevention: 任何离开 Game stage 的路径都同时清理 world player、活动 NPC 状态和可见对象缓存；重进验收必须断言静态对象与任务状态都重新 bootstrap。
- Verification: `TestQuestSessionAcceptFinishAndPersist` 覆盖接取→登出保存→同连接重登恢复→完成→再次登出；Go 全量测试通过。

### 2026-08-11 — Quest NPC 校验不能信任客户端上下文

- Symptom: Go 实现曾把 `AcceptQuest.NPCIndex` 和当前活动 NPC 作为完成任务的额外条件，导致合法请求可能被拒绝。
- Root cause: 将“客户端打开了哪个 NPC”误当成了原版 `PlayerObject` 的授权条件；原版按任务定义查找 NPC，只检查当前地图和 DataRange。
- Prevention: request 中可由客户端伪造的 NPC ID 和 session 中缓存的 active NPC 只用于 UI/script 状态，不参与 Quest accept/finish 授权；回归测试必须覆盖错误 ID 仍能通过。
- Verification: Quest NPC range/request-ID 测试和同连接重登后的接取/完成会话测试通过。

### 2026-08-11 — 工具链格式化必须按语言分组

- Symptom: Quest 收尾时把 C# exporter 文件和 Go 文件一起传给 `gofmt`，命令在 C# 文件处报 `expected 'package'`，导致格式化门禁中断。
- Root cause: 只按本批改动文件列表执行格式化，没有按扩展名和项目工具链拆分命令。
- Prevention: 提交前按语言分别运行格式化和编译检查；Go 只交给 `gofmt`，C# 只交给 .NET SDK/对应格式化工具，并在命令失败后确认源码没有被部分修改。
- Verification: 重跑仅包含 Go 文件的 `gofmt`、`git diff --check`，随后单独检查 .NET SDK 可用性并记录未验证边界。
- Strengthening after recurrence: Mail 收尾再次把 `Program.cs` 放入 `gofmt` 参数后，禁止直接复用“全部改动文件”作为格式化输入；必须先按 `.go` 扩展名生成或人工核对参数清单，C# exporter 另行探测 `dotnet`/`csc`/`mcs`，本轮已用显式 Go 文件清单完成格式化。

### 2026-08-12 — 禁邮标志不能合并成一个通用失败分支

- Symptom: Go 初版把 `DontTrade`、`NoMail` 和 Rental `DontTrade` 都返回通用聊天加 `MailSent(-1)`，且聊天文本没有附带 legacy FriendlyName。
- Root cause: 只迁移了“物品不可邮寄”的领域结论，没有逐分支对照原版的聊天文本、FriendlyName 清洗和是否发送失败结果包。
- Prevention: 失败分类必须保留 definition/rental 标志来源；按原版先 trailing-digits、后方括号清洗名称，并分别锁定 `DontTrade`/Rental 仅 Chat、`NoMail` 为 Chat 加 `MailSent(-1)`。
- Verification: net.Pipe 会话分别覆盖 definition `DontTrade`、`NoMail` 和 Rental `DontTrade`，断言精确默认英文、差异化包序列和失败后背包状态；全量普通/race 测试通过。

### 2026-08-12 — 全局邮件锁不得跨网络写包

- Symptom: Refine 取消的误迁移邮件分支曾用 `defer` 释放 `mailMu`，导致后续取消结果包也可能在全局邮件锁内写入；慢客户端会阻塞其他邮件、交易和登出路径。
- Root cause: 锁的作用域按整个函数设置，而真正需要原子保护的只有邮件持久化及 world 快照同步。
- Prevention: `mailMu` 只包围邮件状态读取/提交，并保持 `mailMu -> world.mu` 顺序；在任何 `writePacket`、跨会话 Send 或日志保存前显式释放。
- Verification: 删除不符合原版可达语义的 Refine 邮件分支；其余 Mail/Trade 路径均先生成快照或通知、释放 `mailMu`，再执行网络投递。

### 2026-08-12 — 迁移分支必须证明可达性而非补全注释意图

- Symptom: Go 把 Refine 取消时“最后一个空槽被前一物品占用”的后续物品自动邮寄，并准备补发新邮件通知；原版此时不会进入邮寄分支，而会把物品保留在 Refine 工作台。
- Root cause: 只根据 `RefineCancel` 中的邮寄注释和 `MailInfo.Send` 推断意图，没有把外层空槽循环与 `CanGainItem` 的实现合并做可达性分析；原版只在先找到空槽后调用 `CanGainItem`，而后者发现任意空槽就直接返回 true。
- Prevention: 对看似存在的 legacy 分支同时核对调用条件、循环边界和被调函数，并构造状态表证明分支可达；不可达意图不能替代客户端实际可观察行为。
- Verification: Go 现在只为找到空槽的 Refine 物品发送 `RetrieveRefineItem`，背包填满后的后续物品继续留在工作台；单元测试和交互取消→KeepAlive→登出持久化 transcript 均通过。

### 2026-08-12 — 启动包排序必须区分角色物品与功能定义阶段

- Symptom: Quest 会话测试仍要求三个任务物品定义出现在 `MapInformation` 前，并一度准备把 Quest/Recipe 所需定义全部提前到地图包之前。
- Root cause: 把 Go bridge 的 `SelectInfo.ItemInfos` 目录与世界级 Quest/Recipe 定义混为一谈，没有沿 legacy `StartGameSuccess` 的实际调用顺序核对：`GetItemInfo` 只遍历角色三类物品格，随后才是 `GetMapInfo -> GetUserInfo -> GetQuestInfo -> GetRecipeInfo`；Recipe 的关联物品定义也由后置的 `CheckRecipeInfo` 发送。
- Prevention: 修改 bootstrap 顺序前同时列出 `StartGameSuccess` 调用链、每个 helper 的真实遍历范围和 FIFO `Enqueue` 行为；不能因为包都叫 `NewItemInfo` 就把不同来源统一提前。
- Verification: Quest 首次登录 transcript 改为 `MapInformation -> UserInformation -> quest NewItemInfo* -> NewQuestInfo`，通用 bootstrap 状态机继续区分角色物品和功能定义阶段；Go 全量普通/race 测试、vet、build 与 diff 检查通过。

### 2026-08-12 — 一次性异常排查不能变成固定汇报项

- Symptom: 用户只是临时询问一次异常现象，后续进度消息却持续重复相关分析，形成无关噪声。
- Root cause: 把一次诊断请求误解成了长期监控和每轮汇报要求。
- Prevention: 临时排查默认只回答当次；除非同类异常再次影响迁移、需要用户决策或用户明确追问，否则不主动重复诊断结论，也不把它加入固定进度模板。
- Verification: 后续迁移仅在整批完成、真实阻塞或需要用户决策时汇报，不再附带该一次性排查内容。

### 2026-08-12 — 公会默认创建费用必须解析 Legacy 物品名

- Symptom: Go 无 world export 时最初只要求 1,000,000 金币；补上默认 `WoomaHorn` 后，费用记录只有名称、索引为零，导致真实背包中的 Wooma Horn 无法匹配。
- Root cause: Legacy 默认配置先保存去空格物品名，再由 `LinkGuildCreationItems` 对完整物品目录解析；Go 把已解析的 `ItemInfo` 与未解析的默认名称当成了同一种输入。
- Prevention: 默认费用保留 Legacy 的金币加 WoomaHorn 两项；事务开始前按去空格、忽略大小写规则从角色物品定义解析名称，随后才执行原子校验和扣除。默认配置和 exporter 配置必须共用同一事务路径。
- Verification: auth 测试锁定 `WoomaHorn` → `Wooma Horn` 的解析与扣除，完整 NPC 会话锁定 `DeleteItem → LoseGold → GuildStatus`，Go 全量普通/race 门禁通过。

### 2026-08-14 — 状态魔法归属必须沿 Legacy 命中分支确认

- Symptom: 迁移矩阵和实现分析一度把 Slow/Frozen 当成 IceStorm 的待迁移状态，而 Legacy 的 IceStorm 实际只执行 3×3 MAC 区域伤害。
- Root cause: 根据技能名称和常见游戏直觉推断状态归属，没有先把施法入口、延迟命中 switch 和 `AddPoison` 调用点连成完整可达路径；真正施加 Slow/Frozen 的技能是 FrostCrunch。
- Prevention: 每批状态型技能先从 Legacy 命中 resolver 建立“spell → damage → secondary effect”表，再检索所有状态创建调用点交叉确认；文档、实现和测试必须共用该表，禁止从技能名称推断效果。
- Verification: Legacy spell switch 确认 FireBang/IceStorm 只有区域 MAC 伤害、FrostCrunch 命中后才执行 Slow/Frozen 的独立门槛与概率；Go 区域魔法测试保持 IceStorm 纯伤害 transcript，FrostCrunch 领域和三会话测试覆盖完整状态生命周期。

### 2026-08-14 — 当前批次收尾后停止的指令优先于长期迁移计划

- Symptom: 用户明确要求“干完这个就不要再做”时，既有计划仍包含分析和实现下一批功能，存在当前提交完成后继续扩展工作范围的风险。
- Root cause: 把长期迁移 Goal 的持续执行惯性当成当前轮默认，没有立即按最新的停止边界收窄计划。
- Prevention: 收到“完成当前批次后停止”指令后，只执行在途批次必需的测试、提交、C# 零变化检查和工作树核验；不得分析或实现下一批，也不得把尚未 100% 完成的整体迁移 Goal 标记完成。
- Verification: 本轮仅收口并提交 ProtectionField、Rage、SwiftFeet 及经验记录，核验两个仓库后停止，没有开启下一批源码修改。

### 2026-08-14 — Legacy 定宽整数进入领域类型必须显式转换

- Symptom: `RespawnTickOption.UserCount` 为 `int`，读取 Legacy `int32` 后直接赋值导致受影响包编译失败。
- Root cause: 二进制定宽字段进入 Go 领域模型时遗漏了显式窄/宽类型边界。
- Prevention: 读取 Legacy 整数先落到同宽临时变量，再显式转换到领域类型；新增解析字段后立即执行受影响包的编译门禁。
- Verification: 改为 `value.Options[index].UserCount = int(userCount)` 后，`internal/worlddata`、`internal/legacyworld`、`internal/config` 和 `cmd/crystal-server` 包级测试通过。

### 2026-08-15 — 长 Markdown 行补丁必须使用已读取的完整上下文

- Symptom: 更新 P5 矩阵时使用了带省略号的占位行作为 `apply_patch` 上下文，补丁校验失败，没有修改文件。
- Root cause: 没有先读取实际长行就假定其内容可用缩写匹配；Markdown 矩阵的整行字段不允许把省略号当作通配符。
- Prevention: 长行文档修改先用当前仓库的 `sed`/精确检索读取真实上下文，再做最小完整行替换；补丁失败后重新读取并确认文件未变，禁止凭失败输出继续判断。
- Verification: 失败补丁未产生写入；随后读取实际 P5 行并用完整上下文更新 AI=4 说明，`git diff --check` 将在提交前验证格式。
- Strengthening after recurrence: 本批再次把不完整上下文用于 AI=29 矩阵更新，补丁仍在校验阶段失败；以后每次长行更新必须先读取包含前后完整句子的固定行块，并只替换该实际块，禁止手写任何省略号或猜测尾句。
- Verification after strengthening: 第二次失败补丁没有写入；随后读取 `sed -n '108,132p'` 的真实 note，按完整上下文修正重复句并加入 AI=29 说明，Go 文档 diff 检查保持通过。
- Strengthening after second recurrence: AI=100 更新虽使用了真实上下文，仍把被匹配的保留句再次复制进 replacement，产生重复文本；以后采用“删除旧句再插入新句”的最小补丁，应用后必须立即 `sed` 复读目标块并检查相邻句不重复。
- Verification after second strengthening: 重复句在提交前已被移除，AI=100 note 的实际块复读正确；Go 定向测试继续通过，文档 diff 检查将在门禁中复核。
- Strengthening after third recurrence: AI=65 更新第一次仍按视觉换行猜测物理行边界，导致完整上下文匹配失败；长 Markdown 补丁前必须用 `sed -n l` 确认实际行边界，并优先锚定已读取的短、唯一连续句。
- Verification after third strengthening: 失败补丁未产生写入；随后按 `sed`/`sed -n l` 的真实行块成功加入 AI=65 说明，Go 文档 `git diff --check` 与全量门禁通过。
- Strengthening after fourth recurrence: 即使目标段落已复读，替换上下文中的标点或斜杠差异也会使 Markdown 补丁整块拒绝；应用前必须逐字复制实际行，补丁失败后重新读取目标段，禁止凭近似文案重试。
- Verification after fourth strengthening: 本批 AI=94 矩阵首次补丁因把实际 `HP/私有包` 写成近似标点而拒绝，未产生写入；重新检索真实行后按精确上下文追加 lessons，Go 文档随后通过差异检查。
- Strengthening after fifth recurrence: AI=97 矩阵更新再次因把视觉折行和近似句子当作完整上下文而拒绝；以后长段落只在 `sed -n l` 确认物理行后，以已读取的短唯一句插入，禁止把相邻保留句复制进 replacement。
- Verification after fifth recurrence: 失败补丁未写入；随后按真实 `packet matrices are covered.` 行重新插入 AI=97 说明，Go 文档 `git diff --check` 和全量门禁通过。

### 2026-08-15 — Go AI 距离常量必须匹配 int32 几何运算

- Symptom: AI=108 MudZombie 首次包级编译因 `distance > legacyMudZombieLineDistance` 的 `int32`/`int` 比较失败。
- Root cause: 新增距离常量显式声明为 `int`，而世界坐标和 `maxInt32` 结果使用 `int32`；Go 不允许这两个不同的具体类型直接比较。
- Prevention: 几何阈值优先使用无类型常量，或在声明处与坐标运算统一为 `int32`；新增 AI 后立即执行包级编译，不把类型错误留到全量门禁。
- Verification: 将 MudZombie 线距离改为无类型常量后，包级编译和四个 MudZombie 定向测试通过。

### 2026-08-15 — WhiteMammoth PoisonTarget 的 chance=0 只做一次抗性检定

- Symptom: WhiteMammoth stomp 初版动作把 `PoisonTarget(..., 0, ...)` 迁移为两次抗性检定，和 Legacy 的可观察抵抗概率不一致。
- Root cause: 沿用了其他已迁移怪物动作的双检定字段，没有逐行读取 WhiteMammoth 的 `PoisonTarget` 调用及其单次 `PoisonResistWeight` 门禁。
- Prevention: 每个新 AI 的毒效果都要独立展开 chance、抗性检定次数、值计算顺序和状态包；不能从相邻 AI 复用默认次数。
- Verification: WhiteMammoth 动作断言固定 `PoisonResistChecks=1`，并用抗性 roll 计数测试确认 stomp 只消费一次抵抗检定。

### 2026-08-15 — 本批次两次路径边界复发仍须整体作废错误证据

- Symptom: 本批次一次 Go 命令误带 Legacy 路径，另一次 Legacy 命令误带 Go 路径；两次均只读失败、未产生写入，但输出不能用于迁移判断。
- Root cause: 切换仓库后复用了上一调用的路径参数，没有把绝对 `workdir`、根目录校验和相对路径 allowlist 绑定为一个不可拆分的调用契约。
- Prevention: 跨仓库读取/写入拆成独立工具调用；每次先核对 `git rev-parse --show-toplevel`，命令参数只允许当前仓库路径，出现混合路径或非零结果时整体丢弃并重跑。
- Verification: 两次错误调用均在读取/启动阶段失败且两个工作树无源码变化；随后按 Legacy、Go 分开的已核验根目录重新读取，后续实现和测试未使用错误输出。

### 2026-08-17 — PeacockSpider cooldown must stop target movement

- Symptom: The first PeacockSpider ranged `net.Pipe` transcript received an extra `ObjectWalk` during the delayed projectile impact, and the deterministic roll fixture unexpectedly reached the movement fallback bound.
- Root cause: The migrated specialized `ProcessTarget` path treated `!CanAttack` as permission to chase; Legacy returns immediately during ActionTime/AttackTime recovery before evaluating attack range or movement.
- Prevention: For every delayed monster AI, model the `Target == null || !CanAttack` early return before the attack-range/movement branch; add a transcript assertion that the cooldown tick emits no movement packet and does not consume movement rolls.
- Verification: PeacockSpider's cooldown/session test now passes with only the ranged packet at generation and the ordered damage/paralysis packets at impact; the full `cmd/crystal-server` suite passes.

### 2026-08-17 — DarkCaptain 范围、推退与传送必须按客户端副作用验收

- Symptom: DarkCaptain 初版范围测试把半径边界、Fullmoon 推退和传送后的目标切换当成单一伤害行为，遗漏了边界之外目标、`ObjectPushed` 广播及传送后下一 tick 重新选目标的可观察结果。
- Root cause: 只按技能名称概括了 AI 效果，没有分别展开 Thunder/MassThunder 的范围比较、Fullmoon 的每目标推退通知和 `Teleport` 对缓存目标字段的同步；真实会话还可能被后台维护 tick 抢先消费。
- Prevention: 新 AI 测试先建立坐标半径/目标种类矩阵，再按每个目标的私有与观察者包序断言；推退必须验证坐标、方向和 `ObjectPushed`；传送改变实体人口后同步 `MonsterAITargetID/Kind`，真实 `net.Pipe` transcript 在手工 tick 前冻结初始化、搜索和动作时间并用 KeepAlive 屏障。
- Verification: DarkCaptain world 测试覆盖 Player、owned Monster/Hero、半径边界、Thunder/MassThunder、Orb、Line/Fullmoon、传送、延迟伤害和 Slave 清理；authenticated `net.Pipe` transcript 与包级定向回归通过，修复后的传送后弱目标选择测试确认缓存目标已同步。

### 2026-08-18 — Kirin 迁移前必须核对实际 Go 包布局

- Symptom: Kirin 对照期间一次只读调用访问了不存在的 Go `internal/world/` 路径，读取失败且没有产生写入。
- Root cause: 先按预想的目录结构拼接路径，没有从 Go 仓库根目录核对实际文件布局；实现包实际位于 `cmd/crystal-server`。
- Prevention: 路径定向读取前先在目标仓库执行根目录确认和 `rg --files` 文件定位；失败读取的全部输出作废，不据此判断源码状态。
- Verification: 重新从 Go 仓库根目录定位并读取 `cmd/crystal-server` 的 world/AI 文件，随后包级编译与 Kirin 定向测试通过。

### 2026-08-18 — IcePhantom 批次选择与新增 helper 必须以当前矩阵和签名为准

- Symptom: 延续摘要把已完成的 AI=112 指向为下一批；切换到 IcePhantom 后首次包级编译又把 `*worldMonster` 传给按值接收的攻击力 helper，编译失败。
- Root cause: 使用了过期的会话状态而不是当前迁移矩阵，并在新增 AI 文件中没有在调用点复核 helper 的接收者和值/指针边界。
- Prevention: 选批次先核对当前矩阵、工厂启用状态和源码缺口；每次新增 AI 接线后立即执行 `gofmt` 与 `go test ./cmd/crystal-server -run '^$' -count=1`，并显式检查 helper 签名。
- Verification: 当前矩阵/工厂核对后改为 AI=178；攻击力调用改为显式解引用，IcePhantom 包级编译、世界测试和会话测试通过。

### 2026-08-18 — AntCommander Green poison 恢复值只能应用一次

- Symptom: 对照 Player 的 HumanObject.ApplyPoison 语义时发现 Green poison helper 先手动扣除 PoisonRecovery，再调用同样会扣恢复值的共享 Go helper；恢复值非零会把持续时间重复缩短。
- Root cause: 将目标类型的 Legacy ApplyPoison 逻辑和通用 `addPlayerAppliedPoisonLocked` 的已迁移恢复处理重复拼接。
- Prevention: Player/英雄 Green/Red poison 统一只经过各自的 applied-poison helper；专用代码只保留 Dazed 的额外 effect/chat 行为，并为非零恢复值建立回归夹具。
- Verification: AntCommander 远程 Green poison 测试设置恢复值 2 并确认 7 秒变为 5 秒；定向 AI=196、认证 session 与完整服务端包测试均通过。

### 2026-08-18 — PetEnhancer 成功 admission 必须恢复 Cast 标志

- Symptom: PetEnhancer 已正确通过友方宠物判定并入队 500ms action，但定向测试收到的 `S.Magic.Cast` 仍为 false；EnemyGuild 与自身宠物的成功路径也被误判为失败。
- Root cause: Go 分支为复现 Legacy 失败目标先把 `result.Magic.Cast` 置为 false，却遗漏了 valid target 分支中的 true 赋值。
- Prevention: 对“失败时 Cast=false、成功时 Cast=true”的法术，在 admission 测试中同时断言成功/失败的 `S.Magic`、`FoundTarget`、目标 ID 和 action 数量，并在成功分支紧邻目标校验恢复 Cast=true。
- Verification: 补上成功分支赋值后，PetEnhancer 延迟 Buff、友好模式矩阵和延迟对象存在性定向测试全部通过。
- Additional symptom: 首次新增测试在解析 AddBuff payload 时引用了循环外不存在的 `err`，包测试编译失败。
- Additional root cause: `wire` 已在外层声明，解析结果需要在循环内用短变量声明并回填，不能直接给未声明的函数返回错误变量赋值。
- Strengthened prevention: 对循环内函数调用使用 `parsed, err := ...`，成功后再赋值给外层结果；先运行目标包的编译/定向测试，再进入全仓库门禁。
- Additional verification: 修正局部变量后，`go test ./cmd/crystal-server -run 'TestPetEnhancer' -count=1` 通过。

### 2026-08-18 — 新增 Go 法术辅助函数要先检查包级符号

- Symptom: FireWall 首次定向编译失败，新增的 `movePointPair` 与现有 `kirin.go` 包级函数重名。
- Root cause: 新文件抽取坐标辅助函数时只检查了当前文件，没有先在整个 `cmd/crystal-server` 包内搜索同名符号。
- Prevention: 新增 Go 包级函数前先用 `rg -n "func .*候选名" cmd/crystal-server --glob '*.go'` 检查全包命名；通用能力优先复用现有函数，专用 helper 使用明确前缀。
- Verification: helper 改为 `fireWallMovePointPair` 后，重新运行定向编译并继续 FireWall 行为测试。

### 2026-08-18 — 结构字段补丁要避免同一 struct 的重叠插入

- Symptom: 元素系统首次定向编译报 `worldPlayer` 的四个元素状态字段重复声明。
- Root cause: 两个相邻字段锚点都属于同一个 `worldPlayer`，结构补丁把同一组字段插入了两次。
- Prevention: 修改结构前先用 `sed -n ...l` 确认完整 struct 边界；同一组字段只选一个语义相邻锚点，补丁后立即做最小包编译。
- Verification: 删除动作字段处的重复组，保留集中术状态旁的一组；`go test ./internal/protocol ./cmd/crystal-server -run '^Test(SetElemental|Elemental)$'` 通过。

### 2026-08-18 — 新增 Go 文件要先清理 import 并核对领域结构字段

- Symptom: 定向 Go 测试首次编译同时报新文件的 `protocol` import 未使用，以及访问不存在的 `worldMonster.InSafeZone` 字段；本批 HellFire 新文件再次出现未使用的 `protocol` import。
- Root cause: 先按 Legacy 类型直译了安全区判断，且新增文件完成后没有立即清理 import/跑最小编译。
- Prevention: 新增 Go 文件完成后先运行 `gofmt` 和目标包最小编译，并逐项检查 import；跨模型字段使用前先查实际 struct 定义，安全区等派生状态通过现有 world helper 获取。
- Verification: 删除 HellFire 的未使用 import，删除旧错误字段访问并改用 `positionInSafeZoneLocked`；随后 HellFire、Lightning/HeavenlySword 定向测试均通过。

### 2026-08-18 — 新增 MagicDefence 字段不得改写通用玩家魔法防御

- Symptom: diff review 发现把通用延迟玩家魔法的固定 `playerArmour(..., true)` 改成了新动作字段表达式；未修正时，未标记线技能的旧法术可能错误地切换防御语义。
- Root cause: 为复用新线技能的 MAC 标志，误把新增动作元数据扩展到了已有通用玩家路径；Legacy 通用玩家魔法本来就始终使用 MAC。
- Prevention: 新增动作字段只在新分支消费；修改共享 resolver 前先保存旧分支的防御参数，分别核对玩家与怪物的 Legacy armour 语义。
- Verification: 恢复通用玩家路径的固定 MAC，线技能路径显式传入 `MagicDefence=true`；全量 Go 测试、race、vet 和 build 均通过。

### 2026-08-18 — Hallucination 要同时迁移结算状态与通用 AI 搜索门控

- Symptom: Hallucination 施法、延迟成功、Amulet 消耗和 MediumOrchid 颜色测试已通过；首次回归审查发现 Go 通用怪物搜索路径没有读取 HallucinationTime，若只保留专用 AI 的已有门控，普通 AI 仍可能在效果期间重新锁定玩家。新增测试夹具初次编译还把 uint32 对象 ID 直接填入 int32 的 MonsterInfo.Index。
- Root cause: 实现初版只按 HumanObject.CompleteMagic 的直接状态写入迁移，遗漏了 MonsterObject.ProcessSearch 的下游消费者；测试夹具没有先核对 worlddata.MonsterInfo.Index 的实际类型。
- Prevention: 迁移带计时字段的效果时，沿字段引用逐项检查显示、tick、AI/目标搜索和过期路径，并为每个下游行为添加确定性断言；新夹具先核对结构体字段类型，再运行目标包最小编译。
- Verification: 补上通用 AI 搜索在 HallucinationTime 内跳过、到期后恢复搜索的测试，并修正 Index 类型；Hallucination 世界/认证 net.Pipe 测试、go test ./...、go test -race ./cmd/crystal-server、go vet ./... 和 go build ./... 均通过。

### 2026-08-18 — MoonLight/DarkBody 迁移要覆盖近战增伤消费者

- Symptom: MoonLight 的施法、隐身、过期和受击揭示测试通过后，审查发现 Go 近战仍只使用基础 DC，没有实现 Legacy `Attack` 在揭示前加入 `magic.GetPower()` 的行为。
- Root cause: 只按法术入口和 Buff 状态迁移，遗漏了同一 Buff 在普通近战伤害计算中的下游消费者。
- Prevention: 每个新增 Buff 都沿 Legacy 类型的全部引用检查施法、移动/攻击揭示、伤害/防御计算、过期和登录恢复；至少添加一个带确定性 `MPowerBase` 的命中伤害断言及包顺序断言。
- Verification: Go 已在近战开始前清理 MoonLight/DarkBody、捕获对应等级的 spell power 并加入 DC；等级 0 MoonLight 的 5 点增伤和 `RemoveBuff -> ObjectHidden -> ObjectAttack` 顺序测试通过。

### 2026-08-18 — MoonMist 的练习时机要同时覆盖 admission 与 delayed train

- Symptom: MoonMist 的首次 Go 迁移只在 admission 发送 `MagicLeveled`；对照 `Map.CompleteMagic` 后发现命中目标时延迟分支仍设置 `train=true`，因此成功命中少练习一次。
- Root cause: 只读取了 `HumanObject.MoonMist` 的立即 `LevelMagic`，没有沿用 `CompleteMagic` 末尾的统一 `if (train) player.LevelMagic(magic)`，把“立即练习”和“命中后再次练习”误合并成一个行为。
- Prevention: 对每个同时有 admission 方法和 delayed `CompleteMagic` case 的法术，分别记录每一处 `LevelMagic` 调用及其条件；测试既断言 admission 包，也断言命中/未命中后的最终经验和包序。
- Verification: MoonMist resolver 已在任一 AC 命中后追加一次 `levelMagicLocked`，未命中仍不追加；世界测试确认经验 1→2，认证转录确认延迟施法者包尾为 `MagicLeveled`，定向测试通过。

### 2026-08-19 — Go 多返回值必须先解包再放入定长坐标

- Symptom: IceThrust 首次针对性编译失败，`movePoint` 的 `(int32, int32)` 返回值直接放入 `[2]int32` 字面量，被 Go 报为 single-value context。
- Root cause: 将多返回值函数误当作可自动展开的复合字面量元素；Go 不会在这种位置隐式解包。
- Prevention: 坐标函数的多返回值先赋给 `x, y` 局部变量，再构造数组/结构体；新增几何代码先运行目标包编译测试。
- Verification: 失败输出用于修复依据；修正三个 IceThrust 起点后 `go test ./cmd/crystal-server -run 'IceThrust' -count=3`、目标包 race 测试和全仓库测试均通过。

### 2026-08-19 — 选择迁移批次前必须核对当前源码而非陈旧矩阵

- Symptom: IceThrust 完成后根据 README 将 `LoginSuccess` 当作仍待迁移的 P3 批次，随后才发现 Go 登录流程、协议序列化和现有测试已经完整覆盖。
- Root cause: 直接信任陈旧文档中的待办状态，没有先用当前源码、测试和 Legacy 对照确认功能是否已存在。
- Prevention: 选择下一批前逐项核对 Go 入口、协议编码、行为测试和迁移矩阵；源码与测试是权威，文档只用于发现候选，已实现的功能不得重复修改。
- Verification: 独立核对 Go `main.go`/`internal/protocol/packet.go`、Legacy `ServerPackets.cs` 及现有测试后确认 `LoginSuccess` 已完成；该误判未产生代码改动，后续候选改为重新研究的缺失法术。

### 2026-08-19 — Legacy CrippleShot Vampire 区域必须累计 VampAmount

- Symptom: 初版 Go 区域实现能重复命中主目标并清零 VampireShot，却没有增加 VampireAmount。
- Root cause: 只复用了普通 VampireShot 命中后的累计逻辑，遗漏了 Legacy 3x3 循环中每个符合条件的 targetob 都执行一次 VampAmount 更新。
- Prevention: 对区域技能逐语句核对循环体内的副作用；除伤害和状态外，必须单独列出每个 cell object 触发的资源累计、计时和广播。
- Verification: 重新读取 HumanObject.cs 的 CrippleShot 分支确认每个区域对象都累计 value×(level+1)×0.25；Go 已提取共享累计 helper，并加入双目标 VampAmount 世界断言。

### 2026-08-19 — BattleCry 最高等级练习不会增加经验

- Symptom: BattleCry level-3 threshold 测试命中目标后预期经验增加 1，但实际经验保持 0。
- Root cause: 测试把“命中触发 LevelMagic”误当成“最高等级仍会累计经验”；Go/Legacy 的 `LevelMagic` 对 level 3 直接结束，不发练习进度。
- Prevention: 新增技能练习断言前先按当前 magic level 检查 `LevelMagic` 的 level 0/1/2/3 分支；最高等级只断言效果，不断言经验或 `MagicLeveled` 增长。
- Verification: 将 level-3 命中期望改为经验不变，BattleCry 世界/会话定向测试通过；level 0/1/2 仍验证一次练习经验。

### 2026-08-18 — 新法术支持列表与飞行规则必须分别核对

- Symptom: CatTongue 初次编译时在同一个 Go 支持列表中被加入两次；预期加入的是 CanFly 路径规则，结果形成重复 switch case。
- Root cause: 修改 `worldSpellBehaviorSupported` 和 `worldMagicRequiresCanFly` 时只按搜索结果插入，没有在补丁后检查每个法术在各职责列表中的唯一位置。
- Prevention: 新增法术时分别核对“行为支持、路径校验、影响解析”三个入口，并在编译前检索法术常量的全部 case，确保每个 switch 只出现一次。
- Verification: 删除重复 case、将 CatTongue 加入正确的 CanFly switch 后，Go 包编译、CatTongue 世界/会话测试及 race 测试通过。

### 2026-08-18 — Go 多返回值必须先显式接收再组合

- Symptom: HalfMoon/CrossHalfMoon 初次编译失败，把多返回值函数调用直接作为带其他参数的 helper 实参时触发 `multiple-value ... in single-value context`。
- Root cause: Go 不允许在普通多参数调用中把一个多返回值表达式隐式展开为多个实参。
- Prevention: 组合技能结果前先用有意义的局部变量显式接收每个返回值，再逐项写入结果结构或通知列表；新增 helper 后先运行目标包编译。
- Verification: 拆分为 `impactHit`、`impactKilled`、`impactArmour`、`impactNotifications` 后，HalfMoon/CrossHalfMoon 世界测试、CrescentSlash 回归测试和会话测试通过。
- Strengthening after recurrence: BladeAvalanche 起点组装再次触发同类错误；凡是坐标/范围 helper 返回多个值，必须先解包到命名变量，再组装数组或结构体。
- Verification after strengthening: 三个 BladeAvalanche 起点改为显式 `leftX,leftY` 等变量后，目标包重新通过编译。

### 2026-08-19 — 生产代码不能依赖 `*_test.go` helper

- Symptom: Thrusting helper 的初稿曾引用只在 `hiding_buffs_test.go` 声明的 `mustWorldSpellInfo`，若直接构建生产包会出现未定义标识符。
- Root cause: 复用了测试夹具 helper，却没有先确认声明所在文件及其是否属于非测试构建。
- Prevention: 生产代码只调用非测试文件中的 helper；新增调用前用 `rg` 核对声明文件，并在定向测试后运行 `go build ./...`。
- Verification: 已改用生产 helper `worldSpellInfoFor`；Thrusting 世界/会话测试、race、`go vet ./...` 和 `go build ./...` 均通过。

### 2026-08-19 — DoubleSlash 队列插入顺序不等于命中语义顺序

- Symptom: DoubleSlash 首次定向测试在 300ms 断言目标 HP 为 87，实际为 84；执行的是无护甲的敏捷段，而不是应先减 3 点 MAC 的 MACAgility 段。
- Root cause: 将 Legacy 先入队的 400ms action 误命名为第一命中，忽略了 `HumanObject.Attack` 在分支内先排入 400ms Agility、分支后再排入 300ms MACAgility 的结构。
- Prevention: 迁移延迟技能时分别记录“队列插入顺序”和“Due 时间/防御类型语义”；测试同时断言 action 排列、Due、防御标志及每个时间点的 HP。
- Verification: 修正为 400ms Agility action 先入队、300ms MACAgility action 后入队；DoubleSlash 世界测试、会话测试和旧 Warrior 回归测试通过。

### 2026-08-19 — TwinDrake 的 PvP Stun 外层条件不是“关闭抗性即必定尝试”

- Symptom: 初步阅读把 `PvpCanResistPoison=false` 理解成玩家目标跳过抗毒随机并继续尝试 Stun，可能错误地给 PvP 目标附加效果 5。
- Root cause: `HumanObject.CompleteAttack` 先判断 `((target.Race != Player) || Settings.PvpCanResistPoison)`，默认配置为 false 时玩家目标直接不进入 `PoisonAttackWeight` 和 40 分支；该设置只影响 `ApplyPoison` 的后续抗性，不会反转外层条件。
- Prevention: 迁移带配置短路的状态效果时，分别展开外层资格条件、抗性随机、等级门和成功随机；不要用配置字段名称的直觉含义替代布尔表达式，并单独验证默认 PvP 配置下的 Player/Monster 分支。
- Verification: Go 实现保留默认 `sepWarriorPvpCanResistPoison=false` 的 Player 短路，Monster/Hero 使用权重 10、等级 `< attacker+10` 和非玩家上限 20；TwinDrake 世界、会话和 race 定向测试通过。

### 2026-08-19 — Go 世界数据 stat 常量要与字段来源区分

- Symptom: BladeAvalanche authenticated session 测试首次包级编译找不到 `monsterStatMinMAC`/`monsterStatMaxMAC`。
- Root cause: 把运行时怪物字段名称误当成 `worlddata.MonsterStat.ID` 常量；项目统一使用 `statMinMAC`/`statMaxMAC` 作为数据表 stat ID。
- Prevention: 新建世界数据 fixture 时先查 `worlddata` 加载映射和同类测试，区分 `monsterStat*` 的 HP/DC 常量与 `stat*` 的通用属性常量。
- Verification: 将 session fixture 改用 `statMinMAC`/`statMaxMAC` 后重新执行目标包编译测试。

### 2026-08-19 — CounterAttack 成功排队后立即消费运行时旗标

- Symptom: 玩家反击测试在动作已排队、伤害值正确时失败，入站伤害返回后 `CounterAttack` 已为 false。
- Root cause: 测试把“触发前已武装”误当成“成功排队后仍武装”；Legacy `CounterAttackCast` 在发送 ObjectMagic 和加入 DelayedAction 后立即执行 `CounterAttack = false`，只保留已排队的延迟伤害。
- Prevention: 分别断言触发前的 armed 状态、成功排队后的 one-shot 消费状态，以及失败距离/概率分支的保留状态；不要用同一时点断言覆盖三个阶段。
- Verification: 将入站命中断言改为成功排队后必须清除旗标，并保留失败距离测试验证失败时仍为 armed。
### 2026-08-19 — CounterAttack 触发点必须保留受击副作用顺序

- Symptom: 初版 Go 反击在共享玩家伤害函数进入成功命中分支后立即调用，早于 Legacy 的 GatherElement；若攻击者同时触发元素收集，反击 ObjectMagic 会被错误地排在该副作用之前。
- Root cause: 只按“反击包必须早于 Struck/ObjectStruck”定位调用点，没有完整对照 `HumanObject.Attacked` 的 GatherElement、状态清理、CounterAttackCast、Struck 顺序。
- Prevention: 接入受击触发器时先列出成功命中分支的所有可见副作用，再按 Legacy 的相对顺序插入；测试应覆盖触发包与已有副作用同时存在时的包序，而不是只验证无副作用夹具。
- Verification: Go 调用已移动到 `gatherElementLocked` 之后、Struck 之前；CounterAttack 世界/会话定向测试及完整定向 Warrior 回归通过。

### 2026-08-19 — LionRoar 的 Legacy ApplyPoison 必须保留零初始 TickTime

- Symptom: LionRoar 首版 Go world test 能写入 `LRParalysis`，但 impact tick 没有发布 `ObjectPoisoned`；同时 session 期望的首次毒状态与 `Elapsed` 不一致。
- Root cause: Legacy `Map.CompleteMagic` 构造的 `Poison` 只设置 `Duration`/`TickSpeed`，`Time`/`TickTime` 初始为零；迁移时错误地预填了 `TickAt = now + 1s`，跳过了同一 world tick 的首次状态计算。
- Prevention: 迁移毒效果时必须逐字段对照 Legacy 构造器；没有显式设置 `Time`/`TickTime` 就保持 Go `TickAt` 为零，让 `tickPoisons` 在施毒 impact 立即计算 `CurrentPoison`/`ObjectPoisoned`，并断言首个 `Elapsed`。
- Verification: LionRoar world 与认证 `net.Pipe` transcript 均验证了 impact 时 `MagicLeveled -> ObjectPoisoned`、`Elapsed=1`、`Duration=magic.Level+2` 和后续持续状态。

### 2026-08-19 — 相似上下文补丁后必须核对唯一落点

- Symptom: 新增 `ObserverPackets` 的补丁第一次命中了攻击处理分支，而不是预期的 Magic 处理分支。
- Root cause: 两处代码共享相同的 `ObserverPacket` 条件，补丁上下文过短，未在编辑后立即确认落点。
- Prevention: 对重复上下文只使用带有相邻业务语句的唯一补丁锚点；每次补丁后立即用 `rg` 和局部 `sed` 检查命中位置，再继续实现或测试。
- Verification: 本轮在编译前发现并移除了攻击分支中的错误循环，`rg` 已确认 `ObserverPackets` 只位于 Magic 处理路径和结果结构中。

### 2026-08-19 — 新世界行为实现必须先核对现有结构字段

- Symptom: ShoulderDash 最小编译先后报 `worldPlayer` 不存在 `Dead` 字段、NPC map value 不能与 `nil` 比较，以及分支调用参数仍多传一个 request。
- Root cause: 新 helper 根据 Legacy 对象模型推断了 Go runtime 的字段和值类型，接口签名调整后没有立即同步所有调用点。
- Prevention: 写新世界 helper 前先读取目标 Go struct/map 定义并搜索所有调用点；每次签名或字段调整后先运行目标包的 `go test -run '^$'` 编译门槛，再扩展行为测试。
- Verification: 本轮编译失败已定位到三处具体假设，已分别改为 HP 判断、值类型 NPC 访问和正确的 helper 参数；随后目标包最小编译、ShoulderDash world/session 测试及全量 build 均通过。

### 2026-08-19 — Go append 混合单值与展开切片必须拆分

- Symptom: ShoulderDash 失败分支编译时报 `too many arguments in call to append`。
- Root cause: Go 不允许在同一次 `append` 调用中把一个普通元素和另一个切片的 `...` 展开参数混用。
- Prevention: 构造有序通知 transcript 时，先用一次 `append(slice, element)` 插入私有包，再用独立的 `append(slice, otherSlice...)` 插入广播包；补丁后立即运行目标包的最小编译测试。
- Verification: 拆成两次 append 后，ShoulderDash world/session 定向测试与目标包编译均通过。

### 2026-08-19 — NPC 输入按钮必须按 Legacy 的 `/@@KEY` 语法构造

- Symptom: 新增 `NPCConfirmInput` 会话测试首次使用 `/[@@INPUT]`，Go 正确解析了页面文本但没有把按钮登记为 `[@@INPUT]`，第二次调用无响应并在 net.Pipe 超时。
- Root cause: NPC 按钮解析器从 `/@` 标记开始提取键名；方括号属于解析器生成格式，输入源应写 `/@@INPUT`，不能把方括号再放进源文本。
- Prevention: 新增 NPC 脚本夹具先用 `ParseNPCScript` 断言 `page.Buttons`，再写网络 transcript；严格区分源脚本链接语法与客户端传输的方括号键名。
- Verification: 改为 `/@@INPUT` 后，协议/脚本单测及认证 net.Pipe 测试连续通过，并观察到 `NPCRequestInput -> NPCResponse -> GainedGold` 顺序。

### 2026-08-19 — NPC 输入批次全量回归仍需标注既有基线失败

- Symptom: 本批 `go test ./... -count=1` 仍失败于 Blizzard/MeteorStrike 两个 30 秒 net.Pipe 超时、HealingCircle 30 秒超时和 TucsonMage 额外 `[61 72]` 包。
- Root cause: 与上一会话记录的实时 ticker/客户端消费边界基线完全相同，失败不涉及 NPC 输入、协议 payload 或脚本动作替换。
- Prevention: 保留失败测试名、超时和额外 packet 作为基线证据；以新增会话测试、相关包 race、全仓库编译和 vet/build 作为本批验收，不为无关基线改动 NPC 实现。
- Verification: 账户二进制导入批次于 2026-08-19 08:11–08:13 UTC 再次得到同一失败指纹；`internal/legacyaccount`、导入命令、所有内部包、`go test ./... -run '^$'`、`go vet ./...` 与 `go build ./...` 均通过，确认该基线未扩散到本批。
- Strengthened prevention: 全量回归报告必须同时给出失败测试的精确名称/超时、受影响的服务端包，以及新增相关包和全仓静态检查结果；只有失败指纹发生变化才进入本批代码归因。
- Verification: `NPCInput`/`BuyItemBack` 目标测试、相关 `go test -race` 和 `go test ./... -run '^$'` 均通过；全量失败集合与既有 lessons 完全一致。

### 2026-08-19 — 元数据批次全量服务端回归可能超过常规基线时限

- Symptom: 元数据保留批次的 `go test ./... -count=1` 在服务端包无输出运行约 6 分钟后中止；隔离 `go test ./cmd/crystal-server -count=1 -timeout=180s` 最终在 Blizzard/MeteorStrike 用例上超时，栈同时显示 net.Pipe 读端等待和服务端 `writePacket` 锁等待。
- Root cause: 既有实时会话测试的生产者/客户端消费边界在全包串行压力下会把已知 30 秒 net.Pipe 超时放大为测试进程级等待；本批只改协议桥接字段与账户/auth 持久化，没有修改 `cmd/crystal-server` 的会话实现。
- Prevention: 全量服务端回归必须设置明确的 package timeout；出现无输出挂起时先中止并用带 `-v` 的单个失败测试和隔离服务端包复现，不能把放大的等待静默视为通过，也不能未经定向证据修改无关会话代码。
- Verification: 元数据定向测试、相关 race、全仓编译、vet/build 通过；单独 Blizzard/MeteorStrike 测试仍稳定复现两个 30 秒 net.Pipe 失败，确认异常属于同一基线族但本次全量耗时更长。

- Strengthened evidence: 运行时账户 checkpoint 批次使用 `go test ./... -count=1 -timeout=180s` 仍在 `cmd/crystal-server` 失败：Blizzard/MeteorStrike 各约 30 秒 `read/write on closed pipe`，随后 HealingCircle 在总计约 181.7 秒时触发测试 timeout；auth/config/legacyaccount/legacyaccountbridge 及其 race、全仓编译、vet/build 均通过，checkpoint 代码不在失败栈中。
- Strengthened prevention: 以后全量服务端回归固定带 `-timeout`，同时保留“先通过的内部包 + 精确服务端失败测试/耗时 + 独立定向复现”的三段证据；只有新增失败包或栈进入本批代码才归因并修复。

### 2026-08-19 — 启动入口抽取后必须重新声明原 main 作用域变量

- Symptom: 将生产启动流程从 `main` 抽到可测试函数后，目标包编译报 `undefined: err`。
- Root cause: 原 `main` 中配置加载声明的局部 `err` 被重构边界带走，新函数仍使用赋值形式加载 world export，却没有自己的错误变量。
- Prevention: 抽取入口函数时逐项检查原函数局部变量的声明范围；完成结构重构后先运行目标包的最小编译测试，再添加或解释行为断言。
- Verification: 在启动 helper 中补充 `var err error` 后，重新执行目标启动/关闭测试，确认编译错误消失并继续验证实际重启链路。

### 2026-08-19 — Reincarnation 施法包顺序必须按入队点还原

- Symptom: 初版 Go transcript 将 `DeleteItem` 放在 `UserLocation` 之前；对照 Legacy 后发现施法者实际顺序是 `HealthChanged -> UserLocation -> Chat -> ObjectSpell -> DeleteItem -> Magic`。
- Root cause: 只按业务阶段组织 `worldMagicResult.SelfPackets`，没有追踪 Legacy `Magic`、`SpellObject.Spawned`、`Chat` 和 `ConsumeItem` 各自的入队位置。
- Prevention: 迁移任何多包动作时，先按源码调用顺序列出每个包的入队点与收件人，再决定放入 self、pre-notification 或 delayed 队列；session transcript 必须覆盖施法者和目标两端。
- Verification: 将 Reincarnation 删除包移到 pre-Magic 通知尾部后，Legacy 对照顺序与 Go 双会话 transcript 一致，定向 Reincarnation 测试通过。

### 2026-08-19 — 选迁移批次前必须核对实际运行时调用链

- Symptom: 看到 Go `ItemAwakening` 注释称觉醒效果尚未应用后，把它误选为待实现的 P11 缺口。
- Root cause: 只依据陈旧注释判断状态，没有先沿 `addItemStats`、装备统计计算和玩家刷新链路核对；实际 Go 已按六种觉醒类型把值投影到 DC/MC/SC/AC/MAC/HP+MP。
- Prevention: 从 TODO、注释或矩阵选择批次前，必须搜索真实运行时入口和调用链；已有行为只补覆盖测试/修正文档，不重复实现。
- Verification: 检查 Go 装备统计与 `refreshPlayerStatsLocked` 调用链，确认觉醒值已生效；随后转向实际未迁移的元素配置边界。

### 2026-08-19 — 装备套装迁移先核对共享常量与 Legacy 完整门槛

- Symptom: 装备套装补丁首轮目标编译因 `statFreezing`/`statPoisonAttack` 在 Go 包内重复声明失败；修复后 Mir 套装定向测试又因夹具放入 12 件 Mir 装备而没有触发 Legacy 的十件完整奖励。
- Root cause: 添加装备字段前没有先做包级常量检索；测试夹具按“所有可用槽位”构造，忽略了 Legacy `MirSet.Count() == 10` 的精确条件。
- Prevention: 新增 stat 常量前先在目标 Go 包全局搜索同名定义；迁移带精确数量/槽位门槛的规则时，测试夹具必须逐槽映射 Legacy 条件，并单独断言完整与部分奖励。
- Verification: 移除重复声明、将 Torch/Amulet 槽置空保留十个 Mir 槽位后，普通套装/Mir 套装/钓鱼竿定向测试通过，目标包重新编译成功。

### 2026-08-19 — 掉落奖励必须沿 CreateDropItem 调用链迁移

- Symptom: Go 已加载 `RandomItemStats`，但普通怪物/任务掉落和钓鱼奖励仍直接使用 `CreateFreshItem` 形态，缺少 Legacy 的随机耐久、随机附加属性、诅咒、扩槽、鉴定和名称过期语义。
- Root cause: 只迁移了掉落树选择和地面物品路由，没有继续核对 Legacy `Envir.CreateDropItem` 在 Monster/Player/Fishing 调用点的后续对象初始化；智能生物奖励的随机逻辑也因此被孤立在专用路径。
- Prevention: 迁移掉落功能时先把“掉落选择 -> UserItem 创建 -> 随机升级 -> 过期/鉴定 -> 全局 ID -> 交付”列成完整调用链，并把共享随机升级器接入每个已确认的 CreateDropItem 等价入口；FreshItem、ShopItem 和已有持久物品复制必须保持独立。
- Verification: Go 固定向量覆盖耐久、全部附加属性、诅咒、扩槽、鉴定、`[2h]` 过期和全局 ID 分配/耗尽；普通怪物掉落、任务掉落、钓鱼及智能生物定向测试和 `cmd/crystal-server` 全包测试通过。

### 2026-08-19 — 特殊装备负面效果必须携带防御类型上下文

- Symptom: 将 `Paralize` 接入共享玩家伤害函数时，如果只依据原始护甲值判断，物理与 MAC 命中无法区分，魔法攻击可能错误触发 Paralysis。
- Root cause: Go 原有 `damagePlayerLocked` 只接收已计算的 armour，不保留 Legacy `DefenceType`；Legacy `ApplyNegativeEffects` 明确排除 `MAC`/`MACAgility`。
- Prevention: 在共享伤害边界增加显式 `magicDefence` 上下文，并在所有已迁移魔法入口传递 `true`；物理入口保持 `false`，专用未统一路径不宣称已覆盖。
- Verification: 新增物理/魔法玩家与怪物命中测试，断言魔法路径不消耗 1/14 抽取；`go test ./... -count=1 -timeout=5m` 全部通过。

### 2026-08-19 — ChatItem 的定义展开深度必须按 CheckItem 调用点核对

- Symptom: 初版聊天物品实现沿用了 Go 其他物品可见性路径的递归 socket 展开；重新读取 Legacy `MirConnection.CheckItem` 后发现它只检查主物品和一层 `Slots`。
- Root cause: 把另一个功能路径的递归 helper 当成了聊天链路的协议契约，没有逐行核对 `CheckItem` 的循环深度与发包顺序。
- Prevention: 每个入口分别记录定义展开的深度、顺序和 per-connection cache 语义；相邻功能即使共用 `StoredItem`，也不能直接复用不同可观察路径的递归策略。
- Verification: 聊天路径已改为主物品加直接 socket；测试夹具加入二层 socket 并确认只发送前一层 ItemInfo，协议/服务器定向测试通过。

### 2026-08-20 — Tree 的 HumanObject 命中不能误删专属武器/元素钩子

- Symptom: Tree 批次文档初稿把“无通用副作用”写成了“无武器/元素副作用”，与 Legacy `Tree.Attacked(HumanObject)` 的 `DamageWeapon` 和 `GatherElement` 分支不一致。
- Root cause: 将 MonsterObject 与 HumanObject 两个 overload 的共同 ACAgility/固定 1 HP 结果，错误概括成相同的后置副作用；Legacy 只省略通用 AttackBonus/暴击/毒伤，HumanObject 命中仍保留武器与元素钩子。
- Prevention: 对每个 Legacy overload 分开列出 admission、命中副作用和死亡收尾；文档使用“无通用 DamageIndicator/bonus/critical/poison，保留 HumanObject weapon/element hooks”的精确表述。
- Verification: Go `treeAttackedByPlayerLocked` 在命中门禁之后调用 `damagePlayerWeaponLocked` 与 `gatherElementLocked`；迁移矩阵和交接文档已同步修正，Tree 与全包测试通过。

### 2026-08-20 — 只读定位正则也要先做最小语法校验

- Symptom: 读取 Go 目标判定 helper 时，组合多个函数名的 `rg` 正则括号未闭合，命令直接失败。
- Root cause: 为一次查询覆盖多个函数，临时拼接了嵌套分组，却没有先用最小模式验证正则语法。
- Prevention: 组合定位条件优先使用多个简单 `rg` 查询或已验证的非捕获/捕获分组；复杂模式先在单仓库、只读命令中做最小语法检查，再依赖输出判断。
- Verification: 本次失败输出未用于源码判断；随后将改用逐个函数名的简单查询，且该 lesson 已在继续修改前追加。

### 2026-08-20 — 只读审计的 JavaScript 编排字符串必须先闭合

- Symptom: 一次仅查询 Go 代码的 `functions.exec` 编排因 JavaScript 字符串引号未闭合而在执行前报 `SyntaxError`，没有产生 shell 输出。
- Root cause: 为拼接多段只读查询时直接嵌入引号，未先让脚本解析器通过最小语法检查。
- Prevention: 复杂只读编排先拆成单条 `exec_command`，或先用固定字符串/模板字面量完成语法校验；脚本解析失败时不得把它当作仓库证据。
- Verification: 该调用没有启动命令或修改文件；随后使用单仓库、单命令查询重新取得目标 helper 输出。

### 2026-08-20 — Legacy handoff 多行补丁必须先用合法 JavaScript 字面量编排

- Symptom: AI=22 handoff 的首次 `functions.exec` 调用把实际换行直接放入双引号字符串，执行前报 `SyntaxError: Invalid or unexpected token`，没有修改文件。
- Root cause: 多行 `apply_patch` 参数没有使用模板字面量或显式换行转义，脚本解析阶段先于工具调用失败。
- Prevention: 通过 `functions.exec` 调用 `apply_patch` 时，多行 patch 一律使用模板字面量并转义其中的模板定界符；提交前检查脚本自身可解析，失败调用不得作为文件状态证据。
- Verification: 首次调用无 shell/apply_patch 执行且工作树未变化；随后将使用合法模板字面量重新应用完整 handoff patch。

### 2026-08-20 — handoff 补丁数组中的每一行必须是合法 JavaScript 字符串

- Symptom: AI=25 handoff 的首次 `functions.exec` 编排因一行 `@@` 未包在字符串中，在工具执行前报 `SyntaxError`，handoff 未变化。
- Root cause: 多行 patch 使用逐行数组时漏写了字符串定界符，脚本解析先于 `apply_patch` 失败。
- Prevention: 逐行 patch 数组中每个元素都必须是完整字符串；调用前检查数组项是否都带引号，失败编排不作为文件状态证据。
- Verification: 首次调用没有进入 `apply_patch` 且 Legacy 工作树未新增 handoff 变化；随后将用合法逐行数组重试并检查 diff。
- Strengthening after same-session recurrence: handoff 的第二次数组补丁又遗漏了 patch 行前缀，`apply_patch` 在校验阶段拒绝整组补丁，文件仍未变化。
- Strengthened prevention: 数组项的 JavaScript 字符串定界符与 patch 的 ` ` / `+` / `-` 前缀分别检查；优先拆成单个小 hunk，逐次确认成功。
- Verification after strengthening: 随后的 Legacy handoff 表格、Go HEAD、门禁和 AI=25 段落均分别成功应用，随后检查 diff。
- Strengthening after same-session recurrence: AI=26 handoff 的首次编排在 JavaScript join 转义处未执行，第二次编排又遗漏了 update hunk 行首的补丁前缀，apply_patch 拒绝整组补丁。
- Strengthened prevention: 逐行数组补丁的 join 语法先做最小可解析检查；每个 update-hunk 行逐项确认带有空格、加号或减号前缀，并拆分为插入、替换、追加三个小 hunk。
- Verification after strengthening: 两次失败调用均未修改 handoff；随后三个小 hunk 均成功应用，AI=26 段落、Go HEAD 和门禁记录均已核对。
- Strengthening after same-session recurrence: AI=30 handoff 的两次编排先后在 `apply_patch` 校验阶段发现替换/上下文行缺少 `-`/`+`/空格前缀，整组补丁均未写入。
- Strengthened prevention: 生成逐行 patch 数组时，替换行必须成对带 `-`/`+`，上下文行必须带一个空格，空白上下文也必须保留为单独的空格行；先读取带行号的精确区块，再拆分替换和插入 hunk。
- Verification after strengthening: 第三次使用合法 hunk 成功更新 AI=30 handoff 表格和段落，随后重新读取行号区块确认内容。
- Strengthening after same-session recurrence: AI=35 矩阵补丁首次把段落上下文行漏写 patch 前缀，第二次修正时又把上下文内容误写成实际制表符，`apply_patch` 均拒绝或产生不期望的排版风险。
- Strengthened prevention: 文档 patch 数组中，patch 标记空格与上下文正文必须分开；上下文行使用“一个前缀空格 + 原文”，新增正文不携带猜测的制表符；长矩阵更新拆成单一插入/替换 hunk，并立即用 `sed` 复核。
- Verification after strengthening: 随后分别成功插入 AI=35 概述、替换基础说明和新增表格行，复核显示无意制表符且 `git diff --check` 通过。

### 2026-08-20 — 只读定位正则复发时必须立即改用固定模式

- Symptom: AI=41/42 测试 helper 查询临时组合正则括号未闭合，`rg` 在读取阶段失败；该调用没有源码证据。
- Root cause: 为覆盖多个 helper 手工拼接复杂分组，没有先执行最小正则语法检查。
- Prevention: 只读定位优先使用多个 `rg -F` 固定模式；需要组合时先单独验证正则，任何非零读取输出整体作废，不从空/部分输出推断代码。
- Verification: 失败调用未写入；随后用独立固定模式成功定位 helper，并在本批定向测试前完成实现核对。

### 2026-08-20 — ThunderElement 移动分支必须保留 ObjectWalk

- Symptom: ThunderElement 的移动已经改变了怪物坐标，但初版 admission 通知没有包含 `ObjectWalk`，客户端观察到位置变化却没有移动包。
- Root cause: 复用移动 helper 时丢弃了 `monsterAIMoveToLocked` 返回的通知 slice，只保留了后续攻击动作。
- Prevention: 任何 AI 的移动与攻击同 tick 分支都必须把移动 helper 的通知按 Legacy 顺序追加到返回值；测试同时断言坐标、`ObjectWalk` 和后续攻击。
- Verification: `TestGameWorldThunderElementAdmissionMovesThenAttacksAndRescans` 固定移动包后，AI=49 定向普通测试及 `-race -count=3` 通过。

### 2026-08-24 — NET-P1-GATES-001 integration findings

- Symptom: 首版 gate candidate 遗漏账号/角色创建的 24 小时 IP block、普通 shutdown reason 0、MaxUser 满载时 fatal wake、500 ms receive-disconnect grace、同一 receive 中 valid+malformed 的原子丢弃、成功连接日志和 IPv6 `Split(':')[0]`；日志测试还用非同步 `bytes.Buffer` 触发 race。
- Root cause: 只迁移 listener 与单帧 reader 的明显限制，把 `ConnectionLogs`、StopNetwork、异步 ReceiveData 批次边界和 Disconnecting 延迟误当成后续 lifecycle/logging 范围；测试按帧而不是按一次 8 KiB receive 建模。
- Prevention: 网络 gate leaf 必须枚举所有 IPBlocks 写点和 direct consumers；把 accept re-arm、receive callback、packet queue、disconnect grace、shutdown reason 与日志顺序作为一个有限状态机；共享日志 sink 使用同步 writer。
- Verification: 主审逐项裁决并实现 account/character 阈值怪癖、fatal wake、reason 0、原子 receive parser、绝对 idle deadline、500 ms grace、Legacy enum history/IP authority和同步日志测试；focused `-count=20`、focused race `-count=5`、fresh unexcluded full tests/full race/vet/build 均通过。

### 2026-08-24 — NET-P1-STATUS-001 timer and entry findings

- Symptom: 首版 deterministic test 把 `NextSendTime == 0` 特判为 elapsed time 等于零也立即发送，并只从注入 listener 的内部 helper 证明 status 接线。
- Root cause: 把“正常运行中的第一个 process tick”简化成无条件 first-send，没有保留 Legacy `Time > NextSendTime` 的初始严格边界；同时把可测试 service 当成 production runtime 本身。
- Prevention: status timer 必须分别测试 elapsed zero、首个正毫秒、十秒 equality 和下一毫秒；生产入口必须在同一 runtime 函数中注入最小 listener opener，断言 game listener 之后确实请求 `Settings.IPAddress:3000`。
- Verification: service 已移除 zero-time 特判并锁定 strict positive first tick；runtime-entry test 记录真实 opener 调用序列、固定地址与双 listener 关闭，独立 TCP 测试覆盖五连接 backpressure/release。

### 2026-08-28 — NPC GOTO access tests must preserve the literal page-key token

- Symptom: the first selected-button regression used `GOTO HIDDEN` but asserted
  the parser would expose `[@HIDDEN]`; the focused worlddata test failed because
  the actual grammar preserves the token as `[HIDDEN]`.
- Root cause: the test normalized a control-flow argument by intent instead of
  using the Legacy parser's literal `"[" + parts[1] + "]"` rule.
- Prevention: derive NPC page keys from the exact script token; use
  `GOTO @HIDDEN` when the target header is `[@HIDDEN]`, and keep flow targets
  separate from client-authorized SAY buttons.
- Verification: the fixture now uses `GOTO @HIDDEN`; the exact two selected-
  button tests pass and prove the page grammar retains the flow target while
  the active access set excludes it.

### 2026-08-28 — NPC-P7-PAGE-GRAMMAR signature closure compile recurrence

- Symptom: two successive Page Grammar patches changed callers before changing
  `parseLegacyNPCSegment` and `npcSelectedButtonAllows`; each immediate minimum
  compile failed with a too-many-arguments error.
- Root cause: the main agent knowingly split a signature closure across patch
  transactions instead of enumerating and changing declaration plus consumers
  together.
- Prevention: before any signature patch, enumerate every consumer and include
  the declaration and all callers in one patch/compile transaction; never use
  an expected compiler failure as a patch sequencing device.
- Verification: both declarations were brought into sync, `gofmt` ran, and the
  exact package compile gates subsequently exited 0; active lesson C06 was
  strengthened rather than duplicated.

### 2026-08-28 — Turkish Page Grammar fixture must uppercase the root literally

- Symptom: the first culture test queried `[@main]` and expected the Legacy
  `[@MAIN]` page; under `tr-TR`, `i` uppercases to `İ`, so lookup correctly
  failed.
- Root cause: the test asserted invariant intent instead of applying the same
  CurrentCulture mapping as the production lookup.
- Prevention: derive culture-sensitive keys rune by rune; use literal
  `[@MAIN]` to reach the fixed root and reserve dotless/dotted-I variants for
  the target mismatch being tested.
- Verification: the corrected root query plus Turkish dotless target and dotted
  mismatch corpus passes the exact Page Grammar test command.

### 2026-08-28 — Automatic-script root names must not accidentally invoke markers

- Symptom: the first 00Monster/00Robot root-wiring corpus used `[@_SPAWN]` and
  `[@_TIME]`, overlooking that those words invoke Legacy default-script marker
  consumers before page registration.
- Root cause: the fixture tested a generic `[@_]` root with names that also had
  type-specific semantics in `ParseDefault`.
- Prevention: use marker-neutral names for pure root-grammar tests; reserve
  MAPCOORD/CUSTOMCOMMAND/SPAWN/DIE/TIME cases for their owning action/callback
  leaf with required environment data and side-effect assertions.
- Verification: the corpus now uses `[@_MOBHOOK]`/`[@_ROBOTHOOK]`, its repeated
  production-root test passes, and marker behavior remains explicitly deferred.

### 2026-08-28 — NPC-P7-PAGE-GRAMMAR terminal review and token-boundary repair

- Symptom: terminal review found that joining substituted `%ARG` words and
  reparsing them could split one argument containing spaces; action commands
  still used invariant casing; tab-separated `GOTO` could execute despite not
  being discovered. The first repair then broke quoted CHECKITEM fixtures.
- Root cause: the candidate modeled Legacy's in-place token mutation as a
  string round trip and reused a whitespace-wide/quote-aware splitter without
  separating literal-space token boundaries from quote handling.
- Prevention: carry `%ARG` replacements as a word slice into condition/action
  parsers; split only on literal spaces while preserving quoted values; inject
  CurrentCulture casing for command and page-key normalization; assert both
  discovery and execution absence for tab-separated commands.
- Verification: spaced ARG condition/action, Turkish dotted/dotless action,
  tab GOTO, quoted CHECKITEM, full worlddata and compile tests pass. The review's
  proposed nil-Args panic was rejected because `NPCPage.Args` is initialized at
  declaration (`NPCPage.cs:9`), so Go `len(nil)==0` matches `Count==0`.
- Routing: duplicate automatic roots are constructed exactly here, but Legacy's
  execute-every-matching-root behavior remains owned by
  `NPC-P7-DEFAULT-CALLBACK-001`/`NPC-P7-MONSTER-ROBOT-SCRIPT-001`; this leaf must
  record that dependency rather than widening into callback execution.

### 2026-08-28 — NPC control-flow calls have per-call input lifetime and page-shared BREAK carry

- Symptom: an immediate `GOTO` reused `%INPUTSTR` from its producer and awarded
  gold, while `BREAK` in a page's final segment failed to stop a queued
  self-`GOTO`; the second call repeated the page instead of returning empty.
- Root cause: the Go runner treated an initial page plus queued pages as one
  input transaction and represented `BreakFromSegments` as a call-local bool.
  Legacy removes `NPCInputStr` at the end of every `NPCScript.Call`, but stores
  `BreakFromSegments` on the shared `NPCPage`; a final-segment BREAK is consumed
  only by the next call's first segment iteration.
- Prevention: consume input after each detached page call before scanning its
  immediate queue, and attach race-safe BREAK carry to parsed page values so
  script/page copies share it. Keep a local fallback only for hand-built unit
  pages, and test a BREAK+self-GOTO rather than only mid-page BREAK.
- Verification: the two production-entry regressions first failed with
  `ServerGainedGold` before the chained response and repeated `First pass.`;
  after the repair the exact pair, all control-flow/Speech-Input tests and the
  full `internal/worlddata` package pass.
- Review strengthening: read-only terminal review found Roll's page parameter
  was resolved after `SetRollResult`, unlike Legacy's all-parameters-before-
  switch loop. Control parameters are now resolved as one snapshot before any
  RNG/state mutation; the focused test proves consecutive roll pages observe
  the prior results `0` then `1`, while emitted results are `1` then `6`.
- Further closure: parameter resolution now clones immutable script actions and
  preserves Legacy's `FindVariable` → per-space-token `ReplaceValue` → literal
  `%INPUTSTR` order, so client input cannot recursively invoke a formatter.
  Parsed page arguments are always wrapped, including Legacy's double-wrap for
  an already bracketed token. A queued `GOTO` to `[@@...]` now requests input
  before page lookup/execution and uses the player's prior active NPC ID.
- Concurrency review: terminal review initially reported that another session
  could steal the page-shared BREAK carry. Full production tracing showed every
  direct ordinary/default call, immediate scan and due scan executes inside one
  global flow mutex, matching Legacy's single work-loop interval; the reviewer
  explicitly withdrew the finding. Shared parser state is safe only while that
  serialization remains a protected invariant, so future entry points must be
  added under the same wrapper and traced before acceptance.

### 2026-08-28 — Control-flow full gate exposed an unrelated Hallucination timeout

- Symptom: the first full server package and first full-repository test each
  failed only at `TestSessionHallucinationTranscript` after 30 seconds with a
  closed pipe; no stack entered the NPC control-flow candidate.
- Root cause: the archived Hallucination session fixture is scheduling-sensitive
  under a broad run, not a demonstrated control-flow regression.
- Prevention: preserve the full-gate exits, isolate the exact test repeatedly,
  and require a fresh unexcluded full pass; do not change unrelated production
  code or describe an excluded run as success.
- Verification: exact isolated `-count=1` and `-count=10` both passed, followed
  by fresh full server-package passes in 81.555 and 86.007 seconds. The final
  fresh unexcluded `go test -count=1 ./...` passed with server 85.013 seconds,
  and full `go test -race -count=1 ./...` passed with server 97.385 seconds;
  `go vet ./...` and `go build ./...` also exited 0.
- Reward Rates recurrence: the first fresh full test again failed only at the
  same line 123 closed pipe. Its first isolated count-10 also flaked, while an
  immediate exact count-1 and second count-10 passed; the next fresh unexcluded
  full test passed with server 82.811 seconds. No Hallucination scope changed.

### 2026-08-28 — NPC Action State parser/runtime returns and source-proven unsafe quirks

- Symptom: the recovered candidate treated a missing-arity `ParseAct` return as
  aborting the remaining ACT list, attached segment state to every parsed
  action, mutated stale session equipment, used host time in the ordinary
  runner, resolved state parameters twice and emitted generic logs without the
  current queued page. The first full gate exposed the over-broad unexported
  state through exact action equality tests.
- Root cause: parser-line return and runtime `Act` return were conflated;
  `NPCSegment.Param1/2/3` ownership was widened beyond PARAM plus its direct
  MONGEN consumer; new state behavior bypassed the existing item revision,
  world-clock and localized MessageQueue authorities.
- Prevention: trace all fourteen branches and their direct helpers before
  implementation. Skip only the malformed parser line, but stop the runtime
  action tail on failed conversions; retain one resolved parameter snapshot;
  share PARAM state only with its direct consumer; rebase UNEQUIPITEM through
  `CommitCharacterItemMutation`; pass the current detached call page and world
  clock into every ordinary/default action entry.
- Source adjudication: safety review proposals to sanitize value/random paths,
  suppress divide/empty-file/replacement/INI panics, repair `InIReader`'s
  adjacent-section `b-1` insertion or persist timers were rejected. Exact
  Legacy source proves these are reachable quirks; only directory creation is
  outside the reader's swallowed-I/O blocks. Regressions preserve each ruling.
- Workflow correction: one Legacy `Reporting.cs` read appended a Go path and a
  later read guessed the wrong `PlayerObject.cs` directory. Both calls were
  discarded wholesale; repository-separated reads and a prior `find` rerun
  supplied the accepted evidence under the existing C01/C02 rules.
- Verification: parser/protocol/runtime/ordinary/default tests cover all 14
  keys, files across restart, RNG order, timer due/relogin/restart loss,
  experience reset, UI wire order and latest-authority equipment restrictions.
  Focused count-10 and race count-3, fresh full test/vet/build and fresh full
  race passed; terminal Luna revision 3 returned `No findings`. Go commit:
  `83a37867942edc42d780edacb1083db4bfebd13c`.

### 2026-08-28 — Local-condition session fixture must satisfy guild bootstrap

- Symptom: the first production-entry red test stopped before NPC evaluation
  with `GuildMutationLevelTooLow`; after fixing the level, an unconsumed
  `ServerAddBuff` bootstrap packet was mistaken for `ServerNPCResponse`.
- Root cause: the fixture assumed AdminAccount bypassed the guild level gate and
  that the generic mail-session bootstrap consumed the guild newbie Buff packet.
- Prevention: seed the real required level before `CreateGuild`, then enumerate
  and consume every guild-specific post-bootstrap packet before issuing the NPC
  request. A condition test is not valid until its first failure is the expected
  success/ELSE branch rather than setup or packet-order noise.
- Verification: after both fixture repairs, the same exact test reached the
  intended unmigrated condition evaluator and failed only with
  `Local conditions failed.` instead of the expected success response. Final
  green/repeated/race evidence belongs to the leaf closure record.
- Follow-up symptom: the retained-character-count adapter test tried to create
  a fifth active character and failed with result `4` before reaching its
  tombstone assertion.
- Follow-up root cause: the fixture confused raw retained-record capacity with
  the four-active-character creation gate.
- Follow-up prevention: create four active records, tombstone one through the
  production delete authority, and only then create the fifth retained record;
  separately assert the four-row selection projection and five-row raw count.
- Follow-up verification: the repaired auth/context-focused command exited 0
  for both `internal/auth` and `cmd/crystal-server`.

### 2026-08-28 — Local Conditions require parser, culture and authority parity together

- Symptom: the recovered candidate passed its first focused tests but ordinal
  string ordering, callback-delegated negative RANDOM, generic/quote-aware
  condition parsing, projected character count, swallowed name-list read errors
  and a trailing-newline phantom entry still differed from Legacy.
- Root cause: shared helpers were generalized from existing Go behavior instead
  of following each `NPCSegment.ParseCheck` and `Check(PlayerObject)` callsite;
  selection projection was mistaken for raw `AccountInfo.Characters.Count`.
- Prevention: freeze parser tokenization/minima separately from evaluation;
  distinguish caught and uncaught comparison operators; inject CurrentCulture
  collation while keeping ordinal equality; reject negative RANDOM before RNG;
  preserve BOM-aware `File.ReadAllLines`, raw retained account count and exact
  timer/flag quirks through ordinary/default production contexts.
- Source correction: only quoted name-list paths receive the parser's regex
  special case. Quoted CHECKITEM/CHECKCALC text remains literal space-split
  tokens; prior handmade multiword CHECKITEM fixtures were not Legacy evidence.
- Verification: auth/worlddata/server focused count-10 and race count-3 passed;
  fresh unexcluded `go test -count=1 ./...`, `go vet ./...`, `go build ./...`
  and `go test -race -count=1 ./...` exited 0. Luna reviews found five initial
  mismatches, then the newline edge, and revision 3 returned `No findings`.
  Go commit: `424978eabe5de76d84979f3fe108bf7968173aa0`.

### 2026-08-28 — Shop scope must distinguish a local snapshot from source mutation

- Symptom: frozen ledger N and the routed Active Leaf claimed repeated BUY
  mutated shared/static `Goods` counts, but exact Legacy code constructs
  `new List<UserItem>(Goods)` and appends `UsedGoods` only to that local list.
- Root cause: discovery prose treated shallow member aliasing as list-membership
  mutation and called an instance field static without tracing `GetOrAdd`.
- Prevention: for collection quirks, separately record container identity,
  member aliasing, source counts and persistence; reread the constructor and
  exact mutation target before freezing an acceptance denominator.
- Verification: main reread `NPCScript.cs:1-112,919-1045` and corrected matrix,
  active index and handoff before any Shop Quirks code write. The finite contract
  now requires stable `G/U` source counts, per-open `G+U` enabled snapshots,
  BUYNEW=`G`, and disabled BUYBACK/BUYUSED panel absence.

### 2026-08-28 — Read-only shop snapshots must not erase empty BuyBack owners

- Symptom: the enabled two-user session expected two empty owner keys after
  each user's first BUYBACK open, but only the second owner's key remained.
- Root cause: Go reused expiry processing on every shop snapshot, and that
  helper deleted entries that were already empty before the pass; Legacy
  `ProcessSpecial` creates the key and does not run `ProcessGoods` on open.
- Prevention: page reads must not run expiry processing at all. Preserve empty
  keys and nominally expired items until the strict scheduled NPC pass, then
  remove every empty owner exactly as `ProcessGoods` does.
- Verification: enabled first/repeated/two-user opens preserve both empty keys;
  an explicit due-time pass removes them. Nonempty items remain buy-back eligible
  after nominal expiry until that pass, then drain in stable owner order.

### 2026-08-28 — Mutable UsedGoods needs an explicit empty restart override

- Symptom: a read-only review found that buying the last runtime UsedGoods item
  updated only `world.npcs`; restart reloaded the static export and resurrected
  that item, unlike Legacy `NeedSave`/`SaveGoods`.
- Root cause: the mutable world sidecar persisted only respawn state, and an
  omitted/empty UsedGoods list was not represented as authoritative state.
- Prevention: persist a deterministic per-NPC UsedGoods snapshot in the atomic
  runtime companion; retain an entry with an empty list so it overrides static
  import, clone item state under the world lock, and mark every add/remove/
  restore path dirty for retry-safe persistence.
- Follow-up findings: terminal review found that the last-player ticker stop
  could strand a dirty removal, empty owners lacked their scheduled cleanup,
  owner-map iteration randomized UsedGoods append order, and read helpers moved
  expired BuyBack items before Legacy's scheduled pass. All four are now closed:
  stop-to-empty persists, scheduled processing owns every drain, normalized
  owners are sorted, and reads/mutations never process expiry.
- Forced-save ruling: `SaveGoods(true)` clears every remaining BuyBack list at
  graceful shutdown; owner association is transient, but eligible items become
  public persisted UsedGoods. Go now forces that conversion before its atomic
  runtime snapshot rather than silently dropping the items.
- Verification: authenticated BUYUSED removal, forced BuyBack conversion,
  explicit-empty overlay, owner-key loss and relogin G+U all pass. Early fixture
  failures came from nil-versus-empty expectation, a missing registered map and
  forgetting the fixture's initial UsedGoods; each was repaired at the fixture
  boundary without weakening behavior. Focused count 10/race count 3 plus fresh
  full test, race, vet and build exit 0.

### 2026-08-29 — NPC MOVE's `200` argument is not a 200-draw algorithm

- Symptom: the frozen Teleport Actions row and active index described
  coordinate-less `MOVE` as a bounded 200-draw search.
- Root cause: discovery inferred behavior from the call
  `TeleportRandom(200, 0, targetmap)` without tracing the current virtual helper.
  Legacy `MapObject.TeleportRandom` ignores both arguments and performs exactly
  one `Random.Next(WalkableCells.Count)` selection.
- Prevention: RNG acceptance must enumerate the callee body, not arguments at
  the callsite. Preserve the one draw, empty/cell-null failure and ignored
  return value; do not migrate the separate 200-attempt `GetRandomPoint` helper.
- Verification: exact Legacy ranges `NPCSegment.cs:3075-3088` and
  `MapObject.cs:803-813` were reread before any Teleport code write; matrix row
  194, ledger P, active index and current handoff now state the source-proven
  one-draw contract.
- Implementation recurrence: the first adapter reused `combatRollLocked`, whose
  intentional `bound <= 1` fast path silently skipped `Random.Next(1)`. The
  teleport adapter now owns a unit-bound-consuming helper; a one-cell loaded map
  records exactly `[1]`, while nil `Cells` and empty walkables record no draw.
- Fixture findings: an empty-recall bypass test targeted an unreachable page,
  which the accepted reachable-page parser correctly omitted; the repaired graph
  reaches the secret page through an intermediate button while leaving it
  unauthorized from the current page. A separate authenticated fixture also
  exceeded the real character-name limit, and repeated delayed-flow runs showed
  KeepAlive may occur on either side of the due NPC response. Fixtures now obey
  the real name constraint and locate the exact barrier marker in the already
  collected prefix before waiting for another frame. Focused count-50 and race
  count-3, then fresh full test/race/vet/build, all exit 0.
- Terminal review adjudication: the claimed extra self Revelation health packet
  was withdrawn because `BroadcastHealthChange` uses `Map.Broadcast`, not
  `MapObject.Broadcast`, while Revelation is active; `Map.Broadcast` includes
  the moving player exactly as the Go transcript expects. The stale current-
  session `gameCharacter` location finding was valid and direct NPC delivery now
  updates its map/coordinates/direction alongside the active-map variables.
- Cross-session finding and failed first repair: a remote group member's
  `NPCTeleportTransition` callback refreshed a riding mount and wrote that
  target session's HP/character locals from the leader's NPC-call goroutine.
  Moving the entire callback onto the target loop with synchronous completion
  then deadlocked: the leader held the global serialized NPC-flow mutex while
  waiting, and the member loop was blocked acquiring that same mutex. The exact
  group test timed out and a 3-second stack identified both wait sites.
- Prevention and verification: cross-session delivery now refreshes only the
  target's locked world mount authority and packets, then queues the existing
  map transition; target session locals remain updated by its own pending-
  transition loop. The authenticated group fixture mounts the remote member and
  locks MountUpdate between self teleport and passive snapshot. Exact ordinary
  and race count-20 pass; full leaf gates must be rerun after this review fix.
- Review revision 2 found the remote callback still read mutable
  `worldObjectID` across teardown. Callback installation now captures its
  immutable recipient object ID and uses only that ID for snapshot, packets,
  mount, rental/trade cancellation and passive delivery. Authenticated
  `INSTANCEMOVE` second-instance and cross-map two-member `GROUPRECALL` entries
  close the remaining production-entry risk. Revision 3 reports `No findings`.
- Final post-review evidence: owned gofmt/compile, focused count-50/race count-3,
  mounted group and instance/recall ordinary/race count-20, relevant Flow/rental
  regressions, fresh `go test -count=1 ./...`, `go vet ./...`, `go build ./...`
  and `go test -race -count=1 ./...` all exit 0.

### 2026-08-29 — Quest reward level-up fixtures require a post-level threshold

- Symptom: the first reward session test observed a correct level and remainder
  but serialized `MaxExperience=0` instead of the hand-written expected 100.
- Root cause: the two-entry synthetic experience table omitted the guard slot
  required by `progression.MaxExperience` after reaching level two.
- Prevention: derive every level-up fixture table from the production level
  lookup and include the reached level's next threshold; do not infer slice
  length from the number of transitions.
- Verification: the fixture now uses `{5,100,100}` and locks
  `LevelChanged(2,2,100)`; focused ordinary count-10 and race count-3 pass.
- Main review finding: the first config patch parsed ExpRate but bypassed
  `InIReader.ReadSingle`'s missing/invalid fallback and Setup.ini write-back;
  reward items also used a character-local maximum while the existing auth
  allocator explicitly owns Legacy reward IDs and intentional pre-admission
  gaps. The final patch uses a Single-aware legacy reader for both rates and
  `authService.AllocateItemID` before capacity admission; focused tests lock
  rewritten defaults, distinct IDs, restart continuity and the next global ID.

### 2026-08-29 — Lifecycle producer assertions need their new import in the same patch

- Symptom: the first quest-lifecycle focused run failed at compile time because
  a new `reflect.DeepEqual` assertion in `world_test.go` omitted `reflect`.
- Root cause: the packet-order hunk and import closure were reviewed separately.
- Prevention: every new test symbol must land with its import in one patch, then
  run the package compile-only gate before the behavioral test.
- Verification: the import was added, compile-only passed, and the exact default
  NPC lifecycle transcript then passed.

### 2026-08-29 — Lifecycle full-package gate hit an unrelated Hiding closed-pipe flake

- Symptom: the first server-package gate failed only
  `TestSessionHidingTranscriptPersistenceAndExpiry` at its 30-second closed-pipe
  assertion; the stack did not enter the quest lifecycle files.
- Root cause: package-wide scheduling exposed the existing net.Pipe fixture's
  timing-sensitive shutdown path, not a quest timer packet or state mismatch.
- Prevention: attribute by the first failing test/stack, rerun the exact case at
  count 1 and 10, then require a fresh unexcluded package gate.
- Verification: exact count 1 and count 10 both exit 0; the fresh server-package
  rerun then passed in 82.897s.
- Full-gate recurrence: the next `go test -count=1 ./...` failed only
  `TestNPCP7TeleportActionsSessionTimedRecallCapturedOriginAndInclusiveDue`
  because its flagged queue was still length one after the due scan. Exact
  count 1 and count 10 both passed; no quest state or timer appeared in the
  stack.
- Closure reruns: two later fresh full attempts hit only the established
  Hallucination closed-pipe path and
  `TestSessionMapHazardSpawnDamageRemovalAndRestart` observing HP 9 rather than
  8 after its restart bootstrap. The latter test stops the ticker only after
  bootstrap, so package-load scheduling can admit one real regen tick; exact
  Hallucination count 1 and map-hazard count 20 passed. The final fresh
  unexcluded full run then passed in 82.937s.

### 2026-08-29 — Preparation completion markers must be one-bootstrap state

- Symptom: a quest completed by login preparation would retain its
  `questBootstrapCompletions` marker after the first StartGame transcript, so a
  same-connection logout and StartGame could replay the completion-only
  `ExpireTimer` even though the quest was already complete.
- Root cause: the session map was reset on SelectCharacter but read without
  consumption during StartGame; logout-to-character-select does not require a
  second SelectCharacter before restarting the same character.
- Prevention: treat preparation-discovered completion as an edge-triggered
  bootstrap event and consume its marker when building that quest's first Add
  transcript; never retain transition markers as level state.
- Verification: `TestQuestP7LifecycleTimerBootstrapCompletionIsConsumedOnce`
  and the authenticated same-connection logout/StartGame transcript lock first-
  bootstrap Expire/Set/Change and subsequent Set/Change only; focused count-20,
  touched-package compile and final integration gates exit 0.

### 2026-08-29 — A source write invalidates every overlapping test result

- Symptom: the main thread added the production relogin transcript while an
  earlier focused ordinary/race command was still running, so both apparent
  passes had an unstable source snapshot and were discarded.
- Root cause: test execution and the next review-driven patch were treated as
  independent even though the Go tool can read files throughout compilation.
- Prevention: once any gate starts, freeze all files in its package until the
  process exits; after an overlap, never retain partial output and rerun the
  complete command from a stable status.
- Verification: after the relogin test and marker fix were finalized, the exact
  focused count-10 plus race count-3 command was rerun without writes and both
  invocations exited 0.
