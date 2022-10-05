
using System.Diagnostics;


namespace Statistics_1_HW
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            long fib_number = long.Parse(textBox1.Text);
            if (fib_number <= 45)
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                label1.Text = "Result : " + fib(fib_number);
                stopwatch.Stop();
                var elapsed_ms = stopwatch.ElapsedMilliseconds;
                label3.Text = "Execution time in ms : "+ elapsed_ms;

            }
            else {
                label1.Text = "Result : ";
                label3.Text = "Execution time in ms : 0";
            }
            
        }

        private long fib(long value) {
            if (value <= 0)
            {
                return 0;
            }
            else if (value <= 2) {
                return 1;
            }else
            {
                return fib(value - 1) + fib(value - 2);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }


    }
}