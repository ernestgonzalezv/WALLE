namespace Geo_Wall_E
{
    public class Compiler
    {
        private string source;
        private static List<SyntaxToken> tokens = new();
        private static List<SyntaxNode> nodes = new();
        public List<IDrawable> Result { get; }
        public Compiler(string source)
        {
            this.source = source;
            Result = Compile(source);
        }

        public static List<IDrawable> Compile(string source)
        {
            List<IDrawable> Result = new();
            try
            {
                Lexer lexer = new(source);
                tokens = lexer.Lexing();
                Parser parser = new(tokens);
                nodes = parser.Parsing();
                Interpreter interpreter = new(nodes);
                Result = interpreter.Evaluate();
                return Result;
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