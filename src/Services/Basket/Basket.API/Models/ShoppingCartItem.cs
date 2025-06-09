namespace Basket.API.Models;

public class ShoppingCartItem
{
    public int Quantity { get; set; } = default!;

    public string Color { get; set; } = default!;

    public double Price { get; set; } = default!;

    public Guid ProductId { get; set; } = default!;

    public string ProductName { get; set; } = default!;
}