/*
    Copyright (C) <2017>  <Sherwin Bayer, Jasneel Chauhan, Heemesh Bhikha, Melvin Mathew> 
    This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
 
    This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Affero General Public License for more details.
    You should have received a copy of the GNU Affero General Public License along with this program.  If not, see <http://www.gnu.org/licenses/>. 

    This program also makes use of the EPPlus API. This library is unmodified, the program simply makes use of its API: you can redistribute it and/or modify it under the terms of the GNU Library General Public License (LGPL) Version 2.1, February 1999.
    You should have received a copy of the GNU Library General Public License along with this program.  If not, see <http://epplus.codeplex.com/license/>. 

*/
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
        public static string generateExcelFileReport(string folderpath, string[] accountNums, double[] fixedWasteWaterCosts,
            double[] totalCosts, string[] propertyLocations, string[] accountTypes, string[] invoiceDates, string[] dueDates, string[] thisReadingDates,
            string[] lastReadingDates, double[] chargesForEachTenant, int[] thisReadingAmounts, int[] lastReadingAmounts, string[] thisReadingAmountTypes,
            string[] lastReadingAmountTypes, double[] wasteWaterPercentages, string[] waterUnitRates, string[] wasteWaterUnitRates, string[] waterConsumptions,
            string[] wasteWaterConsumptions, string[] waterCosts, string[] wasteWaterCosts, string[] rVias, string[] whoPays, string[] tenantNames, string[] tenantDRs,
            string[] procedures, string[] toTenantVias, string[] ownerFees, string[] ownerNames)
        {
            DateTime currentDate = DateTime.Now.Date;
            FileInfo newFile = new FileInfo(folderpath + @"\Overall Scanned Report.xlsx");
            if (newFile.Exists)
            {
                newFile.Delete();  // Ensures we create a new workbook
                newFile = new FileInfo(folderpath + @"\Overall Scanned Report.xlsx");
            }
            using (ExcelPackage package = new ExcelPackage(newFile))
            {
                // Add a new worksheet to the empty workbook
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Tenant Info");

                // Rows and column indices not indexed at zero!

                // Add a merged header title
                worksheet.Cells[1, 1, 1, 27].Merge = true;
                worksheet.Cells[1, 1, 1, 27].Value = "Overall Extracted Data for the use of First Property Management";
                worksheet.Cells[1, 1, 1, 27].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                // Add the column titles
                worksheet.Cells[2, 1].Value = "Current Date";
                worksheet.Cells[2, 2].Value = "Property Address";
                worksheet.Cells[2, 3].Value = "Owner Name";
                worksheet.Cells[2, 4].Value = "Account No.";
                worksheet.Cells[2, 5].Value = "Who Pays";
                worksheet.Cells[2, 6].Value = "Invoice Date";
                worksheet.Cells[2, 7].Value = "Invoice Amount"; // This column is meant to hold the total cost amount from the .pdf
                worksheet.Cells[2, 8].Value = "Tenant Full Name";
                worksheet.Cells[2, 9].Value = "Tnt"; // This column holds the (total cost - fixed waste water charges) amount
                worksheet.Cells[2, 10].Value = "From Date";
                worksheet.Cells[2, 11].Value = "To Date";
                worksheet.Cells[2, 12].Value = "Recd. Via";
                worksheet.Cells[2, 13].Value = "Owner Fee";
                worksheet.Cells[2, 14].Value = "Tenant Dr";
                worksheet.Cells[2, 15].Value = "Procedure";
                worksheet.Cells[2, 16].Value = "To Tnt Via";
                worksheet.Cells[2, 17].Value = "Previous Reading"; // Previous reading
                worksheet.Cells[2, 18].Value = "Estimate/Actual"; // Estimate/Actual
                worksheet.Cells[2, 19].Value = "This Reading"; // This Reading
                worksheet.Cells[2, 20].Value = "Estimate/Actual"; // Estimate/Actual
                worksheet.Cells[2, 21].Value = "Vol. Water kL"; // Vol. Water kL
                worksheet.Cells[2, 22].Value = "Rate";
                worksheet.Cells[2, 23].Value = "Vol. Water $";
                worksheet.Cells[2, 24].Value = "Vol. Waste Water %";
                worksheet.Cells[2, 25].Value = "Vol. Waste Water kL";
                worksheet.Cells[2, 26].Value = "Rate";
                worksheet.Cells[2, 27].Value = "Vol. Waste Water $";
                /*worksheet.Cells[2, 22].Value = "Tnt"; // This column holds the (total cost - fixed waste water charges) amount
                worksheet.Cells[2, 23].Value = "R Via";
                worksheet.Cells[2, 24].Value = "Tenant DR";
                worksheet.Cells[2, 25].Value = "Procedure";
                worksheet.Cells[2, 26].Value = "To Tenant Via";
                worksheet.Cells[2, 27].Value = "Owner Fee";*/
               
                // Format the cells 
                worksheet.Cells["G3:G" + accountNums.Length + 2].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells["I3:I" + accountNums.Length + 2].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells["X3:X" + accountNums.Length + 2].Style.Numberformat.Format = "#,##0.00";
                //worksheet.Cells["G3:H" + accountNums.Length + 2].Style.Numberformat.Format = "dd-mmm-yy";
                // Date format doesn't seem to work?

                // Add the extracted data 
                for (int i = 3; i <= accountNums.Length + 2; i++)
                {
                    worksheet.Cells[i, 1].Value = currentDate.ToShortDateString();
                    worksheet.Cells[i, 2].Value = propertyLocations[i - 3];
                    worksheet.Cells[i, 3].Value = ownerNames[i-3];
                    worksheet.Cells[i, 4].Value = accountNums[i - 3];
                    worksheet.Cells[i, 5].Value = whoPays[i - 3];
                    worksheet.Cells[i, 6].Value = invoiceDates[i - 3];
                    worksheet.Cells[i, 7].Value = totalCosts[i-3];
                    //worksheet.Cells[i, 7].Formula = "=DATEVALUE("+ thisReadingDates[i - 3] +")";
                    worksheet.Cells[i, 8].Value = tenantNames[i-3];
                    //worksheet.Cells[i, 8].Formula = "=DATEVALUE(" + lastReadingDates[i - 3] + ")";
                    worksheet.Cells[i, 9].Value = chargesForEachTenant[i - 3];
                    worksheet.Cells[i, 10].Value = lastReadingDates[i - 3];
                    worksheet.Cells[i, 11].Value = thisReadingDates[i - 3];
                    worksheet.Cells[i, 12].Value = rVias[i - 3];
                    worksheet.Cells[i, 13].Value = ownerFees[i - 3];
                    worksheet.Cells[i, 14].Value = tenantDRs[i - 3];
                    worksheet.Cells[i, 15].Value = procedures[i - 3];
                    worksheet.Cells[i, 16].Value = toTenantVias[i - 3];
                    worksheet.Cells[i, 17].Value = lastReadingAmounts[i - 3];
                    worksheet.Cells[i, 18].Value = lastReadingAmountTypes[i - 3];
                    worksheet.Cells[i, 19].Value = thisReadingAmounts[i - 3];
                    worksheet.Cells[i, 20].Value = thisReadingAmountTypes[i - 3];
                    worksheet.Cells[i, 21].Value = waterConsumptions[i - 3];
                    worksheet.Cells[i, 22].Value = waterUnitRates[i - 3];
                    worksheet.Cells[i, 23].Value = waterCosts[i - 3];
                    worksheet.Cells[i, 24].Value = wasteWaterPercentages[i - 3];
                    worksheet.Cells[i, 25].Value = wasteWaterConsumptions[i - 3];
                    worksheet.Cells[i, 26].Value = wasteWaterUnitRates[i - 3];
                    worksheet.Cells[i, 27].Value = wasteWaterCosts[i - 3];
                }

                // Now format the header;
                using (var range = worksheet.Cells[1, 1, 1, 27])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.Maroon);
                    range.Style.Font.Color.SetColor(Color.White);
                    range.Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Gold);
                }

                // Now format the column titles;
                using (var range = worksheet.Cells[2, 1, 2, 27])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.DarkBlue);
                    range.Style.Font.Color.SetColor(Color.White);
                    //range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }

                //worksheet.Calculate();

                // Create an autofilter for the range
                worksheet.Cells["A2:AA" + accountNums.Length + 2].AutoFilter = true;

                worksheet.Cells.AutoFitColumns(0);  //Autofit columns for all cells

                // Set some document properties
                package.Workbook.Properties.Title = "Overall Scanned Information Report";
                package.Workbook.Properties.Author = "Sherwin Bayer, Jasneel Chauhan, Heemesh Bhikha, Melvin Mathew";
                package.Workbook.Properties.Comments = "This will need to be converted into a .csv to be imported into Palace";

                // Set some extended property values
                package.Workbook.Properties.Company = "First Property Management";
                package.Workbook.Properties.Manager = "Mohijit Sengupta, Jolene Munro";
                package.Save();

            }

            return newFile.FullName;

        }
    }
}
