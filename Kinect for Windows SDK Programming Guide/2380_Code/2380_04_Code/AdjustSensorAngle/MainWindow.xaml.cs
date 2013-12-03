
namespace AdjustSensorAngle
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
        ///  Define the kinect sensor
        /// </summary>
        private KinectSensor sensor;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(this.MainWindow_Loaded);
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

                // start the sensor
                this.sensor.Start();

                // Enable the color stream if it's not enabled
                if (!this.sensor.ColorStream.IsEnabled)
                {
                    this.sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
                }

                this.sensor.ColorFrameReady += new EventHandler<ColorImageFrameReadyEventArgs>(this.sensor_ColorFrameReady);
            }
            else
            {
                MessageBox.Show("No Device Conneted");
            }

            this.currentAngle.Content = string.Format("Current Elevation Angle : {0 }", this.sensor.ElevationAngle.ToString());
        }

        /// <summary>
        /// Handles the ColorFrameReady event of the sensor control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Microsoft.Kinect.ColorImageFrameReadyEventArgs"/> instance containing the event data.</param>
      protected void sensor_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
           // Get the current color image frame
            using (ColorImageFrame imageFrame = e.OpenColorImageFrame())
            {
                // return if there is any dropped image frame.
                if (imageFrame == null)
                {
                    return;
                }

                // Calculate the byte[] for the image frame data length
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
            }
        }

        /// <summary>
        /// Sets the sensor angle.
        /// </summary>
        /// <param name="angleValue">The angle value.</param>
        private void SetSensorAngle(int angleValue)
        {
            if (angleValue > this.sensor.MinElevationAngle || angleValue < this.sensor.MaxElevationAngle)
            {
                this.sensor.ElevationAngle = angleValue;
            }
        }

        /// <summary>
        /// Gets the maximum elavation angle.
        /// </summary>
        /// <returns></returns>
        private int GetMaximumElavationAngle()
        {
            return this.sensor.MaxElevationAngle;
        }

        /// <summary>
        /// Gets the maximum elavation angle.
        /// </summary>
        /// <returns></returns>
        private int GetMinimumElavationAngle()
        {
            return this.sensor.MinElevationAngle;
        }

        /// <summary>
        /// Handles the Click event of the motorUp control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void motorUp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.sensor.ElevationAngle += 5;
                this.currentAngle.Content = string.Format("Current Elevation Angle : {0 }", this.sensor.ElevationAngle.ToString());
            }
            catch (ArgumentOutOfRangeException argumentOutOfRange)
            {
                MessageBox.Show(argumentOutOfRange.Message);
            }
        }

        /// <summary>
        /// Handles the Click event of the motorDown control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void motorDown_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.sensor.ElevationAngle -= 5;
                this.currentAngle.Content = string.Format("Current Elevation Angle : {0 }", this.sensor.ElevationAngle.ToString());
            }
            catch (ArgumentOutOfRangeException aorExc)
            {
                MessageBox.Show(aorExc.Message);
            }
        }

        /// <summary>
        /// Handles the Click event of the motorSet control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void motorSet_Click(object sender, RoutedEventArgs e)
        {
            int angleValue;

            // Check if the entered data is not valid.
            if (!int.TryParse(cameraAngle.Text, out angleValue))
            {
                MessageBox.Show("The sensor angle value is not valid !");
                return;
            }

            // return if the current angle value and given value are same
            if (this.sensor.ElevationAngle == angleValue)
            {
                return;
            }

            // check for maximum elevation angle
            if (angleValue > this.sensor.MaxElevationAngle)
            {
                MessageBox.Show(string.Format("The camera angle cannot be higher than {0}!", this.sensor.MaxElevationAngle));
                return;
            }

            // check for minimum elevation angle
            if (angleValue < this.sensor.MinElevationAngle)
            {
                MessageBox.Show(string.Format("The camera angle cannot be lower than {0}!", this.sensor.MinElevationAngle));
                return;
            }

            // Change the elevation angle
            try
            {
                this.sensor.ElevationAngle = angleValue;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                this.currentAngle.Content = string.Format("Current Elevation Angle : {0 }", this.sensor.ElevationAngle.ToString());
            }
        }
    }
}
