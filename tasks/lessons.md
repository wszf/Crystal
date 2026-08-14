# Lessons learned

Record project-specific corrections and failure-prevention patterns here.

### 2026-08-14 — LightSetting 测试必须按旧版的 hour*2 区间逐值核对

- Symptom: 动态 `TimeOfDay` 定向测试把本地小时 9 期望为 Evening，Go 测试失败；旧版实际返回 Night。
- Root cause: 看到 Evening 的 `16/17` 区间后按直觉把连续本地小时映射成 8/9，遗漏了 Legacy 先执行 `Now.Hour * 2 % 24`。
- Prevention: 时间迁移先固定中间量 `hours = hour*2%24`，再列出每个边界小时的枚举和 wire 值；定义类型负例也显式转换为协议底层 `byte`，避免测试类型与业务枚举混淆。
- Verification: 修正 hour 9 为 Night、负例显式 `byte(LightNight)` 后，协议、探针和服务端 TimeOfDay 定向测试通过。

### 2026-08-14 — 全局时钟副作用必须区分在线 runtime 与纯 world fixture

- Symptom: 全仓测试中大量使用合成 epoch 时间调用 `world.tick` 的战斗、掉落和治疗 transcript 收到意外 `TimeOfDay` 通知并失败。
- Root cause: 新增的全局时间更新在所有 world fixture 上无条件执行；旧测试的 `tick` 不是在线服务 runtime，却被当成真实服务器时钟循环。
- Prevention: 需要连接级后台 ticker 才启用全局时钟副作用；`startTicker` 负责启用并用当前/注入时钟刷新状态，纯 world fixture 默认保持关闭，定向时间测试显式开启。
- Verification: 已把更新门禁移到 `lightsEnabled`，在线 session 通过 `setLightClock` 启用；先前受污染的现有测试及动态 TimeOfDay 定向测试随后重新运行验证。

### 2026-08-14 — 跨仓库只读检索必须显式固定工作目录

- Symptom: 两次只读 `rg` 检索及一次补丁命令把原 Crystal 路径和 Crystal.GoServer 路径混用/重复拼接，输出或命令来自错误路径，增加了判断迁移状态的风险。
- Root cause: 并行检索与补丁调用中没有复核绝对工作目录，且把仓库路径再次拼进了已绝对化的文件路径。
- Prevention: 跨仓库查询一律使用绝对 `workdir`，同一批次先分别打印目标仓库的 `git status`，补丁目标只使用已核验的绝对路径；禁止凭相对路径或重复拼接推断结果属于哪个仓库。
- Verification: 本批次已分别在两个绝对工作目录执行状态检查，确认原仓库仅追加本 lessons 记录，Go 仓库仅包含 Observer 未提交改动；错误路径命令均在创建/修改前失败且未改动文件。
- Strengthening after recurrence: 即使同一批次已经固定了两个仓库的 `workdir`，补丁仍可能因手工复制绝对路径时漏掉中间目录而指向不存在的仓库；跨仓库修改前必须对每个补丁目标执行 `test -f`/`git rev-parse --show-toplevel`，失败时先停止补丁，不得依赖 apply 工具的部分输出判断是否已修改。
- Verification after strengthening: 本批次先分别读取 Legacy lessons 与 Go 状态，再用完整 `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer` 根路径核验；一次错误的缺少 `me_work` 目标在写入前被拒绝，随后所有 Go 补丁均在核验后的绝对路径成功应用，两个仓库的 C# 差异/未跟踪检查保持为空。

### 2026-08-14 — UserMagic 冷却必须在世界快照中转换为剩余时间

- Symptom: Go 运行时已经设置了每个技能的 `CastReadyAt`，但注销快照仍复制旧的 `StoredMagic.CastTime`；跨注销重登会丢失活动冷却，技能升级经验也没有统一的完整魔法 slice 提交入口。
- Root cause: 只迁移了客户端 `ClientMagic` 的字段和在线施法门禁，没有把 Legacy 注销时“绝对 CastTime 减当前时间、就绪写入 int.MinValue”这条持久化边界接到 world/auth 提交路径。
- Prevention: 运行时冷却使用确定的 `now` 转换为正的剩余毫秒，已就绪统一使用 Legacy 哨兵；`playerCharacterSnapshot` 与显式/异常注销、Observer 接管共同调用完整 `UpdateCharacterMagics`，并用恢复后的客户端 payload 验证边界。
- Verification: 新增 world CastTime 活跃/到期快照测试、auth 魔法 slice 深拷贝测试；P5 ElectricShock 定向、协议 packet、服务端包级编译均通过，后续继续执行全量普通/race 门禁。

### 2026-08-14 — Inspect 测试必须区分 Hero 运行表的 owner key 与 object ID

- Symptom: Hero 检查快照测试把 `world.heroes` 按 Hero `ObjectID` 建表，真实解析却按 owner `ObjectID` 查表，导致合法 Hero 请求返回空快照。
- Root cause: 只看到 map value 中的 Hero object ID，没有读取 Go 运行表所有读写路径确认 map key 语义。
- Prevention: 构造运行态 fixture 前先核对该表的 key contract；同时设置 map key、value 的 `ObjectID` 和 `OwnerID`，并用成功解析及深拷贝断言验证。
- Verification: 修正 fixture 为 `world.heroes[ownerObjectID]` 后，Hero detached-snapshot 与真实 Inspect session 定向测试通过。

### 2026-08-14 — 协议尾字段测试必须显式初始化非零语义哨兵

- Symptom: `UserInformation` observe-flags 尾部测试用 `UserInfo{}` 的零值推断 `SummonedCreatureType=None`，实际 Legacy-compatible `None` 哨兵为 99，断言失败。
- Root cause: 把 Go zero value 当成业务枚举的 None 值，未按生产角色创建路径显式设置协议哨兵。
- Prevention: 固定 wire vector 时逐项初始化业务枚举和 sentinel，禁止用结构体零值代表非零 Legacy 常量；同时断言完整尾部字节序列。
- Verification: 测试显式设置 `IntelligentCreatureNone` 后，protocol 定向、全仓普通和 race 测试均通过。

### 2026-08-13 — 在线组 fixture 必须在 world enter 后建立

- Symptom: 区域魔法的 Group 模式测试已在手工 `SelectInfo` 中设置相同 `GroupID`，命中阶段仍伤害了预期友方玩家。
- Root cause: `world.enter` 会主动清除持久化或手工携带的 stale `GroupID`；测试把进入前字段当成在线组权威投影，实际两名玩家入场后都已变为无组。
- Prevention: 所有依赖在线组关系的 world 测试先让全部成员完成 `world.enter`，再调用 `establishRuntimeGroupForTest` 建立 `world.groups` 与成员 `GroupID`；目标行为前断言非零且相等，禁止只在构造体中预填组号。
- Verification: 区域魔法 fixture 改为 enter 后建立组，Group 模式只命中后来进入区域的非组员，友方、离开者和安全区玩家均保持满血；定向测试通过。

### 2026-08-13 — 新增静态对象类型必须同步完整可见性 transcript

- Symptom: KingOfHill `ObjectSpell` 已正确进入会话可见性后，服务端整包测试仍在第二次 NPC 请求前读到遗留的 packet 150，`TestSessionConquestStartStopRefreshesWarZoneAndNPCVisibility` 失败。
- Root cause: 生产层新增了第五类静态可见对象，但首次修改只扩展了会话状态和刷新函数，没有在同一批次更新已有开战/停战真实会话的新增与移除包期望。
- Prevention: 每新增静态对象类型，都同时列出登录、原地重复刷新、进入/离开范围、领域创建/删除四条路径；明确旧对象移除、新普通对象加入、旧新类型移除、新类型加入的顺序，并一次性更新所有 helper 调用和精确 transcript。
- Verification: 边界测试现锁定几何、无效格、稳定 ObjectID、登录/移动恢复和删除；真实会话锁定开战 `NPC ObjectRemove -> ObjectSpell`、停战 `NewNPCInfo -> ObjectNPC -> effect ObjectRemove`，服务端整包测试通过。

### 2026-08-13 — ControlPoints 结算必须组合 EndWar 与 TakeConquest 的提前返回

- Symptom: Go 的 ControlPoints `End` 无条件清空积分并把每个控制点归一到最终 owner；静态复核发现，Legacy 平局保留当前 owner 时会在 `TakeConquest` 的“winning guild 已拥有 Conquest”门禁提前返回，控制点积分和各点 owner 实际都保持不变。
- Root cause: 只按 `EndWar` 的“计算最终赢家后调用 TakeConquest”概括结算意图，没有把被调函数入口门禁、是否真正换 owner、积分重置和旗帜刷新组成完整可达路径。
- Prevention: 迁移跨函数结算时分别建立“owner 真正变化”和“owner 保持”状态表；只有实际通过 `TakeConquest` 并换 owner 才重置积分、重投全部控制点（包括 owner 已相同的点）和主旗帜，平局/不可接管路径保留运行状态。
- Verification: 领域测试现同时锁定平局保留积分/分点 owner，以及新赢家清空积分并为全部控制点生成刷新事件；控制点结束包序测试覆盖颜色 → 全部控制点 → 主 owner → `BroadcastInfo`，全量普通/race 测试通过。

### 2026-08-13 — 长测试封装必须保留并轮询 exec 会话

- Symptom: `go test ./cmd/crystal-server` 超过首次 30 秒 yield 后，JavaScript 包装只输出 `exit_code/output/wall`，遗漏返回的 `session_id`，外层脚本结束时无法确认测试最终状态，只能重新执行。
- Root cause: 把首次 `exec_command` 返回当成终态，并在序列化时丢弃了继续轮询所需字段；“脚本已完成”不等于其启动的长命令已完成。
- Prevention: 可能超过 yield 的门禁统一保留完整返回值，并在 `session_id` 存在时循环调用 `write_stdin`，直到取得明确 `exit_code`；不得以空输出或外层 cell 完成代替测试成功。
- Verification: 服务端整包、全仓普通测试和全仓 race 均改用会话轮询取得 `exit_code=0`，随后 `go vet ./...` 与 `go build ./...` 也明确返回 0。

### 2026-08-13 — 持久化重试必须分离领域提交与可重复落盘

- Symptom: 未配置账户 JSON 路径时，Conquest 生命周期通知会永久停在 `pendingSave`；连续两次资产保存失败后，重放第一条旧通知还会把 authority HP 从较新的 80 暂时写回旧值 90 再保存。
- Root cause: 把“没有持久化回调”误当成“保存仍未成功”，并把只应执行一次的 authority 状态提交与可重复执行的落盘操作放进同一个 `BeforeSend` 重试闭包。
- Prevention: 可选持久化回调为空时按成功的 no-op 处理；提交后通知拆成一次性领域写入和可重复保存两个阶段，队列重试只能保存当前最新 authority，不能重新应用旧快照。
- Verification: 新增无持久化即时投递测试，以及 90→80 两次失败后重试仍只保存最新 80 的回归测试；Conquest 定向、服务端整包、全量普通/race、vet、build 与差异门禁均通过。

### 2026-08-13 — 缓存目标和延迟投射物必须在命中阶段重验攻城资格

- Symptom: 玩家远程/魔法在开战时成功排入队列后，即使命中前战争结束或攻击者公会成为新 owner，仍会扣除 Conquest 资产生命；Hero 与普通宠物也能保留失效目标继续攻击。
- Root cause: `playerCanAttackMonsterLocked` 只用于请求/选目标阶段，延迟 resolver 和伴侣缓存目标的真正命中阶段没有复用同一门禁。
- Prevention: 所有延迟攻击和跨 tick 目标缓存都执行两阶段校验：选择/排队时校验一次，真正命中前按当前战争、owner、地图和存活状态再次校验；状态变化后必须清空伴侣目标且不产生伤害通知。
- Verification: 回归测试覆盖 Hero/普通宠物选中后战争结束，以及玩家远程/魔法排队后战争结束或 owner 变化；目标 HP、通知和动作队列终态均已锁定，服务端整包测试通过。

### 2026-08-13 — 战争颜色刷新与 BroadcastInfo 是两条独立协议副作用

- Symptom: Conquest 开始/结束只为 WarZone 变化的玩家发送对象刷新，遗漏其他地图的 Legacy `BroadcastInfo`；三人测试中的颜色包顺序还随 Go map 迭代随机变化。
- Root cause: 把 `RefreshNameColour` 的 self/object colour 包和 `StartWar`/`EndWar` 的全服逐玩家 `BroadcastInfo` 合并成一条局部通知路径，同时直接遍历无序玩家 map。
- Prevention: 分别建模颜色变化、全局逐 subject 的附近 `ObjectPlayer` 广播和 NPC 强制开战的额外 announcement 循环；所有可观察玩家遍历先按 ObjectID 排序，再用接收者矩阵锁定每条连接的顺序与重复次数。
- Verification: 参与地图与无关地图的双玩家矩阵均收到正确 `ObjectPlayer`，三玩家 START/STOP 精确包序连续运行 10 次稳定通过，真实会话仍保持颜色/聊天/响应/NPC 可见性顺序。

### 2026-08-13 — Conquest Siege 的运行实体语义必须按实际 Gate 类型迁移

- Symptom: Siege 在修复时使用 Gate 方向公式，但初始加载和普通受击只按 Wall/默认方向处理，导致同一资产在修复前后显示不一致。
- Root cause: 依据数据库列表名 `Siege` 推断运行类型，忽略 Legacy `ConquestGuildSiegeInfo.Spawn` 实际严格创建 AI 72 `Gate` 并调用 `CheckDirection`。
- Prevention: 迁移多态资产时沿 Spawn 的实际构造类型决定运行行为；列表分类只决定持久字段和 NPC 动作，方向、受击和对象投影应复用真实实例类型的公式。
- Verification: Siege 加载、受击方向刷新现使用 Gate 的 midpoint-to-even 公式，并由独立方向与 `ObjectTurn` 回归测试覆盖。

### 2026-08-13 — 权威状态 fixture 必须先完成领域种子再做 JSON 重载

- Symptom: Conquest 全量修复的 Admin 测试预期复活 1 个 Archer，实际得到 0/1；Gate、Wall、Siege 计数仍看似正常。
- Root cause: fixture 为设置 Admin 先保存并重载 auth，此时 JSON 已显式写入空的 `conquests` 数组；后续 Legacy seed 被现代空状态正确拒绝，Bind 只能按定义创建默认存活 Archer 和满血结构。
- Prevention: 通过 JSON 重载改变账户元数据时，必须先导入并绑定本用例依赖的所有权威领域状态，再保存、修改和重载；重载后在执行目标动作前断言关键非默认种子仍存在。
- Verification: fixture 已调整为 Conquest seed/Bind → Admin JSON 重载 → world load；Admin RepairAll 现稳定得到 Archer 1/1，并验证四类终态和公会金币不变。

### 2026-08-13 — Go 条件中的短声明只能位于 if 初始化子句

- Symptom: 新增 Conquest 测试把 `got := ...` 写在 `if existingCondition || got := ...` 中，包级编译报 non-name on left side of `:=`。
- Root cause: 把 Go 的 `if init; condition` 语法误写成了布尔表达式内部声明。
- Prevention: 条件需要复用计算结果时，在 `if` 前单独声明，或严格写成 `if got := expression; conditionUsingGot { ... }`；短声明不得出现在 `&&`/`||` 表达式中。
- Verification: 结果变量改为条件前声明后，Conquest NPC 定向包测试编译并全部通过。

### 2026-08-13 — 特殊物品分支不能绕过通用使用门禁

- Symptom: Guild Skill Scroll 的 shape 10 分支最初直接进入公会逻辑，遗漏通用 `CanUseItem` 校验，也允许死亡角色使用。
- Root cause: 把特殊 shape 当成完整入口实现，只迁移了分支领域效果，没有先执行所有物品共享的角色状态和物品可用性门禁。
- Prevention: 每个特殊物品 dispatch 先列出并复用通用入口前置条件，再进入 shape 专属逻辑；至少覆盖死亡、无权限/不满足条件、定义缺失、成功消费和失败不消费。
- Verification: shape 10 现先调用 `canUseCharacterItem` 并拒绝死亡角色；无公会、非 leader、定义缺失、重复、成功消费等真实入口测试均通过。

### 2026-08-13 — 登录规范化瞬态字段必须回写权威持久层

- Symptom: 登录时从 session/world 投影移除了旧 JSON 错误保存的 newbie Buff type 115，但 auth 内存仍保留旧值，正常登出或重载可能再次恢复该瞬态 Buff。
- Root cause: 把登录规范化当成连接局部清理，没有同步拥有角色持久状态的 auth authority。
- Prevention: 登录期间清理或修复任何持久字段时，必须同时更新 session、world 与 auth 权威记录，并通过正常登出和 JSON 重载验证旧值不会复活；瞬态运行时状态不得写回持久模型。
- Verification: type 115 登录过滤现立即同步 auth；测试覆盖登录运行时属性生效、正常登出和磁盘重载后 type 115 不存在。

### 2026-08-13 — Buff 生命与魔法钳制必须保留逐步包状态

- Symptom: 同一个 Buff 同时降低 MaxHP 和 MaxMP 时，若先算最终状态再发包，两条 `HealthChanged` 会携带相同终态，丢失 Legacy 的 HP 步骤中间状态；逐包持久化又会产生重复写入。
- Root cause: 把连续属性刷新副作用压成一个最终快照，没有分离 wire 中间状态与最终权威持久状态。
- Prevention: 同时变化 HP/MP 时按原调用顺序捕获每一步 payload，先生成 HP 钳制包再生成 MP 钳制包；全部状态完成后只持久化最终快照一次。
- Verification: 回归测试锁定 `HealthChanged(18,20)` 后接 `HealthChanged(18,14)`，并断言只执行一次持久化。

### 2026-08-13 — 新测试引入依赖后立即执行最小编译

- Symptom: Buff 测试新增 `time` 标准库调用后遗漏 import，直到后续包测试才暴露编译失败。
- Root cause: 连续扩展测试场景时没有在首次使用新标识符后立即运行受影响包的编译门禁。
- Prevention: 新增标准库或 package 标识符后立刻核对 import，并执行 `go test <package> -run '^$' -count=1`；恢复编译绿色后再继续增加 transcript 或跨包测试。
- Verification: 已补齐 `time` import；Buff 定向测试和 `cmd/crystal-server` 整包测试通过，提交前继续执行全量门禁。

### 2026-08-13 — 公会进度 fixture 必须显式固定等级阈值

- Symptom: 公会经验测试最初没有观察到经验变化，因为共享 fixture 默认创建等级 5 公会，而测试配置的经验表已没有可升级阈值。
- Root cause: 测试依赖公会默认等级，没有把待验证的当前等级、当前经验和下一等级阈值组成明确前置状态。
- Prevention: 所有等级/经验测试显式设置实体等级、经验、完整阈值表和预期剩余经验；测试开始先断言该 fixture 确实存在目标升级边界。
- Verification: fixture 现固定公会等级与阈值，并覆盖最终经验同时进入玩家、普通宠物、mentee bank 和公会贡献的路径。

### 2026-08-14 — net.Pipe 包数量必须先由接收者矩阵推导

- Symptom: 两人组队接受测试先后把 leader/member 读取数量写成 8/9，实际生产序列是 7/11，两个 reader 因为一方少消费、另一方等待不存在的包而互相反压并最终挂起。
- Root cause: 根据旧测试和心算反复猜总包数，没有先把每条通知按接收者、包 ID 和顺序完整投影；`net.Pipe` 无缓冲，因此错误数量不仅造成断言失败，还会阻塞服务端向另一连接继续写入。
- Prevention: 多连接 transcript 在编码 count 前先列出领域通知的完整接收者矩阵，再机械统计每条连接；所有连接的 reader 必须在触发操作前并发启动，读取完成后再检查共享状态。功能新增包时先更新矩阵，再更新 count 和期望 ID，禁止靠超时或试数修正。
- Verification: 两人入组矩阵锁定 leader 7 包、member 11 包，三人入组与跨地图聊天另有精确 domain/session transcript；`TestSessionTwoPlayerGroupInviteAcceptAndLeave` 不再挂起，受影响包全量测试通过。

### 2026-08-14 — Shell 搜索模式中的反引号不能裸露

- Symptom: 用 `rg` 搜索 Markdown 文本时，把包含反引号的模式放进双引号，shell 尝试执行其中的 `GroupID` 并输出 `command not found`。
- Root cause: 忽略了双引号内反引号仍会触发命令替换，把文档字面量直接拼进了 shell 命令。
- Prevention: 搜索 Markdown、Go struct tag 或其他可能含反引号的文本时，整段 pattern 使用单引号；需要同时包含单引号时改用参数化调用或拆分模式，禁止把未知文本插入双引号命令。
- Verification: 后续同类 `rg` 查询使用单引号模式完成，源码和文档未被命令替换影响；提交前 `git diff --check` 与敏感文件检查继续通过。

### 2026-08-13 — 原子回滚测试的期望快照不能与请求共享引用

- Symptom: `AttachSealedHero` 的伪造 stats 负例报告角色权威状态被修改，实际变化来自测试直接改写了与调用前 `before` 快照共享的 `StoredItem.AddedStats` map。
- Root cause: 构造事务请求时浅拷贝了物品切片/指针，把“待篡改输入”和“零变更基线”当成两份数据，导致测试自身污染期望值。
- Prevention: 所有原子失败测试在修改 request 前，递归深拷贝 item grids、Hero snapshots、stats maps 和嵌套 slots；失败后分别从服务重新读取角色与 registry，并与独立基线比较。
- Verification: attach fixture 改用 `cloneItemInfos`、`cloneStoredItems`、`cloneStoredHeroes` 构造请求；伪造 stats、stale stack、stale Hero 槽、满槽、已绑定及双侧物品缺失用例均通过。

### 2026-08-13 — 全局实体表不能简化成角色槽内嵌生命周期

- Symptom: P8 Hero 草稿把完整 Hero 只存于角色槽；按原版执行封印时一旦清空槽位，封印物品虽然保留 Hero ID，Hero 本体却会从 Go 持久化状态消失，无法再次解封。
- Root cause: 只迁移了 Character 保存的 Hero 槽投影，没有同时保留 Legacy 独立 Hero 全局表；把“当前绑定关系”和“实体生命周期”合并成了同一份内嵌数据。
- Prevention: 迁移由全局表实体加外键槽位组成的数据模型时，权威存储必须分别表达实体 registry 与绑定投影；解绑、封印、删除只改变绑定/删除状态，不能隐式释放全局身份、名称或 ID。旧 JSON 可从槽位重建 registry，新格式必须覆盖游离实体保存重载和按 ID 恢复。
- Verification: `TestUnboundHeroRegistryLifecycleSurvivesJSONAndRetainsName`、`TestHeroUnbindRetainsRegistryAndRequiresExplicitRebind`、`TestHeroRegistryIsAuthoritativeAcrossCommitAndMaximumItemIDScan`、封印会话重载及旧 JSON fallback 测试已覆盖游离实体、名字占用、`AddedStats[129]` 按 ID 恢复和 ID 连续性。

### 2026-08-13 — Hero 解封业务类型必须依据变更前绑定状态

- Symptom: Type 42 封印物品解封后，用“本次是否成功召唤”区分首次 Hero 与仓库 Hero，会把 `NoHero` 地图上的首次绑定误报成“已加入仓库”，并遗漏首次 Hero 的未召唤持久状态。
- Root cause: 业务分支读取了地图门禁后的运行时结果，而 Legacy 的分支条件是解封前是否已有当前 Hero；绑定关系与召唤结果是两个独立状态。
- Prevention: 解封事务前固定捕获 `hadCurrentHero`；提交 auth/world/JSON 后，以它决定首次 Hero 或仓库 Hero 的包形状，再独立依据地图门禁决定是否生成 runtime。首次 Hero 在 `NoHero` 地图必须持久化为已绑定但未召唤。
- Verification: 首次解封、已有 Hero、`NoHero` 和槽位满四条真实会话均断言包序、auth/world/JSON 终态；跨 `NoHero` 地图及断线重登测试锁定未召唤状态不会复活。

### 2026-08-13 — Hero 可见性必须按接收者矩阵拆分属性包

- Symptom: 初版 Hero 广播给所有观察者相同的 `ObjectHealth`/`ObjectMana`，并重复发送颜色包；Hero 名称颜色还直接沿用了内部 MediumOrchid，而 Legacy 的 viewer-relative 投影为 White。
- Root cause: 把 `Broadcast` 对象包、owner/group 属性可见性和 owner-only Mana 合并成一条通知路径，没有展开 `HumanObject.GetInfoEx(viewer)` 与召唤后的额外 owner 更新。
- Prevention: 明确矩阵：所有可见者收到 `ObjectHero` 和一次颜色；owner 与非零同组收到可见生命，owner 额外收到 Mana 及召唤后的第二次 Health；后续进入视野只发其当时有权看到的 Health，不补 Mana。对象名称颜色从 viewer-relative 投影生成。
- Verification: `TestGameWorldHeroSummonObserverPacketMatrix` 和 `TestRefreshStaticHeroVisibilityObserverMatrix` 分别锁定召唤时与后续入视野的 owner/group/普通观察者包序、数量、颜色和 Mana 边界。

### 2026-08-13 — 会话局部物品变更必须先同步 world 再读取整角色快照

- Symptom: P8 整包回归中，普通/太阳药水已经正确消费、删除响应也成功，但登出持久化又出现两个旧物品；`TestSessionUseAndDeleteItemTranscriptAndPersistence` 稳定失败。
- Root cause: 本批为了宠物登出读取 world 快照，使既有 `DeleteItem` 未同步 world 的缺口变成可见；退出阶段的整角色快照覆盖了 session 中更新后的背包。最初尝试只合并 `Pets` 仍会被后续整快照覆盖，未触及真正的状态所有权问题。
- Prevention: 任何 session 局部物品消费、删除、移动或创建一旦成功，必须在可能读取 `playerCharacterSnapshot` 前同步 `world.updatePlayerItems`；伴侣持久化只负责其领域字段，不能用条件式旧快照合并掩盖其他领域未同步。新增登出读取路径后，必须重跑所有会话物品持久化测试。
- Verification: `ClientDeleteItem` 成功后现同步 world，删除了无效的宠物条件合并；目标用例连续运行 20 次及 `cmd/crystal-server` 整包测试均通过。

### 2026-08-13 — 等级化实体恢复必须在刷新属性后无条件应用保存生命值

- Symptom: 等级 2 宠物基础 HP 20、刷新后 MaxHP 60、保存 HP 50 时，旧条件 `info.HP < pet.HP` 会错误保留初始化 HP 20；Wizard 非 Clone 的零 `TameTime` 也被误当成永久宠物，且到期没有名称广播。
- Root cause: 用刷新前/初始化生命值作为恢复条件，并把 `TameTime > 0` 错当成“字段是否存在”；Legacy 实际对 Wizard 非 Clone 总是执行 `now + TameTime`，到期解除主人后发送 `ObjectName`。
- Prevention: 恢复等级化实体时先计算 MaxHP/属性，再无条件赋保存 HP 并钳制到 `[0, MaxHP]`；时间字段的零值语义必须沿真实加载调用核对，不能用非零判断代替存在性；所有所有权到期路径同时核对可见名称/颜色等广播。
- Verification: 新增等级宠物 HP/属性/速度、特殊三倍经验/等级上限、PetSave 筛选、正负剩余时间，以及零 TameTime 到期 `ObjectName` 的回归测试；协议 serializer 与 malformed-data 测试通过。

### 2026-08-13 — 声明的持久化字段不代表 Legacy 实际写入记录

- Symptom: `PetInfo` 声明了 `TameTime`，但 117/0 `Server.MirADB` 的构造、保存和读取路径均未处理它，每只普通宠物实际仍只有 14 字节；若按字段声明追加读取会错位后续角色数据。
- Root cause: 把模型成员列表误当成二进制格式契约，没有以真实 reader/writer 调用序列核定记录宽度。
- Prevention: 迁移 Legacy 二进制记录时逐字段对照构造、Save 和 reader，并用后续哨兵字段锁定偏移；未写入旧格式但 Go 运行时需要的状态通过 Go JSON 模型单独贯通，不能改变旧记录宽度。
- Verification: Go 导入器继续严格消费 14 字节并令导入 `TameTime=0`，偏移回归测试保留后续字段；非零/负 `TameTime` 已通过 protocol/auth JSON 往返测试。

### 2026-08-13 — 提交 tracked diff 不会自动包含未跟踪源码

- Symptom: P11 首次用 `git diff --name-only -z | xargs git add` 暂存时，只提交了已跟踪修改，十个新 Go 源码/测试文件仍留在工作区，提交统计与已通过测试的源码集合不一致。
- Root cause: `git diff --name-only` 默认不列出 untracked 文件，把“当前差异清单”误当成了完整工作区清单。
- Prevention: 提交前以 `git status --short` 为权威清单；明确暂存目标范围时同时处理 `??` 文件，提交后立即再次检查 status 与 `git show --stat`。禁止仅依赖 `git diff --name-only` 构造完整暂存集合。
- Verification: 十个未跟踪 Go 文件已显式暂存并 amend 到同一个 P11 提交，Go 仓库提交后工作区为空，提交包含 43 个文件。

### 2026-08-13 — 延迟物品生产必须同时固定锁序和最终快照顺序

- Symptom: 智能生物自动生产黑石最初在 `world.mu` 内调用 auth 全局 ID 分配器；移出锁后，生产通知又排在通用持久化通知之前，后续旧角色快照可能覆盖刚生成的物品。
- Root cause: 只把跨域调用改成延迟执行，没有把同一 tick 的所有快照按实际提交时刻排序；“锁外执行”与“最终权威状态”被当成两个无关问题。
- Prevention: world 锁内只准备不可变创建上下文，所有 auth/全局 ID/随机物品创建在锁外执行；同一 tick 先提交锁内捕获的普通状态和拾取金币，再执行物品创建、重入 world 合并并提交包含新物品的终态。测试必须让创建回调主动获取 `world.mu` 验证无反向锁序，并逐个执行通知后检查 auth 最终物品。
- Verification: 黑石创建回调已整体移到 world 锁外，生产通知排在通用 persist 之后；锁序超时测试和 auth 终态测试通过，`cmd/crystal-server` 整包测试通过。

### 2026-08-13 — 测试 helper 与生产标识符必须先做包级冲突检查

- Symptom: 新增生产 helper 时使用了测试文件已有的 `intelligentCreatureRewardRoll` 名称，导致同包编译冲突；新增磁盘会话测试又在使用 `filepath` 后遗漏 import。
- Root cause: patch 前只检索生产文件，没有检索整个 Go package 的标识符和 import 需求，也没有紧跟最小编译检查。
- Prevention: 新增 package 级标识符或标准库依赖前，用 `rg` 搜索整个目录（包含 `*_test.go`）；每个语义 patch 后立即 `gofmt` 并运行受影响包的定向编译/测试，再继续扩大改动。
- Verification: 生产 helper 改为 `rollIntelligentCreatureReward`，补齐 `path/filepath` import；智能生物定向测试和服务端整包测试均通过。

### 2026-08-13 — 测试调用 helper 前必须读取完整返回签名

- Symptom: Buff 会话 fixture 把无返回值的 `UpdateCharacterHealth` 当成可用于条件判断的成功布尔值，造成包级编译失败。
- Root cause: 只按同类 `Update...` 命名推断返回值，没有读取当前方法的真实声明。
- Prevention: 每次新增测试 helper 调用先用 `rg` 定位声明并读取完整参数/返回签名；首次 patch 后先跑受影响包的最小编译，再扩展 transcript。
- Verification: fixture 改为按无返回值签名调用，Buff 定向测试与 `cmd/crystal-server` 整包测试通过。

### 2026-08-13 — 定时业务必须沿调用链传递同一个确定时间

- Symptom: 自动黑石的名称时效最初使用 `time.Now()`，与 tick 测试传入的 `now` 不一致；给 fixture 名称加入 `[2h]` 后又不再匹配默认黑石配置名。
- Root cause: 创建 helper 在调用链中重新读取墙上时间，测试 fixture 也隐式依赖默认名称匹配，随机/时间/配置三类输入没有全部显式化。
- Prevention: 所有 tick 驱动的过期、随机和生产逻辑把同一个 `now`、roll 与配置名称封装进不可变上下文向下传递；带时效标记的物品 fixture 显式设置对应配置名，不依赖默认模糊匹配。
- Verification: 自动黑石使用 tick 的 `now` 计算 `[2h]` 到期，测试显式调用 `setIntelligentCreatureSettings`，确定性 `CreateDropItem` 与时效测试通过。

### 2026-08-13 — 通知测试 helper 不得隐式提交所有副作用

- Symptom: 为方便测试而让 tick helper 自动执行所有 `BeforeSend` 后，既有用例无法再断言延迟持久化/创建的通知数量和时序。
- Root cause: 读取状态的 helper 混入了投递副作用，调用者无法选择观察“生成通知”还是“完成投递”两个阶段。
- Prevention: tick helper 只返回通知；另设显式 deliver helper，并在需要时逐项执行 `BeforeSend`。涉及多阶段事务的测试分别断言通知顺序、阶段中间态和最终权威状态。
- Verification: `intelligentCreatureTestTick` 与 `intelligentCreatureTestDeliver` 已拆分，黑石锁序和 stale 快照回归测试可分别验收投递前后状态。

### 2026-08-13 — Buff 协议对象必须使用运行时 ObjectID

- Symptom: 智能生物奖励和安全区 Buff 测试若使用持久化角色 Index 编码 `AddBuff`/`PauseBuff`，单角色 fixture 可能碰巧通过，但观察者会定位到错误世界对象。
- Root cause: 把数据库角色身份和当前 world 会话身份混用，没有在 wire transcript 中用不同数值锁定边界。
- Prevention: 所有 Object*、Buff 增删/暂停协议字段使用 `worldPlayer.ObjectID`；持久化 API 才使用 `SelectInfo.Index`。会话 fixture 必须断言 bootstrap 分配的 ObjectID 出现在 payload 中。
- Verification: WonderDrug `AddBuff` 与安全区 `PauseBuff` 会话测试都解析并断言真实运行时 ObjectID，同时验证 auth 与 JSON 持久化。

### 2026-08-13 — 特殊物品迁移必须审计通用入口尾部副作用

- Symptom: 只实现 Strongbox/BlackStone/WonderDrug/Knapsack 等特殊分支时，容易遗漏通用 UseItem 尾部的二次响应、源物品消耗或无效果也消费等 Legacy 可观察行为。
- Root cause: 测试只调用领域 helper 或只关注奖励结果，没有覆盖生产 dispatch 返回后的通用处理链。
- Prevention: 每种特殊物品同时测试领域结果和真实生产入口；逐项列出分支内部与通用尾部各自的消费、持久化和响应包，尤其锁定 Strongbox 双 `UseItem`、FortuneCookie 无效果仍消费以及材料不足/不适用路径。
- Verification: 当前生产会话覆盖 BlackStone、Strongbox、WonderDrug、FortuneCookie、Knapsack 全部 shape，并断言完整包序列、源物品终态、auth 状态与磁盘重载。

### 2026-08-13 — 测试参考模型不能替代生产入口覆盖

- Symptom: 奖励 shape 的参考调度测试即使全部通过，也不能证明 `main.go` 的真实 `ClientUseItem` 分支已接线或按相同顺序持久化和发包。
- Root cause: 参考模型与生产实现可能共享错误假设，且绕过连接状态、通用尾部和 auth/world 桥接。
- Prevention: 参考模型只用于穷举规则；每个功能簇至少增加一个真实 dispatch/net.Pipe 会话，覆盖生产 packet 入口、状态桥、网络 transcript、登出和 JSON 重载。
- Verification: `TestCurrentGoDispatchHandlesEveryRewardShapeInProduction` 之外，新增奖励生产会话逐 shape 经过真实 ClientUseItem，并从账户 JSON 重载验证终态。

### 2026-08-13 — 会话 transcript 必须包含登录后的延迟 Buff 包

- Symptom: 带持久 Buff 的角色登录后，延迟发送的 `AddBuff` 可能落在第一条业务请求响应之前，若测试只按静态 bootstrap 列包会把它误判成目标动作响应。
- Root cause: 登录状态完成与定时/延迟通知并非同一同步边界，fixture 没有把已持久 Buff 的后续投递列入 transcript。
- Prevention: 带 Buff 的会话测试在首个业务 marker 前显式消费并断言登录后 `AddBuff`，或把它作为有序 transcript 的第一项；payload 同时断言运行时 ObjectID。
- Verification: 安全区移动会话锁定 `AddBuff -> PauseBuff -> HealthChanged -> UserLocation`，并通过登出及磁盘重载验证。

### 2026-08-13 — 地图领域通知与真实接收者矩阵必须同时验收

- Symptom: 智能生物召回/跨图/禁宠地图的领域状态正确，不代表 owner、旧地图观察者和目标地图观察者都收到正确的 Teleport/Remove/ObjectMonster/Health 顺序。
- Root cause: 只断言 world 对象坐标或通知总数，没有按每个接收者过滤领域通知并走真实会话投递。
- Prevention: 地图对象功能同时建立领域接收者矩阵和 net.Pipe 会话：分别断言 owner、旧观察者、新观察者的 packet ID、payload ObjectID 与顺序；禁用地图还要验证持久化解除召唤。
- Verification: 智能生物 visibility 测试覆盖第二观察者登录、移动、召回、跨图和禁宠地图，逐接收者断言通知并通过服务端整包测试。

### 2026-08-13 — Buff 属性 fixture 必须从真实基础属性逐项计算

- Symptom: WonderDrug/Knapsack 与安全区暂停测试若直接猜最终 HP、背包负重或 buff stats，容易遗漏 class/level 基础公式、现有附加属性和叠加规则。
- Root cause: 把预期终值按直觉填写，没有从角色基础属性、物品 AddedStats、已有 Buff 和暂停状态逐项代入生产计算。
- Prevention: 属性测试固定角色 class/level/HP/MP 和源物品 Stats/AddedStats，先列出基础值与每层增量，再断言最终 stats、生命钳制和“延长时长但不替换原 stats”等怪癖。
- Verification: 当前测试锁定 WonderDrug HP、Knapsack Luck→BagWeight 及二次使用仅延长时长，安全区暂停后 MaxHP/HP 钳制也由生产会话验证。

### 2026-08-13 — 双向协议 transcript 不得按 packet ID 反推方向

- Symptom: 智能生物 transcript 的第 21 项把客户端 `UpdateIntelligentCreature=125` 误判成服务端方向，因为服务端 `ObjectEffect` 也合法使用 ordinal 125。
- Root cause: 测试只保存 ID，并通过一组服务端 ID 推导方向；Crystal 的客户端与服务端枚举是独立命名空间，允许跨方向重号。
- Prevention: 所有双向 transcript 期望使用显式有序 `(ID, direction)` slice，每一帧同时声明 ordinal 和方向；禁止用 packet ID、数值范围或名称集合反推方向。
- Verification: P11 transcript 已改成逐项显式 ID/方向期望，合法的 125 跨方向重号分别按真实发送方向验收。

### 2026-08-13 — `go test -run` 正则必须作为单个 shell 参数引用

- Symptom: 定向测试命令中的 `TestA|TestB` 未加引号，zsh 把竖线解析成管道，首段测试输出被后续的 `command not found` 覆盖。
- Root cause: 在构造 shell 命令时只关注 Go 的正则语义，没有同时保护 shell 元字符。
- Prevention: 所有含 `|`、`(`、`)` 或通配符的 `go test -run` 表达式统一用单引号包成一个参数；若通过参数数组组装命令，不能用无转义的空格拼接后直接交给 shell。
- Verification: 改为 `-run 'TestFishing|TestAwakening|TestRanking|TestEquipCharacterSlotItem'` 后，服务端定向测试通过。

### 2026-08-13 — 有序协议 transcript 禁止使用 map 驱动

- Symptom: P11 Go 探针定向测试在发送觉醒重置请求时出现 `io: read/write on closed pipe`，对端已因收到顺序漂移的前一请求而退出。
- Root cause: 测试用 Go map 保存 NPC 面板读取器和客户端请求函数，却让 `net.Pipe` 服务端按固定 Legacy 顺序读取；map 迭代顺序不稳定，不能表达 wire transcript。
- Prevention: 所有有序协议、数据库迁移步骤和副作用序列统一用显式 slice/数组声明顺序；map 仅用于不关心顺序的集合断言。对端报 EOF/closed pipe 时先核对双方完整 packet 序列。
- Verification: 两处 map 已改成具名函数的有序 slice；P11 定向测试连续运行 10 次、`internal/probe` 整包测试、P11 race 测试和 `go vet` 均通过。

### 2026-08-13 — 新测试 fixture 必须先检索真实 bootstrap API

- Symptom: 排行榜领域测试按记忆调用不存在的 `AddTestAccount`，导致包级编译失败。
- Root cause: 把其他项目常见的测试 helper 名称带入当前 auth 包，没有先检索现有账户 fixture；本项目公开 helper 实际是 `AddPlaintextAccount`。
- Prevention: 新测试调用服务 bootstrap/helper 前先用 `rg` 查声明和同包现有用法，复制真实签名后再写测试；包级仅编译门禁应紧跟新增 fixture。
- Verification: 排行榜 fixture 改用 `AddPlaintextAccount(id, password, nil)`，auth 定向测试恢复通过。
- Strengthening after recurrence: 即使 helper 名称已通过检索确认，也必须读取其完整函数签名，不能凭同类 `Update...` 方法习惯推断有 `bool` 返回值；本次 `UpdateCharacterMapRuntime` 是无返回值写入，测试误将其用在条件表达式导致包级编译失败。新增 fixture 后先跑最小包级编译，再扩展网络 transcript。

### 2026-08-13 — 新功能 ordinal 必须套用当前基线的历史插入偏移

- Symptom: P11 首次把本地 Legacy enum 直接计数得到的钓鱼、觉醒和排行榜 ordinal 写入 Go，立即与当前 Go 的 `SendOutputMessage` 等常量冲突。
- Root cause: 忘记当前迁移 wire 基线在 `DeleteItem` 和 `TownRevive` 处各有一个历史插入；本地只读 C# 快照的后续枚举值必须整体加一，不能把原始计数直接用于当前基线。
- Prevention: 每批新增协议先定位目标包前后两个已迁移常量，机械应用已记录的方向插入偏移，再在同一 patch 中加入显式 ordinal 和方向唯一性测试；任何冲突都先修正基线计算，不能挪用空闲 ID。
- Verification: P11 生产常量改为 Server `ObjectEffect=125`、`FishingUpdate=201`、`NPCAwakening=225`、`Rankings=253`，Client `FishingCast=103`、`AwakeningNeedMaterials=112`、`GetRanking=136`；现有 ordinal 测试表在统一偏移循环前故意填写 C# 快照原值，`got` 才填写当前生产值。首次把当前值直接写进 `wants` 导致测试再次加一，现已修正并由完整唯一性测试锁定。

### 2026-08-13 — 清理 helper 的多 hunk patch 也必须逐段复读

- Symptom: 清理两个未使用 helper 时，一次多 hunk patch 因第二个函数正文与记忆中的调用顺序不一致而整体拒绝；随后把 `rg`、格式化和测试串在同一命令中，又因预期的零匹配让整条门禁提前退出。
- Root cause: 依赖先前摘要而没有在 patch 前复读每个目标函数的精确正文，并把“零匹配即成功”的检索当作普通零退出命令。
- Prevention: 删除多个独立 helper 时逐个读取、逐个 patch；需要断言无匹配的 `rg` 单独执行并按空输出验收，不能让其退出码短路后续格式化或测试。
- Verification: 两个 helper 已按实际正文分别删除，无用 `strings` import 单独清理；格式化、定向测试和全量门禁随后独立执行。
- Strengthening after recurrence: `git diff --no-index` 在文件确有差异且内容合法时也固定返回 1；不得把它与 `git diff --check` 串成一个总门禁并把预期的 1 误报为失败。新增文件检查要单独运行，并显式把“有差异”的退出码 1 转换为成功，同时保留真正的 whitespace 错误输出。

### 2026-08-13 — 跨领域锁内禁止调用公会 authority

- Symptom: 公会战争/领地骨架最初在持有 `world.mu` 时调用 `GuildAuthority`，而 authority 的事务顺序是 authority → auth；这会与公会投影、会话初始化或生命周期线程形成反向锁序风险。
- Root cause: 把“刷新在线投影”写成了锁内查询权威状态，没有先切分 detached authority 快照与 world 写入阶段。
- Prevention: 公会 authority 调用必须在 `world.mu` 外完成；先取得敌对、战争或领地结果并释放 authority/auth 锁，再进入 world 锁合并投影和构造通知，网络投递与 SaveJSON 都放在所有锁之外。懒初始化同样先复制配置/定义，再在 world 锁外构造 authority。
- Verification: P9 接线改为 authority 查询/事务 → world 投影 → SaveJSON → 通知四阶段，服务端整包测试通过；race 门禁在提交前继续验证。

### 2026-08-13 — 领域层状态成功不等于完整网络 transcript

- Symptom: 战争和领地领域测试已覆盖扣款、敌对和所有权，但首次双会话测试仍暴露 `ColourChanged`/`ObjectColourChanged`/`ObjectPlayer` 的接收者顺序，以及离线时实际只有 `ObjectRemove` 而没有跨公会成员包。
- Root cause: 只按业务提交结果推断网络行为，没有按每个在线观察者展开 Legacy 的广播循环和会话注销路径。
- Prevention: 跨玩家功能必须同时建立领域终态测试与 net.Pipe 接收者矩阵；逐个列出发起者、同会成员、敌会成员和普通观察者的包序列，并以实际路径为准修正 fixture，不能凭相似公会通知推断注销包。
- Verification: 新增战争双会话和领地分页/购买 transcript，锁定双方聊天、金库、颜色/对象刷新与持久终态；定向服务端测试通过。

### 2026-08-13 — Schema tests must assert semantics, not gofmt alignment

- Symptom: Adding GuildSettings fields made existing expected structs stale, while a static schema guard failed only because `gofmt` changed column-alignment spaces.
- Root cause: Schema evolution was not accompanied by all aggregate fixture updates, and source-text assertions encoded incidental whitespace instead of declarations.
- Prevention: When extending persisted structs, search every composite literal and legacy-JSON fixture for expected values; normalize source whitespace (or inspect types structurally) before static declaration checks, and explicitly test omitted JSON fields retain zero-value runtime fallbacks.
- Verification: Guild settings fixtures now cover defaults, Setup.ini overrides, invalid-value fallback, round-trip values, and old-JSON zero values; both `./internal/worlddata` and `./internal/legacyworld` pass.

### 2026-08-13 — 独立 Go 导出器必须复现 Legacy 加载语义而非只解码字段

- Symptom: 世界导出器能完整解码 117/0 文件，但账户归档角色仍会绑定拍卖，Windows 反斜杠/大小写路径在 Unix 主机失效，嵌套 `#INSERT` 没有继续展开，缺失物品定义会中止整个拍卖或公会文件，Quest NPC ID 也可能与运行时实例不一致。
- Root cause: 把二进制字段布局正确等同于加载结果等价，遗漏了原服务端在解码后的归档过滤、文件系统语义、增长列表展开、`BindItem` 容错及地图/NPC 实例化顺序。
- Prevention: 所有独立 Go 迁移工具沿“读取 → Legacy 过滤/绑定 → 可观察投影”逐层验收；路径同时兼容 `\\`/`/` 和 Windows 大小写，动态 include 保留原增长列表顺序并加循环/规模保护，记录必须先完整消费再按绑定结果丢弃，运行时身份由导出器显式携带并保留旧 JSON fallback。
- Verification: 新增归档月份边界与拍卖绑定、跨平台路径、两层/循环 Drop include、拍卖/公会缺失定义、NPC 数据库/地图顺序及无效坐标、Monster 客户端投影的 Go 回归测试；定向测试通过，提交前继续执行全量 test/race/vet/build 门禁。

### 2026-08-13 — C# 源码与既有工具必须保持只读基线

- Symptom: 为公会仓库补协议向量和导出字段时，直接修改了迁移仓库中的两个 `.cs` 工具文件，破坏了用户用于逐项对照的 C# 基线。
- Root cause: 把“C# 可作为迁移辅助工具”误当成了允许继续演进 C#；实际上服务端、测试客户端、协议探针和数据转换工具都属于待迁移范围。
- Prevention: 两个仓库中的所有 `.cs` 一律只读，不新增、不修改、不删除、不重命名；只可读取作为行为证据。运行时、测试客户端、协议探针、导入/导出及其他迁移工具全部用 Go 实现。每次提交前分别执行工作区 diff、暂存区 diff 和未跟踪文件三项 `.cs` 检查，结果必须全部为空。
- Verification: 已用反向补丁精确撤销本批两个未提交 C# 差异，并把语言边界固化到 `AGENTS.md`；用户再次明确工具也必须用 Go 后，本批只修改 Go、Markdown 和 Git 元数据，提交前继续以两仓库六项 `.cs` 零输出门禁验收。

### 2026-08-13 — 跨 auth/world 物品事务必须先同步权威状态再投递网络

- Symptom: P11 审查发现 Storage 附件已经在 auth 原子提交，但 world 物品快照直到 `RefreshItem`/结果包写出后才更新；钓鱼 Tick 也先投递通知再持久化，觉醒拆解新增的 `ItemInfos` 没有同步到 world。连接写失败后的 cleanup 可能用旧 world 快照覆盖已提交状态。
- Root cause: 把成功响应顺序当成了整个事务顺序，没有区分“auth/world 双权威状态提交”和“可能失败的网络通知”；同时只同步物品格，遗漏了新物品定义目录。
- Prevention: 所有跨 auth/world 的物品事务先完成 auth 提交、world `ItemInfos`/三类物品格同步及必要落盘，再按 Legacy 顺序投递网络包；网络失败只能影响通知，不能让 cleanup 从旧快照回滚事务。多个定时结果先选取最后一个 changed 快照持久化，再保持原结果顺序投递全部通知。
- Verification: `advanceFishing` 已改为先持久化最后一个变化快照再投递 Tick 通知；`EquipSlotItem` 在任何响应写入前同步 world；觉醒持久化同时同步 `ItemInfos` 与物品格，并新增 world/auth 定义一致性测试和网络材料不足事务测试。提交前继续运行普通/race 全量门禁。

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

### 2026-08-13 — 公会门禁必须以 auth 权威成员关系为准

- Symptom: 公会仓库 world 入口先看 session 中缓存的 `Character.GuildIndex`，会让已失效但尚未同步的公会投影走到安全区提示；Type 3 请求还可能在确认权威成员关系前消耗一次性列表状态。
- Root cause: 把连接局部投影当成了成员资格的授权源，并过早提交了 `GuildCanRequestItems=false` 副作用。
- Prevention: 公会授权先在 auth 的同一锁域内校验 guild 和 member，再检查安全区/权限；一次性状态只有在所有前置校验成功后才能消耗。world 快照只用于定位连接和投递包，不决定权威成员资格。
- Verification: 新增 stale world GuildIndex 回归测试，锁定金钱请求优先返回 NotPartOfGuild，失败的列表请求保留 `GuildCanRequestItems`；公会仓库定向测试通过。

### 2026-08-12 — 公会 ordinal 必须锁定目标 wire 基线而非相似源码快照

- Symptom: 初次从本地 `Shared/Enums.cs` 计数得到公会 Client `79..83`、Server `84/166..171`，但当前迁移目标的精确 wire 基线实际整体高一位，并且需要包含 `GuildExpGain=171`。
- Root cause: 把工作区中的一个 enum 快照直接当成最终目标版本，未先与主线正在使用的当前协议基线交叉确认；同时只按用户最初列举包名收口，漏掉了夹在 `GuildInvite` 与 `GuildNameRequest` 之间、决定后者 ordinal 的 `GuildExpGain`。
- Prevention: 对版本可能漂移的协议枚举，同时核对完整成员序列、主线当前 wire 基线和相邻占位包；显式测试必须包含整个连续区间，不能只断言功能入口包。收到权威 ordinal 修正后，以其为当前迁移基线并检查相邻常量是否遗漏。
- Verification: 公会测试现显式断言 Client `80..84`、Server `ObjectGuildNameChanged=85` 与 `GuildNoticeChange..GuildNameRequest=167..172`，并覆盖 `GuildExpGain` UInt32 小端 payload；`go test ./internal/protocol -count=1` 通过。
- Strengthening after recurrence: 插入枚举成员会让插入点后的所有现有常量漂移，局部修目标包会制造跨功能 ID 冲突（本次为 `ObjectGuildNameChanged=85` 与旧 `GainExperience=85`）。今后发现插入点后必须机械审计该方向所有已定义常量，更新所有领域 ordinal 测试，并让唯一性测试枚举生产常量表中的每个已定义 ID；当前验证覆盖 175 个 Server 与 116 个 Client 常量，且锁定 Server `HealthChanged=77 → DeleteItem=80 → ObjectGuildNameChanged=85 → GainExperience=86`、Client `GroupInvite=62 → TownRevive=69 → EditGuildMember=80` 边界。

### 2026-08-12 — 单次异常排查不得变成固定汇报项

- Symptom: 用户只为确认一次异常而询问模型、Goal 或代码比对状态，后续迁移仍反复展示同类截图分析和内部状态。
- Root cause: 把一次性诊断误当成长期进度模板，没有在问题解释清楚后恢复到只汇报交付结果的沟通边界。
- Prevention: 一次性异常只回答当次；除非用户再次询问或出现新的真实异常，后续不主动汇报代码比对、Goal 数量、模型名称、内部调度或已解释过的原因。正常迁移仅在整批完成、真实阻塞或必须由用户决策时汇报。
- Verification: 本轮已停止重复发送该类状态，并继续直接完成关系功能批次。

### 2026-08-12 — 跨会话状态变更必须主动唤醒目标会话

- Symptom: 爱人跨地图召回已立即发送 MapChanged/ObjectTeleportIn，但目标会话的本地地图快照、位置持久化以及 NPC/怪物/地面物品刷新，要等目标客户端再发一个包才执行；全量测试还暴露了额外传送包与 barrier 的时序竞争。
- Root cause: 共享 world 状态由发起者会话修改，目标会话却阻塞在 ReadFrame；初版 pending 队列只有轮询消费，没有事件唤醒，且测试把固定包数量误当成了跨 goroutine 的完成屏障。随后直接反复改 socket read deadline 虽能唤醒，却会与下一次 ReadFrame 竞争，造成已入队 KeepAlive 被旧 deadline 立即超时并丢失；临时 wake channel 又因重建窗口丢通知。
- Prevention: 外部会话入队 transition 后通过会话生命周期内固定的 buffered wake channel 唤醒其所有者；会话用单个受控读 goroutine 等待客户端帧，并在等待期间可多次应用本地状态、持久化和可见对象刷新，随后继续消费同一个客户端帧。只有真正的会话/任务超时才短暂设置 read deadline，业务唤醒禁止借用 socket deadline；队列不能用单槽覆盖，wake channel 不能在读循环中重建。会话测试使用 KeepAlive 完成屏障，并允许已经验证过但可能稍后到达的传送尾包，不能靠客户端主动包来触发业务状态。
- Verification: 同地图、跨地图静态对象刷新、无需目标主动发包的位置持久化和连续两个 pending transition 均有 net.Pipe 覆盖；Go 全量测试通过。

### 2026-08-12 — 会话 fixture 必须显式设置方向并穷举装备门控

- Symptom: 跨地图召回测试期望落在 (1,0)，但角色默认方向为 0，实际合法目标是原地 (0,0)；婚戒替换测试最初又因角色等级 1 与默认 RequiredType=Level/RequiredAmount=2 不符而提前走装备失败分支。
- Root cause: fixture 依赖了隐式默认方向和物品需求字段，没有把目标坐标公式及 CanEquipItem 的职业、性别、等级/属性门控全部固定下来。
- Prevention: 传送 fixture 在登录前显式持久化方向并按 Front 计算目标；装备事务 fixture 显式设置合法 RequiredType、RequiredAmount、RequiredClass、RequiredGender，同时为已绑定婚戒等隐藏状态补负例。
- Verification: 跨地图召回稳定落在预期前方坐标，婚戒替换覆盖余额不足、非法物品、已绑定婚戒及成功原子交换。
- Strengthening after recurrence: 该规则同样适用于钓鱼附件、Mount/Socket 与 Storage 来源的 `EquipSlotItem`；`RequiredClass=0` 或 `RequiredGender=0` 不是“无限制”，而是位掩码不包含任何角色。所有可使用附件 fixture 必须显式给当前职业/性别位，边界负例则单独改变目标字段。

### 2026-08-12 — 跨会话测试数据必须避开 bootstrap 提前消费

- Symptom: 为验证召回后的地面金钱刷新而预置目标地图对象时，对象在调用者 bootstrap 阶段就被发送，导致召回 transcript 缺少预期 ObjectGold。
- Root cause: 测试在会话建立前就把共享对象放在当前可见范围，没有区分 bootstrap 可见性与目标动作后的新增可见性。
- Prevention: 需要验证动作触发刷新时，先把对象放在不可见地图/坐标，相关客户端 bootstrap 完成后再移动到目标落点；NPC、怪物和地面对象分别断言 fixture 数量与动作后包序列。
- Verification: 跨地图召回测试在无需 KeepAlive 触发业务的情况下收到 NPC 定义/对象、怪物和 ObjectGold。

### 2026-08-12 — 关系提交必须分离 world/auth 锁并同步会话快照

- Symptom: 婚姻和导师初版在持有 `world.mu` 时调用 auth 原子事务与离线角色查询，且 session 的 `gameCharacter` 可能在另一会话提交后用旧整角色状态覆盖最新关系字段；导师经验只改了在线 world，登出可能丢失。
- Root cause: 把在线投影、权威持久状态和连接局部快照当作同一份对象，未定义跨层锁顺序和字段级同步边界。
- Prevention: 关系事务统一由独立 `relationshipMu` 串行化，按 world 快照 → 释放 world 锁 → auth 原子提交/查询 → 重取 world 玩家并字段合并执行；禁止同时持有 world/auth 锁。session 层从 world 同步关系/进度字段，导师临时经验在登出前原子转入 auth，关系功能只合并其拥有的字段。
- Verification: 关系 world/auth 定向测试、双会话 net.Pipe 婚姻/导师完整 transcript、导师登出/到期经验结算和 Go 全量 race 门禁用于验证。
- Strengthening after review: 不得在持有 `world.mu` 时读取 auth 的离线关系记录；先复制角色索引/等级并释放 world 锁，再查 auth，重取 world 玩家并校验快照未漂移后提交。导师奖励必须在 `SaveJSON` 前完成 world/auth 经验变更，登录 bootstrap 严格拆成 Lover → 到期 Chat/MentorUpdate/奖励（或普通 MentorUpdate），到期只在登录检查；学生升级后再执行等级差自动解除。C# 默认 unchecked 的 `uint` 乘法、`long` 加法和 `long → uint` 转换也必须按位宽回绕，不能自行改成饱和运算。

### 2026-08-12 — Go 门禁前检查并清理可重建构建缓存

- Symptom: 新关系网络测试编译时出现 `no space left on device`，系统数据卷仅剩约 204 MiB，而 Go build cache 占用约 5.4 GiB。
- Root cause: 长期多批次 `go test`/race 构建缓存累积，门禁前未检查临时卷余量。
- Prevention: 大批次全量/race 门禁前用 `df -h` 和 `du -sh $(go env GOCACHE)` 检查空间；不足时仅执行 `go clean -cache -testcache` 清理可重建缓存，不删除项目或用户数据。
- Verification: 清理后数据卷恢复约 25 GiB，关系双会话测试重新编译并通过。

### 2026-08-12 — 新增 bootstrap 包必须同步所有 transcript fixture

- Symptom: 加入登录后的 `FriendUpdate` 后，功能测试正确，但大量旧会话测试仍把 `ReceiveMail` 后的下一包写死为物体或 `TimeOfDay`，整包测试集中失败。
- Root cause: bootstrap 是共享、严格有序的协议面；只更新通用 helper 和新功能测试，没有先枚举所有手写启动序列。
- Prevention: 每新增、删除或重排 bootstrap 包，先用 `rg` 搜索前后相邻包 ID，并一次性更新通用状态机与所有显式 expected 序列；随后先跑整个会话 package，再跑全量门禁。
- Verification: 所有 `ReceiveMail` 后的启动序列已加入 `FriendUpdate`，`go test ./cmd/crystal-server -count=1` 通过。

### 2026-08-12 — 邮件 transcript 必须消费发送方删除包与接收方定义包

- Symptom: 带附件的黑名单邮件兼容测试先期望 `MailSent`，实际先收到 `DeleteItem`；接收方先期望 `ReceiveMail`，实际先收到附件的 `NewItemInfo`。
- Root cause: 测试只关注邮件结果，遗漏附件发送的完整可观察包序列。
- Prevention: 邮件 fixture 按是否有附件分别列出发送方 `DeleteItem`/扣费/`MailSent` 与接收方新邮件提示/未见物品定义/`ReceiveMail`；共享世界重登还要先用 barrier 消费加入/离开通知。
- Verification: 黑名单下纯文本与附件两条真实网络路径均通过定向社交测试，附件删除、定义、邮件顺序和持久状态都有断言。

### 2026-08-12 — 源码比对与子任务细节只作为内部过程

- Symptom: 源码逐段比对、子任务完成报告被频繁发送给用户，界面还因已完成代理未及时关闭而呈现持续活动，造成迁移反复汇报、模型来回切换的观感。
- Root cause: 把内部审计证据和每条并行线的原始输出当成了用户必须逐项接收的进度；同时没有在取得结果后立即关闭 completed 子代理，也没有区分线程跨轮次换模与同一时刻的模型路由。
- Prevention: 源码比对结果默认仅用于内部实现和验收；只在整批功能完成、需要用户决策、出现真实阻塞或长时间工作需简短报平安时汇报。取得子代理结果后立即关闭，并在解释模型活动前同时核对主线程当前模型和子线程生命周期。
- Verification: 本次审计确认主线程当前固定为 `gpt-5.6-sol`，Luna 仅存在于历史轮次/已关闭子线程；遗留的 Maxwell、Gibbs 已关闭，当前所有子线程均为 closed，后续不再逐项转发源码比对结果。
- Strengthening after correction: 用户对 Goal/模型状态的询问属于一次性故障排查，不得沉淀为每轮固定报告；除非用户再次询问或发现新的实际异常，后续不主动复述 Goal 数量、模型名称或子线程状态。
- Second strengthening after correction: 用户针对异常截图或某项内部状态的单次询问，只回答当次问题；不得把该分析扩展成持续汇报模板，也不得在后续迁移批次中重复发送相同的代码比对、调度状态或原因分析。验证方式是后续仅汇报整批迁移结果、真实阻塞和必须由用户决定的事项。
- Third strengthening after correction: 用户指出某类内部分析无需每次展示后，立即从所有后续进度模板中移除该项；即使内容更新或截图形式变化，也不能以“新结果”为由再次主动发送。除非用户明确重新询问，否则只在内部用于实现和验收。

### 2026-08-12 — 市场文本必须区分网络快照与 AddItem 后的 UserItem

- Symptom: Market 系统邮件曾输出未格式化的 `7000`，市场 Success/Hint 只清理 ASCII 数字；固定价物品部分合堆时，Go 文本显示原始 `(5)`，原版因 `AddItem` 修改同一对象而显示剩余 `(3)`。
- Root cause: 邮件与会话层各自实现 FriendlyName，且把发送前必须保留的 `GainedItem` 快照错误地同时用于操作后的文本。
- Prevention: FriendlyName 一律先按 Unicode 数字清理尾缀、再移除方括号，金额使用千分位；物品入包前 clone 原始数量，Success/Hint 则使用 AddItem 后的剩余数量。
- Verification: auth 与 net.Pipe 测试分别锁定 `7,000`、全角数字清理、原始 `GainedItem.Count=5` 和成功文本 `(3)`。

### 2026-08-12 — 拍卖到期与 stale Search 必须保留 legacy 生命周期

- Symptom: Go 初版每 500ms 并在每个 Game 请求前处理到期，消除了原版十分钟扫描窗口；已撤回拍品的旧搜索请求返回 reason 7，而原版因仍持有 AuctionInfo 引用返回 reason 3。
- Root cause: 把按请求查询当前 map 的 Go 模型当成了原版全局定时器和连接级对象引用模型，没有验收到期前后及移除后的旧引用。
- Prevention: 服务启动立即扫描一次、随后严格每十分钟扫描，禁止请求前隐式扫描；移除拍品时保留运行期终态 tombstone，使 stale buy 按 Sold→2、其他已移除→3 返回。
- Verification: 定时常量、显式到期处理、stale 撤回会话和 sold/withdrawn auth 测试通过。

### 2026-08-12 — GameShop 必须用不同的 GIndex 与 ItemInfo.Index 锁定怪癖

- Symptom: 既有测试一直令 `GIndex == ItemInfo.Index`，掩盖了原版库存读取用 Info.Index、写入用 GIndex、Stock 包再发 Info.Index，以及每封购买邮件消费两个 MailID 的行为。
- Root cause: fixture 使用相同索引让错误键路径退化成正常字典操作，同时只断言邮件存在，没有断言全局 ID 步进。
- Prevention: GameShop 回归 fixture 固定使用不同索引，分别断言查找键、持久化键、Stock 包键、重复购买覆盖语义和 MailID 2/4 步进；Quantity 1..99 必须先于商品查找校验。
- Verification: auth 与完整会话测试覆盖错位库存、非法数量静默、双 MailID、信用扣款和邮件 transcript。

### 2026-08-12 — 经济事务必须先落盘再做可失败的网络投递

- Symptom: SellNow、到期和 GameShop 成功后若先写在线连接，断线错误可能让已提交经济状态来不及保存；批量邮件遇到一个失效连接也可能短路其他玩家。
- Root cause: 把持久化提交和在线通知当成一个顺序循环，没有区分权威状态与尽力投递的副作用。
- Prevention: 在持锁事务内原子准备/提交货币、物品、拍卖、邮件和 ID，释放锁后先保存 JSON，再按 legacy 顺序投递；批量通知继续遍历，只保留第一个错误。
- Verification: 失败不扣款/不改拍卖、断线后状态保存、多个收件人继续投递以及普通/race 测试通过。

### 2026-08-12 — 会话 fixture 的角色名也必须满足 3–15 字符约束

- Symptom: 新增拍卖定义测试使用 `DefinitionViewer`，角色创建返回 1，定向测试在业务逻辑前失败。
- Root cause: 只检查了账号 ID，没有复用角色名同样存在的长度与字符集约束。
- Prevention: 测试账号和角色名统一使用 3–15 个 ASCII 字母数字，并在 fixture 创建失败时先核对认证枚举与输入长度，再排查目标功能。
- Verification: 改为 `DefViewer` 后嵌套 ItemInfo、市场会话及全量测试通过。

### 2026-08-12 — 同一文件的格式化写入与读取验证必须串行

- Symptom: 本轮曾把 `gofmt -w` 与对相同文件的 `rg` 放进并行调用；另有跨仓库读取曾通过命令链组合，存在读取中间态和混淆失败来源的风险。
- Root cause: 只按降低延迟判断可并行性，没有把格式化视为写操作，也没有保持每个仓库、每个验证步骤的独立 workdir/退出状态。
- Prevention: apply_patch、gofmt 和任何生成操作均串行完成，随后再并行执行互不写入的读取；跨仓库命令各用独立绝对 workdir，禁止用 `&&`、`;` 或管道拼接多个验证步骤。
- Verification: 后续格式化、定向测试、全量 test/race/vet/build 和两个仓库的 diff/status 均用独立调用完成。

### 2026-08-12 — 大型多文件 patch 必须按稳定 hunk 拆分并复读

- Symptom: 一次同时修改 service/economy 的 patch 因最后一个 `removeAuctionLocked` 上下文不匹配而整体拒绝；GameShop 测试 patch 也因手写失败文案与当前源码不同而未应用。
- Root cause: 多文件 patch 依赖较早读取的长上下文，任一末端 hunk 漂移都会让整批失败；工具成功返回也不能证明每个预期字段都已落盘。
- Prevention: 每个文件或语义 hunk 单独 patch，使用函数签名/字段名等短稳定锚点；失败后立即重新读取精确上下文，成功后用 `rg`/`sed` 和 diff 复核，JavaScript 包装只用普通字符串，禁止让未定义值变成 `NaN` 参与 patch 文本。
- Verification: 拆分后 retired 状态、双 MailID、会话测试和文档均正确落盘，`git diff --check` 与全量 Go 门禁通过。
- Strengthening after recurrence: P11 收尾曾把 fishing、EquipSlotItem 和 awakening 三个独立修正放入一个 patch，最后一个 main.go hunk 漂移导致整块拒绝。即使改动属于同一审查批次，也必须按单文件、单语义提交 patch；被拒绝后先确认前面 hunk 均未应用，再逐项重做并立即跑最小定向测试。
- Second strengthening after recurrence: 终端或普通 `sed` 输出的视觉换行不代表文件中的物理行边界；长段落 patch 前先用 `sed -n l` 核对行首和续行，再选择真实存在的短锚点。文档也必须按单文件 patch，避免一个错误行首让其他文件的正确 hunk 一并回滚。本次 Conquest 文档补丁确认无部分写入后按 README、迁移矩阵分别重做，并以 diff 复核落点。

### 2026-08-12 — 买回数量测试必须区分堆叠上限与当前存量

- Symptom: 买回用例用 `Count=9` 配合 `StackSize=1/5`，加入原版堆叠上限门控后 transcript 等待超时；旧实现因缺少门控反而掩盖了 fixture 错误。
- Root cause: 把“超过当前存量时截断”误写成“任意超大数量都截断”；原版先拒绝超过 `ItemInfo.StackSize` 的请求，再把合法范围内超过存量的请求截到存量。
- Prevention: 数量测试同时列出 `request.Count`、`ItemInfo.StackSize`、`stored.Count`；截断场景必须满足 `stored.Count < request.Count <= StackSize`。
- Verification: 买回用例改为 `9 <= StackSize=10`，UsedGoods 用例改为 `3 < Count=4 <= StackSize=5`，并通过完整 net.Pipe transcript。

### 2026-08-12 — 原版物品价格必须按附加属性数值总量计算

- Symptom: 带多个或负值附加属性的 UsedGoods 买回价格可能与原版不同。
- Root cause: Go 价格辅助函数曾按附加属性键数量计算；原版 `Stats.Count` 是每个属性值绝对值之和。
- Prevention: 迁移包含 `Stats` 的物品公式时，先对照 `Stats.Count` 的实现及符号规则，再复用统一的数值总量辅助函数；不要把 map 长度当成属性强度。
- Verification: 增加附加属性买回价格回归测试，`{2, -3}`、数量 2、单价 1000 得到原版价格 3000。

### 2026-08-12 — 背包持久化断言必须按物品身份而非槽位

- Symptom: `[BUYUSED]` net.Pipe transcript 的购买和登出数据都正确，但测试按 `Inventory[0]` 断言，实际物品被放入原版可用背包区的第 7 格而失败。
- Root cause: 把背包内部的自动落位策略误当成了对外行为契约；物品加入逻辑会按物品类别选择合法起始槽位。
- Prevention: 验证跨层物品迁移时按 `UniqueID`、数量、状态和持久化结果断言；只有原版明确固定槽位的 Move/Storage 测试才断言数组索引。
- Verification: 改为扫描持久化背包查找 `UniqueID` 后，`[BUYUSED]`、UsedGoods 合并/刷新及登出测试通过。

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

### 2026-08-11 — apply_patch hunk 的上下文行必须显式标记

- Symptom: 修改 NPC 条件测试时，第一次 patch 因未给上下文行添加空格/加号标记而被工具拒绝。
- Root cause: 手写 patch 时把普通源码行当成了未标记的 hunk 内容，没有遵守 unified diff 的上下文格式。
- Prevention: 通过工具封装 patch 前，所有保留行都以空格开头，删除行以 `-` 开头，新增行以 `+` 开头；patch 失败后先重新读取目标文件，不假设变更已经落盘。
- Verification: 按稳定锚点拆分 patch 后，四个 Go 文件的 diff、gofmt、定向测试和 `git diff --check` 均通过。
- Strengthening after recurrence: 本批首次给 `worldMagicAction` 增加区域动作字段时，patch 数组中的 Go 原文以制表符直接开头，遗漏 unified diff 要求的上下文空格，整个 hunk 被拒绝。今后数组中每条保留源码行必须先加一个字面量空格，再接源码原有缩进；构造后逐行检查除 marker 外的首字符只能是空格、`+` 或 `-`。
- Verification after recurrence: 失败补丁未产生部分修改；区域动作结构改用带显式上下文空格的小 hunk 重做，并在后续读取和最小编译中确认字段只出现一次。

### 2026-08-11 — 会话测试 fixture 必须复用真实桥接签名

- Symptom: Craft net.Pipe 测试初版调用 UpdateCharacterItems 时少传了一个物品网格参数，并把 mapdata.NewOpen 的宽度变量以 int 传入，导致测试包无法编译。
- Root cause: 新测试 fixture 是按记忆拼接 helper 调用，没有先读取现有持久化 API 和地图构造函数的完整签名。
- Prevention: 新增跨层测试 fixture 前先用 rg/源码确认 helper 签名；对 UpdateCharacterItems 明确按 ItemInfos、Inventory、Equipment、QuestInventory 四段传参，地图尺寸在调用边界显式转换为 int32。
- Verification: 修正后 go test ./cmd/crystal-server -run 'TestCraftSession' -count=1 通过。

### 2026-08-11 — 迁移状态必须贯通原版导出器与 Go 持久化桥

- Symptom: Refine 的 Go JSON/运行时状态已经支持 `CurrentRefine`，但原版账户导出器没有输出当前强化物品和收取截止时间，跨数据库迁移会丢失进行中的强化任务。
- Root cause: 只检查了 Go 侧的 JSON bridge 和 logout persistence，没有沿着原版数据库 → .NET exporter → Go JSON → Game stage 的完整链路核对字段与时间单位。
- Prevention: 每个迁移功能都要同时检查原版持久化字段、导出器字段、Go bridge 字段和运行时恢复逻辑；单调计时器必须在导出时转换成跨进程的绝对时间，并验证零值语义。
- Verification: `Crystal.LegacyAccountExport` 已补充 Refine workbench/current item/deadline 导出；当前环境没有 .NET SDK，需在具备 SDK 的环境补跑 exporter 编译，Go 验证继续执行全量 test/race/vet/build。

### 2026-08-11 — Refine 概率断言必须逐项代入公式

- Symptom: Refine 材料测试把成功率期望写成 79，实际实现按原版公式得到 94。
- Root cause: 手算时漏加了幸运项的 5% 和基础成功率的 20%，测试断言没有逐项列出中间结果。
- Prevention: 写强化概率断言前固定列出材料、矿石、幸运、基础四项及最终减项；涉及整数除法和边界材料时逐项核算后再运行测试。
- Verification: 修正期望为 94 后，Refine 定向测试通过；后续继续执行全量 Go 测试与竞态验证。

## Entry format

```markdown
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

### 2026-08-11 — 地图 fixture 必须与 x-major 索引和函数返回值一致

- Symptom: 地图测试首次失败于门索引断言，服务端测试同时出现 `NewOpen` 返回值数量不匹配。
- Root cause: fixture 按错误的 cell 坐标写入门数据，调用点也把返回 `(map, error)` 当成了三值结果。
- Prevention: 先固定并复用 `x*Height+y` 的索引规则；修改构造函数后立即检查所有调用点的返回签名。
- Verification: 修正 fixture 与调用点后重新运行完整 Go 测试、`go vet` 和 `git diff --check`。

### 2026-08-11 — 协议测试必须遵守 Login/Select 状态边界

- Symptom: 新增账户生命周期的 net.Pipe 测试在改密请求处收到 EOF。
- Root cause: 测试在登录成功进入 Select 阶段后发送了只允许 Login 阶段处理的 `ChangePassword` 请求。
- Prevention: 为每个客户端包记录原服务端允许的 `GameStage`，状态机测试按 Version → Login-stage operation → Login → Select 的顺序发送。
- Verification: 将改密请求移到登录前后，重新运行完整 Go 测试、`go vet` 与 `go test -race`。

### 2026-08-11 — Go 配置解析不要在同一作用域重复声明 err

- Symptom: 新增配置加载器后，`go test ./...` 和 `go vet ./...` 在 `var err error` 处报告 `err redeclared in this block`。
- Root cause: 读取文件的短变量声明已经在当前作用域创建了 `err`，解析 INI 时又用 `var err error` 重复声明。
- Prevention: 在同一作用域复用已有错误变量；只有需要缩小作用域时才使用带初始化的局部声明。
- Verification: 修正后重新运行 `gofmt`、`go test ./...`、`go vet ./...` 和 `git diff --check`。

### 2026-08-11 — 时间值进入协议结构前必须显式转换为 .NET binary

- Symptom: 存储密码接线后编译失败，`time.Time` 被赋给协议结构的 `int64 StoragePasswordLastSet`。
- Root cause: auth 层使用语义化的 `time.Time`，而 Crystal wire payload 使用 `DateTime.ToBinary()` 的 64 位值，边界转换遗漏。
- Prevention: 领域层保留 `time.Time`，进入协议结构或 payload 时统一调用 `protocol.DotNetDateTimeBinary`，并为零时间保留 .NET `DateTime.MinValue` 的编码。
- Verification: 修正后重新运行完整 Go 测试、`go vet`、race 测试和构建。

### 2026-08-11 — Go 复合字面量中不能插入局部语句

- Symptom: 角色运行时字段接入后，`gofmt`/编译在 `UserInfo{}` 内的 `hp, mp :=` 处报告缺少逗号和非法语法。
- Root cause: 局部变量声明和条件语句被放进了结构体复合字面量；复合字面量只能包含字段表达式。
- Prevention: 先在字面量外完成默认值和派生值计算，再把最终变量作为字段值传入。
- Verification: 修正后重新运行 `gofmt`、`go test ./...`、`go vet ./...` 和 `git diff --check`。

### 2026-08-11 — 通过脚本封装 apply_patch 时必须处理 Markdown 反引号

- Symptom: 本轮更新 NPCAction 模型时，包含 JSON struct tag 的 patch 又因使用 JavaScript 模板字符串承载反引号而未执行。
- Root cause: 预防规则只在新增 lessons 时落实，代码 patch 仍沿用了模板字符串；工具封装层和源码问题没有区分处理。
- Prevention: 所有包含 struct tag、Markdown 或 raw-string 内容的 patch 都禁止模板字符串；统一用普通字符串数组拼接，或拆成不含反引号的稳定锚点 patch，并在工具返回后检查目标文件。
- Verification: 改用稳定锚点 patch 后 NPC 消息动作实现成功，parser/端到端定向测试、Go 全量 race/vet/build、文档差异检查均通过。

### 2026-08-11 — .NET 工具必须单独标注未编译验证

- Symptom: 新增 `Crystal.LegacyWorldExport` 或修改 `Crystal.ProtocolProbe` 后尝试执行 .NET 构建，当前环境没有 `dotnet`，命令输出 `dotnet unavailable`。
- Root cause: 运行环境只具备 Go 工具链，不能把 .NET 项目静态检查当成真实编译验证。
- Prevention: 提交前先探测 `dotnet`/`csc`/`mcs`；若均不可用，记录 exporter 与 probe 的未验证边界，并在有 .NET 8 SDK 的环境补跑两者及现有客户端探针。
- Verification: Go 的 `test`、`race`、`vet`、`build` 与差异检查通过；本轮怪物掉落/default NPC exporter 仍因同一环境限制未编译，.NET exporter 和 ProtocolProbe 保留为待 SDK 环境验证项。

### 2026-08-11 — 地图格式修正必须同步 fixture 的真实记录步长

- Symptom: 按原版 `LoadMapCellsV100` 修正 v100 为 27 字节单元后，旧测试仍把 fishing 字段写在 30 字节偏移，地图测试失败。
- Root cause: fixture 是按迁移代码的旧猜测构造的，没有与 .NET 字段偏移和记录步长绑定。
- Prevention: 每种地图格式先从原版 loader 固定 header、字段偏移、记录步长，再生成最小 fixture；修正偏移时同时更新正向和截断数据测试。
- Verification: fixture 修正后重新运行 mapdata 全量测试，并继续执行 Go 全量测试、race、vet、build 和 diff 检查。

### 2026-08-11 — 传送包必须按目标类逐字段对照而非复用相似包

- Symptom: 编写 NPC 传送的 MapChanged payload 初稿时误用了 MapInformation 的天气标志布局，并遗漏了目标类末尾字段；差异审查在测试前发现。
- Root cause: 两个包都包含地图元数据，但 .NET MapChanged 的字段顺序是 Lights + Location + Direction + MapDarkLight + Music + Weather，不能直接套用 MapInformation 的 flags。
- Prevention: 为每个新包从原版 packet 类的 WritePacket 逐字段列出 payload 表，再实现独立 serializer；不要以名称或相邻包推断布局，并为长度、字符串、尾字段和端到端包序列各加断言。
- Verification: 修正后 TestNPCTransportAndTeleportPayloadsMatchLegacyLayout 与 NPC 传送 net.Pipe 测试通过，随后 go test -race ./... 通过。

### 2026-08-11 — NPC 脚本 fixture 必须保留真实换行边界

- Symptom: 构造脚本 JSON/net.Pipe fixture 时曾把换行转义成字面量反斜杠，解析器会把多个 page 当成一行。
- Root cause: 测试字符串经过工具封装层二次转义，未在写入后检查 Go 源码中的实际转义层级。
- Prevention: 脚本 fixture 统一使用 Go 字符串的单层反斜杠 n，并在测试中断言 page 数量、文本顺序和按钮目标，不只断言请求成功。
- Verification: 增加 TestParseNPCScriptTextAndButtons 和脚本端到端 page 测试；普通/race 全量 Go 测试通过。

### 2026-08-11 — Packet ordinal 必须从完整 enum 锁定

- Symptom: 对照怪物 packet 时发现已有 Go 的 NPC、Player、Storage 等常量因遗漏原版 enum 成员而整体偏移，测试虽然通过但真实客户端会把 packet 解释成别的类型。
- Root cause: 只按相邻名称或记忆手写 ordinal，没有从 `Shared/Enums.cs` 的完整 `ServerPacketIds`/`ClientPacketIds` 顺序核对。
- Prevention: 每条新增 packet 先从完整 enum 计算 ordinal；Go 协议包测试必须包含关键 ID 的显式 legacy ordinal 断言，禁止只使用 Go 常量自洽测试。
- Verification: 修正 `ObjectPlayer`、`NewMonsterInfo`、`NewNPCInfo`、`ObjectMonster`、`ObjectNPC`、NPC/Storage 以及 Monster/NPC 请求 IDs，并通过 `TestPacketIDsMatchLegacyEnums` 与全量 Go 测试。

### 2026-08-11 — 新增 packet 布局测试必须先按字段宽度核算长度

- Symptom: 新增 `ObjectAttack` payload 测试把长度写成 20，测试实际序列化出 16 字节而失败。
- Root cause: 把字段数量误当成了额外的长度，未按 `uint32 + int32 + int32 + 4*byte` 逐字段求和。
- Prevention: 每个 packet 先从 legacy `WritePacket` 列出字段类型和顺序，手算并在测试中同时断言长度与关键偏移；不要凭相邻 payload 估算长度。
- Verification: 修正为 16 字节后，协议测试通过；提交前继续运行 `go test ./...`、`go test -race ./...`、`go vet ./...` 和 `git diff --check`。

### 2026-08-11 — 新增 packet ordinal 测试必须同步 wants 与 got（强化）

- Symptom: 门 packet ordinal 测试加入 legacy 期望后，测试先报告 `ServerOpendoor = 0`；同一阶段新增 `ServerWorldMapSetup` 时又复现了 `= 0`。
- Root cause: 新 ID 被分两次 patch 追加，第二次只更新了期望表，遗漏实际值表，map 缺失项被 Go 的零值掩盖。
- Prevention: 每新增一个 ordinal，必须在同一个 patch 中成对更新 legacy `wants` 与 Go `got`，两侧都保留显式 legacy 数值；patch 后立即运行 ordinal 测试，再继续写功能代码。
- Verification: 补齐 `got["ServerOpendoor"]` 和 `got["ServerWorldMapSetup"]` 后，协议、地图和服务端测试重新通过。

### 2026-08-11 — 移动阻挡测试必须验证实际目标格

- Symptom: 门自动关闭测试把玩家放在门内，随后向远离门的方向移动，却期望移动被门阻挡。
- Root cause: 测试场景没有先核对方向对应的目标坐标；关闭的门只阻挡进入带门的目标格，不阻挡离开该格。
- Prevention: 编写移动/碰撞断言时先画出起点、方向、步数和每个中间目标格；门阻挡用“门外进入门格”场景验证，离开门格单独验证可通行。
- Verification: 将玩家恢复到门外并尝试进入关闭门格后，门状态单测通过。

### 2026-08-11 — Go 测试 helper 作用域必须按 package 检查

- Symptom: 地图查询测试在 `cmd/crystal-server` 编译时找不到 `readTestDotNetString`，同时缺少该测试自己的二进制解析 import。
- Root cause: 把 `internal/protocol` package 的测试私有 helper 当成了 `main` package 可复用 API；Go 测试文件之间只有同 package 标识符可见，`*_test.go` helper 不会跨 package 导出。
- Prevention: 新增测试先确认 package 声明和 helper 所在目录；跨 package 只调用公开函数，必要时在当前 package 定义带 `t.Helper()` 的本地适配器。
- Verification: 改为调用公开 `protocol.ReadDotNetString` 并补齐 `encoding/binary` 后，地图协议测试编译通过。

### 2026-08-11 — 复合协议包必须覆盖外层字段

- Symptom: 地图信息测试能正确解析标题和地图内容，但对照 `NewMapInfo.WritePacket` 时发现 Go payload 遗漏了开头的 `MapIndex`；真实客户端会从错误的偏移读取标题。
- Root cause: 只验证了复用的 `ClientMapInfo.Save` 内容，没有把外层 packet 类自己的字段纳入 payload 对照。
- Prevention: 每个复合 packet 先分别列出 packet 外层字段和嵌套对象字段；测试从第一个字节开始解析完整 payload，并断言外层索引、长度和嵌套字段。
- Verification: `NewMapInfoPayload` 现在先写入 `MapIndex`，协议、world 和 net.Pipe 测试均从完整 payload 解析并通过。

### 2026-08-11 — 地图切换必须持久化 CurrentMapIndex

- Symptom: 跨地图后只更新角色坐标，logout 再登录时坐标会被解释为旧地图坐标，角色回到错误地图。
- Root cause: 原有 `UpdateCharacterRuntime` API 只接受 x/y/direction，没有把地图索引作为同一份运行时位置状态保存。
- Prevention: 所有会改变地图的路径使用 map-aware runtime 更新接口；普通坐标更新接口保留给不改变地图的旧调用，并为 map index 单独断言。
- Verification: 增加 `TestUpdateCharacterMapRuntimePersistsMapIndex`，地图 movement net.Pipe 测试使用新接口更新运行时状态，Go 全量测试通过。

### 2026-08-11 — 地图切换可见性必须双向刷新

- Symptom: 传送后的玩家会被发送给目标地图观察者，但当前玩家没有收到目标地图已有玩家对象。
- Root cause: 只实现了旧地图 `ObjectRemove` 和当前玩家向新地图广播，遗漏了目标地图已有对象的 `GetObjects` 对等行为。
- Prevention: 地图切换验收同时断言旧观察者收到移除、新观察者收到当前玩家对象、当前玩家收到新地图已有玩家对象，并在静态对象刷新前完成玩家对象同步。
- Verification: transition world 状态测试锁定 old/new map 集合，net.Pipe 路径发送双向 player packet；全量 Go 测试通过。

### 2026-08-11 — 战斗 transcript 期望必须先算伤害终态

- Symptom: 远程攻击单测把 3 点生命、4 点攻击、1 点护甲的场景期望为未击杀，实际 `damage - armour == HP` 触发了 `ObjectDied`，测试失败。
- Root cause: 测试只按“远程命中”直觉填写 `Killed` 和通知数量，没有先按当前确定性伤害公式核算 HP 终态。
- Prevention: 战斗测试先列出攻击力、护甲、有效伤害、初始/最终 HP，再决定 struck、damage、death packet 序列；通知数量必须与该终态一致。
- Verification: 修正远程 transcript 断言为四包并重新运行 Go 全量测试通过。

### 2026-08-11 — awk 枚举核对变量不要使用保留字

- Symptom: 用 awk 计算原版 packet enum ordinal 时，命令因 `in` 变量名触发语法错误而未执行。
- Root cause: `in` 是 awk 的语法关键字，不能作为普通变量名；这是核对脚本错误，不是项目源码错误。
- Prevention: 编写 awk 枚举脚本时使用 `inside` 等非保留变量名，并先用最小命令验证脚本能运行，再依赖其输出修改协议常量。
- Verification: 改用 `inside` 后成功核对 `MagicKey=57`、`Magic=58`、`NewMagic=117`、`Magic=120`、`MagicDelay=121`、`MagicCast=122`、`ObjectMagic=123`，随后以原版 enum 数值为依据实现测试。

### 2026-08-11 — apply_patch 上下文必须重新读取精确空格

- Symptom: 批量加入魔法 packet ordinal 的 patch、随后给 `world.go` 增加字段的 patch、更新迁移矩阵的 patch、本轮加入 Chat ordinal 的 patch、本轮更新地图 gate 文案的 patch，以及本轮首次加入 GainExperience ordinal 的 patch，都因实际对齐空格或换行与手写上下文不一致而未应用。
- Root cause: patch 上下文包含了脆弱的列对齐空格，未先读取目标文件的精确文本；本轮 `HealthChanged` 表格与迁移矩阵文案再次触发同一问题。
- Prevention: 修改已有表格、结构体或文档段落时先用 `rg`/`sed -n l` 核对精确上下文，再拆成以稳定字段名或单行句子为锚点的小 patch；patch 失败后不继续假设文件已变更，并在同一轮重复失败时改用更小的锚点。对代码和文档分别应用、分别检查 `git diff`。
- Verification: 本轮重新读取 `packet_test.go`、迁移矩阵和 README 后按稳定行锚点分块应用，GainExperience 协议、NPC parser、经验表和定向端到端测试通过；随后继续执行全量校验。

### 2026-08-11 — net.Pipe 测试必须先消费副作用包

- Symptom: NPC `ENTERMAP` 端到端测试在传送后直接发送 keep-alive，测试挂起；第一次断言还在服务端完成持久化前读取了角色状态。
- Root cause: `net.Pipe` 没有缓冲，地图切换后的静态对象移除包尚未被客户端消费时，服务端会阻塞写包；读取响应包也不等于 handler 已完成后续副作用。
- Prevention: 为每条 net.Pipe transcript 列出完整发送顺序，先消费 `ObjectRemove` 等副作用包，再用后续 keep-alive/响应建立处理完成屏障后检查持久化状态。
- Verification: `TestSessionNPCEnterMapConsumesNeedMoveDestination` 补齐旧地图 NPC 移除断言和 keep-alive 屏障后，定向 Go 测试通过。

### 2026-08-11 — 饱和余额测试必须先算每一步终态

- Symptom: NPC 货币单测在余额已经饱和后仍用 `TakeGold(10)` 期望扣除整笔余额，断言得到 10 而不是预期的最大值。
- Root cause: 测试把“请求超过余额时钳制”与“请求值为 10”混为一谈，没有按初始余额、饱和增益和实际扣款请求逐步计算终态。
- Prevention: 对带上限/下限的经济操作先列出每个操作后的余额，再选择确实超过余额的请求验证钳制；packet amount 也必须断言实际变化量。
- Verification: 将扣款请求改为 `uint32` 最大值后，NPC 货币 transcript 和 auth 余额单测通过。

### 2026-08-11 — LevelUp transcript 必须保留 HP/MP 中间状态

- Symptom: `CHANGELEVEL` 端到端测试收到第一条 `HealthChanged(100, 100)`，原版顺序应为先 `SetHP` 的 `(100, 20)`，再 `SetMP` 的 `(100, 100)`。
- Root cause: 实现先把 HP/MP 都更新为最终值，再序列化第一条副作用包，丢失了 `SetHP` 与 `SetMP` 之间的可观察状态。
- Prevention: 对连续副作用逐步记录旧状态、每一步新状态和 packet 顺序；只有完成 transcript 写出后才用最终状态作为后续动作基线。
- Verification: 修正为使用旧 MP 构造第一条包后，`TestSessionNPCScriptChangeLevelAction`、健康动作定向测试、观察者通知测试和协议布局测试通过；随后 `go test ./...`、`go test -race ./...`、`go vet ./...`、`go build ./...` 与 `git diff --check` 全部通过。

### 2026-08-11 — 经验状态抽取后必须检查残留局部变量

- Symptom: 将经验升级逻辑抽取到共享 helper 时，初版 patch 在 world 状态写回处残留了已不存在的局部变量引用，静态复查在测试前发现。
- Root cause: 用宽泛上下文替换相似代码时命中了相邻的 CHANGELEVEL 状态写回，未立即检查 helper 调用后的变量来源。
- Prevention: 抽取状态计算函数后，立即逐行核对 result 的 Level、Experience、HP、MP 写回点，并在继续扩展功能前运行 gofmt、定向编译和单元测试。
- Verification: 修正为使用计算结果的 Level 后，经验表、世界状态、NPC parser、net.Pipe transcript 和 go test ./... 全部通过。

### 2026-08-11 — 条件 NPC transcript 必须显式设置角色前置状态

- Symptom: 条件 NPC 端到端测试在第二次调用时仍收到 ELSE 文本，成功分支没有被选中。
- Root cause: fixture 只设置了金币，却忘记 CreateCharacter 的默认等级是 1；测试场景期望等级条件 LEVEL >= 2 成功。
- Prevention: 每个条件 transcript 在启动会话前显式写入并断言等级、金币、性别、职业、地图和坐标等前置状态，不依赖创建默认值。
- Verification: 显式写入等级 2 后，低金币 ELSE 和满足等级/金币 SUCCESS 两条 net.Pipe 路径均通过，且 race 全量测试通过。

### 2026-08-11 — 跨功能测试 patch 必须使用唯一行为锚点

- Symptom: 更新 FireBall 延迟/MP transcript 时，通用的 `Damage != 4` 上下文先误改了相邻的近战测试；本轮修改远程 fixture 的同名 HP 行又误改了 melee fixture。
- Root cause: 相似断言或 fixture 行跨多个测试重复出现，patch 没有把 `world.magicAttack`/`world.rangeAttack` 或测试函数名作为唯一锚点。
- Prevention: 修改相似测试时先用测试函数名和调用函数组成双重上下文；同一常量在多个 fixture 出现时必须把函数头、地图尺寸和调用一起纳入 patch；patch 后立即用 `git diff --` 检查命中位置，再运行最小定向测试。
- Verification: 恢复近战 fixture、按 `TestGameWorldRangedAttackUsesTargetPacketAndDamageTranscript` 唯一上下文重做后，远程命中/Miss 定向测试、`go test ./...`、race、vet 和 build 均通过。

### 2026-08-11 — 协议类型命名必须先避开已有 packet 常量

- Symptom: 新增客户端魔法资料结构时，Go 编译器报告 `ClientMagic (constant) is not a type` 和同名类型重复声明。
- Root cause: `internal/protocol` 已经用 `ClientMagic` 表示客户端 packet ordinal，却在同一包中再次使用该标识符声明 wire 数据类型。
- Prevention: 新增协议领域类型前先检索同包常量、函数和类型名称；packet ID 保留 legacy 名称，数据结构使用 `ClientMagicInfo` 等不冲突的明确后缀。
- Verification: 重命名后定向协议/auth/world 测试、Go 全量测试、race、vet 和 build 全部通过。

### 2026-08-11 — apply_patch 目标路径必须先验证仓库根目录

- Symptom: 新增协议测试时误把目标路径写成仓库外的相似目录，工具实际创建了一个临时文件，正确仓库没有变化。
- Root cause: 手写绝对路径时重复了目录片段，调用前没有用当前仓库根目录和目标文件存在性做交叉确认。
- Prevention: 生成 patch 前先执行 `pwd`/`git rev-parse --show-toplevel`，对新增文件逐项检查目标路径；patch 后立即检查 `git status --short` 和目标文件绝对路径，禁止凭工具成功返回推断命中正确仓库。
- Verification: 删除误创建的仓库外文件，按正确路径重新创建测试，并通过定向协议/world 测试和 diff 检查。

### 2026-08-11 — 同级迁移仓库路径必须先从实际目录发现

- Symptom: 继续 Go 迁移时先假定 Go 项目位于原仓库内的 `Crystal-Go`，本轮竞态命令又因手写成重复的 `Dropbox/Dropbox` 而未启动。
- Root cause: 跨仓库命令没有在执行前复用已核对的绝对根目录，路径依赖手写和历史命名假设。
- Prevention: 涉及多个仓库时，先发现 `go.mod` 并执行 `git rev-parse --show-toplevel`；将输出的绝对根目录作为后续所有命令的唯一 `workdir`，不手写拼接路径。
- Verification: 使用已确认的 `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer` 重跑后，格式化、Go 全量测试、竞态测试、`vet`、构建和 `git diff --check` 均通过。

### 2026-08-11 — apply_patch JavaScript 封装必须逐行校验 patch 标记

- Symptom: 通过 `functions.exec` 组装 patch 时，多次因未引用 `@@`/`*** End Patch` 或错误的数组行出现 JavaScript 语法错误；本轮 GreatFireBall/ThunderBolt 文档和测试 patch 又重复了同类失败。
- Root cause: patch 文本和 JavaScript 源码共用一层封装，工具标记没有作为字符串逐行传入；修正后仍有调用使用了未闭合或未引用的 marker。
- Prevention: 使用数组拼接 patch 时，`*** Begin/End Patch`、每个 `@@`、每个上下文行都必须是独立字符串；先完成 `const patch = [...].join("\\n")` 的语法检查，再调用工具，失败后改用更小 patch，不把工具返回当成源码状态。
- Verification: 将配置、world、远程/魔法测试和文档拆成小 patch 后，目标文件 diff 与 `git diff --check` 均正确；`go test ./...`、race、vet、build 全部通过，GreatFireBall/ThunderBolt 提交为 `46732e4`。

### 2026-08-11 — Markdown patch 必须核对目标文件和字符串行

- Symptom: 本轮文档 patch 曾因 JavaScript 数组行误写成一元 `+` 表达式而报 `NaN`/语法错误，也曾把迁移矩阵段落误作为 README 上下文而无法应用。
- Root cause: patch 封装层的字符串标记没有逐行复核，且相似文案没有在修改前用文件名和行号确认唯一目标。
- Prevention: Markdown patch 使用双引号数组逐行拼接，避免把 `+` 放在字符串外；调用前逐项检查每个 hunk 行都以空格、`+` 或 `-` 开始，新增行不得遗漏 `+`；每次只修改一个文件，先用 `nl -ba`/`sed -n l` 核对完整行，再按文件分别检查 `git diff`。
- Verification: README 与 `docs/migration-matrix.md` 已分别按精确上下文更新，`git diff --check` 和 Go 全量测试通过；本轮复发后已强化 hunk 首字符检查规则。
- Strengthening after recurrence: 删除 Markdown 列表项时，patch 行必须以两个连字符开头（第一个是 diff 删除标记，第二个是文档列表标记）；上下文列表项必须以空格再接连字符开头。应用前按“patch 标记 + 文档内容”两层机械检查。

### 2026-08-11 — 多观察者战斗 transcript 必须展开接收者数量

- Symptom: MeteorShower 测试第一次把三个目标的命中包期望为 6 条，实际收到 12 条。
- Root cause: 每个目标的 `ObjectStruck`/`DamageIndicator` 会分别发送给范围内的施法者和观察者，测试只按目标数计算，没有乘以接收者数。
- Prevention: 编写广播型战斗 transcript 时先列出目标数、每目标包数和每个范围内接收者，再按发送顺序断言总数与关键 payload；私有包和广播包分开计算。
- Verification: 修正 MeteorShower 期望为 12 条后，定向测试通过，并将继续运行 race/vet/build 全量校验。

### 2026-08-11 — 怪物 fixture 不应假设 spawn 输入顺序

- Symptom: ThunderStorm/FlameField 测试把 object ID 2 当作亡灵，实际读取到的是位于 `(3,0)` 的普通 Goblin。
- Root cause: `loadMonsters` 会按 `MapIndex`、`RespawnIndex`、`MonsterIndex` 和坐标排序后再分配 object ID，输入 slice 顺序不是稳定的对象身份契约。
- Prevention: 生成怪物 fixture 后按坐标、`Info.Undead` 或其他领域属性定位目标；只有在 loader 明确保证顺序时才断言 object ID。
- Verification: 测试改为按坐标和 `Undead` 属性查找目标，`TestGameWorldThunderStormAndFlameFieldAreaRules` 通过。

### 2026-08-11 — 测试 helper 调用必须核对完整函数签名

- Symptom: 修正怪物 fixture 后，测试编译失败，`monstersNear` 实际需要四个参数但调用只传了三个。
- Root cause: 修改测试时只记住了查询坐标和距离，遗漏了函数签名中的 `mapIndex` 参数。
- Prevention: 复用测试 helper 前先检索函数声明和现有调用，按参数名称逐项核对地图、坐标和范围；修改后立即运行定向编译测试。
- Verification: 补齐 `mapIndex` 参数并执行 `gofmt` 后，ThunderStorm/FlameField 定向测试通过。

### 2026-08-11 — MagicInfo 伤害期望必须逐项代入公式

- Symptom: IceStorm 测试把 `MC=4`、`MPowerBase=12`、`PowerBase=14` 的 level-0 伤害写成 19，实际结果为 21。
- Root cause: 手算时漏加了 `round(MPowerBase/4)=3` 的完整项，沿用了另一条魔法的旧期望。
- Prevention: 每个魔法 fixture 先从当前 `MagicInfo` 记录列出基础 MC、MPower、Power、倍率、护甲和 MP cost，再逐项计算伤害与最终 HP；不同 spell 不复用相似数字。
- Verification: IceStorm 期望修正为 21、目标有效伤害 20 后，定向测试通过，随后继续执行全量验证。

### 2026-08-11 — Markdown hunk 标记复发后的机械检查

- Symptom: FireBang/IceStorm 文档 patch 再次出现新增行遗漏 `+`，并因相似段落上下文造成重复文案。
- Root cause: 仅靠人工阅读数组内容，没有在调用前校验每个 hunk 行的首字符和目标文件中的唯一锚点。
- Prevention: apply_patch 前逐行断言 hunk 行首字符属于空格、`+`、`-`，并对同一段文案执行 `rg` 计数，确认目标文件只命中一次。
- Verification: 清理迁移矩阵重复段落后，README/矩阵差异检查和 Go 全量测试通过。

### 2026-08-11 — world 通知的指针和值边界必须显式处理

- Symptom: Healing action 编译失败，`worldPlayer` 指针不能直接赋给 `worldNotification.Recipient` 的值类型字段。
- Root cause: 世界对象 map 保存 `*worldPlayer`，而通知快照刻意保存值副本；新增通知路径遗漏了显式解引用。
- Prevention: 读取对象 map 后先确认 API 需要指针还是快照值；进入通知、排序或跨锁返回值时统一使用显式 `*player` 副本，并在编译后检查接收者身份。
- Verification: 改为 `Recipient: *target` 后，Healing 定向测试、Go 全量测试、race、vet 和 build 均通过。

### 2026-08-11 — world 测试必须显式投递返回的通知

- Symptom: Healing 的 world tick 已返回健康通知，但测试中的 packet capture 为空。
- Root cause: `world.magicAttack`/`world.tick` 只构造并返回 `worldNotification`，不会自动调用 `deliverWorldNotifications`；测试只检查返回 slice，没有执行投递层。
- Prevention: 需要验证实际客户端包时，先断言 world 返回的通知 transcript，再显式调用 `deliverWorldNotifications`，最后检查 Send capture；不要把返回通知误认为已经写入连接。
- Verification: 补齐 cast/impact 两段通知投递后，目标/施法者 packet 顺序断言通过，且全量 race 测试通过。

### 2026-08-11 — functions.exec 封装 patch 前必须先校验字符串和工作目录

- Symptom: 本轮协议、auth、world 和 main 的多个 patch 曾因 JavaScript 数组中遗漏字符串引号、未引用 patch 结束标记或把加号写成数组外表达式而未执行；一次检索还因 workdir 重复目录导致进程无法创建。
- Root cause: patch 文本、JavaScript 语法和跨仓库绝对路径同时手写，没有在调用前做逐行 hunk 标记检查、JavaScript 语法检查和仓库根目录复核。
- Prevention: 组装 patch 时所有 Begin/End、@@、上下文和新增行都必须是独立字符串；调用前检查每个 hunk 行首字符属于空格、加号或减号，并复用已确认的绝对仓库根目录；工具失败后立即检查目标文件，不把失败返回当成部分成功。
- Verification: 重新按数组字符串逐行修正后，protocol/auth/world/main 修改均落在 Go 仓库，gofmt 与 protocol/auth/world 定向测试通过；后续继续执行全量检查。
- Strengthening after recurrence: `*** End Patch` 也必须是数组中的已引用字符串，不能落在 `.join()` 调用之后成为 JavaScript token；构造后先机械确认首项为 Begin、末项为 End，再调用工具。本次 Conquest auth 补丁在执行前被 JavaScript 解析拒绝，源码无部分修改，已改为单文件小补丁重做。

### 2026-08-11 — PvP 友方规则必须保留 legacy 的严格时间和空公会语义

- Symptom: 复核攻击模式迁移时发现 Red/Brown 在 `BrownTime` 恰好到期时被 Go 判为友方；EnemyGuild 对无公会施法者也被错误判为非友方。
- Root cause: 用 `!now.Before(...)` 近似了 legacy 的严格 `Envir.Time > BrownTime`，并把 `Guild.IsEnemy(null)` 的 false 语义简化成了双方必须有公会。
- Prevention: 对照原版条件逐运算符迁移（`Before`/`After` 保持严格边界），并为 null guild、同 guild、敌对 guild、非敌对 guild 分别建立 truth table 和测试。
- Verification: 增加 BrownTime 相等时刻、敌对公会、非敌对公会和无公会 ally 测试；Go 定向测试通过，提交前继续运行 race、vet、build 和 diff 检查。

### 2026-08-11 — net.Pipe 广播必须并发消费所有接收者

- Symptom: 玩家 PvP 端到端 transcript 在服务端广播攻击和死亡包时挂起。
- Root cause: `net.Pipe` 没有缓冲；服务端向多个连接顺序写广播包时，尚未被读取的接收者会阻塞后续写入，单独消费一个连接无法推进整个广播。
- Prevention: 为每个 net.Pipe transcript 列出所有接收者和完整包序列；涉及广播时为每个连接启动并发 reader，或先建立等价的消费屏障，再等待 handler 完成。
- Verification: `TestSessionPlayerMeleePvPTranscript` 并发消费目标连接的 7 个广播包，PvP 定向测试通过；提交前继续执行 race 测试。

### 2026-08-11 — 重复测试配置必须用函数名锚定 patch

- Symptom: 修复 PvP transcript 的 timeout 时，非唯一上下文曾误改已有可见性测试的 `cfg.TimeOut`。
- Root cause: 多个测试包含相同的 `cfg.AllowStartGame`/`cfg.TimeOut` 行，patch 没有把测试函数名和 fixture 一起作为定位条件。
- Prevention: 修改重复配置时必须把完整测试函数头、关键 fixture 或调用 API 放入 patch 上下文；patch 后先看 `git diff --unified=0` 和所有同名配置行，再运行定向测试。
- Verification: 已恢复可见性测试原有 2 秒 timeout，仅保留 PvP transcript 的 30 秒 timeout，并重新通过定向测试。

### 2026-08-11 — apply_patch 上下文行首字符必须保留（强化）

- Symptom: 本轮同步 NPC 升级上限时，第一次 patch 把代码上下文行写成了没有前导空格的字符串，工具拒绝整个 hunk；第二次又因文档行未按实际完整句子定位而失败。
- Root cause: 组装 patch 数组时把 hunk 的上下文语义误当成普通源码文本，且没有先读取目标行的完整前后文；失败后还同时修改多个文件，扩大了定位范围。
- Prevention: patch 数组中每个上下文行必须显式以一个空格开头，新增/删除行分别以 `+`/`-` 开头；先用 `nl -ba` 或 `sed -n l` 读取精确行，再按单文件、单行为锚点拆 patch，失败后检查 diff 再重试。
- Verification: 拆分为代码 patch 和文档 patch 后，NPC 会话上限同步及 README/迁移矩阵措辞均正确落盘；后续用 `gofmt`、定向测试和 `git diff --check` 验证。

### 2026-08-11 — P6 跨仓库目标和 patch 上下文必须双重确认

- Symptom: 继续 P6 物品迁移时，曾因手写相似仓库路径把 patch 目标指向错误位置；本轮物品 ordinal patch 又因上下文行缺少 patch 必需的前导空格而被拒绝。
- Root cause: 跨仓库编辑没有把已确认的绝对仓库根目录和 apply_patch hunk 语法作为调用前的机械检查项。
- Prevention: 修改前先用 `git rev-parse --show-toplevel`/目标文件存在性确认仓库；patch 数组逐行检查 Begin/End、`@@` 和上下文首字符，失败后立即查看目标仓库 `git status` 与 diff。
- Verification: 物品协议测试和文档最终都落在 `Crystal.GoServer`，原仓库无误写；ordinal 定向测试、Go 全量校验和 `git diff --check` 作为提交前门禁。

### 2026-08-11 — 物品槽位 transcript 必须逐步计算交换与空位

- Symptom: `MoveItem(0→2)` 后的 P6 端到端测试把原槽 1 物品期望在槽 0，导致 logout 持久化断言失败。
- Root cause: 只看“移动到目标槽”的直觉，没有按 legacy `array[to] = array[from]`、`array[from] = oldTarget` 逐步推导后续 split 空位和 merge 槽位。
- Prevention: 物品事务测试先画出每一步的槽位、UniqueID、Count，再断言 split 产生的新槽和 merge 后的空槽；不要只按请求文字推断终态。
- Verification: 修正 transcript 后，Move/Split/Merge 的 net.Pipe 响应顺序和 JSON logout 持久化状态通过。

### 2026-08-11 — 装备条件比较必须显式统一整数类型

- Symptom: 新增装备解析时，Go 编译器拒绝 `byte` 的 `RequiredAmount` 与 `uint16` 的角色等级直接比较。
- Root cause: 协议字段保留 legacy 的 `byte` 宽度，领域角色等级使用 `uint16`，边界比较遗漏了显式转换。
- Prevention: 从 wire 读取的窄整数进入等级/计数比较前统一转换到领域类型，并在新增条件分支后立即运行定向 `go test`。
- Verification: 将候选装备等级转换为 `uint16` 后，协议与服务端装备测试通过。

### 2026-08-11 — 装备 fixture 必须填充 legacy 默认关联字段

- Symptom: 装备单测中的物品明明类型、职业和性别都匹配，却被 `WeddingRing != -1` 规则拒绝。
- Root cause: Go 零值 `WeddingRing=0` 不等于 legacy `UserItem` 构造器的默认 `-1`，测试 fixture 没有显式还原该默认值。
- Prevention: 构造现代 `StoredItem` fixture 时显式设置所有非零语义默认值，尤其是 `SoulBoundID=-1`、`WeddingRing=-1`、空 sockets/awakening 与耐久度。
- Verification: 补齐默认字段后，装备/卸下限制、属性刷新和会话 transcript 均通过。

### 2026-08-11 — nil 物品格与空物品格必须区分

- Symptom: 会话装备请求返回失败；原因是持久化的 equipment slice 是非 nil 空数组，默认容量补齐函数只对 nil 生效。
- Root cause: legacy 14 格默认语义在 Go 中由 `nil` 触发，空 slice 表示一个真实的零长度格子，不能混同为缺省值。
- Prevention: 对需要默认容量的导入数据传递 nil 或显式补齐 14/46/40 格；测试同时覆盖 nil、空 slice 和正确容量三种输入。
- Verification: 会话 fixture 改为 nil equipment/quest grids 后，Equip/Remove/Logout transcript 与持久化断言通过。
- Strengthening after recurrence: auth 领域事务测试若要验证合法背包槽，也必须显式创建 Legacy 的 46 格 Inventory；新建角色的 nil/零长度 Inventory 不代表可用空背包。否则合法取出会被正确判成目标越界，容易被误判为事务实现问题。

### 2026-08-11 — 装备阶段跨仓库 patch 目标复发后必须先锁定根目录

- Symptom: 在原 Crystal 仓库工作目录下首次补协议时，patch 错把目标写成 `internal/protocol/item_actions.go`，未命中同级 Go 仓库。
- Root cause: 已知 Go 仓库是同级目录，却没有在 apply_patch 调用中使用已验证的 `../Crystal.GoServer/...` 目标。
- Prevention: 每次跨仓库编辑前执行 `git rev-parse --show-toplevel`，再把同一绝对根目录转换为 apply_patch 的相对目标；工具失败后立即检查两仓库 status。
- Verification: 改用 `../Crystal.GoServer/...` 后所有装备源码、测试和文档均落在 Go 仓库，原仓库没有误写代码。

### 2026-08-11 — 装备 patch 仍需机械校验 hunk 标记

- Symptom: 插入装备会话 case 和测试循环时，JavaScript patch 封装曾因未引用 `*** End Patch`，以及上下文行缺少前导空格而整块拒绝。
- Root cause: patch 标记、上下文语义和 JavaScript 字符串层级同时手写，调用前没有逐行校验。
- Prevention: 所有 patch 行先放入已引用的字符串数组；逐行确认 Begin/End、`@@`、上下文空格和新增/删除标记，再调用工具并查看 diff。
- Verification: 重做小块 patch 后 main handler、会话测试和 `git diff --check` 均通过。

### 2026-08-11 — UseItem/DeleteItem transcript 必须区分副作用与回显字段

- Symptom: 迁移物品使用时，若把普通药水当作即时治疗，或把 DeleteItem 的实际裁剪数量写回响应，会与原版客户端可观察行为不一致。
- Root cause: legacy `UseItem` 对普通药水只累加恢复池、太阳水按 `ChangeHP` → `ChangeMP` 顺序产生健康包；`PlayerObject.DeleteItem` 在构造响应后才裁剪 `Count`，因此响应仍回显请求值。
- Prevention: 新增物品动作先从原版 handler、领域副作用方法和客户端响应处理共同列出完整 transcript；分别保存“实际状态变化”和“wire 回显字段”，并为每个分支增加 net.Pipe 顺序断言。
- Verification: 协议布局、普通/太阳水单测、太阳水健康包顺序、删除超量回显和 logout JSON 持久化测试通过。

### 2026-08-11 — 地面物品测试必须移动到真实拾取格

- Symptom: 地面物品/金币已经掉落到 `(1,0)`，测试却让玩家停在 `(0,0)` 直接拾取，结果没有收到任何拾取结果。
- Root cause: legacy `PickUp` 只遍历玩家当前 cell，不会在可见范围内自动寻找物品；测试只验证了可见性，没有验证当前格语义。
- Prevention: 每个掉落 transcript 先记录 origin、搜索到的 drop location、移动步骤和 pickup cell；可见不等于可拾取，拾取前必须显式移动到对象坐标。
- Verification: 修正 world fixture，并在双会话 transcript 中通过走到 `(1,0)`/`(2,0)` 后再拾取。

### 2026-08-11 — net.Pipe 多会话启动也必须消费 bootstrap 广播

- Symptom: 第二个会话启动后等待第一个会话的 `ObjectPlayer` 超时。
- Root cause: `world.enter` 之后服务端先向第二个连接写已有玩家对象，再向第一个连接写新玩家对象；`net.Pipe` 无缓冲，第二个连接未消费前服务端无法继续到第一个连接。
- Prevention: 多会话 transcript 先列出 bootstrap 广播顺序；启动第二会话后先消费其 `ObjectPlayer`，再等待第一会话的对等包；后续每次广播为所有接收者建立并发 reader。
- Verification: 双会话掉落/拾取/金币 transcript 通过普通测试和 `go test -race ./...`。

### 2026-08-11 — Go 与 C# 工具链必须按文件类型隔离

- Symptom: 迁移过程中曾把 C# 导出器文件交给 `gofmt`，命令报语法错误；Go 源码本身没有问题。
- Root cause: 批量格式化命令使用了过宽的路径集合，没有先按扩展名和语言工具链划分目标。
- Prevention: `gofmt` 只接收 Go 文件/Go 目录；C# 只在检测到 `dotnet`/C# 工具链后使用对应格式化或构建命令，不能用 Go 工具替代静态验证。
- Verification: 本轮只对 Go 目录执行 `gofmt`；C# exporter 仅做最小字段 diff 检查，并明确保留 .NET SDK 环境下的编译验证边界。

### 2026-08-11 — FriendlyName 清洗顺序必须保持 legacy 顺序

- Symptom: 地面物品名称清洗初稿先移除方括号再移除尾部数字，`Drop Blade7[rare]` 会错误变成 `Drop Blade`。
- Root cause: 原版 `ItemInfo.FriendlyName` 明确先执行尾部数字正则，再移除方括号；相似的两个清洗步骤不可交换。
- Prevention: 迁移字符串派生字段时保留原方法的操作顺序，并用“数字在方括号前/后”两种 fixture 做差异断言。
- Verification: Go 实现改为 trailing-digits → bracketed-text，双会话 ObjectItem transcript 与全量测试通过。

### 2026-08-11 — 系统提示文本也属于可观察协议行为

- Symptom: 地图禁止丢弃和 Owner 拒绝拾取的 Go 提示初稿语义相近但不等于原版默认本地化文本。
- Root cause: 只迁移了 Chat packet 类型，没有从 `Shared/Language.cs` 核对 `ServerTextKeys` 的默认字符串。
- Prevention: 对每个失败/提示分支同时对照 packet 类型、默认文本和参数；未迁移本地化表时先使用原版英文键值，不自行改写措辞。
- Verification: Go handler 已使用 `CanNotDrop` 和 `CannotPickupNotOwner` 的默认文本，并通过全量 Go 测试与差异检查。

### 2026-08-11 — Storage bootstrap transcript 必须消费物品定义

- Symptom: 新增 Storage net.Pipe 测试时，启动 helper 期望先收到 `MapInformation`，实际先收到 `NewItemInfo`，导致 bootstrap 断言失败。
- Root cause: 角色包含 `ItemInfos` 时，Go Game bootstrap 与 legacy 一样会在地图和用户信息前发送每个物品定义；测试直接复用了只适用于空物品目录的 helper。
- Prevention: 每个 session fixture 先列出 `ItemInfos` 数量和完整 bootstrap 顺序；含物品定义时逐个消费 `NewItemInfo`，或使用显式支持物品目录的 helper，不能按测试名称猜测首包。
- Verification: Storage store/take-back JSON transcript 改为消费物品定义后，Storage 定向测试和 Go 全量测试通过。

### 2026-08-11 — apply_patch 新增函数必须立即检查闭合与上下文空行

- Symptom: 通过 patch 插入 Storage 协议测试时，函数末尾闭合大括号遗漏，`gofmt` 在后续函数声明处报告 `expected '('`；多个小 patch 还因空行上下文未标记而被拒绝。
- Root cause: patch 数组同时承载 JavaScript 字符串、apply_patch 标记和 Go 语法，插入函数前后只核对了行为行，没有检查新增函数的完整括号和空行上下文。
- Prevention: 新增函数 patch 必须包含并核对完整 `func`/`}`；空行在 hunk 中用带上下文或新增标记显式表示；patch 返回后立即查看目标文件、运行 `gofmt`，再继续扩展功能。
- Verification: 补齐协议测试闭合后，`gofmt`、协议/auth/server 定向测试通过；继续执行完整 race/vet/build 门禁。

### 2026-08-11 — 跨仓库绝对路径必须复用已验证根目录

- Symptom: 本轮补 Trade 溢出邮件时，apply_patch 首次把 Go 仓库路径写成了重复目录，工具找不到目标文件，补丁未落地。
- Root cause: 已知 Go 仓库真实根目录为 `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`，但调用时凭记忆手写路径，未在 patch 前交叉验证目标文件。
- Prevention: 跨仓库修改前先执行 `git rev-parse --show-toplevel` 和目标文件存在性检查；apply_patch 只使用该输出生成的绝对路径，失败后立即检查两仓库 status，禁止凭工具错误/成功推断源码状态。
- Verification: 改用已验证路径后 StoredMail、auth 持久化、Trade fallback 和测试均正确落在 Go 仓库；定向 Go 测试、竞态测试和 `git diff --check` 通过。

### 2026-08-11 — Repair ordinal 与浮点费用必须从原版逐项核算

- Symptom: Repair 初稿沿用了摘要中的客户端 ordinal `55/57`，与原版 `MagicKey=57` 冲突；会话测试的 125% 费用也曾把 `75*1.25` 写成 94。
- Root cause: 依赖二手摘要和心算，没有从完整 `ClientPacketIds` 枚举以及 C# `float` 截断规则分别建立证据和计算表。
- Prevention: 新增 packet 先从完整 `Shared/Enums.cs` 计算 ordinal 并加入显式 legacy wants；涉及金额时按原版字段类型、运算顺序、截断点和每一步余额写出 transcript，再填写断言。
- Verification: 原版核对确认 `RepairItem=54`、`SRepairItem=56`；协议 ordinal/payload 测试与普通/特殊 Repair net.Pipe transcript 通过，费用断言分别为 93 和 375。

### 2026-08-11 — 跨仓库只读检索也必须使用对应根目录

- Symptom: 本轮把原版 `Shared`/`Server` 检索路径放在 Go 仓库工作目录下，命令报路径不存在；源码没有被修改，但检查结果不完整。
- Root cause: 只确认了 Go 仓库的 workdir，没有为同一批跨仓库命令分别绑定原版 Crystal 根目录和 Go 根目录。
- Prevention: 跨仓库读写前分别执行 `git rev-parse --show-toplevel`，并让每条命令的 workdir 与目标文件所属仓库一致；不要在一个仓库 workdir 中访问另一个仓库的相对路径。
- Verification: 后续原版查询统一使用 `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`，Go 查询统一使用 `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`，并在修改前检查两个仓库的 status。
- Strengthening after recurrence: 禁止在同一条 `exec_command` 中混合两个仓库的相对路径；需要跨仓库时拆成独立调用，并在输出中保留仓库标识。
- Second strengthening after recurrence: 并行调用也不能靠“同一批任务”推断 workdir；构造每个调用后先机械核对其路径只属于该调用的仓库。本次 P9 查询把 Go 测试路径和原版 `Libraries` 路径放进 Crystal workdir，命令整体无结果；拆分为两个绝对 workdir 后再继续，防止把空检索误当成不存在。
- Third strengthening after recurrence: 即便查询结果逻辑相关，也不能把原版源码读取与 Go 标识符检索合并成一条命令；每次调用在执行前按“命令中的每个相对路径都属于 workdir”逐项验收。本次 Conquest 核对把 `Server/MirObjects/GuildObject.cs` 放进 Go workdir，命令只读失败且无文件改动；已拆成两个仓库各自独立重跑。
- Fourth strengthening after recurrence: 同一仓库的搜索目录也必须先由 `rg --files` 或已验证路径清单确认；任一不存在的目录都会让 `rg` 以 2 退出，即使同时打印了其他目录的部分结果，也不能当作完整证据。本次误带不存在的 `Libraries` 后，已仅用确认存在的 `Server`/`Shared` 重跑 Conquest 查询。
- Fifth strengthening after recurrence: 即使同一查询只想并列核对 Go 与 Legacy 常量，也不能在 Go workdir 的 `rg` 参数中附带 `Server/...cs`；本批核对 ControlPoints 上限时该混用再次以 2 退出。以后先按仓库构造两份路径清单并分别调用，当前已在 Crystal 根目录单独重跑 `MAX_KING_POINTS/MAX_CONTROL_POINTS` 查询并取得完整结果。
- Sixth strengthening after recurrence: 已确认仓库根目录仍不代表任意常见子目录存在；本批在原版 Crystal workdir 的检索参数中误带 Go 专属 `cmd`，使 `rg` 以 2 退出。每次多目录搜索前必须先用该仓库的 `rg --files` 生成实际顶层路径集合，命令参数只能从集合中选择；当前已移除 `cmd` 并在原版 `Server/MirObjects`、`Shared` 与 Go 仓库各自独立重跑相关查询。
- Seventh strengthening after recurrence: 本批只读分析又在 Go 仓库搜索不存在的 `tasks`，并在 Go workdir 的同一命令中混入原版 `Server/MirObjects/PlayerObject.cs`。今后每条检索命令先在该命令自己的 workdir 用 `rg --files` 确认全部目标路径；命令参数只能来自这份清单，Go 与 Legacy 即使逻辑相关也必须拆成两个独立调用，任何 `rg` 退出码 2 都视为证据无效并在正确根目录完整重跑。
- Verification after seventh recurrence: 已在 Crystal 根目录确认 `tasks/lessons.md` 与原版 `Server` 路径，在 Crystal.GoServer 根目录单独确认 Go 源码路径；后续区域魔法核对将按仓库独立执行，不再混合相对路径。
- Eighth strengthening after immediate recurrence: 追加第七次规则后的下一条调用仍把 Legacy `Server` 检索附在 Go 协议读取命令后，说明仅靠执行前自然语言提醒无效。此后在提交每个 `exec_command` 前执行前缀 allowlist 检查：Crystal.GoServer 命令出现 `Server/`、`Shared/`、`Client/` 或 `tasks/` 时禁止执行；Crystal 命令出现 `cmd/`、`internal/` 或 Go 文档路径时同样禁止执行。逻辑相关的读取也必须由两个独立 tool call 承载。
- Verification after eighth recurrence: 失败调用只完成了 Go 文件读取，Legacy 查询以退出码 2 明确作废；`BeginMagic` 证据已改在 Crystal 根目录的独立调用中重跑，后续命令逐条应用仓库前缀 allowlist。
- Ninth strengthening after recurrence: FrostCrunch 复核时仍在 Crystal workdir 的多段 `sed` 末尾误附 Go 路径 `cmd/crystal-server/world.go`，最终退出码 2。以后一个 `exec_command` 只允许一个仓库的一组已存在前缀；准备调用时先按 workdir 写出允许前缀（Crystal: `Server/Shared/Client/tasks`，Go: `cmd/internal/docs/README.md`），参数中出现另一组即拆成下一次调用，禁止在已写好的命令尾部追加跨仓库路径。
- Verification after ninth recurrence: 混合调用结果已作废；Legacy `MonsterObject.ProcessPoison`/`MapObject.ApplyPoison` 与 Go `worldMonster`/AI 字段读取分别在各自根目录重跑并取得 `exit_code=0`，后续修正也仅写入所属仓库。
- Tenth strengthening after recurrence: 自增益技能下一批分析时又在 Crystal.GoServer workdir 查询 Legacy `Server/...`，说明仅靠人工前缀复核仍会漏过临时追加的参数。此后 Legacy 与 Go 证据读取不仅分调用，还分不同的 `functions.exec` cell；每个 cell 只允许固定仓库根目录，调用前以该根目录的 `rg --files` 结果确认目标存在，禁止在同一 JavaScript 数组中编排两个仓库的源码查询。
- Verification after tenth recurrence: 该退出码 2 的结果已作废；本轮恢复迁移前分别确认 Crystal 仅使用 `Server/Shared/Client/tasks`，Crystal.GoServer 仅使用 `cmd/internal/docs/README.md`，后续 ProtectionField/Rage/SwiftFeet 证据将按独立 cell 读取，并在两个仓库分别检查 `.cs` 零变化。
- Eleventh strengthening after recurrence: 本批虽先由 `rg` 找到 Buff 定义位于 `Server/MirDatabase/BuffInfo.cs`，随后仍凭命名习惯追加了不存在的 `Buff.cs`；Go 查询又把未匹配的 shell `*catalog*_test.go` 直接作为参数，分别造成退出码 1/2。以后读取文件前只使用同一仓库 `rg --files -g '<pattern>'` 实际返回的精确路径；文件筛选使用 `rg -g`，禁止 shell 展开未验证的 glob，也禁止把已成功的 `sed` 与猜测性检索串成一条命令。
- Verification after eleventh recurrence: Buff 模型已改用确认存在的 `Server/MirDatabase/BuffInfo.cs` 独立重读；Go 测试文件清单已用 `rg --files cmd/crystal-server -g '*magic*test.go'` 验证，后续实现只修改清单中存在的文件或通过 `apply_patch` 明确新建文件。

### 2026-08-11 — 商店会话 fixture 必须恢复 RentalInformation 元数据

- Symptom: NPC 商店 Sell 失败门禁测试预期 Rental `DontSell` 返回普通失败包，但测试先收到“此处不能出售该物品”的类型限制聊天包。
- Root cause: fixture 只创建了 Rental 对应的 `ItemInfo`，没有把 `RentalInformation.BindingFlags=DontSell` 挂到实际背包物品；服务端因此按 `[TYPES]` 分支处理了它。
- Prevention: 测试 legacy 物品绑定规则时分别核对 definition 级 `ItemInfo.Bind`、instance 级 `StoredItem.RentalInformation.BindingFlags` 和 NPC 的 `[TYPES]`，不要只按索引命名推断 fixture 已具备语义元数据。
- Verification: 为实际 Rental item 补齐 binding flags 后，DontSell、Rental DontSell、类型限制的 net.Pipe transcript 与全量 Go 测试通过。

### 2026-08-11 — 新协议 patch 必须以当前常量表为唯一上下文

- Symptom: TownRevive 协议 patch 把尚未存在的 `ClientChangeHero` 当作 Go 常量表上下文，apply_patch 被拒绝；目标文件没有部分修改。
- Root cause: 直接套用了原版 enum 的相邻名称，没有先读取 Go 当前已迁移常量的精确范围。
- Prevention: 新增 packet ordinal 时先从完整原版 enum 锁定数值，再从目标 Go 文件读取真实相邻标识；只使用当前存在的单行稳定锚点，patch 失败后立即检查 diff/status。
- Verification: 改用 `ServerUserStorage`、`ServerObjectRevived` 和当前存在的客户端常量作为锚点后继续实现，并在定向协议测试中同时断言 legacy ordinal。

### 2026-08-11 — TownRevive 回归验证必须检查 fixture 与仓库边界

- Symptom: PK Town 配置测试初版把换行写成字面量标记；本轮多个 `functions.exec` patch wrapper 又出现漏引号/漏 hunk 标记；一次只读检索还使用了不完整的 Go 仓库路径。
- Root cause: 测试 fixture、JavaScript patch 字符串和跨仓库 workdir 都依赖手写文本，调用前没有逐层检查实际文件内容、hunk 首字符和仓库根目录。
- Prevention: 写入后用源码读取确认 fixture 的真实转义层级；patch 先逐行检查字符串引号、`*** Begin/End Patch`、`@@` 及上下文前导空格；跨仓库命令分别使用已验证的绝对根目录，禁止混用相对路径。
- Verification: 修正 fixture 后 config/TownRevive 定向测试和 Go 全量 test/race/vet/build 通过；每次工具调用后均检查对应仓库 status/diff，未发生跨仓库误写。

### 2026-08-11 — NPC 物品堆叠终态必须逐步推导

- Symptom: 本轮 `GIVEITEM` 测试的背包终态曾两次误算，先后把 fresh stack 的协议数量、已有堆叠和空槽放置顺序混在一起。
- Root cause: 没有按每次 `AddItem` 的“先合并已有堆、再按物品类别找槽位”逐步记录操作前后状态，也没有把发送前 clone 与背包中的可变对象分开核算。
- Prevention: 物品 transcript 先列出每个 fresh item 的 UniqueID/Count、合并目标、剩余数量和最终槽位；wire clone 与库存终态分别断言，不能凭总数量推断槽位。
- Verification: 修正 `GIVEITEM/TAKEITEM` 单测和 net.Pipe 持久化断言后，定向测试与 Go 全量 test/race/vet/build 通过。

### 2026-08-11 — 跨功能 patch 必须使用唯一函数锚点

- Symptom: NPC 物品动作 patch 曾因使用相似的 `return`/`addCharacterItem` 上下文误命中 `addCharacterItem`，目标函数没有得到预期修改。
- Root cause: 相邻功能复用了相同的局部代码形状，patch 没有同时带上完整函数签名和行为调用作为唯一定位条件。
- Prevention: 修改已有 helper 时以完整函数声明、调用方和唯一行为行组成双重锚点；每次 patch 后立即查看目标文件的 `git diff`，确认没有跨函数命中。
- Verification: 误命中在测试前被发现并修正；当前 `item_transactions.go` diff 只包含 NPC 物品 helper，定向测试、全量门禁和 `git diff --check` 通过。

### 2026-08-11 — apply_patch 上下文行必须显式保留前导空格

- Symptom: 本轮修改 NPC 物品 handler 时，两次 patch 因 `log.Printf` 和函数末尾 `}` 作为上下文行却缺少 hunk 要求的前导空格而被拒绝；源码没有部分落盘。
- Root cause: JavaScript 字符串数组把源码行和 patch 语义混在一起，人工检查只看了内容，没有检查每行首字符。
- Prevention: 调用 `apply_patch` 前逐行检查上下文以一个空格开头、删除以 `-` 开头、新增以 `+` 开头；失败后先查 status/diff，再重试更小 hunk。
- Verification: 按单函数小 hunk 重做，检查到 `executeNPCItemAction` 的完整返回状态与日志均落盘；随后用 gofmt、定向测试和完整 Go 门禁验证。

### 2026-08-11 — 文档检索也必须复用已确认的仓库根目录

- Symptom: 本轮 Go 文档检索命令把工作目录手写成重复的 `me_work` 路径，进程未能启动，文档检查被延迟。
- Root cause: 只记住了仓库名称，没有复用前面 `git rev-parse --show-toplevel` 得到的绝对路径。
- Prevention: 所有跨仓库命令（包括只读的 README/文档检索）统一使用已验证的绝对根目录；命令失败后先检查路径和两仓库 status，再继续。
- Verification: 改用 `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer` 后 README/迁移矩阵检查完成，未产生文件修改。

### 2026-08-11 — NPC 响应包不能作为动作执行完成屏障

- Symptom: `SET` 会话测试读到 `ServerNPCResponse` 后立即检查角色旗标，偶发看到旧值并误报迁移失败。
- Root cause: Go NPC handler 按 legacy transcript 先写响应包，再执行页面 Actions；`net.Pipe` 客户端读到响应时，服务端仍可能在执行 `SET`。
- Prevention: 会话测试在断言 NPC 动作副作用前发送并消费 `ClientKeepAlive`/`ServerKeepAlive`，把 keep-alive 返回作为当前请求处理完成的屏障。
- Verification: `SET` + `BREAK` + 下一页面 `CHECK` 会话测试改用 keep-alive 屏障后稳定通过。

### 2026-08-11 — 跨仓库并行查询必须按调用拆分

- Symptom: 本轮并行核对 NPC 技能时，其中一条 Go workdir 命令混入了原版 `Shared/Enums.cs` 路径，查询报文件不存在；源码没有被修改，但原版证据检查被打断。
- Root cause: 并行命令数组只统一设置了一个 workdir，却把两个仓库的相对路径放进同一批查询中。
- Prevention: 同一批跨仓库查询也必须按仓库拆成独立调用；Go 调用只使用 Go 相对路径，原版调用只使用 Crystal 相对路径，并在输出中保留仓库标识。
- Verification: 后续原版 enum/NPC 动作查询改用 `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`，Go 协议/运行时查询改用 `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`。

### 2026-08-11 — NPC 技能重复判断必须以角色技能列表为准

- Symptom: Go 运行时为了兼容无持久化技能的测试玩家可能注入默认 FireBall；如果 GIVESKILL 只检查运行时 `Magics` map，会把默认技能误判为角色已经学习，或在移除时无法与持久化列表对齐。
- Root cause: 运行时施法表和角色持久化 `Info.Magics` 的职责不同；前者可能包含迁移阶段的 fallback，后者才对应原版 NPC `player.Info.Magics.Any(...)` 判断。
- Prevention: GIVESKILL/REMOVESKILL 的存在性、索引和持久化更新统一以 `SelectInfo.Magics`/`worldPlayer.Character.Magics` 为源；运行时 map 只负责施法门禁和数值。
- Verification: NPC 技能 net.Pipe 测试从持久化 FireBall 学习 ThunderBolt、按索引移除 FireBall，并同时断言 auth 与 world 快照一致。

### 2026-08-11 — NPC 链式页必须逐页执行特殊面板

- Symptom: GOTO/CALL 初版只在整条链最后按 active page 发送 shop/repair/refine 面板，链中间进入功能页时客户端只能收到文本，缺少对应功能包。
- Root cause: 控制流 job 队列只复用了页面文本和 actions，特殊页处理仍留在原请求末尾，没有随每个 job 的生命周期执行。
- Prevention: 把 NPC 页处理拆成“选择响应 → 执行动作/追加链 → 当前页特殊处理”，每个链式 job 都按自身 page key 跑 shop、repair、refine、storage 和 collect 分支，并分别锁定包序列。
- Verification: 新增 GOTO 到 refine 页和自循环上限的 net.Pipe 测试；现有 shop、repair、refine、storage 测试和控制流测试均通过。

### 2026-08-11 — 无响应门禁测试不能调用 writeAndRead

- Symptom: 为验证 CALL 外部脚本的 active-script 页面门禁时，测试使用 writeAndRead 发送应被忽略的请求，net.Pipe 一直等待响应并触发超时。
- Root cause: 被拒绝的 NPC 请求按协议不会产生响应，writeAndRead 的读取步骤只能用于必有响应的请求。
- Prevention: 无响应断言只编码并写入请求，再发送一个确定有响应的 KeepAlive 作为处理完成屏障；需要包序列时显式区分忽略请求和响应请求。
- Verification: active-script 越权页测试改为 raw write + KeepAlive，定向控制流测试在 15 秒门禁内稳定通过。

### 2026-08-11 — NPC 脚本 fixture 必须检查源码实际反斜杠层级

- Symptom: 新增链式 refine fixture 时把双反斜杠 n 写成了 Go 源码中的双反斜杠，解析器看见字面量换行标记，测试在读取链式响应时阻塞。
- Root cause: JavaScript patch 字符串和 Go 字符串各有一层转义，写入后没有立即用十六进制/源码读取确认实际字节。
- Prevention: 生成脚本 fixture 后立刻检查目标行；Go 双引号字符串只保留单层反斜杠 n，跨工具层需要用普通字符串拼接，禁止凭视觉判断转义层级。
- Verification: 修正为真实换行转义后，GOTO/CALL 与 GOTO 特殊面板测试通过；此前同类换行边界规则继续适用。

### 2026-08-11 — 同一连接重新登录必须清空可见对象缓存

- Symptom: 任务登出后在同一连接重新 StartGame，服务器先发送任务状态，但没有重新发送 NPC 对象；会话验收按启动包序列阻塞。
- Root cause: logout 已移除 world player，却保留了 session 层 `visibleNPCs`/`visibleMonsters` 集合，下一次刷新把仍存在的静态对象误判为客户端已拥有。
- Prevention: 任何离开 Game stage 的路径都同时清理 world player、活动 NPC 状态和可见对象缓存；重进验收必须断言静态对象与任务状态都重新 bootstrap。
- Verification: `TestQuestSessionAcceptFinishAndPersist` 覆盖接取→登出保存→同连接重登恢复→完成→再次登出；Go 全量测试通过。

### 2026-08-11 — .NET exporter 与 Go JSON bridge 必须逐字段核对语义

- Symptom: 导出器把 `CurrentQuests` 写成任务对象列表，而 Go bridge 的同名字段是 `[]int32`，导致真实账户 JSON 无法按预期恢复。
- Root cause: 只对照了字段名，没有对照字段的元素类型和“ID 列表/详情列表”职责。
- Prevention: 每增加跨语言字段，必须同时核对 C# 类型、导出 JSON 形状、Go 结构体类型和 round-trip fixture；`currentQuests` 只保存 ID，`questProgress` 保存详情。
- Verification: 增加 JSON 原始形状断言和加载后进度 round-trip 测试。

### 2026-08-11 — Quest NPC 校验不能信任客户端上下文

- Symptom: Go 实现曾把 `AcceptQuest.NPCIndex` 和当前活动 NPC 作为完成任务的额外条件，导致合法请求可能被拒绝。
- Root cause: 将“客户端打开了哪个 NPC”误当成了原版 `PlayerObject` 的授权条件；原版按任务定义查找 NPC，只检查当前地图和 DataRange。
- Prevention: request 中可由客户端伪造的 NPC ID 和 session 中缓存的 active NPC 只用于 UI/script 状态，不参与 Quest accept/finish 授权；回归测试必须覆盖错误 ID 仍能通过。
- Verification: Quest NPC range/request-ID 测试和同连接重登后的接取/完成会话测试通过。

### 2026-08-11 — 测试断言要区分 normalize 的状态修复与业务进度变化

- Symptom: 对旧版缺少任务明细的进度调用 normalize 后，测试把结构补全误判为“没有变化”或反过来误报业务进度变化。
- Root cause: 断言只看 `changed` 数量，没有先区分 schema normalization、任务状态变化和实际计数变化。
- Prevention: 测试同时断言规范化后的 task 数组、计数、完成状态和 packet 语义；不要用一个布尔值代表所有变化。
- Verification: legacy progress normalization、任务列表和 JSON round-trip 测试覆盖上述边界。

### 2026-08-11 — 工具链格式化必须按语言分组

- Symptom: Quest 收尾时把 C# exporter 文件和 Go 文件一起传给 `gofmt`，命令在 C# 文件处报 `expected 'package'`，导致格式化门禁中断。
- Root cause: 只按本批改动文件列表执行格式化，没有按扩展名和项目工具链拆分命令。
- Prevention: 提交前按语言分别运行格式化和编译检查；Go 只交给 `gofmt`，C# 只交给 .NET SDK/对应格式化工具，并在命令失败后确认源码没有被部分修改。
- Verification: 重跑仅包含 Go 文件的 `gofmt`、`git diff --check`，随后单独检查 .NET SDK 可用性并记录未验证边界。
- Strengthening after recurrence: Mail 收尾再次把 `Program.cs` 放入 `gofmt` 参数后，禁止直接复用“全部改动文件”作为格式化输入；必须先按 `.go` 扩展名生成或人工核对参数清单，C# exporter 另行探测 `dotnet`/`csc`/`mcs`，本轮已用显式 Go 文件清单完成格式化。

### 2026-08-11 — 断线广播不能被失效连接短路

- Symptom: 组队成员通过 net.Pipe 断开后，在线 leader 先收到 ObjectRemove，收不到应有的 DeleteGroup。
- Root cause: 广播按通知顺序写入时，第一个已断开成员的写操作报错，deliverWorldNotifications 立即返回，后续在线成员没有收到通知。
- Prevention: 广播遍历必须继续投递所有 recipient，只保存并返回第一个错误；新增断线场景要同时断言在线成员的协议包顺序和 world 清理结果。
- Verification: 修正广播后双会话测试稳定收到 DeleteGroup → ObjectRemove；普通测试、race、vet 和 build 全部通过。

### 2026-08-11 — net.Pipe 多会话测试要先建立 reader 并消费完整 transcript

- Symptom: 组队邀请/离开测试曾因服务端写包等待客户端读取，或在第一包到达时过早检查共享状态而失败。
- Root cause: net.Pipe 没有缓冲；组队操作会向双方发送多包，最后一个响应不等于另一会话的清理已经完成。
- Prevention: 在触发跨会话操作前为每条连接建立异步 reader，按 legacy 顺序消费全部副作用包；检查 world 状态前使用会话完成 channel 作为屏障，并在读取共享 map 时持锁。
- Verification: TestSessionTwoPlayerGroupInviteAcceptAndLeave 覆盖邀请、接受、DeleteGroup、ObjectRemove 和断线清理，且 race 测试通过。

### 2026-08-11 — apply_patch 封装必须验证路径和闭合标记

- Symptom: 更新 lessons 时一次 patch 因目标路径重复了仓库目录而未应用。
- Root cause: JavaScript 工具封装中的绝对路径是手写的，提交前没有检查目标文件是否存在；复杂 Markdown/raw 内容也容易遗漏 End Patch 闭合标记。
- Prevention: 调用 apply_patch 前先确认绝对路径和目标文件；多行 Markdown/raw 内容使用普通字符串数组拼接，明确包含 Begin Patch/End Patch，并在返回后读取文件确认落盘。
- Verification: 修正路径后 lessons 成功追加；随后 git diff --check 通过。

### 2026-08-12 — 会话测试账号必须符合认证层字符约束

- Symptom: RequiredGroup net.Pipe 测试使用带连字符的账号 ID，登录在测试 fixture 阶段被认证层拒绝，无法到达地图门禁断言。
- Root cause: 测试 fixture 只关注业务语义，没有先复用认证 parser 对账号 ID 的字符集约束。
- Prevention: 新增会话测试账号先从认证校验和现有 fixture 归纳合法字符集；优先使用仅含小写字母和数字的稳定 ID，不把非法账号错误归因于业务功能。
- Verification: 改用 `requiredgroup`/`rgleader` 等合法 ID 后，RequiredGroup 普通、登录恢复、多会话测试通过。

### 2026-08-12 — 跨会话地图变更必须延迟同步 session 局部状态

- Symptom: 组员被动离组时，world 已经把玩家移回安全地图，但直接在其他 session 回调里修改 `activeMap`/坐标会与主循环并发访问，且可能遗漏静态可见对象刷新。
- Root cause: world 通知在调用方或 ticker goroutine 投递，session 局部状态只应由所属读循环拥有；地图切换的 wire transcript 与本地状态提交被混在一个回调里。
- Prevention: 回调只发送换图包并把 transition 放入 mutex 保护的 pending 状态；所属 session 在下一次读包前统一更新地图、坐标、角色快照、持久化和静态可见对象。
- Verification: 双 net.Pipe 会话覆盖 DeleteGroup、ObjectRemove、MapChanged、UserLocation、ObjectPlayer、Chat 顺序；普通/race/vet/build 全部通过。

### 2026-08-12 — 地图门禁失败路径必须保持事务性

- Symptom: TownRevive 先恢复 HP、ENTERMAP 先清空 pending 目的地，再检查 RequiredGroup 时，失败请求会留下不可见的生命值或入口状态变化。
- Root cause: 业务动作的副作用早于目标地图资格校验提交，失败结果没有回滚。
- Prevention: 所有 RequiredGroup 入口先完成地图存在、坐标和组人数校验；成功后才写回 HP/坐标，ENTERMAP 只有非门禁失败时才消费 pending 目的地。
- Verification: RequiredGroup world 测试断言 TownRevive 不恢复 HP、ENTERMAP 保留 pending；全量 Go 门禁通过。

### 2026-08-12 — 物品加入背包前必须复制网络快照

- Symptom: NPC 商店批量购买改为先更新角色背包后再编码 `GainedItem` 时，`addCharacterItem` 合并/消耗了传入对象，客户端收到的购买数量变成 0。
- Root cause: 领域层的背包操作会原地修改传入 `StoredItem`；网络 payload 不能直接引用会被后续事务改变的对象。
- Prevention: 所有会调用 `addCharacterItem`、堆叠合并或删除的路径，在第一次状态变更前复制独立的 packet snapshot；测试同时断言内部持久化数量和线上的 `GainedItem.Count`。
- Verification: NPC 商店普通购买和 `[BUYBACK]` 会话 transcript 均断言数量/UniqueID，Go 全量 test、race、vet、build 和 `git diff --check` 通过。

### 2026-08-12 — 邮件落库与在线到达 transcript 必须分别验收

- Symptom: 玩家邮件和交易溢出邮件已经写入收件箱，但在线客户端只收到 `ReceiveMail` 或完全没有即时刷新，缺少原版 `NewMail` 系统聊天及完整附件定义顺序。
- Root cause: 只沿持久化状态验证了 `MailInfo.Send` 的结果，没有迁移 `NewMail -> Process -> ReceiveChat -> CheckItem -> GetMail` 这条在线可观察链路。
- Prevention: 每个创建邮件的入口同时验收离线存储和在线 `Chat -> NewItemInfo* -> ReceiveMail` transcript；跨会话只生成通知快照，释放 world/mail 锁后再写连接。
- Verification: 普通邮件、带邮票附件邮件、交易取消溢出邮件测试均断言到达顺序，Mail 定向普通与 race 测试通过。

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

### 2026-08-12 — 定时协议常量必须先查完整枚举

- Symptom: 启动状态机补任务定时包时曾使用不存在的 `protocol.ServerQuestExpired`，导致新增测试无法编译。
- Root cause: 按业务语义猜测了包名，没有先检索 legacy `ServerPacketIds` 和 Go 常量表；实际任务计时使用通用 `ServerSetTimer`。
- Prevention: 新增 transcript 常量前先用 `rg` 同时核对 legacy enum、packet class 和 Go 常量，并在同一修改中补显式 ordinal 断言；禁止从功能名称推导常量名。
- Verification: bootstrap helper 现在只接受已核实的 `ServerSetTimer`/`ServerChangeQuest`，协议和服务端全量门禁通过。

### 2026-08-12 — 持久化物品目录不能代替连接级已发送状态

- Symptom: Quest 定义在第一次登录顺序正确，但定义被写入 `SelectInfo.ItemInfos` 后，重登会把全部定义错误提前到 `MapInformation` 前；Mail 附件也可能因目录中已有定义而跳过其应位于 `ReceiveMail` 前的 `NewItemInfo`。
- Root cause: `gameItemCatalog` 同时承担服务端持久化定义目录和客户端当前连接已知集合；前者跨登录保留，后者必须随每次 StartGame 清空，两者语义不能合并。
- Prevention: 每个连接维护独立、并发安全的 sent-item-info 集合；初始角色格、Quest、Recipe、Mail、NPC 商品和显式 RequestItemInfo 各按原版阶段检查并标记，目录是否已有定义只决定服务端是否追加，不能决定是否发包。
- Verification: Quest 重登锁定“当前携带定义 → Map/User → 其余 Quest 定义”，Mail 嵌套附件锁定“CompleteQuest → NewItemInfo* → ReceiveMail”，UsedGoods 与重复 RequestItemInfo transcript 通过；Go 全量 test、race、vet、build 和 `git diff --check` 全部通过。

### 2026-08-12 — Goal 状态审计必须使用当前 CODEX_HOME 并关联线程生命周期

- Symptom: 只调用当前线程的 Goal 查询时看起来只有 1 个 Goal，但当前 Codex 实例的数据库实际有 4 条 `active` 记录，容易把“当前线程 Goal”误报成“整个实例正在执行的 Goal”。
- Root cause: Goal 查询接口按当前线程返回；同时若未先解析 `CODEX_HOME`，可能误查默认 `~/.codex`。子代理线程已经关闭后，其 Goal 行还可能残留为 `active`，单看 Goal 表不能代表仍在运行。
- Prevention: 回答 Goal 总数前先确认当前 `CODEX_HOME`，查询其 `goals_1.sqlite`，再与同目录 `state_5.sqlite` 的线程和 `thread_spawn_edges` 生命周期关联；分别报告“数据库标记 active”和“实际仍运行”，且不得手工改内部数据库清理残留。
- Verification: 本次关联审计确认 4 条 `active` 中仅主线程 1 条仍执行，另外 3 条都属于 `closed` 子代理；该主线程下 23 条子代理边全部为 `closed`，当前没有活跃子代理。

### 2026-08-12 — 跨会话同步必须按领域 revision 定向合并

- Symptom: Rental 为接收后台到期/死亡更新，把每次读包前的 world 同步扩大为整角色物品格和邮件覆盖，导致同一 session 刚完成的合成、装备、背包移动、精炼、修理、仓库和使用物品状态被 stale world 快照回滚；全量测试同时出现多类持久化和 transcript 失败。
- Root cause: world 快照既包含跨会话共享状态，也包含当前 session 自己拥有、但并非每条业务路径都立即回写 world 的局部状态；无版本判断的全量赋值把两种所有权混在一起。
- Prevention: 保留 Quest/Group 的既有定向同步；为 Rental 增加独立 revision，只在 Rental 生产路径实际修改网格/邮件时递增，并在 revision 变化时同步 Rental 所需字段。新增共享领域必须先明确所有权和变更版本，禁止为了接收一个后台字段而整角色覆盖。
- Verification: 修复后先前失败的 Craft、Equipment、ItemMove、Refine、Repair、Storage、Use/Delete 会话测试与 Rental 定向测试全部恢复通过。

### 2026-08-12 — 并行功能线交接后必须先恢复包级编译绿色

- Symptom: Rental 与移动兼容线合入共享工作树后，`cmd/crystal-server` 留下未使用的 `oldDirection` 和尚未实现的 `deliverMovementMapTransition`，后续工作处在不可编译中间态，放大了“迁移没有进展”的感受。
- Root cause: 多条功能线同时触碰 `main.go`/`world.go`，交接时只记录了剩余事项，没有在每个写入边界立即执行最小包级编译；新的兼容分支继续叠加在未完成的整合点上。
- Prevention: 并行线优先按互斥文件或稳定接口拆分；任何功能线完成写入后，整合者先运行受影响包的仅编译门禁，编译未恢复绿色前不再叠加另一条共享文件修改；未完成 helper 必须与调用点在同一整合步落盘。
- Verification: 本次 `go test ./cmd/crystal-server -run '^$' -count=1` 在 0.1 秒内准确定位两个整合缺口；修复后必须以同一命令恢复绿色，再进入 Rental 定向及全量门禁。

### 2026-08-12 — 地图切换包序列必须按触发路径拆分

- Symptom: Go 曾把地图坐标移动、NPC `ENTERMAP`、NPC `MOVE` 和 RequiredGroup 强制离开统一投递为 `MapChanged + UserLocation`，导致移动测试读取到后续 NPC 包，并掩盖 `ObjectTeleportIn` 的差异。
- Root cause: 复用了一个通用 transition helper，没有保留 legacy `CompleteMapMovement`、`Teleport(..., false)` 与默认 effectful `Teleport` 三条独立调用链。
- Prevention: transition 先标注触发类型，再分别锁定坐标移动/ENTERMAP 仅 `MapChanged`，MOVE/强制传送为 `MapChanged + ObjectTeleportIn`，TownRevive 使用自己的 `MapChanged` 路径；禁止用统一的 `UserLocation` 补包。
- Verification: 更新 movement、ENTERMAP、MOVE、RequiredGroup 和 Rental 顺序测试后，Go 全量普通/race 测试、vet、build 与差异检查通过。

### 2026-08-12 — ActionTime 迁移必须兼容连接队列与直接世界测试

- Symptom: 给 Turn/Walk/Run 写入 350/600ms `ActionReadyAt` 后，既有 net.Pipe 会话连续移动被同步读循环立即拒绝，大量距离门禁和可见性测试停在原地。
- Root cause: legacy 连接会把请求留在队列中等 `ActionTime` 到期再处理，而当前 Go 会话同步读取后立即执行；只迁移时间字段却没有迁移队列调度会改变外部行为。
- Prevention: 世界 helper 保留精确 capability/ActionTime 边界；会话入口保留单条 retry movement，在 `ActionReadyAt` 到期后重新派发，匹配 legacy `_retryList`，不能直接清零时间门禁。
- Verification: 直接 helper 的 capability 测试与连续 session movement、Craft/Shop 距离、地面拾取和静态可见性测试同时通过全量普通/race 门禁。
- Strengthening after P5 combat recurrence: melee、range、magic 也必须复用同一条会话 retry 边界；为其补门禁后，旧 `TestSessionFireBallTranscript` 仍把“立即收到冷却拒绝”当成正确结果，首次全包测试因此在 1800ms 后读到了重派成功的 `HealthChanged`。迁移 ActionTime/AttackTime/SpellTime 时必须同步审计既有 transcript 断言，区分世界 helper 的即时 capability 拒绝与连接层 `_retryList` 的延迟重派，不能为了保留旧 Go 测试而破坏 Legacy 会话行为。
- Verification after strengthening: FireBall 会话测试现锁定等待全局 SpellTime 后重派、再次扣 MP、更新方向并发送完整 Magic transcript；Haste/Fury 测试锁定 AttackSpeed 冷却，六技能测试锁定全局与单技能 CastTime 分离，`go test ./...` 与 `go test -race ./...` 全量通过。

### 2026-08-12 — 一次性异常排查不能变成固定汇报项

- Symptom: 用户只是临时询问一次异常现象，后续进度消息却持续重复相关分析，形成无关噪声。
- Root cause: 把一次诊断请求误解成了长期监控和每轮汇报要求。
- Prevention: 临时排查默认只回答当次；除非同类异常再次影响迁移、需要用户决策或用户明确追问，否则不主动重复诊断结论，也不把它加入固定进度模板。
- Verification: 后续迁移仅在整批完成、真实阻塞或需要用户决策时汇报，不再附带该一次性排查内容。

### 2026-08-12 — 跨语言 round-trip fixture 不得直接比较含 map 的结构体

- Symptom: 公会导出 schema 测试首次编译失败，测试用 `!=` 比较包含 `ItemStats` map 的 `protocol.ItemInfo`。
- Root cause: 把结构体整体比较误当成通用零值检查，未先确认其字段是否全部可比较。
- Prevention: 为跨语言 JSON fixture 做零值或完整对象断言前，先检查 slice、map、pointer 字段；包含不可比较字段时统一使用 `reflect.DeepEqual` 或逐字段断言。
- Verification: 零值 creation-cost Item 改用 `reflect.DeepEqual` 后，`go test ./internal/worlddata -count=1` 通过。
- Strengthening after recurrence: `reflect.DeepEqual` 仍会区分 nil 与空 slice/map；wire parser 会把零长度 Slots、AddedStats、Awake.Values 规范化为空非 nil 容器。构造协议 round-trip fixture 时必须按 parser 的规范形状显式初始化这些字段，或逐字段比较语义值，不能把 nil/空容器差异误判成序列化错误。
- Second strengthening after recurrence: 即时 Buff 双连接测试又准备使用 `observerBuff != buff`，而 `ClientBuffInfo` 内含 `ItemStats` map 与 `Values` slice；虽在编译前复读时发现，仍说明新增断言没有先执行可比较性检查。今后写 `==`/`!=` 前先展开目标类型字段；协议对象默认逐字段断言，确需整体比较时才使用 `reflect.DeepEqual`，并同时明确 nil/空容器规范。
- Verification after second recurrence: 观察者 AddBuff 改为 `reflect.DeepEqual`，随后 `go test ./cmd/crystal-server -run '^$' -count=1` 和完整即时 Buff net.Pipe 测试均通过。

### 2026-08-12 — C# 顶层导出器新增显式参数类型时必须核对命名空间

- Symptom: 公会 creation-cost helper 使用了 `GuildItemVolume` 作为参数类型，而该类型声明在 `Server.MirObjects`，原有 exporter imports 不包含该命名空间。
- Root cause: 调用点可通过类型推断访问成员，但抽成具名 helper 后需要编译器直接解析参数类型；静态审查最初只核对了字段，没有核对类型所属 namespace。
- Prevention: C# exporter 新增 helper 签名时，用原版类型声明反查 namespace，并显式加入对应 `using`；无 SDK 环境的静态 guard 同时锁定该 import 和签名。
- Verification: world exporter 已加入 `using Server.MirObjects;`，静态 schema guard 覆盖 import；`go test ./internal/worlddata -count=1` 通过，真实 C# 编译仍留待具备 .NET 8 SDK 的环境验证。

### 2026-08-12 — 公会默认创建费用必须解析 Legacy 物品名

- Symptom: Go 无 world export 时最初只要求 1,000,000 金币；补上默认 `WoomaHorn` 后，费用记录只有名称、索引为零，导致真实背包中的 Wooma Horn 无法匹配。
- Root cause: Legacy 默认配置先保存去空格物品名，再由 `LinkGuildCreationItems` 对完整物品目录解析；Go 把已解析的 `ItemInfo` 与未解析的默认名称当成了同一种输入。
- Prevention: 默认费用保留 Legacy 的金币加 WoomaHorn 两项；事务开始前按去空格、忽略大小写规则从角色物品定义解析名称，随后才执行原子校验和扣除。默认配置和 exporter 配置必须共用同一事务路径。
- Verification: auth 测试锁定 `WoomaHorn` → `Wooma Horn` 的解析与扣除，完整 NPC 会话锁定 `DeleteItem → LoseGold → GuildStatus`，Go 全量普通/race 门禁通过。

### 2026-08-12 — 注销通知快照与投递时机必须分离

- Symptom: 公会注销状态包若在 `world.leave` 前立即投递，会先写向正在关闭且无人读取的 net.Pipe，阻塞后续在线成员；若在 leave 后才构造，又丢失注销者姓名并退化为角色数字 ID。
- Root cause: 把“从在线 world 读取姓名并生成通知”和“向剩余会话实际写包”绑定在同一步，没有保留快照边界。
- Prevention: 离线流程先在玩家仍在线时提交 auth 状态并构造包含姓名的通知快照，完成 world 移除后再投递；通知接收者列表排除注销者，持久化在状态提交后执行。
- Verification: 双成员 world 测试锁定仅另一成员收到 `GuildMemberChange(Status=0, Name=leader)`；公会会话测试、Go 全量普通/race 门禁通过。

### 2026-08-14 — 状态魔法归属必须沿 Legacy 命中分支确认

- Symptom: 迁移矩阵和实现分析一度把 Slow/Frozen 当成 IceStorm 的待迁移状态，而 Legacy 的 IceStorm 实际只执行 3×3 MAC 区域伤害。
- Root cause: 根据技能名称和常见游戏直觉推断状态归属，没有先把施法入口、延迟命中 switch 和 `AddPoison` 调用点连成完整可达路径；真正施加 Slow/Frozen 的技能是 FrostCrunch。
- Prevention: 每批状态型技能先从 Legacy 命中 resolver 建立“spell → damage → secondary effect”表，再检索所有状态创建调用点交叉确认；文档、实现和测试必须共用该表，禁止从技能名称推断效果。
- Verification: Legacy spell switch 确认 FireBang/IceStorm 只有区域 MAC 伤害、FrostCrunch 命中后才执行 Slow/Frozen 的独立门槛与概率；Go 区域魔法测试保持 IceStorm 纯伤害 transcript，FrostCrunch 领域和三会话测试覆盖完整状态生命周期。

### 2026-08-14 — 手工 world tick 测试必须隔离后台 ticker 并使用会话屏障

- Symptom: FrostCrunch 三会话测试手工推进命中/到期 tick 时，100ms 后台 world ticker 可能抢先发布状态；Frozen 攻击测试读到入口先发送的 `UserLocation` 后立即检查 world，又可能早于真正的 admission 处理。
- Root cause: 确定性 synthetic tick 与生产后台 ticker 同时驱动同一世界，且把 handler 中间包误当成请求完成信号；`net.Pipe` 的包到达只证明该次写入完成，不证明后续领域逻辑已结束。
- Prevention: 需要手工时间轴的会话测试在状态创建前停止该 world 的测试 ticker，并确认可能已选中的空 tick 返回；读取入口中间包后再发送并消费 `KeepAlive`，用后续请求作为所属会话完成屏障，再检查共享状态或断言没有广播。
- Verification: 后台 ticker 隔离后，三会话 FrostCrunch transcript 稳定锁定伤害、两条 Chat、Slow/Frozen 广播、Frozen 拒绝以及逐步清除；Frozen 攻击后的 KeepAlive 屏障确认方向和动作队列均未变化。

### 2026-08-14 — 状态门禁必须收窄到原版 capability 边界

- Symptom: FrostCrunch 初版在普通宠物和 Conquest archer 的外层 AI tick 遇到 Frozen 就直接 `continue`，同时跳过了普通宠物驯服到期/跨图召回和 archer 搜索目标等不属于移动或攻击的生命周期工作。
- Root cause: 把“Frozen 时不能 Move/Attack”简化成“整个对象不处理”，没有沿 Legacy `MonsterObject.Process` 的顺序区分 tame lifecycle、`ProcessSearch`、recall 与最终 `CanMove`/`CanAttack` 门禁。
- Prevention: 每个控制状态先标注它约束的原版 capability；门禁只能放在对应动作提交点，外层对象 tick、到期清理、持久化、召回和搜索继续运行。新增状态测试必须同时覆盖“受禁动作不发生”和“非受禁生命周期仍推进”。
- Verification: Go 已把 Frozen 从普通宠物/archer 外层循环移到 follow/target/attack 动作点；回归测试确认宠物保留目标且不移动/攻击、Frozen 期间仍执行 tame 到期广播，archer 仍更新搜索/缓存目标但不创建投射物，相关聚焦测试通过。

### 2026-08-14 — 当前批次收尾后停止的指令优先于长期迁移计划

- Symptom: 用户明确要求“干完这个就不要再做”时，既有计划仍包含分析和实现下一批功能，存在当前提交完成后继续扩展工作范围的风险。
- Root cause: 把长期迁移 Goal 的持续执行惯性当成当前轮默认，没有立即按最新的停止边界收窄计划。
- Prevention: 收到“完成当前批次后停止”指令后，只执行在途批次必需的测试、提交、C# 零变化检查和工作树核验；不得分析或实现下一批，也不得把尚未 100% 完成的整体迁移 Goal 标记完成。
- Verification: 本轮仅收口并提交 ProtectionField、Rage、SwiftFeet 及经验记录，核验两个仓库后停止，没有开启下一批源码修改。

### 2026-08-14 — Observer transcript 新增 helper 后必须立即做包级编译

- Symptom: Observer 会话测试初稿引用了不存在的 UserInformation parser、错误哨兵和未解析的 Name 字段，随后真实 transcript 又暴露了目标连接自身的攻击/远程/施法包顺序与战斗退出锁。
- Root cause: 连续扩展测试时按意图猜测同包 helper 签名，并把观察者视角的转发包误当成目标连接唯一可见包，没有先复用现有 parser、核对 handler 写入顺序和 logout gate。
- Prevention: 新增 Observer 测试先检索整个 package 的 parser/错误值/返回签名，立即运行 `go test ./cmd/crystal-server -run '^$'`；每条多连接 transcript 分别列出目标、自身、观察者和退出 gate 的包矩阵，再写断言。
- Verification: 复用 `parseIntelligentCreatureSessionUserInfo` 并补齐 Name 后，Observer 拒绝、Admin 绕过、切换/generation、退出、持久 toggle、静态/地面对象顺序及服务端整包测试均通过。

### 2026-08-14 — 广播包必须按源 ObjectID 转发给 Observer

- Symptom: 复核 Observer forwarding 时发现把每个 `worldPlayer.Send` 回调都转发给该回调所属玩家的观察者，会把发送给附近接收者的源玩家动作包误投给错误观察者并可能重复。
- Root cause: 把“通知接收者的 Send”误当成“动作源的 Send”，没有区分 `notifyPlayers` 的 recipient 与产生 `ObjectTurn/Attack/RangeAttack/Magic` 的 source。
- Prevention: Observer forwarding 只由动作源路径显式提交带源 ObjectID 的 packet；需要复用 world notification 时先确认源包是否也会发给源连接，禁止在通用 recipient callback 中推断来源。
- Verification: 攻击、远程攻击改为 source `ObserverPacket`，移动和魔法保留 source path 转发；全仓普通/race 测试、vet/build 与 Observer transcript 均通过。
