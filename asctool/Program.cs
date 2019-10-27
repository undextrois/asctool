using System;
using System.Collections.Generic;
using NDesk.Options;
using System.Configuration;
using System.IO;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Text;
using System.Diagnostics;
using System.DirectoryServices.ActiveDirectory;
using System.Security;
using System.Security.Principal;
using System.Runtime.InteropServices;
using System.Threading;
using System.Linq;

//http://www.ndesk.org/doc/ndesk-options/NDesk.Options/OptionSet.html
//http://www.dotnetperls.com/main
namespace asctool
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            ascProcess ascProcess = new ascProcess();
            bool show_help = false,
                 connectDB = false,
                 iisreset = false,
                 iisstart = false,
                 backup = false,
                 copy = false,
                 initialize = false,
                 checkFile = false,
                 checkPath = false;
            StreamWriter sw;
            FileStream fs;
            TextWriter tmp;
            string dbMessage, copy_status, backup_status, error;
            

            #region OptionSet
            OptionSet p = new OptionSet() 
            {
                {"d|database", "update database.",
                    v=> connectDB=v !=null },
                {"k|iis stop", "stop IIS.",
                    v=> iisreset=v !=null },
                {"s|iis start", "start IIS.", 
                    v => iisstart = v != null },
                {"b|backup", "backup website file.", 
                    v => backup = v != null },
                {"c|copy", "copy new website file.", 
                    v => copy = v != null },
                {"i|start deployment", "deployment process.", 
                    v => initialize = v != null },
                { "h|help",  "show this message and exit.", 
                    v => show_help = v != null }
            };
            #endregion

            #region extra Parse
            List<string> extra = new List<string>();
            try
            {
                if (args == null || args.Length == 0)
                {
                    show_help = true;   
                }
                else
                {
                    extra = p.Parse(args);
                    if (extra.Any())
                    {
                        foreach (var item in extra)
                        {
                            Console.WriteLine("\nunrecognized option: {0}", item);
                        }
                        show_help = true;
                    }
                }
            }
            catch (OptionException e)
            {
                //Console.Write("greet: ");
                Console.WriteLine(e.Message);
                //Console.WriteLine("Try `greet --help' for more information.");
                //Console.ReadLine();
               
            }
            #endregion

                if (show_help)
                {
                    //show_help = Console.ReadLine();
                    //show_help = true;
                    ascProcess.ShowHelp(p);
                    //Console.WriteLine("help: {0}", show_help);
                }

            #region name
            //string message;
            //if (extra.Count > 0)
            //{
            //    message = string.Join(" ", extra.ToArray());
            //    //Debug("using new message: {0}", message);
            //}
            //else
            //{
            //    message = "Hello {0}!";
            //    //Debug("using default message: {0}", message);
            //}

            //foreach (string name in names)
            //{
            //    for (int i = 0; i < repeat; ++i)
            //        Console.WriteLine(message, name);
            //}
            #endregion

            #region database
                if (connectDB)
                {
                    Console.WriteLine("\nAsctool version 1.0 ");
                    Console.WriteLine("Apsmith (c) 2014 \n");
                    if (ascProcess.CheckConnection())
                    {
                        dbMessage = "\nDatabase connection has been established.\n";
                        Console.WriteLine(dbMessage);
                        ascProcess.dbExecute();
                        dbMessage = "\nSQL query execution complete.";
                        Console.WriteLine(dbMessage);
                        Environment.Exit(0);
                    }
                }
            #endregion

            #region iis start or stop
            if (iisreset)
            {
                try
                {
                    Console.WriteLine("\nAsctool version 1.0 ");
                    Console.WriteLine("Apsmith (c) 2014 \n");
                    ascProcess.ExecuteCommandSync("iisreset /stop");
                    Environment.Exit(0);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("\nAn Error Occured.\nPlease enter destination path for logs: \n");
                    error = Console.ReadLine();

                    #region errorPath
                    while (!checkPath)
                    {
                        if ((File.Exists(error) || Directory.Exists(error)) && checkPath == false)
                        {
                            fs = new FileStream(@"" + error + "(" + DateTime.Now.ToString("MM-dd-yy_HHmm") + ")" + ".txt", FileMode.Create);
                            sw = new StreamWriter(fs);
                            tmp = Console.Out;
                            Console.SetOut(sw);
                            Console.WriteLine(ex.Message);
                            sw.Close();
                            Console.SetOut(tmp);
                            Console.WriteLine(ex.Message);
                            break;
                        }
                        Console.WriteLine("{0} Sorry your input is invalid. \n", error);
                        Console.WriteLine("\nAn Error Occured.\nPlease enter destination path for logs: \n");
                        error = Console.ReadLine();
                        continue;
                    }
                    #endregion
                }
            }


            if (iisstart)
            {
                try
                {
                    Console.WriteLine("\nAsctool version 1.0 ");
                    Console.WriteLine("Apsmith (c) 2014 \n");
                    ascProcess.ExecuteCommandSync("iisreset /start");
                    Environment.Exit(0);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("\nAn Error Occured.\nPlease enter destination path for logs: \n");
                    error = Console.ReadLine();

                    #region errorPath
                    while (!checkPath)
                    {
                        if ((File.Exists(error) || Directory.Exists(error)) && checkPath == false)
                        {
                            fs = new FileStream(@"" + error + "(" + DateTime.Now.ToString("MM-dd-yy_HHmm") + ")" + ".txt", FileMode.Create);
                            sw = new StreamWriter(fs);
                            tmp = Console.Out;
                            Console.SetOut(sw);
                            Console.WriteLine(ex.Message);
                            sw.Close();
                            Console.SetOut(tmp);
                            Console.WriteLine(ex.Message);
                            break;
                        }
                        Console.WriteLine("{0} Sorry your input is invalid. \n", error);
                        Console.WriteLine("\nAn Error Occured.\nPlease enter destination path for logs: \n");
                        error = Console.ReadLine();
                        continue;
                    }
                    #endregion
                }
            }
            #endregion

            #region backup
            
            if (backup)
            {
                Console.WriteLine("\nAsctool version 1.0 ");
                Console.WriteLine("Apsmith (c) 2014 \n");
                Console.WriteLine("Please enter source path: " + "(e.g. " + @"C:\foldername\.." + ") \n");
                string source = Console.ReadLine();
                while (!checkFile)
                {
                    if ((Directory.Exists(source)) && checkFile == false && ascProcess.IsDirectoryEmpty(source) == false)
                    {
                        string sourceDirName = @"" + source;
                        Console.WriteLine("\nPlease enter destination path: " + "(e.g. " + @"C:\foldername\.." + ") \n");
                        string dirSource = Console.ReadLine();
                        Console.WriteLine("\n");
                        string destDirName = @"" + dirSource + "(" + File.GetLastWriteTime(sourceDirName).ToString("MM-dd-yy") + ")";
                        if (Directory.Exists(sourceDirName))
                        {
                            DirectoryInfo sourceInfo = new DirectoryInfo(sourceDirName);
                            DirectoryInfo destInfo = new DirectoryInfo(destDirName);
                            ascProcess.BackCopy(sourceInfo, destInfo);
                            backup_status = "Backup successful.";
                            Console.WriteLine(backup_status);
                            Environment.Exit(0);
                        }
                        else
                        {
                            backup_status = "File doesn't exists.";
                            Console.WriteLine(backup_status);
                        }
                        break;
                    }

                    Console.WriteLine("\nAn Error Occured.\nPlease enter destination path for logs: \n");
                    error = Console.ReadLine();

                    #region errorPath
                    while (!checkPath)
                    {
                        if ((File.Exists(error) || Directory.Exists(error)) && checkPath == false)
                        {
                            fs = new FileStream(@"" + error + "(" + DateTime.Now.ToString("MM-dd-yy_HHmm") + ")" + ".txt", FileMode.Create);
                            sw = new StreamWriter(fs);
                            tmp = Console.Out;
                            Console.SetOut(sw);
                            Console.WriteLine("Sorry {0} is invalid or folder/s empty. \n", source);
                            sw.Close();
                            Console.SetOut(tmp);
                            Console.WriteLine("Sorry {0} is invalid or folder/s empty. \n", source);
                            Console.WriteLine("Please enter source path: " + "(e.g. " + @"C:\foldername\.." + ") \n");
                            source = Console.ReadLine();
                            break;
                        }
                        Console.WriteLine("{0} Sorry your input is invalid. \n", error);
                        Console.WriteLine("\nAn Error Occured.\nPlease enter destination path for logs: \n");
                        error = Console.ReadLine();
                        continue;
                    }
                    #endregion
                    continue;
                }
                
            }
            #endregion

            #region copy
            
            if (copy)
            {
                Console.WriteLine("\nAsctool version 1.0 ");
                Console.WriteLine("Apsmith (c) 2014 \n");
                Console.WriteLine("Please enter source path: " + "(e.g. " + @"C:\foldername\.." + ") \n");
                string source = Console.ReadLine();
                while (!checkFile)
                {
                    if ((Directory.Exists(source)) && checkFile == false && ascProcess.IsDirectoryEmpty(source) == false)
                    {
                        string sourceDirName2 = @"" + source;
                        Console.WriteLine("\nPlease enter destination path: " + "(e.g. " + @"C:\foldername\.." + ") \n");
                        string dirSource = Console.ReadLine();
                        Console.WriteLine("\n");
                        string destDirName2 = @"" + dirSource;
                        if (Directory.Exists(sourceDirName2))
                        {
                            DirectoryInfo sourceInfo2 = new DirectoryInfo(sourceDirName2);
                            DirectoryInfo destInfo2 = new DirectoryInfo(destDirName2);
                            ascProcess.BackCopy(sourceInfo2, destInfo2);
                            copy_status = "Copy successful.";
                            Console.WriteLine(copy_status);
                            Environment.Exit(0);
                        }
                        else
                        {
                            copy_status = "File doesn't exists.";
                            Console.WriteLine(copy_status);
                        }
                        break;
                    }
                    Console.WriteLine("\nAn Error Occured.\nPlease enter destination path for logs: \n");
                    error = Console.ReadLine();

                    #region errorPath
                    while (!checkPath)
                    {
                        if ((File.Exists(error) || Directory.Exists(error)) && checkPath == false)
                        {
                            fs = new FileStream(@"" + error + "(" + DateTime.Now.ToString("MM-dd-yy_HHmm") + ")" + ".txt", FileMode.Create);
                            sw = new StreamWriter(fs);
                            tmp = Console.Out;
                            Console.SetOut(sw);
                            Console.WriteLine("Sorry {0} is invalid or folder/s empty. \n", source);
                            sw.Close();
                            Console.SetOut(tmp);
                            Console.WriteLine("Sorry {0} is invalid or folder/s empty. \n", source);
                            Console.WriteLine("Please enter source path: " + "(e.g. " + @"C:\foldername\.." + ") \n");
                            source = Console.ReadLine();
                            break;
                        }
                        Console.WriteLine("{0} Sorry your input is invalid. \n", error);
                        Console.WriteLine("\nAn Error Occured.\nPlease enter destination path for logs: \n");
                        error = Console.ReadLine();
                        continue;
                    }
                    #endregion
                    continue;
                }
                
            }
            #endregion

            #region initialize
            if (initialize)
            {
                ascProcess.Init();
            }
            Console.Read();
            #endregion
        }

        #region ascProcess
        public class ascProcess
        {
            public bool checkPath = false;
            StreamWriter sw;
            FileStream fs;
            TextWriter tmp;
            string error;
            public static string saveConnString;

            public string dbConnection()
            {
                SqlConnectionStringBuilder scsb;
                bool checkPath = false;
                List<string> list = new List<string>();
                scsb = new SqlConnectionStringBuilder();

                #region SQL CONNECTION PATH
                    Console.WriteLine("Please enter connection string source path:\n");
                    string sqldatasource = Console.ReadLine();
                    string source = @"" + sqldatasource;
                    try
                    {
                        while (!checkPath)
                        {
                            if ((File.Exists(source) || Directory.Exists(source)) && checkPath == false)
                            {
                                string[] strArr = File.ReadAllLines(source);
                                scsb["Data Source"] = strArr[0];
                                scsb["Initial Catalog"] = strArr[1];
                                scsb["integrated Security"] = true;
                                scsb["User ID"] = strArr[2];
                                scsb["Password"] = strArr[3];
                                break;
                            }
                            Console.WriteLine("{0} Sorry your input is invalid. \n", source);
                            Console.WriteLine("Please enter connection string source path:\n");
                            source = Console.ReadLine();
                            continue;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                #endregion

                return saveConnString = scsb.ConnectionString;
            }

            public void dbExecute()
            {
                string sqlRead;
                string sourceSQL;
                bool checkSQL = false;
                SqlConnection connection;
                ascProcess aP = new ascProcess();

                #region checkSQL
                while (!checkSQL)
                {
                        try
                        {
                            Console.WriteLine("\nPlease enter source path: \n");
                            sourceSQL = Console.ReadLine();
                            #region checkPath
                            while (!checkPath)
                            {
                                if ((File.Exists(sourceSQL) || Directory.Exists(sourceSQL)) && checkPath == false)
                                {
                                    aP.ProcessDirectory(sourceSQL);
                                    connection = new SqlConnection(saveConnString);
                                    StreamReader sqlQuery = new StreamReader(sourceSQL);

                                    sqlRead = sqlQuery.ReadToEnd();
                                    SqlCommand cmd = new SqlCommand(sqlRead, connection);

                                    connection.Open();
                                    cmd.ExecuteNonQuery();
                                    connection.Close();
                                    break;
                                }
                                Console.WriteLine("\n{0} Sorry your input is invalid. \n", sourceSQL);
                                Console.WriteLine("Please enter file path:  \n");
                                sourceSQL = Console.ReadLine();
                                continue;
                            }
                            #endregion
                            
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Please enter destination path: \n");
                            string error= Console.ReadLine();

                            #region errorPath
                            while (!checkPath)
                            {
                                if ((File.Exists(error) || Directory.Exists(error)) && checkPath == false)
                                {
                                    fs = new FileStream(@"" + error + "(" + DateTime.Now.ToString("MM-dd-yy_HHmm") + ")" + ".txt", FileMode.Create);
                                    sw = new StreamWriter(fs);
                                    tmp = Console.Out;
                                    Console.SetOut(sw);
                                    Console.WriteLine("SQL Query execution failed, " + ex.Message);
                                    sw.Close();
                                    Console.SetOut(tmp);
                                    Console.WriteLine("\n{0}\n", ex.Message);
                                    break;
                               }
                                Console.WriteLine("{0} Sorry your input is invalid. \n", error);
                                Console.WriteLine("Please enter source path: \n");
                                sourceSQL = Console.ReadLine();
                                continue;
                            }
                            #endregion
                        }
                        break;
                    }
                #endregion
            }

            public bool CheckConnection()
            {
                dbConnection();
                return true;
            }

            public void ExecuteCommandSync(object command)
            {
                ProcessStartInfo procStartInfo = new ProcessStartInfo(@"c:\windows\system32\cmd.exe", "/c" + command);
                procStartInfo.RedirectStandardOutput = true;
                procStartInfo.UseShellExecute = false;
                procStartInfo.CreateNoWindow = true;

                Process proc = new Process();
                proc.StartInfo = procStartInfo;
                proc.Start();
                string result = proc.StandardOutput.ReadToEnd();
                Console.WriteLine(result);
            }

            public void BackCopy(DirectoryInfo source, DirectoryInfo target)
            {
                    if (Directory.Exists(target.FullName) == false)
                    {
                        Directory.CreateDirectory(target.FullName);
                        ascProcess ap = new ascProcess();
                        Console.WriteLine("please wait. creating directory {0}", target.FullName);
                        ap.threadWait();
                    }

                    foreach (FileInfo fi in source.GetFiles())
                    {
                        fi.CopyTo(Path.Combine(target.ToString(), fi.Name), true);
                    }

                    foreach (DirectoryInfo diSourceDir in source.GetDirectories())
                    {
                        DirectoryInfo nextTargetDir = target.CreateSubdirectory(diSourceDir.Name);
                        BackCopy(diSourceDir, nextTargetDir);
                    }
            }

            public void ProcessDirectory(string targetDirectory)
            {
                Console.WriteLine("\n{0} is a valid directory \n", targetDirectory);
            }

            public void threadWait()
            {
                char[] anims = new char[] { '/', '|', '\\' };
                int animsIndex = 0;
                for (int n = 0; n < 50; n++)
                {
                    Console.Write(anims[animsIndex]);
                    Thread.Sleep(100);
                    Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                    animsIndex++;
                    if (animsIndex >= anims.Length)
                        animsIndex = 0;
                }
            }

            public void Init()
            {
                ascProcess ascInit = new ascProcess();
                string dbMessage, backup_status, copy_status, source, error;
                bool checkFile = false,
                    checkPath = false;
                tmp = Console.Out;
                Console.WriteLine("\nAsctool version 1.0 ");
                Console.WriteLine("Apsmith (c) 2014 \n");
                    ascInit.threadWait();
                    try
                    {
                        if (ascInit.CheckConnection())
                        {

                            try
                            {
                                dbMessage = "Database connection has been established.";
                                Console.WriteLine(dbMessage);
                            }
                            catch (Exception ex)
                            {
                                Console.SetOut(sw);
                                Console.WriteLine(ex.Message);
                                sw.Close();
                            }

                            ascInit.threadWait();

                            ascInit.dbExecute();

                            dbMessage = "SQL query execution complete.";
                            Console.WriteLine("\n");
                            Console.WriteLine(dbMessage);
                            ascInit.threadWait();

                            #region try IIS STOP
                            try
                            {
                                ascInit.ExecuteCommandSync("iisreset /stop");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("\nAn Error Occured.\nPlease enter destination path for logs: \n");
                                error = Console.ReadLine();

                                #region errorPath
                                while (!checkPath)
                                {
                                    if ((File.Exists(error) || Directory.Exists(error)) && checkPath == false)
                                    {
                                        fs = new FileStream(@"" + error + "(" + DateTime.Now.ToString("MM-dd-yy_HHmm") + ")" + ".txt", FileMode.Create);
                                        sw = new StreamWriter(fs);
                                        tmp = Console.Out;
                                        Console.SetOut(sw);
                                        Console.WriteLine(ex.Message);
                                        sw.Close();
                                        Console.SetOut(tmp);
                                        Console.WriteLine(ex.Message);
                                        break;
                                    }
                                    Console.WriteLine("{0} Sorry your input is invalid. \n", error);
                                    Console.WriteLine("\nAn Error Occured.\nPlease enter destination path for logs: \n");
                                    error = Console.ReadLine();
                                    continue;
                                }
                                #endregion
                            }
                            #endregion


                            #region while source backUp
                            Console.WriteLine("\nAsctool version 1.0 ");
                            Console.WriteLine("Apsmith (c) 2014 \n");
                            Console.WriteLine("Please enter source path: " + "(e.g. " + @"C:\foldername\.." + ") \n");
                            source = Console.ReadLine();
                            while (!checkFile)
                            {
                                if ((Directory.Exists(source)) && checkFile == false && ascInit.IsDirectoryEmpty(source) == false)
                                {
                                    string sourceDirName = @"" + source;
                                    Console.WriteLine("\nPlease enter destination path: " + "(e.g. " + @"C:\foldername\.." + ") \n");
                                    string dirSource = Console.ReadLine();
                                    Console.WriteLine("\n");
                                    string destDirName = @"" + dirSource + "(" + File.GetLastWriteTime(sourceDirName).ToString("MM-dd-yy") + ")";
                                    if (Directory.Exists(sourceDirName))
                                    {
                                        DirectoryInfo sourceInfo = new DirectoryInfo(sourceDirName);
                                        DirectoryInfo destInfo = new DirectoryInfo(destDirName);
                                        ascInit.BackCopy(sourceInfo, destInfo);
                                        backup_status = "Backup successful.";
                                        Console.WriteLine(backup_status);
                                    }
                                    else
                                    {
                                        backup_status = "File doesn't exists.";
                                        Console.WriteLine(backup_status);
                                    }
                                    break;
                                }

                                Console.WriteLine("\nAn Error Occured.\nPlease enter destination path for logs: \n");
                                error = Console.ReadLine();

                                #region errorPath
                                while (!checkPath)
                                {
                                    if ((File.Exists(error) || Directory.Exists(error)) && checkPath == false)
                                    {
                                        fs = new FileStream(@"" + error + "(" + DateTime.Now.ToString("MM-dd-yy_HHmm") + ")" + ".txt", FileMode.Create);
                                        sw = new StreamWriter(fs);
                                        tmp = Console.Out;
                                        Console.SetOut(sw);
                                        Console.WriteLine("Sorry {0} is invalid or folder/s empty. \n", source);
                                        sw.Close();
                                        Console.SetOut(tmp);
                                        Console.WriteLine("Sorry {0} is invalid or folder/s empty. \n", source);
                                        Console.WriteLine("Please enter source path: " + "(e.g. " + @"C:\foldername\.." + ") \n");
                                        source = Console.ReadLine();
                                        break;
                                    }
                                    Console.WriteLine("{0} Sorry your input is invalid. \n", error);
                                    Console.WriteLine("\nAn Error Occured.\nPlease enter destination path for logs: \n");
                                    error = Console.ReadLine();
                                    continue;
                                }
                                #endregion
                                continue;
                            }
                            #endregion

                            ascInit.threadWait();

                            #region while CopyNew
                            Console.WriteLine("\nAsctool version 1.0 ");
                            Console.WriteLine("Apsmith (c) 2014 \n");
                            Console.WriteLine("Please enter source path: " + "(e.g. " + @"C:\foldername\.." + ") \n");
                            source = Console.ReadLine();
                            while (!checkFile)
                            {
                                if ((Directory.Exists(source)) && checkFile == false && ascInit.IsDirectoryEmpty(source) == false)
                                {
                                    string sourceDirName2 = @"" + source;
                                    Console.WriteLine("\nPlease enter destination path: " + "(e.g. " + @"C:\foldername\.." + ") \n");
                                    string dirSource = Console.ReadLine();
                                    Console.WriteLine("\n");
                                    string destDirName2 = @"" + dirSource;
                                    if (Directory.Exists(sourceDirName2))
                                    {
                                        DirectoryInfo sourceInfo2 = new DirectoryInfo(sourceDirName2);
                                        DirectoryInfo destInfo2 = new DirectoryInfo(destDirName2);
                                        ascInit.BackCopy(sourceInfo2, destInfo2);
                                        copy_status = "Copy successful.";
                                        Console.WriteLine(copy_status);
                                    }
                                    else
                                    {
                                        copy_status = "File doesn't exists.";
                                        Console.WriteLine(copy_status);
                                    }
                                    break;
                                }
                                Console.WriteLine("\nAn Error Occured.\nPlease enter destination path for logs: \n");
                                error = Console.ReadLine();

                                #region errorPath
                                while (!checkPath)
                                {
                                    if ((File.Exists(error) || Directory.Exists(error)) && checkPath == false)
                                    {
                                        fs = new FileStream(@"" + error + "(" + DateTime.Now.ToString("MM-dd-yy_HHmm") + ")" + ".txt", FileMode.Create);
                                        sw = new StreamWriter(fs);
                                        tmp = Console.Out;
                                        Console.SetOut(sw);
                                        Console.WriteLine("Sorry {0} is invalid or folder/s empty. \n", source);
                                        sw.Close();
                                        Console.SetOut(tmp);
                                        Console.WriteLine("Sorry {0} is invalid or folder/s empty. \n", source);
                                        Console.WriteLine("Please enter source path: " + "(e.g. " + @"C:\foldername\.." + ") \n");
                                        source = Console.ReadLine();
                                        break;
                                    }
                                    Console.WriteLine("{0} Sorry your input is invalid. \n", error);
                                    Console.WriteLine("\nAn Error Occured.\nPlease enter destination path for logs: \n");
                                    error = Console.ReadLine();
                                    continue;
                                }
                                #endregion

                                continue;
                            }
                            #endregion

                            ascInit.threadWait();

                            Console.WriteLine("\n");

                            #region try IIS Start
                            try
                            {
                                ascInit.ExecuteCommandSync("iisreset /start");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("\nAn Error Occured.\nPlease enter destination path for logs: \n");
                                error = Console.ReadLine();

                                #region errorPath
                                while (!checkPath)
                                {
                                    if ((File.Exists(error) || Directory.Exists(error)) && checkPath == false)
                                    {
                                        fs = new FileStream(@"" + error + "(" + DateTime.Now.ToString("MM-dd-yy_HHmm") + ")" + ".txt", FileMode.Create);
                                        sw = new StreamWriter(fs);
                                        tmp = Console.Out;
                                        Console.SetOut(sw);
                                        Console.WriteLine(ex.Message);
                                        sw.Close();
                                        Console.SetOut(tmp);
                                        Console.WriteLine(ex.Message);
                                        break;
                                    }
                                    Console.WriteLine("{0} Sorry your input is invalid. \n", error);
                                    Console.WriteLine("\nAn Error Occured.\nPlease enter destination path for logs: \n");
                                    error = Console.ReadLine();
                                    continue;
                                }
                                #endregion
                            }
                            #endregion


                            Console.WriteLine("Deployment process complete. " + DateTime.Now.ToString("MM-dd-yy HH:mm"));
                            Environment.Exit(0);
                        }
                        else
                        {
                            dbMessage = "SQL query execution incomplete.";
                            Console.WriteLine(dbMessage);
                        }
                    }
                    catch (Exception ex)
                    {
                        #region errorLogs
                        Console.WriteLine("\nAn Error Occured.\nPlease enter destination path for logs: \n");
                        error = Console.ReadLine();
                        while (!checkPath)
                        {
                            if ((File.Exists(error) || Directory.Exists(error)) && checkPath == false)
                            {
                                fs = new FileStream(@"" + error + "(" + DateTime.Now.ToString("MM-dd-yy_HHmm") + ")" + ".txt", FileMode.Create);
                                sw = new StreamWriter(fs);
                                Console.SetOut(sw);
                                Console.WriteLine(ex.Message);
                                Console.SetOut(tmp);
                                Console.WriteLine(ex.Message);
                                sw.Close();
                                break;
                            }
                            Console.WriteLine("{0} Sorry your input is invalid. \n", error);
                            Console.WriteLine("\nAn Error Occured.\nPlease enter destination path for logs: \n");
                            error = Console.ReadLine();
                            continue;
                        }
                        #endregion
                    }
                    
            }

            public void ShowHelp(OptionSet p)
            {                
                Console.WriteLine("\nAsctool version 1.0 ");
                Console.WriteLine("Apsmith (c) 2014");

                #region extra Greetings
                /*
            Console.WriteLine ("Usage: greet [OPTIONS]+ message");
            Console.WriteLine ("Greet a list of individuals with an optional message.");
            Console.WriteLine ("If no message is specified, a generic greeting is used.");
            */
                #endregion

                Console.WriteLine("\nOptions: \n");
                p.WriteOptionDescriptions(Console.Out);
                Environment.Exit(0);
             }

            public bool IsDirectoryEmpty(string path)
            {
                return !Directory.EnumerateFileSystemEntries(path).Any();
            }
        }
        #endregion
    }
}        
