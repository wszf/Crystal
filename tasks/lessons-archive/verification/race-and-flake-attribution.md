### 2026-08-15 — net.Pipe AI transcript 必须显式隔离认证与后台 ticker 状态

- Symptom: AI=131 会话夹具先因账号/角色名超出 15 字符限制登录失败，随后默认登录 HP 只有 18，后台 ticker 还抢先消费了 AI 的 3000ms 搜索随机数。
- Root cause: 测试把领域名称和登录体力当成无约束值，并在服务启动前没有冻结已加载怪物的 AI 初始化/时间状态。
- Prevention: 真实会话 fixture 使用认证正则允许的短标识；bootstrap 后读取实际协议 MP，再显式设置 world 权威 HP；启动服务前将怪物初始化并把搜索/动作时间置于未来，停止 ticker 后才注入确定性目标和时钟。
- Verification: TucsonGeneral Rage transcript 现稳定锁定登录后的 Rage、15 个岩石、两次命中和移除包序，连续定向运行通过。

### 2026-08-11 — net.Pipe 广播必须并发消费所有接收者

- Symptom: 玩家 PvP 端到端 transcript 在服务端广播攻击和死亡包时挂起。
- Root cause: `net.Pipe` 没有缓冲；服务端向多个连接顺序写广播包时，尚未被读取的接收者会阻塞后续写入，单独消费一个连接无法推进整个广播。
- Prevention: 为每个 net.Pipe transcript 列出所有接收者和完整包序列；涉及广播时为每个连接启动并发 reader，或先建立等价的消费屏障，再等待 handler 完成。
- Verification: `TestSessionPlayerMeleePvPTranscript` 并发消费目标连接的 7 个广播包，PvP 定向测试通过；提交前继续执行 race 测试。

### 2026-08-14 — 手工 world tick 测试必须隔离后台 ticker 并使用会话屏障

- Symptom: FrostCrunch 三会话测试手工推进命中/到期 tick 时，100ms 后台 world ticker 可能抢先发布状态；Frozen 攻击测试读到入口先发送的 `UserLocation` 后立即检查 world，又可能早于真正的 admission 处理。
- Root cause: 确定性 synthetic tick 与生产后台 ticker 同时驱动同一世界，且把 handler 中间包误当成请求完成信号；`net.Pipe` 的包到达只证明该次写入完成，不证明后续领域逻辑已结束。
- Prevention: 需要手工时间轴的会话测试在状态创建前停止该 world 的测试 ticker，并确认可能已选中的空 tick 返回；读取入口中间包后再发送并消费 `KeepAlive`，用后续请求作为所属会话完成屏障，再检查共享状态或断言没有广播。
- Verification: 后台 ticker 隔离后，三会话 FrostCrunch transcript 稳定锁定伤害、两条 Chat、Slow/Frozen 广播、Frozen 拒绝以及逐步清除；Frozen 攻击后的 KeepAlive 屏障确认方向和动作队列均未变化。
- Strengthening after recurrence: 毒状态的 `Elapsed` 按实际处理事件次数递增，不会因一次 synthetic tick 的墙上时间跨越而补齐多个周期；到期测试必须按递增且严格晚于 `TickAt` 的每个 tick 逐步驱动，再单独执行清除广播阶段。
- Verification after recurrence: AI=95 FlameAssassin 首次到期测试把 8 秒跳跃误当成 8 次处理，仅得到 `Elapsed=2`；改为逐秒、递增纳秒边界的 8 次 tick 后，Slow 到期与 `Poisoned/ObjectPoisoned(None)` 顺序通过。

### 2026-08-15 — 手工 world tick 停止 ticker 必须等待 goroutine 退出

- Symptom: 全仓 race 门禁曾在 FrostCrunch 会话的合成时间轴中看到 Frozen 清除通知为空；单测偶发通过。
- Root cause: 测试只关闭后台 ticker 的 stop channel 并固定 sleep，没有确认 ticker goroutine 已退出；真实时间 tick 可能在合成时间轴 tick 前处理 1970 年的毒状态。
- Prevention: world ticker 暴露仅供内部同步的完成 channel；手工时间轴 fixture 关闭 ticker 后等待该 channel，再调用 synthetic `world.tick`，禁止用固定 sleep 充当 goroutine 完成屏障。
- Verification: FrostCrunch 普通与 race 定向测试各连续 10 次通过；提交前重新执行全仓 race 门禁。

### 2026-08-16 — race 会话夹具的共享 AI 配置必须在 world.mu 下更新

- Symptom: `go test -race ./...` 检测到 FlyingStatue/GasToad session 测试直接改写 `monsterAIEnabled`/`monsterAIRoll`，后台连接循环同时在 `world.tick` 中读取；GasToad 在 race 变慢时还会让后台 tick 先消费一次性攻击。
- Root cause: 停止维护 ticker 不会停止每个连接的请求维护循环；测试把共享 AI 配置当成本地字段，并把人工基准时间设在墙钟当前时刻。
- Prevention: 所有在线 session 的共享 AI 注入通过持有 `world.mu` 的 helper 完成，AI 开关写入同样加锁；人工 transcript 时钟至少领先墙钟一小时，避免连接循环抢先处理未来动作。
- Verification: FlyingStatue/GasToad session 在 `go test -race` 下连续 10 次通过；随后将重跑全量 race 门禁。

### 2026-08-17 — race 下人工 session 时间轴必须冻结光照时钟

- Symptom: SwiftFeet 到期的合成 `world.tick` 在 race 下收到 `ServerTimeOfDay`（ID 61）而不是预期的 `ServerRemoveBuff`，单个领域状态本身没有错误。
- Root cause: 停止后台 ticker 只等待 ticker goroutine 退出，仍保留 `lightsEnabled`；人工到期时间与墙钟光照区间不同，在线世界会把全局光照变化插入精确 transcript。
- Prevention: 停止维护 ticker 后，在每个使用人工时间轴的在线 fixture 中用 `setLightClock` 固定一个稳定时刻，再驱动 synthetic tick；不能把 ticker 停止当成光照副作用关闭。
- Verification: SwiftFeet 定向普通/race、Go 全量 `go test -race ./...`、`go vet ./...` 与 `go build ./...` 均通过，且没有额外 TimeOfDay 包。

### 2026-08-17 — race 门禁前检查可重建缓存空间

- Symptom: AI=155 首次全仓 `go test -race ./...` 在链接主服务测试二进制前因 `no space left on device` 失败，未执行到测试断言。
- Root cause: 主机数据卷仅剩约 186 MiB，累积的 Go build/test cache 占用了可回收空间。
- Prevention: 将编译/测试链接失败先分类为环境资源错误；在代码诊断前读取 `df -h`，仅清理可由 Go 重新生成的 build/test cache，再重跑同一 race 命令。
- Verification: 清理后可用空间恢复到约 6.1 GiB，`go test -race ./...` 完整通过；普通测试、vet 和 build 也均通过。

### 2026-08-18 — 全仓 transcript 失败必须先单测隔离再归因

- Symptom: AI=212 批次首次全仓 `go test -race`，以及 AI=214 对象分派补丁后的全仓普通测试，偶发失败 `TestSessionTucsonMageNormalAttackTranscript`，报告未收到预期攻击包；对应 AI 测试没有失败。
- Root cause: 目前证据只显示并行全仓运行中的 session transcript 抖动，不能据此把失败归因到 PurpleFaeFlower、SepWarrior 或普通宠物对象分派；同一 Tucson transcript 单独运行可通过。
- Prevention: 全仓普通/race 出现单一 transcript 失败时，先以完整测试名隔离并重复运行，再用明确的已知 flaky 排除项重跑门禁；不要为无调用关系的 AI 修改生产代码。
- Verification: Tucson 单测普通与 race 定向重跑通过后，再用 Oma/Tucson 两条已确认排除项完成全仓 race 复核；AI=214 定向普通/race 与其余全仓门禁保持通过。
- Further evidence: AI=216 的一次全包普通运行再次只失败 `TestSessionOmaMageRangeSlowFrozenTranscript`，随机边界为 `[2 1]` 而非 `[1]`；AI=216 定向测试本身全部通过，未出现相关行为回归。
- Prevention strengthening: 将该 OmaMage transcript 与 Tucson transcript 一并作为已确认 flaky 排除项；先隔离验证，再使用同时排除两者的普通/race 全仓命令作为批次门禁证据。
- Verification after strengthening: AI=216 后续定向普通/race、排除两条已知 transcript 的全仓普通/race、vet 和 build 均需分别通过。

### 2026-08-18 — 延迟传送会话测试要停止后台 ticker 并固定成功随机源

- Symptom: Blink 会话测试在手动冲击 tick 后偶发拿到空通知集；即使停止 world ticker，未固定随机源时仍可能没有迁移包。
- Root cause: 真实会话启动 ticker 与连接维护 tick 会竞争消费 200ms 延迟动作；Blink 0 级成功门槛还依赖 `roll(4) == 0`，默认随机源会合法地拒绝传送。
- Prevention: 会话测试在 bootstrap 后停止 ticker，再用确定性 `combatRoll` 覆盖成功分支；手动 tick 前确认动作仍在队列，避免把时序或随机性误判为业务回归。
- Verification: 停止 ticker 并固定 roll 后，Blink `net.Pipe` 会话测试连续 5 次通过，且仍验证 `MapChanged -> ObjectEffect -> AddBuff` 顺序。

### 2026-08-19 — 全包 race 失败必须按栈区分既有共享 fixture 竞争

- Symptom: 本批再次运行 `go test -race ./cmd/crystal-server -count=1 -timeout=5m`，既有 `TestGuildBuffSessionNewbieLoginReplacesStalePersistedBuff` 与 `TestSessionBlinkTranscriptIncludesDelayedMapChangeEffectAndBuff` 分别报告 `reconcileEquipmentSpecialBuffsLocked` 写入和测试/通知侧 `intelligentCreatureBuffByType` 读取同一运行时对象；普通全包测试、AI=86 定向 race、vet、build 均通过。
- Root cause: session ticker goroutine 与这些测试在未统一锁边界的共享玩家/生物状态上并发读写；race 栈不经过 AI=86 文件或其延迟动作 resolver。
- Prevention: 每批先跑新增 slice 的 `-race` 定向门禁，再逐条审阅全包 race 的完整栈和测试名称；把 ticker/通知读取与 `intelligentCreatureBuffByType` 的共享对象竞争作为既有 fixture 隔离项单独排期，不把无关 race 归因到新功能，也不把全包 race 记为通过。
- Verification: `go test -race ./cmd/crystal-server -run ManectricClaw -count=1 -timeout=5m` 通过；`go test ./... -count=1 -timeout=5m`、`go vet ./...`、`go build ./...` 通过；全包 race 的两条失败栈均定位到 `player_spell_buffs.go:738` 与 `intelligent_creature_items.go:549`，本批未改动相关代码。
- Strengthening after recurrence: AI=87 批次再次运行全包 race 时，`TestGuildBuffSessionNewbieLoginReplacesStalePersistedBuff` 重现同一 `player_spell_buffs.go:738` 写入与 `intelligent_creature_items.go:549` 读取竞争；新增 AI=87 定向 race 仍通过，且栈不经过 ManectricBlest。
- Verification after recurrence: 本次全包 race 仍只作为已知失败记录，不宣称通过；AI=87 定向 race、普通全包测试、vet 和 build 均通过，相关共享 fixture 未在本批改动。
- Strengthening after AI=88 recurrence: 本批全包 race 再次复现上述 GuildBuff 竞争，并新增同一共享状态竞争在 `TestSessionBlinkTranscriptIncludesDelayedMapChangeEffectAndBuff` 的 `intelligentCreatureBuffByType` 读取栈中出现；两条写入栈仍指向 `reconcileEquipmentSpecialBuffsLocked`/`player_spell_buffs.go:738`，没有进入 ManectricKing 实现。
- Verification after AI=88 recurrence: `go test -race ./cmd/crystal-server -run ManectricKing -count=1 -timeout=5m` 通过；普通 `go test ./... -count=1 -timeout=5m`、`go vet ./...` 与 `go build ./...` 通过；全包 race 保持既知失败，不宣称通过，继续按共享 session fixture 隔离项排期。
- Strengthening after AI=51 recurrence: 本次全包 race 再次复现 `TestGuildBuffSessionNewbieLoginReplacesStalePersistedBuff` 的 `player_spell_buffs.go:742` 写入与 `intelligent_creature_items.go:549` 读取竞争，并新增既有 `TestSessionKirinIceThrustTranscript` 对 `monsterAIRollLocked`/测试随机回调的未同步读写；失败栈均未进入 HedgeKekTal 实现或其 session transcript。
- Verification after AI=51 recurrence: `go test -race ./cmd/crystal-server -run 'HedgeKekTal' -count=1 -timeout=300s` 通过；全包 race 继续按既有共享 session fixture 失败处理，不宣称通过。

### 2026-08-19 — AI=89 收尾仍需记录全包 race 的新增既有 fixture 栈

- Symptom: `go test -race ./cmd/crystal-server -count=1` 在 AI=89 代码不相关的 `TestSessionDarkBodySpawnAndRecallTranscript`、`TestGuildBuffSessionNewbieLoginReplacesStalePersistedBuff` 失败；前者新增 `cloneProtocolCharacterBuffs` 读取与后者已知的 `intelligentCreatureBuffByType` 读取，均与 ticker 的 `reconcileEquipmentSpecialBuffsLocked` 写入竞争。
- Root cause: session ticker goroutine 与测试/通知侧读取共享玩家 Buff slice 时没有统一锁边界；race 栈没有进入 IcePillar 或其直接特效路由。
- Prevention: 每批继续运行新增 AI 的定向 race，并按完整 race 栈和测试名分类；若栈只落在既有 `player_spell_buffs.go`/`intelligent_creature_items.go`，记录为共享 fixture 隔离项，不将全包 race 误报为通过或把无关修复带入当前批次。
- Verification: IcePillar 定向 race 通过；普通 `go test ./cmd/crystal-server -count=1`、`go test ./...`、`go vet ./...`、`go build ./...` 通过；本次全包 race 的失败栈固定在上述既有 Buff 读写路径。
- Strengthening after AI=84 recurrence: 本批 `go test -race ./cmd/crystal-server -count=1 -timeout=5m` 再次复现 `TestGuildBuffSessionNewbieLoginReplacesStalePersistedBuff` 在 `player_spell_buffs.go:738` 与 `intelligent_creature_items.go:549` 的共享 Buff 竞争，并新增 `TestSessionHidingTranscriptPersistenceAndExpiry` 在 `player_spell_buffs.go:738` 与 `equipment_transactions.go:778` 的 ticker/装备统计竞争；两条栈均未进入 WingedTigerLord。
- Verification after AI=84 recurrence: AI=84 定向 race、普通 `go test ./... -count=1 -timeout=5m`、`go vet ./...` 和 `go build ./...` 通过；全包 race 保持已知共享 session fixture 失败，不宣称通过。

### 2026-08-20 — AI=66 全包 race 失败仍需归因到既有 session 夹具

- Symptom: AI=66 完成后的全包 `go test -race ./cmd/crystal-server -count=1 -timeout=600s` 仍失败，复现 `TestGuildBuffSessionNewbieLoginReplacesStalePersistedBuff`、`TestSessionKirinIceThrustTranscript` 与 `TestSessionBlinkTranscriptIncludesDelayedMapChangeEffectAndBuff` 的数据竞争；AI=66 定向 race 未失败。
- Root cause: 失败栈分别落在既有 GuildBuff/装备 buff 共享状态、Kirin transcript 随机回调和 Blink map-transition buff ticker 的并发读写，不在 CrazyManworm 生产路径或其测试。
- Prevention: 每批迁移同时保留目标定向 race 与全包 race；全包失败时按测试名和 race 栈归因，只有栈指向本批代码才修改生产实现，否则在 handoff 中明确记录为既有门禁问题。
- Verification: `go test -race ./cmd/crystal-server -run 'CrazyManworm' -count=3 -timeout=300s` 通过；普通全包、全仓库测试、`go vet ./...` 与 `go build ./...` 也通过；重跑的全包 race 日志稳定命中上述既有测试。

### 2026-08-20 — AI=65 全包 race 仍需按实际失败集合归因

- Symptom: AI=65 批次的全包 `go test -race ./cmd/crystal-server -count=1 -timeout=600s` 失败，当前重跑命中 `TestGuildBuffSessionNewbieLoginReplacesStalePersistedBuff` 与 `TestSessionKirinIceThrustTranscript`；AI=65/66 定向 race 通过。
- Root cause: race 栈仍落在既有 GuildBuff/装备 buff 共享状态与 Kirin transcript 随机回调竞争，未进入 MutatedManworm 生产路径或新测试。
- Prevention: handoff 必须记录本次实际失败测试集合，不用上批 race 结果替代本批证据；继续分离目标定向 race 和全包 race，避免为既有栈改动迁移代码。
- Verification: `go test -race ./cmd/crystal-server -run 'MutatedManworm|CrazyManworm' -count=3 -timeout=300s` 通过，普通 cmd/全仓库测试、vet 和 build 通过；重跑日志中的 race 栈仅指向既有文件。

### 2026-08-20 — 全包 race 的新失败必须先做单测复跑归因

- Symptom: AI=31/32 批次完整 `cmd/crystal-server` race 重现既有 GuildBuff/Kirin 数据竞争，并出现 `TestSessionOmaMageRangeSlowFrozenTranscript` 的 `[2 1]`/`[1]` 随机边界断言失败。
- Root cause: race 并发维护路径触达了非本批的共享状态和 OmaMage transcript 随机记录；失败栈未进入 RightGuard/LeftGuard 代码。
- Prevention: 每批同时跑目标 race、普通全包和 full race；full race 新失败先单独 `-run` 与多次 `-count` 复跑并检查栈，再决定是否属于本批，不能因时间相邻修改生产代码。
- Verification: RightGuard/LeftGuard `-race -run ... -count=3` 通过；OmaMage 普通测试通过、单次 race 偶尔通过但 race `-count=5` 重现，full race 失败栈仍只指向既有测试/维护路径，已移交 handoff。

### 2026-08-20 — AI=33 全包 race 仍需按本次实际失败栈归因

- Symptom: AI=33 批次的 `go test -race ./cmd/crystal-server -count=1 -timeout=600s` 失败，命中 `TestGuildBuffSessionNewbieLoginReplacesStalePersistedBuff`、`TestSessionKirinIceThrustTranscript` 与 `TestSessionBlinkTranscriptIncludesDelayedMapChangeEffectAndBuff`；AI=33 定向 race 未失败。
- Root cause: race 栈分别落在既有装备 buff reconciliation、Kirin transcript 随机回调和 Blink map-transition buff ticker 的并发读写，没有进入 MinotaurKing 生产路径或其会话夹具。
- Prevention: 每批保留目标定向 race 与完整 race；完整 race 失败时记录当前实际测试集合和栈归属，不能用上一批的失败名单替代，也不能为非本批并发栈修改迁移实现。
- Verification: `go test -race ./cmd/crystal-server -run 'MinotaurKing' -count=3 -timeout=300s` 通过；普通 cmd/全仓库测试、`go vet ./...` 与 `go build ./...` 通过；完整 race 输出未出现 AI=33 文件或测试栈。

### 2026-08-20 — AI=34 全包 race 仍需按实际栈归因

- Symptom: `go test -race ./... -count=1 -timeout=900s` 仍失败，命中 `TestGuildBuffSessionNewbieLoginReplacesStalePersistedBuff`、`TestSessionKirinIceThrustTranscript` 与 `TestSessionBlinkTranscriptIncludesDelayedMapChangeEffectAndBuff`。
- Root cause: race 栈分别落在既有装备 buff reconciliation、Kirin transcript 随机回调和 Blink map-transition buff ticker 的并发读写，没有进入 FrostTiger 生产路径或新会话夹具。
- Prevention: 每批同时运行本 AI 的定向 race 与全包 race；全包失败按当前测试名和栈归因，非本批栈只写入 handoff/lessons，不为其修改迁移实现。
- Verification: `go test -race ./cmd/crystal-server -run 'FrostTiger' -count=3 -timeout=300s` 通过；普通 `go test ./...`、`go vet ./...` 与 `go build ./...` 通过，全包 race 输出未出现 AI=34 文件或测试栈。

### 2026-08-20 — AI=36 全包 race 仍需按本次实际失败栈归因

- Symptom: `go test -race ./... -count=1 -timeout=900s` 仍失败，命中 `TestSessionDarkBodySpawnAndRecallTranscript`、`TestGuildBuffSessionNewbieLoginReplacesStalePersistedBuff`、`TestSessionKirinIceThrustTranscript` 与 `TestSessionBlinkTranscriptIncludesDelayedMapChangeEffectAndBuff`。
- Root cause: race 栈分别落在既有装备 buff reconciliation 与读取、Kirin 会话实时 ticker 的随机回调，以及 Blink map-transition buff ticker 的并发读写；没有进入 Yimoogi 生产路径或 Yimoogi 会话夹具。
- Prevention: 每批同时运行本 AI 的定向 race 与全包 race；全包失败必须按本次实际测试名和栈归属记录，不能沿用上一批失败清单，也不能为非本批并发栈修改迁移实现。
- Verification: `go test -race ./cmd/crystal-server -run 'Yimoogi' -count=3 -timeout=600s`、普通 `go test ./...`、`go vet ./...` 与 `go build ./...` 通过；全包 race 输出未出现 Yimoogi 文件或测试栈。

### 2026-08-20 — AI=37/38 全包 race 仍需与定向 race 分开归因

- Symptom: 本批 `go test -race ./... -count=1 -timeout=900s` 失败，当前摘要命中 `TestGuildBuffSessionNewbieLoginReplacesStalePersistedBuff`、`TestSessionKirinIceThrustTranscript` 与 `TestSessionOmaMageRangeSlowFrozenTranscript`；AI37/38 定向 race 通过。
- Root cause: GuildBuff 栈落在既有装备 buff reconciliation 与会话读取竞争，Kirin 栈落在既有会话实时 ticker 与随机回调竞争，OmaMage 栈是上一条记录的真实时钟服务循环额外移动随机调用；均未进入 CrystalSpider/HolyDeva 实现或夹具。
- Prevention: 每批同时保留目标定向 race、普通全包测试和完整 race；完整 race 必须用过滤摘要保留本次实际测试名，不能沿用旧失败清单，也不能为非本批栈修改迁移代码。
- Verification: `go test -race ./cmd/crystal-server -run 'CrystalSpider|HolyDeva|SummonShinsuAndHolyDeva' -count=3 -timeout=600s`、定向普通测试、`go vet ./...` 与 `go build ./...` 通过；完整 race 摘要仅报告上述既有会话/装备竞争。

### 2026-08-20 — AI=41/42 全包 race 仍需与定向 race 分开归因

- Symptom: 本批完整 `go test -race ./... -count=1 -timeout=900s` 仍命中 `TestGuildBuffSessionNewbieLoginReplacesStalePersistedBuff`、`TestSessionKirinIceThrustTranscript` 与 `TestSessionOmaMageRangeSlowFrozenTranscript`；YinDevilNode 定向 race 未失败。
- Root cause: 失败栈仍属于既有 GuildBuff/装备状态、Kirin 实时 ticker/随机回调和 OmaMage session 维护 tick，未进入 AI=41/42 生产代码或测试。
- Prevention: 每批保留目标定向 race、全仓普通和完整 race；完整 race 只按本次实际测试名/栈归因，非本批并发问题写入交接而不修改迁移实现。
- Verification: AI=41/42 race `-count=5`、全仓普通、`go vet ./...` 与 `go build ./...` 通过；完整 race 摘要未出现 YinDevilNode 文件或测试栈。

### 2026-08-20 — AI=43 全包 race 必须记录本次实际失败集合

- Symptom: 本批完整 `go test -race ./... -count=1 -timeout=900s` 命中 `TestSessionDarkBodySpawnAndRecallTranscript`、`TestGuildBuffSessionNewbieLoginReplacesStalePersistedBuff` 与 `TestSessionKirinIceThrustTranscript`；OmaKing 定向 race 未失败。
- Root cause: race 栈仍落在既有智能生物会话、GuildBuff/装备共享状态和 Kirin ticker/随机回调，未进入 OmaKing 文件或测试。
- Prevention: handoff 记录每批实际失败名；目标定向 race、普通全包、完整 race 分开执行，非本批栈只分类记录，不为其修改迁移实现。
- Verification: OmaKing race `-count=5`、全仓普通、`go vet ./...` 与 `go build ./...` 通过；完整 race 过滤摘要未出现 OmaKing。

### 2026-08-20 — AI=45/46 全包 race 失败必须按最终实际集合归因

- Symptom: 本批最终完整 `go test -race ./... -count=1 -timeout=900s` 命中 `TestGuildBuffSessionNewbieLoginReplacesStalePersistedBuff`、`TestSessionKirinIceThrustTranscript` 与 `TestSessionOmaMageRangeSlowFrozenTranscript`；Foxman 定向 race 通过。
- Root cause: 失败栈属于既有 GuildBuff/装备共享状态、Kirin ticker/随机回调和 OmaMage session maintenance，未进入 RedFoxman/WhiteFoxman 代码或测试；同批较早的一次 race 摘要中的 Hiding 失败未在最终重跑复现。
- Prevention: 每批以最终实际失败名更新 handoff，不沿用前批 race 清单；只修改栈指向本批的生产代码，其余保留为既有门禁问题。
- Verification: Foxman race `-count=5`、全仓普通、`go vet ./...` 与 `go build ./...` 通过；完整 race 摘要未出现 Foxman 文件或测试栈。

### 2026-08-20 — AI=48 全包 race 仍需按最终实际集合归因

- Symptom: GuardianRock 最终完整 `go test -race ./... -count=1 -timeout=900s` 命中 `TestGuildBuffSessionNewbieLoginReplacesStalePersistedBuff` 与 `TestSessionKirinIceThrustTranscript`；GuardianRock 定向 race 未失败。
- Root cause: race 栈属于既有 GuildBuff/装备共享状态和 Kirin ticker/随机回调，未进入 GuardianRock 文件或测试。
- Prevention: 每批保留目标定向 race、全仓普通和完整 race；最终 handoff 只记录本次实际集合，非本批并发问题不修改迁移实现。
- Verification: GuardianRock race `-count=5`、全仓普通、`go vet ./...` 与 `go build ./...` 通过；完整 race 摘要未出现 GuardianRock。

### 2026-08-20 — AI=47 全包 race 必须记录最终实际失败集合

- Symptom: TrapRock 最终完整 `go test -race ./... -count=1 -timeout=900s` 命中 `TestGuildBuffSessionNewbieLoginReplacesStalePersistedBuff`、`TestSessionKirinIceThrustTranscript`、`TestSessionOmaMageRangeSlowFrozenTranscript` 与 `TestSessionBlinkTranscriptIncludesDelayedMapChangeEffectAndBuff`；TrapRock 定向 race 未失败。
- Root cause: 失败栈属于既有 GuildBuff/装备、Kirin ticker/随机回调、OmaMage maintenance 和 Blink map-transition buff ticker，未进入 AI=47 生产文件或测试。
- Prevention: 完整 race 必须在当前 Go HEAD 上重跑并记录最终测试集合；定向 race、普通全包和完整 race 分开归因，非本批栈不修改迁移实现。
- Verification: TrapRock race `-count=5`、服务端完整普通、protocol、vet/build 通过；完整 race 摘要未出现 TrapRock。

### 2026-08-20 — AI=49/50 全包 race 仍需按最终实际失败集合归因

- Symptom: 本批最终 `go test -race ./... -count=1 -timeout=900s` 失败于 `TestGuildBuffSessionNewbieLoginReplacesStalePersistedBuff` 和 `TestSessionBlinkTranscriptIncludesDelayedMapChangeEffectAndBuff`；ThunderElement/GreatFoxSpirit 定向 race 未失败。
- Root cause: 两个 race 栈分别落在既有装备 Buff reconciliation 与 Blink map-transition Buff 的共享状态读写，未进入 AI=49/50 生产代码或新会话夹具。
- Prevention: 每批保留目标定向 race、普通全包和完整 race；完整 race 只按当前运行的实际测试名/栈归因，非本批并发问题写入交接而不修改迁移实现。
- Verification: AI=49/50 定向 race、`go test ./...`、`go vet ./...` 和 `go build ./...` 通过；完整 race 输出未出现 ThunderElement/GreatFoxSpirit 文件或测试栈。

### 2026-08-21 — AI=54 普通全仓与完整 race 按实际栈隔离既有失败

- Symptom: `go test ./... -count=1 -timeout=900s` 两次命中 `TestSessionOmaMageRangeSlowFrozenTranscript` 的随机边界 `[2 1]`/`[1]`；`go test -race ./... -count=1 -timeout=1800s` 同时命中该失败和 `TestGuildBuffSessionNewbieLoginReplacesStalePersistedBuff` 的装备 Buff reconciliation 数据竞争。
- Root cause: OmaMage 真实 session maintenance 会额外消费随机；GuildBuff 栈在 `player_spell_buffs.go:742,824` 与测试读取 `intelligent_creature_items.go:549` 之间竞争。两者均未进入 DragonStatue 生产文件、测试或状态面。
- Prevention: 保留目标定向 race、无排除普通全仓、完整 race 和精确单测复跑四层证据；既有失败只能按当前栈归因，不修改无关迁移代码，也不能把带排除项的通过冒充完整门禁通过。
- Verification: AI=54 定向 race `-count=5`、最终服务端普通全包、`go vet ./...`、`go build ./...` 均通过；排除精确 OmaMage 既有失败后 `go test ./...` 通过，完整 race 输出未出现 AI=54 文件或测试栈。

### 2026-08-22 — AI=59 全仓 race 失败必须排除既有 GuildBuff/OmaMage 栈

- Symptom: `go test -race ./... -count=1 -timeout=900s` 失败于 `TestGuildBuffSessionNewbieLoginReplacesStalePersistedBuff` 的 `player_spell_buffs.go`/共享 session fixture race，并于 `TestSessionOmaMageRangeSlowFrozenTranscript` 的既有随机边界 `[2 1]`/期望 `[1]`；没有 AI=59 栈。
- Root cause: 共享在线 Buff slice 的既有并发读写和 OmaMage 测试的随机调用边界，与 HumanAssassin 生产路径无关。
- Prevention: 保留 AI=59 定向 race 作为本批门禁；全仓 race 按实际测试名/栈归因，不为无关路径修改当前 AI。
- Verification: `go test -race ./cmd/crystal-server -run 'DarkBody|HumanAssassin' -count=3 -timeout=600s` 通过；普通 `go test ./...`、`go vet ./...` 和 `go build ./...` 通过。

### 2026-08-22 — AI=59 全仓 race 必须记录当前实际失败集合

- Symptom: 当前 `go test -race ./... -count=1 -timeout=900s` 失败于 `TestGuildBuffSessionNewbieLoginReplacesStalePersistedBuff` 的 `player_spell_buffs.go:742,824`/共享 session fixture race、`TestSessionHidingTranscriptPersistenceAndExpiry` 的 `reconcileEquipmentSpecialBuffsLocked` 与 `calculatePlayerEquipmentStatsForMount` 并发读写，以及 `TestSessionOmaMageRangeSlowFrozenTranscript` 的随机边界 `[2 1]`/期望 `[1]`；没有 AI=59 栈。
- Root cause: 既有在线装备 Buff reconciliation 与会话读取竞争、Hiding 会话装备统计并发读取，以及 OmaMage 真实 session maintenance 的随机调用边界，均不经过 HumanAssassin 生产路径。
- Prevention: 以当前 HEAD 的完整 race 实际测试名和栈更新 handoff；保留 AI=59 定向 race 作为本批门禁，不为无关并发问题修改当前 AI，也不能用带排除项的通过替代完整 race。
- Verification: `go test -race ./cmd/crystal-server -run 'DarkBody|HumanAssassin' -count=3 -timeout=600s` 通过；无排除项普通 `go test ./...`、`go vet ./...` 和 `go build ./...` 通过，完整 race 输出未出现 AI=59 文件或测试栈。

### 2026-08-22 — AI=70 全包 race 必须按当前实际失败集合归因

- Symptom: 本批 `go test -race ./... -count=1 -timeout=900s` 失败于 `TestSessionDarkBodySpawnAndRecallTranscript` 的 `reconcileEquipmentSpecialBuffsLocked`/`calculatePlayerEquipmentStatsForMount` 并发读写，以及 `TestGuildBuffSessionNewbieLoginReplacesStalePersistedBuff` 的既有 `player_spell_buffs.go`/共享 session fixture race；没有 AI=70 文件或测试栈。
- Root cause: 失败均位于既有装备 Buff reconciliation、DarkBody 会话统计和 GuildBuff 共享 fixture，并发读取/写入不经过 Hugger 生产路径；AI=70 定向 race 自身通过。
- Prevention: 每批保留 AI 定向普通/race、普通全仓、完整 race、vet/build 四层证据；完整 race 只按当前实际测试名和栈归因，不沿用旧清单，也不为非本批并发问题修改迁移代码。
- Verification: `go test ./cmd/crystal-server -run 'PoisonHugger|Hugger' -count=10 -timeout=600s`、`go test -race ./cmd/crystal-server -run 'PoisonHugger|Hugger' -count=3 -timeout=600s`、普通 `go test ./...`、`go vet ./...`、`go build ./...` 均通过；完整 race 输出未出现 AI=70 栈。
