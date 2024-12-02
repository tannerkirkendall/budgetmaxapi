using Application.Common.Interfaces;
using Domain;
using MediatR;

namespace Application.UseCases.HomePage.Transactions.Queries;

public class GetTransactionSummaryQueryHandler(ICurrentUserService userService, ITransactionRepository transactionRepository, ICategoriesRepository categoriesRepository) 
    : IRequestHandler<GetTransactionSummaryQuery, GetTransactionSummaryResult>
{
    public async Task<GetTransactionSummaryResult> Handle(GetTransactionSummaryQuery request, CancellationToken cancellationToken)
    {
        var accountId = userService.AccountId;
        var transactions = await transactionRepository.GetTransactionsByAccountId(accountId);
        var categories = (await categoriesRepository.GetCategories(accountId)).ToList();
        var subCategories = (await categoriesRepository.GetSubCategories(accountId)).ToList();
        
        var resultSet = new List<GetTransactionSummaryResult.CategorySummary>();

        foreach (var transaction in transactions)
        {
            var subCategoryLookup = subCategories.First(x => x.SubCategoryId == transaction.SubCategoryId);
            var categoryExists = resultSet.SingleOrDefault(x => x.CategoryId == subCategoryLookup.CategoryId);
            
            if (categoryExists == null)
            {
                resultSet.Add(new GetTransactionSummaryResult.CategorySummary
                {
                    Category = categories.First(x => x.CategoryId == subCategoryLookup.CategoryId).CategoryName,
                    CategoryId = subCategoryLookup.CategoryId,
                    BudgetAmount = 0,
                    BudgetRemaining = 0 - transaction.Amount,
                    SubCategoriesSummary =
                    [
                        new()
                        {
                            SubCategory = subCategoryLookup.SubCategoryName,
                            SubCategoryId = subCategoryLookup.SubCategoryId,
                            BudgetAmount = 0,
                            BudgetRemaining = 0,
                            AmountSpent = transaction.Amount
                        }
                    ]

                });
            }
            else
            {
                var subCategoryExists = categoryExists.SubCategoriesSummary.SingleOrDefault(x => x.SubCategoryId == transaction.SubCategoryId);
                if (subCategoryExists == null)
                {
                    categoryExists.SubCategoriesSummary.Add(new GetTransactionSummaryResult.SubCategorySummary()
                    {
                        SubCategory = subCategoryLookup.SubCategoryName,
                        SubCategoryId = subCategoryLookup.SubCategoryId,
                        BudgetAmount = 0,
                        BudgetRemaining = 0,
                        AmountSpent = transaction.Amount
                    });
                }
                else
                {
                    subCategoryExists.AmountSpent += transaction.Amount;
                }
            }
            

        }

        
        return new GetTransactionSummaryResult
        {
            CategoriesSummary = resultSet
        };
    }
}


public class GetTransactionSummaryQuery : IRequest<GetTransactionSummaryResult>
{
    
}


public class GetTransactionSummaryResult
{
    public List<CategorySummary> CategoriesSummary { get; set; } = new();
    

    public class CategorySummary
    {
        public string Category { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public decimal BudgetAmount { get; set; }
        public decimal BudgetRemaining { get; set; }
        public List<SubCategorySummary> SubCategoriesSummary { get; set; } = new();
        
    }

    public class SubCategorySummary
    {
        public string SubCategory { get; set; } = string.Empty;
        public int SubCategoryId { get; set; }
        public decimal BudgetAmount { get; set; }
        public decimal AmountSpent { get; set; }
        public decimal BudgetRemaining { get; set; }
    }
}