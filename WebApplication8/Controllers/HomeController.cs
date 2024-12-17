using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using WebApplication8.Models;
using System.Configuration;
using OfficeOpenXml;
using System.Text;
using System.Net;
using ClosedXML.Excel;
using System.Globalization;


namespace WebApplication8.Controllers
{
    public class HomeController : Controller
    {
        private string cs = ConfigurationManager.ConnectionStrings["dbcccd"].ConnectionString;
        StudentDBContext context = new StudentDBContext();
        public ActionResult Index()
        {
            return View();
        }




        public JsonResult SearchFieldByPage(string search, int pageSize, int pageNumber)
        {
            try
            {
                using (var connection = new SqlConnection(cs))
                {
                    connection.Open();

                    using (var command = new SqlCommand("GetPagedBookings", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Search", string.IsNullOrEmpty(search) ? (object)DBNull.Value : search);
                        command.Parameters.AddWithValue("@PageSize", pageSize);
                        command.Parameters.AddWithValue("@PageNumber", pageNumber);

                        using (var reader = command.ExecuteReader())
                        {
                            var bookings = new List<Registration>();
                            int totalRecords = 0;

                            while (reader.Read())
                            {
                                var id = reader.IsDBNull(0) ? 0 : Convert.ToInt32(reader[0]);
                                var FullName = reader.IsDBNull(1) ? " " : Convert.ToString(reader[1]);
                                var BookingType = reader.IsDBNull(2) ? "" : Convert.ToString(reader[2]);
                                var Country = reader.IsDBNull(3) ? "" : Convert.ToString(reader[3]);
                                var State = reader.IsDBNull(4) ? "" : reader[4].ToString();
                                var BookingDate = reader.IsDBNull(5) ? DateTime.MinValue : Convert.ToDateTime(reader[5]);
                                var Status = reader.IsDBNull(6) ? false : Convert.ToBoolean(reader[6]);
                                var Description = reader.IsDBNull(7) ? "" : Convert.ToString(reader[7]);
                                var Image = reader.IsDBNull(8) ? "" : reader[8].ToString();
                                var pdf = reader.IsDBNull(9) ? "" : Convert.ToString(reader[9]);
                                var CreatedDate = reader.IsDBNull(10) ? DateTime.MinValue : Convert.ToDateTime(reader[10]);
                                var UpdatedDate = reader.IsDBNull(11) ? DateTime.MinValue : Convert.ToDateTime(reader[11]);

                                bookings.Add(new Registration
                                {
                                    RegId = id,
                                    FullName = FullName,
                                    BookingType = BookingType,
                                    Country = Country,
                                    State = State,
                                    BookingDate = BookingDate,
                                    Status = Status,
                                    Description = Description,
                                    CreatedDate = CreatedDate,
                                    UpdatedDate = UpdatedDate,
                                    Image = Image,
                                    Pdf = pdf
                                });

                                // Assuming TotalRecords is the same for all rows, read it only once
                                if (totalRecords == 0 && !reader.IsDBNull(12))
                                {
                                    totalRecords = Convert.ToInt32(reader[12]);
                                    System.Diagnostics.Debug.WriteLine($"Total Records: {totalRecords}");
                                }
                            }

                            var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

                            return Json(new { data = bookings, totalRecords, totalPages }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult getdata()
        {
            StudentDBContext db = new StudentDBContext();
            List<Registration> obj = db.GetStudents();
            return Json(obj, JsonRequestBehavior.AllowGet);
        }


        
        public JsonResult Create()
        {
            StudentDBContext context = new StudentDBContext();
            List<Countries> obj = context.GetCountry();
            List<States> stat = context.GetState();
            Registration model = new Registration
            {
                Countries = obj,
                States = stat
            };

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Create(Registration std, HttpPostedFileBase Image, HttpPostedFileBase Pdf)
        {
            try
            {
                if (Image != null && Image.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(Image.FileName);
                    string filePath = Path.Combine(Server.MapPath("~/ImgUpload"), fileName);
                    Image.SaveAs(filePath);

                    // Assign file path to Image property
                    std.Image = fileName;
                }

                if (Pdf != null && Pdf.ContentLength > 0)
                {
                    string pdfName = Path.GetFileName(Pdf.FileName);
                    string pdfPath = Path.Combine(Server.MapPath("~/PdfUpload"), pdfName);
                    Pdf.SaveAs(pdfPath);

                    // Assign file path to Image property
                    std.Pdf = pdfName;
                }

                if (ModelState.IsValid == true | 1 == 1)
                {
                    StudentDBContext context = new StudentDBContext();
                    bool check = context.AddStudent(std);
                    if (check == true)
                    {
                        TempData["InsertMessage"] = "Data has been Inserted Successfully";
                        ModelState.Clear();
                        return Json(check, JsonRequestBehavior.AllowGet);
                    }
                }

                else
                {
                    return Json(std, JsonRequestBehavior.AllowGet);
                }

                return Json(new { message = "Succes" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpGet]
        public JsonResult Edit1(int id)
        {
            StudentDBContext context = new StudentDBContext();
            var row = context.GetStudents(id);
            return Json(row, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Edit(int id, Registration reg, HttpPostedFileBase Image, HttpPostedFileBase Pdf)
        {
            try
            {
                if (ModelState.IsValid == true || 1 == 1)
                {
                    StudentDBContext context = new StudentDBContext();
                    var row = context.GetStudents(id);

                    if (Image != null && Image.ContentLength > 0)
                    {
                        string fileName = Path.GetFileName(Image.FileName);
                        string filePath = Path.Combine(Server.MapPath("~/ImgUpload"), fileName);
                        Image.SaveAs(filePath);

                        // Assign file path to Image property
                        reg.Image = fileName;
                    }
                    else
                    {
                        reg.Image = row.Image;
                    }

                    if (Pdf != null && Pdf.ContentLength > 0)
                    {
                        string pdfName = Path.GetFileName(Pdf.FileName);
                        string pdfPath = Path.Combine(Server.MapPath("~/PdfUpload"), pdfName);
                        Pdf.SaveAs(pdfPath);

                        // Assign file path to Image property
                        reg.Pdf = pdfName;
                    }
                    else
                    {
                        reg.Pdf = row.Pdf;
                    }

                    bool check = context.UpdateStudent(reg);
                    if (check == true)
                    {
                        TempData["EditMessage"] = "Data has been Updated Successfully";
                        ModelState.Clear();
                        return Json(check, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(new { message = "Succes" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception EX)
            {
                return Json(EX, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            StudentDBContext context = new StudentDBContext();
            var row = context.DeleteStudent(id);
            return Json(row, JsonRequestBehavior.AllowGet);
        }

        public int Checkunique(string FullName, string BookingDate, string BookingType)
        {
            StudentDBContext db = new StudentDBContext();
            Registration model = new Registration();

            List<Registration> result = db.GetStudents();


            int isAvailable = 0;


            var NewList = result.Where(x => x.FullName == FullName && x.BookingDate.ToString("yyyy-MM-dd") == BookingDate && x.BookingType == BookingType).ToList();
            if (NewList.Count > 0) { isAvailable = 1; }
            else { isAvailable = 0; }

            return isAvailable;

        }

        public JsonResult DELIMG(int id)
        {
            StudentDBContext context = new StudentDBContext();
            var row = context.DELIMG(id);
            return Json(row, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DELPDF(int id)
        {
            StudentDBContext context = new StudentDBContext();
            var row = context.DELPDF(id);
            return Json(row, JsonRequestBehavior.AllowGet);
        }




        public FileResult ExportToExcel()
        {

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Registrations Data");


                worksheet.Cells[1, 1].Value = "Id";
                worksheet.Cells[1, 2].Value = "FullName";
                worksheet.Cells[1, 3].Value = "BookingType";
                worksheet.Cells[1, 4].Value = "Category";
                worksheet.Cells[1, 5].Value = "Subcategory";
                worksheet.Cells[1, 6].Value = "BookingDate";
                worksheet.Cells[1, 7].Value = "Status";
                worksheet.Cells[1, 8].Value = "Description";
                worksheet.Cells[1, 9].Value = "Image";
                worksheet.Cells[1, 10].Value = "Pdf";
                worksheet.Cells[1, 11].Value = "CreatedDate";
                worksheet.Cells[1, 12].Value = "UpdatedDate";

                List<Registration> students = gdata();
                int row = 2;

                foreach (var student in students)
                {
                    worksheet.Cells[row, 1].Value = student.RegId;
                    worksheet.Cells[row, 2].Value = student.FullName;
                    worksheet.Cells[row, 3].Value = student.BookingType;
                    worksheet.Cells[row, 4].Value = student.Country;
                    worksheet.Cells[row, 5].Value = student.State;
                    worksheet.Cells[row, 6].Value = student.BookingDate != null ? student.BookingDate.ToString("dd-MM-yyyy") : string.Empty;
                    string status = student.Status ? "Active" : "Inactive";
                    worksheet.Cells[row, 7].Value = status;
                    worksheet.Cells[row, 8].Value = student.Description;
                    worksheet.Cells[row, 9].Value = student.Image;
                    worksheet.Cells[row, 10].Value = student.Pdf;
                    worksheet.Cells[row, 11].Value = student.CreatedDate != null ? student.CreatedDate.ToString("dd-MM-yyyy") : string.Empty;
                    worksheet.Cells[row, 12].Value = student.UpdatedDate != null ? student.UpdatedDate.ToString("dd-MM-yyyy") : string.Empty;

                    row++;
                }


                var fileContent = package.GetAsByteArray();
                var contentDisposition = new System.Net.Mime.ContentDisposition
                {
                    FileName = "Registration.xlsx",
                    Inline = false
                };
                Response.AppendHeader("Content-Disposition", contentDisposition.ToString());

                return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Registration.xlsx");
            }
        }


        private List<Registration> gdata()
        {
            StudentDBContext db = new StudentDBContext();
            return db.GetStudents().ToList();
        }


        public FileResult ExportToCsv()
        {
            var sb = new StringBuilder();


            sb.AppendLine("Id,FullName,BookingType,Category,Subcategory,BookingDate,Status,Description,Image,Pdf,CreatedDate,UpdatedDate");

            List<Registration> students = gdata(); // Assuming gdata() fetches the list of registrations

            foreach (var student in students)
            {
                string bookingDate = student.BookingDate != null ? student.BookingDate.ToString("dd-MM-yyyy") : string.Empty;
                string createdDate = student.CreatedDate != null ? student.CreatedDate.ToString("dd-MM-yyyy") : string.Empty;
                string updatedDate = student.UpdatedDate != null ? student.UpdatedDate.ToString("dd-MM-yyyy") : string.Empty;
                string status = student.Status ? "Active" : "Inactive";

                sb.AppendLine($"{student.RegId},{student.FullName},{student.BookingType},{student.Country},{student.State},{bookingDate},{status},{student.Description},{student.Image},{student.Pdf},{createdDate},{updatedDate}");
            }

            byte[] fileBytes = Encoding.UTF8.GetBytes(sb.ToString());
            var contentDisposition = new System.Net.Mime.ContentDisposition
            {
                FileName = "Registration.csv",
                Inline = false  // false = prompt the user to download the file
            };
            Response.AppendHeader("Content-Disposition", contentDisposition.ToString());

            return File(fileBytes, "text/csv", "Registration.csv");
        }



   

        [HttpPost]
        public JsonResult UploadExcel(HttpPostedFileBase file)
        {
            string errorMessage = "";
            string success = "";
            List<int> rowsNumbers = new List<int>(); // Insert data rows numbers
            Dictionary<string, List<int>> rowErrors = new Dictionary<string, List<int>>(); // Store row-wise error

            try
            {
                if (file != null && file.ContentLength > 0)
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    using (var package = new ExcelPackage(file.InputStream))
                    {
                        var worksheet = package.Workbook.Worksheets.First();
                        var rowCount = worksheet.Dimension.Rows;

                        var excelDataList = new List<Registration>();
                        bool flag = false; // Flag is used to check if there are any validation errors

                        var DB = new StudentDBContext();
                        var existingRecords = DB.GetStudents(); // Assume this method retrieves all existing registrations
                        var existingRecordSet = existingRecords.ToHashSet(); // Create a set for quick lookup

                        for (int row = 2; row <= rowCount; row++) // row = 2 because row 1 is header
                        {
                            // Declare the 'data' object before using it
                            var data = new Registration();

                            // Validate FullName
                            data.FullName = worksheet.Cells[row, 1].Value?.ToString().Trim();
                            if (string.IsNullOrWhiteSpace(data.FullName) || data.FullName.ToLower() == "abcd")
                            {
                                flag = true;
                                if (!rowErrors.ContainsKey("FullName"))
                                    rowErrors["FullName"] = new List<int>();

                                rowErrors["FullName"].Add(row); // Track the row with invalid or missing full name
                            }

                            // Validate BookingType (Only allow "General" and "OBC")
                            data.BookingType = worksheet.Cells[row, 2].Value?.ToString().Trim();
                            if (string.IsNullOrWhiteSpace(data.BookingType) ||
                                !(data.BookingType.Equals("General", StringComparison.OrdinalIgnoreCase) ||
                                  data.BookingType.Equals("OBC", StringComparison.OrdinalIgnoreCase)))
                            {
                                flag = true;
                                if (!rowErrors.ContainsKey("BookingType"))
                                    rowErrors["BookingType"] = new List<int>();

                                rowErrors["BookingType"].Add(row); // Track the row with invalid booking type
                            }

                            // Validate Category (Only allow specific values)
                            data.Country = worksheet.Cells[row, 3].Value?.ToString().Trim();
                            if (string.IsNullOrWhiteSpace(data.Country) ||
                                !(data.Country.Equals("Diploma", StringComparison.OrdinalIgnoreCase) ||
                                  data.Country.Equals("Engeering", StringComparison.OrdinalIgnoreCase)
                                  ))
                            {
                                flag = true;
                                if (!rowErrors.ContainsKey("Category")) // Replacing 'Country' with 'Category'
                                    rowErrors["Category"] = new List<int>();

                                rowErrors["Category"].Add(row); // Track the row with invalid category
                            }

                            // Validate Subcategory (Only allow specific values)
                            data.State = worksheet.Cells[row, 4].Value?.ToString().Trim();
                            if (string.IsNullOrWhiteSpace(data.State) ||
                                !(data.State.Equals("It", StringComparison.OrdinalIgnoreCase) ||
                                  data.State.Equals("Computer", StringComparison.OrdinalIgnoreCase) ||
                                  data.State.Equals("Civil", StringComparison.OrdinalIgnoreCase) ||
                                  data.State.Equals("Mechanical", StringComparison.OrdinalIgnoreCase)))
                            {
                                flag = true;
                                if (!rowErrors.ContainsKey("Subcategory")) // Replacing 'State' with 'Subcategory'
                                    rowErrors["Subcategory"] = new List<int>();

                                rowErrors["Subcategory"].Add(row); // Track the row with invalid subcategory
                            }

                            // Validate BookingDate
                            var bookingDateValue = worksheet.Cells[row, 5].Value;
                            DateTime bookingDate;
                            if (bookingDateValue == null || !DateTime.TryParse(bookingDateValue.ToString(), out bookingDate) || bookingDateValue.ToString().ToLower() == "abcd")
                            {
                                flag = true;
                                if (!rowErrors.ContainsKey("BookingDate"))
                                    rowErrors["BookingDate"] = new List<int>();

                                rowErrors["BookingDate"].Add(row); // Track the row with invalid booking date
                                bookingDate = DateTime.MinValue; // Optional default
                            }
                            else
                            {
                                data.BookingDate = bookingDate; // Assign valid date if parsed correctly
                            }

                            // Validate Status (Only allow "Active" and "Inactive")
                            var statusValue = worksheet.Cells[row, 6].Value;
                            bool status = false;
                            if (statusValue != null)
                            {
                                var statusString = statusValue.ToString().Trim().ToLower();
                                if (statusString == "active")
                                {
                                    status = true;
                                }
                                else if (statusString == "inactive")
                                {
                                    status = false;
                                }
                                else
                                {
                                    flag = true;
                                    if (!rowErrors.ContainsKey("Status"))
                                        rowErrors["Status"] = new List<int>();

                                    rowErrors["Status"].Add(row); // Track the row with invalid status
                                }
                            }

                            // Assign the status value to the data object
                            data.Status = status;

                            // Validate Description
                            data.Description = worksheet.Cells[row, 7].Value?.ToString().Trim();
                            if (data.Description != null && data.Description.ToLower() == "abcd")
                            {
                                flag = true;
                                if (!rowErrors.ContainsKey("Description"))
                                    rowErrors["Description"] = new List<int>();

                                rowErrors["Description"].Add(row); // Track the row with invalid description
                            }

                            // Check for duplicates in FullName only
                            if (!string.IsNullOrWhiteSpace(data.FullName) && data.BookingDate != DateTime.MinValue)
                            {
                                var duplicate = existingRecordSet.Any(r => r.FullName == data.FullName && r.BookingDate == data.BookingDate);
                                if (duplicate)
                                {
                                    flag = true;
                                    if (!rowErrors.ContainsKey("FullName and BookingDate"))
                                        rowErrors["FullName and BookingDate"] = new List<int>();

                                    rowErrors["FullName and BookingDate"].Add(row); // Track the row with duplicate FullName and BookingDate
                                }
                            }

                            if (!flag)
                            {
                                excelDataList.Add(data); // Add valid data to the list
                                rowsNumbers.Add(row); // Track valid rows (use the actual row number)
                            }
                        }

                        if (flag)
                        {
                            // Generate error message from rowErrors
                            foreach (var error in rowErrors)
                            {
                                errorMessage += $"\n{error.Key} is invalid or duplicate in rows {string.Join(", ", error.Value)}\n";
                            }

                            return Json(new { success = false, message = errorMessage, Popup = "Error" });
                        }
                        else
                        {
                            try
                            {
                                foreach (var data in excelDataList)
                                {
                                    DB.addStudentExcel(data); // Add data to the database
                                }

                                success = "Data Imported successfully";
                                return Json(new { success = true, message = success, Popup = "Success" });
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Exception during database operation: {ex}");
                                return Json(new { success = false, message = "An error occurred while saving data to the database." });
                            }
                        } 
                    }
                }
            }    
            catch
            {
                return Json(new { success = false, message = "An error occurred" });
            }

            return Json(new { success = false, message = "No File Selected" });
        }




        








 







    }
}



