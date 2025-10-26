# If Trees Could Talk (Game Off 2024)

An async multiplayer prototype built with Unity (6000.0.23f1) and a Django REST API.

- Backend: Django 5, Django REST Framework, PostgreSQL, Nginx, Docker
- Client: Unity 6000.0.23f1 (a.k.a. Unity 6)
- License: MIT

## Game Jam
Originally created for GitHub Game Off 2024.
- Itch page: https://itsmawal.itch.io/if-trees-could-talk
- Jam submission: https://itch.io/jam/game-off-2024/rate/3148425

## Overview
This repo contains two main parts:
- `game/` — Unity project with gameplay prototype.
- `api/` — Django REST API used by the game for auth and persistent world messages.

Core API features:
- Auth via Unity Player Auth bearer token → issues a short-lived API token for subsequent requests
- Create and list in-world messages with a position and normal vector
- Owner-restricted updates on messages

## Tech Stack
- Django 5.1.x, DRF 3.15.x
- PostgreSQL 14
- Python 3.13 (Docker images)
- Nginx reverse proxy (HTTP→HTTPS redirect, static serving)
- Unity 6000.0.23f1

## Repository Layout
- `game/` — Unity project
- `api/` — Django backend
  - `service/` — app containing models, views, serializers, auth
  - `app/` — project config (settings, urls)
  - `docker-compose.yml`, `Dockerfile`, `nginx/`

## Getting Started (Backend)

### Prerequisites
- Docker and Docker Compose
- Optional: Python 3.13 + virtualenv if running without Docker

### Environment Variables
Create `api/.env` (the backend reads from `api/app/.env` path resolution). Example values:

```env
# Django
SECRET_KEY=replace-me
DEBUG=True
ALLOWED_HOSTS=localhost,127.0.0.1
CSRF_TRUSTED_ORIGINS=https://localhost,http://localhost

# Database
POSTGRES_NAME=gameoff
POSTGRES_USER=gameoff
POSTGRES_PASSWORD=gameoff

# Admin
ADMIN_PATH=admin/

# Unity Auth
UNITY_PROJECT_ID=your-unity-project-id
```

Notes:
- `ADMIN_PATH` controls the Django admin URL path.
- `UNITY_PROJECT_ID` is forwarded to Unity Player Auth validation.

### Run with Docker
From `api/` directory:

```bash
docker compose up --build
```

This will start:
- PostgreSQL (`db_postgresql`)
- Django `backend` (gunicorn on port 443 in the internal network)
- Nginx `nginx` (serving 80→443 with TLS using certs in `api/nginx/certs`)

Access:
- API via `https://localhost/` (proxied to backend)
- Static files under `https://localhost/static/`

Provide valid TLS cert/key in `api/nginx/certs` as `fullchain.pem` and `privkey.pem`. For local dev, you may use self-signed certs.

### Run Locally without Docker (dev only)
```bash
cd api
python -m venv .venv && . .venv/Scripts/activate  # Windows PowerShell adjust as needed
pip install -r requirements.txt
python manage.py migrate
python manage.py runserver 0.0.0.0:8000
```

Then hit `http://localhost:8000/api/` directly (skipping Nginx). Ensure you set `DEBUG=True` and `ALLOWED_HOSTS` appropriately.

## API
Base paths (via project urls):
- Admin: `/{ADMIN_PATH}` (from env)
- API root: `/api/`
- DRF login: `/api-auth/`

### Authentication Flow
1) Client (Unity) obtains a Unity Player Auth bearer token.
2) Call our endpoint to exchange it for an API token (or create new user+token):

```
POST /api/authenticate/
Authorization: Token {unity_bearer_token}
Content-Type: application/json

{
  "playerId": "<unity_player_id>",
  "playerName": "OptionalDisplayedName"
}
```

Response:
```json
{"token": "<api_token>"}
```

3) Use the returned API token for subsequent requests:
```
Authorization: Token <api_token>
```

### Endpoints

- `GET /api/` — API root with links
- `POST /api/authenticate/` — exchange Unity bearer for API token
- `GET /api/messages/` — list messages
- `POST /api/messages/` — create message
- `GET /api/messages/{id}/` — retrieve message
- `PUT/PATCH /api/messages/{id}/` — update message (owner-only)

#### Message Schema
- `id`: integer (read-only)
- `owner`: string (read-only; set from authenticated user)
- `message`: string (required)
- `position`: JSON object `{ "x": float, "y": float, "z": float }`
- `normal`: JSON object `{ "x": float, "y": float, "z": float }`

Validation ensures `position` and `normal` are dicts with numeric `x|y|z`.

#### Examples (curl)
List messages:
```bash
curl -k https://localhost/api/messages/
```

Create message:
```bash
curl -k -X POST https://localhost/api/messages/ \
  -H "Authorization: Token <api_token>" \
  -H "Content-Type: application/json" \
  -d '{
    "message": "Hello from the forest",
    "position": {"x": 1.0, "y": 2.0, "z": 3.0},
    "normal": {"x": 0.0, "y": 1.0, "z": 0.0}
  }'
```

Update message:
```bash
curl -k -X PATCH https://localhost/api/messages/1/ \
  -H "Authorization: Token <api_token>" \
  -H "Content-Type: application/json" \
  -d '{"message": "Edited"}'
```

Notes:
- `-k` allows self-signed TLS during local dev.
- Owner-only updates enforced by `IsOwnerOrReadOnly`.

## Unity Project
- Version: 6000.0.23f1 (Unity 6)
- Open `game/` in Unity Hub/Editor.
- Configure your environment or in-game settings to supply the Unity bearer to the backend `POST /api/authenticate/`, then persist the returned API token for subsequent requests.

### Third-Party Unity Assets
This project used several third-party Unity assets during development. They are not included in this repository due to licensing. The project should open and run; you may need to reimport equivalent assets locally (textures, models, shaders, music/SFX, or tooling) for full fidelity.

## Deployment Notes
- Nginx proxies 80→443 and upstreams to Gunicorn at `backend:443` (Docker network). Ensure certs are present at `api/nginx/certs`.
- Set `DEBUG=False`, strong `SECRET_KEY`, and proper `ALLOWED_HOSTS`, `CSRF_TRUSTED_ORIGINS`.
- Database is Postgres 14 via the included service; use managed Postgres in production.

## Contributing
PRs and issues welcome. Please open issues for bugs or feature ideas. For larger changes, start a discussion.

## License
MIT — see `LICENSE`.
