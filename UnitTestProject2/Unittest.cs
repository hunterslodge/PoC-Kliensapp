using Hotcakes.CommerceDTO.v1.Catalog;
using Hotcakes.CommerceDTO.v1;
using Moq;
using PoC_Kliensapp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Drawing;
using System.Security.Cryptography;
using System.Windows.Forms;
using Hotcakes.CommerceDTO.v1.Client;
using Hotcakes.CommerceDTO.v1.Contacts;
using Hotcakes.Web.Data;
using System.Data;

namespace UnitTestProject2
{
    public class Unittest
    {
        [Test]
        public void TestDeleteProduct()
        {
            // Arrange
            var form = new Form1();
            form.productDataGridView = new DataGridView();
            var expectedRowCount = form.productDataGridView.Rows.Count - 1;
            // add a row to DataGridView to delete
            form.productDataGridView.Rows.Add("123", "ABC", "Product1", "10 Ft", DateTime.Now);

            // Act
            var scrollPosition = form.productDataGridView.FirstDisplayedScrollingRowIndex;
            form.Törlés_fv();
            var actualRowCount = form.productDataGridView.Rows.Count;
            var lastRowIndex = actualRowCount - 1;
            var actualProductId = lastRowIndex >= 0 ? form.productDataGridView.Rows[lastRowIndex].Cells[0].Value : null;

            // Assert
            Assert.AreEqual(expectedRowCount, actualRowCount);
            Assert.IsNull(actualProductId);
        }
    }
}
