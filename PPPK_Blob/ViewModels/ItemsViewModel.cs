using Azure.Storage.Blobs.Models;
using PPPK_Blob.Dao;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPPK_Blob.ViewModels
{
    internal class ItemsViewModel
    {
        public const string FORWARD_SLASH = "/";
        private readonly List<string> ALLOWED_FILE_TYPES = new List<string> { "JPEG", "TIFF", "PNG", "SVG", "GIF" };

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
        private string directory;

        public ItemsViewModel()
        {
            Directories = new ObservableCollection<string>();
            Items = new ObservableCollection<BlobItem>();
            Refresh();
        }

        private void Refresh()
        {
            Items.Clear();
            Directories.Clear();

            Repository.Container.GetBlobs().ToList().ForEach(item =>
            {
                if (item.Name.Contains(FORWARD_SLASH))
                {
                    string dir = item.Name.Substring(0, item.Name.LastIndexOf(FORWARD_SLASH));

                    if (!Directories.Contains(dir))
                    {
                        Directories.Add(dir);
                    }
                }

                if (string.IsNullOrEmpty(Directory) && !item.Name.Contains(FORWARD_SLASH))
                {
                    Items.Add(item);
                }
                else if (!string.IsNullOrEmpty(Directory) && item.Name.StartsWith($"{Directory}{FORWARD_SLASH}"))
                {
                    Items.Add(item);
                }
            });
        }

        public async Task UploadAsync(string path)
        {
            string filename = path.Substring(path.LastIndexOf(Path.DirectorySeparatorChar) + 1);
            string directoryName = path.Substring(path.LastIndexOf(".") + 1).ToUpper();

            foreach (var item in ALLOWED_FILE_TYPES)
            {
                if (item == directoryName)
                {
                    filename = $"{directoryName}{FORWARD_SLASH}{filename}";

                    using (var fs = File.OpenRead(path))
                    {
                        await Repository.Container.GetBlobClient(filename).UploadAsync(fs, true);
                    }

                    Refresh();
                }
            }
        }

        public async Task DownloadAsync(BlobItem item, string path)
        {
            using (var fs = File.OpenWrite(path))
            {
                await Repository.Container.GetBlobClient(item.Name).DownloadToAsync(fs);
            }
            Refresh();
        }

        public async Task DeleteAsync(BlobItem item)
        {
            await Repository.Container.GetBlobClient(item.Name).DeleteAsync();
            Refresh();
        }
    }
}
