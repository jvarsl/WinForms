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
    public partial class Return : Form
    {
        private readonly Laborai frm1;
        private string orderNr = "0";
        public Return(Laborai frm)
        {
            InitializeComponent();
            frm1 = frm;
        }

        public Return(string ordNr, Laborai frm)
        {
            InitializeComponent();
            orderNr = ordNr;
            frm1 = frm;
        }

        DataAccess db = new DataAccess();
        static decimal correctValue = 0;
        private void loadInfo()
        {
            try
            {
                decimal tx1;
                tx1 = decimal.Parse(textBox1.Text);
                var proNr = db.returnSkolinasiProduct(tx1);
                if (proNr.GRAZINO == DateTime.MinValue)
                {
                    int returnDateDifference = (DateTime.Now - proNr.IKI).Days;
                    label2.Text = $"Produktas {proNr.PRODUKTO_NR} paimtas {proNr.NUO}\nyra ";
                    if (returnDateDifference >= 0) label2.Text += returnDateDifference.ToString() + " dienas vėluojamas";
                    else label2.Text += (returnDateDifference * (-1)).ToString() + " dienomis anksčiau";
                    correctValue = proNr.UZSAKYMO_NR;
                    button2.Enabled = true;
                }
                else
                {
                    label2.Text = $"Užsakymas jau grąžintas\n{proNr.GRAZINO}";
                    button2.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                label2.Text = ex.Message.ToString();
                button2.Enabled = false;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                db.updateSkolinasiGrazino(correctValue);
                button2.Enabled = false;
                label2.Text = $"Užsakymas {correctValue} grąžintas";
                frm1.updateBinding();
            }
            catch (Exception ex) { label2.Text = ex.Message.ToString(); }
        }

        private void Return_Load(object sender, EventArgs e)
        {
            if (orderNr != "0") textBox1.Text = orderNr;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text == "") label2.Text = "";
            else loadInfo();
        }
    }
}
