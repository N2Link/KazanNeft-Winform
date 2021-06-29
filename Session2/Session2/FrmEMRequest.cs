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
    public partial class FrmEMRequest : Form
    {
        private Asset asset;
        Session2Entities db = new Session2Entities();
        public FrmEMRequest()
        {
            InitializeComponent();
        }

        public FrmEMRequest(Asset asset)
        {
            InitializeComponent();

            this.asset = db.Assets.SingleOrDefault(p=>p.ID==asset.ID);
        }

        private void FrmEMRequest_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            comboBox1.DataSource = db.Priorities.ToList();
            comboBox1.DisplayMember = "Name";
            label2.Text = asset.AssetSN;
            label4.Text = asset.AssetName;
            label6.Text = asset.DepartmentLocation.Department.Name;


        }

        private void button1_Click(object sender, EventArgs e)
        {
            string temp = textBox1.Text.Trim();
            if (temp == "")
            {
                MessageBox.Show("Have to fill description");
                return;
            }
            EmergencyMaintenance em = new EmergencyMaintenance()
            {
                AssetID = asset.ID,
                EMReportDate = DateTime.Now,
                OtherConsiderations = textBox2.Text,
                DescriptionEmergency = textBox1.Text,
                PriorityID = (comboBox1.SelectedItem as Priority).ID,
            };
            db.EmergencyMaintenances.Add(em);
            db.SaveChanges();
            MessageBox.Show("Your Em is sent");
            this.Close();
        }
    }
}
