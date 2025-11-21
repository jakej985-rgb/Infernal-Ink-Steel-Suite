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

        public static readonly DependencyProperty IsGlowingProperty =
            DependencyProperty.Register("IsGlowing", typeof(bool), typeof(NeonButton), new PropertyMetadata(false, OnIsGlowingChanged));

        public bool IsGlowing
        {
            get { return (bool)GetValue(IsGlowingProperty); }
            set { SetValue(IsGlowingProperty, value); }
        }

        private static void OnIsGlowingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var button = (NeonButton)d;
            button.UpdateGlowEffect();
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

            var gradient = new LinearGradientBrush
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(0, 1),
                GradientStops =
                [
                    new GradientStop(GetLighterColor(newGlowColor, 1.3f), 0.0),
                    new GradientStop(newGlowColor, 1.0)
                ]
            };

            button.Background = gradient;
            button.UpdateGlowEffect();
        }

        private void UpdateGlowEffect()
        {
            if (IsGlowing)
            {
                Effect = new DropShadowEffect
                {
                    Color = GlowColor,
                    BlurRadius = 20,
                    ShadowDepth = 0
                };
            }
            else
            {
                Effect = null;
            }
        }

        private static Color GetLighterColor(Color color, float factor)
        {
            return Color.FromArgb(color.A, (byte)(color.R * factor > 255 ? 255 : color.R * factor),
                                           (byte)(color.G * factor > 255 ? 255 : color.G * factor),
                                           (byte)(color.B * factor > 255 ? 255 : color.B * factor));
        }
    }
}
