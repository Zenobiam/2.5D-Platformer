# 2026-10-24 — Файлы входа агентов, INTENT.md, механическая защита git

Статус: accepted
Контекст: стартовый промпт (порядок чтения, стоп-правила) пользователь вставлял вручную каждую сессию; цели проекта были разбросаны по `project-context.md` § 1 и `.cursor/rules/project-goals.mdc`; правило «не коммитить без просьбы» держалось только на тексте, а `.claude/settings.local.json` разрешал `git add/commit/reset/restore` без вопроса. Поводом стал разбор чужого опыта (система правил для агентов в не-Unity репозитории).
Решение:
- `AGENTS.md` в корне — общее ядро (порядок чтения, стоп-правила); `CLAUDE.md` импортирует `AGENTS.md`, `INTENT.md`, `Ai.Context/agent-playbook.md`.
- `INTENT.md` — источник истины по целям и не-целям; `project-context.md` § 1 и `project-goals.mdc` ссылаются на него.
- `.claude/settings.json` проекта: `permissions.ask` для `git commit`, `push`, `reset`, `checkout`, `restore`, `clean`, `rm` (Bash и PowerShell, плюс `Remove-Item`). Проверено 2026-10-24: запрос подтверждения появляется и в режиме bypass permissions, и в той же сессии без перезапуска (тест — `git checkout --help`).
- Заодно: карта «курс → у нас» (`Ai.Architecture/course-map.md`), порядок работы и временный код в playbook, таблица «что обновить» в `Ai.Context/README.md`, архив handoff, реестр классов дефектов `D:\Unity\Knowledge\process\review-insights.md`.
- Дополнение того же дня: общий регламент вынесен в `D:\Unity\Knowledge\process\agent-workflow.md` (в `agent-playbook.md` осталась проектная часть); `permissions.ask` — также в `~/.claude/settings.json`; глобальный `~/.claude/CLAUDE.md` импортирует `agent-workflow.md`; `D:\Unity\Knowledge` — git-репозиторий; набор для нового проекта и для коллег — `D:\Unity\Knowledge\starter-kit\` (заменяет `Docs/AgentHandoff/`). Цель — минимум настройки нового проекта и передача коллегам.
Почему: ядро правил подгружается само и переживает сжатие контекста; цели в одном месте; необратимые git-действия подтверждаются harness'ом, а не памятью агента.
Отвергнуто:
- «Не заводить `CLAUDE.md`/`AGENTS.md`» (запись в `project-context.md` § 3 до 2026-10-24) — промпт приходилось повторять.
- Только `CLAUDE.md` — Cursor читает `AGENTS.md`, ядро было бы не общим.
- Из чужой системы не взяли: контрактные тесты в CI, зеркало skills, политику покрытия, шаблон MR, серии код-планов, рабочий дневник — для учебного проекта без CI это лишнее.
