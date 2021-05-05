using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TechSupportData;
//Doris created on 2021-05-04
namespace ProductMaintenance
{
    public partial class ProductMaintenanceGUI : Form
    {
        //reference to TechSupportData project added to the GUI project
        private TechSupportContext context = new TechSupportContext();
        private Products selectedProduct;//define the current selected product
        public ProductMaintenanceGUI()
        {
            InitializeComponent();
        }
        public void dgvProducts_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            string id = dgvProducts.CurrentRow.Cells[0].Value.ToString();
            selectedProduct = context.Products.Find(id); //find by pk value
        }
        private void ProductMaintenanceGUI_Load(object sender, EventArgs e)
        {
            
            using (TechSupportContext db = new TechSupportContext()) //create db context object
            {
                dgvProducts.ClearSelection();
                //retrieve products data
                var products = db.Products.OrderBy(p => p.ProductCode).Select//order alphabetically by product code
                               (p => new
                               {
                                   p.ProductCode,
                                   p.Name,
                                   p.Version,
                                   p.ReleaseDate
                               }).ToList();
                dgvProducts.DataSource = products;

                // change columns' header text
                dgvProducts.Columns[0].HeaderText = "Product Code";
                dgvProducts.Columns[3].HeaderText = "Release Date";

                // bold the column headers
                dgvProducts.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
                // change background colour on alternating rows
                dgvProducts.AlternatingRowsDefaultCellStyle.BackColor = Color.LightBlue;
                dgvProducts.ClearSelection();
            }
        }
        //user clicks on Add product
        private void btnAdd_Click(object sender, EventArgs e)
        {
            var addModifyProductForm = new frmAddorModify()
            {
                AddProduct = true   // initializer
            };
            DialogResult result = addModifyProductForm.ShowDialog(); //display model
            if (result == DialogResult.OK) // user clicked Accept on the second form
            {
                try
                {
                    selectedProduct = addModifyProductForm.Product;// record product from the second
                                                                   // form as the current product
                    context.Products.Add(selectedProduct);
                    context.SaveChanges();
                    DisplayProducts();
                }
                catch (DbUpdateException ex)
                {
                    HandleDatabaseError(ex);
                }
                catch (Exception ex)
                {
                    HandleGeneralError(ex);
                }
            }
        }

        private void DisplayProducts()
        {
            dgvProducts.Columns.Clear(); //clear old content
            var products = context.Products
                .OrderBy(p => p.ProductCode)
                .Select(p => new { p.ProductCode, p.Name, p.Version, p.ReleaseDate })
                .ToList();
            dgvProducts.DataSource = products;   
        }

        private void HandleDatabaseError(DbUpdateException ex)
        {
            string errorMessage = "";
            var sqlException = (SqlException)ex.InnerException;
            foreach (SqlError error in sqlException.Errors)
            {
                errorMessage += "ERROR CODE:  " + error.Number + " " +
                                error.Message + "\n";
            }
            MessageBox.Show(errorMessage);
        }
       
        //user clicks on the modify button to update existing record
        private void btnModify_Click(object sender, EventArgs e)
        {
            var addModifyProductForm = new frmAddorModify()
            { // object initializer
                AddProduct = false,
                Product = selectedProduct
            };
            DialogResult result = addModifyProductForm.ShowDialog();// display modal
            if (result == DialogResult.OK)// user clicked OK on the second form
            {
                try
                {
                    selectedProduct = addModifyProductForm.Product; // new data
                    context.SaveChanges();
                    DisplayProducts();
                }
                catch (DbUpdateException ex)
                {
                    HandleDatabaseError(ex);
                }
                catch (Exception ex)
                {
                    HandleGeneralError(ex);
                }
            }
            //ModifyProduct();
        }
        //private void ModifyProduct() //modify method
        //{
            
        //}
        //remove the selected record
        private void btnRemove_Click(object sender, EventArgs e)
        {
            DialogResult result =
                 MessageBox.Show($"Delete {selectedProduct.ProductCode.Trim()}?",
                 "Confirm Delete", MessageBoxButtons.YesNo,
                 MessageBoxIcon.Question);                //ask the user to confirm the delete action
            if (result == DialogResult.Yes)// user confirmed
            {
                try
                {
                    context.Products.Remove(selectedProduct);
                    context.SaveChanges(true);
                    DisplayProducts();
                }
                catch (DbUpdateException ex)
                {
                    HandleDatabaseError(ex);
                }
                catch (Exception ex)
                {
                    HandleGeneralError(ex);
                }
            }
        }

        private void HandleGeneralError(Exception ex)
        {
            MessageBox.Show(ex.Message, ex.GetType().ToString());
        }

        //exit the system
        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
       
       
        
    }
}
