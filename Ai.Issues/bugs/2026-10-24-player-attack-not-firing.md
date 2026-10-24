# 2026-10-24 — Удар игрока не срабатывал; ошибки Inspector при старте Play Mode

## Проблема

1. Удар не работал ни мышью (Game), ни экранной кнопкой (Simulator).
2. После включения анимации — `'CyborgGirl' AnimationEvent 'NewEvent' on animation 'Hit' has no receiver!` на каждый удар, урона нет.
3. При каждом старте Play Mode — 5 ошибок редактора: `GameObjectInspector.OnDisable` NullReferenceException, `SerializedObjectNotCreatableException` (Transform/Animator/NavMeshAgent Inspector), `MissingReferenceException: m_Targets of GameObjectInspector`.

## Причина

1. В `SampleScene` → `InputHandler` → `PlayerInput` (Invoke Unity Events) не было события `Player/Attack` → `InputReceiver.OnAttack`. Действие `Attack` добавили в `.inputactions` позже, список событий не обновился.
2. В `FreeHandController` не было состояния `Hit`: триггер `Attack` вёл только `Aiming → Fire`.
3. Событие клипа `Hit` в импортёре `Hit.FBX` было `NewEvent` на 0.0 (имя по умолчанию), а не `OnAttack`.
4. Ошибки Inspector — устаревший скрытый Inspector-редактор в памяти Editor для объекта Skeleton. Выделение и сброс раскладки не помогли; в коде проекта editor-хуков нет. Вероятно, оставлен командами Unity CLI (`com.unity.pipeline`: `set_selection`, захват окон) от нескольких сессий.

## Что сделали

1–2. Editor-скрипт `Assets/Game/Editor/PlayerAttackSetup.cs` (**Tools → Setup → Player Attack**, идемпотентный): слушатель `InputReceiver.OnAttack` на `Player/Attack`; состояние `Hit` (клип `Hit`), `Any State → Hit` по `Attack`, `Hit → Idle` по Exit Time 0.9.
3. Пользователь в импортёре: Function `OnAttack`, время ≈0.21 (кадр ~5 из 25), Apply.
4. Перезапуск Unity Editor — ошибки ушли.

## Как не повторить

- Новое действие в `.inputactions` → проверить список Events у `PlayerInput` (или запустить setup-скрипт).
- Событие клипа из FBX — после правки обязательно Apply и проверить `functionName` в `.meta`.
- Inspector-ошибки при пустом выделении и без наших editor-хуков → сначала перезапуск Editor, потом искать в проекте.
