# Handoff: контекст для нового проекта / нового чата

Скопируй эту папку в новый Unity-проект (или вставь этот файл в первый сообщение агенту через `@`).

## Что уже глобально (не копировать)

Лежит в `%USERPROFILE%\.cursor\rules\` и действует во всех проектах:

- `unity-csharp-principles.mdc` — общий стиль Unity C#
- `explain-comparisons.mdc` — «лучше/хуже» только с объяснением почему

## Что скопировать в новый проект

В корень нового проекта:

```
.cursor/rules/csharp-style.mdc      ← адаптируй namespaces под новый репо
.cursor/rules/project-goals.mdc     ← перепиши цели/жанр нового проекта
.cursor/rules/course-reference.mdc  ← если снова идёшь по k-syndicate
.cursor/mcp.json                    ← поправь --project-path на новый путь
```

Шаблоны правил — рядом в `rules-templates/`.

## Unity CLI + Editor (один раз на машине)

Уже установлено: `%LOCALAPPDATA%\Unity\bin\unity.exe`  
В проекте нужен пакет `com.unity.pipeline` (Unity 6+):

```powershell
$env:Path = "$env:LOCALAPPDATA\Unity\bin;$env:Path"
unity pipeline install --project-path "<NEW_PROJECT>" --non-interactive --proxy-disable
unity mcp configure cursor --local --project-path "<NEW_PROJECT>" --yes
```

Открой Editor с проектом → `unity status` должен показать `ready`.  
VPN: для Package Manager лучше **TUN**, не Proxy (кривой system proxy ломает `packages.unity.com`).

## Референс курса

`G:\MediaGetDownloads\RealyGood\k-syndicate.school - Архитектура мобильных игр на UNITY для профессионалов (2021)\`  
CodeBase уроков: `...\0X ...\knowledge-is-power-master\src\KnowledgeIsPower\Assets\CodeBase`  
Сверяться с курсом, не «улучшать» без просьбы.

## Исходный эталонный проект

`D:\Unity\Projects\2.5D Platformer`  
Жанр: 2.5D platformer. Цель: пример + документация для ускорения следующих проектов (в т.ч. с агентами).

Ключевые паттерны:

- `GameBootstrapper` → `GameMain` → `GameStateMachine`
- `ServiceLocator` + factories + `IAssetsProvider`
- `EnemyAnimator` + `IAnimationStateReader` + `AnimatorStateReporter`
- Уровень: `SampleScene` → `Level/DemoRoute` (Tools → Build Demo Level Geometry)

## Промпт для нового чата (можно вставить целиком)

```
Продолжаем работу по тому же подходу, что в эталоне 2.5D Platformer.
Прочитай @Docs/AgentHandoff/HANDOFF.md и правила в .cursor/rules.
User rules (Unity C# + объяснять сравнения) уже глобальные.
Сверяйся с курсом k-syndicate CodeBase, когда делаем фичи из уроков.
Не трогай логику/save без просьбы; стиль: _fields, GetComponent в Awake, без лишнего SerializeField.
Цель: читаемый переносимый код + документация для агентов.
Текущая задача: <ОПИШИ ЗАДАЧУ>
```
