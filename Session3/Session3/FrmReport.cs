using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Session3
{
    public partial class FrmReport : Form
    {
        Session3Entities db = new Session3Entities();
        public FrmReport()
        {
            InitializeComponent();
        }

        private void FrmReport_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            comboBox1.DataSource = db.Warehouses.ToList();
            comboBox1.DisplayMember = "Name";
        }

        private decimal getCurrentStock(Warehouse warehouse, Part part)
        {
            return getReceiveStock(warehouse, part) - getSentStock(warehouse, part);
        }

        private decimal getSentStock(Warehouse warehouse, Part part)
        {
            decimal sent = 0;
            try
            {
                sent = db.OrderItems.Where(p => p.PartID == part.ID && p.Order.Warehouse.ID == warehouse.ID).Sum(p => p.Amount);
            }
            catch { };
            return sent;
        }

        private decimal getReceiveStock(Warehouse warehouse, Part part)
        {
            decimal receive = 0;
            try
            {
                receive = db.OrderItems.Where(p => p.PartID == part.ID && p.Order.Warehouse1.ID == warehouse.ID).Sum(p => p.Amount);
            }
            catch { }
            return receive;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            var listPart = db.Parts.ToList();
            foreach (var item in listPart)
            {
                dataGridView1.Rows.Add(item.Name, getCurrentStock(comboBox1.SelectedItem as Warehouse, item), getReceiveStock(comboBox1.SelectedItem as Warehouse, item),"");
                if ((bool)item.BatchNumberHasRequired)
                {
                    dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[3].Value = "View Batch Number";
                }
                dataGridView1.Rows[dataGridView1.RowCount - 1].Tag = item;
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if(radioButton1.Checked== true)
            {
                dataGridView1.Columns[2].Visible = false;
                for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    if (dataGridView1.Rows[i].Cells[1].Value.ToString() == "0.00")
                    {
                        dataGridView1.Rows[i].Visible = false;
                    }
                }
            }
            else
            {
                dataGridView1.Columns[2].Visible = true;
                for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    dataGridView1.Rows[i].Visible = true;
                }
            }

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked == true)
            {
                dataGridView1.Columns[1].Visible = false;
            }
            else
            {
                dataGridView1.Columns[1].Visible = true;
            }
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3.Checked == true)
            {
                dataGridView1.Columns[2].Visible = false;
                dataGridView1.Columns[1].Visible = false;

                for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    if (dataGridView1.Rows[i].Cells[1].Value.ToString() != "0.00")
                    {
                        dataGridView1.Rows[i].Visible = false;
                    }
                }
            }
            else
            {
                dataGridView1.Columns[2].Visible = true;
                dataGridView1.Columns[1].Visible = true;
                for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    dataGridView1.Rows[i].Visible = true;
                }
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            List<string> listBatchNumber = new List<string>();
            long id = (comboBox1.SelectedItem as Warehouse).ID;
            if (dataGridView1.CurrentCell.Value.ToString() == "View Batch Number")
            {
                var x = dataGridView1.CurrentRow.Tag as Part;
                listBatchNumber = db.OrderItems.Where(p => p.PartID == x.ID && p.Order.Warehouse1.ID ==id).Select(p=>p.BatchNumber).Distinct().ToList();
                string mess = "";
                foreach (var item in listBatchNumber)
                {
                    mess += "\n" + item;
                }
                MessageBox.Show(mess);
            }

        }
    }
}
