namespace Desic.Api.Dtos;

// open api does not currently support custom object array/list types as query parameters
// so this class is designed to use primitive array types which is supported, e.g. ?orderBy=Name&asc=true&orderBy=Id&asc=false
public class OrderingMethodFromQuery<T> where T : struct, Enum
{
    public List<T> OrderBy { get; set; } = [];
    public List<bool> Asc { get; set; } = [];
}
