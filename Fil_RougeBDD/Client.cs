using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fil_RougeBDD
{
    class Client
    {
        private string num_c;
        private string nom;
        private string adresse;

        public Client(string numC, string nom1, string adresse1)
        {
            this.num_c = numC;
            this.nom = nom1;
            this.adresse = adresse1;
        }

        public string Num_c
        {
            get { return this.num_c; }
        }
        public string Nom
        {
            get { return this.nom; }
        }
        public string Adresse
        {
            get { return this.adresse; }
        }
    }
}
