using System;
using System.Collections.Generic;
using System.Linq;
using CommonDomain;
using Raven.Client;

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
                            .SagaId;
                }
                else
                {
                    sagaid = Guid.NewGuid();
                    Data.Add(new SagaIdentifier
                                 {
                                     SagaId = sagaid,
                                     SagaType = typeof (TSaga).FullName,
                                     CorrelationKey = correlationKey
                                 });
                }
                return sagaid;
            }
        }

        #endregion
    }

    public class RavenDbSagaIdStore : ISagaIdStore
    {
        private readonly IDocumentStore documentStore;

        public RavenDbSagaIdStore(IDocumentStore documentStore)
        {
            if (documentStore == null) throw new ArgumentNullException("documentStore");
            this.documentStore = documentStore;
        }

        public Guid GetSagaIdFromCorrelationKey<TSaga>(object correlationKey) where TSaga : ISaga
        {
            if (correlationKey == null) throw new ArgumentNullException("correlationKey");
            var sagaType = typeof (TSaga).FullName;
            Func<SagaIdentifier, bool> getItemFunc = d => (d.CorrelationKey != null && d.CorrelationKey.ToString() == correlationKey.ToString()) && d.SagaType == sagaType;

            using (var session = this.documentStore.OpenSession())
            {
                if (session.Query<SagaIdentifier>().Customize(c=>c.WaitForNonStaleResults()).Any(getItemFunc))
                {
                    return session.Query<SagaIdentifier>().Customize(a=>a.WaitForNonStaleResults()).Single(getItemFunc).SagaId;
                }
                Guid sagaId = Guid.NewGuid();
                session.Store(new SagaIdentifier()
                    {
                        CorrelationKey = correlationKey, SagaId = sagaId, SagaType = sagaType
                    });
                session.SaveChanges();
                return sagaId;
            }
        }
    }

    internal class SagaIdentifier
    {
        public Guid SagaId { get; set; }
        public string SagaType { get; set; }
        public object CorrelationKey { get; set; }
    }
}