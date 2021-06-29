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
    public partial class FrmTranEdit : Form
    {
        private Order order;
        Session3Entities db = new Session3Entities();

        public FrmTranEdit()
        {
            InitializeComponent();
        }

        public FrmTranEdit(Order order)
        {
            InitializeComponent();
            this.order = db.Orders.SingleOrDefault(p=>p.ID == order.ID) ;
        }

        private void FrmTranEdit_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            comboBox1.DataSource = db.Warehouses.Where(p => p.ID == order.SourceWarehouseID).ToList();
            comboBox1.DisplayMember = "Name"; 

            comboBox2.DataSource = db.Warehouses.Where(p => p.ID == order.DestinationWarehouseID).ToList();
            comboBox2.DisplayMember = "Name";

            comboBox3.DataSource = db.OrderItems.Where(p=>p.Order.Warehouse1.Name == comboBox1.Text).Select(p=>p.Part).Distinct().ToList();
            comboBox3.DisplayMember = "Name";

            var listOrderItem = order.OrderItems.ToList();
            foreach (var item in listOrderItem)
            {
                dataGridView1.Rows.Add(item.Part.Name, item.BatchNumber, item.Amount, "Remove");
                dataGridView1.Rows[dataGridView1.RowCount - 1].Tag = item;
            }
            dateTimePicker1.Value = order.Date;
            numericUpDown1.Minimum = 1;
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox4.Text = "";

            if ((bool)(comboBox3.SelectedItem as Part).BatchNumberHasRequired)
            {
                comboBox4.Enabled = true;
                comboBox4.DataSource = db.OrderItems.Where(p => p.Part.Name == comboBox3.Text && p.Order.Warehouse1.Name == comboBox1.Text).ToList();
                comboBox4.DisplayMember = "Name";
            }
            else
            {
                comboBox4.Enabled = false;
            }
        }

        private decimal getCurrentStock(Warehouse warehouse, Part part, string batchnumber)
        {
            return getReceiveStock(warehouse, part, batchnumber) - getSentStock(warehouse, part, batchnumber);
        }

        private decimal getSentStock(Warehouse warehouse, Part part, string batchnumber)
        {
            decimal sent = 0;
            try
            {
                sent = db.OrderItems.Where(p => p.PartID == part.ID && p.Order.SourceWarehouseID == warehouse.ID && p.BatchNumber == batchnumber).Sum(p => p.Amount);
            }
            catch { };
            return sent;
        }

        private decimal getReceiveStock(Warehouse warehouse, Part part, string batchnumber)
        {
            decimal receive = 0;
            try
            {
                receive = db.OrderItems.Where(p => p.PartID == part.ID && p.Order.DestinationWarehouseID == warehouse.ID && p.BatchNumber == batchnumber).Sum(p => p.Amount);
            }
            catch { }
            return receive;
        }



        private void button1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                if (dataGridView1.Rows[i].Cells[0].Value.ToString() == comboBox3.Text && dataGridView1.Rows[i].Cells[1].Value.ToString() == comboBox4.Text)
                {
                    var x = dataGridView1.Rows[i].Tag as OrderItem;
                    if (getCurrentStock(comboBox1.SelectedItem as Warehouse, comboBox3.SelectedItem as Part, comboBox4.Text) - numericUpDown1.Value - x.Amount < 0)
                    {
                        MessageBox.Show("Cant add");
                        return;
                    }

                    x.Amount += numericUpDown1.Value;
                    dataGridView1.Rows[i].Cells[2].Value = x.Amount;
                    return;

                }
                if (getCurrentStock(comboBox1.SelectedItem as Warehouse, comboBox3.SelectedItem as Part, comboBox4.Text) - numericUpDown1.Value < 0)
                {
                    MessageBox.Show("Cant add");
                    return;
                }
            }

            OrderItem orderItem = new OrderItem()
            {
                Amount = numericUpDown1.Value,
                BatchNumber = comboBox4.Text,
                PartID = (comboBox3.SelectedItem as Part).ID

            };
            dataGridView1.Rows.Add(comboBox3.Text, comboBox4.Text, numericUpDown1.Value, "Remove");
            dataGridView1.Rows[dataGridView1.RowCount - 1].Tag = orderItem;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.CurrentCell.Value.ToString() == "Remove")
            {
                var x = dataGridView1.CurrentRow.Tag as OrderItem;
                if (getCurrentStock(comboBox2.SelectedItem as Warehouse, comboBox3.SelectedItem as Part, comboBox4.Text) - x.Amount < 0)
                {
                    MessageBox.Show("You cant remove");
                }
                else
                {
                    try
                    {
                        db.OrderItems.Remove(db.OrderItems.SingleOrDefault(p => p.ID == x.ID));
                    }
                    catch { };
                    dataGridView1.Rows.Remove(dataGridView1.CurrentRow);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                var x = dataGridView1.Rows[i].Tag as OrderItem;
                if (x.OrderID == 0)
                {
                    order.OrderItems.Add(x);
                }
            }
            db.SaveChanges();
            MessageBox.Show("Successful");
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
