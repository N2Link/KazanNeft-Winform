using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Session2
{
    public partial class FrmUser : Form
    {
        private Employee employee;

        Session2Entities db = new Session2Entities();
        public FrmUser()
        {
            InitializeComponent();
        }

        public FrmUser(Employee employee)
        {
            InitializeComponent();
            this.employee = db.Employees.SingleOrDefault(p=>p.ID==employee.ID);
        }

        private void FrmUser_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            dataGridView1.Rows.Clear();
            var listAsset = employee.Assets.ToList();
            foreach (var item in listAsset)
            {
                var listEM = item.EmergencyMaintenances.ToList();
                if(listEM.Count == 0)
                {
                    dataGridView1.Rows.Add(item.AssetSN, item.AssetName, "--", "0");
                }
                else
                {
                    if (listEM.Last().EMEndDate == null || listEM.Last().EMEndDate>DateTime.Now)
                    {
                        dataGridView1.Rows.Add(item.AssetSN, item.AssetName, "--", listEM.Count-1);
                        dataGridView1.Rows[dataGridView1.RowCount - 1].DefaultCellStyle.BackColor = Color.Red;
                    }
                    else if (listEM.Last().EMEndDate != null || listEM.Last().EMEndDate < DateTime.Now)
                    {
                        dataGridView1.Rows.Add(item.AssetSN, item.AssetName, ((DateTime)listEM.Last().EMEndDate).ToString("dd-MM-yyyy"), listEM.Count);

                    }
                }
                dataGridView1.Rows[dataGridView1.RowCount - 1].Tag = item;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow.DefaultCellStyle.BackColor != Color.Red)
            {
                var aseet = dataGridView1.CurrentRow.Tag as Asset;
                FrmEMRequest f = new FrmEMRequest(aseet);
                this.Hide();
                f.ShowDialog();
                this.LoadData();
                this.Show();
            }
            else
            {
                MessageBox.Show("This asset is ongoing Em");
                return;
            }
        }
    }
}
