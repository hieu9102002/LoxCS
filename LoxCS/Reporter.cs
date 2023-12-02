namespace LoxCS;

public interface IReporter
{
    public bool HadError { get; }
    public string SourceCode { get; set; }
    public void Error(Token token, string message);
    public void Error(int line, int column, string message);
    public void ResetError();
}
internal class ConsoleReporter: IReporter
{
    public bool HadError { get; private set; }
    public string SourceCode { get; set; } = "";

    public void Error(Token token, string message)
    {
        var (line, column) = SourceCode.GetLineAndColumnFromOffset(token.Offset - (token.Type == TokenType.EOF ? 1 : 0));
        Report(line, column, token.Type == TokenType.EOF ? " at end" : $" at '{token.Lexeme}'", message);
    }

    public void Error(int line, int column, string message)
    {
        Report(line, column, "", message);
    }

    public void Report(int line, int column, string where, string message)
    {
        Console.Error.WriteLine($"[line {line} column {column}] Error{where}: {message}");
        HadError = true;
    }

    public void ResetError()
    {
        HadError = false;
    }
}
