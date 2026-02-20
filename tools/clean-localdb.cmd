@echo off
sqlcmd -S "(localdb)\mssqllocaldb" -i "%~dp0databases-drop-all.sql" -o "drop-all.sql"
sqlcmd -S "(localdb)\mssqllocaldb" -i "drop-all.sql"
del "drop-all.sql"

%~dp0shrink-localdb-model.cmd