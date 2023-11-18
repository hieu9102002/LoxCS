namespace LoxCS;
internal class Scanner(string source, IReporter reporter)
{
    private static readonly IReadOnlyDictionary<string, TokenType> Keywords = new Dictionary<string, TokenType>
    {
        {"and", TokenType.AND},
        {"class", TokenType.CLASS},
        {"else", TokenType.ELSE},
        {"false", TokenType.FALSE},
        {"true", TokenType.TRUE},
        {"for", TokenType.FOR},
        {"fun", TokenType.FUN},
        {"if", TokenType.IF},
        {"nil", TokenType.NIL},
        {"or", TokenType.OR},
        {"print", TokenType.PRINT},
        {"return", TokenType.RETURN},
        {"super", TokenType.SUPER},
        {"this", TokenType.THIS},
        {"var", TokenType.VAR},
        {"while", TokenType.WHILE}
    };

    private int _start;
    private int _current;
    private bool IsAtEnd => _current >= source.Length;

    public IEnumerable<Token> ScanTokens()
    {
        _current = 0;
        while (!IsAtEnd)
        {
            _start = _current;
            var token = ScanToken();
            if (token != null)
            {
                yield return token;
            }
        }

        var eof = new Token(TokenType.EOF, "", null, _current);
        yield return eof;
    }

    private Token? ScanToken()
    {
        var c = Advance();
        switch (c)
        {
            case '(': return CreateToken(TokenType.LEFT_PAREN);
            case ')': return CreateToken(TokenType.RIGHT_PAREN);
            case '{': return CreateToken(TokenType.LEFT_BRACE);
            case '}': return CreateToken(TokenType.RIGHT_BRACE);
            case ',': return CreateToken(TokenType.COMMA);
            case '.': return CreateToken(TokenType.DOT);
            case '-': return CreateToken(TokenType.MINUS);
            case '+': return CreateToken(TokenType.PLUS);
            case '*': return CreateToken(TokenType.STAR);
            case ';': return CreateToken(TokenType.SEMICOLON);
            case '!': return CreateToken(Match('=') ? TokenType.BANG_EQUAL : TokenType.BANG);
            case '=': return CreateToken(Match('=') ? TokenType.EQUAL_EQUAL: TokenType.EQUAL);
            case '>': return CreateToken(Match('=') ? TokenType.GREATER_EQUAL: TokenType.GREATER);
            case '<': return CreateToken(Match('=') ? TokenType.LESS_EQUAL : TokenType.LESS);
            case '/':
                if (Match('/'))
                {
                    while (Peek() != '\n' && !IsAtEnd) Advance();
                } 
                else if (Match('*')) BlockComment();
                else return CreateToken(TokenType.SLASH);
                break;
            case '"': return String();
            case ' ':
            case '\r':
            case '\t':
            case '\n':
                break;
            default:
                if (c.IsDigit()) return Number();
                if (c.IsAlpha()) return Identifier();
                ReportError("Unexpected character.");
                break;
        }

        return null;
    }

    private void ReportError(string message)
    {
        var (line, col) = source.GetLineAndColumnFromOffset(_start);
        reporter.Error(line, col, message);
    }

    private Token CreateToken(TokenType type, object? literal = null)
    {
        var text = source[_start.._current];
        return new Token(type, text, literal, _start);
    }

    private Token? String()
    {
        while (Peek() != '"' && !IsAtEnd) Advance();

        if (IsAtEnd)
        {
            ReportError("Unterminated string.");
            return null;
        }

        Advance();

        var value = source[(_start + 1)..(_current - 1)];
        return CreateToken(TokenType.STRING, value);
    }

    private Token Number()
    {
        while (Peek().IsDigit()) Advance();

        if (Peek() != '.' || !PeekNext().IsDigit())
            return CreateToken(TokenType.NUMBER, double.Parse(source[_start.._current]));

        Advance();
        while (Peek().IsDigit()) Advance();

        return CreateToken(TokenType.NUMBER, double.Parse(source[_start.._current]));
    }

    private Token Identifier()
    {
        while (Peek().IsAlphaNumeric()) Advance();
        var text = source[_start.._current];
        return CreateToken(Keywords.TryGetValue(text, out var token) ? token : TokenType.IDENTIFIER);
    }

    private void BlockComment()
    {
        while (!(Peek() == '*' && PeekNext() == '/') && !IsAtEnd)
        {
            Advance();
        }

        if (IsAtEnd)
        {
            ReportError("Unterminated block comment");
            return;
        }
        Advance();
        Advance();
    }

    private bool Match(char expected)
    {
        if (IsAtEnd) return false;
        if (source[_current] != expected) return false;

        _current++;
        return true;
    }

    private char Advance() => source[_current++];
    private char Peek() => IsAtEnd ? '\0' : source[_current];
    private char PeekNext() => _current + 1 >= source.Length ? '\0' : source[_current+1];
}

public static class SourceExtensions
{
    public static (int, int) GetLineAndColumnFromOffset(this string source, int offset)
    {
        var lines = source[..(offset + 1)].Split('\n');
        return (lines.Length, lines.Last().Length);
    }

    public static bool IsDigit(this char c) => c is >= '0' and <= '9';

    public static bool IsAlpha(this char c) => c is >= 'A' and <= 'Z' or >= 'a' and <= 'z' or '_';

    public static bool IsAlphaNumeric(this char c) => c.IsAlpha() || c.IsDigit();
}
