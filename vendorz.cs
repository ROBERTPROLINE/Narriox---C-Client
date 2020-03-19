using System;
using System.Data.SQLite;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace Narriox
{
    public partial class vendorz : Form
    {
        static string current_location = System.Reflection.Assembly.GetExecutingAssembly().Location;
        static string current_directory = Directory.GetParent(current_location).FullName;
        static string scrip_location = current_directory + Path.DirectorySeparatorChar ;

        string product, product_id, product_price, product_delivery, product_vendor, order_status, sell_status, product_desc;

        private void btnorder_Click(object sender, EventArgs e)
        {
            if (btnorder.Text == "Place Order")
            {
                //send order to server and save to local db
            }
            else
            {
                //delete order from server and localdb

            }
        }

        private void product_qty_SelectedIndexChanged(object sender, EventArgs e)
        {
            double totalprice = double.Parse(product_price) * double.Parse(product_qty.Text);
            amt.Text = string.Format("${0}",totalprice);
        }

        private void searchbox_TextChanged(object sender, EventArgs e)
        {
            string searchtext = searchbox.Text;
            
            if (searchtext!="" && productslist.Items.Count>7)
            {
                foreach (string cd in productslist.Items)
                {
                    if (!cd.Contains(searchtext))
                    {
                        productslist.Items.Remove(value: cd);
                    }
                    
                }

            }
            
        }

        public vendorz()
        {
            InitializeComponent();
        }

        private void btnsell_Click(object sender, EventArgs e)
        {
            Form be_a_vendor = new register_vendor();
            be_a_vendor.Show();
        }

        private void vendorz_Load(object sender, EventArgs e)
        {

            new Thread(()=> {
                //load vendorlist

                string connstr = string.Format(@"URI = file:{0}\\lat-co\\vendorz.buff", scrip_location);
                SQLiteConnection con = new SQLiteConnection(connstr);
                con.Open();
                SQLiteCommand cmd_ = new SQLiteCommand(con);

                cmd_.CommandText = "select * from vendors";
                SQLiteDataReader dr = cmd_.ExecuteReader();
                while (dr.Read())
                {
                    string vendor_name = (string)dr["vendor_name"];
                    vendor_list.Items.Add(vendor_name);
                }


            }).Start();


            string conncode = string.Format(@"URI = file:{0}\\lat-co\\products.db", scrip_location);
            SQLiteConnection p = new SQLiteConnection(conncode);
            p.Open();
            SQLiteCommand cmd = new SQLiteCommand(p);

            string ctext = ("select * from products where sell_status = 'unsold'");
            cmd.CommandText = ctext;

            SQLiteDataReader prdctreader = cmd.ExecuteReader();
            while (prdctreader.Read())
            {
                product = (string)prdctreader["name"];
                product_id = (string)prdctreader["product_id"];
                product_price = (string)prdctreader["price"];
                product_vendor = (string)prdctreader["vendor"];
                order_status = (string)prdctreader["status"];
                product_delivery = (string)prdctreader["delivery"];
                productslist.Items.Add(product);

            }
            p.Close();
        }

        private void vendor_list_SelectedIndexChanged(object sender, EventArgs e)
        {
            productslist.Items.Clear();

            string vendor = vendor_list.Text;


            string conncode = string.Format(@"URI = file:{0}\\lat-co\\products.db", scrip_location);
            SQLiteConnection p = new SQLiteConnection(conncode);
            p.Open();
            SQLiteCommand cmd = new SQLiteCommand(p);

            string ctext = string.Format("select * from products where vendor = '{0}'",vendor);
            cmd.CommandText = ctext;

            SQLiteDataReader prdctreader = cmd.ExecuteReader();
            while (prdctreader.Read())
            {
                product = (string)prdctreader["name"];
                product_id = (string)prdctreader["product_id"];
                product_price = (string)prdctreader["price"];
                product_vendor = (string)prdctreader["vendor"];
                order_status = (string)prdctreader["status"];
                sell_status = (string)prdctreader["sell_status"];
                product_delivery = (string)prdctreader["delivery"];
                product_desc = (string)prdctreader["product_desc"];
                productslist.Items.Add(product);
            }
            p.Close();
        }

        private void productslist_SelectedIndexChanged(object sender, EventArgs e)
        {
            string status = order_status;

            if (order_status == "ordered")
            {
                btnorder.Text = "Remove Order";
            }
            else
            {
                btnorder.Text = "Place Order";
            }

            objprice.Text = product_price;
            objvendor.Text = product_vendor;
            product_qty.Text = "0";
            delivery.Text = product_delivery;
            description.Text = product_desc;
            currentobj_pic.ImageLocation = string.Format("lat-co\\products_data\\{0}.png",product_id);
            currentobj_pic.Show();
        }
    }
}
