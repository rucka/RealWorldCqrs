using System.Collections.Generic;
using ManagedDesigns.RealWorldCqrs.Core.Domain.QueryModel;

namespace ManagedDesigns.RealWorldCqrs.Web.ViewModels
{
    public class HomeOrderMonitorViewModel
        : BaseViewModel
    {
        public IList<RejectedOrder> RejectedOrders { get; private set; }
        public IList<AcceptedOrder> AcceptedOrders { get; private set; }
        public IList<OrderInProgress> InProgressOrders { get; private set; }

        public IList<ManagerInfo> AvailableManagers { get; private set; }

        public OperationResult Result { get; set; }
        public string OperationMessage { get; set; }

        public HomeOrderMonitorViewModel()
        {
            this.Title = "Order Monitor";
            this.RejectedOrders = new List<RejectedOrder>();
            this.AcceptedOrders = new List<AcceptedOrder>();
            this.InProgressOrders = new List<OrderInProgress>();
            this.AvailableManagers = new List<ManagerInfo>();

            this.Result = OperationResult.None;
        }

        public enum OperationResult
        {
            None,
            Ok,
            Ko
        }
    }
}