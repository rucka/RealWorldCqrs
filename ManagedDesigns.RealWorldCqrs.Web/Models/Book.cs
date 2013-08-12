using System.ComponentModel.DataAnnotations;

namespace ManagedDesigns.RealWorldCqrs.Web.Models
{
    public class Book
    {
        public string Name { get; set; }
        public string Author { get; set; }
        public string Url { get; set; }
        public string ImageUrl { get; set; }
        public double Price { get; set; }
    }

}