# Entity Framework Core Migration Projects

## Manual Changes To Initial Migration

This should be at the end of the `Initial.Up` method

```c#
migrationBuilder.CreateAppUserAndPermissions(password: "2d4ba4c0-6cd1-4c7c-b08c-0db156c44116");
```

This should be at the beginning of the `Initial.Down` method

```c#
migrationBuilder.UndoCreateAppUserAndPermissions();
```

If the initial migration is ever deleted and automatically recreated these will need to be added again (except for `Sqlite` which does not support database users/logins).