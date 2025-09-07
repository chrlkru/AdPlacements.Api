# AdPlacements.Api

## ������ �������

### ����������
- .NET 8 SDK

### ����������
1. ����������� �����������:
   ```bash
   git clone https://github.com/<username>/<repo>.git
   cd <repo>/AdPlacements.Api
��������� ������:

bash
���������� ���
dotnet run
������� Swagger UI �� ������, ������� ����� ������� � ������� (������ http://localhost:5000/swagger).

API
POST /api/platforms/upload � �������� ����� � ���������� (multipart/form-data, ���� File).

GET /api/platforms?location=/path � ����� �������� ��� ��������� �������.

������
bash
���������� ���
curl -X POST -F "file=@sample-data/valid.txt" http://localhost:5000/api/platforms/upload
curl "http://localhost:5000/api/platforms?location=/ru/svrd/revda"