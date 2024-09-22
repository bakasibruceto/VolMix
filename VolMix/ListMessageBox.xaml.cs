using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace VolMix
{
    public partial class ListMessageBox : Window
    {
        public string? SelectedApplication { get; private set; }
        private DispatcherTimer _timer;

        public ListMessageBox(Window owner)
        {
            InitializeComponent();
            Owner = owner; // Set the owner to the main window
            LoadApplications();

            // Initialize and start the timer
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(5); // Set the interval to 5 seconds
            _timer.Tick += (s, e) => LoadApplications();
            _timer.Start();
        }

        private void LoadApplications()
        {
            // List of common browser process names
            var browserNames = new List<string> { "chrome", "firefox", "msedge", "iexplore", "opera" };

            // Get all running processes
            var processes = Process.GetProcesses();

            // Filter and select process names and icons
            var applicationInfos = processes
                .Where(p => !string.IsNullOrEmpty(p.MainWindowTitle)) // Filter out processes without a window title
                .Select(p => new
                {
                    Name = p.ProcessName,
                    Icon = GetIcon(p)
                })
                .Distinct()
                .ToList();

            // Bind the list to the ListBox (assuming you have a ListBox named ApplicationsList)
            ApplicationsList.ItemsSource = applicationInfos;

            // Automatically select the browser if found
            var browserProcess = applicationInfos.FirstOrDefault(info => browserNames.Contains(info.Name.ToLower()));
            if (browserProcess != null)
            {
                ApplicationsList.SelectedItem = browserProcess;
                SelectedApplication = browserProcess.Name;
            }
        }

        private BitmapImage? GetIcon(Process process)
        {
            try
            {
                var icon = System.Drawing.Icon.ExtractAssociatedIcon(process.MainModule.FileName);
                if (icon != null)
                {
                    var bitmap = icon.ToBitmap();
                    var bitmapImage = new BitmapImage();
                    using (var memory = new System.IO.MemoryStream())
                    {
                        bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                        memory.Position = 0;
                        bitmapImage.BeginInit();
                        bitmapImage.StreamSource = memory;
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.EndInit();
                    }
                    return bitmapImage;
                }
            }
            catch
            {
                // Handle exceptions (e.g., access denied)
            }
            return null;
        }

        private void SelectButton_Click(object? sender, RoutedEventArgs e)
        {
            var selectedItem = ApplicationsList.SelectedItem as dynamic;
            SelectedApplication = selectedItem?.Name;
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}