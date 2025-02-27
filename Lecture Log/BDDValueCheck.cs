using System;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Globalization;

namespace BDDValueCheck
{
    public class CBDDValueCheck
    {
        private static void CheckVal()
        {
            string settings = "Data Source=log_info.db;Version=3";
            string query = "SELECT TDate FROM T_TRANS";
            string[] columns = {"TDate", "Remise", "Num", "TTime", "Approved", "Collecte",
                                "Amount", "Aid", "Pan", "Iso2", "TacIac", "Online", "Emv",
                                "Prop", "PanHash", "Name", "Bank", "Tags", "IdVoie", "Smact",
                                "Timings" };
            try
            {
                // Connexion à la base de données SQLite
                using (SQLiteConnection connection = new SQLiteConnection(settings))
                {
                    connection.Open();
                    // Création d'une commande SQLite avec la requête
                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        // Exécution de la commande et récupération des résultats dans reader
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            // Tant que la valeur n'est pas nulle
                            while (reader.Read())
                            {
                                // Lire la valeur de la colonne TDate
                                string? tDate = reader[columns[0]].ToString();

                                // Vérification du format et de la validité de la date avec DateTime.TryParseExact
                                DateTime parsedDate;
                                if (!DateTime.TryParseExact(tDate, "yyMMdd", null, System.Globalization.DateTimeStyles.None, out parsedDate))
                                {
                                    Console.WriteLine($":: [-] La date '{tDate}' est invalide (mois ou jour incorrect).");
                                }
                                else
                                {
                                    Console.WriteLine(tDate); // Affiche la date si elle est valide
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(":: [-] " + e.Message);
            }
        }

        public static void MainBDD()
        {
            CheckVal();
        }
    }
}