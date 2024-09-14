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
    /// 
   


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
                MessageBox.Show("Item removed!");
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
                Margin = new Thickness(0), // Adjusted margin
                Height = 282,
                HorizontalAlignment = HorizontalAlignment.Left
            };

            // Add the inner elements to the new MixContainer
            Border sliderBorder = new Border
            {
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
                HorizontalAlignment = HorizontalAlignment.Center,
                Height = 204,
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
                Orientation = Orientation.Vertical
            };

            sliderBorder.Child = slider;

            Border labelBorder = new Border
            {
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
                HorizontalAlignment = HorizontalAlignment.Center,
                Height = 26,
                Margin = new Thickness(0, 244, 0, 0),
                VerticalAlignment = VerticalAlignment.Top,
                Width = 60
            };

            Label volCount = new Label
            {
                Content = "100%\n",
                VerticalAlignment = VerticalAlignment.Top,
                Height = 28,
                Foreground = Brushes.White,
                FontSize = 16,
                HorizontalAlignment = HorizontalAlignment.Center,
                Width = 50,
                Margin = new Thickness(0, -5, 0, 0)
            };

            labelBorder.Child = volCount;

            Border emptyBorder = new Border
            {
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
                Margin = new Thickness(0, 7, 0, 215),
                Width = 60,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            // Add the borders to the new MixContainer
            newMixContainer.Children.Add(sliderBorder);
            newMixContainer.Children.Add(labelBorder);
            newMixContainer.Children.Add(emptyBorder);

            // Add the new MixContainer to the MixerContainer
            MixerContainer.Children.Add(newMixContainer);
        }

    }
}