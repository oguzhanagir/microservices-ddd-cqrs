
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Basket.API.Data;

public class CachedBasketRepository(IBasketRepository repository, IDistributedCache cache) : IBasketRepository
{
    public async Task<ShoppingCart> GetAsync(string userName, CancellationToken cancellationToken = default)
    {
        var cacheBasket = await cache.GetStringAsync(userName, cancellationToken);

        if (!string.IsNullOrEmpty(cacheBasket))
        {
            var cachedCart = JsonSerializer.Deserialize<ShoppingCart>(cacheBasket);
            if (cachedCart != null)
                return cachedCart;
        }

        var shoppingCart = await repository.GetAsync(userName, cancellationToken);

        var serializedCart = JsonSerializer.Serialize(shoppingCart);
        await cache.SetStringAsync(userName, serializedCart, cancellationToken);

        return shoppingCart;
    }

    public async Task<ShoppingCart> StoreAsync(ShoppingCart shoppingCart, CancellationToken cancellationToken = default)
    {
        await repository.StoreAsync(shoppingCart, cancellationToken);

        await cache.SetStringAsync(shoppingCart.UserName, JsonSerializer.Serialize(shoppingCart), cancellationToken);

        return shoppingCart;
    }

    public async Task<bool> DeleteAsync(string userName, CancellationToken cancellationToken = default)
    {
        var deletedFromRepo = await repository.DeleteAsync(userName, cancellationToken);

        if (deletedFromRepo)
        {
            await cache.RemoveAsync(userName, cancellationToken);
        }

        return deletedFromRepo;
    }
}
