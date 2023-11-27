namespace Domain.Kernal.Models;

public abstract class Entity<T> : IEquatable<Entity<T>>
    where T : notnull
{
    public T Id { get; protected set; }

    protected Entity(T id)
    {
        Id = id;
    }

    public override bool Equals(object? obj)
    {
        return obj is Entity<T> entity && Id.Equals(entity.Id);
    }

    public bool Equals(Entity<T>? other)
    {
        return Equals((object?)other);
    }

    public static bool operator ==(Entity<T> left, Entity<T> right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Entity<T> left, Entity<T> right)
    {
        return !Equals(left, right);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}
