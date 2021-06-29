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
    public partial class FrmAdmin : Form
    {
        Session2Entities db = new Session2Entities();
        public FrmAdmin()
        {
            InitializeComponent();
        }

        private void FrmAdmin_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            dataGridView1.Rows.Clear();
            var listEM = db.EmergencyMaintenances.Where(p => p.EMEndDate > DateTime.Now || p.EMEndDate == null).OrderBy(p => p.EMReportDate).OrderByDescending(p => p.PriorityID).ToList();
            foreach (var item in listEM)
            {
                dataGridView1.Rows.Add(item.Asset.AssetSN, item.Asset.AssetName, item.EMReportDate.ToString("dd-MM-yyyy"), item.Asset.Employee.FirstName + " " + item.Asset.Employee.LastName, item.Asset.DepartmentLocation.Department.Name);
                dataGridView1.Rows[dataGridView1.RowCount - 1].Tag = item;
                if (item.PriorityID == 2)
                {
                    dataGridView1.Rows[dataGridView1.RowCount - 1].DefaultCellStyle.BackColor = Color.Orange;

                }
                if (item.PriorityID == 3)
                {
                    dataGridView1.Rows[dataGridView1.RowCount - 1].DefaultCellStyle.BackColor = Color.Red;
                }

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var em = dataGridView1.CurrentRow.Tag as EmergencyMaintenance;
            FrmEMDetail f = new FrmEMDetail(em);
            this.Hide();
            f.ShowDialog();
            this.LoadData();
            this.Show();
        }
    }
}
