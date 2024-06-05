using System.ComponentModel.DataAnnotations.Schema;

namespace Shop.Models
{
    [Table("Stock")]
    public class Stock
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public int Quantity { get; set; }
        public Item? Item { get; set; }
    }
}
