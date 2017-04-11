using System;
using Gtk;
using System.Windows.Forms;

namespace FirstPropertyManagementWB
{
	public partial class interfaceWindow : Gtk.Window
	{
		public interfaceWindow() :
				base(Gtk.WindowType.Toplevel)
		{
			this.Build();
		}

		protected void mPDF_Click(object sender, EventArgs e)
		{
			FileChooserDialog fc = new FileChooserDialog("File", this, 
			                                             FileChooserAction.SelectFolder,
			                                             "Cancel", ResponseType.Cancel,
			                                             "Open", ResponseType.Accept);


			if (fc.Run() == (int)ResponseType.Accept)
			{
				Console.WriteLine(fc.CurrentFolder);
			}
			fc.Destroy();

		}

		protected void sPDF_Click(object sender, EventArgs e)
		{
			FileChooserDialog fc = new FileChooserDialog("File", this,
			                                             FileChooserAction.Open,
														 "Cancel", ResponseType.Cancel,
														 "Open", ResponseType.Accept);

			//Console.WriteLine(fc.ToString());
			if (fc.Run() == (int)ResponseType.Accept)
			{
				Console.WriteLine(fc.Filename);
			}
			fc.Destroy();
		}

		protected void db_Click(object sender, EventArgs e)
		{
			//MainWindow databaseWindow = new MainWindow();
			//databaseWindow.Show();
			//databaseWindow.FormClosed += new FormClosedEventHandler(Form_Closed);
			//databaseWindow
			//databaseSettings_button.Visible = false;
		}
	}
}
