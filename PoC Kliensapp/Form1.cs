using Hotcakes.CommerceDTO.v1;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Hotcakes.CommerceDTO.v1.Client;
using Hotcakes.CommerceDTO.v1.Contacts;
using Hotcakes.CommerceDTO.v1.Catalog;


namespace PoC_Kliensapp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            GetProducts();  
        }

        public void GetProducts()
        {
            string url = "http://20.234.113.211:8095/";
            string key = "1-be27b88a-de65-48f3-9d66-fea7e3179d36";

            Api proxy = new Api(url, key);


            ApiResponse<List<ProductDTO>> response = proxy.ProductsFindAll();
            string json = JsonConvert.SerializeObject(response);

            ApiResponse<List<ProductDTO>> deserializedResponse = JsonConvert.DeserializeObject<ApiResponse<List<ProductDTO>>>(json);

            DataTable dt = new DataTable();
            
            dt.Columns.Add("Bvin", typeof(string));
            dt.Columns.Add("Sku", typeof(string));
            dt.Columns.Add("ProductName", typeof(string));
            dt.Columns.Add("SitePrice", typeof(long));
            dt.Columns.Add("LongDescription", typeof(string));

            foreach (ProductDTO item in deserializedResponse.Content)
            {
                dt.Rows.Add(item.Bvin, item.Sku, item.ProductName, item.SitePrice,item.LongDescription );
            }

            productDataGridView.DataSource = dt;
        }


        private void Törlés_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Biztos", "Biztos", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                string url = "http://20.234.113.211:8095/;
                string key = "1-be27b88a-de65-48f3-9d66-fea7e3179d36";

                Api proxy = new Api(url, key);


                int rowIndex = productDataGridView.CurrentCell.RowIndex;
                int columnIndex = productDataGridView.CurrentCell.ColumnIndex;

                var productId = productDataGridView[columnIndex, rowIndex].Value.ToString();


                ApiResponse<bool> response = proxy.ProductsDelete(productId);

                GetProducts();
            }
                
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string url = "http://20.234.113.211:8095/";
            string key = "1-be27b88a-de65-48f3-9d66-fea7e3179d36";

            Api proxy = new Api(url, key);

            int rowIndex = productDataGridView.CurrentCell.RowIndex;
            int columnIndex = productDataGridView.CurrentCell.ColumnIndex;
            // specify the product to look for
            var productId = productDataGridView[columnIndex, rowIndex].Value.ToString();

            // call the API to find the product to update
            var product = proxy.ProductsFind(productId).Content;

            // update one or more properties of the product
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                return;
            }
            else
            {
                product.SitePrice = decimal.Parse(textBox1.Text);
            }
            

            // call the API to update the product
            ApiResponse<ProductDTO> response = proxy.ProductsUpdate(product);

            GetProducts();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Biztos", "Biztos", MessageBoxButtons.YesNo) == DialogResult.No)
            {
                e.Cancel = true;
            }
        }
    }
}
