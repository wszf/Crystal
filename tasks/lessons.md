# Active lessons

Read this file at the start of every session. It contains only canonical,
cross-cutting prevention rules. Historical feature-specific evidence is stored
under [`tasks/lessons-archive/`](lessons-archive/README.md); do not read that
directory wholesale.

Before repeating a workflow or starting a migration batch, search the archive
with the AI ID, entity/module name, and failure/workflow keyword, then read only
matching sections. Keep active lessons within the recommended limit of 50 KB
or 500 lines. Strengthen an existing canonical lesson instead of adding a
duplicate.

### 2026-08-21 C01 — 每次工具调用必须保持单仓库路径闭包

- Symptom: Go/Legacy 路径混入同一调用，出现部分成功输出、尾部失败或错误目标。
- Root cause: 把工作目录、仓库根和参数列表分开考虑，并复用了另一仓库的路径。
- Prevention: 一次调用只属于一个仓库；先核验 `git rev-parse --show-toplevel`，参数只允许该根下已存在的路径；切换仓库必须新开调用；不得在当前 workdir 中用 `git -C`、绝对路径或脚本参数指向另一仓库；不得以只读、并行或减少往返为例外。
- Verification: 命令零退出且所有路径属于同一根；任一读取失败、非零退出或混合根调用时，丢弃该调用的全部输出（包括前面成功的片段），不得用于实现、测试归因或文档。本轮跨仓库 status 审计因混入另一根路径作废，随后已拆成两次单仓调用重跑；本批一次 Legacy workdir 混入 Go 文件路径的只读调用同样整体作废，随后按仓库拆开重跑；本轮 Legacy 方向核对命令再次混入 Go 相对路径，整次输出作废，随后按仓库边界重跑。 本轮一次 Legacy 读取调用误附 Go 相对路径，整次输出再次作废并已拆分重跑；一次委派消息误将已选 AI=8 写成 AI=80，相关 AI=80 tracing 已明确丢弃并按 AI=8 重做。

### 2026-08-21 C02 — 路径、glob、正则和 shell 字符串必须先做最小验证

- Symptom: 猜测目录、空 glob、裸反引号、错误正则或未闭合字符串导致勘察失败。
- Root cause: 依赖 shell 隐式展开和记忆中的文件布局，没有先验证最小查询。
- Prevention: 先用 `rg --files` 列精确文件；优先 fixed pattern 或显式 `-e`；引用正则并检查字符串、反引号和参数边界；所有 `rg` 选项必须放在 `--` 前，`--` 后只能放 pattern/路径；禁止未引用 glob，也禁止把换行文件列表放进 zsh 标量命令替换后期待自动分词；shell 变量不得使用 `PATH`/`path` 等环境保留名或 zsh 只读特殊参数（如 `status`）；多文件列表直接用 `rg --glob`，或用 NUL 分隔加 `xargs -0`；调用 CLI 子命令前按对应 `--help` 核对全局与子命令选项位置；数据库对象名必须从实际 schema 复制，禁止查看 schema 后仍使用惯例名称猜测；调用语言模块前先核对运行时版本/可用性，并优先让目标程序自身解析配置。
- Verification: `rg` 选项、`path` 覆盖 `PATH`、未命中 glob 和 zsh 标量命令替换均已最小重跑；本批 archive 检索从失败的裸 `*.json`/换行标量改为直接 `rg --glob` 后零错误完成；Python 3.9 缺少 `tomllib` 时改由实际 Codex CLI 解析配置；Codex 验证脚本将只读 `status` 改为 `rc`，并把全局 approval 选项移到 `exec` 前后成功执行；Goal 数据库查询从猜测的 `goals` 改为 schema 中实际的 `thread_goals` 后成功核对字段约束；本批从 `cmd/crystal-server` 子目录误用根级 `./cmd/crystal-server` 失败后，改用 `go test .`，并将仓库根/命令包路径作为同一最小验证。

### 2026-08-21 C03 — 补丁必须使用精确、唯一、小范围上下文

- Symptom: patch 被拒绝、落到相似函数、部分 hunk 成功或格式化后锚点失效。
- Root cause: 使用陈旧正文、模糊锚点或人工拼装错误 hunk 标记。
- Prevention: patch 前复读精确物理行；按唯一函数锚点拆小 hunk；检查完整路径、上下文行和闭合标记。
- Verification: 逐段复读 diff，并运行格式化、最小编译和定向测试；任一 patch 失败时不采用同调用的其他结果。

### 2026-08-21 C04 — C# 基线只读，语言工具链严格隔离

- Symptom: 迁移或格式化流程误触 `.cs`，或用错误工具链验证不同语言。
- Root cause: 把 Legacy 对照和 Go 迁移实现当成同一可编辑工作区。
- Prevention: 所有迁移实现和工具使用 Go；`.cs` 只读；格式化、编译、patch 和提交按语言及仓库分组。
- Verification: 两仓分别检查 tracked、staged、untracked `.cs`，结果必须为空。

### 2026-08-21 C05 — 复用 API 前核对完整声明而非猜测对称名称

- Symptom: helper 不存在、receiver 遗漏、返回值数量错误、字段或常量名称猜错。
- Root cause: 依据相似模块、Legacy 名称或“应该对称”推断 Go API。
- Prevention: 先读取声明、receiver、参数顺序、返回值、领域类型和包级符号，再接线。
- Verification: 新调用接入后立即运行包级只编译门禁；本批测试夹具误写不存在的 `worldMagic.Spell` 字段后由编译器立即拦截，复读声明并改用实际 `Level` 字段后定向测试通过。

### 2026-08-21 C06 — 行为判断前先通过 Go 语法、类型和 vet 门禁

- Symptom: 未使用变量、自赋值、类型宽度、多返回值或复合字面量错误阻止行为测试。
- Root cause: 一次写入过多逻辑，在编译失败时仍试图分析生产语义。
- Prevention: 小步运行 `gofmt` 和 `go test ... -run '^$'`；显式转换不同领域类型，再进入行为测试。
- Verification: 最小编译、定向测试和 `go vet` 分层通过。

### 2026-08-21 C07 — 全量、race 和环境失败必须按实际栈归因

- Symptom: 新批次被既有 session/race 问题、缓存、磁盘或超时噪声误判为回归。
- Root cause: 只看最终 `FAIL`，没有保留退出码、测试名、栈和定向复跑结果。
- Prevention: 分开运行定向、普通全量和 race；保存退出码和失败摘要；环境错误先检查空间与可重建缓存。
- Verification: 只以实际失败栈是否进入本批代码或测试为归因依据，已知排除项写入交接而不修改无关模块。

### 2026-08-21 C08 — 测试夹具必须复用真实 bootstrap、helper 和认证约束

- Symptom: 生产分支未执行，测试先因缺少 runtime 实体、派生属性、元数据或名称约束失败。
- Root cause: 人工夹具只建立持久数据，未复现登录、world enter、summon 或 bootstrap 生命周期。
- Prevention: 优先复用现有 helper；按 seed→load→enter→summon 顺序建立状态；满足真实账号、角色和配置约束。
- Verification: 先确认夹具到达目标生产入口，再断言行为和 transcript。

### 2026-08-21 C09 — Go value-map 实体修改后必须显式写回并回读

- Symptom: 局部副本中的 HP、毒物、计时器已改变，但 world 状态未变或被旧副本覆盖。
- Root cause: 把 `map[ID]Struct` 当作引用容器，多个 helper 交错读写旧副本。
- Prevention: 每阶段修改后立即写回；调用会重读 map 的 helper 前先提交；后续保存从权威 map 回读。
- Verification: 同时断言权威 map、下一 tick 行为和会话可观察结果。

### 2026-08-21 C10 — 持久层、runtime、动作队列和 wire 编码必须分层建模

- Symptom: owner key 与 ObjectID、持久目标类型与动作编码、绝对时间与剩余时间被混用。
- Root cause: 同一概念在不同层具有不同 ID、枚举、默认值和派生规则。
- Prevention: 明确 storage→runtime→queued action→wire 的转换边界，不跨层推断字段。
- Verification: 分别断言持久快照、运行实体、队列内容和 payload。

### 2026-08-21 C11 — 随机测试必须复现完整调用流和精确区间

- Symptom: 遗漏 `Next(1)`、混淆排他/包含上界，或初始化、后台和延迟阶段额外消费随机数。
- Root cause: 只按业务分支记录随机，没有展开嵌套 helper、构造、命中和防御抽样。
- Prevention: 按调用栈列出每次随机调用、顺序、阶段和 bound，并使用记录型确定性源。
- Verification: 分阶段断言随机序列，并重复普通/race 测试确认稳定；AI=77 HellPirate 的延迟命中还核对了固定范围 `Random.Next(1)`，确定性 hook 不再把合法的 unit-bound 消费误报为失败。

### 2026-08-21 C12 — 人工时钟必须隔离所有后台 tick 和全局时间源

- Symptom: 手工 tick 前 session loop 已移动、攻击、刷新状态或消费随机数。
- Root cause: 停止 world ticker 被误认为同时停止连接维护循环和其他实体 AI。
- Prevention: 冻结 ticker、session loop、独立实体计时和全局时间源；必要时等待 goroutine 完全退出。
- Verification: 重复/race 运行只出现人工推进产生的事件和随机调用。

### 2026-08-21 C13 — 延迟动作必须区分 admission、snapshot、impact 和后续 tick

- Symptom: 发起时状态被错误用于命中，或命中时新增 Legacy 不存在的距离、资格或状态门禁。
- Root cause: 没有明确哪些值在 admission 捕获、哪些在 impact 重验、哪些在后续 tick 生效。
- Prevention: 为每个延迟动作列出快照字段、命中重验项、Due 边界和动作后状态。
- Verification: 测试覆盖入队后状态变化、精确 Due、命中结果、后续 tick 和动作清理。

### 2026-08-21 C14 — 构造器、对象身份、population 和生命周期必须单独审计

- Symptom: ObjectID 冲突、自引用、错误方向/计时器、未召唤 runtime 对象或旧夹具被新 AI 污染。
- Root cause: 手工插入对象绕过构造器、`nextObjectID`、Spawn 初始化和 population 注册。
- Prevention: 复用真实构造/Spawn；手工插入时推进 ID 并初始化完整可观察状态；新增 AI 同步 population。
- Verification: 分别断言构造后、首次 tick、攻击后及死亡/清理状态。

### 2026-08-21 C15 — 坐标、方向、距离和地图布局必须从夹具计算

- Symptom: 手写距离、方向 ordinal、x/y 顺序或推退方向与实际算法不一致。
- Root cause: 依据视觉直觉，没有使用项目的距离、方向表、地图布局和边界规则。
- Prevention: 从攻击者、目标和地图尺寸计算几何；区分移动向量、最终朝向和 payload 方向。
- Verification: 同一计算变量驱动动作时间、位置终态和 wire 断言。

### 2026-08-21 C16 — 数值期望必须代入真实公式、派生属性和类型宽度

- Symptom: HP、MP、伤害、费用、持续时间或随机 bound 按直觉计算错误。
- Root cause: 忽略等级、派生属性、钳制、中间状态、协议宽度或当前库存。
- Prevention: 逐项代入生产公式，明确单位和整数类型，并从夹具真实字段推导。
- Verification: 同时断言中间值、最终值和对应通知，而不是只检查单个结果。

### 2026-08-21 C17 — 协议变更必须同步 ordinal、布局、方向和完整 payload

- Symptom: ordinal 基线、字段长度、包方向或尾字段更新不完整。
- Root cause: 从 packet ID 或相似包推断 wire，没有读取完整 enum 和 serializer。
- Prevention: 以目标 wire 基线和当前常量表为准；核算字段宽度和完整帧；使用非零语义哨兵。
- Verification: ordinal 向量、编码/解析 round-trip、固定 payload 和双向 transcript 全部通过。

### 2026-08-21 C18 — Transcript 必须验证完整事件顺序和所有副作用

- Symptom: 主行为正确但漏掉 bootstrap、练习、状态、聊天、移动或延迟包。
- Root cause: 把领域成功或一个响应包当作整个动作的完成屏障。
- Prevention: 沿实际入队和广播点列出完整有序事件；禁止 map 驱动有序期望；保留中间状态。
- Verification: 逐包核对数量、顺序、payload 和最终领域状态。

### 2026-08-21 C19 — 通知必须按接收者和可见性矩阵验收

- Symptom: 私有包、目标包、观察者广播或跨地图移除发送给错误对象。
- Root cause: 只验证事件存在，没有展开 fan-out、源坐标和接收者边界。
- Prevention: 建立 actor/target/owner/observer/其他地图矩阵，并区分私有与公开 payload。
- Verification: 每个接收者独立消费 transcript，同时确认不应接收者没有包。

### 2026-08-21 C20 — net.Pipe 测试必须建立 reader、就绪屏障并消费完整前导

- Symptom: 服务端先写导致阻塞，bootstrap 残留包抢占预期响应，或 cleanup 死锁。
- Root cause: 假设请求后才有响应，或把最后一个 bootstrap 包当作稳定 game loop。
- Prevention: 写入前启动所有 reader；并发消费多接收者；使用 post-bootstrap 屏障；异步 callback 不直接调用 `Fatal`。
- Verification: 普通、重复和 race 模式均无阻塞，包数与接收者矩阵一致。

### 2026-08-21 C21 — 迁移必须沿真实 Legacy 调用链、动态类型和 override

- Symptom: 按名称、注释、陈旧矩阵或相似实现迁移，遗漏重载、尾部副作用或 Legacy 怪癖。
- Root cause: 读取声明但没有追踪 Spawn、调用者、helper 和消费者。
- Prevention: 从真实入口追到构造类型、override、共享 helper 和所有消费者；当前源码与测试优先于文档。
- Verification: 用生产入口测试覆盖可达路径、历史怪癖和关键失败分支。

### 2026-08-21 C22 — 不同 capability、门禁和业务阶段不得过度复用

- Symptom: 搜索 gate 被用于攻击、权限被误认为绕过冷却，或特殊分支绕过通用门禁。
- Root cause: 把相似布尔条件合并为一个过宽 helper。
- Prevention: 分别建模搜索、移动、攻击、可见性、权限、冷却和状态门禁；复用前证明语义集合完全相同。
- Verification: 为每个 gate 提供独立成功/失败组合测试。

### 2026-08-21 C23 — 权威事务、锁、落盘和网络投递必须分层排序

- Symptom: 网络失败阻断持久化，跨领域锁死锁，或重试重复领域副作用。
- Root cause: 把状态提交、序列化和尽力通知放在同一锁内或重试循环中。
- Prevention: 锁内原子更新领域状态；释放锁后落盘；最后投递网络；重试只重复可幂等步骤。
- Verification: 覆盖失败回滚、断线保存、批量通知继续和 race 门禁。

### 2026-08-21 C24 — 跨 world/auth/session 状态必须以 authority 和 revision 同步

- Symptom: 权威状态已变，但在线 session 使用陈旧成员关系、物品或地图快照。
- Root cause: 直接覆盖整快照，或在错误 authority/锁域内判定。
- Prevention: 明确字段 authority；分离锁；按 revision 定向合并并主动唤醒目标会话。
- Verification: 跨会话测试确认权威存储、在线快照和双方通知一致。

### 2026-08-21 C25 — 导入导出和 round-trip 必须验证语义规范化

- Symptom: 字段解码成功但加载语义错误，或 nil/empty、map、时间表示造成伪差异。
- Root cause: 把 schema 对称等同于领域加载等价。
- Prevention: 逐字段核对 Legacy 加载顺序、默认值和 canonical 空集合；不可比较结构逐字段或 DeepEqual。
- Verification: Legacy→Go→持久化 round-trip 与截断、默认和非零哨兵测试通过。

### 2026-08-21 C26 — Schema、配置或常量扩展必须更新完整契约面

- Symptom: 生产字段已增加，但 defaults、JSON、Setup.ini、导出器或完整结构断言失败。
- Root cause: 只修改结构体或 loader，没有检索所有消费者和夹具。
- Prevention: 同步 schema、默认值、loader、runtime fallback、导出器和兼容性测试。
- Verification: 相关内部包定向测试及全仓测试共同通过。

### 2026-08-21 C27 — 测试判据必须来自生产入口和当前权威状态

- Symptom: 参考模型通过但生产入口未覆盖，或测试读取旧副本、陈旧矩阵和手写常量。
- Root cause: 测试复制实现，没有观察真实入口、最终 map 状态和可观察协议。
- Prevention: 通过生产入口驱动，并从当前源码、夹具坐标、公式和权威实体推导期望。
- Verification: 同时验证领域终态、持久状态和网络 transcript。

### 2026-08-21 C28 — Active lessons 只保留可执行、跨批次规则

- Symptom: 单次异常、某个 AI 数值或已知失败栈不断成为固定汇报项，且复合 AI 标题只索引首个 ID，导致上下文膨胀或检索漏项。
- Root cause: lessons 同时承担事故日志、交接记录和长期规则；归档职责不清，索引器把 `AI=52/53` 当成单 ID。
- Prevention: 复发项强化同一 canonical；功能特定或历史事实进入对应 archive 文件；已归档证据只追加不删除，冻结的 legacy 分片不得改写；manifest 的 `ai_ids` 必须收录复合标题中的每个 ID；active 达到 50 KB 或 500 行前归档。
- Verification: active 每条规则跨批次可用且通过大小门禁；manifest 可按任一复合 AI ID 定位冻结原始块，831 个 legacy 历史块仍可无损重建。

### 2026-08-22 C29 — compact 前必须完成并校验 durable handoff

- Symptom: 自动 compact 摘要遗漏当前未提交批次、仓库状态或失败归因，恢复后的信息与预期不一致。
- Root cause: 把 compact 摘要当成迁移记录，或只在“快要 compact”之后才补 handoff，导致 compact 前没有可核验的完整状态边界。
- Prevention: 每次 compact 前，或收到 compaction/上下文上限/rollover 信号时，立即停止实现和测试，先写/刷新 `tasks/migration-handoff.md`；即使当前批次只有 Markdown/文档变更也必须执行。记录两仓路径、分支/HEAD、完整 tracked/staged/untracked 状态、所属文件、测试退出码与失败归因、矩阵行、未提交工作和恢复命令；回读并对照两仓校验后再 compact。compact 后沿用同一 active Goal，不因 compact 单独重开 Goal；自动摘要仅作不可信上下文。
- Verification: `agents.md`、`tasks/goal-task.md` 和 `tasks/migration-handoff.md` 均将其定义为 hard gate，并要求无 handoff 时先从两仓重建记录再继续。
