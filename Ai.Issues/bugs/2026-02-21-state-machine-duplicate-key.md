# 2026-02-21 — BootstrapState не вызывался: дубликат ключа в словаре состояний

Статус: resolved. Дата — по времени файла расшифровки Cursor (`…-Assets-Game/agent-transcripts/8c304b33-…`), перенесено 2026-10-25.

## Симптом

`NullReferenceException` в `PlayerMovementController.Update`: `InputService` = `null`.

## Причина

В `GameStateMachine` оба состояния были записаны под одним ключом:

```csharp
[typeof(BootstrapState)] = new BootstrapState(...),
[typeof(BootstrapState)] = new LoadLevelState(...),   // молча перезаписал первое
```

Инициализатор словаря через индексатор `[key] = value` **не бросает исключение** на повторном ключе, а перезаписывает. `Enter<BootstrapState>()` входил в `LoadLevelState`, `RegisterServices()` не вызывался, сервис ввода не создавался.

## Что сделали

Второе состояние под своим типом: `[typeof(LoadLevelState)] = …`.

## Как не повторить

- Ошибку нашли, только когда прошли по цепочке вызовов от `GameBootstrapper`. Перед этим агент дважды «чинил» NRE проверками на `null` и повторным получением сервиса в `Update` — ошибка пропадала, а причина оставалась. Правило: `<Knowledge>\process\agent-workflow.md` § 2.
- Словарь состояний в `GameStateMachine` по-прежнему заполняется через индексатор (2026-10-25): при добавлении состояния — проверить, что тип ключа совпадает с типом объекта. Вариант `{ typeof(X), new X() }` бросает `ArgumentException` на дубликате — предложение, не сделано.
