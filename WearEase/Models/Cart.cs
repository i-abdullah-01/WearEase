/*using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WearEase.Models
{
    public class Cart
    {
        public int Id { get; set; }

        [Required]
        public string CustomerId { get; set; }
        public IdentityUser Customer { get; set; }

        public ICollection<CartItem> CartItems { get; set; }

    }
}
*/
using System.ComponentModel.DataAnnotations;

namespace WearEase.Models
{
    public class Cart
    {
        public int Id { get; set; }

        [Required]
        public string CustomerId { get; set; }

        public List<CartItem> CartItems { get; set; } = new();
    }
}
