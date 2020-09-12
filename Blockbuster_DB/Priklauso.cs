using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBLaborai
{
    class Priklauso
    {
        public decimal PRODUKTO_NR { get; set; }
        public decimal ZANRO_NR { get; set; }
        public List<Zanras> Zanrai { get; set; }
    }
}
