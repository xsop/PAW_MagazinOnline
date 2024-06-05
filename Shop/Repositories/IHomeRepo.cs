namespace Shop
{
    public interface IHomeRepo
    {
        Task<IEnumerable<Item>> GetItems(string sTerm = "", int? catId = null);
        Task<IEnumerable<Category>> GetCategories();
    }
}