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

public class Program
{
    public static string fileData = new FileName().FILE_DATA;
    public static string finalFile = new FileName().FINAL_FILE;
    public static string fileBdd = new FileName().FILE_BDD;
    public static string log_dir = new ConstantVar().LOG_DIRECTORY;
    public static Regex recupTramePattern = new ConstantVar().RECUP_TRAME_PATTERN;
    public static string[] bannedChar = new ConstantVar().BANNED_CHAR;

    private static async Task CreateBdd()
    {
        string settings = "Data Source=log_info.db;Version=3";
        Console.WriteLine(":: [*] BDD Connection.");
        try
        {
            using (SQLiteConnection connection = new SQLiteConnection(settings))
            {
                await connection.OpenAsync(); // OpenAsync pour que l'on puisse ouvrir la bdd sans attendre que la ligne s'exécute 
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    // Création de la BDD
                    command.CommandText = TblChar.CREATE_BDD;
                    await command.ExecuteNonQueryAsync();
                    Console.WriteLine(":: [+] BDD Connection OK !\r\n::");

                    using (var transaction = connection.BeginTransaction())
                    {
                        using (StreamReader reader = new StreamReader(finalFile))
                        {
                            string? line;
                            Console.WriteLine(":: [*] Execute SQL queries.");
                            //command.CommandText =
                            //        @"
                            //        INSERT INTO T_TRANS 
                            //        (Collecte,Remise,Num,TDate,TTime,Pan,Approved,Amount,Aid,PanHash,Emv,Iso2,
                            //        Online,Prop,TACIAC,Name,Bank,Tags,Timings,IdVoie,Smact)
                            //        VALUES ($)
                            //        ";
                            // Parameters

                            // Lire chaque ligne du fichier
                            while ((line = reader.ReadLine()) != null)
                            {
                                command.CommandText = line;
                                command.ExecuteNonQuery();  // Exécuter la commande SQL de manière asynchrone
                            }
                            //await transaction.CommitAsync();
                            transaction.Commit();
                            Console.WriteLine(":: [+] Queries executed successfully !");
                        }
                    }
                }
            }// Close BDD 
        }
        catch (Exception e)
        {
            Console.WriteLine(":: [-] " + e.Message);
        }
    }

    private static void FileManagement()
    {
        Console.WriteLine(":: [*] Creating SQL queries.");
        using (StreamReader reader = new StreamReader(fileData))
        {
            List<string> tblCommandSQL = new List<string>();
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                if (Regex.IsMatch(line, @"^insert"))
                {
                    // SI la taille == 0 ALORS j'ajoute la ligne dans le tableau 
                    if (tblCommandSQL.Count == 0)
                    {
                        tblCommandSQL.Add(line);
                    }
                    else
                    {
                        string? commandSQL = string.Join("", tblCommandSQL);
                        using (StreamWriter writer = new StreamWriter(finalFile, true))
                        {
                            writer.WriteLine(commandSQL);
                        }
                        tblCommandSQL.Clear();
                        tblCommandSQL.Add(line);
                    }
                }
                else
                {
                    tblCommandSQL.Add(line);
                }
            }
            string? lastCommandSQL = string.Join("", tblCommandSQL);
            using (StreamWriter writer = new StreamWriter(finalFile, true))
            {
                writer.WriteLine(lastCommandSQL);
            }
        }
        Console.WriteLine(":: [+] SQL queries created !\r\n::");
    }

    private static void WriteLogFile(string lineLog, StreamWriter writer)
    {
        try
        {
            if (recupTramePattern.Match(lineLog).Success) // Sans .Success la condition retournerait un objet de type match et non un bool 
            {
                if (!lineLog.Contains(bannedChar[0]) &&
                    !lineLog.Contains(bannedChar[1]) &&
                    !lineLog.Contains(bannedChar[2]) &&
                    !lineLog.Contains(bannedChar[3]) &&
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
                        writer.WriteLine(lineLog.Trim());
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
        if (Directory.Exists(log_dir))
        {
            // Parcourir tous les fichiers du dossier
            string[] fichiers = Directory.GetFiles(log_dir);

            using (StreamWriter writer = new StreamWriter(fileData, true)) // Ouvre le fichier une seule fois
            {
                Console.WriteLine(":: [*] Reading files.");
                foreach (string fichier in fichiers)
                {
                    //Console.WriteLine($"Traitement du fichier : {fichier}");
                    using (StreamReader reader = new StreamReader(fichier))
                    {
                        //Console.WriteLine($":: Lecture du fichier : {fichier}"); 
                        string? line;
                        // Lire chaque ligne du fichier
                        while ((line = reader.ReadLine()) != null)
                        {
                            WriteLogFile(line, writer);
                        }
                    }
                }
                Console.WriteLine(":: [+] Files are read !\r\n::");
            }
        }
        else
        {
            Console.WriteLine(":: [-] The 'log' folder doesn't exist.");
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

        // Delete files before execution
        if (File.Exists(fileData)) File.Delete(fileData);
        if (File.Exists(finalFile)) File.Delete(finalFile);
        if (File.Exists(fileBdd)) File.Delete(fileBdd);

        ReadAllFile();
        FileManagement();
        _ = CreateBdd(); // "_ =" sert à ce que la valeur retournée est ignorée

        // Arrête le chronomètre
        stopwatch.Stop();

        // Récupère le temps écoulé
        TimeSpan elapsed = stopwatch.Elapsed;
        DisplayRuntime(elapsed);

        //BDDValueCheck.CBDDValueCheck.MainBDD();
    }
}
