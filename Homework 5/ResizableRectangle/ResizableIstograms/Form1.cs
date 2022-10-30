using System.Windows.Forms;

namespace ResizableRectangle
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Bitmap b;
        Graphics g;

        int x_down;
        int y_down;

        int x_mouse;
        int y_mouse;

        int r_width;
        int r_height;

        Rectangle r;

        bool drag = false;
        bool resizing = false;

        private void button1_Click(object sender, EventArgs e)
        {
            b = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            g = Graphics.FromImage(b);

            int rbase = 0;
            int raltezza = 0;
            if (!int.TryParse(textBox1.Text, out rbase))
                MessageBox.Show("Set an integer value for the rectangle base");
            if (!int.TryParse(textBox2.Text, out raltezza))
                MessageBox.Show("Set an integer value for the rectangle height");

            r = new Rectangle(20, 20, rbase, raltezza);

            g.DrawRectangle(Pens.Black, r);
            pictureBox1.Image = b;
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (r.Contains(e.X, e.Y))
            {
                x_mouse = e.X;
                y_mouse = e.Y;

                x_down = r.X;
                y_down = r.Y;

                r_width = r.Width;
                r_height = r.Height;

                if (e.Button == MouseButtons.Left)
                {
                    drag = true;
                }
                else if (e.Button == MouseButtons.Right)
                {
                    resizing = true;
                }
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            drag = false;
            resizing = false;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            int delta_x = e.X - x_mouse;
            int delta_y = e.Y - y_mouse;

            if (r != null)
            {
                if (drag)
                {

                    r.X = x_down + delta_x;
                    r.Y = y_down + delta_y;

                    draw(r, g);
                }
                else if (resizing)
                {
                    r.Width = r_width + delta_x;
                    r.Height = r_height + delta_y;

                    draw(r, g);
                }
            }
        }

        private void draw(Rectangle r, Graphics g)
        {
            g.Clear(Color.White);

            g.DrawRectangle(Pens.Black, r);
            pictureBox1.Image = b;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}