using System;
using System.Linq;
using ManagedDesigns.RealWorldCqrs.Core.Domain.Model;
using ManagedDesigns.RealWorldCqrs.Core.Domain.QueryModel;
using ManagedDesigns.RealWorldCqrs.Core.Infrastructure;
using MassTransit;

namespace ManagedDesigns.RealWorldCqrs.Core.Domain.Denormalizers
{
    public class ValidationOrderDenormilizer
        : Consumes<OrderValidatedByManager>.All
          , Consumes<OrderNotValidatedByManager>.All
    {
        private readonly IUpdateStorage storage;
        private readonly ILogger logger;

        //public ValidationOrderDenormilizer(IUpdateStorage storage)
        //    : this(storage, new EmptyLogger())
        //{
        //}

        public ValidationOrderDenormilizer(IUpdateStorage storage, ILogger logger)
        {
            if (storage == null) throw new ArgumentNullException("storage");
            if (logger == null) throw new ArgumentNullException("logger");
            this.storage = storage;
            this.logger = logger;
        }

        public void Consume(OrderNotValidatedByManager message)
        {
            if (
                storage.Items<ValidationOrder>().Any(
                    o => o.ManagerId == message.ManagerId.ToString() && o.OrderId == message.OrderId.ToString()))
            {
                return;
            }
            storage.Add(new ValidationOrder
                            {
                                OrderId = message.OrderId.ToString(),
                                ManagerId = message.ManagerId.ToString(),
                                ManagerName = message.ManagerName,
                                IsValidated = false,
                                NotValidatedReason = message.Reason
                            });
            logger.LogInfo(LoggerNames.Denormalizer, "Order '{0}' validation refused by '{1}' due to '{2}'", message.OrderId.ToString(), message.ManagerName, message.Reason);
        }

        public void Consume(OrderValidatedByManager message)
        {
            if (
                storage.Items<ValidationOrder>().Any(
                    o => o.ManagerId == message.ManagerId.ToString() && o.OrderId == message.OrderId.ToString()))
            {
                return;
            }
            storage.Add(new ValidationOrder
                            {
                                OrderId = message.OrderId.ToString(),
                                ManagerId = message.ManagerId.ToString(),
                                ManagerName = message.ManagerName,
                                IsValidated = true
                            });
            logger.LogInfo(LoggerNames.Denormalizer, "Order '{0}' validation accepted by '{1}'", message.OrderId.ToString(), message.ManagerName);
        }
    }
}