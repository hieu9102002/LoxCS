using LoxCS;

namespace LoxCSTest
{
    public class ScannerTests
    {
        private const string SourceCode = "print \"Hello\";\nprint \"Hi\";\n{\n\tvar variable = nil;\n}";

        [Theory]
        [InlineData(0, 1, 1)]
        [InlineData(15,2,1)]
        [InlineData(30,4,2)]
        public void GetLineAndColumnCorrectly(int offset, int line, int col)
        {
            Assert.Equal(SourceCode.GetLineAndColumnFromOffset(offset), (line, col));
        }
    }
}