namespace Geo_Wall_E
{
    public abstract class SyntaxNode
    {
        public abstract SyntaxKind Type { get; }
    }
    public abstract class SyntaxShapes : SyntaxNode
    {
        public abstract override SyntaxKind Type { get; }
    }
    
    public abstract class ExpressionSyntax : SyntaxNode
    {
        public abstract override SyntaxKind Type { get; }
    }
    public class Empty : SyntaxNode
    {
        public override SyntaxKind Type => SyntaxKind.EmptyToken;
    }
}