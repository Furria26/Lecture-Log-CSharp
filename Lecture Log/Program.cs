using System;
using System.IO;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;
using System.Data.SQLite;
using System.Diagnostics;
using Var;
using BDDValueCheck;
using System.Threading.Tasks;
using System.ComponentModel.Design;
<<<<<<< HEAD
using System.Security.Cryptography;
using System.Xml.Linq;

public class Program
{
    private static void WriteCommandFile(string lineLog, StreamWriter writer, List<string> linesRecovered)
    {
        // Variable contenant la requête SQL
        string oneSQLCommand = string.Join("", linesRecovered);

        string requiredValue = "Values (";
        int index = oneSQLCommand.IndexOf(requiredValue);
        int LengthToDeleted = index + requiredValue.Length;

        // Supprimer les caractères pour avec que les valeurs
        lineLog = oneSQLCommand.Substring(LengthToDeleted, oneSQLCommand.Length - (LengthToDeleted + 1));
        // Créer un tableau de valeur
        string[] lineValue = lineLog.Split(',');


        ValueBDD.Collecte = Convert.ToInt32(lineValue[0]);
        ValueBDD.Remise = Convert.ToInt32(lineValue[1]);
        ValueBDD.Num = Convert.ToInt32(lineValue[2]);
        ValueBDD.TDate = lineValue[3];
        ValueBDD.TTime = lineValue[4];
        ValueBDD.Pan = lineValue[5];
        ValueBDD.Approved = Convert.ToInt32(lineValue[6]);
        ValueBDD.Amount = Convert.ToInt32(lineValue[7]);
        ValueBDD.Aid = lineValue[8];
        ValueBDD.PanHash = lineValue[9];
        ValueBDD.Emv = lineValue[10];
        ValueBDD.Iso2 = lineValue[11];
        ValueBDD.Online = lineValue[12];
        ValueBDD.Prop = lineValue[13];
        ValueBDD.TACIAC = lineValue[14];
        ValueBDD.Name = lineValue[15];
        ValueBDD.Bank = lineValue[16];
        ValueBDD.Tags = lineValue[17];
        ValueBDD.Timings = lineValue[18];
        ValueBDD.IdVoie = lineValue[19];
        ValueBDD.Smact = Convert.ToInt32(lineValue[20]);

        writer.WriteLine(ValueBDD.TDate + "," + ValueBDD.Remise + "," + ValueBDD.Num + "," + ValueBDD.TTime + "," +
                    ValueBDD.Approved + "," + ValueBDD.Collecte + "," + ValueBDD.Amount + "," + ValueBDD.Aid + "," +
                    ValueBDD.Pan + "," + ValueBDD.Iso2 + "," + ValueBDD.TACIAC + "," + ValueBDD.Online + "," +
                    ValueBDD.Emv + "," + ValueBDD.Prop + "," + ValueBDD.PanHash + "," + ValueBDD.Name + "," +
                    ValueBDD.Bank + "," + ValueBDD.Tags + "," + ValueBDD.IdVoie + "," + ValueBDD.Smact + "," +
                    ValueBDD.Timings);
    }

    private static void FiltreLine(string lineLog, StreamWriter writer, List<string> linesRecovered)
=======

public class Program
{
    public static string finalFile = new FileName().FINAL_FILE;
    public static string fileBdd = new FileName().FILE_BDD;
    public static string log_dir = new ConstantVar().LOG_DIRECTORY;
    public static Regex recupTramePattern = new ConstantVar().RECUP_TRAME_PATTERN;
    public static string[] bannedChar = new ConstantVar().BANNED_CHAR;

    // C'est une variable non static
    //public List<string> linesRecovered = new List<string>();

    private static async Task BddManager()
    {
        string settings = "Data Source=log_info.db;Version=3";
        try
        {
            using (SQLiteConnection connection = new SQLiteConnection(settings))
            {
                await connection.OpenAsync(); // OpenAsync pour que l'on puisse ouvrir la bdd sans attendre que la ligne s'exécute 
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    Console.WriteLine(":: [*] Execute SQL queries.");
                    for (int i = 0; i < 1; i++) //TEST
                    {
                        using (var transaction = connection.BeginTransaction())
                        {
                            using (StreamReader reader = new StreamReader(finalFile))
                            {
                                string? line;
                                // Lire chaque ligne du fichier
                                while ((line = reader.ReadLine()) != null)
                                {
                                    command.CommandText = line;
                                    //Console.WriteLine(line + "\r\n");
                                    command.ExecuteNonQuery();
                                }
                                //await transaction.CommitAsync();
                                transaction.Commit();
                            }
                        }
                    }
                    Console.WriteLine(":: [+] Queries executed successfully !");
                }
            }// Close BDD 
        }
        catch (Exception e)
        {
            Console.WriteLine(":: [-] " + e.Message);
        }
    }

    private static void InsertInToFileSQL(StreamWriter writer, List<string> allCommandSQL)
    {
        List<string> hundredCommandSQL = new List<string>();
        for (int i = 0; i < allCommandSQL.Count; i++)
        {
            if (i == 0)
            {
                hundredCommandSQL.Add(allCommandSQL[i]);
            }
            else if (i % 100 == 0 && i != 0)
            {
                string? stringCmdSQL = string.Join(",", hundredCommandSQL);
                writer.Write(stringCmdSQL + "\r\n");
                hundredCommandSQL.Clear();
                stringCmdSQL = "";
                hundredCommandSQL.Add(allCommandSQL[i]);
            }
            else
            {
                if (i == allCommandSQL.Count - 1)
                {
                    allCommandSQL[i] = allCommandSQL[i].Substring(157);
                    string? stringCmdSQL = string.Join(",", hundredCommandSQL);
                    //for (global::System.Int32 j = 0; j < 100000; j++)
                    //{
                    //    writer.Write(stringCmdSQL + "," + allCommandSQL[i] + "\r\n");
                    //}
                    writer.Write(stringCmdSQL + "," + allCommandSQL[i] + "\r\n");
                    
                    hundredCommandSQL.Clear();
                }
                else
                {
                    allCommandSQL[i] = allCommandSQL[i].Substring(157);
                    hundredCommandSQL.Add(allCommandSQL[i]);
                }
            }
        }
        if (hundredCommandSQL.Count > 0)
        {
            string stringCmdSQL = string.Join(",", hundredCommandSQL);
            writer.Write(stringCmdSQL + "\r\n");
        }
    }

    private static void WriteLogFile(string lineLog, StreamWriter writer, List<string> linesRecovered, List<string> allCommandSQL)
>>>>>>> 66c9d4e89a548de3e04c951030c695f941f798cd
    {
        try
        {
            // Filtrage de la ligne
            if (ConstantVar.RECUP_TRAME_PATTERN.Match(lineLog).Success) // Sans .Success la condition retournerait un objet de type match et non un bool 
            {
                if (!lineLog.Contains(ConstantVar.BANNED_CHAR[0]) &&
                    !lineLog.Contains(ConstantVar.BANNED_CHAR[1]) &&
                    !lineLog.Contains(ConstantVar.BANNED_CHAR[2]) &&
                    !lineLog.Contains(ConstantVar.BANNED_CHAR[3]) &&
                    !lineLog.Contains("where Remise="))
                {
                    // Remplacer les caractères
                    for (int count = 0; count < 4; count++)
                    {
                        lineLog = Regex.Replace(lineLog, TblChar.SEARCH_CHAR[count], TblChar.REPLACE_CHAR[count]);
                    }
                    if (!Regex.IsMatch(lineLog, @"^Emv =") && !Regex.IsMatch(lineLog, @"^Collecte=\d{2} where"))
                    {
                        for (int count = 3; count < TblChar.SEARCH_CHAR.Length; count++)
                        {
                            lineLog = Regex.Replace(lineLog, TblChar.SEARCH_CHAR[count], TblChar.REPLACE_CHAR[count]);
                        }
<<<<<<< HEAD

                        linesRecovered.Add(lineLog);
                        if (linesRecovered.Count == 6)
                        {
                            WriteCommandFile(lineLog, writer, linesRecovered);   
=======
                        linesRecovered.Add(lineLog);
                        if (linesRecovered.Count == 6)
                        {
                            // Création de la requête sql simple
                            string? oneSQLCommand = string.Join("", linesRecovered);
                            //writer.WriteLine(oneSQLCommand);
                            allCommandSQL.Add(oneSQLCommand);
>>>>>>> 66c9d4e89a548de3e04c951030c695f941f798cd
                            linesRecovered.Clear();
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($":: [-] Erreur lors de l'écriture : {e.Message}");
        }
    }

    private static void ReadAllFile()
    {
        if (Directory.Exists(ConstantVar.LOG_DIRECTORY))
        {
            // Parcourir tous les fichiers du dossier
<<<<<<< HEAD
            string[] fichiers = Directory.GetFiles(ConstantVar.LOG_DIRECTORY);
            using (StreamWriter writer = new StreamWriter(FileName.FINAL_FILE, true)) // Ouvre le fichier une seule fois
=======
            string[] fichiers = Directory.GetFiles(log_dir);
            using (StreamWriter writer = new StreamWriter(finalFile, true)) // Ouvre le fichier une seule fois
>>>>>>> 66c9d4e89a548de3e04c951030c695f941f798cd
            {
                Console.WriteLine(":: [*] Reading files.");
                // Tableau pour les Lignes récupérées
                List<string> linesRecovered = new List<string>();
<<<<<<< HEAD
                // Boucler sur tous les fichiers 
=======
                // Tableau contenant toutes les requêtes SQL
                List<string> allCommandSQL = new List<string>();
>>>>>>> 66c9d4e89a548de3e04c951030c695f941f798cd
                foreach (string fichier in fichiers)
                {
                    using (StreamReader reader = new StreamReader(fichier))
                    {
                        string? line;
                        // Lire chaque ligne du fichier
                        while ((line = reader.ReadLine()) != null)
                        {
<<<<<<< HEAD
                            FiltreLine(line, writer, linesRecovered);
=======
                            WriteLogFile(line, writer, linesRecovered, allCommandSQL);
>>>>>>> 66c9d4e89a548de3e04c951030c695f941f798cd
                        }
                    }
                }
                Console.WriteLine(":: [+] Files are read !\r\n::");
                InsertInToFileSQL(writer, allCommandSQL);
            }
        }
        else
        {
            Console.WriteLine(":: [-] The 'log' folder doesn't exist.");
        }
    }

    private static void CreateBdd()
    {
        string settings = "Data Source=log_info.db;Version=3";
        Console.WriteLine(":: [*] BDD Connection.");

        using (SQLiteConnection connection = new SQLiteConnection(settings))
        {
<<<<<<< HEAD
            // Ouverture de la base de données 
            connection.Open();
            using (SQLiteCommand command = new SQLiteCommand(connection))
            {
                if (File.Exists(FileName.FILE_BDD))
=======
            connection.OpenAsync(); // OpenAsync pour que l'on puisse ouvrir la bdd sans attendre que la ligne s'exécute 
            using (SQLiteCommand command = new SQLiteCommand(connection))
            {
                if (!File.Exists(fileBdd))
                {
                    // Création de la table T_TRANS
                    command.CommandText = TblChar.CREATE_BDD;
                    command.ExecuteNonQueryAsync();
                    Console.WriteLine(":: [+] BDD Connection OK !\r\n::");
                }
                else
>>>>>>> 66c9d4e89a548de3e04c951030c695f941f798cd
                {
                    // Réinitialisation de la table 
                    command.CommandText = "DROP TABLE IF EXISTS T_TRANS";
                    command.ExecuteNonQueryAsync();

                    command.CommandText = TblChar.CREATE_BDD;
                    command.ExecuteNonQueryAsync();
                    Console.WriteLine(":: [+] BDD Connection OK !\r\n::");
                }
<<<<<<< HEAD
                else
                {
                    // Création de la table T_TRANS
                    command.CommandText = TblChar.CREATE_BDD;
                    command.ExecuteNonQueryAsync();
                    Console.WriteLine(":: [+] BDD Connection OK !\r\n::");
                }
=======
>>>>>>> 66c9d4e89a548de3e04c951030c695f941f798cd
            }
        }
    }

    private static void OutputTitle()
    {
        Console.WriteLine("::---------------------------------------------------------------------------------------------------");
        Console.WriteLine(":: Lecture Log - (C) Vinci-Autoroutes");
        Console.WriteLine("::---------------------------------------------------------------------------------------------------");
    }

    private static void DisplayRuntime(TimeSpan elapsed)
    {
        // Affiche le temps écoulé en format minute:seconde:milliseconde
        Console.WriteLine("::---------------------------------------------------------------------------------------------------");
        Console.WriteLine(":: Temps écoulé: {0:D2}min {1:D2}sec {2:D3}ms",
            elapsed.Minutes,          // Minutes
            elapsed.Seconds,          // Secondes
            elapsed.Milliseconds);    // Millisecondes
        Console.WriteLine("::---------------------------------------------------------------------------------------------------");
    }

    public static void Main()
    {
        OutputTitle();

        // Démarre un chronomètre pour calculer le temps d'exécution 
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        CreateBdd();

        // Delete files before execution
<<<<<<< HEAD
        if (File.Exists(FileName.FINAL_FILE)) File.Delete(FileName.FINAL_FILE);

        ReadAllFile();

        string sqlitePath = @"C:\sqlite\sqlite3.exe"; // Remplacez par le chemin réel de sqlite3
        string arguments = @"log_info.db "".import --csv file_command_sql.txt T_TRANS""";
        
        // Lancer l'écriture dans la base de donner avec .import
        Process.Start(sqlitePath, arguments);
=======
        if (File.Exists(finalFile)) File.Delete(finalFile);

        ReadAllFile();
        _ = BddManager(); // "_ =" sert à ce que la valeur retournée soit ignorée
>>>>>>> 66c9d4e89a548de3e04c951030c695f941f798cd

        // Arrête le chronomètre
        stopwatch.Stop();

        // Récupère le temps écoulé
        TimeSpan elapsed = stopwatch.Elapsed;
        DisplayRuntime(elapsed);
<<<<<<< HEAD
=======

        //BDDValueCheck.CBDDValueCheck.MainBDD();


        //string sqlitePath = @"C:\sqlite\sqlite3.exe"; // Remplacez par le chemin réel de sqlite3
        //string arguments = @"log_info.db "".import --csv file_command_sql_2.csv T_TRANS""";

        //Process.Start(sqlitePath, arguments);
>>>>>>> 66c9d4e89a548de3e04c951030c695f941f798cd
    }
}