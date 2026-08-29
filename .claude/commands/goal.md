# Crystal migration Goal command

执行一次可恢复的 Crystal Go 迁移工作周期，不把本命令当作 Goal 完成证明。
`$ARGUMENTS` 只能作为当前 Leaf 内的有限指令，不能改变 Goal 边界。

1. 依次只读读取 `agents.md`、`tasks/lessons.md`、`tasks/goal-task.md`、`tasks/migration-active.md` 和 `tasks/migration-handoff.md`。
2. 分别核对 Legacy 与 Go 仓库的根目录、分支、HEAD、tracked/staged/untracked 状态；保留既有修改。
3. 只读取 active index 指定的 Go 迁移矩阵锚点和相关 Legacy/Go 源码；不要整体读取矩阵、历史 handoff archive 或宽泛源码树。
4. 恢复同一 Goal 的 Primary Active Leaf，先冻结有限行为清单，再开始生产实现；不得因本命令重开 Goal 或已完成 Leaf。
5. Goal 只约束 Leaf 边界、证据和质量门禁；Agent 身份和固定数量不需要登记，由 Ultracode 按依赖、资源和 authority 冲突动态拆分 bounded workstreams。
6. 并行 workstream 必须声明 Leaf、读写文件、authority 锁、依赖、禁止范围和验收证据；共享或耦合写入必须串行或隔离。
7. Legacy 的所有 `.cs` 永久只读；迁移实现、测试客户端、探针和工具必须使用 Go；测试不得替代生产代码。
8. 通过生产入口完成 focused/repeated/race 验证；按 Goal 契约执行集成和阶段门禁，失败时记录实际命令、退出码和归因。
9. 控制面变化后运行 `tasks/check-migration-control.sh`；完成 Leaf 后更新矩阵、active index、handoff，并只提交明确拥有的文件。
10. 默认不 push、不合并 `main`；不要重开已 Complete 的 Leaf 或跨入其他 Leaf 的业务生命周期。
