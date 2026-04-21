/*namespace WearEase.Models.ViewModels
{
    public class CheckoutViewModel
    {
        // Billing info
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }

        // Cart
        public Cart Cart { get; set; }

        // Totals
        public decimal SubTotal { get; set; }
        public decimal Shipping { get; set; }
        public decimal Total { get; set; }
        public string PaymentMethod { get; set; } // COD / Card

        // Card (Demo)
        public string CardNumber { get; set; }
        public string Expiry { get; set; }
        public string CVV { get; set; }
    }
}
*/
using System.ComponentModel.DataAnnotations;

namespace WearEase.Models.ViewModels
{
    public class CheckoutViewModel
    {
        // Billing info
        [Required]
        public string FullName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string PostalCode { get; set; }

        [Required]
        public string Country { get; set; }

        // Cart
        public Cart Cart { get; set; }

        // Totals
        public decimal SubTotal { get; set; }
        public decimal Shipping { get; set; }
        public decimal Total { get; set; }

        [Required]
        public string PaymentMethod { get; set; } // COD / Card

        // Card (only required if Card selected)
        public string CardNumber { get; set; }
        public string Expiry { get; set; }
        public string CVV { get; set; }
    }
}
