using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DBLaborai
{
    public partial class DeleteClient : Form
    {
        private readonly Laborai frm1;
        public DeleteClient(Laborai frm)
        {
            InitializeComponent();
            frm1 = frm;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                decimal tx1 = decimal.Parse(textBox1.Text);
                decimal tx2 = decimal.Parse(textBox2.Text);
                DataAccess db = new DataAccess();
                label3.Text = db.DeleteKlientas(tx1, tx2) ? "Successfully deleted the client" : "Client not found and not deleted!";
                frm1.updateBinding();
            } catch (Exception ex) { label3.Text = ex.Message.ToString(); }

        }
    }
}
