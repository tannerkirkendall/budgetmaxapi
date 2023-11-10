using Application.Interfaces;
using Infrastructure;
using MediatR;

namespace Application.HomePage.Queries.Categories;

public class GetCategoriesAndSubCategoriesHandler : IRequestHandler<GetCategoriesAndSubCategoriesRequest, GetCategoriesAndSubCategoriesResult>
{
    private readonly IRepository _repo;
    private readonly ICurrentUserService _userService;

    public GetCategoriesAndSubCategoriesHandler(IRepository repo, ICurrentUserService userService)
    {
        _repo = repo;
        _userService = userService;
    }
    
    public async Task<GetCategoriesAndSubCategoriesResult> Handle(GetCategoriesAndSubCategoriesRequest request, CancellationToken cancellationToken)
    {
        var categories = await _repo.GetCategoriesByAccountId(_userService.AccountId);
        var subCategories = await _repo.GetSubCategoriesByAccountId(_userService.AccountId);
        var subCategoriesList = subCategories.ToList();

        var returnObject = new GetCategoriesAndSubCategoriesResult();
        foreach (var c in categories)
        {
            var thisSubCats = subCategoriesList
                .Where(x => x.CategoryId == c.CategoryId)
                .Select(sc => new GetCategoriesAndSubCategoriesResult.SubCategory {Id = sc.SubCategoryId, Name = sc.SubCategoryName}).ToList();

            var cat = new GetCategoriesAndSubCategoriesResult.Category
            {
                Id = c.CategoryId,
                Name = c.CategoryName,
                SubCategories = thisSubCats
            };
            returnObject.Categories.Add(cat);
        }
        
        return returnObject;
    }
}

public class GetCategoriesAndSubCategoriesRequest : IRequest<GetCategoriesAndSubCategoriesResult>
{
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