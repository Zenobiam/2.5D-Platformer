# 2026-10-30 — Окна в игре (урок 13 UI part 2)

## Проблема

Окно магазина (урок 12) лежало префабом; его нужно открывать кнопкой в игре, закрывать и показывать в нём данные прогресса.

## Что сделали

- По курсу: `WindowBase` (крестик, прогресс, хуки), `ShopWindow`, `OpenWindowButton`, `WindowService` (`Open(WindowId)`), `UIFactory` (`CreateUIRoot`, `CreateShop` по `WindowStaticData`), `WindowConfig`/`WindowStaticData`; регистрация `UIFactory` и `WindowService` до `GameFactory`; `GameFactory.CreateHud` → `OpenWindowButton.Init`; `LoadSceneState` создаёт `UIRoot` первым.
- Классы HUD (`ActorUI`, `HpBar`, `HpCamera`, `LootCounter`, `MobileOnly`) — в `UI/Elements/`, как в курсе.
- Редактор: `Tools → UI → Build Shop Window` (+ `UIRoot`, `ShopWindow`, `Money_container`, `WindowStaticData`), `Tools → UI → Build Player HUD` (+ `Shop_button` вне `Mobile`).

## Что пошло не так и как исправили

- Баг курса: `ShopWindow.SubscribeUpdates` подписывал на `LootData.Changed` сам себя — исправлено (`RefreshSkullText`), отступление.
- `StaticDataService.Load()` грузит `WindowStaticData` при старте — до запуска Editor-скрипта Play Mode падал бы с NRE; порядок шагов записан в брифе.

## Вывод урока

Ручной DI: фабрика окон передаёт зависимости руками, каждое новое окно — правка фабрики, окна и сервиса окон (`<Knowledge>\conventions\dependency-injection.md`).

## Проверка

`0 error CS`; префабы, `UIRoot`, `WindowStaticData` по файлам; Play Mode — пользователь («закончили с UI», 2026-10-30).

Бриф: `Ai.Tasks/2026-10-30-ui-windows.md` (локально).
