# Enemy Skeleton — анимации сейчас (2026-09-20)

Префаб `Resources/Enemies/Skeleton/Prefab` → controller `FBX/Skeleton.controller`.

**States:** Locomotion (default, blend Speed: IDLE@0 / Walk@0.5 / Run@1), LeftAttack, RightAttack, SplashAttack, Damage, Death.  
**Params:** float `Speed`; triggers `Death`, `Damage`, `SplashAttack`, `LeftAttack`, `RightAttack`.  
**Код:** `EnemyAnimator` API = params controller 1:1; `AnimateAlongAgent` пишет `Speed = Clamp01(velocity)`. Chase (agent 3.5) → Speed≈1 → **Run**. Attack/Damage/Death в коде есть, никто не вызывает (кроме RightAttack клипа ниже); Left/Splash/Damage — ещё заглушки (Death state → `death_test`, не `Death.anim`).

**Rig:** Skeleton FBX importer = Humanoid (`animationType: 2`), но runtime Avatar/`isHumanMotion` = **не Humanoid** — IDLE/Walk/Run ключат **Generic Transform** по путям `DeformationSystem/Root_M/...` (quat `m_LocalRotation`), не muscles. База поз для атак — IDLE@0.

### RightAttack (готово)
- Клип: `Assets/Resources/Enemies/Skeleton/Animations/RightAttack.anim` (заменил заглушку).
- Длина **0.85s**, `loopTime=0`; фазы 0 → windup 0.22 → strike 0.42 → recover 0.68 → rest 0.85.
- **16 костей**, **67 curve bindings**, 5 ключей/кость (quat); тело: Root/Hips/Knees, Spine/Chest, Neck/Head, Scapula/Shoulder/Elbow/Wrist **R** + контрбаланс L.
- Root position зафиксирован (delta≈0) — без root-motion слайда; state `RightAttack` в controller уже на этот guid.
- Пересборка: `AgentScripts/BuildSkeletonRightAttack.cs` через `unity command run_script`.
- **Forward fix:** strike по `go.forward` (+Z), не вбок (+X). Shoulder strike ≈`(-13,-90,-99)`; SampleAnimation strike F=0.65 R≈0 (было F=0.17 R=0.67).
