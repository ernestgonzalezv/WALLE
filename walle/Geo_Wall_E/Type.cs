using System.Reflection.Metadata.Ecma335;

namespace Geo_Wall_E
{
    public abstract class Type
    {
        public abstract SyntaxType SyntaxType { get; }
    }
    public class ArcType : Type, IDrawable
    {
        public Measure Measure { get; private set;  }
        public string Name { get; private set;  }
        Color IDrawable.DrawableColor => Color.BLACK;
        string IDrawable.DrawableStr => Name;
        public override SyntaxType SyntaxType => SyntaxType.Arc;
        public PointType CenterPoint { get; private set;  }
        public PointType StartPoint { get; private set;  }
        public PointType EndPoint { get; private set;  }
         Geo_Wall_E.Type IDrawable.DrawableType => new ArcType(CenterPoint,StartPoint,EndPoint,Measure,Name);

        public ArcType(PointType centerPoint, PointType startPoint, PointType endPoint, Measure measure, string name)
        {
            StartPoint = startPoint;
            EndPoint = endPoint;
            Measure = measure;
            Name = name;
            CenterPoint = centerPoint;
            
        }
       
    }

    public class CircleType : Type, IDrawable
    {
        public override SyntaxType SyntaxType => SyntaxType.Circle;
        public PointType CenterPoint { get; private set; }
        public Measure CircleRadius { get; private set; }
        Color IDrawable.DrawableColor => Color.BLACK;
        Type IDrawable.DrawableType => new CircleType(CenterPoint,CircleRadius,Name);
        public string Name { get; private set; }
        string IDrawable.DrawableStr => Name;
        public CircleType(PointType centerPoint, Measure circleRadius, string name)
        {
            CenterPoint = centerPoint;
            CircleRadius = circleRadius;
            Name = name;
        }

        
    }

    public class EmptyType : Type
    {
        public override SyntaxType SyntaxType => SyntaxType.Empty;
    }

    public class Function : Type
    {
        public override SyntaxType SyntaxType => SyntaxType.Function;
        public SyntaxToken Name {get; private set;}
        public List<ExpressionSyntax> Arguments {get; private set;}
        public ExpressionSyntax Body {get; private set;}
        public Function(SyntaxToken name, List<ExpressionSyntax> args, ExpressionSyntax body)
        {
            Name = name;
            Arguments = args;
            Body = body;
        }
    }

    public class Line : Type, IDrawable
    {
        Type IDrawable.DrawableType => new Line(Point1,Point2,"");
        Color IDrawable.DrawableColor => Color.BLACK;
        public override SyntaxType SyntaxType => SyntaxType.Line;
        public PointType Point1 { get; private set; }
        public PointType Point2 { get; private set; }
        public string Name { get; private set; }
        string IDrawable.DrawableStr => Name;

        public Line(PointType point1, PointType point2, string name)
        {
            Point1 = point1;
            Point2 = point2;
            Name = name;
        }

    }
    public class Number : Type
    {
        public override SyntaxType SyntaxType => SyntaxType.Number;

        public double Value { get; private set; }

        public Number(double value)
        {
            Value = value;
        }

    }

    public class String : Type
    {
        public override SyntaxType SyntaxType => SyntaxType.String;
        public  string Value { get; private set; }

        public String(string value)
        {
            Value = value;
        }
    }
    public class Boolean : Type
    {
        public override SyntaxType SyntaxType => SyntaxType.Boolean;
        public int Value { get; private set;}
        public Boolean(bool value)
        {
            if (!value) Value = 0;
            else Value = 1;
        }
    }


    public class Measure : Type
    {
        public string Name { get; private set;  }
        public double Distance { get; private set;  }
        public override SyntaxType SyntaxType => SyntaxType.Measure;
        public PointType Point1 { get; private set;  }
        public PointType Point2 { get; private set;  }
        
        public Measure(PointType point1, PointType point2, string name)
        {
            Point1 = point1;
            Point2 = point2;
            Name = name;
            Distance = Math.Sqrt(Math.Pow(Point2.X - Point1.X, 2) + Math.Pow(Point2.Y - Point1.Y, 2));
        }

        public Measure(double measure, string name)
        {
            Point1 = new PointType("");
            Point2 = new PointType("");
            Name = name;
            Distance = measure;
        }
        
    }

    public class PointType : Type, IDrawable
    {
        public override SyntaxType SyntaxType => SyntaxType.Point;
        public string Name { get; private set; }
        string IDrawable.DrawableStr => Name;
        public double X { get; set; }
        public double Y { get; set; }
        Type IDrawable.DrawableType => new PointType("");
        Color IDrawable.DrawableColor => Color.BLACK;
        public PointType(string name)
        {
            Name = name;
            X = RandomCoordinate();
            Y = RandomCoordinate();
        }
        public static int RandomCoordinate()
        { 
            var random = new Random();
            return random.Next(0,400);
        }
    }

    public class RayType : Type, IDrawable
    {
        Type IDrawable.DrawableType => new RayType(StartPoint,Point,"");
        Color IDrawable.DrawableColor => Color.BLACK;
        public override SyntaxType SyntaxType =>SyntaxType.Ray;
        public PointType StartPoint { get; private set; }
        public PointType Point { get; private set; }
        public string Name { get; private set; }
        string IDrawable.DrawableStr => Name;
        public RayType(PointType startPoint, PointType point, string name)
        {
            StartPoint = startPoint;
            Point = point;
            Name = name;
        }
        

    }


    public class SegmentType : Type, IDrawable
    {
        Type IDrawable.DrawableType => new SegmentType(StartPoint,EndPoint,"");
        Color IDrawable.DrawableColor => Color.BLACK;
        public override SyntaxType SyntaxType => SyntaxType.Segment;
        public PointType StartPoint { get; private set; }
        public PointType EndPoint { get; private set; }
        public string Name { get; private set; }
        string IDrawable.DrawableStr => Name;

        public SegmentType(PointType startPoint, PointType endPoint, string name)
        {
            StartPoint = startPoint;
            EndPoint = endPoint;
            Name = name;
        }
        

    }

    public class SequenceType : Type
    {
        public string Name { get; private set; }
        public override SyntaxType SyntaxType => SyntaxType.Sequence;
        public List<Type> SequenceElements { get; private set; }
        public SequenceType(List<Type> sequenceElements, string name)
        {
            SequenceElements = sequenceElements;
            Name = name;
        }
    }

    public class Undefined : Type
    {
        public override SyntaxType SyntaxType => SyntaxType.Undefined;
    }

}