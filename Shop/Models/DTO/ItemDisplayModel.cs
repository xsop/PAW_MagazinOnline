namespace Shop.Models.DTO
{
    public class ItemDisplayModel
    {
        public IEnumerable<Item> Items { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public string sterm { get; set; } = "";
        public int? catId { get; set; } = null;


    }
}
