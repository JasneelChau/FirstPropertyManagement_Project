using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemIOPath = System.IO.Path;

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
                while(files.MoveNext())
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
                                dueDate = ScanningPDF_methods.getDueDate(linesOfText);
                                accountNumber = ScanningPDF_methods.getAccountNumber(linesOfText);
                                //Console.WriteLine("Account Number is: " + accountNumber);
                                //Console.WriteLine("Waste water cost equals: " + wasteWaterCost.ToString("0.00"));
                                //Console.WriteLine("Total cost equals: " + totalCost.ToString("0.00"));
                                //Console.WriteLine("Property Location is: " + propertyLocation);
                                //Console.WriteLine("Account Type is: " + accountType);
                                //Console.WriteLine("Due date is: " + dueDate);
                            }
                            else if (i == 1) // Second page
                            {
                                thisReadingDate = ScanningPDF_methods.getReadingDates(linesOfText, "This reading");
                                lastReadingDate = ScanningPDF_methods.getReadingDates(linesOfText, "Last reading");
                               //Console.WriteLine("This reading date equals: " + thisReadingDate);
                               // Console.WriteLine("The last reading date equals: " + lastReadingDate);
                            }
                            else // Execute if there is ever more than 2 pages within the .pdf file
                            {
                                Console.WriteLine("Program only extracts the relevant data needed if they are " +
                                                  "located on the first 2 pages on the .pdf watercare invoice.");
                                break;
                            }

                        }

                        // The bool parameter needs to updated according to procedure field from database
                        double amountToCharge = ScanningPDF_methods.calculateChargeTenantAmount(totalCost, wasteWaterCost, true);

                        FileStream createFile = new FileStream(fileNameReportPath + " Report.txt", FileMode.Create, FileAccess.Write);
                        StreamWriter sw = new StreamWriter(createFile);
                        sw.WriteLine("Report of Extracted data from " + fileNameNoExtension +
                                        " for the use of First Property Management");
                        sw.WriteLine("Account number is: " + accountNumber);
                        sw.WriteLine("Waste water cost equals: " + wasteWaterCost.ToString("0.00"));
                        sw.WriteLine("Total cost equals: " + totalCost.ToString("0.00"));
                        sw.WriteLine("Property Location is: " + propertyLocation);
                        sw.WriteLine("Account Type is: " + accountType);
                        sw.WriteLine("Due date is: " + dueDate);
                        sw.WriteLine("This reading date equals: " + thisReadingDate);
                        sw.WriteLine("The last reading date equals: " + lastReadingDate);
                        sw.WriteLine("Amount to charge tenant is $" + amountToCharge.ToString("0.00"));
                        sw.Close();

                    }
                }
                
            }
            catch (IOException e)
            {
                Console.WriteLine("Sorry, there was an error.");
                Console.WriteLine(e.Message);
            }
            
        }
    }
}
