using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using IronPython.Compiler;
using IronPython.Hosting;
using IronPython.Runtime;
using IronPython.Modules;
using System.IO;
using System.Data.SQLite;
using System.Net;
using System.Net.Sockets;


namespace Narriox
{
    public partial class welcome : Form
    {
        static string msg1, lbl1;
        static string current_location = System.Reflection.Assembly.GetExecutingAssembly().Location;
        static string current_directory = Directory.GetParent(current_location).FullName;
        static string scrip_location = current_directory + Path.DirectorySeparatorChar + "data";

        public welcome(string msg11, string lbl11)
        {
            

            msg1 = msg11;
            lbl1 = lbl11;

            
            InitializeComponent();
        }



        private void button1_Click(object sender, EventArgs e)
        {
                string signature = System.Environment.UserDomainName.ToString();
                string pcuid = System.Environment.UserName.ToString(); ;
                string cs = string.Format(@"URI=file:{0}\\cred98", scrip_location);
                SQLiteConnection conn1 = new SQLiteConnection(cs);
                conn1.Open();

                SQLiteCommand cmd = new SQLiteCommand(conn1);
                cmd.CommandText = string.Format("insert into credentials values('{0}', '{1}', 'trial', '{2}', '{3}','{4}')", 
                    txtusername.Text, txtpwd.Text,DateTime.Now.AddDays(1).ToLongDateString(),signature,pcuid);

                cmd.ExecuteNonQuery();
                conn1.Close();
                MessageBox.Show("Account successully created !\nPlease subscribe to use our services!!");
                Application.Restart();

          
            
        }

        

        private static void updateCredentials(string username, string password)
        {
           
        }

       
        private void logo_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void main_Load(object sender, EventArgs e)
        {
         
                string cs = string.Format(@"URI=file:{0}\\cred98", scrip_location);
                SQLiteConnection conn1 = new SQLiteConnection(cs);
                conn1.Open();

                SQLiteCommand cmd = new SQLiteCommand(conn1);
                cmd.CommandText = "create table credentials(name text, pwd text, package text, expiry text, signature text, pcuid text)";
                cmd.ExecuteNonQuery();
                conn1.Close();
           
         
            

            loginpanel.Visible = false;

            //lblmain.Text = lbl1;
            //msgmain.Text = msg1;
            
            progressBar1.Value = 0;
            try
            {
                for (int i = 0; i < 75; i++)
                {
                    progressBar1.Value += i;
                    Thread.Sleep(5);
                    if (progressBar1.Value > 80)
                    {
                        msgmain.Text = "Checking Files ...";
                    }

                    if (progressBar1.Value > 79)
                    {
                        loginpanel.Visible = true;
                        msgmain.Text = "Thanks for choosing us ... !";
                        progressBar1.Value = 100;
                    }
                }

              
               
            }
            catch 
            {
                //MessageBox.Show(ex.Message);
            }           
        }
    }                       
}

