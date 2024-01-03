using Geo_Wall_E;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace Walle
{
    public partial class MainForm : Form
    {
        private TextBox inputTextBox;
        private Button Compile;
        private List<IDrawable> MyList;
        private bool isDrawing;
        private Button Run;
        private Label ErrorLabel;
        private Panel DrawingPan;
        private PictureBox Painting;
        private PictureBox Walle1;
                            
        private Button Stop;
        private Label Code;
        
        public MainForm()
        {
            inputTextBox = new TextBox();
            Compile = new Button();
            Run = new Button();
            Stop = new Button();
            Code = new Label();
            ErrorLabel = new Label();
            DrawingPan = new Panel();
            Painting = new PictureBox();
            Walle1 = new PictureBox();

            MyList = new List<IDrawable>();
            isDrawing = false;

            //InitializeComponent();
            Compile.BackColor = System.Drawing.SystemColors.ButtonFace;
            Compile.Cursor = System.Windows.Forms.Cursors.Default;
            Compile.Font = new System.Drawing.Font("Marlett Medium", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            Compile.Click += Compile_Click;
            Compile.Size = new System.Drawing.Size(97, 42);
            Compile.Location = new System.Drawing.Point(460, 75);
            Compile.Text = "Run";
            Compile.UseVisualStyleBackColor = false;

            Run.BackColor = System.Drawing.SystemColors.ButtonFace;
            Run.Font = new System.Drawing.Font("Marlett Medium", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            Run.Click += Run_Click;
            Run.Size = new System.Drawing.Size(97, 42);
            Run.Location = new System.Drawing.Point(460, 200);
            Run.Text = "Draw";
            Run.Enabled = false;
            Run.UseVisualStyleBackColor = false;

            Stop.BackColor = System.Drawing.SystemColors.ButtonFace;
            Stop.Font = new System.Drawing.Font("Marlett Medium", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            Stop.Click += Stop_Click;
            Stop.Location = new System.Drawing.Point(460, 325);
            Stop.Size = new System.Drawing.Size(97, 42);
            Stop.Text = "Stop";
            Stop.Enabled = false;
            Stop.UseVisualStyleBackColor = false;

           

            inputTextBox.BackColor = System.Drawing.SystemColors.ActiveBorder;
            inputTextBox.Location = new System.Drawing.Point(50, 415);
            inputTextBox.Multiline = true;
            inputTextBox.Size = new System.Drawing.Size(322, 350);
            inputTextBox.TextChanged += new EventHandler(TextBoxInput_TextChanged);

            Code.AutoSize = true;
            Code.Font = new System.Drawing.Font("Marlett Medium", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            Code.Location = new System.Drawing.Point(165, 389);
            Code.Size = new System.Drawing.Size(38, 15);
            Code.Text = "Enter Your Code: ";

            ErrorLabel.AutoSize = false;
            ErrorLabel.Font = new System.Drawing.Font("Marlett Medium", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

            ErrorLabel.Location = new System.Drawing.Point(435, 389);
            ErrorLabel.Size = new System.Drawing.Size(250, 86);
            ErrorLabel.Text = "Compilation Errors";

            DrawingPan.AutoScroll = true;
            DrawingPan.BackColor = System.Drawing.SystemColors.ActiveBorder;
            DrawingPan.Location = new System.Drawing.Point(690, 78); 
            DrawingPan.Size = new Size(700, 700);

            Painting.BackColor = System.Drawing.SystemColors.ActiveBorder;
            Painting.Location = new System.Drawing.Point(0, 0); 
            Painting.Size = new System.Drawing.Size(3500, 3500); 
            Painting.TabStop = false;
            Painting.Paint += new PaintEventHandler(Painting_Paint);

            

            Walle1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            Walle1.Location = new System.Drawing.Point(50, 75);
            Walle1.Size = new System.Drawing.Size(300, 300);
            Walle1.TabStop = false;
            Walle1.SizeMode = PictureBoxSizeMode.StretchImage;
            Walle1.Image = Image.FromFile("photo_2024-01-03_03-00-58.jpg");



            BackColor = System.Drawing.Color.DimGray;
            WindowState = FormWindowState.Maximized;
            Controls.Add(Run);
            Controls.Add(Compile);
            Controls.Add(Stop);
            Controls.Add(inputTextBox);
            Controls.Add(Code);
            Controls.Add(ErrorLabel);
            Controls.Add(DrawingPan);
            DrawingPan.Controls.Add(Painting);
            Controls.Add(Walle1);
            Cursor = System.Windows.Forms.Cursors.Default;
            ForeColor = System.Drawing.SystemColors.ControlText;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            ((System.ComponentModel.ISupportInitialize)(this.Painting)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        public void AddDrawable(IDrawable drawable)
        {
            MyList.Add(drawable);
            Painting.Invalidate(); 
        }

        private void TextBoxInput_TextChanged(object sEndPointer, EventArgs e)
        {
            Run.Enabled = false;
        }
        private void Compile_Click(object sEndPointer, EventArgs e)
        {
            try
            {
                string userInput = inputTextBox.Text;
                Compiler compiler = new Compiler(userInput);
                MyList = compiler.Answer;
                Run.Enabled = true;
                ErrorLabel.Text = "Compilation Errors"; 
            }
            catch (Exception ex)
            {
                
                ErrorLabel.Text = ex.Message;
                Run.Enabled = false; 
            }
        }

        private void Run_Click(object sEndPointer, EventArgs e)
        {

            isDrawing = true;
            Stop.Enabled = true;
            Painting.Invalidate();
            Stop.Enabled = false;
            Run.Enabled = false;
        }

        private void Stop_Click(object sEndPointer, EventArgs e)
        {
            isDrawing = false;
            Stop.Enabled = false;
        }

       
        private void Painting_Paint(object sEndPointer, PaintEventArgs e)
        {
            
            foreach (var drawable in MyList)
            {
                if (isDrawing == false)
                {
                    return;
                }
                System.Drawing.Color color = SearchColor(drawable);
                Pen pen = new Pen(color);
                if (drawable.DrawableStr != null)
                {
                    Random random = new Random();
                    int x = random.Next(Painting.Width);
                    int y = random.Next(Painting.Height);
                    string text = drawable.DrawableStr;
                    Font font = new Font("Marlett Medium", 16);
                    SolidBrush brush = new SolidBrush(color);
                    e.Graphics.DrawString(text, font, brush, x, y);
                }

                switch (drawable.DrawableType)
                {
                    case Geo_Wall_E.PointType:
                        DrawPoint(e.Graphics, pen, drawable.DrawableType);
                        break;
                    case Line:
                        DrawLine(e.Graphics, pen, drawable.DrawableType);
                        break;
                    case SegmentType:
                        DrawSegment(e.Graphics, pen, drawable.DrawableType);
                        break;
                    case RayType:
                        DrawRay(e.Graphics, pen, drawable.DrawableType);
                        break;
                    case CircleType:
                        DrawCircle(e.Graphics, pen, drawable.DrawableType);
                        break;
                    case ArcType:
                        DrawArc(e.Graphics, pen, drawable.DrawableType);
                        break;
                    case SequenceType:
                        DrawSequence(e.Graphics, pen, drawable.DrawableType);
                        break;
                    default:
                        ErrorLabel.Text = "Can't Draw this type";
                        break;
                }
                
            }

        }
       

        private System.Drawing.Color SearchColor(IDrawable drawable)
        {
            return drawable.DrawableColor switch
            {
                Geo_Wall_E.Color.RED => System.Drawing.Color.Red,
                Geo_Wall_E.Color.GREEN => System.Drawing.Color.Green,
                Geo_Wall_E.Color.BLUE => System.Drawing.Color.Blue,
                Geo_Wall_E.Color.YELLOW => System.Drawing.Color.Yellow,
                Geo_Wall_E.Color.CYAN => System.Drawing.Color.Cyan,
                Geo_Wall_E.Color.BLACK => System.Drawing.Color.Black,
                Geo_Wall_E.Color.MAGENTA => System.Drawing.Color.Magenta,
                Geo_Wall_E.Color.WHITE => System.Drawing.Color.White,
                Geo_Wall_E.Color.GRAY => System.Drawing.Color.Gray,
            };
        }
        private  void DrawPoint(Graphics g, Pen pencil, Geo_Wall_E.Type drawable)
        {
  

            Geo_Wall_E.PointType point = (Geo_Wall_E.PointType)drawable;
            g.DrawEllipse(pencil, (int)point.X + DrawingPan.HorizontalScroll.Value, (int)point.Y, 5, 5);
        }

        private void DrawLine(Graphics g, Pen pencil, Geo_Wall_E.Type drawable)
        {
            Line l = (Line)drawable;
            double m = (l.Point2.Y - l.Point1.Y) / (l.Point2.X - l.Point1.X);
            int xLeft = 0;
            int yLeft = (int)(l.Point1.Y - m * l.Point1.X);
            int xRight = Painting.Width;
            int yRight = (int)(m * (xRight - l.Point1.X) + l.Point1.Y);
            g.DrawLine(pencil, xLeft, yLeft, xRight, yRight);
        }

        private void DrawSegment(Graphics g, Pen pencil, Geo_Wall_E.Type drawable)
        {
            SegmentType s = (SegmentType)drawable;
            g.DrawLine(pencil, (int)s.StartPoint.X, (int)s.StartPoint.Y, (int)s.EndPoint.X, (int)s.EndPoint.Y);
        }

        private void DrawRay(Graphics g, Pen pencil, Geo_Wall_E.Type drawable)
        {
            RayType r = (RayType)drawable;
            double m = (r.Point.Y - r.StartPoint.Y) / (r.Point.X - r.StartPoint.X);

            if (r.StartPoint.Y < r.Point.Y)
            {
                int xLeft = 0;
                int yLeft = (int)(r.StartPoint.Y - m * r.StartPoint.X);
                g.DrawLine(pencil, (int)r.StartPoint.X, (int)r.StartPoint.Y, xLeft, yLeft);
            }
            else
            {
                int xRight = Painting.Width;
                int yRight = (int)(m * (xRight - r.StartPoint.X) + r.StartPoint.Y);
                g.DrawLine(pencil, (int)r.StartPoint.X, (int)r.StartPoint.Y, xRight, yRight);
            }
        }

        private void DrawCircle(Graphics g, Pen pencil, Geo_Wall_E.Type drawable)
        {
            CircleType c = (CircleType)drawable;
            g.DrawEllipse(pencil, (int)(c.CenterPoint.X - c.CircleRadius.Distance), (int)(c.CenterPoint.Y - c.CircleRadius.Distance), (int)c.CircleRadius.Distance * 2, (int)c.CircleRadius.Distance * 2);
        }

        private void DrawArc(Graphics g, Pen pencil, Geo_Wall_E.Type drawable)
        {
            ArcType a = (ArcType)drawable;
            float StartPointAngle = (float)(Math.Atan2(a.StartPoint.Y - a.CenterPoint.Y, a.StartPoint.X - a.CenterPoint.X) * 180 / Math.PI);
            float EndPointAngle = (float)(Math.Atan2(a.EndPoint.Y - a.CenterPoint.Y, a.EndPoint.X - a.CenterPoint.X) * 180 / Math.PI);
            if (EndPointAngle < StartPointAngle)
            {
                EndPointAngle += 360;
            }
            float sweepAngle = EndPointAngle - StartPointAngle;
            g.DrawLine(pencil, (int)a.CenterPoint.X, (int)a.CenterPoint.Y, (int)a.StartPoint.X, (int)a.StartPoint.Y);
            g.DrawLine(pencil, (int)a.CenterPoint.X, (int)a.CenterPoint.Y, (int)a.EndPoint.X, (int)a.EndPoint.Y);
            g.DrawArc(pencil, (int)(a.CenterPoint.X - a.Measure.Distance), (int)(a.CenterPoint.Y - a.Measure.Distance), (int)a.Measure.Distance * 2, (int)a.Measure.Distance * 2, StartPointAngle, sweepAngle);
        }
        private void DrawSequence(Graphics g, Pen pencil, Geo_Wall_E.Type drawable)
        {
            SequenceType sequence = (SequenceType)drawable;
            foreach (var item in sequence.SequenceElements)
            {
                switch (item)
                {
                    case Geo_Wall_E.PointType:
                        DrawPoint(g, pencil, item);
                        break;
                    case Line:
                        DrawLine(g, pencil, item);
                        break;
                    case SegmentType:
                        DrawSegment(g, pencil, item);
                        break;
                    case RayType:
                        DrawRay(g, pencil, item);
                        break;
                    case CircleType:
                        DrawCircle(g, pencil, item);
                        break;
                    case ArcType:
                        DrawArc(g, pencil, item);
                        break;
                    case SequenceType:
                        DrawSequence(g, pencil, item);
                        break;
                    default:
                        ErrorLabel.Text = "Cant Draw this type";
                        break;
                }
            }
        }


        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }

        
    }
}
