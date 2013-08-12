using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ManagedDesigns.RealWorldCqrs.Web.Models;

namespace ManagedDesigns.RealWorldCqrs.Web.ViewModels
{
    public class HomeIndexViewModel 
        : BaseViewModel
    {
        public IList<Book> Books { get; private set; }

        public HomeIndexViewModel()
        {
            this.Books = new List<Book>();
        }
    }
}