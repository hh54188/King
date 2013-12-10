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

namespace KinectCam
{
    public partial class MainWindow : Window
    {
        private MainWindowViewModel viewModel;
        private KinectSensor sensor;
        private byte[] pixelData;
        // 计算frame rate
        private int TotalFrames { get; set; }
        private DateTime lastTime = DateTime.MaxValue;
        private int LastFrames { get; set; }
        int currentFrameRate = 0;

        public MainWindow()
        {
            InitializeComponent();
            viewModel = new MainWindowViewModel();
            // 此时viewModel还是空，这样赋值有用吗？
            // 是否需要放在Loaded += 的后面？
            this.DataContext = viewModel;
            this.Loaded += MainWindow_Loaded;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.StartSensor();
            // 给控件绑定事件
            this.ColorImageFormatSelection.SelectionChanged += ColorImageFormatSelection_SelectionChanged;
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
        private void StartKinectCam()
        {
            if (KinectSensor.KinectSensors.Count > 0)
            {
                // 这个是什么语法？
                this.sensor = KinectSensor.KinectSensors.FirstOrDefault(sensorItem => sensorItem.Status == KinectStatus.Connected);
                this.StartSensor();
                this.sensor.ColorStream.Enable();
                this.sensor.ColorFrameReady += sensor_ColorFrameReady;
            }
            else
            {
                MessageBox.Show("NO DEVICE!");
                this.Close();
            }
        }

        void sensor_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            // 使用using，可以在using结束时，回收所有using段内的内存。
            // 每一帧都要占不少内存，更何况每秒30帧
            using (ColorImageFrame imageFrame = e.OpenColorImageFrame())
            {
                if (imageFrame == null)
                {
                    return;
                }
                else
                {
                    this.pixelData = new byte[imageFrame.PixelDataLength];
                    imageFrame.CopyPixelDataTo(this.pixelData);

                    int stride = imageFrame.Width * imageFrame.BytesPerPixel;
                    this.VideoControl.Source = BitmapSource.Create(
                        imageFrame.Width,
                        imageFrame.Height,
                        96,
                        96,
                        PixelFormats.Bgr32,
                        null,
                        pixelData,
                        stride);

                    // IsFrameNumberEnabled默认是什么？在哪里设置的？
                    if (this.viewModel.IsFrameNumberEnabled)
                    {
                        this.viewModel.FrameNumber = this.GetCurrentFrameNumber(imageFrame);
                    }
                }
            }
        }
        private int GetCurrentFrameRate()
        {
            ++this.TotalFrames;
            DateTime currentTime = DateTime.Now;
            var timeSpan = currentTime.Subtract(this.lastTime);
            if (this.lastTime == DateTime.MaxValue || timeSpan >= TimeSpan.FromSeconds(1))
            {
                currentFrameRate = (int)Math.Round((this.TotalFrames - this.LastFrames) / timeSpan.TotalSeconds);
                this.LastFrames = this.TotalFrames;
                this.lastTime = currentTime;
            }
            return currentFrameRate;
        }
        private int GetCurrentFrameNumber(ColorImageFrame imageFrame)
        {
            return imageFrame.FrameNumber;
        }

        private void ColorImageFormatSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.ChangeColorImageFormat();
        }

        private void ChangeColorImageFormat()
        {
            if (this.sensor.IsRunning)
            {
                this.viewModel.CurrentImageFormat = (ColorImageFormat)this.ColorImageFormatSelection.SelectedItem;
                this.sensor.ColorStream.Enable(this.viewModel.CurrentImageFormat == ColorImageFormat.Undefined ? ColorImageFormat.RgbResolution640x480Fps30 : this.viewModel.CurrentImageFormat);
            }
        }
    }
}
