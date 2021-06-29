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
    public partial class Form1 : Form
    {
        Session2Entities db = new Session2Entities();
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
           if(textBox1.Text==""|| textBox2.Text == "")
            {
                MessageBox.Show("Fill the form");
                return;
            }
            var employee = db.Employees.SingleOrDefault(p => p.Username == textBox1.Text && p.Password == textBox2.Text);
            if (employee!=null)
            {
                MessageBox.Show("Successfull");
                if (employee.isAdmin != null)
                {

                    FrmAdmin f = new FrmAdmin();
                    this.Hide();
                    f.ShowDialog();
                    this.Show();
                }
                else
                {
                    FrmUser f = new FrmUser(employee);
                    this.Hide();
                    f.ShowDialog();
                    this.Show();
                }
            }
            else
            {
                MessageBox.Show("Incorrect");
                return;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
