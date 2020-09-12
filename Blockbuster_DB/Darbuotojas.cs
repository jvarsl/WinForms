using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBLaborai
{
    class Darbuotojas
    {
        public string TABEL_NR { get; set; }
        public string BLOCKB_NR { get; set; }
        public decimal ASM_KODAS { get; set; }
        public string VARDAS { get; set; }
        public string PAVARDE { get; set; }
        public DateTime GIM_DATA { get; set; }
        public string E_PASTAS { get; set; }
        public string TELEFONAS { get; set; }
        public Blockbusteris blockbusteris { get; set; }
    }
}
