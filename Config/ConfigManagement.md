# Спецификация конфигурации

## 1. Система контроля версий
- Используется Git.
- Репозиторий: локальный `HealthMonitoringApp/.git`.
- Удалённый репозиторий: (если есть) `https://github.com/student/HealthMonitoringApp`

## 2. Управление версиями
- Семантическое версионирование: `MAJOR.MINOR.PATCH`.
- Текущая версия: 1.0.0.

## 3. Игнорируемые файлы (`.gitignore`)
Visual Studio
bin/
obj/
.vs/
*.user
*.suo

Database
*.db
*.sqlite

Logs
*.log
temp/

## 4. Настройки Git
- `core.autocrlf = true` (для совместимости окончаний строк в Windows).
- Имя пользователя: Student.
- Email: student@bmtech.ru.

## 5. Правила оформления коммитов
- Сообщение на русском или английском, кратко отражающее суть изменений.
- Пример: `"Add requirements document"` или `"Исправлена ошибка парсинга сахара"`.