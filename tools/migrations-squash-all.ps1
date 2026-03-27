$pathMigrationsSqlite = "../src/Infrastructure.Data.Sqlite/Migrations/"
$pathMigrationsSqlServer = "../src/Infrastructure.Data.SqlServer/Migrations/"
$fileToKeep = ".editorconfig"
Get-ChildItem -Path $pathMigrationsSqlite -File | Where-Object { $_.Name -ne $fileToKeep } | Remove-Item -Force
Get-ChildItem -Path $pathMigrationsSqlServer -File | Where-Object { $_.Name -ne $fileToKeep } | Remove-Item -Force
dotnet ef migrations add Initial --context ApplicationDbContext --project ../src/Infrastructure.Data.Sqlite
dotnet ef migrations add Initial --context ApplicationDbContext --project ../src/Infrastructure.Data.SqlServer