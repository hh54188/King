/*Monitoring Skeleton Change: The Application shows the list of skeleton TrackingId,
 * Tracked Time along with the total number of joints was tracked for the skeleton. 
 * The list updates only when there is a new skeleton tracks by the sensor 
 * for the first skeleton position. */

namespace MonitoringSkeletonChange
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows;
    using Microsoft.Kinect;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Current Skeleton ID
        /// </summary>
        public int currentSkeletonID = 0;

        /// <summary>
        /// The sensor
        /// </summary>
        private KinectSensor sensor;

        /// <summary>
        /// Placeholder for skeleton
        /// </summary>
        private Skeleton[] totalSkeleton = new Skeleton[6];

        /// <summary>
        /// Observable Collection for Skeleton Tracking Info
        /// </summary>
        ObservableCollection<SkeletonTrackingInfo> skeletonTrackingItemCollection = new ObservableCollection<SkeletonTrackingInfo>();

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow" /> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(this.MainWindow_Loaded);
            this.trackingIDListBox.ItemsSource = this.skeletonTrackingItemCollection;
            this.DataContext = this;
        }

        /// <summary>
        /// Handles the Loaded event of the MainWindow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (KinectSensor.KinectSensors.Count > 0)
            {
                this.sensor = KinectSensor.KinectSensors.Where(item => item.Status == KinectStatus.Connected).FirstOrDefault();

                if (!this.sensor.SkeletonStream.IsEnabled)
                {
                    this.sensor.SkeletonStream.Enable();
                    this.sensor.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(this.sensor_SkeletonFrameReady);
                }
                this.sensor.Start();
            }
            else
            {
                MessageBox.Show("No Device Connected");
                Application.Current.Shutdown();
                return;
            }
        }

        /// <summary>
        /// Handles the SkeletonFrameReady event of the sensor control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SkeletonFrameReadyEventArgs" /> instance containing the event data.</param>
        void sensor_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame == null)
                {
                    return;
                }

                skeletonFrame.CopySkeletonDataTo(this.totalSkeleton);

                // Get the first one from the tracked skeleton
                Skeleton skeleton = (from trackskeleton in this.totalSkeleton
                                     where trackskeleton.TrackingState == SkeletonTrackingState.Tracked
                                     select trackskeleton).FirstOrDefault();

                // Check for not matched Tracking ID
                if (skeleton != null && this.currentSkeletonID != skeleton.TrackingId)
                {
                    this.currentSkeletonID = skeleton.TrackingId;
                    int totalTrackedJoints = skeleton.Joints.Where(item => item.TrackingState == JointTrackingState.Tracked).Count();
                    this.skeletonTrackingItemCollection.Add(new SkeletonTrackingInfo { TrackingID = this.currentSkeletonID, TrackedTime = DateTime.Now.ToString("hh:mm:ss"), TotalTrackedJoints = totalTrackedJoints });
                }
            }
        }

    }
}
