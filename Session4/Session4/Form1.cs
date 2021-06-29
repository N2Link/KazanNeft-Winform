using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Session4
{
    public partial class Form1 : Form
    {
        Session4Entities db = new Session4Entities();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var listOrderItem = db.OrderItems.Where(p => p.Order.EmergencyMaintenance.EMEndDate != null).OrderByDescending(p=>p.Order.Date).ToList();
            var listDate = listOrderItem.Select(p => p.Order.Date.ToString("yyyy-MM")).ToList();
            listDate = listDate.Distinct().ToList();
            var listDepartment = db.Departments.ToList();
            dataGridView1.ColumnCount = listDate.Count + 1;
            for (int i = 1; i < dataGridView1.ColumnCount; i++)
            {
                dataGridView1.Columns[i].HeaderText = listDate[i - 1];
            }
            int count = 0;
            foreach (var item in listDepartment)
            { 
                dataGridView1.Rows.Add(item.Name);
                for (int i = 1; i < dataGridView1.ColumnCount; i++)
                {
                    int year = Convert.ToInt32(listDate[i - 1].Substring(0, 4));
                    int month = Convert.ToInt32(listDate[i - 1].Substring(5));
                    int sum =(int) listOrderItem.Where(p => p.Order.Date.Month == month && p.Order.Date.Year == year && p.Order.EmergencyMaintenance.Asset.DepartmentLocation.Department.ID == item.ID).Sum(p=>p.Amount*p.UnitPrice);
                    dataGridView1.Rows[count].Cells[i].Value = sum;
                }
                count++;
            }

            for (int i = 1; i < dataGridView1.ColumnCount ; i++)
            {
                List<int> minmax = new List<int>();
                for (int j = 0; j < dataGridView1.RowCount; j++)
                {
                    int x = Convert.ToInt32(dataGridView1.Rows[j].Cells[i].Value.ToString());
                    if (x == 0) continue;
                    else
                    {
                        minmax.Add(x);
                    }
                }
                for (int j = 0; j < dataGridView1.RowCount; j++)
                {
                    if (Convert.ToInt32(dataGridView1.Rows[j].Cells[i].Value.ToString()) == minmax.Min())
                    {
                        dataGridView1.Rows[j].Cells[i].Style.BackColor = Color.Green;
                    }
                    if (Convert.ToInt32(dataGridView1.Rows[j].Cells[i].Value.ToString()) == minmax.Max())
                    {
                        dataGridView1.Rows[j].Cells[i].Style.BackColor = Color.Red;
                    }
                }
            }
            //pie
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                int sum = 0;
                for (int j = 1; j < dataGridView1.ColumnCount; j++)
                {
                    sum += Convert.ToInt32(dataGridView1.Rows[i].Cells[j].Value.ToString()); 
                }
                chart1.Series["s"].Points.AddXY(dataGridView1.Rows[i].Cells[0].Value.ToString(), sum);
            }
            //stack

            for (int i = 0; i < listDepartment.Count; i++)
            {
                chart2.Series.Add(new System.Windows.Forms.DataVisualization.Charting.Series() 
                {
                    ChartArea = "ChartArea1",
                ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.StackedColumn,
                Legend = "Legend1",
                Name = listDepartment[i].Name


            });

            }
            for (int i = 1; i < dataGridView1.ColumnCount; i++)
            {
                for (int j = 0; j < dataGridView1.RowCount; j++)
                {
                    chart2.Series[dataGridView1.Rows[j].Cells[0].Value.ToString()].Points.AddXY(dataGridView1.Columns[i].HeaderText, dataGridView1.Rows[j].Cells[i].Value);
                }
            }


            dataGridView2.ColumnCount = listDate.Count + 1;
            dataGridView2.Rows.Add("Highest Cost");
            dataGridView2.Rows.Add("Most Number");
            var listPart = db.Parts.ToList();
            for (int i = 1; i < dataGridView2.ColumnCount; i++)
            {
                dataGridView2.Columns[i].HeaderText = listDate[i - 1];
                int year = Convert.ToInt32(listDate[i - 1].Substring(0, 4));
                int month = Convert.ToInt32(listDate[i - 1].Substring(5));
                decimal maxCost = (decimal)listOrderItem.Where(p => p.PartID == listPart.First().ID && p.Order.Date.Month == month && p.Order.Date.Year == year).Sum(p => p.Amount * p.UnitPrice);
                decimal maxAmount = (decimal)listOrderItem.Where(p => p.PartID == listPart.First().ID && p.Order.Date.Month == month && p.Order.Date.Year == year).Sum(p => p.Amount);
                int posCost = 0;
                int posAmount = 0;
                int pos = 0;
                foreach (var item in listPart)
                {
                    var x = (decimal)listOrderItem.Where(p => p.PartID == item.ID && p.Order.Date.Month == month && p.Order.Date.Year == year).Sum(p => p.Amount * p.UnitPrice);
                    if (x > maxCost) { maxCost = x; posCost = pos; };
                    var z = (decimal)listOrderItem.Where(p => p.PartID == item.ID && p.Order.Date.Month == month && p.Order.Date.Year == year).Sum(p => p.Amount);
                    if (z > maxAmount) { maxAmount = x; posAmount = pos; };
                    pos++;
                }
                dataGridView2.Rows[0].Cells[i].Value = listPart[posCost].Name;
                dataGridView2.Rows[1].Cells[i].Value = listPart[posAmount].Name;

            }


            ///

            dataGridView3.ColumnCount = listDate.Count + 1;
            dataGridView3.Rows.Add("Asset");
            dataGridView3.Rows.Add("Department");
            var listAsset = db.Assets.ToList();
            for (int i = 1; i < dataGridView3.ColumnCount; i++)
            {
                dataGridView3.Columns[i].HeaderText = listDate[i - 1];
                int year = Convert.ToInt32(listDate[i - 1].Substring(0, 4));
                int month = Convert.ToInt32(listDate[i - 1].Substring(5));
                decimal maxCost = (decimal)listOrderItem.Where(p => p.Order.EmergencyMaintenance.AssetID == listAsset.First().ID && p.Order.Date.Month == month && p.Order.Date.Year == year).Sum(p => p.Amount * p.UnitPrice);
                int posCost = 0;
                int pos = 0;
                foreach (var item in listAsset)
                {
                    var x = (decimal)listOrderItem.Where(p => p.Order.EmergencyMaintenance.AssetID == item.ID && p.Order.Date.Month == month && p.Order.Date.Year == year).Sum(p => p.Amount * p.UnitPrice);
                    if (x > maxCost) { maxCost = x; posCost = pos; };
                    pos++;
                }
                dataGridView3.Rows[0].Cells[i].Value = listAsset[posCost].AssetName;
                dataGridView3.Rows[1].Cells[i].Value = listAsset[posCost].DepartmentLocation.Department.Name;

            }
            comboBox1.Items.Add("English");
            comboBox1.Items.Add("VietNamese");
            comboBox1.SelectedIndex = 0;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            FrmIven f = new FrmIven();
            this.Hide();
            f.ShowDialog();
            this.Close();            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(comboBox1.SelectedItem.ToString() != "English")
            {
                groupBox1.Text = "Tổng chi của từng bộ phận theo tháng";
                groupBox2.Text = "Bộ phận tốn chi phí/ được mua nhiều nhất mỗi tháng";
                groupBox3.Text = "Tài sản tốn nhiều chi phí nhất theo từng tháng";
            }
            else
            {
                groupBox1.Text = "EM spending  by Department";
                groupBox2.Text = "Monthly report for Most-USed Parts";
                groupBox3.Text = "Monthly Report of Costly Assets";
            }
        }
    }
}
