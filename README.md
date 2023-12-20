# EStore.API

## Overview

This project is a .NET 8 Web API built following the principles of Clean Architecture. It leverages various technologies and design patterns from DDD to provide a robust and scalable solution.

## Features

- **Clean Architecture:** The project is structured following the Clean Architecture principles, ensuring a separation of concerns and maintainability.
- **MediatR:** Utilizes the MediatR library for implementing the mediator pattern, promoting loose coupling between components.
- **Entity Framework Core and Dapper:** Uses both EF Core and Dapper for data access, providing flexibility and performance optimization where needed.
- **NEST and Elasticsearch:** Implements Elasticsearch using NEST for efficient and scalable product searching.
- **Mapster:** Utilizes Mapster for object-to-object mapping, simplifying data transformations.
- **Quartz:** Implements Quartz for job scheduling, enhancing the project with timely tasks.
- **Domain-Driven Design (DDD) Principles:** Incorporates DDD principles, including domain models, domain events, and domain services to create a robust and maintainable codebase.
- **Authentication:** Implements JWT Bearer authentication for secure access to the API.
- **Authorization:** Implements permission-based access control from scratch, ensuring that users have the necessary permissions to perform specific actions.
- **Outbox Pattern:** Implements the Outbox pattern to store domain events, ensuring reliable event processing.
- **Global Error Handling:** New way for global error handling following RFC specification and reimplement dotnet ProblemDetailsFactory.
- **Reuslt Pattern:** Generic result type which work with domain errors to avoid throwing exceptions and return explicit errors.
- **Cache:** Implement Cache-Aside Pattern to optimize complex queries, using MediatR piplines feature.
- **Unit Testing:** Unit testing using x-unit framework.

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Elasticsearch](https://www.elastic.co/guide/en/elasticsearch/reference/current/install-elasticsearch.html)

### Installation

1. Clone the repository.
2. Configure the database connection in `appsettings.json`.
3. Run the database migrations using `dotnet ef database update`.
4. Start the Elasticsearch server.
5. Start the application using `dotnet run`.

### Usage

- Ensure Elasticsearch is running.
- Explore the API documentation to understand available endpoints and their functionalities.

### Syncing Database and Elasticsearch

The project implements domain events and the Outbox pattern to synchronize SQL database changes with the Elasticsearch index for products. This ensures real-time and efficient searching capabilities.

### Background Service and Quartz

The project uses a background service with Quartz to handle domain events in the Outbox, ensuring asynchronous and reliable processing.

### eStore API

This API serves as an eStore, providing endpoints to manage products, orders, categories and more. It seamlessly integrates Elasticsearch for efficient product searching.

## Contributing

We welcome contributions! Feel free to submit issues, feature requests, or pull requests.

## License

This project is licensed under the [MIT License](LICENSE).

## Notes

This project is still under development.

