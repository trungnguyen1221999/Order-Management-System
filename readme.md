# Order Management System

A backend system for a simple e-commerce platform, covering product management, customer management, order processing, payment, and notifications — built with **.NET** and **Clean Architecture**.

## Table of Contents

- [Overview](#overview)
- [Domain Model](#domain-model)
- [Value Objects](#value-objects)
- [Business Rules](#business-rules)
- [Order Status State Machine](#order-status-state-machine)
- [Domain Events](#domain-events)
- [Solution Structure](#solution-structure)
- [Project Dependency Rules](#project-dependency-rules)
- [NuGet Packages](#nuget-packages)
- [Request Flow](#request-flow)
- [Roadmap](#roadmap)

## Overview

Core features:

- **Product** — name, price, stock
- **Customer** — personal info, shipping addresses
- **Order** — create, confirm, ship, cancel
- **Payment** — payment gateway integration
- **Notification** — order confirmation and status emails

## Domain Model

### Order (Aggregate Root)

The central entity of the system.

| Field | Description |
|---|---|
| `OrderId` | Unique identifier |
| `CustomerId` | Who placed the order |
| `OrderItems` | List of items in the order |
| `ShippingAddress` | Value Object |
| `Status` | Order status (see [state machine](#order-status-state-machine)) |
| `TotalAmount` | Money Value Object |
| `PlacedAt` | Timestamp |

### OrderItem (Child Entity)

Exists only within an `Order`.

| Field | Description |
|---|---|
| `ProductId` | Referenced product |
| `ProductName` | Snapshot at order time |
| `UnitPrice` | Snapshot at order time |
| `Quantity` | Quantity ordered |
| `Subtotal` | `UnitPrice × Quantity` |

> Snapshotting `ProductName` and `UnitPrice` prevents historical orders from changing when the product's price is later updated.

### Product

`ProductId`, `Name`, `Description`, `Price`, `StockQuantity`, `IsActive`

### Customer

`CustomerId`, `FullName`, `Email`, `Addresses`

## Value Objects

| Value Object | Properties | Validation |
|---|---|---|
| `Money` | `Amount` (decimal), `Currency` (string) | `Amount >= 0`; `Currency` is a non-empty 3-letter code |
| `Address` | `Street`, `City`, `Province`, `PostalCode`, `Country` | No empty fields; valid postal code format |
| `Email` | `Value` (string) | Valid email format, non-empty, ≤ 256 chars |

## Business Rules

Business rules live in the **Domain layer**, not in controllers or application services.

**Order**
- Must contain at least one `OrderItem`
- Can only be cancelled while `Pending` or `Confirmed`
- Items cannot be added/removed once `Shipped` or `Delivered`
- `TotalAmount` is recalculated as the sum of all item subtotals
- Can only move to `Shipped` after being `Confirmed`
- Cannot order a product with `StockQuantity = 0`

**OrderItem**
- `Quantity >= 1`
- No duplicate `ProductId` within the same order
- Snapshot `UnitPrice` must be `> 0`

**Customer**
- `Email` must be unique across the system
- A locked account cannot place orders

## Order Status State Machine

Status transitions are strictly controlled — no arbitrary changes.

| From | To | Condition | Trigger |
|---|---|---|---|
| *(new)* | Pending | At least 1 item, product in stock | `PlaceOrder` |
| Pending | Confirmed | Payment successful | `ConfirmOrder` |
| Pending | Cancelled | — | `CancelOrder` |
| Confirmed | Shipped | Packed by warehouse | `ShipOrder` |
| Confirmed | Cancelled | Before handoff to shipper | `CancelOrder` |
| Shipped | Delivered | Shipper confirms delivery | `DeliverOrder` |
| Shipped | Cancelled | ❌ Not allowed | — |
| Delivered | Cancelled | ❌ Not allowed | — |

> Implemented via a private setter and factory methods on the `Order` entity — no public setter for `Status`.

## Domain Events

| Event | Triggered When | Handler Responsibility |
|---|---|---|
| `OrderPlaced` | Order successfully created | Send confirmation email, deduct stock |
| `OrderConfirmed` | Payment confirmed | Send email, notify warehouse |
| `OrderShipped` | Handed off to shipper | Send tracking email |
| `OrderDelivered` | Shipper confirms delivery | Update reports, send feedback request |
| `OrderCancelled` | Order cancelled | Restock, refund if paid |

## Solution Structure

```
OrderManagement.sln
├── src/
│   ├── OrderManagement.Domain/          # Domain Layer
│   │   ├── Entities/                    # Order, Customer, Product
│   │   ├── ValueObjects/                # Money, Address, Email
│   │   ├── Events/                      # OrderPlaced, OrderShipped...
│   │   ├── Repositories/                # IOrderRepository (interface)
│   │   └── Services/                    # OrderPricingService
│   │
│   ├── OrderManagement.Application/     # Application Layer
│   │   ├── Orders/
│   │   │   ├── Commands/                # PlaceOrderCommand + Handler
│   │   │   └── Queries/                 # GetOrderQuery + Handler
│   │   ├── Common/
│   │   │   ├── Behaviors/               # Logging, Validation pipeline
│   │   │   └── Interfaces/              # IEmailService, IPaymentGateway
│   │   └── DTOs/                        # OrderDto, OrderSummaryDto
│   │
│   ├── OrderManagement.Infrastructure/  # Infrastructure Layer
│   │   ├── Persistence/
│   │   │   ├── Configurations/          # EF Core configuration
│   │   │   └── Repositories/            # OrderRepository (EF Core)
│   │   ├── Services/                    # EmailService, PaymentService
│   │   └── DependencyInjection.cs
│   │
│   └── OrderManagement.WebApi/          # Presentation Layer
│       ├── Controllers/
│       ├── Middleware/
│       └── Program.cs
│
└── tests/
    ├── OrderManagement.Domain.Tests/
    ├── OrderManagement.Application.Tests/
    └── OrderManagement.Integration.Tests/
```

## Project Dependency Rules

The Clean Architecture Dependency Rule is enforced through project references — a dependency in the wrong direction is an architecture violation.

| Project | References | Must Not Reference |
|---|---|---|
| Domain | *(none)* | Application, Infrastructure, WebApi |
| Application | Domain | Infrastructure, WebApi |
| Infrastructure | Application, Domain | — |
| WebApi | Application, Infrastructure | — |

> **Infrastructure does not reference Application.** Infrastructure *implements* interfaces defined in Application — this is Dependency Inversion, and the most commonly confused point for newcomers.

## NuGet Packages

| Package | Layer | Purpose |
|---|---|---|
| MediatR | Application | CQRS — command/query dispatch |
| FluentValidation | Application | Command/query validation |
| Mapster | Application | Domain → DTO mapping |
| Microsoft.EntityFrameworkCore | Infrastructure | ORM |
| Microsoft.EntityFrameworkCore.SqlServer | Infrastructure | SQL Server driver |
| Dapper | Infrastructure | Raw SQL for read side |
| Polly | Infrastructure | Retry / circuit breaker |
| StackExchange.Redis | Infrastructure | Redis cache |
| Serilog.AspNetCore | WebApi | Structured logging |
| Swashbuckle.AspNetCore | WebApi | Swagger UI |

## Request Flow

### Command Flow — Place Order (`POST /api/orders`)

1. Request hits `OrdersController`
2. Controller deserializes into `PlaceOrderCommand`
3. Controller calls `mediator.Send(command)`
4. `ValidationBehavior` validates input
5. `LoggingBehavior` logs the start of processing
6. `TransactionBehavior` opens a transaction
7. `PlaceOrderCommandHandler` runs:
   - Loads `Customer` and `Product`s from repositories
   - Calls `Order.Create()` factory method
   - `Order` builds `OrderItem`s and validates invariants
   - `Order` raises the `OrderPlaced` domain event
   - Saves the `Order` via repository
8. `TransactionBehavior` commits the transaction
9. Domain event handlers run (send email, deduct stock)
10. Handler returns `Result<OrderId>`
11. Controller maps the result to `201 Created`

### Query Flow — List Orders

1. Request hits `OrdersController`
2. Controller builds `GetOrdersQuery`
3. MediatR dispatches to `GetOrdersQueryHandler`
4. Handler reads directly from the database via Dapper or an EF Core projection
5. Maps results to `List<OrderSummaryDto>`
6. Controller returns `200 OK`

> Queries bypass domain entities entirely — reading straight into DTOs avoids loading a full entity just to display a few fields.

## Roadmap

| Milestone | Deliverables |
|---|---|
| 2.1 Entity & Value Object | `Order`, `OrderItem`, `Money`, `Address` |
| 2.2 Aggregate Root | `Order` aggregate with invariants |
| 2.3 Domain Event | `OrderPlaced`, `OrderShipped` events |
| 2.4 Repository Interface | `IOrderRepository`, `IUnitOfWork` |
| 2.5 Domain Service | `OrderPricingService`, Specification pattern |
| 3.1 CQRS | `PlaceOrderCommand`, `GetOrderQuery` |
| 3.2 Command Handler | `PlaceOrderCommandHandler`, Result pattern |
| 3.3 Query Handler | `GetOrdersQueryHandler`, DTOs |
| 3.4 Validation | `PlaceOrderCommandValidator` |
| 3.5 Pipeline Behavior | Logging, Validation, Transaction behaviors |
| 4.1 EF Core | `ApplicationDbContext`, Fluent configuration |
| 4.2 Repository | `OrderRepository`, `UnitOfWork` implementation |
| 4.3 External Service | `EmailService` (SendGrid), `PaymentService` |
| 5.1 Controller | `OrdersController` — thin, no business logic |
| 5.2 Error Handling | Problem Details, exception middleware |
| 6.x Testing | Unit, integration, and architecture tests |