namespace FinanceApp;

public static class DomainFactory
{
    public static BankAccount CreateBankAccount(string name, decimal initialBalance)
    {
        return new BankAccount(name, initialBalance);
    }

    public static Category CreateCategory(CategoryType type, string name)
    {
        return new Category(type, name);
    }

    public static Operation CreateOperation(CategoryType type, Guid bankAccountId, decimal amount, DateTime date, Guid categoryId, string description = "")
    {
        return new Operation(type, bankAccountId, amount, date, categoryId, description);
    }
}