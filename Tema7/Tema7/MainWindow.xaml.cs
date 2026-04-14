using System.Windows;

namespace Tema7
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BSplineManager _bSplineManager;

        public MainWindow()
        {
            InitializeComponent();
            _bSplineManager = new BSplineManager(DrawingCanvas);
            ResetButton.Click += (s, e) => _bSplineManager.Reset();
        }
    }
}