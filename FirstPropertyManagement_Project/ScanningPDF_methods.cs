using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.IO;
using System.Collections;
using System.Text.RegularExpressions;
//using MySql.Data.MySqlClient;
using SystemIOPath = System.IO.Path;
using System.Globalization;

namespace FirstPropertyManagement_Project
{
    public class ScanningPDF_methods
    {
        //public static int lineCounter = 0;

        public static string ReadPdfFile(object fileName)
        {
            PdfReader reader = new PdfReader(fileName.ToString());
            string strText = null;

            for (int page = 1; page <= reader.NumberOfPages; page++) // Page has to be initialised as 1, not 0!!!
            {
                ITextExtractionStrategy its = new SimpleTextExtractionStrategy();
                string s = PdfTextExtractor.GetTextFromPage(reader, page, its);

                s = Encoding.UTF8.GetString(ASCIIEncoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(s)));

                strText = strText + s;
            }
            reader.Close();
            return strText;
        }

        public static ArrayList ReadPdfFileArrayList(object fileName) // IDK why this can't be private
        {
            PdfReader reader = new PdfReader(fileName.ToString());
            //string strText = null;
            ArrayList texts = new ArrayList();

            for (int page = 1; page <= reader.NumberOfPages; page++) // Page has to be initialised as 1, not 0!!!
            {
                ITextExtractionStrategy its = new SimpleTextExtractionStrategy();
                string s = PdfTextExtractor.GetTextFromPage(reader, page, its);

                s = Encoding.UTF8.GetString(ASCIIEncoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(s)));

                texts.Add(s);

                // string s contains all the text from a single page in the pdf file, therefore
                // every increment within this for loop, will store all the text from a single page
                // into the texts arraylist. 
            }
            reader.Close();
            return texts;
        }

        public static int getNumberOfLines(char[] characters)
        {
            int numberOfLines = 0;

            for (int i = 0; i < characters.Length; i++)
            {
                if (characters[i] == '\n')
                {
                    numberOfLines++;
                }
            }
            numberOfLines++; // Perhaps this can help take into account the very last line of text
            // in a page?
            return numberOfLines;
        }

        public static string[] getLinesOfText(char[] characters, int numberOfLines)
        {
            int lineCounter = 0; // This should not exceed the numberOfLines
            StringBuilder text = new StringBuilder();
            string[] linesOfText = new string[numberOfLines];
            for (int i = 0; i < characters.Length; i++)
            {
                if (characters[i] == '\n')
                {
                    // Once a new line character is reached, add all the characters currently in the
                    // string builder into the string array
                    //text.Append(" ..." + lineCounter);
                    linesOfText[lineCounter] = text.ToString();
                    text.Clear(); // Then clear all the characters in the string builder at this 
                                  // instant in order to gather all the characters from the next line

                    // This checks to make sure the line counter doesn't exceed the number of lines
                    // on a single page
                    if (lineCounter < numberOfLines)
                        lineCounter++;
                    else
                        break;
                }
                else
                {
                    text.Append(characters[i]); // If no new line character is detected at
                    // the current index, append the current character to the string builder
                }
            }

            // This makes sure to include the characters contained in the last line of text.
            linesOfText[lineCounter] = text.ToString();
            text.Clear();
            return linesOfText;
        }

        public static double getRelevantCharges(string[] linesOfText, string textToSearch, string overdueText)
        {
            int length = linesOfText.Length;
            ArrayList matchingLines = new ArrayList();
            // An arraylist to store the indices where the specified textToSearch is found
            // Within the pdf, there may be multiple lines where the textToSearch is found
            // hence a dynamic list needs to be maintained to know which lines to search
            // in order to find the relevant data that is needed.

            //const string wasteWaterText = "Wastewater fixed charges";
            for (int i = 0; i < length; i++)
            {
                if (linesOfText[i].Contains(textToSearch)) // If textToSearch is found in the current
                {                                          // line, then add that line number/index to the arraylist
                    matchingLines.Add(i);
                }
            }

            object[] matchingLinesObjArray = matchingLines.ToArray();
            string relevantCostStr = "";
            string lineToCheck = "";
            double relevantCost = 0;
            // Use a second for loop here to iterate through all the lines where textToSearch was found
            // in order to retrieve the relevant data needed from that specific line of text
            for (int i = 0; i < matchingLinesObjArray.Length; i++)
            {
                lineToCheck = linesOfText[Convert.ToInt32(matchingLinesObjArray[i])];
                relevantCostStr = Regex.Replace(lineToCheck, @"[^\d+|\.\-]", "").Trim();
                // This regular expression should replace every character that is NOT a decimal digit,
                // a period (.) or a dash (-) with nothing
                //lineToCheck.Substring(textToSearch.Length).Replace("$", "").Trim(); ALTERNATIVE WAY OF OBTAINING WASTEWATER COST

                // Checking to see if relevant data was actually contained in this specific line
                // of text
                if ((!relevantCostStr.Equals("")) || relevantCostStr != null)
                {
                    // Specifically, since this method is used to obtain a price/cost figure
                    // we can check to see if the data found can be converted into a double
                    // if so, then we can be more sure that we found a price/cost figure
                    if (Double.TryParse(relevantCostStr, out relevantCost))
                    {
                        relevantCost = Double.Parse(relevantCostStr);
                        matchingLines.Clear(); // Clear the arraylist to store the new indices pertaining 
                        // to the next search (i.e. searching for overdue text)
                        break; // Once correct data is found, stop searching and break out of the loop
                    }
                }
            }

            relevantCostStr = String.Empty;

            // Begin searching for overdue text and cost

            for (int i = 0; i < length; i++)
            {
                if (linesOfText[i].Contains(overdueText)) // If overdueText is found in the current
                {                                         // line, then add that line number/index to the arraylist
                    matchingLines.Add(i);
                }
            }

            matchingLinesObjArray = matchingLines.ToArray();
            double overdueCost = 0;
            for (int i = 0; i < matchingLinesObjArray.Length; i++)
            {
                lineToCheck = linesOfText[Convert.ToInt32(matchingLinesObjArray[i])];
                relevantCostStr = Regex.Replace(lineToCheck, @"[^\d+|\.\-]", "").Trim();
                // This regular expression should replace every character that is NOT a decimal digit,
                // a period (.) or a dash (-) with nothing
                //lineToCheck.Substring(textToSearch.Length).Replace("$", "").Trim(); 

                // Checking to see if relevant data was actually contained in this specific line
                // of text
                if ((!relevantCostStr.Equals("")) || relevantCostStr != null)
                {
                    // Specifically, since this method is used to obtain a price/cost figure
                    // we can check to see if the data found can be converted into a double
                    // if so, then we can be more sure that we found a price/cost figure
                    if (Double.TryParse(relevantCostStr, out overdueCost))
                    {
                        overdueCost = Double.Parse(relevantCostStr);
                        matchingLines.Clear();
                        break; // Once correct data is found, stop searching and break out of the loop
                    }
                }
            }
            relevantCost = relevantCost + overdueCost;
            return relevantCost;
        }

        public static string getRelevantTextData(string[] linesOfText, string textToSearch)
        {
            int length = linesOfText.Length;
            ArrayList matchingLines = new ArrayList();

            for (int i = 0; i < length; i++)
            {
                if (linesOfText[i].Contains(textToSearch))
                {
                    matchingLines.Add(i);
                }
            }

            object[] matchingLinesObjArray = matchingLines.ToArray();
            string relevantTextData = "";
            string lineToCheck = "";
            for (int i = 0; i < matchingLinesObjArray.Length; i++)
            {
                lineToCheck = linesOfText[Convert.ToInt32(matchingLinesObjArray[i])];
                relevantTextData = lineToCheck.Substring(textToSearch.Length).Replace(":", "").Trim();

                if (!relevantTextData.Equals(""))
                {
                    break;
                }
            }

            if (relevantTextData.Equals(""))
            {
                relevantTextData = null;
            }

            return relevantTextData;
        }

        // This getDueOrInvoiceDate function works under the assumption that the latest date found
        // on the first page in the watercare invoice .pdf, will always be considered as 
        // the due date OR if searching for the invoice date, the assumption will be that the earliest
        // date found will always be considered as the invoice date
        // When calling this method, make whichDate equal to 1 if searching for due date,
        // If searching for invoice date, make whichDate equal to 0

        public static string getDueOrInvoiceDate(string[] linesOfText, int whichDate)
        {
            int length = linesOfText.Length;
            string dateOne = "";
            string dateTwo = "";
            DateTime dateOneDT = DateTime.Today; // No purpose behind this but to simply assign a value to DateTime object
            DateTime dateTwoDT = DateTime.Today;

            for (int i = 0; i < linesOfText.Length; i++)
            {
                // This if statement checks whether the current line data does not equal nothing AND if the 
                // line data matches the following pattern: 2 decimal digits, a white space character,
                // 3 alphabetical characters (lower or upper case), a white space character and 4 decimal
                // digits. We use this pattern because the format for the date on this line is supposed
                // to be DD MMM YYYY , where DD and YYYY are numbers and MMM is text AND the length of the
                // current line is equal to 11 characters
                if ((!linesOfText[i].Equals("")) && (Regex.IsMatch(linesOfText[i], @"^\d{2}\s[a-zA-Z]{3}\s\d{4}$")) && (linesOfText[i].Length == 11))
                {
                    if (dateOne.Equals(""))
                    {
                        dateOne = linesOfText[i];
                        // Converting the date found into a DateTime object with an english NZ date/time format
                        dateOneDT = DateTime.Parse(dateOne, new CultureInfo("en-NZ", true), DateTimeStyles.AllowWhiteSpaces & DateTimeStyles.AssumeLocal);
                    }
                    else
                    {
                        dateTwo = linesOfText[i];
                        dateTwoDT = DateTime.Parse(dateTwo, new CultureInfo("en-NZ", true), DateTimeStyles.AllowWhiteSpaces & DateTimeStyles.AssumeLocal);
                    }
                }

                // If both string fields contain a date, then perform a comparison of dates
                if ((!dateOne.Equals("")) && (!dateTwo.Equals("")))
                {
                    // Searching for due date
                    if (whichDate == 1)
                    {
                        // If dateOneDT's date is later than dateTwoDT's date i.e. greater than zero
                        if (dateOneDT.CompareTo(dateTwoDT) > 0)
                        {
                            // Don't do anything because dateOneDT is later than
                            // dateTwoDT
                        }
                        else
                        {
                            dateOneDT = dateTwoDT;
                            dateOne = dateTwo;
                        }
                    }
                    else if (whichDate == 0) // Searching for invoice date
                    {
                        // If dateOneDT's date is earlier than or the same as dateTwoDT's date i.e. less than or equal to zero
                        if (dateOneDT.CompareTo(dateTwoDT) <= 0)
                        {
                            // Don't do anything because dateOneDT is earlier than
                            // or the same as dateTwoDT
                        }
                        else
                        {
                            dateOneDT = dateTwoDT;
                            dateOne = dateTwo;
                        }
                    }
                    else // Invalid whichDate parameter value
                    {
                        return "Invalid parameter";
                    }
                }
            }

            if (dateOne.Equals(""))
            {
                dateOne = "Correct date not found";
            }

            return dateOne;
        }

        public static string getReadingDates(string[] linesOfText, string textToSearch)
        {
            int length = linesOfText.Length;
            ArrayList matchingLines = new ArrayList();

            for (int i = 0; i < length; i++)
            {
                if (linesOfText[i].Contains(textToSearch))
                {
                    matchingLines.Add(i);
                }
            }

            object[] matchingLinesObjArray = matchingLines.ToArray();
            string relevantTextData = "";
            string lineToCheck = "";
            for (int i = 0; i < matchingLinesObjArray.Length; i++)
            {
                lineToCheck = linesOfText[Convert.ToInt32(matchingLinesObjArray[i])];
                relevantTextData = lineToCheck.Substring(textToSearch.Length, 10).Replace(":", "").Trim();
                // Assuming the length of the substring we need to extract the date, is '10' because
                // the date format on the pdf is as follows: DD-MMM-YY\s
                // \s is a white space character, the last index the substring falls on is excluded
                // hence length is 10, not 9

                // This if statement checks to see whether the substring gathered is not empty AND whether
                // it conforms to the following pattern: 2 decimal digits, a hyphen(-), 3 alphabetical 
                // characters (upper or lower case), a hyphen and 2 decimal digits
                if ((!relevantTextData.Equals("")) && (Regex.IsMatch(relevantTextData, @"^\d{2}\-[a-zA-Z]{3}\-\d{2}$")))
                {
                    break;
                }
            }

            if(relevantTextData.Equals("") || ((!relevantTextData.Equals("")) && (!Regex.IsMatch(relevantTextData, @"^\d{2}\-[a-zA-Z]{3}\-\d{2}$"))))
            {
                relevantTextData = null;
            }

            return relevantTextData;
        }

        public static string getAccountNumber(string[] scannedInformation)
        {
            int accountNumberLength = 10;
            int dashPosition = 7;
            string accountNo = null;
            string pattern = @"^\d{7}(-\d{2})";

            for (int i = 0; i < scannedInformation.Length; i++)
            {
                if (scannedInformation[i].Length == accountNumberLength && scannedInformation[i].IndexOf("-") == dashPosition)
                {
                    if (Regex.IsMatch(scannedInformation[i], pattern))
                    {
                        accountNo = scannedInformation[i];
                        break;
                    }
                }
            }
            if (accountNo == null)
                return accountNo;
            else
                return accountNo;
        }

        public static double calculateChargeTenantAmount(double totalCost, double fixedCost, bool procedureOne)
        {
            double chargeAmount = 0;
            if(procedureOne)
            {
                // Perform necessary steps on Palace
                chargeAmount = totalCost - fixedCost;
            }
            else
            {
                // Perform necessary steps on Palace
                chargeAmount = totalCost - fixedCost;
            }

            return chargeAmount;
        }

        // This method should only be called as a backup to getRelevantCharges method if the latter is unable
        // to retrieve the total cost from the watercare invoice, as this method is not as reliable. It 
        // obtains the total cost assuming that it will always be found in the line that is 2 indices before
        // the line which contains the propertyLocationValue

        public static double getTotalCostBackup(string[] linesOfText, string propertyLocationValue)
        {
            int propertyLocationValueLine = 0;
            int totalCostLine = 0;
            string totalCostStr;
            double totalCost = 0;

            for (int i = 0; i < linesOfText.Length; i++)
            {
                if (linesOfText[i].Equals(propertyLocationValue))
                {
                    propertyLocationValueLine = i;
                    totalCostLine = propertyLocationValueLine - 2; // Assuming the total cost is always in the line 2 indices
                                                                   // before the propertyLocationValue line
                    break;
                }
            }

            // If the assumed total cost line contains a '$' sign, we can be more sure we have found
            // a price/cost figure, then use a regular expression to replace all characters that are not
            // a decimal digit or a (.) or a (-) with nothing.
            // Then if the remaining string can be parsed into a double, we can be more sure that we have 
            // found the total cost.
            if (linesOfText[totalCostLine].Contains('$'))
            {
                totalCostStr = (Regex.Replace(linesOfText[totalCostLine], @"[^\d+|\.\-]", "").Trim());

                if (Double.TryParse(totalCostStr, out totalCost))
                {
                    totalCost = Double.Parse(totalCostStr);
                }
            }

            return totalCost;
        }

        // This method should only be called as a backup to getDueDate method if the latter is unable
        // to retrieve the due date from the watercare invoice, as this method is not as reliable. It 
        // obtains the due date assuming that it will always be found in the line that is 2 indices after
        // the line which contains the propertyLocationValue

        public static string getDueDateBackup(string[] linesOfText, string propertyLocationValue)
        {
            int propertyLocationValueLine = 0;
            int dueDateLine = 0;
            string dueDateStr = "";

            for (int i = 0; i < linesOfText.Length; i++)
            {
                if (linesOfText[i].Equals(propertyLocationValue))
                {
                    propertyLocationValueLine = i;
                    dueDateLine = propertyLocationValueLine + 2; // Assuming the due date is always in the line 2 indices
                                                                 // after the propertyLocationValue line
                    break;
                }
            }

            // This if statement checks whether the assumed due date line does not equal nothing AND if the 
            // line matches the following pattern: 2 decimal digits, a white space character,
            // 3 alphabetical characters (lower or upper case), a white space character and 4 decimal
            // digits. We use this pattern because the format for the date on this line is supposed
            // to be DD MMM YYYY , where DD and YYYY are numbers and MMM is text AND the length of the
            // current line is equal to 11 characters

            if ((!linesOfText[dueDateLine].Equals("")) && (Regex.IsMatch(linesOfText[dueDateLine], @"^\d{2}\s[a-zA-Z]{3}\s\d{4}$")) && (linesOfText[dueDateLine].Length == 11))
            {
                dueDateStr = linesOfText[dueDateLine];
            }
            else
            {
                dueDateStr = "Due date not found";
            }

            return dueDateStr;
        }

        // This is one of the methods that will be used to obtain the information from the Details section that is 
        // located on the 2nd page in the watercare invoice .pdf 
        // This method works by first searching for a line that equals chargeDetailsText (Charge details) and makes
        // that line equal to the startIndex, then searches for the line that equals consumptionDetailsText 
        // (Consumption details) and makes that line equal to the endIndex. 
        // The lines in between the startIndex and endIndex contain all the lines of text that pertain to the
        // charge details. Those lines are added to a list and then converted to a string array so that it can be
        // returned.

        public static string[] getChargeDetails(string[] linesOfText, string chargeDetailsText, string consumptionDetailsText)
        {
            int length = linesOfText.Length;
            int startIndex = 0;
            int endIndex = 0;
            List<string> chargeDetails = new List<string>();

            for (int i = 0; i < length; i++)
            {
                if (linesOfText[i].Equals(chargeDetailsText))
                {
                    startIndex = i;
                }

                if (startIndex != 0) // Precaution to make sure that the endIndex is only searched for after the startIndex
                {                   // has been found
                    if (linesOfText[i].Equals(consumptionDetailsText))
                    {
                        endIndex = i;
                        break;
                    }
                }

            }

            for (int i = startIndex; i < endIndex; i++)
            {
                chargeDetails.Add(linesOfText[i]);
            }

            return chargeDetails.ToArray();
        }

        // This is one of the methods that will be used to obtain the information from the Details section that is 
        // located on the 2nd page in the watercare invoice .pdf 
        // This method works by first searching for a line that equals consumptionDetailsText (Consumption details) and 
        // makes that line equal to the startIndex, then searches for the line that conatins consumptionDetailsText 
        // (Wastewater) and makes the line after this equal to the endIndex. 
        // The endIndexText should be equal to "Wastewater" according to the current watercare invoice .pdf format as of
        // 07/02/2017 since that is the last field of data within the consumption details
        // The lines in between the startIndex and endIndex contain all the lines of text that pertain to the
        // consumption details. Those lines are added to a list and then converted to a string array 
        // so that it can be returned.

        public static string[] getConsumptionDetails(string[] linesOfText, string consumptionDetailsText, string endIndexText)
        {
            int length = linesOfText.Length;
            int startIndex = 0;
            int endIndex = 0;
            List<string> consumptionDetails = new List<string>();

            for (int i = 0; i < length; i++)
            {
                if (linesOfText[i].Equals(consumptionDetailsText))
                {
                    startIndex = i;
                }

                if (startIndex != 0) // Precaution to make sure that the endIndex is only searched for after the startIndex
                {                    // has been found
                    if (linesOfText[i].Contains(endIndexText))
                    {
                        endIndex = i + 1;
                        break;
                    }
                }

            }

            for (int i = startIndex; i < endIndex; i++)
            {
                consumptionDetails.Add(linesOfText[i]);
            }

            return consumptionDetails.ToArray();
        }


        // This method should only be called as a backup to getRelevantCharges method if the latter is unable
        // to retrieve the fixed waste water cost from the watercare invoice, as this method is not as reliable
        // Furthurmore, this backup method should only be called when the main is extracting data from the 
        // SECOND page of the watercare invoice .pdf 
        // This method works by searching through all lines of text until one equals "Fixed charges", if found
        // then the method assumes the next line after should contain the fixed waste water cost. 

        // Example of text output from the second page from a watercare invoice .pdf
        /* ...
           ...
           Fixed Charges
           Wastewater 33 days $205.000 pa $ 18.53
           * 38 chars is the line length
           * "pa $ 18.53" - substring needed (38(length) - 8) = desired length of substring?
        */
        // In some invoices, there may be a second wastewater cost figure after the initital figure found
        // hence the precautionary step taken at the end of the for loop which prevents the program from
        // breaking out of the said for loop if there is a second wastewater cost figure
        public static double getWasteWaterCostBackup(string[] linesOfText, string fixedChargesText)
        {
            int length = linesOfText.Length;
            string relevantTextData = "";
            string lineToCheck = "";
            string relevantCostStr = "";
            int indexToCheck = 0;
            double wasteWaterCost = 0;
            double currentIndexWasteWaterCost = 0;

            for (int i = 0; i < length; i++)
            {
                // If current line equals fixedChargesText OR if indexToCheck is greater than zero (essentially meaning
                // if this if block has been entered at least once)
                if (linesOfText[i].Equals(fixedChargesText) || indexToCheck > 0)
                {
                    indexToCheck = i + 1;
                    lineToCheck = linesOfText[indexToCheck]; // Assuming that once the text "Fixed Charges" is found on
                                                             // the second page, the next line contains the waste water
                                                             // cost amount
                    if (lineToCheck.Contains("Wastewater"))
                    {
                        relevantTextData = lineToCheck.Substring(lineToCheck.Length - 8).Trim();
                        if (relevantTextData.Contains('$'))
                        {
                            relevantCostStr = Regex.Replace(relevantTextData, @"[^\d+|\.\-]", "").Trim(); // Assuming the substring follows
                                                                                                          // the above format, this regular expression can replace the '$' with nothing.

                            // Further check to make sure the remaining string left over can be converted into a double, if so,
                            // then we are more sure that we have found the fixed wastewater cost
                            if (Double.TryParse(relevantCostStr, out currentIndexWasteWaterCost))
                            {
                                currentIndexWasteWaterCost = Double.Parse(relevantCostStr);
                                wasteWaterCost = wasteWaterCost + currentIndexWasteWaterCost;
                            }
                        }
                        //break;
                    }

                    if (!linesOfText[indexToCheck + 1].Contains("Wastewater")) // This step checks to see whether
                        break;                                                 // the next line of text, after the
                                                                               // initial waste water figure was found, also contains another
                                                                               // waste water figure, if so then perform the same procedure again
                                                                               // on that line, IF NOT, then break out of the for loop
                }
            }

            return wasteWaterCost;
        }

        public static string backupThisReadingMethod(string[] pdftext, int ThisOrLast)
        {
            int whichLine = 0;
            for (int i = 0; i < pdftext.Length; i++)
            {
                if (pdftext[i].Contains("Consumption") && (pdftext[i].Substring(pdftext[i].Length - 2)).Equals("kL"))
                {
                    whichLine = i;
                }
            }
            if (whichLine == 0)
                return "Correct Reading not found";
            else
                return pdftext[whichLine - ThisOrLast].Substring(13);
        }

        public static string backupGetAccountMethod(string[] pdftext)
        {
            int whichLine = 0;
            for (int i = 0; i < pdftext.Length; i++)
            {
                if (pdftext[i].Equals("(RES)"))
                {
                    whichLine = i;
                }
            }

            if (whichLine == 0)
                return "Account Number not found";
            else
                return pdftext[whichLine - 1];
        }

        public static string backupGetAccount_usingFileName(string filename)
        {
            return filename.Substring(15, 10);
        }

        // This method will be used to obtain the "This reading" or
        // the "Last reading" lines. Pass in "This reading" or "Last
        // reading" as a parameter into textToSearch
        // Only call this method after you have called the getConsumptionDetails
        // method and it returns a string array with valid text

        public static string getThisOrLastReadingLine(string[] consumptionDetails, string textToSearch)
        {
            string lineToCheck = "";
            for (int i = 0; i < consumptionDetails.Length; i++)
            {
                if (consumptionDetails[i].Contains(textToSearch))
                {
                    lineToCheck = consumptionDetails[i];
                    break;
                }
            }

            return lineToCheck;
        }

        // This method will be used to obtain the consumption amount from "This reading" or
        // the "Last reading" lines. Only call this method once you have obtained valid text
        // after calling the getConsumptionDetails method and the getThisOrLastReadingLine method, 
        // the string array returned from the former should be passed as a parameter into the latter, then
        // the line returned from the latter method should be passed as a parameter into lineToCheck

        public static int getReadingAmount(string lineToCheck, string textToSearch)
        {
            int readingAmount = 0;
            string relevantTextData = "";
            string readingAmountStr = "";

            // lineToCheck should be equal to something like this:
            // This reading 10-Feb-2017 3633 Estimate

            // Assuming the length of the substring we need to work with, is '11' because
            // the date format on the pdf is as follows: DD-MMM-YY\s
            // \s is a white space character, the last index the substring falls on is excluded
            // hence we must make sure that starting index for the substring does not include
            // any numeric digits that are part of the reading date, hence we add '11', not '10'
            // to the length of textToSearch
            if (!lineToCheck.Equals(""))
            {
                relevantTextData = lineToCheck.Substring(textToSearch.Length + 11).Trim();

                // Using regular expression to replace all non numeric characters in the substring with
                // nothing

                readingAmountStr = Regex.Replace(relevantTextData, @"[^\d+]", "").Trim();

                if (!readingAmountStr.Equals(""))
                {
                    if (Int32.TryParse(readingAmountStr, out readingAmount))
                    {
                        readingAmount = Int32.Parse(readingAmountStr);
                    }
                }
            }

            return readingAmount;
        }

        // This method will be used to determine whether the reading amounts from "This reading" or
        // the "Last reading" lines are "Estimate" or "Actual". Only call this method once you have obtained 
        // valid text after calling the getConsumptionDetails method and the getThisOrLastReadingLine method, 
        // the string array returned from the former should be passed as a parameter into the latter, then
        // the line returned from the latter method should be passed as a parameter into lineToCheck

        public static string getReadingAmountType(string lineToCheck)
        {
            string result = "";

            if (lineToCheck.Contains("Estimate"))
                result = "Estimate";
            else if (lineToCheck.Contains("Actual"))
                result = "Actual";
            else
                result = "Not found";

            return result;
        }

        // This method is used to obtain the wastewater percentage. Make sure to call the
        // getConsumptionDetails method first before calling this method. Pass in the string
        // array returned by getConsumptionDetails as a parameter into this method here

        public static double getWasteWaterPercent(string[] consumptionDetails)
        {
            string lineToCheck = "";
            string relevantTextData = "";
            string wasteWaterPercentStr = "";
            double wasteWaterPercent = 0;

            for (int i = 0; i < consumptionDetails.Length; i++)
            {
                if (consumptionDetails[i].Contains("Wastewater") && consumptionDetails[i].Contains('@') &&
                    consumptionDetails[i].Contains('%'))
                {
                    lineToCheck = consumptionDetails[i];
                    break;
                }
            }

            // lineToCheck should look something like this:
            // Wastewater @78.50% 5.50 kL
            if (!lineToCheck.Equals(""))
            {
                relevantTextData = lineToCheck.Substring(lineToCheck.IndexOf('@'), (lineToCheck.IndexOf('%') - lineToCheck.IndexOf('@'))).Trim();
                wasteWaterPercentStr = Regex.Replace(relevantTextData, @"[^\d+|\.\-]", "");

                if (!wasteWaterPercentStr.Equals(""))
                {
                    if (Double.TryParse(wasteWaterPercentStr, out wasteWaterPercent))
                    {
                        wasteWaterPercent = Double.Parse(wasteWaterPercentStr);
                    }
                }
            }

            return wasteWaterPercent;
        }

        // This method is used to obtain the Water or WasteWater line from the Charge Details section
        // of the watercare invoice. The textToSearch parameter should be made equal to either "Water"
        // or "Wastewater", depending on which line of text you would like to obtain

        public static string getBackUpWaterDetails(string[] pdftext, string textToSearch)
        {
            string lineNeeded = "";
            for (int i = 0; i < pdftext.Length; i++)
            {
                if (pdftext[i].Contains(textToSearch) && pdftext[i].Contains("/kL") && pdftext[i].Contains("$"))
                {
                    lineNeeded = pdftext[i];

                    // This checks whether the next line of text contains another Water or Wastewater line, if so
                    // then simply return the text "Rates Revised", as this was a requirement mentioned by the client

                    if (pdftext[i + 1].Contains(textToSearch) && pdftext[i].Contains("/kL") && pdftext[i].Contains("$"))
                    {
                        lineNeeded = "Rates Revised";
                    }
                    break;
                }
            }

            if (lineNeeded.Equals(""))
                lineNeeded = "error not water details";

            return lineNeeded;
        }

        // This method is used to obtain the unit rate for the water and wastewater
        // Call this method after obtaining a string from the getBackUpWaterDetails method

        public static string getUnitRate(string waterOrWasteWaterLine)
        {
            string rate = "";
            if ((!waterOrWasteWaterLine.Equals("Rates Revised")) && (!waterOrWasteWaterLine.Equals("error not water details")))
            {
                rate = waterOrWasteWaterLine.Split('$')[1].Split('/')[0];
            }
            else if (waterOrWasteWaterLine.Equals("Rates Revised"))
            {
                rate = waterOrWasteWaterLine;
            }
            return rate;
        }

        // This method is used to obtain the water and wastewater consumption in kL
        // Call this method after obtaining a string from the getBackUpWaterDetails method

        public static string getWaterConsumption(string waterOrWasteWaterLine, string textToSearch)
        {
            string consumption = "";
            if ((!waterOrWasteWaterLine.Equals("Rates Revised")) && (!waterOrWasteWaterLine.Equals("error not water details")))
            {
                consumption = waterOrWasteWaterLine.Split("kL".ToCharArray())[0].Substring(textToSearch.Length).Trim();

                // This if statement checks whether a single water or wastewater line contains a date enclosed within brackets since
                // some pdf's do. If so, then simply use a substring to omit the date enclosed within the rounded brackets and then
                // replace the closing rounded bracket ')' with nothing

                if (consumption.Contains(")"))
                {
                    consumption = consumption.Substring(consumption.LastIndexOf(")")).Replace(")", "").Trim();
                }
            }
            else if (waterOrWasteWaterLine.Equals("Rates Revised"))
            {
                consumption = waterOrWasteWaterLine;
            }
            return consumption;
        }

        // This method is used to obtain the water and wastewater costs from the details section
        // Call this method after obtaining a string from the getBackUpWaterDetails method

        public static string getWaterDetailsCost(string waterOrWasteWaterLine)
        {
            string cost = "";
            if ((!waterOrWasteWaterLine.Equals("Rates Revised")) && (!waterOrWasteWaterLine.Equals("error not water details")))
            {
                cost = waterOrWasteWaterLine.Substring(waterOrWasteWaterLine.LastIndexOf('$')).Replace("$", "").Trim();
            }
            else if (waterOrWasteWaterLine.Equals("Rates Revised"))
            {
                cost = waterOrWasteWaterLine;
            }
            return cost;
        }

        // This method is used to obtain the total water consumption figure in kL
        // from the details section
        // Only call this method once you have called the getConsumptionDetails method
        // and have obtained its string array
        // Additionally, this method should only be used if there are multiple lines
        // for the water and wastewater fields in the details section i.e. Rates Revised

        public static string getOnlyWaterConsumption(string[] consumptionDetails)
        {
            string lineToCheck = "";
            string relevantTextData = "";
            string waterConsumptionStr = "";
            double waterConsumption = 0;
            string waterConsumptionStrFinal = "";

            for (int i = 0; i < consumptionDetails.Length; i++)
            {
                if ((consumptionDetails[i].Contains("Consumption")) && (consumptionDetails[i].Contains("kL")))
                {
                    lineToCheck = consumptionDetails[i];
                    break;
                }
            }

            if (!lineToCheck.Equals(""))
            {
                relevantTextData = lineToCheck.Substring("Consumption".Length, (lineToCheck.IndexOf("kL") - "Consumption".Length)).Trim();
                waterConsumptionStr = Regex.Replace(relevantTextData, @"[^\d+|\.\-]", "");

                if (!waterConsumptionStr.Equals(""))
                {
                    if (Double.TryParse(waterConsumptionStr, out waterConsumption))
                    {
                        waterConsumption = Double.Parse(waterConsumptionStr);
                        waterConsumptionStrFinal = waterConsumption.ToString("0.00");
                    }
                }
            }
            return waterConsumptionStrFinal;
        }
    }


}
