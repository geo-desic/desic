# Entity Framework Core Migration Projects

## dotnet format validation

An .editorconfig file in the Migrations folder containing the following should prevent the auto-generated code from being flagged by `dotnet format` as needing formatting changes.

```ini
[*]
generated_code = true
```
