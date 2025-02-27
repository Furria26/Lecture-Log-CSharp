using System;
using System.Text.RegularExpressions;
using System.Data.SqlClient;

namespace BDDValueCheck
{
    ////private static string query = "SELECT TDate FROM T_TRANS";
    //private class CheckVal
    //{
    //    // Méthode pour vérifier le format de la date "ddMMyyyy"
    //    public static bool CheckDate(string date)
    //    {
    //        // Vérifie si la date correspond au format "ddMMyyyy"
    //        string pattern = @"^\d{6}$";  // 6 chiffres (jour, mois, année sous forme "ddMMyyyy")

    //        if (Regex.IsMatch(date, pattern))
    //        {
    //            // Essayer de convertir la chaîne avec le format exact "ddMMyyyy"
    //            return DateTime.TryParseExact(date, "yyMMdd", null, System.Globalization.DateTimeStyles.None, out _);
    //        }
    //        return false;
    //    }
    //}

    public class CBDDValueCheck
    {
        private static void CheckVal()
        {
            string[] columns = ["TDate", "Remise", "Num", "TTime", "Approved", "Collecte", 
                                "Amount", "Aid", "Pan", "Iso2", "TacIac", "Online", "Emv",
                                "Prop", "PanHash", "Name", "Bank", "Tags", "IdVoie", "Smact", 
                                "Timings"];

            //for (int i = 0; i < columns.Length; i++)
            //{
            //    Console.WriteLine(columns[i]);
            //}


        }

        public static void MainBDD()
        {
            CheckVal();
        }
    }
}