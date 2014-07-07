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

namespace AngleController
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

            this.Increase.Click += Increase_Click;
            this.Decrease.Click += Decrease_Click;
            this.Reset.Click += Reset_Click;
        }

        void Reset_Click(object sender, RoutedEventArgs e)
        {
            this.SetSensorAngle(0);
        }

        void Decrease_Click(object sender, RoutedEventArgs e)
        {
            this.SetSensorAngle(this.sensor.ElevationAngle - 1);
        }

        void Increase_Click(object sender, RoutedEventArgs e)
        {
            this.SetSensorAngle(this.sensor.ElevationAngle + 1);
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
                if (this.sensor != null && !this.sensor.IsRunning)
                {
                    this.sensor.Start();
                }
            }

            this.AngleLabel.Content = this.sensor.ElevationAngle;
        }

        private void SetSensorAngle(int angleValue)
        {
            if (angleValue > this.sensor.MinElevationAngle && angleValue < this.sensor.MaxElevationAngle)
            {
                this.sensor.ElevationAngle = angleValue;
                this.AngleLabel.Content = angleValue;
            }
        }
    }
}
