using eShopOnBlazor.Models;
using eShopOnBlazor.Models.Infrastructure;
using eShopOnBlazor.ViewModel;

namespace eShopOnBlazor.Services;

public class CatalogServiceMock : ICatalogService
{
    private readonly List<CatalogItem> _catalogItems;

    public CatalogServiceMock()
    {
        _catalogItems = new List<CatalogItem>(PreconfiguredData.GetPreconfiguredCatalogItems());
    }

    public PaginatedItemsViewModel<CatalogItem> GetCatalogItemsPaginated(int pageSize = 10, int pageIndex = 0)
    {
        var items = ComposeCatalogItems(_catalogItems);

        var itemsOnPage = items
            .OrderBy(c => c.Id)
            .Skip(pageSize * pageIndex)
            .Take(pageSize)
            .ToList();

        return new PaginatedItemsViewModel<CatalogItem>(
            pageIndex, pageSize, items.Count, itemsOnPage);
    }

    public CatalogItem? FindCatalogItem(int id)
    {
        return _catalogItems.FirstOrDefault(x => x.Id == id);
    }

    public IEnumerable<CatalogType> GetCatalogTypes()
    {
        return PreconfiguredData.GetPreconfiguredCatalogTypes();
    }

    public IEnumerable<CatalogBrand> GetCatalogBrands()
    {
        return PreconfiguredData.GetPreconfiguredCatalogBrands();
    }

    public void CreateCatalogItem(CatalogItem catalogItem)
    {
        var maxId = _catalogItems.Max(i => i.Id);
        catalogItem.Id = ++maxId;
        _catalogItems.Add(catalogItem);
    }

    public void UpdateCatalogItem(CatalogItem modifiedItem)
    {
        var originalItem = FindCatalogItem(modifiedItem.Id);
        if (originalItem != null)
        {
            _catalogItems[_catalogItems.IndexOf(originalItem)] = modifiedItem;
        }
    }

    public void RemoveCatalogItem(CatalogItem catalogItem)
    {
        _catalogItems.Remove(catalogItem);
    }

    private static List<CatalogItem> ComposeCatalogItems(List<CatalogItem> items)
    {
        var catalogTypes = PreconfiguredData.GetPreconfiguredCatalogTypes();
        var catalogBrands = PreconfiguredData.GetPreconfiguredCatalogBrands();
        items.ForEach(i => i.CatalogBrand = catalogBrands.First(b => b.Id == i.CatalogBrandId));
        items.ForEach(i => i.CatalogType = catalogTypes.First(b => b.Id == i.CatalogTypeId));

        return items;
    }
}
