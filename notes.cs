using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Threading;

namespace Narriox
{
    public partial class notes : Form
    {
        public string scrip = load_main.scrip_location;
        public notes()
        {
            InitializeComponent();
        }

        private void btnadd_Click(object sender, EventArgs e)
        {
            Form cs = new newCourse();
            cs.Show();
        }

        private void notes_Load(object sender, EventArgs e)
        {
            try
            {
                new Thread(() => {
                    try
                    {
                        string connstring = string.Format(@"URI=file:{0}\\data\\courx.buff", scrip);
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

                            courses.Items.Add(name);
                        }
                        sql.Close();

                        if (this.courses.Items.Count > 0)
                            this.courses.SetSelected(0, true);

                        string newstr = string.Format(@"URI=file:{0}\\data\\crx\\{1}.dat", scrip, courses.SelectedItem.ToString());
                        SQLiteConnection crn = new SQLiteConnection(newstr);

                        crn.Open();

                        SQLiteCommand cmd = new SQLiteCommand(crn);
                        cmd.CommandText = "select * from topics";
                        SQLiteDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            string topic = (string)reader["topic"];
                            string notes = (string)reader["notes"];
                            topics.Items.Add(topic);
                            if (notz.Text.Length < 4)
                            {
                                notz.Text = notes;
                            }
                        }
                        crn.Close();
                    }
                    catch { }

                }).Start();
            }
            catch
            {
                
            }
            
           
        }

        private void courses_SelectedIndexChanged(object sender, EventArgs e)
        {

            new Thread(() => {
                try
                {
                    topics.Items.Clear();
                    string newstr = string.Format(@"URI=file:{0}\\data\\crx\\{1}.dat", scrip, courses.SelectedItem.ToString());
                    SQLiteConnection crn = new SQLiteConnection(newstr);

                    crn.Open();

                    SQLiteCommand cmd = new SQLiteCommand(crn);
                    cmd.CommandText = "select * from topics";
                    SQLiteDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        string topic = (string)reader["topic"];
                        string notes = (string)reader["notes"];
                        if (!topics.Items.Contains(topic))
                        {
                            topics.Items.Add(topic);
                        }

                        if (notz.Text.Length < 4)
                        {
                            notz.Text = notes;
                        }
                    }
                    crn.Close();
                }
                catch { }

            }).Start();

        }

        private void topics_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string newstr = string.Format(@"URI=file:{0}\\data\\crx\\{1}.dat", scrip, courses.SelectedItem.ToString());
                SQLiteConnection crn = new SQLiteConnection(newstr);

                crn.Open();
                notz.Text = "";

                SQLiteCommand cmd = new SQLiteCommand(crn);
                cmd.CommandText = string.Format("select * from topics where topic = '{0}'", topics.SelectedItem.ToString());
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string notes = (string)reader["notes"];
                    notz.Text = notes;
                }
            }
            catch
            {

            }
            
        }

        private void button4_Click(object sender, EventArgs e)
        {

        }
    }

    public class Course
    {
        public string course_name;
        public List<string> course_topics;
        public string topic_notes;
        public string scripl = load_main.scrip_location;

        public Course(string course)
        {
            this.course_name = course;
        }

        public void retTopics()
        {
            
            string connstring = string.Format(@"URI=file:{0}\\data\\courx.buff",scripl);
            SQLiteConnection sql = new SQLiteConnection(connstring);
            sql.Open();

            SQLiteCommand cmd = new SQLiteCommand("");
        }
    }
}
