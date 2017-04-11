/*
    Copyright (C) <2017>  <Sherwin Bayer, Jasneel Chauhan, Heemesh Bhikha, Melvin Mathew> 
    This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
 
    This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Affero General Public License for more details.
    You should have received a copy of the GNU Affero General Public License along with this program.  If not, see <http://www.gnu.org/licenses/>. 

    This program also makes use of the EPPlus API. This library is unmodified, the program simply makes use of its API: you can redistribute it and/or modify it under the terms of the GNU Library General Public License (LGPL) Version 2.1, February 1999.
    You should have received a copy of the GNU Library General Public License along with this program.  If not, see <http://epplus.codeplex.com/license/>. 

*/
using System;
using Gtk;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Data.Sql;
using GLib;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;

public partial class MainWindow : Gtk.Window
{
	static String connString = "SERVER= ;PORT=;DATABASE=;UID=;PASSWORD=";
	MySqlConnection connection = new MySqlConnection(connString);
	String getAccountFromDataGrid;
	private ListStore mListStore2 = new ListStore (typeof (string), typeof (string), typeof (string), typeof (string), typeof (string)
										 , typeof (string), typeof (string), typeof (string), typeof (string), typeof (string)
											, typeof (string), typeof (string));

	public MainWindow() : base(Gtk.WindowType.Toplevel)
	{
		Build();
		getListStore ();
		updateTableV2 (display_view);
		updateTableV2 (editView); 
	}

	protected void OnDeleteEvent(object sender, DeleteEventArgs a)
	{
		DeleteEvent += delegate {
			Gtk.Application.Quit ();
		};
		//Gtk.Application.Quit();
		//a.RetVal = true;
	}

	protected void refreshTable_button(object sender, EventArgs e)
	{
		getListStore ();
		updateTableV2 (display_view);
	}

	protected void add_User_button(object sender, EventArgs e)
	{
		Boolean fullAccountID = addAccountID_entry.Text.Length > 0;
		Boolean fullTenantAddress = addTenantAddress_entry.Text.Length > 0;
		Boolean fullTenantName = addTenantName_entry.Text.Length > 0;

		if (fullAccountID && fullTenantAddress && fullTenantName) 
		{
			try 
			{
				connection.Open ();
				String addUserQuery = @"INSERT INTO FIRST_PROPERTY_DATABASE 
	                                           (ACCOUNT_ID, TENANT_ADDRESS, R_VIA, PAYEE, TENANT_NAME, 
	                                           APX_DT, TENANT_DR, PROCEDURES, TO_TNT_VIA, OWNER_FEE, 
	                                           OWNER_NAME, ACCOUNT_TYPE) 
	                                           VALUES 
	                                           (@ACCOUNT_ID, @TENANT_ADDRESS, @R_VIA, @PAYEE, @TENANT_NAME, 
	                                           @APX_DT, @TENANT_DR, @PROCEDURES, @TO_TNT_VIA, @OWNER_FEE, 
	                                           @OWNER_NAME, @ACCOUNT_TYPE);";
				using (MySqlCommand cmd = new MySqlCommand ()) 
				{
					cmd.CommandText = addUserQuery;
					cmd.Connection = connection;
					cmd.Parameters.AddWithValue ("@ACCOUNT_ID", addAccountID_entry.Text);
					cmd.Parameters.AddWithValue ("@TENANT_ADDRESS", addTenantAddress_entry.Text);
					cmd.Parameters.AddWithValue ("@R_VIA", addRVia_entry.Text);
					cmd.Parameters.AddWithValue ("@PAYEE", addPayee_entry.Text);
					cmd.Parameters.AddWithValue ("@TENANT_NAME", addTenantName_entry.Text);
					cmd.Parameters.AddWithValue ("@APX_DT", addApxDT_entry.Text);
					cmd.Parameters.AddWithValue ("@TENANT_DR", addTenantDr_entry.Text);
					cmd.Parameters.AddWithValue ("@PROCEDURES", addProcedures_entry.Text);
					cmd.Parameters.AddWithValue ("@TO_TNT_VIA", addToTntVia_entry.Text);
					cmd.Parameters.AddWithValue ("@OWNER_FEE", addOwnerFee_entry.Text);
					cmd.Parameters.AddWithValue ("@OWNER_NAME", addOwnerName_entry.Text);
					cmd.Parameters.AddWithValue ("@ACCOUNT_TYPE", addAccountType_entry.Text);
					cmd.ExecuteNonQuery ();
				}
				connection.Close ();

				userAdded_label.Visible = true;
				addAccountID_entry.Text = "";
				addTenantAddress_entry.Text = "";
				addRVia_entry.Text = "";
				addPayee_entry.Text = "";
				addTenantName_entry.Text = "";
				addApxDT_entry.Text = "";
				addTenantDr_entry.Text = "";
				addProcedures_entry.Text = "";
				addToTntVia_entry.Text = "";
				addOwnerFee_entry.Text = "";
				addOwnerName_entry.Text = "";
				addAccountType_entry.Text = "";
				getListStore ();

			} 
			catch (Exception ex) 
			{
				connection.Close ();
				MessageDialog md = new MessageDialog (null, DialogFlags.Modal, MessageType.Error
								 , ButtonsType.Ok, "Error: " + ex.Message);
				md.Title = "Error";
				md.Icon = global::Stetic.IconLoader.LoadIcon (this, "gtk-dialog-error", global::Gtk.IconSize.LargeToolbar);
				md.WindowPosition = ((global::Gtk.WindowPosition)(1));
				md.Run ();
				md.Destroy ();

			}
		} 
		else 
		{
			MessageDialog md = new MessageDialog (null, DialogFlags.Modal, MessageType.Error
							 , ButtonsType.Ok, "Please fill in the required fields");
			md.Title = "Error";
			md.Icon = global::Stetic.IconLoader.LoadIcon (this, "gtk-dialog-error", global::Gtk.IconSize.LargeToolbar);
			md.WindowPosition = ((global::Gtk.WindowPosition)(1));
			md.Run ();
			md.Destroy ();
		}
	}


	protected void delete_User_button(object sender, EventArgs e)
	{
		Boolean fullAccountID = deleteAccountID_entry.Text.Length > 0;

		if (fullAccountID) 
		{
			try 
			{
				connection.Open ();
				String deleteUserQuery = @"DELETE FROM FIRST_PROPERTY_DATABASE 
		                                       WHERE 
		                                       ACCOUNT_ID=@ACCOUNT_ID;";
				MessageDialog result = new MessageDialog (null, DialogFlags.Modal, MessageType.Question, ButtonsType.OkCancel,
														 "Delete Account Number: " + deleteAccountID_entry.Text);
				result.Title = "Confirmation";
				result.Icon = global::Stetic.IconLoader.LoadIcon (this, "gtk-dialog-warning", global::Gtk.IconSize.LargeToolbar);
				result.WindowPosition = ((global::Gtk.WindowPosition)(1));
				result.Response += delegate (object o, ResponseArgs respargs) 
				{
					if (respargs.ResponseId == ResponseType.Ok) 
					{
						using (MySqlCommand cmd = new MySqlCommand ()) 
						{
							cmd.CommandText = deleteUserQuery;
							cmd.Connection = connection;
							cmd.Parameters.AddWithValue ("ACCOUNT_ID", deleteAccountID_entry.Text);
							cmd.ExecuteNonQuery ();
						}

						userDeleted_label.Visible = true;
						deleteAccountID_entry.Text = String.Empty;
					}
					if (respargs.ResponseId == ResponseType.Cancel) 
					{
					}
				};
				result.Run ();
				result.Destroy ();


				connection.Close ();
				getListStore ();
			} 
			catch (Exception ex) 
			{
				connection.Close ();
				MessageDialog md = new MessageDialog (null, DialogFlags.Modal, MessageType.Error
								 , ButtonsType.Ok, "Error: " + ex.Message);
				md.Title = "Error";
				md.Icon = global::Stetic.IconLoader.LoadIcon (this, "gtk-dialog-error", global::Gtk.IconSize.LargeToolbar);
				md.WindowPosition = ((global::Gtk.WindowPosition)(1));
				md.Run ();
				md.Destroy ();
			}
		} 
		else
		{
			MessageDialog md = new MessageDialog(null, DialogFlags.Modal, MessageType.Error
								 , ButtonsType.Ok, "Please enter a valid account number");
			md.Title = "Error";
			md.Icon = global::Stetic.IconLoader.LoadIcon (this, "gtk-dialog-error", global::Gtk.IconSize.LargeToolbar);
			md.WindowPosition = ((global::Gtk.WindowPosition)(1));
			md.Run ();
			md.Destroy ();

		}
	}


	protected void edit_refresh_table(object sender, EventArgs e)
	{
		getListStore();
		updateTableV2(editView);
	}

	protected void change_user_clicked(object sender, EventArgs e)
	{
		try
		{
			if (getAccountFromDataGrid != null) {

				connection.Open ();
				String editUserQuery = @"UPDATE FIRST_PROPERTY_DATABASE                 
	                                        SET ACCOUNT_ID = @ACCOUNT_ID,
	                                        TENANT_ADDRESS = @TENANT_ADDRESS,
	                                        R_VIA = @R_VIA,
	                                        PAYEE = @PAYEE,
	                                        TENANT_NAME = @TENANT_NAME,
	                                        APX_DT = @APX_DT, 
	                                        TENANT_DR = @TENANT_DR,
	                                        PROCEDURES = @PROCEDURES,
	                                        TO_TNT_VIA = @TO_TNT_VIA,
	                                        OWNER_FEE = @OWNER_FEE,
	                                        OWNER_NAME = @OWNER_NAME,
	                                        ACCOUNT_TYPE = @ACCOUNT_TYPE
	                                        WHERE 
	                                        ACCOUNT_ID = @ACCOUNT_ID_KEY;";
				using (MySqlCommand cmd = new MySqlCommand ()) {
					cmd.CommandText = editUserQuery;
					cmd.Connection = connection;
					cmd.Parameters.AddWithValue ("@ACCOUNT_ID", editAccountID_entry.Text);
					cmd.Parameters.AddWithValue ("@TENANT_ADDRESS", editTenantAddress_entry.Text);
					cmd.Parameters.AddWithValue ("@R_VIA", editRVia_entry.Text);
					cmd.Parameters.AddWithValue ("@PAYEE", editPayee_entry.Text);
					cmd.Parameters.AddWithValue ("@TENANT_NAME", editTenantName_entry.Text);
					cmd.Parameters.AddWithValue ("@APX_DT", editApxDT_entry.Text);
					cmd.Parameters.AddWithValue ("@TENANT_DR", editTenantDr_entry.Text);
					cmd.Parameters.AddWithValue ("@PROCEDURES", editProcedures_entry.Text);
					cmd.Parameters.AddWithValue ("@TO_TNT_VIA", editToTntVia_entry.Text);
					cmd.Parameters.AddWithValue ("@OWNER_FEE", editOwnerFee_entry.Text);
					cmd.Parameters.AddWithValue ("@OWNER_NAME", editOwnerName_entry.Text);
					cmd.Parameters.AddWithValue ("@ACCOUNT_TYPE", editAccountType_entry.Text);
					cmd.Parameters.AddWithValue ("@ACCOUNT_ID_KEY", getAccountFromDataGrid);
					cmd.ExecuteNonQuery ();
				}
				userChanged_label.Visible = true;
			} 
			else 
			{
				MessageDialog md = new MessageDialog (null, DialogFlags.Modal, MessageType.Error
							 , ButtonsType.Ok, "Please select the user to change");
				md.Title = "Error";
				md.Icon = global::Stetic.IconLoader.LoadIcon (this, "gtk-dialog-error", global::Gtk.IconSize.LargeToolbar);
				md.WindowPosition = ((global::Gtk.WindowPosition)(1));
				md.Run ();
				md.Destroy ();
			}

			editAccountID_entry.Text = "";
			editTenantAddress_entry.Text = "";
			editRVia_entry.Text = "";
			editPayee_entry.Text = "";
			editTenantName_entry.Text = "";
			editApxDT_entry.Text = "";
			editTenantDr_entry.Text = "";
			editProcedures_entry.Text = "";
			editToTntVia_entry.Text = "";
			editOwnerFee_entry.Text = "";
			editOwnerName_entry.Text = "";
			editAccountType_entry.Text = "";
			//
			getAccountFromDataGrid = null;

			connection.Close();
			getListStore ();
			updateTableV2 (editView);

		}
		catch (Exception ex)
		{
			connection.Close ();
			MessageDialog md = new MessageDialog(null, DialogFlags.Modal, MessageType.Error
		                                         , ButtonsType.Ok, "Error: " + ex.Message);
			md.Title = "Error";
			md.Icon = global::Stetic.IconLoader.LoadIcon (this, "gtk-dialog-error", global::Gtk.IconSize.LargeToolbar);
			md.WindowPosition = ((global::Gtk.WindowPosition)(1));
			md.Run();
			md.Destroy();
			
		}
	}

	protected void Search_button_clicked(object sender, EventArgs e)
	{
		Boolean fullAccountID = searchAccountID_entry.Text.Length > 0;

		if (fullAccountID) 
		{
			foreach (TreeViewColumn col in searchView.Columns) 
			{
				searchView.RemoveColumn (col);
			}

			TreeViewColumn accountIDColumn = new TreeViewColumn ();
			accountIDColumn.Title = "Account ID";
			TreeViewColumn tenantAddressColumn = new TreeViewColumn ();
			tenantAddressColumn.Title = "Tenant Address";
			TreeViewColumn rViaColumn = new TreeViewColumn ();
			rViaColumn.Title = "R Via";
			TreeViewColumn payeeColumn = new TreeViewColumn ();
			payeeColumn.Title = "Payee";
			TreeViewColumn tenantNameColumn = new TreeViewColumn ();
			tenantNameColumn.Title = "Tenant Name";
			TreeViewColumn apxDTColumn = new TreeViewColumn ();
			apxDTColumn.Title = "APX DT";
			TreeViewColumn tenantDRColumn = new TreeViewColumn ();
			tenantDRColumn.Title = "Tenant DR";
			TreeViewColumn proceduresColumn = new TreeViewColumn ();
			proceduresColumn.Title = "Procedures";
			TreeViewColumn toTntViaColumn = new TreeViewColumn ();
			toTntViaColumn.Title = "To Tnt Via";
			TreeViewColumn ownerFeeColumn = new TreeViewColumn ();
			ownerFeeColumn.Title = "Owner Fee";
			TreeViewColumn ownerNameColumn = new TreeViewColumn ();
			ownerNameColumn.Title = "Owner Name";
			TreeViewColumn accountTypeColumn = new TreeViewColumn ();
			accountTypeColumn.Title = "Account Type";

			searchView.AppendColumn (accountIDColumn);
			searchView.AppendColumn (tenantAddressColumn);
			searchView.AppendColumn (rViaColumn);
			searchView.AppendColumn (payeeColumn);
			searchView.AppendColumn (tenantNameColumn);
			searchView.AppendColumn (apxDTColumn);
			searchView.AppendColumn (tenantDRColumn);
			searchView.AppendColumn (proceduresColumn);
			searchView.AppendColumn (toTntViaColumn);
			searchView.AppendColumn (ownerFeeColumn);
			searchView.AppendColumn (ownerNameColumn);
			searchView.AppendColumn (accountTypeColumn);

			ListStore mListStore = new ListStore (typeof (string), typeof (string), typeof (string), typeof (string), typeof (string)
												 , typeof (string), typeof (string), typeof (string), typeof (string), typeof (string)
													, typeof (string), typeof (string));

			searchView.Model = mListStore;
			try 
			{
				connection.Open ();
				String searchQueryCommand = @"SELECT ACCOUNT_ID, TENANT_ADDRESS,
	                                             R_VIA, PAYEE, TENANT_NAME, APX_DT, 
	                                             TENANT_DR, PROCEDURES, TO_TNT_VIA, 
	                                             OWNER_FEE, OWNER_NAME, ACCOUNT_TYPE
	                                             FROM FIRST_PROPERTY_DATABASE 
	                                             WHERE ACCOUNT_ID = @ACCOUNT_ID;";
				using (MySqlCommand cmd = new MySqlCommand ()) 
				{
					cmd.CommandText = searchQueryCommand;
					cmd.Connection = connection;
					cmd.Parameters.AddWithValue ("@ACCOUNT_ID", searchAccountID_entry.Text);
					cmd.ExecuteNonQuery ();
					using (MySqlDataReader reader = cmd.ExecuteReader ()) 
					{
						while (reader.Read ()) 
						{
							mListStore.AppendValues (reader.GetString (0), reader.GetString (1), reader.GetString (2), reader.GetString (3),
													reader.GetString (4), reader.GetString (5), reader.GetString (6), reader.GetString (7),
													reader.GetString (8), reader.GetString (9), reader.GetString (10), reader.GetString (11));
						}
					}
					connection.Close ();
				}

				searchAccountID_entry.Text = "";
			} 
			catch (Exception ex) 
			{
				connection.Close ();
				MessageDialog md = new MessageDialog (null, DialogFlags.Modal, MessageType.Error
								 , ButtonsType.Ok, "Error: " + ex.Message);
				md.Title = "Error";
				md.Icon = global::Stetic.IconLoader.LoadIcon (this, "gtk-dialog-error", global::Gtk.IconSize.LargeToolbar);
				md.WindowPosition = ((global::Gtk.WindowPosition)(1));
				md.Run ();
				md.Destroy ();

			}

			CellRendererText accountIDCell = new CellRendererText ();
			accountIDColumn.PackStart (accountIDCell, true);
			CellRendererText tenantAddressCell = new CellRendererText ();
			tenantAddressColumn.PackStart (tenantAddressCell, true);
			CellRendererText rViaCell = new CellRendererText ();
			rViaColumn.PackStart (rViaCell, true);
			CellRendererText payeeCell = new CellRendererText ();
			payeeColumn.PackStart (payeeCell, true);
			CellRendererText vCell = new CellRendererText ();
			tenantNameColumn.PackStart (vCell, true);
			CellRendererText apxDTCell = new CellRendererText ();
			apxDTColumn.PackStart (apxDTCell, true);
			CellRendererText tenantDRCell = new CellRendererText ();
			tenantDRColumn.PackStart (tenantDRCell, true);
			CellRendererText proceduresCell = new CellRendererText ();
			proceduresColumn.PackStart (proceduresCell, true);
			CellRendererText toTntViaCell = new CellRendererText ();
			toTntViaColumn.PackStart (toTntViaCell, true);
			CellRendererText ownerFeeCell = new CellRendererText ();
			ownerFeeColumn.PackStart (ownerFeeCell, true);
			CellRendererText ownerNameCell = new CellRendererText ();
			ownerNameColumn.PackStart (ownerNameCell, true);
			CellRendererText accountTypeCell = new CellRendererText ();
			accountTypeColumn.PackStart (accountTypeCell, true);

			accountIDColumn.AddAttribute (accountIDCell, "text", 0);
			tenantAddressColumn.AddAttribute (tenantAddressCell, "text", 1);
			rViaColumn.AddAttribute (rViaCell, "text", 2);
			payeeColumn.AddAttribute (payeeCell, "text", 3);
			tenantNameColumn.AddAttribute (vCell, "text", 4);
			apxDTColumn.AddAttribute (apxDTCell, "text", 5);
			tenantDRColumn.AddAttribute (tenantDRCell, "text", 6);
			proceduresColumn.AddAttribute (proceduresCell, "text", 7);
			toTntViaColumn.AddAttribute (toTntViaCell, "text", 8);
			ownerFeeColumn.AddAttribute (ownerFeeCell, "text", 9);
			ownerNameColumn.AddAttribute (ownerNameCell, "text", 10);
			accountTypeColumn.AddAttribute (accountTypeCell, "text", 11);
		}
		else
		{
			MessageDialog md = new MessageDialog (null, DialogFlags.Modal, MessageType.Error
								 , ButtonsType.Ok, "Please enter a valid account number");
			md.Title = "Error";
			md.Icon = global::Stetic.IconLoader.LoadIcon (this, "gtk-dialog-error", global::Gtk.IconSize.LargeToolbar);
			md.WindowPosition = ((global::Gtk.WindowPosition)(1));
			md.Run ();
			md.Destroy ();
		}
	}

	protected void Rowclicks(object o, RowActivatedArgs args)
	{
		int i = 0;
		for (i = 0; i < 13; i++)
		{
			var model = editView.Model;
			TreeIter iter;
			model.GetIter(out iter, args.Path);
			var value = model.GetValue(iter, i);

			if (i == 0)
			{
				editAccountID_entry.Text = value.ToString();
				getAccountFromDataGrid = editAccountID_entry.Text;
			}
			
			else if (i == 1)
				editTenantAddress_entry.Text = value.ToString();
			else if (i == 2)
				editRVia_entry.Text = value.ToString();
			else if (i == 3)
				editPayee_entry.Text = value.ToString();
			else if (i == 4)
				editTenantName_entry.Text = value.ToString();
			else if (i == 5)
				editApxDT_entry.Text = value.ToString();
			else if (i == 6)
				editTenantDr_entry.Text = value.ToString();
			else if (i == 7)
				editProcedures_entry.Text = value.ToString();
			else if (i == 8)
				editToTntVia_entry.Text = value.ToString();
			else if (i == 9)
				editOwnerFee_entry.Text = value.ToString();
			else if (i == 10)
				editOwnerName_entry.Text = value.ToString();
			else if (i == 11)
				editAccountType_entry.Text = value.ToString();
		}
	}

	public void getListStore()
	{
		mListStore2.Clear ();

		try {
			connection.Open ();

			String tableQueryCommand = "SELECT * FROM FIRST_PROPERTY_DATABASE WHERE 1;";
			using (MySqlCommand cmd = new MySqlCommand ()) {
				cmd.CommandText = tableQueryCommand;
				cmd.Connection = connection;
				cmd.ExecuteNonQuery ();

				using (MySqlDataReader reader = cmd.ExecuteReader ()) {
					while (reader.Read ()) {
						mListStore2.AppendValues (reader.GetString (0), reader.GetString (1), reader.GetString (2), reader.GetString (3),
												reader.GetString (4), reader.GetString (5), reader.GetString (6), reader.GetString (7),
												reader.GetString (8), reader.GetString (9), reader.GetString (10), reader.GetString (11));
					}
				}
			}
			connection.Close ();
		} catch (Exception ex) {
			connection.Close ();
			MessageDialog md = new MessageDialog (null, DialogFlags.Modal, MessageType.Error
							 , ButtonsType.Ok, "Error: " + ex.Message);
			md.Title = "Error";
			md.Icon = global::Stetic.IconLoader.LoadIcon (this, "gtk-dialog-error", global::Gtk.IconSize.LargeToolbar);
			md.WindowPosition = ((global::Gtk.WindowPosition)(1));
			md.Run ();
			md.Destroy ();
		}

	}

	public void updateTableV2(Gtk.TreeView table)
	{
		foreach (TreeViewColumn col in table.Columns) 
		{
			table.RemoveColumn (col);
		}

		TreeViewColumn accountIDColumn = new TreeViewColumn ();
		accountIDColumn.Title = "Account ID";
		TreeViewColumn tenantAddressColumn = new TreeViewColumn ();
		tenantAddressColumn.Title = "Tenant Address";
		TreeViewColumn rViaColumn = new TreeViewColumn ();
		rViaColumn.Title = "R Via";
		TreeViewColumn payeeColumn = new TreeViewColumn ();
		payeeColumn.Title = "Payee";
		TreeViewColumn tenantNameColumn = new TreeViewColumn ();
		tenantNameColumn.Title = "Tenant Name";
		TreeViewColumn apxDTColumn = new TreeViewColumn ();
		apxDTColumn.Title = "APX DT";
		TreeViewColumn tenantDRColumn = new TreeViewColumn ();
		tenantDRColumn.Title = "Tenant DR";
		TreeViewColumn proceduresColumn = new TreeViewColumn ();
		proceduresColumn.Title = "Procedures";
		TreeViewColumn toTntViaColumn = new TreeViewColumn ();
		toTntViaColumn.Title = "To Tnt Via";
		TreeViewColumn ownerFeeColumn = new TreeViewColumn ();
		ownerFeeColumn.Title = "Owner Fee";
		TreeViewColumn ownerNameColumn = new TreeViewColumn ();
		ownerNameColumn.Title = "Owner Name";
		TreeViewColumn accountTypeColumn = new TreeViewColumn ();
		accountTypeColumn.Title = "Account Type";

		table.AppendColumn (accountIDColumn);
		table.AppendColumn (tenantAddressColumn);
		table.AppendColumn (rViaColumn);
		table.AppendColumn (payeeColumn);
		table.AppendColumn (tenantNameColumn);
		table.AppendColumn (apxDTColumn);
		table.AppendColumn (tenantDRColumn);
		table.AppendColumn (proceduresColumn);
		table.AppendColumn (toTntViaColumn);
		table.AppendColumn (ownerFeeColumn);
		table.AppendColumn (ownerNameColumn);
		table.AppendColumn (accountTypeColumn);

		table.Model = mListStore2;

		CellRendererText accountIDCell = new CellRendererText ();
		accountIDColumn.PackStart (accountIDCell, true);
		CellRendererText tenantAddressCell = new CellRendererText ();
		tenantAddressColumn.PackStart (tenantAddressCell, true);
		CellRendererText rViaCell = new CellRendererText ();
		rViaColumn.PackStart (rViaCell, true);
		CellRendererText payeeCell = new CellRendererText ();
		payeeColumn.PackStart (payeeCell, true);
		CellRendererText vCell = new CellRendererText ();
		tenantNameColumn.PackStart (vCell, true);
		CellRendererText apxDTCell = new CellRendererText ();
		apxDTColumn.PackStart (apxDTCell, true);
		CellRendererText tenantDRCell = new CellRendererText ();
		tenantDRColumn.PackStart (tenantDRCell, true);
		CellRendererText proceduresCell = new CellRendererText ();
		proceduresColumn.PackStart (proceduresCell, true);
		CellRendererText toTntViaCell = new CellRendererText ();
		toTntViaColumn.PackStart (toTntViaCell, true);
		CellRendererText ownerFeeCell = new CellRendererText ();
		ownerFeeColumn.PackStart (ownerFeeCell, true);
		CellRendererText ownerNameCell = new CellRendererText ();
		ownerNameColumn.PackStart (ownerNameCell, true);
		CellRendererText accountTypeCell = new CellRendererText ();
		accountTypeColumn.PackStart (accountTypeCell, true);

		accountIDColumn.AddAttribute (accountIDCell, "text", 0);
		tenantAddressColumn.AddAttribute (tenantAddressCell, "text", 1);
		rViaColumn.AddAttribute (rViaCell, "text", 2);
		payeeColumn.AddAttribute (payeeCell, "text", 3);
		tenantNameColumn.AddAttribute (vCell, "text", 4);
		apxDTColumn.AddAttribute (apxDTCell, "text", 5);
		tenantDRColumn.AddAttribute (tenantDRCell, "text", 6);
		proceduresColumn.AddAttribute (proceduresCell, "text", 7);
		toTntViaColumn.AddAttribute (toTntViaCell, "text", 8);
		ownerFeeColumn.AddAttribute (ownerFeeCell, "text", 9);
		ownerNameColumn.AddAttribute (ownerNameCell, "text", 10);
		accountTypeColumn.AddAttribute (accountTypeCell, "text", 11);
	}

	protected void Testcontrol(object o, SwitchPageArgs args)
	{
		userAdded_label.Visible = false;
		userDeleted_label.Visible = false;
		userChanged_label.Visible = false;

		updateTableV2(editView);
		updateTableV2 (display_view);
	}

	protected void clearAddUserButton_Click (object sender, EventArgs e)
	{
		addAccountID_entry.Text = "";
		addTenantAddress_entry.Text = "";
		addRVia_entry.Text = "";
		addPayee_entry.Text = "";
		addTenantName_entry.Text = "";
		addApxDT_entry.Text = "";
		addTenantDr_entry.Text = "";
		addProcedures_entry.Text = "";
		addToTntVia_entry.Text = "";
		addOwnerFee_entry.Text = "";
		addOwnerName_entry.Text = "";
		addAccountType_entry.Text = "";
	}

	protected void clearEditUserButton_Click (object sender, EventArgs e)
	{
		editAccountID_entry.Text = "";
		editTenantAddress_entry.Text = "";
		editRVia_entry.Text = "";
		editPayee_entry.Text = "";
		editTenantName_entry.Text = "";
		editApxDT_entry.Text = "";
		editTenantDr_entry.Text = "";
		editProcedures_entry.Text = "";
		editToTntVia_entry.Text = "";
		editOwnerFee_entry.Text = "";
		editOwnerName_entry.Text = "";
		editAccountType_entry.Text = "";
		//
		getAccountFromDataGrid = null;
	}
}
