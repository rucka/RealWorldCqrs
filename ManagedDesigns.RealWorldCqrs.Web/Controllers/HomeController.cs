using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ManagedDesigns.RealWorldCqrs.Core.Domain.Commands;
using ManagedDesigns.RealWorldCqrs.Web.Components;
using ManagedDesigns.RealWorldCqrs.Web.Models;
using ManagedDesigns.RealWorldCqrs.Web.ViewModels;
using MassTransit;

namespace ManagedDesigns.RealWorldCqrs.Web.Controllers
{
    public class HomeController : Controller
    {
        public ReadModel ReadModel { get; private set; }
        public IServiceBus Bus { get; private set; }
        
        public HomeController(ReadModel readModel, IServiceBus bus)
        {
            if (readModel == null) throw new ArgumentNullException("readModel");
            ReadModel = readModel;
            Bus = bus;
        }

        public ActionResult Index()
        {
            var model = new HomeIndexViewModel();

            this.ReadModel.GetBooks().ForEach(model.Books.Add);

            return View(model);
        }

        [HttpGet]
        public ActionResult Backoffice()
        {
            var model = new HomeBackofficeViewModel();
            this.ReadModel.GetManagers().ForEach(model.Managers.Add);
            return View(model);
        }

        [HttpGet]
        public ActionResult OrderMonitor()
        {
            var model = new HomeOrderMonitorViewModel();
            this.ReadModel.GetInProgressOrders().ForEach(model.InProgressOrders.Add);
            this.ReadModel.GetAcceptedOrders().ForEach(model.AcceptedOrders.Add);
            this.ReadModel.GetRejectedOrders().ForEach(model.RejectedOrders.Add);
            this.ReadModel.GetManagers().ForEach(model.AvailableManagers.Add);
            
            return View("OrderMonitor", model);
        }

        [HttpPost]
        public ActionResult AssignOrder(string orderId, string managerId)
        {
            var model = new HomeOrderMonitorViewModel();

            try
            {
                var orders = ReadModel.GetInProgressOrders().ToArray();
                var managers = ReadModel.GetManagers().ToArray();
                if (!ReadModel.GetInProgressOrders().Any(o => o.OrderId == orderId))
                {
                    model.Result = HomeOrderMonitorViewModel.OperationResult.Ko;
                    model.OperationMessage = string.Format("Order '{0}' not exists.", managerId);
                    return View("OrderMonitor", model);
                }
                if (!ReadModel.GetManagers().Any(m => m.Id == managerId))
                {
                    model.Result = HomeOrderMonitorViewModel.OperationResult.Ko;
                    model.OperationMessage = string.Format("Manager '{0}' not exists.", managerId);
                    return View("OrderMonitor", model);
                }

                this.Bus.Publish(new ValidateOrder(Guid.Parse(managerId), Guid.Parse(orderId)));

                model.Result = HomeOrderMonitorViewModel.OperationResult.Ok;
                this.ReadModel.GetInProgressOrders().ForEach(model.InProgressOrders.Add);
                this.ReadModel.GetAcceptedOrders().ForEach(model.AcceptedOrders.Add);
                this.ReadModel.GetRejectedOrders().ForEach(model.RejectedOrders.Add);
                this.ReadModel.GetManagers().ForEach(model.AvailableManagers.Add);
            }
            catch
            {
                model.Result = HomeOrderMonitorViewModel.OperationResult.Ko;
                model.OperationMessage = "Assign order cannot be submitted. Please, retry later.";
                return View("OrderMonitor", model);
            }
        
            return View("OrderMonitor", model);
        }

        [HttpPost]
        public JsonResult PlaceOrder(string book, int quantity)
        {
            string errorMessage = string.Empty;
            try
            {
                if (!this.ReadModel.GetBooks().Any(b => b.Name == book))
                {
                    errorMessage = string.Format("book '{0}' not exists in catalog");
                }

                if (quantity <= 0)
                {
                    errorMessage = string.Format("Quantity not valid");
                }

                if (!errorMessage.Any())
                {
                    var price = this.ReadModel.GetBooks().Single(b => b.Name == book).Price;
                    var cmd = new PlaceOrder(Guid.NewGuid(), book, book, quantity, price);
                    this.Bus.Publish(cmd);
                }
            }
            catch (Exception e)
            {
                errorMessage = "An error occours during request. Please ask to administrator";
            }
           

            return Json(new
                {
                    Result = !errorMessage.Any(),
                    Error = errorMessage
                });
        }

        [HttpPost]
        public ActionResult CreateManager(string fullname)
        {
            var model = new HomeBackofficeViewModel();
            if (string.IsNullOrWhiteSpace(fullname))
            {
                model.Result = HomeBackofficeViewModel.OperationResult.Ko;
                model.OperationMessage = "Full name cannot be null";
                return View("Backoffice", model);
            }

            if (!this.IsValidManagerName(fullname))
            {
                model.Result = HomeBackofficeViewModel.OperationResult.Ko;
                model.OperationMessage = string.Format("Manager '{0}' already exists", fullname);
                return View("Backoffice", model);
            }

            try
            {
                var nameparts = fullname.Split(' ');
                var firstname = nameparts.Count() > 1 ? nameparts.First() : string.Empty;
                var surname = nameparts.Count() > 1 ? nameparts.Skip(1).First() : nameparts.First();

                this.Bus.Publish(new CreateManager(Guid.NewGuid(), firstname.Trim(), surname.Trim()));

                model.Result = HomeBackofficeViewModel.OperationResult.Ok;
                model.OperationMessage = string.Format("Manager '{0}' creation submitted.", fullname);
            }
            catch
            {
                model.Result = HomeBackofficeViewModel.OperationResult.Ko;
                model.OperationMessage = "Create manager cannot be submitted. Please, retry later.";
            }

            return View("Backoffice", model);
        }

        [HttpGet]
        public bool IsValidManagerName(string fullname)
        {
            if (string.IsNullOrWhiteSpace(fullname))
            {
                return false;
            }
            return !this.ReadModel.GetManagers().Any(m => fullname.Trim() == m.Fullname);
        }
    }
}
