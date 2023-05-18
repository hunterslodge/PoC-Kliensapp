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
using Hotcakes.CommerceDTO.v1.Orders;
using Hotcakes.Web.Data;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;


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
                bool productFound = false;

                /*
                
                ApiResponse<List<OrderSnapshotDTO>> response2 = proxy.OrdersFindAll();
                List<string> bvins = response2.Content.Select(snapshot => snapshot.bvin).ToList();
                */
                foreach (OrderDTO orderDTO in allOrders)
                {
                    if (orderDTO.Items.Any(i => i.ProductId == productId) && orderDTO.StatusName != "Complete")
                    {
                        MessageBox.Show("A kért termék nem törölhető, mert már benne van egy rendelésben");
                        productFound = true;
                        break;
                    }
                }
                if (!productFound)
                {
                    ApiResponse<bool> response = proxy.ProductsDelete(productId);
                    MessageBox.Show("A kért termék törölhető");
                }

                // restore the scrollbar position and refresh the DataGridView
                GetProducts();




                /*
                ApiResponse<OrderDTO> orderResponse = proxy.OrdersFind("");
                string json = JsonConvert.SerializeObject(orderResponse);

                ApiResponse<OrderDTO> deserializedResponse = JsonConvert.DeserializeObject<ApiResponse<OrderDTO>>(json);
                OrderDTO orderDTO = deserializedResponse.Content;

                if (orderDTO != null)
                {
                    bool productFound = false; // Flag to track if the product is found in the order

                    foreach (LineItemDTO lineItem in orderDTO.Items)
                    {
                        if (lineItem.ProductId == productId)
                        {
                            string orderBvin = orderDTO.Bvin;
                            // Use the orderBvin for further processing
                            // ...
                            productFound = true;
                            break;
                        }
                    }

                    if (productFound)
                    {
                        MessageBox.Show("A kért termék nem törölhető, mert már benne van egy rendelésben");
                    }
                    else
                    {
                        ApiResponse<bool> response = proxy.ProductsDelete(productId);
                        MessageBox.Show("A kért termék törölhető");
                    }
                }
                */
                /*
                ApiResponse<bool> response = proxy.ProductsDelete(productId);
                // restore the scrollbar position and refresh the DataGridView
                GetProducts();
                */






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

        private async void textBox2_TextChanged(object sender, EventArgs e)
        {
            await SearchAsync();
        }

        private async Task SearchAsync()
        {
            string url = "http://20.234.113.211:8095/";
            string key = "1-be27b88a-de65-48f3-9d66-fea7e3179d36";

            Api proxy = new Api(url, key);

            List<ProductDTO> productList = await Task.Run(() =>
            {
                ApiResponse<List<ProductDTO>> response = proxy.ProductsFindAll();
                return response.Content;
            });

            var query = from p in productList
                        where p.ProductName.ToLower().Contains(textBox2.Text.ToLower())
                        select new { p.Bvin, p.Sku, p.ProductName, SitePrice = p.SitePrice.ToString("F0") + " Ft", p.CreationDateUtc };

            productDataGridView.DataSource = query.ToList();
        }
        //aszinkronos futtatás
        private Task<List<OrderDTO>> FeltöltAsync(Api proxy, IProgress<int> progress)
        {
            DateTime startDate = new DateTime(2023, 3, 2);

            return Task.Run(() =>
            {
                ApiResponse<List<OrderSnapshotDTO>> response2 = proxy.OrdersFindAll();

                List<string> bvins = response2.Content
                    .Where(snapshot => snapshot.TimeOfOrderUtc > startDate && snapshot.StatusName != "")
                    .Select(snapshot => snapshot.bvin)
                    .ToList();

                List<OrderDTO> allOrders = new List<OrderDTO>();

                for (int i = 0; i < bvins.Count; i++)
                {
                    ApiResponse<OrderDTO> orderResponse = proxy.OrdersFind(bvins[i]);
                    OrderDTO orderDTO = orderResponse.Content;
                    allOrders.Add(orderDTO);

                    // Report progress
                    int progressPercentage = (i + 1) * 100 / bvins.Count;
                    progress.Report(progressPercentage);
                }

                return allOrders;
            });
        }

        private List<OrderDTO> allOrders;

        private async void Form1_Load_1(object sender, EventArgs e)
        {
            string url = "http://20.234.113.211:8095/";
            string key = "1-be27b88a-de65-48f3-9d66-fea7e3179d36";

            Api proxy = new Api(url, key);

            // Update the label text to indicate loading
            loadingLabel.Text = "Rendelések betöltése...";
            progressBar.Style = ProgressBarStyle.Continuous;
            progressBar.Value = 0;
            progressBar.Visible = true;
            Törlés.Enabled= false;

            // Use a Progress object to report progress updates
            
            var progress = new Progress<int>(value =>
            {
                progressBar.Value = value;
            });

            // Load the orders asynchronously
            allOrders = await FeltöltAsync(proxy, progress);

            // Update the label text to indicate completion
            loadingLabel.Text = "Rendelések sikeresen betöltve!";
            progressBar.Visible = false;
            Törlés.Enabled = true;
        }


    }
}
