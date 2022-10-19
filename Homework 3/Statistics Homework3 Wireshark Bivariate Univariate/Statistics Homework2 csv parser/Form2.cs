using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Statistics_Homework2_csv_parser
{
    public partial class Form2 : Form
    {
        public List<string> comboBoxItems = new List<string>();
        public List<DataGridViewColumn> dg_columns = new List<DataGridViewColumn>();
        public DataGridView dataGridViewCp = new DataGridView();
        public int comboBox1ColumnIndex = -1;
        public int comboBox2ColumnIndex = -1;
        public List<DataGridViewCell> firstHalfCells = new List<DataGridViewCell>();
        public List<DataGridViewCell> secondHalfCells = new List<DataGridViewCell>();

        public Form2()
        {
            InitializeComponent();
        }

        public Form2(DataGridView dataGridViewCp)
        {
            InitializeComponent();
            this.dataGridViewCp = dataGridViewCp;

            foreach (DataGridViewColumn col in dataGridViewCp.Columns) {
                col.Selected = false;
                comboBox1.Items.Add(col.Name);
                comboBox2.Items.Add(col.Name);
                comboBoxItems.Add(col.Name);
                dg_columns.Add(col);    
            }
            textBox1.Enabled = false;
            textBox2.Enabled = false;

        }

        public static double Truncate(double value, int precision)
        {
            return Math.Truncate(value * Math.Pow(10, precision)) / Math.Pow(10, precision);
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

        private Dictionary<string, Interval> string_intervals_creation(List<string> valuesS)
        {
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

        public void comboBox1SelectedChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem != null)
            {
                foreach (DataGridViewColumn col in dataGridViewCp.Columns)
                {
                    if (col.Name.Equals(comboBox1.SelectedItem.ToString()))
                    {
                        double output = 0;
                        comboBox1ColumnIndex = col.Index;
                        col.Selected = true;

                        firstHalfCells.Clear();
                        foreach (DataGridViewCell cell in dataGridViewCp.SelectedCells)
                            firstHalfCells.Add(cell);

                        if (double.TryParse(dataGridViewCp.SelectedCells[0].Value.ToString().Replace(".", ","), out output))
                        {
                            textBox1.Enabled = true;
                        }
                        else
                        {
                            textBox1.Enabled = false;
                        }
                        dataGridViewCp.ClearSelection();
                    }
                }
            }
        }

        public void comboBox2SelectedChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedItem != null) {
                foreach (DataGridViewColumn col in dataGridViewCp.Columns)
                {
                    if (col.Name.Equals(comboBox2.SelectedItem.ToString()))
                    {
                        double output = 0;
                        comboBox2ColumnIndex = col.Index;
                        col.Selected = true;

                        secondHalfCells.Clear();
                        foreach(DataGridViewCell cell in dataGridViewCp.SelectedCells)
                            secondHalfCells.Add(cell);

                        if (double.TryParse(dataGridViewCp.SelectedCells[0].Value.ToString().Replace(".", ","), out output))
                        {
                            textBox2.Enabled = true;
                        }
                        else
                        {
                            textBox2.Enabled = false;
                        }
                        dataGridViewCp.ClearSelection();
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1ColumnIndex == comboBox2ColumnIndex) {
                MessageBox.Show("You need to select 2 DIFFERENT variables to create a Bivariate Distribution");
                return;
            }
            bivariate.DataSource = null;
            bivariate.Columns.Clear();
            bivariate.Rows.Clear();

            double output = 0;

            List<string> firstHalfListString = new List<string>();
            List<string> secondHalfListString = new List<string>();
            List<double> firstHalfListDouble = new List<double>();
            List<double> secondHalfListDouble = new List<double>();

            foreach (DataGridViewCell cell in firstHalfCells)
            {
                if (double.TryParse(cell.Value.ToString().Replace(".", ","), out output))
                {
                    firstHalfListDouble.Add(double.Parse(cell.Value.ToString().Replace(".", ",")));
                }
                else
                {
                    firstHalfListString.Add(cell.Value.ToString());
                }
            }

            foreach (DataGridViewCell cell in secondHalfCells)
            {
                if (double.TryParse(cell.Value.ToString().Replace(".", ","), out output))
                {
                    secondHalfListDouble.Add(double.Parse(cell.Value.ToString().Replace(".", ",")));
                }
                else
                {
                    secondHalfListString.Add(cell.Value.ToString());
                }
            }
            if (firstHalfListDouble.Count > 0 && secondHalfListDouble.Count > 0)
            {
                bivariate_matrix_creation(comboBoxItems[firstHalfCells[0].ColumnIndex], firstHalfListDouble, comboBoxItems[secondHalfCells[0].ColumnIndex], secondHalfListDouble);
            }
            else if(firstHalfListString.Count > 0 && secondHalfListString.Count > 0)
            {
                bivariate_matrix_creation(comboBoxItems[firstHalfCells[0].ColumnIndex], firstHalfListString, comboBoxItems[secondHalfCells[0].ColumnIndex], secondHalfListString);
            }
            else if (firstHalfListDouble.Count > 0 && secondHalfListString.Count > 0)
            {
                bivariate_matrix_creation(comboBoxItems[firstHalfCells[0].ColumnIndex], firstHalfListDouble, comboBoxItems[secondHalfCells[0].ColumnIndex], secondHalfListString);
            }
            else
            {
                bivariate_matrix_creation(comboBoxItems[firstHalfCells[0].ColumnIndex], firstHalfListString, comboBoxItems[secondHalfCells[0].ColumnIndex], secondHalfListDouble);
            }
        }

        private void bivariate_matrix_creation(string header1, List<string> firstHalf, string header2, List<string> secondHalf) { 
            Dictionary<string, Interval> intervals1 = new Dictionary<string, Interval>();
            Dictionary<string, Interval> intervals2 = new Dictionary<string, Interval>();
            
            intervals1 = string_intervals_creation(firstHalf);
            intervals2 = string_intervals_creation(secondHalf);
    
            foreach (string key in intervals1.Keys) {
                DataGridViewTextBoxColumn c = new DataGridViewTextBoxColumn();
                //c.HeaderText = firstHalf.Keys.First();
                c.Name = key;
                c.SortMode = DataGridViewColumnSortMode.NotSortable;
                c.Selected = false;
                bivariate.Columns.Add(c);
            }
            foreach (string key in intervals2.Keys)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(this.bivariate);
                row.HeaderCell.Value = $""+key;
                this.bivariate.Rows.Add(row);
                this.bivariate.AllowUserToAddRows = false;
                this.bivariate.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            }

            Dictionary<(string,string), int> matrix_value = new Dictionary<(string,string), int>();

            for (int index = 0; index < firstHalf.Count(); index++){
                    if (!matrix_value.ContainsKey((firstHalf[index], secondHalf[index])))
                        matrix_value.Add((firstHalf[index], secondHalf[index]), 1);
                    else
                        matrix_value[(firstHalf[index], secondHalf[index])]++;
                
            }
            foreach(DataGridViewRow r in bivariate.Rows){
                string rh = r.HeaderCell.Value.ToString();
                foreach (DataGridViewCell cell in r.Cells) {
                    if(matrix_value.ContainsKey((cell.OwningColumn.Name, rh)))
                        cell.Value = Truncate(((double.Parse(matrix_value[(cell.OwningColumn.Name, rh)].ToString())/firstHalf.Count)*100),2).ToString()+"%";
                    else
                        cell.Value = "0%";
                }
            }

        }

        private void bivariate_matrix_creation(string header1, List<double> firstHalf, string header2, List<string> secondHalf)
        {
            List<Interval> intervals1 = new List<Interval>();
            Dictionary<string, Interval> intervals2 = new Dictionary<string, Interval>();

            double output;
            double granularity = 0;

            if (double.TryParse(textBox1.Text, out output))
            {
                granularity = Convert.ToDouble(textBox1.Text);
            }
            else
            {
                MessageBox.Show("Insert a valid Intervals number");
                return;
            }

            double step = (firstHalf.Max() - firstHalf.Min()) / granularity;

            intervals1 = double_intervals_creation(granularity, firstHalf, step);
            intervals2 = string_intervals_creation(secondHalf);
            Dictionary<string, Interval> interval_by_interval_name = new Dictionary<string, Interval>();
            foreach (Interval dInterval in intervals1)
            {
                DataGridViewTextBoxColumn c = new DataGridViewTextBoxColumn();
                //c.HeaderText = firstHalf.Keys.First();
                c.Name = dInterval.ToStringDoubleNoCount();
                interval_by_interval_name.Add(dInterval.ToStringDoubleNoCount(), dInterval);
                c.SortMode = DataGridViewColumnSortMode.NotSortable;
                c.Selected = false;
                bivariate.Columns.Add(c);
            }
            foreach (string key in intervals2.Keys)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(this.bivariate);
                row.HeaderCell.Value = $"" + key;
                this.bivariate.Rows.Add(row);
                this.bivariate.AllowUserToAddRows = false;
                this.bivariate.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            }

            
            Dictionary<(double, string), int> matrix_value = new Dictionary<(double, string), int>();
            Dictionary<(string,string), int> assoc_values = new Dictionary<(string, string), int>();
            for (int index = 0; index < firstHalf.Count(); index++)
            {
                if (!matrix_value.ContainsKey((firstHalf[index], secondHalf[index]))) {
                    matrix_value.Add((firstHalf[index], secondHalf[index]), 1);
                }
                else
                    matrix_value[(firstHalf[index], secondHalf[index])]++;
            }
            //SIAMO QUI
            foreach((double,string) key in matrix_value.Keys) {
                foreach (string s in interval_by_interval_name.Keys) {
                    if (interval_by_interval_name[s].itIsInsideNoCount(key.Item1)) {
                        if (!assoc_values.ContainsKey((s, key.Item2)))
                        {
                            assoc_values.Add((s, key.Item2), matrix_value[key]);
                            break;
                        }
                        else {
                            assoc_values[(s, key.Item2)]++;
                            break;
                        }
                            
                    }
                }
            }

            foreach (DataGridViewRow r in bivariate.Rows)
            {
                string rh = r.HeaderCell.Value.ToString();
                foreach (DataGridViewCell cell in r.Cells)
                {
                    if(assoc_values.ContainsKey((cell.OwningColumn.Name, rh)))
                        cell.Value = Truncate(((double.Parse(assoc_values[(cell.OwningColumn.Name, rh)].ToString()) / firstHalf.Count) * 100), 2).ToString() + "%";
                    else
                        cell.Value = "0%";
                }
            }
            
        }

        private void bivariate_matrix_creation(string header1, List<double> firstHalf, string header2, List<double> secondHalf)
        {
            List<Interval> intervals2 = new List<Interval>();
            List<Interval> intervals1 = new List<Interval>();

            double output;
            double granularity1 = 0;
            double granularity2 = 0;
            if (double.TryParse(textBox1.Text, out output))
            {
                granularity1 = Convert.ToDouble(textBox1.Text);
            }
            else
            {
                MessageBox.Show("Insert a valid Interval number for Variable 1");
                return;
            }
            if (double.TryParse(textBox2.Text, out output))
            {
                granularity2 = Convert.ToDouble(textBox2.Text);
            }
            else
            {
                MessageBox.Show("Insert a valid Interval number for Variable 2");
                return;
            }
            double step1 = (firstHalf.Max() - firstHalf.Min()) / granularity1;
            double step2 = (secondHalf.Max() - secondHalf.Min()) / granularity2;

            intervals2 = double_intervals_creation(granularity2, secondHalf, step2);
            intervals1 = double_intervals_creation(granularity1, firstHalf, step1);
            Dictionary<string, Interval> interval_by_interval_name_1 = new Dictionary<string, Interval>();
            Dictionary<string, Interval> interval_by_interval_name_2 = new Dictionary<string, Interval>();
            foreach (Interval dInterval1 in intervals1)
            {
                DataGridViewTextBoxColumn c = new DataGridViewTextBoxColumn();
                c.Name = dInterval1.ToStringDoubleNoCount();
                interval_by_interval_name_1.Add(dInterval1.ToStringDoubleNoCount(), dInterval1);
                c.SortMode = DataGridViewColumnSortMode.NotSortable;
                c.Selected = false;
                bivariate.Columns.Add(c);
            }
            foreach (Interval dInterval2 in intervals2)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(this.bivariate);
                row.HeaderCell.Value = $"" + dInterval2.ToStringDoubleNoCount();
                interval_by_interval_name_2.Add(dInterval2.ToStringDoubleNoCount(), dInterval2);
                this.bivariate.Rows.Add(row);
                this.bivariate.AllowUserToAddRows = false;
                this.bivariate.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            }


            Dictionary<(double, double), int> matrix_value = new Dictionary<(double, double), int>();
            Dictionary<(string, string), int> assoc_values = new Dictionary<(string, string), int>();
            for (int index = 0; index < firstHalf.Count(); index++)
            {
                if (!matrix_value.ContainsKey((firstHalf[index], secondHalf[index])))
                {
                    matrix_value.Add((firstHalf[index], secondHalf[index]), 1);
                }
                else
                    matrix_value[(firstHalf[index], secondHalf[index])]++;
            }
            //SIAMO QUI
            foreach ((double, double) key in matrix_value.Keys)
            {
                foreach (string s in interval_by_interval_name_1.Keys)
                {
                    if (interval_by_interval_name_1[s].itIsInsideNoCount(key.Item1))
                    {
                        foreach (string s2 in interval_by_interval_name_2.Keys) 
                        {
                            if (interval_by_interval_name_2[s2].itIsInsideNoCount(key.Item2))
                            {
                                if (!assoc_values.ContainsKey((s,s2)))
                                {
                                    assoc_values.Add((s,s2), matrix_value[key]);
                                    break;
                                }
                                else
                                {
                                    assoc_values[(s, s2)]++;
                                    break;
                                }

                            }
                        }
                        break;
                    }
                }
            }

            foreach (DataGridViewRow r in bivariate.Rows)
            {
                string rh = r.HeaderCell.Value.ToString();
                foreach (DataGridViewCell cell in r.Cells)
                {
                    if (assoc_values.ContainsKey((cell.OwningColumn.Name, rh)))
                        cell.Value = Truncate(((double.Parse(assoc_values[(cell.OwningColumn.Name, rh)].ToString()) / firstHalf.Count) * 100), 2).ToString() + "%";
                    else
                        cell.Value = "0%";
                }
            }

        }

        private void bivariate_matrix_creation(string header1, List<string> firstHalf, string header2 , List<double> secondHalf)
        {
            List<Interval> intervals2 = new List<Interval>();
            Dictionary<string, Interval> intervals1 = new Dictionary<string, Interval>();

            double output;
            double granularity = 0;

            if (double.TryParse(textBox2.Text, out output))
            {
                granularity = Convert.ToDouble(textBox2.Text);
            }
            else
            {
                MessageBox.Show("Insert a valid Intervals number");
                return;
            }

            double step = (secondHalf.Max() - secondHalf.Min()) / granularity;

            intervals2 = double_intervals_creation(granularity, secondHalf, step);
            intervals1 = string_intervals_creation(firstHalf);
            Dictionary<string, Interval> interval_by_interval_name = new Dictionary<string, Interval>();
            foreach (string key in intervals1.Keys)
            {
                DataGridViewTextBoxColumn c = new DataGridViewTextBoxColumn();
                //c.HeaderText = firstHalf.Keys.First();
                c.Name = key;
                c.SortMode = DataGridViewColumnSortMode.NotSortable;
                c.Selected = false;
                bivariate.Columns.Add(c);
            }
            foreach(Interval dInterval in intervals2)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(this.bivariate);
                row.HeaderCell.Value = $"" + dInterval.ToStringDoubleNoCount();
                interval_by_interval_name.Add(dInterval.ToStringDoubleNoCount(), dInterval);
                this.bivariate.Rows.Add(row);
                this.bivariate.AllowUserToAddRows = false;
                this.bivariate.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            }


            Dictionary<(string, double), int> matrix_value = new Dictionary<(string, double), int>();
            Dictionary<(string, string), int> assoc_values = new Dictionary<(string, string), int>();
            for (int index = 0; index < firstHalf.Count(); index++)
            {
                if (!matrix_value.ContainsKey((firstHalf[index], secondHalf[index])))
                {
                    matrix_value.Add((firstHalf[index], secondHalf[index]), 1);
                }
                else
                    matrix_value[(firstHalf[index], secondHalf[index])]++;
            }
            //SIAMO QUI
            foreach ((string, double) key in matrix_value.Keys)
            {
                foreach (string s in interval_by_interval_name.Keys)
                {
                    if (interval_by_interval_name[s].itIsInsideNoCount(key.Item2))
                    {
                        if (!assoc_values.ContainsKey((key.Item1,s)))
                        {
                            assoc_values.Add((key.Item1,s), matrix_value[key]);
                            break;
                        }
                        else
                        {
                            assoc_values[(key.Item1, s)]++;
                            break;
                        }

                    }
                }
            }

            foreach (DataGridViewRow r in bivariate.Rows)
            {
                string rh = r.HeaderCell.Value.ToString();
                foreach (DataGridViewCell cell in r.Cells)
                {
                    if (assoc_values.ContainsKey((cell.OwningColumn.Name, rh)))
                        cell.Value = Truncate(((double.Parse(assoc_values[(cell.OwningColumn.Name, rh)].ToString()) / firstHalf.Count) * 100), 2).ToString() + "%";
                    else
                        cell.Value = "0%";
                }
            }

        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }
    }
}
