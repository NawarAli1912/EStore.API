# EStore.API

## Overview

This project is a .NET 8 Web API built following the principles of Clean Architecture. It leverages various technologies and design patterns from DDD to provide a robust and scalable solution.

## Features

- **Clean Architecture:** ensures a modular and maintainable codebase by following Clean Architecture principles.
- **MediatR:** Utilizes the MediatR library for implementing the mediator pattern, promoting loose coupling between components.
- **Entity Framework Core and Dapper:** Uses both EF Core and Dapper for data access, providing flexibility and performance optimization where needed.
- **NEST and Elasticsearch:** Implements Elasticsearch using NEST for efficient and scalable product searching.
- **Mapster:** Utilizes Mapster for object-to-object mapping, simplifying data transformations.
- **Quartz:** Implements Quartz for job scheduling, enhancing the project with timely tasks.
- **Domain-Driven Design (DDD) Principles:** Incorporates DDD principles, including domain models, domain events, and domain services to create a robust and maintainable codebase.
- **Authentication:** Implements JWT Bearer authentication for secure access to the API.
- **Authorization:** Secures API access with JWT Bearer authentication and custom permission-based control.
- **Outbox Pattern:** Ensures reliable event processing by implementing the Outbox pattern for storing domain events.
- **Global Error Handling:** Adheres to RFC specifications for global error handling, with a reimplementation of the dotnet ProblemDetailsFactory.
- **Reuslt Pattern:** Utilizes a generic result type for explicit error handling, avoiding exceptions.
- **Cache:** Implement Cache-Aside Pattern to optimize complex queries, using MediatR piplines feature.
- **Unit Testing:** Unit testing using x-unit framework.
- **Architecture Tests:** Utilizes NetArchTest.Rules to ensure compliance with Clean Architecture principles.
- **AWS S3 for Image Storage**: s Amazon Web Services (AWS) S3 for secure and scalable storage of product and customers images. This integration enhances the overall media asset management, providing a reliable solution for handling product images.

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Elasticsearch](https://www.elastic.co/guide/en/elasticsearch/reference/current/install-elasticsearch.html)

### Installation

1. **Clone the repository.**
2. **Configure the database connection in `appsettings.json`.**
3. **Run database migrations using `dotnet ef database update`.**
4. **Start the Elasticsearch server.**
5. **Configure AWS S3 settings for product images.**
   - **AWS S3 Configuration:**
     - Make sure you have an AWS account and access to the S3 service.
     - Configure your AWS credentials on your machine.
     - Since this project is currently set up to use AWS services with the root user, ensure that you are using the appropriate credentials on your local machine.
6. **Start the application using `dotnet run`.**

### Usage

- Ensure Elasticsearch is running.
- Explore the API Endpoints to understand available endpoints and their functionalities.

### Syncing Database and Elasticsearch

The project implements domain events and the Outbox pattern to synchronize SQL database changes with the Elasticsearch index for products. This ensures real-time and efficient searching capabilities.

### Background Service and Quartz

The project uses a background service with Quartz to handle domain events in the Outbox, ensuring asynchronous and reliable processing.

### EStore API

This API serves as an eStore, providing endpoints to manage products, orders, categories and more. It seamlessly integrates Elasticsearch for efficient product searching.

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

## Notes

This project is still under development.

