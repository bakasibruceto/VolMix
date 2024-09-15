using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VolMix
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                // Handle double-click to maximize/restore
                MaximizeButton_Click(sender, e);
            }
            else
            {
                // Handle drag move
                this.DragMove();
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
                if (sender is Button removeButton && removeButton.Tag is Grid mixContainer)
                {
                    MixerContainer.Children.Remove(mixContainer);
                    MessageBox.Show("Item removed!");
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
                Margin = new Thickness(0),
                Height = 327,
                HorizontalAlignment = HorizontalAlignment.Left,
                AllowDrop = true
            };

            // Add the inner elements to the new MixContainer
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

            Slider slider = new Slider
            {
                Style = (Style)FindResource("SliderStyle1"),
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 9, 0, 0),
                VerticalAlignment = VerticalAlignment.Top,
                Width = 18,
                Height = 150,
                Orientation = Orientation.Vertical,
                Minimum = 0,
                Maximum = 100,
                Value = 100
            };

            sliderBorder.Child = slider;

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
                Text = "100%",
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left,
                Foreground = Brushes.White,
                FontSize = 16,
                TextAlignment = TextAlignment.Center,
                Width = 50,
                Margin = new Thickness(5, 0, 0, 0)
            };

            labelBorder.Child = volCount;

            Border imageBorder = new Border
            {
                CornerRadius = new CornerRadius(5, 5, 0, 0),
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
                Margin = new Thickness(0, 7, 0, 260),
                Width = 60,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            Image image = new Image
            {
                Margin = new Thickness(9, 9, 9, 9)
            };

            imageBorder.Child = image;

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
                Tag = newMixContainer // Set the Tag to the MixContainer
            };

            removeButton.Click += RemoveButton_Click;

            // Add the borders and button to the new MixContainer
            newMixContainer.Children.Add(sliderBorder);
            newMixContainer.Children.Add(labelBorder);
            newMixContainer.Children.Add(imageBorder);
            newMixContainer.Children.Add(removeButton);

            // Add the new MixContainer to the MixerContainer
            MixerContainer.Children.Add(newMixContainer);
        }

        
    }
}