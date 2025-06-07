
namespace Basket.API.Data;

public class BasketRepository(IDocumentSession session) : IBasketRepository
{
    public async Task<ShoppingCart> GetAsync(string userName, CancellationToken cancellationToken = default)
    {
        var shoppingCart = await session.LoadAsync<ShoppingCart>(userName, cancellationToken);
        return shoppingCart is null ? throw new BasketNotFoundException(userName) : shoppingCart;
    }

    public async Task<ShoppingCart> StoreAsync(ShoppingCart shoppingCart, CancellationToken cancellationToken = default)
    {
        session.Store(shoppingCart);
        await session.SaveChangesAsync(cancellationToken);
        return shoppingCart;
    }

    public async Task<bool> DeleteAsync(string userName, CancellationToken cancellationToken = default)
    {
        var shoppingCart = await session.LoadAsync<ShoppingCart>(userName, cancellationToken);
        if (shoppingCart is null)
        {
            throw new BasketNotFoundException(userName);
        }

        try
        {
            session.Delete(shoppingCart);
            await session.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
