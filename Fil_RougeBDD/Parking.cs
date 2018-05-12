using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fil_RougeBDD
{
    class Parking
    {
        private string id_parking;
        private string nom;
        private string adresse;
        private int arrondissement;

        public Parking(string id_p,string nom1,string adresse1, int arrond)
        {
            this.id_parking = id_p;
            this.nom = nom1;
            this.adresse = adresse1;
            this.arrondissement = arrond;
        }

        public string Id_parking
        {
            get { return this.id_parking; }
        }
        public string Nom
        {
            get { return this.nom; }
        }
        public string Adresse
        {
            get { return this.Adresse; }
        }
        public int Arrondissement
        {
            get { return this.arrondissement; }
        }
    }
}
