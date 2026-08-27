### 2026-08-27 — Base-family fixed-range attack仍消费 unit-bound RNG

- Symptom: inherited base attack改用完整 `MonsterObject.GetAttackPower` 后，既有 AI=0 测试因只允许 bound 2/3/100 而在实际 bound 1 失败。
- Root cause: 旧 common helper 对固定 MinDC/MaxDC 短路，没有保留 Legacy `Random.Next(min,max+1)` 的 `Next(1)` 消费；测试把旧缺口固化成 hook 契约。
- Prevention: inherited `MonsterObject.Attack` 必须走完整 Luck 与 unit-bound power helper；任何固定范围测试都显式接收并断言 bound 1。
- Verification: bounded `monster_ai_test.go` hook已加入 bound 1，BaseFamily focused 与既有 BasicMonsterAI 集合重跑 exit 0。

### 2026-08-27 — Base-family target扩展会改变旧多实体夹具的首个目标

- Symptom: common-population 测试原预期 AI=0 朝 Player 行走，新增 inherited Monster/Hero 搜索后却先攻击相邻的 Player-owned Monster；BindingShot 测试还先收到两个独立 name-colour 包。
- Root cause: 旧夹具把“owned pet 不由 common loop 处理”误当成“wild Monster 不能把它选作目标”，并把共享 world tick 的无关前导包当作目标动作 transcript。
- Prevention: population 测试用 Hidden 等真实搜索门禁隔离非目标实体；专测 BindingShot 顺序时从完整 transcript 中提取 `ObjectMonster`/`ObjectAttack` 契约，同时由 authenticated session test 单独锁定无噪声完整 melee transcript。
- Verification: living pet 仍保持不被 common loop 处理且不再污染该 fixture；BindingShot 保留 snapshot-before-attack，focused BaseFamily/Basic/BindingShot 集合 exit 0。

### 2026-08-27 — Wild base Monster 的 SafeZone 与 NoFight 必须拆开裁决

- Symptom: 首轮修正把 wild base-family 的 SafeZone 与 NoFight 一起移除，测试也把两个条件放进同一成功路径，掩盖了 SafeZone 本应拒绝攻击。
- Root cause: 只追到 `PlayerObject.IsAttackTarget(MonsterObject)` 的 wild 分支，漏读了更外层 `MapObject.IsAttackTarget(MapObject)` 在 overload dispatch 前无条件执行双方 SafeZone gate；NoFight 才是该 Monster overload 不检查的条件。
- Prevention: 每个目标类型分别追 common wrapper 与动态 overload；SafeZone、NoFight、GM 和 attack mode 各用独立正反例，禁止把两个 gate 合并成一个夹具。
- Verification: Player/owned-Monster/Hero 三类 SafeZone admission 均无 attack action，三类 NoFight 均保留攻击与命中，Player 入队后进入 SafeZone 会在 impact 重验时取消伤害。

### 2026-08-27 — Admission 拒绝后同 tick 的 idle 行为仍可产生包

- Symptom: SafeZone 定向测试要求整个 tick 无通知，实际正确地未排入攻击，却收到后续 idle `ObjectTurn`，测试误报失败。
- Root cause: 将“目标不可攻击”误当成“本 tick 完全停止”；Legacy 顺序会继续执行无目标 roam。
- Prevention: gate 测试只断言目标动作/队列/伤害不存在，并单独测试 idle 顺序；除非 Legacy 明确短路整个 ProcessAI，不以空 transcript 作为拒绝判据。
- Verification: 断言改为排除 `ObjectAttack` 且 action queue 为空后，保留合法 idle packet，精确 SafeZone 测试 exit 0。

### 2026-08-27 — Hero Hiding 不只是搜索期布尔字段

- Symptom: 初版只把 Hero `Hidden` 投影到 ObjectHero 和搜索过滤，已有锁定该 Hero 的普通 Monster 仍保留 Target，Hero 自主 Walk 后也继续隐藏。
- Root cause: 漏迁 `MapObject.AddBuff(Hiding)` 的 `HideFromTargets()` 副作用和 `HumanObject.Walk` 成功 admission 后的 `RemoveBuff(Hiding)`；只审计了 FindTarget 条件。
- Prevention: 隐身生命周期同时覆盖 add/reset 时按 DataRange、CoolEye 和等级清 target，成功 movement 的 reveal-before-walk 顺序、失败 movement 保留，以及 expiry 的 RemoveBuff/ObjectHidden 顺序。
- Verification: blind/低等级 CoolEye/equal-level CoolEye 三分支、reset 后重清、ObjectHero.Hidden、成功/失败 Walk 和延迟 RemoveBuff 定向测试 exit 0。

### 2026-08-27 — 扩大 common population 后旧 fixture 必须冻结 RoamAt

- Symptom: fresh full test 中 FloatingRock、HornedArcher、ToxicGhoul、TurtleGrass 和 WereTiger 用作 clone/友军/死亡目标/阻挡物的 AI=0/default Monster 开始额外消费 10/3/8 RNG 并发出 ObjectTurn。
- Root cause: 夹具只冻结 Action/Move/Attack/Search timer；base-family 纳入真实 common loop 后，零值 `MonsterAIRoamAt` 已到期，无目标对象会执行继承 roam。
- Prevention: 非目标 Monster 夹具若跨 world tick，必须按目标行为分别冻结 SearchAt 与 RoamAt；不能用 ActionAt 很远替代独立 idle timer，也不能扩大随机 hook 来吞掉不相关生产行为。
- Verification: 六个已登记 fixture 文件补齐远期 RoamAt，所有原 full-gate 失败测试精确重跑 exit 0，随后 fresh full gate 待重跑。

### 2026-08-27 — Unit-bound 修复必须复用现有 helper 并保持调用者隔离

- Symptom: 首次新增 `combatRollIncludingUnitBoundLocked` 与既有声明重复导致 compile exit 1；随后直接把共享 `damageAncientBringerMonsterLocked` 改为 unit-bound，使 GasToad、Mantis、StrayCat、TucsonMage 四个已冻结 RNG fixture 在 full test 收到额外 bound 1。
- Root cause: 未先全包搜索完整 helper 声明，并把 BaseFamily 专属 impact 需求落到所有 mapped-AI 共用入口。
- Prevention: 接线前搜索完整 receiver method；共享 damage helper 用显式 roll adapter 保留既有默认调用者，只让 BaseFamily 传入 unit-bound stream；不要用全局随机语义变更代替叶级适配。
- Verification: 重复 helper 已移除，touched compile exit 0；四个原失败测试精确重跑 exit 0，BaseFamily Player/owned-Monster fixed-range RNG test exit 0。

### 2026-08-27 — Movement/RNG 测试只断言目标边界，不推断无关通知或完整流

- Symptom: 首次新测把无观察者的 Hero 成功移动误判为必须返回通知，并把 Player 装备副作用与 Hero tick 的额外随机消费误写成固定两/三个 draw，focused test exit 1。
- Root cause: 将状态 mutation 与 fan-out 混为一体，并对共享 world tick 的完整 RNG 流做了超出目标边界的假设。
- Prevention: movement 同时回读坐标与 cell-order authority，通知只在存在接收者时断言；unit-bound test 在 admission 后重置记录器并只锁定目标 impact 的必要前缀，或隔离到目标类型的专用 adapter。
- Verification: 修正后的 stacking/cell-order/unit-bound 三项 focused test exit 0。

### 2026-08-27 — 同型 resolver 补丁必须带完整函数锚点

- Symptom: Player unit-bound patch 只按 `switch target.Kind`/Player case 匹配，实际落入更早的 AxeSkeleton resolver；BaseFamily 仍走旧 damage helper，而测试又被装备耐久的 bound-1 前缀伪装成通过。
- Root cause: 相邻 resolver 结构同型，补丁缺少目标函数声明；测试只检查共享 combat stream 前缀，没有复读物理落点。
- Prevention: 每个同型 target resolver hunk 必须携带完整 `func` 声明，patch 后同时回读目标与前一同型函数；定向 RNG 测试之外还要用 diff 审计确认 adapter 的实际 caller。
- Verification: AxeSkeleton 路径已恢复，BaseFamily resolver 精确改用 unit-bound combat adapter；两族精确测试 `-count=10` exit 0。

### 2026-08-27 — Respawn 必须先消费构造器 RNG 再选择 cell

- Symptom: 终审发现 Go 的 tick/time respawn 先随机选择位置、再 materialize base-family Monster；Legacy 则先 `new MonsterObject` 消费 CoolEye/direction/regen/search/roam 五次，再由 `Spawn(MapRespawn)` 消费位置 RNG。
- Root cause: 初始迁移把 constructor 与 spawn 的随机源拆成独立 helper，并沿用旧的 position-first call site；单 cell 的 `Next(1)` 还被短路。
- Prevention: base-family respawn 通过一个生产 helper 使用同一 `respawnRoll` stream，固定五次 constructor draw 后再做 position draw，且 unit bound 也必须消费；非 base-family 保持已冻结路径。
- Verification: 新定向测试锁定 bounds `100,8,10000,3000,1000,1`、direction 和 X/Y/RespawnX/Y；既有 time/tick respawn 与 BaseFamily focused/repeated/race 均通过。

### 2026-08-27 — Safe-zone 终审必须包含 MapObject common wrapper

- Symptom: terminal reviewer 仅引用 `PlayerObject.IsAttackTarget(MonsterObject)` 中 wild 分支并报告 wild 可绕 SafeZone。
- Root cause: 漏掉真实调用 `ob.IsAttackTarget(this)` 先进入 `MapObject.IsAttackTarget(MapObject)`，其在 overload dispatch 前检查双方 SafeZone；trainer target 才同时豁免双方。
- Prevention: 任何 overload 结论必须从静态调用点追 common wrapper，再追动态 overload；不能把内层 early return 当作完整行为。
- Verification: Legacy `MapObject.cs:428-453` 与 `MonsterObject.cs:2403-2418` 证明 wrapper；Go 三类目标独立 SafeZone 正例与 NoFight 反例、impact-time revalidation tests 通过，因此该 review finding 被裁决为 false positive。

### 2026-08-27 — Alone runtime consumer 与配置接线分叶但不能硬编码

- Symptom: base-family 默认正确停止 alone Monster，但没有表达 `Settings.MonsterProcessWhenAlone=true` 时仍执行 stacking/search/roam 的 runtime seam。
- Root cause: 只迁默认 false 行为，并把 P1 配置 follow-up 误解为可以延后 P5 consumer 本身。
- Prevention: P5 lifecycle 提供 world runtime field 并在 `CheckAlone` 后裁决；P1 follow-up 只负责 INI/default/write-back/startup wiring，不重做 AI lifecycle。
- Verification: 默认 false 不消费 RNG；true 时 Alone 保持 true但按 `10,3,8` roam sequence 转向，focused/repeated/race tests 通过。

### 2026-08-27 — Fresh full gate 的 session timeout 必须隔离归因并再跑全包

- Symptom: 一次 fresh `go test -count=1 ./...` 在 30 秒后仅失败 `TestSessionHidingTranscriptPersistenceAndExpiry`，报 closed pipe；同批其余包通过。
- Root cause: 高负载 full run 中既有 session transcript 超时；本叶只改 Hero Hiding，失败用例是 Player Hiding，不能据一次全包超时裁决 production regression，也不能忽略。
- Prevention: 先精确单测和 repeated 复现，再跑 fresh touched package，最后重新执行完整 unexcluded gate；所有退出码都保留，禁止只报告成功重跑。
- Verification: 精确单测 `-count=1`、`-count=10` 及 fresh `go test -count=1 ./cmd/crystal-server` 均 exit 0；最终 full rerun仍为 leaf closure 必需。

### 2026-08-28 — Specialized ProcessAI overrides determine whether Alone applies

- Symptom: ordinary and dynamically created HellBombs advanced their absolute
  ten-second explosion deadline with no nearby players, while Legacy pauses
  `ProcessTarget` behind the default `MonsterProcessWhenAlone=false` gate.
- Root cause: the shared Go HellBomb ticker bypassed inherited
  `MonsterObject.ProcessAI`; the neighboring AI=60-63 classes were incorrectly
  treated as precedent even though their concrete `ProcessAI` overrides bypass
  `CheckAlone` in Legacy.
- Prevention: for every specialized class, resolve virtual dispatch before
  applying a common lifecycle gate. Reuse the P5 runtime Alone seam only when
  the concrete class actually inherits base `ProcessAI`, and keep delayed
  actions outside that gate.
- Verification: focused tests lock default pause, cached three-second
  `AloneTime`, equality-time resume after a player arrives, and the enabled
  option; ordinary/protected HellBomb count-10 and race-count-3 pass.
