# Michi — Backend

## Wymagania wstępne

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (z obsługą `docker compose`)

## Szybki start (środowisko deweloperskie)

1. **Utwórz lokalny plik `.env` na podstawie szablonu:**
   ```bash
   cp .env.example .env
   ```
   *(W Windows PowerShell: `Copy-Item .env.example .env`)*

2. **Wygeneruj klucz `JWT_KEY` (minimum 32 znaki) i wpisz go do pliku `.env`:**

3. **Uruchom kontenery deweloperskie:**
   ```bash
   docker compose -f docker-compose.dev.yml up -d
   ```

4. **Sprawdź status kontenerów:**
   ```bash
   docker compose -f docker-compose.dev.yml ps
   ```

## Usługi lokalne i porty

| Usługa | Obraz | Adres / Port | Domyślne dane dostępowe (`.env.example`) |
| :--- | :--- | :--- | :--- |
| **PostgreSQL + PostGIS** | `postgis/postgis:16-3.4` | `localhost:5432` | Baza: `michi_db`<br>User: `michi`<br>Hasło: `michi_pass` |
| **RustFS (S3 Storage)** | `rustfs/rustfs:latest` | API: `http://localhost:9000`<br>Panel: `http://localhost:9001` | Login: `rustfsadmin`<br>Hasło: `rustfsadmin`<br>Bucket: `michi-dev-media` |
| **Mailpit (SMTP / Web UI)** | `axllent/mailpit:latest` | SMTP: `localhost:1025`<br>Panel: `http://localhost:8025` | Brak autoryzacji |

### Pierwsza konfiguracja bucketa S3 (RustFS)

Po pierwszym uruchomieniu kontenerów wejdź na `http://localhost:9001`, zaloguj się danymi z pliku `.env` (domyślnie `rustfsadmin` / `rustfsadmin`) i utwórz prywatny bucket o nazwie zgodnej ze zmienną `S3_BUCKET_NAME` (`michi-dev-media`). Pliki są przechowywane w trwałym wolumenie `s3-data`.

## Przydatne komendy

- **Zatrzymanie kontenerów (dane w wolumenach pozostają zachowane):**
  ```bash
  docker compose -f docker-compose.dev.yml down
  ```
- **Pełny reset środowiska (usunięcie kontenerów wraz z wolumenami bazy i plików):**
  ```bash
  docker compose -f docker-compose.dev.yml down -v
  ```
- **Weryfikacja rozszerzenia PostGIS w bazie danych:**
  ```bash
  docker exec -it michi-db psql -U michi -d michi_db -c "SELECT PostGIS_Full_Version();"
  ```

## Struktura plików konfiguracyjnych

- `docker-compose.dev.yml` — definicja usług deweloperskich (PostgreSQL + PostGIS, RustFS, Mailpit) oraz wolumenów (`db-data`, `s3-data`)
- `.env.example` — szablon zmiennych środowiskowych wymaganych do uruchomienia projektu
- `.editorconfig` — spójne zasady formatowania kodu dla całego zespołu
- `.gitignore` / `.dockerignore` — wykluczenia plików lokalnych, sekretów (`.env`) oraz katalogów IDE (`.idea/`)