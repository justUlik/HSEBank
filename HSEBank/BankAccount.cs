namespace FinanceApp;

public class BankAccount
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public decimal Balance { get; set; }
    public decimal InitialBalance { get; set; }

    public BankAccount(string name, decimal initialBalance)
    {
        Id = Guid.NewGuid();
        Name = name;
        Balance = initialBalance;
        InitialBalance = initialBalance;
    }

    public BankAccount() { }

    public void UpdateBalance(decimal amount)
    {
        Balance += amount;
    }

    public void SetBalance(decimal newBalance)
    {
        Balance = newBalance;
    }

    public override string ToString()
    {
        return $"[ID: {Id}] {Name} - Баланс: {Balance}";
    }
}