# Managing Entity Framework Core Migrations

## Option 1: Reverting Specific Migrations

1. List existing migrations:
   ```bash
   dotnet ef migrations list
   ```

2. Revert to a specific migration:
   ```bash
   dotnet ef database update PreviousMigrationName
   dotnet ef migrations remove
   ```

## Option 2: Complete Reset (First/Only Migration)

1. Revert all migrations:
   ```bash
   dotnet ef database update 0
   dotnet ef migrations remove
   ```

## Option 3: Drop Database and Start Fresh (Development Only)

1. Drop the database:
   ```bash
   dotnet ef database drop -f
   ```

2. Delete the Migrations folder manually

3. Create fresh migration:
   ```bash
   dotnet ef migrations add InitialCreate -o Data/Migrations
   dotnet ef database update
   ```

## Creating New Migrations

```bash
dotnet ef migrations add MigrationName -o Data/Migrations
dotnet ef database update
```