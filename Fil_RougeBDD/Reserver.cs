using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fil_RougeBDD
{
    class Reserver
    {
        private string num_c;
        private string id_s;
        private string date_r;
        private bool confirme;
        private double note;

        public Reserver(string num_c1, string id_s1, string date, bool confirm, double note1)
        {
            this.num_c = num_c1;
            this.id_s = id_s1;
            this.date_r = date;
            this.confirme = confirm;
            this.note = note1;
        }

        public string Num_c
        {
            get { return this.num_c; }
        }
        public string Id_s
        {
            get { return this.id_s; }
        }
        public string Date_r
        {
            get { return this.date_r; }
        }
        public bool Confirme
        {
            get { return this.confirme; }
            set { this.confirme = value; }
        }
        public double Note
        {
            get { return this.note; }
        }
    }
}
