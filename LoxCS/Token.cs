namespace LoxCS;
public record Token(TokenType Type, string Lexeme, object? Literal, int Offset)
{
    public override string ToString()
        => $"{Type} {Lexeme} {Literal}";
}
