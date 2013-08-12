using System;
using System.Linq;
using ManagedDesigns.RealWorldCqrs.Core.Domain.QueryModel;
using ManagedDesigns.RealWorldCqrs.Core.Infrastructure;

namespace ManagedDesigns.RealWorldCqrs.Web.Models
{
    public class ReadModel
    {   
        private readonly IQueryStorage storage;

        public ReadModel(IQueryStorage storage)
        {
            if (storage == null) throw new ArgumentNullException("storage");
            this.storage = storage;
        }

        public IQueryable<ManagerInfo> GetManagers()
        {
            return
                (from m in this.storage.Items<ManagerInfo>()
                select m).AsQueryable();
        }

        public IQueryable<AcceptedOrder> GetAcceptedOrders()
        {
            return
                (from o in this.storage.Items<AcceptedOrder>()
                 select o
                 ).AsQueryable();
        }

        public IQueryable<OrderInProgress> GetInProgressOrders()
        {
            return
                (from o in this.storage.Items<OrderInProgress>()
                 select o
                 ).AsQueryable();
        }

        public IQueryable<RejectedOrder> GetRejectedOrders()
        {
            return
                (from o in this.storage.Items<RejectedOrder>()
                 select o
                 ).AsQueryable();
        } 

        public IQueryable<Book> GetBooks()
        {
            return new[]
                {
                    new Book()
                        {
                            Name = "Implementing Domain-Driven Design",
                            Author = " Vaughn Vernon",
                            Price = 35.02,
                            Url =
                                "http://www.amazon.com/Implementing-Domain-Driven-Design-Vaughn-Vernon/dp/0321834577/ref=sr_1_1?ie=UTF8&qid=1366752123&sr=8-1&keywords=DDD",
                            ImageUrl =
                                "~/Content/images/iddd.png"
                        },
                    new Book()
                        {
                            Name = "Domain-Driven Design: Tackling Complexity in the Heart of Software",
                            Author = "Eric Evans",
                            Price = 48.83,
                            Url =
                                "http://www.amazon.com/Domain-Driven-Design-Tackling-Complexity-Software/dp/0321125215/ref=sr_1_2?ie=UTF8&qid=1366752123&sr=8-2&keywords=DDD",
                            ImageUrl ="~/Content/images/ddd.jpg"
                        },
                    new Book()
                        {
                            Name = "Event Centric: Finding Simplicity in Complex Systems",
                            Author = "Greg Young",
                            Price = 53.81,
                            Url =
                                "http://www.amazon.com/Event-Centric-Simplicity-Addison-Wesley-Signature/dp/0321768221",
                            ImageUrl = "~/Content/images/eventcentric.jpg"
                        },
                }.AsQueryable();
        }
    }
}