using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TechSupportData;
//Doris created on 2021-05-04
namespace ProductMaintenance
{
    public partial class frmAddorModify : Form
    {
        // these public properties are set by the main form
        public Products Product { get; set; }// selected product on the main form
        public bool AddProduct { get; set; }// flag that distinguishes Add from Modify
        public frmAddorModify()
        {
            InitializeComponent();
        }

        
        private void frmAddorModify_Load(object sender, EventArgs e)
        {
            if (AddProduct) //Add product
            {
                this.Text = "Add Product";
                txtCode.ReadOnly = false;// allow entry of new product code
            }
            else //Modify product
            {
                this.Text = "Modify Product";
                txtCode.ReadOnly = true;  // can't change existing product code
                this.DisplayProduct();
            }
        }

        private void DisplayProduct()
        {           
            txtCode.Text = Product.ProductCode;
            txtName.Text = Product.Name;
            txtVersion.Text = Product.Version.ToString();
            txtReleaseDate.Text = Product.ReleaseDate.ToString();
        }
        // user clicks on OK button
        private void btnOK_Click(object sender, EventArgs e)
        {
            if (IsValidData())
            {
                if (AddProduct) // Add
                {
                    // initialize the Product property with new Products object
                    this.Product = new Products();
                }
                this.LoadProductData(); // we have an object (public property Product) with new data
                this.DialogResult = DialogResult.OK;
            }
        }

        
        private bool IsValidData() //validate the input use validator class
        {
            bool success = true;
            string errorMessage = "";

            errorMessage += Validator.IsPresent(txtCode.Text, "Product Code");
            errorMessage += Validator.IsPresent(txtName.Text, "Name");
            errorMessage += Validator.IsPresent(txtVersion.Text, "Version");
            errorMessage += Validator.IsDecimal(txtVersion.Text, "Version");
            errorMessage += Validator.IsPresent(txtReleaseDate.Text, "Release Date");
            errorMessage += Validator.IsDate(txtReleaseDate.Text); // check if the date time follows the right format
            if (errorMessage != "")
            {
                success = false;
                MessageBox.Show(errorMessage, "Entry Error");
            }
            return success;
        }
        private void LoadProductData()  //display the product 
        {
            Product.ProductCode = txtCode.Text;
            Product.Name = txtName.Text;
            Product.Version = Convert.ToDecimal(txtVersion.Text);
            Product.ReleaseDate = Convert.ToDateTime(txtReleaseDate.Text);
        }
        //user clicks on cancel button
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
