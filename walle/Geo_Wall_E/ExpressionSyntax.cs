
namespace Geo_Wall_E
{
    public class ArcExpressionSyntax : ExpressionSyntax, ICheckType
    {
        public override SyntaxKind Type => SyntaxKind.ArcToken;
        public ExpressionSyntax CenterPoint { get; private set; }
        public ExpressionSyntax StartPoint { get; private set; }
        public ExpressionSyntax EndPoint { get; private set; }
        public ExpressionSyntax Measure { get; private set; }

        public ArcExpressionSyntax(ExpressionSyntax centerPoint, ExpressionSyntax startPoint, ExpressionSyntax endPoint, ExpressionSyntax measure)
        {
            StartPoint = startPoint;
            EndPoint = endPoint;
            CenterPoint = centerPoint;            
            Measure = measure;
        }

        public Type Check(Scope scope)
        {
            Type startPoint = ((ICheckType)StartPoint).Check(scope);
            Type endPoint = ((ICheckType)EndPoint).Check(scope);
            Type measure = ((ICheckType)Measure).Check(scope);
            Type centerPoint = ((ICheckType)CenterPoint).Check(scope);
        
            if (centerPoint.SyntaxType != SyntaxType.Point) throw new TypeCheckerError(0, 0, "Se esperaba un punto pero en su lugar se obtuvo " + centerPoint.SyntaxType);
            else if (startPoint.SyntaxType != SyntaxType.Point) throw new TypeCheckerError(0, 0, "Se esperaba un punto pero en su lugar se obtuvo " + startPoint.SyntaxType);
            else if (endPoint.SyntaxType != SyntaxType.Point) throw new TypeCheckerError(0, 0, "Se esperaba un punto pero en su lugar se obtuvo " + endPoint.SyntaxType);
            else if (measure.SyntaxType != SyntaxType.Measure) throw new TypeCheckerError(0, 0, "Se esperaba una medida pero en su lugar se obtuvo " + measure.SyntaxType);
            else return new ArcType((PointType)centerPoint, (PointType)startPoint, (PointType)endPoint, (Measure)measure, null!);
        }
    }
    public class BinaryExpressionSyntax : ExpressionSyntax, ICheckType
    {
        public override SyntaxKind Type => SyntaxKind.BinaryExpression;
        public ExpressionSyntax LeftExpression { get; private set; }
        public SyntaxToken OperatorToken { get; private set; }
        public ExpressionSyntax RightExpression { get; private set; }
        public BinaryExpressionSyntax(ExpressionSyntax leftExpression, SyntaxToken operatorToken, ExpressionSyntax rightExpression)
        {
            LeftExpression = leftExpression;
            OperatorToken = operatorToken;
            RightExpression = rightExpression;
        }

        public Type Check(Scope scope)
        {
            Type left = ((ICheckType)LeftExpression).Check(scope);
            Type right = ((ICheckType)RightExpression).Check(scope);
            if (left is Number Left && right is Number Right)
            {
                switch (OperatorToken.Kind)
                {
                    //arithmetic
                    case SyntaxKind.PlusToken:
                        return new Number(Left.Value + Right.Value);
                    case SyntaxKind.MinusToken:
                        return new Number(Left.Value - Right.Value);
                    case SyntaxKind.PowToken:
                        return new Number(Math.Pow(Left.Value, Right.Value));
                    case SyntaxKind.StarToken:
                        return new Number(Left.Value * Right.Value);
                    case SyntaxKind.SlashToken:
                        if (Right.Value == 0) throw new TypeCheckerError(0, 0, "Impossible(Dividing by zero)");
                        return new Number(Left.Value / Right.Value);
                    case SyntaxKind.ModuToken:
                        if (Right.Value == 0) throw new TypeCheckerError(0, 0, "Impossible(Dividing by zero)");
                        return new Number(Left.Value % Right.Value);
                    //comp
                    case SyntaxKind.MoreToken:
                        if (Left.Value > Right.Value) return new Number(1);
                        else return new Number(0);
                    case SyntaxKind.MoreOrEqualToken:
                        if (Left.Value >= Right.Value) return new Number(1);
                        else return new Number(0);
                    case SyntaxKind.LessToken:
                        if (Left.Value < Right.Value) return new Number(1);
                        else return new Number(0);
                    case SyntaxKind.LessOrEqualToken:
                        if (Left.Value <= Right.Value) return new Number(1);
                        else return new Number(0);
                    case SyntaxKind.DoubleEqualToken:
                        if (Left.Value == Right.Value) return new Number(1);
                        else return new Number(0);
                    
                    //Logical Operators
                    case SyntaxKind.NoEqualToken:
                        if (Left.Value != Right.Value) return new Number(1);
                        else return new Number(0);
                    case SyntaxKind.AndToken:
                        if (Left.Value != 0 && Right.Value != 0) return new Number(1);
                        else return new Number(0);
                    case SyntaxKind.OrToken:
                        if (Left.Value == 1 || Right.Value == 1) return new Number(1);
                        else return new Number(0);
                    default:
                        throw new TypeCheckerError(0, 0, "Impossible" + OperatorToken.Lexeme);
                }
            }
            else if (left is Measure leftMeasure && right is Measure rightMeasure)
            {
                return OperatorToken.Kind switch
                {
                    SyntaxKind.MoreOrEqualToken => (leftMeasure.Distance < rightMeasure.Distance) ? new Number(0) : new Number(1),
                    SyntaxKind.LessOrEqualToken => (leftMeasure.Distance <= rightMeasure.Distance) ? new Number(1) : new Number(0),
                    SyntaxKind.LessToken => (leftMeasure.Distance < rightMeasure.Distance) ? new Number(1) : new Number(0),
                    SyntaxKind.EqualToken => (leftMeasure.Distance != rightMeasure.Distance) ? new Number(0) : new Number(1),
                    SyntaxKind.NoEqualToken => (leftMeasure.Distance != rightMeasure.Distance) ? new Number(1) : new Number(0),
                    SyntaxKind.PlusToken => new Measure(leftMeasure.Distance + rightMeasure.Distance, ""),
                    SyntaxKind.MinusToken => new Measure(Math.Abs(leftMeasure.Distance - rightMeasure.Distance), ""),
                    SyntaxKind.SlashToken => new Number((int)(leftMeasure.Distance / rightMeasure.Distance)),
                    SyntaxKind.MoreToken => (leftMeasure.Distance > rightMeasure.Distance) ? new Number(1) : new Number(0),

                    _ => throw new TypeCheckerError(0, 0, "Impossible" + OperatorToken.Lexeme),
                };
            }
            else if (left is Measure LeftShit && right is Number RightShit)
            {
                if (OperatorToken.Kind == SyntaxKind.StarToken) return new Measure(LeftShit.Distance * Math.Abs((int)RightShit.Value), "");
                else throw new TypeCheckerError(0, 0, "" + OperatorToken.Lexeme);
            }
            else if (left is Number numberLeft && right is Measure measureRight)
            {
                if (OperatorToken.Kind == SyntaxKind.StarToken) return new Measure(measureRight.Distance * Math.Abs((int)numberLeft.Value), "");
                else throw new TypeCheckerError(0, 0, "Impossible" + OperatorToken.Lexeme);
            }
            throw new TypeCheckerError(0, 0, "Impossible" + OperatorToken.Lexeme);
        }
    }
    public class CircleExpressionSyntax : ExpressionSyntax, ICheckType
    {
        public ExpressionSyntax CenterPoint { get; private set; }
        public ExpressionSyntax CircleRadius { get; private set; }
        public override SyntaxKind Type => SyntaxKind.CircleToken;
        
        public CircleExpressionSyntax(ExpressionSyntax centerPoint, ExpressionSyntax circleRadius)
        {
            CenterPoint = centerPoint;
            CircleRadius = circleRadius;
        }

        public Type Check(Scope scope)
        {
            Type Center = ((ICheckType)CenterPoint).Check(scope);
            Type CirclRadius = ((ICheckType)CircleRadius).Check(scope);
            if (Center.SyntaxType != SyntaxType.Point) throw new TypeCheckerError(0, 0, "Unexpected Token : " + Center.SyntaxType + "Expected Token: PointToken ");
            else if (CirclRadius.SyntaxType != SyntaxType.Measure) throw new TypeCheckerError(0, 0, "Unexpected Token: " + CirclRadius.SyntaxType + "Expected Token: PointToken");
            else return new CircleType((PointType)Center, (Measure)CirclRadius, "");
        }
    }

    public class ConditionalExpressionSyntax : ExpressionSyntax, ICheckType
    {
        public override SyntaxKind Type => SyntaxKind.ConditionalExpression;
        public ExpressionSyntax IfExpression { get; private set; }
        public ExpressionSyntax ThenExpression { get; private set; }
        public ExpressionSyntax ElseExpression { get; private set; }
        public ConditionalExpressionSyntax(ExpressionSyntax ifexpression, ExpressionSyntax thenExpression, ExpressionSyntax elseExpression)
        {
            ThenExpression = thenExpression;
            ElseExpression = elseExpression;
            IfExpression = ifexpression; 
        }

        public Type Check(Scope scope)
        {
            Type ifExpression = ((ICheckType)IfExpression).Check(scope);
            {
                if (ifExpression.SyntaxType == SyntaxType.Number)
                {
                    if (((Number)ifExpression).Value == 0)
                    {
                        return ((ICheckType)ElseExpression).Check(scope);
                    }
                    else return ((ICheckType)ThenExpression).Check(scope);
                }
                if (ifExpression.SyntaxType == SyntaxType.Undefined) return ((ICheckType)ElseExpression).Check(scope);
                if (ifExpression.SyntaxType == SyntaxType.Sequence)
                {
                    if (((SequenceType)ifExpression).SequenceElements.Count == 0) return ((ICheckType)ElseExpression).Check(scope);
                    else return ((ICheckType)ThenExpression).Check(scope);
                }
                return ((ICheckType)ThenExpression).Check(scope);
            }
        }
    }

    public class FunctionCallingExpressionSyntax : ExpressionSyntax, ICheckType

    {
        public override SyntaxKind Type => SyntaxKind.FunctionCallExpression;
        public SyntaxToken Name { get; private set; }
        public List<ExpressionSyntax> Args { get; private set; }
        public SyntaxNode FunctionBody { get; private set; }

        public FunctionCallingExpressionSyntax(SyntaxToken name, List<ExpressionSyntax> args, SyntaxNode functionBody)
        {
            Name = name;
            Args = args;
            FunctionBody = functionBody;
        }
        int maxCallings = 100;
        int cantCall = 0;
        public Type Check(Scope scope)
        {
            if (cantCall < maxCallings)
            {
                Type function = scope.GetTypes(Name.Lexeme!);
                if (function.SyntaxType == SyntaxType.Function)
                {
                    if (Args.Count == ((Function)function).Arguments.Count)
                    {
                        Dictionary<string, Type> args = new();
                        for (int i=0; i<Args.Count;i++)
                        {
                            VariableExpressionSyntax variable = (VariableExpressionSyntax)((Function)function).Arguments[i];
                            try
                            {
                                args.Add(variable.Name.Lexeme!, ((ICheckType)Args[i]).Check(scope));
                            }
                            catch (TypeCheckerError)
                            {
                                throw new TypeCheckerError(0, 0, "Can't evaluate the arg => " + ((Function)function).Arguments[i]!.ToString()!);
                            }
                        }
                        scope.SetScope();
                        cantCall++;
                        foreach (var arg in args)
                        {
                            scope.SetSyntaxKind(arg.Key, arg.Value);
                        }
                        Type ans = ((ICheckType)((Function)function).Body).Check(scope);
                        scope.DeleteScope();
                        cantCall--;
                        return ans;
                    }
                    throw new TypeCheckerError(0, 0, "Function" + ((Function)function).Name + " Expected " + ((Function)function).Arguments.Count + " args ");
                }
                throw new TypeCheckerError(0, 0, "Function" + ((Function)function).Name + "hasn't been defined");
            }
            throw new SemanticError(0, 0, "Stack Overflow");
        }
    }

    public class IntersectExpression : ExpressionSyntax, ICheckType
    {
        public override SyntaxKind Type => SyntaxKind.IntersectionToken;
        public ExpressionSyntax Figure1 { get; private set; }
        public ExpressionSyntax Figure2 { get; private set; }
        public IntersectExpression(ExpressionSyntax f1, ExpressionSyntax f2)
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
            if (figure1.SyntaxType == SyntaxType.Point && figure2.SyntaxType == SyntaxType.Circle || figure1.SyntaxType == SyntaxType.Circle && figure2.SyntaxType == SyntaxType.Point)
            {
                PointType p;
                CircleType c;
                if (figure1 is PointType) { p = (PointType)figure1; c = (CircleType)figure2; }
                else { c = (CircleType)figure1; p = (PointType)figure2; }
                // Calcula la distancia entre el centro de la circunferencia y el punto usando la ecuación de la circunferencia
                double distance = Math.Abs(Math.Sqrt(Math.Pow(p.X - c.CenterPoint.X, 2) + Math.Pow(p.Y - c.CenterPoint.Y, 2)));
                // Si esta es igual al radio de la circunferencia entonces el punto esta contenido 
                if (distance == Math.Abs(c.CircleRadius.Distance))
                {
                    points.Add(p);
                }
                return new SequenceType(points, "");
            }
            //Linea, circulo
            else if (figure1.SyntaxType == SyntaxType.Line && figure2.SyntaxType == SyntaxType.Circle || figure1.SyntaxType == SyntaxType.Circle && figure2.SyntaxType == SyntaxType.Line)
            {
                Line l;
                CircleType c;
                if (figure1 is Line) { l = (Line)figure1; c = (CircleType)figure2; }
                else { c = (CircleType)figure1; l = (Line)figure2; }
                points = IntersectCircleLine(c.CenterPoint, c.CircleRadius, l.Point1, l.Point2);
                return new SequenceType(points, "");
            }
            //RayType, circulo
            else if (figure1.SyntaxType == SyntaxType.Ray && figure2.SyntaxType == SyntaxType.Circle || figure1.SyntaxType == SyntaxType.Circle && figure2.SyntaxType == SyntaxType.Ray)
            {
                RayType r;
                CircleType c;
                if (figure1 is RayType) { r = (RayType)figure1; c = (CircleType)figure2; }
                else { c = (CircleType)figure1; r = (RayType)figure2; }
                points = IntersectCircleLine(c.CenterPoint, c.CircleRadius, r.StartPoint, r.Point);
                double dx = r.Point.X - r.StartPoint.X;
                double dy = r.Point.Y - r.StartPoint.Y;
                // Verifica que el punto pertenezca a la semirrecta
                foreach (var point in points)
                {
                    double intercept = ((((PointType)point).X - r.StartPoint.X) * dx + (((PointType)point).Y - r.StartPoint.Y) * dy) / (Math.Pow(dx, 2) + Math.Pow(dy, 2));
                    if (intercept < 0)
                    {
                        points.Remove(point);
                    }
                }
                return new SequenceType(points, "");
            }
            //SegmentTypeo, circulo
            else if (figure1.SyntaxType == SyntaxType.Segment && figure2.SyntaxType == SyntaxType.Circle || figure1.SyntaxType == SyntaxType.Circle && figure2.SyntaxType == SyntaxType.Segment)
            {
                SegmentType s;
                CircleType c;
                if (figure1 is Line) { s = (SegmentType)figure1; c = (CircleType)figure2; }
                else { c = (CircleType)figure1; s = (SegmentType)figure2; }
                points = IntersectCircleLine(c.CenterPoint, c.CircleRadius, s.StartPoint, s.EndPoint);
                double dx = s.EndPoint.X - s.StartPoint.X;
                double dy = s.EndPoint.Y - s.StartPoint.Y;
                // Verifica que el punto pertenezca a la SegmentTypeo
                foreach (var point in points)
                {
                    double intercept = ((((PointType)point).X - s.StartPoint.X) * dx + (((PointType)point).Y - s.StartPoint.Y) * dy) / (Math.Pow(dx, 2) + Math.Pow(dy, 2));
                    if (intercept < 0 || intercept > 1)
                    {
                        points.Remove(point);
                    }
                }
                return new SequenceType(points, "");
            }
            //Arco, circulo
            else if (figure1.SyntaxType == SyntaxType.Arc && figure2.SyntaxType == SyntaxType.Circle || figure1.SyntaxType == SyntaxType.Circle && figure2.SyntaxType == SyntaxType.Arc)
            {
                ArcType a1;
                CircleType c;
                if (figure1 is ArcType) { a1 = (ArcType)figure1; c = (CircleType)figure2; }
                else { c = (CircleType)figure1; a1 = (ArcType)figure2; }

                double inicialAng1 = Math.Atan2(a1.StartPoint.Y - a1.CenterPoint.Y, a1.StartPoint.X - a1.CenterPoint.X);
                double EndPointAng1 = Math.Atan2(a1.EndPoint.Y - a1.CenterPoint.Y, a1.EndPoint.X - a1.CenterPoint.X);
                points = IntersectCircles(a1.CenterPoint, a1.Measure, c.CenterPoint, c.CircleRadius);
                if (points.Count == 1)
                {
                    PointType p = (PointType)points[0];
                    //Angulo del punto de intersección respecto al arco
                    double angulo = Math.Atan2(p.Y - a1.CenterPoint.Y, p.X - a1.CenterPoint.X);
                    if (angulo >= inicialAng1 && angulo <= EndPointAng1)
                    {
                        return new SequenceType(points, "");
                    }
                    else
                    {
                        points.Remove(p);
                    }
                }

                if (points.Count == 2)
                {
                    PointType Point1 = (PointType)points[0];
                    //Angulo del punto de intersección respecto al  arco
                    double angulo = Math.Atan2(Point1.Y - a1.CenterPoint.Y, Point1.X - a1.CenterPoint.X);
                    if (angulo >= inicialAng1 && angulo <= EndPointAng1)
                    {
                    }
                    else
                    {
                        points.Remove(Point1);
                    }

                    PointType Point2 = (PointType)points[1];
                    //Angulo del punto de intersección respecto al  arco
                    angulo = Math.Atan2(Point2.Y - a1.CenterPoint.Y, Point2.X - a1.CenterPoint.X);
                    if (angulo >= inicialAng1 && angulo <= EndPointAng1)
                        return new SequenceType(points, "");

                    else
                    {
                        points.Remove(Point2);
                    }

                    return new SequenceType(points, "");

                }
            }
            //Circulo, circulo
            else if (figure1.SyntaxType == SyntaxType.Circle && figure2.SyntaxType == SyntaxType.Circle)
            {
                CircleType c1 = (CircleType)figure1;
                CircleType c2 = (CircleType)figure2;
                points = IntersectCircles(c1.CenterPoint, c1.CircleRadius, c2.CenterPoint, c2.CircleRadius);
                if (points.Count != 0)
                    return new SequenceType(points, "");

            }
            //Punto, punto
            else if (figure1.SyntaxType == SyntaxType.Point && figure2.SyntaxType == SyntaxType.Point)
            {
                PointType p = (PointType)figure1;
                PointType s = (PointType)figure2;
                if (p.X == s.X && p.Y == s.Y)
                {
                    points.Add(p);
                }
                return new SequenceType(points, "");
            }
            //Punto, linea
            else if (figure1.SyntaxType == SyntaxType.Point && figure2.SyntaxType == SyntaxType.Line || figure1.SyntaxType == SyntaxType.Line && figure2.SyntaxType == SyntaxType.Point)
            {
                PointType p;
                Line l;
                if (figure1 is PointType) { p = (PointType)figure1; l = (Line)figure2; }
                else { l = (Line)figure1; p = (PointType)figure2; }
                if (p.Y - l.Point1.Y == (l.Point2.Y - l.Point1.Y) / (l.Point2.X - l.Point1.X) * (p.X - l.Point1.X))
                {
                    points.Add(p);
                }
                return new SequenceType(points, "");
            }
            //Punto, RayType
            else if (figure1.SyntaxType == SyntaxType.Point && figure2.SyntaxType == SyntaxType.Ray || figure1.SyntaxType == SyntaxType.Ray && figure2.SyntaxType == SyntaxType.Point)
            {
                PointType p;
                RayType r;
                if (figure1 is PointType) { p = (PointType)figure1; r = (RayType)figure2; }
                else { r = (RayType)figure1; p = (PointType)figure2; }
                double dx = r.Point.X - r.StartPoint.X;
                double dy = r.Point.Y - r.StartPoint.Y;
                double intercept = ((p.X - r.StartPoint.X) * dx + (p.Y - r.StartPoint.Y) * dy) / (Math.Pow(dx, 2) + Math.Pow(dy, 2));
                if (intercept >= 0)
                {
                    points.Add(p);
                }
                return new SequenceType(points, "");
            }

            //Punto, SegmentTypeo
            else if (figure1.SyntaxType == SyntaxType.Point && figure2.SyntaxType == SyntaxType.Segment || figure1.SyntaxType == SyntaxType.Segment && figure2.SyntaxType == SyntaxType.Point)
            {
                PointType p;
                SegmentType s;
                if (figure1 is SegmentType) { s = (SegmentType)figure1; p = (PointType)figure2; }
                else { p = (PointType)figure1; s = (SegmentType)figure2; }
                double dx = s.EndPoint.X - s.StartPoint.X;
                double dy = s.EndPoint.Y - s.StartPoint.Y;
                // Verifica que el punto pertenezca a la SegmentTypeo

                double intercept = ((p.X - s.StartPoint.X) * dx + (p.Y - s.StartPoint.Y) * dy) / (Math.Pow(dx, 2) + Math.Pow(dy, 2));
                if (intercept >= 0 && intercept <= 1)
                {
                    points.Add(p);
                }
                return new SequenceType(points, "");
            }

            //Punto, arco
            else if (figure1.SyntaxType == SyntaxType.Point && figure2.SyntaxType == SyntaxType.Arc || figure1.SyntaxType == SyntaxType.Arc && figure2.SyntaxType == SyntaxType.Point)
            {
                PointType p;
                ArcType c;
                if (figure1 is PointType) { p = (PointType)figure1; c = (ArcType)figure2; }
                else { c = (ArcType)figure1; p = (PointType)figure2; }
                // Calcula la distancia entre el centro del arco y el punto usando la ecuación de la circunferencia
                double distance = Math.Abs(Math.Sqrt(Math.Pow(p.X - c.CenterPoint.X, 2) + Math.Pow(p.Y - c.CenterPoint.Y, 2)));
                // Si esta es igual al radio de la circunferencia entonces el punto esta contenido 
                if (distance == Math.Abs(c.Measure.Distance))
                {
                    //Calcular si el punto esta entre los limites del arco
                    double arcPointsDistance = Math.Abs(Math.Sqrt(Math.Pow(c.StartPoint.X - c.EndPoint.X, 2) + Math.Pow(c.StartPoint.Y - c.EndPoint.Y, 2)));
                    double starPointDistance = Math.Abs(Math.Sqrt(Math.Pow(p.X - c.StartPoint.X, 2) + Math.Pow(p.Y - c.StartPoint.Y, 2)));
                    double EndPointPointDistance = Math.Abs(Math.Sqrt(Math.Pow(p.X - c.EndPoint.X, 2) + Math.Pow(p.Y - c.EndPoint.Y, 2)));
                    if (arcPointsDistance == (starPointDistance + EndPointPointDistance))
                    {
                        points.Add(p);
                    }
                }
                return new SequenceType(points, "");
            }
            //Arco, arco
            else if (figure1.SyntaxType == SyntaxType.Arc && figure2.SyntaxType == SyntaxType.Arc)
            {
                ArcType a1 = (ArcType)figure1;
                ArcType a2 = (ArcType)figure2;

                double inicialAng1 = Math.Atan2(a1.StartPoint.Y - a1.CenterPoint.Y, a1.StartPoint.X - a1.CenterPoint.X);
                double EndPointAng1 = Math.Atan2(a1.EndPoint.Y - a1.CenterPoint.Y, a1.EndPoint.X - a1.CenterPoint.X);

                double inicialAng2 = Math.Atan2(a2.StartPoint.Y - a2.CenterPoint.Y, a2.StartPoint.X - a2.CenterPoint.X);
                double EndPointAng2 = Math.Atan2(a2.EndPoint.Y - a2.CenterPoint.Y, a2.EndPoint.X - a2.CenterPoint.X);

                points = IntersectCircles(a1.CenterPoint, a1.Measure, a2.CenterPoint, a2.Measure);
                if (points.Count == 1)
                {
                    PointType p = (PointType)points[0];
                    //Angulo del punto de intersección respecto al primer arco
                    double angulo = Math.Atan2(p.Y - a1.CenterPoint.Y, p.X - a1.CenterPoint.X);
                    if (angulo >= inicialAng1 && angulo <= EndPointAng1)
                    {
                        //Angulo del punto de intersección respecto al segundo arco
                        angulo = Math.Atan2(p.Y - a2.CenterPoint.Y, p.X - a2.CenterPoint.X);
                        if (angulo >= inicialAng2 && angulo <= EndPointAng2)
                        {
                            return new SequenceType(points, "");
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
                    PointType Point1 = (PointType)points[1];
                    //Angulo del punto de intersección respecto al primer arco
                    double angulo = Math.Atan2(Point1.Y - a1.CenterPoint.Y, Point1.X - a1.CenterPoint.X);
                    if (angulo >= inicialAng1 && angulo <= EndPointAng1)
                    {
                        //Angulo del punto de intersección respecto al segundo arco
                        angulo = Math.Atan2(Point1.Y - a2.CenterPoint.Y, Point1.X - a2.CenterPoint.X);
                        if (!(angulo >= inicialAng2) && !(angulo <= EndPointAng2))
                        {
                            points.Remove(Point1);
                        }
                    }
                    else
                    {
                        points.Remove(Point1);
                    }

                    PointType Point2 = (PointType)points[0];
                    //Angulo del punto de intersección respecto al primer arco
                    angulo = Math.Atan2(Point2.Y - a1.CenterPoint.Y, Point2.X - a1.CenterPoint.X);
                    if (angulo >= inicialAng1 && angulo <= EndPointAng1)
                    {
                        //Angulo del punto de intersección respecto al segundo arco
                        angulo = Math.Atan2(Point2.Y - a2.CenterPoint.Y, Point2.X - a2.CenterPoint.X);
                        if (angulo >= inicialAng2 && angulo <= EndPointAng2)
                        {
                            return new SequenceType(points, "");
                        }
                        else
                        {
                            points.Remove(Point2);
                        }
                    }
                    else
                    {
                        points.Remove(Point2);
                    }
                    if (points.Count == 1)
                    {
                        return new SequenceType(points, "");
                    }

                }

            }
            //Linea, linea 
            else if (figure1.SyntaxType == SyntaxType.Line && figure2.SyntaxType == SyntaxType.Line)
            {
                Line l1 = (Line)figure1;
                Line l2 = (Line)figure2;
                points = IntersectLines(l1.Point1, l1.Point2, l2.Point1, l2.Point2);
                if (points.Count == 1)
                {
                    return new SequenceType(points, "");
                }
            }
            //Linea, SegmentTypeo
            else if (figure1.SyntaxType == SyntaxType.Segment && figure2.SyntaxType == SyntaxType.Line || figure1.SyntaxType == SyntaxType.Line && figure2.SyntaxType == SyntaxType.Segment)
            {
                SegmentType s;
                Line l;
                if (figure1 is SegmentType) { s = (SegmentType)figure1; l = (Line)figure2; }
                else { l = (Line)figure1; s = (SegmentType)figure2; }
                points = IntersectLines(l.Point1, l.Point2, s.StartPoint, s.EndPoint);
                if (points.Count == 1)
                {
                    double dx = s.EndPoint.X - s.StartPoint.X;
                    double dy = s.EndPoint.Y - s.StartPoint.Y;
                    PointType p = (PointType)points[0];
                    // Verifica que el punto pertenezca a la SegmentTypeo
                    double intercept = ((p.X - s.StartPoint.X) * dx + (p.Y - s.StartPoint.Y) * dy) / (Math.Pow(dx, 2) + Math.Pow(dy, 2));
                    if (!(intercept < 0) && !(intercept > 1))
                    {
                        points.Remove(p);
                    }
                    else return new SequenceType(points, "");
                }
            }
            //RayType, lines
            else if (figure1.SyntaxType == SyntaxType.Ray && figure2.SyntaxType == SyntaxType.Line || figure1.SyntaxType == SyntaxType.Line && figure2.SyntaxType == SyntaxType.Ray)
            {
                RayType r;
                Line l;
                if (figure1 is SegmentType) { r = (RayType)figure1; l = (Line)figure2; }
                else { l = (Line)figure1; r = (RayType)figure2; }
                points = IntersectLines(l.Point1, l.Point2, r.StartPoint, r.Point);
                if (points.Count == 1)
                {
                    PointType p = (PointType)points[0];
                    double dx = r.Point.X - r.StartPoint.X;
                    double dy = r.Point.Y - r.StartPoint.Y;
                    double intercept = ((p.X - r.StartPoint.X) * dx + (p.Y - r.StartPoint.Y) * dy) / (Math.Pow(dx, 2) + Math.Pow(dy, 2));
                    if (intercept >= 0)
                    {
                        return new SequenceType(points, "");
                    }
                    else points.Remove(p);
                }
            }
            //Linea, arco
            else if (figure1.SyntaxType == SyntaxType.Arc && figure2.SyntaxType == SyntaxType.Line || figure1.SyntaxType == SyntaxType.Line && figure2.SyntaxType == SyntaxType.Arc)
            {
                ArcType a;
                Line l;
                if (figure1 is SegmentType) { a = (ArcType)figure1; l = (Line)figure2; }
                else { l = (Line)figure1; a = (ArcType)figure2; }
                points = IntersectCircleLine(a.CenterPoint, a.Measure, l.Point1, l.Point2);
                if (points.Count == 1)
                {
                    PointType p = (PointType)points[0];
                    if (IntersectPointArc(p, a))
                        return new SequenceType(points, "");
                    points.Remove(p);
                }
                if (points.Count == 2)
                {
                    PointType Point1 = (PointType)points[0];
                    if (!IntersectPointArc(Point1, a))
                    {
                        points.Remove(Point1);
                    }
                    PointType Point2 = (PointType)points[1];
                    if (IntersectPointArc(Point2, a))
                        return new SequenceType(points, "");
                    else points.Remove(Point2);
                    if (points.Count == 1)
                        return new SequenceType(points, "");
                }
            }
            //SegmentTypeo, SegmentTypeo
            else if (figure1.SyntaxType == SyntaxType.Segment && figure2.SyntaxType == SyntaxType.Segment)
            {
                SegmentType s1 = (SegmentType)figure1;
                SegmentType s2 = (SegmentType)figure2;
                points = IntersectLines(s1.StartPoint, s1.EndPoint, s2.StartPoint, s2.EndPoint);
                if (points.Count == 1)
                {
                    PointType p = (PointType)points[0];
                    if (IntersectPointSegmentType(p, s1) && IntersectPointSegmentType(p, s2))
                        return new SequenceType(points, "");
                    else points.Remove(p);
                }
            }
            //SegmentTypeo, RayType
            else if (figure1.SyntaxType == SyntaxType.Ray && figure2.SyntaxType == SyntaxType.Segment || figure1.SyntaxType == SyntaxType.Segment && figure2.SyntaxType == SyntaxType.Ray)
            {
                SegmentType s;
                RayType r;
                if (figure1 is SegmentType) { s = (SegmentType)figure1; r = (RayType)figure2; }
                else { r = (RayType)figure1; s = (SegmentType)figure2; }
                points = IntersectLines(r.StartPoint, r.Point, s.StartPoint, s.EndPoint);
                if (points.Count == 1)
                {
                    PointType p = (PointType)points[0];
                    if (!IntersectPointRayType(p, r) || !IntersectPointSegmentType(p, s))
                    {
                        points.Remove(p);
                    }

                }
                return new SequenceType(points, "");
            }
            //SegmentTypeo, arco
            else if (figure1.SyntaxType == SyntaxType.Arc && figure2.SyntaxType == SyntaxType.Segment || figure1.SyntaxType == SyntaxType.Segment && figure2.SyntaxType == SyntaxType.Arc)
            {
                SegmentType s;
                ArcType a;
                if (figure1 is SegmentType) { s = (SegmentType)figure1; a = (ArcType)figure2; }
                else { a = (ArcType)figure1; s = (SegmentType)figure2; }
                points = IntersectCircleLine(a.CenterPoint, a.Measure, s.StartPoint, s.EndPoint);
                if (points.Count == 1)
                {
                    PointType p = (PointType)points[0];
                    if (!IntersectPointArc(p, a) || !IntersectPointSegmentType(p, s))
                    {
                        points.Remove(p);
                    }
                }
                return new SequenceType(points, "");
            }

            //RayType, RayType
            else if (figure1.SyntaxType == SyntaxType.Ray && figure2.SyntaxType == SyntaxType.Ray)
            {
                RayType r1 = (RayType)figure1;
                RayType r2 = (RayType)figure2;
                points = IntersectLines(r1.StartPoint, r1.Point, r2.StartPoint, r2.Point);
                if (points.Count == 1)
                {
                    PointType p = (PointType)points[0];
                    if (!IntersectPointRayType(p, r1) || !IntersectPointRayType(p, r2))
                    {
                        points.Remove(p);
                    }
                }
                return new SequenceType(points, "");
            }
            //RayType,Arc
            else if (figure1.SyntaxType == SyntaxType.Arc && figure2.SyntaxType == SyntaxType.Ray || figure1.SyntaxType == SyntaxType.Ray && figure2.SyntaxType == SyntaxType.Arc)
            {
                RayType r;
                ArcType a;
                if (figure1 is RayType) { r = (RayType)figure1; a = (ArcType)figure2; }
                else { a = (ArcType)figure1; r = (RayType)figure2; }
                points = IntersectCircleLine(a.CenterPoint, a.Measure, r.StartPoint, r.Point);
                if (points.Count == 1)
                {
                    PointType p = (PointType)points[0];
                    if (!IntersectPointArc(p, a) || !IntersectPointRayType(p, r))
                    {
                        points.Remove(p);
                    }
                }
                return new SequenceType(points, "");
            }


            throw new TypeCheckerError(0, 0, "No es posible calcular la intersección entre " + Figure1 + " y " + Figure2);
        }

        bool IntersectPointSegmentType(PointType p, SegmentType s)
        {
            double dx = s.EndPoint.X - s.StartPoint.X;
            double dy = s.EndPoint.Y - s.StartPoint.Y;
            // Verifica que el punto pertenezca a la SegmentTypeo
            double intercept = ((p.X - s.StartPoint.X) * dx + (p.Y - s.StartPoint.Y) * dy) / (Math.Pow(dx, 2) + Math.Pow(dy, 2));
            if (intercept < 0 || intercept > 1)
            {
                return false;
            }
            else return true;
        }

        bool IntersectPointRayType(PointType p, RayType r)
        {
            double dx = r.Point.X - r.StartPoint.X;
            double dy = r.Point.Y - r.StartPoint.Y;
            double intercept = ((p.X - r.StartPoint.X) * dx + (p.Y - r.StartPoint.Y) * dy) / (Math.Pow(dx, 2) + Math.Pow(dy, 2));
            if (intercept >= 0)
            {
                return true;
            }
            else return false;
        }

        bool IntersectPointArc(PointType p, ArcType a1)
        {
            double inicialAng1 = Math.Atan2(a1.StartPoint.Y - a1.CenterPoint.Y, a1.StartPoint.X - a1.CenterPoint.X);
            double EndPointAng1 = Math.Atan2(a1.EndPoint.Y - a1.CenterPoint.Y, a1.EndPoint.X - a1.CenterPoint.X);
            double angulo = Math.Atan2(p.Y - a1.CenterPoint.Y, p.X - a1.CenterPoint.X);
            if (angulo >= inicialAng1 && angulo <= EndPointAng1)
            {
                return true;
            }
            else return false;
        }

        List<Type> IntersectCircleLine(PointType CenterPoint, Measure CircleRadius, PointType Point1, PointType Point2)
        {
            List<Type> points = new();
            // m es la pEndPointiente de la recta 
            double m = (Point2.Y - Point1.Y) / (Point2.X - Point1.X);
            // ecuación de la recta 
            double b = Point1.Y - m * Point1.X;
            // coordenada x del centro de la circunferencia
            double h = CenterPoint.X;
            // coordenada y del centro de la circunferencia
            double k = CenterPoint.Y;
            // radio de la circunferencia
            double r = CircleRadius.Distance;
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
                PointType intersect = new("")
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
                PointType intersect1 = new("")
                {
                    X = x1,
                    Y = y1
                };
                PointType intersect2 = new("")
                {
                    X = x2,
                    Y = y2
                };
                points.Add(intersect1);
                points.Add(intersect2);
            }
            return points;
        }

        List<Type> IntersectCircles(PointType CenterPoint1, Measure CircleRadius1, PointType CenterPoint2, Measure CircleRadius2)
        {
            List<Type> points = new();
            //distancia entre los centros de las circunferencias
            double d = Math.Sqrt(Math.Pow(CenterPoint2.X - CenterPoint1.X, 2) + Math.Pow(CenterPoint2.Y - CenterPoint1.Y, 2));
            //si la distancia es igual a la suma de los radios entonces son tangentes
            if (d == CircleRadius1.Distance + CircleRadius2.Distance)
            {
                double x = (CircleRadius1.Distance * CenterPoint2.X - CircleRadius2.Distance * CenterPoint1.X) / (CircleRadius1.Distance - CircleRadius2.Distance);
                double y = (CircleRadius1.Distance * CenterPoint2.Y - CircleRadius2.Distance * CenterPoint1.Y) / (CircleRadius1.Distance - CircleRadius2.Distance);
                PointType p = new("")
                {
                    X = x,
                    Y = y
                };
                points.Add(p);
            }
            //en el primer caso uno de los círculos estaría contenido dentro del otro y en el otro no se intersectan
            else if (d > Math.Abs(CircleRadius1.Distance - CircleRadius2.Distance) && d < CircleRadius1.Distance + CircleRadius2.Distance)
            {
                double a = (Math.Pow(CircleRadius1.Distance, 2) - Math.Pow(CircleRadius2.Distance, 2) + Math.Pow(d, 2)) / (2 * d);
                double h = Math.Sqrt(Math.Pow(CircleRadius1.Distance, 2) - Math.Pow(a, 2));

                double x2 = CenterPoint1.X + a * (CenterPoint2.X - CenterPoint1.X) / d;
                double y2 = CenterPoint1.Y + a * (CenterPoint2.Y - CenterPoint1.Y) / d;

                PointType Point1 = new("")
                {
                    X = x2 + h * (CenterPoint2.Y - CenterPoint1.Y) / d,
                    Y = y2 - h * (CenterPoint2.X - CenterPoint1.X) / d
                };
                PointType Point2 = new("")
                {
                    X = x2 - h * (CenterPoint2.Y - CenterPoint1.Y) / d,
                    Y = y2 + h * (CenterPoint2.X - CenterPoint1.X) / d
                };
                points.Add(Point1);
                points.Add(Point2);
            }
            return points;
        }

        List<Type> IntersectLines(PointType firstlinePoint1, PointType firstlinePoint2, PointType secondtlinePoint1, PointType secondtlinePoint2)
        {
            List<Type> points = new();
            //Hallar pEndPointiente de la primera linea
            double m1 = (firstlinePoint1.Y - firstlinePoint2.Y) / (firstlinePoint1.X - firstlinePoint2.X);
            //Hallar punto medio de la primera linea
            //Para coordenadas X
            double xMiddle1 = (firstlinePoint1.X + firstlinePoint2.X) / 2;
            //Para coordenadas Y
            double yMiddle1 = (firstlinePoint1.Y + firstlinePoint2.Y) / 2;

            //Hallar pEndPointiente de la segunda linea
            double m2 = (secondtlinePoint1.Y - secondtlinePoint2.Y) / (secondtlinePoint1.X - secondtlinePoint2.X);
            //Hallar punto medio de la primera linea
            //Para coordenadas X
            double xMiddle2 = (secondtlinePoint1.X + secondtlinePoint2.X) / 2;
            //Para coordenadas Y
            double yMiddle2 = (secondtlinePoint1.Y + secondtlinePoint2.Y) / 2;
            if (m1 != m2)
            {
                // Coordenada x del punto de intersección
                double x = (yMiddle2 - yMiddle1 + m1 * xMiddle1 - m2 * xMiddle1) / (m1 - m2);
                // Coordenada y del punto de intersección
                double y = yMiddle1 + m1 * (x - xMiddle1);
                PointType intersect = new("")
                {
                    X = x,
                    Y = y
                };
                points.Add(intersect);
            }
            return points;
        }
    }

    public class LetInExpressionSyntax : ExpressionSyntax, ICheckType
    {
        public override SyntaxKind Type => SyntaxKind.LetInExpression;
        public List<SyntaxShapes> LetExpression { get; private set; }
        public ExpressionSyntax InExpression { get; private set; }
        public LetInExpressionSyntax(List<SyntaxShapes> letExpression, ExpressionSyntax inExpression)
        {
            LetExpression= letExpression;
            InExpression = inExpression;
        }

        public Type Check(Scope scope)
        {
            scope.SetScope();
            foreach (var statement in LetExpression)
            {
                Evaluator.EvaluateStatementExpressionSyntax(statement, scope, new List<IDrawable>());
            }
            Type ans = ((ICheckType)InExpression).Check(scope);
            scope.DeleteScope();
            return ans;
        }
    }


    public class LineExpressionSyntax : ExpressionSyntax, ICheckType
    {
        public override SyntaxKind Type => SyntaxKind.LineToken;
        public ExpressionSyntax StartPoint { get; private set; }
        public ExpressionSyntax EndPoint { get; private set; }
        public LineExpressionSyntax(ExpressionSyntax startPoint, ExpressionSyntax endPoint)
        {
            StartPoint = startPoint;
            EndPoint = endPoint;
        }
        
        public Type Check(Scope scope)
        {
            Type startPoint = ((ICheckType)StartPoint).Check(scope);
            Type endPoint = ((ICheckType)EndPoint).Check(scope);
            if (startPoint.SyntaxType != SyntaxType.Point) throw new TypeCheckerError(0, 0, "Unexpected Token : " + startPoint.SyntaxType + " PointToken Expected");
            else if (endPoint.SyntaxType != SyntaxType.Point) throw new TypeCheckerError(0, 0, "Unexpected Token " + endPoint.SyntaxType + " PointToken Expected");
            else return new Line((PointType)startPoint, (PointType)endPoint,  "");
        }
    }
    public class NumberExpressionSyntax : ExpressionSyntax, ICheckType
    {
        public override SyntaxKind Type => SyntaxKind.NumberToken;
        public SyntaxToken NumberToken { get; private set; }
        public NumberExpressionSyntax(SyntaxToken number)
        {
            NumberToken = number;
        }

        public Type Check(Scope scope)
        {
            return new Number(double.Parse(NumberToken.Lexeme!));
        }
    }
    public class StringExpressionSyntax : ExpressionSyntax, ICheckType
    {
        public override SyntaxKind Type => SyntaxKind.StringToken;
        public SyntaxToken StringToken { get; private set; }
        public StringExpressionSyntax(SyntaxToken string_)
        {
            StringToken = string_;
        }

        public Type Check(Scope scope)
        {
            return new String(StringToken.Lexeme!);
        }
    }
    public class VariableExpressionSyntax : ExpressionSyntax, ICheckType
    {
        public SyntaxToken Name { get; private set; }
        public override SyntaxKind Type => SyntaxKind.VariableToken;
        
        public VariableExpressionSyntax(SyntaxToken name)
        {
            Name = name;
        }
        public Type Check(Scope scope)
        {
            return scope.GetTypes(Name.Lexeme!);
        }
    }
    public class PIExpressionSyntax : ExpressionSyntax, ICheckType
    {
        public string B { get; private set; }

        public override SyntaxKind Type => SyntaxKind.PIToken;

        public PIExpressionSyntax(string value)
        {
            B = value;
        }

        public Type Check(Scope scope)
        {
            return new Number(Math.PI);
        }
    }
    public class EExpression : ExpressionSyntax, ICheckType
    {
        public string B { get; private set; }
        public override SyntaxKind Type => SyntaxKind.EToken;
        public EExpression(string value)
        {
            B = value;
        }
        public Type Check(Scope scope)
        {
            return new Number(Math.E);
        }
    }


    public class LogExpression : ExpressionSyntax, ICheckType
    {
        public SyntaxToken Log { get; private set; }
        public ExpressionSyntax Base { get; private set; }
        public ExpressionSyntax Value { get; private set; }
    
        public override SyntaxKind Type => SyntaxKind.LogToken;

        public LogExpression(SyntaxToken logExpression, ExpressionSyntax value, ExpressionSyntax baseNumber)
        {
            Log = logExpression;
            Value = value;
            Base = baseNumber;
        }

        public Type Check(Scope scope)
        {
            Type value = ((ICheckType)Value).Check(scope);
            Type base_ = ((ICheckType)Base).Check(scope);
            if (value.SyntaxType != SyntaxType.Number) throw new TypeCheckerError(0, 0, "Unexpected Token " + value.SyntaxType + "PointToken Expected");
            if (base_.SyntaxType != SyntaxType.Number) throw new TypeCheckerError(0, 0, "Unexpected Token " + base_.SyntaxType + "PointToken Expected");
            else return new Number(Math.Log(((Number)value).Value, ((Number)base_).Value));
        }
    }

    public class MeasureExpression : ExpressionSyntax, ICheckType
    {
        public override SyntaxKind Type => SyntaxKind.MeasureToken;
        public ExpressionSyntax StartPoint { get; private set; }
        public ExpressionSyntax EndPoint { get; private set; }

        public MeasureExpression(ExpressionSyntax startPoint, ExpressionSyntax endPoint)
        {
            StartPoint = startPoint;
            EndPoint = endPoint;
        }

        public Type Check(Scope scope)
        {
            Type startPoint = ((ICheckType)StartPoint).Check(scope);
            Type endPoint = ((ICheckType)EndPoint).Check(scope);
            if (startPoint.SyntaxType != SyntaxType.Point) throw new TypeCheckerError(0, 0, "Unexpected Tokek: " + startPoint.SyntaxType + "PointToken Expected");
            else if (endPoint.SyntaxType != SyntaxType.Point) throw new TypeCheckerError(0, 0, "Unexpected Tokek: " + endPoint.SyntaxType + "PointToken Expected");
            else return new Measure((PointType)startPoint, (PointType)endPoint, "");
        }
    }


    public class RayExpression : ExpressionSyntax, ICheckType
    {
        public override SyntaxKind Type => SyntaxKind.RayToken;
        public ExpressionSyntax StartPoint { get; private set; }
        public ExpressionSyntax EndPoint { get; private set; }
        public RayExpression(ExpressionSyntax startPoint, ExpressionSyntax endPoint)
        {
            StartPoint = startPoint;
            EndPoint = endPoint;
        }

        public Type Check(Scope scope)
        {
            Type startPoint = ((ICheckType)StartPoint).Check(scope);
            Type endPoint = ((ICheckType)EndPoint).Check(scope);
            if (startPoint.SyntaxType != SyntaxType.Point) throw new TypeCheckerError(0, 0, "Unexpected Token:  " + startPoint.SyntaxType + "PointToken Expected");
            else if (endPoint.SyntaxType != SyntaxType.Point) throw new TypeCheckerError(0, 0, "Unexpected Token:  " + endPoint.SyntaxType + "PointToken Expected");
            else return new RayType((PointType)startPoint, (PointType)endPoint, "");
        }
    }

    public class SegmentExpression : ExpressionSyntax, ICheckType
    {
        public override SyntaxKind Type => SyntaxKind.SegmentToken;
        public ExpressionSyntax StartPoint { get; private set; }
        public ExpressionSyntax EndPoint { get; private set; }
        public SegmentExpression(ExpressionSyntax startPoint, ExpressionSyntax endPoint)
        {
            StartPoint = startPoint;
            EndPoint = endPoint;
        }

        public Type Check(Scope scope)
        {
            Type startPoint = ((ICheckType)StartPoint).Check(scope);
            Type endPoint = ((ICheckType)EndPoint).Check(scope);
            if (startPoint.SyntaxType != SyntaxType.Point) throw new TypeCheckerError(0, 0, "Unexpected Token: " + startPoint.SyntaxType + "PointToken Expected");
            else if (endPoint.SyntaxType != SyntaxType.Point) throw new TypeCheckerError(0, 0, "Unexpected Token" + endPoint.SyntaxType + "PointToken Expected");
            else return new SegmentType((PointType)startPoint, (PointType)endPoint, "");
        }
    }


    public class SequenceTypeExpression : ExpressionSyntax, ICheckType
    {
        public override SyntaxKind Type => SyntaxKind.SequenceToken;
        public List<ExpressionSyntax>? SequenceElements { get; private set; }
        public SyntaxToken? Start { get; private set; }
        public SyntaxToken? End { get; private set; }
        public ConcatenatedSequenceTypeExpression? SequenceType { get; private set; }
        public InfiniteSequenceTypeExpression? InfiniteSequenceType { get; private set; }
        public Empty? Empty { get; private set; }

        public SequenceTypeExpression(List<ExpressionSyntax> sequenceElements)
        {
            SequenceElements = sequenceElements;
        }
        public SequenceTypeExpression(SyntaxToken start, SyntaxToken end)
        {
            Start = start;
            End = end;
        }
        public SequenceTypeExpression(SyntaxToken start)
        {
            Start = start;
        }
        public SequenceTypeExpression(ConcatenatedSequenceTypeExpression sequenceType)
        {
            SequenceType = sequenceType;
        }
        public SequenceTypeExpression(Empty empty)
        {
            Empty = empty;
        }

        public Type Check(Scope scope)
        {
            List<Type> SequenceElements = new();
            if (SequenceElements != null)
            {
                foreach (var element in SequenceElements)
                {
                    SequenceElements.Add(((ICheckType)element).Check(scope));
                }
                if (SequenceElements.All(x => x.SyntaxType == SequenceElements[0].SyntaxType)) return new SequenceType(SequenceElements, "");
                else throw new TypeCheckerError(0, 0, "Sequence Elements aren't the same SyntaxType");
            }
            if (Start != null && End != null)
            {
                
                if ((double)End.Value! - (double)Start.Value! > 1000) return (SequenceType)new InfiniteSequenceTypeExpression(Start).Check(scope);
                else
                {
                    double start = (double)Start.Value!;
                    double end = (double)End.Value!;
                    var SequenceType = Enumerable.Range(Convert.ToInt32(start), Convert.ToInt32(end) - Convert.ToInt32(start) + 1).Select(x => new Number(x));
                    List<Type> seq = new();
                    foreach (var item in SequenceType)
                    {
                        seq.Add(item);
                    }
                    return new SequenceType(seq, "");
                }
            }
            if (Start != null && End == null)
            {
                
                return (SequenceType)new InfiniteSequenceTypeExpression(Start).Check(scope);
            }
            if (SequenceType != null)
            {
                
                return (SequenceType)((ICheckType)SequenceType).Check(scope);
            }
            if (Empty != null)
            {
                return new EmptyType();
            }
            else throw new TypeCheckerError(0, 0, "Invalid Sequence");
        }
    }

    public class ConcatenatedSequenceTypeExpression : ExpressionSyntax, ICheckType
    {
        public override SyntaxKind Type => SyntaxKind.SequenceToken;
        public SequenceTypeExpression? FirstSequenceType { get; private set; }
        public UndefinedExpressionSyntax? Undefined { get; private set; }
        public SequenceTypeExpression? SecondSequenceType { get; private set; }

        public ConcatenatedSequenceTypeExpression(SequenceTypeExpression first, SequenceTypeExpression second)
        {
            FirstSequenceType = first;
            SecondSequenceType = second;
        }

        public ConcatenatedSequenceTypeExpression(SequenceTypeExpression first, UndefinedExpressionSyntax undefined)
        {
            FirstSequenceType = first;
            Undefined = undefined;
        }

        public ConcatenatedSequenceTypeExpression(UndefinedExpressionSyntax undefined, SequenceTypeExpression second)
        {
            SecondSequenceType = second;
            Undefined = undefined;
        }

        public Type Check(Scope scope)
        {
            if (FirstSequenceType != null && SecondSequenceType != null)
            {
                SequenceType first = (SequenceType)((ICheckType)FirstSequenceType!).Check(scope);
                SequenceType second = (SequenceType)((ICheckType)SecondSequenceType!).Check(scope);
                List<Type> SequenceType = new(first.SequenceElements.Concat(second.SequenceElements));
                if (SequenceType.All(x => x.SyntaxType == SequenceType[0].SyntaxType)) return new SequenceType(SequenceType, "");
                else throw new TypeCheckerError(0, 0, "Gotta b the same SyntaxType to Concatenate()");
            }
            if (FirstSequenceType != null && Undefined != null)
            {
                SequenceType first = (SequenceType)((ICheckType)FirstSequenceType!).Check(scope);
                Undefined undefined = (Undefined)((ICheckType)Undefined!).Check(scope);
                first.SequenceElements.Add(undefined);
                return new SequenceType(first.SequenceElements, "");
            }
            if (SecondSequenceType != null && Undefined != null)
            {
                return new Undefined();
            }
            else throw new TypeCheckerError(0, 0, "Invalid Sequence");
        }
    }

    public class InfiniteSequenceTypeExpression : ExpressionSyntax, ICheckType
    {
        public override SyntaxKind Type => SyntaxKind.SequenceToken;
        public SyntaxToken Start { get; private set; }
        public InfiniteSequenceTypeExpression(SyntaxToken start)
        {
            Start = start;
        }

        public Type Check(Scope scope)
        {
            double start = (double)Start.Value!;
            List<Type> SequenceType = (List<Type>)Enumerable.Range((int)start, (int)start + 1000).Select(x => new Number(x));
            return new SequenceType(SequenceType, "");
        }
    }


    public class BetweenParenExpressionSyntax : ExpressionSyntax, ICheckType
    {
        public override SyntaxKind Type => SyntaxKind.BetweenParenthesisExpression;
        public ExpressionSyntax ExpressionSyntax { get; private set; }

        public BetweenParenExpressionSyntax(ExpressionSyntax exp)
        {
            ExpressionSyntax = exp;
        }

        public Type Check(Scope scope)
        {
            return ((ICheckType)ExpressionSyntax).Check(scope);
        }
    }
    public class Randoms : ExpressionSyntax
    {
        public override SyntaxKind Type => SyntaxKind.RandomsToken;
        public SequenceType RandomSequence { get; private set; }

        public Randoms()
        {
            List<Type> randoms = GenerateRandom();
            RandomSequence = new SequenceType(randoms, "");
        }

        private List<Type> GenerateRandom()
        {
            var randoms = new List<Type>();
            for (int i = 0; i < new Random().Next(0, 100); i++)
            {
                randoms.Add(new Number(new Random().NextDouble()));
            }
            return randoms;
        }

    }
    public class RandomPointExpressionSyntax : ExpressionSyntax, ICheckType
    {
        public override SyntaxKind Type => SyntaxKind.PointsToken;
        public ExpressionSyntax Figure { get; private set; }

        public RandomPointExpressionSyntax(ExpressionSyntax figure)
        {
            Figure = figure;
        }

        public Type Check(Scope scope)
        {
            Type fig = ((ICheckType)Figure).Check(scope);
            List<Type> points = CreateRandomPointExpressionSyntax(fig);
            return new SequenceType(points, "");
        }
        private List<Type> CreateRandomPointExpressionSyntax(Type figure)
        {
            List<Type> randoms = new();
            Random random = new();
            for (int i = 0; i <  100000; i++)
            {
                PointType point = new("");
                point.X = random.Next(0,1000);
                point.Y = random.Next(0,1000);
                bool isInFigure = Belongs(point, figure);
                if (isInFigure) randoms.Add(new PointType(""));
                else continue;
            }
            return randoms;
        }

        private bool Belongs(PointType p, Type figure)
        {
            if (figure is Line l)
            {
                if (p.Y - l.Point1.Y == (l.Point2.Y - l.Point1.Y) / (l.Point2.X - l.Point1.X) * (p.X - l.Point1.X)) return true;
                else return false;
            }
            else if (figure is RayType r)
            {
                double intercept = ((p.X - r.StartPoint.X) * r.Point.X - r.StartPoint.X + (p.Y - r.StartPoint.Y) * r.Point.Y - r.StartPoint.Y) / (Math.Pow(r.Point.X - r.StartPoint.X, 2) + Math.Pow(r.Point.Y - r.StartPoint.Y, 2));
                if (intercept >= 0) return true;
                else return false;
            }
            else if (figure is SegmentType s)
            {
                double intercept = ((p.X - s.StartPoint.X) * s.EndPoint.X - s.StartPoint.X + (p.Y - s.StartPoint.Y) * s.EndPoint.Y - s.StartPoint.Y) / (Math.Pow(s.EndPoint.X - s.StartPoint.X, 2) + Math.Pow(s.EndPoint.Y - s.StartPoint.Y, 2));
                if (intercept < 0 || intercept > 1) return true;
                else return false;
            }
            else if (figure is CircleType c)
            {
                if (Math.Sqrt(Math.Pow(c.CenterPoint.X - p.X, 2) + Math.Pow(c.CenterPoint.Y - p.Y, 2)) == c.CircleRadius.Distance) return true;
                else return false;
            }
            else if (figure is ArcType a)
            {
                double distance = Math.Abs(Math.Sqrt(Math.Pow(p.X - a.CenterPoint.X, 2) + Math.Pow(p.Y - a.CenterPoint.Y, 2)));
                if (distance == Math.Abs(a.Measure.Distance))
                {
                    double arcPointsDistance = Math.Abs(Math.Sqrt(Math.Pow(a.StartPoint.X - a.EndPoint.X, 2) + Math.Pow(a.StartPoint.Y - a.EndPoint.Y, 2)));
                    double starPointDistance = Math.Abs(Math.Sqrt(Math.Pow(p.X - a.StartPoint.X, 2) + Math.Pow(p.Y - a.StartPoint.Y, 2)));
                    double EndPointPointDistance = Math.Abs(Math.Sqrt(Math.Pow(p.X - a.EndPoint.X, 2) + Math.Pow(p.Y - a.EndPoint.Y, 2)));
                    if (arcPointsDistance == (starPointDistance + EndPointPointDistance))
                    {
                        return true;
                    }
                    return false;
                }
                else return false;
            }
            else throw new TypeCheckerError(0, 0, "Invalid(Can't get the Overlap)");
        }
    }
    public class SamplesExpressionSyntax : ExpressionSyntax
    {
        public override SyntaxKind Type => SyntaxKind.SamplesToken;
        public SequenceType Sequence { get; private set; }

        public SamplesExpressionSyntax()
        {
            List<Type> points = GenerateSamples();
            Sequence = new SequenceType(points, "");
        }
        private List<Type> GenerateSamples()
        {
            var randoms = new List<Type>();
            for (int i=0; i<new Random().Next(0, 100); i++)
            {
                randoms.Add(new PointType(""));
            }
            return randoms;
        }
    }
    public class Count : ExpressionSyntax, ICheckType
    {
        public override SyntaxKind Type => SyntaxKind.CountToken;
        public ExpressionSyntax Sequence { get; private set; }
        public Count(ExpressionSyntax sequence)
        {
            Sequence = sequence;
        }

        public Type Check(Scope scope)
        {
            Type sequence = ((ICheckType)Sequence).Check(scope);
            if (sequence is SequenceType seq)
            {
                if (seq.SequenceElements.Count != 0 || seq.SequenceElements.Count <= 100) return new Number(seq.SequenceElements.Count);
                else return new Undefined();
            }
            else throw new TypeCheckerError(0, 0, "Sequence Expected when doing Count()");
        }
    }


    public class UnaryExpressionSyntax : ExpressionSyntax, ICheckType
    {
        public override SyntaxKind Type => SyntaxKind.UnaryExpression;
        public SyntaxToken OperatorToken { get; private set; }
        public ExpressionSyntax OperandToken { get; private set; }


        public UnaryExpressionSyntax(SyntaxToken operatorToken, ExpressionSyntax operandToken)
        {
            OperatorToken = operatorToken;
            OperandToken = operandToken;
        }

        public Type Check(Scope scope)
        {
            Type Operand = ((ICheckType)OperandToken).Check(scope);
            switch (Operand.SyntaxType)
            {
                case SyntaxType.Number:
                    if (OperatorToken.Kind == SyntaxKind.SqrtToken) return new Number(Math.Sqrt(((Number)Operand).Value));
                    if (OperatorToken.Kind == SyntaxKind.SinToken) return new Number(Math.Sin(((Number)Operand).Value));
                    if (OperatorToken.Kind == SyntaxKind.CosToken) return new Number(Math.Cos(((Number)Operand).Value));
                    if (OperatorToken.Kind == SyntaxKind.ExpoToken) return new Number(Math.Pow(Math.E, ((Number)Operand).Value));
                    if (OperatorToken.Kind == SyntaxKind.PlusToken) return new Number(((Number)Operand).Value);
                    if (OperatorToken.Kind == SyntaxKind.MinusToken) return new Number(-((Number)Operand).Value);
                    
                    if (OperatorToken.Kind == SyntaxKind.NotToken)
                    {
                        if (((Number)Operand).Value == 0) return new Number(1);
                        else return new Number(0);
                    }
                    else throw new TypeCheckerError(0, 0, "Can't Operate");
                default:
                    throw new TypeCheckerError(0, 0, "Can't Operate");
            }
        }
    }

    public class UndefinedExpressionSyntax : ExpressionSyntax, ICheckType
    {
        public override SyntaxKind Type => SyntaxKind.UndefinedToken;

        public Type Check(Scope scope)
        {
            return new Undefined();
        }
    }



}
