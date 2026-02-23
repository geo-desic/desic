FOR %%i IN (Desic.EntityFrameworkCore.Sqlite\Migrations\*)    DO IF /I NOT "%%i"=="Desic.EntityFrameworkCore.Sqlite\Migrations\.editorconfig" DEL /F /Q "%%i"
FOR %%i IN (Desic.EntityFrameworkCore.SqlServer\Migrations\*) DO IF /I NOT "%%i"=="Desic.EntityFrameworkCore.Sqlite\Migrations\.editorconfig" DEL /F /Q "%%i"
dotnet ef migrations add Initial --context DesicContext --project ./Desic.EntityFrameworkCore.Sqlite
dotnet ef migrations add Initial --context DesicContext --project ./Desic.EntityFrameworkCore.SqlServer
pause