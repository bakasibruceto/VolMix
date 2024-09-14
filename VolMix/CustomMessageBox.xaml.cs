using System.Windows;

namespace VolMix
{
    public partial class CustomMessageBox : Window
    {
        public bool Result { get; private set; }

        public CustomMessageBox(Window owner, string message)
        {
            InitializeComponent();
            Owner = owner;
            MessageTextBlock.Text = message;
        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            Result = true;
            this.Close();
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            Result = false;
            this.Close();
        }

        public static bool Show(Window owner, string message)
        {
            CustomMessageBox messageBox = new CustomMessageBox(owner, message);
            messageBox.ShowDialog();
            return messageBox.Result;
        }
    }
}