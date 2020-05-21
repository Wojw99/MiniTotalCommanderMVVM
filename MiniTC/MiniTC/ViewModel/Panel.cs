using MiniTC.ViewModel.BaseClasses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MiniTC.ViewModel
{
    class Panel : BaseViewModel
    {
        private string path;
        private string driveSelected;
        private string listSelected;
        private ObservableCollection<string> contentCollection;

        public Panel()
        {
            DriveArray = Directory.GetLogicalDrives();
            DriveSelected = DriveArray[0];

            Path = DriveSelected;
            ContentCollection = LoadContentCollection(DriveSelected);
        }

        #region Get/Set
        public string Path
        {
            get
            {
                return path;
            }
            set
            {
                path = value; OnPropertyChanged(nameof(Path));
            }
        }

        public string DriveSelected
        {
            get
            {
                return driveSelected;
            }
            set
            {
                driveSelected = value; OnPropertyChanged(nameof(DriveSelected));
            }
        }

        public string ListSelected
        {
            get
            {
                return listSelected;
            }
            set
            {
                listSelected = value; OnPropertyChanged(nameof(ListSelected));
            }
        }

        public string[] DriveArray { get; set; }
        public ObservableCollection<string> ContentCollection
        {
            get => contentCollection; 
            set
            {
                contentCollection = value; OnPropertyChanged(nameof(ContentCollection));
            }
        }
        #endregion

        public ObservableCollection<string> LoadContentCollection(string path)
        {
            try
            {
                var collection = new ObservableCollection<string>();
                var directories = Directory.GetDirectories(path);
                var files = Directory.GetFiles(path);

                if (!DriveArray.Contains(path))
                {
                    collection.Add("..");
                }

                for (int i = 0; i < directories.Length; i++)
                {
                    collection.Add("<D>" + directories[i]);
                }

                for (int i = 0; i < files.Length; i++)
                {
                    collection.Add(files[i]);
                }

                return collection;
            } 
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(ex.Message);
                SetDefaultPath(new object());
                return ContentCollection;
            }
        }

        public void SetDefaultPath(object sender)
        {
            Path = DriveSelected;
            ContentCollection = LoadContentCollection(DriveSelected);
        }

        public void UpdatePathAndContent(object sender)
        {
            if (ListSelected != null)
            {
                if (ListSelected == "..")
                {
                    var tmp = File.GetAttributes(Path);
                    var parent = Directory.GetParent(Path);

                    if (tmp.HasFlag(FileAttributes.Directory))
                    {
                        Path = parent.FullName;
                        ContentCollection = LoadContentCollection(Path);
                    }
                    else
                    {
                        parent = Directory.GetParent(parent.FullName);
                        Path = parent.FullName;
                        ContentCollection = LoadContentCollection(Path);
                    }

                    ListSelected = null;
                }
                else if (ListSelected[0] == '<')
                {
                    Path = ListSelected.Substring(3);
                    ContentCollection = LoadContentCollection(Path);
                }
                else
                {
                    Path = ListSelected;
                }
            }
        }
    }
}
