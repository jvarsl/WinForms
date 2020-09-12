using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DBLaborai
{
    public partial class EditUser : Form
    {
        private List<Klientas> klientas = new List<Klientas>();
        private DataAccess db = new DataAccess();
        private readonly Laborai frm1;
        public EditUser(Laborai frm)
        {
            InitializeComponent();
            listViewSetup();
            klientasList();
            frm1 = frm;
        }


        private void listViewSetup()
        {
            listView1.FullRowSelect = true;
            listView1.Columns[0].Width = 40;
            listView1.Columns[1].Width = 110;
            listView1.Columns[2].Width = 110;
            listView1.Columns[3].Width = 110;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            klientasList();
            fillClientData();
        }

        private void klientasList()
        {
            label13.Text = "";
            listView1.Items.Clear();
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

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            klientasList();
        }

        private void fillClientData()
        {
            foreach (Klientas k in klientas)
            {
                if (k.KLIENTO_NR.ToString() == textBox1.Text && textBox1.Text != "0")
                {
                    textBox4.Text = k.KLIENTO_NR.ToString();
                    textBox5.Text = k.VARDAS;
                    textBox6.Text = k.PAVARDE.Replace(" ","");
                    textBox7.Text = k.ASM_KODAS.ToString();
                    textBox8.Text = k.GIM_DATA.ToShortDateString();
                    textBox9.Text = k.E_PASTAS;
                    textBox2.Text = k.TELEFONAS;
                }
            }
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            textBox1.Text = listView1.SelectedItems[0].SubItems[0].Text;
            fillClientData();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (!textBox2.Text.All(char.IsDigit))
            {
                label6.Text = "Leidžiami tik \nskaičiai";
                button1.Enabled = false;
            }
            else { label6.Text = "";
                button1.Enabled = true;
            }
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            if(textBox6.Text.Length>=2 && !textBox6.Text.Any(char.IsDigit))
            {
                label4.Text = "";
                button1.Enabled = true;
            }
            else
            {
                label4.Text = "Leidžiamos tik \nraidės";
                button1.Enabled = false;
            }
        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {
            if (textBox9.Text.Length >= 4 && textBox9.Text.Contains('@'))
            {
                label12.Text = "";
                button1.Enabled = true;
            }
            else
            {
                label12.Text = "Neteisingas el. \npaštas";
                button1.Enabled = false;
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                decimal tx4 = decimal.Parse(textBox4.Text);
                db.updateKlientas(tx4, textBox6.Text, textBox9.Text, textBox2.Text);
                textBox1.Clear();
                textBox3.Clear();
                label13.Text = $"{tx4} nr kliento duomenys atnaujinti";
                frm1.updateBinding();
            }
            catch (Exception ex) { MessageBox.Show($"Pranešimas:\n{ex.Message}", "Klaida"); }
        }
    }
}
