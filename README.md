# 🔥 Sizzling Hot Products

A full-stack application that calculates the top-selling ("sizzling hot") products for Bunnings, built with **.NET Web API** and **Next.js**.

---

## 🛠️ Tech Stack

| Layer    | Technology                                | Reason                                                                                                                                                                                              |
| -------- | ----------------------------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Backend  | .NET 8 Web API                            | Matches the role's technical requirements; strong typing and LINQ make aggregation logic clean and testable                                                                                         |
| Frontend | Next.js 16 + React 19 (TypeScript)        | Aligns with the team's frontend stack. Given the scope of this challenge the app uses client-side rendering, but the architecture is ready to adopt SSR or SSG if data fetching requirements evolve |
| Styling  | Tailwind CSS 4 + shadcn/ui + Radix UI     | Utility-first styling with accessible, headless primitives — fast to iterate, consistent design tokens                                                                                              |
| API      | TanStack React Query 5 + Axios            | Declarative server-state management with built-in caching, loading, and error states                                                                                                                |
| Testing  | Vitest + React Testing Library (Frontend) | Fast Vite-native test runner that aligns with the Next.js build toolchain                                                                                                                           |

---

## 📁 Project Structure

```
/
├── backend/
│   ├── Backend.sln
│   ├── Web.Host/                      # .NET Web API
│   │   ├── Constants/                 # Date format constants
│   │   ├── Controllers/               # ProductController
│   │   ├── Data/                      # orders.json, products.json
│   │   ├── Filters/                   # GlobalExceptionFilter
│   │   ├── Helpers/                   # DateTimeConverter
│   │   ├── Interfaces/                # IProductService, IFileReaderService
│   │   ├── Models/
│   │   │   ├── Domain/                # Order, OrderEntry, Product
│   │   │   └── Responses/             # ClientResponse, GetTopProductsResponse
│   │   ├── Services/                  # ProductService, FileReaderService
│   │   └── Properties/                # Launch settings
│   └── Web.Host.Test/                 # xUnit test project
├── frontend/
│   ├── app/                           # Next.js App Router
│   │   ├── page.tsx                   # Home page
│   │   ├── layout.tsx                 # Root layout with metadata & fonts
│   │   └── globals.css                # Global styles & Tailwind imports
│   ├── components/
│   │   ├── TopProductsBoard/          # Main feature component
│   │   │   ├── TopProductsBoard.tsx   # Board with date picker & results table
│   │   │   └── components/
│   │   │       ├── DatePicker.tsx     # Date range picker (react-day-picker)
│   │   │       └── FlameIcon.tsx      # Flame badge icon
│   │   └── ui/                        # shadcn/ui base components
│   ├── lib/
│   │   ├── providers.tsx              # React Query provider
│   │   ├── request.ts                 # Axios instance with interceptors
│   │   └── utils.ts                   # cn() className helper
│   ├── services/
│   │   └── get-hot-products.ts        # API fetch function
│   ├── types/                         # TypeScript type definitions
│   ├── utils/
│   │   └── format-date.ts             # Date formatting helpers
│   └── tests/
└── README.md
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

### 1. Configure the Frontend Environment

```bash
cd frontend
cp .env.example .env.development
```

`.env.development` contains one variable:

| Variable                            | Default                     | Description                 |
| ----------------------------------- | --------------------------- | --------------------------- |
| `NEXT_PUBLIC_WEB_HOST_API_BASE_URL` | `http://localhost:8000/api` | Base URL of the backend API |

Change the value if your backend runs on a different port or host.

### 2. Backend User Secrets

The backend requires the following user secret to configure CORS:

```bash
cd backend/Web.Host
dotnet user-secrets set "CORSAllowedOrigins" "http://localhost:3000"
```

### 3. Run the Backend

```bash
cd backend/Web.Host
dotnet restore
dotnet run
```

The API will be available at `http://localhost:8000`.
Swagger UI is available at http://localhost:8000/swagger.

### 4. Run the Frontend

```bash
cd frontend
npm install
npm run dev
```

The app will be available at `http://localhost:3000`.

---

## 📡 API Endpoints

| Method | Endpoint                                                | Description                                            |
| ------ | ------------------------------------------------------- | ------------------------------------------------------ |
| GET    | `/api/products?startDate=2026-04-21`                    | Returns top product from startDate to today (default)  |
| GET    | `/api/products?startDate=2026-04-21&endDate=2026-04-23` | Returns daily and period top products for a date range |
| GET    | `/api/products?startDate=2026-04-21&endDate=2026-04-21` | Returns daily top product                              |

**Query Parameters**

| Parameter   | Type                     | Description                                   |
| ----------- | ------------------------ | --------------------------------------------- |
| `startDate` | `DateTime  (yyyy-MM-dd)` | Start of the date range                       |
| `endDate`   | `DateTime  (yyyy-MM-dd)` | End of the date range; defaults to 2026-04-23 |

- startDate must be earlier than or equal to endDate.
- To query a single day, provide the same value for both startDate and endDat.

**Response Shape**

```json
{
  "success": true,
  "body": {
    "daily": [
      { "date": "2026-04-21", "product": { "id": "P1", "name": "..." } }
    ],
    "period": {
      "startDate": "2026-04-21",
      "endDate": "2026-04-23",
      "product": { "id": "P1", "name": "..." }
    }
  }
}
```

---

## 💡 Solution Approach & Design Decisions

### Core Algorithm

The solution centres on efficiently computing the top-selling product per day (or date range) while applying the required deduplication and cancellation rules.

### Deduplication via HashSet

The business rules require that:

- A product sale is counted only once per order
- Multiple orders of the same product by the same customer on the same day are excluded from the total

To enforce this, a `HashSet` is used with a composite key of `(date, customerId, productId)`. When processing each order entry, if the key already exists in the set, the entry is skipped — ensuring each customer-product combination is counted at most once per day, regardless of how many orders they placed.

### Core Data Structure

The fundamental aggregation structure is:

```csharp
Dictionary<DateOnly, Dictionary<string, int>> salesByDateAndProduct
// date → (productId → validSaleCount)
```

This captures the effective sale count per product per day. For a single-day query, the result is read directly from the relevant date key. For a date range query, counts are accumulated across all dates in the range before finding the maximum — reflecting the requirement that sales compound over a period.

### Handling Cancellations

Cancelled orders do not carry their own line items; the original order must be located to identify which products need to be decremented. A linear search for each cancellation would produce **O(n²)** complexity.

To avoid this, completed orders are pre-indexed into a lookup dictionary keyed by `orderId`:

```csharp
Dictionary<string, Order> completedOrdersById
```

This reduces each cancellation lookup to **O(1)**. The cancellation is applied against the **original order's date**, consistent with the business rule that a cancellation reverses the sale on the day it was originally made. A decrement only occurs if the original entry was actually counted — i.e., if its composite key exists in the `HashSet`. Additionally, the key is removed from the `HashSet` to correctly support the case where a customer cancels and then re-purchases the same product on the same day.

### Product Name Resolution

Products are pre-indexed by `productId`:

```csharp
Dictionary<string, Product> productsById
```

This allows product name retrieval in **O(1)** during result mapping, avoiding repeated list scans.

### Tiebreaking

When multiple products share the highest sale count on a given day or period, the result is determined by **case-insensitive alphabetical ordering** of the product name, with the first result selected.

### Complexity Summary

| Phase                         | Implementation         | Complexity |
| ----------------------------- | ---------------------- | ---------- |
| Pre-indexing completed orders | `ToDictionary`         | O(n)       |
| Pre-indexing products         | `ToDictionary`         | O(p)       |
| Main order loop               | Single traversal       | O(n)       |
| Per-entry deduplication       | `HashSet.Contains/Add` | O(1)       |
| Cancellation lookup           | `GetValueOrDefault`    | O(1)       |
| Top product resolution        | Per-day sort           | O(p log p) |

## **Overall: O(n + m)** — linear in the size of the input data.

## Error Handling

A `GlobalExceptionFilter` intercepts unhandled exceptions and maps them to standardised `ClientResponse` error envelopes:

| Exception                   | HTTP Status |
| --------------------------- | ----------- |
| `FileNotFoundException`     | 404         |
| `InvalidOperationException` | 400         |
| All others                  | 500         |

### Date Parsing

A custom `DateTimeConverter` handles the input data's `dd/MM/yyyy` date format during JSON deserialization, while serializing responses as `yyyy-MM-dd`.

## 💡 Assumptions

- Today's date is fixed at 23/04/2026 as specified; if no `endDate` is provided, `2026-04-23` is used as the default.
- Cancelled orders are credited against the product total for the original order's date (matched by `orderId`), not the cancellation date. This follows the example in the brief where a cancellation on day 2 removes the sale from day 1.
- "Past 3 days" for the period calculation refers to 21/04/2026 – 23/04/2026 inclusive.
- Only `completed` and `cancelled` statuses are present in the data. Unknown order statuses are ignored to ensure data integrity and avoid introducing undefined business behaviour.
- Alphabetical tiebreaking uses case-insensitive sorting on the product name

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
