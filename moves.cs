using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Threading;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;


//vana-sekai

namespace Narriox
{

    public partial class moves : Form
    {
        public static Dictionary<string, string[]> movie_download;

        static string current_location = System.Reflection.Assembly.GetExecutingAssembly().Location;
        static string current_directory = Directory.GetParent(current_location).FullName;
        static string scrip_location = current_directory + Path.DirectorySeparatorChar + "data";

        static Dictionary<string, string> downloads;

        static string conncode = string.Format(@"URI = file:{0}\\media-movies-links.db", scrip_location);
        static SQLiteConnection mvies = new SQLiteConnection(conncode);


        public moves()
        {
            InitializeComponent();
            downloads = new Dictionary<string, string>(5);
            downloads.Clear();
            btndownload.BackColor = Color.LawnGreen;
        }

        private void moves_Load(object sender, EventArgs e)
        {
            //if movie available offline replace download with watch now
            new Thread(() => {
                //fill in ads

                try
                {
                    var request = WebRequest.Create("http://erchatt.webs.com/movies2.jpg");

                    using (var response = request.GetResponse())
                    using (var stream = response.GetResponseStream())
                    {
                        moviepic.Image = Bitmap.FromStream(stream);
                    }



                }
                catch 
                {
                    moviepic.ImageLocation = "data\\adbuff";
                    moviepic.Show();
                }

            }).Start();
            new Thread(() =>
            {
                try
                {
                    search.Visible = false;
                    TempFile t = new TempFile();

                    string conncode1 = string.Format(@"URI = file:{0}\\media-movies-links.db", scrip_location);
                    SQLiteConnection cn = new SQLiteConnection(conncode1);
                    cn.Open();

                    SQLiteCommand cmd = new SQLiteCommand(cn);
                    cmd.CommandText = "select * from movie_links";
                    SQLiteDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        
                        string addrs = (string)dr["link"];

                        try
                        {
                            WebClient wb = new WebClient();
                            string s = wb.DownloadString(addrs);

                            foreach (string i in LinkFinder.Find(s))
                            {
                                string webaddrs = string.Format("{0}/{1}", addrs, i);

                                TempFile tmp = new TempFile(i, webaddrs);
                                movielist.Items.Add(i);
                            }
                            wb.Dispose();
                        }
                        catch 
                        {
                            
                            continue;
                        }
                        
                    }
                    cn.Close();
                }
                catch (Exception ex) { MessageBox.Show(ex.ToString()); }
                search.Visible = true;
            }).Start();



        }

        private void genrelist_SelectedIndexChanged(object sender, EventArgs e)
        {
            movielist.Items.Clear();

            new Thread(()=> {
                try
                {
                    /**
                    string Gen = genrelist.Text;
                    string mvigenre = string.Format("cache_data\\movie_data\\{0}.png", Gen);
                    genrepic.ImageLocation = mvigenre;
                    genrepic.Show();

                    btndownload.Text = "Download Movies";
                    moviepic.Visible = false;
                    moviename.Text = "Movie  Name";
                    moviesize.Text = "Size in GB";
                    
                    mvies.Open();
                    SQLiteCommand cmd = new SQLiteCommand(mvies);

                    string ctext = string.Format("select * from movies where genre = '{0}'", Gen);
                    cmd.CommandText = ctext;

                    SQLiteDataReader mvreader = cmd.ExecuteReader();
                    while (mvreader.Read())
                    {
                        string mvname = (string)mvreader["name"];
                        Movie mv = new Movie(mvname);
                        movielist.Items.Add(mv.movie_name);
                                          
                    }
                    mvies.Close();
                 
                    **/


                    SQLiteConnection cn = new SQLiteConnection(conncode);
                    cn.Open();

                    SQLiteCommand cmd = new SQLiteCommand(cn);
                    cmd.CommandText = string.Format("select * from movie_links where genre = '{0}'", genrelist.SelectedItem);
                    SQLiteDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        string addrs1 = (string)dr["link"];
                        
                        try
                        {
                            WebClient wb = new WebClient();
                            string s = wb.DownloadString(addrs1);

                            foreach (string i in LinkFinder.Find(s))
                            {
                                string webaddrs = string.Format("{0}/{1}", addrs1, i);

                                TempFile tmp = new TempFile(i, webaddrs);
                                movielist.Items.Add(i);
                            }
                            wb.Dispose();
                        }
                        catch
                        {
                            continue;
                        }

                    }
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            }).Start();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form.ActiveForm.Hide();
        }

        private void btndownload_Click(object sender, EventArgs e)
        {
            try
            {
                string cm = movielist.SelectedItem.ToString();
                TempFile tmp = new TempFile(cm);
                string mname = tmp.retRieveCache();
                new Thread(()=> {
                    Movie mv = new Movie(mname, cm);
                    mv._Download();
                }).Start();
               

            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void btnTrailer_Click(object sender, EventArgs e)
        {
            new Thread(() =>
            {
                //download trailer and play

            }).Start();
        }
        private void movielist_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string current_movie = movielist.SelectedItem.ToString();


            }
            catch { }
            return;

        }

        private void movie_link_Click(object sender, EventArgs e)
        {
            TempFile retr = new TempFile(movielist.SelectedItem.ToString());
            string pml = retr.retRieveCache();
            txtlink.Text = pml;
        }

        private void search_TextChanged(object sender, EventArgs e)
        {
            new Thread(()=> {
                try {
                    string searchtext = search.Text.ToString();
                    string conncode = string.Format(@"URI = file:{0}\\tempData.movies", scrip_location);
                    SQLiteConnection tmpConn = new SQLiteConnection(conncode);

                    if (searchtext.Length > 2)
                    {
                        try
                        {

                            tmpConn.Open();
                        }
                        catch
                        {

                        }

                        new Thread(() => {
                            movielist.Items.Clear();

                            SQLiteCommand cmd = new SQLiteCommand(mvies);
                            cmd.CommandText = "select * from tmpfile";
                            SQLiteDataReader dr = cmd.ExecuteReader();
                            while (dr.Read())
                            {
                                string name = (string)dr["tmpfileName"];
                                string link = (string)dr["permalink"];

                                if (name.ToUpper().Contains(searchtext.ToUpper()) | link.ToUpper().Contains(searchtext.ToUpper()))
                                {
                                    movielist.Items.Add(name);
                                }
                                else
                                {

                                }
                            }
                            tmpConn.Close();
                        }).Start();
                    }
                }
                catch
                {

                }
            }).Start();
        }
    }

    class Movie
    {

        static string current_location = System.Reflection.Assembly.GetExecutingAssembly().Location;
        static string current_directory = Directory.GetParent(current_location).FullName;
        static string scrip_location = current_directory + Path.DirectorySeparatorChar + "data";


        static string conncode = string.Format(@"URI = file:{0}\\media-movies.db", scrip_location);
        string movi_link, movie_name;

        SQLiteConnection sql = new SQLiteConnection(conncode);

        static WebClient usr;

        public Movie(string link, string name)
        {
            this.movi_link = link;
            this.movie_name = name;
        }

        public void _Download()
        {
            try
            {

                usr = new WebClient();
                MessageBox.Show("Download Task Added!\n", movie_name);
                usr.DownloadFile(movi_link, fileName: string.Format("Downloads\\{0}",movie_name));
                usr.Dispose();

                MessageBox.Show("Download Complete : ", movie_name);
            }
            catch
            {
                MessageBox.Show("Movie download failed\nError");
            }


        }
    }

    public struct LinkItem
            {
                public string Href;
                public string Text;

                public override string ToString()
                {
                    return Href + "\n\t" + Text;
                }
            }

    static class LinkFinder
            {
                public static List<string> Find(string file)
                {
                    List<string> list = new List<string>();

                    // 1.
                    // Find all matches in file.
                    MatchCollection m1 = Regex.Matches(file, @"(<a.*?>.*?</a>)",
                        RegexOptions.Singleline);

                    // 2.
                    // Loop over each match.
                    foreach (Match m in m1)
                    {
                        string value = m.Groups[1].Value;
                        LinkItem i = new LinkItem();


                        // 3.
                        // Get href attribute.
                        Match m2 = Regex.Match(value, @"href=\""(.*?)\""",
                            RegexOptions.Singleline);
                        if (m2.Success)
                        {
                            i.Href = m2.Groups[1].Value;
                        }

                        // 4.
                        // Remove inner tags from text.
                        string t = Regex.Replace(value, @"\s*<.*?>\s*", "",
                            RegexOptions.Singleline);
                        i.Text = t;

                        string fileAddress = string.Format("{0}", m2);
                //MessageBox.Show(finalLocaltion);
                     if (fileAddress.Contains("mkv") | fileAddress.Contains("mp4"))
                        {
                           
                            string file2 = fileAddress.Remove(0, 6);
                            string finalLocaltion = file2.TrimEnd('"');
                            list.Add(finalLocaltion);
                        }


                    }
                    return list;
                }
            }

    public class TempFile
            {
                public string tmpFolder = "tempData.movies", tmpFileName, PermaLink;

                static string current_location = System.Reflection.Assembly.GetExecutingAssembly().Location;
                static string current_directory = Directory.GetParent(current_location).FullName;
                static string scrip_location = current_directory + Path.DirectorySeparatorChar + "data";

                public TempFile(string tmpFile, string pml)
                {

                    this.tmpFileName = tmpFile;
                    this.PermaLink = pml;
                    SaveCAche();
                }

                public TempFile(string tempFile)
                {
                    this.tmpFileName = tempFile;
                }

                public TempFile()
                {
                    try
                    {
                        string conncode = string.Format(@"URI = file:{0}\\{1}", scrip_location, tmpFolder);
                        SQLiteConnection tmpConn = new SQLiteConnection(conncode);
                        tmpConn.Open();

                        SQLiteCommand cmd = new SQLiteCommand(tmpConn);
                        cmd.CommandText = string.Format("delete from tmpfile");
                        cmd.ExecuteNonQuery();

                        tmpConn.Close();
                    }
                    catch
                    {

                        MessageBox.Show("Failed to clean dumps\nRestart app to run smoothly");
                    }
                }

                public void SaveCAche()
                {
                    try
                    {
                        string conncode = string.Format(@"URI = file:{0}\\{1}", scrip_location, tmpFolder);
                        SQLiteConnection tmpConn = new SQLiteConnection(conncode);
                        tmpConn.Open();

                        SQLiteCommand cmd = new SQLiteCommand(tmpConn);
                        cmd.CommandText = string.Format("insert into tmpfile values('{0}','{1}','{2}')", tmpFileName, PermaLink, DateTime.Now.ToLongDateString());
                        cmd.ExecuteNonQuery();

                        tmpConn.Close();
                    }
                    catch
                    {

                        MessageBox.Show("Failed to save movie data \nRestart app to run smoothly");
                    }


                }

                public string retRieveCache()
                {
                    this.PermaLink = "";

                    try
                    {
                        string conncode = string.Format(@"URI = file:{0}\\{1}", scrip_location, tmpFolder);
                        SQLiteConnection tmpConn = new SQLiteConnection(conncode);
                        tmpConn.Open();

                        SQLiteCommand cmd = new SQLiteCommand(tmpConn);
                        cmd.CommandText = string.Format("select * from tmpfile where tmpfileName = '{0}'", this.tmpFileName);
                        SQLiteDataReader reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            this.PermaLink = (string)reader["permalink"];

                        }
                        tmpConn.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());

                    }

                    return PermaLink;
                }

            }
}






