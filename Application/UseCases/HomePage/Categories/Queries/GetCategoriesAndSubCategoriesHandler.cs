using Application.Common.Interfaces;
using MediatR;

namespace Application.UseCases.HomePage.Categories.Queries;

public class GetCategoriesAndSubCategoriesHandler(ICategoriesRepository repo, ICurrentUserService userService)
    : IRequestHandler<GetCategoriesAndSubCategoriesQuery, GetCategoriesAndSubCategoriesResult>
{
    public async Task<GetCategoriesAndSubCategoriesResult> Handle(GetCategoriesAndSubCategoriesQuery query, CancellationToken cancellationToken)
    {
        var categories = (await repo.GetCategories(userService.AccountId)).ToList();
        var subCategories = await repo.GetSubCategories(userService.AccountId);
        var subCategoriesList = subCategories.ToList();

        var returnObject = new GetCategoriesAndSubCategoriesResult();

        foreach (var subCategory in subCategoriesList)
        {
            returnObject.Categories.Add(new GetCategoriesAndSubCategoriesResult.SubCategory
            {
                SubCategoryId = subCategory.SubCategoryId,
                SubCategoryName = subCategory.SubCategoryName,
                CategoryName = categories.First(x => x.CategoryId == subCategory.CategoryId).CategoryName,
                CategoryId = subCategory.CategoryId
            });
        }
        
        return returnObject;
    }
}

public class GetCategoriesAndSubCategoriesQuery : IRequest<GetCategoriesAndSubCategoriesResult>
{
}

public class GetCategoriesAndSubCategoriesResult
{
    public List<SubCategory> Categories { get; set; } = new();
    public class SubCategory
    {
        public int SubCategoryId { get; set; }
        public string SubCategoryName { get; set; } = "";
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = "";
    }

}