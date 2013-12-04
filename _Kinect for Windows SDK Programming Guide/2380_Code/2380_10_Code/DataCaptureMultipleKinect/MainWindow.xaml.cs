
namespace DataCaptureMultipleKinect
{
    using Microsoft.Kinect;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
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
            this.sensor1 = KinectSensor.KinectSensors[0];
            this.sensor1.ColorStream.Enable();
            this.sensor1.ColorFrameReady += sensor1_ColorFrameReady;
            this.sensor1.Start();

            this.sensor2 = KinectSensor.KinectSensors[1];
            this.sensor2.ColorStream.Enable();
            this.sensor2.ColorFrameReady += sensor2_ColorFrameReady;
            this.sensor2.Start();
        }

        /// <summary>
        /// Handles the DepthFrameReady event of the sensor2 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="DepthImageFrameReadyEventArgs" /> instance containing the event data.</param>
        void sensor2_DepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            using (DepthImageFrame depthimageFrame = e.OpenDepthImageFrame())
            {
                if (depthimageFrame == null)
                {
                    return;
                }

                short[] pixelData = new short[depthimageFrame.PixelDataLength];
                int stride = depthimageFrame.Width * 2;
                depthimageFrame.CopyPixelDataTo(pixelData);

                SensorBImageViewer.Source = BitmapSource.Create(
                   depthimageFrame.Width, depthimageFrame.Height, 96, 96, PixelFormats.Gray16, null, pixelData, stride);
            }
        }

        /// <summary>
        /// Handles the DepthFrameReady event of the sensor1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="DepthImageFrameReadyEventArgs" /> instance containing the event data.</param>
        void sensor1_DepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            using (DepthImageFrame depthimageFrame = e.OpenDepthImageFrame())
            {
                if (depthimageFrame == null)
                {
                    return;
                }

                short[] pixelData = new short[depthimageFrame.PixelDataLength];
                int stride = depthimageFrame.Width * 2;
                depthimageFrame.CopyPixelDataTo(pixelData);

                SensorAImageViewer.Source = BitmapSource.Create(
                   depthimageFrame.Width, depthimageFrame.Height, 96, 96, PixelFormats.Gray16, null, pixelData, stride);
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

                SensorBImageViewer.Source = BitmapSource.Create(imageframe.Width, imageframe.Height, 96, 96, PixelFormats.Bgr32, null, pixelData, imageframe.Width * 4);
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

        /// <summary>
        /// Handles the event event of the CheckBoxDevice1DepthCheck control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void CheckBoxDevice1DepthCheck_event(object sender, RoutedEventArgs e)
        {
            CheckBox chkbox = (CheckBox)sender;
            if (chkbox.IsChecked == true)
            {
                EnableDepthStreamforDevice1();
            }
            else if (chkbox.IsChecked == false)
            {
                EnableColorStreamforDevice1();
            }
        }

        /// <summary>
        /// Enables the color streamfor device1.
        /// </summary>
        private void EnableColorStreamforDevice1()
        {
            if (this.sensor1.DepthStream.IsEnabled)
            {
                this.sensor1.DepthStream.Disable();
                this.sensor1.ColorStream.Enable();
                this.sensor1.ColorFrameReady += sensor1_ColorFrameReady;
            }
        }

        /// <summary>
        /// Enables the depth streamfor device1.
        /// </summary>
        private void EnableDepthStreamforDevice1()
        {
            if (this.sensor1.ColorStream.IsEnabled)
            {
                this.sensor1.ColorStream.Disable();
                this.sensor1.DepthStream.Enable();
                this.sensor1.DepthFrameReady += sensor1_DepthFrameReady;
            }
        }

        /// <summary>
        /// Handles the event event of the CheckBoxDevice2DepthCheck control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void CheckBoxDevice2DepthCheck_event(object sender, RoutedEventArgs e)
        {
            CheckBox chkbox = (CheckBox)sender;
            if (chkbox.IsChecked == true)
            {
                EnableDepthStreamforDevice2();
            }
            else if (chkbox.IsChecked == false)
            {
                EnableColorStreamforDevice2();
            }
        }

        /// <summary>
        /// Enables the color streamfor device2.
        /// </summary>
        private void EnableColorStreamforDevice2()
        {
            if (this.sensor2.DepthStream.IsEnabled)
            {
                this.sensor2.DepthStream.Disable();
                this.sensor2.ColorStream.Enable();
                this.sensor2.ColorFrameReady += sensor2_ColorFrameReady;
            }
        }

        /// <summary>
        /// Enables the depth streamfor device2.
        /// </summary>
        private void EnableDepthStreamforDevice2()
        {
            if (this.sensor2.ColorStream.IsEnabled)
            {
                this.sensor2.ColorStream.Disable();
                this.sensor2.DepthStream.Enable();
                this.sensor2.DepthFrameReady += sensor2_DepthFrameReady;
            }
        }
    }
}
