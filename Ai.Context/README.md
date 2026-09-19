# Ai.Context — канон для агентов

**Читать в первую очередь** перед любой работой по этому репозиторию.

## Порядок чтения

1. Этот README  
2. [`agent-playbook.md`](agent-playbook.md) — **обязательно для workers**: роли, kickoff, минимум ops  
3. [`project-context.md`](project-context.md) — цели, стек, правила, handoff  
4. [`preferences.md`](preferences.md) — куда класть deliverables  
5. При необходимости — [`../Ai.Issues/`](../Ai.Issues/) (прошлые проблемы и решения)  
6. Архитектурный обзор — [`../Ai.Architecture/`](../Ai.Architecture/)

## Папки `Ai.*` в корне репо

| Папка | Назначение |
|---|---|
| `Ai.Context/` | Долгоживущий канон (этот раздел) |
| `Ai.Issues/` | Один MD на issue: проблема → причина → что сделали |
| `Ai.Architecture/` | Обзоры структуры кода / сверка с курсом |

Имена папок именно `Ai.<name>`, не `issues` / `context` / `docs` для агентского канона.

## Связанные пути

- Правила стиля/целей: [`.cursor/rules/`](../.cursor/rules/)  
- Handoff-kit для **нового** репо: [`Docs/AgentHandoff/HANDOFF.md`](../Docs/AgentHandoff/HANDOFF.md)  
- Старый указатель: [`Docs/project-context.md`](../Docs/project-context.md) → редирект сюда  

## Схема имён Issues

`Ai.Issues/YYYY-MM-DD-short-kebab.md` (дата сессии/фикса + короткий slug).
