namespace Geo_Wall_E{

public class SyntaxToken
    {
        public SyntaxKind Kind { get; }
        public string? Lexeme { get; }
        public int Column { get; }
        public int Line { get; }
        public object? Value { get; }
        public SyntaxToken(SyntaxKind kind, object? value,  int column, string lexeme,  int line)
        {
            Kind = kind;
            Value = value;
            Column = column;
            Lexeme = lexeme;
            Line = line;
            
        }
    }
}