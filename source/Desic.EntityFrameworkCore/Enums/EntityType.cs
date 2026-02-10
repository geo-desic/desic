namespace Desic.EntityFrameworkCore.Enums;

// do not change ushort values => used to generate ids in the database
public enum EntityType : ushort
{
    #pragma warning disable format
    Unspecified   = 1,
    EntityType    = 2,
    Tag           = 3,
    User          = 4,
    #pragma warning restore format
}
