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
            int mon_id_client = E2(connectionString);
            int id_sejour_choisi=Message1(connectionString);
            List<string> liste_info_client = returnInfoClient();
            List<string> liste_info_sejour = returnInfoSejour();
            string voiture_dispo = E3(connectionString);
            List<List<string>> appartements_valides = E5();
            if (R1(voiture_dispo))
            {
                List<List<string>> Appartements_valides = E5();
                if (J3(appartements_valides))
                {
                    Message2(connectionString,mon_id_client,liste_info_sejour,liste_info_client,id_sejour_choisi,voiture_dispo,appartements_valides);
                    //Message3(sejour_client, un_client, Appartements_valides, listeReserver,listeV,voiture_dispo);
                    //Message4(sejour_client, un_client, listeT);
                }
                else Console.WriteLine("Pas d'appartement disponible conforme à votre recherche.");
            }
            else Console.WriteLine("Pas de voiture disponible...");
            Console.ReadKey();
        }

        public static int E2(string connectionString)
        {

            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "select num_c from client where nom='Tucoulou' and adresse='ESILV, la défense'";
            MySqlDataReader reader;
            reader = command.ExecuteReader();
            int mon_id = -1;
            if (reader.Read())
            {
                mon_id = reader.GetInt16(0);
            }
            else
            {
                connection.Close();
                connection.Open();
                command.CommandText = "select max(num_c) from client";
                reader = command.ExecuteReader();
                reader.Read();
                mon_id = reader.GetInt16(0)+1;
                connection.Close();
                connection.Open();
                command.CommandText = "insert into client values ("+mon_id+ ",'Tucoulou','ESILV, la défense')";
                reader = command.ExecuteReader();
                reader.Read();
            }
            connection.Close();
            return mon_id;
        }
        public static int Message1(string connectionString)
        {
            int id_sejour = -1;
            string nom_sejour = "visite de la Defense";
            int arrondissment = 16;
            string nom_theme = "les grands immeubles";
            string id_theme = "vDef";
            string date = "14";
            string description_theme = "les avenues ";
            XmlDocument docXml = new XmlDocument();
            int num_client = E2(connectionString);
            docXml.CreateXmlDeclaration("1.0", "UTF -8", "no");
            XmlElement racine = docXml.CreateElement("M1");
            docXml.AppendChild(racine);
            XmlElement autreBalise = docXml.CreateElement("NomClient");
            autreBalise.InnerText = "Tucoulou";
            racine.AppendChild(autreBalise);
            XmlElement deuxiemeBalise = docXml.CreateElement("AdresseClient");
            deuxiemeBalise.InnerText = "ESILV, la défense";
            racine.AppendChild(deuxiemeBalise);
            XmlElement troisiemeBalise = docXml.CreateElement("Date");
            troisiemeBalise.InnerText = date;
            racine.AppendChild(troisiemeBalise);
            XmlElement quatriemeBalise = docXml.CreateElement("Sejour");
            quatriemeBalise.InnerText = nom_sejour;
            racine.AppendChild(quatriemeBalise);
            docXml.Save("M1.xml");
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "select id_s from sejour where description='"+nom_sejour+"'";
            MySqlDataReader reader;
            reader = command.ExecuteReader();
            if(reader.Read())
            {
                id_sejour = reader.GetInt32(0);
            }
            else
            {
                connection.Close();
                connection.Open();
                command.CommandText = "insert into theme values ('"+id_theme+"','"+nom_theme+"',"+arrondissment+",'"+description_theme+"')";
                reader = command.ExecuteReader();
                reader.Read();
                connection.Close();
                connection.Open();
                command.CommandText = "select count(*) from sejour";
                reader = command.ExecuteReader();
                reader.Read();
                id_sejour = (reader.GetInt16(0) + 1);
                connection.Close();
                connection.Open();
                command.CommandText = "insert into sejour values ("+id_sejour+",'"+ nom_sejour + "','"+id_theme+"')";
                reader = command.ExecuteReader();
                reader.Read();
            }
            connection.Close();
            return id_sejour;
        }
        public static List<string> returnInfoClient()
        {
            List<string> listeInfo_sejour = new List<string>();
            string fileName = "M1.xml";
            XPathDocument doc = new XPathDocument(fileName);
            XPathNavigator nav = doc.CreateNavigator();
            XPathExpression expr;
            expr = nav.Compile("M1");
            XPathNodeIterator nodes = nav.Select(expr);
            nodes.Current.MoveToFirstChild();
            nodes.Current.MoveToChild("NomClient", "");
            string nom = nodes.Current.Value;
            nodes.Current.MoveToParent();
            nodes.Current.MoveToChild("AdresseClient","");
            string adresse = nodes.Current.Value;
            listeInfo_sejour.Add(nom);
            listeInfo_sejour.Add(adresse);
            return listeInfo_sejour;
        }
        public static List<string> returnInfoSejour()
        {
            List<string> listeInfo_sejour = new List<string>();
            string fileName = "M1.xml";
            XPathDocument doc = new XPathDocument(fileName);
            XPathNavigator nav = doc.CreateNavigator();
            XPathExpression expr;
            expr = nav.Compile("M1");
            XPathNodeIterator nodes = nav.Select(expr);
            nodes.Current.MoveToFirstChild();
            nodes.Current.MoveToChild("Date", "");
            string date = nodes.Current.Value;
            nodes.Current.MoveToParent();
            nodes.Current.MoveToChild("Sejour", "");
            string sejour = nodes.Current.Value;
            listeInfo_sejour.Add(date);
            listeInfo_sejour.Add(sejour);
            return listeInfo_sejour;
        }
        public static string E3(string connectionString)
        {
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "select r.immat from ranger r,parking p, voiture v where r.id_p=p.id_p and p.arrond=16 and v.immat=r.immat and v.disponible=true ";
            MySqlDataReader reader;
            reader = command.ExecuteReader();
            string voiture_dispo = "";
            if(reader.Read())
            {
                voiture_dispo= reader.GetString(0);
            }
            else
            {
                connection.Close();
                connection.Open();
                command.CommandText = "select r.immat from ranger r,voiture v where v.immat=r.immat and v.disponible=true ";
                reader = command.ExecuteReader();
                if(reader.Read())
                {
                    voiture_dispo = reader.GetString(0);
                }
            }
            connection.Close();
            return voiture_dispo;
        }
        public static List<List<string>> E5()
        {
            StreamReader reader = new StreamReader("ReponseRBNP.json");
            JsonTextReader jreader = new JsonTextReader(reader);
            List<string> liste = new List<string>();
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
                        if (liste[i] == "bedrooms" && Convert.ToDouble(liste[i + 1]) == 1) compteur++;
                    }
                    if (compteur == 4) maliste.Add(liste);
                    liste = new List<string>();
                    compteur = 0;
                }
                if (jreader.Value != null && (jreader.TokenType.ToString().ToLower() == "propertyname" || jreader.TokenType.ToString().ToLower() == "float" || jreader.TokenType.ToString().ToLower() == "string" || jreader.TokenType.ToString().ToLower() == "integer"))
                {
                    liste.Add(jreader.Value.ToString());
                }
            }
            jreader.Close();
            reader.Close();
            return maliste;
        }
        public static bool R1(string voiture)
        {
            if (voiture != "") return true;
            else return false;
        }
        public static bool J3(List<List<string>> Appartements_valides)
        {
            if (Appartements_valides != null) return true;
            else return false;
        }
        public static void Message2(string connectionString,int id_client, List<string> liste_sejour, List<string> liste_client, int id_sejour_choisi, string voiture_dispo, List<List<string>> appartements_valides)
        {
            //listeReserver.Add(new Reserver(adherent.Num_c, sejour_selectionne.Id_sejour, "14", false, -1));
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "insert into reserver values ("+id_client+","+id_sejour_choisi+",'"+liste_sejour[0]+"',false,-1)";
            MySqlDataReader reader;
            reader = command.ExecuteReader();Console.WriteLine("haa"); Console.ReadKey();



            command.CommandText = "select ";
            reader = command.ExecuteReader();
            string nom_parking = "";
            string num_place = "";
            while (reader.Read())
            {
                nom_parking = reader.GetString(0);
                num_place = reader.GetString(1);
                break;
            }
            /*
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
            baliseAdherentNom.InnerText = liste_client[0];
            deuxiemeBalise.AppendChild(baliseAdherentNom);
            XmlElement baliseAdherentnum = docXml.CreateElement("Numero");
            baliseAdherentnum.InnerText = id_client;
            deuxiemeBalise.AppendChild(baliseAdherentnum);
            XmlElement nomTheme = docXml.CreateElement("NomTheme");
            nomTheme.InnerText = liste_sejour[1];
            racine.AppendChild(nomTheme);
            XmlElement troisiemeBalise = docXml.CreateElement("Date");
            troisiemeBalise.InnerText = liste_sejour[0];
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
            docXml.Save("M2.xml");*/
        }
        
        


       /*
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

        */
    }
}