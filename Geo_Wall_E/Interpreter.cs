using System.Runtime.InteropServices;

namespace Geo_Wall_E
{
    public class Interpreter
    {
        private static Scope scope = new();
        private List<SyntaxNode> Nodes = new();
        private static Randoms random = new();
        private static Samples samples = new();
        public Interpreter(List<SyntaxNode> nodes)
        {
            Nodes = nodes;
        }

        public List<IDrawable> Evaluate()
        {
            List<IDrawable> toDraw = new();
            try
            {
                foreach (var node in Nodes)
                {
                    if (node is Statement stmt)
                    {
                        EvaluateStmt(stmt, scope, toDraw);
                    }
                    else if (node is ExpressionSyntax expressions)
                    {
                        TypeCheck(scope, expressions);
                    }
                    else _ = new EmptyType();

                }
            }
            catch (SemanticError error)
            {
                error.HandleException();
                return null!;
            }
            return toDraw;

        }

        public static void EvaluateStmt(Statement stmt, Scope scope, List<IDrawable> todraw)
        {
            switch (stmt)
            {
                case AssignationStatement:
                    EvaluateAssignation((AssignationStatement)stmt,scope);
                    break;
                case ArcStatement:
                    if (((ArcStatement)stmt).Sequence) scope.SetTypes(((ArcStatement)stmt).Name.Lexeme!, ArcStatement.ArcSequence());
                    else scope.SetTypes(((ArcStatement)stmt).Name.Lexeme!, new Arc(new Point(""), new Point(""), new Point(""), new Measure(new Point(""), new Point(""), ""), ((ArcStatement)stmt).Name.Lexeme!));
                    break;
                case CircleStatement:
                    if (((CircleStatement)stmt).Sequence) scope.SetTypes(((CircleStatement)stmt).Name.Lexeme!, CircleStatement.CircleSequence());
                    else scope.SetTypes(((CircleStatement)stmt).Name.Lexeme!, new Circle(new Point(""), new Measure(new Point(""), new Point(""), ""), ((CircleStatement)stmt).Name.Lexeme!));
                    break;
                case DrawStatement:
                    EvaluateDraw(todraw, (DrawStatement)stmt);
                    break;
                case FunctionStatement:
                    scope.SetTypes(((FunctionStatement)stmt).Name.Lexeme!, new Function(((FunctionStatement)stmt).Name, ((FunctionStatement)stmt).Arguments, ((FunctionStatement)stmt).Body));
                    break;
                case LineStatement:
                    if (((LineStatement)stmt).Sequence) scope.SetTypes(((LineStatement)stmt).Name.Lexeme!, LineStatement.LineSequence());
                    else scope.SetTypes(((LineStatement)stmt).Name.Lexeme!, new Line(new Point(""), new Point(""), ((LineStatement)stmt).Name.Lexeme!));
                    break;
                case PointStatement:
                    if (((PointStatement)stmt).Sequence) scope.SetTypes(((PointStatement)stmt).Name.Lexeme!, PointStatement.PointSequence());
                    else scope.SetTypes(((PointStatement)stmt).Name.Lexeme!, new Point(""));
                    break;
                case RayStatement:
                    if (((RayStatement)stmt).Sequence) scope.SetTypes(((RayStatement)stmt).Name.Lexeme!, RayStatement.RaySequence());
                    else scope.SetTypes(((RayStatement)stmt).Name.Lexeme!, new Ray(new Point(""), new Point(""), ((RayStatement)stmt).Name.Lexeme!));
                    break;
                case SegmentStatement:
                    if (((SegmentStatement)stmt).Sequence) scope.SetTypes(((SegmentStatement)stmt).Name.Lexeme!, SegmentStatement.SegmentSequence());
                    else scope.SetTypes(((SegmentStatement)stmt).Name.Lexeme!, new Segment(new Point(""), new Point(""), ((SegmentStatement)stmt).Name.Lexeme!));
                    break;
            }
        }

        private static void EvaluateDraw(List<IDrawable> toDraw, DrawStatement stmt)
        {
            if (stmt.ExpressionSequence != null)
            {
                foreach (var variable in stmt.ExpressionSequence)
                {
                    IDrawable fig = new Drawable(scope.GetTypes(variable.Name.Lexeme!), stmt.Color, stmt.Name);
                    toDraw.Add(fig);
                }
            }
            else if (stmt.Expression is ExpressionSyntax)
            {
                switch (stmt.Expression)
                {
                    case ArcExpression:
                    case CircleExpression:
                    case FunctionCallExpression:
                    case LetInExpression:
                    case LineExpression:
                    case RayExpression:
                    case SegmentExpression:
                        toDraw.Add(new Drawable(((ICheckType)stmt.Expression).Check(scope), stmt.Color, stmt.Name));
                        break;
                    case VariableExpression:
                        var type = scope.GetTypes(((VariableExpression)stmt.Expression).Name.Lexeme!);
                        if (type is Sequence sequence)
                        {
                            foreach (var element in sequence.Elements)
                            {
                                toDraw.Add(new Drawable(element, stmt.Color, stmt.Name));
                            }
                        }
                        toDraw.Add(new Drawable(type, stmt.Color, stmt.Name));
                        break;
                }
            }
            else
            {
                switch (stmt.Expression)
                {
                    case ArcStatement:
                    case CircleStatement:
                    case FunctionStatement:
                    case LineStatement:
                    case PointStatement:
                    case RayStatement:
                    case SegmentStatement:
                        EvaluateStmt(stmt, scope, toDraw);
                        break;
                    default:
                        throw new SemanticError(0, 0, "No es posible evaluar esta expresión");
                }

            }
        }

        public static void EvaluateAssignation(AssignationStatement asig, Scope scope)
        {
            if (asig.Name != null)
            {
                if (asig.Assignment is SequenceExpression || asig.Assignment is IntersectExpression || asig.Assignment is FunctionCallExpression)
                {
                    Sequence sequence = (Sequence)((ICheckType)asig.Assignment).Check(scope);
                    for (int i = 0; i < asig.Name.Count; i++)
                    {
                        if (asig.Name[i].Type == SyntaxKind.UnderscoreToken) continue;
                        if (i > sequence.Elements.Count) scope.SetTypes(asig.Name[i].Lexeme!, new EmptyType());
                        if (asig.Name[i].Type == SyntaxKind.RestToken || i == asig.Name.Count - 1)
                        {
                            Sequence rest = new(new(), "");
                            for (int j = i; j < sequence.Elements.Count; j++)
                            {
                                rest.Elements.Add(sequence.Elements[j]);
                            }
                            scope.SetTypes(asig.Name[i].Lexeme!, rest);
                            break;
                        }
                        else scope.SetTypes(asig.Name[i].Lexeme!, sequence.Elements[i]);
                    }
                }
                if (asig.Assignment is UndefinedExpression)
                {
                    for (int i = 0; i < asig.Name.Count; i++)
                    {
                        scope.SetTypes(asig.Name[i].Lexeme!, new Undefined());
                    }
                }
                if (asig.Assignment is Randoms)
                {
                    for (int i = 0; i < asig.Name.Count; i++)
                    {

                        if (asig.Name[i].Type == SyntaxKind.UnderscoreToken || asig.Name[i].Type == SyntaxKind.RestToken) continue;
                        if (asig.Name.Count > random.RandomSequence.Elements.Count) scope.SetTypes(asig.Name[i].Lexeme!, new EmptyType());
                        scope.SetTypes(asig.Name[i].Lexeme!, random.RandomSequence.Elements[i]);
                    }
                }
                if (asig.Assignment is Samples)
                {
                    for (int i = 0; i < asig.Name.Count; i++)
                    {

                        if (asig.Name[i].Type == SyntaxKind.UnderscoreToken || asig.Name[i].Type == SyntaxKind.RestToken) continue;
                        if (asig.Name.Count > samples.Sequence.Elements.Count) scope.SetTypes(asig.Name[i].Lexeme!, new EmptyType());
                        scope.SetTypes(asig.Name[i].Lexeme!, samples.Sequence.Elements[i]);
                    }
                }
                if (asig.Assignment is RandomPoints)
                {
                    Sequence sequence = (Sequence)((ICheckType)asig.Assignment).Check(scope);
                    for (int i = 0; i < asig.Name.Count; i++)
                    {
                        if (asig.Name[i].Type == SyntaxKind.UnderscoreToken || asig.Name[i].Type == SyntaxKind.RestToken) continue;
                        if (asig.Name.Count > sequence.Elements.Count) scope.SetTypes(asig.Name[i].Lexeme!, new EmptyType());
                        else scope.SetTypes(asig.Name[i].Lexeme!, sequence.Elements[i]);
                    }
                }
            }
            else switch (asig.Assignment)
                {
                    case ArcExpression:
                    case CircleExpression:
                    case FunctionCallExpression:
                    case IntersectExpression:
                    case LetInExpression:
                    case LineExpression:
                    case MeasureExpression:
                    case NumberExpression:
                    case RayExpression:
                    case SegmentExpression:
                    case SequenceExpression:
                    case VariableExpression:
                        scope.SetTypes(asig.Name_!.Lexeme!, ((ICheckType)asig.Assignment).Check(scope));
                        break;
                    default:
                        throw new SemanticError(0, 0, "No es posible evaluar esta expresión");
                }
        }

        public Type TypeCheck(Scope scope, ExpressionSyntax expressions)
        {
            return expressions switch
            {
                ArcExpression exp => exp.Check(scope),
                BetweenParenExpressions exp => exp.Check(scope),
                BinaryExpression exp => exp.Check(scope),
                CircleExpression exp => exp.Check(scope),
                ConcatenatedSequenceExpression exp => exp.Check(scope),
                ConditionalExpression exp => exp.Check(scope),
                Count exp => exp.Check(scope),
                EExpression exp => exp.Check(scope),
                FunctionCallExpression exp => exp.Check(scope),
                InfiniteSequenceExpression exp => exp.Check(scope),
                IntersectExpression exp => exp.Check(scope),
                LetInExpression exp => exp.Check(scope),
                LineExpression exp => exp.Check(scope),
                LogExpression exp => exp.Check(scope),
                MeasureExpression exp => exp.Check(scope),
                NumberExpression exp => exp.Check(scope),
                PIExpression exp => exp.Check(scope),
                RandomPoints exp => exp.Check(scope),
                RayExpression exp => exp.Check(scope),
                SegmentExpression exp => exp.Check(scope),
                SequenceExpression exp => exp.Check(scope),
                StringExpression exp => exp.Check(scope),
                UnaryExpression exp => exp.Check(scope),
                UndefinedExpression exp => exp.Check(scope),
                VariableExpression exp => exp.Check(scope),
                _ => throw new TypeCheckerError(0, 0, "No es posible realizar esta instrucción"),
            };
        }
    }
}
