using DisplayViewReport.Report;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Mvc;
using Microsoft.Reporting.WebForms; // For ReportViewer and ReportDataSource
using System.Data; // For DataTable usage
using System.Web.UI.WebControls;
using DisplayViewReport.Models;


namespace DisplayViewReport.Controllers
{
    public class HomeController : Controller
    {

        private readonly string _connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString;

        // Fetch all products (Read)
        public ActionResult Index()
        {
            List<Product> products = new List<Product>();
            string query = "SELECT * FROM Products";

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Product product = new Product
                        {
                            ProductId = (int)reader["ProductId"],
                            Name = reader["Name"].ToString(),
                            Price = (decimal)reader["Price"]
                        };
                        products.Add(product);
                    }
                }
            }
            catch (SqlException ex)
            {
                ViewBag.Error = $"Error fetching products from the database: {ex.Message}";
                return View("Error");
            }

            return View(products);
        }

        // GET: Create product view 
        public ActionResult Create()
        {
            return View();
        }

        // POST: Create product (Create)
        [HttpPost]
        public ActionResult Create(Product product)
        {
            if (!ModelState.IsValid)
            {
                return View(product);
            }

            string query = "INSERT INTO Products (Name, Price) VALUES (@Name, @Price)";

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Name", product.Name);
                    cmd.Parameters.AddWithValue("@Price", product.Price);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                ViewBag.Error = $"Error inserting product into the database: {ex.Message}";
                return View("Error");
            }

            return RedirectToAction("Index");
        }

        // GET: Show the product data in the edit form (Update)
        public ActionResult Edit(int id)
        {
            Product product = null;
            string query = "SELECT * FROM Products WHERE ProductId = @ProductId";

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ProductId", id);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        product = new Product
                        {
                            ProductId = (int)reader["ProductId"],
                            Name = reader["Name"].ToString(),
                            Price = (decimal)reader["Price"]
                        };
                    }
                }
            }
            catch (SqlException ex)
            {
                ViewBag.Error = $"Error fetching product details: {ex.Message}";
                return View("Error");
            }

            if (product == null)
            {
                return HttpNotFound();
            }

            return View(product);
        }

        // POST: Update the product (Update)
        [HttpPost]
        public ActionResult Edit(Product product)
        {
            if (!ModelState.IsValid)
            {
                return View(product);
            }

            string query = "UPDATE Products SET Name = @Name, Price = @Price WHERE ProductId = @ProductId";

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ProductId", product.ProductId);
                    cmd.Parameters.AddWithValue("@Name", product.Name);
                    cmd.Parameters.AddWithValue("@Price", product.Price);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                ViewBag.Error = $"Error updating the product: {ex.Message}";
                return View("Error");
            }

            return RedirectToAction("Index");
        }

        // GET: Show delete confirmation page (Delete)
        public ActionResult Delete(int id)
        {
            Product product = null;
            string query = "SELECT * FROM Products WHERE ProductId = @ProductId";

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ProductId", id);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        product = new Product
                        {
                            ProductId = (int)reader["ProductId"],
                            Name = reader["Name"].ToString(),
                            Price = (decimal)reader["Price"]
                        };
                    }
                }
            }
            catch (SqlException ex)
            {
                ViewBag.Error = $"Error fetching product for deletion: {ex.Message}";
                return View("Error");
            }

            if (product == null)
            {
                return HttpNotFound();
            }

            return View(product);
        }

        // POST: Delete the product (Delete)
        [HttpPost]
        public ActionResult DeleteConfirmed(int ProductId)
        {
            string query = "DELETE FROM Products WHERE ProductId = @ProductId";

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ProductId", ProductId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                ViewBag.Error = $"Error deleting product: {ex.Message}";
                return View("Error");
            }

            return RedirectToAction("Index");
        }
        public ActionResult About()
        {
            MyDataSet ds = new MyDataSet(); // Ensure the dataset is initialized
            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;
            reportViewer.SizeToReportContent = true;
            reportViewer.Width = Unit.Percentage(100); // Use 100% for the view
            reportViewer.Height = Unit.Percentage(600); // Adjust height as needed

            var connectionString = ConfigurationManager.ConnectionStrings["CitizenM10ConnectionString2"].ConnectionString;

            try
            {
                using (SqlConnection conx = new SqlConnection(connectionString))
                {
                    SqlDataAdapter adp = new SqlDataAdapter("SELECT * FROM Citizen", conx);
                    conx.Open(); // Open connection explicitly
                    adp.Fill(ds, ds.Tables[0].TableName);
                    Console.WriteLine("Number of rows retrieved: " + ds.Tables[0].Rows.Count);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving data: " + ex.Message);
            }

            var reportPath = Request.MapPath("~/Report/MyReport.rdlc");
            reportViewer.LocalReport.ReportPath = reportPath;
            reportViewer.LocalReport.DataSources.Clear();
            reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", ds.Tables[0]));

            ViewBag.ReportViewer = reportViewer;

            return View();
        }
       
        public ActionResult Contact()
        {
            MyDataSet ds = new MyDataSet(); // Ensure the dataset is initialized
            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;
            reportViewer.SizeToReportContent = true;
            reportViewer.Width = Unit.Percentage(100); // Use 100% for the view
            reportViewer.Height = Unit.Percentage(600); // Adjust height as needed

            var connectionString = ConfigurationManager.ConnectionStrings["MvcDatabaseConnectionString"].ConnectionString;

            try
            {
                using (SqlConnection conx = new SqlConnection(connectionString))
                {
                    SqlDataAdapter adp = new SqlDataAdapter("SELECT * FROM Products", conx);
                    conx.Open(); // Open connection explicitly
                    adp.Fill(ds, ds.Tables[0].TableName);
                    Console.WriteLine("Number of rows retrieved: " + ds.Tables[0].Rows.Count);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving data: " + ex.Message);
            }

            var reportPath = Request.MapPath("~/Report/MyReport1.rdlc");
            reportViewer.LocalReport.ReportPath = reportPath;
            reportViewer.LocalReport.DataSources.Clear();
            reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet2", ds.Tables[0]));

            ViewBag.ReportViewer = reportViewer;

            return View();
        }
    }
}
/*public ActionResult Contact()
{


    string connectionString = ConfigurationManager.ConnectionStrings["CitizenM10ConnectionString2"].ConnectionString;
    string message;

    try
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            message = "Connection successful!";
        }
    }
    catch (Exception ex)
    {
        message = "Connection failed: " + ex.Message;
    }

    ViewBag.Message = message;



    return View();
}*/