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
using System.Windows.Forms;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemIOPath = System.IO.Path;
using MySql.Data.MySqlClient;
using System.Data;


namespace FirstPropertyManagementWB
{
	public partial class interfaceWindow : Gtk.Window
	{
	
		public interfaceWindow() : base(Gtk.WindowType.Toplevel)
		{
			this.Build();
			title_label.ModifyFont (Pango.FontDescription.FromString ("Georgia 28"));
			DeleteEvent += delegate {
				Gtk.Application.Quit ();
			};
		}

		protected void mPDF_Click(object sender, EventArgs e)
		{
			multiPDFLoadingLabel.Text = "Scanning...";
			FileChooserDialog fc = new FileChooserDialog("File", this, FileChooserAction.SelectFolder, "Cancel", ResponseType.Cancel,"Open", ResponseType.Accept); 
			String selectedfolder = "";
			if (fc.Run() == (int)ResponseType.Accept)
			{
				selectedfolder = fc.CurrentFolder;
				fc.Destroy();
				MultiPDF.multiPDF(selectedfolder);
				multiPDFLoadingLabel.Text = "";
			}
			else
			{
				fc.Destroy();
				multiPDFLoadingLabel.Text = "";
			}

		}

		protected void sPDF_Click(object sender, EventArgs e)
		{
			multiPDFLoadingLabel.Text = "Scanning...";
			FileChooserDialog fc = new FileChooserDialog("File", this,FileChooserAction.Open,"Cancel", ResponseType.Cancel, "Open", ResponseType.Accept);
			string selectedfile = "";
			if (fc.Run() == (int)ResponseType.Accept)
			{
				selectedfile = fc.Filename;
				fc.Destroy();
				SinglePDF.singlePDF(selectedfile);
				multiPDFLoadingLabel.Text = "";
			}
			else
			{
				fc.Destroy();
				multiPDFLoadingLabel.Text = "";
			}
		}

		protected void pressed(object sender, EventArgs e)
		{
			MainWindow databaseWindow = new MainWindow();
			databaseWindow.Show();
		}
	}
}

