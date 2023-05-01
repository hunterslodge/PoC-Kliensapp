using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestProject1
{
    public class teszt
    {
        [TestClass]
        public class Form1Tests
        {
            [TestMethod]
            public void TestGetProducts()
            {
                // Arrange
                DataTable expectedTable = new DataTable();
                expectedTable.Columns.Add("Bvin", typeof(string));
                expectedTable.Columns.Add("Sku", typeof(string));
                expectedTable.Columns.Add("ProductName", typeof(string));
                expectedTable.Columns.Add("SitePrice", typeof(string));
                expectedTable.Columns.Add("CreationDateUtc", typeof(DateTime));
                DataRow row1 = expectedTable.NewRow();
                row1["Bvin"] = "123";
                row1["Sku"] = "ABC";
                row1["ProductName"] = "Product1";
                row1["SitePrice"] = "10 Ft";
                row1["CreationDateUtc"] = DateTime.Now;
                expectedTable.Rows.Add(row1);
                ApiResponse<List<ProductDTO>> response = new ApiResponse<List<ProductDTO>>();
                List<ProductDTO> content = new List<ProductDTO>();
                ProductDTO product = new ProductDTO();
                product.Bvin = "123";
                product.Sku = "ABC";
                product.ProductName = "Product1";
                product.SitePrice = 10;
                product.CreationDateUtc = DateTime.Now;
                content.Add(product);
                response.Content = content;
                string json = JsonConvert.SerializeObject(response);
                string url = "http://20.234.113.211:8095/";
                string key = "1-be27b88a-de65-48f3-9d66-fea7e3179d36";
                Mock<Api> mockApi = new Mock<Api>(url, key);
                mockApi.Setup(m => m.ProductsFindAll()).Returns(response);
                Form1 form = new Form1();
                form.productDataGridView = new DataGridView();

                // Act
                form.GetProducts();
                DataTable actualTable = (DataTable)form.productDataGridView.DataSource;

                // Assert
                Assert.AreEqual(expectedTable.Rows.Count, actualTable.Rows.Count);
                Assert.AreEqual(expectedTable.Columns.Count, actualTable.Columns.Count);
                for (int i = 0; i < expectedTable.Rows.Count; i++)
                {
                    for (int j = 0; j < expectedTable.Columns.Count; j++)
                    {
                        Assert.AreEqual(expectedTable.Rows[i][j], actualTable.Rows[i][j]);
                    }
                }
            }
        }
    }
}
