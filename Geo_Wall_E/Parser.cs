using System.Text.RegularExpressions;

namespace Geo_Wall_E
{
    public class Parser
    {
        private Scope scope = new();
        private ColorStack colorStack = new();
        private readonly List<SyntaxToken> tokens;
        private int current = 0;
        public Parser(List<SyntaxToken> tokens)
        {
            this.tokens = tokens;
        }
        private bool isTheEnd => currentToken.Type == SyntaxKind.EndFileToken;
        private SyntaxToken currentToken => tokens[current];
        private SyntaxToken previousToken => tokens[current - 1];
        private SyntaxToken nextToken => tokens[current + 1];

        public List<SyntaxNode> Parsing()
        {
            try
            {
                List<SyntaxNode> nodes = Parse();
                return nodes;
            }
            catch (SyntaxError error)
            {
                error.HandleException();
                return null!;
            }
        }

        private List<SyntaxNode> Parse()
        {
            List<SyntaxNode> nodes = new();
            if (tokens[0].Type == SyntaxKind.ImportToken)
            {
                Advance();
                Match(SyntaxKind.String);
    
                string fileName = tokens[1].Lexeme!.Replace("\"", "");
                string path = Path.GetFullPath(fileName);
                if (!Path.Exists(path)) throw new FileNotFoundException();
                string source = System.Text.Encoding.Default.GetString(File.ReadAllBytes(path));
                Lexer lexer = new(source);
                Parser parser = new(lexer.Lexing());
                nodes = parser.Parsing();

            }
            while (!isTheEnd)
            {
                nodes.Add(ParseFunction());
                ExpectedAndAdvance(SyntaxKind.SemicolonToken);
            }
            return nodes;
        }

        private SyntaxNode ParseFunction()
        {
            if (Expected(SyntaxKind.ID))
            {
                if (nextToken.Type == SyntaxKind.OpenParenthesisToken)
                {
                    SyntaxToken name = Advance();
                    if (!scope.variablesInFunction.ContainsKey(name.Lexeme!))
                    {
                        scope.Search(new FunctionStatement(name, null!, null!));
                        Advance();
                        List<ExpressionSyntax> arguments = GetArguments();
                        MatchAndAdvance(SyntaxKind.CloseParenthesisToken);
                        MatchAndAdvance(SyntaxKind.EqualToken);
                        ExpressionSyntax body = ParseExpression();
                        MatchAndAdvance(SyntaxKind.SemicolonToken);
                        FunctionStatement function = new(name, arguments, body);
                        scope.Search(function);
                        return function;
                    }
                    current--;
                    return ParseFunctionCall();
                }
                return ParseId();
            }
            return ParseStmt();
        }

        private SyntaxNode ParseStmt()
        {
            return currentToken.Type switch
            {
                SyntaxKind.ArcToken => ParseArc(),
                SyntaxKind.CircleToken => ParseCircle(),
                SyntaxKind.ColorToken => ParseColor(),
                SyntaxKind.CountToken => ParseCount(),
                SyntaxKind.DrawToken => ParseDraw(),
                SyntaxKind.ID => ParseId(),
                SyntaxKind.LineToken => ParseLine(),
                SyntaxKind.PointToken => ParsePoint(),
                SyntaxKind.RayToken => ParseRay(),
                SyntaxKind.RestToken => ParseId(),
                SyntaxKind.RestoreToken => ParseRestore(),
                SyntaxKind.SegmentToken => ParseSegment(),
                SyntaxKind.UnderscoreToken => ParseId(),
                _ => ParseExpression(),
            };
        }

        private ExpressionSyntax ParseExpression()
        {
            return currentToken.Type switch
            {
                SyntaxKind.ArcToken => ParseArcExpression(),
                SyntaxKind.CircleToken => ParseCircleExpression(),
                //SyntaxKind.ID => ParseFunctionCall(),
                SyntaxKind.IfToken => ParseConditional(),
                SyntaxKind.IntersectToken => ParseIntersect(),
                SyntaxKind.LetToken => ParseLetInExpression(),
                SyntaxKind.LineToken => ParseLineExpression(),
                SyntaxKind.MeasureToken => ParseMeasure(),
                SyntaxKind.OpenBracketsToken => ParseSequence(),
                SyntaxKind.OpenParenthesisToken => ParseBetweenParenExpressions(),
                SyntaxKind.PointsToken => ParsePoints(),
                SyntaxKind.RandomsToken => ParseRandoms(),
                SyntaxKind.RayToken => ParseRayExpression(),
                SyntaxKind.RestToken => new VariableExpression(Advance()),
                SyntaxKind.SamplesToken => ParseSamples(),
                SyntaxKind.SegmentToken => ParseSegmentExpression(),
                _ => ParseLogical(),
            };
        }

        private ExpressionSyntax ParseFunctionCall()
        {
            SyntaxToken name = Advance();
            if (ExpectedAndAdvance(SyntaxKind.OpenParenthesisToken))
            {
                if (scope.variablesInFunction.ContainsKey(name.Lexeme!))
                {
                    //MatchAndAdvance(SyntaxKind.OpenParenthesisToken);
                    List<ExpressionSyntax> arguments = new();
                    if (!ExpectedAndAdvance(SyntaxKind.CloseParenthesisToken))
                    {
                        do
                        {
                            if (!ExpectedAndAdvance(SyntaxKind.CloseParenthesisToken))
                            {
                                ExpressionSyntax argument = ParseExpression();
                                arguments.Add(argument);
                            }
                            else throw new SyntaxError(currentToken.Line, currentToken.Column, "Se esperaba la declaración de un argumento para la función en lugar de " + currentToken.Lexeme);
                        } while (ExpectedAndAdvance(SyntaxKind.SeparatorToken));
                    }
                    MatchAndAdvance(SyntaxKind.CloseParenthesisToken);
                    return new FunctionCallExpression(name, arguments, null!);
                }
                throw new SyntaxError(currentToken.Line, currentToken.Column, "La función " + currentToken.Lexeme + " no ha sido declarada");
            }
            return new VariableExpression(name);
        }

        private ExpressionSyntax ParseLogical()
        {
            ExpressionSyntax expression = Equal();
            
            while (Expected(SyntaxKind.AndToken, SyntaxKind.OrToken))
            {
                SyntaxToken oper = Advance();
                ExpressionSyntax right = Equal();
                expression = new BinaryExpression(expression, oper, right);
            }
            return expression;
        }

        private ExpressionSyntax Equal()
        {
            ExpressionSyntax expression = Compare();
            
            int temp = 0;
            while (Expected(SyntaxKind.DoubleEqualToken, SyntaxKind.NoEqualToken))
            {
                SyntaxToken oper = Advance();
                if (temp >= 1) throw new SyntaxError(currentToken.Line, currentToken.Column, "No es posible utilizar el operador " + currentToken.Lexeme + " más de una vez en una misma condición");
                ExpressionSyntax right = Compare();
                expression = new BinaryExpression(expression, oper, right);
                temp++;
            }
            return expression;
        }

        private ExpressionSyntax Compare()
        {
            ExpressionSyntax expression = Term();
            int temp = 0;
            while (Expected(SyntaxKind.MoreToken, SyntaxKind.MoreOrEqualToken, SyntaxKind.LessToken, SyntaxKind.LessOrEqualToken))
            {
                SyntaxToken oper = Advance();
                if (temp >= 1) throw new SyntaxError(currentToken.Line, currentToken.Column, "No es posible utilizar el operador " + currentToken.Lexeme + " más de una vez en una misma condición");
                ExpressionSyntax right = Term();
                expression = new BinaryExpression(expression, oper, right);
                temp++;
            }
            return expression;
        }

        private ExpressionSyntax Term()
        {
            ExpressionSyntax expression = Factor(); 
            while (Expected(SyntaxKind.MinusToken, SyntaxKind.PlusToken))
            {
                SyntaxToken oper = Advance();
                ExpressionSyntax right = Factor();
                expression = new BinaryExpression(expression, oper, right);
            }
            return expression;
        }

        private ExpressionSyntax Factor()
        {
            ExpressionSyntax expression = Power();
            while (Expected(SyntaxKind.MultToken, SyntaxKind.DivToken, SyntaxKind.ModuToken))
            {
                SyntaxToken oper = Advance();
                ExpressionSyntax right = Power();
                expression = new BinaryExpression(expression, oper, right);
            }
            return expression;
        }

        private ExpressionSyntax Power()
        {
            ExpressionSyntax expression = Unary();
        
            if (Expected(SyntaxKind.PowToken))
            {
                SyntaxToken oper = Advance();
                ExpressionSyntax right = Power();
                expression = new BinaryExpression(expression, oper, right);
            }
            return expression;
        }

        private ExpressionSyntax Unary()
        {
            
            if (Expected(SyntaxKind.NotToken, SyntaxKind.MinusToken, SyntaxKind.PlusToken))
            {
                SyntaxToken oper = Advance();
                return new UnaryExpression(oper, Unary());
            }
            return MathFunction();
        }

        private ExpressionSyntax MathFunction()
        {
            
            if (Expected(SyntaxKind.LogToken))
            {
                SyntaxToken operlog = Advance();
                Match(SyntaxKind.OpenParenthesisToken);
                ExpressionSyntax value = ParseBetweenParenExpressions();
                ExpressionSyntax Base = ParseExpression();
                ExpressionSyntax expression = new LogExpression(operlog, value, Base);
                return expression;
            }
            if (Expected(SyntaxKind.SinToken, SyntaxKind.CosToken, SyntaxKind.SqrtToken, SyntaxKind.ExpoToken))
            {
                SyntaxToken oper = Advance();
                MatchAndAdvance(SyntaxKind.OpenParenthesisToken);
                UnaryExpression expression = new(oper, ParseExpression());
                MatchAndAdvance(SyntaxKind.CloseParenthesisToken);
                return expression;
            }
            return Literal();
        }

        private ExpressionSyntax Literal()
        {
            switch (currentToken.Type)
            {
                case SyntaxKind.Number: Advance(); return new NumberExpression(previousToken);
                case SyntaxKind.String: Advance(); return new StringExpression(previousToken);
                case SyntaxKind.PIToken: Advance(); return new PIExpression((string)previousToken.Value!);
                case SyntaxKind.EToken: Advance(); return new EExpression((string)previousToken.Value!);
                case SyntaxKind.UndefinedToken: Advance(); return new UndefinedExpression();
                case SyntaxKind.ID: return ParseFunctionCall();
                default: throw new SyntaxError(currentToken.Line, currentToken.Column, "");
            }
        }

        private ConditionalExpression ParseConditional()
        {
            Advance();
            ExpressionSyntax _if = ParseExpression();
            MatchAndAdvance(SyntaxKind.ThenToken);
            ExpressionSyntax then = ParseExpression();
            MatchAndAdvance(SyntaxKind.ElseToken);
            ExpressionSyntax _else = ParseExpression();
            return new ConditionalExpression(_if, then, _else);
        }

        private ExpressionSyntax ParseLetInExpression()
        {
            Advance();
            
            List<Statement> assigments = new();
            do
            {
                assigments.Add((Statement)ParseStmt());
            } while (!Expected(SyntaxKind.InToken));
            MatchAndAdvance(SyntaxKind.InToken);
            ExpressionSyntax _in = ParseExpression();
            return new LetInExpression(assigments, _in);
        }

        private BetweenParenExpressions ParseBetweenParenExpressions()
        {
            Advance();
            BetweenParenExpressions exp = new(ParseExpression());
            Match(SyntaxKind.CloseParenthesisToken);
            return exp;
        }

        private SequenceExpression ParseSequence()
        {
            Advance();
            if (ExpectedAndAdvance(SyntaxKind.CloseBracketsToken))
            {
                Empty empty = new();
                return new SequenceExpression(empty);
            }
            if (nextToken.Type == SyntaxKind.ThreeDotsToken)
            {
                Match(SyntaxKind.Number);
                SyntaxToken start = Advance();
                Advance();
                if (Expected(SyntaxKind.Number))
                {
                    SyntaxToken end = Advance();
                    MatchAndAdvance(SyntaxKind.CloseBracketsToken);
                    return new SequenceExpression(start, end);
                }
                MatchAndAdvance(SyntaxKind.CloseBracketsToken);
                return new SequenceExpression(start);
            }
            List<ExpressionSyntax> sequence = new()
            {
                ParseExpression()
            };
            while (ExpectedAndAdvance(SyntaxKind.SeparatorToken))
            {
                sequence.Add(ParseExpression());
            }
            MatchAndAdvance(SyntaxKind.CloseBracketsToken);
            if (ExpectedAndAdvance(SyntaxKind.PlusToken)) return ParseConcatenatedSequence(sequence);
            return new SequenceExpression(sequence);
        }

        private SequenceExpression ParseConcatenatedSequence(List<ExpressionSyntax> sequence)
        {
            SequenceExpression sequence_ = new(sequence);
            if (Expected(SyntaxKind.UndefinedToken))
            {
                Advance();
                UndefinedExpression undefined = new();
                if (ExpectedAndAdvance(SyntaxKind.PlusToken))
                {
                    Match(SyntaxKind.OpenBracketsToken);
                    return new SequenceExpression(new ConcatenatedSequenceExpression(undefined, ParseSequence()));
                }
                return new SequenceExpression(new ConcatenatedSequenceExpression(sequence_, undefined));
            }
            Match(SyntaxKind.OpenBracketsToken);
            return new SequenceExpression(new ConcatenatedSequenceExpression(sequence_, ParseSequence()));
        }

        private Samples ParseSamples()
        {
            Advance();
            MatchAndAdvance(SyntaxKind.OpenParenthesisToken);
            MatchAndAdvance(SyntaxKind.CloseParenthesisToken);
            return new Samples();
        }

        private Randoms ParseRandoms()
        {
            Advance();
            MatchAndAdvance(SyntaxKind.OpenParenthesisToken);
            MatchAndAdvance(SyntaxKind.CloseParenthesisToken);
            return new Randoms();
        }

        private RandomPoints ParsePoints()
        {
            Advance();
            MatchAndAdvance(SyntaxKind.OpenParenthesisToken);
            ExpressionSyntax figure = ParseExpression();
            MatchAndAdvance(SyntaxKind.CloseParenthesisToken);
            return new RandomPoints(figure);
        }

        private MeasureExpression ParseMeasure()
        {
            Advance();
            MatchAndAdvance(SyntaxKind.OpenParenthesisToken);
            ExpressionSyntax point1 = ParseExpression();
            MatchAndAdvance(SyntaxKind.SeparatorToken);
            ExpressionSyntax point2 = ParseExpression();
            MatchAndAdvance(SyntaxKind.CloseParenthesisToken);
            return new MeasureExpression(point1, point2);
        }

        private ExpressionSyntax ParseIntersect()
        {
            Advance();
            MatchAndAdvance(SyntaxKind.OpenParenthesisToken);
            ExpressionSyntax figure1 = ParseExpression();
            MatchAndAdvance(SyntaxKind.SeparatorToken);
            ExpressionSyntax figure2 = ParseExpression();
            MatchAndAdvance(SyntaxKind.CloseParenthesisToken);
            return new IntersectExpression(figure1, figure2);
        }

        private SyntaxNode ParseDraw()
        {
            Advance();
            if (ExpectedAndAdvance(SyntaxKind.OpenBracketsToken)) return ParseDrawSequence();
            SyntaxNode expression = ParseStmt();
            if (Expected(SyntaxKind.String))
            {
                SyntaxToken name = Advance();
                MatchAndAdvance(SyntaxKind.SemicolonToken);
                return new DrawStatement(name.Lexeme!, expression, colorStack.Peek());
            }
            MatchAndAdvance(SyntaxKind.SemicolonToken);
            return new DrawStatement("", expression, colorStack.Peek());
        }

        private SyntaxNode ParseDrawSequence()
        {
            Match(SyntaxKind.ID);
            List<VariableExpression> variables = new()
            {
                new VariableExpression(Advance())
            };
            if (ExpectedAndAdvance(SyntaxKind.CloseBracketsToken)) return new DrawStatement("", variables, colorStack.Peek());
            else
            {
                do
                {
                    MatchAndAdvance(SyntaxKind.SeparatorToken);
                    Match(SyntaxKind.ID);
                    variables.Add(new VariableExpression(Advance()));

                } while (!ExpectedAndAdvance(SyntaxKind.CloseBracketsToken));
                if (Expected(SyntaxKind.String))
                {
                    SyntaxToken name = Advance();
                    MatchAndAdvance(SyntaxKind.SemicolonToken);
                    return new DrawStatement(name.Lexeme!, variables, colorStack.Peek());
                }
                MatchAndAdvance(SyntaxKind.SemicolonToken);
                return new DrawStatement("", variables, colorStack.Peek());
            }
        }

        private Count ParseCount()
        {
            Advance();
            MatchAndAdvance(SyntaxKind.OpenParenthesisToken);
            ExpressionSyntax sequence = ParseExpression();
            MatchAndAdvance(SyntaxKind.CloseParenthesisToken);
            return new Count(sequence);
        }

        private SyntaxNode ParseSegment()
        {
            if (nextToken.Type == SyntaxKind.OpenParenthesisToken) return ParseSegmentExpression();
            Advance();
            if (ExpectedAndAdvance(SyntaxKind.SequenceToken))
            {
                Match(SyntaxKind.ID);
                SyntaxToken id = Advance();
                MatchAndAdvance(SyntaxKind.SemicolonToken);
                return new SegmentStatement(id, true);
            }
            Match(SyntaxKind.ID);
            SyntaxToken Id = Advance();
            MatchAndAdvance(SyntaxKind.SemicolonToken);
            return new SegmentStatement(Id, false);
        }

        private SegmentExpression ParseSegmentExpression()
        {
            Advance();
            MatchAndAdvance(SyntaxKind.OpenParenthesisToken);
            ExpressionSyntax start = ParseExpression();
            MatchAndAdvance(SyntaxKind.SeparatorToken);
            ExpressionSyntax end = ParseExpression();
            MatchAndAdvance(SyntaxKind.CloseParenthesisToken);
            return new SegmentExpression(start, end);
        }

        private SyntaxNode ParseRay()
        {
            if (nextToken.Type == SyntaxKind.OpenParenthesisToken) return ParseRayExpression();
            Advance();
            if (ExpectedAndAdvance(SyntaxKind.SequenceToken))
            {
                Match(SyntaxKind.ID);
                SyntaxToken id = Advance();
                MatchAndAdvance(SyntaxKind.SemicolonToken);
                return new RayStatement(id, true);
            }
            Match(SyntaxKind.ID);
            SyntaxToken Id = Advance();
            MatchAndAdvance(SyntaxKind.SemicolonToken);
            return new RayStatement(Id, false);
        }

        private RayExpression ParseRayExpression()
        {
            Advance();
            MatchAndAdvance(SyntaxKind.OpenParenthesisToken);
            ExpressionSyntax start = ParseExpression();
            MatchAndAdvance(SyntaxKind.SeparatorToken);
            ExpressionSyntax point2 = ParseExpression();
            MatchAndAdvance(SyntaxKind.CloseParenthesisToken);
            return new RayExpression(start, point2);
        }

        private SyntaxNode ParsePoint()
        {
            Advance();
            if (ExpectedAndAdvance(SyntaxKind.SequenceToken))
            {
                Match(SyntaxKind.ID);
                SyntaxToken id = Advance();
                MatchAndAdvance(SyntaxKind.SemicolonToken);
                return new PointStatement(id, true);
            }
            Match(SyntaxKind.ID);
            SyntaxToken Id = Advance();
            MatchAndAdvance(SyntaxKind.SemicolonToken);
            return new PointStatement(Id, false);
        }

        private SyntaxNode ParseLine()
        {
            if (nextToken.Type == SyntaxKind.OpenParenthesisToken) return ParseLineExpression();
            Advance();
            if (ExpectedAndAdvance(SyntaxKind.SequenceToken))
            {
                Match(SyntaxKind.ID);
                SyntaxToken id = Advance();
                MatchAndAdvance(SyntaxKind.SemicolonToken);
                return new LineStatement(id, true);
            }
            Match(SyntaxKind.ID);
            SyntaxToken Id = Advance();
            MatchAndAdvance(SyntaxKind.SemicolonToken);
            return new LineStatement(Id, false);

        }

        private LineExpression ParseLineExpression()
        {
            Advance();
            MatchAndAdvance(SyntaxKind.OpenParenthesisToken);
            ExpressionSyntax point1 = ParseExpression();
            MatchAndAdvance(SyntaxKind.SeparatorToken);
            ExpressionSyntax point2 = ParseExpression();
            MatchAndAdvance(SyntaxKind.CloseParenthesisToken);
            return new LineExpression(point1, point2);
        }

        private SyntaxNode ParseCircle()
        {
            if (nextToken.Type == SyntaxKind.OpenParenthesisToken) return ParseCircleExpression();
            Advance();
            if (ExpectedAndAdvance(SyntaxKind.SequenceToken))
            {
                Match(SyntaxKind.ID);
                SyntaxToken id = Advance();
                MatchAndAdvance(SyntaxKind.SemicolonToken);
                return new CircleStatement(id, true);
            }
            Match(SyntaxKind.ID);
            SyntaxToken Id = Advance();
            MatchAndAdvance(SyntaxKind.SemicolonToken);
            return new CircleStatement(Id, false);
        }

        private CircleExpression ParseCircleExpression()
        {
            Advance();
            MatchAndAdvance(SyntaxKind.OpenParenthesisToken);
            ExpressionSyntax center = ParseExpression();
            MatchAndAdvance(SyntaxKind.SeparatorToken);
            ExpressionSyntax measure = ParseExpression();
            MatchAndAdvance(SyntaxKind.CloseParenthesisToken);
            return new CircleExpression(center, measure);
        }

        private SyntaxNode ParseArc()
        {
            if (nextToken.Type == SyntaxKind.OpenParenthesisToken) return ParseArcExpression();
            Advance();
            if (ExpectedAndAdvance(SyntaxKind.SequenceToken))
            {
                Match(SyntaxKind.ID);
                SyntaxToken id = Advance();
                MatchAndAdvance(SyntaxKind.SemicolonToken);
                return new ArcStatement(id, true);
            }
            Match(SyntaxKind.ID);
            SyntaxToken Id = Advance();
            MatchAndAdvance(SyntaxKind.SemicolonToken);
            return new ArcStatement(Id, false);
        }

        private ArcExpression ParseArcExpression()
        {
            Advance();
            MatchAndAdvance(SyntaxKind.OpenParenthesisToken);
            ExpressionSyntax center = ParseExpression();
            MatchAndAdvance(SyntaxKind.SeparatorToken);
            ExpressionSyntax point1 = ParseExpression();
            MatchAndAdvance(SyntaxKind.SeparatorToken);
            ExpressionSyntax point2 = ParseExpression();
            MatchAndAdvance(SyntaxKind.SeparatorToken);
            ExpressionSyntax measure = ParseExpression();
            MatchAndAdvance(SyntaxKind.CloseParenthesisToken);
            return new ArcExpression(center, point1, point2, measure);
        }

        private SyntaxNode ParseRestore()
        {
            Advance();
            colorStack.Pop();
            MatchAndAdvance(SyntaxKind.SemicolonToken);
            return new Empty();
        }

        private SyntaxNode ParseColor()
        {
            Advance();
            if (Expected(SyntaxKind.ColorBlackToken)) colorStack.Push(Color.BLACK);
            else if (Expected(SyntaxKind.ColorBlueToken)) colorStack.Push(Color.BLUE);
            else if (Expected(SyntaxKind.ColorCyanToken)) colorStack.Push(Color.CYAN);
            else if (Expected(SyntaxKind.ColorGrayToken)) colorStack.Push(Color.GRAY);
            else if (Expected(SyntaxKind.ColorGreenToken)) colorStack.Push(Color.GREEN);
            else if (Expected(SyntaxKind.ColorMagentaToken)) colorStack.Push(Color.MAGENTA);
            else if (Expected(SyntaxKind.ColorRedToken)) colorStack.Push(Color.RED);
            else if (Expected(SyntaxKind.ColorWhiteToken)) colorStack.Push(Color.WHITE);
            else if (Expected(SyntaxKind.ColorYellowToken)) colorStack.Push(Color.YELLOW);
            else throw new SyntaxError(currentToken.Line, currentToken.Column, "Se esperaba la declaración de un color");
            Advance();
            MatchAndAdvance(SyntaxKind.SemicolonToken);
            return new Empty();
        }

        private SyntaxNode ParseId()
        {
            if (nextToken.Type == SyntaxKind.OpenParenthesisToken) return ParseFunctionCall();
            if (nextToken.Type == SyntaxKind.SeparatorToken)
            {
                List<SyntaxToken> names = new()
                {
                    Advance()
                };
                do
                {
                    MatchAndAdvance(SyntaxKind.SeparatorToken);
                    if (Expected(SyntaxKind.ID, SyntaxKind.UnderscoreToken, SyntaxKind.RestToken))
                    {
                        SyntaxToken name = Advance();
                        names.Add(name);
                    }
                    else throw new SyntaxError(currentToken.Line, currentToken.Column, "Se esperaba una declaración de variable");
                } while (!ExpectedAndAdvance(SyntaxKind.EqualToken));
                ExpressionSyntax body = ParseExpression();
                MatchAndAdvance(SyntaxKind.SemicolonToken);
                return new AssignationStatement(names, body);
            }
            if (ExpectedAndAdvance(SyntaxKind.UnderscoreToken)) return new Empty();
            if (Expected(SyntaxKind.RestToken)) return new SequenceExpression(Advance());
            if (current != 0 && previousToken.Type == SyntaxKind.DrawToken)
            {
                return new VariableExpression(Advance());
            }
            SyntaxToken id = Advance();
            MatchAndAdvance(SyntaxKind.EqualToken);
            ExpressionSyntax body_ = ParseExpression();
            MatchAndAdvance(SyntaxKind.SemicolonToken);
            return new AssignationStatement(id, body_);

        }

        #region Métodos auxiliares
        private bool MatchAndAdvance(SyntaxKind type)
        {
            if (type == currentToken.Type)
            {
                Advance();
                return true;
            }
            throw new SyntaxError(previousToken.Line, previousToken.Column, "Olvidó poner un " + type.ToString() + " después de " + previousToken.Lexeme);
        }
        private SyntaxToken Advance()
        {
            current++;
            return previousToken;
        }
        private bool Match(SyntaxKind type)
        {
            if (type == currentToken.Type)
            {
                return true;
            }
            else
                throw new SyntaxError(previousToken.Line, previousToken.Column, "Olvidó poner un " + type.ToString() + " después de " + previousToken.Lexeme);
        }
        private bool Expected(params SyntaxKind[] types)
        {
            foreach (var type in types)
            {
                if (type == currentToken.Type)
                {
                    return true;
                }
            }
            return false;
        }
        private bool ExpectedAndAdvance(params SyntaxKind[] types)
        {
            foreach (var type in types)
            {
                if (type == currentToken.Type)
                {
                    Advance();
                    return true;
                }
            }
            return false;
        }
        private List<ExpressionSyntax> GetArguments()
        {
            List<ExpressionSyntax> arguments = new();
            
            if (Expected(SyntaxKind.CloseParenthesisToken)) return arguments;

            do
            {
                if (Match(SyntaxKind.ID))
                {
                    arguments.Add(new VariableExpression(currentToken));
                    Advance();
                }
            } while (ExpectedAndAdvance(SyntaxKind.SeparatorToken));
            return arguments;
        }
        #endregion
    }
}
