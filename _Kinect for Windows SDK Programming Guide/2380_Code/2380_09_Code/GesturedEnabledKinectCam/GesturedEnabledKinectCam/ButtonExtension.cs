using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Shapes;

namespace GesturedEnabledKinectCam
{
    public static class ButtonExtension
    {
        public static ButtonPosition GetPosition(this Button button)
        {
            Point buttonPosition = button.PointToScreen(new Point());
            return new ButtonPosition { Left = buttonPosition.X, 
                Right = buttonPosition.X + button.ActualWidth, 
                Top = buttonPosition.Y,
                Bottom = buttonPosition.Y + button.Height };
        }
    }
}
