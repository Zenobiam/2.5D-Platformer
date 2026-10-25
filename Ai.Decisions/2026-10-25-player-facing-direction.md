# 2026-10-25 — Направление игрока как значение в PlayerFacing

Статус: accepted
Контекст: `PlayerFacing` поворачивает только модель (`modelTransform` → вложенный `CyborgGirl`), корень `Player` не поворачивается. `CameraController` следит за корнем и брал направление из `_followTarget.rotation.eulerAngles.y > 90` — у корня всегда 0, поэтому камера всегда забегала на `_aheadDistance` (4) вправо, даже при ходьбе влево. `PlayerAttack` брал `transform.forward` и был прав только потому, что висит на модели. Направление нигде не хранилось как значение.
Решение (вариант B): `PlayerFacing.Direction` (±1) и `Forward` (`(Direction, 0, 0)`), инициализация в `Awake`; модель поворачивается на `Direction * 90°`; `CameraController.Follow` берёт `PlayerFacing` у цели и читает `Direction`; `PlayerAttack` бьёт в `_facing.Forward` (`GetComponentInParent<PlayerFacing>()` в `Awake`).
Почему: одно место правды о направлении; камера и атака не зависят от того, на каком трансформе висит компонент и как повёрнута модель; переносится в любой 2.5D-проект.
Отвергнуто:
- A. Только поправить камеру (читать поворот модели) — направление по-прежнему выводится из углов, ловушка остаётся следующему потребителю.
- C. Поворачивать корень — поворот наследуют `CharacterController` и всё, что висит на корне; камера всё равно сравнивает углы.
- D. Отражение `localScale.x = -1` — для 3D-меша отражает нормали, освещение, коллайдеры, ломает гуманоид и `transform.forward`.
- E. Поворот «три четверти» к камере — визуальный выбор, возможен позже поверх B.
Не сделано (предложено): сглаживание `LerpAngle(… rotationSpeed * Time.deltaTime)` зависит от частоты кадров; `CameraController` теперь зависит от `Game.Scripts.Player` (развязать интерфейсом — только по просьбе, сверх курса).
Файлы: `Assets/Game/Scripts/Player/PlayerFacing.cs`, `Camera/CameraController.cs`, `Player/PlayerAttack.cs`.
