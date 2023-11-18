using LoxCS;

namespace LoxCSTest;
public class RpnVisitorTests
{
    [Fact]
    public void ShouldPrintCorrectly()
    {
        var expr = new Binary(
            new Grouping(new Binary(
                new Literal(1),
                new Token(TokenType.PLUS, "+", null, 1),
                new Literal(2)
            )),
            new Token(TokenType.STAR, "*", null, 1),
            new Grouping(new Binary(
                new Literal(4),
                new Token(TokenType.MINUS, "-", null, 1),
                new Literal(3)
            ))
        );

        var printer = new RpnVisitor();
        Assert.Equal("1 2 + 4 3 - *", printer.Print(expr));
    }
}
