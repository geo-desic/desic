namespace Desic.EntityFrameworkCore.Models
{
    public class User
    {
        public Guid? Id { get; set; }
        public long? SequentialId { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? CreatedBy { get; set; }
        public string? CreatedByType { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string? ModifiedBy { get; set; }
        public string? ModifiedByType { get; set; }
        public bool? Hidden { get; set; }
        public string? Username { get; set; }
    }
}