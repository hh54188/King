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

namespace DepthCamera
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private KinectSensor sensor;

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
            this.Closing += MainWindow_Closing;
            this.NearMode.Checked += NearMode_Checked;
            this.NearMode.Unchecked += NearMode_Unchecked;
        }

        void NearMode_Unchecked(object sender, RoutedEventArgs e)
        {
            this.sensor.DepthStream.Range = DepthRange.Near;
        }

        void NearMode_Checked(object sender, RoutedEventArgs e)
        {
            this.sensor.DepthStream.Range = DepthRange.Default;
        }

        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.sensor != null && this.sensor.IsRunning)
            {
                this.sensor.Stop();
            }
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (KinectSensor.KinectSensors.Count > 0)
            {
                this.sensor = KinectSensor.KinectSensors[0];
            }

            if (this.sensor != null && !this.sensor.DepthStream.IsEnabled)
            {
                this.sensor.DepthStream.Enable();
                //this.sensor.DepthFrameReady += sensor_DepthFrameReady;
            }

            if (this.sensor != null && !this.sensor.IsRunning)
            {
                this.sensor.Start();
            }
        }

        void sensor_DepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            this.TEST.Content = this.sensor.DepthStream.TooNearDepth;
            using (DepthImageFrame depthimageframe = e.OpenDepthImageFrame())
            {
                if (depthimageframe == null)
                {
                    return;
                }

                short[] pixelData = new short[depthimageframe.PixelDataLength];
                int stride = depthimageframe.Width * 2;
                depthimageframe.CopyPixelDataTo(pixelData);
                this.DepthController.Source = BitmapSource.Create(depthimageframe.Width, depthimageframe.Height, 96, 96, PixelFormats.Gray16, null, pixelData, stride);
            }
        }
    }
}
