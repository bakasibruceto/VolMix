using System.Globalization;
using System.Text;
using System.Windows;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NAudio.CoreAudioApi;
using AudioSwitcher.AudioApi;
using System.Data;
using System.Diagnostics;
using System.Windows.Threading;
using VolMix.EventHandlers;

namespace VolMix
{
    public partial class MainWindow : Window
    {


        private MMDeviceEnumerator _deviceEnumerator;
        private MMDevice? _defaultPlaybackDevice;
        private MMDevice? _defaultDevice;
        private AudioSessionControl? _currentSession;
        private DispatcherTimer? _sessionCheckTimer;
        private string? _selectedApplicationName;
        private Slider? _selectedVolumeSlider;
        private int? _selectedProcessId;



        public MainWindow()
        {
            InitializeComponent();
            _deviceEnumerator = new MMDeviceEnumerator();
            _defaultPlaybackDevice = _deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, NAudio.CoreAudioApi.Role.Multimedia);
            _currentSession = null;
            _defaultDevice = _defaultPlaybackDevice;

            // Get the system volume and set it to the VolumeSlider
            if (_defaultDevice != null)
            {
                float systemVolume = _defaultDevice.AudioEndpointVolume.MasterVolumeLevelScalar * 100;
                VolumeSlider.Value = systemVolume;
            }

            _sessionCheckTimer = new DispatcherTimer();
        }

        private void RefreshAudioSessions()
        {
            if (_defaultPlaybackDevice == null)
            {
                MessageBox.Show("Default playback device not found.");
                return;
            }

            var sessions = _defaultPlaybackDevice.AudioSessionManager.Sessions;
            // Update your UI or internal state with the new list of sessions
            // For example, you can update a ListBox with the new sessions
        }

        private void System_VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_defaultDevice != null)
            {
                _defaultDevice.AudioEndpointVolume.MasterVolumeLevelScalar = (float)(e.NewValue / 100.0);
            }
        }

        private void listButton_Click(object sender, RoutedEventArgs e)
        {
            var listMessageBox = new ListMessageBox(this); // Pass the main window as the owner
            if (listMessageBox.ShowDialog() == true)
            {
                string? selectedApp = listMessageBox.SelectedApplication;
                if (!string.IsNullOrEmpty(selectedApp))
                {
                    if (sender is Button listButton && listButton.Tag is Tuple<Slider, Image> tag)
                    {
                        ConnectVolumeSliderToApplication(selectedApp, tag.Item1, tag.Item2);
                    }
                }
            }
        }


        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_currentSession != null && _selectedProcessId.HasValue)
            {
                try
                {
                    var sessionProcess = Process.GetProcessById(_selectedProcessId.Value);
                    if (sessionProcess != null && sessionProcess.ProcessName.Equals(_selectedApplicationName, StringComparison.OrdinalIgnoreCase))
                    {
                        _currentSession.SimpleAudioVolume.Volume = (float)(e.NewValue / 100);
                    }
                    else
                    {
                        // If the process is no longer running, reset the current session
                        ResetCurrentSession();
                    }
                }
                catch (ArgumentException)
                {
                    // Process.GetProcessById throws ArgumentException if the process is not running
                    ResetCurrentSession();
                }
                catch (Exception ex)
                {
                    // Log or handle other exceptions
                    MessageBox.Show($"Error: {ex.Message}");
                    ResetCurrentSession();
                }
            }
        }

        private void ConnectVolumeSliderToApplication(string applicationName, Slider volumeSlider, Image appIcon)
        {
            _selectedApplicationName = applicationName;
            _selectedVolumeSlider = volumeSlider;

            if (_defaultPlaybackDevice == null)
            {
                MessageBox.Show("Default playback device not found.");
                return;
            }

            // Set the application icon
            var icon = AudioHelper.GetApplicationIcon(applicationName);
            if (icon != null)
            {
                appIcon.Source = icon;
            }

            // Set up the volume slider to update the application's volume once the session is found
            _selectedVolumeSlider.ValueChanged += VolumeSlider_ValueChanged;

            // Initialize the timer if it's null
            if (_sessionCheckTimer == null)
            {
                _sessionCheckTimer = new DispatcherTimer();
                _sessionCheckTimer.Interval = TimeSpan.FromSeconds(1); // Check every second
                _sessionCheckTimer.Tick += CheckForAudioSession;
            }

            // Start the timer to check for audio sessions periodically
            _sessionCheckTimer.Start();

            CheckForAudioSession(null, null); // Initial check
        }

        private void CheckForAudioSession(object? sender, EventArgs? e)
        {
            if (_defaultPlaybackDevice == null || string.IsNullOrEmpty(_selectedApplicationName))
            {
                return;
            }

            var sessions = _defaultPlaybackDevice.AudioSessionManager.Sessions;
            AudioSessionControl? session = null;

            for (int i = 0; i < sessions.Count; i++)
            {
                var sessionControl = sessions[i];
                var sessionProcess = Process.GetProcessById((int)sessionControl.GetProcessID);

                if (sessionProcess != null && sessionProcess.ProcessName.Equals(_selectedApplicationName, StringComparison.OrdinalIgnoreCase))
                {
                    session = sessionControl;
                    _selectedProcessId = sessionProcess.Id; // Store the process ID
                    break;
                }
            }

            if (session != null)
            {
                _currentSession = session;
                if (_selectedVolumeSlider != null)
                {
                    _selectedVolumeSlider.Value = _currentSession.SimpleAudioVolume.Volume * 100;
                }
            }
            else
            {
                // If the session is not found, reset the current session
                ResetCurrentSession();
            }
        }

        private void ResetCurrentSession()
        {
            _currentSession = null;
            _selectedProcessId = null;
            if (_selectedVolumeSlider != null)
            {
                _selectedVolumeSlider.Value = 0; // Optionally reset the slider value
            }
        }




        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            // Show the custom confirmation dialog with the main window as the owner
            bool result = CustomMessageBox.Show(this, "Are you sure you want to remove this item?");

            // Handle the user's response
            if (result)
            {
                // Logic to handle the remove action
                if (sender is Button removeButton)
                {
                    if (removeButton.Tag is Grid mixContainer)
                    {
                        // Detach event handlers
                        if (_selectedVolumeSlider != null)
                        {
                            _selectedVolumeSlider.ValueChanged -= VolumeSlider_ValueChanged;
                        }

                        // Reset state
                        ResetCurrentSession();

                        MixerContainer.Children.Remove(mixContainer);
                        MessageBox.Show("Item removed!");
                    }
                    else
                    {
                        MessageBox.Show("Tag is not set correctly.");
                    }
                }
                else
                {
                    MessageBox.Show("Sender is not a Button.");
                }
            }
            else
            {
                // Logic if the user selects "No"
                MessageBox.Show("Remove action canceled.");
            }
        }


        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            // Create a new MixContainer Grid
            Grid newMixContainer = new Grid
            {
                Width = 75,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(0, 0, 0, 0),
                Height = 327,
                HorizontalAlignment = HorizontalAlignment.Left
            };

            // Create the IconHead Border
            Border iconHead = new Border
            {
                CornerRadius = new CornerRadius(5, 5, 0, 0),
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
                Margin = new Thickness(0, 7, 0, 260),
                Width = 60,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            // Create the Grid inside the IconHead Border
            Grid iconGrid = new Grid();
            Image appIcon = new Image
            {
                Margin = new Thickness(9, 9, 9, 9)
            };

            // Create the VolumeSlider
            Slider volumeSlider = new Slider
            {
                Style = (Style)FindResource("CustomSliderStyle"),
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 9, 0, 0),
                VerticalAlignment = VerticalAlignment.Top,
                Width = 30,
                Height = 150,
                Orientation = Orientation.Vertical,
                Minimum = 0,
                Maximum = 100,
                Value = 100
            };

            // Create the listButton
            Button listButton = new Button
            {
                Content = "+",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Width = 20,
                Height = 20,
                Background = Brushes.Transparent,
                Foreground = Brushes.White,
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
                Tag = new Tuple<Slider, Image>(volumeSlider, appIcon) // Store the volumeSlider and appIcon in the Tag property
            };
            listButton.Click += listButton_Click;

            iconGrid.Children.Add(appIcon);
            iconGrid.Children.Add(listButton);
            iconHead.Child = iconGrid;

            // Create the VolumeSlider Border
            Border sliderBorder = new Border
            {
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
                HorizontalAlignment = HorizontalAlignment.Center,
                Height = 179,
                VerticalAlignment = VerticalAlignment.Top,
                Width = 60,
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF2C2F33")),
                Margin = new Thickness(0, 66, 0, 0)
            };

            sliderBorder.Child = volumeSlider;

            // Create the volCount Border
            Border labelBorder = new Border
            {
                CornerRadius = new CornerRadius(0, 0, 5, 5),
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
                HorizontalAlignment = HorizontalAlignment.Center,
                Height = 26,
                Margin = new Thickness(0, 244, 0, 0),
                VerticalAlignment = VerticalAlignment.Top,
                Width = 60
            };

            TextBlock volCount = new TextBlock
            {
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left,
                Foreground = Brushes.White,
                FontSize = 16,
                TextAlignment = TextAlignment.Center,
                Width = 50,
                Margin = new Thickness(5, 0, 0, 0)
            };

            // Bind the TextBlock to the Slider's Value
            Binding binding = new Binding("Value")
            {
                Source = volumeSlider,
                Converter = (IValueConverter)FindResource("IntegerPercentageConverter")
            };
            volCount.SetBinding(TextBlock.TextProperty, binding);

            labelBorder.Child = volCount;

            // Create the Remove Button
            Button removeButton = new Button
            {
                Content = "Remove",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Top,
                Width = 60,
                Height = 30,
                Margin = new Thickness(0, 280, 0, 0),
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF2C2F33")),
                Foreground = Brushes.White,
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
                Tag = newMixContainer // Store the newMixContainer in the Tag property
            };
            removeButton.Click += RemoveButton_Click;

            // Add the elements to the new MixContainer
            newMixContainer.Children.Add(iconHead);
            newMixContainer.Children.Add(sliderBorder);
            newMixContainer.Children.Add(labelBorder);
            newMixContainer.Children.Add(removeButton);

            // Add the new MixContainer to the MixerContainer
            MixerContainer.Children.Add(newMixContainer);
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}