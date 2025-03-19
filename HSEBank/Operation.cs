namespace FinanceApp;

public class Operation
{
    public Guid Id { get; set; }
    public CategoryType Type { get; set; }
    public Guid BankAccountId { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string Description { get; set; }
    public Guid CategoryId { get; set; }

    public Operation(CategoryType type, Guid bankAccountId, decimal amount, DateTime date, Guid categoryId, string description = "")
    {
        if (amount < 0)
        {
            throw new ArgumentException("Сумма операции должна быть неотрицательной.");    
        }

        Id = Guid.NewGuid();
        Type = type;
        BankAccountId = bankAccountId;
        Amount = amount;
        Date = date;
        CategoryId = categoryId;
        Description = description;
    }

    public Operation() { }

    public override string ToString()
    {
        return $"[ID: {Id}] {Type} {Amount} от {Date.ToShortDateString()}, Описание: {Description}";
    }
}