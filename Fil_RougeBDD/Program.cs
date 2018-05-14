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
            List<Reserver> listeReserver = InstancieListeReserverFromBDD(connectionString);
            Sejour sejour_client=Message1(listeC,listeS,listeT,un_client);
            Ranger voiture_dispo= E3(connectionString,listeR);
            if (R1(voiture_dispo))
            {
                List<List<string>> Appartements_valides = E5();
                if (J3(Appartements_valides))
                {
                    Message2(connectionString, sejour_client, listeReserver, listeT, un_client, voiture_dispo, Appartements_valides);
                    Message3(sejour_client, un_client, Appartements_valides, listeReserver,listeV,voiture_dispo);
                    Message4(sejour_client, un_client, listeT);
                }
                else Console.WriteLine("Pas d'appartement disponible conforme à votre recherche.");
            }
            else Console.WriteLine("Pas de voiture disponible...");
            Console.ReadKey();
        }

        public static List<Client> InstancieListeClientFromBDD(string connectionString)
        {
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            List<Client> listeC = new List<Client>();
            command.CommandText = "select v.num_c, v.nom, v.adresse from client v";
            MySqlDataReader reader;
            reader = command.ExecuteReader();

            while (reader.Read()) 
            {
                    listeC.Add(
                        new Client(reader.GetString(0),reader.GetString(1),reader.GetString(2))
                   );
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
            command.CommandText = "select immat, disponible, motif, marque, modele, nbr_places from voiture";
            MySqlDataReader reader;
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                    bool dispo = false;
                    string motif = "";
                    if (reader.GetString(1).ToLower() == "true") dispo = true;
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
            command.CommandText = "select id_p,nom,adresse,arrond from parking";
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
            command.CommandText = "select id_s,description,id_t from sejour";
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
        public static Voiture returnVoitureFromRanger(Ranger voiture, List<Voiture> listeV)
        {
            for (int i = 0; i < listeV.Count(); i++)
            {
                if (listeV[i].Immat == voiture.Immat) return listeV[i];
            }
            return null;
        }
        public static List<Theme> InstancieListeThemeFromBDD(string connectionString)
        {
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            List<Theme> listeT = new List<Theme>();
            command.CommandText = "select id_t,nom,arrond,description from theme";
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
            command.CommandText = "select id_p, immat, date_r,num_place from ranger";
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
        public static List<Reserver> InstancieListeReserverFromBDD(string connectionString)
        {
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            List<Reserver> listeReserver = new List<Reserver>();
            command.CommandText = "select num_c, id_s, date_r,confirme,note from reserver";
            MySqlDataReader reader;
            reader = command.ExecuteReader();
            bool confirme = true;                //Parce que les reservations présentes dans la base de donnée sont considérées déjà reservées et validées.
            double note = -1;                    //Pas de notes pour les reservations de la base de donnée donc je les mets à -1 pour les differencier des reservations validées.
            while (reader.Read())
            {
                listeReserver.Add(
                    new Reserver(reader.GetString(0), reader.GetString(1), reader.GetString(2), confirme,note)
               );
            }
            connection.Close();
            return listeReserver;
        }
        public static bool R1(Ranger voiture)
        {
            if (voiture != null) return true;
            else return false;
        }
        public static bool J3(List<List<string>> Appartements_valides)
        {
            if (Appartements_valides != null) return true;
            else return false;
        }


        public static Sejour Message1(List<Client> listeC, List<Sejour>listeS, List<Theme> listeT, Client un_client)
        {
            XmlDocument docXml = new XmlDocument();
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
        public static List<List<string>> E5()
        {
            StreamReader reader = new StreamReader("ReponseRBNP.json");
            JsonTextReader jreader = new JsonTextReader(reader);
            List<string> liste=new List<string>();
            List<List<string>> maliste = new List<List<string>>();
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
                    if (compteur == 4) maliste.Add(liste);
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
            return maliste;
        }
        public static void Message2(string connectionString, Sejour sejour_selectionne,List<Reserver>listeReserver, List<Theme> listeT, Client adherent, Ranger voiture_selectionne, List<List<string>> appartements)
        {
            listeReserver.Add(new Reserver(adherent.Num_c,sejour_selectionne.Id_sejour,"14",false,-1));
            Theme theme_sejour = null;
            for (int i = 0; i < listeT.Count(); i++)
            {
                if (sejour_selectionne.Theme_sejour == listeT[i].Id_theme) theme_sejour = listeT[i];
            }
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            List<Client> listeC = new List<Client>();
            command.CommandText = "select p.nom, r.num_place from parking p, ranger r where r.id_p=p.id_p and r.immat='" + voiture_selectionne.Immat + "'";
            MySqlDataReader reader;
            reader = command.ExecuteReader();
            string nom_parking = "";
            string num_place = "";
            while (reader.Read())
            {
                nom_parking = reader.GetString(0);
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
            Nom.InnerText = nom_parking;
            parking.AppendChild(Nom);
            XmlElement numeroPlace = docXml.CreateElement("NumeroPlace");
            numeroPlace.InnerText = num_place;
            parking.AppendChild(numeroPlace);
            XmlElement immat = docXml.CreateElement("ImmatriculationVoiture");
            immat.InnerText = voiture_selectionne.Immat;
            parking.AppendChild(immat);
            XmlElement PropositionAppartement = docXml.CreateElement("PropositionsAppartements");
            racine.AppendChild(PropositionAppartement);

            XmlElement Proposition1 = docXml.CreateElement("Proposition1");
            PropositionAppartement.AppendChild(Proposition1);
            XmlElement Host_id1 = docXml.CreateElement("host_id");
            Proposition1.AppendChild(Host_id1);
            Host_id1.InnerText = appartements[0][1];
            XmlElement Proposition2 = docXml.CreateElement("Proposition2");
            PropositionAppartement.AppendChild(Proposition2);
            XmlElement Host_id2 = docXml.CreateElement("host_id");
            Proposition2.AppendChild(Host_id2);
            Host_id2.InnerText = appartements[1][1];
            XmlElement Proposition3 = docXml.CreateElement("Proposition3");
            PropositionAppartement.AppendChild(Proposition3);
            XmlElement Host_id3 = docXml.CreateElement("host_id");
            Proposition3.AppendChild(Host_id3);
            Host_id3.InnerText = appartements[2][1];
            XmlElement prix1 = docXml.CreateElement("prix");
            Proposition1.AppendChild(prix1);
            prix1.InnerText = appartements[0][19];
            XmlElement prix2 = docXml.CreateElement("prix");
            Proposition2.AppendChild(prix2);
            prix2.InnerText = appartements[1][19];
            XmlElement prix3 = docXml.CreateElement("prix");
            Proposition3.AppendChild(prix3);
            prix3.InnerText = appartements[2][19];
            docXml.Save("M2.xml");
        }
        public static void Message3(Sejour sejour_selectionne,Client adherent, List<List<string>> appartements, List<Reserver> listeReserver, List<Voiture>listeV, Ranger voiture_dispo)
        {
            XmlDocument docXml = new XmlDocument();
            docXml.CreateXmlDeclaration("1.0", "UTF -8", "no");
            XmlElement racine = docXml.CreateElement("M3");
            docXml.AppendChild(racine);
            XmlElement autreBalise = docXml.CreateElement("NumeroDeSejour");
            autreBalise.InnerText = sejour_selectionne.Id_sejour;
            racine.AppendChild(autreBalise);
            XmlElement cinquiemeBalise = docXml.CreateElement("InfoValidation");
            cinquiemeBalise.InnerText = "Sejour validé";
            racine.AppendChild(cinquiemeBalise);
            XmlElement reference_appart = docXml.CreateElement("Reference_appartement_choisi");
            reference_appart.InnerText =appartements[0][1] ;
            racine.AppendChild(reference_appart);
            docXml.Save("M3.xml");

            for (int i=0;i< listeReserver.Count(); i++)
            {
                if (listeReserver[i].Num_c == adherent.Num_c && listeReserver[i].Id_s == sejour_selectionne.Id_sejour && listeReserver[i].Date_r == "14") listeReserver[i].Confirme = true;
            }
            Voiture voiture = returnVoitureFromRanger(voiture_dispo, listeV);
            if (voiture != null)
            {
                voiture.Disponible = false;
                voiture.Motif = "reservee";
            }
        }
        public static void Message4(Sejour sejour_selectionne, Client adherent, List<Theme> listeT)
        {
            Theme theme_sejour = null;
            for (int i = 0; i < listeT.Count(); i++)
            {
                if (sejour_selectionne.Theme_sejour == listeT[i].Id_theme) theme_sejour = listeT[i];
            }
            XmlDocument docXml = new XmlDocument();
            docXml.CreateXmlDeclaration("1.0", "UTF -8", "no");
            XmlElement racine = docXml.CreateElement("M4");
            docXml.AppendChild(racine);
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
            cinquiemeBalise.InnerText = "séjour impossible, veuillez choisir une autre date";
            racine.AppendChild(cinquiemeBalise);
            docXml.Save("M4.xml");
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
