using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace FirstPropertyManagement_Project
{

    class DatabaseExtraction_methods
    {
        private static String connString = "SERVER= 31.220.105.200;PORT=3306;DATABASE=redflixc_FPM;UID=redflixc_heemesh;PASSWORD=heemesh2017";
        private static MySqlConnection connection = new MySqlConnection(connString);

        // This is really slow, maybe don't use ref variables and just return a string array?

        public static void getAdditionalDetails(string accountNumber, ref string r_Via, ref string payee, ref string tenantName,
            ref string tenant_Dr, ref string procedure, ref string to_Tnt_Via, ref string ownerFee, ref string ownerName)
        {
            if ((accountNumber != null) && ((!accountNumber.Equals("")) || (!accountNumber.Equals("Account Number not found"))))
            {
                try
                {
                    connection.Open();
                    String searchQueryCommand = @"SELECT R_VIA, PAYEE, TENANT_NAME, 
                                             TENANT_DR, PROCEDURES, TO_TNT_VIA, 
                                             OWNER_FEE, OWNER_NAME 
                                             FROM FIRST_PROPERTY_DATABASE 
                                             WHERE ACCOUNT_ID = @ACCOUNT_ID;";
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.CommandText = searchQueryCommand;
                        cmd.Connection = connection;
                        cmd.Parameters.AddWithValue("@ACCOUNT_ID", accountNumber);
                        cmd.ExecuteNonQuery();

                        using (MySqlDataReader myReader = cmd.ExecuteReader())
                        {

                            if (myReader.HasRows)
                            {
                                while (myReader.Read())
                                {
                                    r_Via = myReader.GetString("R_VIA");
                                    payee = myReader.GetString("PAYEE");
                                    tenantName = myReader.GetString("TENANT_NAME");
                                    tenant_Dr = myReader.GetString("TENANT_DR");
                                    procedure = myReader.GetString("PROCEDURES");
                                    to_Tnt_Via = myReader.GetString("TO_TNT_VIA");
                                    ownerFee = myReader.GetString("OWNER_FEE");
                                    ownerName = myReader.GetString("OWNER_NAME");
                                    /*for (int i = 0; i < myReader.FieldCount; i++)
                                    {
                                        accountDetails[i] = myReader[i].ToString();
                                    }*/
                                }
                            }
                            else
                            {
                                r_Via = "No matching account number";
                                payee = "No matching account number";
                                tenantName = "No matching account number";
                                tenant_Dr = "No matching account number";
                                procedure = "No matching account number";
                                to_Tnt_Via = "No matching account number";
                                ownerFee = "No matching account number";
                                ownerName = "No matching account number";
                            }
                        }
                    }
                    connection.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    //MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
