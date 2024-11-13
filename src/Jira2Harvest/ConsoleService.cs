namespace Jira2Harvest
{
    public class ConsoleService
    {
        public void WriteLine(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ResetColor();
        }
    }
}
