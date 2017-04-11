﻿/*
    Copyright (C) <2017>  <Sherwin Bayer, Jasneel Chauhan, Heemesh Bhikha, Melvin Mathew> 
    This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
 
    This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Affero General Public License for more details.
    You should have received a copy of the GNU Affero General Public License along with this program.  If not, see <http://www.gnu.org/licenses/>. 

    This program also makes use of the EPPlus API. This library is unmodified, the program simply makes use of its API: you can redistribute it and/or modify it under the terms of the GNU Library General Public License (LGPL) Version 2.1, February 1999.
    You should have received a copy of the GNU Library General Public License along with this program.  If not, see <http://epplus.codeplex.com/license/>. 

*/

using System;
using Gtk;

namespace FirstPropertyManagementWB 
{
	class MainClass
	{
		public static void Main (string [] args)
		{
			Application.Init ();
			Gtk.Settings.Default.SetLongProperty ("gtk-button-images", 1, "");
			FirstPropertyManagementWB.interfaceWindow win = new FirstPropertyManagementWB.interfaceWindow ();
			//MainWindow win = new MainWindow();			win.Show ();
			Application.Run ();
			Application.Quit ();

		}
	}
}
