FOR %%i IN (src\Infrastructure.Data.Sqlite\Migrations\*)    DO IF /I NOT "%%i"=="src\Infrastructure.Data.Sqlite\Migrations\.editorconfig" DEL /F /Q "%%i"
FOR %%i IN (src\Infrastructure.Data.SqlServer\Migrations\*) DO IF /I NOT "%%i"=="src\Infrastructure.Data.SqlServer\Migrations\.editorconfig" DEL /F /Q "%%i"
dotnet ef migrations add Initial --context ApplicationDbContext --project ./src/Infrastructure.Data.Sqlite
dotnet ef migrations add Initial --context ApplicationDbContext --project ./src/Infrastructure.Data.SqlServer
pause