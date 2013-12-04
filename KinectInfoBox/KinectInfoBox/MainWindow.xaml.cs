﻿using Microsoft.Kinect;
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
            }
        }

        private void StopSensor()
        {
            if (this.sensor != null && this.sensor.IsRunning)
            {
                this.sensor.Stop();
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
