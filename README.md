# Mimre API

A production-ready REST API for an online photo gallery platform built for photographers. Built with C# .NET 10 following Clean Architecture, Domain-Driven Design, and CQRS patterns.

---

## Features

- **Gallery Management** - Create, organize, and publish photo galleries with password protection and expiry dates
- **Album Organization** - Group photos into albums within galleries
- **Async Photo Processing** - Upload photos and have thumbnails and watermarks generated automatically in the background
- **Client Sharing** - Generate share links for clients to view galleries without an account
- **JWT Authentication** - Secure photographer accounts with access and refresh token rotation
- **Infinite Scroll Pagination** - Cursor-based pagination for photos, offset-based for galleries and albums
- **Redis Caching** - Distributed caching for high-traffic public gallery endpoints
- **Rate Limiting** - IP-based rate limiting per endpoint type
- **Structured Logging** - Serilog with console, file, and Seq sinks

---

## Tech Stack

| Layer            | Technology                                   |
| ---------------- | -------------------------------------------- |
| Language         | C# .NET 10                                   |
| API              | ASP.NET Core Minimal APIs                    |
| Database         | MSSQL via Entity Framework Core (Code-First) |
| Storage          | Azure Blob Storage (Azurite locally)         |
| Queue            | Azure Storage Queues                         |
| Cache            | Redis (in-memory fallback for development)   |
| Image Processing | SkiaSharp                                    |
| Authentication   | JWT Bearer + Refresh Tokens                  |
| Validation       | FluentValidation                             |
| Mediator         | MediatR (CQRS)                               |
| Logging          | Serilog                                      |
| Containerization | Docker + Docker Compose                      |
| CI               | GitHub Actions                               |

---

## Architecture

The solution follows **Clean Architecture** with strict inward dependency rules:

```
Mimre.Api                - Entry point, Minimal API endpoints, middleware
Mimre.Application        - CQRS handlers, DTOs, validators, interfaces
Mimre.Infrastructure     - EF Core, Azure Blob, Redis, JWT, SkiaSharp
Mimre.Domain             - Entities, domain events, exceptions, interfaces
Mimre.Worker             - Background service for async image processing
```

### Key Patterns

- **Domain-Driven Design** - Entities encapsulate their own business rules with private setters and domain methods
- **CQRS via MediatR** - Commands and queries are strictly separated with a pipeline for validation and domain event dispatch
- **Repository + Unit of Work** - Abstracts data access with a single transaction boundary
- **Domain Events** - Entities raise events (e.g. `PhotoUploadedEvent`, `GalleryPublishedEvent`) dispatched after `SaveChanges`

---

## Project Structure

```
Mimre/
  ├── Mimre.Domain/
  │     ├── Entities/           - User, Gallery, Album, Photo, ShareLink
  │     ├── Events/             - Domain events
  │     ├── Exceptions/         - DomainException, NotFoundException
  │     └── Interfaces/         - Repository and service contracts
  ├── Mimre.Application/
  │     ├── Common/
  │     │     ├── Behaviors/    - ValidationBehavior, DomainEventBehavior
  │     │     ├── Interfaces/   - IUnitOfWork, ICacheService
  │     │     └── Settings/     - JwtSettings, CacheSettings
  │     ├── DTOs/               - GalleryDto, AlbumDto, PhotoDto, etc.
  │     └── Features/
  │           ├── Auth/         - Register, Login, RefreshToken
  │           ├── Galleries/    - CRUD + Publish + Password + Cover
  │           ├── Albums/       - CRUD
  │           ├── Photos/       - Upload, Delete, SetCover, Process
  │           ├── ShareLinks/   - Create, Delete, GetByToken, Verify
  │           └── Users/        - GetCurrentUser, UpdateProfile
  ├── Mimre.Infrastructure/
  │     ├── Auth/               - JwtTokenService, PasswordHasher
  │     ├── BlobStorage/        - AzureBlobStorageService
  │     ├── Caching/            - CacheService, MemoryCacheService
  │     ├── Email/              - MailKitEmailService
  │     ├── ImageProcessing/    - SkiaImageProcessingService
  │     ├── Persistence/        - MimreDbContext, Repositories, UoW
  │     └── Queue/              - AzureStorageQueueService
  ├── Mimre.Api/
  │     ├── Endpoints/          - Auth, Gallery, Album, Photo, ShareLink, User
  │     ├── Middleware/         - ExceptionHandlingMiddleware
  │     ├── RateLimiting/       - RateLimitingConfiguration, Policies
  │     └── Services/           - CurrentUserService
  └── Mimre.Worker/
        └── Workers/            - PhotoProcessingWorker
```

---

## API Endpoints

### Auth

| Method | Endpoint             | Description                         |
| ------ | -------------------- | ----------------------------------- |
| POST   | `/api/auth/register` | Register a new photographer account |
| POST   | `/api/auth/login`    | Login and receive JWT tokens        |
| POST   | `/api/auth/refresh`  | Refresh an expired access token     |

### Galleries

| Method | Endpoint                        | Description                     |
| ------ | ------------------------------- | ------------------------------- |
| GET    | `/api/galleries`                | Get all galleries (paginated)   |
| GET    | `/api/galleries/{slug}`         | Get gallery by slug             |
| GET    | `/api/galleries/{id}`           | Get gallery by ID               |
| POST   | `/api/galleries`                | Create a new gallery            |
| PUT    | `/api/galleries/{id}`           | Update gallery title            |
| DELETE | `/api/galleries/{id}`           | Delete gallery and all contents |
| POST   | `/api/galleries/{id}/publish`   | Publish a gallery               |
| POST   | `/api/galleries/{id}/unpublish` | Unpublish a gallery             |
| PATCH  | `/api/galleries/{id}/password`  | Set or remove gallery password  |

### Albums

| Method | Endpoint                          | Description                          |
| ------ | --------------------------------- | ------------------------------------ |
| GET    | `/api/albums/gallery/{galleryId}` | Get albums for a gallery (paginated) |
| POST   | `/api/albums`                     | Create a new album                   |
| PUT    | `/api/albums/{id}`                | Update album title and sort order    |
| DELETE | `/api/albums/{id}`                | Delete album and all its photos      |

### Photos

| Method | Endpoint                       | Description                               |
| ------ | ------------------------------ | ----------------------------------------- |
| GET    | `/api/photos/album/{albumId}`  | Get photos (cursor-based infinite scroll) |
| POST   | `/api/photos/upload/{albumId}` | Upload one or more photos                 |
| DELETE | `/api/photos/{id}`             | Delete a photo                            |
| PATCH  | `/api/photos/{id}/set-cover`   | Set photo as gallery cover                |

### Share Links

| Method | Endpoint                               | Description                   |
| ------ | -------------------------------------- | ----------------------------- |
| GET    | `/api/share-links/gallery/{galleryId}` | Get share links for a gallery |
| POST   | `/api/share-links`                     | Create a share link           |
| DELETE | `/api/share-links/{id}`                | Delete a share link           |

### Client (Public)

| Method | Endpoint                     | Description                             |
| ------ | ---------------------------- | --------------------------------------- |
| GET    | `/api/client/{token}`        | View a published gallery via share link |
| POST   | `/api/client/{token}/verify` | Verify a gallery password               |

### Users

| Method | Endpoint        | Description                 |
| ------ | --------------- | --------------------------- |
| GET    | `/api/users/me` | Get current user profile    |
| PUT    | `/api/users/me` | Update current user profile |

---

## Rate Limiting

| Policy  | Endpoints                   | Limit                         |
| ------- | --------------------------- | ----------------------------- |
| Auth    | `/api/auth/*`               | 10 requests/min per IP        |
| Upload  | `/api/photos/upload`        | 20 burst, 2/sec refill per IP |
| General | All authenticated endpoints | 100 requests/min per IP       |
| Client  | `/api/client/*`             | 200 requests/min per IP       |

---

## Photo Processing Flow

```
Client uploads photo(s)
     ↓
API streams file to Azure Blob Storage (originals/)
     ↓
Photo record created with Status: Pending
     ↓
Job enqueued to Azure Storage Queue
     ↓
Worker picks up job (polls every 2 seconds)
     ↓
Downloads original from blob
     ↓
Generates WebP thumbnail (max 800px, SkiaSharp)
     ↓
Applies watermark (SkiaSharp)
     ↓
Uploads thumbnail → thumbnails/
Uploads watermarked → watermarked/
     ↓
Photo record updated with Status: Ready
```

---

## Getting Started

### Prerequisites

- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [.NET 10 SDK](https://dotnet.microsoft.com/download)

### Running Locally

1. Clone the repository:

```bash
git clone https://github.com/jgmondero/mimre-api.git
cd mimre-api
```

2. Create a `.env` file in the solution root:

```env
SA_PASSWORD=YourStrong@Passw0rd!
JWT_SECRET=your-secret-key-at-least-32-characters-long
STORAGE_KEY=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==
```

3. Start all services:

```bash
docker compose up
```

4. Open the API documentation:

```
http://localhost:8080/scalar
```

### Running Without Docker

1. Start SQL Server, Azurite, and Redis locally
2. Update `appsettings.Development.json` with your connection strings
3. Run migrations:

```bash
dotnet ef database update --project Mimre.Infrastructure --startup-project Mimre.Api
```

4. Run the API and Worker:

```bash
# Terminal 1
cd Mimre.Api && dotnet run

# Terminal 2
cd Mimre.Worker && dotnet run
```

---

## Environment Variables

| Variable      | Description                            |
| ------------- | -------------------------------------- |
| `SA_PASSWORD` | SQL Server SA password                 |
| `JWT_SECRET`  | JWT signing secret (min 32 characters) |
| `STORAGE_KEY` | Azure Storage account key              |

---

## CI/CD

GitHub Actions runs on every push to `main`:

- Restores dependencies
- Builds the entire solution in Release mode
- Verifies API and Worker publish successfully

---

## License

Copyright © 2026 jgmondero. All rights reserved.
