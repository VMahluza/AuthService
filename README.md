# AuthService

A comprehensive, Domain-Driven Design (DDD) based Authentication and Identity Management Service. This project demonstrates a clean architecture approach to building secure, scalable, and maintainable authentication systems using modern technologies.

## 🚀 Features

*   **User Management**: Secure user registration, login, and profile management.
*   **Authentication**: Robust JWT-based authentication with access and refresh tokens.
*   **Authorization**: Granular Role-Based Access Control (RBAC) with Groups and Permissions.
*   **Security**:
    *   Secure password hashing.
    *   Session management with revocation capabilities.
    *   Audit logging for security-critical events.
*   **Architecture**:
    *   **Domain-Driven Design (DDD)**: Rich domain model encapsulating business logic.
    *   **CQRS**: Command Query Responsibility Segregation for optimized read/write operations.
    *   **Clean Architecture**: Strict separation of concerns (Domain, Application, Infrastructure, API).
*   **Frontend**: Modern, responsive UI built with Next.js and Material UI.

## 🛠️ Technology Stack

### Backend
*   **.NET 10**: The latest version of the .NET platform.
*   **ASP.NET Core Web API**: For building high-performance RESTful APIs.
*   **Entity Framework Core**: ORM for database interactions.
*   **MySQL**: Relational database for data persistence.
*   **MediatR**: For implementing the Mediator pattern and CQRS.
*   **FluentValidation**: For strong validation rules.

### Frontend
*   **Next.js**: React framework for production-grade applications.
*   **React**: Library for building user interfaces.
*   **Material UI (MUI)**: Comprehensive UI component library.
*   **TypeScript**: For type-safe JavaScript development.

### Infrastructure
*   **Docker**: Containerization for consistent development and deployment environments.
*   **Docker Compose**: Orchestration of multi-container applications.

## 📂 Project Structure

The solution follows a strict Clean Architecture structure:

*   **`AuthService.Domain`**: The core of the application. Contains Entities, Value Objects, Enums, and Repository Interfaces. No external dependencies.
*   **`AuthService.Application`**: Contains business logic, CQRS Commands & Queries, Validators, and DTOs. Depends only on the Domain.
*   **`AuthService.Infrastructure`**: Implementation of interfaces defined in Domain/Application. Handles Database (EF Core), External Services, etc.
*   **`AuthService.API`**: The entry point. REST API controllers, Middleware, and Dependency Injection setup.
*   **`AuthService.UI`**: The frontend application (Next.js).
*   **`AuthService.Tests`**: Unit and Integration tests.

## 🏁 Getting Started

### Prerequisites
*   [Docker Desktop](https://www.docker.com/products/docker-desktop)
*   [.NET SDK](https://dotnet.microsoft.com/download) (Latest version)
*   [Node.js](https://nodejs.org/) (LTS version)

### Running with Docker (Recommended for Database)

1.  Start the database container:
    ```bash
    docker-compose up -d
    ```
    This will spin up a MySQL instance with the configuration defined in `docker-compose.yml`.

### Running the Backend

1.  Navigate to the API directory:
    ```bash
    cd AuthService.API
    ```
2.  Update the connection string in `appsettings.json` if necessary (default is configured for local Docker).
3.  Run the application:
    ```bash
    dotnet run
    ```
    The API will be available at `https://localhost:7049` (or similar, check launch logs).

### Running the Frontend

1.  Navigate to the UI directory:
    ```bash
    cd AuthService.UI/ui
    ```
2.  Install dependencies:
    ```bash
    npm install
    # or
    pnpm install
    ```
3.  Start the development server:
    ```bash
    npm run dev
    ```
    The UI will be available at `http://localhost:3000`.

## 📚 Documentation

*   **API Contracts**: See `API_CONTRACTS_COMPLETE.md` for detailed API specifications.
*   **Architecture**: See `documentation/AuthenticationService.drawio` for architectural diagrams.
*   **Security**: See `JWT_SECURITY_IMPLEMENTATION_COMPLETE.md` for security details.

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

1.  Fork the repository.
2.  Create your feature branch (`git checkout -b feature/AmazingFeature`).
3.  Commit your changes (`git commit -m 'Add some AmazingFeature'`).
4.  Push to the branch (`git push origin feature/AmazingFeature`).
5.  Open a Pull Request.

## 📄 License

This project is licensed under the MIT License.
