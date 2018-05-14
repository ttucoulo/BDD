using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fil_RougeBDD
{
    class Voiture
    {
        private string immat;
        private bool disponible;
        private string motif;
        private string marque;
        private string modele;
        private int nb_places;

        public Voiture(string immat1,bool disponible1,string motif1,string marque1,string modele1,int nbr_place1)
        {
            this.immat = immat1;
            this.disponible = disponible1;
            this.motif = motif1;
            this.marque = marque1;
            this.modele = modele1;
            this.nb_places = nbr_place1;
        }

        public string Immat
        {
            get { return this.immat; }
        }
        public bool Disponible
        {
            get { return this.disponible; }
            set { this.disponible = value; }
        }
        public string Motif
        {
            get { return this.motif; }
            set { this.motif = value; }
        }
    }
}
