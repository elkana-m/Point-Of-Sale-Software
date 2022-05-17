using modernSale.StoreDb;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace modernSale.Prototype
{
    public struct RevenueByDate
    {
        public string Date { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class Dashboard : DbConnection
    {
        // Fields & Properties
        private DateTime startDate;
        private DateTime endDate ;
        private int numberDays;

        public int NumCustomers { get; private set; }
        public int NumSuppliers  { get; private set; }
        public int NumProducts { get; private set; }
        public List<KeyValuePair<String, int>> TopProductsList { get; private set; }   // ListTopProducts for top selling products
        public List<KeyValuePair<String, int>> LowStockList { get; private set; }      // for products under stick
        public List<RevenueByDate> GrossRevenueList { get; private set; }               // total revenue
        public int NumOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalProfit { get; set; }

        // ctor
        public Dashboard()
        {

        }

        // method to get the total num of items
        private void GetNumberItems()
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                using(var command = new SqlCommand())               // not to change
                {
                    command.Connection = connection;               // not to change
                    // Get Total Number of Customers count by id
                    command.CommandText = "select count(Cust_ID) from Customer";
                    NumCustomers = (int)command.ExecuteScalar();                           // execute query & returns first col & row

                    // Total Number of Suppliers
                    command.CommandText = "select count(Supplier_ID) from Supplier";
                    NumSuppliers = (int)command.ExecuteScalar();

                    // Total Number of Suppliers
                    command.CommandText = "select count(Prod_ID) from Product";
                    NumProducts = (int)command.ExecuteScalar();

                    // Total Number of Orders
                    command.CommandText = @"select count(Order_ID) from Order1
                                           where Order_Date between @fromDate and @toDate";
                    command.Parameters.Add("@fromDate", System.Data.SqlDbType.DateTime).Value = startDate;
                    command.Parameters.Add("@toDate", System.Data.SqlDbType.DateTime).Value = endDate;
                    NumOrders = (int)command.ExecuteScalar();
                }
            }
        }
        private void GetOrderAnalysis()
        {
            GrossRevenueList = new List<RevenueByDate>();
            TotalProfit = 0;
            TotalRevenue = 0;

            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand())               // not to change
                {
                    command.Connection = connection;               // not to change
                    command.CommandText = @"Select Order_Date, SUM(Order_Total)
                                            from Order1
                                            where Order_Date between @fromDate and @toDate
                                            group by Order_Date";
                    command.Parameters.Add("@fromDate", System.Data.SqlDbType.DateTime).Value = startDate;
                    command.Parameters.Add("@toDate", System.Data.SqlDbType.DateTime).Value = endDate;
                    var reader = command.ExecuteReader();
                    var resultTable = new List<KeyValuePair<DateTime, decimal>>();
                    while(reader.Read())
                    {
                        resultTable.Add(
                            new KeyValuePair<DateTime, decimal>((DateTime)reader[0], (decimal)reader[1]));
                        TotalRevenue += (decimal)reader[0];
                    }
                    TotalProfit = TotalRevenue * 0.2m; //20%
                    reader.Close();

                    //Group by Days
                    if (numberDays <= 30)
                    {
                        foreach (var item in resultTable)
                        {
                            GrossRevenueList.Add(new RevenueByDate()
                            {
                                Date = item.Key.ToString("dd MMM"),
                                TotalAmount = item.Value
                            });
                        }
                    }

                    // Group by weeks using LINQ (7 days Ago)
                    else if (numberDays <= 92)
                    {
                        GrossRevenueList = (from orderList in resultTable
                                            group orderList by CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(
                                                orderList.Key, CalendarWeekRule.FirstDay, DayOfWeek.Monday)
                                            into order
                                            select new RevenueByDate
                                            {
                                                Date = "Week " + order.Key.ToString(),
                                                TotalAmount = order.Sum(amount => amount.Value)
                                            }).ToList();
                    }

                    // Group by Months
                    else if (numberDays <= (365 * 2))
                    {
                        bool isYear = numberDays <= 365 ? true : false;
                        GrossRevenueList = (from orderList in resultTable
                                            group orderList by orderList.Key.ToString("MMM yyy")
                                            into order
                                            select new RevenueByDate
                                            {
                                                Date = isYear ? order.Key.Substring(0, order.Key.IndexOf(" ")) : order.Key,
                                                TotalAmount = order.Sum(amount => amount.Value)
                                            }).ToList();
                    }

                    // Group by Years
                    else
                    {
                        GrossRevenueList = (from orderList in resultTable
                                            group orderList by orderList.Key.ToString("yyy")
                                            into order
                                            select new RevenueByDate
                                            {
                                                Date = order.Key,
                                                TotalAmount = order.Sum(amount => amount.Value)
                                            }).ToList();
                    }
                }
            }
        }
        private void GetProductAnalysis()                   // method to get the perfomance of products
        {
            TopProductsList = new List<KeyValuePair<string, int>>();
            LowStockList = new List<KeyValuePair<string, int>>();
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand())               // not to change
                {
                    SqlDataReader reader;
                    command.Connection = connection;
                    command.CommandText = @"Select top 5 P.Prod_Name, SUM(Order_Item.Quantity) as Q
                                            from Order1, Order_Item 
                                            inner join Product P on P.Prod_ID = Order_Item.Prod_ID
                                            inner join Order1 O on O.Order_ID = Order_Item.Order_ID
                                            where O.Order_Date between @fromDate and @toDate
                                            group by P.Prod_Name
                                            order by Q desc";
                    command.Parameters.Add("@fromDate", System.Data.SqlDbType.DateTime).Value = startDate;
                    command.Parameters.Add("@toDate", System.Data.SqlDbType.DateTime).Value = endDate;
                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        TopProductsList.Add(new KeyValuePair<string, int>(reader[0].ToString(), (int)reader[1]));
                    }
                    reader.Close();

                    // Running out Products in Stock
                    command.CommandText = @"select Prod_Name, Stock
                                            from Product
                                            where Stock <= 6 and Is_Discontinued = 0";
                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        LowStockList.Add(
                            new KeyValuePair<string, int>(reader[0].ToString(), (int)reader[1]));
                    }
                    reader.Close();
                }
            }
        }
    
        // public methods
        public bool LoadData(DateTime startDate, DateTime endDate)
        {
            endDate = new DateTime(endDate.Year, endDate.Month, endDate.Day,
                                   endDate.Hour, endDate.Minute, 59);
            if (startDate != this.startDate || endDate != this.endDate)
            {
                this.startDate = startDate;
                this.endDate = endDate;
                this.numberDays = (endDate - startDate).Days;

                GetNumberItems();
                GetProductAnalysis();
                GetOrderAnalysis();
                Console.WriteLine($"Refreshed Data{startDate} - {endDate}");
                return true;
            }
            else
            {
                Console.WriteLine($"Fail to refresh Data, same query: {startDate} - {endDate}");
                return false;
            }
        }
    
    }
}
