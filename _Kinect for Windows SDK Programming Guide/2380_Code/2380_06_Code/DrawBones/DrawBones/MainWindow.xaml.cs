/*Drawing Bones: This solution tracks all the joints that construct the complete right hand bone sequence.
 * This will work in both seated and default skeleton tracking mode.*/

namespace DrawBones
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Shapes;
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

        Skeleton skeleton;

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
                skeleton = (from trackskeleton in totalSkeleton
                                          where trackskeleton.TrackingState == SkeletonTrackingState.Tracked
                                          select trackskeleton).FirstOrDefault();

                // if the first skeleton returns null
                if (skeleton == null)
                    return;  

                // As we are checking for Right Hand and Shoulder Center to make sure the complete bone sequence is tracked.
                if (skeleton.Joints[JointType.ShoulderCenter].TrackingState == JointTrackingState.Tracked && skeleton.Joints[JointType.HandRight].TrackingState == JointTrackingState.Tracked)
                {
                    myCanvas.Children.Clear();
                    this.DrawRightHandBoneSequence();
                }
            }
        }

   
        /// <summary>
        /// Draws the right arm.
        /// </summary>
        private void DrawRightHandBoneSequence()
        {
            if (skeleton != null)
            {
                drawBone(skeleton.Joints[JointType.ShoulderCenter], skeleton.Joints[JointType.ShoulderRight]);
                drawBone(skeleton.Joints[JointType.ShoulderRight], skeleton.Joints[JointType.ElbowRight]);
                drawBone(skeleton.Joints[JointType.ElbowRight], skeleton.Joints[JointType.WristRight]);
                drawBone(skeleton.Joints[JointType.WristRight], skeleton.Joints[JointType.HandRight]);
            }
        }

        /// <summary>
        /// Draws the bone.
        /// </summary>
        /// <param name="trackedJoint1">The tracked joint1.</param>
        /// <param name="trackedJoint2">The tracked joint2.</param>
        void drawBone(Joint trackedJoint1, Joint trackedJoint2)
        {
            Line skeletonBone = new Line();
            skeletonBone.Stroke = Brushes.Black;
            skeletonBone.StrokeThickness = 3;

            Point joint1 = this.ScalePosition(trackedJoint1.Position);
            skeletonBone.X1 = joint1.X;
            skeletonBone.Y1 = joint1.Y;

            Point joint2 = this.ScalePosition(trackedJoint2.Position);
            skeletonBone.X2 = joint2.X;
            skeletonBone.Y2 = joint2.Y;

            myCanvas.Children.Add(skeletonBone);
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
