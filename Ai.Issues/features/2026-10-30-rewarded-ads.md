# 2026-10-30 — Реклама за вознаграждение (урок 14 Rewarded ads)

## Проблема

Курс делает рекламу на Unity Ads через пакет Advertisement Legacy (`IUnityAdsListener`, `Advertisement.IsReady/Show`). По документации Unity с 31.01.2026 прямая интеграция через этот пакет не поддерживается (замена — LevelPlay); реклама Unity работает только на Android/iOS, а основная платформа проекта сейчас ПК.

## Что сделали

- Архитектура курса: `IAdsService` (готовность + событие, показ с колбэком, награда), `RewardedAdItem` (кнопка, объекты «есть / нет рекламы», `LootData.Add(Reward)`), `ShopWindow.Construct(ads, progress)`, `UIFactory(…, IAdsService)`, `BootstrapState.RegisterAdsService` до `UIFactory`, `WindowBase.Init` → `Initialize`.
- Реализация — `StubAdsService` (решение пользователя): всегда готова, сразу засчитывает просмотр. Реальный сервис (LevelPlay) — при мобильной сборке, заменой одной регистрации.
- Editor: `ShopWindowBuilder` — `AdNotAvailable_text`, `RewardedAdItem` на `AdsItem_container`, `ShopWindow.AdItem`.

## Вывод урока

Сервис за интерфейсом позволяет подменить реализацию (заглушка ↔ SDK), не трогая окно; цена ручного DI — фабрика окон знает конкретный тип окна и его зависимости.

## Проверка

`0 error CS`; `Shop.prefab` по файлу; Play Mode — пользователь (WATCH AD → +15 черепов в окне и на HUD).

Бриф: `Ai.Tasks/2026-10-30-rewarded-ads.md` (локально). Решение: `Ai.Decisions/2026-10-30-pc-first.md` § «Монетизация на ПК».
