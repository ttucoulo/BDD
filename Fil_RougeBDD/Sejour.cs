using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fil_RougeBDD
{
    class Sejour
    {
        private string id_sejour;
        private string description_sejour;
        private string theme_sejour;

        public Sejour(string id, string description, string theme)
        {
            this.id_sejour = id;
            this.description_sejour = description;
            this.theme_sejour = theme;
        }

        public string Id_sejour
        {
            get { return this.id_sejour; }
        }
        public string Description_sejour
        {
            get { return this.description_sejour; }
        }
        public string Theme_sejour
        {
            get { return this.theme_sejour; }
        }
    }
}
