using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MultimediaUpdater;

namespace Narriox
{
    public partial class newCourse : Form
    {
        public newCourse()
        {
            InitializeComponent();
        }

        private void txtsearch_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void btnreq_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Kindly submit your request on whatsapp : +263778111160");
        }

        private void newCourse_Load(object sender, EventArgs e)
        {
            new Thread(() => {
                try
                {
                    if (!File.Exists("data\\courx-totall.buff"))
                    {
                        File.Create("data\\courx-totall.buff");
                    }

                    File.Delete("data\\courx-totall.tmp");
                    File.Copy("data\\courx-totall.buff", "data\\courx-totall.tmp");
                    ;
                    WebClient ttl = new WebClient();
                    ttl.DownloadFile("http://erchatt.webs.com/courx-total.buff", "data\\courx-totall.buff");
                }
                catch
                {

                }

                new Thread(() => {
                    try
                    {
                        string connstring = string.Format(@"URI=file:{0}\\data\\courx-totall.buff", Narriox.load_main.scrip_location);
                        SQLiteConnection sql = new SQLiteConnection(connstring);
                        sql.Open();

                        SQLiteCommand cdm = new SQLiteCommand(sql);
                        cdm.CommandText = "select * from courses";
                        SQLiteDataReader dr = cdm.ExecuteReader();
                        while (dr.Read())
                        {
                            string name = (string)dr["course_name"];
                            string id = (string)dr["course_id"];
                            string description = (string)dr["description"];

                            coursesavail.Items.Add(name);
                        }
                        sql.Close();

                    }
                    catch
                    {

                    }
                }).Start();

            }).Start();

         
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try{
                string course_selected = coursesavail.SelectedItem.ToString();
                string connstring = string.Format(@"URI=file:{0}\\data\\courx.buff", load_main.scrip_location);
                SQLiteConnection sql = new SQLiteConnection(connstring);
                sql.Open();

                SQLiteCommand cdm = new SQLiteCommand(sql);
                cdm.CommandText = string.Format("insert into courses values('no-id', '{0}','no=desc')", course_selected);
                cdm.ExecuteNonQuery();
                MessageBox.Show("Course Added Successfully");
                sql.Close();
            } catch
            {
                MessageBox.Show("Error Adding Course");
            }
            
            
        }
    }
}
