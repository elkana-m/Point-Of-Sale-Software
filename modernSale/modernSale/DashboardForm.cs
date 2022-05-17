using modernSale.Prototype;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace modernSale
{
    public partial class Form1 : Form
    {
        private Dashboard dashboard;
        private Product product;
        public Form1()
        {
            InitializeComponent();
            CustomizeDesign();

            // Default - 7 Days
            dtpStartDate.Value = DateTime.Today.AddDays(-7);
            dtpEndDate.Value = DateTime.Now;
            btn7DaysAgo.Select();

            dashboard = new Dashboard();
            LoadData();
        }

        private void LoadData()
        {
            var refreshData = dashboard.LoadData(dtpStartDate.Value, dtpEndDate.Value);
            if (!refreshData)
            {
                Console.WriteLine("Failed loading view data models");
            }
            else
            {
                lblNumOrders.Text = dashboard.NumOrders.ToString();
                lblTotRevenue.Text = "$" + dashboard.TotalRevenue.ToString();
                lblTotProfit.Text = "$" + dashboard.TotalProfit.ToString();

                lblNumCust.Text = dashboard.NumCustomers.ToString();
                /*lblNumEmp.Text = dashboard.NumEmployees.ToString();*/
                lblNumOrders.Text = dashboard.NumOrders.ToString();
                lblProd.Text = dashboard.NumProducts.ToString();

                chartPerformance.DataSource = dashboard.TopProductsList;
                chartPerformance.Series[0].XValueMember = "Date";
                chartPerformance.Series[0].YValueMembers = "TotalAmount";
                chartPerformance.DataBind();

                chartGRevenue.DataSource = dashboard.GrossRevenueList;
                chartGRevenue.Series[0].XValueMember = "Key";
                chartGRevenue.Series[0].YValueMembers = "Value";
                chartGRevenue.DataBind();
            }
        }

        // Method to Hide submenu panels --> return void
        private void CustomizeDesign()
        {
            // changing visible properties to false
            //btnDashboard
            pnlDrpDwnProduct.Visible = false ;
            pnlDrpDwnInventory.Visible = false ;
           // pnlSideMenu.Visible = false ;                // should be activated once the hamburger button is set or auto-hover is set and off
        }

        private void hideSubMenu()
        {
            if (pnlDrpDwnInventory.Visible == true)
            {
                pnlDrpDwnInventory.Visible = false ;
            }
            if (pnlDrpDwnProduct.Visible == true)
            {
                pnlDrpDwnProduct.Visible = false;
            }
           
        }

        private void showSubMenu(Panel subMenu)
        {
            if (subMenu.Visible == false)
            {
                hideSubMenu() ;
                subMenu.Visible = true;
            }
            else
                subMenu.Visible = false ;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btn1_Click(object sender, EventArgs e)
        {

        }

        private void btnUsers_Click(object sender, EventArgs e)
        {
            hideSubMenu();
        }
        // method represents the btnDashboard_Click
        private void btnDashboard_Click(object sender, EventArgs e)
        {
            // action to be done when Dashboard is clicked
            hideSubMenu();
        }

        private void panelLogo_Paint(object sender, PaintEventArgs e)
        {
            hideSubMenu();
        }

        private void btn2_Click(object sender, EventArgs e)
        {

        }

        private void btnProducts_Click(object sender, EventArgs e)
        {
            showSubMenu(pnlDrpDwnProduct) ;
            
        }

        private void btnSold_Click(object sender, EventArgs e)
        {
            // ...
            // code for this button's stuff need an action to show form/box or send to web 
            // ...
            hideSubMenu() ;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            // ...
            // code for this button's stuff need an action to show form/box or send to web 
            // ...
            hideSubMenu();
        }

        private void btnAttempted_Click(object sender, EventArgs e)
        {
            // ...
            // code for this button's stuff need an action to show form/box or send to web 
            // ...
            hideSubMenu();
        }

        private void btnInventory_Click(object sender, EventArgs e) 
        {
            showSubMenu(pnlDrpDwnInventory);
        }

        private void btnCustomers_Click(object sender, EventArgs e)
        {

        }

        private void btnSuppliers_Click(object sender, EventArgs e)
        {

        }

        private void btnStock_Click(object sender, EventArgs e)
        {

        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            // ...
            // code for this button's stuff need an action to show form/box or send to web 
            // ...
            hideSubMenu();          // hide submenu when Help btn is clicked
        }

        private void lbl_profileName_Click(object sender, EventArgs e)
        {

        }

        private void lbl_profileTitle_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void lbl_Dashboard_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click (object sender, EventArgs e)
        {

        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnToday_Click(object sender, EventArgs e)
        {
            dtpStartDate.Value = DateTime.Today;
            dtpEndDate.Value = DateTime.Now;
            LoadData();
        }
        private void btn7DaysAgo_Click(object sender, EventArgs e)
        {
            dtpStartDate.Value = DateTime.Today.AddDays(-7);
            dtpEndDate.Value = DateTime.Now;
            LoadData();
        }

        private void btnCurrentMonth_Click(object sender, EventArgs e)
        {
            dtpStartDate.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            dtpEndDate.Value = DateTime.Now;
            LoadData();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            LoadData();
        }
    }
}
