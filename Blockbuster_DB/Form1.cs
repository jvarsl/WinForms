using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DBLaborai
{
    public partial class Laborai : Form
    {
        public Laborai()
        {
            InitializeComponent();
            updateBinding();
            fillDarbuotojasNames();
        }

        public void updateBinding()
        {
            DataAccess db = new DataAccess();
            decimal tx2;
            try
            {
                tx2 = string.IsNullOrEmpty(textBox2.Text) ? 0 : decimal.Parse(textBox2.Text);
            }
            catch (Exception e)
            {
                MessageBox.Show(label2.Text + ": " + e.Message.ToString() + "\nPrograma praleido šį parametrą.", "Klaida",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning); tx2 = 0;
                textBox2.Clear();
            }

            bool grazintiSelection = checkBox1.Checked;
            List<FullUzsakymas> fz = db.GetFullUzsakymasFull(textBox1.Text, tx2, grazintiSelection);
            var fzs = fz.Select(a => new
            {
                OrderNr = a.UZSAKYMO_NR,
                ClientNr = a.KLIENTO_NR,
                Vardas = a.klientas.VARDAS,
                Surname = a.klientas.PAVARDE,
                ProductNr = a.video.PRODUKTO_NR,
                ProductName = a.video.PROD_PAV,
                BlockPav = a.blockbusteris.BLOCKB_PAV,
                Nuo = a.skolinasi.NUO,
                Iki = a.skolinasi.IKI,
                Return = a.skolinasi.GRAZINO,
            }).ToList();
            var bindingSource1 = new System.Windows.Forms.BindingSource { DataSource = fzs };
            dataGridView1.DataSource = bindingSource1;

            if (fzs.Any()) emptyWarningLabel.Text = $"Rasti {fzs.Count()} rezultatai";
            else emptyWarningLabel.Text = "Nieko nerasta";

            dataGridFormatting();
        }

        private void dataGridFormatting()
        {
            dataGridView1.Columns[0].HeaderCell.Value = "Užsakymo nr";
            dataGridView1.Columns[1].HeaderCell.Value = "Kliento nr";
            dataGridView1.Columns[4].HeaderCell.Value = "Produkto nr";
            dataGridView1.Columns[5].HeaderCell.Value = "Produkto pavadinimas";
            dataGridView1.Columns[6].HeaderCell.Value = "Blockbusteris";
            dataGridView1.Columns[3].HeaderCell.Value = "Pavardė";
            dataGridView1.Columns[9].HeaderCell.Value = "Grąžino";
            dataGridView1.BackgroundColor = Color.White;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }


        private void button4_Click(object sender, EventArgs e)
        {
            updateBinding();
        }

        private void fillDarbuotojasNames()
        {
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            DataAccess db = new DataAccess();
            var tabeliai = db.getDarbuotojasNames();
            foreach (var tab in tabeliai)
            {
                comboBox1.Items.Add(tab.TABEL_NR);
            }
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();
            checkBox1.Checked = false;
        }

        private void updateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Return returnForm = new Return(this);
            returnForm.ShowDialog();
        }

        private void insertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddOrder insertForm = new AddOrder(comboBox1.Text, this);
            insertForm.ShowDialog();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox1.Enabled = false;
            functionalityToolStripMenuItem.Enabled = true;
            comboBox1.Hide();
            Laborai.ActiveForm.Text += $" ({comboBox1.Text})";
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteClient dlForm = new DeleteClient(this);
            dlForm.ShowDialog();
        }

        private void toExcelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int counter = 0;
            Microsoft.Office.Interop.Excel._Application app = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel._Workbook workbook = app.Workbooks.Add(Type.Missing);
            Microsoft.Office.Interop.Excel._Worksheet worksheet = null;
            app.Visible = true;
            worksheet = workbook.Sheets["Sheet1"];
            worksheet = workbook.ActiveSheet;
            for (int i = 1; i < dataGridView1.Columns.Count + 1; i++)
            {
                worksheet.Cells[1, i] = dataGridView1.Columns[i - 1].HeaderText;
            }
            int iForMyExcel = 0;
            int x = 0;
            try
            {
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    x = i;
                    int jForMyExcel = 0;
                    for (int j = 0; j < dataGridView1.Columns.Count; j++)
                    {
                        if (dataGridView1.Rows[i].Cells[j].Value != null && dataGridView1.Rows[i].Cells[j].Value.ToString() != DateTime.MinValue.ToString())
                        {
                            if (j != 8) worksheet.Cells[iForMyExcel + 2, jForMyExcel + 1] = dataGridView1.Rows[i].Cells[j].Value.ToString();
                            else worksheet.Cells[iForMyExcel + 2, jForMyExcel + 1] = dataGridView1.Rows[i].Cells[j].Value.ToString().Substring(0, dataGridView1.Rows[i].Cells[j].Value.ToString().Length - 9);
                        }
                        else
                        {
                            worksheet.Cells[iForMyExcel + 2, jForMyExcel + 1] = "";
                        }
                        jForMyExcel++;
                    }
                    counter++;
                    if (i >= 2 && dataGridView1.Rows[i].Cells[7].Value.ToString().Substring(0, 7) != dataGridView1.Rows[i + 1].Cells[7].Value.ToString().Substring(0, 7))
                    {
                        worksheet.Cells[iForMyExcel + 3, 8] = dataGridView1.Rows[i].Cells[7].Value.ToString().Substring(0, 7) + " laikotarpiu: ";
                        worksheet.Cells[iForMyExcel + 3, 9] = counter.ToString();
                        worksheet.Cells[iForMyExcel + 3, 9].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft;
                        worksheet.Cells[iForMyExcel + 3, 8].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight;
                        worksheet.Cells[iForMyExcel + 3, 1].EntireRow.Font.Bold = true;
                        worksheet.Cells[iForMyExcel + 3, 8].Interior.Color = Color.FromArgb(255, 230, 230, 230);
                        worksheet.Cells[iForMyExcel + 3, 9].Interior.Color = Color.FromArgb(255, 230, 230, 230);
                        iForMyExcel++; jForMyExcel++; //kadangi kuriam savo eilute, tai padarom vietos daugiau likusiems irasams
                        counter = 0;
                    }
                    iForMyExcel++;
                }
            }
            catch { }//paskutine row invalid bus, kur ilgame if'e lygina, tad nieko nedarom
            finally
            {
                worksheet.Cells[iForMyExcel + 3, 8] = dataGridView1.Rows[x - 1].Cells[7].Value.ToString().Substring(0, 7) + " laikotarpiu:";
                worksheet.Cells[iForMyExcel + 3, 9] = counter.ToString();
                worksheet.Cells[iForMyExcel + 3, 9].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft;
                worksheet.Cells[iForMyExcel + 3, 8].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight;
                worksheet.Cells[1, 1].EntireRow.Font.Bold = true;
                worksheet.Cells[iForMyExcel + 3, 8].Interior.Color = Color.FromArgb(255, 230, 230, 230);
                worksheet.Cells[iForMyExcel + 3, 9].Interior.Color = Color.FromArgb(255, 230, 230, 230);
                worksheet.Cells[iForMyExcel + 3, 1].EntireRow.Font.Bold = true;
                Microsoft.Office.Interop.Excel.Range tRange = worksheet.UsedRange;
                tRange.Borders.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
                tRange.Borders.Weight = Microsoft.Office.Interop.Excel.XlBorderWeight.xlThin;


                worksheet.Columns.AutoFit();
            }
        }


        private void copyAlltoClipboard()
        {
            dataGridView1.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;
            dataGridView1.MultiSelect = true;
            dataGridView1.SelectAll();
            DataObject dataObj = dataGridView1.GetClipboardContent();
            if (dataObj != null)
                Clipboard.SetDataObject(dataObj);
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.CurrentCell.ColumnIndex.Equals(0))
            {
                //4 column yra musu ID vieta, toliau randam row vieta
                int rowId = dataGridView1.CurrentCell.RowIndex;
                decimal prodId = decimal.Parse(dataGridView1.Rows[rowId].Cells[4].Value.ToString());
                decimal uzsId = decimal.Parse(dataGridView1.Rows[rowId].Cells[0].Value.ToString());
                OrderDetails orderDetailsForm = new OrderDetails(uzsId, prodId, comboBox1.Text, this);
                orderDetailsForm.ShowDialog();
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            updateBinding();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            updateBinding();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            updateBinding();
        }

        private void redaguotiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditUser edForm = new EditUser(this);
            edForm.ShowDialog();
        }
    }
}
