namespace Mimre.Domain.Common;

public abstract class AuditableEntity : BaseEntity
{
    public DateTime CreatedOn { get; set; }
    public DateTime ModifiedOn { get; set; }
}
