using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fil_RougeBDD
{
    class Controleur
    {
        private int IdCO;
        private string nomCO;
        private string prenomCO;
        public Controleur (int id,string nom,string prenom)
        {
            this.IdCO = id;
            this.nomCO = nom;
            this.prenomCO = prenom;
        }
        public int IDCO
        {
            get { return this.IdCO; }
        }
        public string NomCO
        {
            get { return this.nomCO; }
        }
        public string PrenomCO
        {
            get { return this.prenomCO; }
        }
    }
}
