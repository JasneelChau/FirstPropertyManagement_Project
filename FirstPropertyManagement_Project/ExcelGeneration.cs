using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstPropertyManagement_Project
{
    class ExcelGeneration
    {

        public static string generateExcelFileReport(string folderpath, string[] accountNums, double[] wasteWaterCosts,
            double[] totalCosts, string[] propertyLocations, string[] accountTypes, string[] dueDates, string[] thisReadingDates,
            string[] lastReadingDates, double[] chargesForEachTenant)
        {
            FileInfo newFile = new FileInfo(folderpath + @"\Overall Scanned Report.xlsx");
            if (newFile.Exists)
            {
                newFile.Delete();  // ensures we create a new workbook
                newFile = new FileInfo(folderpath + @"\Overall Scanned Report.xlsx");
            }
            using (ExcelPackage package = new ExcelPackage(newFile))
            {
                // add a new worksheet to the empty workbook
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Tenant Info");

                // rows and column indices not indexed at zero!

                // Add a merged header title
                worksheet.Cells[1, 1, 1, 9].Merge = true;
                worksheet.Cells[1, 1, 1, 9].Value = "Overall Extracted Data for the use of First Property Management";
                worksheet.Cells[1, 1, 1, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                // Add the column titles
                worksheet.Cells[2, 1].Value = "Account Number";
                worksheet.Cells[2, 2].Value = "Fixed Wastewater";
                worksheet.Cells[2, 3].Value = "Total Cost";
                worksheet.Cells[2, 4].Value = "Property Location";
                worksheet.Cells[2, 5].Value = "Account Type";
                worksheet.Cells[2, 6].Value = "Due Date";
                worksheet.Cells[2, 7].Value = "This Reading Date";
                worksheet.Cells[2, 8].Value = "Last Reading Date";
                worksheet.Cells[2, 9].Value = "Tenant Charges";

                // Format the cells 
                worksheet.Cells["B3:C" + accountNums.Length + 2].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells["I3:I" + accountNums.Length + 2].Style.Numberformat.Format = "#,##0.00";
                //worksheet.Cells["G3:H" + accountNums.Length + 2].Style.Numberformat.Format = "dd-mmm-yy";
                // Date format doesn't seem to work?

                // Add the extracted data 
                for (int i = 3; i <= accountNums.Length + 2; i++)
                {
                    worksheet.Cells[i, 1].Value = accountNums[i - 3];
                    worksheet.Cells[i, 2].Value = wasteWaterCosts[i - 3];
                    worksheet.Cells[i, 3].Value = totalCosts[i - 3];
                    worksheet.Cells[i, 4].Value = propertyLocations[i - 3];
                    worksheet.Cells[i, 5].Value = accountTypes[i - 3];
                    worksheet.Cells[i, 6].Value = dueDates[i - 3];
                    worksheet.Cells[i, 7].Value = thisReadingDates[i - 3];
                    //worksheet.Cells[i, 7].Formula = "=DATEVALUE("+ thisReadingDates[i - 3] +")";
                    worksheet.Cells[i, 8].Value = lastReadingDates[i - 3];
                    //worksheet.Cells[i, 8].Formula = "=DATEVALUE(" + lastReadingDates[i - 3] + ")";
                    worksheet.Cells[i, 9].Value = chargesForEachTenant[i - 3];
                }

                // Now format the header;
                using (var range = worksheet.Cells[1, 1, 1, 9])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.Maroon);
                    range.Style.Font.Color.SetColor(Color.White);
                    range.Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Gold);
                }

                // Now format the column titles;
                using (var range = worksheet.Cells[2, 1, 2, 9])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.DarkBlue);
                    range.Style.Font.Color.SetColor(Color.White);
                    //range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }

                //worksheet.Calculate();

                //Create an autofilter for the range
                worksheet.Cells["A2:I" + accountNums.Length + 2].AutoFilter = true;

                worksheet.Cells.AutoFitColumns(0);  //Autofit columns for all cells

                // set some document properties
                package.Workbook.Properties.Title = "Overall Scanned Information Report";
                package.Workbook.Properties.Author = "Sherwin Bayer";
                package.Workbook.Properties.Comments = "This will need to be converted into a .csv to be imported into Palace";

                // set some extended property values
                package.Workbook.Properties.Company = "First Property Management";
                package.Save();

            }

            return newFile.FullName;

        }
    }
}
