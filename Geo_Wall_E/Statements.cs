namespace Geo_Wall_E
{
    public class ArcStatement : Statement
    {
        public override SyntaxKind Type => SyntaxKind.CircleToken;
        public SyntaxToken Name { get; private set; }
        public bool Sequence { get; private set; }
        public ArcStatement(SyntaxToken name, bool sequence)
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
    public class SegmentStatement : Statement
    {
        public override SyntaxKind Type => SyntaxKind.SegmentToken;
        public SyntaxToken Name { get; private set; }
        public bool Sequence { get; private set; }
        public SegmentStatement(SyntaxToken name, bool sequence)
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

     public class AssignationStatement : Statement
    {
        public override SyntaxKind Type => SyntaxKind.AssignationStatement;
        public List<SyntaxToken>? Name { get; private set; }
        public SyntaxToken? Name_ { get; private set; }
        public ExpressionSyntax Assignment { get; private set; }
        public AssignationStatement(List<SyntaxToken> name, ExpressionSyntax assignment)
        {
            Name = name;
            Assignment = assignment;
        }
        public AssignationStatement(SyntaxToken name, ExpressionSyntax assignment)
        {
            Name_ = name;
            Assignment = assignment;
        }
    }

    public class CircleStatement : Statement
    {
        public override SyntaxKind Type => SyntaxKind.CircleToken;
        public SyntaxToken Name { get; private set; }
        public bool Sequence { get; private set; }
        public CircleStatement(SyntaxToken name, bool sequence)
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

    public class DrawStatement : Statement
    {
        public override SyntaxKind Type => SyntaxKind.DrawToken;
        public string Name { get; private set; }
        public SyntaxNode? Expression { get; private set; }
        public List<VariableExpression>? ExpressionSequence { get; private set; }
        public Color Color { get; private set;}
        public DrawStatement(string name, SyntaxNode exp, Color color)
        {
            Name = name;
            Expression = exp;
            Color = color;
        }
        public DrawStatement(string name, List<VariableExpression> expSequence, Color color)
        {
            Name = name;
            ExpressionSequence = expSequence;
            Color = color;
        }
    }

    public class FunctionStatement : Statement
    {
        public override SyntaxKind Type => SyntaxKind.FunctionDeclaration;
        public SyntaxToken Name { get; private set; }
        public List<ExpressionSyntax> Arguments { get; private set; }
        public ExpressionSyntax Body { get; private set; }
        public FunctionStatement(SyntaxToken name, List<ExpressionSyntax> arguments, ExpressionSyntax body)
        {
            Name = name;
            Arguments = arguments;
            Body = body;
        }
    }

    
    public class PointStatement : Statement
    {
        public override SyntaxKind Type => SyntaxKind.PointToken;
        public SyntaxToken Name { get; private set; }
        public bool Sequence { get; private set; }
        public PointStatement(SyntaxToken name, bool sequence)
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

    public class RayStatement : Statement
    {
        public override SyntaxKind Type => SyntaxKind.RayToken;
        public SyntaxToken Name { get; private set; }
        public bool Sequence { get; private set; }
        public RayStatement(SyntaxToken name, bool sequence)
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


    public class LineStatement : Statement
    {
        public override SyntaxKind Type => SyntaxKind.LineToken;
        public SyntaxToken Name { get; private set; }
        public bool Sequence { get; private set; }
        public LineStatement(SyntaxToken name, bool sequence)
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
