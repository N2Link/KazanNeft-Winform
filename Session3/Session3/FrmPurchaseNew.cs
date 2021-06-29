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
    public partial class FrmPurchaseNew : Form
    {
        Session3Entities db = new Session3Entities();
        public FrmPurchaseNew()
        {
            InitializeComponent();
        }

        private void FrmPurchaseNew_Load(object sender, EventArgs e)
        {
            LoadData();

        }

        private void LoadData()
        {
            comboBox1.DataSource = db.Suppliers.ToList();
            comboBox1.DisplayMember = "Name";
            comboBox2.DataSource = db.Warehouses.ToList();
            comboBox2.DisplayMember = "Name";
            comboBox3.DataSource = db.Parts.ToList();
            comboBox3.DisplayMember = "Name";
            dateTimePicker1.MinDate = DateTime.Now;
            numericUpDown1.Minimum = 1;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                if (dataGridView1.Rows[i].Cells[0].Value.ToString() == comboBox3.Text && dataGridView1.Rows[i].Cells[1].Value.ToString() == textBox1.Text)
                {
                    var x = dataGridView1.Rows[i].Tag as OrderItem;
                    x.Amount += numericUpDown1.Value;
                    dataGridView1.Rows[i].Cells[2].Value = x.Amount;
                    return;
                }
            }
            OrderItem orderItem = new OrderItem()
            {
                Amount = numericUpDown1.Value,
                PartID = (comboBox3.SelectedItem as Part).ID,
                Part = comboBox3.SelectedItem as Part,
                BatchNumber = textBox1.Text
            };
            dataGridView1.Rows.Add(comboBox3.Text, textBox1.Text, numericUpDown1.Value, "Remove");
            dataGridView1.Rows[dataGridView1.RowCount - 1].Tag = orderItem;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.CurrentCell.Value.ToString() == "Remove")
            {
                dataGridView1.Rows.Remove(dataGridView1.CurrentRow);
            }
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox1.Text = "";
            if ((bool)(comboBox3.SelectedItem as Part).BatchNumberHasRequired)
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
            if (dataGridView1.RowCount == 0)
            {
                MessageBox.Show("List part Empty");
                return;
            }
            Order order = new Order()
            {
                Date = dateTimePicker1.Value,
                TransactionTypeID = 1,
                SupplierID = (comboBox1.SelectedItem as Supplier).ID,
                DestinationWarehouseID = (comboBox2.SelectedItem as Warehouse).ID,

            };
            db.Orders.Add(order);
            db.SaveChanges();
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                var x = dataGridView1.Rows[i].Tag as OrderItem;
                order.OrderItems.Add(x);
            }
            db.SaveChanges();
            MessageBox.Show("Susscessful");
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
