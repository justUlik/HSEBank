namespace FinanceApp;

public class CreateOperationCommand : ICommand
{
    private readonly FinanceFacade _facade;
    private readonly CategoryType _type;
    private readonly Guid _bankAccountId;
    private readonly decimal _amount;
    private readonly DateTime _date;
    private readonly Guid _categoryId;
    private readonly string _description;

    public CreateOperationCommand(FinanceFacade facade, CategoryType type, Guid bankAccountId, decimal amount, DateTime date, Guid categoryId, string description = "")
    {
        _facade = facade;
        _type = type;
        _bankAccountId = bankAccountId;
        _amount = amount;
        _date = date;
        _categoryId = categoryId;
        _description = description;
    }

    public void Execute()
    {
        _facade.CreateOperation(_type, _bankAccountId, _amount, _date, _categoryId, _description);
        Console.WriteLine("Операция успешно создана.");
    }
}