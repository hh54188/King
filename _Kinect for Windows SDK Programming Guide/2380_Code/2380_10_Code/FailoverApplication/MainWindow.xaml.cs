
namespace FailoverApplication
{
    using Microsoft.Kinect;
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    /// <summary>
    /// Main Window
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// The sensor1
        /// </summary>
        private KinectSensor sensor1;

        /// <summary>
        /// The sensor2
        /// </summary>
        private KinectSensor sensor2;

        /// <summary>
        /// The kinect sensor info
        /// </summary>
        ObservableCollection<KinectInfo> kinectSensorInfo = new ObservableCollection<KinectInfo>();

        /// <summary>
        /// The current active ID
        /// </summary>
        private Guid currentActiveID;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow" /> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        /// <summary>
        /// Handles the Loaded event of the MainWindow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.StartDevice1();
            KinectSensor.KinectSensors.StatusChanged += KinectSensors_StatusChanged;
            lstsensor.DataContext = kinectSensorInfo;
        }

        /// <summary>
        /// Starts the device2.
        /// </summary>
        private void StartDevice2()
        {
            this.sensor2 = KinectSensor.KinectSensors.FirstOrDefault(sensorItem => sensorItem.Status == KinectStatus.Connected);
            if (this.sensor2 != null)
            {
                this.sensor2.ColorStream.Enable();
                this.sensor2.ColorFrameReady += sensor2_ColorFrameReady;
                this.sensor2.Start();
                this.currentActiveID = Guid.NewGuid();
                kinectSensorInfo.Add(new KinectInfo { ID = this.currentActiveID, DeviceID = sensor2.UniqueKinectId, ConnectionID = sensor2.DeviceConnectionId, Started = DateTime.Now.ToShortTimeString() });
            }
        }

        /// <summary>
        /// Starts the device1.
        /// </summary>
        private void StartDevice1()
        {
            this.sensor1 = KinectSensor.KinectSensors.FirstOrDefault(sensorItem => sensorItem.Status == KinectStatus.Connected);

            if (this.sensor1 != null)
            {
                this.sensor1.ColorStream.Enable();
                this.sensor1.ColorFrameReady += sensor1_ColorFrameReady;
                this.sensor1.Start();
                this.currentActiveID = Guid.NewGuid();
                kinectSensorInfo.Add(new KinectInfo { ID = this.currentActiveID, DeviceID = sensor1.UniqueKinectId, ConnectionID = sensor1.DeviceConnectionId, Started = DateTime.Now.ToShortTimeString() });
            }
        }

        /// <summary>
        /// Handles the StatusChanged event of the KinectSensors control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="StatusChangedEventArgs" /> instance containing the event data.</param>
        void KinectSensors_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            switch (e.Status)
            {
                case KinectStatus.Connected:
                    break;
                case KinectStatus.Disconnected:
                    CheckIfNeedToStart(e.Sensor);
                    break;
                case KinectStatus.Error:
                    CheckIfNeedToStart(e.Sensor);
                    break;
                case KinectStatus.InsufficientBandwidth:
                    CheckIfNeedToStart(e.Sensor);
                    break;
                case KinectStatus.NotPowered:
                    CheckIfNeedToStart(e.Sensor);
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Checks if need to start.
        /// </summary>
        /// <param name="kinectSensor">The kinect sensor.</param>
        private void CheckIfNeedToStart(KinectSensor kinectSensor)
        {
            if (kinectSensor.IsRunning)
            {
                if (kinectSensor.UniqueKinectId == sensor1.UniqueKinectId)
                {
                    this.StopDevice1();
                    this.StartDevice2();
                }
                else
                {
                    this.StopDevice2();
                    this.StartDevice1();
                }
            }
        }

        /// <summary>
        /// Stops the device2.
        /// </summary>
        private void StopDevice2()
        {
            if (this.sensor2 != null && this.sensor2.IsRunning)
            {
                KinectInfo kinfo = this.kinectSensorInfo.FirstOrDefault(item => item.ID == this.currentActiveID);
                this.sensor2.Stop();
            }
        }

        /// <summary>
        /// Stops the device1.
        /// </summary>
        private void StopDevice1()
        {
            if (this.sensor1 != null && this.sensor1.IsRunning)
            {
                KinectInfo kinfo = this.kinectSensorInfo.FirstOrDefault(item => item.ID == this.currentActiveID);
                this.sensor1.Stop();
            }
        }

        /// <summary>
        /// Handles the ColorFrameReady event of the sensor2 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ColorImageFrameReadyEventArgs" /> instance containing the event data.</param>
        void sensor2_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (ColorImageFrame imageframe = e.OpenColorImageFrame())
            {
                if (imageframe == null)
                {
                    return;
                }

                byte[] pixelData = new byte[imageframe.PixelDataLength];

                imageframe.CopyPixelDataTo(pixelData);

                SensorAImageViewer.Source = BitmapSource.Create(imageframe.Width, imageframe.Height, 96, 96, PixelFormats.Bgr32, null, pixelData, imageframe.Width * 4);

            }
        }

        /// <summary>
        /// Handles the ColorFrameReady event of the sensor1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ColorImageFrameReadyEventArgs" /> instance containing the event data.</param>
        void sensor1_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (ColorImageFrame imageframe = e.OpenColorImageFrame())
            {
                if (imageframe == null)
                {
                    return;
                }

                byte[] pixelData = new byte[imageframe.PixelDataLength];

                imageframe.CopyPixelDataTo(pixelData);

                SensorAImageViewer.Source = BitmapSource.Create(imageframe.Width, imageframe.Height, 96, 96, PixelFormats.Bgr32, null, pixelData, imageframe.Width * 4);

            }
        }

    }
}
