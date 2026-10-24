# Атака игрока: ввод, событие клипа, поиск целей

Дата: 2026-10-24
Статус: в работе (ввод и событие готовы, урон и мобильная кнопка — нет)

## Что сделано

- **Ввод:** действие `Attack` в `PlayerControls.inputactions` (карта `Player`), привязка `<Mouse>/leftButton` (для редактора). `InputReceiver.OnAttack` помечает отпускание кнопки, `IInputService.AttackButtonUp()` возвращает `true` один раз. Как в курсе (`AttackButtonUp`).
- **Событие клипа:** `OnAttack` добавлено в `Hit.FBX.meta` на кадре 5 (0.1667 с). Вызов идёт на объект с `Animator` (`CyborgGirl`).
- **Компонент:** `PlayerAttack` на `CyborgGirl` (по аналогии с `HeroAttack` курса). `Update` запускает `PlayAttack`, если атака не идёт. `OnAttack` ищет цели через `Physics.OverlapSphereNonAlloc` по `Hittable`-маске и вызывает `TakeDamage` у `IHealth` цели.
- **Слой:** `Hittable` (индекс 15) добавлен в `Tags and Layers`. В проекте нет слоя с именем из курса, поэтому маска задаётся полем в Inspector.

## Отличия от курса

- В курсе `HeroAttack` стоит на корне с `Animator`. У нас `Animator` на `CyborgGirl`, а `CharacterController` на корне `Player`. Поэтому `CharacterController` берётся через `GetComponentInParent`.
- В курсе маска — `LayerMask.NameToLayer("Hittable")`. У нас маска в Inspector, потому что не хотим завязываться на имя слоя в коде.

## Что осталось

- Коллайдер тела у `Skeleton` (сейчас только триггеры) со слоем `Hittable`.
- Состояние `Hit` с клипом и переход по триггеру `Attack` в `FreeHandController` (сейчас `Attack` ведёт в `Fire`).
- Мобильный ввод: экранная кнопка атаки в HUD. Мышь оставить для редактора.
- Удалить `using UnityEditor;` из `EnemyAttack.cs`.

## Мобильная разработка

Курс мобильный. Экранные кнопки, масштабирование HUD и отсутствие аллокаций в кадре — обязательные требования (см. `Knowledge/conventions/`).
