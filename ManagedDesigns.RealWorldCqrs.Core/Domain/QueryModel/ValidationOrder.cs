namespace ManagedDesigns.RealWorldCqrs.Core.Domain.QueryModel
{
    public class ValidationOrder
    {
        public string OrderId { get; set; }
        public string ManagerId { get; set; }
        public string ManagerName { get; set; }
        public bool IsValidated { get; set; }
        public string NotValidatedReason { get; set; }
    }
}