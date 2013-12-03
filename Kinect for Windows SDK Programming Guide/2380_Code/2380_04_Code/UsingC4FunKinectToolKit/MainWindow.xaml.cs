// Kinect for Windows SDK Programming Guide
// Chapter 05
// Using Coding 4 Fun Toolkit Sample
//-------------------------------------------------------------

namespace UsingC4FunKinectToolKit
{
    using System;
    using System.Windows;
    using Coding4Fun.Kinect.Wpf;
    using Microsoft.Kinect;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        ///  Define the kinect sensor
        /// </summary>
        private KinectSensor sensor;

        /// <summary>
        /// Gets or sets the pixel data.
        /// </summary>
        /// <value>The pixel data.</value>
        private byte[] pixelData { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(this.MainWindow_Loaded);
            Unloaded += new RoutedEventHandler(MainWindow_Unloaded);
        }

        /// <summary>
        /// Handles the Unloaded event of the MainWindow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void MainWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            sensor.Stop();
        }

        /// <summary>
        /// Handles the Loaded event of the MainWindow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        protected void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {

            // Check if there kinect connected
            if (KinectSensor.KinectSensors.Count > 0)
            {
                // Get the first kinect sensor
                this.sensor = KinectSensor.KinectSensors[0];
                KinectSensor sensor1 = KinectSensor.KinectSensors[0];
               
                // start the sensor
                this.sensor.Start();

                this.sensor.ColorStream.Disable();
                if (!this.sensor.ColorStream.IsEnabled)
                {
                    this.sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
                    this.sensor.ColorFrameReady += new EventHandler<ColorImageFrameReadyEventArgs>(this.sensor_ColorFrameReady);
                }
            }
            else
            {
                MessageBox.Show("No Device Conneted");
            }
        }

        void sensor_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (ColorImageFrame imageFrame = e.OpenColorImageFrame())
            {
                // Check if the incoming frame is not null
                if (imageFrame != null)
                {
                    // Get the piel data in byte array
                    this.pixelData = new byte[imageFrame.PixelDataLength];

                    // Copy the pixel data
                    imageFrame.CopyPixelDataTo(this.pixelData);

                    // Using C4F Extension method ToBitMapSource
                    this.colorimageControl.Source = imageFrame.ToBitmapSource();
                }
            }
        }

    }
}