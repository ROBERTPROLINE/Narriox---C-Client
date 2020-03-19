using Narriox;
using System;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Forms;

internal class Program
{
    // Methods
    private static void clearAccounts()
    {
        try
        {
            File.Delete(@"data\cred98.db");
        }
        catch
        {
            MessageBox.Show("Failed to intialize [Error : 234]");
        }
    }

    [STAThread]
    private static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        using (Process process = Process.GetCurrentProcess())
        {
            process.PriorityClass = ((ProcessPriorityClass)ProcessPriorityClass.High);
        }
        string str3 = Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName + Path.DirectorySeparatorChar.ToString() + "data";

        
        try
        {
            new Thread(()=> {
                string conncode1 = string.Format(@"URI = file:{0}\\media-movies-links.db", str3);
                SQLiteConnection cn = new SQLiteConnection(conncode1);
                cn.Open();
                    SQLiteCommand cd = new SQLiteCommand(cn);
                    cd.CommandText = "delete from movie_links";
                    try
                    {
                        cd.ExecuteNonQuery();
                        SQLiteCommand command = new SQLiteCommand(cn);
                        string address = "https://erchatt.webs.com/movies";
                        string file = new WebClient().DownloadString(address);
                        foreach (string str4 in LinkFinder.Find(file))
                        {
                            if (!str3.Contains("webs.com"))
                            {
                                try
                                {
                                    command.CommandText = $"insert into movie_links values ('{str4}', 'no-genre','no-desc')";
                                    command.ExecuteNonQuery();
                                    
                                }
                                catch (Exception exception)
                                {
                                    MessageBox.Show(exception.Message);
                                }
                            }
                        }
                    }
                    catch (Exception exception2)
                    {
                        Console.WriteLine(exception2.Message);
                    }
                    cn.Close();

            }).Start();

            new Thread(() =>
            {
                if (!File.Exists("lstupdate"))
                {
                    SQLiteConnection updateReaderCreate = new SQLiteConnection($"URI=file:{str3}\\lstpdate");
                    updateReaderCreate.Open();
                    SQLiteCommand sql = new SQLiteCommand(updateReaderCreate);
                    sql.CommandText = "create table updates(lstpdate text)";
                    sql.ExecuteNonQuery();
                    updateReaderCreate.Close();

                }
                else
                {

                }

                string lstupdate1 = "";
                string nextupdate = "Wednesday, March 25, 2020";
                SQLiteConnection updateReader = new SQLiteConnection($"URI=file:{str3}\\lstpdate");
                updateReader.Open();
                SQLiteCommand cmd = new SQLiteCommand(updateReader);
                cmd.CommandText = "select * from updates";
                SQLiteDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    lstupdate1 = (string)dr["lstupdate"];
                }
                if ((DateTime.Parse(nextupdate).Date)<=DateTime.Now)
                {
                    if (lstupdate1==nextupdate)
                    {
                        Thread.CurrentThread.Priority = ThreadPriority.Highest;
                        string[] strArray = new string[] {"updater.exe" };
                        foreach (string str in strArray)
                        {
                            ProcessStartInfo startInfo = new ProcessStartInfo();
                            startInfo.FileName = str;
                            startInfo.UseShellExecute = false;
                            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                            //Start the process
                            Process proc = Process.Start(startInfo);

                        }
                    }
                    else
                    {

                    }
                }
                else
                {

                }
                
            }).Start();

        }
        catch (Exception exception)
        {
            MessageBox.Show(exception.ToString());
        }
        if (!File.Exists(@"data\cred98"))
        {
            Application.Run(new welcome("welcome new user", "Welcome !!"));
        }
        else
        {
            try
            {
                SQLiteConnection connection = new SQLiteConnection($"URI=file:{str3}\\cred98");
                connection.Open();
                SQLiteCommand command = new SQLiteCommand(connection)
                {
                    CommandText = "select * from credentials"
                };
                command.Prepare();
                SQLiteDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    string str5 = (string)reader["name"];
                    string str6 = (string)reader["package"];
                    string str7 = (string)reader["signature"];
                    string s = (string)reader["expiry"];
                    string str9 = (string)reader["pcuid"];
                    if ((str7 != Environment.UserDomainName) | (str9 != Environment.UserName))
                    {
                        connection.Close();
                        clearAccounts();
                    }
                    if (str6 == "expired")
                    {
                        MessageBox.Show("Please Subscribe to use our Service");
                        Application.Run(new createAccount());
                        connection.Close();
                    }
                    if (DateTime.Parse(s).Date <= DateTime.Now.Date)
                    {
                        new SQLiteCommand(connection) { CommandText = "update credentials set package = 'expired'" }.ExecuteNonQuery();
                        MessageBox.Show("Expiry date reached\nPlease subscribe");
                        Application.Restart();
                        connection.Close();
                    }
                    else
                    {
                        Application.Run(new load_main());
                        connection.Close();
                    }
                    connection.Close();
                }
                connection.Close();
            }
            catch (Exception exception2)
            {
                MessageBox.Show(exception2.Message);
            }
        }
    }
}                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  