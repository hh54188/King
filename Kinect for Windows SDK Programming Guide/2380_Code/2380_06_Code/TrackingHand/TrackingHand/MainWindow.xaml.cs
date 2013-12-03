/*Working Solution- Hand Tracking with Joint Position Display: 
 * The application tracks your right hand and displays the hand movement and joint position. 
 */

namespace TrackingHand
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using Microsoft.Kinect;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// The sensor
        /// </summary>
        KinectSensor sensor;

        /// <summary>
        /// Total number of skeleton can be tracked.
        /// </summary>
        Skeleton[] totalSkeleton = new Skeleton[6];

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow" /> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(MainWindow_Loaded);
        }

        /// <summary>
        /// Handles the Loaded event of the MainWindow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Check if sensor is connected
            if (KinectSensor.KinectSensors.Count == 0)
            {
                MessageBox.Show("No Device Connected");
                Application.Current.Shutdown();
                return;
            }

            // get the reference of first connected sensor from collection
            this.sensor = KinectSensor.KinectSensors.Where(item => item.Status == KinectStatus.Connected).FirstOrDefault();

            // check if the skeleton stream is enabled. if yes, then just start the sensor, else enable and attach the event handler.
            if (!this.sensor.SkeletonStream.IsEnabled)
            {
                this.sensor.SkeletonStream.Enable();
                this.sensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated;
                this.sensor.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(sensor_SkeletonFrameReady);
            }

            // start the sensor.
            this.sensor.Start();
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
                // check for frame drop.
                if (skeletonFrame == null)
                {
                    return;
                }
                // copy the frame data in to the collection
                skeletonFrame.CopySkeletonDataTo(totalSkeleton);

                // get the first Tracked skeleton
                Skeleton firstSkeleton = (from trackskeleton in totalSkeleton
                                          where trackskeleton.TrackingState == SkeletonTrackingState.Tracked
                                          select trackskeleton).FirstOrDefault();

                // if the first skeleton returns null
                if (firstSkeleton == null)
                {
                    return;
                }

                // As we are checking for Right Hand, check if Right Hand Tracking State.  Call the mapping method only if the joint is tracked.
                if (firstSkeleton.Joints[JointType.HandRight].TrackingState == JointTrackingState.Tracked)
                {
                    this.MapJointsWithUIElement(firstSkeleton);
                }
            }
        }

        /// <summary>
        /// Maps the joints with UI element.
        /// </summary>
        /// <param name="skeleton">The skeleton.</param>
        private void MapJointsWithUIElement(Skeleton skeleton)
        {
            // Get the Points in 2D Space to map in UI. for that call the Scale postion method.
            Point mappedPoint = this.ScalePosition(skeleton.Joints[JointType.HandRight].Position);
            myhandPosition.Content = string.Format("X:{0},Y:{1}", mappedPoint.X, mappedPoint.Y);
            Canvas.SetLeft(righthand, mappedPoint.X);
            Canvas.SetTop(righthand, mappedPoint.Y);
        }

        /// <summary>
        /// Scales the position.
        /// </summary>
        /// <param name="skeletonPoint">The skeltonpoint.</param>
        /// <returns></returns>
        private Point ScalePosition(SkeletonPoint skeletonPoint)
        {
            // return the depth points from the skeleton point
            DepthImagePoint depthPoint = this.sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(skeletonPoint, DepthImageFormat.Resolution640x480Fps30);
            return new Point(depthPoint.X, depthPoint.Y);
        }

    }
}
