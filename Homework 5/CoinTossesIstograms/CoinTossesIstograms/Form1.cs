using System.Buffers;

namespace CoinTossesIstograms
{
    public partial class Form1 : Form
    {
        private Bitmap b;
        private Graphics g;
        private Random r;
        private Pen penRelativeTrajectory;
        private Pen penAbsoluteTrajectory;
        private Pen penNormalizedTrajectory;

        public Form1()
        {
            InitializeComponent();
            this.b = new Bitmap(this.pictureBox1.Width, this.pictureBox1.Height);
            this.r = new Random();
            this.penRelativeTrajectory = new Pen(Color.Orange, 2);
            this.penAbsoluteTrajectory = new Pen(Color.Blue, 2);
            this.penNormalizedTrajectory = new Pen(Color.DarkGray, 2);
        }

        public static double Truncate(double value, int precision)
        {
            return Math.Truncate(value * Math.Pow(10, precision)) / Math.Pow(10, precision);
        }

        private List<Interval> float_distribution(List<double> values)
        {

            double granularity = 5;

            double step = (values.Max() - values.Min()) / granularity;
            List<Interval> intervals = this.double_intervals_creation(granularity, values, step);

            foreach (double value in values)
            {
                foreach (Interval interval in intervals)
                {
                    interval.itIsInside(value);
                }
            }
            foreach (Interval interval in intervals)
            {
                //textBox2.Text += System.Environment.NewLine + interval.ToStringDoubleNoCount() + Truncate((double.Parse(interval.count.ToString()) / double.Parse(values.Count.ToString())) * 100, 2) + "%";
            }

            return intervals;

        }

        private List<Interval> double_intervals_creation(double granularity, List<double> values, double step)
        {
            List<Interval> intervals = new List<Interval>();
            for (int i = 0; i < granularity; i++)
            {
                if (i == granularity - 1)
                {
                    if (i == 0)
                    {
                        Interval interval = new Interval(values.Min(), values.Max(), 0, true);
                        intervals.Add(interval);
                    }
                    else
                    {
                        Interval interval = new Interval(intervals[i - 1].max, intervals[i - 1].max + step, 0, true);
                        intervals.Add(interval);
                    }

                }
                else if (i == 0)
                {
                    Interval interval = new Interval(values.Min(), values.Min() + step, 0, true);
                    intervals.Add(interval);
                }
                else
                {
                    Interval interval = new Interval(intervals[i - 1].max, intervals[i - 1].max + step, 0, false);
                    intervals.Add(interval);
                }
            }
            return intervals;

        }

        private PointF fromRealToVirtual(PointF XY, Point min, Point max, Rectangle r)
        {
            float newX = max.X - min.X == 0 ? 0 : (r.Left + r.Width * (XY.X - min.X) / (max.X - min.X));
            float newY = max.Y - min.Y == 0 ? 0 : (r.Top + r.Height - r.Height * (XY.Y - min.Y) / (max.Y - min.Y));
            return new PointF(newX, newY);
        }

        public void createIstogramVertical(Rectangle istogramSpace, Graphics g, int x, int y, int w, int h, List<Interval> intervals)
        {
            int step = (w / 7);
            int max_value = 0;
            foreach (Interval i in intervals)
            {
                if (i.count > max_value)
                    max_value = i.count;
            }
            int X = x+step;
            Pen istoPen = new Pen(Color.Orange, 20);
            foreach (Interval interval in intervals)
            {
                double pct = double.Parse(interval.count.ToString()) / double.Parse(max_value.ToString());
                
                if (interval.count > 0)
                {
                    g.DrawLine(istoPen,
                               new Point(X, y + h - 40),
                               new Point(X, y + h - 5 - (int)(pct * (h - 20)))  // Use that percentage of the height
                    );
                    Rectangle firstText = new Rectangle(X - 28, y + h - 30, 60, 40);
                    g.DrawString(interval.ToStringDoubleNoCountRelative(), new Font("Tahoma", 8), Brushes.Black, firstText);
                }
                X += step;
            }

        }

        public void createIstogramHorizontal(Rectangle istogramSpace, Graphics g, int x, int y, int w, int h, List<Interval> intervals)
        {
            int step = (h / 7);
            int max_value = 0;
            foreach (Interval i in intervals)
            {
                if (i.count > max_value)
                    max_value = i.count;
            }
            int H = h - step;
            Pen istoPen = new Pen(Color.Orange, 20);
            for(int i = intervals.Count -1 ; i >= 0 ; i--)
            {
                double pct = double.Parse(intervals[i].count.ToString()) / double.Parse(max_value.ToString());

                if (intervals[i].count > 0)
                {
                    g.DrawLine(istoPen,
                               new Point(x+40, H),
                               new Point(x+((int)(pct * (w - 30))), H )  // Use that percentage of the height
                    );
                    Rectangle firstText = new Rectangle(x + 40, H - 30, 60, 40);
                    g.DrawString(intervals[i].ToStringDoubleNoCountRelative(), new Font("Tahoma", 8), Brushes.Black, firstText);
                }
                H -= step;
            }

        }


        private void button1_Click(object sender, EventArgs e)
        {

            this.b = new Bitmap(this.pictureBox1.Width, this.pictureBox1.Height);
            this.g = Graphics.FromImage(this.b);
            this.g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.g.Clear(Color.White);

            double successProbability = 0.5;

            Point min = new Point(0, 0), max = new Point(100, 100);


            Rectangle verticalWindow = new Rectangle(20, 20, this.b.Width / 2 -20, this.b.Height - 40);
            Rectangle horizWindow = new Rectangle(this.b.Width/2, 20, this.b.Width/2 -20, this.b.Height - 40);

            g.DrawRectangle(Pens.Black, verticalWindow);
            g.DrawRectangle(Pens.Black, horizWindow);

            List<double> relativeLastTrials = new List<double>();

            for (int i = 0; i < 100; i++)
            {
                int Y = 0;
                List<PointF> absolutePoints = new List<PointF>();
                List<PointF> relativePoints = new List<PointF>();
                List<PointF> normalizedPoints = new List<PointF>();
                for (int X = 1; X <= 100; ++X)
                {
                    double Uniform = r.NextDouble();
                    if (Uniform < successProbability)
                    {
                        ++Y;
                    }

                    float relativeY = (float)Y / (float)X;

                    if (X == 100)
                    {

                        relativeLastTrials.Add(double.Parse(relativeY.ToString()));
                    }
                }
            }

            List<Interval> relativeIntervals = float_distribution(relativeLastTrials);


            this.createIstogramVertical(verticalWindow, g, 20, 20, this.b.Width / 2, this.b.Height - 40, relativeIntervals);
            this.createIstogramHorizontal(horizWindow, g, this.b.Width / 2, 20, this.b.Width / 2, this.b.Height - 40, relativeIntervals);

            this.pictureBox1.Image = b;
        }
    }
}