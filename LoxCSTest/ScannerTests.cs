using LoxCS;
using NSubstitute;

namespace LoxCSTest
{
    public class ScannerTests
    {
        private const string SourceCode = "print \"Hello\";\nprint \"Hi\";\n{\n\tvar variable = nil;\n}// This is a comment\n/*This is a block comment*/";

        [Theory]
        [InlineData(0, 1, 1)]
        [InlineData(15,2,1)]
        [InlineData(30,4,2)]
        public void GetLineAndColumnCorrectly(int offset, int line, int col)
        {
            Assert.Equal(SourceCode.GetLineAndColumnFromOffset(offset), (line, col));
        }

        [Fact]
        public void ParseCodeCorrectly()
        {
            var reporter = Substitute.For<IReporter>();

            var scanner = new Scanner(SourceCode, reporter);
            var tokens = scanner.ScanTokens();

            var expected = new Token[]
            {
                new(TokenType.PRINT, "print", null, 0),
                new(TokenType.STRING, "\"Hello\"", "Hello", 6),
                new(TokenType.SEMICOLON, ";", null, 13),
                new(TokenType.PRINT, "print", null, 15),
                new(TokenType.STRING, "\"Hi\"", "Hi", 21),
                new(TokenType.SEMICOLON, ";", null, 25),
                new(TokenType.LEFT_BRACE, "{",  null, 27),
                new(TokenType.VAR, "var", null, 30),
                new(TokenType.IDENTIFIER, "variable", null, 34),
                new(TokenType.EQUAL, "=", null, 43),
                new(TokenType.NIL,  "nil", null, 45),
                new(TokenType.SEMICOLON,";",null, 48),
                new(TokenType.RIGHT_BRACE, "}", null, 50),
                new(TokenType.EOF, "", null, SourceCode.Length)
            };

            Assert.Equal(expected, tokens);
        }

        [Fact]
        public void ShouldRaiseErrorWithUnterminatedString()
        {
            var reporter = Substitute.For<IReporter>();
            var scanner = new Scanner("print \";", reporter);

            var tokens = scanner.ScanTokens();
            var expected = new Token[]
            {
                new(TokenType.PRINT, "print", null, 0),
                new(TokenType.EOF, "", null, 8)
            };

            Assert.Equal(expected, tokens);
            reporter.Received().Error(1,7,"Unterminated string.");
        }

        [Fact]
        public void ShouldRaiseErrorWithUnterminatedBlockComment()
        {
            var reporter = Substitute.For<IReporter>();
            var scanner = new Scanner("print /*", reporter);

            var tokens = scanner.ScanTokens();
            var expected = new Token[]
            {
                new(TokenType.PRINT, "print", null, 0),
                new(TokenType.EOF, "", null, 8)
            };

            Assert.Equal(expected, tokens);
            reporter.Received().Error(1, 7, "Unterminated block comment");
        }

        [Fact]
        public void ShouldRaiseErrorWithUnrecognizedSymbol()
        {
            var reporter = Substitute.For<IReporter>();
            var scanner = new Scanner("$%^;", reporter);

            var tokens = scanner.ScanTokens();
            var expected = new Token[]
            {
                new(TokenType.SEMICOLON, ";", null, 3),
                new(TokenType.EOF, "", null, 4)
            };

            Assert.Equal(expected, tokens);
            reporter.Received().Error(1, 1, "Unexpected character.");
            reporter.Received().Error(1, 2, "Unexpected character.");
            reporter.Received().Error(1, 3, "Unexpected character.");
        }
    }
}