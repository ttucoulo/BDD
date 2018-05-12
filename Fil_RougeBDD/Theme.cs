using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fil_RougeBDD
{
    class Theme
    {
        private string id_theme;
        private string nom_theme;
        private int arrondissementT;
        private string descriptif_theme;

        public Theme (string id,string nom, int arr, string descriptif)
        {
            this.id_theme = id;
            this.nom_theme= nom;
            this.arrondissementT = arr;
            this.descriptif_theme = descriptif;
        }
        public string Nom_theme
        {
            get { return this.nom_theme; }
        }
        public int ArrondissementT
        {
            get { return this.arrondissementT; }
        }
        public string Descriptif
        {
            get { return this.descriptif_theme; }
        }
        public string Id_theme
        {
            get { return this.id_theme; }
        }
    }
}
