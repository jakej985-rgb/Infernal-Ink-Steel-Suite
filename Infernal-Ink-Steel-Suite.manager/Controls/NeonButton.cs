using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace InfernalInkSteelSuite.Controls
{
    public class NeonButton : Button
    {
        public NeonButton()
        {
            DefaultStyleKey = typeof(NeonButton);
            GlowColor = (Color)ColorConverter.ConvertFromString("#FF00FFFF");
        }

        public static readonly DependencyProperty GlowColorProperty =
            DependencyProperty.Register("GlowColor", typeof(Color), typeof(NeonButton), new PropertyMetadata(Colors.Cyan, OnGlowColorChanged));

        public Color GlowColor
        {
            get { return (Color)GetValue(GlowColorProperty); }
            set { SetValue(GlowColorProperty, value); }
        }

        private static void OnGlowColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var button = (NeonButton)d;
            var newGlowColor = (Color)e.NewValue;

            button.Effect = new DropShadowEffect
            {
                Color = newGlowColor,
                BlurRadius = 20,
                ShadowDepth = 0
            };

            var gradient = new LinearGradientBrush();
            gradient.StartPoint = new Point(0, 0);
            gradient.EndPoint = new Point(0, 1);
            gradient.GradientStops.Add(new GradientStop(GetLighterColor(newGlowColor, 1.3f), 0.0));
            gradient.GradientStops.Add(new GradientStop(newGlowColor, 1.0));

            button.Background = gradient;
        }
        private static Color GetLighterColor(Color color, float factor)
        {
            return Color.FromArgb(color.A, (byte)(color.R * factor > 255 ? 255 : color.R * factor),
                                           (byte)(color.G * factor > 255 ? 255 : color.G * factor),
                                           (byte)(color.B * factor > 255 ? 255 : color.B * factor));
        }
    }
}
