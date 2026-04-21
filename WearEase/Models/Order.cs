/*using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace WearEase.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required]
        public string CustomerId { get; set; }
        public IdentityUser Customer { get; set; }

        public DateTime OrderDate { get; set; }

        [Required]
        public decimal TotalAmount { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
*/
using System.ComponentModel.DataAnnotations;

namespace WearEase.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required]
        public string CustomerId { get; set; }

        public DateTime OrderDate { get; set; }
        public string  CustomerName { get; set; }
        public string  CustomerEmail { get; set; }

        [Required]
        public decimal TotalAmount { get; set; }

        public List<OrderItem> OrderItems { get; set; } = new();
        public string Status { get; set; }   //NEW
        public string PaymentMethod { get; set; }  // COD / Card
        public string PaymentStatus { get; set; }
    }
}
