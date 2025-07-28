using Microsoft.AspNetCore.Http;
using System;
using System.Security.Claims;

namespace DopamineStore.Services
{
    public class CartService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string CartSessionKey = "CartId";

        public CartService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetCartId()
        {
            var user = _httpContextAccessor.HttpContext?.User;

            if (user?.Identity?.IsAuthenticated ?? false)
            {
                return user.FindFirstValue(ClaimTypes.NameIdentifier);
            }

            string? cartId = _httpContextAccessor.HttpContext?.Request.Cookies[CartSessionKey];
            if (string.IsNullOrEmpty(cartId))
            {
                cartId = Guid.NewGuid().ToString();
                var cookieOptions = new CookieOptions
                {
                    IsEssential = true,
                    Expires = DateTime.Now.AddDays(30)
                };
                _httpContextAccessor.HttpContext?.Response.Cookies.Append(CartSessionKey, cartId, cookieOptions);
            }
            return cartId;
        }
    }
}