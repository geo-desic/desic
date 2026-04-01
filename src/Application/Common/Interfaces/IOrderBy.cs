namespace Desic.Application.Common.Interfaces;

public interface IOrderBy<T> : IOrderBy where T : struct, Enum
{
    new T Property { get; set; }
    object IOrderBy.Property
    {
        get => Property;
        set
        {
            if (value is T enumValue)
            {
                Property = enumValue;
            }
            else
            {
                throw new ArgumentException($"Value must be of type {typeof(T).Name}");
            }
        }
    }
}

public interface IOrderBy
{
    bool Ascending { get; set; }
    object Property { get; set; }
}