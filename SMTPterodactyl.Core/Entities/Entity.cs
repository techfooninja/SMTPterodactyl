namespace SMTPterodactyl.Core.Entities
{
    using System;

    public abstract class Entity
    {
        protected Entity(Guid id)
        {
            this.Id = id;
        }

        public Guid Id { get; private set; }
    }
}
