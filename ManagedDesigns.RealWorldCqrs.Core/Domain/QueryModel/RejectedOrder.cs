namespace ManagedDesigns.RealWorldCqrs.Core.Domain.QueryModel
{
    public class RejectedOrder
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public string Reason { get; set; }
    }
}