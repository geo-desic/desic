namespace Desic.EntityFrameworkCore.Entities
{
    public class CreatableEntity : MinimalEntity
    {
        public Guid CreatedById { get; set; }
        public Guid CreatedByTypeId { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
