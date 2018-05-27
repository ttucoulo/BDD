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
            List<string> liste_info_client = P1_client();
            List<string> liste_info_sejour = P1_sejour();
            Console.WriteLine("Début de la démo\n\nL'utilisateur souhaite reserver un sejour et a donc entré les critères de selection d'un sejour.\n\n");
            Console.WriteLine("Appuyez sur la barre d'espace pour la selection d'une voiture disponible"); Console.ReadKey();
            string voiture_dispo = E3(connectionString,mon_id_client,liste_info_sejour);
            Console.WriteLine("Appuyez sur la barre d'espace pour chercher un logement conforme à votre recherche."); Console.ReadKey();
            List<List<string>> appartements_valides = E5();
            if (R1(voiture_dispo))
            {
                J1(liste_info_sejour);
                List<List<string>> Appartements_valides = E5();

                if (J3(appartements_valides))
                {
                    Console.WriteLine("Appuyez sur la barre d'espace pour generer le XML M2 (message de confirmation)"); Console.ReadKey();
                    Message2(connectionString,mon_id_client,liste_info_sejour,liste_info_client,id_sejour_choisi,voiture_dispo,appartements_valides);
                    Console.WriteLine("Appuyez sur la barre d'espace pour generer le message M3 (validation du sejour)."); Console.ReadKey();
                    Message3(connectionString,mon_id_client,id_sejour_choisi,appartements_valides,liste_info_sejour, voiture_dispo);
                    Console.Clear();
                    Console.WriteLine("Check-Out\n\nL'utilisateur a garé la voiture sur une place\n\n.");
                    Console.WriteLine("Appuyez sur la barre d'espace pour enregistrer le placement de la voiture à rendre."); Console.ReadKey();
                    string voiture_deposee=enregistrementVehicule(connectionString, mon_id_client, voiture_dispo, liste_info_sejour, "P1", "A1");
                    Console.WriteLine("Appuyez sur la barre d'espace pour l'attribution d'une note au sejour."); Console.ReadKey();
                    attributionNote(connectionString, 4, mon_id_client, id_sejour_choisi, liste_info_sejour);
                    Console.WriteLine("Appuyez sur la barre d'espace pour passer au contrôle du vehicule rendu."); Console.ReadKey();
                    controleVehicule(connectionString, voiture_deposee);
                    entretien_et_mise_a_disponible(connectionString,voiture_deposee);
                    requetes_statistiques(connectionString,voiture_deposee,mon_id_client); Console.Clear();
                    Console.WriteLine("Fin de la démo.");
                }
                else Console.WriteLine("Pas d'appartement disponible conforme à votre recherche.");
            }
            else Console.WriteLine("Pas de voiture disponible...");
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
        public static List<string> P1_client()
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
        public static List<string> P1_sejour()
        {
            List<string> listeInfo_sejour = new List<string>();
            string fileName = "M1.xml";
            string ville = "La Défense";
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
            listeInfo_sejour.Add(ville);
            return listeInfo_sejour;
        }
        public static string E3(string connectionString, int client_id,List<string> liste_info_sejour)
        {
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "select r.immat, r.id_p from ranger r,parking p, voiture v where r.id_p=p.id_p and p.arrond=16 and v.immat=r.immat and v.disponible=true ";
            MySqlDataReader reader;
            reader = command.ExecuteReader();
            string voiture_dispo = "";
            string id_p = "";
            if(reader.Read())
            {
                voiture_dispo= reader.GetString(0);
                id_p = reader.GetString(1);
                connection.Close();
                connection.Open();
                command.CommandText = "delete from ranger where immat='" + voiture_dispo + "' and id_p='" + id_p + "' and date_r='18-01-" + liste_info_sejour[0] + "'";
                reader = command.ExecuteReader();
            }
            else
            {
                connection.Close();
                connection.Open();
                command.CommandText = "select r.immat,r.id_p from voiture v,ranger r where v.disponible=true and r.immat=v.immat ";
                reader = command.ExecuteReader();
                if(reader.Read())
                {
                    voiture_dispo = reader.GetString(0);
                    id_p = reader.GetString(1);
                    connection.Close();
                    connection.Open();
                    command.CommandText = "delete from ranger where immat='" + voiture_dispo + "' and id_p='" + id_p + "' and date_r='18-01-" + liste_info_sejour[0] + "'";
                    reader = command.ExecuteReader();
                }
            }
            connection.Close();
            connection.Open();
            command.CommandText = "select num_c from utiliser where num_c="+client_id+" and immat='"+voiture_dispo+ "' and date_d='18-01-" + liste_info_sejour[0] + "'";
            reader = command.ExecuteReader();
            if (reader.Read())
            {
                
            }
            else
            {
                connection.Close();
                connection.Open();
                command.CommandText = "insert into utiliser values("+client_id+",'"+voiture_dispo+"','18-01-"+liste_info_sejour[0]+"','18-01-25')";
                reader = command.ExecuteReader();
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
        public static bool J3(List<List<string>> appartements_valides)
        {
            if (appartements_valides != null) return true;
            else return false;
        }
        public static void Message2(string connectionString,int id_client, List<string> liste_sejour, List<string> liste_client, int id_sejour_choisi, string voiture_dispo, List<List<string>> appartements_valides)
        {
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();
            MySqlDataReader reader;
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "select num_c from reserver where num_c="+id_client+" and id_s="+id_sejour_choisi+" and date_r='18-01-"+liste_sejour[0]+"'";
            reader = command.ExecuteReader();
            if (reader.Read())
            {
            }
            else
            {
                connection.Close();
                connection.Open();
                command.CommandText = "insert into reserver values (" + id_client + "," + id_sejour_choisi + ",'18-01-" + liste_sejour[0] + "',false,-1)";
                reader = command.ExecuteReader();
                reader.Read();
            }
            string nom_parking = "";
            string num_place = "";
            connection.Close();
            connection.Open();
            command.CommandText = "select p.nom, r.num_place from ranger r, parking p where r.immat='"+voiture_dispo+"' and r.id_p=p.id_p";
            reader = command.ExecuteReader();
            reader.Read();
            if (reader.Read())
            {
                nom_parking = reader.GetString(0);
                num_place = reader.GetString(1);
            }
            XmlDocument docXml = new XmlDocument();
            docXml.CreateXmlDeclaration("1.0", "UTF -8", "no");
            XmlElement racine = docXml.CreateElement("M2");
            docXml.AppendChild(racine);
            XmlElement autreBalise = docXml.CreateElement("NumeroDeSejour");
            autreBalise.InnerText = id_sejour_choisi.ToString();
            racine.AppendChild(autreBalise);
            XmlElement deuxiemeBalise = docXml.CreateElement("Adherent");
            racine.AppendChild(deuxiemeBalise);
            XmlElement baliseAdherentNom = docXml.CreateElement("Nom");
            baliseAdherentNom.InnerText = liste_client[0];
            deuxiemeBalise.AppendChild(baliseAdherentNom);
            XmlElement baliseAdherentnum = docXml.CreateElement("Numero");
            baliseAdherentnum.InnerText = id_client.ToString();
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
            immat.InnerText = voiture_dispo;
            parking.AppendChild(immat);
            XmlElement PropositionAppartement = docXml.CreateElement("PropositionsAppartements");
            racine.AppendChild(PropositionAppartement);
            XmlElement Proposition1 = docXml.CreateElement("Proposition1");
            PropositionAppartement.AppendChild(Proposition1);
            XmlElement Host_id1 = docXml.CreateElement("host_id");
            Proposition1.AppendChild(Host_id1);
            Host_id1.InnerText = appartements_valides[0][1];
            XmlElement Proposition2 = docXml.CreateElement("Proposition2");
            PropositionAppartement.AppendChild(Proposition2);
            XmlElement Host_id2 = docXml.CreateElement("host_id");
            Proposition2.AppendChild(Host_id2);
            Host_id2.InnerText = appartements_valides[1][1];
            XmlElement Proposition3 = docXml.CreateElement("Proposition3");
            PropositionAppartement.AppendChild(Proposition3);
            XmlElement Host_id3 = docXml.CreateElement("host_id");
            Proposition3.AppendChild(Host_id3);
            Host_id3.InnerText = appartements_valides[2][1];
            XmlElement prix1 = docXml.CreateElement("prix");
            Proposition1.AppendChild(prix1);
            prix1.InnerText = appartements_valides[0][19];
            XmlElement prix2 = docXml.CreateElement("prix");
            Proposition2.AppendChild(prix2);
            prix2.InnerText = appartements_valides[1][19];
            XmlElement prix3 = docXml.CreateElement("prix");
            Proposition3.AppendChild(prix3);
            prix3.InnerText = appartements_valides[2][19];
            docXml.Save("M2.xml");
        }
        public static void Message3(string connectionString, int mon_id_client, int id_sejour_choisi, List<List<string>> appartements_valides, List<string> liste_info_sejour, string voiture_dispo)
        {
            XmlDocument docXml = new XmlDocument();
            docXml.CreateXmlDeclaration("1.0", "UTF -8", "no");
            XmlElement racine = docXml.CreateElement("M3");
            docXml.AppendChild(racine);
            XmlElement autreBalise = docXml.CreateElement("NumeroDeSejour");
            autreBalise.InnerText = id_sejour_choisi.ToString();
            racine.AppendChild(autreBalise);
            XmlElement cinquiemeBalise = docXml.CreateElement("InfoValidation");
            cinquiemeBalise.InnerText = "Sejour validé";
            racine.AppendChild(cinquiemeBalise);
            XmlElement reference_appart = docXml.CreateElement("Reference_appartement_choisi");
            reference_appart.InnerText = appartements_valides[0][1];
            racine.AppendChild(reference_appart);
            docXml.Save("M3.xml");

            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();
            MySqlDataReader reader;
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "update reserver set confirme=true where id_s="+id_sejour_choisi+" and num_c="+ mon_id_client+" and date_r='18-01-"+ liste_info_sejour[0] +"'";
            reader = command.ExecuteReader();

            connection.Close();
            connection.Open();
            command.CommandText = "update voiture set disponible=false where immat='"+voiture_dispo+"'";
            reader = command.ExecuteReader();

            connection.Close();
            connection.Open();
            command.CommandText = "update voiture set motif='reservée' where immat='" + voiture_dispo + "'";
            reader = command.ExecuteReader();
            
        }
        public static void Message4(int mon_id_client, List<string> liste_client, List<string> liste_info_sejour)
        {
            XmlDocument docXml = new XmlDocument();
            docXml.CreateXmlDeclaration("1.0", "UTF -8", "no");
            XmlElement racine = docXml.CreateElement("M4");
            docXml.AppendChild(racine);
            XmlElement deuxiemeBalise = docXml.CreateElement("Adherent");
            racine.AppendChild(deuxiemeBalise);
            XmlElement baliseAdherentNom = docXml.CreateElement("Nom");
            baliseAdherentNom.InnerText = liste_client[0];
            deuxiemeBalise.AppendChild(baliseAdherentNom);
            XmlElement baliseAdherentnum = docXml.CreateElement("Numero");
            baliseAdherentnum.InnerText = mon_id_client.ToString();
            deuxiemeBalise.AppendChild(baliseAdherentnum);
            XmlElement nomTheme = docXml.CreateElement("NomTheme");
            nomTheme.InnerText = liste_info_sejour[1];
            racine.AppendChild(nomTheme);
            XmlElement troisiemeBalise = docXml.CreateElement("Date");
            troisiemeBalise.InnerText = liste_info_sejour[0];
            racine.AppendChild(troisiemeBalise);
            XmlElement cinquiemeBalise = docXml.CreateElement("InfoValidation");
            cinquiemeBalise.InnerText = "séjour impossible, veuillez choisir une autre date";
            racine.AppendChild(cinquiemeBalise);
            docXml.Save("M4.xml");
        }
        public static void J1(List<string> liste_info_sejour)
        {
            StreamWriter writer = new StreamWriter("J1.json");
            JsonTextWriter jwriter = new JsonTextWriter(writer);
            jwriter.WriteStartObject();
            jwriter.WritePropertyName("Ville");
            jwriter.WriteValue(liste_info_sejour[2]);
            jwriter.WritePropertyName("Date");
            jwriter.WriteValue(liste_info_sejour[0]);
            jwriter.WriteEndObject();
            jwriter.Close();
            writer.Close();
        }
        public static string enregistrementVehicule(string connectionString, int client, string voiture_a_rendre, List<string> liste_info_sejour, string id_p_voiture_deposee, string place_deposee)
        {
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();
            MySqlDataReader reader;
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "select immat from ranger where immat='"+voiture_a_rendre+"' and id_p='"+id_p_voiture_deposee+"' and date_r='18-01-25'";
            reader = command.ExecuteReader();
            if (reader.Read())
            {

            }
            else
            {
                connection.Close();
                connection.Open();
                command.CommandText = "insert into ranger values('" + id_p_voiture_deposee + "','" + voiture_a_rendre + "','18-01-25','" + place_deposee + "')";
                reader = command.ExecuteReader();
            }
            connection.Close();
            return voiture_a_rendre;
        }
        public static void attributionNote(string connectionString, int note_attribuee, int id_client, int id_sejour_choisi,List<string> liste_info_sejour)
        {
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();
            MySqlDataReader reader;
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "update reserver set note="+note_attribuee+" where num_c="+id_client+" and id_s="+id_sejour_choisi+" and date_r='18-01-"+liste_info_sejour[0]+"'";
            reader = command.ExecuteReader();
            connection.Close();
        }
        public static void controleVehicule(string connectionString, string voiture_deposee)
        {
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();
            MySqlDataReader reader;
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "select nom_v from voiture where immat='"+voiture_deposee+"'";
            reader = command.ExecuteReader();
            string nom_v_voiture_deposee = "";
            if (reader.Read())
            {
                nom_v_voiture_deposee = reader.GetString(0);
            }
            connection.Close();
            connection.Open();
            command.CommandText = "select immat from controler where immat='"+voiture_deposee+"' and nom_v='"+nom_v_voiture_deposee+"' and date_ctrl='18-01-28'";
            reader = command.ExecuteReader();
            if (reader.Read())
            {

            }
            else
            {
                connection.Close();
                connection.Open();
                command.CommandText = "insert into controler values('" + voiture_deposee + "','" + nom_v_voiture_deposee + "','18-01-28')";
                reader = command.ExecuteReader();
            }
            connection.Close();
        }
        public static void entretien_et_mise_a_disponible(string connectionString, string voiture_deposee)
        {
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();
            MySqlDataReader reader;
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "select immat from entretenir where immat='"+voiture_deposee+"' and id_m='nett'";
            reader = command.ExecuteReader();
            if (reader.Read())
            {

            }
            else
            {
                connection.Close();
                connection.Open();
                command.CommandText = "insert into entretenir values('"+voiture_deposee+"','nett','18-01-28')";
                reader = command.ExecuteReader();
            }
            connection.Close();
            connection.Open();
            command.CommandText = "update voiture set disponible=true where immat='"+voiture_deposee+"'";
            reader = command.ExecuteReader();
            connection.Close();
        }
        public static void requetes_statistiques(string connectionString, string voiture_deposee, int num_client)
        {
            Console.WriteLine("Affichage des interventions sur la voiture du client précédent\n");
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();
            MySqlDataReader reader;
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "select id_m from entretenir where immat='"+voiture_deposee+"'";
            reader = command.ExecuteReader();
            string maintenances = "";
            while (reader.Read())
            {
                maintenances = reader.GetString(0);
                Console.WriteLine(maintenances);
            }
            connection.Close();
            Console.ReadKey();

            Console.WriteLine("Affichage des voitures utilisées par le client précédent\n");
            connection.Open();
            command.CommandText = "select immat from utiliser where num_c ="+ num_client;
            reader = command.ExecuteReader();
            string immat = "";
            while (reader.Read())
            {
                immat = reader.GetString(0);
                Console.WriteLine(immat);
            }
            connection.Close();
            Console.ReadKey();

            Console.WriteLine("Affichage de la rentabilité par mois du vehicule du dernier client, dans notre exemple, le nombre de locations du vehicule du dernier client dans le mois.\n");
            connection.Open();
            command.CommandText = "select count(*) from utiliser where immat='"+voiture_deposee+"' and date_d like '2018-%'";
            reader = command.ExecuteReader();
            int num_locations = 0;
            if (reader.Read())
            {
                num_locations = reader.GetInt16(0);
            }
            Console.WriteLine((float)(num_locations*100/12)+" %");
            connection.Close();
            Console.ReadKey();
        }
    }
}