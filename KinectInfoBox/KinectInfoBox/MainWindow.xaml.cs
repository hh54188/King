using Microsoft.Kinect;
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

namespace KinectInfoBox
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
        }

        private void StartSensor() 
        {
            if (this.sensor != null && !this.sensor.IsRunning) 
            {
                this.sensor.Start();
            }
        }

        private void StopSensor()
        {
            if (this.sensor != null && this.sensor.IsRunning)
            {
                this.sensor.Stop();
            }
        }

        protected void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (KinectSensor.KinectSensors.Count > 0)
            {
                this.sensor = KinectSensor.KinectSensors[0];
                this.StartSensor();
                this.sensor.ColorStream.Enable();
                this.sensor.DepthStream.Enable();
                this.sensor.SkeletonStream.Enable();
            }
            else 
            {
                MessageBox.Show("No device is connected with system");
                this.Close();
            }
        }
    }
}
