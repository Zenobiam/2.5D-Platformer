# 2026-10-30 — Окно магазина (урок 12 UI part 1)

## Проблема

Урок 12 — вёрстка окна магазина без кода. Паки UI курса частично отличаются от наших; ассеты из Asset Store без 9-slice; курс мобильный, а проверяем на ПК.

## Что сделали

- Паки UI в `Assets/ThirdParty/` до первых ссылок (`Simple Fantasy GUI` совпадает с курсом по guid; `RPG icons`, `2D Casual UI` — замены `Free RPG Icons`, `GUI CARTOON`); пакет `com.unity.2d.sprite` (Sprite Editor).
- `Tools → UI → Build Shop Window` (`Assets/Game/Editor/ShopWindowBuilder.cs`): 9-slice курса на 6 спрайтах и `Generate Physics Shape` выкл; префабы `Resources/UI/Window`, `ShopItem`, `Shop` (вложенные префабы, свой Canvas у прокрутки, Raycast Target только у кнопок и прокрутки). Повторный запуск не затирает правки спрайтов/текста в Inspector.
- Под ПК: `ScrollRect.scrollSensitivity` 50 (колесо даёт 6 единиц на щелчок — иначе ~130 щелчков на ленту). Решение «пока ПК» — `Ai.Decisions/2026-10-30-pc-first.md`.

## Что пошло не так и как исправили

- Подпись внизу не читалась — в курсе все тексты Bold, `Count_text` своего цвета; скрипт поставил обычные. Исправлено в скрипте.
- В Prefab Mode окно «разваливалось» — не баг: маленький `Canvas (Environment)`, а внутри контейнеров фиксированные размеры. Смотреть в контексте сцены; в игре размер держит `CanvasScaler` (для окон — Match 0, как в курсе).
- Новые префабы скрипт сначала создавал в открытой сцене — переделано на preview-сцену, чтобы не помечать сцену изменённой.

## Проверка

`0 error CS`; префабы и `.meta` по файлам (иерархия, якоря, спрайты, Bold, `m_ScrollSensitivity: 50`); пользователь — вид в Game view, колесо в Play Mode.

Бриф: `Ai.Tasks/2026-10-30-ui-shop-layout.md` (локально). Знания: `<Knowledge>\conventions\unity-ui.md`.
