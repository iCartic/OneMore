﻿//************************************************************************************************
// Copyright © 2020 Steven M Cohn.  All rights reserved.
//************************************************************************************************

#pragma warning disable CS3003 // Type is not CLS-compliant

namespace River.OneMoreAddIn.Dialogs
{
	using System;
	using System.Threading;
	using System.Windows.Forms;
	using Resx = River.OneMoreAddIn.Properties.Resources;


	internal partial class ExportDialog : LocalizableForm
	{
		public ExportDialog(int pageCount)
		{
			InitializeComponent();

			if (NeedsLocalizing())
			{
				Text = Resx.ExportDialog_Text;

				Localize(new string[]
				{
					"folderLabel",
					"formatLabel",
					"formatBox",
					"okButton",
					"cancelButton"
				});
			}

			groupBox.Text = string.Format(Resx.ExportDialog_groupBox_Text, pageCount);
			pathBox.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			formatBox.SelectedIndex = 0;
		}


		protected override void OnShown(EventArgs e)
		{
			Location = new System.Drawing.Point(Location.X, Location.Y - (Height / 2));
			UIHelper.SetForegroundWindow(this);
		}


		public string FolderPath => pathBox.Text;


		public SaveAsCommand.ExportFormat Format
		{
			get
			{
				switch (formatBox.SelectedIndex)
				{
					case 0: return SaveAsCommand.ExportFormat.HTML;
					case 1: return SaveAsCommand.ExportFormat.PDF;
					case 2: return SaveAsCommand.ExportFormat.Word;

					default:
						return SaveAsCommand.ExportFormat.XML;
				}
			}
		}


		private void ChangePath(object sender, EventArgs e)
		{
			okButton.Enabled = pathBox.Text.Trim().Length > 0;
		}


		private void BrowseFolders(object sender, EventArgs e)
		{
			try
			{
				string path = null;

				// FolderBrowserDialog must run in an STA thread
				var thread = new Thread(() =>
				{
					using (var dialog = new FolderBrowserDialog()
					{
						Description = "Export pages to this folder:",
						SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
					})
					{
						// cannot use owner parameter here or it will hang! cross-threading
						if (dialog.ShowDialog() == DialogResult.OK)
						{
							path = dialog.SelectedPath;
						}
					}
				});

				thread.SetApartmentState(ApartmentState.STA);
				thread.IsBackground = true;
				thread.Start();
				thread.Join();

				pathBox.Text = path;

			}
			catch (Exception exc)
			{
				Logger.Current.WriteLine("Error running FolderBrowserDialog", exc);
			}
		}
	}
}
