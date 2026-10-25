# 2026-02-21 — Ввод: статический InputReceiver в сцене + InputService

Статус: accepted (решение пользователя; дата — по времени файла расшифровки Cursor, точного времени в ней нет)
Источник: расшифровка Cursor `~/.cursor/projects/d-Unity-Projects-2-5D-Platformer-Assets-Game/agent-transcripts/8c304b33-…` (перенесено 2026-10-25).

Контекст: `PlayerInput` (Input System, Invoke Unity Events) в сцене вызывал методы удалённого `PlayerInputController`; `InputService` — обычный C#-класс, его нельзя повесить на объект и указать в Unity Events, поэтому ввода не было.

Решение: схема **PlayerInput → InputReceiver → InputService → потребители**.
- `InputReceiver` — MonoBehaviour на отдельном объекте сцены `InputHandler` (вместе с `PlayerInput`), принимает события `OnMove`/`OnJump`/`OnRun`/`OnCrouch`/`OnAttack` и пишет состояние в **статические** поля.
- `InputService` (`IInputService`, обычный C#) читает статику `InputReceiver` и регистрируется в `BootstrapState` через `ServiceLocator`; создаётся без ссылок на сцену.
- `InputHandler` — отдельный объект, не игрок: игрок создаётся фабрикой и не должен нести сценовые события ввода.

Почему: сервис остаётся обычным C# за интерфейсом (как в курсе), а Unity Events получают MonoBehaviour; без поиска объекта сцены при старте и без передачи ссылок через bootstrap.

Отвергнуто:
- Мост `InputForwarder` с передачей в `GameMain.InputService` — зависел от порядка `Awake` (bootstrap должен был успеть раньше игрока).
- `InputService` как MonoBehaviour на игроке — терялся интерфейс и регистрация сервиса, расходилось с курсом.
- `InputReceiver` как экземпляр, переданный в конструктор `InputService` (поиск в сцене из `GameBootstrapper` с `[DefaultExecutionOrder(-100)]`) — лишняя связь bootstrap со сценой.

Минусы выбранного (знать):
- Статическое состояние общее на всё приложение: один игрок, нельзя два `InputReceiver`.
- При выключенном Domain Reload (Enter Play Mode Options) статика переживает перезапуск Play Mode — значения нужно сбрасывать.
- Хуже тестируется, чем экземпляр за интерфейсом.

Связано: `Ai.Architecture/course-deviations.md` (строка «Ввод»), `Ai.Issues/features/2026-10-24-player-attack-input.md`.
