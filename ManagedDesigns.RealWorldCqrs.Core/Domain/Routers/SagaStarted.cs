using System;

namespace ManagedDesigns.RealWorldCqrs.Core.Domain.Routers
{
    public class SagaStarted 
    {
        public SagaStarted(Guid sagaId, object correlationId)
        {
            SagaId = sagaId;
            CorrelationId = correlationId;
        }

        public Guid SagaId { get; set; }
        public object CorrelationId { get; set; }
    }
}