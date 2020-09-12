using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace DBLaborai
{
    class DataAccess
    {
        public List<Darbuotojas> getDarbuotojasNames()
        {
            using (IDbConnection cnn = new System.Data.SqlClient.SqlConnection(Helper.CnnVal()))
            {
                string sql = "select TABEL_NR from DARBUOTOJAS;";
                var vardai = cnn.Query<Darbuotojas>(sql).ToList();
                return vardai;
            }
        }

        public List<FullUzsakymas> GetFullUzsakymasFull(string pav, decimal nr, bool grazBool)
        {
            using (IDbConnection cnn = new System.Data.SqlClient.SqlConnection(Helper.CnnVal()))
            {
                string sql = @"select * from UZSAKYMAS U
                        inner join KLIENTAS K on U.KLIENTO_NR = K.KLIENTO_NR
                        inner join SKOLINASI S on U.UZSAKYMO_NR = S.UZSAKYMO_NR
                        inner join VIDEO V on S.PRODUKTO_NR = V.PRODUKTO_NR
                        inner join DARBUOTOJAS D on U.TABEL_NR = D.TABEL_NR
                        inner join BLOCKBUSTERIS B on D.BLOCKB_NR = B.BLOCKB_NR";
                if (pav.Count() != 0 || nr != 0 || grazBool)
                {
                    int i = 0;
                    sql += " where ";
                    if (pav.Count() != 0) { sql += "K.PAVARDE like @Pavarde"; i++; }
                    if (nr != 0)
                    {
                        if (i == 1) sql += " and ";
                        sql += "U.UZSAKYMO_NR = @Unr"; i++;
                    }
                    if (grazBool)
                    {
                        if (i >= 1) sql += " and ";
                        sql += "S.GRAZINO IS NULL";
                    }
                }
                sql += " order by U.UZSAKYMO_NR desc";

                var uzsakymai = cnn.Query<FullUzsakymas, Klientas, Skolinasi, Video, Darbuotojas, Blockbusteris, FullUzsakymas>(sql, (f, k, s, v, d, b) =>
                    {
                        f.klientas = k;
                        f.skolinasi = s;
                        f.video = v;
                        f.darbuotojas = d;
                        f.blockbusteris = b;
                        return f;
                    }, new { Pavarde = pav + "%", UNr = nr }, splitOn: "KLIENTO_NR,PRODUKTO_NR,PRODUKTO_NR,TABEL_NR,BLOCKB_NR").ToList();
                return uzsakymai;
            }
        }


        public Skolinasi returnSkolinasiProduct(decimal orderNr)
        {
            using (IDbConnection cnn = new System.Data.SqlClient.SqlConnection(Helper.CnnVal()))
            {
                var p = new { ONr = orderNr };
                string sql = @"select * from SKOLINASI where UZSAKYMO_NR = @ONr";
                var uzsNr = cnn.Query<Skolinasi>(sql, p).Single();
                return uzsNr;
            }
        }

        public void updateSkolinasiGrazino(decimal orderNr)
        {
            using (IDbConnection cnn = new System.Data.SqlClient.SqlConnection(Helper.CnnVal()))
            {
                string sql = @"update SKOLINASI
                    set GRAZINO = GETDATE()
                    where UZSAKYMO_NR = @nr";
                var result = cnn.Execute(sql, new
                {
                    nr = orderNr
                });
            }
        }

        public bool checkIfExistClient(decimal klNr)
        {
            using (IDbConnection cnn = new System.Data.SqlClient.SqlConnection(Helper.CnnVal()))
            {
                string sql = "select count(distinct 1) from KLIENTAS where KLIENTO_NR = @KlieNr";
                var exists = cnn.ExecuteScalar<bool>(sql, new { KlieNr = klNr });
                return exists;
            }
        }

        public bool checkIfExistProd(decimal prNr)
        {
            using (IDbConnection cnn = new System.Data.SqlClient.SqlConnection(Helper.CnnVal()))
            {
                string sql = "select count(distinct 1) from VIDEO where PRODUKTO_NR = @ProdNr";
                var exists = cnn.ExecuteScalar<bool>(sql, new { ProdNr = prNr });
                return exists;
            }
        }

        public void InsertOrder(string tabelNr, decimal klNr, decimal prNr, int termDienos)
        {
            using (IDbConnection cnn = new System.Data.SqlClient.SqlConnection(Helper.CnnVal()))
            {
                string sql1 = @"insert into UZSAKYMAS ([TABEL_NR],[KLIENTO_NR]) values(@TabNr, @KlieNr)
                    SELECT CAST(SCOPE_IDENTITY() as decimal)";
                string sql2 = @"insert into SKOLINASI ([PRODUKTO_NR],[UZSAKYMO_NR],[NUO],[IKI]) 
                    values(@ProdNr, @UzsakNr, GETDATE(), Convert(date, getdate()+@TermDienos));";
                cnn.Open();
                using (var transaction = cnn.BeginTransaction())
                {

                    try
                    {
                        var uzsNr = cnn.Query<decimal>(sql1, new { TabNr = tabelNr, KlieNr = klNr }, transaction: transaction).Single();
                        cnn.Execute(sql2, new { ProdNr = prNr, UzsakNr = uzsNr, TermDienos = termDienos }, transaction: transaction);
                        transaction.Commit();
                    }
                    catch (Exception) { transaction.Rollback(); throw; }
                }
            }
        }

        public bool DeleteKlientas(decimal klNr, decimal asmNr)
        {
            using (IDbConnection cnn = new System.Data.SqlClient.SqlConnection(Helper.CnnVal()))
            {
                string sql = "delete from KLIENTAS where KLIENTO_NR = @KlieNr and ASM_KODAS = @AsmNr";
                var isSuccess = cnn.Execute(sql, new { KlieNr = klNr, AsmNr = asmNr });
                return isSuccess > 0;
            }
        }

        public List<Priklauso> getPriklausoZanrai(decimal prNr)
        {
            using (IDbConnection cnn = new System.Data.SqlClient.SqlConnection(Helper.CnnVal()))
            {
                string sql = "select * from PRIKLAUSO P inner join ZANRAS Z on P.ZANRO_NR = Z.ZANRO_NR where P.PRODUKTO_NR = @ProdNr";
                var priklausoDictionary = new Dictionary<decimal, Priklauso>();
                var list = cnn.Query<Priklauso, Zanras, Priklauso>(sql, (priklauso, zanras) =>
                {
                    Priklauso priklausoEntry;
                    if (!priklausoDictionary.TryGetValue(priklauso.ZANRO_NR, out priklausoEntry))
                    {
                        priklausoEntry = priklauso;
                        priklausoEntry.Zanrai = new List<Zanras>();
                        priklausoDictionary.Add(priklausoEntry.ZANRO_NR, priklausoEntry);
                    }

                    priklausoEntry.Zanrai.Add(zanras);
                    return priklausoEntry;
                }, new { ProdNr = prNr}, splitOn: "ZANRO_NR").Distinct().ToList();
                return list;
            }
        }

        public Video getProductInfo(decimal prNr)
        {
            using (IDbConnection cnn = new System.Data.SqlClient.SqlConnection(Helper.CnnVal()))
            {
                string sql = "select * from VIDEO where PRODUKTO_NR = @ProdNr";
                var prod = cnn.Query<Video>(sql, new { ProdNr = prNr }).Single();
                return prod;
            }
        }

        public Filmas getFilmasInfo(decimal prNr)
        {
            using (IDbConnection cnn = new System.Data.SqlClient.SqlConnection(Helper.CnnVal()))
            {
                string sql = @"select * from VIDEO
inner join FILMAS on VIDEO.PRODUKTO_NR = FILMAS.PRODUKTO_NR
where FILMAS.TRUKME is not null and VIDEO.PRODUKTO_NR = @ProdNr";
                var prod = cnn.Query<Filmas>(sql, new { ProdNr = prNr }).Single();
                return prod;
            }
        }

        public Zaidimas getZaidimasInfo(decimal prNr)
        {
            using (IDbConnection cnn = new System.Data.SqlClient.SqlConnection(Helper.CnnVal()))
            {
                string sql = @"select * from VIDEO
inner join ZAIDIMAS on VIDEO.PRODUKTO_NR = ZAIDIMAS.PRODUKTO_NR
where ZAIDIMAS.PLATFORMA is not null and VIDEO.PRODUKTO_NR = @ProdNr";
                var prod = cnn.Query<Zaidimas>(sql, new { ProdNr = prNr }).Single();
                return prod;
            }
        }

        public Skolinasi getSkolinasiInfo(decimal orNr)
        {
            using (IDbConnection cnn = new System.Data.SqlClient.SqlConnection(Helper.CnnVal()))
            {
                string sql = "select * from SKOLINASI where UZSAKYMO_NR = @OrderNr;";
                var prod = cnn.Query<Skolinasi>(sql, new { OrderNr = orNr }).Single();
                return prod;
            }
        }

        public Uzsakymas getUzsakymasInfo(decimal orNr)
        {
            using (IDbConnection cnn = new System.Data.SqlClient.SqlConnection(Helper.CnnVal()))
            {
                string sql = "select * from UZSAKYMAS where UZSAKYMO_NR = @OrderNr;";
                var prod = cnn.Query<Uzsakymas>(sql, new { OrderNr = orNr }).Single();
                return prod;
            }
        }

        public List<Darbuotojas> getDarbuotojoBlockbusterisInfo(string tNr)
        {
            using (IDbConnection cnn = new System.Data.SqlClient.SqlConnection(Helper.CnnVal()))
            {
                string sql = @"SELECT * from DARBUOTOJAS D
                    inner join BLOCKBUSTERIS B on B.BLOCKB_NR = D.BLOCKB_NR
                    where D.TABEL_NR = @TabNr";
                var prod = cnn.Query<Darbuotojas, Blockbusteris, Darbuotojas>(sql, (d, b) => {
                    d.blockbusteris = b;
                    return d;
                }, new { TabNr = tNr }, splitOn: "BLOCKB_NR").ToList();
                return prod;
            }
        }

        public List<Klientas> getKlientasInfo(decimal klNr, string pav)
        {
            using (IDbConnection cnn = new System.Data.SqlClient.SqlConnection(Helper.CnnVal()))
            {
                string sql = "";
                if (klNr == -1 && pav == "") sql = $"select * from KLIENTAS;";
                if (klNr == -1 && pav != "") sql = $"select * from KLIENTAS where PAVARDE LIKE @Pavarde;";
                if (klNr != -1 && pav == "") sql = $"select * from KLIENTAS where KLIENTO_NR = @ClientNr;";
                if (klNr != -1 && pav != "") sql = $"select * from KLIENTAS where KLIENTO_NR = @ClientNr and PAVARDE LIKE @Pavarde;";
                var kl = cnn.Query<Klientas>(sql, new { ClientNr = klNr, Pavarde = pav+"%" }).ToList();
                return kl;
            }
        }

        public List<Video> getVideoInfo(decimal klNr, string pav)
        {
            using (IDbConnection cnn = new System.Data.SqlClient.SqlConnection(Helper.CnnVal()))
            {
                string sql = "";
                if (klNr == -1 && pav == "") sql = $"select * from VIDEO;";
                if (klNr == -1 && pav != "") sql = $"select * from VIDEO where PROD_PAV LIKE @Pavarde;";
                if (klNr != -1 && pav == "") sql = $"select * from VIDEO where PRODUKTO_NR = @ClientNr;";
                if (klNr != -1 && pav != "") sql = $"select * from VIDEO where PRODUKTO_NR = @ClientNr and PROD_PAV LIKE @Pavarde;";
                var kl = cnn.Query<Video>(sql, new { ClientNr = klNr, Pavarde = pav + "%" }).ToList();
                return kl;
            }
        }

        public List<Zaidimas> getZaidimasInfo()
        {
            using (IDbConnection cnn = new System.Data.SqlClient.SqlConnection(Helper.CnnVal()))
            {
                string sql = $"select * from ZAIDIMAS;";
                var kl = cnn.Query<Zaidimas>(sql).ToList();
                return kl;
            }
        }

        public List<Filmas> getFilmasInfo()
        {
            using (IDbConnection cnn = new System.Data.SqlClient.SqlConnection(Helper.CnnVal()))
            {
                string sql = $"select * from FILMAS;";
                var kl = cnn.Query<Filmas>(sql).ToList();
                return kl;
            }
        }

        public void updateKlientas(decimal klNr, string pav, string elp, string tlf)
        {
            using (IDbConnection cnn = new System.Data.SqlClient.SqlConnection(Helper.CnnVal()))
            {
                if (klNr == 0) klNr = -1;
                string sql = "update KLIENTAS set PAVARDE = @Pavarde, E_PASTAS = @Epastas, TELEFONAS = @Telefonas where KLIENTO_NR = @KlieNr";
                cnn.Execute(sql, new { KlieNr = klNr, Pavarde = pav, Epastas = elp, Telefonas = tlf });
            }
        }
    }
}
