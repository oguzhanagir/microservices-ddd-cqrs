namespace Basket.API.Data;

public interface IBasketRepository
{
    Task<ShoppingCart> GetAsync(string userName, CancellationToken cancellationToken = default);
    Task<ShoppingCart> StoreAsync(ShoppingCart shoppingCart, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string userName, CancellationToken cancellationToken = default);
}
