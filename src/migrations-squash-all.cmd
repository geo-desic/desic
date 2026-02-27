FOR %%i IN (Infrastructure.Data.Sqlite\Migrations\*)    DO IF /I NOT "%%i"=="Infrastructure.Data.Sqlite\Migrations\.editorconfig" DEL /F /Q "%%i"
FOR %%i IN (Infrastructure.Data.SqlServer\Migrations\*) DO IF /I NOT "%%i"=="Infrastructure.Data.SqlServer\Migrations\.editorconfig" DEL /F /Q "%%i"
dotnet ef migrations add Initial --context DesicContext --project ./Infrastructure.Data.Sqlite
dotnet ef migrations add Initial --context DesicContext --project ./Infrastructure.Data.SqlServer
pause