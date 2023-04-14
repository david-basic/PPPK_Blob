using Azure.Storage.Blobs.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPPK_Blob.ViewModels
{
    internal class ItemsViewModel
    {
        public const string ForwardSlash = "/";

        public ObservableCollection<string> Directories { get; }
        public ObservableCollection<BlobItem> Items { get; }
        public string Directory
        {
            get => directory;
            set
            {
                directory = value;
                Refresh();
            }
        }

        public ItemsViewModel()
        {
            Directories = new ObservableCollection<string>();
            Items = new ObservableCollection<BlobItem>();
            Refresh();
        }

        private void Refresh()
        {

        }

        private string directory;

    }
}
