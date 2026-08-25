namespace OMS.Domain.Common
{
    public interface IAuditableEntity
    {
        DateTime CreatedAt { get; }
        DateTime? UpdatedAt { get; }
    }
}
