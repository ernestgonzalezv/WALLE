namespace Geo_Wall_E
{
    public class Compiler
    {
        private string txt;
        private static List<SyntaxNode> SyntaxNodes = new List<SyntaxNode> ();
        public List<IDrawable> Answer { get; }
        private static List<SyntaxToken> SyntaxTokens = new List<SyntaxToken>();
        
        public Compiler(string txt)
        {
            this.txt = txt;
            Answer = Compile(txt);
        }

        public static List<IDrawable> Compile(string txt)
        {
            List<IDrawable> Answer = new List<IDrawable>();
            try
            {
                Lexer Lexer = new(txt);
                SyntaxTokens = Lexer.Lexing();
                Parser Parser = new(SyntaxTokens);
                SyntaxNodes = Parser.ParsingTheWholeThing();
                Evaluator Evaluator = new(SyntaxNodes);
                Answer = Evaluator.Evaluate();
                return Answer;
            }
            catch (Error e)
            {
                e.HandleException();
            }
            catch (System.Exception)
            {

            }
            return null!;
        }
    }
}