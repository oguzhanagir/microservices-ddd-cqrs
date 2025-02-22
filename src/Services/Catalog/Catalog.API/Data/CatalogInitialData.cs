using Marten.Schema;

namespace Catalog.API.Data;

public class CatalogInitialData : IInitialData
{
    public async Task Populate(IDocumentStore store, CancellationToken cancellation)
    {
        using var session = store.LightweightSession();

        if (await session.Query<Product>().AnyAsync())
            return;

        session.Store<Product>(GetPreConfiguredProducts());
        await session.SaveChangesAsync();
    }

    private static IEnumerable<Product> GetPreConfiguredProducts() => new List<Product>
    {
        new Product()
        {
            Id = Guid.NewGuid(),
            Name = "Product 1",
            Description = "Product 1 Description",
            ImageFile = "image-product.png",
            Price = 1000,
            Category = new List<string>{ "Test Product" }
        }
    };
}
