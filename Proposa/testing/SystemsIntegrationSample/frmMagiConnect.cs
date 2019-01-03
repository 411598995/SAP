using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using System.Text;
using System.IO;

namespace SystemsIntegrationSample
{
    public partial class frmMagiConnectSample : Form
    {
        public frmMagiConnectSample()
        {
            InitializeComponent();
        }

        public string GetMagiConnectLogin()
        {
            return System.Configuration.ConfigurationManager.AppSettings["MagiConnectLogin"].ToString();
        }

        public string GetMagiConnectPassword()
        {
            return System.Configuration.ConfigurationManager.AppSettings["MagiConnectPassword"].ToString();
        }

        private void btnExportOrders_Click(object sender, EventArgs e)
        {
            MagiConnect_Sales.sales SalesWebService = new SystemsIntegrationSample.MagiConnect_Sales.sales();

            // This exports into XML all orders for the last 2 weeks
            string Login = GetMagiConnectLogin();
            string Password = GetMagiConnectPassword();
            Guid OrderStatus_NewOrder = new Guid(System.Configuration.ConfigurationManager.AppSettings["MagiConnectOrderStatus_NewOrder"]);

            string OrdersXML = SalesWebService.ExportOrders(Login, Password, OrderStatus_NewOrder);

            ProcessOrders(OrdersXML);

            txtResults.Text = OrdersXML;
        }

        private void btnUpdateOrderStatus_Click(object sender, EventArgs e)
        {
            MagiConnect_Sales.sales SalesWebService = new SystemsIntegrationSample.MagiConnect_Sales.sales();

            string GetInput = Microsoft.VisualBasic.Interaction.InputBox("Please enter the ORDER ID (must be a GUID)", "Enter Order ID", "", 0, 0);
            Guid OrderID = new Guid(GetInput);

            // This changes the status of the order from NEW ORDER to ORDER COMPLETED
            // and also updates the order with a tracking reference number along with an optional status text message for the order
            string Login = GetMagiConnectLogin();
            string Password = GetMagiConnectPassword();
            Guid OrderStatus_NewOrder = new Guid(System.Configuration.ConfigurationManager.AppSettings["MagiConnectOrderStatus_NewOrder"]);
            Guid OrderStatus_Completed = new Guid(System.Configuration.ConfigurationManager.AppSettings["MagiConnectOrderStatus_Completed"]);
            string OrderTrackingReference = "TST001";
            string OrderStatusMessage = "This is a sample status message for the order";

            string Results = SalesWebService.UpdateOrderStatus(Login, Password,
                OrderID, OrderStatus_NewOrder, OrderStatus_Completed, OrderTrackingReference, OrderStatusMessage);

            txtResults.Text = Results;
        }

        private void btnExportOrdersUsingDateRange_Click(object sender, EventArgs e)
        {
            MagiConnect_Sales.sales SalesWebService = new SystemsIntegrationSample.MagiConnect_Sales.sales();

            // This exports into XML all orders for the last 2 weeks
            string Login = GetMagiConnectLogin();
            string Password = GetMagiConnectPassword();
            Guid OrderStatus_NewOrder = new Guid(System.Configuration.ConfigurationManager.AppSettings["MagiConnectOrderStatus_NewOrder"]);
            DateTime FromDate = DateTime.Today.AddDays(-14);

            string OrdersXML = SalesWebService.ExportOrdersUsingDateRange(Login, Password,
                OrderStatus_NewOrder, FromDate, DateTime.MaxValue);

            ProcessOrders(OrdersXML);

            txtResults.Text = OrdersXML;
        }

        private void ProcessOrders(string OrdersXML)
        {
            // This loads the ORDERS XML passed in and cycles through each order
            XmlDocument OrdersXMLObject = new XmlDocument();
            OrdersXMLObject.Load(new StringReader(OrdersXML));

            XmlNodeList XMLCurrentOrders = OrdersXMLObject.SelectNodes("//Order");
            foreach (XmlNode XMLCurrentOrder in XMLCurrentOrders)
            {
                // Extract out values from the XML Node
                string OrderID_String = XMLCurrentOrder.SelectSingleNode("OrderID").InnerText;
                Guid OrderID = new Guid(OrderID_String);
                
                string OrderNumber = XMLCurrentOrder.SelectSingleNode("OrdNo").InnerText;
                
                string OrderDate_String = XMLCurrentOrder.SelectSingleNode("Date").InnerText;
                DateTime OrderDate = Convert.ToDateTime(OrderDate_String);
                
                string ItemsTotal_String = XMLCurrentOrder.SelectSingleNode("ItemsTotal").InnerText;
                // Items may not have any order items
                if (ItemsTotal_String == "")
                { ItemsTotal_String = "0"; }
                decimal ItemsTotal = Convert.ToDecimal(ItemsTotal_String);

                string OrderItemsCountString = XMLCurrentOrder.SelectSingleNode("NoItems").InnerText;
                if (OrderItemsCountString == "")
                { OrderItemsCountString = "0"; }
                decimal OrderItemsCount = Convert.ToDecimal(OrderItemsCountString);
                
                int OrderItemIndex = 0;

                bool OrderAlreadyExists = false;
                
                // Look up that the ORDER ID (or ORDER NUMBER as this is also unique) does not already exist
                // - if it does, skip this step and go straight to UPDATE ORDERS STATUS
                // PUT CODE IN HERE TO CHECK IF THE ORDER ALREADY EXISTS ON YOUR INTERNAL SYSTEM



                if (OrderAlreadyExists == false)
                {
                    // OPTIONAL: PUT CODE IN HERE TO START A DATABASE TRANSACTION FOR YOUR INTERNAL SYSTEM
                    // SO THAT THE ORDER HEADER and ORDER ITEMS are put in together at the same time



                    // PUT CODE IN HERE TO INSERT THE ORDER HEADER INTO YOUR SYSTEM



                    // get the order items
                    XmlNodeList XMLCurrentOrderItems = XMLCurrentOrder.SelectNodes("OrderItem");

                    // Cycle through the orders items
                    foreach (XmlNode XMLCurrentOrderItem in XMLCurrentOrderItems)
                    {
                        // Increment the order item index
                        OrderItemIndex++;

                        string OrderItemID_String = XMLCurrentOrderItem.SelectSingleNode("ItemID").InnerText;
                        Guid OrderItemID = new Guid(OrderItemID_String);

                        string Code = XMLCurrentOrderItem.SelectSingleNode("Code").InnerText;

                        string UnitPrice_String = XMLCurrentOrderItem.SelectSingleNode("Price").InnerText;
                        decimal UnitPrice = Convert.ToDecimal(UnitPrice_String);

                        // Reduce the unit price by any discount amount that has been apportioned to it
                        string UnitPriceDisc_String = XMLCurrentOrderItem.SelectSingleNode("PriceDisc").InnerText;
                        if (UnitPriceDisc_String == "")
                        { UnitPriceDisc_String = "0"; }
                        decimal UnitPriceDisc = Convert.ToDecimal(UnitPriceDisc_String);

                        UnitPrice -= UnitPriceDisc;

                        string Quantity_String = XMLCurrentOrderItem.SelectSingleNode("Qty").InnerText;
                        decimal Quantity = Convert.ToDecimal(Quantity_String);
                        
                        // PUT CODE IN HERE TO INSERT THE ORDER ITEM INTO YOUR SYSTEM



                    }

                    // OPTIONAL: PUT CODE IN HERE TO COMMIT THE TRANSACTION IF THERE WERE NO ERRORS



                }

                // Update the status of the order on the web server
                // - This is put here as if order has already been uploaded, we still update it's status as it
                //   should mean that the batch job fell over the last time on this order after it uploaded it but before it updated the website
                Guid FromStatusID = new Guid(System.Configuration.ConfigurationManager.AppSettings["MagiConnectOrderStatus_NewOrder"]);
                Guid ToStatusID = new Guid(System.Configuration.ConfigurationManager.AppSettings["MagiConnectOrderStatus_Downloaded"]);

                try
                {
                    // Set the timeout to 10 seconds and catch the error if it goes over this as if the web service isnt instant, it could hang
                    // so we just try the web service again
                    MagiConnect_Sales.sales SalesWebService = new SystemsIntegrationSample.MagiConnect_Sales.sales();
                    SalesWebService.Timeout = 10000;

                    // This changes the status of the order from NEW ORDER to ORDER COMPLETED
                    // and also updates the order with a tracking reference number along with an optional status text message for the order
                    string Login = GetMagiConnectLogin();
                    string Password = GetMagiConnectPassword();
                    Guid OrderStatus_NewOrder = new Guid(System.Configuration.ConfigurationManager.AppSettings["MagiConnectOrderStatus_NewOrder"]);
                    Guid OrderStatus_Downloaded = new Guid(System.Configuration.ConfigurationManager.AppSettings["MagiConnectOrderStatus_Downloaded"]);
                    string OrderTrackingReference = "";
                    string OrderStatusMessage = "";

                    SalesWebService.UpdateOrderStatus(Login, Password,
                        OrderID, OrderStatus_NewOrder, OrderStatus_Downloaded, OrderTrackingReference, OrderStatusMessage);
                }
                catch (Exception e)
                {
                    // YOU CAN COPY THE CODE ABOVE TO RETRY THE WEB SERVICE A SECOND TIME
                    // or YOU CAN JUST LEAVE IT AS THE WEB SERVICE WILL UPDATE IT THE NEXT TIME
                    // THIS BATCH JOB IS RUN



                }
            }
        }
    }
}