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
using System.ComponentModel;

namespace KinectInfoBox
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowViewModel viewModel;
        private KinectSensor sensor;
        private void SetKinectInfo()
        {
            if (this.sensor != null)
            {
                this.viewModel.ConnectionID = this.sensor.DeviceConnectionId;
                this.viewModel.DeviceID = this.sensor.UniqueKinectId;
                this.viewModel.SensorStatus = this.sensor.Status.ToString();
                this.viewModel.IsColorStreamEnabled = this.sensor.ColorStream.IsEnabled;
                this.viewModel.IsDepthStreamEnabled = this.sensor.DepthStream.IsEnabled;
                this.viewModel.IsSkeletonStreamEnabled = this.sensor.SkeletonStream.IsEnabled;
                this.viewModel.SensorAngle = this.sensor.ElevationAngle;
            }
        }
        public MainWindow()
        {
            InitializeComponent();
            // 绑定窗口级别事件，
            // 在打开窗口与关闭窗口时
            // 分别打开与关闭Kinect
            this.Loaded += this.MainWindow_Loaded;
            this.Closing += this.MainWinow_Closed;

            // 绑定数据
          
            this.viewModel = new MainWindowViewModel();
            this.viewModel.CanStart = false;
            this.viewModel.CanStop = false;
            this.DataContext = this.viewModel;

            // 绑定点击事件
            this.BUTTON_SENSOR_CLOSE.Click += BUTTON_SENSOR_CLOSE_Click;
            this.BUTTON_SENSOR_START.Click += BUTTON_SENSOR_START_Click;
        }
        // 按钮点击事件
        private void BUTTON_SENSOR_CLOSE_Click(object sender, RoutedEventArgs e)
        {
            this.StopSensor();
        }
        private void BUTTON_SENSOR_START_Click(object sender, RoutedEventArgs e)
        {
            this.StartSensor();
        }
        private void StartSensor() 
        {
            if (this.sensor != null && !this.sensor.IsRunning) 
            {
                this.sensor.Start();
                this.viewModel.CanStart = false;
                this.viewModel.CanStop = true;
            }
        }

        private void StopSensor()
        {
            if (this.sensor != null && this.sensor.IsRunning)
            {
                this.sensor.Stop();
                this.viewModel.CanStart = true;
                this.viewModel.CanStop = false;
            }
        }
        protected void MainWinow_Closed(object sender, CancelEventArgs e)
        {
            if (this.sensor != null && this.sensor.IsRunning)
            {
                this.StopSensor();
            }
            
        }
        protected void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (KinectSensor.KinectSensors.Count > 0)
            {   
                // 如果连接了多个Kinect的话
                this.sensor = KinectSensor.KinectSensors[0];
                this.StartSensor();

                this.sensor.ColorStream.Enable();
                this.sensor.DepthStream.Enable();
                this.sensor.SkeletonStream.Enable();

                // 监听Sensor状态
                // 如果状态改变，则更新视图
                KinectSensor.KinectSensors.StatusChanged += KinectSensors_StatusChanged;

                this.SetKinectInfo();
            }
            else 
            {
                MessageBox.Show("No device is connected with system");
                this.Close();
            }
        }

        void KinectSensors_StatusChanged(object sender, StatusChangedEventArgs e)
        {   
            // 更改当前sensor状态，更新实例属性
            // 实例时也会触发OnNotifyPropertyChange事件
            // 再回过头来触发视图的更改

            // 可不可以在这里直接更改视图？
            // 一定要确保实例更新之后才更新model层？
            this.viewModel.SensorStatus = e.Status.ToString();
        }
    }
}
