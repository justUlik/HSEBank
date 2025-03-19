namespace FinanceApp;

public class FinanceFacade
    {
        private List<BankAccount> accounts = new List<BankAccount>();
        private List<Category> categories = new List<Category>();
        private List<Operation> operations = new List<Operation>();

        public BankAccount CreateBankAccount(string name, decimal initialBalance)
        {
            var account = DomainFactory.CreateBankAccount(name, initialBalance);
            accounts.Add(account);
            return account;
        }

        public IEnumerable<BankAccount> GetBankAccounts()
        {
            return accounts;
        }

        public void UpdateBankAccount(Guid id, string newName)
        {
            var account = accounts.FirstOrDefault(a => a.Id == id);
            if (account != null)
            {
                account.Name = newName;
            }
        }

        public void DeleteBankAccount(Guid id)
        {
            accounts.RemoveAll(a => a.Id == id);
        }

        public Category CreateCategory(CategoryType type, string name)
        {
            var category = DomainFactory.CreateCategory(type, name);
            categories.Add(category);
            return category;
        }

        public IEnumerable<Category> GetCategories()
        {
            return categories;
        }

        public void UpdateCategory(Guid id, string newName)
        {
            var category = categories.FirstOrDefault(c => c.Id == id);
            if (category != null)
            {
                category.Name = newName;
            }
        }

        public void DeleteCategory(Guid id)
        {
            categories.RemoveAll(c => c.Id == id);
        }

        public Operation CreateOperation(CategoryType type, Guid bankAccountId, decimal amount, DateTime date, Guid categoryId, string description = "")
        {
            var operation = DomainFactory.CreateOperation(type, bankAccountId, amount, date, categoryId, description);
            operations.Add(operation);

            var account = accounts.FirstOrDefault(a => a.Id == bankAccountId);
            if (account != null)
            {
                if (type == CategoryType.Income)
                    account.UpdateBalance(amount);
                else if (type == CategoryType.Expense)
                    account.UpdateBalance(-amount);
            }

            return operation;
        }

        public IEnumerable<Operation> GetOperations()
        {
            return operations;
        }

        public void DeleteOperation(Guid id)
        {
            operations.RemoveAll(o => o.Id == id);
        }

        public decimal GetIncomeExpenseDifference(DateTime start, DateTime end)
        {
            decimal income = operations.Where(o => o.Date >= start && o.Date <= end && o.Type == CategoryType.Income).Sum(o => o.Amount);
            decimal expense = operations.Where(o => o.Date >= start && o.Date <= end && o.Type == CategoryType.Expense).Sum(o => o.Amount);
            return income - expense;
        }

        public Dictionary<string, decimal> GroupOperationsByCategory(Guid bankAccountId)
        {
            var result = operations
                .Where(o => o.BankAccountId == bankAccountId)
                .GroupBy(o => categories.FirstOrDefault(c => c.Id == o.CategoryId)?.Name ?? "Неизвестно")
                .ToDictionary(g => g.Key, g => g.Sum(o => o.Amount));
            return result;
        }

        public Dictionary<string, (decimal TotalIncome, decimal TotalExpense)> GetMonthlySummary()
        {
            var summary = operations
                .GroupBy(o => o.Date.ToString("yyyy-MM"))
                .ToDictionary(
                    g => g.Key,
                    g => (
                        TotalIncome: g.Where(o => o.Type == CategoryType.Income).Sum(o => o.Amount),
                        TotalExpense: g.Where(o => o.Type == CategoryType.Expense).Sum(o => o.Amount)
                    )
                );
            return summary;
        }

        public void RecalculateBalances()
        {
            foreach (var account in accounts)
            {
                decimal sumOps = operations
                    .Where(o => o.BankAccountId == account.Id)
                    .Sum(o => o.Type == CategoryType.Income ? o.Amount : -o.Amount);
                account.SetBalance(account.InitialBalance + sumOps);
            }
            Console.WriteLine("Балансы пересчитаны.");
        }

        public void LoadData(List<BankAccount> newAccounts, List<Category> newCategories, List<Operation> newOperations)
        {
            accounts = newAccounts;
            categories = newCategories;
            operations = newOperations;
        }
    }
