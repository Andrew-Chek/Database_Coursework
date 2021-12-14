using static System.Console;
using consoleApp;
namespace generation
{
    public class ConsoleLog
    {
        private Generation generation;
        public ConsoleLog(Generation generation)
        {
            this.generation = generation;
        }
        public void ProcessCommands()
        {
            while (true)
            {
                Write("Enter a command: ");
                string command = ReadLine();
                if (command.Contains("insert"))
                {
                    if (command.Contains("item"))
                    {
                        Item item = generation.FillItem();
                        WriteLine($"Id of new item is: {generation.items.Insert(item)}");
                    }
                    else
                    {
                        WriteLine("Unknown command.");
                    }
                }
                else if (command.Contains("generate"))
                {
                    int num;
                    while (true)
                    {
                        Write("Enter a num of generated values: ");
                        string value = ReadLine();
                        if (Validation.CheckInteger(value) && int.Parse(value) > 0)
                        {
                            num = int.Parse(value);
                            break;
                        }
                        else
                        {
                            WriteLine("Number wasn`t correct, please enter again!");
                        }
                    }
                    if(command.Contains("all"))
                    {
                        generation.GenerateItems(num);
                        generation.GenerateMods(num);
                    }
                    else if(command.Contains("items"))
                    {
                        generation.GenerateItems(num);
                    }
                    else if(command.Contains("mods"))
                    {
                        generation.GenerateMods(num);
                    }
                    else if(command.Contains("dataset"))
                    {
                        generation.GenerateModsDataset(num);
                        generation.GenerateItemsDataset(num);
                    }
                }
                else if (command == "exit" || command == "")
                {
                    WriteLine("Bye.");
                    break;
                }
                else
                {
                    WriteLine("Unknown command.");
                }
            }
        }
    }
}
