# 2026-09-15 — Настройка инструментов: Unity CLI, пакет Cursor IDE, клипы-заглушки, Rider

Статус: resolved. Источник: расшифровка Cursor `~/.cursor/projects/d-Unity-Projects-2-5D-Platformer/agent-transcripts/a78367c5-…` (2026-09-15), перенесено 2026-10-25. Прокси/TUN и `Mono.Cecil` — в [`2026-09-19-console-errors.md`](2026-09-19-console-errors.md).

## 1. Пакет Cursor IDE не ставился

**Симптом:** Package Manager: `The requested dependency 'com.unity.ide.cursor' does not match the name 'com.boxqkrtm.ide.cursor'`.
**Причина:** ключ git-пакета в `Packages/manifest.json` должен совпадать с `name` в `package.json` репозитория пакета.
**Сделали:** ключ `com.boxqkrtm.ide.cursor` (так в `manifest.json` на 2026-10-25), версия 2.0.28.

## 2. Спам «Cannot bind generic curve on Transform» от клипов Skeleton

**Причина:** агент сгенерировал клипы-заглушки (`LeftAttack`, `RightAttack`, `SplashAttack`, `Damage`, `Death`, `death_test`), записав `m_LocalPosition.x` и т. п. как generic float curves. Mecanim биндит Transform только через position/rotation/scale curves.
**Сделали:** клипы переписаны на `PositionCurves` / `EulerCurves`. Позже `Damage.anim` и `Death.anim` пересобраны скриптом `Tools → Setup → Skeleton Animations` (`Ai.Architecture/editor-setup.md` § 4).

## 3. Розовые материалы на URP

Материалы Skeleton (`SceletonVersion1–3_m`) и тайлов AssetHunts (`Color_Palette_01`) были на Built-in Standard → переведены на URP Lit (проверено по `m_Shader` 2026-10-25). Для Yurowm — [`2026-09-19-yurowm-materials.md`](2026-09-19-yurowm-materials.md).

## 4. Rider не предлагал MonoBehaviour

**Причина:** нет `.sln`/`.csproj` — Rider не видит проект как Unity.
**Сделали:** Unity → Preferences → External Tools → Rider → Regenerate project files; открывать `.sln`, плагин Unity Support включён.

## 5. Unity CLI / Pipeline

После `unity pipeline install` Pipeline-сервер в Editor недоступен до перезапуска Unity; второй экземпляр того же проекта не открывать. Шаги и ловушки — `<Knowledge>\process\agent-tooling.md`.
