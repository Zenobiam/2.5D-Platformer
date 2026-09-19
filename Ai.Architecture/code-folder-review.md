# Обзор папок `Assets/Game/Scripts`

**Дата:** 2026-09-19  
**Статус:** актуальный снимок (не PR)

## Слои

```
GameBootstrapper → GameMain → GameStateMachine
  BootstrapState → LoadProgressState → LoadLevelState → GameLoopState
```

| Папка | Роль |
|---|---|
| `Core/` | Bootstrapper, GameMain, runner |
| `StateMachine/` | игровые состояния (не путать с устаревшим «player State/») |
| `Infrastructure/` | ServiceLocator |
| `Factories/` + `Providers/` | GameFactory, IAssetsProvider, AssetPath |
| `Services/` | Input, SaveLoad, PersistentProgress |
| `Player/` | Movement + Visual |
| `Enemy/` | Animator, Aggro, NavMesh follow (блок курса 05) |
| `Animation/` | IAnimationStateReader, AnimatorStateReporter, AnimatorState |
| `Camera/`, `Triggers/`, `Data/`, `Constants/` | вспомогательное |

## Вердикт

Раскладка совпадает с правилами проекта и курсом: сервисы без MonoBehaviour где можно, геймплей через factory/assets, стиль `_camelCase` + GetComponent в `Awake`.  
Дальше по курсу — довести Enemy (aggro/атаки/спавн), не выдумывая новый DI-стек.

Подробности стека: [`../Ai.Context/project-context.md`](../Ai.Context/project-context.md).  
Связанный issue: [`../Ai.Issues/features/2026-09-19-code-architecture-review.md`](../Ai.Issues/features/2026-09-19-code-architecture-review.md).
