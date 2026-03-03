using System.Windows;

namespace Tema2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Action _redrawCurrentFigure = () => { };
        public int ParameterA { get; set; } = int.MinValue;
        public int ParameterB { get; set; } = int.MinValue;
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            DrawingCanvas.SizeChanged += DrawingCanvas_SizeChanged;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            CurveDrawer.DrawAxes(DrawingCanvas);
            _redrawCurrentFigure();
        }

        private void DrawingCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            CurveDrawer.DrawAxes(DrawingCanvas);
            _redrawCurrentFigure();
        }

        private void ElipsisButton_Click(object sender, RoutedEventArgs e)
        {
            if (ParameterA == int.MinValue || ParameterB == int.MinValue)
            {
                ParameterA = 0; ParameterB = 2;
            }

            _redrawCurrentFigure = () => CurveDrawer.DrawEllipse(DrawingCanvas);
            _redrawCurrentFigure();
        }

        private void SpiralButton_Click(object sender, RoutedEventArgs e)
        {
            if (ParameterA == int.MinValue || ParameterB == int.MinValue)
            {
                ParameterA = 0; ParameterB = 20;
            }

            _redrawCurrentFigure = () => CurveDrawer.DrawSpiral(DrawingCanvas);
            _redrawCurrentFigure();
        }

        private void ParabolaButton_Click(object sender, RoutedEventArgs e)
        {
            if (ParameterA == int.MinValue || ParameterB == int.MinValue)
            {
                ParameterA = -2; ParameterB = +2;
            }

            _redrawCurrentFigure = () => CurveDrawer.DrawParabola(DrawingCanvas);
            _redrawCurrentFigure();
        }
    }
}
