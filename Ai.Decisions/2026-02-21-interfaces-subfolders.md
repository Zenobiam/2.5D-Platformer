# 2026-02-21 — Интерфейсы в подпапках `Interfaces/` своей области

Статус: accepted (решение пользователя; дата — по времени файла расшифровки Cursor)
Источник: расшифровка Cursor `…-Assets-Game/agent-transcripts/8c304b33-…` (перенесено 2026-10-25).

Решение: интерфейс лежит в подпапке `Interfaces/` **той же области**, где реализация: `StateMachine/Interfaces/IState.cs`, `Factories/Interfaces/IGameFactory.cs`, `Services/Input/Interfaces/IInputService.cs`, `Infrastructure/Interfaces/ICoroutineRunner.cs`. Namespace = путь: `Game.Scripts.<Область>.Interfaces` (так в коде на 2026-10-25; при первом переносе namespace оставляли без `.Interfaces`, позже привели к пути).

Почему: область (фича или слой) остаётся одной папкой — интерфейс рядом с реализацией и потребителями; правило «путь = namespace» не нарушается.

Отвергнуто:
- Одна общая папка `Scripts/Interfaces/` для всех интерфейсов — агент предложил первым, пользователь поправил: контракт отрывается от своей области, папка превращается в свалку.

Перед этим (тогда же): `Core/` разгружен — состояния, `GameStateMachine`, `SceneLoader`, `LoadingCurtain` перенесены в `StateMachine/` (+ `States/`), `ICoroutineRunner` — в `Infrastructure/`. Ревью проверяет «misuse папок `Interfaces/`» (`Ai.Context/agent-playbook.md`).
