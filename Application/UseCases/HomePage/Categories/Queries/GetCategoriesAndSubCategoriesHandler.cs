using Application.Common.Interfaces;
using MediatR;

namespace Application.UseCases.HomePage.Categories.Queries;

public class GetCategoriesAndSubCategoriesHandler(ICategoriesRepository repo, ICurrentUserService userService)
    : IRequestHandler<GetCategoriesAndSubCategoriesQuery, GetCategoriesAndSubCategoriesResult>
{
    public async Task<GetCategoriesAndSubCategoriesResult> Handle(GetCategoriesAndSubCategoriesQuery query, CancellationToken cancellationToken)
    {
        var categories = await repo.GetCategories(userService.AccountId);
        var subCategories = await repo.GetSubCategories(userService.AccountId);
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

public class GetCategoriesAndSubCategoriesQuery : IRequest<GetCategoriesAndSubCategoriesResult>
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