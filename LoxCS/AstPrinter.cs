namespace LoxCS;

public class AstPrinter : IVisitor<string>
{
    public string Print(Expr expr) => expr.Accept(this);

    public string Visit(Binary expr)
        => Parenthesize(expr.Operator.Lexeme, expr.Left, expr.Right);

    public string Visit(Literal expr)
        => expr.Value?.ToString() ?? "nil";

    public string Visit(Grouping expr)
        => Parenthesize("group", expr.Expression);

    public string Visit(Unary expr)
        => Parenthesize(expr.Operator.Lexeme, expr.Right);

    private string Parenthesize(string name, params Expr[] exprs)
        => $"({name} {string.Join(' ', exprs.Select(expr => expr.Accept(this)))})";
}