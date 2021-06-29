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
    public partial class Form1 : Form
    {
        Session3Entities db = new Session3Entities();
        public Form1()
        {
            InitializeComponent();
        }

        private void purchaseOrderManagementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmPurchaseNew f = new FrmPurchaseNew();
            this.Hide();
            f.ShowDialog();
            this.LoadData();
            this.Show();
        }

        private void warehouseManagementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmTranNew f = new FrmTranNew();
            this.Hide();
            f.ShowDialog();
            this.LoadData();
            this.Show();
        }

        private void inventoryReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmReport f = new FrmReport();
            this.Hide();
            f.ShowDialog();
            this.LoadData();
            this.Show();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            dataGridView1.Rows.Clear();
            var listOrder = db.Orders.ToList().OrderBy(p => p.Date);
            foreach (var item in listOrder)
            {
                var listOrderItem = item.OrderItems.ToList();
                foreach (var oi in listOrderItem)
                {
                    if (item.TransactionTypeID == 1)
                    {
                        dataGridView1.Rows.Add(oi.Part.Name, item.TransactionType.Name, item.Date.ToString("dd-MM-yyyy"), oi.Amount, item.Supplier.Name, item.Warehouse1.Name, "Edit", "Remove" );
                        dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[3].Style.BackColor = Color.Green;
                    }
                    else
                    {
                        dataGridView1.Rows.Add(oi.Part.Name, item.TransactionType.Name, item.Date.ToString("dd-MM-yyyy"), oi.Amount, item.Warehouse.Name, item.Warehouse1.Name, "Edit", "Remove");
                    }
                    dataGridView1.Rows[dataGridView1.RowCount - 1].Tag = item;
                    dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[7].Tag = oi;
                }
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if(dataGridView1.CurrentCell.Value.ToString() == "Edit")
            {
                var order = dataGridView1.CurrentRow.Tag as Order;
                if (order.TransactionTypeID == 1)
                {
                    FrmPurchaseEdit f = new FrmPurchaseEdit(order);
                    this.Hide();
                    f.ShowDialog(); 
                    this.LoadData(); 
                    this.Show();
                }
                else
                {
                    FrmTranEdit f = new FrmTranEdit(order);
                    this.Hide();
                    f.ShowDialog();
                    this.LoadData();
                    this.Show();
                }
            }
            if (dataGridView1.CurrentCell.Value.ToString() == "Remove")
            {
                var x = dataGridView1.CurrentCell.Tag as OrderItem;
                db.OrderItems.Remove(x);
                dataGridView1.Rows.Remove(dataGridView1.CurrentRow);
            }
            db.SaveChanges();

        }
    }
}
