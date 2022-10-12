using System.Configuration;
using System.Security.Cryptography;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;
using Timer = System.Windows.Forms.Timer;

namespace Statistics_HW_2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Random rand = new();
        int timeleft = 30;

        private void button1_Click(object sender, EventArgs e)
        {
            timer1.Interval = 1000;
            timer1.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Stop();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            timeleft = 30;
            timervalue.Text = "" + timeleft;
            label1.Text = "";
        }

       
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (timeleft > 0)
            {
                timeleft -= 1;
                timervalue.Text = "" + timeleft;
                label1.Text = "" + rand.Next(100);
            }
            else
            {
                timer1.Stop();
            }
        }

    }
}