using static System.Net.Mime.MediaTypeNames;

namespace OtherDistributionsApp
{
    public partial class Form1 : Form
    {
        Graphics g, g2, g3, g4, g5;
        Rectangle rect1, rect2, rect3, rect4, rect5;

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            numberOfSamples = trackBar1.Value;
            label1.Text = "# samples : " + numberOfSamples.ToString();
        }

        Pen PenTrajectory;
        int numberOfSamples = 1000;
        Bitmap b, b2, b3, b4, b5;

        public Form1()
        {
            InitializeComponent();
            InitializeGraphics();
            trackBar1.Maximum = 100000;
            trackBar1.Minimum = 1000;
        }

        private void InitializeGraphics()
        {
            b = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            g = Graphics.FromImage(b);

            b2 = new Bitmap(pictureBox2.Width, pictureBox2.Height);
            g2 = Graphics.FromImage(b2);

            b3 = new Bitmap(pictureBox3.Width, pictureBox3.Height);
            g3 = Graphics.FromImage(b3);

            b4 = new Bitmap(pictureBox4.Width, pictureBox4.Height);
            g4 = Graphics.FromImage(b4);

            b5 = new Bitmap(pictureBox5.Width, pictureBox5.Height);
            g5 = Graphics.FromImage(b5);
        }

        private List<Interval> double_intervals_creation( List<double> values, int granularity)
        {
            List<Interval> intervals = new List<Interval>();
            double step = (values.Max() - values.Min()) / granularity;
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

            foreach (double value in values)
            {
                foreach (Interval interval in intervals)
                {
                    interval.itIsInside(value);
                }
            }
            return intervals;

        }


        public void createIstogramVertical(Rectangle istogramSpace, Graphics g, int x, int y, int w, int h, List<Interval> intervals, int granularity, string text)
        {
            int step = (w / granularity);
            int max_value = 0;
            foreach (Interval i in intervals)
            {
                if (i.count > max_value)
                    max_value = i.count;
            }
            int X = x;
            Pen istoPen = new Pen(Color.Orange, 7);
            foreach (Interval interval in intervals)
            {
                double pct = double.Parse(interval.count.ToString()) / double.Parse(max_value.ToString());

                if (interval.count > 0)
                {
                    g.DrawLine(istoPen,
                               new Point(X, y + h),
                               new Point(X, y + h - (int)(pct * h))  // Use that percentage of the height
                    );
                    
                }
                X += step;
            }

            Rectangle stringPos = new Rectangle(istogramSpace.Left, istogramSpace.Top + 2 * (istogramSpace.Height / 10), istogramSpace.Width, istogramSpace.Height / 10);
            

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.g = Graphics.FromImage(this.b);
            this.g2 = Graphics.FromImage(this.b2);
            this.g3 = Graphics.FromImage(this.b3);
            this.g4 = Graphics.FromImage(this.b4);
            this.g5 = Graphics.FromImage(this.b5);

            this.g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.g.Clear(Color.Black);

            this.g2.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.g2.Clear(Color.Black);

            this.g3.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.g3.Clear(Color.Black);

            this.g4.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.g4.Clear(Color.Black);

            this.g5.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.g5.Clear(Color.Black);

            rect1 = new Rectangle(20, 20, this.b.Width - 40, this.b.Height - 40);
            rect2 = new Rectangle(20, 20, this.b2.Width - 40, this.b2.Height - 40);
            rect3 = new Rectangle(20, 20, this.b3.Width - 40, this.b3.Height - 40);
            rect4 = new Rectangle(20, 20, this.b4.Width - 40, this.b4.Height - 40);
            rect5 = new Rectangle(20, 20, this.b5.Width - 40, this.b5.Height - 40);
            g.DrawRectangle(Pens.Black, rect1);
            g2.DrawRectangle(Pens.Black, rect2);
            g3.DrawRectangle(Pens.Black, rect3);
            g4.DrawRectangle(Pens.Black, rect4);
            g5.DrawRectangle(Pens.Black, rect5);

            Random module = new Random();
            Random angle = new Random();
            List<double> normal = new List<double>();
            List<double> chisquared = new List<double>();
            List<double> cauchy = new List<double>();
            List<double> Ffisher = new List<double>();
            List<double> Tstudent = new List<double>();

            for (int i = 0; i < numberOfSamples; i++)
            {
                //Real Box-Muller-Transform with variance 1 and Mean 0
                double p_rand = Math.Sqrt(-2 * Math.Log(module.NextDouble()));
                double p_angle = angle.NextDouble() * 2 * Math.PI;
                double X = p_rand * Math.Cos(p_angle);
                double Y = p_rand * Math.Sin(p_angle);

                normal.Add(X);
                chisquared.Add(X * X);
                cauchy.Add(X / Y);
                Ffisher.Add((X * X) / (Y * Y));
                Tstudent.Add(X / Math.Sqrt((Y * Y)));
            }


            double cauchy_average = cauchy.Average();
            cauchy = cauchy.Where(x => (x >= cauchy_average - 50 && x <= cauchy_average + 50)).ToList();

            Ffisher = Ffisher.Where(x => (x <= 50)).ToList();
            Tstudent = Tstudent.Where(x => (x >= -50 && x <= 50)).ToList();

            List<Interval> normal_dist = double_intervals_creation(normal,30);
            List<Interval> chisquared_dist = double_intervals_creation(chisquared, 30);
            List<Interval> cauchy_dist = double_intervals_creation(cauchy, 30);
            List<Interval> Ffisher_dist = double_intervals_creation(Ffisher, 30);
            List<Interval> Tstudent_dist = double_intervals_creation(Tstudent, 30);
            this.createIstogramVertical(rect1, g, 20, 20, this.b.Width -20, this.b.Height -20, normal_dist,30, "Normal");
            this.createIstogramVertical(rect2, g2, 20, 20, this.b2.Width - 20, this.b2.Height - 20, chisquared_dist, 30, "Chi-Squared");
            this.createIstogramVertical(rect3, g3, 20, 20, this.b3.Width - 20, this.b3.Height - 20, cauchy_dist, 30, "cauchy");
            this.createIstogramVertical(rect4, g4, 20, 20, this.b4.Width - 20, this.b4.Height - 20, Ffisher_dist, 30, "fisher");
            this.createIstogramVertical(rect5, g5, 20, 20, this.b5.Width - 20, this.b5.Height - 20, Tstudent_dist, 30, "TStudent");

            pictureBox5.Image = b5;
            pictureBox4.Image = b4;
            pictureBox2.Image = b2;
            pictureBox3.Image = b3;
            pictureBox1.Image = b;
        }



    }
}