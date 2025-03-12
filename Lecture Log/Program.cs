using System;
using System.Text.RegularExpressions;
using System.Data.SQLite;
using System.Diagnostics;
using SyslogLogging;
using Var;

public class Program
{
    public static LoggingModule errorLog = new LoggingModule("Error.log");
    public static int countFinalFileLine = 1;

    private static void VerifPan(string Pan)
    {
        if (Pan.Length == 18)
        {
            // Pour supprimer les simples côtes du début et de la fin de Pan (ex : '50176742XXXXXX69')
            Pan = Pan.Substring(1, Pan.Length - 2);
        }
        else
        {
            errorLog.Error($": Taille de Pan incorrect à la ligne {countFinalFileLine} du fichier {FileName.FINAL_FILE}");
        }
    }

    private static void VerifAid(string Aid)
    {
        if (Aid.Length == 16)
        {
            // Pour supprimer les simples côtes du début et de la fin de TTime (ex : 'A0000000421010')
            Aid = Aid.Substring(1, Aid.Length - 2);
        }
        else
        {
            errorLog.Error($": Taille de Aid incorrect à la ligne {countFinalFileLine} du fichier {FileName.FINAL_FILE}"); 
        }
    }

    private static void VerifAmout(int? Amout)
    {
        if (Amout >= 1000 && Amout < 100 || !Amout.HasValue)
        {
            errorLog.Error($": Format de Amout {Amout} est incorrect à ligne {countFinalFileLine} du fichier {FileName.FINAL_FILE}");
        }
    }

    private static void VerifTTime(string TTime)
    {
        if (TTime.Length == 8)
        {
            // Pour supprimer les simples côtes du début et de la fin de TTime (ex : '250307')
            TTime = TTime.Substring(1, TTime.Length - 2);
            if (!IsAllDigits(TTime))
            {
                errorLog.Error($": Format de TTime incorrect à la ligne {countFinalFileLine} du fichier {FileName.FINAL_FILE}");
            }
        }
        else 
        { 
            errorLog.Error($": Taille de TTime incorrect à la ligne {countFinalFileLine} du fichier {FileName.FINAL_FILE}"); 
        }
    }

    private static void VerifTDate(string TDate)
    {
        if (TDate.Length == 8)
        {
            // Pour supprimer les simples côtes du début et de la fin de TDates (ex : '250307')
            TDate = TDate.Substring(1, TDate.Length - 2);
            DateTime parsedDate;
            // Verification du format de TDate 
            if (!DateTime.TryParseExact(TDate, "yyMMdd", null, System.Globalization.DateTimeStyles.None, out parsedDate))
            {
                errorLog.Error($": Format de la date {TDate} est incorrect à ligne {countFinalFileLine} du fichier {FileName.FINAL_FILE}");
            }
        }
        else { errorLog.Error($": Taille de la date incorrect à la ligne {countFinalFileLine} du fichier {FileName.FINAL_FILE}"); }
    }

    static bool IsAllDigits(string str)
    {
        foreach (char singleChar in str)
        {
            if (!char.IsDigit(singleChar))
            {
                return false;
            }
        }
        return true;
    }

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
        if (!ValueBDD.Collecte.HasValue)    // Vérifier si la valeur Collecte est null ou est un entier 
        {
            errorLog.Error($": Format de Collecte incorrect à la ligne {countFinalFileLine} du fichier {FileName.FINAL_FILE}");
        }

        ValueBDD.Remise = Convert.ToInt32(lineValue[1]);
        if (!ValueBDD.Remise.HasValue)  // Vérifier si la valeur Remise n'est pas null ou est un entier 
        {
            errorLog.Error($": Format de Remise incorrect à la ligne {countFinalFileLine} du fichier {FileName.FINAL_FILE}");
        }

        ValueBDD.Num = Convert.ToInt32(lineValue[2]);
        if (!ValueBDD.Num.HasValue) // Vérifier si la valeur Num n'est pas null ou est un entier 
        {
            errorLog.Error($": Format de Num incorrect à la ligne {countFinalFileLine} du fichier {FileName.FINAL_FILE}");
        }

        ValueBDD.Approved = Convert.ToInt32(lineValue[6]);
        if (!ValueBDD.Approved.HasValue)    // Vérifier si la valeur Approuved n'est pas null ou est un entier 
        {
            errorLog.Error($": Format de Approuved incorrect à la ligne {countFinalFileLine} du fichier {FileName.FINAL_FILE}");
        }

        ValueBDD.Amount = Convert.ToInt32(lineValue[7]);
        if (!ValueBDD.Amount.HasValue)    // Vérifier si la valeur Amount n'est pas null ou est un entier 
        {
            errorLog.Error($": Format de Amout incorrect à la ligne {countFinalFileLine} du fichier {FileName.FINAL_FILE}");
        } else { VerifAmout(ValueBDD.Amount); }

        ValueBDD.TDate = lineValue[3];
        VerifTDate(ValueBDD.TDate);

        ValueBDD.TTime = lineValue[4];
        VerifTTime(ValueBDD.TTime);

        ValueBDD.Pan = lineValue[5];
        VerifPan(ValueBDD.Pan);

        ValueBDD.Aid = lineValue[8];
        VerifAid(ValueBDD.Aid);

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
        countFinalFileLine += 1;
    }

    private static void FiltreLine(string lineLog, StreamWriter writer, List<string> linesRecovered)
    {
        try
        {
            // Filtrage de la ligne
            if (ConstantVar.RECUP_TRAME_PATTERN.Match(lineLog).Success) 
                // Sans .Success la condition retournerait un objet de type match et non un bool 
            {
                if (!lineLog.Contains(ConstantVar.BANNED_CHAR[0]) &&
                    !lineLog.Contains(ConstantVar.BANNED_CHAR[1]) &&
                    !lineLog.Contains(ConstantVar.BANNED_CHAR[2]) &&
                    !lineLog.Contains(ConstantVar.BANNED_CHAR[3]) &&
                    !lineLog.Contains("where Remise="))
                {
                    // Transformation de la ligne
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
                        linesRecovered.Add(lineLog);
                        // "6" pour la fin de la requête 
                        if (linesRecovered.Count == 6)
                        {
                            WriteCommandFile(lineLog, writer, linesRecovered);
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
            string[] fichiers = Directory.GetFiles(ConstantVar.LOG_DIRECTORY);
            using (StreamWriter writer = new StreamWriter(FileName.FINAL_FILE, true)) // Ouvre le fichier une seule fois
            {
                Console.WriteLine(":: [*] Reading files.");
                // Tableau pour les Lignes récupérées
                List<string> linesRecovered = new List<string>();

                // Boucler sur tous les fichiers 
                foreach (string fichier in fichiers)
                {
                    using (StreamReader reader = new StreamReader(fichier))
                    {
                        string? line = "";
                        // Lire chaque ligne du fichier
                        while ((line = reader.ReadLine()) != null)
                        {
                            FiltreLine(line, writer, linesRecovered);
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

    private static void CreateBdd()
    {
        string settings = "Data Source=log_info.db;Version=3";
        Console.WriteLine(":: [*] BDD Connection.");

        using (SQLiteConnection connection = new SQLiteConnection(settings))
        {
            // Ouverture de la base de données 
            connection.Open();
            using (SQLiteCommand command = new SQLiteCommand(connection))
            {
                if (File.Exists(FileName.FILE_BDD))
                {
                    // Réinitialisation de la table 
                    command.CommandText = "DROP TABLE IF EXISTS T_TRANS";
                    command.ExecuteNonQueryAsync();
                }
                command.CommandText = TblChar.CREATE_BDD;
                command.ExecuteNonQueryAsync();
                Console.WriteLine(":: [+] BDD Connection OK !\r\n::");
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
        // Démarre un chronomètre pour calculer le temps d'exécution 
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        OutputTitle();
        CreateBdd();

        // Delete files before execution
        if (File.Exists(FileName.FINAL_FILE)) File.Delete(FileName.FINAL_FILE);

        ReadAllFile();

        string sqlitePath = @"C:\sqlite\sqlite3.exe"; // Remplacez par le chemin réel de sqlite3
        string arguments = @"log_info.db "".import --csv file_command_sql.csv T_TRANS""";
        Process.Start(sqlitePath, arguments);

        // Ce Sleep sert à laisser le temps à la command .import de s'exécuter  
        System.Threading.Thread.Sleep(50);

        // Arrête le chronomètre
        stopwatch.Stop();

        // Récupère le temps écoulé
        TimeSpan elapsed = stopwatch.Elapsed;
        DisplayRuntime(elapsed);
    }
}