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
            //string[] query = {"SELECT TDate FROM T_TRANS", "SELECT Remise FROM T_TRANS", "SELECT Num FROM T_TRANS",
            //                    "SELECT TTime FROM T_TRANS", "SELECT Approved FROM T_TRANS", "SELECT Collecte FROM T_TRANS", 
            //                    "SELECT Amout FROM T_TRANS", "SELECT Aid FROM T_TRANS", "SELECT Pan FROM T_TRANS",
            //                    "SELECT Iso2 FROM T_TRANS", "SELECT TacIac FROM T_TRANS", "SELECT Online FROM T_TRANS",
            //                    "SELECT Emv FROM T_TRANS", "SELECT Prop FROM T_TRANS", "SELECT PanHash FROM T_TRANS",
            //                    "SELECT Name FROM T_TRANS", "SELECT Bank FROM T_TRANS", "SELECT Tags FROM T_TRANS", 
            //                    "SELECT IdVoie FROM T_TRANS", "SELECT Smact FROM T_TRANS", "SELECT Timings FROM T_TRANS"};

            string[] columns = {"TDate", "Remise", "Num", "TTime", "Approved", "Collecte",
                                "Amount", "Aid", "Pan", "Iso2", "TacIac", "Online", "Emv",
                                "Prop", "PanHash", "Name", "Bank", "Tags", "IdVoie", "Smact",
                                "Timings" };

            // LINQ sert à générer un tableau à partir de la variable columns
            string[] query = columns.Select(col => $"SELECT {col} FROM T_TRANS").ToArray();
            //for (int i = 0; i < query.Length; i++)
            //{
            //    Console.WriteLine(columns[i]);
            //    Console.WriteLine(query[i] + "\r\n");
            //}


            try
            {
                // Connexion à la base de données SQLite
                using (SQLiteConnection connection = new SQLiteConnection(settings))
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(query[0], connection))
                    {
                        // Exécution de la commande et récupération des résultats dans reader
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            // Tant que la valeur n'est pas nulle
                            while (reader.Read())
                            {
                                // Lire la valeur de la colonne TDate
                                string? tDate = reader[columns[0]].ToString();

                                // Vérification du format
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