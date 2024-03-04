using System;

namespace Desic.Entities
{
    public interface IUser : IReadOnlyUser
    {
        new Guid? Id { get;  set; }
        new long? SequentialId { get; set; }
        new DateTime? CreatedOn { get; set; }
        new string CreatedBy { get; set; }
        new string CreatedByType { get; set; }
        new DateTime? ModifiedOn { get; set; }
        new string ModifiedBy { get; set; }
        new string ModifiedByType { get; set; }
        new bool? Hidden { get; set; }
        new string Username { get; set; }
    }
}
