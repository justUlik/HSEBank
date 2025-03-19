namespace FinanceApp;

public class Category
{
    public Guid Id { get; set; }
    public CategoryType Type { get; set; }
    public string Name { get; set; }

    public Category(CategoryType type, string name)
    {
        Id = Guid.NewGuid();
        Type = type;
        Name = name;
    }

    public Category() { }

    public override string ToString()
    {
        return $"[ID: {Id}] {Name} ({Type})";
    }
}
