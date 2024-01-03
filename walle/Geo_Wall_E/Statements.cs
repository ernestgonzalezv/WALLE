using System.Drawing;

namespace Geo_Wall_E
{
    public class ArcStatement : SyntaxShapes
    {
        public SyntaxToken Name { get; private set; }
        public bool InSequence { get; private set; }
        public override SyntaxKind Type => SyntaxKind.ArcToken;
        public ArcStatement(SyntaxToken name, bool inSequence)
        {
            Name = name;
            InSequence = inSequence;
        }
        public static SequenceType ArcSequence()
        {
            var arcs = new List<Type>();
            var RandomInt = new Random();
            var count = RandomInt.Next(100, 999);
            for (int i = 0; i < count; i++)
            {
                var A= new ArcType(new PointType(" ")
                {
                    X = RandomInt.Next(0,1000),Y = RandomInt.Next(0,1000)
                },new PointType(""){
                    X= RandomInt.Next(0,1000), Y = RandomInt.Next(0,1000)
                },new PointType(""){
                    X=RandomInt.Next(0,1000), Y= RandomInt.Next(0,1000)
                }, new Measure(new PointType(""){
                    X=RandomInt.Next(0,1000), Y = RandomInt.Next(0,1000)
                }, new PointType(""){
                    X=RandomInt.Next(0,1000), Y = RandomInt.Next(0,1000)
                }, ""), "");
                arcs.Add(A);
            }
            return new SequenceType(arcs, "");
        }
    }

    public class AssignationStatement : SyntaxShapes
    {
        public override SyntaxKind Type => SyntaxKind.AssignationStatement;
        public SyntaxToken? Name_ { get; private set; }
        public List<SyntaxToken>? Name { get; private set; }
        public ExpressionSyntax Assignment { get; private set; }
        public AssignationStatement(List<SyntaxToken> name, ExpressionSyntax assignment)
        {
            Name = name;
            Assignment = assignment;
        }
        public AssignationStatement(SyntaxToken name_, ExpressionSyntax assignment)
        {
            Name_ = name_;
            Assignment = assignment;
        }
    }

    public class CircleStatement : SyntaxShapes
    {
        public override SyntaxKind Type => SyntaxKind.CircleToken;
        public SyntaxToken Name { get; private set; }
        public bool Sequence { get; private set; }
        public CircleStatement(SyntaxToken name, bool sequence)
        {
            Name = name;
            Sequence = sequence;
        }
        public static SequenceType CircleSequence()
        {
            var circles = new List<Type>();
            var RandomInt = new Random();
            for (int i=0;i<RandomInt.Next(100,999);i++)
            {
                CircleType A = new(new PointType(""){
                    X= RandomInt.Next(0,1000) , Y = RandomInt.Next(0,1000)
                }, new Measure(new PointType(""){
                    X=RandomInt.Next(0,1000), Y= RandomInt.Next(0,1000)
                }, new PointType(""){
                    X=RandomInt.Next(0,1000), Y=RandomInt.Next(0,1000)
                }, ""), "");
                circles.Add(A);
            }
            return new SequenceType(circles, "");
        }
    }

    public class DrawStatement : SyntaxShapes
    {
        public Color Color { get; private set;}
        public override SyntaxKind Type => SyntaxKind.DrawToken;
        public string Name { get; private set; }
        public SyntaxNode? ExpressionNode { get; private set; }
        public List<VariableExpressionSyntax>? ExpressionSequence { get; private set; }
       
        public DrawStatement(string name, SyntaxNode expressionNode, Color color)
        {
            Name = name;
            ExpressionNode = expressionNode;
            Color = color;
        }
        public DrawStatement(string name, List<VariableExpressionSyntax> expSequence, Color color)
        {
            Name = name;
            ExpressionSequence = expSequence;
            Color = color;
        }
    }

    public class FunctionStatement : SyntaxShapes
    {
        public override SyntaxKind Type => SyntaxKind.FunctionDeclaration;
        public SyntaxToken Name { get; private set; }
        public List<ExpressionSyntax> Args { get; private set; }
        public ExpressionSyntax FunctionBody { get; private set; }
        public FunctionStatement(SyntaxToken name, List<ExpressionSyntax> args, ExpressionSyntax functionBody)
        {
            Name = name;
            Args = args;
            FunctionBody = functionBody;
        }
    }

    public class LineStatement : SyntaxShapes
    {
        public override SyntaxKind Type => SyntaxKind.LineToken;
        public SyntaxToken Name { get; private set; }
        public bool InSequence { get; private set; }
        public LineStatement(SyntaxToken name, bool inSequence)
        {
            Name = name;
            InSequence = inSequence;
        }

        public static SequenceType LineSequence()
        {
            var linesst = new List<Type> ();
            var RandomInt = new Random();
            for (int i = 0; i < RandomInt.Next(100,999); i++)
            {
                linesst.Add(new Line(new PointType(""){
                    X=RandomInt.Next(0,1000), Y = RandomInt.Next(0,1000)
                }, new PointType(""){
                    X=RandomInt.Next(0,1000), Y= RandomInt.Next(0,1000)
                },""));
            }
            return new SequenceType(linesst,"");
        }

    }

    public class PointStatement : SyntaxShapes
    {
        public override SyntaxKind Type => SyntaxKind.PointsToken;
        public SyntaxToken Name { get; private set; }
        public bool InSequence { get; private set; }
        public PointStatement(SyntaxToken name, bool inSequence)
        {
            Name = name;
            InSequence = inSequence;
        }

        public static SequenceType PointSequence()
        {
            var RandomInt = new Random();
            var pointsst = new List<Type>();
            for (int i = 0; i < RandomInt.Next(100, 999); i++)
            {
                pointsst.Add(new PointType(""){
                    X=RandomInt.Next(0,1000), Y=RandomInt.Next(0,1000)
                });
            }
            return new SequenceType(pointsst, "");
        }
    }

    public class RayStatement : SyntaxShapes
    {
        public override SyntaxKind Type => SyntaxKind.RayToken;
        public bool InSequence { get; private set; }
        public SyntaxToken Name { get; private set; }
        public RayStatement(SyntaxToken name, bool inSequence)
        {
            Name = name;
            InSequence = inSequence;
        }
        public static SequenceType RaySequence()
        {
            var RandomInt = new Random();
            List<Type> rays = new List<Type> ();
            
            for (int i=0; i<RandomInt.Next(100, 999); i++)
            {
                PointType P1 = new("")
                {
                    X = RandomInt.Next(0, 1000),
                    Y = RandomInt.Next(0, 1000)
                };
                PointType P2 = new("")
                {
                    X = RandomInt.Next(0, 1000),
                    Y = RandomInt.Next(0, 1000)
                };
                rays.Add(new RayType(P1, P2,""));
            }
            return new SequenceType(rays,"");
        }
    }

    public class SegmentStatement : SyntaxShapes
    {
        public override SyntaxKind Type => SyntaxKind.SegmentToken;
        public bool InSequence { get; private set; }
        public SyntaxToken Name { get; private set; }
        public SegmentStatement(SyntaxToken name, bool inSequence)
        {
            Name = name;
            InSequence = inSequence;
        }
        public static SequenceType SegmentSequence()
        {
            var RandomInt = new Random ();
            var segmentsst = new List<Type>();
            for (int i=0;i<RandomInt.Next(100, 999);i++)
            {
                segmentsst.Add(new SegmentType(new PointType(""){
                    X=RandomInt.Next(0,1000), Y = RandomInt.Next(0,1000)
                }, new PointType(""){
                    X=RandomInt.Next(0,1000), Y = RandomInt.Next(0,1000)
                },""));
            }
            return new SequenceType(segmentsst,"");
        }
    }

}
