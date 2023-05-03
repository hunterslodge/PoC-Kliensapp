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
using Hotcakes.Web.Data;


namespace PoC_Kliensapp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            GetProducts();
            foreach (DataGridViewColumn column in productDataGridView.Columns)
            {
                column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
        }

        public void GetProducts()
        {
            // get the current scrollbar position
            int scrollPosition = productDataGridView.FirstDisplayedScrollingRowIndex;

            // your existing code to fetch and populate the DataGridView
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
            dt.Columns.Add("SitePrice", typeof(string));
            dt.Columns.Add("CreationDateUtc", typeof(DateTime));


            productDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            productDataGridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders;




            foreach (ProductDTO item in deserializedResponse.Content)
            {
                dt.Rows.Add(item.Bvin, item.Sku, item.ProductName, item.SitePrice.ToString("F0") + " Ft", item.CreationDateUtc);
            }
           

            // save the selected row index and row count
            int selectedRowIndex = productDataGridView.CurrentRow != null ? productDataGridView.CurrentRow.Index : 0;
            int rowCount = productDataGridView.Rows.Count;

            // set the DataSource and restore the scrollbar position
            productDataGridView.DataSource = dt;
            
            

            if (scrollPosition != -1)
            {
                productDataGridView.FirstDisplayedScrollingRowIndex = scrollPosition;
            }

            // restore the selected row index if the DataGridView has rows
            if (rowCount > 0)
            {
                int indexToSelect = Math.Min(selectedRowIndex, productDataGridView.Rows.Count - 1);
                productDataGridView.CurrentCell = productDataGridView.Rows[indexToSelect].Cells[0];
            }
        }


        private void Törlés_Click(object sender, EventArgs e)
        {
            Törlés_fv();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Módosít();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Biztos", "Biztos", MessageBoxButtons.YesNo) == DialogResult.No)
            {
                e.Cancel = true;
            }
        }
        public void Törlés_fv()
        {
            if (MessageBox.Show("Biztos", "Biztos", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                // get the current scrollbar position
                int scrollPosition = productDataGridView.FirstDisplayedScrollingRowIndex;

                // your existing code to delete the selected row
                string url = "http://20.234.113.211:8095/";
                string key = "1-be27b88a-de65-48f3-9d66-fea7e3179d36";

                Api proxy = new Api(url, key);

                int rowIndex = productDataGridView.CurrentCell.RowIndex;
                int selectedRowIndex = productDataGridView.CurrentRow.Index;
                // get the productId from the first column of the selected row
                var productId = productDataGridView.Rows[rowIndex].Cells[0].Value.ToString();

                ApiResponse<bool> response = proxy.ProductsDelete(productId);

                // restore the scrollbar position and refresh the DataGridView
                GetProducts();

            }
        }

        public void Módosít()
        {
            string url = "http://20.234.113.211:8095/";
            string key = "1-be27b88a-de65-48f3-9d66-fea7e3179d36";

            Api proxy = new Api(url, key);

            int rowIndex = productDataGridView.CurrentCell.RowIndex;

            // get the ID of the selected product from the first column of the same row
            var productId = productDataGridView.Rows[rowIndex].Cells["Bvin"].Value.ToString();

            // call the API to find the product to update
            var product = proxy.ProductsFind(productId).Content;

            // validate the input
            if (!decimal.TryParse(textBox1.Text, out decimal value) || value == 0)
            {
                MessageBox.Show("Érvénytelen érték. Kérjük adjon meg egy nem 0 számot.");
                return;
            }


            // update the SitePrice property of the product
            product.SitePrice = decimal.Parse(textBox1.Text);

            // call the API to update the product
            ApiResponse<ProductDTO> response = proxy.ProductsUpdate(product);
            textBox1.Clear();
            GetProducts();
            // find the edited row by the product ID
            foreach (DataGridViewRow row in productDataGridView.Rows)
            {
                if (row.Cells["Bvin"].Value.ToString() == productId)
                {
                    // highlight the row
                    row.Selected = true;
                    break;
                }
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            Search();
        }
        private void Search()
        {
            string url = "http://20.234.113.211:8095/";
            string key = "1-be27b88a-de65-48f3-9d66-fea7e3179d36";

            Api proxy = new Api(url, key);

            ApiResponse<List<ProductDTO>> response = proxy.ProductsFindAll();
            List<ProductDTO> productList = response.Content;

            var query = from p in productList
                        where p.ProductName.ToLower().Contains(textBox2.Text.ToLower())
                        select new { p.Bvin, p.Sku, p.ProductName, SitePrice = p.SitePrice.ToString("F0") + " Ft", p.CreationDateUtc };

            productDataGridView.DataSource = query.ToList();
        }
    }
}
