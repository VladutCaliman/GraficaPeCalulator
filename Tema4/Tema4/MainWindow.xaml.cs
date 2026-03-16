using System.Windows;

namespace Tema4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Polynom _polynom;
        public MainWindow()
        {
            InitializeComponent();
            _polynom = new(DrawingCanvas);
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
            => _polynom.Reset();
    }
}