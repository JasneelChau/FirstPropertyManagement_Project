﻿using System;
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
                else
                {
                    relevantTextData = textToSearch + " not found";
                }
            }
            return relevantTextData;
        }

        // This getDueDate function works under the assumption that the latest date found
        // on the first page in the watercare invoice .pdf, will always be considered as 
        // the due date

        public static string getDueDate(string[] linesOfText)
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
                else
                {
                    relevantTextData = textToSearch + " date not found";
                }
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
                return "Error, no Account Number";
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
    }
}
