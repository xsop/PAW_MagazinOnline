using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Shop.Repositories
{
    public class CartRepo : ICartRepo
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userMgr;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartRepo(ApplicationDbContext db, IHttpContextAccessor httpContextAccessor,
            UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userMgr = userManager;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<int> AddItem(int itemId, int qty)
        {
            string userId = GetUserId();
            using var transaction = _db.Database.BeginTransaction();
            try
            {
                if (string.IsNullOrEmpty(userId))
                    throw new Exception("User invalid (maybe not logged in)");
                var cart = await GetCart(userId);
                if (cart is null)
                {
                    cart = new ShoppingCart
                    {
                        UserId = userId
                    };
                    _db.ShoppingCarts.Add(cart);
                }
                _db.SaveChanges();
                // cart detail section
                var cartItem = _db.CartDetails
                                  .FirstOrDefault(a => a.ShoppingCartId == cart.Id && a.ItemId == itemId);
                var item = _db.Items.Find(itemId);
                if (item is null)
                {
                    throw new Exception($"Item with id {itemId} not found");
                }
                if (cartItem != null)
                {
                    // Check if the quantity exceeds the available stock
                    var stock = _db.Stocks.FirstOrDefault(a => a.ItemId == itemId);
                    if (stock is null)
                    {
                        throw new Exception("Stock not found");
                    }
                    if (cartItem.Quantity + qty > stock.Quantity)
                    {
                        throw new Exception("Item quantity exceeds available stock");
                    }
                    cartItem.Quantity = cartItem.Quantity + qty;
                }
                else
                {
                    cartItem = new CartDetail
                    {
                        ItemId = itemId,
                        ShoppingCartId = cart.Id,
                        Quantity = qty,
                        UnitPrice = item.Price
                    };
                    _db.CartDetails.Add(cartItem);
                }
                _db.SaveChanges();
                transaction.Commit();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            var cartItemCount = await GetCartItemCount(userId);
            return cartItemCount;
        }


        public async Task<int> RemoveItem(int itemId)
        {
            string userId = GetUserId();
            try
            {
                if (string.IsNullOrEmpty(userId))
                    throw new Exception("User invalid (maybe not logged in)");
                var cart = await GetCart(userId);
                if (cart is null)
                    throw new Exception("Cart error");
                // cart detail section
                var cartItem = _db.CartDetails.FirstOrDefault(a => a.ShoppingCartId == cart.Id && a.ItemId == itemId);
                if (cartItem is null)
                    throw new Exception("Empty cart");
                else if (cartItem.Quantity == 1)
                    _db.CartDetails.Remove(cartItem);
                else
                    cartItem.Quantity = cartItem.Quantity - 1;
                _db.SaveChanges();
            }
            catch (Exception)
            {
            }
            var cartItemCount = await GetCartItemCount(userId);
            return cartItemCount;
        }

        public async Task<int> GetCartItemCount(string userId = "")
        {
            if (string.IsNullOrEmpty(userId))
            {
                userId = GetUserId();
            }
            var totalQuantity = await _db.CartDetails
                .Where(cd => cd.ShoppingCart.UserId == userId)
                .SumAsync(cd => cd.Quantity);
            return totalQuantity;
        }

        public async Task<bool> DoCheckout()
        {
            using var transaction = _db.Database.BeginTransaction();
            try
            {
                // logic
                // move data from cartDetail to order and order detail then we will remove cart detail
                var userId = GetUserId();
                if (string.IsNullOrEmpty(userId))
                    throw new Exception("User invalid (maybe not logged in)");
                var cart = await GetCart(userId);
                if (cart is null)
                    throw new Exception("Cart error");
                var cartDetail = _db.CartDetails
                                    .Where(a => a.ShoppingCartId == cart.Id).ToList();
                if (cartDetail.Count == 0)
                    throw new Exception("Empty cart");
                var order = new Order
                {
                    UserId = userId,
                    CreateDate = DateTime.Now,
                    OrderStatusId = 1
                };
                _db.Orders.Add(order);
                _db.SaveChanges();
                foreach (var item in cartDetail)
                {
                    var orderDetail = new OrderDetail
                    {
                        ItemId = item.ItemId,
                        OrderId = order.Id,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice
                    };
                    _db.OrderDetails.Add(orderDetail);

                    var stock = _db.Stocks.FirstOrDefault(a => a.ItemId == item.ItemId);
                    if (stock is null)
                        throw new Exception("Stock not found");
                    if(stock.Quantity < item.Quantity)
                        throw new Exception("Stock not enough");
                    stock.Quantity = stock.Quantity - item.Quantity;
                }
                _db.SaveChanges();

                // removing the cartdetails
                _db.CartDetails.RemoveRange(cartDetail);
                _db.SaveChanges();
                transaction.Commit();
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        private string GetUserId()
        {
            var principal = _httpContextAccessor.HttpContext.User;
            string userId = _userMgr.GetUserId(principal);
            return userId;
        }
        public async Task<ShoppingCart> GetUserCart()
        {
            var userId = GetUserId();
            if (userId == null)
                throw new Exception("User invalid (maybe not logged in)");
            var shoppingCart = await _db.ShoppingCarts
                                  .Include(a => a.CartDetails)
                                  .ThenInclude(a => a.Item)
                                  .ThenInclude(a => a.Stock)
                                  .Include(a => a.CartDetails)
                                  .ThenInclude(a => a.Item)
                                  .ThenInclude(a => a.Category)
                                  .Where(a => a.UserId == userId).FirstOrDefaultAsync();
            return shoppingCart;

        }
        public async Task<ShoppingCart> GetCart(string userId)
        {
            var cart = await _db.ShoppingCarts.FirstOrDefaultAsync(x => x.UserId == userId);
            return cart;
        }
    }
}