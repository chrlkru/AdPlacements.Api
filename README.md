# AdPlacements.Api

## Запуск сервиса

### Требования
- .NET 8 SDK

### Инструкция
1. Клонировать репозиторий:
   ```bash
   git clone https://github.com/chrlkru/AdPlacements.Api.git
   cd <repo>/AdPlacements.Api
Запустить сервис:

     
      dotnet run

Открыть Swagger UI по адресу, который будет выведен в консоли (обычно http://localhost:7107/swagger).

Открыть Swagger UI по адресу, который будет выведен в консоли (обычно http://localhost:7107/swagger).

API
POST /api/platforms/upload — загрузка файла с площадками (multipart/form-data, поле File).

GET /api/platforms?location=/path — поиск площадок для указанной локации.

Пример
bash
Копировать код
curl -X POST -F "file=@sample-data/valid.txt" http://localhost:5000/api/platforms/upload
curl "http://localhost:5000/api/platforms?location=/ru/svrd/revda"