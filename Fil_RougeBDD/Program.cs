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
            // Bien verifier , via Workbench par exemple , que ces parametres de connexion sont valides !!!
            string connectionString = " SERVER = fboisson.ddns.net ; PORT = 3306; DATABASE = TUCO_THIB; UID = S6-TUCO-THIB;PASSWORD = 8441;";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            List<Client> listeV = new List<Client>();
            //command.CommandText = " SELECT distinct v.immat,v.modele, v.marque, v.achatA, v.compteur,p.pseudo from voiture v, proprietaire p where p.codeP=v.codeP";
            MySqlDataReader reader;
            reader = command.ExecuteReader();

            while (reader.Read()) // parcours ligne par ligne
            {
                //string currentRowAsString = "";
                for (int i = 0; i < reader.FieldCount; i++) // parcours cellule par cellule
                {
                    //string valueAsString = reader.GetValue(i).ToString(); // recuperation de la valeur de chaque cellule sous forme d' une string ( voir cependant les differentes methodes disponibles !!)
                    //currentRowAsString +=" "+valueAsString;
                    listeV.Add(
                        new Client(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetInt32(3), reader.GetInt32(4), reader.GetString(5))
                   );
                }
                //Console.WriteLine(currentRowAsString); // affichage de la ligne ( sous forme d'une " grosse "string ) sur la sortie standard
            }
            connection.Close();
            Console.ReadKey();
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
        public static void Exo2()
        {
            XmlDocument docXml = new XmlDocument();
            // création de l en tête XML (no <= > pas de DTD associée)
            docXml.CreateXmlDeclaration("1.0", "UTF -8", "no");
            XmlElement racine = docXml.CreateElement("BD");
            docXml.AppendChild(racine);
            XmlElement autreBalise = docXml.CreateElement("titre");
            autreBalise.InnerText = "On a marché sur la Lune";
            racine.AppendChild(autreBalise);
            XmlElement deuxiemeBalise = docXml.CreateElement("auteur");
            deuxiemeBalise.InnerText = "Hergé";
            racine.AppendChild(deuxiemeBalise);
            XmlElement troisiemeBalise = docXml.CreateElement("nb_pages");
            troisiemeBalise.InnerText = "62";
            racine.AppendChild(troisiemeBalise);
            docXml.Save("test.xml");
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
    }
}
