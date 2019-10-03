using System;
using System.IO;
using System.Windows.Forms;
using Prism.Commands;
using Prism.Mvvm;

namespace ScriptFilenameFixer
{
	public class MainViewModel : BindableBase
	{
		private string _dirPath;
		private string _outputString;

		public string DirPath
		{
			get { return _dirPath; }
			set { SetProperty(ref _dirPath, value); }
		}

		public string OutputString
		{
			get { return _outputString; }
			set { SetProperty(ref _outputString, value); }
		}
		public DelegateCommand FixCommand { get; private set; }
		public DelegateCommand ChangePathCommand { get; private set; }

		public MainViewModel()
		{
			FixCommand = new DelegateCommand(ChangeFilenamesInPath);
			ChangePathCommand = new DelegateCommand(ChangePath);
		}

		private void ChangePath()
		{
			FolderBrowserDialog folderDialog = new FolderBrowserDialog();

			DialogResult result = folderDialog.ShowDialog();
			if (result.ToString() == "OK")
				DirPath = folderDialog.SelectedPath;
		}

		private void ChangeFilenamesInPath()
		{
			if (IsValidPath(DirPath))
			{
				try
				{
					OutputString = String.Empty;
					var pathes = Directory.GetFiles(DirPath, "*.*", SearchOption.AllDirectories);
					foreach (var path in pathes)
					{
						if (path.Contains(" "))
						{
							string filename = Path.GetFileName(path);
							string filename_no_space = filename.Replace(" ", "");
							string fullOutputPath = path.Replace(filename, filename_no_space);
							if (!File.Exists(fullOutputPath))
							{
								File.Move(path, fullOutputPath);
								OutputString += $"{path} was changed. \n";
							}
							else
							{
								OutputString += $"{path} was not changed because another file with the same name exists. \n";
							}
						}
					}
				}
				catch (Exception e)
				{
					OutputString += e.ToString();
				}
			}
		}

		private bool IsValidPath(string dirPath)
		{
			if (string.IsNullOrEmpty(dirPath))
			{
				return false;
			}

			return Directory.Exists(dirPath);
		}
	}
}
