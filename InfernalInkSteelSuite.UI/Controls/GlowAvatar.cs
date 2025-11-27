using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace InfernalInkSteelSuite.UI.Controls
{
    public class GlowAvatar : Grid
    {
        private readonly Ellipse _ellipse;
        private readonly TextBlock _initialsTextBlock;

        public GlowAvatar()
        {
            _ellipse = new Ellipse();
            _initialsTextBlock = new TextBlock
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Foreground = Brushes.White,
                FontSize = 48,
                FontWeight = FontWeights.Bold
            };

            Children.Add(_ellipse);
            Children.Add(_initialsTextBlock);
        }

        public static readonly DependencyProperty SizeProperty =
            DependencyProperty.Register("Size", typeof(double), typeof(GlowAvatar), new PropertyMetadata(100.0, OnSizeChanged));

        public double Size
        {
            get { return (double)GetValue(SizeProperty); }
            set { SetValue(SizeProperty, value); }
        }

        private static void OnSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (GlowAvatar)d;
            control.Width = (double)e.NewValue;
            control.Height = (double)e.NewValue;
            control._initialsTextBlock.FontSize = (double)e.NewValue / 2;
        }

        public static readonly DependencyProperty InitialsProperty =
            DependencyProperty.Register("Initials", typeof(string), typeof(GlowAvatar), new PropertyMetadata("U", OnInitialsChanged));

        public string Initials
        {
            get { return (string)GetValue(InitialsProperty); }
            set { SetValue(InitialsProperty, value); }
        }

        private static void OnInitialsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (GlowAvatar)d;
            control._initialsTextBlock.Text = (string)e.NewValue;
        }

        public static readonly DependencyProperty BackgroundColorProperty =
            DependencyProperty.Register("BackgroundColor", typeof(Brush), typeof(GlowAvatar), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(60, 120, 180)), OnBackgroundColorChanged));

        public Brush BackgroundColor
        {
            get { return (Brush)GetValue(BackgroundColorProperty); }
            set { SetValue(BackgroundColorProperty, value); }
        }

        private static void OnBackgroundColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (GlowAvatar)d;
            control._ellipse.Fill = (Brush)e.NewValue;
        }
    }
}
