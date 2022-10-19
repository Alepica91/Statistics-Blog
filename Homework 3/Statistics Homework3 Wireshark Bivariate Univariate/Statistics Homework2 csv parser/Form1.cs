using Microsoft.VisualBasic.FileIO;
using System;
using System.Data.Common;
using System.Diagnostics;
using System.Windows.Forms;
using System.Windows.Markup;

namespace Statistics_Homework2_csv_parser
{
    public partial class Form1 : Form
    {
        public List<string> Headers = new List<string>();
        public Form1()
        {
            InitializeComponent();
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullColumnSelect;
            textBox2.Multiline = true;
            textBox2.ScrollBars = ScrollBars.Both;

        }
        public static double Truncate(double value, int precision)
        {
            return Math.Truncate(value * Math.Pow(10, precision)) / Math.Pow(10, precision);
        }

        private void csvParserGrid() {
            dataGridView1.DataSource = null;
            dataGridView1.Columns.Clear();
            dataGridView1.Rows.Clear();
            var fileContent = string.Empty;
            var filePath = string.Empty;
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "csv files (*.csv)| *.csv";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    filePath = openFileDialog.FileName;
                    string[] filenamearray = filePath.ToString().Split('\\');
                    label5.Text = filenamearray[filenamearray.Length-1];

                    //Read the contents of the file into a stream
                    var fileStream = openFileDialog.OpenFile();

                    using (TextFieldParser parser = new TextFieldParser(fileStream))
                    {
                        DataGridView dataGrid1 = new DataGridView();
                        parser.TextFieldType = FieldType.Delimited;
                        parser.SetDelimiters(",");

                        //add columns
                        string[] headers = parser.ReadFields();
                        foreach (string f in headers) {
                            DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
                            column.Name = f;
                            column.SortMode = DataGridViewColumnSortMode.NotSortable;
                            column.Selected = false;
                            dataGridView1.Columns.Add(column);
                        }
                        //add rows
                        while (!parser.EndOfData)
                        {
                            string[] fields = parser.ReadFields();
                            dataGridView1.Rows.Add(fields);
                        }
                    }
                }
            }
        }
        private void stringColumnDetected(object sender, EventArgs e) {
            double output = 0;
            DataGridViewSelectedCellCollection cells = dataGridView1.SelectedCells;
            if (cells.Count <= 0) return;
            if (double.TryParse(cells[0].Value.ToString().Replace(".", ","), out output))
            {
                textBox1.Enabled = true;
            }
            else
            {
                textBox1.Enabled = false;
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            double output;
            int textBox;
            textBox2.Text = "";          
            
            List<double> values = new List<double>();
            List<String> valuesS = new List<string>();
            foreach (DataGridViewCell cell in dataGridView1.SelectedCells)
            {
                if(double.TryParse(cell.Value.ToString().Replace(".", ","),out output))
                {
                    if (cell.Value.ToString() != "" && cell.Value.ToString() != null)
                        values.Add(double.Parse(cell.Value.ToString().Replace(".", ",")));
                }
                else
                {
                    if (cell.Value.ToString() != "" && cell.Value.ToString() != null)
                        valuesS.Add(cell.Value.ToString());
                }
            }
            if (values.Count > 0 && valuesS.Count > 0)
            {
                MessageBox.Show("Impossible calculate distribution for selected column, different values type in the same column.");
                return;
            }
            else if (valuesS.Count > 0) {
                string_distribution(valuesS);

            }
            else
            {
                double_sistribution(values);
            }

        }

            private void button1_Click(object sender, EventArgs e)
        {
            csvParserGrid();
        }

        private void string_distribution(List<string> valuesS) {

            Dictionary<string, Interval> distributionResult = string_intervals_creation(valuesS);

            foreach (String key in distributionResult.Keys)
            {
                textBox2.Text += System.Environment.NewLine + (distributionResult[key]).ToStringStringNoCount() + Truncate((double.Parse(distributionResult[key].count.ToString()) / double.Parse(valuesS.Count.ToString())) * 100,2) + "%\n";
            }
        }

        private void double_sistribution(List<double> values) {
            double output;
            double granularity = 0;
            
            if (double.TryParse(textBox1.Text, out output))
            {
                granularity = Convert.ToDouble(textBox1.Text);
            }
            else {
                MessageBox.Show("Insert a valid Intervals number");
                return;
            }
                
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
                textBox2.Text += System.Environment.NewLine + interval.ToStringDoubleNoCount() + Truncate((double.Parse(interval.count.ToString())/double.Parse(values.Count.ToString()))*100,2)+"%";
            }

        }

        private List<Interval> double_intervals_creation(double granularity, List<double> values, double step) {
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

        private Dictionary<string, Interval> string_intervals_creation(List<string> valuesS) {
            Dictionary<string, Interval> distributionResult = new Dictionary<string, Interval>();
            foreach (string s in valuesS)
            {
                if (distributionResult.ContainsKey(s))
                    distributionResult[s].count++;
                else
                {
                    distributionResult.Add(s, new Interval(s, 1));
                }
            }
            return distributionResult;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var biVarForm = new Form2(dataGridView1);
            biVarForm.Show();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}