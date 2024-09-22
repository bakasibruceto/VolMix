using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using NAudio.CoreAudioApi;

namespace VolMix
{
    public static class AudioHelper
    {
        public static ImageSource GetApplicationIcon(string applicationName)
        {
            try
            {
                var process = Process.GetProcessesByName(applicationName).FirstOrDefault();
                if (process != null)
                {
                    var icon = System.Drawing.Icon.ExtractAssociatedIcon(process.MainModule.FileName);
                    if (icon != null)
                    {
                        return System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                            icon.Handle,
                            Int32Rect.Empty,
                            BitmapSizeOptions.FromEmptyOptions());
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting icon for {applicationName}: {ex.Message}");
            }
            return null;
        }

        public static void StartSessionCheckTimer(ref DispatcherTimer timer, EventHandler checkSessionHandler)
        {
            if (timer == null)
            {
                timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(1); // Check every 5 seconds
                timer.Tick += checkSessionHandler;
            }
            timer.Start();
        }
    }
}