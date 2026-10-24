# Ai.Tasks — kickoff-брифы для агентов

Один файл — одна задача. Формат берётся из [`Ai.Context/agent-playbook.md`](../Ai.Context/agent-playbook.md) (Kickoff template).

## Шаблон

```text
### YYYY-MM-DD — короткое название
Статус: planned | in progress | done | blocked
Исполнитель: координатор | unity-worker | readme-check | человек

Goal:
Constraints:
Paths:
DoD:
Don't-touch:

Итог:
```

## Правила

- Имя файла: `YYYY-MM-DD-short-kebab.md`, при появлении нового статуса файл обновляем, а не дублируем.
- Бриф пишется до старта работы, итог дописывается после.
- Завершённые задачи, которые привели к фиксу, дублируются в `Ai.Issues/` (bugs или features).
- Задачи по Enemy/Aggro, которые ведёт другой агент, сюда не ставим без его ведома.
