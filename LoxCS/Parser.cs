namespace LoxCS;

public class Parser : IDisposable
{
    private readonly IEnumerator<Token> _tokens;
    private readonly IReporter _reporter;
    private bool IsAtEnd => Current.Type == TokenType.EOF;
    private Token Current => _tokens.Current;
    private Token Previous { get; set; }

    public Parser(IEnumerable<Token> tokens, IReporter reporter)
    {
        _tokens = tokens.GetEnumerator();
        Previous = _tokens.Current;
        _tokens.MoveNext();
        _reporter = reporter;
    }

    public Expr? Parse()
    {
        try
        {
            return Expression();
        }
        catch (ParseException)
        {
            return null;
        }
    }

    private Expr Expression() => Equality();

    private Expr Equality() => LeftReduce(Comparison, TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL);

    private Expr Comparison() =>
        LeftReduce(Term, TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL);

    private Expr Term() => LeftReduce(Factor, TokenType.PLUS, TokenType.MINUS);

    private Expr Factor() => LeftReduce(Unary, TokenType.SLASH, TokenType.STAR);

    private Expr Unary()
    {
        if (!Match(TokenType.BANG, TokenType.MINUS)) return Primary();
        var @operator = Previous;
        var right = Unary();
        return new Unary(@operator, right);
    }

    private Expr Primary()
    {
        if (Match(TokenType.FALSE)) return new Literal(false);
        if (Match(TokenType.TRUE)) return new Literal(true);
        if (Match(TokenType.NIL)) return new Literal(null);

        if (Match(TokenType.NUMBER, TokenType.STRING)) return new Literal(Previous.Literal);

        if (Match(TokenType.LEFT_PAREN))
        {
            var expr = Expression();
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after expression");
            return new Grouping(expr);
        }

        throw Error(Current, "Expect expression.");
    }

    private void Synchronize()
    {
        Advance();
        while (!IsAtEnd)
        {
            if (Previous.Type == TokenType.SEMICOLON) return;
            switch (Current.Type)
            {
                case TokenType.CLASS: case TokenType.FOR: case TokenType.FUN: case TokenType.IF:
                case TokenType.PRINT: case TokenType.RETURN: case TokenType.VAR: case TokenType.WHILE: return;
            }

            Advance();
        }
    }

    private Token Consume(TokenType type, string message)
    {
        if (Check(type)) return Advance();
        throw Error(Current, message);
    }

    private ParseException Error(Token token, string message)
    {
        _reporter.Error(token, message);
        return new ParseException();
    }
    private Expr LeftReduce(Func<Expr> operand, params TokenType[] tokens)
    {
        var expr = operand();

        while (Match(tokens))
        {
            var @operator = Previous;
            var right = operand();
            expr = new Binary(expr, @operator, right);
        }

        return expr;
    }

    private bool Match(params TokenType[] tokens)
    {
        if (!tokens.Any(Check)) return false;
        Advance();
        return true;
    }

    private bool Check(TokenType token) => !IsAtEnd && Current.Type == token;

    private Token Advance()
    {
        if (IsAtEnd) return Previous;
        Previous = _tokens.Current;
        _tokens.MoveNext();
        return Previous;
    }

    public void Dispose()
    {
        _tokens.Dispose();
    }
}
