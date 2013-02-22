using System;
using System.Collections.Generic;
using CommonDomain;
using ManagedDesigns.RealWorldCqrs.Core.Infrastructure;

namespace ManagedDesigns.RealWorldCqrs.Core.Domain.Model
{
    public class Manager : AggregateRoot<Manager.ManagerMemento>
    {
        private const double OrderMinimunAmount = 100;

        private Manager(Guid id, string firstname, string lastname)
            : this(id)
        {
            RaiseEvent(new ManagerCreated(Id, firstname, lastname));
        }

        public static Manager Create(Guid id, string firstname, string lastname)
        {
            return new Manager(id, firstname, lastname);
        }

        private Manager(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentNullException("id", "id cannot be empty");
            }
            Id = id;
        }

        private string Fullname
        {
            get { return string.Format("{0} {1}", State.Firstname, State.Lastname); }
        }


        public void ValidateOrder(Order order)
        {
            if (order == null) throw new ArgumentNullException("order");

            if (State.OrdersValidated.Contains(order.Id))
            {
                return;
            }

            if (order.TotalAmountIsOver(OrderMinimunAmount))
            {
                RaiseEvent(new OrderValidatedByManager(Id, order.Id, Fullname));
                return;
            }
            RaiseEvent(new OrderNotValidatedByManager(Id, order.Id, string.Format("Order has totalAmount lower or equal than {0}", OrderMinimunAmount), Fullname));
        }

        #region ApplyEvents

        private void Apply(ManagerCreated @event)
        {
            State.Firstname = @event.Firstname;
            State.Lastname = @event.Lastname;
        }

        private void Apply(OrderValidatedByManager @event)
        {
            State.OrdersValidated.Add(@event.OrderId);
        }

        private void Apply(OrderNotValidatedByManager @event)
        {
            State.OrdersValidated.Add(@event.OrderId);
        }
        #endregion

        #region Nested type: ManagerMemento

        public class ManagerMemento : IMemento
        {
            public string Firstname;
            public string Lastname;

            public ManagerMemento()
            {
                OrdersValidated = new List<Guid>();
            }

            public IList<Guid> OrdersValidated { get; set; }

            #region IMemento Members

            public Guid Id { get; set; }
            public int Version { get; set; }

            #endregion
        }

        #endregion
    }
}