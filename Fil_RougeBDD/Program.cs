using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;
using MySql.Data.MySqlClient;

namespace Fil_RougeBDD
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = " SERVER = fboisson.ddns.net ; PORT = 3306; DATABASE = TUCO_THIB; UID = S6-TUCO-THIB;PASSWORD = 8441;";
            Client un_client = new Client("20-5", "Tucoulou", "ESILV, La Defense");
            List<Voiture> listeV= InstancieListeVoitureFromBDD(connectionString);
            List<Client> listeC=InstancieListeClientFromBDD(connectionString);
            List<Parking> listeP = InstancieListeParkingFromBDD(connectionString);
            List<Sejour> listeS = InstancieListeSejourFromBDD(connectionString);
            List<Theme> listeT = InstancieListeThemeFromBDD(connectionString);
            List<Ranger> listeR = InstancieListeRangerFromBDD(connectionString);
            Sejour sejour_client=Message1(listeC,listeS,listeT,un_client);
            Ranger voiture_dispo= E3(connectionString,listeR);
            List<String> Appartement_recueilli = E5();
            Message2(connectionString, sejour_client, listeT, un_client, voiture_dispo);
            Console.ReadKey();
        }

        public static List<Client> InstancieListeClientFromBDD(string connectionString)
        {
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            List<Client> listeC = new List<Client>();
            command.CommandText = "select distinct v.num_c, v.nom, v.adresse from client v";
            MySqlDataReader reader;
            reader = command.ExecuteReader();

            while (reader.Read()) 
            {
                //for (int i = 0; i < reader.FieldCount; i++) // parcours cellule par cellule
                //{
                    listeC.Add(
                        new Client(reader.GetString(0),reader.GetString(1),reader.GetString(2))
                   );
                //}
                //Console.WriteLine(currentRowAsString); // affichage de la ligne ( sous forme d'une " grosse "string ) sur la sortie standard
            }
            connection.Close();
            return listeC;
        }
        public static List<Voiture> InstancieListeVoitureFromBDD(string connectionString)
        {
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            List<Voiture> listeV = new List<Voiture>();
            command.CommandText = "select distinct immat, disponible, motif, marque, modele, nbr_places from voiture";
            MySqlDataReader reader;
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                    bool dispo = false;
                    string motif = "";
                    if (reader.GetString(1) == "True") dispo = true;
                    if (reader.GetValue(2) != null) motif = reader.GetValue(2).ToString();
                    listeV.Add(
                        new Voiture(reader.GetString(0), dispo, motif, reader.GetString(3), reader.GetString(4), (int)reader.GetValue(5))
                   );
            }
            connection.Close();
            return listeV;
        }
        public static List<Parking> InstancieListeParkingFromBDD(string connectionString)
        {
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            List<Parking> listeP = new List<Parking>();
            command.CommandText = "select distinct id_p,nom,adresse,arrond from parking";
            MySqlDataReader reader;
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                listeP.Add(
                    new Parking(reader.GetString(0), reader.GetString(1),reader.GetString(2), (int)reader.GetValue(3))
                );
               
            }
            connection.Close();
            return listeP;
        }
        public static List<Sejour> InstancieListeSejourFromBDD(string connectionString)
        {
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            List<Sejour> listeP = new List<Sejour>();
            command.CommandText = "select distinct id_s,description,id_t from sejour";
            MySqlDataReader reader;
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                listeP.Add(
                    new Sejour(reader.GetString(0), reader.GetString(1), reader.GetString(2))
                );

            }
            connection.Close();
            return listeP;
        }
        public static Sejour returnSejourFromId(List<Sejour> sejour, string id_sejour)
        {
            for (int i=0;i< sejour.Count(); i++)
            {
                if (sejour[i].Id_sejour == id_sejour) return sejour[i];
            }
            return null;
        }
        public static List<Theme> InstancieListeThemeFromBDD(string connectionString)
        {
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            List<Theme> listeT = new List<Theme>();
            command.CommandText = "select distinct id_t,nom,arrond,description from theme";
            MySqlDataReader reader;
            reader = command.ExecuteReader();

            while (reader.Read())
            {
                string description = "";
                if (reader.GetValue(3) != null) description = reader.GetValue(2).ToString();
                listeT.Add(
                    new Theme(reader.GetString(0), reader.GetString(1), (int)reader.GetValue(2), description)
               );
            }
            connection.Close();
            return listeT;
        }
        public static List<Ranger> InstancieListeRangerFromBDD(string connectionString)
        {
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            List<Ranger> listeR = new List<Ranger>();
            command.CommandText = "select distinct id_p, immat, date_r,num_place from ranger";
            MySqlDataReader reader;
            reader = command.ExecuteReader();

            while (reader.Read())
            {
                listeR.Add(
                    new Ranger(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3))
               );
            }
            connection.Close();
            return listeR;
        }


        public static Sejour Message1(List<Client> listeC, List<Sejour>listeS, List<Theme> listeT, Client un_client)
        {
            XmlDocument docXml = new XmlDocument();
            // création de l en tête XML (no <= > pas de DTD associée)
            string num_client=E2(listeC, un_client);
            docXml.CreateXmlDeclaration("1.0", "UTF -8", "no");
            XmlElement racine = docXml.CreateElement("M1");
            docXml.AppendChild(racine);
            XmlElement autreBalise = docXml.CreateElement("NomClient");
            autreBalise.InnerText =un_client.Nom;
            racine.AppendChild(autreBalise);
            XmlElement deuxiemeBalise = docXml.CreateElement("AdresseClient");
            deuxiemeBalise.InnerText = un_client.Adresse;
            racine.AppendChild(deuxiemeBalise);
            XmlElement troisiemeBalise = docXml.CreateElement("Date");
            troisiemeBalise.InnerText = "14";
            racine.AppendChild(troisiemeBalise);
            XmlElement quatriemeBalise = docXml.CreateElement("Sejour");
            quatriemeBalise.InnerText = "visite de la Defense";
            racine.AppendChild(quatriemeBalise);
            docXml.Save("M1.xml");
            Theme new_theme = new Theme("pa2", "Les immeubles", 16, "pour l'exo PFR");
            listeT.Add(new_theme);
            Sejour new_sejour = new Sejour("11", "visite de la Defense", new_theme.Id_theme);
            listeS.Add(new_sejour);
            return new_sejour;
        }
        public static string E2(List<Client> listeC, Client un_client)
        {
            for (int i = 0; i < listeC.Count(); i++)
            {
                if (listeC[i].Nom != un_client.Nom || listeC[i].Num_c != un_client.Num_c || listeC[i].Adresse != un_client.Adresse)
                {
                    listeC.Add(un_client);
                }
            }
            return un_client.Num_c;
        }
        public static Ranger E3(string connectionString, List<Ranger> listeR)
        {
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "select r.immat from ranger r,parking p, voiture v where r.id_p=p.id_p and p.arrond=16 and v.immat=r.immat and v.disponible=true ";
            MySqlDataReader reader;
            reader = command.ExecuteReader();
            Ranger voiture_dispo = null;
            bool sort=false;
            int compteur = 0;
            while (reader.Read())
            {
                compteur++;
                for(int i = 0; i < listeR.Count(); i++)
                {
                    if (listeR[i].Immat == reader.GetString(0) && !sort)
                    {
                        voiture_dispo=listeR[i];
                        sort = true;
                    }
                }

            }
            if (compteur == 0)
            {
                connection.Close();
                connection = new MySqlConnection(connectionString);
                connection.Open();
                command = connection.CreateCommand();
                command.CommandText = "select r.immat from ranger r,voiture v where v.immat=r.immat and v.disponible=true ";
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    for (int i = 0; i < listeR.Count(); i++)
                    {
                        if (listeR[i].Immat == reader.GetString(0)) voiture_dispo = listeR[i];
                    }
                }
            }
            connection.Close();
            return voiture_dispo;
        }
        public static List<String> E5()
        {
            StreamReader reader = new StreamReader("ReponseRBNP.json");
            JsonTextReader jreader = new JsonTextReader(reader);
            List<string> liste=new List<string>();
            int compteur = 0;
            while (jreader.Read())
            {
                if (jreader.TokenType.ToString().ToLower() == "startobject")
                {
                    for (int i = 0; i < liste.Count(); i++)
                    {
                        if (liste[i] == "availability" && liste[i + 1] == "yes") compteur++;
                        if (liste[i] == "borough" && int.Parse(liste[i + 1]) == 16) compteur++;
                        if (liste[i] == "overall_satisfaction" && Convert.ToDouble(liste[i + 1]) >= 4.5) compteur++;
                        if (liste[i] == "bedrooms" &&Convert.ToDouble(liste[i + 1]) == 1) compteur++;
                    }
                    if (compteur == 4) return liste;
                    liste = new List<string>();
                    compteur = 0;
                }
                if (jreader.Value != null && (jreader.TokenType.ToString().ToLower() == "propertyname" || jreader.TokenType.ToString().ToLower() =="float" || jreader.TokenType.ToString().ToLower() == "string" || jreader.TokenType.ToString().ToLower() == "integer"))
                {
                    liste.Add(jreader.Value.ToString());
                }
            }
            jreader.Close();
            reader.Close();
            return null;
        }
        public static void Message2(string connectionString, Sejour sejour_selectionne,List<Theme> listeT, Client adherent, Ranger voiture_selectionne)
        {
            Theme theme_sejour = null;
            for(int i = 0; i < listeT.Count(); i++)
            {
                if (sejour_selectionne.Theme_sejour == listeT[i].Id_theme) theme_sejour = listeT[i];
            }
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            List<Client> listeC = new List<Client>();
            command.CommandText = "select p.nom, r.num_place from parking p, ranger r where r.id_p=p.id_p and r.immat='"+voiture_selectionne.Immat+"'";
            MySqlDataReader reader;
            reader = command.ExecuteReader();
            string nom_parking="";
            string num_place = "";
            while (reader.Read())
            {
                nom_parking= reader.GetString(0);
                num_place = reader.GetString(1);
                break;
            }

            XmlDocument docXml = new XmlDocument();
            docXml.CreateXmlDeclaration("1.0", "UTF -8", "no");
            XmlElement racine = docXml.CreateElement("M2");
            docXml.AppendChild(racine);
            XmlElement autreBalise = docXml.CreateElement("NumeroDeSejour");
            autreBalise.InnerText = sejour_selectionne.Id_sejour;
            racine.AppendChild(autreBalise);
            XmlElement deuxiemeBalise = docXml.CreateElement("Adherent");
            racine.AppendChild(deuxiemeBalise);
            XmlElement baliseAdherentNom = docXml.CreateElement("Nom");
            baliseAdherentNom.InnerText = adherent.Nom;
            deuxiemeBalise.AppendChild(baliseAdherentNom);
            XmlElement baliseAdherentnum = docXml.CreateElement("Numero");
            baliseAdherentnum.InnerText = adherent.Num_c;
            deuxiemeBalise.AppendChild(baliseAdherentnum);
            XmlElement nomTheme = docXml.CreateElement("NomTheme");
            nomTheme.InnerText = theme_sejour.Nom_theme;
            racine.AppendChild(nomTheme);
            XmlElement troisiemeBalise = docXml.CreateElement("Date");
            troisiemeBalise.InnerText = "14";
            racine.AppendChild(troisiemeBalise);
            XmlElement cinquiemeBalise = docXml.CreateElement("InfoValidation");
            cinquiemeBalise.InnerText = "Sejour validé";
            racine.AppendChild(cinquiemeBalise);
            XmlElement parking = docXml.CreateElement("Parking");
            racine.AppendChild(parking);
            XmlElement Nom = docXml.CreateElement("Nom");
            Nom.InnerText =nom_parking;
            parking.AppendChild(Nom);
            XmlElement numeroPlace = docXml.CreateElement("NumeroPlace");
            numeroPlace.InnerText = num_place;
            parking.AppendChild(numeroPlace);
            XmlElement immat = docXml.CreateElement("ImmatriculationVoiture");
            immat.InnerText = voiture_selectionne.Immat;
            parking.AppendChild(immat);
            XmlElement PropositionAppartement = docXml.CreateElement("PropositionAppartement");
            racine.AppendChild(PropositionAppartement);
            docXml.Save("M2.xml");
        }



        public static void Exo1()
        {
            string fileName = "bdtheque.xml";
            XPathDocument doc = new XPathDocument(fileName);
            XPathNavigator nav = doc.CreateNavigator();
            XPathExpression expr;
            expr = nav.Compile("bdtheque/BD");

            XPathNodeIterator nodes = nav.Select(expr);
            while (nodes.MoveNext())
            {
                /* à compléter ...
                en regardant les différentes propri étés et méthodes publiques
                disponibles depuis l'objet nodes . Current (de type XPathNavigator )
                */
                string isbn = nodes.Current.GetAttribute("isbn", "");
                nodes.Current.MoveToFirstChild();
                string titre = nodes.Current.Value;
                nodes.Current.MoveToNext();
                string auteur = nodes.Current.Value;
                bool nbPagesExiste = nodes.Current.MoveToNext();
                string nbPages = "";
                if (nbPagesExiste)
                {
                    nbPages = nodes.Current.Value;
                }
                Console.WriteLine(titre + "(" + nbPages + " pages), ecrit par " + auteur + ", numero ISBN : " + isbn);
            }
        }

    }
}
