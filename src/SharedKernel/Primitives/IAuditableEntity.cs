namespace SharedKernel.Primitives;

public interface IAuditableEntity
{
    DateTime CreatedAtUtc { get; set; }

    DateTime ModifiedAtUtc { get; set; }
}
