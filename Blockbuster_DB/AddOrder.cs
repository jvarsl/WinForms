using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DBLaborai
{
    public partial class AddOrder : Form
    {
        private string tabelNr = "";
        DataAccess db = new DataAccess();
        private readonly Laborai frm1;
        public AddOrder(string s, Laborai frm)
        {
            InitializeComponent();
            listViewSetup();
            tabelNr = s;
            frm1 = frm;
        }

        private void listViewSetup()
        {
            listView1.FullRowSelect = true;
            listView2.FullRowSelect = true;
            klientasList();
            produktasList();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                decimal clNr = decimal.Parse(textBox1.Text);
                decimal prNr = decimal.Parse(textBox2.Text);
                int days = 0;
                if (radioButton1.Checked) days = 7;
                if (radioButton2.Checked) days = 14;
                bool cl = db.checkIfExistClient(clNr);
                bool pr = db.checkIfExistProd(prNr);
                if (cl && pr)
                {
                    db.InsertOrder(tabelNr, clNr, prNr, days);
                    label4.Text = "Užsakymas pridėtas";
                    clearValues();
                    frm1.updateBinding();
                }
                else { label4.Text = $"Klaida\nKlientas egzistuoja = {cl}; \nProduktas egzistuoja = {pr}"; }
            }
            catch (Exception ex) { label4.Text = "Error " + ex.Message.ToString(); }
        }

        private void clearValues()
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
        }

        private void AddOrder_Load(object sender, EventArgs e)
        {
            radioButton1.Checked = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            klientasList();
        }

        private void klientasList()
        {
            listView1.Items.Clear();
            List<Klientas> klientas = new List<Klientas>();
            decimal klNr;
            try
            {
                klNr = string.IsNullOrEmpty(textBox1.Text) ? -1 : decimal.Parse(textBox1.Text);
            }
            catch (Exception e)
            {
                {
                    errorBox(e);
                    textBox1.Clear();
                    listView1.Items.Clear();
                    klNr = -1;
                }
            } 
            klientas = db.getKlientasInfo(klNr, textBox3.Text);
            foreach (Klientas k in klientas)
            {
                if (k.KLIENTO_NR.ToString() != "0")
                {
                    ListViewItem item = new ListViewItem(k.KLIENTO_NR.ToString());
                    item.SubItems.Add(k.VARDAS);
                    item.SubItems.Add(k.PAVARDE);
                    item.SubItems.Add(k.GIM_DATA.ToShortDateString());
                    listView1.Items.Add(item);
                }
            }
        }

        private void errorBox(Exception e)
        {
            MessageBox.Show(e.Message.ToString() + "\nNumeris gali būti tik skaičiai", "Klaida",
    MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void produktasList()
        {
            listView2.Items.Clear();
            List<Video> video = new List<Video>();
            List<Zaidimas> zaidimas = new List<Zaidimas>();
            List<Filmas> filmas = new List<Filmas>();

            decimal prNr;
            try
            {
                prNr = string.IsNullOrEmpty(textBox2.Text) ? -1 : decimal.Parse(textBox2.Text);
            }
            catch (Exception e)
            {
                {
                    errorBox(e);
                    textBox2.Clear();
                    listView2.Items.Clear();
                    prNr = -1;
                }
            }
            video = db.getVideoInfo(prNr, textBox4.Text);
            zaidimas = db.getZaidimasInfo();
            filmas = db.getFilmasInfo();
            foreach (Video k in video)
            {
                foreach (Zaidimas z in zaidimas)
                {
                    if (z.PRODUKTO_NR == k.PRODUKTO_NR)
                    {
                        ListViewItem item = new ListViewItem(k.PRODUKTO_NR.ToString());
                        item.SubItems.Add(k.PROD_PAV);
                        item.SubItems.Add(k.LEIDIMO_METAI.ToString());
                        item.SubItems.Add(z.PLATFORMA);
                        item.SubItems.Add("-");
                        item.SubItems.Add("-");
                        listView2.Items.Add(item);
                        break;
                    }
                }
                foreach (Filmas z in filmas)
                {
                    if (z.PRODUKTO_NR == k.PRODUKTO_NR)
                    {
                        ListViewItem item = new ListViewItem(k.PRODUKTO_NR.ToString());
                        item.SubItems.Add(k.PROD_PAV);
                        item.SubItems.Add(k.LEIDIMO_METAI.ToString());
                        item.SubItems.Add("-");
                        item.SubItems.Add(z.TRUKME.ToString());
                        item.SubItems.Add(z.FORMATAS);
                        listView2.Items.Add(item);
                        break;
                    } 
                }
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            klientasList();
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            textBox1.Text = listView1.SelectedItems[0].SubItems[0].Text;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            produktasList();
        }

        private void listView2_DoubleClick(object sender, EventArgs e)
        {
            textBox2.Text = listView2.SelectedItems[0].SubItems[0].Text;
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            produktasList();
        }
    }
}