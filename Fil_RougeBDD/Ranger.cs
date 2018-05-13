using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fil_RougeBDD
{
    class Ranger
    {
        private string id_p;
        private string immat;
        private string date_rangee;
        private string num_place;

        public Ranger(string id_p1, string immat1,string date, string num)
        {
            this.id_p = id_p1;
            this.immat = immat1;
            this.date_rangee = date;
            this.num_place = num;
        }

        public string Id_p
        {
            get { return this.id_p; }
        }
        public string Immat
        {
            get { return this.immat; }
        }
        public string Date_rangee
        {
            get { return this.date_rangee; }
        }
        public string Num_place
        {
            get { return this.num_place; }
        }
    }
}
