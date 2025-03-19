using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.Json;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace FinanceApp;

public class DataManager
    {
        private readonly FinanceFacade facade;

        public DataManager(FinanceFacade facade)
        {
            this.facade = facade;
        }

        public void ExportToJson(string filePath)
        {
            var data = new DataContainer
            {
                Accounts = facade.GetBankAccounts().ToList(),
                Categories = facade.GetCategories().ToList(),
                Operations = facade.GetOperations().ToList()
            };
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(data, options);
            File.WriteAllText(filePath, json);
            Console.WriteLine($"Данные экспортированы в JSON: {filePath}");
        }

        public void ImportFromJson(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine("Файл не найден.");
                return;
            }
            string json = File.ReadAllText(filePath);
            var data = JsonSerializer.Deserialize<DataContainer>(json);
            if (data != null)
            {
                facade.LoadData(data.Accounts, data.Categories, data.Operations);
                Console.WriteLine("Данные импортированы из JSON.");
            }
        }

        public void ExportToYaml(string filePath)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Accounts:");
            foreach (var acc in facade.GetBankAccounts())
            {
                sb.AppendLine($"  - Id: {acc.Id}");
                sb.AppendLine($"    Name: {acc.Name}");
                sb.AppendLine($"    Balance: {acc.Balance}");
                sb.AppendLine($"    InitialBalance: {acc.InitialBalance}");
            }
            sb.AppendLine("Categories:");
            foreach (var cat in facade.GetCategories())
            {
                sb.AppendLine($"  - Id: {cat.Id}");
                sb.AppendLine($"    Type: {cat.Type}");
                sb.AppendLine($"    Name: {cat.Name}");
            }
            sb.AppendLine("Operations:");
            foreach (var op in facade.GetOperations())
            {
                sb.AppendLine($"  - Id: {op.Id}");
                sb.AppendLine($"    Type: {op.Type}");
                sb.AppendLine($"    BankAccountId: {op.BankAccountId}");
                sb.AppendLine($"    Amount: {op.Amount}");
                sb.AppendLine($"    Date: {op.Date}");
                sb.AppendLine($"    Description: {op.Description}");
                sb.AppendLine($"    CategoryId: {op.CategoryId}");
            }
            File.WriteAllText(filePath, sb.ToString());
            Console.WriteLine($"Данные экспортированы в YAML: {filePath}");
        }

        public void ImportFromYaml(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine("Файл не найден.");
                return;
            }

            string yaml = File.ReadAllText(filePath);
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(new NullNamingConvention()) 
                .WithTypeConverter(new CustomDateTimeConverter())
                .Build();
            try
            {
                var data = deserializer.Deserialize<DataContainer>(yaml);
                if (data != null)
                {
                    facade.LoadData(data.Accounts, data.Categories, data.Operations);
                    Console.WriteLine("Данные импортированы из YAML.");
                }
                else
                {
                    Console.WriteLine("Ошибка десериализации YAML.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при импорте YAML: " + ex.Message);
                Console.WriteLine(ex);
            }
        }

        public void ExportToCsv(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            StringBuilder sbAcc = new StringBuilder();
            sbAcc.AppendLine("Id,Name,Balance,InitialBalance");
            foreach (var acc in facade.GetBankAccounts())
            {
                sbAcc.AppendLine($"{acc.Id},{acc.Name},{acc.Balance},{acc.InitialBalance}");
            }
            File.WriteAllText(Path.Combine(directoryPath, "accounts.csv"), sbAcc.ToString());

            StringBuilder sbCat = new StringBuilder();
            sbCat.AppendLine("Id,Type,Name");
            foreach (var cat in facade.GetCategories())
            {
                sbCat.AppendLine($"{cat.Id},{cat.Type},{cat.Name}");
            }
            File.WriteAllText(Path.Combine(directoryPath, "categories.csv"), sbCat.ToString());

            StringBuilder sbOp = new StringBuilder();
            sbOp.AppendLine("Id,Type,BankAccountId,Amount,Date,Description,CategoryId");
            foreach (var op in facade.GetOperations())
            {
                sbOp.AppendLine($"{op.Id},{op.Type},{op.BankAccountId},{op.Amount},{op.Date},{op.Description},{op.CategoryId}");
            }
            File.WriteAllText(Path.Combine(directoryPath, "operations.csv"), sbOp.ToString());

            Console.WriteLine($"Данные экспортированы в CSV-файлы в папку: {directoryPath}");
        }

        public void ImportFromCsv(string directoryPath)
        {
            try
            {
                string accountsPath = Path.Combine(directoryPath, "accounts.csv");
                string categoriesPath = Path.Combine(directoryPath, "categories.csv");
                string operationsPath = Path.Combine(directoryPath, "operations.csv");

                if (!File.Exists(accountsPath) || !File.Exists(categoriesPath) || !File.Exists(operationsPath))
                {
                    Console.WriteLine("Один или несколько CSV файлов не найдены.");
                    return;
                }

                List<BankAccount> importedAccounts = new List<BankAccount>();
                List<Category> importedCategories = new List<Category>();
                List<Operation> importedOperations = new List<Operation>();

                var accLines = File.ReadAllLines(accountsPath).Skip(1);
                foreach (var line in accLines)
                {
                    var parts = line.Split(',');
                    if (parts.Length >= 4)
                    {
                        importedAccounts.Add(new BankAccount
                        {
                            Id = Guid.Parse(parts[0]),
                            Name = parts[1],
                            Balance = decimal.Parse(parts[2]),
                            InitialBalance = decimal.Parse(parts[3])
                        });
                    }
                }

                var catLines = File.ReadAllLines(categoriesPath).Skip(1);
                foreach (var line in catLines)
                {
                    var parts = line.Split(',');
                    if (parts.Length >= 3)
                    {
                        importedCategories.Add(new Category
                        {
                            Id = Guid.Parse(parts[0]),
                            Type = (CategoryType)Enum.Parse(typeof(CategoryType), parts[1]),
                            Name = parts[2]
                        });
                    }
                }

                var opLines = File.ReadAllLines(operationsPath).Skip(1);
                foreach (var line in opLines)
                {
                    var parts = line.Split(',');
                    if (parts.Length >= 7)
                    {
                        importedOperations.Add(new Operation
                        {
                            Id = Guid.Parse(parts[0]),
                            Type = (CategoryType)Enum.Parse(typeof(CategoryType), parts[1]),
                            BankAccountId = Guid.Parse(parts[2]),
                            Amount = decimal.Parse(parts[3]),
                            Date = DateTime.Parse(parts[4]),
                            Description = parts[5],
                            CategoryId = Guid.Parse(parts[6])
                        });
                    }
                }

                facade.LoadData(importedAccounts, importedCategories, importedOperations);
                Console.WriteLine("Данные импортированы из CSV.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при импорте CSV: " + ex.Message);
            }
        }
    }
