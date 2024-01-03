namespace Geo_Wall_E
{
    public class Lexer
    {
        
        private int Current = 0;
        private int Line = 1;
        private int Column = 1;
        private int Start = 0;
        private readonly string Text;
        private bool IsTheEnd => Current >= Text.Length;

        public Lexer(string Text)
        {
            this.Text = Text;
        }
        private List<SyntaxToken> tokens = new();


        //lexing methods
        public List<SyntaxToken> Lexing()
        {
            try
            {
                while (!IsTheEnd)
                {
                    Start = Current;
                    ScanTokens();
                }
                tokens.Add(new SyntaxToken(SyntaxKind.EOFToken, null!, Column+1, null!, Line));
                return tokens;
            }
            catch (LexicalError error)
            {
                error.HandleException();
                return null!;
            }
        }
        private void ScanTokens()
        {
            char CurrentChar = Text[Current];
            switch (CurrentChar)
            {
                case ' ':
                case '\t':
                case '\r':
                Advance();
                break;
                case '"': GetStringToken(); break;
                case '\n': Line++; Column = 0; Current++; break;
                default:
                    if (IsLetter(CurrentChar))
                    {
                        GetIdentifier();
                        break;
                    }
                    else if (IsNumber(CurrentChar))
                    {
                        GetDigits();
                        break;
                    }
                    ScanTokensOperator();
                    break;
            }
        }
        
        //auxiliar methods
        private char Peek()
        {
            if (IsTheEnd) return '\0';
            return Text[Current];
        }
        private bool Match(char expected)
        {
            if (IsTheEnd || Text[Current] != expected) return false;
            Advance();
            return true;
        }
        private void AddToken(SyntaxKind type)
        {
            AddToken(type, null);
        }
        private void AddToken(SyntaxKind type, object? literal){tokens.Add(new SyntaxToken(type, literal, Column, Text.Substring(Start, Current - Start) ,  Line));}
        private void Advance(){Current++;Column++;}
        private bool IsLetter(char CurrentChar)
        {
            return ('a' <= CurrentChar && CurrentChar <= 'z') || ('A' <= CurrentChar && CurrentChar <= 'Z');
        }
        private bool IsNumber(char CurrentChar)
        {
            return ('0' <= CurrentChar && CurrentChar <= '9') ;
        }
        private void ScanTokensOperator()
        {
            char CurrentChar = Text[Current];
            Advance();
            if (SingleCharacterTokens.ContainsKey(CurrentChar))
            {
                switch (CurrentChar)
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
                        AddToken(SyntaxKind.StarToken); break;
                    case '+':
                        AddToken(SyntaxKind.PlusToken); break;
                    case '/':
                        if (Match('/')) Comments();
                        else AddToken(SyntaxKind.SlashToken);
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
                        else throw new LexicalError(Line, Column, "Invalid Token");
                        break;
                    default:
                        throw new LexicalError(Line, Column, "Invalid Token");
                }
            }
        }

        private void GetIdentifier()
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
                type = AccessingKinds[id];
                AddToken(type);

            }
            catch (KeyNotFoundException)
            {
                type = SyntaxKind.IdentifierToken;
                AddToken(type, id);
            }
        }

        private void GetDigits()
        {
            string number = Text[Current].ToString();
            bool IsDot = false;
            Advance();
            for (int i = Current; i < Text.Length; i++)
            {
                
                if (Text[i] == '.') IsDot = true;
                if (!char.IsDigit(Text[i]) && Text[i - 1] == '.') throw new LexicalError(Line, Column, "Después de '.' se esperaba un número");
                if (char.IsLetter(Text[i])) throw new LexicalError(Line, Column, "Numbers can't have letters");
                if (Text[i] == '.' && IsDot == true) throw new LexicalError(Line, Column, "Numbers can't have two dots");
                if (!char.IsDigit(Text[i]) && Text[i] != '.') break;
                number += Text[i];
                Advance();
            }
            AddToken(SyntaxKind.NumberToken, double.Parse(number));
        }
        private void Comments()
        {
            while (IsTheEnd && Peek() != '\n')
            {
                Advance();
            }
        }
        private void GetStringToken()
        {
            string str = "";
            Advance();

            while (Text[Current] != '"' && Current < Text.Length)
            {
                str += Text[Current];
                Advance();
                if (Text[Current] == '\n') Line++; Column = 0;
            }

            if (Current == Text.Length) throw new LexicalError(Line, Column, "Faltan comillas en su expresión");

            Advance();

            AddToken(SyntaxKind.StringToken, str);
        }


    // accessing Kinds with Diccs


        private static readonly Dictionary<string, SyntaxKind> AccessingKinds = new Dictionary<string, SyntaxKind>()
        {
            { "point",SyntaxKind.PointToken },
            { "points",SyntaxKind.PointsToken },
            { "draw",SyntaxKind.DrawToken },
            { "undefined", SyntaxKind.UndefinedToken },
            { "sequence", SyntaxKind.SequenceToken },
            { "Line", SyntaxKind.LineToken },
            { "if",SyntaxKind.IfToken },
            { "then",SyntaxKind.ThenToken },
            { "else",SyntaxKind.ElseToken },
            { "sin",SyntaxKind.SinToken},
            { "cos",SyntaxKind.CosToken},
            { "let", SyntaxKind.LetToken },
            { "in", SyntaxKind.InToken },
            { "log", SyntaxKind.LogToken },
            { "sqrt", SyntaxKind.SqrtToken },
            { "expo", SyntaxKind.ExpoToken },
            { "color",SyntaxKind.ColorToken },
            { "black",SyntaxKind.ColorBlackToken },
            { "blue",SyntaxKind.ColorBlueToken },
            { "cyan",SyntaxKind.ColorCyanToken },
            { "gray",SyntaxKind.ColorGrayToken },
            { "PI", SyntaxKind.PIToken },
            { "E", SyntaxKind.EToken },
            { "segment", SyntaxKind.SegmentToken },
            { "ray", SyntaxKind.RayToken },
            { "circle", SyntaxKind.CircleToken },
            { "restore", SyntaxKind.RestoreToken },
            { "import", SyntaxKind.ImportToken },
            { "arc", SyntaxKind.ArcToken },
            { "measure", SyntaxKind.MeasureToken },
            { "intersect", SyntaxKind.IntersectionToken },
            { "count", SyntaxKind.CountToken },
            { "randoms", SyntaxKind.RandomsToken },
            { "rest", SyntaxKind.RestToken },
            { "samples", SyntaxKind.SamplesToken },
            { "green",SyntaxKind.ColorGreenToken },
            { "magenta",SyntaxKind.ColorMagentaToken },
            { "red",SyntaxKind.ColorRedToken },
            { "white",SyntaxKind.ColorWhiteToken },
            { "yellow",SyntaxKind.ColorYellowToken },
        };

        private static readonly Dictionary<char, SyntaxKind> SingleCharacterTokens = new()
        {
            { ';', SyntaxKind.SemicolonToken },
            { '(', SyntaxKind.OpenParenthesisToken },
            { '-', SyntaxKind.MinusToken },
            { '/', SyntaxKind.SlashToken },
            { '=', SyntaxKind.EqualToken },
            { '*', SyntaxKind.StarToken },
            { '%', SyntaxKind.ModuToken },
            { '!', SyntaxKind.NotToken },
            { '<', SyntaxKind.LessToken },
            { '>', SyntaxKind.MoreToken },
            { '|', SyntaxKind.OrToken },
            { '&', SyntaxKind.AndToken },
            { '.', SyntaxKind.ThreeDotsToken },
            { '^', SyntaxKind.PowToken },
            { ')', SyntaxKind.CloseParenthesisToken},
            { '{', SyntaxKind.OpenBracketsToken },
            { '}', SyntaxKind.CloseBracketsToken },
            { ',', SyntaxKind.SeparatorToken },
            { '+', SyntaxKind.PlusToken },
            { '_', SyntaxKind.UnderscoreToken },
        };
        
    }
}
