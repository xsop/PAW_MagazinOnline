namespace Shop.Repositories
{
    public interface IUserOrderRepo
    {
        Task<IEnumerable<Order>> UserOrders();
    }
}