# Pizza Place

Full-stack pizza sales platform with an ASP.NET Core API and an Angular 22 sales dashboard.

## Repository layout

- `back-end/` — Clean Architecture .NET 10 Web API (`PizzaPlace.Api`)
- `front-end/` — Angular 22 sales dashboard (`pizza-place-web`)

## Prerequisites

- .NET 10 SDK
- SQL Server (`localhost\MSSQLSERVER2022` by default; see `back-end/PizzaPlace.Api/appsettings.json`)
- Node.js `^22.22.3` or `^24.15.0` (or newer supported Angular 22 engines)
- npm 8+

## Run the API

```powershell
cd back-end/PizzaPlace.Api
dotnet run
```

API URLs:

- HTTP: `http://localhost:5063`
- HTTPS: `https://localhost:7043`
- Swagger (Development): `http://localhost:5063/swagger`

On startup the API migrates the database and seeds CSV data from `PizzaPlace.Api/Data` when the catalog is empty.

## Sales endpoints

| Endpoint | Purpose |
| --- | --- |
| `GET /api/sales/dashboard?fromDate&toDate` | KPI cards, prior-period revenue change, peak weekday/hour, daily trend, category mix, top pizzas |
| `GET /api/sales?fromDate&toDate&query&category&size&page&pageSize&sortBy&sortDirection` | Paginated searchable sales line items |
| `GET /api/sales/filters` | Distinct categories/sizes and available date bounds |

Examples are in [`back-end/PizzaPlace.Api/PizzaPlace.Api.http`](back-end/PizzaPlace.Api/PizzaPlace.Api.http).

Business metrics produced for the dashboard:

- Total revenue, order count, pizzas sold
- Average order value and pizzas per order
- Revenue change vs the immediately preceding equal-length period
- Peak weekday and peak hour for staffing guidance
- Category revenue share and top five pizzas by revenue

Profit/margin is intentionally omitted because the source data has no cost fields.

## Run the Angular dashboard

```powershell
cd front-end
npm install
npm start
```

Open `http://localhost:4200`.

The Angular dev server proxies `/api` to `http://localhost:5063` via `front-end/proxy.conf.json`. CORS is also enabled on the API for `http://localhost:4200`.

## Tests

Backend:

```powershell
cd back-end
dotnet test
```

Frontend:

```powershell
cd front-end
npm test -- --watch=false
```

## Production build (frontend)

```powershell
cd front-end
npm run build
```
