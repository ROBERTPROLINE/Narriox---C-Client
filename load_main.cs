using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Net;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Data.SQLite;
using System.Diagnostics;

namespace Narriox
{
    public partial class load_main : Form
    {
        public static string uname ;
        public static string upwd;
        public static string fullname;
        public static string bmessage;
        public static string reg_status;

        public static Dictionary<string,int> msg1;
        public static Dictionary<string, int> msg2;

        public static Dictionary<string, string[]> favouritz1;
        public static Dictionary<string, string[]> favouritz2;

        public static string[] multimedia = new string[2];
        public static string[] newoncourses = new string[2];

        public static string current_location = System.Reflection.Assembly.GetExecutingAssembly().Location;
        public static string current_directory = Directory.GetParent(current_location).FullName;
        public static string scrip_location = current_directory + Path.DirectorySeparatorChar;

        public static IPHostEntry iphost = Dns.GetHostEntry(Dns.GetHostName());
        public static IPAddress ipaddr = IPAddress.Parse("127.0.0.1");

        
        public static string cs = string.Format(@"URI=file:{0}\\data\\cred98", scrip_location);
        public static SQLiteConnection conn = new SQLiteConnection(cs);

        public load_main()
        {
            InitializeComponent();
        }
        public static void display_new_content()
        {
            //display new content on main::form

        }

        private void load_main_Load(object sender, EventArgs e)
        {
            new Thread(()=> {
                string[] strArray = new string[] { "MultimediaUpdater.exe" };
                foreach (string str in strArray)
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.FileName = str;
                    startInfo.UseShellExecute = false;
                    startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    //Start the process
                    Process proc = Process.Start(startInfo);

                }
            }).Start();
            new Thread(()=> {
                //fill in ads

                try
                {
                    var request = WebRequest.Create("http://erchatt.webs.com/movies.jpg");

                    using (var response = request.GetResponse())
                    using (var stream = response.GetResponseStream())
                    {
                        advertpic1.Image = Bitmap.FromStream(stream);
                    }

                  

                }
                catch 
                {
                    advertpic1.ImageLocation = "data\\adbuff";
                    advertpic1.Show();
                }
                
            }).Start();
           

            conn.Open();
            SQLiteCommand cmd = new SQLiteCommand(conn);
            cmd.CommandText = "select * from credentials";
            cmd.Prepare();
            SQLiteDataReader reader = cmd.ExecuteReader(System.Data.CommandBehavior.SingleRow);
            if(reader.Read())
            {
                string packg1 = "";
                //name.Text = (string)reader["fullname"];
                username.Text = (string)reader["name"];
                string packg = (string)reader["package"];
                //MessageBox.Show("Your Subscription is : "+packg);

                if (packg ==("1"))
                {
                    package.BackColor = Color.Brown;
                    packg1 = "Basic";
                }
                if (packg == "2" | packg ==  "gold")
                {
                    package.BackColor = Color.Gold;
                    packg1 = "Gold";
                }
                if (packg == ("3") | packg == "platinum")
                {
                    package.BackColor = Color.Indigo;
                    packg1 = "Platinum";
                }
                if (packg == "4" | packg == "trial")
                {
                    package.BackColor = Color.Crimson;
                    packg1 = "Trial";
                }

                if(packg == "5" | packg == "")
                {
                    packg1 = "Subscribe Now";
                }
                package.Text = packg1;
                conn.Close();
            }
            conn.Close();

            /**
            new Thread(() => {
                //Thread.CurrentThread.Priority = ThreadPriority.Highest;
                while (true)
                {
                    try
                    {
                        checkSubscription();
                        Thread.Sleep(1500);
                    }catch(Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }

            }).Start();
        **/

            if (File.Exists("bin\\img.buf"))
            {
                propic.ImageLocation = "bin\\img.buf";
                propic.Show();
            }
            else
            {
                MessageBox.Show("Please Update yoy image !\nclick the box below Account info");
            }
            string adverts = scrip_location + "lat-data";

            time_.Text = DateTime.Today.ToLongDateString();
          

        }

        public static void checkSubscription()
        {
             int portNum = 98;
             IPEndPoint r_server = new IPEndPoint(ipaddr, portNum);
             Socket mysoc = new Socket(ipaddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            //conn.Open();
            string username = "", password = "", package = "";
            SQLiteConnection conn1 = new SQLiteConnection(cs);
            conn1.Open();
            SQLiteCommand sql = new SQLiteCommand(conn1);
            sql.CommandText = "select * from credentials";
            SQLiteDataReader dr = sql.ExecuteReader();
            while (dr.Read()) {

                username = (string)dr["name"];
                password = (string)dr["pwd"];
                package = (string)dr["package"];
            }

           conn1.Close();

            try
            {
                mysoc.Connect(r_server);

                while (mysoc.Connected)
                {
                    byte[] server_msg = new byte[512];
                    int byterecv = mysoc.Receive(server_msg);

                    string uidpwd = String.Format("{0}::{1}", username, password);
                    byte[] msg = Encoding.ASCII.GetBytes(uidpwd);
                    int msg_sent = mysoc.Send(msg);

                    byte[] msg_recv = new byte[1024];
                    int bytrecv = mysoc.Receive(msg_recv);

                    string authcode = Encoding.ASCII.GetString(msg_recv);
                    //MessageBox.Show(authcode);
                    mysoc.Close();
                    if (authcode.Contains("package-expired"))
                    {
                        Form.ActiveForm.Enabled = false;
                        MessageBox.Show("Please Subscribe to use our Service");
                        
                        SQLiteCommand cd = new SQLiteCommand(conn1);
                        cd.CommandText = "update credentials set package = '5'";
                        try
                        {
                            cd.ExecuteNonQuery();
                            MessageBox.Show("subscription Expired !\nPlease Subscribe");
                            
                            Application.Exit();
                        }
                        catch
                        {
                            MessageBox.Show("Critical Error : Contact customer service if error persists\nError copde [db-0]");
                            conn.Close();
                            
                            Application.Exit();
                        }
                        Thread.CurrentThread.Abort();
                        Application.Exit();
                    }
                    if (authcode.Contains("drman"))
                    {
                        Thread.Sleep(250000);
                        string[] pkg = authcode.Split('-');
                        package = pkg[1];
                        string nump = "";
                        //MessageBox.Show(package);
                        if (package.Contains("basic"))
                        {
                            nump = "1";
                            SQLiteCommand d = new SQLiteCommand(conn);
                            string d1 = string.Format("update credentials set package = '{0}'", nump);
                            d.CommandText = d1;
                            d.ExecuteNonQuery();
                            conn.Close();
                        }
                        else if (package.Contains("gold"))
                        {
                            nump = "2";
                            SQLiteCommand d = new SQLiteCommand(conn);
                            string d1 = string.Format("update credentials set package = '{0}'", nump);
                            d.CommandText = d1;
                            d.ExecuteNonQuery();
                            conn.Close();
                        }
                        else if (package.Contains("platinum"))
                        {
                            //MessageBox.Show("Package is platinum");
                            nump = "3";
                            SQLiteCommand d = new SQLiteCommand(conn);
                            string d1 = string.Format("update credentials set package = '{0}'", nump);
                            d.CommandText = d1;
                            d.ExecuteNonQuery();
                            conn.Close();
                        }
                        else
                        {
                            MessageBox.Show(authcode);
                            nump = "5";
                            SQLiteCommand d = new SQLiteCommand(conn);
                            string d1 = string.Format("update credentials set package = '{0}'", nump);
                            d.CommandText = d1;
                            d.ExecuteNonQuery();
                            conn.Close();
                            Form.ActiveForm.Enabled = false;
                            Thread.CurrentThread.Abort();
                            Application.Run(new createAccount());
                        }

                        conn1.Close();

                    }
                    else
                    {
                        Form.ActiveForm.Enabled = false;
                        MessageBox.Show("Criticat failure in App Databases !!\nExiting Now\nContact Cutomer Support\nemail : robertsibanda20@gmail.com");
                        conn.Close();
                        Thread.CurrentThread.Abort();
                        Application.Exit();
                    }
                }
            }

            catch
            {
               
            }
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process pr =  System.Diagnostics.Process.GetCurrentProcess();
          
            pr.Kill();

            Application.Exit();
        }

        private void propic_Click(object sender, EventArgs e)
        {

            try
            {
                OpenFileDialog fd = new OpenFileDialog();
                fd.ShowDialog();

                File.Delete("bin\\img.buf");
                string pc = fd.FileName;
                File.Copy(pc, "bin\\img.buf");

                propic.ImageLocation = "bin\\img.buf";
                propic.Show();
                fd.Dispose();
            }
            catch { }
            
        }

        private void btnbooks_Click(object sender, EventArgs e)
        {
            books books = new books();
            Form bks = books;
            bks.StartPosition = FormStartPosition.CenterScreen;
            bks.Show();
        }

        private void btnNotes_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Notes comming soon\nPlease check after 5 days");
        }

        private void btnmovies_Click(object sender, EventArgs e)
        {
            Thread.Sleep(2);
            Form bks = new moves();
            bks.StartPosition = FormStartPosition.CenterScreen;
            bks.Show();
        }

        private void syncbar_Click(object sender, EventArgs e)
        {

        }

        private void package_Click(object sender, EventArgs e)
        {
            new Thread(() => {
                //background services
                Thread.CurrentThread.Priority = ThreadPriority.Highest;
                string[] backgr = { "Activation.exe", "updater.exe" };
                foreach (string app in backgr)
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo();

                    startInfo.FileName = app;

                    startInfo.WindowStyle = ProcessWindowStyle.Normal;
                  
                    Process process = new Process();

                    process.StartInfo = startInfo;

                    process.Start();
                }

                Application.Exit();

            }).Start();
        }

        private void btnvendor_Click(object sender, EventArgs e)
        {
            Form vndrz = new vendorz();
            vndrz.Show();

        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void advertpic1_Click(object sender, EventArgs e)
        {

        }

        private void btnsettings_Click(object sender, EventArgs e)
        {
            Form st = new Settings();
            st.Show();
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void ads_next_Click(object sender, EventArgs e)
        {

        }
    }


}
