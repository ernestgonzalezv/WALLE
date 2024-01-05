namespace Geo_Wall_E
{
    public class Arc : Type, IDrawable
    {
        public override TypeOfElement TypeOfElement => TypeOfElement.Arc;
        public Point Center { get; private set;  }
        public Point Start { get; private set;  }
        public Point End { get; private set;  }
        public Measure Measure { get; private set;  }
        public string Name { get; private set;  }

        public Arc(Point center, Point start, Point end, Measure measure, string name)
        {
            Center = center;
            Start = start;
            End = end;
            Measure = measure;
            Name = name;
        }
        Geo_Wall_E.Type IDrawable.Type => new Arc(Center,Start,End,Measure,Name);

        Color IDrawable.Color => Color.BLACK;

        string IDrawable.Phrase => Name;
    }

    public class Circle : Type, IDrawable
    {
        public override TypeOfElement TypeOfElement => TypeOfElement.Circle;
        public Point Center { get; private set; }
        public Measure Radius { get; private set; }
        public string Name { get; private set; }
        public Circle(Point center, Measure radius, string name)
        {
            Center = center;
            Radius = radius;
            Name = name;
        }

        Color IDrawable.Color => Color.BLACK;
        
        string IDrawable.Phrase => Name;

        Type IDrawable.Type => new Circle(Center,Radius,Name);
    }

    public class EmptyType : Type
    {
        public override TypeOfElement TypeOfElement => TypeOfElement.Empty;
    }

    public class Function : Type
    {
        public override TypeOfElement TypeOfElement => TypeOfElement.Function;
        public Token Name {get; private set;}
        public List<Expressions> Arguments {get; private set;}
        public Expressions Body {get; private set;}
        public Function(Token name, List<Expressions> args, Expressions body)
        {
            Name = name;
            Arguments = args;
            Body = body;
        }
    }

    public class Line : Type, IDrawable
    {
        public override TypeOfElement TypeOfElement => TypeOfElement.Line;
        public Point P1 { get; private set; }
        public Point P2 { get; private set; }
        public string Name { get; private set; }

        public Line(Point p1, Point p2, string name)
        {
            P1 = p1;
            P2 = p2;
            Name = name;
        }

        Type IDrawable.Type => new Line(P1,P2,"");

        Color IDrawable.Color => Color.BLACK;
        string IDrawable.Phrase => Name;

    }

    public class Number : Type
    {
        public override TypeOfElement TypeOfElement => TypeOfElement.Number;

        public double Value { get; private set; }

        public Number(double value)
        {
            Value = value;
        }

    }

    public class String : Type
    {
        public override TypeOfElement TypeOfElement => TypeOfElement.String;
        public  string Value { get; private set; }

        public String(string value)
        {
            Value = value;
        }
    }
    public class Boolean : Type
    {
        public override TypeOfElement TypeOfElement => TypeOfElement.Boolean;
        public int Value { get; private set;}
        public Boolean(bool value)
        {
            if (value) Value = 1;
            else Value = 0;
        }
    }

    public class Measure : Type
    {
        public override TypeOfElement TypeOfElement => TypeOfElement.Measure;
        public Point P1 { get; private set;  }
        public Point P2 { get; private set;  }
        public string Name { get; private set;  }
        public double Measure_ { get; private set;  }

        public Measure(Point p1, Point p2, string name)
        {
            P1 = p1;
            P2 = p2;
            Name = name;
            Measure_ = Math.Sqrt(Math.Pow(P2.X - P1.X, 2) + Math.Pow(P2.Y - P1.Y, 2));
        }

        public Measure(double measure, string name)
        {
            P1 = new Point("");
            P2 = new Point("");
            Name = name;
            Measure_ = measure;
        }
        public double MeasureBetweenPoints()
        {
            double measure = Math.Sqrt(Math.Pow(P2.X - P1.X, 2) + Math.Pow(P2.Y - P1.Y, 2));
            return measure;
        }
    }

    public class Point : Type, IDrawable
    {
        public override TypeOfElement TypeOfElement => TypeOfElement.Point;
        public string Name { get; private set; }
        public double X { get; set; }
        public double Y { get; set; }
        public Point(string name)
        {
            Name = name;
            X = RandomCoordinateX();
            Y = RandomCoordinateY();
        }
        Type IDrawable.Type => new Point("");

        Color IDrawable.Color => Color.BLACK;
        string IDrawable.Phrase => Name;

        public static int RandomCoordinateX()
        {
            //Genera un número random entre 0 y 400
            Random random = new();
            int MaxPointCoordinate = 400;
            return random.Next(0,MaxPointCoordinate);
        }
        public static int RandomCoordinateY()
        {
            //Genera un número random entre 0 y 400
            Random random = new();
            int MaxPointCoordinate = 400;
            return random.Next(0,MaxPointCoordinate);
        }
    }

    public class Ray : Type, IDrawable
    {
        public override TypeOfElement TypeOfElement =>TypeOfElement.Ray;
        public Point Start { get; private set; }
        public Point Point { get; private set; }
        public string Name { get; private set; }

        public Ray(Point start, Point point, string name)
        {
            Start = start;
            Point = point;
            Name = name;
        }
        Type IDrawable.Type => new Ray(Start,Point,"");

        Color IDrawable.Color => Color.BLACK;
        string IDrawable.Phrase => Name;

    }

    public class Segment : Type, IDrawable
    {
        public override TypeOfElement TypeOfElement => TypeOfElement.Segment;
        public Point Start { get; private set; }
        public Point End { get; private set; }
        public string Name { get; private set; }

        public Segment(Point start, Point end, string name)
        {
            Start = start;
            End = end;
            Name = name;
        }
        Type IDrawable.Type => new Segment(Start,End,"");

        Color IDrawable.Color => Color.BLACK;
        string IDrawable.Phrase => Name;

    }

    public class Sequence : Type
    {
        public override TypeOfElement TypeOfElement => TypeOfElement.Sequence;
        public List<Type> Elements { get; private set; }
        public string Name { get; private set; }

        public Sequence(List<Type> elements, string name)
        {
            Elements = elements;
            Name = name;
        }
    }

    public enum TypeOfElement
    {
        Arc,
        Boolean,
        Circle,
        Constant,
        Empty,
        Function,
        Line,
        Measure,
        Number,
        Point,
        Ray,
        Segment,
        String,
        Sequence,
        Undefined,
    }

    public abstract class Type
    {
        public abstract TypeOfElement TypeOfElement { get; }
    }

    public class Undefined : Type
    {
        public override TypeOfElement TypeOfElement => TypeOfElement.Undefined;
    }

}