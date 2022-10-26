using System.Buffers;

namespace CoinTosses
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

        private void button1_Click_1(object sender, EventArgs e)
        {
            int TrialsCount = 0;
            int sequences = 0;
            if (!int.TryParse(textBox1.Text, out TrialsCount) || TrialsCount <= 1)
                return;
            if (!int.TryParse(textBox2.Text, out sequences) || sequences <= 0)
                return;
            this.b = new Bitmap(this.pictureBox1.Width, this.pictureBox1.Height);
            this.g = Graphics.FromImage(this.b);
            this.g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.g.Clear(Color.White);

            
            double successProbability = 0.5;

            Point min = new Point(0, 0), max = new Point(TrialsCount, TrialsCount);


            Rectangle virtualWindow = new Rectangle(20, 20, this.b.Width - 40, this.b.Height - 40);

            g.DrawRectangle(Pens.Black, virtualWindow);


            List<double> absoluteLastTrials = new List<double>(); 
            List<double> relativeLastTrials = new List<double>();
            List<double> normalizedLastTrials = new List<double>();

            

            for (int i = 0; i < sequences; i++) {
                int Y = 0;
                List<PointF> absolutePoints = new List<PointF>();
                List<PointF> relativePoints = new List<PointF>();
                List<PointF> normalizedPoints = new List<PointF>();
                for (int X = 1; X <= TrialsCount; ++X)
                {
                    double Uniform = r.NextDouble();
                    if (Uniform < successProbability)
                    {
                        ++Y;
                    }
                    float normalizedY = (float)(Y) / (float)Math.Sqrt(X);
                    float relativeY = (float)Y / (float)X;
                    absolutePoints.Add(fromRealToVirtual(new PointF((float)X, (float)Y), min, max, virtualWindow));
                    normalizedPoints.Add(fromRealToVirtual(new PointF((float)X, normalizedY), min, max, virtualWindow));
                    relativePoints.Add(fromRealToVirtual(new PointF((float)X, relativeY), min, max, virtualWindow));

                    if (X == TrialsCount) {
                        absoluteLastTrials.Add(double.Parse(Y.ToString()));
                        normalizedLastTrials.Add(double.Parse(normalizedY.ToString()));
                        relativeLastTrials.Add(double.Parse(relativeY.ToString()));
                    }
                }
                g.DrawLines(penAbsoluteTrajectory, absolutePoints.ToArray());
                g.DrawLines(penRelativeTrajectory, relativePoints.ToArray());
                g.DrawLines(penNormalizedTrajectory, normalizedPoints.ToArray());
            }
            



            /*Legend*/
            Rectangle Legend = new Rectangle(30, 30, 150, 110);
            g.DrawRectangle(Pens.Black, Legend);

            Rectangle legendText = new Rectangle(75, 35, 60, 18);
            g.DrawString("Legend:", new Font("Tahoma", 10), Brushes.Black, legendText);

            Rectangle firstItem = new Rectangle(40, 55, 15, 15);
            g.FillRectangle(Brushes.Orange, firstItem);

            Rectangle firstText = new Rectangle(firstItem.Right + 3, firstItem.Top + 2, 100, firstItem.Height + 3);
            g.DrawString("Relative Frequency", new Font("Tahoma", 8), Brushes.Black, firstText);

            Rectangle secondItem = new Rectangle(40, firstItem.Bottom + 10, 15, 15);
            g.FillRectangle(Brushes.Blue, secondItem);

            Rectangle secondText = new Rectangle(secondItem.Right + 3, secondItem.Top + 2, 100, secondItem.Height + 3);
            g.DrawString("Absolute Frequency", new Font("Tahoma", 8), Brushes.Black, secondText);

            Rectangle thirdItem = new Rectangle(40, secondItem.Bottom + 10, 15, 15);
            g.FillRectangle(Brushes.DarkGray, thirdItem);

            Rectangle thirdText = new Rectangle(thirdItem.Right + 3, thirdItem.Top + 2, 150, thirdItem.Height + 3);
            g.DrawString("Normalized Frequency", new Font("Tahoma", 8), Brushes.Black, thirdText);

            List<Interval> absoluteIntervals = float_distribution(absoluteLastTrials);
            List<Interval> relativeIntervals = float_distribution(relativeLastTrials);
            List<Interval> normalizedIntervals = float_distribution(normalizedLastTrials);

            //da 190-30 a 340-180
            Rectangle absoluteIstogram = new Rectangle(30 + 150 + 10, 30, 250, 200);
            g.DrawRectangle(Pens.Black, absoluteIstogram);
            this.createIstogramAbsolute(absoluteIstogram, g, 190,30,250,150, absoluteIntervals);

            Rectangle normalizedIstogram = new Rectangle(30 + 150 + 10, 240, 250, 200);
            g.DrawRectangle(Pens.Black, normalizedIstogram);
            this.createIstogramNormalized(normalizedIstogram, g, 190, 240, 250, 150, normalizedIntervals);

            Rectangle relativeIstogram = new Rectangle(450, 30, 250, 200);
            g.DrawRectangle(Pens.Black, relativeIstogram);
            this.createIstogramRelative(relativeIstogram, g, 450, 30, 250, 150, relativeIntervals);

            this.pictureBox1.Image = b;
        }


        public void createIstogramAbsolute(Rectangle istogramSpace, Graphics g, int x , int y, int w, int h, List<Interval> intervals) {
            int step = (w / 7);
            int max_value = 0;
            foreach (Interval i in intervals) { 
                if(i.count > max_value)
                    max_value = i.count;
            }
            int X = x+step-15;
            Pen istoPen = new Pen(Color.Blue, 20);
            foreach (Interval interval in intervals) {
                double pct = double.Parse(interval.count.ToString()) / double.Parse(max_value.ToString());
                X += step;
                if (interval.count > 0) {
                    g.DrawLine(istoPen,
                               new Point(X, y + h - 5),
                               new Point(X, y + h - 5 - (int)(pct * (h-20)))  // Use that percentage of the height
                    );
                    Rectangle firstText = new Rectangle(X-14, y + h - 2, 32, 40);
                    g.DrawString(interval.ToStringDoubleNoCountAbsolute(), new Font("Tahoma", 8), Brushes.Black, firstText);
                }

            }

        }

        public void createIstogramRelative(Rectangle istogramSpace, Graphics g, int x, int y, int w, int h, List<Interval> intervals)
        {
            int step = (w / 7);
            int max_value = 0;
            foreach (Interval i in intervals)
            {
                if (i.count > max_value)
                    max_value = i.count;
            }
            int X = x + step - 15;
            Pen istoPen = new Pen(Color.Orange, 20);
            foreach (Interval interval in intervals)
            {
                double pct = double.Parse(interval.count.ToString()) / double.Parse(max_value.ToString());
                X += step;
                if (interval.count > 0)
                {
                    g.DrawLine(istoPen,
                               new Point(X, y + h - 5),
                               new Point(X, y + h - 5 - (int)(pct * (h - 20)))  // Use that percentage of the height
                    );
                    Rectangle firstText = new Rectangle(X - 14, y + h - 2, 32, 40);
                    g.DrawString(interval.ToStringDoubleNoCountRelative(), new Font("Tahoma", 8), Brushes.Black, firstText);
                }

            }

        }

        public void createIstogramNormalized(Rectangle istogramSpace, Graphics g, int x, int y, int w, int h, List<Interval> intervals)
        {
            int step = (w / 7);
            int max_value = 0;
            foreach (Interval i in intervals)
            {
                if (i.count > max_value)
                    max_value = i.count;
            }
            int X = x + step - 15;
            Pen istoPen = new Pen(Color.DarkGray, 20);
            foreach (Interval interval in intervals)
            {
                double pct = double.Parse(interval.count.ToString()) / double.Parse(max_value.ToString());
                X += step;
                if (interval.count > 0)
                {
                    g.DrawLine(istoPen,
                               new Point(X, y + h - 5),
                               new Point(X, y + h - 5 - (int)(pct * (h - 20)))  // Use that percentage of the height
                    );
                    Rectangle firstText = new Rectangle(X - 14, y + h - 2, 32, 40);
                    g.DrawString(interval.ToStringDoubleNoCountRelative(), new Font("Tahoma", 8), Brushes.Black, firstText);
                }

            }

        }
    }
}