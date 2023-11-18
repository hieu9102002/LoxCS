using LoxCS;

namespace LoxCSTest;

public class AstPrinterTests
{
    [Fact]
    public void ShouldPrintCorrectly()
    {
        var expr = new Binary(
            new Unary(
                new Token(TokenType.MINUS, "-", null, 1),
                new Literal(123)),
            new Token(TokenType.STAR, "*", null, 5),
            new Grouping(
                new Literal(45.67)));

        var printer = new AstPrinter();

        Assert.Equal("(* (- 123) (group 45.67))", printer.Print(expr));
    }
}