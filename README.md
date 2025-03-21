- Project Name
CrowdFunding Website

- Prerequisites
Ensure you have the following installed:

+ .NET SDK (latest version recommended)
+ MySQL Server
+ MySQL Workbench (Optional)
+ Visual Studio (or VS Code with C# extensions)
+ dotnet-ef tool (if migrations are required)
+ Installation Steps

- Clone the repository:

+ Run the command: git clone https://github.com/ahmedelb123/CrowdFundingProject.git
+ Navigate to the project directory using: cd your-repository
  
- Configure the database connection:

+ Open the appsettings.json file
+ Find the "ConnectionStrings" section and update it with your database credentials:
+ "DefaultConnection": "Server=localhost;Database=your_db;User=root;Password=your_password;"
  
- Restore dependencies:

+ Run the command: dotnet restore
- Apply database migrations (if applicable):

+ Run the command: dotnet ef database update
- If no migrations exist, create one using:
+ dotnet ef migrations add InitialCreate
+ dotnet ef database update
- Run the application:

+ Execute the command: dotnet run
- Common Commands
+ To add a migration, use: dotnet ef migrations add MigrationName
+ To update the database, use: dotnet ef database update
+ To run the project, use: dotnet run
+ To restore packages, use: dotnet restore
