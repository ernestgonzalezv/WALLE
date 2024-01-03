

namespace Geo_Wall_E
{
    public class Parser
    {
        private Scope Scope = new Scope();
        private ColorSet ColorStack = new ColorSet();
        private readonly List<SyntaxToken> Tokens;
        private int current = 0;
        public Parser(List<SyntaxToken> tokens)
        {
            Tokens = tokens;
        }
        private bool IsTheEnd => CurrentToken.Kind == SyntaxKind.EOFToken;
        private SyntaxToken CurrentToken => Tokens[current];
        private SyntaxToken PreviousToken => Tokens[current - 1];
        private SyntaxToken NextToken => Tokens[current + 1];

        public List<SyntaxNode> ParsingTheWholeThing()
        {
            try
            {
                return ParseExpressionSyntax();
            }
            catch (SyntaxError Error)
            {
                Error.HandleException();
                return null!;
            }
        }

        private List<SyntaxNode> ParseExpressionSyntax()
        {
            var Nodos = new List<SyntaxNode>();
            if (Tokens[0].Kind == SyntaxKind.ImportToken)
            {
                AdvanceToken();
                Match(SyntaxKind.StringToken);
                string Name = Tokens[1].Lexeme!.Replace("\"", "");
                string path = Path.GetFullPath(Name);
                if (!Path.Exists(path)) throw new FileNotFoundException();
                string Text = System.Text.Encoding.Default.GetString(File.ReadAllBytes(path));
                Lexer lexer = new(Text);
                Parser Parser = new(lexer.Lexing());
                Nodos = Parser.ParsingTheWholeThing();
            }
            while (!IsTheEnd)
            {
                Nodos.Add(ParseExpressionSyntaxFunctionExpressionSyntax());
                ExpectedAndAdvance(SyntaxKind.SemicolonToken);
            }
            return Nodos; // finally :)
        }

        private SyntaxNode ParseExpressionSyntaxFunctionExpressionSyntax()
        {
            if (Expected(SyntaxKind.IdentifierToken))
            {
                if (NextToken.Kind == SyntaxKind.OpenParenthesisToken)
                {
                    var name = AdvanceToken();
                    if (!Scope.Variables.ContainsKey(name.Lexeme!))
                    {
                        Scope.Search(new FunctionStatement(name, null!, null!));
                        AdvanceToken();
                        List<ExpressionSyntax> parameters = GetFunctionArgs();
                        MatchToken(SyntaxKind.CloseParenthesisToken);
                        MatchToken(SyntaxKind.EqualToken);
                        ExpressionSyntax body = ParseExpressionSyntaxExpression();
                        MatchToken(SyntaxKind.SemicolonToken);
                        FunctionStatement function = new(name, parameters, body);
                        Scope.Search(function);
                        return function;
                    }
                    current--;
                    return ParseExpressionSyntaxFunctionExpressionSyntaxCallingExpressionSyntax();
                }
                return ParseExpressionSyntaxIdentifier();
            }
            return ParseExpressionSyntaxStatementExpressionSyntax();
        }

        private SyntaxNode ParseExpressionSyntaxStatementExpressionSyntax()
        {
            return CurrentToken.Kind switch
            {
                SyntaxKind.SegmentToken => ParseExpressionSyntaxSegmentExpression(),
                SyntaxKind.UnderscoreToken => ParseExpressionSyntaxIdentifier(),
                SyntaxKind.ColorToken => ParseExpressionSyntaxColorExpression(),
                SyntaxKind.CountToken => ParseExpressionSyntaxCountExpressionSyntax(),
                SyntaxKind.DrawToken => ParseExpressionSyntaxDrawingSequenceTypeExpression(),
                SyntaxKind.IdentifierToken => ParseExpressionSyntaxIdentifier(),
                SyntaxKind.ArcToken => ParseExpressionSyntaxArcExpression(),
                SyntaxKind.CircleToken => ParseExpressionSyntaxCircleExpression(),
                SyntaxKind.LineToken => ParseExpressionSyntaxLineExpression(),
                SyntaxKind.PointToken => ParseExpressionSyntaxPointExpression(),
                SyntaxKind.RayToken => ParserayExpression(),
                SyntaxKind.RestToken => ParseExpressionSyntaxIdentifier(),
                SyntaxKind.RestoreToken => ParserestoreExpressionSyntax(),
                _ => ParseExpressionSyntaxExpression(),
            };
        }

        private ExpressionSyntax ParseExpressionSyntaxExpression()
        {
            return CurrentToken.Kind switch
            {
                SyntaxKind.ArcToken => ParseExpressionSyntaxArcExpressionSyntax(),
                SyntaxKind.CircleToken => ParseExpressionSyntaxCircleExpressionSyntax(),
                //SyntaxKind.IdentifierToken => ParseExpressionSyntaxFunctionExpressionSyntaxCallingExpressionSyntax(),
                SyntaxKind.IfToken => ParseExpressionSyntaxConditionalExpressionSyntax(),
                SyntaxKind.LetToken => ParseExpressionSyntaxLetInExpressionSyntax(),
                SyntaxKind.LineToken => ParseExpressionSyntaxLineExpressionSyntax(),
                SyntaxKind.MeasureToken => ParseExpressionSyntaxMeasureExpressionSyntax(),
                SyntaxKind.OpenBracketsToken => ParseExpressionSyntaxSequenceTypeExpression(),
                SyntaxKind.OpenParenthesisToken => ParseExpressionSyntaxBetweenParenthesisExpressionSyntax(),
                SyntaxKind.PointsToken => ParserandomPointsExpression(),
                SyntaxKind.RandomsToken => ParserandomExpression(),
                SyntaxKind.RayToken => ParserayExpressionExpressionSyntax(),
                SyntaxKind.RestToken => new VariableExpressionSyntax(AdvanceToken()),
                SyntaxKind.SamplesToken => ParseExpressionSyntaxSamplesExpression(),
                SyntaxKind.SegmentToken => ParseExpressionSyntaxSegmentExpressionSyntax(),
                SyntaxKind.IntersectionToken => ParseExpressionSyntaxIntersectionExpressionSyntax(),
                _ => ParseExpressionSyntaxLogicalExpressionSyntax(),
            };
        }

        private ExpressionSyntax ParseExpressionSyntaxFunctionExpressionSyntaxCallingExpressionSyntax()
        {
            var functionsname = AdvanceToken();
            if (ExpectedAndAdvance(SyntaxKind.OpenParenthesisToken))
            {
                if (Scope.Variables.ContainsKey(functionsname.Lexeme!))
                {
                    //MatchToken(SyntaxKind.OpenParenthesisToken);
                    List<ExpressionSyntax> args = new();
                    if (!ExpectedAndAdvance(SyntaxKind.CloseParenthesisToken))
                    {
                        do
                        {
                            if (!ExpectedAndAdvance(SyntaxKind.CloseParenthesisToken)){args.Add(ParseExpressionSyntaxExpression());}
                            else throw new SyntaxError(CurrentToken.Line, CurrentToken.Column, "Se esperaba la declaración de un argumento para la función en lugar de " + CurrentToken.Lexeme);
                        } while (ExpectedAndAdvance(SyntaxKind.SeparatorToken));
                    }
                    MatchToken(SyntaxKind.CloseParenthesisToken);
                    return new FunctionCallingExpressionSyntax(functionsname, args, null!);
                }
                throw new SyntaxError(CurrentToken.Line, CurrentToken.Column, "La función " + CurrentToken.Lexeme + " no ha sIdentifierTokeno declarada");
            }
            return new VariableExpressionSyntax(functionsname);
        }

        private ExpressionSyntax ParseExpressionSyntaxLogicalExpressionSyntax()
        {
            var LogicalExpression = ParseExpressionSyntaxEqualExpressionSyntax();
            //Mientras sea '|' o '&' continuará el ciclo 
            while (Expected(SyntaxKind.AndToken, SyntaxKind.OrToken))
            {
                SyntaxToken OperatorToken = AdvanceToken();
                var RightExpressionSyntax = ParseExpressionSyntaxEqualExpressionSyntax();
                LogicalExpression = new BinaryExpressionSyntax(LogicalExpression, OperatorToken, RightExpressionSyntax);
            }
            return LogicalExpression;
        }

        private ExpressionSyntax ParseExpressionSyntaxEqualExpressionSyntax()
        {
            var EqualExpressionSyntax = ParseExpressionSyntaxComparationExpressionSyntax();
            int temp = 0;
            while (Expected(SyntaxKind.DoubleEqualToken, SyntaxKind.NoEqualToken))
            {
                SyntaxToken OperatorToken = AdvanceToken();
                if (temp >= 1) throw new SyntaxError(CurrentToken.Line, CurrentToken.Column, "No es posible utilizar el OperatorTokenador " + CurrentToken.Lexeme + " más de una vez en una misma condición");
                var RightExpressionSyntax = ParseExpressionSyntaxComparationExpressionSyntax();
                EqualExpressionSyntax = new BinaryExpressionSyntax(EqualExpressionSyntax, OperatorToken, RightExpressionSyntax);
                temp++;
            }
            return EqualExpressionSyntax;
        }

        private ExpressionSyntax ParseExpressionSyntaxComparationExpressionSyntax()
        {
            var ComparationExpression = ParseExpressionSyntaxTermExpressionSyntax();
            var temp = 0;
            while (Expected( SyntaxKind.LessToken, SyntaxKind.MoreOrEqualToken, SyntaxKind.MoreToken, SyntaxKind.LessOrEqualToken))
            {
                var OperatorToken = AdvanceToken();
                if (temp >= 1) throw new SyntaxError(CurrentToken.Line, CurrentToken.Column, "No es posible utilizar el OperatorTokenador " + CurrentToken.Lexeme + " más de una vez en una misma condición");
                var RightExpressionSyntax = ParseExpressionSyntaxTermExpressionSyntax();
                ComparationExpression = new BinaryExpressionSyntax(ComparationExpression, OperatorToken, RightExpressionSyntax);
                temp++;
            }
            return ComparationExpression;
        }

        private ExpressionSyntax ParseExpressionSyntaxTermExpressionSyntax()
        {
            ExpressionSyntax TermExpression = Factor();
            while (Expected(SyntaxKind.MinusToken, SyntaxKind.PlusToken))
            {
                SyntaxToken OperatorToken = AdvanceToken();
                ExpressionSyntax RightExpressionSyntax = Factor();
                TermExpression = new BinaryExpressionSyntax(TermExpression, OperatorToken, RightExpressionSyntax);
            }
            return TermExpression;
        }

        private ExpressionSyntax Factor()
        {
            ExpressionSyntax factorExpression = ParseExpressionSyntaxPowerExpressionSyntax();
            //Mientras sea '/' , '*' o '%' continuará el ciclo 
            while (Expected(SyntaxKind.SlashToken,SyntaxKind.StarToken, SyntaxKind.ModuToken))
            {
                SyntaxToken OperatorToken = AdvanceToken();
                ExpressionSyntax RightExpressionSyntax = ParseExpressionSyntaxPowerExpressionSyntax();
                factorExpression = new BinaryExpressionSyntax(factorExpression, OperatorToken, RightExpressionSyntax);
            }
            return factorExpression;
        }

        private ExpressionSyntax ParseExpressionSyntaxPowerExpressionSyntax()
        {
            ExpressionSyntax PowerExpression = ParseExpressionSyntaxUnaryExpressionSyntaxSyntax();
            if (Expected(SyntaxKind.PowToken))
            {
                SyntaxToken OperatorToken = AdvanceToken();
                ExpressionSyntax RightExpressionSyntax = ParseExpressionSyntaxPowerExpressionSyntax();
                PowerExpression = new BinaryExpressionSyntax(PowerExpression, OperatorToken, RightExpressionSyntax);
            }
            return PowerExpression;
        }

        private ExpressionSyntax ParseExpressionSyntaxUnaryExpressionSyntaxSyntax()
        {
            if (Expected(SyntaxKind.MinusToken,SyntaxKind.NotToken,  SyntaxKind.PlusToken))
            {
                SyntaxToken OperatorToken = AdvanceToken();
                return new UnaryExpressionSyntax(OperatorToken, ParseExpressionSyntaxUnaryExpressionSyntaxSyntax());
            }
            return ParseExpressionSyntaxMathFunctionExpressionSyntax();
        }

        private ExpressionSyntax ParseExpressionSyntaxMathFunctionExpressionSyntax()
        {
            if (Expected(SyntaxKind.LogToken))
            {
                var OperatorTokenlog = AdvanceToken();
                Match(SyntaxKind.OpenParenthesisToken);
                var LogNumber = ParseExpressionSyntaxBetweenParenthesisExpressionSyntax();
                var LogBase = ParseExpressionSyntaxExpression();
                return new LogExpression(OperatorTokenlog, LogNumber, LogBase);
            }
            if (Expected( SyntaxKind.CosToken , SyntaxKind.SinToken, SyntaxKind.ExpoToken, SyntaxKind.SqrtToken))
            {
                var OperatorToken = AdvanceToken();
                MatchToken(SyntaxKind.OpenParenthesisToken);
                var expression = new UnaryExpressionSyntax(OperatorToken, ParseExpressionSyntaxExpression());
                MatchToken(SyntaxKind.CloseParenthesisToken);
                return expression;
            }
            return ParseExpressionSyntaxLiteralExpressionSyntax();
        }

        private ExpressionSyntax ParseExpressionSyntaxLiteralExpressionSyntax()
        {
            switch (CurrentToken.Kind)
            {
                case SyntaxKind.IdentifierToken: return ParseExpressionSyntaxFunctionExpressionSyntaxCallingExpressionSyntax();
                case SyntaxKind.NumberToken: AdvanceToken(); return new NumberExpressionSyntax(PreviousToken);
                case SyntaxKind.PIToken: AdvanceToken(); return new PIExpressionSyntax((string)PreviousToken.Value!);
                case SyntaxKind.EToken: AdvanceToken(); return new EExpression((string)PreviousToken.Value!);
                case SyntaxKind.StringToken: AdvanceToken(); return new StringExpressionSyntax(PreviousToken);
                case SyntaxKind.UndefinedToken: AdvanceToken(); return new UndefinedExpressionSyntax();
                default: throw new SyntaxError(CurrentToken.Line, CurrentToken.Column, "");
            }
        }

        private ConditionalExpressionSyntax ParseExpressionSyntaxConditionalExpressionSyntax()
        {
            AdvanceToken();
            ExpressionSyntax IfCondition = ParseExpressionSyntaxExpression();
            MatchToken(SyntaxKind.ThenToken);
            ExpressionSyntax ResponseToCondition = ParseExpressionSyntaxExpression();
            MatchToken(SyntaxKind.ElseToken);
            ExpressionSyntax ElseResponse = ParseExpressionSyntaxExpression();
            return new ConditionalExpressionSyntax(IfCondition, ResponseToCondition, ElseResponse);
        }

        private ExpressionSyntax ParseExpressionSyntaxLetInExpressionSyntax()
        {
            AdvanceToken();
            
            var Variables = new List<SyntaxShapes>();
            do
            {
                Variables.Add((SyntaxShapes)ParseExpressionSyntaxStatementExpressionSyntax());
            } while (!Expected(SyntaxKind.InToken));
            
            MatchToken(SyntaxKind.InToken);
            ExpressionSyntax AfterIn = ParseExpressionSyntaxExpression();
            //MatchToken(SyntaxKind.SemicolonToken);
            return new LetInExpressionSyntax(Variables, AfterIn);
        }

        private BetweenParenExpressionSyntax ParseExpressionSyntaxBetweenParenthesisExpressionSyntax()
        {
            AdvanceToken();
            var expressionBetween = new BetweenParenExpressionSyntax(ParseExpressionSyntaxExpression());
            Match(SyntaxKind.CloseParenthesisToken);
            return expressionBetween;
        }

        private SequenceTypeExpression ParseExpressionSyntaxSequenceTypeExpression()
        {
            AdvanceToken();
            if (ExpectedAndAdvance(SyntaxKind.CloseBracketsToken))
            {
                var empty = new Empty();
                return new SequenceTypeExpression(empty);
            }
            if (NextToken.Kind == SyntaxKind.ThreeDotsToken)
            {
                Match(SyntaxKind.NumberToken);
                SyntaxToken StartingToken = AdvanceToken();
                AdvanceToken();
                if (Expected(SyntaxKind.NumberToken))
                {
                    SyntaxToken end = AdvanceToken();
                    MatchToken(SyntaxKind.CloseBracketsToken);
                    return new SequenceTypeExpression(StartingToken, end);
                }
                MatchToken(SyntaxKind.CloseBracketsToken);
                return new SequenceTypeExpression(StartingToken);
            }
            List<ExpressionSyntax> mySequence = new List<ExpressionSyntax> (){ParseExpressionSyntaxExpression()};
            while (ExpectedAndAdvance(SyntaxKind.SeparatorToken))
            {
                mySequence.Add(ParseExpressionSyntaxExpression());
            }
            MatchToken(SyntaxKind.CloseBracketsToken);
            if (ExpectedAndAdvance(SyntaxKind.PlusToken)) return ParseExpressionSyntaxConcatenatedSequenceTypeExpression(mySequence);
            return new SequenceTypeExpression(mySequence);
        }

        private SequenceTypeExpression ParseExpressionSyntaxConcatenatedSequenceTypeExpression(List<ExpressionSyntax> sequence)
        {
            SequenceTypeExpression CurrentSequence = new(sequence);
            if (Expected(SyntaxKind.UndefinedToken))
            {
                AdvanceToken();
                var UndefinedExpressionSyntax = new UndefinedExpressionSyntax ();
                if (ExpectedAndAdvance(SyntaxKind.PlusToken))
                {
                    Match(SyntaxKind.OpenBracketsToken);
                    return new SequenceTypeExpression(new ConcatenatedSequenceTypeExpression(UndefinedExpressionSyntax, ParseExpressionSyntaxSequenceTypeExpression()));
                }
                return new SequenceTypeExpression(new ConcatenatedSequenceTypeExpression(CurrentSequence, UndefinedExpressionSyntax));
            }
            Match(SyntaxKind.OpenBracketsToken);
            return new SequenceTypeExpression(new ConcatenatedSequenceTypeExpression(CurrentSequence, ParseExpressionSyntaxSequenceTypeExpression()));
        }

        private SamplesExpressionSyntax ParseExpressionSyntaxSamplesExpression()
        {
            AdvanceToken();
            MatchToken(SyntaxKind.OpenParenthesisToken);
            MatchToken(SyntaxKind.CloseParenthesisToken);
            return new SamplesExpressionSyntax();
        }

        private Randoms ParserandomExpression()
        {
            AdvanceToken();
            MatchToken(SyntaxKind.OpenParenthesisToken);
            MatchToken(SyntaxKind.CloseParenthesisToken);
            return new Randoms();
        }

        private RandomPointExpressionSyntax ParserandomPointsExpression()
        {
            AdvanceToken();
            MatchToken(SyntaxKind.OpenParenthesisToken);
            ExpressionSyntax graphics = ParseExpressionSyntaxExpression();
            MatchToken(SyntaxKind.CloseParenthesisToken);
            return new RandomPointExpressionSyntax(graphics);
        }

        private MeasureExpression ParseExpressionSyntaxMeasureExpressionSyntax()
        {
            AdvanceToken();
            MatchToken(SyntaxKind.OpenParenthesisToken);
            ExpressionSyntax point1 = ParseExpressionSyntaxExpression();
            MatchToken(SyntaxKind.SeparatorToken);
            ExpressionSyntax point2 = ParseExpressionSyntaxExpression();
            MatchToken(SyntaxKind.CloseParenthesisToken);
            return new MeasureExpression(point1, point2);
        }

        private ExpressionSyntax ParseExpressionSyntaxIntersectionExpressionSyntax()
        {
            AdvanceToken();
            MatchToken(SyntaxKind.OpenParenthesisToken);
            ExpressionSyntax graphics = ParseExpressionSyntaxExpression();
            MatchToken(SyntaxKind.SeparatorToken);
            ExpressionSyntax Graphics = ParseExpressionSyntaxExpression();
            MatchToken(SyntaxKind.CloseParenthesisToken);
            return new IntersectExpression(graphics, Graphics);
        }

        private SyntaxNode ParseExpressionSyntaxDrawingSequenceTypeExpression()
        {
            AdvanceToken();
            if (ExpectedAndAdvance(SyntaxKind.OpenBracketsToken)) return ParseExpressionSyntaxDrawingSequenceTypeExpressionSyntax();
            SyntaxNode expression = ParseExpressionSyntaxStatementExpressionSyntax();
            if (Expected(SyntaxKind.StringToken))
            {
                SyntaxToken name = AdvanceToken();
                MatchToken(SyntaxKind.SemicolonToken);
                return new DrawStatement(name.Lexeme!, expression, ColorStack.PeekColorStack());
            }
            MatchToken(SyntaxKind.SemicolonToken);
            return new DrawStatement("", expression, ColorStack.PeekColorStack());
        }

        private SyntaxNode ParseExpressionSyntaxDrawingSequenceTypeExpressionSyntax()
        {
            Match(SyntaxKind.IdentifierToken);
            List<VariableExpressionSyntax> variables = new()
            {
                new VariableExpressionSyntax(AdvanceToken())
            };
            if (ExpectedAndAdvance(SyntaxKind.CloseBracketsToken)) return new DrawStatement("", variables, ColorStack.PeekColorStack());
            else
            {
                do
                {
                    MatchToken(SyntaxKind.SeparatorToken);
                    Match(SyntaxKind.IdentifierToken);
                    variables.Add(new VariableExpressionSyntax(AdvanceToken()));

                } while (!ExpectedAndAdvance(SyntaxKind.CloseBracketsToken));
                if (Expected(SyntaxKind.StringToken))
                {
                    SyntaxToken name = AdvanceToken();
                    MatchToken(SyntaxKind.SemicolonToken);
                    return new DrawStatement(name.Lexeme!, variables, ColorStack.PeekColorStack());
                }
                MatchToken(SyntaxKind.SemicolonToken);
                return new DrawStatement("", variables, ColorStack.PeekColorStack());
            }
        }

        private Count ParseExpressionSyntaxCountExpressionSyntax()
        {
            AdvanceToken();
            MatchToken(SyntaxKind.OpenParenthesisToken);
            ExpressionSyntax sequence = ParseExpressionSyntaxExpression();
            MatchToken(SyntaxKind.CloseParenthesisToken);
            return new Count(sequence);
        }

        private SyntaxNode ParseExpressionSyntaxSegmentExpression()
        {
            if (NextToken.Kind == SyntaxKind.OpenParenthesisToken) return ParseExpressionSyntaxSegmentExpressionSyntax();
            AdvanceToken();
            if (ExpectedAndAdvance(SyntaxKind.SequenceToken))
            {
                Match(SyntaxKind.IdentifierToken);
                SyntaxToken ID = AdvanceToken();
                MatchToken(SyntaxKind.SemicolonToken);
                return new SegmentStatement(ID, true);
            }
            Match(SyntaxKind.IdentifierToken);
            SyntaxToken IdentifierToken = AdvanceToken();
            MatchToken(SyntaxKind.SemicolonToken);
            return new SegmentStatement(IdentifierToken, false);
        }

        private SegmentExpression ParseExpressionSyntaxSegmentExpressionSyntax()
        {
            AdvanceToken();
            MatchToken(SyntaxKind.OpenParenthesisToken);
            ExpressionSyntax StartingToken = ParseExpressionSyntaxExpression();
            MatchToken(SyntaxKind.SeparatorToken);
            ExpressionSyntax end = ParseExpressionSyntaxExpression();
            MatchToken(SyntaxKind.CloseParenthesisToken);
            return new SegmentExpression(StartingToken, end);
        }

        private SyntaxNode ParserayExpression()
        {
            if (NextToken.Kind == SyntaxKind.OpenParenthesisToken) return ParserayExpressionExpressionSyntax();
            AdvanceToken();
            if (ExpectedAndAdvance(SyntaxKind.SequenceToken))
            {
                Match(SyntaxKind.IdentifierToken);
                SyntaxToken ID = AdvanceToken();
                MatchToken(SyntaxKind.SemicolonToken);
                return new RayStatement(ID, true);
            }
            Match(SyntaxKind.IdentifierToken);
            SyntaxToken IdentifierToken = AdvanceToken();
            MatchToken(SyntaxKind.SemicolonToken);
            return new RayStatement(IdentifierToken, false);
        }

        private RayExpression ParserayExpressionExpressionSyntax()
        {
            AdvanceToken();
            MatchToken(SyntaxKind.OpenParenthesisToken);
            ExpressionSyntax StartingToken = ParseExpressionSyntaxExpression();
            MatchToken(SyntaxKind.SeparatorToken);
            ExpressionSyntax point2 = ParseExpressionSyntaxExpression();
            MatchToken(SyntaxKind.CloseParenthesisToken);
            return new RayExpression(StartingToken, point2);
        }

        private SyntaxNode ParseExpressionSyntaxPointExpression()
        {
            AdvanceToken();
            if (ExpectedAndAdvance(SyntaxKind.SequenceToken))
            {
                Match(SyntaxKind.IdentifierToken);
                SyntaxToken ID = AdvanceToken();
                MatchToken(SyntaxKind.SemicolonToken);
                return new PointStatement(ID, true);
            }
            Match(SyntaxKind.IdentifierToken);
            SyntaxToken IdentifierToken = AdvanceToken();
            MatchToken(SyntaxKind.SemicolonToken);
            return new PointStatement(IdentifierToken, false);
        }

        private SyntaxNode ParseExpressionSyntaxLineExpression()
        {
            if (NextToken.Kind == SyntaxKind.OpenParenthesisToken) return ParseExpressionSyntaxLineExpressionSyntax();
            AdvanceToken();
            if (ExpectedAndAdvance(SyntaxKind.SequenceToken))
            {
                Match(SyntaxKind.IdentifierToken);
                SyntaxToken id = AdvanceToken();
                MatchToken(SyntaxKind.SemicolonToken);
                return new LineStatement(id, true);
            }
            Match(SyntaxKind.IdentifierToken);
            SyntaxToken ID = AdvanceToken();
            MatchToken(SyntaxKind.SemicolonToken);
            return new LineStatement(ID, false);

        }

        private LineExpressionSyntax ParseExpressionSyntaxLineExpressionSyntax()
        {
            AdvanceToken();
            MatchToken(SyntaxKind.OpenParenthesisToken);
            ExpressionSyntax point1 = ParseExpressionSyntaxExpression();
            MatchToken(SyntaxKind.SeparatorToken);
            ExpressionSyntax point2 = ParseExpressionSyntaxExpression();
            MatchToken(SyntaxKind.CloseParenthesisToken);
            return new LineExpressionSyntax(point1, point2);
        }

        private SyntaxNode ParseExpressionSyntaxCircleExpression()
        {
            if (NextToken.Kind == SyntaxKind.OpenParenthesisToken) return ParseExpressionSyntaxCircleExpressionSyntax();
            AdvanceToken();
            if (ExpectedAndAdvance(SyntaxKind.SequenceToken))
            {
                Match(SyntaxKind.IdentifierToken);
                SyntaxToken id = AdvanceToken();
                MatchToken(SyntaxKind.SemicolonToken);
                return new CircleStatement(id, true);
            }
            Match(SyntaxKind.IdentifierToken);
            SyntaxToken ID = AdvanceToken();
            MatchToken(SyntaxKind.SemicolonToken);
            return new CircleStatement(ID, false);
        }

        private CircleExpressionSyntax ParseExpressionSyntaxCircleExpressionSyntax()
        {
            AdvanceToken();
            MatchToken(SyntaxKind.OpenParenthesisToken);
            ExpressionSyntax center = ParseExpressionSyntaxExpression();
            MatchToken(SyntaxKind.SeparatorToken);
            ExpressionSyntax measure = ParseExpressionSyntaxExpression();
            MatchToken(SyntaxKind.CloseParenthesisToken);
            return new CircleExpressionSyntax(center, measure);
        }

        private SyntaxNode ParseExpressionSyntaxArcExpression()
        {
            if (NextToken.Kind == SyntaxKind.OpenParenthesisToken) return ParseExpressionSyntaxArcExpressionSyntax();
            AdvanceToken();
            if (ExpectedAndAdvance(SyntaxKind.SequenceToken))
            {
                Match(SyntaxKind.IdentifierToken);
                SyntaxToken id = AdvanceToken();
                MatchToken(SyntaxKind.SemicolonToken);
                return new ArcStatement(id, true);
            }
            Match(SyntaxKind.IdentifierToken);
            SyntaxToken ID = AdvanceToken();
            MatchToken(SyntaxKind.SemicolonToken);
            return new ArcStatement(ID, false);
        }

        private ArcExpressionSyntax ParseExpressionSyntaxArcExpressionSyntax()
        {
            AdvanceToken();
            MatchToken(SyntaxKind.OpenParenthesisToken);
            ExpressionSyntax center = ParseExpressionSyntaxExpression();
            MatchToken(SyntaxKind.SeparatorToken);
            ExpressionSyntax point1 = ParseExpressionSyntaxExpression();
            MatchToken(SyntaxKind.SeparatorToken);
            ExpressionSyntax point2 = ParseExpressionSyntaxExpression();
            MatchToken(SyntaxKind.SeparatorToken);
            ExpressionSyntax measure = ParseExpressionSyntaxExpression();
            MatchToken(SyntaxKind.CloseParenthesisToken);
            return new ArcExpressionSyntax(center, point1, point2, measure);
        }

        private SyntaxNode ParserestoreExpressionSyntax()
        {
            AdvanceToken();
            ColorStack.PopColorStack();
            MatchToken(SyntaxKind.SemicolonToken);
            return new Empty();
        }

        private SyntaxNode ParseExpressionSyntaxColorExpression()
        {
            AdvanceToken();
            if (Expected(SyntaxKind.ColorBlackToken)) ColorStack.PushColorStack(Color.BLACK);
            else if (Expected(SyntaxKind.ColorBlueToken)) ColorStack.PushColorStack(Color.BLUE);
            else if (Expected(SyntaxKind.ColorGrayToken)) ColorStack.PushColorStack(Color.GRAY);
            else if (Expected(SyntaxKind.ColorGreenToken)) ColorStack.PushColorStack(Color.GREEN);
            else if (Expected(SyntaxKind.ColorMagentaToken)) ColorStack.PushColorStack(Color.MAGENTA);
            else if (Expected(SyntaxKind.ColorCyanToken)) ColorStack.PushColorStack(Color.CYAN);
            else if (Expected(SyntaxKind.ColorRedToken)) ColorStack.PushColorStack(Color.RED);
            else if (Expected(SyntaxKind.ColorWhiteToken)) ColorStack.PushColorStack(Color.WHITE);
            else if (Expected(SyntaxKind.ColorYellowToken)) ColorStack.PushColorStack(Color.YELLOW);
            
            else throw new SyntaxError(CurrentToken.Line, CurrentToken.Column, "Color Assignment Expected");
            AdvanceToken();
            MatchToken(SyntaxKind.SemicolonToken);
            return new Empty();
        }

        private SyntaxNode ParseExpressionSyntaxIdentifier()
        {
            if (NextToken.Kind == SyntaxKind.OpenParenthesisToken) return ParseExpressionSyntaxFunctionExpressionSyntaxCallingExpressionSyntax();
            if (NextToken.Kind == SyntaxKind.SeparatorToken)
            {
                var names = new List<SyntaxToken> {AdvanceToken()};
                do
                {
                    MatchToken(SyntaxKind.SeparatorToken);
                    if (Expected(SyntaxKind.IdentifierToken, SyntaxKind.UnderscoreToken, SyntaxKind.RestToken))
                    {names.Add(AdvanceToken());}
                    else throw new SyntaxError(CurrentToken.Line, CurrentToken.Column, "Variable Assignment Expected");
                } while (!ExpectedAndAdvance(SyntaxKind.EqualToken));
                ExpressionSyntax body = ParseExpressionSyntaxExpression();
                MatchToken(SyntaxKind.SemicolonToken);
                return new AssignationStatement(names, body);
            }
            if (ExpectedAndAdvance(SyntaxKind.UnderscoreToken)) return new Empty(); // empty seq
            if (Expected(SyntaxKind.RestToken)) return new SequenceTypeExpression(AdvanceToken());
            if (PreviousToken.Kind  == SyntaxKind.DrawToken && current != 0 ){return new VariableExpressionSyntax(AdvanceToken());}
            SyntaxToken IdentifierToken = AdvanceToken();
            MatchToken(SyntaxKind.EqualToken);
            ExpressionSyntax body_ = ParseExpressionSyntaxExpression();
            MatchToken(SyntaxKind.SemicolonToken);
            return new AssignationStatement(IdentifierToken, body_);

        }

        //auxiliar methods
        private bool MatchToken(SyntaxKind Kind)
        {
            if (Kind == CurrentToken.Kind)
            {
                AdvanceToken();
                return true;
            }
            throw new SyntaxError(PreviousToken.Line, PreviousToken.Column, "OlvIdentifierTokenó poner un " + Kind.ToString() + " después de " + PreviousToken.Lexeme);
        }
        private SyntaxToken AdvanceToken()
        {
            current++;
            return PreviousToken;
        }
        private bool Match(SyntaxKind Kind)
        {
            if (Kind == CurrentToken.Kind)
            {
                return true;
            }
            else
                throw new SyntaxError(PreviousToken.Line, PreviousToken.Column, "OlvIdentifierTokenó poner un " + Kind.ToString() + " después de " + PreviousToken.Lexeme);
        }
        private bool Expected(params SyntaxKind[] Kinds)
        {
            foreach (var Kind in Kinds)
            {
                if (Kind == CurrentToken.Kind)
                {
                    return true;
                }
            }
            return false;
        }
        private bool ExpectedAndAdvance(params SyntaxKind[] Kinds)
        {
            foreach (var Kind in Kinds)
            {
                if (Kind == CurrentToken.Kind)
                {
                    AdvanceToken();
                    return true;
                }
            }
            return false;
        }
        private List<ExpressionSyntax> GetFunctionArgs()
        {
            List<ExpressionSyntax> args = new List<ExpressionSyntax>();
            
            if (Expected(SyntaxKind.CloseParenthesisToken)) return args;
            do
            {
                if (Match(SyntaxKind.IdentifierToken))
                {
                    args.Add(new VariableExpressionSyntax(CurrentToken));
                    AdvanceToken();
                }
            } while (ExpectedAndAdvance(SyntaxKind.SeparatorToken));
            return args;
        }
    }
}
