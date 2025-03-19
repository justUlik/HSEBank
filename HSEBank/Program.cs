using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.Json;

namespace FinanceApp
{
    class Program
    {
        static void Main(string[] args)
        {
            FinanceFacade facade = new FinanceFacade();
            DataManager dataManager = new DataManager(facade);
            
            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("\nМеню:");
                Console.WriteLine("1. Создать счет");
                Console.WriteLine("2. Создать категорию");
                Console.WriteLine("3. Создать операцию");
                Console.WriteLine("4. Показать разницу доходов и расходов");
                Console.WriteLine("5. Показать счета");
                Console.WriteLine("6. Показать категории");
                Console.WriteLine("7. Показать операции");
                Console.WriteLine("8. Выход");
                Console.WriteLine("9. Экспорт данных");
                Console.WriteLine("10. Импорт данных");
                Console.WriteLine("11. Пересчитать балансы");
                Console.WriteLine("12. Показать месячную аналитику");
                Console.WriteLine("13. Показать группировку счета по категориям");
                Console.Write("Выберите пункт: ");

                string input = Console.ReadLine();
                Console.WriteLine();
                switch (input)
                {
                    case "1":
                        Console.Write("Введите название счета: ");
                        string accountName = Console.ReadLine();
                        Console.Write("Введите начальный баланс: ");
                        if (decimal.TryParse(Console.ReadLine(), out decimal initBalance))
                        {
                            var newAccount = facade.CreateBankAccount(accountName, initBalance);
                            Console.WriteLine("Счет создан: " + newAccount);
                        }
                        else
                        {
                            Console.WriteLine("Некорректный баланс.");
                        }

                        break;
                    case "2":
                        Console.Write("Введите название категории: ");
                        string catName = Console.ReadLine();
                        Console.Write("Введите тип категории (1 - Доход, 2 - Расход): ");
                        string typeInput = Console.ReadLine();
                        CategoryType catType = typeInput == "1" ? CategoryType.Income : CategoryType.Expense;
                        var newCategory = facade.CreateCategory(catType, catName);
                        Console.WriteLine("Категория создана: " + newCategory);
                        break;
                    case "3":
                        var categories = facade.GetCategories();
                        var curBankAccounts = facade.GetBankAccounts();
                        if (!categories.Any())
                        {
                            Console.WriteLine("Пока вы не создали ни одного счета.");
                            break;
                        }

                        if (!curBankAccounts.Any())
                        {
                            Console.WriteLine("Пока вы не создали ни одной категории.");
                            break;
                        }
                        Console.WriteLine("Доступные счета:");
                        foreach (var acc in curBankAccounts)
                        {
                            Console.WriteLine(acc);
                        }

                        Console.Write("Введите ID счета: ");
                        if (!Guid.TryParse(Console.ReadLine(), out Guid accId))
                        {
                            Console.WriteLine("Некорректный ID.");
                            break;
                        }

                        Console.WriteLine("Доступные категории:");
                        foreach (var cat in categories)
                        {
                            Console.WriteLine(cat);
                        }

                        Console.Write("Введите ID категории: ");
                        if (!Guid.TryParse(Console.ReadLine(), out Guid catId))
                        {
                            Console.WriteLine("Некорректный ID.");
                            break;
                        }

                        Console.Write("Введите сумму операции: ");
                        if (!decimal.TryParse(Console.ReadLine(), out decimal amount))
                        {
                            Console.WriteLine("Некорректная сумма.");
                            break;
                        }

                        Console.Write("Введите описание: ");
                        string desc = Console.ReadLine();

                        Console.Write("Введите тип операции (1 - Доход, 2 - Расход): ");
                        string opTypeInput = Console.ReadLine();
                        CategoryType opType = opTypeInput == "1" ? CategoryType.Income : CategoryType.Expense;

                        try
                        {
                            ICommand createOpCommand =
                                new CreateOperationCommand(facade, opType, accId, amount, DateTime.Now, catId, desc);
                            ICommand timedCommand = new TimedCommandDecorator(createOpCommand);
                            timedCommand.Execute();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        
                        break;
                    case "4":
                        Console.Write("Введите начальную дату (формат ГГГГ-ММ-ДД): ");
                        if (!DateTime.TryParse(Console.ReadLine(), out DateTime startDate))
                        {
                            Console.WriteLine("Некорректная дата.");
                            break;
                        }

                        Console.Write("Введите конечную дату (формат ГГГГ-ММ-ДД): ");
                        if (!DateTime.TryParse(Console.ReadLine(), out DateTime endDate))
                        {
                            Console.WriteLine("Некорректная дата.");
                            break;
                        }

                        decimal diff = facade.GetIncomeExpenseDifference(startDate, endDate);
                        Console.WriteLine($"Разница между доходами и расходами: {diff}");
                        break;
                    case "5":
                        Console.WriteLine("Счета:");
                        foreach (var acc in facade.GetBankAccounts())
                        {
                            Console.WriteLine(acc);
                        }

                        break;
                    case "6":
                        Console.WriteLine("Категории:");
                        foreach (var cat in facade.GetCategories())
                        {
                            Console.WriteLine(cat);
                        }

                        break;
                    case "7":
                        Console.WriteLine("Операции:");
                        foreach (var op in facade.GetOperations())
                        {
                            Console.WriteLine(op);
                        }

                        break;
                    case "8":
                        exit = true;
                        break;
                    case "9":
                        Console.WriteLine("Выберите формат экспорта: 1 - JSON, 2 - CSV, 3 - YAML");
                        string exportChoice = Console.ReadLine();
                        if (exportChoice == "1")
                        {
                            Console.Write("Введите путь к файлу (например, data.json): ");
                            string jsonPath = Console.ReadLine();
                            dataManager.ExportToJson(jsonPath);
                        }
                        else if (exportChoice == "2")
                        {
                            Console.Write("Введите путь к папке для CSV файлов (data_folder): ");
                            string csvDir = Console.ReadLine();
                            dataManager.ExportToCsv(csvDir);
                        }
                        else if (exportChoice == "3")
                        {
                            Console.Write("Введите путь к файлу (например, data.yaml): ");
                            string yamlPath = Console.ReadLine();
                            dataManager.ExportToYaml(yamlPath);
                        }
                        else
                        {
                            Console.WriteLine("Неверный выбор.");
                        }

                        break;
                    case "10":
                        Console.WriteLine("Выберите формат импорта: 1 - JSON, 2 - CSV, 3 - YAML");
                        string importChoice = Console.ReadLine();
                        if (importChoice == "1")
                        {
                            Console.Write("Введите путь к файлу (например, data.json): ");
                            string jsonPath = Console.ReadLine();
                            dataManager.ImportFromJson(jsonPath);
                        }
                        else if (importChoice == "2")
                        {
                            Console.Write("Введите путь к папке с CSV файлами: ");
                            string csvDir = Console.ReadLine();
                            dataManager.ImportFromCsv(csvDir);
                        }
                        else if (importChoice == "3")
                        {
                            Console.Write("Введите путь к файлу (например, data.yaml): ");
                            string yamlPath = Console.ReadLine();
                            dataManager.ImportFromYaml(yamlPath);
                        }
                        else
                        {
                            Console.WriteLine("Неверный выбор.");
                        }

                        break;
                    case "11":
                        facade.RecalculateBalances();
                        break;
                    case "12":
                        var monthlySummary = facade.GetMonthlySummary();
                        Console.WriteLine("Месячная аналитика:");
                        foreach (var item in monthlySummary)
                        {
                            Console.WriteLine(
                                $"Месяц: {item.Key} | Доход: {item.Value.TotalIncome} | Расход: {item.Value.TotalExpense}");
                        }

                        break;
                    case "13":
                        Console.WriteLine("Введите ID счета для группировки операций по категориям:");
                        if (!Guid.TryParse(Console.ReadLine(), out Guid groupAccId))
                        {
                            Console.WriteLine("Некорректный ID.");
                            break;
                        }
                        var grouping = facade.GroupOperationsByCategory(groupAccId);
                        Console.WriteLine("Группировка операций по категориям:");
                        foreach (var kvp in grouping)
                        {
                            Console.WriteLine($"Категория: {kvp.Key} - Сумма: {kvp.Value}");
                        }
                        break;
                    default:
                        Console.WriteLine("Неверный выбор.");
                        break;
                }
            }

            Console.WriteLine("Завершение работы.");
        }
    }
}