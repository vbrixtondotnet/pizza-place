# Pizza Place

Full-stack pizza sales analytics platform. An ASP.NET Core Web API serves catalog and order data plus sales insights; an Angular dashboard lets you filter, chart, and browse those sales.

## Project description

Pizza Place loads historical pizza shop data (pizzas, pizza types, orders, and order line items) into SQL Server, then exposes REST endpoints for:

- **Catalog browsing** — pizzas, pizza types, orders, and order details
- **Sales analytics** — KPI cards, prior-period revenue change, peak weekday/hour, daily trend, category mix, and top pizzas
- **Sales search** — paginated, filterable, sortable line items for the dashboard table

The Angular front end is a single sales dashboard: date/category/size/search filters, KPI cards, insight summaries, Chart.js visualizations, and a Material data table. Filters apply consistently across KPIs, charts, and the table.

Profit/margin metrics are intentionally omitted — the source CSVs have no cost fields.

## Repository layout

```
pizza-place/
├── back-end/                 # .NET solution (Clean Architecture)
│   ├── PizzaPlace.Api/       # Web API host, Swagger, CORS, CSV seed data
│   ├── PizzaPlace.Application/
│   ├── PizzaPlace.Domain/
│   ├── PizzaPlace.Infrastructure/
│   └── PizzaPlace.Application.Tests/
└── front-end/                # Angular 22 sales dashboard (pizza-place-web)
```

## Technology stack

### Backend

| Layer | Technology |
| --- | --- |
| Runtime / framework | .NET 10, ASP.NET Core Web API |
| Architecture | Clean Architecture (Api → Application → Domain; Infrastructure implements persistence) |
| ORM | Entity Framework Core 10 (`Microsoft.EntityFrameworkCore.SqlServer`) |
| API docs | Swashbuckle / Swagger (Development only) |
| CSV seeding | CsvHelper |
| Tests | `PizzaPlace.Application.Tests` via `dotnet test` |

### Front end

| Concern | Technology |
| --- | --- |
| Framework | Angular 22 |
| UI | Angular Material + Angular CDK |
| Charts | Chart.js + ng2-charts |
| Language | TypeScript ~6 |
| Unit tests | Vitest + jsdom |
| Dev proxy | `proxy.conf.json` → API on port `5063` |

### Database

| Item | Detail |
| --- | --- |
| Engine | Microsoft SQL Server |
| Default database | `PizzaPlaceDb` |
| Default connection | `Server=localhost\MSSQLSERVER2022;...` in `back-end/PizzaPlace.Api/appsettings.json` |
| Schema / data | EF Core migrations on startup; CSV seed from `PizzaPlace.Api/Data` when the catalog is empty |

Seed CSVs: `pizzas.csv`, `pizza_types.csv`, `orders.csv`, `order_details.csv`.

## Prerequisites

- **.NET 10 SDK**
- **SQL Server** reachable with the connection string above (adjust instance name if yours differs)
- **Node.js** `^22.22.3` or `^24.15.0` (or another engine supported by Angular 22)
- **npm** 8+ (repo lists `packageManager`: `npm@11.12.1`)

## How to run

### 1. Configure the database (if needed)

Edit `back-end/PizzaPlace.Api/appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost\\MSSQLSERVER2022;Database=PizzaPlaceDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
}
```

Ensure SQL Server is running and your Windows account can create/connect to `PizzaPlaceDb`.

### 2. Start the API

```powershell
cd back-end/PizzaPlace.Api
dotnet run
```

| URL | Purpose |
| --- | --- |
| `http://localhost:5063` | HTTP API (default `http` profile) |
| `https://localhost:7043` | HTTPS (`https` profile) |
| `http://localhost:5063/swagger` | Swagger UI (Development) |

On startup the API:

1. Applies EF Core migrations
2. Seeds CSV data from `PizzaPlace.Api/Data` when the catalog is empty

Sample HTTP requests: [`back-end/PizzaPlace.Api/PizzaPlace.Api.http`](back-end/PizzaPlace.Api/PizzaPlace.Api.http).

### 3. Start the Angular dashboard

```powershell
cd front-end
npm install
npm start
```

Open **http://localhost:4200**.

The dev server proxies `/api` to `http://localhost:5063`. The API also allows CORS from `http://localhost:4200` and `https://localhost:4200`.

Run **API and front end together** for a working dashboard.

### Production build (front end)

```powershell
cd front-end
npm run build
```

Output is written under `front-end/dist/`.

## Tests

**Backend**

```powershell
cd back-end
dotnet test
```

**Front end**

```powershell
cd front-end
npm test -- --watch=false
```

## Backend endpoints

Base URL (local): `http://localhost:5063`

### Sales analytics (dashboard)

| Method | Path | Description |
| --- | --- | --- |
| `GET` | `/api/sales/dashboard` | KPIs, insights, daily trend, category breakdown, top pizzas |
| `GET` | `/api/sales` | Paginated searchable sales line items |
| `GET` | `/api/sales/filters` | Distinct categories/sizes and available date bounds |

#### `GET /api/sales/dashboard`

Query parameters:

| Parameter | Type | Notes |
| --- | --- | --- |
| `fromDate` | `DateOnly` (optional) | Inclusive start; defaults from data when omitted |
| `toDate` | `DateOnly` (optional) | Inclusive end |
| `query` | string (optional) | Text filter (e.g. pizza name) |
| `category` | string (optional) | Pizza category filter |
| `size` | string (optional) | Pizza size filter |

Same search/category/size filters as the sales list so KPIs and charts match the table.

Example:

```http
GET /api/sales/dashboard?fromDate=2015-01-01&toDate=2015-01-31&category=Chicken&size=L
```

Dashboard payload includes:

- **KPIs** — total revenue, order count, pizzas sold, average order value, average pizzas per order, revenue change vs the prior equal-length period
- **Insights** — peak weekday, peak hour, top category, top pizza name
- **Daily trend**, **category breakdown**, **top pizzas** (by revenue)

#### `GET /api/sales`

Query parameters:

| Parameter | Type | Default | Notes |
| --- | --- | --- | --- |
| `fromDate` | `DateOnly` (optional) | — | Inclusive start |
| `toDate` | `DateOnly` (optional) | — | Inclusive end |
| `query` | string (optional) | — | Text filter |
| `category` | string (optional) | — | Category filter |
| `size` | string (optional) | — | Size filter |
| `page` | int | `1` | Must be ≥ 1 |
| `pageSize` | int | `25` | Must be 1–100 |
| `sortBy` | string | `date` | See allowed values below |
| `sortDirection` | string | `desc` | `asc` or `desc` |

Allowed `sortBy` values: `date`, `time`, `orderId`, `pizzaName`, `category`, `size`, `quantity`, `unitPrice`, `lineRevenue`.

Example:

```http
GET /api/sales?fromDate=2015-01-01&toDate=2015-01-31&query=chicken&category=Chicken&size=L&page=1&pageSize=25&sortBy=lineRevenue&sortDirection=desc
```

#### `GET /api/sales/filters`

No query parameters. Returns categories, sizes, `minDate`, and `maxDate` for filter UI defaults.

### Catalog & orders

| Method | Path | Description |
| --- | --- | --- |
| `GET` | `/api/Pizzas` | All pizzas |
| `GET` | `/api/Pizzas/{id}` | Pizza by string id |
| `GET` | `/api/PizzaTypes` | All pizza types |
| `GET` | `/api/PizzaTypes/{id}` | Pizza type by string id |
| `GET` | `/api/Orders` | All orders |
| `GET` | `/api/Orders/{id}` | Order by integer id (`404` if missing) |
| `GET` | `/api/OrderDetails/{id}` | Order detail by integer id (`404` if missing) |

Invalid sales query parameters return **400** with validation problem details.

## Architecture notes

- **Backend** follows Clean Architecture: controllers stay thin; application services own business rules; Infrastructure owns EF Core and CSV seeding.
- **Seeding runs once** when the catalog is empty — changing CSVs after a successful seed does not automatically re-import; recreate or clear the DB if you need a fresh load.
- **Front-end proxy** avoids hard-coding the API origin during `ng serve`; production builds typically need an environment-specific API base URL if the API is hosted separately.
- **CORS** is scoped to local Angular origins for development.

## Troubleshooting

| Issue | What to check |
| --- | --- |
| API fails on startup | SQL Server running; connection string instance name; permissions to create `PizzaPlaceDb` |
| Empty / missing data | Confirm CSVs exist under `PizzaPlace.Api/Data` and were copied to the output directory; try a fresh database if seeding already skipped |
| Dashboard shows network errors | API running on `5063`; front end started with `npm start` (proxy enabled) |
| Swagger missing | App must run with `ASPNETCORE_ENVIRONMENT=Development` |

## License / scope

Project code and sample sales data are intended for local development and demonstration of the Pizza Place sales dashboard and API.
