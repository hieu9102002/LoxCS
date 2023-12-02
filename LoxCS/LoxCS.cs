namespace LoxCS;

class LoxCS
{
    private static readonly ConsoleReporter Reporter = new();

    public static int Main(string[] args)
    {
        switch (args.Length)
        {
            case > 1:
                Console.WriteLine("Usage: loxcs [script]");
                return 64;
            case 1:
                return RunFile(args[0]);
            default:
                RunREPL();
                break;
        }
        
        return 0;
    }


    private static void RunREPL()
    {
        while (true)
        {
            Console.Write("> ");
            var line = Console.ReadLine();
            if (line == null) break;
            Run(line);
            Reporter.ResetError();
        }
    }

    private static int RunFile(string path)
    {
        var code = File.ReadAllText(path);
        Run(code);
        return Reporter.HadError ? 65 : 0;
    }

    private static void Run(string source)
    {
        var scanner = new Scanner(source, Reporter);
        Reporter.SourceCode = source;
        var tokens = scanner.ScanTokens();
        using var parser = new Parser(tokens, Reporter);
        var expression = parser.Parse();

        if (Reporter.HadError || expression is null) return;
        Console.WriteLine(new AstPrinter().Print(expression));
    }
}
