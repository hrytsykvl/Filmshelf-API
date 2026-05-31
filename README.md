# FilmShelf API

Веб-API на .NET 8 для пошуку фільмів, рецензій та інтелектуальних рекомендацій. FilmShelf поєднує дані з **TMDb**, традиційне **машинне навчання** та **великі мовні моделі (LLM)** для надання персоналізованих рекомендацій фільмів за допомогою шести різних стратегій.

## Архітектура

Рішення побудоване на багатошаровій архітектурі:

```
FilmShelf.API        →  Контролери, middleware, валідація (ASP.NET Core 8)
FilmShelf.BAL        →  Бізнес-логіка, сервіси, рекомендаційні системи
FilmShelf.DAL        →  Entity Framework Core, репозиторії, Unit of Work
FilmShelf.TMDbClient →  Інтеграція з TMDb API v3 через RestSharp
```

## Основні можливості

### Каталог фільмів та інтеграція з TMDb

FilmShelf отримує каталог фільмів з [The Movie Database (TMDb)](https://www.themoviedb.org/) API v3. Фоновий сервіс синхронізації (`SyncBackgroundService`) завантажує популярні та трендові фільми за налаштованим щоденним розкладом, зберігаючи знімки сторінок для пакетного імпорту. Фільми збагачуються акторським складом, режисерами, жанрами та постерами.

- **Пошук** фільмів за назвою через запити до TMDb
- **Пакетний імпорт** до 100 фільмів за TMDb ID в одному запиті
- **Планова синхронізація** щоденно завантажує нові популярні фільми

### Рекомендаційна система — шість методів

FilmShelf реалізує шість стратегій рекомендацій, кожна з різними компромісами між точністю, швидкістю та інтерпретованістю:

| Метод | Підхід | Ключова деталь |
|---|---|---|
| **ML (матрична факторизація)** | Колаборативна фільтрація через Microsoft.ML | Декомпозиція матриці оцінок, 20 ітерацій |
| **На основі контенту** | Вектори ознак із жанрів, режисера, акторів | Косинусна подібність з L2-нормалізованими профілями |
| **User-CF** | Колаборативна фільтрація на основі користувачів | Кореляція Пірсона, K=30 найближчих сусідів |
| **LLM (Claude)** | Anthropic Claude з аналізом кожного фільму | Оцінки 0–10 з поясненнями природною мовою |
| **Llama/Ollama** | Локальна Llama 3.2 або Azure OpenAI (GPT-5.3) | Подвійний режим: локальний інференс або хмарний fallback |
| **Embedding** | Azure OpenAI embeddings + Azure AI Search | HNSW-пошук по 1536-вимірним векторам |

Усі методи мають спільний інтерфейс: повертають top-K результатів, виключають вже оцінені та додані до списку перегляду фільми, підтримують hold-out оцінювання.

#### LLM-рекомендації

LLM-рекомендатори будують профіль користувача з історії рецензій, а потім просять модель проаналізувати кожен фільм-кандидат і присвоїти оцінку релевантності. Відповіді кешуються на 24 години для кожного користувача та провайдера для контролю вартості та затримок.

- **Claude**: Використовує Anthropic Messages API (`claude-sonnet-4-6`) зі структурованим JSON-виводом, 6000 max tokens.
- **Llama/Ollama**: Працює з локальним екземпляром Ollama (`llama3.2`) або перемикається на Azure OpenAI chat deployment, налаштовується через `LlamaSettings.UseAzureOpenAi`.

#### Embedding-рекомендації

Фільми індексуються в **Azure AI Search** з текстовими описами, перетвореними на 1536-вимірні вектори через модель `text-embedding-3-small` від Azure OpenAI. Під час рекомендації історія рецензій користувача кодується в embedding профілю та зіставляється з індексом за допомогою HNSW-пошуку.

### Офлайн-оцінювання

Система оцінювання (`OfflineEvaluationService`) тестує всі шість методів за допомогою крос-валідації leave-one-out. Метрики:

- **Hit Rate** — Чи потрапив вилучений фільм у top-K?
- **Precision@K** — Частка релевантних елементів у top-K
- **NDCG** — Нормалізований дисконтований кумулятивний виграш
- **Reciprocal Rank** — Позиція першого влучання в ранжуванні

Порівняння методів: `/api/evaluation/all` (усі разом) або `/api/evaluation/{method}` (окремо).

### Рецензії та соціальні функції

- Користувачі оцінюють фільми (0–10) та пишуть текстові рецензії
- Відповіді на рецензії у вигляді гілок обговорення
- **Сповіщення в реальному часі** через SignalR при відповіді на вашу рецензію
- Щоденні масові сповіщення з популярними фільмами

### Списки перегляду

Користувачі створюють іменовані списки перегляду та додають до них фільми. Фільми зі списків виключаються з результатів рекомендацій, щоб пропонувати лише нові відкриття.

### Автентифікація

- **JWT Bearer Tokens** — access-токени на 1 годину з refresh-токенами на 24 години (HMAC SHA256)
- **Google OAuth** — вхід через Google з валідацією токена
- **Скидання пароля** — процес через email за допомогою Mailjet

## Технологічний стек

| Рівень | Технологія |
|---|---|
| Фреймворк | .NET 8, ASP.NET Core |
| База даних | SQL Server 2022, Entity Framework Core |
| Автентифікація | ASP.NET Identity, JWT, Google OAuth |
| ML | Microsoft.ML (матрична факторизація) |
| LLM | Anthropic Claude API, Ollama, Azure OpenAI |
| Векторний пошук | Azure AI Search (HNSW) |
| Реальний час | SignalR WebSockets |
| Email | Mailjet |
| Дані фільмів | TMDb API v3 (RestSharp) |
| Логування | Serilog (консоль + файл з ротацією) |
| Валідація | FluentValidation |
| Маппінг | AutoMapper |
| Контейнери | Docker, Docker Compose |
| Хмара | Azure (Key Vault, OpenAI, AI Search, Container Apps) |

## Початок роботи

### Передумови

- .NET 8 SDK
- SQL Server 2022 (або використовуйте Docker Compose)
- TMDb API ключ ([themoviedb.org](https://www.themoviedb.org/settings/api))

### Запуск через Docker Compose

```bash
docker-compose up
```

Це запускає API на порту `8080` та екземпляр SQL Server на порту `1433`.

### Локальний запуск

1. Оновіть `appsettings.json`, вказавши рядок підключення та API-ключі:
   - `TmdbSettings:ApiKey` — API-ключ TMDb
   - `JwtSettings:Key` — секретний ключ JWT
   - `ClaudeSettings:ApiKey` — API-ключ Anthropic (необов'язково, для LLM-рекомендацій)
   - `AzureOpenAiSettings` — облікові дані Azure OpenAI (необов'язково, для embeddings/Llama)
   - `AzureSearchSettings` — облікові дані Azure AI Search (необов'язково, для embedding-рекомендацій)
   - `MailJetSettings` — облікові дані Mailjet (необов'язково, для email)

2. Застосуйте міграції та запустіть:
   ```bash
   cd FilmShelf/FilmShelf.API
   dotnet ef database update --project ../FilmShelf.DAL
   dotnet run
   ```

3. Swagger UI доступний за адресою `/swagger`.

### Додаткові сервіси

| Функція | Потребує |
|---|---|
| LLM-рекомендації (Claude) | API-ключ Anthropic |
| LLM-рекомендації (Llama) | Локальний екземпляр Ollama або Azure OpenAI |
| Embedding-рекомендації | Azure OpenAI + Azure AI Search |
| Email (скидання пароля) | Обліковий запис Mailjet |
| Векторне індексування | Індекс Azure AI Search |

## Огляд API

| Ендпоінт | Опис |
|---|---|
| `GET /api/movie/{id}` | Деталі фільму |
| `GET /api/movie/search` | Пошук фільмів |
| `GET /api/movie/recommendations` | Отримати рекомендації (method: ml, content, user-cf, llm, embedding, llama) |
| `GET /api/movie/recommendations/compare` | Порівняння ML та LLM |
| `POST /api/movie/import` | Пакетний імпорт з TMDb |
| `POST /api/account/token` | Вхід |
| `POST /api/account/register` | Реєстрація |
| `POST /api/account/google-login` | Google OAuth |
| `CRUD /api/review` | Рецензії на фільми |
| `CRUD /api/watchlist` | Списки перегляду |
| `GET /api/notification` | Сповіщення користувача |
| `GET /api/evaluation/all` | Бенчмарк усіх методів рекомендацій |

## Структура проєкту

```
FilmShelf.API/
├── Controllers/         # Movie, Account, Review, Watchlist, Notification, Actor, Evaluation
├── Middleware/           # Обробка виключень
├── VMs/                 # View-моделі та валідатори
└── Program.cs           # Реєстрація сервісів та конвеєр

FilmShelf.BAL/
├── Services/
│   ├── RecommendationService              # ML матрична факторизація
│   ├── ContentBasedRecommendationService  # Фільтрація на основі контенту
│   ├── CollaborativeRecommendationService # User-CF (Пірсон + KNN)
│   ├── LlmRecommendationService           # Рекомендації Claude
│   ├── LlamaRecommendationService         # Ollama / Azure OpenAI
│   ├── EmbeddingRecommendationService     # Векторний пошук
│   ├── OfflineEvaluationService           # Метрики оцінювання
│   └── ...
├── DTOs/
└── Mapping/             # Профілі AutoMapper

FilmShelf.DAL/
├── Context/             # EF Core DbContext (IdentityDbContext)
├── Entities/            # Movie, Review, Actor, Genre, Notification тощо
├── Repositories/        # Репозиторій + Unit of Work
└── Migrations/

FilmShelf.TMDbClient/
├── Services/            # Інтеграція з TMDb API
├── Models/              # Моделі відповідей TMDb
└── Settings/            # Конфігурація TMDb
```
