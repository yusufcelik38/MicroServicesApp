using Basket.Api.Data.Interfaces;
using Basket.Api.Entities;
using Basket.Api.Repository.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Basket.Api.Repository
{
    public class BasketRepository : IBasketRepository
    {
        private readonly IBasketContext _context;

        public BasketRepository(IBasketContext context)
        {
            _context = context;
        }

        public async Task<bool> DeleteBasket(string userName)
        {
            return await _context
                .Redis
                .KeyDeleteAsync(userName);
        }

        public async Task<BasketCart> GetBasket(string userName)
        {
            var basket = await _context
                 .Redis
                 .StringGetAsync(userName);
            if (basket.IsNullOrEmpty)
            {
                return null;
            }
            return JsonConvert.DeserializeObject<BasketCart>(basket);
        }

        public async Task<BasketCart> UpdateBasket(BasketCart basket)
        {
            var updated = await _context
                .Redis
                .StringSetAsync(basket.UserName, JsonConvert.SerializeObject(basket));
            return updated ? await GetBasket(basket.UserName) : null;
        }
    }
}
