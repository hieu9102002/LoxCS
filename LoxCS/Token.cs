namespace LoxCS;
internal class Token(TokenType type, string lexeme, object? literal, int offset)
{
    public override string ToString()
        => $"{type} {lexeme} {literal}";
}
