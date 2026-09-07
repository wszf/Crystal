### 2026-08-15 — GasToad 多目标毒物写回必须先提交 value-map 伤害副本

- Symptom: GasToad Type 2 对宠物的直接伤害已生效，但同一延迟结算后的 Green poison 列表为空、宠物只少了 10 点 HP。
- Root cause: Go `world.monsters` 是 value map；伤害函数修改局部副本后，毒物 helper 从 map 取出并写回了带毒副本，调用方随后又用未带毒的旧伤害副本覆盖了 map。
- Prevention: 延迟多目标处理每次修改 value-map 实体后，先写回权威 map，再调用会再次读取/写回的状态 helper；若还需保存，必须从 map 回读而不是提交旧副本。
- Verification: GasToad Type 2 world transcript 对 Player、Monster 宠物和 Hero 的 HP/Green poison/Elapsed 均稳定通过；全量测试前的定向回归已确认宠物从 100→83 且保留毒物。

### 2026-08-13 — 持久化重试必须分离领域提交与可重复落盘

- Symptom: 未配置账户 JSON 路径时，Conquest 生命周期通知会永久停在 `pendingSave`；连续两次资产保存失败后，重放第一条旧通知还会把 authority HP 从较新的 80 暂时写回旧值 90 再保存。
- Root cause: 把“没有持久化回调”误当成“保存仍未成功”，并把只应执行一次的 authority 状态提交与可重复执行的落盘操作放进同一个 `BeforeSend` 重试闭包。
- Prevention: 可选持久化回调为空时按成功的 no-op 处理；提交后通知拆成一次性领域写入和可重复保存两个阶段，队列重试只能保存当前最新 authority，不能重新应用旧快照。
- Verification: 新增无持久化即时投递测试，以及 90→80 两次失败后重试仍只保存最新 80 的回归测试；Conquest 定向、服务端整包、全量普通/race、vet、build 与差异门禁均通过。

### 2026-08-13 — 提交 tracked diff 不会自动包含未跟踪源码

- Symptom: P11 首次用 `git diff --name-only -z | xargs git add` 暂存时，只提交了已跟踪修改，十个新 Go 源码/测试文件仍留在工作区，提交统计与已通过测试的源码集合不一致。
- Root cause: `git diff --name-only` 默认不列出 untracked 文件，把“当前差异清单”误当成了完整工作区清单。
- Prevention: 提交前以 `git status --short` 为权威清单；明确暂存目标范围时同时处理 `??` 文件，提交后立即再次检查 status 与 `git show --stat`。禁止仅依赖 `git diff --name-only` 构造完整暂存集合。
- Verification: 十个未跟踪 Go 文件已显式暂存并 amend 到同一个 P11 提交，Go 仓库提交后工作区为空，提交包含 43 个文件。

### 2026-08-13 — 通知测试 helper 不得隐式提交所有副作用

- Symptom: 为方便测试而让 tick helper 自动执行所有 `BeforeSend` 后，既有用例无法再断言延迟持久化/创建的通知数量和时序。
- Root cause: 读取状态的 helper 混入了投递副作用，调用者无法选择观察“生成通知”还是“完成投递”两个阶段。
- Prevention: tick helper 只返回通知；另设显式 deliver helper，并在需要时逐项执行 `BeforeSend`。涉及多阶段事务的测试分别断言通知顺序、阶段中间态和最终权威状态。
- Verification: `intelligentCreatureTestTick` 与 `intelligentCreatureTestDeliver` 已拆分，黑石锁序和 stale 快照回归测试可分别验收投递前后状态。

### 2026-08-12 — 关系提交必须分离 world/auth 锁并同步会话快照

- Symptom: 婚姻和导师初版在持有 `world.mu` 时调用 auth 原子事务与离线角色查询，且 session 的 `gameCharacter` 可能在另一会话提交后用旧整角色状态覆盖最新关系字段；导师经验只改了在线 world，登出可能丢失。
- Root cause: 把在线投影、权威持久状态和连接局部快照当作同一份对象，未定义跨层锁顺序和字段级同步边界。
- Prevention: 关系事务统一由独立 `relationshipMu` 串行化，按 world 快照 → 释放 world 锁 → auth 原子提交/查询 → 重取 world 玩家并字段合并执行；禁止同时持有 world/auth 锁。session 层从 world 同步关系/进度字段，导师临时经验在登出前原子转入 auth，关系功能只合并其拥有的字段。
- Verification: 关系 world/auth 定向测试、双会话 net.Pipe 婚姻/导师完整 transcript、导师登出/到期经验结算和 Go 全量 race 门禁用于验证。
- Strengthening after review: 不得在持有 `world.mu` 时读取 auth 的离线关系记录；先复制角色索引/等级并释放 world 锁，再查 auth，重取 world 玩家并校验快照未漂移后提交。导师奖励必须在 `SaveJSON` 前完成 world/auth 经验变更，登录 bootstrap 严格拆成 Lover → 到期 Chat/MentorUpdate/奖励（或普通 MentorUpdate），到期只在登录检查；学生升级后再执行等级差自动解除。C# 默认 unchecked 的 `uint` 乘法、`long` 加法和 `long → uint` 转换也必须按位宽回绕，不能自行改成饱和运算。

### 2026-08-18 — AI=177 unified diff 上下文必须保留闭合行标记

- Symptom: FrozenKnight 远程测试的一次 patch 因 unified diff 末尾闭合行缺少上下文前导空格被拒绝，文件没有部分写入。
- Root cause: 手写 patch 时把源码闭合行当成普通文本，遗漏了 apply_patch 所需的上下文标记。
- Prevention: 每个 hunk 提交前逐行检查首字符：上下文为空格、新增为 `+`、删除为 `-`；失败后先复读目标文件和 diff，再拆成短 hunk 重做。
- Verification: 小 hunk 重做后目标测试文件只包含预期修改，`gofmt` 和包级编译通过。

