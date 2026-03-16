using System.Windows;

namespace Tema5
{
    public partial class MainWindow : Window
    {
        private HermiteCurve _hermiteManager;

        public MainWindow()
        {
            InitializeComponent();
            // Inițializăm managerul curbei cu canvas-ul din XAML
            _hermiteManager = new HermiteCurve(DrawingCanvas);
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            _hermiteManager.Reset();
        }
    }
}