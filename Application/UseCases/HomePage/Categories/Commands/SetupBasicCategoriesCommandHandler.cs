using Application.Common.Interfaces;

namespace Application.UseCases.HomePage.Categories.Commands;

public class SetupBasicCategoriesCommandHandler(
    ICategoriesRepository categoriesRepository,
    ICurrentUserService currentUserService)
{
    private readonly ICategoriesRepository _categoriesRepository = categoriesRepository;
    private readonly ICurrentUserService _currentUserService = currentUserService;
    
    
}