
namespace MultipleKinectViewer
{
    using KinectStatusNotifier;
    using Microsoft.Kinect;
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// The view model
        /// </summary>
        public MainWindowModelView ViewModel;

        /// <summary>
        /// The kinect sensor info
        /// </summary>
        ObservableCollection<KinectInfo> kinectSensorInfo = new ObservableCollection<KinectInfo>();

        /// <summary>
        /// The notifier
        /// </summary>
        private StatusNotifier notifier = new StatusNotifier();

        /// <summary>
        /// The numberof device
        /// </summary>
        int numberofDevice = 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow" /> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(MainWindow_Loaded);
            KinectSensor.KinectSensors.StatusChanged += new EventHandler<StatusChangedEventArgs>(KinectSensors_StatusChanged);
            this.notifier.Sensors = KinectSensor.KinectSensors;
            this.notifier.AutoNotification = false;
            this.ViewModel = new MainWindowModelView();
            this.DataContext = this.ViewModel;
        }

        /// <summary>
        /// Handles the StatusChanged event of the KinectSensors control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="StatusChangedEventArgs" /> instance containing the event data.</param>
        void KinectSensors_StatusChanged(object sender, StatusChangedEventArgs e)
        {

            KinectInfo kinfo = this.kinectSensorInfo.FirstOrDefault(item => item.ConnectionID.Equals(e.Sensor.DeviceConnectionId));

            if (kinfo != null)
            {
                kinfo.Status = e.Status.ToString();
                this.notifier.NotifierTitle = kinfo.deviceCount;
                this.notifier.NotifierMessage = string.Format("Sensor Status :{0} \nDevice Id: {1}", kinfo.Status, kinfo.DeviceID);
                this.notifier.NotifyStatus();
            }
            else
            {
                kinectSensorInfo.Add(new KinectInfo
                {
                    deviceCount = string.Format("Device  {0}", numberofDevice++),
                    DeviceID = e.Sensor.UniqueKinectId,
                    ConnectionID = e.Sensor.DeviceConnectionId,
                    Status = e.Sensor.Status.ToString()
                });
            }

        }

        /// <summary>
        /// Handles the Loaded event of the MainWindow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            int count = KinectSensor.KinectSensors.Count;

            this.ViewModel.NumberofDevice = count;
            if (count > 0)
            {
                foreach (KinectSensor sensor in KinectSensor.KinectSensors)
                {
                    kinectSensorInfo.Add(new KinectInfo
                    {
                        deviceCount = string.Format("Device  {0}", numberofDevice++),
                        DeviceID = sensor.UniqueKinectId,
                        ConnectionID = sensor.DeviceConnectionId,
                        Status = sensor.Status.ToString()
                    });
                }
                lstsensor.DataContext = kinectSensorInfo;
            }
        }
    }
}