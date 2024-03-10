namespace Desic.EntityFrameworkCore.Entities
{
    public class ModifiableEntity : CreatableEntity
    {
        public Guid ModifiedById { get; set; }
        public Guid ModifiedByTypeId { get; set; }
        public DateTime ModifiedOn { get; set; }
    }
}
