// Kinect for Windows SDK Programming Guide
// Chapter 04
// Get Maximum and minimum elevation angle
//-------------------------------------------------------------

namespace MaxMinElevationAngle
{
    using System.Windows;
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
            }
            else
            {
                MessageBox.Show("No Device Conneted");
            }
        }

        /// <summary>
        /// Handles the Click event of the maximumElevationAngle control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void MaximumElevationAngle_Click(object sender, RoutedEventArgs e)
        {
            this.maxElevationAngle.Text = string.Format("Maximum Elevation Angle : {0}", this.GetMaximumElavationAngle().ToString());
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
        /// <returns>sensor max angle</returns>
        private int GetMaximumElavationAngle()
        {
            return this.sensor.MaxElevationAngle;
        }

        /// <summary>
        /// Gets the minimum elavation angle.
        /// </summary>
        /// <returns>sensor mim angel</returns>
        private int GetMinimumElavationAngle()
        {
            return this.sensor.MinElevationAngle;
        }

        /// <summary>
        /// Handles the Click event of the minimumElevationAngle control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void MinimumElevationAngle_Click(object sender, RoutedEventArgs e)
        {
            this.minElevationAngle.Text = string.Format("Minimum Elevation Angle: {0} ", this.GetMinimumElavationAngle().ToString());
        }
    }
}
