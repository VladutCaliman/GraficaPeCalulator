using System.Windows;

namespace Tema8
{
    public partial class MainWindow : Window
    {
        private PolygonManager _polygonManager;

        public MainWindow()
        {
            InitializeComponent();

            Loaded += (s, e) =>
            {
                _polygonManager = new PolygonManager(DrawingCanvas);
                ResetButton.Click += (btnSender, btnArgs) => _polygonManager.Reset();
            };
        }
    }
}