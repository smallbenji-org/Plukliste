# PluckFish Documentation

## Overview
PluckFish is a warehouse management web application built with ASP.NET Core. It helps users manage products, picking lists, stock, and authentication. The app supports PostgreSQL for data storage and includes dummy repositories for testing.

## Features
- User authentication and role management
- Product management (CRUD)
- Picking list management
- Stock management
- QR code scanner integration (webcam-based)
- Dashboard and analytics (optional)

## Project Structure
- **Controllers/**: Handles HTTP requests and application logic
- **Models/**: Data models for products, picking lists, users, etc.
- **Views/**: Razor views for UI
- **Components/**: Repositories and helpers for data access
- **Interfaces/**: Repository interfaces for dependency injection
- **wwwroot/**: Static files (CSS, JS, images)

## Getting Started
1. Clone the repository
2. Install .NET 9 SDK
3. Configure PostgreSQL connection in `appsettings.json`
4. Run database migrations (if needed)
5. Start the application:
   ```zsh
   dotnet run
   ```
6. Access the app at `https://localhost:5001` (or configured port)

## Authentication
- Users can register and log in
- Roles are managed via `Auth/RoleStore.cs` and `Auth/UserStore.cs`

## Product Management
- Add, edit, delete, and view products
- Product images can be added (optional)

## Picking List Management
- Create and manage picking lists
- Assign products to picking lists

## Stock Management
- View and update stock levels
- Stock changes are tracked

## Testing
- Dummy repositories available for testing without a real database

## Contributing
1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push and create a pull request

## License
MIT License

## Contact
For questions or support, contact the repository owner via GitHub.
