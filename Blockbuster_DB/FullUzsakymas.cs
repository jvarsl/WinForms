using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBLaborai
{
    class FullUzsakymas : Uzsakymas
    {
        public Klientas klientas { get; set; }
        public Skolinasi skolinasi { get; set; }
        public Video video { get; set; }
        public Darbuotojas darbuotojas { get; set; }
        public Blockbusteris blockbusteris { get; set; }
    }
}
