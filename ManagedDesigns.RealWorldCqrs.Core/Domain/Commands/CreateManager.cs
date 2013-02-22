using System;

namespace ManagedDesigns.RealWorldCqrs.Core.Domain.Commands
{
    public sealed class CreateManager
    {
        public CreateManager(Guid id, string firstname, string lastname)
        {
            Id = id;
            Firstname = firstname;
            Lastname = lastname;
        }

        public Guid Id { get; private set; }
        public string Firstname { get; private set; }
        public string Lastname { get; private set; }
    }
}