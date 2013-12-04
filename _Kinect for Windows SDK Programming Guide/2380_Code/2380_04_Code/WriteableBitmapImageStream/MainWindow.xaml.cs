// Kinect for Windows SDK Programming Guide
// Chapter 04
// Adjust Kinect Sensor Angle
namespace WriteableBitmapImageStream
{
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Microsoft.Kinect;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Writeable Bitmap
        /// </summary>
        private WriteableBitmap writeableBitmap;

        /// <summary>
        /// The Sensor
        /// </summary>
        private KinectSensor sensor;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow" /> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            Unloaded += MainWindow_Unloaded;
        }

        /// <summary>
        /// Handles the Unloaded event of the MainWindow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        void MainWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            if (this.sensor != null && this.sensor.IsRunning)
            {
                this.sensor.Stop();
            }
        }

        /// <summary>
        /// Handles the Loaded event of the MainWindow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        protected void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (KinectSensor.KinectSensors.Count > 0)
            {
                this.sensor = KinectSensor.KinectSensors[0];
                this.sensor.Start();
                if (this.sensor !=null & this.sensor.IsRunning && !this.sensor.ColorStream.IsEnabled)
                {
                    this.sensor.ColorStream.Enable();
                    this.writeableBitmap = new WriteableBitmap(this.sensor.ColorStream.FrameWidth,
                                                                    this.sensor.ColorStream.FrameHeight, 96, 96,
                                                                    PixelFormats.Bgr32, null);
                    VideoControl.Source = this.writeableBitmap;
                    this.sensor.ColorFrameReady += sensor_ColorFrameReady;
                }
            }
            else
            {
                MessageBox.Show("No Device Connected");
            }
        }

        /// <summary>
        /// Handles the ColorFrameReady event of the sensor control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ColorImageFrameReadyEventArgs" /> instance containing the event data.</param>
        void sensor_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (ColorImageFrame imageFrame = e.OpenColorImageFrame())
            {
                if (imageFrame != null)
                {
                    byte[] pixelData = new byte[imageFrame.PixelDataLength];
                    imageFrame.CopyPixelDataTo(pixelData);
                    int stride = imageFrame.Width * imageFrame.BytesPerPixel;
                    this.writeableBitmap.WritePixels(
                        new Int32Rect(0, 0, this.writeableBitmap.PixelWidth, this.writeableBitmap.PixelHeight),
                        pixelData,
                        stride,
                        0);
                }
            }
        }
    }
}
