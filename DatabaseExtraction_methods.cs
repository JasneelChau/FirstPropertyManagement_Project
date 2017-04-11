/*
    Copyright (C) <2017>  <Sherwin Bayer, Jasneel Chauhan, Heemesh Bhikha, Melvin Mathew> 
    This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
 
    This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Affero General Public License for more details.
    You should have received a copy of the GNU Affero General Public License along with this program.  If not, see <http://www.gnu.org/licenses/>. 

    This program also makes use of the EPPlus API. This library is unmodified, the program simply makes use of its API: you can redistribute it and/or modify it under the terms of the GNU Library General Public License (LGPL) Version 2.1, February 1999.
    You should have received a copy of the GNU Library General Public License along with this program.  If not, see <http://epplus.codeplex.com/license/>. 

*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;
using Gtk;

namespace FirstPropertyManagementWB
{

    class DatabaseExtraction_methods
    {
        private static String connString = "SERVER= ;PORT=;DATABASE=;UID=;PASSWORD=";
        private static MySqlConnection connection = new MySqlConnection(connString);

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
					
			MessageDialog md = new MessageDialog(null, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, "Sorry an error occurred: " + ex.Message);
			md.Title = "Error";
			md.Icon = global::Stetic.IconLoader.LoadIcon (md, "gtk-dialog-error", global::Gtk.IconSize.LargeToolbar);
			md.WindowPosition = ((global::Gtk.WindowPosition)(1));
			md.Run();
			md.Destroy();

                }
            }
        }

        public static DataRow[] getDatabaseRows()
        {
            DataRow[] rows = null;
            try
            {
                connection.Open();
                String tableQueryCommand = @"SELECT ACCOUNT_ID, R_VIA, PAYEE, TENANT_NAME, 
                                             TENANT_DR, PROCEDURES, TO_TNT_VIA, 
                                             OWNER_FEE, OWNER_NAME
                                             FROM FIRST_PROPERTY_DATABASE;";

                using (MySqlCommand cmd = new MySqlCommand())
                using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
                {
                    cmd.CommandText = tableQueryCommand;
                    cmd.Connection = connection;
                    cmd.ExecuteNonQuery();

                    DataTable table = new DataTable();
                    da.Fill(table);
                    rows = table.Select();
                }
                connection.Close();
            }
            catch (Exception ex)
            {
		connection.Close ();		
		MessageDialog md = new MessageDialog(null, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, "Sorry an error occurred: " + ex.Message);
		md.Title = "Error";
		md.Icon = global::Stetic.IconLoader.LoadIcon (md, "gtk-dialog-error", global::Gtk.IconSize.LargeToolbar);
		md.WindowPosition = ((global::Gtk.WindowPosition)(1));
		md.Run();
		md.Destroy();
	    }
            return rows;
        }

        public static void getAdditionalDetailsV2(DataRow[] rows, string accountNumber, ref string r_Via, ref string payee, ref string tenantName,
            ref string tenant_Dr, ref string procedure, ref string to_Tnt_Via, ref string ownerFee, ref string ownerName)
        {
            
            for (int i = 0; i < rows.Length; i++)
            {
                if(rows[i][0].Equals(accountNumber))
                {
                    r_Via = rows[i][1].ToString();
                    payee = rows[i][2].ToString();
                    tenantName = rows[i][3].ToString();
                    tenant_Dr = rows[i][4].ToString();
                    procedure = rows[i][5].ToString();
                    to_Tnt_Via = rows[i][6].ToString();
                    ownerFee = rows[i][7].ToString();
                    ownerName = rows[i][8].ToString();

                    break;
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
                
      }
}

