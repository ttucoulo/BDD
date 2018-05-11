using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fil_RougeBDD
{
    class Date
    {
        private int numero_semaineD;
        public Date(int num)
        {
            this.numero_semaineD = num;
        }
        public int Numero_semaineD
        {
            get { return this.numero_semaineD; }
            set { this.numero_semaineD = value; }
        }
    }
}
