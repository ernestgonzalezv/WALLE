using System.Runtime.InteropServices;

namespace Geo_Wall_E
{
    public class Evaluator
    {
        private static Scope Scope = new Scope();
         private static Randoms Randoms = new Randoms ();
        private static SamplesExpressionSyntax SamplesExpressionSyntax = new SamplesExpressionSyntax ();
        private List<SyntaxNode> Nodes = new List<SyntaxNode>();
       
        public Evaluator(List<SyntaxNode> SyntaxNodes)
        {
            Nodes = SyntaxNodes;
        }

        public List<IDrawable> Evaluate()
        {
            var ThingstoDraw = new List<IDrawable>();
            try
            {
                foreach (var syntaxNode in Nodes)
                {
                    if (syntaxNode is SyntaxShapes statement)
                    {
                        EvaluateStatementExpressionSyntax(statement, Scope, ThingstoDraw);
                    }
                    else if (syntaxNode is ExpressionSyntax expressions)
                    {
                        SyntaxKindCheck(Scope, expressions);
                    }
                    else _ = new EmptyType();

                }
            }
            catch (SemanticError error)
            {
                error.HandleException();
                return null!;
            }
            return ThingstoDraw;

        }

        public static void EvaluateStatementExpressionSyntax(SyntaxShapes statement, Scope Scope, List<IDrawable> ThingstoDraw)
        {
            switch (statement)
            {
                case AssignationStatement:
                    EvaluateAssignation((AssignationStatement)statement,Scope);
                    break;
                case ArcStatement:
                    if (((ArcStatement)statement).InSequence) Scope.SetSyntaxKind(((ArcStatement)statement).Name.Lexeme!, ArcStatement.ArcSequence());
                    else Scope.SetSyntaxKind(((ArcStatement)statement).Name.Lexeme!, new ArcType(new PointType(""), new PointType(""), new PointType(""), new Measure(new PointType(""), new PointType(""), ""), ((ArcStatement)statement).Name.Lexeme!));
                    break;
                case CircleStatement:
                    if (((CircleStatement)statement).Sequence) Scope.SetSyntaxKind(((CircleStatement)statement).Name.Lexeme!, CircleStatement.CircleSequence());
                    else Scope.SetSyntaxKind(((CircleStatement)statement).Name.Lexeme!, new CircleType(new PointType(""), new Measure(new PointType(""), new PointType(""), ""), ((CircleStatement)statement).Name.Lexeme!));
                    break;
                case DrawStatement:
                    EvaluateDraw(ThingstoDraw, (DrawStatement)statement);
                    break;
                case FunctionStatement:
                    Scope.SetSyntaxKind(((FunctionStatement)statement).Name.Lexeme!, new Function(((FunctionStatement)statement).Name, ((FunctionStatement)statement).Args, ((FunctionStatement)statement).FunctionBody));
                    break;
                case LineStatement:
                    if (((LineStatement)statement).InSequence) Scope.SetSyntaxKind(((LineStatement)statement).Name.Lexeme!, LineStatement.LineSequence());
                    else Scope.SetSyntaxKind(((LineStatement)statement).Name.Lexeme!, new Line(new PointType(""), new PointType(""), ((LineStatement)statement).Name.Lexeme!));
                    break;
                case PointStatement:
                    if (((PointStatement)statement).InSequence) Scope.SetSyntaxKind(((PointStatement)statement).Name.Lexeme!, PointStatement.PointSequence());
                    else Scope.SetSyntaxKind(((PointStatement)statement).Name.Lexeme!, new PointType(""));
                    break;
                case RayStatement:
                    if (((RayStatement)statement).InSequence) Scope.SetSyntaxKind(((RayStatement)statement).Name.Lexeme!, RayStatement.RaySequence());
                    else Scope.SetSyntaxKind(((RayStatement)statement).Name.Lexeme!, new RayType(new PointType(""), new PointType(""), ((RayStatement)statement).Name.Lexeme!));
                    break;
                case SegmentStatement:
                    if (((SegmentStatement)statement).InSequence) Scope.SetSyntaxKind(((SegmentStatement)statement).Name.Lexeme!, SegmentStatement.SegmentSequence());
                    else Scope.SetSyntaxKind(((SegmentStatement)statement).Name.Lexeme!, new SegmentType(new PointType(""), new PointType(""), ((SegmentStatement)statement).Name.Lexeme!));
                    break;
            }
        }

        private static void EvaluateDraw(List<IDrawable> ThingstoDraw, DrawStatement stmt)
        {
            if (stmt.ExpressionSequence != null)
            {
                foreach (var variable in stmt.ExpressionSequence)
                {
                    IDrawable fig = new Drawable(Scope.GetTypes(variable.Name.Lexeme!), stmt.Color, stmt.Name);
                    ThingstoDraw.Add(fig);
                }
            }
            else if (stmt.ExpressionNode is ExpressionSyntax)
            {
                switch (stmt.ExpressionNode)
                {
                    case ArcExpressionSyntax:
                    case CircleExpressionSyntax:
                    case FunctionCallingExpressionSyntax:
                    case LetInExpressionSyntax:
                    case LineExpressionSyntax:
                    case RayExpression:
                    case SegmentExpression:
                        ThingstoDraw.Add(new Drawable(((ICheckType)stmt.ExpressionNode).Check(Scope), stmt.Color, stmt.Name));
                        break;
                    case VariableExpressionSyntax:
                        var type = Scope.GetTypes(((VariableExpressionSyntax)stmt.ExpressionNode).Name.Lexeme!);
                        if (type is SequenceType sequence)
                        {
                            foreach (var element in sequence.SequenceElements)
                            {
                                ThingstoDraw.Add(new Drawable(element, stmt.Color, stmt.Name));
                            }
                        }
                        ThingstoDraw.Add(new Drawable(type, stmt.Color, stmt.Name));
                        break;
                }
            }
            else
            {
                switch (stmt.ExpressionNode)
                {
                    case ArcStatement:
                    case CircleStatement:
                    case FunctionStatement:
                    case LineStatement:
                    case PointStatement:
                    case RayStatement:
                    case SegmentStatement:
                        EvaluateStatementExpressionSyntax(stmt, Scope, ThingstoDraw);
                        break;
                    default:
                        throw new SemanticError(0, 0, "No es posible evaluar esta expresión");
                }

            }
        }

        public static void EvaluateAssignation(AssignationStatement asig, Scope Scope)
        {
            if (asig.Name != null)
            {
                if (asig.Assignment is SequenceTypeExpression || asig.Assignment is IntersectExpression || asig.Assignment is FunctionCallingExpressionSyntax)
                {
                    SequenceType sequence = (SequenceType)((ICheckType)asig.Assignment).Check(Scope);
                    for (int i = 0; i < asig.Name.Count; i++)
                    {
                        if (asig.Name[i].Kind == SyntaxKind.UnderscoreToken) continue;
                        if (i > sequence.SequenceElements.Count) Scope.SetSyntaxKind(asig.Name[i].Lexeme!, new EmptyType());
                        if (asig.Name[i].Kind == SyntaxKind.RestToken || i == asig.Name.Count - 1)
                        {
                            SequenceType rest = new(new(), "");
                            for (int j = i; j < sequence.SequenceElements.Count; j++)
                            {
                                rest.SequenceElements.Add(sequence.SequenceElements[j]);
                            }
                            Scope.SetSyntaxKind(asig.Name[i].Lexeme!, rest);
                            break;
                        }
                        else Scope.SetSyntaxKind(asig.Name[i].Lexeme!, sequence.SequenceElements[i]);
                    }
                }
                if (asig.Assignment is UndefinedExpressionSyntax)
                {
                    for (int i = 0; i < asig.Name.Count; i++)
                    {
                        Scope.SetSyntaxKind(asig.Name[i].Lexeme!, new Undefined());
                    }
                }
                if (asig.Assignment is Randoms)
                {
                    for (int i = 0; i < asig.Name.Count; i++)
                    {

                        if (asig.Name[i].Kind == SyntaxKind.UnderscoreToken || asig.Name[i].Kind == SyntaxKind.RestToken) continue;
                        if (asig.Name.Count > Randoms.RandomSequence.SequenceElements.Count) Scope.SetSyntaxKind(asig.Name[i].Lexeme!, new EmptyType());
                        Scope.SetSyntaxKind(asig.Name[i].Lexeme!, Randoms.RandomSequence.SequenceElements[i]);
                    }
                }
                if (asig.Assignment is SamplesExpressionSyntax)
                {
                    for (int i = 0; i < asig.Name.Count; i++)
                    {

                        if (asig.Name[i].Kind == SyntaxKind.UnderscoreToken || asig.Name[i].Kind == SyntaxKind.RestToken) continue;
                        if (asig.Name.Count > SamplesExpressionSyntax.Sequence.SequenceElements.Count) Scope.SetSyntaxKind(asig.Name[i].Lexeme!, new EmptyType());
                        Scope.SetSyntaxKind(asig.Name[i].Lexeme!, SamplesExpressionSyntax.Sequence.SequenceElements[i]);
                    }
                }
                if (asig.Assignment is RandomPointExpressionSyntax)
                {
                    SequenceType sequence = (SequenceType)((ICheckType)asig.Assignment).Check(Scope);
                    for (int i = 0; i < asig.Name.Count; i++)
                    {
                        if (asig.Name[i].Kind == SyntaxKind.UnderscoreToken || asig.Name[i].Kind == SyntaxKind.RestToken) continue;
                        if (asig.Name.Count > sequence.SequenceElements.Count) Scope.SetSyntaxKind(asig.Name[i].Lexeme!, new EmptyType());
                        else Scope.SetSyntaxKind(asig.Name[i].Lexeme!, sequence.SequenceElements[i]);
                    }
                }
            }
            else switch (asig.Assignment)
                {
                    case ArcExpressionSyntax:
                    case CircleExpressionSyntax:
                    case FunctionCallingExpressionSyntax:
                    case IntersectExpression:
                    case LetInExpressionSyntax:
                    case LineExpressionSyntax:
                    case MeasureExpression:
                    case NumberExpressionSyntax:
                    case RayExpression:
                    case SegmentExpression:
                    case SequenceTypeExpression:
                    case VariableExpressionSyntax:
                        Scope.SetSyntaxKind(asig.Name_!.Lexeme!, ((ICheckType)asig.Assignment).Check(Scope));
                        break;
                    default:
                        throw new SemanticError(0, 0, "No es posible evaluar esta expresión");
                }
        }

        public Type SyntaxKindCheck(Scope Scope, ExpressionSyntax expressions)
        {
            return expressions switch
            {
                ArcExpressionSyntax exp => exp.Check(Scope),
                BetweenParenExpressionSyntax exp => exp.Check(Scope),
                BinaryExpressionSyntax exp => exp.Check(Scope),
                CircleExpressionSyntax exp => exp.Check(Scope),
                ConcatenatedSequenceTypeExpression exp => exp.Check(Scope),
                ConditionalExpressionSyntax exp => exp.Check(Scope),
                Count exp => exp.Check(Scope),
                EExpression exp => exp.Check(Scope),
                FunctionCallingExpressionSyntax exp => exp.Check(Scope),
                InfiniteSequenceTypeExpression exp => exp.Check(Scope),
                IntersectExpression exp => exp.Check(Scope),
                LetInExpressionSyntax exp => exp.Check(Scope),
                LineExpressionSyntax exp => exp.Check(Scope),
                LogExpression exp => exp.Check(Scope),
                MeasureExpression exp => exp.Check(Scope),
                NumberExpressionSyntax exp => exp.Check(Scope),
                PIExpressionSyntax exp => exp.Check(Scope),
                RandomPointExpressionSyntax exp => exp.Check(Scope),
                RayExpression exp => exp.Check(Scope),
                SegmentExpression exp => exp.Check(Scope),
                SequenceTypeExpression exp => exp.Check(Scope),
                StringExpressionSyntax exp => exp.Check(Scope),
                UnaryExpressionSyntax exp => exp.Check(Scope),
                UndefinedExpressionSyntax exp => exp.Check(Scope),
                VariableExpressionSyntax exp => exp.Check(Scope),
                _ => throw new TypeCheckerError(0, 0, "No es posible realizar esta instrucción"),
            };
        }
    }
}
