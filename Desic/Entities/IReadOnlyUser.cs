using System;

namespace Desic.Entities
{
    public interface IReadOnlyUser
    {
        Guid? Id { get; }
        long? SequentialId { get; }
        DateTime? CreatedOn { get; }
        string CreatedBy { get; }
        string CreatedByType { get; }
        DateTime? ModifiedOn { get; }
        string ModifiedBy { get; }
        string ModifiedByType { get; }
        bool? Hidden { get; }
        string Username { get; }
    }
}
