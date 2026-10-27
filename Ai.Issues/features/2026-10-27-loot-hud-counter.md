# 2026-10-27 — Счётчик лута на HUD (урок 10 Loot)

## Проблема

Лут с врагов собирался в `WorldData.LootData.Collected`, но на экране его не было видно.

## Что сделали

- По курсу (итог урока 10): `LootData.Changed` + вызов в `Collect`; `UI/LootCounter` (подписка в `Construct`, `Start` → обновление текста); `GameFactory.CreateHud` передаёт счётчику `WorldData` из прогресса.
- `WorldData` создаёт `LootData` в конструкторе, как в курсе. Без этого на новом прогрессе (`Clear Prefs`) подбор лута падал с NRE в `LootPiece.UpdateWorldData`.
- Отступления (`course-deviations.md`): отписка `LootCounter.OnDestroy` (прогресс живёт дольше HUD); счётчик — дочерний объект `HUD/LootCounter` из `Tools → UI → Build Player HUD`, а не вложенный префаб `Hud/Loot.prefab`.
- Иконка — `Resources/Prefabs/Player/CFX3_T_Skull.png` из курса (импорт Sprite). Текстуру частиц `Effects/…/CFX3_T_Skull.png` не переиспользовали: смена типа импорта сломала бы материал частиц.

## Что пошло не так и как исправили

Длинное число переносилось на вторую строку: у `TextMeshProUGUI` по умолчанию перенос `Normal`, а ширина была фиксированная (280). Исправлено в сборщике: `NoWrap` + `Overflow`, ширину задаёт `HorizontalLayoutGroup` (`childControlWidth/Height`), иконке — `LayoutElement` 100×100. Заодно сборщик перестал затирать цвет, размер и текст, настроенные в Inspector: они задаются только при создании объекта.

## Проверка

`0 error CS`; `HUD.prefab` по файлу (`LootCounter`, `Counter` → `TextMeshProUGUI`, `m_TextWrappingMode: 0`, Missing Script нет); пользователь в Play Mode: число в одну строку, растёт при подборе, сохраняется после перезапуска.

Бриф: `Ai.Tasks/2026-10-27-loot-hud.md` (локально).
