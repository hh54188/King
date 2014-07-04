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

            this.AngleController.Minimum = this.sensor.MinElevationAngle;
            this.AngleController.Maximum = this.sensor.MaxElevationAngle;
            this.AngleController.Value = this.sensor.ElevationAngle;
            this.AngleController.ValueChanged += AngleController_ValueChanged;

            this.AngleLabel.Content = this.sensor.ElevationAngle;

            this.TButton.Click += TButton_Click;
        }

        void TButton_Click(object sender, RoutedEventArgs e)
        {
            this.SetSensorAngle(this.sensor.ElevationAngle + 2);
        }

        void AngleController_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.SetSensorAngle(Convert.ToInt32(e.NewValue));
        }

        private void SetSensorAngle(int angleValue)
        {
            //this.AngleLabel.Content = angleValue;
            //return;

            if (angleValue > this.sensor.MinElevationAngle && angleValue < this.sensor.MaxElevationAngle)
            {
                this.sensor.ElevationAngle = angleValue;
                this.AngleLabel.Content = angleValue;
            }
        }
    }
}
