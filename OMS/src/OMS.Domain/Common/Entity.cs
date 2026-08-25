namespace OMS.Domain.Common
{
    public abstract class Entity
    {
        public Guid Id { get; protected set; }

        protected Entity()
        { }

        protected Entity(Guid id)
        {
            Id = id;
        }

        public override bool Equals(object? obj)
        {
            if (obj is not Entity other)
                return false;
            if (ReferenceEquals(this, other))
                return true;
            if (GetType() != other.GetType())
                return false;

            return Id.Equals(other.Id);
        }

        public static bool operator ==(Entity? left, Entity? right) =>
            left?.Equals(right) ?? right is null;

        public static bool operator !=(Entity? left, Entity? right) => !(left == right);

        public override int GetHashCode() => HashCode.Combine(GetType(), Id);
    }
}