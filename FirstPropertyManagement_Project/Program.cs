/*
    Copyright (C) <2017>  <Sherwin Bayer, Jasneel Chauhan, Heemesh Bhikha, Melvin Mathew> 
    This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
 
    This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Affero General Public License for more details.
    You should have received a copy of the GNU Affero General Public License along with this program.  If not, see <http://www.gnu.org/licenses/>. 

    This program also makes use of the EPPlus API. This library is unmodified, the program simply makes use of its API: you can redistribute it and/or modify it under the terms of the GNU Library General Public License (LGPL) Version 2.1, February 1999.
    You should have received a copy of the GNU Library General Public License along with this program.  If not, see <http://epplus.codeplex.com/license/>. 

*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemIOPath = System.IO.Path;
using MySql.Data.MySqlClient;
using System.Data;

namespace FirstPropertyManagement_Project
{
    class Program
    {
        public static void Main(string[] args)
        {
            //Program test = new Program();

            try
            {
                
                //Testing
                string folderpath = @"C:\FPM_pdf_files";
                // Be sure to update folderpath to match your directory!
                string currentFilePath = "";
                string fileName="";
                string fileextension="";
                string fileNameReportPath="";
                string fileNameNoExtension="";
                IEnumerator files = Directory.GetFiles(folderpath).GetEnumerator();
                DataRow[] dbRows = DatabaseExtraction_methods.getDatabaseRows();

                List<string> accountNumbersList = new List<string>();
                List<double> fixedWasteWaterCostsList = new List<double>();
                List<double> totalCostsList = new List<double>();
                List<string> propertyLocationsList = new List<string>();
                List<string> accountTypesList = new List<string>();
                List<string> dueDatesList = new List<string>();
                List<string> thisReadingDatesList = new List<string>();
                List<string> lastReadingDatesList = new List<string>();
                List<double> chargeTenantList = new List<double>();
                List<string> invoiceDatesList = new List<string>();
                List<int> thisReadingAmountList = new List<int>();
                List<int> lastReadingAmountList = new List<int>();
                List<string> thisReadingAmountTypeList = new List<string>();
                List<string> lastReadingAmountTypeList = new List<string>();
                List<double> wasteWaterPercentageList = new List<double>();
                List<string> waterRateList = new List<string>();
                List<string> wasteWaterRateList = new List<string>();
                List<string> waterConsumptionList = new List<string>();
                List<string> wasteWaterConsumptionList = new List<string>();
                List<string> waterCostsList = new List<string>();
                List<string> wasteWaterCostsList = new List<string>();
                List<string> rViaList = new List<string>();
                List<string> whoPaysList = new List<string>();
                List<string> tenantNameList = new List<string>();
                List<string> tenantDRList = new List<string>();
                List<string> procedureList = new List<string>();
                List<string> toTenantViaList = new List<string>();
                List<string> ownerFeeList = new List<string>();
                List<string> ownerNameList = new List<string>();

                while (files.MoveNext())
                {   
                    currentFilePath = files.Current.ToString(); // Entire file path
                    fileName = SystemIOPath.GetFileName(currentFilePath); // File name + extension only
                    fileextension = SystemIOPath.GetExtension(fileName); // Extension only
                    fileNameNoExtension = fileName.Replace(fileextension, String.Empty);
                    fileNameReportPath = currentFilePath.Replace(fileextension, String.Empty);

                    //Console.WriteLine("+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
                    //Console.WriteLine("File path is: " + files.Current.ToString()); // Entire file path
                   // Console.WriteLine("The current file name is: " + fileName); // File name + extension only
                    //Console.WriteLine("File type is: " + fileextension); // Extension only
                    if(fileextension.Equals(".pdf") || fileextension.Equals(".PDF"))
                    {
                        double wasteWaterCost=0;
                        double totalCost=0;
                        string accountNumber = "";
                        string propertyLocation="";
                        string accountType="";
                        string dueDate="";
                        string thisReadingDate="";
                        string lastReadingDate="";
                        string[] chargeDetails = null;
                        string[] consumptionDetails = null;
                        string invoiceDate = "";
                        string thisReadingLine = "";
                        string lastReadingLine = "";
                        int thisReading = 0;
                        int lastReading = 0;
                        string thisReadingType = "";
                        string lastReadingType = "";
                        double wasteWaterPercentage = 0;
                        string waterRate = "";
                        string wasteWaterRate = "";
                        string waterConsumption = "";
                        string wasteWaterConsumption = "";
                        string waterDetailsCost = "";
                        string wasteWaterDetailsCost = "";
                        string receiveVia = "";
                        string payee = "";
                        string tenantName = "";
                        string tenantDR = "";
                        string procedure = "";
                        string toTenantVia = "";
                        string ownerFee = "";
                        string ownerName = "";
                        double waterFirstPageCost = 0;
                        double wasteWaterFirstPageCost = 0;

                        ArrayList pdftexts = ScanningPDF_methods.ReadPdfFileArrayList(currentFilePath);
                        object[] textsArray = pdftexts.ToArray(); // There should only be 2 pages

                        // By converting the arraylist to an array, each index within the array
                        // will essentially contain all the text from a single page from the pdf.
                        for (int i = 0; i < textsArray.Length; i++)
                        {
                            string currentPage = textsArray[i].ToString();
                            char[] currentPageChars = currentPage.ToCharArray();
                            int numberOfChars = currentPageChars.Length;
                            int numberOfLines = ScanningPDF_methods.getNumberOfLines(currentPageChars);
                            string[] linesOfText = ScanningPDF_methods.getLinesOfText(currentPageChars, numberOfLines);
                            //Console.WriteLine("*****************************************************************************");
                            //Console.WriteLine("Page " + i);
                            //Console.WriteLine("Number of characters on this page is: " + numberOfChars);
                            //Console.WriteLine("Number of lines on this page is: " + numberOfLines);

                            //for (int j = 0; j < linesOfText.Length; j++)
                            //{
                               // Console.WriteLine(linesOfText[j]);
                            //}

                            if (i == 0) // First page
                            {
                                wasteWaterCost = ScanningPDF_methods.getRelevantCharges(linesOfText, "Wastewater fixed charges", "There is no overdue fixed charges");
                                totalCost = ScanningPDF_methods.getRelevantCharges(linesOfText, "Balance of current charges", "Balance still owing, now overdue");
                                propertyLocation = ScanningPDF_methods.getRelevantTextData(linesOfText, "Property location");
                                accountType = ScanningPDF_methods.getRelevantTextData(linesOfText, "Account type");
                                dueDate = ScanningPDF_methods.getDueOrInvoiceDate(linesOfText, 1);
                                accountNumber = ScanningPDF_methods.getAccountNumber(linesOfText);
                                invoiceDate = ScanningPDF_methods.getDueOrInvoiceDate(linesOfText, 0);
                                waterFirstPageCost = ScanningPDF_methods.getRelevantCharges(linesOfText, "Water volumetric charges", "There is no overdue water volumetric charges");
                                wasteWaterFirstPageCost = ScanningPDF_methods.getRelevantCharges(linesOfText, "Wastewater volumetric charges", "There is no overdue wastewater volumetric charges");
                                //Console.WriteLine("Account Number is: " + accountNumber);
                                //Console.WriteLine("Waste water cost equals: " + wasteWaterCost.ToString("0.00"));
                                //Console.WriteLine("Total cost equals: " + totalCost.ToString("0.00"));
                                //Console.WriteLine("Property Location is: " + propertyLocation);
                                //Console.WriteLine("Account Type is: " + accountType);
                                //Console.WriteLine("Due date is: " + dueDate);

                                if (accountNumber == null)
                                {
                                    accountNumber = ScanningPDF_methods.backupGetAccountMethod(linesOfText);
                                }

                                if (totalCost == 0)
                                {
                                    totalCost = ScanningPDF_methods.getTotalCostBackup(linesOfText, propertyLocation);
                                }

                                if (dueDate.Equals("") || dueDate.Equals("Correct date not found"))
                                {
                                    dueDate = ScanningPDF_methods.getDueOrInvoiceDateBackup(linesOfText, propertyLocation, 2);
                                }

                                if (invoiceDate.Equals("") || invoiceDate.Equals("Correct date not found"))
                                {
                                    invoiceDate = ScanningPDF_methods.getDueOrInvoiceDateBackup(linesOfText, propertyLocation, 1);
                                }

                            }
                            else if (i == 1) // Second page
                            {
                                thisReadingDate = ScanningPDF_methods.getReadingDates(linesOfText, "This reading");
                                lastReadingDate = ScanningPDF_methods.getReadingDates(linesOfText, "Last reading");
                                chargeDetails = ScanningPDF_methods.getChargeDetails(linesOfText, "Charge details", "Consumption details");
                                consumptionDetails = ScanningPDF_methods.getConsumptionDetails(linesOfText, "Consumption details", "Wastewater");
                                //Console.WriteLine("This reading date equals: " + thisReadingDate);
                                // Console.WriteLine("The last reading date equals: " + lastReadingDate);
                                thisReadingLine = ScanningPDF_methods.getThisOrLastReadingLine(consumptionDetails, "This reading");
                                lastReadingLine = ScanningPDF_methods.getThisOrLastReadingLine(consumptionDetails, "Last reading");
                                thisReading = ScanningPDF_methods.getReadingAmount(thisReadingLine, "This reading");
                                lastReading = ScanningPDF_methods.getReadingAmount(lastReadingLine, "Last reading");
                                thisReadingType = ScanningPDF_methods.getReadingAmountType(thisReadingLine);
                                lastReadingType = ScanningPDF_methods.getReadingAmountType(lastReadingLine);
                                wasteWaterPercentage = ScanningPDF_methods.getWasteWaterPercent(consumptionDetails);
                                string waterDetailsLine = ScanningPDF_methods.getBackUpWaterDetails(chargeDetails, "Water");
                                string wasteWaterDetailsLine = ScanningPDF_methods.getBackUpWaterDetails(chargeDetails, "Wastewater");

                                if (wasteWaterCost == 0)
                                {
                                    wasteWaterCost = ScanningPDF_methods.getWasteWaterCostBackup(linesOfText, "Fixed charges");
                                }

                                if(thisReadingDate == null)
                                {
                                    thisReadingDate = ScanningPDF_methods.backupThisReadingMethod(linesOfText, 2);
                                }

                                if (lastReadingDate == null)
                                {
                                    lastReadingDate = ScanningPDF_methods.backupThisReadingMethod(linesOfText, 1);
                                }

                                if (waterDetailsLine.Equals("error not water details"))
                                {
                                    waterRate = "Need backup method for water line";
                                }

                                if (wasteWaterDetailsLine.Equals("error not water details"))
                                {
                                    wasteWaterRate = "Need backup method for wastewater line";
                                }

                                waterRate = ScanningPDF_methods.getUnitRate(waterDetailsLine);
                                wasteWaterRate = ScanningPDF_methods.getUnitRate(wasteWaterDetailsLine);
                                waterConsumption = ScanningPDF_methods.getWaterConsumption(waterDetailsLine, "Water");
                                wasteWaterConsumption = ScanningPDF_methods.getWaterConsumption(wasteWaterDetailsLine, "Wastewater");
                                waterDetailsCost = ScanningPDF_methods.getWaterDetailsCost(waterDetailsLine);
                                wasteWaterDetailsCost = ScanningPDF_methods.getWaterDetailsCost(wasteWaterDetailsLine);

                                if (waterConsumption.Equals("Rates Revised"))
                                {
                                    waterConsumption = String.Empty;
                                    waterConsumption = ScanningPDF_methods.getOnlyWaterConsumption(consumptionDetails);
                                }

                                if (waterDetailsCost.Equals("Rates Revised"))
                                {
                                    waterDetailsCost = String.Empty;
                                    waterDetailsCost = waterFirstPageCost.ToString("0.00");
                                }

                                if (wasteWaterDetailsCost.Equals("Rates Revised"))
                                {
                                    wasteWaterDetailsCost = String.Empty;
                                    wasteWaterDetailsCost = wasteWaterFirstPageCost.ToString("0.00");
                                }

                            }
                            else // Execute if there is ever more than 2 pages within the .pdf file
                            {
                                Console.WriteLine("Program only extracts the relevant data needed if they are " +
                                                  "located on the first 2 pages on the .pdf watercare invoice.");
                                break;
                            }

                        }
                        

                        DatabaseExtraction_methods.getAdditionalDetailsV2(dbRows, accountNumber, ref receiveVia, ref payee, ref tenantName,
                        ref tenantDR, ref procedure, ref toTenantVia, ref ownerFee, ref ownerName);

                        // The bool parameter needs to updated according to procedure field from database
                        double amountToCharge = ScanningPDF_methods.calculateChargeTenantAmount(waterFirstPageCost, wasteWaterFirstPageCost, true);

                        FileStream createFile = new FileStream(fileNameReportPath + " Report.txt", FileMode.Create, FileAccess.Write);
                        StreamWriter sw = new StreamWriter(createFile);
                        sw.WriteLine("Report of Extracted data from " + fileNameNoExtension +
                                        " for the use of First Property Management");
                        sw.WriteLine("Account number is: " + accountNumber);
                        sw.WriteLine("Fixed waste water cost equals: " + wasteWaterCost.ToString("0.00"));
                        sw.WriteLine("Invoice amount (Total amount from watercare invoice) equals: " + totalCost.ToString("0.00"));
                        sw.WriteLine("Property Location is: " + propertyLocation);
                        sw.WriteLine("Account Type is: " + accountType);
                        sw.WriteLine("Invoice date is: " + invoiceDate);
                        sw.WriteLine("Due date is: " + dueDate);
                        sw.WriteLine("This reading date equals: " + thisReadingDate);
                        sw.WriteLine("The last reading date equals: " + lastReadingDate);
                        sw.WriteLine("Amount to charge tenant (Total Vol. Charges) is $" + amountToCharge.ToString("0.00"));
                        sw.WriteLine("---------------------------------------");
                        sw.WriteLine("The following text contains information from the 'Details' section");
                        sw.WriteLine("\n");
                        for (int j = 0; j < chargeDetails.Length; j++)
                        {
                            sw.WriteLine(chargeDetails[j]);
                        }

                        for (int j = 0; j < consumptionDetails.Length; j++)
                        {
                            sw.WriteLine(consumptionDetails[j]);
                        }
                        sw.WriteLine("---------------------------------------");
                        sw.WriteLine("This reading consumption amount equals: " + thisReading);
                        sw.WriteLine("This reading consumption amount is an '" + thisReadingType + "' value");
                        sw.WriteLine("The last reading consumption amount equals: " + lastReading);
                        sw.WriteLine("The last reading consumption amount is an '" + lastReadingType + "' value");
                        sw.WriteLine("The wastewater percentage equals: " + wasteWaterPercentage.ToString("0.00") + "%");
                        sw.WriteLine("The water unit rate equals: " + waterRate);
                        sw.WriteLine("The wastewater unit rate equals: " + wasteWaterRate);
                        sw.WriteLine("The water consumption equals: " + waterConsumption);
                        sw.WriteLine("The wastewater consumption equals: " + wasteWaterConsumption);
                        sw.WriteLine("The water details cost equals: " + waterDetailsCost);
                        sw.WriteLine("The wastewater details cost equals: " + wasteWaterDetailsCost);
                        sw.WriteLine("***************************************");
                        sw.WriteLine("The following contains information from the Database");
                        sw.WriteLine("\n");
                        sw.WriteLine("Received via: " + receiveVia);
                        sw.WriteLine("Who pays: " + payee);
                        sw.WriteLine("Tenant name: " + tenantName);
                        sw.WriteLine("Tenant DR: " + tenantDR);
                        sw.WriteLine("Procedure followed: " + procedure);
                        sw.WriteLine("To Tenant Via: " + toTenantVia);
                        sw.WriteLine("Owner fee: " + ownerFee);
                        sw.WriteLine("Owner name: " + ownerName);
                        sw.Close();

                        accountNumbersList.Add(accountNumber);
                        fixedWasteWaterCostsList.Add(wasteWaterCost);
                        totalCostsList.Add(totalCost);
                        propertyLocationsList.Add(propertyLocation);
                        accountTypesList.Add(accountType);
                        dueDatesList.Add(dueDate);
                        thisReadingDatesList.Add(thisReadingDate);
                        lastReadingDatesList.Add(lastReadingDate);
                        chargeTenantList.Add(amountToCharge);
                        invoiceDatesList.Add(invoiceDate);
                        thisReadingAmountList.Add(thisReading);
                        lastReadingAmountList.Add(lastReading);
                        thisReadingAmountTypeList.Add(thisReadingType);
                        lastReadingAmountTypeList.Add(lastReadingType);
                        wasteWaterPercentageList.Add(wasteWaterPercentage);
                        waterRateList.Add(waterRate);
                        wasteWaterRateList.Add(wasteWaterRate);
                        waterConsumptionList.Add(waterConsumption);
                        wasteWaterConsumptionList.Add(wasteWaterConsumption);
                        waterCostsList.Add(waterDetailsCost);
                        wasteWaterCostsList.Add(wasteWaterDetailsCost);
                        rViaList.Add(receiveVia);
                        whoPaysList.Add(payee);
                        tenantNameList.Add(tenantName);
                        tenantDRList.Add(tenantDR);
                        procedureList.Add(procedure);
                        toTenantViaList.Add(toTenantVia);
                        ownerFeeList.Add(ownerFee);
                        ownerNameList.Add(ownerName);
                    }
                }
                string excelFileReport = ExcelGeneration.generateExcelFileReport(folderpath, accountNumbersList.ToArray(), fixedWasteWaterCostsList.ToArray(), totalCostsList.ToArray(),
                    propertyLocationsList.ToArray(), accountTypesList.ToArray(), invoiceDatesList.ToArray(), dueDatesList.ToArray(), thisReadingDatesList.ToArray(), lastReadingDatesList.ToArray(),
                    chargeTenantList.ToArray(), thisReadingAmountList.ToArray(), lastReadingAmountList.ToArray(), thisReadingAmountTypeList.ToArray(), lastReadingAmountTypeList.ToArray(), wasteWaterPercentageList.ToArray(),
                    waterRateList.ToArray(), wasteWaterRateList.ToArray(), waterConsumptionList.ToArray(), wasteWaterConsumptionList.ToArray(), waterCostsList.ToArray(), wasteWaterCostsList.ToArray(), rViaList.ToArray(), whoPaysList.ToArray(),
                    tenantNameList.ToArray(), tenantDRList.ToArray(), procedureList.ToArray(), toTenantViaList.ToArray(), ownerFeeList.ToArray(), ownerNameList.ToArray());

                accountNumbersList.Clear();
                fixedWasteWaterCostsList.Clear();
                totalCostsList.Clear();
                propertyLocationsList.Clear();
                accountTypesList.Clear();
                dueDatesList.Clear();
                thisReadingDatesList.Clear();
                lastReadingDatesList.Clear();
                chargeTenantList.Clear();
                invoiceDatesList.Clear();
                thisReadingAmountList.Clear();
                lastReadingAmountList.Clear();
                thisReadingAmountTypeList.Clear();
                lastReadingAmountTypeList.Clear();
                wasteWaterPercentageList.Clear();
                waterRateList.Clear();
                wasteWaterRateList.Clear();
                waterConsumptionList.Clear();
                wasteWaterConsumptionList.Clear();
                waterCostsList.Clear();
                wasteWaterCostsList.Clear();
                rViaList.Clear();
                whoPaysList.Clear();
                tenantNameList.Clear();
                tenantDRList.Clear();
                procedureList.Clear();
                toTenantViaList.Clear();
                ownerFeeList.Clear();
                ownerNameList.Clear();

            }
            catch (IOException e)
            {
                Console.WriteLine("Sorry, there was an error.");
                Console.WriteLine(e.Message);
            }
            
        }
    }
}
