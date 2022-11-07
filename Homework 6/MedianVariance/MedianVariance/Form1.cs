using System.Windows.Forms;

namespace MedianVariance
{
    public partial class Form1 : Form
    {

        private Random r = new Random();
        List<double> medianDistribution = new List<double>();
        List<double> varianceDistribution = new List<double>();
        double populationVariance = 0.0;
        double populationMedian = 0.0;

        private Bitmap b;
        private Graphics g;
        private Pen penRelativeTrajectory;
        private Pen penAbsoluteTrajectory;
        private Pen penNormalizedTrajectory;

        public Form1()
        {
            InitializeComponent();
            textBox1.Multiline = true;
            textBox1.ScrollBars = ScrollBars.Both;
            this.b = new Bitmap(this.pictureBox1.Width, this.pictureBox1.Height);
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

        public void createIstogramVerticalMedian(Rectangle istogramSpace, Graphics g, int x, int y, int w, int h, List<Interval> intervals)
        {
            int step = (w / 7);
            int max_value = 0;
            foreach (Interval i in intervals)
            {
                if (i.count > max_value)
                    max_value = i.count;
            }
            int X = x + step;
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
                    Rectangle firstText = new Rectangle(X - 35, y + h - 30, 75, 40);
                    g.DrawString(interval.ToStringDoubleNoCountRelative(), new Font("Tahoma", 8), Brushes.Black, firstText);
                }
                X += step;
            }

        }

        public void createIstogramVerticalVariance(Rectangle istogramSpace, Graphics g, int x, int y, int w, int h, List<Interval> intervals)
        {
            int step = (w / 7);
            int max_value = 0;
            foreach (Interval i in intervals)
            {
                if (i.count > max_value)
                    max_value = i.count;
            }
            int X = x + step;
            Pen istoPen = new Pen(Color.Blue, 20);
            foreach (Interval interval in intervals)
            {
                double pct = double.Parse(interval.count.ToString()) / double.Parse(max_value.ToString());

                if (interval.count > 0)
                {
                    g.DrawLine(istoPen,
                               new Point(X, y + h - 40),
                               new Point(X, y + h - 5 - (int)(pct * (h - 20)))  // Use that percentage of the height
                    );
                    Rectangle firstText = new Rectangle(X - 35, y + h - 30, 75, 40);
                    g.DrawString(interval.ToStringDoubleNoCountRelative(), new Font("Tahoma", 8), Brushes.Black, firstText);
                }
                X += step;
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.g = Graphics.FromImage(this.b);
            this.g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.g.Clear(Color.White);
            this.medianDistribution = new List<double>();
            this.varianceDistribution = new List<double>();
            this.populationVariance = 0.0;
            this.populationMedian = 0.0;
            textBox1.Clear();
            List<List<double>> population = new List<List<double>>();

            for (int i = 0; i < 10; i++)
            {
                List<double> relativeFrequencyEachSample = new List<double>();
                for (int X = 0; X < 10000; X++)
                {
                    double Uniform = r.NextDouble();
                    relativeFrequencyEachSample.Add(double.Parse(Uniform.ToString()));
                }
                population.Add(relativeFrequencyEachSample);

            }


            //Median and Variance Distribution of each sample
            foreach (List<double> sample in population)
            {
                double samplemedian = 0.0;
                double samplevariance = 0.0;
                foreach (double random in sample)
                {
                    samplemedian += random;
                }
                samplemedian = samplemedian / 10000;
                medianDistribution.Add(samplemedian);

                foreach (double random in sample)
                {
                    samplevariance += Math.Pow((random - samplemedian), 2);
                }
                varianceDistribution.Add(samplevariance / 10000);
            }

            //global Median and Variance value of population
            foreach (List<double> sample in population)
            {
                foreach (double random in sample)
                {
                    this.populationMedian += random;
                }
            }
            this.populationMedian = populationMedian / 100000;
            foreach (List<double> sample in population)
            {
                foreach (double random in sample)
                {
                    this.populationVariance += Math.Pow((random - this.populationMedian), 2);
                }
            }
            this.populationVariance = populationVariance / 100000;

            for (int i = 0; i < 10; i++)
            {
                textBox1.Text += System.Environment.NewLine + "Sample "+i.ToString()+" Median VS Population Median -> " + Truncate(medianDistribution[i],3).ToString() + " --- " + Truncate(populationMedian,3).ToString()+ "    |Absolute Difference| = "+ Math.Abs(Truncate((medianDistribution[i]-populationMedian),3)).ToString();
            }
            textBox1.Text += System.Environment.NewLine;
            textBox1.Text += System.Environment.NewLine;
            for (int i = 0; i < 10; i++)
            {
                textBox1.Text += System.Environment.NewLine + "Sample "+i.ToString()+" Variance VS Population Variance -> " + Truncate(varianceDistribution[i],3).ToString() + " --- " + Truncate(populationVariance,3).ToString()+ "    |Absolute Difference| = " + Math.Abs(Truncate((varianceDistribution[i] - populationVariance),3)).ToString();
            }

            //istrograms
            Rectangle verticalWindow = new Rectangle(20, 20, this.b.Width / 2 - 20, this.b.Height - 40);
            Rectangle vertical2Window = new Rectangle(this.b.Width / 2, 20, this.b.Width / 2 - 20, this.b.Height - 40);
            List<Interval> medianIntervals = float_distribution(medianDistribution);
            List<Interval> varianceIntervals = float_distribution(varianceDistribution);


            this.createIstogramVerticalMedian(verticalWindow, g, 20, 20, this.b.Width / 2, this.b.Height - 40, medianIntervals);
            this.createIstogramVerticalVariance(vertical2Window, g, this.b.Width / 2, 20, this.b.Width / 2, this.b.Height - 40, varianceIntervals);

            Rectangle Legend = new Rectangle(this.b.Width / 2 -75, 20, 120, 90);
            g.DrawRectangle(Pens.Black, Legend);

            Rectangle legendText = new Rectangle(this.b.Width / 2 - 70, 25, 60, 18);
            g.DrawString("Legend:", new Font("Tahoma", 10), Brushes.Black, legendText);

            Rectangle firstItem = new Rectangle(this.b.Width / 2 - 65, 55, 15, 15);
            g.FillRectangle(Brushes.Orange, firstItem);

            Rectangle firstText = new Rectangle(firstItem.Right + 3, firstItem.Top + 2, 100, firstItem.Height + 3);
            g.DrawString("Median", new Font("Tahoma", 8), Brushes.Black, firstText);

            Rectangle secondItem = new Rectangle(this.b.Width / 2 - 65, firstItem.Bottom + 10, 15, 15);
            g.FillRectangle(Brushes.Blue, secondItem);

            Rectangle secondText = new Rectangle(secondItem.Right + 3, secondItem.Top + 2, 100, secondItem.Height + 3);
            g.DrawString("Variance", new Font("Tahoma", 8), Brushes.Black, secondText);

            this.pictureBox1.Image = b;

        }

    }

  
}