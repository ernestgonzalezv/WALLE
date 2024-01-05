
namespace Geo_Wall_E
{
    public class ArcExpression : Expressions, ICheckType
    {
        public override TypesOfToken Type => TypesOfToken.ArcToken;
        public Expressions Center { get; private set; }
        public Expressions Start { get; private set; }
        public Expressions End { get; private set; }
        public Expressions Measure { get; private set; }

        public ArcExpression(Expressions center, Expressions start, Expressions end, Expressions measure)
        {
            Center = center;
            Start = start;
            End = end;
            Measure = measure;
        }

        public Type Check(Scope scope)
        {
            Type center = ((ICheckType)Center).Check(scope);
            Type start = ((ICheckType)Start).Check(scope);
            Type end = ((ICheckType)End).Check(scope);
            Type measure = ((ICheckType)Measure).Check(scope);
            if (center.TypeOfElement != TypeOfElement.Point) throw new TypeCheckerError(0, 0, "Se esperaba un punto pero en su lugar se obtuvo " + center.TypeOfElement);
            else if (start.TypeOfElement != TypeOfElement.Point) throw new TypeCheckerError(0, 0, "Se esperaba un punto pero en su lugar se obtuvo " + start.TypeOfElement);
            else if (end.TypeOfElement != TypeOfElement.Point) throw new TypeCheckerError(0, 0, "Se esperaba un punto pero en su lugar se obtuvo " + end.TypeOfElement);
            else if (measure.TypeOfElement != TypeOfElement.Measure) throw new TypeCheckerError(0, 0, "Se esperaba una medida pero en su lugar se obtuvo " + measure.TypeOfElement);
            else return new Arc((Point)center, (Point)start, (Point)end, (Measure)measure, null!);
        }
    }

    public class UndefinedExpression : Expressions, ICheckType
    {
        public override TypesOfToken Type => TypesOfToken.UndefinedToken;

        public Type Check(Scope scope)
        {
            return new Undefined();
        }
    }

    public class UnaryExpression : Expressions, ICheckType
    {
        public override TypesOfToken Type => TypesOfToken.UnaryExpression;
        public Token Operator { get; private set; }
        public Expressions ToOperate { get; private set; }


        public UnaryExpression(Token oper, Expressions toOper)
        {
            Operator = oper;
            ToOperate = toOper;
        }

        public Type Check(Scope scope)
        {
            Type toOper = ((ICheckType)ToOperate).Check(scope);
            switch (toOper.TypeOfElement)
            {
                case TypeOfElement.Number:
                    if (Operator.Type == TypesOfToken.PlusToken) return new Number(((Number)toOper).Value);
                    if (Operator.Type == TypesOfToken.MinusToken) return new Number(-((Number)toOper).Value);
                    if (Operator.Type == TypesOfToken.SqrtToken) return new Number(Math.Sqrt(((Number)toOper).Value));
                    if (Operator.Type == TypesOfToken.SinToken) return new Number(Math.Sin(((Number)toOper).Value));
                    if (Operator.Type == TypesOfToken.CosToken) return new Number(Math.Cos(((Number)toOper).Value));
                    if (Operator.Type == TypesOfToken.ExpoToken) return new Number(Math.Pow(Math.E, ((Number)toOper).Value));
                    if (Operator.Type == TypesOfToken.NotToken)
                    {
                        if (((Number)toOper).Value == 0) return new Number(1);
                        else return new Number(0);
                    }
                    else throw new TypeCheckerError(0, 0, "No es posible realizar la operción");
                default:
                    throw new TypeCheckerError(0, 0, "No es posible realizar la operción");
            }
        }
    }

    public class BetweenParenExpressions : Expressions, ICheckType
    {
        public override TypesOfToken Type => TypesOfToken.BetweenParenExpression;
        public Expressions Expressions { get; private set; }

        public BetweenParenExpressions(Expressions exp)
        {
            Expressions = exp;
        }

        public Type Check(Scope scope)
        {
            return ((ICheckType)Expressions).Check(scope);
        }
    }
    public class Randoms : Expressions
    {
        public override TypesOfToken Type => TypesOfToken.RandomsToken;
        public Sequence RandomSequence { get; private set; }

        public Randoms()
        {
            List<Type> randoms = GenerateRandom();
            RandomSequence = new Sequence(randoms, "");
        }

        private List<Type> GenerateRandom()
        {
            List<Type> randoms = new();
            Random cant = new();
            Random numbers = new();
            for (int i = 0; i < cant.Next(0, 100); i++)
            {
                randoms.Add(new Number(numbers.NextDouble()));
            }
            return randoms;
        }

    }
    public class RandomPoints : Expressions, ICheckType
    {
        public override TypesOfToken Type => TypesOfToken.PointsToken;
        public Expressions Figure { get; private set; }

        public RandomPoints(Expressions figure)
        {
            Figure = figure;
        }

        public Type Check(Scope scope)
        {
            Type fig = ((ICheckType)Figure).Check(scope);
            List<Type> points = CreateRandomPoints(fig);
            return new Sequence(points, "");
        }
        private List<Type> CreateRandomPoints(Type figure)
        {
            List<Type> randoms = new();
            Random random = new();
            for (int i = 0; i <  100000; i++)
            {
                Point point = new("");
                point.X = random.Next(0,1000);
                point.Y = random.Next(0,1000);
                bool isInFigure = Belongs(point, figure);
                if (isInFigure) randoms.Add(new Point(""));
                else continue;
            }
            return randoms;
        }

        private bool Belongs(Point p, Type figure)
        {
            if (figure is Line l)
            {
                if (p.Y - l.P1.Y == (l.P2.Y - l.P1.Y) / (l.P2.X - l.P1.X) * (p.X - l.P1.X)) return true;
                else return false;
            }
            else if (figure is Ray r)
            {
                double intercept = ((p.X - r.Start.X) * r.Point.X - r.Start.X + (p.Y - r.Start.Y) * r.Point.Y - r.Start.Y) / (Math.Pow(r.Point.X - r.Start.X, 2) + Math.Pow(r.Point.Y - r.Start.Y, 2));
                if (intercept >= 0) return true;
                else return false;
            }
            else if (figure is Segment s)
            {
                double intercept = ((p.X - s.Start.X) * s.End.X - s.Start.X + (p.Y - s.Start.Y) * s.End.Y - s.Start.Y) / (Math.Pow(s.End.X - s.Start.X, 2) + Math.Pow(s.End.Y - s.Start.Y, 2));
                if (intercept < 0 || intercept > 1) return true;
                else return false;
            }
            else if (figure is Circle c)
            {
                if (Math.Sqrt(Math.Pow(c.Center.X - p.X, 2) + Math.Pow(c.Center.Y - p.Y, 2)) == c.Radius.Measure_) return true;
                else return false;
            }
            else if (figure is Arc a)
            {
                // Calcula la distancia entre el centro del arco y el punto usando la ecuación de la circunferencia
                double distance = Math.Abs(Math.Sqrt(Math.Pow(p.X - a.Center.X, 2) + Math.Pow(p.Y - a.Center.Y, 2)));
                // Si esta es igual al radio de la circunferncia entonces el punto esta contenido 
                if (distance == Math.Abs(a.Measure.Measure_))
                {
                    //Calcular si el punto esta entre los limites del arco
                    double arcPointsDistance = Math.Abs(Math.Sqrt(Math.Pow(a.Start.X - a.End.X, 2) + Math.Pow(a.Start.Y - a.End.Y, 2)));
                    double starPointDistance = Math.Abs(Math.Sqrt(Math.Pow(p.X - a.Start.X, 2) + Math.Pow(p.Y - a.Start.Y, 2)));
                    double endPointDistance = Math.Abs(Math.Sqrt(Math.Pow(p.X - a.End.X, 2) + Math.Pow(p.Y - a.End.Y, 2)));
                    if (arcPointsDistance == (starPointDistance + endPointDistance))
                    {
                        return true;
                    }
                    return false;
                }
                else return false;
            }
            else throw new TypeCheckerError(0, 0, "No es posible realizar el intercepto");
        }
    }
    public class Samples : Expressions
    {
        public override TypesOfToken Type => TypesOfToken.SamplesToken;
        public Sequence Sequence { get; private set; }

        public Samples()
        {
            List<Type> points = GenerateSamples();
            Sequence = new Sequence(points, "");
        }
        private List<Type> GenerateSamples()
        {
            List<Type> randoms = new();
            Random cant = new();
            for (int i = 0; i < cant.Next(0, 100); i++)
            {
                randoms.Add(new Point(""));
            }
            return randoms;
        }
    }
    public class Count : Expressions, ICheckType
    {
        public override TypesOfToken Type => TypesOfToken.CountToken;
        public Expressions Sequence { get; private set; }

        public Count(Expressions sequence)
        {
            Sequence = sequence;
        }

        public Type Check(Scope scope)
        {
            Type sequence = ((ICheckType)Sequence).Check(scope);
            if (sequence is Sequence seq)
            {
                if (seq.Elements.Count != 0 || seq.Elements.Count <= 100) return new Number(seq.Elements.Count);
                else return new Undefined();
            }
            else throw new TypeCheckerError(0, 0, "Se esperaba una secuencia al realizar la operación count()");
        }
    }

    public class SequenceExpression : Expressions, ICheckType
    {
        public override TypesOfToken Type => TypesOfToken.SequenceToken;
        public List<Expressions>? Elements { get; private set; }
        public Token? Start { get; private set; }
        public Token? End { get; private set; }
        public ConcatenatedSequenceExpression? Sequence { get; private set; }
        public InfiniteSequenceExpression? InfiniteSequence { get; private set; }
        public Empty? Empty { get; private set; }

        public SequenceExpression(List<Expressions> elements)
        {
            Elements = elements;
        }
        public SequenceExpression(Token start, Token end)
        {
            Start = start;
            End = end;
        }
        public SequenceExpression(Token start)
        {
            Start = start;
        }
        public SequenceExpression(ConcatenatedSequenceExpression sequence)
        {
            Sequence = sequence;
        }
        public SequenceExpression(Empty empty)
        {
            Empty = empty;
        }

        public Type Check(Scope scope)
        {
            List<Type> elements = new();
            if (Elements != null)
            {
                foreach (var element in Elements)
                {
                    elements.Add(((ICheckType)element).Check(scope));
                }
                if (elements.All(x => x.TypeOfElement == elements[0].TypeOfElement)) return new Sequence(elements, "");
                else throw new TypeCheckerError(0, 0, "Los elementos de la secuencia no son el mismo tipo");
            }
            if (Start != null && End != null)
            {
                //si la intersección es muy grande se considerará que es infinita
                if ((double)End.Value! - (double)Start.Value! > 1000) return (Sequence)new InfiniteSequenceExpression(Start).Check(scope);
                else
                {
                    double start = (double)Start.Value!;
                    double end = (double)End.Value!;
                    var sequence = Enumerable.Range(Convert.ToInt32(start), Convert.ToInt32(end) - Convert.ToInt32(start) + 1).Select(x => new Number(x));
                    List<Type> seq = new();
                    foreach (var item in sequence)
                    {
                        seq.Add(item);
                    }
                    return new Sequence(seq, "");
                }
            }
            if (Start != null && End == null)
            {
                //debería devolver una secuencia infinita
                return (Sequence)new InfiniteSequenceExpression(Start).Check(scope);
            }
            if (Sequence != null)
            {
                //debería devolver una secuencia concatenada
                return (Sequence)((ICheckType)Sequence).Check(scope);
            }
            if (Empty != null)
            {
                return new EmptyType();
            }
            else throw new TypeCheckerError(0, 0, "La secuencia no es válida");
        }
    }

    public class ConcatenatedSequenceExpression : Expressions, ICheckType
    {
        public override TypesOfToken Type => TypesOfToken.SequenceToken;
        public SequenceExpression? FirstSequence { get; private set; }
        public UndefinedExpression? Undefined { get; private set; }
        public SequenceExpression? SecondSequence { get; private set; }

        public ConcatenatedSequenceExpression(SequenceExpression first, SequenceExpression second)
        {
            FirstSequence = first;
            SecondSequence = second;
        }

        public ConcatenatedSequenceExpression(SequenceExpression first, UndefinedExpression undefined)
        {
            FirstSequence = first;
            Undefined = undefined;
        }

        public ConcatenatedSequenceExpression(UndefinedExpression undefined, SequenceExpression second)
        {
            SecondSequence = second;
            Undefined = undefined;
        }

        public Type Check(Scope scope)
        {
            if (FirstSequence != null && SecondSequence != null)
            {
                Sequence first = (Sequence)((ICheckType)FirstSequence!).Check(scope);
                Sequence second = (Sequence)((ICheckType)SecondSequence!).Check(scope);
                List<Type> sequence = new(first.Elements.Concat(second.Elements));
                if (sequence.All(x => x.TypeOfElement == sequence[0].TypeOfElement)) return new Sequence(sequence, "");
                else throw new TypeCheckerError(0, 0, "Para poder concatenar las secuencias deben ser del mismo tipo");
            }
            if (FirstSequence != null && Undefined != null)
            {
                Sequence first = (Sequence)((ICheckType)FirstSequence!).Check(scope);
                Undefined undefined = (Undefined)((ICheckType)Undefined!).Check(scope);
                first.Elements.Add(undefined);
                return new Sequence(first.Elements, "");
            }
            if (SecondSequence != null && Undefined != null)
            {
                return new Undefined();
            }
            else throw new TypeCheckerError(0, 0, "La secuencia no es válida");
        }
    }

    public class InfiniteSequenceExpression : Expressions, ICheckType
    {
        public override TypesOfToken Type => TypesOfToken.SequenceToken;
        public Token Start { get; private set; }
        public InfiniteSequenceExpression(Token start)
        {
            Start = start;
        }

        public Type Check(Scope scope)
        {
            double start = (double)Start.Value!;
            List<Type> sequence = (List<Type>)Enumerable.Range((int)start, (int)start + 1000).Select(x => new Number(x));
            return new Sequence(sequence, "");
        }
    }

    public class SegmentExpression : Expressions, ICheckType
    {
        public override TypesOfToken Type => TypesOfToken.SegmentToken;
        public Expressions Start { get; private set; }
        public Expressions End { get; private set; }
        public SegmentExpression(Expressions start, Expressions end)
        {
            Start = start;
            End = end;
        }

        public Type Check(Scope scope)
        {
            Type start = ((ICheckType)Start).Check(scope);
            Type end = ((ICheckType)End).Check(scope);
            if (start.TypeOfElement != TypeOfElement.Point) throw new TypeCheckerError(0, 0, "Se esperaba un punto pero en su lugar se obtuvo " + start.TypeOfElement);
            else if (end.TypeOfElement != TypeOfElement.Point) throw new TypeCheckerError(0, 0, "Se esperaba un punto pero en su lugar se obtuvo " + end.TypeOfElement);
            else return new Segment((Point)start, (Point)end, "");
        }
    }

    public class RayExpression : Expressions, ICheckType
    {
        public override TypesOfToken Type => TypesOfToken.RayToken;
        public Expressions Start { get; private set; }
        public Expressions Point2 { get; private set; }
        public RayExpression(Expressions start, Expressions point2)
        {
            Start = start;
            Point2 = point2;
        }

        public Type Check(Scope scope)
        {
            Type start = ((ICheckType)Start).Check(scope);
            Type point2 = ((ICheckType)Point2).Check(scope);
            if (start.TypeOfElement != TypeOfElement.Point) throw new TypeCheckerError(0, 0, "Se esperaba un punto pero en su lugar se obtuvo " + start.TypeOfElement);
            else if (point2.TypeOfElement != TypeOfElement.Point) throw new TypeCheckerError(0, 0, "Se esperaba un punto pero en su lugar se obtuvo " + point2.TypeOfElement);
            else return new Ray((Point)start, (Point)point2, "");
        }
    }

    public class MeasureExpression : Expressions, ICheckType
    {
        public override TypesOfToken Type => TypesOfToken.MeasureToken;
        public Expressions Point1 { get; private set; }
        public Expressions Point2 { get; private set; }

        public MeasureExpression(Expressions point1, Expressions point2)
        {
            Point1 = point1;
            Point2 = point2;
        }

        public Type Check(Scope scope)
        {
            Type point1 = ((ICheckType)Point1).Check(scope);
            Type point2 = ((ICheckType)Point2).Check(scope);
            if (point1.TypeOfElement != TypeOfElement.Point) throw new TypeCheckerError(0, 0, "Se esperaba un punto pero en su lugar se obtuvo " + point1.TypeOfElement);
            else if (point2.TypeOfElement != TypeOfElement.Point) throw new TypeCheckerError(0, 0, "Se esperaba un punto pero en su lugar se obtuvo " + point2.TypeOfElement);
            else return new Measure((Point)point1, (Point)point2, "");
        }
    }

    public class LogExpression : Expressions, ICheckType
    {
        public Token Log { get; private set; }
        public Expressions Value { get; private set; }
        public Expressions Base { get; private set; }

        public override TypesOfToken Type => TypesOfToken.LogToken;

        public LogExpression(Token log, Expressions value, Expressions base_)
        {
            Log = log;
            Value = value;
            Base = base_;
        }

        public Type Check(Scope scope)
        {
            Type value = ((ICheckType)Value).Check(scope);
            Type base_ = ((ICheckType)Base).Check(scope);
            if (value.TypeOfElement != TypeOfElement.Number) throw new TypeCheckerError(0, 0, "Se esperaba un número pero en su lugar se obtuvo " + value.TypeOfElement);
            if (base_.TypeOfElement != TypeOfElement.Number) throw new TypeCheckerError(0, 0, "Se esperaba un número pero en su lugar se obtuvo " + base_.TypeOfElement);
            else return new Number(Math.Log(((Number)value).Value, ((Number)base_).Value));
        }
    }

    public class NumberExpression : Expressions, ICheckType
    {
        public override TypesOfToken Type => TypesOfToken.Number;
        public Token Number { get; private set; }
        public NumberExpression(Token number)
        {
            Number = number;
        }

        public Type Check(Scope scope)
        {
            return new Number(double.Parse(Number.Lexeme!));
        }
    }
    public class StringExpression : Expressions, ICheckType
    {
        public override TypesOfToken Type => TypesOfToken.String;
        public Token String { get; private set; }
        public StringExpression(Token string_)
        {
            String = string_;
        }

        public Type Check(Scope scope)
        {
            return new String(String.Lexeme!);
        }
    }
    public class VariableExpression : Expressions, ICheckType
    {
        public override TypesOfToken Type => TypesOfToken.VariableToken;
        public Token Name { get; private set; }
        public VariableExpression(Token name)
        {
            Name = name;
        }

        public Type Check(Scope scope)
        {
            Type name = scope.GetTypes(Name.Lexeme!);
            return name;
        }
    }
    public class PIExpression : Expressions, ICheckType
    {
        public string Value { get; private set; }

        public override TypesOfToken Type => TypesOfToken.PIToken;

        public PIExpression(string value)
        {
            Value = value;
        }

        public Type Check(Scope scope)
        {
            return new Number(Math.PI);
        }
    }
    public class EExpression : Expressions, ICheckType
    {
        public string Value { get; private set; }

        public override TypesOfToken Type => TypesOfToken.EToken;

        public EExpression(string value)
        {
            Value = value;
        }

        public Type Check(Scope scope)
        {
            return new Number(Math.E);
        }
    }

    public class LineExpression : Expressions, ICheckType
    {
        public override TypesOfToken Type => TypesOfToken.LineToken;
        public Expressions Point1 { get; private set; }
        public Expressions Point2 { get; private set; }
        public LineExpression(Expressions point1, Expressions point2)
        {
            Point1 = point1;
            Point2 = point2;
        }
        
        public Type Check(Scope scope)
        {
            Type point1 = ((ICheckType)Point1).Check(scope);
            Type point2 = ((ICheckType)Point2).Check(scope);
            if (point1.TypeOfElement != TypeOfElement.Point) throw new TypeCheckerError(0, 0, "Se esperaba un punto pero en su lugar se obtuvo " + point1.TypeOfElement);
            else if (point2.TypeOfElement != TypeOfElement.Point) throw new TypeCheckerError(0, 0, "Se esperaba un punto pero en su lugar se obtuvo " + point2.TypeOfElement);
            else return new Line((Point)point1, (Point)point2,  "");
        }
    }

    public class LetInExpression : Expressions, ICheckType
    {
        public override TypesOfToken Type => TypesOfToken.LetInExpression;
        public List<Stmt> Let { get; private set; }
        public Expressions In { get; private set; }
        public LetInExpression(List<Stmt> let, Expressions _in)
        {
            Let = let;
            In = _in;
        }

        public Type Check(Scope scope)
        {
            scope.SetScope();
            List<IDrawable> todraw = new();
            foreach (var stmt in Let)
            {
                Interpreter.EvaluateStmt(stmt, scope, todraw);
            }
            Type result = ((ICheckType)In).Check(scope);
            scope.DeleteScope();
            return result;
        }
    }

    public class FunctionCallExpression : Expressions, ICheckType

    {
        public override TypesOfToken Type => TypesOfToken.FunctionCallExpression;
        public Token Name { get; private set; }
        public List<Expressions> Arguments { get; private set; }
        public Node Body { get; private set; }

        public FunctionCallExpression(Token name, List<Expressions> args, Node body)
        {
            Name = name;
            Arguments = args;
            Body = body;
        }
        int maxCantCall = 100;
        int cantCall = 0;
        public Type Check(Scope scope)
        {
            if (cantCall < maxCantCall)
            {
                Type function = scope.GetTypes(Name.Lexeme!);
                if (function.TypeOfElement == TypeOfElement.Function)
                {
                    if (Arguments.Count == ((Function)function).Arguments.Count)
                    {
                        Dictionary<string, Type> arguments = new();
                        for (int i = 0; i < Arguments.Count; i++)
                        {
                            VariableExpression variable = (VariableExpression)((Function)function).Arguments[i];
                            try
                            {
                                arguments.Add(variable.Name.Lexeme!, ((ICheckType)Arguments[i]).Check(scope));
                            }
                            catch (TypeCheckerError)
                            {
                                throw new TypeCheckerError(0, 0, "No se puede evaluar el argumento " + ((Function)function).Arguments[i]!.ToString()!);
                            }
                        }
                        scope.SetScope();
                        cantCall++;
                        foreach (var arg in arguments)
                        {
                            scope.SetTypes(arg.Key, arg.Value);
                        }
                        Type result = ((ICheckType)((Function)function).Body).Check(scope);
                        scope.DeleteScope();
                        cantCall--;
                        return result;
                    }
                    throw new TypeCheckerError(0, 0, "La función " + ((Function)function).Name + " esperaba " + ((Function)function).Arguments.Count + " elementos");
                }
                throw new TypeCheckerError(0, 0, "La función " + ((Function)function).Name + " no ha sido definida");
            }
            throw new SemanticError(0, 0, "Stack Overflow");
        }
    }

    public class ConditionalExpression : Expressions, ICheckType
    {
        public override TypesOfToken Type => TypesOfToken.ConditionalExpression;
        public Expressions If { get; private set; }
        public Expressions Then { get; private set; }
        public Expressions Else { get; private set; }
        public ConditionalExpression(Expressions ifExp, Expressions thenExp, Expressions elseExp)
        {
            If = ifExp;
            Then = thenExp;
            Else = elseExp;
        }

        public Type Check(Scope scope)
        {
            Type if_ = ((ICheckType)If).Check(scope);
            {
                if (if_.TypeOfElement == TypeOfElement.Number)
                {
                    if (((Number)if_).Value == 0)
                    {
                        return ((ICheckType)Else).Check(scope);
                    }
                    else return ((ICheckType)Then).Check(scope);
                }
                if (if_.TypeOfElement == TypeOfElement.Undefined) return ((ICheckType)Else).Check(scope);
                if (if_.TypeOfElement == TypeOfElement.Sequence)
                {
                    if (((Sequence)if_).Elements.Count == 0) return ((ICheckType)Else).Check(scope);
                    else return ((ICheckType)Then).Check(scope);
                }
                return ((ICheckType)Then).Check(scope);
            }
        }
    }

    public class CircleExpression : Expressions, ICheckType
    {
        public override TypesOfToken Type => TypesOfToken.CircleToken;
        public Expressions Center { get; private set; }
        public Expressions Radius { get; private set; }
        public CircleExpression(Expressions center, Expressions radius)
        {
            Center = center;
            Radius = radius;
        }

        public Type Check(Scope scope)
        {
            Type center = ((ICheckType)Center).Check(scope);
            Type radius = ((ICheckType)Radius).Check(scope);
            if (center.TypeOfElement != TypeOfElement.Point) throw new TypeCheckerError(0, 0, "Se esperaba un punto pero en su lugar se obtuvo " + center.TypeOfElement);
            else if (radius.TypeOfElement != TypeOfElement.Measure) throw new TypeCheckerError(0, 0, "Se esperaba un punto pero en su lugar se obtuvo " + radius.TypeOfElement);
            else return new Circle((Point)center, (Measure)radius, "");
        }
    }


    public class BinaryExpression : Expressions, ICheckType
    {
        public override TypesOfToken Type => TypesOfToken.BinaryExpression;
        public Expressions Left { get; private set; }
        public Token Operator { get; private set; }
        public Expressions Right { get; private set; }
        public BinaryExpression(Expressions left, Token oper, Expressions right)
        {
            Left = left;
            Operator = oper;
            Right = right;
        }

        public Type Check(Scope scope)
        {
            Type left = ((ICheckType)Left).Check(scope);
            Type right = ((ICheckType)Right).Check(scope);
            if (left is Number leftNumber && right is Number rightNumber)
            {
                switch (Operator.Type)
                {
                    case TypesOfToken.PlusToken:
                        return new Number(leftNumber.Value + rightNumber.Value);
                    case TypesOfToken.MinusToken:
                        return new Number(leftNumber.Value - rightNumber.Value);
                    case TypesOfToken.MultToken:
                        return new Number(leftNumber.Value * rightNumber.Value);
                    case TypesOfToken.PowToken:
                        return new Number(Math.Pow(leftNumber.Value, rightNumber.Value));
                    case TypesOfToken.MoreToken:
                        if (leftNumber.Value > rightNumber.Value) return new Number(1);
                        else return new Number(0);
                    case TypesOfToken.MoreOrEqualToken:
                        if (leftNumber.Value >= rightNumber.Value) return new Number(1);
                        else return new Number(0);
                    case TypesOfToken.LessToken:
                        if (leftNumber.Value < rightNumber.Value) return new Number(1);
                        else return new Number(0);
                    case TypesOfToken.LessOrEqualToken:
                        if (leftNumber.Value <= rightNumber.Value) return new Number(1);
                        else return new Number(0);
                    case TypesOfToken.DoubleEqualToken:
                        if (leftNumber.Value == rightNumber.Value) return new Number(1);
                        else return new Number(0);
                    case TypesOfToken.NoEqualToken:
                        if (leftNumber.Value != rightNumber.Value) return new Number(1);
                        else return new Number(0);
                    case TypesOfToken.DivToken:
                        if (rightNumber.Value == 0) throw new TypeCheckerError(0, 0, "No se puede dividir por cero");
                        return new Number(leftNumber.Value / rightNumber.Value);
                    case TypesOfToken.ModuToken:
                        if (rightNumber.Value == 0) throw new TypeCheckerError(0, 0, "No se puede dividir por cero");
                        return new Number(leftNumber.Value % rightNumber.Value);
                    case TypesOfToken.AndToken:
                        if (leftNumber.Value != 0 && rightNumber.Value != 0) return new Number(1);
                        else return new Number(0);
                    case TypesOfToken.OrToken:
                        if (leftNumber.Value == 1 || rightNumber.Value == 1) return new Number(1);
                        else return new Number(0);
                    default:
                        throw new TypeCheckerError(0, 0, "No es posible realizar la operación " + Operator.Lexeme);
                }
            }
            else if (left is Measure leftMeasure && right is Measure rightMeasure)
            {
                return Operator.Type switch
                {
                    TypesOfToken.PlusToken => new Measure(leftMeasure.Measure_ + rightMeasure.Measure_, ""),
                    TypesOfToken.MinusToken => new Measure(Math.Abs(leftMeasure.Measure_ - rightMeasure.Measure_), ""),
                    TypesOfToken.DivToken => new Number((int)(leftMeasure.Measure_ / rightMeasure.Measure_)),
                    TypesOfToken.MoreToken => (leftMeasure.Measure_ > rightMeasure.Measure_) ? new Number(1) : new Number(0),
                    TypesOfToken.MoreOrEqualToken => (leftMeasure.Measure_ >= rightMeasure.Measure_) ? new Number(1) : new Number(0),
                    TypesOfToken.LessOrEqualToken => (leftMeasure.Measure_ < rightMeasure.Measure_) ? new Number(1) : new Number(0),
                    TypesOfToken.LessToken => (leftMeasure.Measure_ <= rightMeasure.Measure_) ? new Number(1) : new Number(0),
                    TypesOfToken.EqualToken => (leftMeasure.Measure_ == rightMeasure.Measure_) ? new Number(1) : new Number(0),
                    TypesOfToken.NoEqualToken => (leftMeasure.Measure_ != rightMeasure.Measure_) ? new Number(1) : new Number(0),
                    _ => throw new TypeCheckerError(0, 0, "No es posible realizar la operación " + Operator.Lexeme),
                };
            }
            else if (left is Measure measureLeft && right is Number numberRight)
            {
                if (Operator.Type == TypesOfToken.MultToken) return new Measure(measureLeft.Measure_ * Math.Abs((int)numberRight.Value), "");
                else throw new TypeCheckerError(0, 0, "No es posible realizar la operación " + Operator.Lexeme);
            }
            else if (left is Number numberLeft && right is Measure measureRight)
            {
                if (Operator.Type == TypesOfToken.MultToken) return new Measure(measureRight.Measure_ * Math.Abs((int)numberLeft.Value), "");
                else throw new TypeCheckerError(0, 0, "No es posible realizar la operación " + Operator.Lexeme);
            }
            throw new TypeCheckerError(0, 0, "No es posible realizar la operación " + Operator.Lexeme);
        }
    }


    public class IntersectExpression : Expressions, ICheckType
    {
        public override TypesOfToken Type => TypesOfToken.IntersectToken;
        public Expressions Figure1 { get; private set; }
        public Expressions Figure2 { get; private set; }
        public IntersectExpression(Expressions f1, Expressions f2)
        {
            Figure1 = f1;
            Figure2 = f2;
        }

        public Type Check(Scope scope)
        {
            Type figure1 = ((ICheckType)Figure1).Check(scope);
            Type figure2 = ((ICheckType)Figure2).Check(scope);
            List<Type> points = new();

            //Punto, circulo
            if (figure1.TypeOfElement == TypeOfElement.Point && figure2.TypeOfElement == TypeOfElement.Circle || figure1.TypeOfElement == TypeOfElement.Circle && figure2.TypeOfElement == TypeOfElement.Point)
            {
                Point p;
                Circle c;
                if (figure1 is Point) { p = (Point)figure1; c = (Circle)figure2; }
                else { c = (Circle)figure1; p = (Point)figure2; }
                // Calcula la distancia entre el centro de la circunferencia y el punto usando la ecuación de la circunferencia
                double distance = Math.Abs(Math.Sqrt(Math.Pow(p.X - c.Center.X, 2) + Math.Pow(p.Y - c.Center.Y, 2)));
                // Si esta es igual al radio de la circunferencia entonces el punto esta contenido 
                if (distance == Math.Abs(c.Radius.Measure_))
                {
                    points.Add(p);
                }
                return new Sequence(points, "");
            }
            //Linea, circulo
            else if (figure1.TypeOfElement == TypeOfElement.Line && figure2.TypeOfElement == TypeOfElement.Circle || figure1.TypeOfElement == TypeOfElement.Circle && figure2.TypeOfElement == TypeOfElement.Line)
            {
                Line l;
                Circle c;
                if (figure1 is Line) { l = (Line)figure1; c = (Circle)figure2; }
                else { c = (Circle)figure1; l = (Line)figure2; }
                points = IntersectCircleLine(c.Center, c.Radius, l.P1, l.P2);
                return new Sequence(points, "");
            }
            //Ray, circulo
            else if (figure1.TypeOfElement == TypeOfElement.Ray && figure2.TypeOfElement == TypeOfElement.Circle || figure1.TypeOfElement == TypeOfElement.Circle && figure2.TypeOfElement == TypeOfElement.Ray)
            {
                Ray r;
                Circle c;
                if (figure1 is Ray) { r = (Ray)figure1; c = (Circle)figure2; }
                else { c = (Circle)figure1; r = (Ray)figure2; }
                points = IntersectCircleLine(c.Center, c.Radius, r.Start, r.Point);
                double dx = r.Point.X - r.Start.X;
                double dy = r.Point.Y - r.Start.Y;
                // Verifica que el punto pertenezca a la semirrecta
                foreach (var point in points)
                {
                    double intercept = ((((Point)point).X - r.Start.X) * dx + (((Point)point).Y - r.Start.Y) * dy) / (Math.Pow(dx, 2) + Math.Pow(dy, 2));
                    if (intercept < 0)
                    {
                        points.Remove(point);
                    }
                }
                return new Sequence(points, "");
            }
            //Segmento, circulo
            else if (figure1.TypeOfElement == TypeOfElement.Segment && figure2.TypeOfElement == TypeOfElement.Circle || figure1.TypeOfElement == TypeOfElement.Circle && figure2.TypeOfElement == TypeOfElement.Segment)
            {
                Segment s;
                Circle c;
                if (figure1 is Line) { s = (Segment)figure1; c = (Circle)figure2; }
                else { c = (Circle)figure1; s = (Segment)figure2; }
                points = IntersectCircleLine(c.Center, c.Radius, s.Start, s.End);
                double dx = s.End.X - s.Start.X;
                double dy = s.End.Y - s.Start.Y;
                // Verifica que el punto pertenezca a la segmento
                foreach (var point in points)
                {
                    double intercept = ((((Point)point).X - s.Start.X) * dx + (((Point)point).Y - s.Start.Y) * dy) / (Math.Pow(dx, 2) + Math.Pow(dy, 2));
                    if (intercept < 0 || intercept > 1)
                    {
                        points.Remove(point);
                    }
                }
                return new Sequence(points, "");
            }
            //Arco, circulo
            else if (figure1.TypeOfElement == TypeOfElement.Arc && figure2.TypeOfElement == TypeOfElement.Circle || figure1.TypeOfElement == TypeOfElement.Circle && figure2.TypeOfElement == TypeOfElement.Arc)
            {
                Arc a1;
                Circle c;
                if (figure1 is Arc) { a1 = (Arc)figure1; c = (Circle)figure2; }
                else { c = (Circle)figure1; a1 = (Arc)figure2; }

                double inicialAng1 = Math.Atan2(a1.Start.Y - a1.Center.Y, a1.Start.X - a1.Center.X);
                double endAng1 = Math.Atan2(a1.End.Y - a1.Center.Y, a1.End.X - a1.Center.X);
                points = IntersectCircles(a1.Center, a1.Measure, c.Center, c.Radius);
                if (points.Count == 1)
                {
                    Point p = (Point)points[0];
                    //Angulo del punto de intersección respecto al arco
                    double angulo = Math.Atan2(p.Y - a1.Center.Y, p.X - a1.Center.X);
                    if (angulo >= inicialAng1 && angulo <= endAng1)
                    {
                        return new Sequence(points, "");
                    }
                    else
                    {
                        points.Remove(p);
                    }
                }

                if (points.Count == 2)
                {
                    Point p1 = (Point)points[0];
                    //Angulo del punto de intersección respecto al  arco
                    double angulo = Math.Atan2(p1.Y - a1.Center.Y, p1.X - a1.Center.X);
                    if (angulo >= inicialAng1 && angulo <= endAng1)
                    {
                    }
                    else
                    {
                        points.Remove(p1);
                    }

                    Point p2 = (Point)points[1];
                    //Angulo del punto de intersección respecto al  arco
                    angulo = Math.Atan2(p2.Y - a1.Center.Y, p2.X - a1.Center.X);
                    if (angulo >= inicialAng1 && angulo <= endAng1)
                        return new Sequence(points, "");

                    else
                    {
                        points.Remove(p2);
                    }

                    return new Sequence(points, "");

                }
            }
            //Circulo, circulo
            else if (figure1.TypeOfElement == TypeOfElement.Circle && figure2.TypeOfElement == TypeOfElement.Circle)
            {
                Circle c1 = (Circle)figure1;
                Circle c2 = (Circle)figure2;
                points = IntersectCircles(c1.Center, c1.Radius, c2.Center, c2.Radius);
                if (points.Count != 0)
                    return new Sequence(points, "");

            }
            //Punto, punto
            else if (figure1.TypeOfElement == TypeOfElement.Point && figure2.TypeOfElement == TypeOfElement.Point)
            {
                Point p = (Point)figure1;
                Point s = (Point)figure2;
                if (p.X == s.X && p.Y == s.Y)
                {
                    points.Add(p);
                }
                return new Sequence(points, "");
            }
            //Punto, linea
            else if (figure1.TypeOfElement == TypeOfElement.Point && figure2.TypeOfElement == TypeOfElement.Line || figure1.TypeOfElement == TypeOfElement.Line && figure2.TypeOfElement == TypeOfElement.Point)
            {
                Point p;
                Line l;
                if (figure1 is Point) { p = (Point)figure1; l = (Line)figure2; }
                else { l = (Line)figure1; p = (Point)figure2; }
                if (p.Y - l.P1.Y == (l.P2.Y - l.P1.Y) / (l.P2.X - l.P1.X) * (p.X - l.P1.X))
                {
                    points.Add(p);
                }
                return new Sequence(points, "");
            }
            //Punto, ray
            else if (figure1.TypeOfElement == TypeOfElement.Point && figure2.TypeOfElement == TypeOfElement.Ray || figure1.TypeOfElement == TypeOfElement.Ray && figure2.TypeOfElement == TypeOfElement.Point)
            {
                Point p;
                Ray r;
                if (figure1 is Point) { p = (Point)figure1; r = (Ray)figure2; }
                else { r = (Ray)figure1; p = (Point)figure2; }
                double dx = r.Point.X - r.Start.X;
                double dy = r.Point.Y - r.Start.Y;
                double intercept = ((p.X - r.Start.X) * dx + (p.Y - r.Start.Y) * dy) / (Math.Pow(dx, 2) + Math.Pow(dy, 2));
                if (intercept >= 0)
                {
                    points.Add(p);
                }
                return new Sequence(points, "");
            }

            //Punto, segmento
            else if (figure1.TypeOfElement == TypeOfElement.Point && figure2.TypeOfElement == TypeOfElement.Segment || figure1.TypeOfElement == TypeOfElement.Segment && figure2.TypeOfElement == TypeOfElement.Point)
            {
                Point p;
                Segment s;
                if (figure1 is Segment) { s = (Segment)figure1; p = (Point)figure2; }
                else { p = (Point)figure1; s = (Segment)figure2; }
                double dx = s.End.X - s.Start.X;
                double dy = s.End.Y - s.Start.Y;
                // Verifica que el punto pertenezca a la segmento

                double intercept = ((p.X - s.Start.X) * dx + (p.Y - s.Start.Y) * dy) / (Math.Pow(dx, 2) + Math.Pow(dy, 2));
                if (intercept >= 0 && intercept <= 1)
                {
                    points.Add(p);
                }
                return new Sequence(points, "");
            }

            //Punto, arco
            else if (figure1.TypeOfElement == TypeOfElement.Point && figure2.TypeOfElement == TypeOfElement.Arc || figure1.TypeOfElement == TypeOfElement.Arc && figure2.TypeOfElement == TypeOfElement.Point)
            {
                Point p;
                Arc c;
                if (figure1 is Point) { p = (Point)figure1; c = (Arc)figure2; }
                else { c = (Arc)figure1; p = (Point)figure2; }
                // Calcula la distancia entre el centro del arco y el punto usando la ecuación de la circunferencia
                double distance = Math.Abs(Math.Sqrt(Math.Pow(p.X - c.Center.X, 2) + Math.Pow(p.Y - c.Center.Y, 2)));
                // Si esta es igual al radio de la circunferencia entonces el punto esta contenido 
                if (distance == Math.Abs(c.Measure.Measure_))
                {
                    //Calcular si el punto esta entre los limites del arco
                    double arcPointsDistance = Math.Abs(Math.Sqrt(Math.Pow(c.Start.X - c.End.X, 2) + Math.Pow(c.Start.Y - c.End.Y, 2)));
                    double starPointDistance = Math.Abs(Math.Sqrt(Math.Pow(p.X - c.Start.X, 2) + Math.Pow(p.Y - c.Start.Y, 2)));
                    double endPointDistance = Math.Abs(Math.Sqrt(Math.Pow(p.X - c.End.X, 2) + Math.Pow(p.Y - c.End.Y, 2)));
                    if (arcPointsDistance == (starPointDistance + endPointDistance))
                    {
                        points.Add(p);
                    }
                }
                return new Sequence(points, "");
            }
            //Arco, arco
            else if (figure1.TypeOfElement == TypeOfElement.Arc && figure2.TypeOfElement == TypeOfElement.Arc)
            {
                Arc a1 = (Arc)figure1;
                Arc a2 = (Arc)figure2;

                double inicialAng1 = Math.Atan2(a1.Start.Y - a1.Center.Y, a1.Start.X - a1.Center.X);
                double endAng1 = Math.Atan2(a1.End.Y - a1.Center.Y, a1.End.X - a1.Center.X);

                double inicialAng2 = Math.Atan2(a2.Start.Y - a2.Center.Y, a2.Start.X - a2.Center.X);
                double endAng2 = Math.Atan2(a2.End.Y - a2.Center.Y, a2.End.X - a2.Center.X);

                points = IntersectCircles(a1.Center, a1.Measure, a2.Center, a2.Measure);
                if (points.Count == 1)
                {
                    Point p = (Point)points[0];
                    //Angulo del punto de intersección respecto al primer arco
                    double angulo = Math.Atan2(p.Y - a1.Center.Y, p.X - a1.Center.X);
                    if (angulo >= inicialAng1 && angulo <= endAng1)
                    {
                        //Angulo del punto de intersección respecto al segundo arco
                        angulo = Math.Atan2(p.Y - a2.Center.Y, p.X - a2.Center.X);
                        if (angulo >= inicialAng2 && angulo <= endAng2)
                        {
                            return new Sequence(points, "");
                        }
                        else
                        {
                            points.Remove(p);
                        }
                    }
                    else
                    {
                        points.Remove(p);
                    }
                }
                if (points.Count == 2)
                {
                    Point p1 = (Point)points[1];
                    //Angulo del punto de intersección respecto al primer arco
                    double angulo = Math.Atan2(p1.Y - a1.Center.Y, p1.X - a1.Center.X);
                    if (angulo >= inicialAng1 && angulo <= endAng1)
                    {
                        //Angulo del punto de intersección respecto al segundo arco
                        angulo = Math.Atan2(p1.Y - a2.Center.Y, p1.X - a2.Center.X);
                        if (!(angulo >= inicialAng2) && !(angulo <= endAng2))
                        {
                            points.Remove(p1);
                        }
                    }
                    else
                    {
                        points.Remove(p1);
                    }

                    Point p2 = (Point)points[0];
                    //Angulo del punto de intersección respecto al primer arco
                    angulo = Math.Atan2(p2.Y - a1.Center.Y, p2.X - a1.Center.X);
                    if (angulo >= inicialAng1 && angulo <= endAng1)
                    {
                        //Angulo del punto de intersección respecto al segundo arco
                        angulo = Math.Atan2(p2.Y - a2.Center.Y, p2.X - a2.Center.X);
                        if (angulo >= inicialAng2 && angulo <= endAng2)
                        {
                            return new Sequence(points, "");
                        }
                        else
                        {
                            points.Remove(p2);
                        }
                    }
                    else
                    {
                        points.Remove(p2);
                    }
                    if (points.Count == 1)
                    {
                        return new Sequence(points, "");
                    }

                }

            }
            //Linea, linea 
            else if (figure1.TypeOfElement == TypeOfElement.Line && figure2.TypeOfElement == TypeOfElement.Line)
            {
                Line l1 = (Line)figure1;
                Line l2 = (Line)figure2;
                points = IntersectLines(l1.P1, l1.P2, l2.P1, l2.P2);
                if (points.Count == 1)
                {
                    return new Sequence(points, "");
                }
            }
            //Linea, segmento
            else if (figure1.TypeOfElement == TypeOfElement.Segment && figure2.TypeOfElement == TypeOfElement.Line || figure1.TypeOfElement == TypeOfElement.Line && figure2.TypeOfElement == TypeOfElement.Segment)
            {
                Segment s;
                Line l;
                if (figure1 is Segment) { s = (Segment)figure1; l = (Line)figure2; }
                else { l = (Line)figure1; s = (Segment)figure2; }
                points = IntersectLines(l.P1, l.P2, s.Start, s.End);
                if (points.Count == 1)
                {
                    double dx = s.End.X - s.Start.X;
                    double dy = s.End.Y - s.Start.Y;
                    Point p = (Point)points[0];
                    // Verifica que el punto pertenezca a la segmento
                    double intercept = ((p.X - s.Start.X) * dx + (p.Y - s.Start.Y) * dy) / (Math.Pow(dx, 2) + Math.Pow(dy, 2));
                    if (!(intercept < 0) && !(intercept > 1))
                    {
                        points.Remove(p);
                    }
                    else return new Sequence(points, "");
                }
            }
            //Ray, lines
            else if (figure1.TypeOfElement == TypeOfElement.Ray && figure2.TypeOfElement == TypeOfElement.Line || figure1.TypeOfElement == TypeOfElement.Line && figure2.TypeOfElement == TypeOfElement.Ray)
            {
                Ray r;
                Line l;
                if (figure1 is Segment) { r = (Ray)figure1; l = (Line)figure2; }
                else { l = (Line)figure1; r = (Ray)figure2; }
                points = IntersectLines(l.P1, l.P2, r.Start, r.Point);
                if (points.Count == 1)
                {
                    Point p = (Point)points[0];
                    double dx = r.Point.X - r.Start.X;
                    double dy = r.Point.Y - r.Start.Y;
                    double intercept = ((p.X - r.Start.X) * dx + (p.Y - r.Start.Y) * dy) / (Math.Pow(dx, 2) + Math.Pow(dy, 2));
                    if (intercept >= 0)
                    {
                        return new Sequence(points, "");
                    }
                    else points.Remove(p);
                }
            }
            //Linea, arco
            else if (figure1.TypeOfElement == TypeOfElement.Arc && figure2.TypeOfElement == TypeOfElement.Line || figure1.TypeOfElement == TypeOfElement.Line && figure2.TypeOfElement == TypeOfElement.Arc)
            {
                Arc a;
                Line l;
                if (figure1 is Segment) { a = (Arc)figure1; l = (Line)figure2; }
                else { l = (Line)figure1; a = (Arc)figure2; }
                points = IntersectCircleLine(a.Center, a.Measure, l.P1, l.P2);
                if (points.Count == 1)
                {
                    Point p = (Point)points[0];
                    if (IntersectPointArc(p, a))
                        return new Sequence(points, "");
                    points.Remove(p);
                }
                if (points.Count == 2)
                {
                    Point p1 = (Point)points[0];
                    if (!IntersectPointArc(p1, a))
                    {
                        points.Remove(p1);
                    }
                    Point p2 = (Point)points[1];
                    if (IntersectPointArc(p2, a))
                        return new Sequence(points, "");
                    else points.Remove(p2);
                    if (points.Count == 1)
                        return new Sequence(points, "");
                }
            }
            //segmento, segmento
            else if (figure1.TypeOfElement == TypeOfElement.Segment && figure2.TypeOfElement == TypeOfElement.Segment)
            {
                Segment s1 = (Segment)figure1;
                Segment s2 = (Segment)figure2;
                points = IntersectLines(s1.Start, s1.End, s2.Start, s2.End);
                if (points.Count == 1)
                {
                    Point p = (Point)points[0];
                    if (IntersectPointSegment(p, s1) && IntersectPointSegment(p, s2))
                        return new Sequence(points, "");
                    else points.Remove(p);
                }
            }
            //Segmento, ray
            else if (figure1.TypeOfElement == TypeOfElement.Ray && figure2.TypeOfElement == TypeOfElement.Segment || figure1.TypeOfElement == TypeOfElement.Segment && figure2.TypeOfElement == TypeOfElement.Ray)
            {
                Segment s;
                Ray r;
                if (figure1 is Segment) { s = (Segment)figure1; r = (Ray)figure2; }
                else { r = (Ray)figure1; s = (Segment)figure2; }
                points = IntersectLines(r.Start, r.Point, s.Start, s.End);
                if (points.Count == 1)
                {
                    Point p = (Point)points[0];
                    if (!IntersectPointRay(p, r) || !IntersectPointSegment(p, s))
                    {
                        points.Remove(p);
                    }

                }
                return new Sequence(points, "");
            }
            //Segmento, arco
            else if (figure1.TypeOfElement == TypeOfElement.Arc && figure2.TypeOfElement == TypeOfElement.Segment || figure1.TypeOfElement == TypeOfElement.Segment && figure2.TypeOfElement == TypeOfElement.Arc)
            {
                Segment s;
                Arc a;
                if (figure1 is Segment) { s = (Segment)figure1; a = (Arc)figure2; }
                else { a = (Arc)figure1; s = (Segment)figure2; }
                points = IntersectCircleLine(a.Center, a.Measure, s.Start, s.End);
                if (points.Count == 1)
                {
                    Point p = (Point)points[0];
                    if (!IntersectPointArc(p, a) || !IntersectPointSegment(p, s))
                    {
                        points.Remove(p);
                    }
                }
                return new Sequence(points, "");
            }

            //Ray, ray
            else if (figure1.TypeOfElement == TypeOfElement.Ray && figure2.TypeOfElement == TypeOfElement.Ray)
            {
                Ray r1 = (Ray)figure1;
                Ray r2 = (Ray)figure2;
                points = IntersectLines(r1.Start, r1.Point, r2.Start, r2.Point);
                if (points.Count == 1)
                {
                    Point p = (Point)points[0];
                    if (!IntersectPointRay(p, r1) || !IntersectPointRay(p, r2))
                    {
                        points.Remove(p);
                    }
                }
                return new Sequence(points, "");
            }
            //Ray,Arc
            else if (figure1.TypeOfElement == TypeOfElement.Arc && figure2.TypeOfElement == TypeOfElement.Ray || figure1.TypeOfElement == TypeOfElement.Ray && figure2.TypeOfElement == TypeOfElement.Arc)
            {
                Ray r;
                Arc a;
                if (figure1 is Ray) { r = (Ray)figure1; a = (Arc)figure2; }
                else { a = (Arc)figure1; r = (Ray)figure2; }
                points = IntersectCircleLine(a.Center, a.Measure, r.Start, r.Point);
                if (points.Count == 1)
                {
                    Point p = (Point)points[0];
                    if (!IntersectPointArc(p, a) || !IntersectPointRay(p, r))
                    {
                        points.Remove(p);
                    }
                }
                return new Sequence(points, "");
            }


            throw new TypeCheckerError(0, 0, "No es posible calcular la intersección entre " + Figure1 + " y " + Figure2);
        }

        bool IntersectPointSegment(Point p, Segment s)
        {
            double dx = s.End.X - s.Start.X;
            double dy = s.End.Y - s.Start.Y;
            // Verifica que el punto pertenezca a la segmento
            double intercept = ((p.X - s.Start.X) * dx + (p.Y - s.Start.Y) * dy) / (Math.Pow(dx, 2) + Math.Pow(dy, 2));
            if (intercept < 0 || intercept > 1)
            {
                return false;
            }
            else return true;
        }

        bool IntersectPointRay(Point p, Ray r)
        {
            double dx = r.Point.X - r.Start.X;
            double dy = r.Point.Y - r.Start.Y;
            double intercept = ((p.X - r.Start.X) * dx + (p.Y - r.Start.Y) * dy) / (Math.Pow(dx, 2) + Math.Pow(dy, 2));
            if (intercept >= 0)
            {
                return true;
            }
            else return false;
        }

        bool IntersectPointArc(Point p, Arc a1)
        {
            double inicialAng1 = Math.Atan2(a1.Start.Y - a1.Center.Y, a1.Start.X - a1.Center.X);
            double endAng1 = Math.Atan2(a1.End.Y - a1.Center.Y, a1.End.X - a1.Center.X);
            double angulo = Math.Atan2(p.Y - a1.Center.Y, p.X - a1.Center.X);
            if (angulo >= inicialAng1 && angulo <= endAng1)
            {
                return true;
            }
            else return false;
        }

        List<Type> IntersectCircleLine(Point Center, Measure Radius, Point P1, Point P2)
        {
            List<Type> points = new();
            // m es la pendiente de la recta 
            double m = (P2.Y - P1.Y) / (P2.X - P1.X);
            // ecuación de la recta 
            double b = P1.Y - m * P1.X;
            // coordenada x del centro de la circunferencia
            double h = Center.X;
            // coordenada y del centro de la circunferencia
            double k = Center.Y;
            // radio de la circunferencia
            double r = Radius.Measure_;
            // A, B y C son coeficientes de la ecuación cuadrática que se obtiene al sustituir la ecuación de la recta en la ecuación de la circunferencia para encontrar las coordenadas de los puntos de intersección 
            double A = 1 + Math.Pow(m, 2);
            double B = -2 * h + 2 * m * b - 2 * k * m;
            double C = Math.Pow(h, 2) + Math.Pow(b, 2) + Math.Pow(k, 2) - Math.Pow(r, 2) - 2 * b * k;
            double discriminant = Math.Pow(B, 2) - 4.0 * A * C;

            // Si el discriminante es negativo no existe intersección
            // Si el discriminante es cero existe un solo punto de intersección
            if (discriminant == 0)
            {
                double x = -B / (2 * A);
                double y = m * x + b;
                Point intersect = new("")
                {
                    X = x,
                    Y = y
                };
                points.Add(intersect);
            }
            // Si el discriminante es positivo existen dos puntos de intersección
            else
            {
                double x1 = (-B + Math.Sqrt(discriminant)) / (2.0 * A);
                double y1 = m * x1 + b;
                double x2 = (-B - Math.Sqrt(discriminant)) / (2.0 * A);
                double y2 = m * x2 + b;
                Point intersect1 = new("")
                {
                    X = x1,
                    Y = y1
                };
                Point intersect2 = new("")
                {
                    X = x2,
                    Y = y2
                };
                points.Add(intersect1);
                points.Add(intersect2);
            }
            return points;
        }

        List<Type> IntersectCircles(Point Center1, Measure Radius1, Point Center2, Measure Radius2)
        {
            List<Type> points = new();
            //distancia entre los centros de las circunferencias
            double d = Math.Sqrt(Math.Pow(Center2.X - Center1.X, 2) + Math.Pow(Center2.Y - Center1.Y, 2));
            //si la distancia es igual a la suma de los radios entonces son tangentes
            if (d == Radius1.Measure_ + Radius2.Measure_)
            {
                double x = (Radius1.Measure_ * Center2.X - Radius2.Measure_ * Center1.X) / (Radius1.Measure_ - Radius2.Measure_);
                double y = (Radius1.Measure_ * Center2.Y - Radius2.Measure_ * Center1.Y) / (Radius1.Measure_ - Radius2.Measure_);
                Point p = new("")
                {
                    X = x,
                    Y = y
                };
                points.Add(p);
            }
            //en el primer caso uno de los círculos estaría contenido dentro del otro y en el otro no se intersectan
            else if (d > Math.Abs(Radius1.Measure_ - Radius2.Measure_) && d < Radius1.Measure_ + Radius2.Measure_)
            {
                double a = (Math.Pow(Radius1.Measure_, 2) - Math.Pow(Radius2.Measure_, 2) + Math.Pow(d, 2)) / (2 * d);
                double h = Math.Sqrt(Math.Pow(Radius1.Measure_, 2) - Math.Pow(a, 2));

                double x2 = Center1.X + a * (Center2.X - Center1.X) / d;
                double y2 = Center1.Y + a * (Center2.Y - Center1.Y) / d;

                Point p1 = new("")
                {
                    X = x2 + h * (Center2.Y - Center1.Y) / d,
                    Y = y2 - h * (Center2.X - Center1.X) / d
                };
                Point p2 = new("")
                {
                    X = x2 - h * (Center2.Y - Center1.Y) / d,
                    Y = y2 + h * (Center2.X - Center1.X) / d
                };
                points.Add(p1);
                points.Add(p2);
            }
            return points;
        }

        List<Type> IntersectLines(Point firstlinep1, Point firstlinep2, Point secondtlinep1, Point secondtlinep2)
        {
            List<Type> points = new();
            //Hallar pendiente de la primera linea
            double m1 = (firstlinep1.Y - firstlinep2.Y) / (firstlinep1.X - firstlinep2.X);
            //Hallar punto medio de la primera linea
            //Para coordenadas X
            double xMiddle1 = (firstlinep1.X + firstlinep2.X) / 2;
            //Para coordenadas Y
            double yMiddle1 = (firstlinep1.Y + firstlinep2.Y) / 2;

            //Hallar pendiente de la segunda linea
            double m2 = (secondtlinep1.Y - secondtlinep2.Y) / (secondtlinep1.X - secondtlinep2.X);
            //Hallar punto medio de la primera linea
            //Para coordenadas X
            double xMiddle2 = (secondtlinep1.X + secondtlinep2.X) / 2;
            //Para coordenadas Y
            double yMiddle2 = (secondtlinep1.Y + secondtlinep2.Y) / 2;
            if (m1 != m2)
            {
                // Coordenada x del punto de intersección
                double x = (yMiddle2 - yMiddle1 + m1 * xMiddle1 - m2 * xMiddle1) / (m1 - m2);
                // Coordenada y del punto de intersección
                double y = yMiddle1 + m1 * (x - xMiddle1);
                Point intersect = new("")
                {
                    X = x,
                    Y = y
                };
                points.Add(intersect);
            }
            return points;
        }
    }

}
