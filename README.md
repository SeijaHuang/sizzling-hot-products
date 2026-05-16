# 🔥 Sizzling Hot Products

A full-stack application that calculates the top-selling ("sizzling hot") products for Bunnings, built with **.NET Web API** and **Next.js**.

---

## 🛠️ Tech Stack

| Layer    | Technology           | Reason                                                                                                                                                                                              |
| -------- | -------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Backend  | .NET 8 Web API       | Matches the role's technical requirements; strong typing and LINQ make aggregation logic clean and testable                                                                                         |
| Frontend | Next.js (TypeScript) | Aligns with the team's frontend stack. Given the scope of this challenge the app uses client-side rendering, but the architecture is ready to adopt SSR or SSG if data fetching requirements evolve |

---

## 📁 Project Structure

```
/
├── backend/
│   ├── SizzlingHotProducts.sln
│   ├── SizzlingHotProducts/          # .NET Web API
│   │   ├── Controllers/
│   │   ├── Services/
│   │   │   ├── Interfaces/
│   │   │   └── ProductService.cs
│   │   └── Models/                   # Domain models and response DTOs
│   └── SizzlingHotProducts.Tests/    # Unit tests
├── frontend/                         # Next.js application
│   ├── src/
│   │   ├── components/
│   │   │   └── ProductTable/
│   │   │       ├── ProductTable.tsx
│   │   │       └── ProductTable.test.tsx
│   │   ├── hooks/
│   │   ├── services/                 # API calls
│   │   └── types/
└── inputs/                           # Source JSON data files
```

---

## 🗂️ Repository Structure

This project uses a monorepo structure to simplify development and onboarding, allowing both frontend and backend to be managed and run from a single repository.

In a production environment, frontend and backend could be separated into independent repositories to enable independent deployment and team ownership.
Further architectural decisions would depend on scale and domain complexity.

---

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Node.js 18+](https://nodejs.org/)

### Run the Backend

```bash
cd backend/SizzlingHotProducts
dotnet restore
dotnet run
```

The API will be available at `http://localhost:8000`.

### Run the Frontend

```bash
cd frontend
npm install
npm run dev
```

The app will be available at `http://localhost:3000`.

### Environment Variables

The backend requires the following environment variable:

```bash
# .env or shell export
APP_TODAY_DATE=2026-04-23
```

---

## 📡 API Endpoints

| Method | Endpoint                                                       | Description                                             |
| ------ | -------------------------------------------------------------- | ------------------------------------------------------- |
| GET    | `/api/v1/products/hot?date=2026-04-21`                         | Returns the top sizzling hot product for a specific day |
| GET    | `/api/v1/products/hot?startDate=2026-04-21&endDate=2026-04-23` | Returns the top sizzling hot product over a date range  |

**Query Parameters**

| Parameter   | Type                    | Description                                  |
| ----------- | ----------------------- | -------------------------------------------- |
| `date`      | `DateOnly (yyyy-MM-dd)` | Returns the top product for a single day     |
| `startDate` | `DateOnly (yyyy-MM-dd)` | Start of the date range (use with `endDate`) |
| `endDate`   | `DateOnly (yyyy-MM-dd)` | End of the date range (use with `startDate`) |

- If no parameters are provided, the endpoint defaults to today's date.
- The date parameter cannot be used together with startDate / endDate, and both startDate and endDate must be provided together with startDate earlier than or equal to endDate.

---

## 💡 Assumptions

1. **Today's date** is fixed at `23/04/2026` as specified, configured via an environment variable (`APP_TODAY_DATE`) so it can be adjusted without code changes.

2. **Cancelled orders** are credited against the product total for the **original order's date** (matched by `orderId`), not the cancellation date. This follows the example in the brief where a cancellation on day 2 removes the sale from day 1.

3. **"Past 3 days"** for the period calculation refers to 21/04/2026 – 23/04/2026 inclusive.

4. **Only `completed` and `cancelled` statuses** are present in the data. Unknown order statuses are ignored to ensure data integrity and avoid introducing undefined business behaviour.

5. **Alphabetical tiebreaking** uses case-insensitive sorting on the product name.

---

## ✅ Running Tests

**Backend**

```bash
cd backend
dotnet test
```

**Frontend**

```bash
cd frontend
npm test
```

---

## ⚖️ Trade-offs

- The application loads data from static JSON files instead of a database for simplicity.
- Aggregation is performed in-memory, which is sufficient for the dataset size but would need optimisation for large-scale data.
- Frontend uses client-side rendering to reduce complexity for the challenge scope.

---

## 🔄 CI Pipeline

A GitHub Actions workflow runs automatically on every pull request to `main`, enforcing quality gates before any code is merged:

| Check                 | Tool                                | Scope    |
| --------------------- | ----------------------------------- | -------- |
| Unit tests            | `dotnet test`                       | Backend  |
| Code formatting       | `dotnet format --verify-no-changes` | Backend  |
| Code style & patterns | `ESLint`                            | Frontend |
| Type checking         | `tsc --noEmit`                      | Frontend |

All checks must pass before a PR can be merged. This ensures a consistent codebase and catches regressions early without relying on manual review alone.

---

## 🎓 Expected Outcomes

| Date or Period          | Top Sizzling Hot Product                           |
| ----------------------- | -------------------------------------------------- |
| 21/04/2026              | Ezy Storage 37L Flexi Laundry Basket - White       |
| 22/04/2026              | Ezy Storage 37L Flexi Laundry Basket - White       |
| 23/04/2026              | Arlec 160W Crystalline Solar Foldable Charging Kit |
| 21/04/2026 – 23/04/2026 | Ezy Storage 37L Flexi Laundry Basket - White       |
