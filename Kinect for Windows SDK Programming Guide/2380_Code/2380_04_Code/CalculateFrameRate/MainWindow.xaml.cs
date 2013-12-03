// Kinect for Windows SDK Programming Guide
// Chapter 04
// Get Maximum and minimum elevation angle
//---------------------------------------------------------------

namespace CalculateFrameRate
{
    using System;
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
        /// Kinect Sensor
        /// </summary>
        private KinectSensor sensor;

        /// <summary>
        /// maximum date time value
        /// </summary>
        private DateTime lastTime = DateTime.MaxValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(this.MainWindow_Loaded);
            Unloaded += new RoutedEventHandler(this.MainWindow_Unloaded);
        }

        /// <summary>
        /// Gets or sets the image format.
        /// </summary>
        /// <value>The image format.</value>
        public ColorImageFormat ImageFormat { get; set; }

        /// <summary>
        /// Gets or sets the total frames.
        /// </summary>
        /// <value>The total frames.</value>
        protected int TotalFrames { get; set; }

        /// <summary>
        /// Gets or sets the frame rate.
        /// </summary>
        /// <value>The frame rate.</value>
        private int FrameRate { get; set; }

        /// <summary>
        /// Gets or sets the last frames.
        /// </summary>
        /// <value>The last frames.</value>
        private int LastFrames { get; set; }

        /// <summary>
        /// Handles the Loaded event of the MainWindow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        protected void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Check if Kinect is Connected
            if (KinectSensor.KinectSensors.Count > 0)
            {
                this.StartKinect();
                this.BindImageFormat();
            }
            else
            {
                MessageBox.Show("No Device Connected");
            }
        }

        /// <summary>
        /// Handles the Unloaded event of the MainWindow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        protected void MainWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            this.StopKinect();
        }

        /// <summary>
        /// Stops the kinect.
        /// </summary>
        private void StopKinect()
        {
            if (this.sensor != null)
            {
                this.sensor.Stop();
            }
        }

        /// <summary>
        /// Starts the kinect.
        /// </summary>
        private void StartKinect()
        {
            this.sensor = KinectSensor.KinectSensors[0];
            this.sensor.Start();
            this.sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
            this.sensor.ColorFrameReady += new System.EventHandler<ColorImageFrameReadyEventArgs>(this.Sensor_ColorFrameReady);
        }

        /// <summary>
        /// Handles the ColorFrameReady event of the sensor control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Microsoft.Kinect.ColorImageFrameReadyEventArgs"/> instance containing the event data.</param>
        private void Sensor_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (ColorImageFrame imageFrame = e.OpenColorImageFrame())
            {
                if (imageFrame == null)
                {
                    return;
                }

                byte[] pixelData = new byte[imageFrame.PixelDataLength];

                imageFrame.CopyPixelDataTo(pixelData);

                this.colorimageControl.Source = BitmapSource.Create(
                    imageFrame.Width,
                    imageFrame.Height,
                    96,
                    96,
                    PixelFormats.Bgr32,
                    null,
                    pixelData,
                    imageFrame.Width * 4);

                this.CalculateFrameRate();
                labelFPS.Content = string.Format("Frame Rate : {0}", this.FrameRate.ToString());
            }
        }

        /// <summary>
        /// Handles the SelectionChanged event of the colorFormatControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void ColorFormatControl_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (this.sensor.IsRunning)
            {
                this.ImageFormat = (ColorImageFormat)this.colorFormatControl.SelectedItem;
                this.sensor.ColorStream.Enable(this.ImageFormat == ColorImageFormat.Undefined ? ColorImageFormat.RgbResolution640x480Fps30 : this.ImageFormat);
            }
        }

        /// <summary>
        /// Binds the image format.
        /// </summary>
        private void BindImageFormat()
        {
            foreach (ColorImageFormat colorImageFormat in Enum.GetValues(typeof(ColorImageFormat)))
            {
                this.colorFormatControl.Items.Add(colorImageFormat);
            }

            // Set Default Selected ColorImageFormat.RgbResolution640x480Fps30
            this.colorFormatControl.SelectedIndex = 1;
        }

        /// <summary>
        /// Calculates the frame rate.
        /// </summary>
        private void CalculateFrameRate()
        {
            ++this.TotalFrames;
            DateTime currentTime = DateTime.Now;
            var span = currentTime.Subtract(this.lastTime);
            if (this.lastTime == DateTime.MaxValue || span >= TimeSpan.FromSeconds(1))
            {
                this.FrameRate = (int)Math.Round((this.TotalFrames - this.LastFrames) / span.TotalSeconds);
                this.LastFrames = this.TotalFrames;
                this.lastTime = currentTime;
            }
        }
   }
}
