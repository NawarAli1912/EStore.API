# EStore.API

## Overview

This project is a .NET 10 Web API built following the principles of Clean Architecture.
It leverages various technologies and design patterns from DDD to provide a robust and scalable solution.
Offering a wide range of endpoints to effectively manage products, orders, categories, carts and other essential e-commerce functionalities.
It features a seamless integration with Elasticsearch, ensuring fast and accurate search capabilities for an enhanced product discovery experience.
This API is designed to streamline the operations of an online store, catering to both the business's and the customers' needs with efficiency and ease.

## Features

- **Clean Architecture:** At the core of EStore.API is Clean Architecture, ensuring our codebase remains both modular and maintainable. This approach segregates the system into independent layers, allowing for more straightforward updates and better scalability.
- **MediatR:** Utilizing the MediatR library, our API achieves loose coupling and high cohesion between components. This means each part of the system communicates through a mediator for clearer and more maintainable code.
- **Entity Framework Core and Dapper:** This dual strategy harnesses the strengths of EF Core for straightforward ORM capabilities and Dapper for its nimbleness in executing complex, performance-critical queries. This combination balances ease of development with high performance.
- **NEST and Elasticsearch:** Utilizing NEST, the project connects seamlessly with Elasticsearch, providing scalable and efficient search functionality. This integration ensures that product searches are not only fast but also accurate, enhancing the overall user experience.
- **Mapster:** Mapster streamlines object-to-object mapping, reducing boilerplate code and errors, and increasing the efficiency of data transformations across the application.
- **Quartz:** The implementation of Quartz adds sophisticated job scheduling capabilities, allowing for reliable execution of background tasks and timed operations.
- **Domain-Driven Design (DDD) Principles:** Incorporating DDD enhances the project's structure and clarity, centering around a domain model that reflects the business's complexities and rules, thus fostering a more maintainable and evolvable codebase.
- **Robust Authentication and Authorization:** Implementing JWT Bearer authentication safeguards API access, while custom permission-based controls ensure that users can only access the features and data appropriate to their roles.
- **Outbox Pattern for Event Consistency:** The Outbox pattern is employed to ensure reliable event processing, maintaining consistency between domain events and persistent storage.
- **Global Error Handling:** Adheres to RFC specifications for global error handling, with a reimplementation of the dotnet ProblemDetailsFactory.
- **Result Pattern for Error Handling:** A generic result type is used for explicit and efficient error handling, reducing the reliance on exceptions and leading to more robust and readable code.
- **Cache-Aside Pattern for Optimized Queries:** The application leverages the Cache-Aside pattern within MediatR pipelines to enhance performance, particularly for complex queries where speed and efficiency are paramount.
- **Unit Testing:** Unit testing using x-unit framework.
- **Architecture Compliance Testing:** NetArchTest.Rules are used to validate the compliance of the codebase with Clean Architecture principles, ensuring architectural integrity.
- **AWS S3 for Robust Image Storage:** AWS S3 is integrated for storing product and customer images, providing a secure, scalable, and efficient solution for managing media assets.
- **Idempotency Support:** The system features a solid implementation of idempotency for select endpoints. This guarantees consistent outcomes for repeated requests, promoting reliability in key operations.
- **Fluent Validation Integration:** Incorporates Fluent Validation for robust and streamlined validation processes.

## Getting Started

### Prerequisites

- [Docker](https://www.docker.com/) (with Docker Compose v2)

PostgreSQL and Elasticsearch run in containers, so nothing else needs to be installed locally. The [.NET 10 SDK](https://dotnet.microsoft.com/download) is only required to build or run the API outside Docker.

### Run with Docker Compose

```bash
docker compose up -d --build
```

This builds the API image and starts the API (port 8080), PostgreSQL (5432), and Elasticsearch (9200). On startup the API applies EF Core migrations and seeds an admin user.

- Swagger UI: http://localhost:8080/swagger
- Seeded admin: `admin@estore.com` / `estoreadmin` — POST `/api/login` to obtain a JWT.

Stop the stack (data persists in named volumes):

```bash
docker compose down
```

### Usage

- Explore the API endpoints via Swagger to understand available functionality.

### Entity Framework Core and Dapper: 
This combination offers a balanced approach to data access.
EF Core provides a developer-friendly ORM for standard operations, while Dapper enhances performance for complex queries.
Highlighting this could underscore your commitment to both developer efficiency and application performance.


### Idempotency Support:
Reliable Idempotency Implementation: 
The system's architecture includes a well-designed implementation of idempotency for select endpoints.
This is crucial for ensuring consistent outcomes in the face of repeated requests.
Such a feature is particularly important in distributed systems where network reliability can be an issue,
thereby enhancing the overall robustness and reliability of the application.

### Result Pattern for Explicit Error Handling:
The project adopts the result pattern to handle errors explicitly.
This approach avoids the use of exceptions for control flow,
leading to cleaner and more maintainable code.
By employing a generic result type, the system provides clear and consistent feedback on operations,
significantly improving error handling and enhancing the overall user experience.

### Background Service and Quartz

The project uses a background service with Quartz to handle domain events in the Outbox,
ensuring asynchronous and reliable processing.

### NEST for Enhanced Search Capabilities:
The project leverages NEST in conjunction with Elasticsearch to provide advanced search functionalities.
This integration facilitates quick, scalable, and precise searching capabilities within the product database,
offering users a seamless and effective search experience.

## Contributing

We welcome contributions! Feel free to submit issues, feature requests, or pull requests.

## License

This project is licensed under the [MIT License](LICENSE).

## Codebase Maintainability Index

| Hierarchy                    | Maintainability Index  |
|------------------------------|------------------------|
| src\Application (Debug)      | 87%                    |
| src\Contracts (Debug)        | 99%                    |
| src\Domain (Debug)           | 89%                    |
| src\Infrastructure (Debug)   | 73%                    |
| src\Presentation (Debug)     | 73%                    |
| src\SharedKernel (Debug)     | 95%                    |
