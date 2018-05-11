using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fil_RougeBDD
{
    class Logement
    {
        private string nomL;
        private string adresseL;
        private int arrondissementL;
        private bool disponibiliteL;
        public Logement ( string nom, string adresse, int arr, bool dispo)
        {
            this.nomL = nom;
            this.adresseL = adresse;
            this.arrondissementL = arr;
        }
        public string NomL
        {
            get { return this.nomL; }
        }
        public string AdresseL
        {
            get { return this.adresseL; }
        }
        public int ArrondissementL
        {
            get { return this.arrondissementL; }
        }
        public bool DisponibiliteL
        {
            get { return this.disponibiliteL;}
            set { this.disponibiliteL = value; }
        }
    }
}
