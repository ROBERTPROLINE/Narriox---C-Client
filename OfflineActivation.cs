using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Narriox
{
    public partial class OfflineActivation : Form
    {
        string retkey;
        static string current_location = System.Reflection.Assembly.GetExecutingAssembly().Location;
        static string current_directory = Directory.GetParent(current_location).FullName;
        static string scrip_location = current_directory + Path.DirectorySeparatorChar + "data";

        public OfflineActivation()
        {
            InitializeComponent();
        }

        private void OfflineActivation_Load(object sender, EventArgs e)
        {
            //
        }

        private void button3_Click(object sender, EventArgs e)
        {
            MessageBox.Show("To complete payment send your \napproval code, amount  and username to \nwhatsapp number : +263774611839, +263786772147 or +263786772147");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("To create account please send your name and preferred username \nto our whatsapp number +263778111160");
        }

        private void activatesubs_Click(object sender, EventArgs e)
        {

            //activate with activation  key

                try
                {
                    if (actkey.Text == retkey)
                    {
                        string cs = string.Format(@"URI=file:{0}\\cred98.db", scrip_location);
                        SQLiteConnection conn = new SQLiteConnection(cs);
                        conn.Open();
                        SQLiteCommand cmd = new SQLiteCommand(conn);
                        string pkg = "";

                        if (package.Text == "gold")
                        {
                            pkg = "2";

                        }
                        else if (package.Text == "basic")
                        {
                            pkg = "1";
                        }
                        else if (package.Text == "platinum")
                        {
                            pkg = "3";
                        }
              
                        string d  = string.Format("update credentials set package = '{0}', expiry = '{1}'", pkg, DateTime.Now.AddDays(30).ToLongDateString());
                        cmd.CommandText = d;
                       
                        cmd.ExecuteNonQuery();
                        conn.Close();
                        MessageBox.Show("Offline Activation succesffull");
                    }
                    else
                    {
                        MessageBox.Show(retkey);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
               
       
           
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            

            if ((uid.Text.Length>4) && (pwd.Text.Length>4))
            {
                
                double keycode;
                keycode = new Random().NextDouble();
                string keycode1 = string.Format("{0}", keycode/243);

                string ct = string.Format("{0}",keycode1);

                string[] tk = ct.Split('.');

                string subs = tk[1].Substring(0, 9);
                retkey = string.Format("{0}",double.Parse(subs) * package.Text.Length - 1 + 123);

                twinkey.Text = subs;
            }
        }
    }
}
