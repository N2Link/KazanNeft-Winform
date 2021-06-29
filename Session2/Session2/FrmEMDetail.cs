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
    public partial class FrmEMDetail : Form
    {
        private EmergencyMaintenance em;
        Session2Entities db = new Session2Entities();
        public FrmEMDetail()
        {
            InitializeComponent();
        }

        public FrmEMDetail(EmergencyMaintenance em)
        {
            InitializeComponent();

            this.em = db.EmergencyMaintenances.SingleOrDefault(p=>p.ID==em.ID);
        }

        private void FrmEMDetail_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            if(em.EMEndDate != null)
            {
                dateTimePicker2.Value =(DateTime) em.EMEndDate;
                dateTimePicker2.CustomFormat = "dd-MM-yyyy";
                dateTimePicker1.Enabled = false;
                button2.Enabled = false;
            }
            label2.Text = em.Asset.AssetSN;
            label4.Text = em.Asset.AssetName;
            label6.Text = em.Asset.DepartmentLocation.Department.Name;
            comboBox1.DataSource = db.Parts.ToList();
            comboBox1.DisplayMember = "Name";
            var listCP = em.ChangedParts.ToList();
            numericUpDown1.Minimum = 1;
            foreach (var item in listCP)
            {
                dataGridView1.Rows.Add(item.Part.Name, item.Amount, "Remove");
                dataGridView1.Rows[dataGridView1.RowCount - 1].Tag = item;
            }
            if(em.EMStartDate != null)
            {
                dateTimePicker1.Value = (DateTime)em.EMStartDate;
            }
            dateTimePicker1.MinDate = em.EMReportDate;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                if (dataGridView1.Rows[i].Cells[0].Value.ToString() == comboBox1.Text)
                {
                    var x = dataGridView1.Rows[i].Tag as ChangedPart;
                    x.Amount += numericUpDown1.Value;
                    dataGridView1.Rows[i].Cells[1].Value = x.Amount;
                    return;
                }
            }
            ChangedPart changedPart = new ChangedPart()
            {

                Amount = numericUpDown1.Value, PartID =(comboBox1.SelectedItem as Part ).ID, Part = comboBox1.SelectedItem as Part
            };
            dataGridView1.Rows.Add(comboBox1.Text, numericUpDown1.Value, "Remove");
            dataGridView1.Rows[dataGridView1.RowCount - 1].Tag = changedPart;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                var x = dataGridView1.Rows[i].Tag as ChangedPart;
                if (x.EmergencyMaintenanceID == 0)
                {
                    em.ChangedParts.Add(x);
                }
            }
            db.SaveChanges();
            em.DescriptionEmergency = textBox1.Text;
            if(dateTimePicker2.Enabled== true)
            {
                em.EMEndDate = dateTimePicker2.Value;
            }
            db.SaveChanges();
            MessageBox.Show("Susscessfull");
            this.Close();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if(dataGridView1.CurrentCell.Value.ToString()== "Remove")
            {
               var cp =  dataGridView1.CurrentRow.Tag as ChangedPart;
                if (cp.EmergencyMaintenanceID != 0)
                {
                    db.ChangedParts.Remove(db.ChangedParts.SingleOrDefault(p => p.ID == cp.ID));
                }
                dataGridView1.Rows.Remove(dataGridView1.CurrentRow);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

            string temp = textBox1.Text.Trim();
            if (temp != "")
            {
                dateTimePicker2.Enabled = true;
                dateTimePicker2.MinDate = dateTimePicker1.Value;
                dateTimePicker2.CustomFormat = "dd-MM-yyyy";
            }
            else
            {
                dateTimePicker2.Enabled = false;
                dateTimePicker2.MinDate = dateTimePicker1.Value;
                dateTimePicker2.CustomFormat = "";
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
