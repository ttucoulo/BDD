using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fil_RougeBDD
{
    class Theme
    {
        private string nomT;
        private int arrondissementT;
        public Theme (string nom, int arr)
        {
            this.nomT= nom;
            this.arrondissementT = arr;
        }
        public string NomT
        {
            get { return this.nomT; }
        }
        public int ArrondissementT
        {
            get { return this.arrondissementT; }
        }
    }
}
