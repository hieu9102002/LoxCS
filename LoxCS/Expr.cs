namespace LoxCS;

public partial interface IVisitor<out T>;

public abstract record Expr
{
    public abstract T Accept<T>(IVisitor<T> visitor);
};

public partial record Binary(Expr Left, Token Operator, Expr Right) : Expr;

public partial record Grouping(Expr Expression) : Expr;

public partial record Literal(object? Value) : Expr;

public partial record Unary(Token Operator, Expr Right) : Expr;