namespace Geo_Wall_E
{
    public class SyntaxToken
    {
        public SyntaxKind Type { get; }
        public int Column { get; }
        public object? Value { get; }
        public string? Lexeme { get; }
        public int Line { get; }
        
        public SyntaxToken(SyntaxKind type, object? value, int line, int column, string lexeme)
        {
            Type = type;
            Value = value;
            Line = line;
            Column = column;
            Lexeme = lexeme;
        }
    }
}