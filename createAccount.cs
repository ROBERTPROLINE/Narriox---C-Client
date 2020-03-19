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
using IronPython.Hosting;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Media;
using System.Data.SQLite;

namespace Narriox
{
    public partial class createAccount : Form
    {
        static string current_location = System.Reflection.Assembly.GetExecutingAssembly().Location;
        static string current_directory = Directory.GetParent(current_location).FullName;
        static string scrip_location = current_directory + Path.DirectorySeparatorChar + "data";

        public createAccount()
        {
            InitializeComponent();
        }

        private void createAccount_Load(object sender, EventArgs e)
        {
           
            try
            {
                IPHostEntry iphost = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress ipaddr = IPAddress.Parse("127.0.0.1");

                int portNum = 987;
                IPEndPoint remoteserver = new IPEndPoint(ipaddr, portNum);
                Socket mysoc = new Socket(ipaddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                mysoc.Connect(remoteserver);

                if (File.Exists("kk"))
                {
                    pnlcompletepayment.Visible = false;
                    create_accountpanel.Visible = false;
                    subscribe_panel.Visible = true;
                    static_button_caform.Text = "Complete Subscription";


                }
                else
                {
                    subscribe_panel.Visible = false;
                    pnlcompletepayment.Visible = false;
                    static_button_caform.Text = "Register Account";
                    create_accountpanel.Visible = true;

                }

            }
            catch
            {
                MessageBox.Show("Server Connection failed \nTry our offline activation");
                Form nf = new OfflineActivation();
                nf.ShowDialog();

                //Application.Restart();
            }


        }

        private void subscribe_btn_Click(object sender, EventArgs e)
        {
            subscribe_panel.Visible = true;
            pnlcompletepayment.Visible = false;
            static_button_caform.Text = "Complete Subscription";
            Form.ActiveForm.Text = "Suscribe ";
        }

        private void createaccount_btn_Click(object sender, EventArgs e)
        {

            subscribe_panel.Visible = false;
            pnlcompletepayment.Visible = false;
            static_button_caform.Text = "Register Account";
            create_accountpanel.Visible = true;
            Form.ActiveForm.Text = "Create Free Account";
        }

       

        private void label14_Click(object sender, EventArgs e)
        {

        }

        private void pay_complete_Click(object sender, EventArgs e)
        {
            subscribe_panel.Visible = false;
            create_accountpanel.Visible = false;
            pnlcompletepayment.Visible = true;
            static_button_caform.Text = "Complete Payment";
            Form.ActiveForm.Text = "Complete Payment made by ecocash";

        }

        private void deleteAccount_btn_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Request to delet aCCount has been sent \nRequest id : del0188798f98\nThank you");
        }

        private void static_button_caform_Click(object sender, EventArgs e)
        {
            new Thread(()=> {
                try
                {
                    int portNum = 0;


                    IPHostEntry iphost = Dns.GetHostEntry(Dns.GetHostName());
                    IPAddress ipaddr = IPAddress.Parse("127.0.0.1");


                    if (static_button_caform.Text == "Register Account")
                    {
                        //File.Delete("kk");
                        string name = fstname.Text;
                        string lname = lstname.Text;
                        string email = caemail.Text;
                        string pwd = verpassword.Text;
                        string phon = caphone.Text;
                        string uid = cauid.Text;


                        portNum = 912;
                        IPEndPoint remoteserver = new IPEndPoint(ipaddr, portNum);
                        Socket mysoc = new Socket(ipaddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                        mysoc.Connect(remoteserver);
                        string exp = string.Format("{0}",DateTime.Now.AddDays(2));
                        byte[] bytes2send = Encoding.ASCII.GetBytes(string.Format("{0},{1},{2},{3},{4},{5},{6}", uid, pwd, name, lname, email, phon,exp));
                        int msgsent = mysoc.Send(bytes2send);

                        //show progress for sending
                        //welcome n1 = new welcome();

                        byte[] byt2recv = new byte[512];
                        int bytrecv = mysoc.Receive(byt2recv);
                        string conf = Encoding.ASCII.GetString(bytes: byt2recv);
                        MessageBox.Show(conf);
                        if (conf.Contains("create-account-success"))
                        {
                           

                            string credentials = string.Format("insert into credentials values('{0}','{1}','pic','{2}', 'trial' )", cauid.Text, verpassword.Text, fstname.Text + " " + lstname.Text);
                            string cs = string.Format(@"URI=file:{0}\\cred98.db", scrip_location);
                            SQLiteConnection sq = new SQLiteConnection(cs);
                            sq.Open();
                            SQLiteCommand cmd = new SQLiteCommand(sq);
                            cmd.CommandText = credentials;
                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Account Created Successfully!");
                            try
                            {
                                File.Create("kk");
                            }
                            catch 
                            {

                            }
                            Application.Restart();

                        }

                        else
                        {
                            MessageBox.Show(conf);
                        }

                    }
                    else if (static_button_caform.Text == "Complete Subscription")
                    {
                        //new Thread(()=>subs()).Start();

                        string username = txtusername.Text;
                        string package = combopackages.Text;
                        string password = txtpassword.Text;

                        string subsreq = string.Format("{0}::{1}::{2}",username,password,package);
;                       //MessageBox.Show(subsreq);

                        portNum = 9812;
                        IPEndPoint subserver = new IPEndPoint(ipaddr, portNum);
                        Socket myconn = new Socket(ipaddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                        myconn.Connect(subserver);
                        MessageBox.Show("Please be Patient While Processing");
                        if (myconn.Connected)
                        {
                            
                            byte[] server_approv = new byte[1024];
                            int server_approval = myconn.Receive(server_approv);
                            string msgapprov = Encoding.ASCII.GetString(server_approv);

                            //MessageBox.Show(msgapprov);

                            if (msgapprov.Contains("subs-approv"))
                            {
                                byte[] msg2send = Encoding.ASCII.GetBytes(subsreq);
                                int msgSendt = myconn.Send(msg2send);

                                byte[] serverappr = new byte[1024];
                                int msgappr = myconn.Receive(serverappr);
                                string confm = Encoding.ASCII.GetString(serverappr);
                                if (confm.Contains("msg-notify -> Subscription Complete "))
                                {
                                    try
                                    {
                                        
                                        Thread.Sleep(2);
                                        string cs = string.Format(@"URI=file:{0}\\cred98.db", scrip_location);
                                        SQLiteConnection conn = new SQLiteConnection(cs);
                                        conn.Open();
                                        SQLiteCommand cmd = new SQLiteCommand(conn);
                                        string pkg = "";

                                        if (package == "gold")
                                        {
                                            pkg = "2";

                                        }
                                        else if (package =="basic")
                                        {
                                            pkg = "1";
                                        }
                                        else if(package == "platinum")
                                        {
                                            pkg = "3";
                                        }
                                        MessageBox.Show("Subscription Complete !\nRestart the App");
                                        cmd.CommandText = string.Format("update credentials set package = '{0}'",pkg);
                                        cmd.ExecuteNonQuery();
                                        conn.Close();

                                        Application.Restart();
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show(ex.Message);
                                    }
                                    
                                }
                                else
                                {
                                    MessageBox.Show(confm);
                                }
                            }
                            else
                            {
                                
                            }
                        }
                       
                    }
                    else if (static_button_caform.Text == "Complete Payment")
                    {
                        string username = econame.Text;
                        string phone = ecophone.Text;
                        string approvalcode = ecoapprov.Text;
                        //string amt = ecoamt.Text;

                        portNum = 9009;
                        IPEndPoint payment_server = new IPEndPoint(ipaddr, portNum);
                        Socket myconn = new Socket(ipaddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                        myconn.Connect(payment_server);

                        if (myconn.Connected)
                        {
                                string payment_request = string.Format("{0}::default::{1}::{2}",username,approvalcode,phone);

                                byte[] msg2send = Encoding.ASCII.GetBytes(payment_request);
                                int msgSendt = myconn.Send(msg2send);

                                byte[] serverappr = new byte[1024];
                                int msgappr = myconn.Receive(serverappr);
                                string confm = Encoding.ASCII.GetString(serverappr);
                                if (confm.Contains("Payment Done !!"))
                                {
                                    try
                                    {
                                        MessageBox.Show("Payment Sucessfull !");
                                        Application.Restart();
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show(ex.Message);
                                    }

                                }
                                else
                                {
                                    MessageBox.Show(confm);
                                }                            
                        }
                    }

                }
                catch (Exception)
                {
                    MessageBox.Show("Action Failed !!\nCheck network connections");

                }
            }).Start();
         
           
        }

        private void combopackages_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                cal();
            }
            catch
            {

            }
        }

        private void textBox10_TextChanged(object sender, EventArgs e)
        {
            try
            {
                cal();
            }catch
            {

            }
        }

        public void subs()
        {

            string username;
            string password;

            string cs = string.Format(@"URI=file:{0}\\cred98.db", scrip_location);
            SQLiteConnection sql = new SQLiteConnection(cs);
            sql.Open();
            SQLiteCommand cmd = new SQLiteCommand(sql);
            cmd.CommandText = "select * from credentials where '__rowid__' = '1'";
            SQLiteDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                username = (string)dr["name"];
                password = (string)dr["name"];

                txtusername.Text = username;
                txtpassword.Text = password;
            }
            sql.Close();
        }

        public  void cal()
        {
            txtmonths.Text = "";
            txtmonths.Enabled = false;
            int package_amt = 0;
            int months_ = 1;

            if (combopackages.Text == "basic")
            {
                package_amt = 3;
            }
            else if (combopackages.Text == "gold")
            {
                package_amt = 5;
            }
            else if (combopackages.Text == "platinum")
            {
                package_amt = 8;
            }

            double total = package_amt * months_;
            lblpackage.Text = combopackages.Text;
            lblexpiry.Text = string.Format("Expiry Date : {0}", DateTime.Now.AddMonths(months_));
            lbltotalsum.Text = string.Format("total : {0}", (total));
        }
    }
}
