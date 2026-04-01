using System.Windows;

namespace Tema6
{
    public partial class MainWindow : Window
    {
        private BezierManager _bezierManager;

        public MainWindow()
        {
            InitializeComponent();
            _bezierManager = new BezierManager(DrawingCanvas);
            ResetButton.Click += (s, e) => _bezierManager.Reset();
        }
    }
}