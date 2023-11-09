using Infrastructure;
using MediatR;

namespace Application.HomePage.Queries.Categories;

public class GetCategoriesAndSubCategoriesHandler : IRequestHandler<GetCategoriesAndSubCategoriesRequest, GetCategoriesAndSubCategoriesResult>
{
    private readonly IRepository _repo;

    public GetCategoriesAndSubCategoriesHandler(IRepository repo)
    {
        _repo = repo;
    }
    
    public async Task<GetCategoriesAndSubCategoriesResult> Handle(GetCategoriesAndSubCategoriesRequest request, CancellationToken cancellationToken)
    {
        var categories = await _repo.GetCategoriesByAccountId(request.AccountId);
        var subCategories = await _repo.GetSubCategoriesByAccountId(request.AccountId);
        var subCategoriesList = subCategories.ToList();

        var returnObject = new GetCategoriesAndSubCategoriesResult();
        foreach (var c in categories)
        {
            var thisSubCats = subCategoriesList.Where(x => x.CategoryId == c.CategoryId);
            var ddd = thisSubCats.Select(sc => new GetCategoriesAndSubCategoriesResult.SubCategory {Id = sc.SubCategoryId, Name = sc.SubCategoryName}).ToList();

            var cat = new GetCategoriesAndSubCategoriesResult.Category
            {
                Id = c.CategoryId,
                Name = c.CategoryName,
                SubCategories = ddd
            };
            returnObject.Categories.Add(cat);
        }
        
        return returnObject;
    }
}

public class GetCategoriesAndSubCategoriesRequest : IRequest<GetCategoriesAndSubCategoriesResult>
{
    public GetCategoriesAndSubCategoriesRequest(int accountId)
    {
        AccountId = accountId;
    }
    public int AccountId { get; }
}

public class GetCategoriesAndSubCategoriesResult
{
    public List<Category> Categories { get; set; } = new();
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public List<SubCategory> SubCategories { get; set; } = new();
    }
    public class SubCategory
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
    }
}