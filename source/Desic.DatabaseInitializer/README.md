# Database Initializer

## Contained Database

```sql
EXEC sys.sp_configure 'show advanced', 1;
GO
RECONFIGURE;
GO
EXEC sys.sp_configure 'contained database authentication', 1;
GO
RECONFIGURE;
GO

CREATE DATABASE [Desic] CONTAINMENT = PARTIAL;
```