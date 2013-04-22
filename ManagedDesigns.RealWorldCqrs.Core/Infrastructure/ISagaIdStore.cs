using System;
using System.Collections.Generic;
using System.Linq;
using CommonDomain;

namespace ManagedDesigns.RealWorldCqrs.Core.Infrastructure
{
    public interface ISagaIdStore
    {
        Guid GetSagaIdFromCorrelationKey<TSaga>(object correlationKey) where TSaga : ISaga;
    }

    public class InMemorySagaIdStore : ISagaIdStore
    {
        private static readonly IList<SagaIdentifier> Data = new List<SagaIdentifier>();

        #region ISagaIdStore Members

        public Guid GetSagaIdFromCorrelationKey<TSaga>(object correlationKey) where TSaga : ISaga
        {
            if (correlationKey == null) throw new ArgumentNullException("correlationKey");
            lock (Data)
            {
                Guid sagaid = Guid.Empty;
                SagaIdentifier[] data = Data.ToArray();
                if (data.Any(i => i.SagaType == typeof (TSaga).FullName && correlationKey.Equals(i.CorrelationKey)))
                {
                    sagaid =
                        data.Single(
                            i => i.SagaType == typeof (TSaga).FullName && correlationKey.Equals(i.CorrelationKey))
                            .Id;
                }
                else
                {
                    sagaid = Guid.NewGuid();
                    Data.Add(new SagaIdentifier
                                 {
                                     Id = sagaid,
                                     SagaType = typeof (TSaga).FullName,
                                     CorrelationKey = correlationKey
                                 });
                }
                return sagaid;
            }
        }

        #endregion
    }

    internal class SagaIdentifier
    {
        public Guid Id { get; set; }
        public string SagaType { get; set; }
        public object CorrelationKey { get; set; }
    }
}