using Hotcakes.CommerceDTO.v1.Orders;
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
using Hotcakes.CommerceDTO.v1;
using Hotcakes.CommerceDTO.v1.Client;
using Hotcakes.CommerceDTO.v1.Contacts;
using Hotcakes.CommerceDTO.v1.Catalog;
using Newtonsoft.Json;

namespace PoC_Kliensapp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public void GetProducts()
        {
            string url = "http://www.dnndev.me";
            string key = "1-04ef5d8f-9490-4c54-b45a-a449865431cf";

            Api proxy = new Api(url, key);


            ApiResponse<List<ProductDTO>> response = proxy.ProductsFindAll();
            string json = JsonConvert.SerializeObject(response);

            ApiResponse<List<ProductDTO>> deserializedResponse = JsonConvert.DeserializeObject<ApiResponse<List<ProductDTO>>>(json);

            DataTable dt = new DataTable();
            
            dt.Columns.Add("Bvin", typeof(string));
            dt.Columns.Add("Sku", typeof(string));
            dt.Columns.Add("ProductName", typeof(string));
            dt.Columns.Add("ListPrice", typeof(long));
            dt.Columns.Add("LongDescription", typeof(string));

            foreach (ProductDTO item in deserializedResponse.Content)
            {
                dt.Rows.Add(item.Bvin, item.Sku, item.ProductName, item.ListPrice,item.LongDescription );
            }

            productDataGridView.DataSource = dt;
        }
    }
}
