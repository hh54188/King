

namespace KinectInfoBox
{
    using System.Windows;
    using Microsoft.Kinect;
    using KinectStatusNotifier;
    using System.Linq;
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// The instance of Kinect sensor
        /// </summary>
        private KinectSensor sensor;

        /// <summary>
        /// The Main window View Model
        /// </summary>
        private MainWindowViewModel viewModel;

        /// <summary>
        /// Status Notifier
        /// </summary>
        private StatusNotifier notifier = new StatusNotifier();

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow" /> class.
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();
            this.Loaded += this.MainWindow_Loaded;
            this.viewModel = new MainWindowViewModel();
            this.viewModel.CanStart = false;
            this.viewModel.CanStop = false;
            this.DataContext = this.viewModel;
        }

        /// <summary>
        /// Handles the Loaded event of the MainWindow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        protected void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (KinectSensor.KinectSensors.Count > 0)
            {
                this.notifier.Sensors = KinectSensor.KinectSensors;
                this.sensor = KinectSensor.KinectSensors[0];
                KinectSensor.KinectSensors.StatusChanged += new System.EventHandler<StatusChangedEventArgs>(KinectSensors_StatusChanged);
                this.StartSensor();
                this.sensor.ColorStream.Enable();
                this.sensor.DepthStream.Enable();
                this.sensor.SkeletonStream.Enable();

                this.SetKinectInfo();
            }
            else
            {
                MessageBox.Show("No device is connected with system!");
                this.Close();
            }
        }

        /// <summary>
        /// Status changed for Kinect sensor
        /// </summary>
        /// <param name="sender">Kinect Sensor</param>
        /// <param name="e">Event Args</param>
        void KinectSensors_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            this.viewModel.SensorStatus = e.Status.ToString();
        }

        /// <summary>
        /// Sets the Kinect info.
        /// </summary>
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

        /// <summary>
        /// Handles the Click event of the ButtonStart control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void ButtonStart_Click(object sender, RoutedEventArgs e)
        {
            this.StartSensor();
        }

        /// <summary>
        /// Handles the Click event of the ButtonStop control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void ButtonStop_Click(object sender, RoutedEventArgs e)
        {
            this.StopSensor();
        }

        /// <summary>
        /// Handles the Click event of the ButtonExit control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            this.StopSensor();
            this.Close();
        }

        /// <summary>
        /// Starts the sensor.
        /// </summary>
        private void StartSensor()
        {
            if (this.sensor != null && !this.sensor.IsRunning)
            {
                this.sensor.Start();
                this.viewModel.CanStart = false;
                this.viewModel.CanStop = true;
            }
        }

        /// <summary>
        /// Stops the sensor.
        /// </summary>
        private void StopSensor()
        {
            if (this.sensor != null && this.sensor.IsRunning)
            {
                this.sensor.Stop();
                this.viewModel.CanStart = true;
                this.viewModel.CanStop = false;
            }
        }

    }
}
