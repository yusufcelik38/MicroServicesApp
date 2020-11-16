using Basket.Api.Data.Interfaces;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Basket.Api.Data
{
    public class BasketContext : IBasketContext
    {
        private readonly ConnectionMultiplexer _redisConnection;
        public IDatabase Redis { get; }

        public BasketContext(ConnectionMultiplexer redisConnection)
        {
            _redisConnection = redisConnection;
            Redis = _redisConnection.GetDatabase();
        }

    }
}
