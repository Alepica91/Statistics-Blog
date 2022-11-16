
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Bernoulli_lambda_n
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
            this.penRelativeTrajectory = new Pen(Color.Orange, 1);
            this.penAbsoluteTrajectory = new Pen(Color.Blue, 1);
            this.penNormalizedTrajectory = new Pen(Color.Gray, 1);
        }

        public static double Truncate(double value, int precision)
        {
            return Math.Truncate(value * Math.Pow(10, precision)) / Math.Pow(10, precision);
        }

        private List<Interval> float_distribution(List<double> values)
        {

            double granularity = 10;
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


        public void createIstogramHorizontalInter(Rectangle istogramSpace, Graphics g, int x, int w, Dictionary<float,int> distances)
        {
            float max_value = distances.Keys.Max();
       
            Pen istoPen = new Pen(Color.Green, 4);

            foreach(float key in distances.Keys)
            {
                double currentInterValue = distances[key];
                double pct = (double)distances[key] / (double)max_value;
                g.DrawLine(istoPen,
                           new PointF(x, key),
                           new PointF(x + ((int)(pct * (w - 30))), key)  // Use that percentage of the height
                );
            }

        }

        public void graphCreation() {
            int TrialsCount = (int)numericUpDown2.Value;
            int sequences = (int)numericUpDown3.Value;
            int lambda = (int)numericUpDown1.Value;
            int interarrival = 0;
            this.b = new Bitmap(this.pictureBox1.Width, this.pictureBox1.Height);
            this.g = Graphics.FromImage(this.b);
            this.g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.g.Clear(Color.Black);
            bool absolute = checkBox1.Checked;
            bool relative = checkBox2.Checked;
            bool normalized = checkBox3.Checked;


            double successProbability = double.Parse(lambda.ToString()) / double.Parse(TrialsCount.ToString());

            Point min = new Point(0, 0), max = new Point(TrialsCount, TrialsCount);

            Rectangle virtualWindow = new Rectangle(20, 20, (this.b.Width / 10) * 8 - 40, this.b.Height - 40);

            g.DrawRectangle(Pens.Black, virtualWindow);


            List<double> absoluteLastTrials = new List<double>();
            List<double> relativeLastTrials = new List<double>();
            List<double> normalizedLastTrials = new List<double>();
            List<double> interarrivalValues = new List<double>();
            List<double> successValues = new List<double>();

            Dictionary<float, int> distances = new Dictionary<float, int>();

            for (int i = 0; i < sequences; i++)
            {
                int Y = 0;

                List<PointF> absolutePoints = new List<PointF>();
                List<PointF> relativePoints = new List<PointF>();
                List<PointF> normalizedPoints = new List<PointF>();

                interarrival = 0;
                for (int X = 1; X <= TrialsCount; ++X)
                {

                    double Uniform = r.NextDouble();
                    if (Uniform <= successProbability)
                    {
                        Y++;
                    }
                    else
                    {
                        interarrival++;
                    }
                    float relativeY = ((float)(Y) * TrialsCount) / (float)X;
                    float normalizedY = ((float)Y * (float)Math.Sqrt(TrialsCount)) / (float)Math.Sqrt(X);
                    absolutePoints.Add(fromRealToVirtual(new PointF((float)X, (float)Y), min, max, virtualWindow));
                    normalizedPoints.Add(fromRealToVirtual(new PointF((float)X, normalizedY), min, max, virtualWindow));
                    relativePoints.Add(fromRealToVirtual(new PointF((float)X, relativeY), min, max, virtualWindow));

                    if (X == TrialsCount)
                    {
                        absoluteLastTrials.Add(double.Parse(Y.ToString()));
                        normalizedLastTrials.Add(double.Parse(normalizedY.ToString()));
                        relativeLastTrials.Add(double.Parse(relativeY.ToString()));
                        float Yy = fromRealToVirtual(new PointF((float)X, (float)Y), min, max, virtualWindow).Y;
                        if (distances.ContainsKey(Yy))
                            distances[Yy] = distances[Yy] + interarrival;
                        else
                            distances.Add(Yy, interarrival);
                    }
                }
                if(relative)
                    g.DrawLines(penRelativeTrajectory, relativePoints.ToArray());
                if(normalized)
                    g.DrawLines(penNormalizedTrajectory, normalizedPoints.ToArray());
                if(absolute)
                    g.DrawLines(penAbsoluteTrajectory, absolutePoints.ToArray());


                successValues.Add(Y);
            }

            for (int i = 0; i < interarrivalValues.Count; i++)
            {
                interarrivalValues[i] = interarrivalValues[i] / sequences;
            }

            /*Legend*/
            Rectangle Legend = new Rectangle(230, 30, 150, 110);
            g.DrawRectangle(Pens.White, Legend);

            Rectangle legendText = new Rectangle(275, 35, 60, 18);
            g.DrawString("Legend:", new Font("Tahoma", 10), Brushes.White, legendText);

            Rectangle firstItem = new Rectangle(240, 55, 15, 15);
            g.FillRectangle(Brushes.Orange, firstItem);

            Rectangle firstText = new Rectangle(firstItem.Right + 3, firstItem.Top + 2, 100, firstItem.Height + 3);
            g.DrawString("Relative Frequency", new Font("Tahoma", 8), Brushes.White, firstText);

            Rectangle secondItem = new Rectangle(240, firstItem.Bottom + 10, 15, 15);
            g.FillRectangle(Brushes.Blue, secondItem);

            Rectangle secondText = new Rectangle(secondItem.Right + 3, secondItem.Top + 2, 100, secondItem.Height + 3);
            g.DrawString("Absolute Frequency", new Font("Tahoma", 8), Brushes.White, secondText);

            Rectangle thirdItem = new Rectangle(240, secondItem.Bottom + 10, 15, 15);
            g.FillRectangle(Brushes.Gray, thirdItem);

            Rectangle thirdText = new Rectangle(thirdItem.Right + 3, thirdItem.Top + 2, 150, thirdItem.Height + 3);
            g.DrawString("Normalized Frequency", new Font("Tahoma", 8), Brushes.White, thirdText);

            if(absolute || relative || normalized)
                createIstogramHorizontalInter(virtualWindow, g, ((this.b.Width / 10) * 8) - 20, (this.b.Width / 10) * 2 - 40, distances);

            this.pictureBox1.Image = b;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            this.graphCreation();
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            this.graphCreation();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            this.graphCreation();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            this.graphCreation();
        }
    }
}