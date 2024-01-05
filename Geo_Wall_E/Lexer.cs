namespace Geo_Wall_E
{
    public class Lexer
    {
        private readonly string text;
        private int position = 0;
        private int line = 1;
        private int column = 1;
        private int start = 0;
        private bool isTheEnd => position >= text.Length;

        public Lexer(string text)
        {
            this.text = text;
        }
        private List<SyntaxToken> tokens = new();

        private static readonly Dictionary<string, SyntaxKind> keywords = new Dictionary<string, SyntaxKind>()
        {
           
            { "sqrt", SyntaxKind.SqrtToken },
            { "expo", SyntaxKind.ExpoToken },
            { "PI", SyntaxKind.PIToken },
            { "E", SyntaxKind.EToken },
            { "color",SyntaxKind.ColorToken },
            { "black",SyntaxKind.ColorBlackToken },
            { "blue",SyntaxKind.ColorBlueToken },
            { "cyan",SyntaxKind.ColorCyanToken },
            { "gray",SyntaxKind.ColorGrayToken },
             { "point",SyntaxKind.PointToken },
            { "points",SyntaxKind.PointsToken },
            { "draw",SyntaxKind.DrawToken },
            { "undefined", SyntaxKind.UndefinedToken },
            { "sequence", SyntaxKind.SequenceToken },
            { "line", SyntaxKind.LineToken },
            { "segment", SyntaxKind.SegmentToken },
            { "ray", SyntaxKind.RayToken },
            { "circle", SyntaxKind.CircleToken },
            { "log", SyntaxKind.LogToken },
            { "green",SyntaxKind.ColorGreenToken },
            { "magenta",SyntaxKind.ColorMagentaToken },
            { "red",SyntaxKind.ColorRedToken },
            { "white",SyntaxKind.ColorWhiteToken },
            { "yellow",SyntaxKind.ColorYellowToken },
            { "restore", SyntaxKind.RestoreToken },
            { "import", SyntaxKind.ImportToken },
            { "arc", SyntaxKind.ArcToken },
            { "measure", SyntaxKind.MeasureToken },
            { "intersect", SyntaxKind.IntersectToken },
            { "count", SyntaxKind.CountToken },
            { "randoms", SyntaxKind.RandomsToken },
            { "rest", SyntaxKind.RestToken },
            { "samples", SyntaxKind.SamplesToken },
            { "if",SyntaxKind.IfToken },
            { "then",SyntaxKind.ThenToken },
            { "else",SyntaxKind.ElseToken },
            { "sin",SyntaxKind.SinToken},
            { "cos",SyntaxKind.CosToken},
            { "let", SyntaxKind.LetToken },
            { "in", SyntaxKind.InToken },
            
        };

        private static readonly Dictionary<char, SyntaxKind> charTokens = new()
        {
            { ';', SyntaxKind.SemicolonToken },
            { '(', SyntaxKind.OpenParenthesisToken },
            { ')', SyntaxKind.CloseParenthesisToken},
            { '{', SyntaxKind.OpenBracketsToken },
            { '}', SyntaxKind.CloseBracketsToken },
            { ',', SyntaxKind.SeparatorToken },
            { '+', SyntaxKind.PlusToken },
            { '-', SyntaxKind.MinusToken },
            { '^', SyntaxKind.PowToken },
            { '/', SyntaxKind.DivToken },
            { '=', SyntaxKind.EqualToken },
            { '*', SyntaxKind.MultToken },
            { '%', SyntaxKind.ModuToken },
            { '!', SyntaxKind.NotToken },
            { '<', SyntaxKind.LessToken },
            { '>', SyntaxKind.MoreToken },
            { '|', SyntaxKind.OrToken },
            { '&', SyntaxKind.AndToken },
            { '.', SyntaxKind.ThreeDotsToken },
            { '_', SyntaxKind.UnderscoreToken },
        };
        public List<SyntaxToken> Lexing()
        {
            try
            {
                while (!isTheEnd)
                {
                    start = position;
                    Scan();
                }
                tokens.Add(new SyntaxToken(SyntaxKind.EndFileToken, null!, line, column + 1, null!));
                List<SyntaxToken> list = tokens;
                return list;
            }
            catch (LexicalError error)
            {
                error.HandleException();
                return null!;
            }
        }
        private void Scan()
        {
            char positionChar = text[position];
            switch (positionChar)
            {
                case '"': GetString(); break;
                case ' ':
                case '\t':
                case '\r':
                Advance();
                    break;
                case '\n': line++; column = 0; position++; break;
                default:
                    if (IsNumber(positionChar))
                    {
                        GetNumber();
                        break;
                    }
                    else if (IsLetter(positionChar))
                    {
                        GetID();
                        break;
                    }
                    GetOperator();
                    break;
            }
        }
        private bool IsLetter(char positionChar)
        {
            return ('a' <= positionChar && positionChar <= 'z') || ('A' <= positionChar && positionChar <= 'Z');
        }
        private bool IsNumber(char positionChar)
        {
            if ('0' <= positionChar && positionChar <= '9') return true;
            return false;
        }
        private void GetOperator()
        {
            char positionChar = text[position];
            Advance();
            if (charTokens.ContainsKey(positionChar))
            {
                switch (positionChar)
                {
                    case '(':
                        AddToken(SyntaxKind.OpenParenthesisToken); break;
                    case ')':
                        AddToken(SyntaxKind.CloseParenthesisToken); break;
                    case '{':
                        AddToken(SyntaxKind.OpenBracketsToken); break;
                    case '}':
                        AddToken(SyntaxKind.CloseBracketsToken); break;
                    case ',':
                        AddToken(SyntaxKind.SeparatorToken); break;
                    case ';':
                        AddToken(SyntaxKind.SemicolonToken); break;
                    case '_':
                        AddToken(SyntaxKind.UnderscoreToken); break;
                    case '&':
                        AddToken(SyntaxKind.AndToken); break;
                    case '|':
                        AddToken(SyntaxKind.OrToken); break;
                    case '-':
                        AddToken(SyntaxKind.MinusToken); break;
                    case '^':
                        AddToken(SyntaxKind.PowToken); break;
                    case '%':
                        AddToken(SyntaxKind.ModuToken); break;
                    case '*':
                        AddToken(SyntaxKind.MultToken); break;
                    case '+':
                        AddToken(SyntaxKind.PlusToken); break;
                    case '/':
                        if (Match('/')) GetComment();
                        else AddToken(SyntaxKind.DivToken);
                        break;
                    case '=':
                        if (Match('=')) AddToken(SyntaxKind.DoubleEqualToken);
                        else AddToken(SyntaxKind.EqualToken);
                        break;
                    case '!':
                        if (Match('=')) AddToken(SyntaxKind.NoEqualToken);
                        else AddToken(SyntaxKind.NotToken);
                        break;
                    case '<':
                        if (Match('=')) AddToken(SyntaxKind.LessOrEqualToken);
                        else AddToken(SyntaxKind.LessToken);
                        break;
                    case '>':
                        if (Match('=')) AddToken(SyntaxKind.MoreOrEqualToken);
                        else AddToken(SyntaxKind.MoreToken);
                        break;
                    case '.':
                        if (Match('.') && Match('.')) AddToken(SyntaxKind.ThreeDotsToken);
                        else throw new LexicalError(line, column, "No se reconoce este caracter");
                        break;
                    default:
                        throw new LexicalError(line, column, "No se reconoce este caracter");
                }
            }
        }

        private void GetID()
        {
            string id = "";
            while (IsNumber(Peek()) || IsLetter(Peek()) || Peek() == '_')
            {
                id += Peek();
                Advance();
            }
            SyntaxKind type;
            try
            {
                type = keywords[id];
                AddToken(type);

            }
            catch (KeyNotFoundException)
            {
                type = SyntaxKind.ID;
                AddToken(type, id);
            }
        }

        private void GetNumber()
        {
            string number = text[position].ToString();
            bool IsDot = false;
            Advance();
            for (int i = position; i < text.Length; i++)
            {
                
                if (char.IsLetter(text[i])) throw new LexicalError(line, column, "Después de un número no debe escribir una letra");
            
                if (text[i] == '.' && IsDot == true) throw new LexicalError(line, column, "Este número ya contenía un punto");
                if (!char.IsDigit(text[i]) && text[i - 1] == '.') throw new LexicalError(line, column, "Después de '.' se esperaba un número");
                
                if (text[i] == '.') IsDot = true;
            
                if (!char.IsDigit(text[i]) && text[i] != '.') break;
                number += text[i];
                Advance();
            }
            AddToken(SyntaxKind.Number, double.Parse(number));
        }
        
        private void GetComment()
        {
            while (isTheEnd && Peek() != '\n')
            {
                Advance();
            }
        }
        private void GetString()
        {
            string str = "";
            Advance();

            while (text[position] != '"' && position < text.Length)
            {
                str += text[position];
                Advance();
                if (text[position] == '\n') line++; column = 0;
            }

            if (position == text.Length) throw new LexicalError(line, column, "Faltan comillas en su expresión");

            Advance();

            AddToken(SyntaxKind.String, str);
        }
        private char Peek()
        {
            
            if (isTheEnd) return '\0';
            return text[position];
        }
        private bool Match(char expected)
        {
            if (isTheEnd || text[position] != expected) return false;
            Advance();
            return true;
        }
        private void AddToken(SyntaxKind type)
        {
            AddToken(type, null);
        }
        private void AddToken(SyntaxKind type, object? literal)
        {
            string lexeme = text.Substring(start, position - start);
            tokens.Add(new SyntaxToken(type, literal, line, column, lexeme));
        }
        private void Advance()
        {
            position++;
            column++;
        }
    }
}
