﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Shop.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartRepo _cartRepo;
        public CartController(ICartRepo cartRepo)
        {
            _cartRepo = cartRepo;
        }

        public async Task<IActionResult> AddItem(int itemId, int qty = 1, int redirect = 0)
        {
            try
            {
                var cartCount = await _cartRepo.AddItem(itemId, qty);
                TempData["successMessage"] = "Added to cart";
                if (redirect == 0)
                    return Ok(cartCount);
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction("GetUserCart");

        }

        public async Task<IActionResult> RemoveItem(int itemId)
        {
            var cartCount = await _cartRepo.RemoveItem(itemId);
            return RedirectToAction("GetUserCart");
        }
        public async Task<IActionResult> GetUserCart()
        {
            var cart = await _cartRepo.GetUserCart();
            return View(cart);
        }

        public async Task<IActionResult> GetTotalItemInCart()
        {
            int cartItem = await _cartRepo.GetCartItemCount();
            return Ok(cartItem);
        }

        public async Task<IActionResult> Checkout()
        {
            bool isCheckedOut = await _cartRepo.DoCheckout();
            if (!isCheckedOut)
                throw new Exception("Error: not checked out!");
            return RedirectToAction("Index", "Home");
        }
    }
}
