namespace LoxCS;

public class RpnVisitor : IVisitor<string>
{
    public string Print(Expr expr) => expr.Accept(this);

    public string Visit(Binary expr)
        => $"{expr.Left.Accept(this)} {expr.Right.Accept(this)} {expr.Operator.Lexeme}";

    public string Visit(Literal expr)
        => expr.Value?.ToString() ?? "nil";

    public string Visit(Grouping expr)
        => expr.Expression.Accept(this);

    public string Visit(Unary expr)
        => $"{expr.Right.Accept(this)} {expr.Operator.Lexeme}";
}