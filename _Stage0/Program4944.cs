partial class Program
{
    private static void Main(string[] args)
    {
        Welcome4944();
        Welcome2284();
        Console.ReadKey();
    }

    private static void Welcome4944()
    {
        Console.WriteLine("Enter your name!");
        string name = Console.ReadLine();
        Console.WriteLine("{0}, Welcome to Ayala and Shira's first project!", name);
    }
    static partial void Welcome2284();
}
