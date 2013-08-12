using System.Collections.Generic;
using ManagedDesigns.RealWorldCqrs.Core.Domain.Model;
using ManagedDesigns.RealWorldCqrs.Core.Domain.QueryModel;
using ManagedDesigns.RealWorldCqrs.Web.Models;

namespace ManagedDesigns.RealWorldCqrs.Web.ViewModels
{
    public class HomeBackofficeViewModel
        : BaseViewModel
    {
        public IList<ManagerInfo> Managers { get; private set; }

        public OperationResult Result { get; set; }
        public string OperationMessage { get; set; }

        public HomeBackofficeViewModel()
        {
            this.Title = "Backoffice";
            this.Managers = new List<ManagerInfo>();
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