namespace ManagedDesigns.RealWorldCqrs.Web.ViewModels
{
    public class BaseViewModel
    {
        public string Title { get; set; }

        public BaseViewModel()
        {
            this.Title = "Real world Cqrs & Event Sourcing";
        }
    }
}