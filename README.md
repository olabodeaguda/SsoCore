# SSOCore

SSOCore is a robust, scalable Single Sign-On (SSO) system built with .NET Core, designed to provide secure authentication and authorization services for web applications. It follows Clean Architecture principles, separating concerns into Application, Domain, and Infrastructure layers, making it modular and maintainable.

## Features

- **User Authentication**: Supports login via email/password, OTP, and social providers.
- **Role-Based Access Control**: Manage users, roles, and scopes for fine-grained permissions.
- **Email Notifications**: Integrated email templates for account confirmation, password reset, and login notifications.
- **API-First Design**: RESTful APIs for integration with client applications.
- **Docker Support**: Containerized deployment with Docker Compose.
- **CI/CD Ready**: Azure Pipelines configuration for automated builds and deployments.
- **Testing**: Comprehensive unit tests to ensure reliability.

## Prerequisites

Before running this project, ensure you have the following installed:

- [.NET 6.0 or later](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/) (for frontend assets in the Provider)
- [Docker](https://www.docker.com/) (optional, for containerized deployment)
- A database (e.g., SQL Server, PostgreSQL) – configure connection strings in `appsettings.json`

## Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/your-repo/SSOCore.git
   cd SSOCore
   ```

2. Restore NuGet packages:
   ```bash
   dotnet restore
   ```

3. Install frontend dependencies (if applicable):
   ```bash
   cd src/SsoCore.Provider
   npm install
   ```

4. Configure the database:
   - Update connection strings in `src/SsoCore.Provider/appsettings.json`.
   - Run migrations:
     ```bash
     dotnet ef database update
     ```

## Running the Application

### Using .NET CLI
1. Navigate to the Provider project:
   ```bash
   cd src/SsoCore.Provider
   ```

2. Run the application:
   ```bash
   dotnet run
   ```

The API will be available at `https://localhost:5001` (or as configured).

### Using Docker
1. Build and run with Docker Compose:
   ```bash
   docker-compose up --build
   ```

## Testing

Run the tests using the .NET CLI:
```bash
dotnet test
```

For coverage, use a tool like Coverlet:
```bash
dotnet test /p:CollectCoverage=true
```

## Contributing

We welcome contributions! Please follow these steps:

1. Fork the repository.
2. Create a feature branch: `git checkout -b feature/your-feature`.
3. Commit your changes: `git commit -m 'Add some feature'`.
4. Push to the branch: `git push origin feature/your-feature`.
5. Open a pull request.

Please ensure your code follows the project's coding standards and includes tests.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## API Documentation

For API references, see the Swagger UI at `/swagger` when the application is running.

## Support

If you have any questions or issues, please open an issue on GitHub or contact the maintainers.