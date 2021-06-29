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
    public partial class FrmIven : Form
    {
        Session4Entities db = new Session4Entities();
        public FrmIven()
        {
            InitializeComponent();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void FrmIven_Load(object sender, EventArgs e)
        {
            var listAssetEm = db.EmergencyMaintenances.Where(p => p.EMEndDate == null).ToList();
            List<Asset_EM> asset_EM = new List<Asset_EM>();
            foreach (var item in listAssetEm)
            {
                asset_EM.Add(new Asset_EM(item));
            }
            comboBox1.DataSource = asset_EM;
            comboBox1.DisplayMember = "Name";
            comboBox2.DataSource = db.Warehouses.ToList();
            comboBox2.DisplayMember = "Name";
            comboBox4.Items.Add("FIFO");
            comboBox4.Items.Add("LIFO");
            comboBox4.Items.Add("Minium First");
            comboBox4.SelectedIndex = 0;
            numericUpDown1.Minimum = 1;

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


        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            long id = (comboBox2.SelectedItem as Warehouse).ID;
            var lisrPart = db.OrderItems.Where(p => p.Order.DestinationWarehouseID == id).Select(p => p.Part).ToList();
            comboBox3.DataSource = lisrPart.Distinct().ToList();
            comboBox3.DisplayMember = "Name";

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
                sent = db.OrderItems.Where(p => p.PartID == part.ID && p.Order.Warehouse.ID == warehouse.ID && p.BatchNumber == batchnumber).Sum(p => p.Amount);
            }
            catch { };
            return sent;
        }

        private decimal getReceiveStock(Warehouse warehouse, Part part, string batchnumber)
        {
            decimal receive = 0;
            try
            {
                receive = db.OrderItems.Where(p => p.PartID == part.ID && p.Order.Warehouse1.ID == warehouse.ID && p.BatchNumber == batchnumber).Sum(p => p.Amount);
            }
            catch { }
            return receive;
        }


        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            numericUpDown1.Maximum = getCurrentStock(comboBox2.SelectedItem as Warehouse, comboBox3.SelectedItem as Part);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            long partID = (comboBox3.SelectedItem as Part).ID;
            long warehouseid = (comboBox2.SelectedItem as Warehouse).ID;
            var listbatchNumber = db.OrderItems.Where(p => p.Order.Warehouse1.ID ==  warehouseid && p.PartID ==partID ).GroupBy(p => p.BatchNumber).ToList();
            List<PartinWarehouse> list = new List<PartinWarehouse>();
            foreach (var item in listbatchNumber)
            {
               // getCurrentStock(comboBox2.SelectedItem as Warehouse, comboBox3.SelectedItem as Part, item.Key);

                var x = db.OrderItems.Where(p => p.Order.Warehouse1.ID == warehouseid && p.PartID == partID && p.BatchNumber == item.Key).ToList();
                decimal temp = (decimal)x.Last().UnitPrice;
                if (temp == 0) continue;
                if ( getCurrentStock(comboBox2.SelectedItem as Warehouse, comboBox3.SelectedItem as Part, item.Key) == 0) continue;
                list.Add(new PartinWarehouse
                {
                    part = comboBox3.SelectedItem as Part,
                    amount = getCurrentStock(comboBox2.SelectedItem as Warehouse, comboBox3.SelectedItem as Part, item.Key),
                    batchnumber = (string)item.Key,
                    unitPrice = temp
                });
/*                try
                {
                    list.Add(new PartinWarehouse
                    {
                        part = comboBox3.SelectedItem as Part,
                        amount = getCurrentStock(comboBox2.SelectedItem as Warehouse, comboBox3.SelectedItem as Part, item.Key),
                        batchnumber =(string) item.Key,
                        unitPrice = (decimal)db.OrderItems.Where(p => p.Order.Warehouse1.ID == warehouseid && p.PartID == partID && p.BatchNumber == item.Key).Last().UnitPrice
                    });
                }
                catch { };*/
            }
            decimal amount = numericUpDown1.Value;
            foreach (var item in list)
            {
                if (amount < item.amount)
                {
                    dataGridView1.Rows.Add(item.part.Name, item.batchnumber, item.unitPrice, amount);
                    return;

                }
                else
                {
                    dataGridView1.Rows.Add(item.part.Name, item.batchnumber, item.unitPrice, item.amount);
                    amount -= item.amount;
                }
            }
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int x = dataGridView2.RowCount;
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                dataGridView2.Rows.Add(dataGridView1.Rows[i].Cells[0].Value, dataGridView1.Rows[i].Cells[1].Value, dataGridView1.Rows[i].Cells[2].Value, dataGridView1.Rows[i].Cells[3].Value, "Remove");
                
            }
            dataGridView1.Rows.Clear();

        }

        private void button3_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Successfull");
            this.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
