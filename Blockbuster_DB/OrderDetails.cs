using Dapper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DBLaborai
{
    public partial class OrderDetails : Form
    {
        private List<Priklauso> priklausos = new List<Priklauso>();
        private DataAccess db = new DataAccess();
        private Video videoProd = new Video();
        private Filmas filmas = new Filmas();
        private Zaidimas zaidimas = new Zaidimas();
        private Skolinasi skolinasis = new Skolinasi();
        private Uzsakymas uzsakymass = new Uzsakymas();
        private List<Darbuotojas> darbBlockust = new List<Darbuotojas>();
        private decimal prodNr = 0;
        private decimal orderNr = 0;
        private string zanruOutput = "";
        string tabelNr = "";
        private readonly Laborai frm1;
        public OrderDetails(decimal uzs, decimal pro, string s, Laborai frm)
        {
            InitializeComponent();
            prodNr = pro;
            orderNr = uzs;
            tabelNr = s;
            frm1 = frm;
        }
        private void OrderDetails_Load(object sender, EventArgs e)
        {
            try
            {
                priklausos = db.getPriklausoZanrai(prodNr);
                for (int i = 0; i < priklausos.Count; i++)
                {
                    zanruOutput += priklausos.ElementAt(i).Zanrai[0].ZANRAS + "\n";
                }
                zanruOutput = zanruOutput.Substring(0, zanruOutput.Length - 2);
                videoProd = db.getProductInfo(prodNr);
                try
                {
                    filmas = db.getFilmasInfo(prodNr);
                }
                catch
                {
                    zaidimas = db.getZaidimasInfo(prodNr);
                }
                skolinasis = db.getSkolinasiInfo(orderNr);
                uzsakymass = db.getUzsakymasInfo(orderNr);
                darbBlockust = db.getDarbuotojoBlockbusterisInfo(uzsakymass.TABEL_NR);
                fillAllBoxes();
                controlInsertButton();
            }catch(Exception ex) { label1.Text = ex.Message.ToString(); }
        }

        private void controlInsertButton()
        {
            if (tabelNr.Any()) button1.Enabled = true;
            else
            {
                button1.Enabled = false;
                button1.Text = "Prisijungti dėl grąžinimo patvirtinmo";
            }
            if(skolinasis.GRAZINO != DateTime.MinValue)
            {
                button1.Enabled = false;
                button1.Text = "Produktas yra grąžintas";
            }
        }

        private void fillAllBoxes()
        {
            string videoType = "";
            textBox13.Text = videoProd.PRODUKTO_NR.ToString();
            textBox12.Text = videoProd.PROD_PAV;
            textBox11.Text = videoProd.LEIDIMO_METAI.ToString();
            textBox9.Text = videoProd.LEIDEJAS;
            textBox18.Text = videoProd.METASCORE.ToString();
            richTextBox2.Text = videoProd.APRASYMAS;
            textBox10.Text = videoProd.AMZ_CENZAS;
            richTextBox1.AppendText(zanruOutput);
            if (zaidimas.PLATFORMA != null)
            {
                textBox15.Text = zaidimas.PLATFORMA;
                textBox16.Hide(); textBox17.Hide();
                label21.Hide(); label22.Hide();
                videoType = "žaidimas";
            }
            else if (filmas.FORMATAS != null)
            {
                textBox16.Text = filmas.FORMATAS;
                textBox17.Text = filmas.TRUKME.ToString();
                label23.Hide(); textBox15.Hide();
                videoType = "filmas";
            }
            label1.Text += $" ({videoType})";

            textBox4.Text = orderNr.ToString();
            textBox5.Text = (uzsakymass.KLIENTO_NR.ToString() == "0") ? "ištrintas" : uzsakymass.KLIENTO_NR.ToString();
            textBox6.Text = skolinasis.NUO.ToString();
            textBox7.Text = skolinasis.IKI.ToShortDateString();
            if (skolinasis.GRAZINO == DateTime.MinValue) textBox8.Text = "-";
            else textBox8.Text = skolinasis.GRAZINO.ToString();

            textBox3.Text = uzsakymass.TABEL_NR;

            textBox2.Text = darbBlockust.First().blockbusteris.ADRESAS;
            textBox1.Text = darbBlockust.First().blockbusteris.BLOCKB_PAV;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            Return returnForm = new Return(orderNr.ToString(), frm1);
            returnForm.ShowDialog();
        }
    }
}
