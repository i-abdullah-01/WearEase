using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WearEase.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required, MaxLength(150)]
        public string Name { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public string Description { get; set; }

        public string ImageUrl { get; set; }

        public int CategoryId { get; set; }

        // Navigation
        public Category Category { get; set; }

        public int TotalQuantity { get; set; }

        // Auto-calculated — DO NOT UPDATE manually
       
      //  public int SoldQuantity {  get; set; }
        public int RemainingQuantity {  get; set; }
    }
}
