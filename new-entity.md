# Adding A New Entity
This guide will cover some important steps but it is not necessarily comprehensive.

Non-existant example entity used for this guide: `Person` with pluralization `Persons`

## Domain Project

### System Entity Type
In [SystemEntityTypes.cs](src/Domain/EntityTypes/SystemEntityTypes.cs) add a system entity type following the uniqueness instructions and also add it to the `All()` method.

```c#
    // append to the other existing system entity types (where XXX is the next sequential value)
    public static readonly SystemEntityType Person                        = new(Id: new("00000XXX-0000-0000-0000-000000000000"), Key: "pers", Name: nameof(Person));
```

```c#
    internal static IEnumerable<SystemEntityType> All()
    {
        // append to end of method
        yield return Person;
    }
```

### Domain Namespace/Folder
Add a top level folder/namespace for the entity (plural): `Desic.Domain.Persons`

### Domain Entity 
Add a class to the namespace that inherits (not necessarily directly) from `BaseEntity`. More likely it will inherit from one of its children: `CreatableEntity`, `ModifiableEntity`, `SoftDeletableEntity` 

```c#
namespace Desic.Domain.Persons;

public class Person : SoftDeletableEntity, IStaticEntityType
{
    public static SystemEntityType ClassEntityType => SystemEntityTypes.Person;
    public override SystemEntityType SystemEntityType => ClassEntityType;

    // add desired properties
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    // ...

    // add desired constants
    public const int MaxLengthFirstName = 50;
    public const int MaxLengthLastName = 50;
    // ...
}
```

## Application Project

### IApplicationDbContext
In [IApplicationDbContext.cs](src/Application/Common/Interfaces/IApplicationDbContext.cs) add an alphebetized named (plural) `DbSet<T>` property for the entity.

```c#
    // alphabetize this in the file
    DbSet<Person> Persons { get; }
```

### Log Events
In [LogEvents.cs](src/Application/Common/LogEvents.cs) in the section of the file with all of the entity regions add append a new region for the entity specifying at least the entity's constant integer start value which would reference the start value of the directly prior entity.

```c#
    #region Persons
    internal const int StartPersons                        = StartXXX + OffsetEntity;
    // optionally add any desired categorized log events
    public const int CreatePerson                          = StartPersons + OffsetCreate;
    //...
    #endregion
```

## Application Unit Tests Project

### TestApplicationDbContext
In [TestApplicationDbContext.cs](test/Application.Tests.Unit/TestApplicationDbContext.cs) add an alphabetized named (plural) `DbSet<T>` property for the entity.

```c#
    // alphabetize this in the file
    public DbSet<Person> Persons { get; set; }
```


## Infrastructure Project - Data Folder/Namespace

### Entity Configuration - Persistence Information
Add a class to the `Desic.Infrastructure.Data.Configurations` namespace defining the persistence information (e.g. database table definition).

```c#
namespace Desic.Infrastructure.Data.Configurations;

public class PersonConfiguration(DatabaseFacade databaseFacade) : IEntityTypeConfiguration<Person>
{
    private readonly DatabaseFacade _databaseFacade = databaseFacade ?? throw new ArgumentNullException(nameof(databaseFacade));

    public void Configure(EntityTypeBuilder<Person> builder)
    {
        var schema = _databaseFacade.SupportsSchemas() ? ApplicationDbContext.AppSchema : null;
        builder.ToTable(nameof(ApplicationDbContext.Persons), schema); // note: ApplicationDbContext.Persons won't resolve until the subsequent step
        var columnOrder = builder.ConfigureSoftDeletableEntity(_databaseFacade);
        builder.Property(x => x.FirstName).IsRequired().HasMaxLength(Person.MaxLengthFirstName).HasColumnOrder(++columnOrder);
        builder.Property(x => x.LastName).IsRequired().HasMaxLength(Person.MaxLengthLastName).HasColumnOrder(++columnOrder);
        // other desired properties
        builder.HasIndex(x => x.FirstName).IsUnique(false);
        builder.HasIndex(x => x.LastName).IsUnique(false);
        // other keys/indexes/constraints/...
    }
}
```

### ApplicationDbContext
In [ApplicationDbContext.cs](src/Infrastructure/Data/ApplicationDbContext.cs)
- Add an alphabetized named (plural) `DbSet<T>` for the entity
- Add an alphabetized line to configure the entity in the `OnModelCreating` method using the configuration class created above, e.g. `PersonConfiguration`

```c#
    // alphabetize this in the file
    public DbSet<Person> Persons { get; set; }

    //...

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        if (Database.SupportsSchemas()) modelBuilder.HasDefaultSchema(AppSchema);
        // alphabetized entity configurations
        // ...
        modelBuilder.ApplyConfiguration(new PersonConfiguration()); // <==== new line added
        // ...
        modelBuilder.SetUtcValueConverterForAllDateTimeProperties();
    }
```

### Seedability
If seeding support is desired for the entity then that isn't currently covered by this guide. Can potentially search for `Iso3166Country / Iso3166Countries` in the `Infrastructure` project as a guide.

## Compile Solution
At this point, if everything was done correctly, the solution should hopefully compile.

## New Migration
If a non-development environment doesn't exist yet, can simply run [migrations-squash-all.ps1](tools/migrations-squash-all.ps1). This will delete and re-create the `Initial` migration. However once migrations are actually in use, a new migration would be needed. Choose a mostly unique name for the migration, e.g. `PersonEntityAdded` and execute each of the following commands (for all supported providers) from a terminal in the top level solution directory. See [migrations.md](migrations.md) for more information.

```
dotnet ef migrations add PersonEntityAdded --no-build --context ApplicationDbContext --project ./Infrastructure.Data.Sqlite
dotnet ef migrations add PersonEntityAdded --no-build --context ApplicationDbContext --project ./Infrastructure.Data.SqlServer
```

This should make changes in the `Migrations` folder/namespace inside all of the projects referenced in the commands above.

## Execute Tests
Execute all tests to make sure there aren't any outstanding issues. Can also manually execute the DbUpdater application against a local database server to create the database and view the table for the entity that was created.

## Entity Use Cases
In the Application project can add use cases for the entity, e.g. `CreatePerson`, `GetPerson`, `ListPersons` and any related api endpoints in the Api project. Make sure any such code added is covered under all applicable types of testing (e.g. Unit, Integration, Functional).
