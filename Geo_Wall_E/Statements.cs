namespace Geo_Wall_E
{
    public class ArcStmt : Stmt
    {
        public override TypesOfToken Type => TypesOfToken.CircleToken;
        public Token Name { get; private set; }
        public bool Sequence { get; private set; }
        public ArcStmt(Token name, bool sequence)
        {
            Name = name;
            Sequence = sequence;
        }
        public static Sequence ArcSequence()
        {
            Random random = new();
            List<Type> arcs = new();
            int count = random.Next(100, 999);
            for (int i = 0; i < count; i++)
            {
                Point p = new("")
                {
                    X = random.Next(0, 1000),
                    Y = random.Next(0, 1000)
                };
                Point P1 = new("")
                {
                    X = random.Next(0, 1000),
                    Y = random.Next(0, 1000)
                };
                Point P2 = new("")
                {
                    X = random.Next(0, 1000),
                    Y = random.Next(0, 1000)
                };
                Point Pm1 = new("")
                {
                    X = random.Next(0, 1000),
                    Y = random.Next(0, 1000)
                };
                Point Pm2 = new("")
                {
                    X = random.Next(0, 1000),
                    Y = random.Next(0, 1000)
                };
                Measure m = new(Pm1, Pm2, "");
                Arc c = new(p,P1,P2, m, "");
                arcs.Add(c);
            }
            return new Sequence(arcs, "");
        }
    }
    public class SegmentStmt : Stmt
    {
        public override TypesOfToken Type => TypesOfToken.SegmentToken;
        public Token Name { get; private set; }
        public bool Sequence { get; private set; }
        public SegmentStmt(Token name, bool sequence)
        {
            Name = name;
            Sequence = sequence;
        }
        public static Sequence SegmentSequence()
        {
            Random random = new ();
            List<Type> segments = new ();
            int count = random.Next(100, 999);
            for (int i = 0; i < count; i++)
            {
                Point P1 = new("")
                {
                    X = random.Next(0, 1000),
                    Y = random.Next(0, 1000)
                };
                Point P2 = new("")
                {
                    X = random.Next(0, 1000),
                    Y = random.Next(0, 1000)
                };
                segments.Add(new Segment(P1, P2,""));
            }
            return new Sequence(segments,"");
        }
    }

     public class AssignationStmt : Stmt
    {
        public override TypesOfToken Type => TypesOfToken.AssignationStatement;
        public List<Token>? Name { get; private set; }
        public Token? Name_ { get; private set; }
        public Expressions Assignment { get; private set; }
        public AssignationStmt(List<Token> name, Expressions assignment)
        {
            Name = name;
            Assignment = assignment;
        }
        public AssignationStmt(Token name, Expressions assignment)
        {
            Name_ = name;
            Assignment = assignment;
        }
    }

    public class CircleStmt : Stmt
    {
        public override TypesOfToken Type => TypesOfToken.CircleToken;
        public Token Name { get; private set; }
        public bool Sequence { get; private set; }
        public CircleStmt(Token name, bool sequence)
        {
            Name = name;
            Sequence = sequence;
        }
        public static Sequence CircleSequence()
        {
            Random random = new();
            List<Type> circles = new();
            int count = random.Next(100, 999);
            for (int i = 0; i < count; i++)
            {
                Point p = new("")
                {
                    X = random.Next(0, 1000),
                    Y = random.Next(0, 1000)
                };
                Point P1 = new("")
                {
                    X = random.Next(0, 1000),
                    Y = random.Next(0, 1000)
                };
                Point P2 = new("")
                {
                    X = random.Next(0, 1000),
                    Y = random.Next(0, 1000)
                };
                Measure m = new(P1, P2, "");
                Circle c = new(p, m, "");
                circles.Add(c);
            }
            return new Sequence(circles, "");
        }
    }

    public class DrawStmt : Stmt
    {
        public override TypesOfToken Type => TypesOfToken.DrawToken;
        public string Name { get; private set; }
        public Node? Expression { get; private set; }
        public List<VariableExpression>? ExpressionSequence { get; private set; }
        public Color Color { get; private set;}
        public DrawStmt(string name, Node exp, Color color)
        {
            Name = name;
            Expression = exp;
            Color = color;
        }
        public DrawStmt(string name, List<VariableExpression> expSequence, Color color)
        {
            Name = name;
            ExpressionSequence = expSequence;
            Color = color;
        }
    }

    public class FunctionStmt : Stmt
    {
        public override TypesOfToken Type => TypesOfToken.FunctionDeclaration;
        public Token Name { get; private set; }
        public List<Expressions> Arguments { get; private set; }
        public Expressions Body { get; private set; }
        public FunctionStmt(Token name, List<Expressions> arguments, Expressions body)
        {
            Name = name;
            Arguments = arguments;
            Body = body;
        }
    }

    
    public class PointStmt : Stmt
    {
        public override TypesOfToken Type => TypesOfToken.PointToken;
        public Token Name { get; private set; }
        public bool Sequence { get; private set; }
        public PointStmt(Token name, bool sequence)
        {
            Name = name;
            Sequence = sequence;
        }

        public static Sequence PointSequence()
        {
            Random random = new();
            List<Type> points = new();
            int count = random.Next(100, 999);
            for (int i = 0; i < count; i++)
            {
                Point p = new("")
                {
                    X = random.Next(0, 1000),
                    Y = random.Next(0, 1000)
                };
                points.Add(p);
            }
            return new Sequence(points, "");
        }
    }

    public class RayStmt : Stmt
    {
        public override TypesOfToken Type => TypesOfToken.RayToken;
        public Token Name { get; private set; }
        public bool Sequence { get; private set; }
        public RayStmt(Token name, bool sequence)
        {
            Name = name;
            Sequence = sequence;
        }
        public static Sequence RaySequence()
        {
            Random random = new ();
            List<Type> ray = new ();
            int count = random.Next(100, 999);
            for (int i = 0; i < count; i++)
            {
                Point P1 = new("")
                {
                    X = random.Next(0, 1000),
                    Y = random.Next(0, 1000)
                };
                Point P2 = new("")
                {
                    X = random.Next(0, 1000),
                    Y = random.Next(0, 1000)
                };
                ray.Add(new Ray(P1, P2,""));
            }
            return new Sequence(ray,"");
        }
    }


    public class LineStmt : Stmt
    {
        public override TypesOfToken Type => TypesOfToken.LineToken;
        public Token Name { get; private set; }
        public bool Sequence { get; private set; }
        public LineStmt(Token name, bool sequence)
        {
            Name = name;
            Sequence = sequence;
        }

        public static Sequence LineSequence()
        {
            Random random = new ();
            List<Type> lines = new ();
            int count = random.Next(100, 999);
            for (int i = 0; i < count; i++)
            {
                Point P1 = new("")
                {
                    X = random.Next(0, 1000),
                    Y = random.Next(0, 1000)
                };
                Point P2 = new("")
                {
                    X = random.Next(0, 1000),
                    Y = random.Next(0, 1000)
                };
                lines.Add(new Line(P1, P2,""));
            }
            return new Sequence(lines,"");
        }

    }

}
