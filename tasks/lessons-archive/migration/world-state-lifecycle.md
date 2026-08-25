### 2026-08-17 — 延迟 AI 动作的持久状态必须写回权威怪物副本

- Symptom: HornedSorceror Charged Stomp 的延迟伤害已从动作队列移除，但怪物仍保持 `Immune`，后续玩家攻击持续被拒绝。
- Root cause: 解析器按值接收延迟动作的攻击者；清除免疫只改了局部副本，调用方随后把旧的免疫副本写回世界。
- Prevention: 延迟动作若改变攻击者持久状态，必须在 map 写回前修改调用方的权威副本，或改用指针/显式返回值；测试同时断言状态清除和伤害结果。
- Verification: AI=169 Player/owned-Monster/Hero Stomp transcript 在普通与全量 Go 测试中均确认免疫清除、50 点伤害和后续状态。

### 2026-08-13 — 登录规范化瞬态字段必须回写权威持久层

- Symptom: 登录时从 session/world 投影移除了旧 JSON 错误保存的 newbie Buff type 115，但 auth 内存仍保留旧值，正常登出或重载可能再次恢复该瞬态 Buff。
- Root cause: 把登录规范化当成连接局部清理，没有同步拥有角色持久状态的 auth authority。
- Prevention: 登录期间清理或修复任何持久字段时，必须同时更新 session、world 与 auth 权威记录，并通过正常登出和 JSON 重载验证旧值不会复活；瞬态运行时状态不得写回持久模型。
- Verification: type 115 登录过滤现立即同步 auth；测试覆盖登录运行时属性生效、正常登出和磁盘重载后 type 115 不存在。

### 2026-08-13 — 全局实体表不能简化成角色槽内嵌生命周期

- Symptom: P8 Hero 草稿把完整 Hero 只存于角色槽；按原版执行封印时一旦清空槽位，封印物品虽然保留 Hero ID，Hero 本体却会从 Go 持久化状态消失，无法再次解封。
- Root cause: 只迁移了 Character 保存的 Hero 槽投影，没有同时保留 Legacy 独立 Hero 全局表；把“当前绑定关系”和“实体生命周期”合并成了同一份内嵌数据。
- Prevention: 迁移由全局表实体加外键槽位组成的数据模型时，权威存储必须分别表达实体 registry 与绑定投影；解绑、封印、删除只改变绑定/删除状态，不能隐式释放全局身份、名称或 ID。旧 JSON 可从槽位重建 registry，新格式必须覆盖游离实体保存重载和按 ID 恢复。
- Verification: `TestUnboundHeroRegistryLifecycleSurvivesJSONAndRetainsName`、`TestHeroUnbindRetainsRegistryAndRequiresExplicitRebind`、`TestHeroRegistryIsAuthoritativeAcrossCommitAndMaximumItemIDScan`、封印会话重载及旧 JSON fallback 测试已覆盖游离实体、名字占用、`AddedStats[129]` 按 ID 恢复和 ID 连续性。

### 2026-08-13 — 跨 auth/world 物品事务必须先同步权威状态再投递网络

- Symptom: P11 审查发现 Storage 附件已经在 auth 原子提交，但 world 物品快照直到 `RefreshItem`/结果包写出后才更新；钓鱼 Tick 也先投递通知再持久化，觉醒拆解新增的 `ItemInfos` 没有同步到 world。连接写失败后的 cleanup 可能用旧 world 快照覆盖已提交状态。P6 grid 集成又发现 Sun/Normal Potion 已改 session/world 却未改 auth，随后 Delete 从正确的 auth latest snapshot 提交时把先前药水消耗复活。
- Root cause: 把成功响应顺序当成了整个事务顺序，没有区分“auth/world 双权威状态提交”和“可能失败的网络通知”；同时只同步物品格，遗漏了新物品定义目录或把 session-local delta 留给 logout。P6 shout arming 又尝试在 `world.mu` 内调用只返回 bool 的 auth callback，既新增 world→auth 嵌套锁序，又无法取得 rental normalization 后的真实 auth grids。
- Prevention: 所有跨 auth/world 的物品事务先完成 auth 提交、world `ItemInfos`/三类物品格同步及必要落盘，再按 Legacy 顺序投递网络包；禁止让未提交的 session-local 物品差异跨入下一事务。网络失败只能影响通知，不能让 cleanup 从旧快照回滚事务。多个定时结果先选取最后一个 changed 快照持久化，再保持原结果顺序投递全部通知。禁止用持锁 bool callback 伪造原子性；共享事务层必须显式规定 revision/CAS、normalized snapshot 返回值和唯一锁序，由 owning leaf 统一实现。
- Verification: `advanceFishing` 已改为先持久化最后一个变化快照再投递 Tick 通知；`EquipSlotItem` 在任何响应写入前同步 world；觉醒持久化同时同步 `ItemInfos` 与物品格，并新增 world/auth 定义一致性测试和网络材料不足事务测试。P6 复审拦截并完整撤回了嵌套 callback；grid leaf 的 basic-potion 路径现先同步 rental-normalized auth/world/disk，再发 health/UseItem，定向 Use→Delete 与 revision/CAS 会话测试通过。

### 2026-08-12 — 拍卖到期与 stale Search 必须保留 legacy 生命周期

- Symptom: Go 初版每 500ms 并在每个 Game 请求前处理到期，消除了原版十分钟扫描窗口；已撤回拍品的旧搜索请求返回 reason 7，而原版因仍持有 AuctionInfo 引用返回 reason 3。
- Root cause: 把按请求查询当前 map 的 Go 模型当成了原版全局定时器和连接级对象引用模型，没有验收到期前后及移除后的旧引用。
- Prevention: 服务启动立即扫描一次、随后严格每十分钟扫描，禁止请求前隐式扫描；移除拍品时保留运行期终态 tombstone，使 stale buy 按 Sold→2、其他已移除→3 返回。
- Verification: 定时常量、显式到期处理、stale 撤回会话和 sold/withdrawn auth 测试通过。

### 2026-08-12 — Goal 状态审计必须使用当前 CODEX_HOME 并关联线程生命周期

- Symptom: 只调用当前线程的 Goal 查询时看起来只有 1 个 Goal，但当前 Codex 实例的数据库实际有 4 条 `active` 记录，容易把“当前线程 Goal”误报成“整个实例正在执行的 Goal”。
- Root cause: Goal 查询接口按当前线程返回；同时若未先解析 `CODEX_HOME`，可能误查默认 `~/.codex`。子代理线程已经关闭后，其 Goal 行还可能残留为 `active`，单看 Goal 表不能代表仍在运行。
- Prevention: 回答 Goal 总数前先确认当前 `CODEX_HOME`，查询其 `goals_1.sqlite`，再与同目录 `state_5.sqlite` 的线程和 `thread_spawn_edges` 生命周期关联；分别报告“数据库标记 active”和“实际仍运行”，且不得手工改内部数据库清理残留。
- Verification: 本次关联审计确认 4 条 `active` 中仅主线程 1 条仍执行，另外 3 条都属于 `closed` 子代理；该主线程下 23 条子代理边全部为 `closed`，当前没有活跃子代理。

### 2026-08-17 — 生命周期代码修改前必须复读精确物理行

- Symptom: 修改 EarthGolem Pile 生命周期时曾凭摘要怀疑生成分支缺少 `continue`，准备的补丁与当前源码不符；精确复读后确认分支已有 `continue`，补丁未应用。
- Root cause: 依赖前一轮分析记忆，没有在生命周期分支修改前用带行号/不可歧义的读取重新核对实际控制流。
- Prevention: 修改 spawn/impact/expiry 状态机前先复读完整分支的精确物理行（包括条件、状态写入和 `continue`/`return`），再用最小 hunk 修改；补丁失败或上下文不符时不据工具输出推断源码状态。
- Verification: EarthGolem Pile 的 spawn、首次/重复命中、过期移除和 ordered transcript 均通过，生产状态机未引入重复处理。

### 2026-08-17 — AI=151 value-map 实体必须回读后写回

- Symptom: CaveStatue session 夹具首次包级编译失败，直接给 `world.monsters[1].Route` 和 `RouteMoveReadyAt` 赋值；Go map value 不能对索引表达式的字段赋值。
- Root cause: 把 `world.monsters` 当成指针 map 使用，未先复制实体到局部变量。
- Prevention: 修改 Monster value-map 实体时先取局部副本，完成所有字段变更后通过同一 ObjectID 写回；延迟状态断言也从权威 map 回读。
- Verification: 改为回读/修改/写回后，AI=151 CaveStatue world 与真实 `net.Pipe` 定向测试通过。

### 2026-08-18 — AI=214 普通宠物生命周期必须走虚拟对象投影

- Symptom: SepWarrior 的 `objectPacketAt` 已按 Legacy `GetInfo()` 返回 `ObjectPlayer`，但普通宠物 restore/recall 路径仍直接调用通用 `ordinaryPetObjectPacketAt`，会把 AI=214 发成 `ObjectMonster`。
- Root cause: 为保留普通宠物既有封装，新增路径复用了具体 helper，绕过了按 AI 分派的虚拟对象边界；Legacy 的 `GetInfo` 是运行时类型分派，不能由调用点静态假设。
- Prevention: 所有向客户端发送 Monster/Pet 初始、恢复、召回对象时统一调用 `objectPacketAt`；通用 helper 只保留为非特殊 AI 的 fallback，并为特殊宠物加 owner 外观投影测试。
- Verification: AI=214 ObjectPlayer 测试覆盖主人名字/性别/发型/Light/WingEffect，普通宠物 restore/recall 测试及全量普通/race 门禁通过。

### 2026-08-20 — ThunderElement Repulsion 必须传递真实 pusher ObjectID

- Symptom: ThunderElement 被 Repulsion 推中后，`ObjectStruck.AttackerID` 初版使用了占位 ID，客户端无法看到实际施法者/推击者。
- Root cause: 共享 push resolver 只传递了受击怪物和距离，没有沿调用链保留真实 pusher 的 ObjectID。
- Prevention: 所有 Repulsion 入口显式传递 pusher ObjectID，并在 `ObjectStruck` payload 与世界测试中同时断言该 ID；不要从 target 或当前 AI attacker 推断推击者。
- Verification: `TestGameWorldThunderElementPushedRepulsionDamageUsesPusher` 断言真实 ID 和自伤公式，AI=49 定向普通/race 测试通过。
