using eShopOnBlazor.Models;
using eShopOnBlazor.ViewModel;
using System.Data.Entity;

namespace eShopOnBlazor.Services;

public class CatalogService : ICatalogService
{
    private readonly CatalogDBContext _dbContext;
    private readonly CatalogItemHiLoGenerator _indexGenerator;

    public CatalogService(CatalogDBContext db, CatalogItemHiLoGenerator indexGenerator)
    {
        _dbContext = db;
        _indexGenerator = indexGenerator;
    }

    public PaginatedItemsViewModel<CatalogItem> GetCatalogItemsPaginated(int pageSize, int pageIndex)
    {
        var totalItems = _dbContext.CatalogItems.LongCount();

        var itemsOnPage = _dbContext.CatalogItems
            .Include(c => c.CatalogBrand)
            .Include(c => c.CatalogType)
            .OrderBy(c => c.Id)
            .Skip(pageSize * pageIndex)
            .Take(pageSize)
            .ToList();

        return new PaginatedItemsViewModel<CatalogItem>(
            pageIndex, pageSize, totalItems, itemsOnPage);
    }

    public CatalogItem? FindCatalogItem(int id)
    {
        return _dbContext.CatalogItems.Include(c => c.CatalogBrand).Include(c => c.CatalogType).FirstOrDefault(ci => ci.Id == id);
    }
    public IEnumerable<CatalogType> GetCatalogTypes()
    {
        return _dbContext.CatalogTypes.ToList();
    }

    public IEnumerable<CatalogBrand> GetCatalogBrands()
    {
        return _dbContext.CatalogBrands.ToList();
    }

    public void CreateCatalogItem(CatalogItem catalogItem)
    {
        catalogItem.Id = _indexGenerator.GetNextSequenceValue(_dbContext);
        _dbContext.CatalogItems.Add(catalogItem);
        _dbContext.SaveChanges();
    }

    public void UpdateCatalogItem(CatalogItem catalogItem)
    {
        _dbContext.Entry(catalogItem).State = EntityState.Modified;
        _dbContext.SaveChanges();
    }

    public void RemoveCatalogItem(CatalogItem catalogItem)
    {
        _dbContext.CatalogItems.Remove(catalogItem);
        _dbContext.SaveChanges();
    }
}
