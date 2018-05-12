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
            List<Voiture> listeV= InstancieListeVoitureFromBDD(connectionString);
            List<Client> listeC=InstancieListeClientFromBDD(connectionString);
            List<Parking> listeP = InstancieListeParkingFromBDD(connectionString);
            List<Sejour> listeS = InstancieListeSejourFromBDD(connectionString);
            List<Theme> listeT = InstancieListeThemeFromBDD(connectionString);
            Sejour sejour_client=Message1(listeC,listeS,listeT);
            E3(listeP,listeT,sejour_client);
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
                //for (int i = 0; i < reader.FieldCount; i++) 
                //{
                    bool dispo = false;
                    string motif = "";
                    if (reader.GetString(1) == "true") dispo = true;
                    if (reader.GetValue(2) != null) motif = reader.GetValue(2).ToString();
                    listeV.Add(
                        new Voiture(reader.GetString(0), dispo, motif, reader.GetString(3), reader.GetString(4), (int)reader.GetValue(5))
                   );
                //}
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
        

        public static Sejour Message1(List<Client> listeC, List<Sejour>listeS, List<Theme> listeT)
        {
            XmlDocument docXml = new XmlDocument();
            // création de l en tête XML (no <= > pas de DTD associée)
            Client client = new Client("20-5", "Tucoulou", "ESILV, La Defense");
            string num_client=E2(listeC, client);
            docXml.CreateXmlDeclaration("1.0", "UTF -8", "no");
            XmlElement racine = docXml.CreateElement("M1");
            docXml.AppendChild(racine);
            XmlElement autreBalise = docXml.CreateElement("NomClient");
            autreBalise.InnerText =client.Nom;
            racine.AppendChild(autreBalise);
            XmlElement deuxiemeBalise = docXml.CreateElement("AdresseClient");
            deuxiemeBalise.InnerText = client.Adresse;
            racine.AppendChild(deuxiemeBalise);
            XmlElement troisiemeBalise = docXml.CreateElement("Date");
            troisiemeBalise.InnerText = "14";
            XmlElement quatriemeBalise = docXml.CreateElement("Sejour");
            quatriemeBalise.InnerText = "visite de la Defense";
            racine.AppendChild(quatriemeBalise);
            docXml.Save("M1.xml");
            Theme new_theme = new Theme("pa2", "rue leonard de vinci", 16, "pour l'exo PFR");
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
        public static void E3(List<Parking> listeP, List<Theme> listeT, Sejour sejour)
        {
            Theme theme_sejour = null;
            for (int i = 0; i < listeT.Count(); i++)
            {
                if (listeT[i].Id_theme == sejour.Theme_sejour) theme_sejour = listeT[i];
            }
            if (theme_sejour!=null)
            {
                Parking parking_selectionne = null;
                for (int i = 0; i < listeP.Count(); i++)
                {
                    if (listeP[i].Arrondissement == theme_sejour.ArrondissementT &&  ) parking_selectionne = listeP[i];
                }
            }
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
        public static void Exo6()
        {
            StreamReader reader = new StreamReader("chiens.json");
            JsonTextReader jreader = new JsonTextReader(reader);
            while (jreader.Read())
            {
                // il y a deux sortes de token : avec une valeur associ ée ou non
                if (jreader.Value != null)
                {
                    Console.WriteLine(jreader.TokenType.ToString() + " " + jreader.Value);
                }
                else
                {
                    Console.WriteLine(jreader.TokenType.ToString());
                }
            }
            jreader.Close();
            reader.Close();
        }
        public static void Exo4()
        {
            //BD bd11 = new BD("978 - 2203001169 ", " On a march é sur la Lune ", 62);
            //Console.WriteLine(bd11); // affichage pour dé bug
            // Code pour sérialiser l'objet bd11 en XML dans un fichier "bd11.xml"
            //XmlSerializer xs = new XmlSerializer(typeof(BD)); // l outil desérialisation
            //StreamWriter wr = new StreamWriter("bd11.xml"); // accès en écriture un fichier ( texte )
            //xs.Serialize(wr, bd11); // action desérialiser en XML l'objet bd11
            //wr.Close(); // et d'écrire le résultat dans le fichier manipulé par wr
        }
        public static void Exo5()
        {
            //BD bd11 = null;
            // Désé rialisation . . .
            //XmlSerializer xs = new XmlSerializer(typeof(BD));
            //StreamReader rd = new StreamReader("bd11.xml");
            //BDtheque bdtheque = xs.Deserialize(rd) as BDtheque;
            //rd.Close();
            // Bilan :
            //Console.WriteLine(bd11); // affichage pour contrÃler le contenu de l objet bd11
        }
        public static void AfficherPrettyJson(string fichier)  //Archi pas fini, voir sur moodle corrigé
        {
            StreamReader reader = new StreamReader(fichier);
            JsonTextReader jreader = new JsonTextReader(reader);
            while (jreader.Read())
            {
                if (jreader.TokenType.ToString() == "StartObject")
                {
                    Console.WriteLine("Nouvel Objet\n\n------------\n\n");
                }
                if (jreader.Value != null)
                {
                    Console.WriteLine(jreader.Value);
                }
            }
        }
    }
}
