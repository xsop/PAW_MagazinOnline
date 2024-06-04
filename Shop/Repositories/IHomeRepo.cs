namespace Shop
{
    public interface IHomeRepo
    {
        Task<IEnumerable<Item>> GetItems(string sTerm = "", int catId = 0);
        Task<IEnumerable<Category>> GetCategories();
    }
}