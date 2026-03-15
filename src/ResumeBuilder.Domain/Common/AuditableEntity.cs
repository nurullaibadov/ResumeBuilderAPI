namespace ResumeBuilder.Domain.Common;
public abstract class AuditableEntity : BaseEntity
{
    public string? LastModifiedBy { get; set; }
    public DateTime? LastModifiedAt { get; set; }
}
