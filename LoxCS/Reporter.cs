namespace LoxCS;

internal interface IReporter
{
    public bool HadError { get; }
    public void Error(int line, int column, string message);

    public void Report(int line, int column, string where, string message);

    public void ResetError();
}
internal class ConsoleReporter: IReporter
{
    public bool HadError { get; private set; }
    public void Error(int line, int column, string message)
    {
        Report(line, column, "", message);
    }

    public void Report(int line, int column, string where, string message)
    {
        Console.Error.WriteLine($"[line {line} column {column}] Error {where}: {message}");
        HadError = true;
    }

    public void ResetError()
    {
        HadError = false;
    }
}
