using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Data.SQLite;
using System.Threading;
using System.Net;
using MultimediaUpdater;

namespace Narriox
{
    public partial class books : Form
    {

        static string current_location = System.Reflection.Assembly.GetExecutingAssembly().Location;
        static string current_directory = Directory.GetParent(current_location).FullName;
        static string scrip_location = current_directory + Path.DirectorySeparatorChar + "med";

        static string conncode = string.Format(@"URI = file:{0}\\edu.buff", scrip_location);
        static SQLiteConnection mvies = new SQLiteConnection(conncode);

        public books()
        {
            InitializeComponent();
        }

        private void books_Load(object sender, EventArgs e)
        {
            search.Visible = false;
            new Thread(() => {
                //fill in ads

                try
                {
                    var request = WebRequest.Create("http://erchatt.webs.com/new.jpg");

                    using (var response = request.GetResponse())
                    using (var stream = response.GetResponseStream())
                    {
                        newbooksimage.Image = Bitmap.FromStream(stream);
                    }


                }
                catch 
                {
                    newbooksimage.ImageLocation = "data\\adbuff1";
                    newbooksimage.Show();
                }

            }).Start();
            new Thread(() => {
                //fill in ads

                try
                {
                    var request = WebRequest.Create("http://erchatt.webs.com/fav.jpg");

                    using (var response = request.GetResponse())
                    using (var stream = response.GetResponseStream())
                    {
                        favbooksimage.Image = Bitmap.FromStream(stream);
                    }



                }
                catch 
                {
                    favbooksimage.ImageLocation = "data\\adbuff2";
                    favbooksimage.Show();
                }

            }).Start();

            new Thread(() => {
                //fill in ads

                try
                {
                    var request = WebRequest.Create("http://erchatt.webs.com/main.jpg");

                    using (var response = request.GetResponse())
                    using (var stream = response.GetResponseStream())
                    {
                        mainbooksimage.Image = Bitmap.FromStream(stream);
                    }



                }
                catch 
                {
                    mainbooksimage.ImageLocation = "data\\adbuff3";
                    mainbooksimage.Show();
                }

            }).Start();

           
            SQLiteConnection lnks = new SQLiteConnection(BooksUpdate.conncode);
            try
            {
                lnks.Open();
                mvies.Open();
            }
            catch
            {

            }
            

            SQLiteCommand cd = new SQLiteCommand(mvies)
            {
                CommandText = "delete from books"
            };


            new Thread(() =>
            {
                try
                {
                    
                    cd.ExecuteNonQuery();
                    SQLiteDataReader reader = new SQLiteCommand(lnks) { CommandText = "select * from links" }.ExecuteReader();
                    while (reader.Read())
                    {
                        
                        string str = (string)reader["link"];

                        try
                        {
                            string str2 = new WebClient().DownloadString(str);

                            foreach (string str3 in MultimediaUpdater.LinkFinder.Find(str2))
                            {

                                if (str3.Contains("pdf"))
                                {
                                    try
                                    {
                                        string flink = string.Format("{0}/{1}", str, str3);
                                        booklist.Items.Add(str3);
                                        new SQLiteCommand(mvies) { CommandText = $"insert into books values ('{str3}', '{flink}')" }.ExecuteNonQuery();

                                    }
                                    catch
                                    {
                                        continue;
                                    }
                                }
                        
                            }

                        }
                        catch
                        {
                            continue;
                        }
                    }
                }
                catch 
                {
                    MessageBox.Show("Critical Error !! ");
                    Application.Restart();
                }

                finally
                {
                    mvies.Close();
                    lnks.Close();
                }
                //Form.ActiveForm.Enabled = true;

                mvies.Close();
                lnks.Close();
                search.Visible = true;
            }).Start();
            

        }
        private void download_book_Click(object sender, EventArgs e)
        {
            new Thread(()=> {
                try
                {
                    string cbook = booklist.Text;


                    SQLiteConnection mvies2 = new SQLiteConnection(conncode);
                    mvies2.Open();
                    SQLiteCommand cmd = new SQLiteCommand(mvies2);

                    string ctext = string.Format("select * from books where name = '{0}'", cbook);
                    cmd.CommandText = ctext;

                    SQLiteDataReader mvreader = cmd.ExecuteReader();
                    if (mvreader.Read())
                    {
                        string name = (string)mvreader["name"];
                        string link = (string)mvreader["link"];
                        new Thread(() =>
                        {
                            WebClient wb1 = new WebClient();
                            MessageBox.Show("Downloading ", name);
                            wb1.DownloadFile(link, string.Format("Downloads\\{0}", name));
                            MessageBox.Show("Downloading done ", name);
                        }).Start();

                    }
                    mvies2.Close();

                }
                catch { }
            }).Start();
            
        }

        private void search_TextChanged(object sender, EventArgs e)
        {
            string searchtext  = search.Text.ToString();
            if (searchtext.Length>2)
            {
                SQLiteConnection lnks = new SQLiteConnection(BooksUpdate.conncode);
                try
                {
                    lnks.Open();
                    mvies.Open();
                }
                catch
                {

                }

                new Thread(() => {
                    booklist.Items.Clear();

                    SQLiteCommand cmd = new SQLiteCommand(mvies);
                    cmd.CommandText = "select * from books";
                    SQLiteDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        string name = (string)dr["name"];
                        string link = (string)dr["link"];

                        if (name.ToUpper().Contains(searchtext.ToUpper()) | link.ToUpper().Contains(searchtext.ToUpper()))
                        {
                            booklist.Items.Add(name);
                        }
                        else
                        {

                        }
                    }

                }).Start();
            }
            
        }
    }


    class Book
    {

        static string current_location = System.Reflection.Assembly.GetExecutingAssembly().Location;
        static string current_directory = Directory.GetParent(current_location).FullName;
        static string scrip_location = current_directory + Path.DirectorySeparatorChar + "med";


        static string conncode = string.Format(@"URI = file:{0}\\edu.buff", scrip_location);
        SQLiteConnection sql = new SQLiteConnection(conncode);

        public string movie_id;
        public string bookName, bookTopic, avail, link;
        public string bksFolder;

        public static WebClient usr;

        public Book(string movie_id)
        {
            this.movie_id = movie_id;
            Thread.Sleep(2);
            getDEtails();
        }

        void getDEtails()
        {

            sql.Open();

            SQLiteCommand cmd = new SQLiteCommand(sql);
            cmd.CommandText = String.Format("select * from books where bookName = '{0}'", movie_id);
            SQLiteDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                bookName = (string)dr["bookName"];
                link = (string)dr["link"];
                avail = (string)dr["availability"];
                bookTopic = (string)dr["topicName"];
            }
            sql.Close();
        }

        public void _Download()
        {
            try
            {
                bksFolder = string.Format("med\\medbuff\\");
                usr = new WebClient();

                bksFolder = string.Format("med\\medbuff\\{0}", bookName);
                MessageBox.Show("Download Task Added!\n", bookName);
                usr.DownloadFile(link, fileName: bksFolder);
                usr.Dispose();
                //MessageBox.Show("Download Complete : ", movie_name);


                sql.Open();
                SQLiteCommand cmd = new SQLiteCommand(sql);
                cmd.CommandText = string.Format("update books set availability = 'offline' where bookName = '{0}'", bookName);
                cmd.ExecuteNonQuery();
                sql.Close();

                MessageBox.Show("Download Completed : ", bookName);



            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                MessageBox.Show("Book download failed\nError : ", ex.Message);
                return;
            }


        }

        public void Cancel()
        {
            usr.Dispose();
            usr.CancelAsync();
            usr.Dispose();
            Thread.CurrentThread.Abort();
            MessageBox.Show("Download Canceled");
            //File.Delete(mv_folder);
        }

    }
}
