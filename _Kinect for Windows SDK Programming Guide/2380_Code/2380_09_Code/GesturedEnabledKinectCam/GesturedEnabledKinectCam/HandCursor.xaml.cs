using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Kinect;

namespace GesturedEnabledKinectCam
{
    /// <summary>
    /// Interaction logic for HandCursor.xaml
    /// </summary>
    public partial class HandCursor : UserControl
    {

        public HandCursor()
        {
            InitializeComponent();
        }

        public void SetPosition(KinectSensor kinect, Joint joint)
        {
            ColorImagePoint colorImagePoint = kinect.CoordinateMapper.MapSkeletonPointToColorPoint(joint.Position, ColorImageFormat.RgbResolution640x480Fps30);
            Canvas.SetLeft(this, colorImagePoint.X);
            Canvas.SetTop(this, colorImagePoint.Y);
        }

        public  CursorPoint GetCursorPoint()
        {
            Point elementTopLeft = this.PointToScreen(new Point());
            double centerX = elementTopLeft.X + (this.ActualWidth / 2);
            double centerY = elementTopLeft.Y + (this.ActualHeight / 2);
            return new CursorPoint { X = centerX, Y = centerY };
        }

    

    }
}
