using YamlDotNet.Serialization;

namespace FinanceApp;

public class DataContainer
{
    [YamlMember(Alias = "Accounts")]
    public List<BankAccount> Accounts { get; set; }

    [YamlMember(Alias = "Categories")]
    public List<Category> Categories { get; set; }

    [YamlMember(Alias = "Operations")]
    public List<Operation> Operations { get; set; }
}