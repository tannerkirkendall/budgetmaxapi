using Domain;

namespace Application.Common.Interfaces;

public interface ICategoriesRepository
{ 
    Task<int> CreateNewCategory(int accountId, string categoryName);
    Task<int> CreateNewSubCategory(int accountId, int categoryId, string subCategoryName);
    Task<IEnumerable<Category>> GetCategories(int accountId);
    Task<IEnumerable<SubCategory>> GetSubCategories(int accountId);
}