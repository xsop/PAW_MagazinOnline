using System.ComponentModel.DataAnnotations;

namespace Shop.Models.DTO
{
    public class StockDTO
    {
        public int ItemId { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Quantity out of range")]
        public int Quantity { get; set; }
    }
}
