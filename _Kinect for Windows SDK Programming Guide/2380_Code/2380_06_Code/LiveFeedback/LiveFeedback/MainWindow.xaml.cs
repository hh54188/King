/*Working Solution- Live Feedback to User: 
 *The application will notify an indication to which direction user needs to move so that Kinect can track properly. 
 *You can also explore how sensor elevation change works when it’s required to move up. 
 *Top Edges Flag set - (Select allow auto adjut checkbox for that)
 *This works for both seated and default mode of skeleton tracking
 */
namespace LiveFeedback
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Shapes;
    using System.Windows.Threading;
    using Microsoft.Kinect;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        /// <summary>
        /// Kinect sensor
        /// </summary>
        KinectSensor sensor;

        /// <summary>
        /// Gets or sets a value indicating whether [auto adjust].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [auto adjust]; otherwise, <c>false</c>.
        /// </value>
        public bool AutoAdjust { get; set; }

        /// <summary>
        /// Timer instance
        /// </summary>
        private DispatcherTimer timer = new DispatcherTimer();

        /// <summary>
        /// Tick count
        /// </summary>
        private int TimerTickCount = 20;

        /// <summary>
        /// Default Feedback Brush
        /// </summary>
        public Brush defaultBrush = Brushes.White;

        /// <summary>
        /// Edges Feedback Brush
        /// </summary>
        public Brush EdgesBrushes = Brushes.Gray;

        /// <summary>
        /// The skeleton object
        /// </summary>
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
        /// Starts the timer.
        /// </summary>
        public void StartTimer()
        {

            this.timer.Interval = new TimeSpan(0, 0, 0);
            this.timer.Start();
            this.timer.Tick += this.Timer_Tick;
        }

        /// <summary>
        /// Handles the Loaded event of the MainWindow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (KinectSensor.KinectSensors.Count > 0)
            {
                // get the reference of first connected sensor from collection
                this.sensor = KinectSensor.KinectSensors.Where(item => item.Status == KinectStatus.Connected).FirstOrDefault();

                // check if the skeleton stream is enabled. if yes, then just start the sensor, else enable and attach the event handler.
                this.sensor.SkeletonStream.Enable();
                this.sensor.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(sensor_SkeletonFrameReady);

                // start the sensor.
                this.sensor.Start();
                this.sensor.ElevationAngle = 0;

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
            myCanvas.Children.Clear();
            Brush brush = new SolidColorBrush(Colors.Red);

            Skeleton[] skeletons = null;

            using (SkeletonFrame frame = e.OpenSkeletonFrame())
            {
                if (frame == null)
                {
                    return;
                }
                skeletons = new Skeleton[frame.SkeletonArrayLength];
                frame.CopySkeletonDataTo(skeletons);


                if (skeletons == null) return;
                skeleton = (from trackSkeleton in skeletons
                            where trackSkeleton.TrackingState == SkeletonTrackingState.Tracked
                            select trackSkeleton).FirstOrDefault();

                if (skeleton == null)
                {
                    this.ResetIndicator();
                    return;
                }

                this.DrawSkeleton(skeleton);
            }
        }

        /// <summary>
        /// Checks for clipped edges.
        /// </summary>
        /// <param name="skeleton">The skeleton.</param>
        private void CheckForClippedEdges(Skeleton skeleton)
        {
            switch (skeleton.ClippedEdges)
            {
                case FrameEdges.Bottom:
                    this.bottomedges.Background = EdgesBrushes;
                    break;
                case FrameEdges.Left:
                    this.leftedges.Background = EdgesBrushes;
                    break;
                case FrameEdges.None:
                    this.ResetIndicator();
                    break;
                case FrameEdges.Right:
                    this.rightedges.Background = EdgesBrushes;
                    break;
                case FrameEdges.Top:
                    this.topedges.Background = EdgesBrushes;
                    if (this.AutoAdjust && this.TimerTickCount >= 20)
                    {
                        this.AdjustSensor();
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Resets the indicator.
        /// </summary>
        private void ResetIndicator()
        {
            this.bottomedges.Background = defaultBrush;
            this.leftedges.Background = defaultBrush;
            this.rightedges.Background = defaultBrush;
            this.topedges.Background = defaultBrush;
        }

        /// <summary>
        /// Adjusts the sensor.
        /// </summary>
        private void AdjustSensor()
        {
            Dispatcher.Invoke(DispatcherPriority.Normal,
                new Action(
                    delegate()
                    {

                        try
                        {
                            this.ResetIndicator();
                            this.sensor.ElevationAngle += 10;
                            this.TimerTickCount = 0;
                            this.StartTimer();
                        }
                        catch (ArgumentOutOfRangeException outofRangeex)
                        {
                            // Consider going out of range, setting it to o
                            this.sensor.ElevationAngle = 0;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }

                    }
                ));
        }

        /// <summary>
        /// Draws the skeleton.
        /// </summary>
        /// <param name="skeleton">The skeleton.</param>
        private void DrawSkeleton(Skeleton skeleton)
        {
            CheckForClippedEdges(skeleton);

            // Check for Seated and Default Mode.
            if (this.sensor.SkeletonStream.TrackingMode == SkeletonTrackingMode.Default)
            {
                drawBone(skeleton.Joints[JointType.Head], skeleton.Joints[JointType.ShoulderCenter]);
                drawBone(skeleton.Joints[JointType.ShoulderCenter], skeleton.Joints[JointType.Spine]);
            }
            else
            {
                drawBone(skeleton.Joints[JointType.Head], skeleton.Joints[JointType.ShoulderCenter]);
            }

            drawBone(skeleton.Joints[JointType.ShoulderCenter], skeleton.Joints[JointType.ShoulderLeft]);
            drawBone(skeleton.Joints[JointType.ShoulderLeft], skeleton.Joints[JointType.ElbowLeft]);
            drawBone(skeleton.Joints[JointType.ElbowLeft], skeleton.Joints[JointType.WristLeft]);
            drawBone(skeleton.Joints[JointType.WristLeft], skeleton.Joints[JointType.HandLeft]);

            drawBone(skeleton.Joints[JointType.ShoulderCenter], skeleton.Joints[JointType.ShoulderRight]);
            drawBone(skeleton.Joints[JointType.ShoulderRight], skeleton.Joints[JointType.ElbowRight]);
            drawBone(skeleton.Joints[JointType.ElbowRight], skeleton.Joints[JointType.WristRight]);
            drawBone(skeleton.Joints[JointType.WristRight], skeleton.Joints[JointType.HandRight]);

            drawBone(skeleton.Joints[JointType.Spine], skeleton.Joints[JointType.HipCenter]);
            drawBone(skeleton.Joints[JointType.HipCenter], skeleton.Joints[JointType.HipLeft]);
            drawBone(skeleton.Joints[JointType.HipLeft], skeleton.Joints[JointType.KneeLeft]);
            drawBone(skeleton.Joints[JointType.KneeLeft], skeleton.Joints[JointType.AnkleLeft]);
            drawBone(skeleton.Joints[JointType.AnkleLeft], skeleton.Joints[JointType.FootLeft]);

            drawBone(skeleton.Joints[JointType.HipCenter], skeleton.Joints[JointType.HipRight]);
            drawBone(skeleton.Joints[JointType.HipRight], skeleton.Joints[JointType.KneeRight]);
            drawBone(skeleton.Joints[JointType.KneeRight], skeleton.Joints[JointType.AnkleRight]);
            drawBone(skeleton.Joints[JointType.AnkleRight], skeleton.Joints[JointType.FootRight]);
        }

        /// <summary>
        /// Draws the bone.
        /// </summary>
        /// <param name="trackedJoint1">The tracked joint1.</param>
        /// <param name="trackedJoint2">The tracked joint2.</param>
        void drawBone(Joint trackedJoint1, Joint trackedJoint2)
        {
            Line bone = new Line();
            bone.Stroke = Brushes.Red;
            bone.StrokeThickness = 3;
            Point joint1 = this.ScalePosition(trackedJoint1.Position);
            bone.X1 = joint1.X;
            bone.Y1 = joint1.Y;

            // Draw Joints
            Point mappedPoint1 = this.ScalePosition(trackedJoint1.Position);
            Rectangle r = new Rectangle(); r.Height = 10; r.Width = 10;
            r.Fill = Brushes.Red;
            Canvas.SetLeft(r, mappedPoint1.X - 2);
            Canvas.SetTop(r, mappedPoint1.Y - 2);
            myCanvas.Children.Add(r);

            Point joint2 = this.ScalePosition(trackedJoint2.Position);
            bone.X2 = joint2.X;
            bone.Y2 = joint2.Y;

            Point mappedPoint2 = this.ScalePosition(trackedJoint2.Position);

           // Draw Leaf Joints
            if (LeafJoint(trackedJoint2))
            {
                Rectangle r1 = new Rectangle(); r1.Height = 10; r1.Width = 10;
                r1.Fill = Brushes.Red;
                Canvas.SetLeft(r1, mappedPoint2.X - 2);
                Canvas.SetTop(r1, mappedPoint2.Y - 2);
                myCanvas.Children.Add(r1);
            }

            myCanvas.Children.Add(bone);
        }

        /// <summary>
        /// Leafs the joint.
        /// </summary>
        /// <param name="joint">The joint.</param>
        /// <returns></returns>
        private bool LeafJoint(Joint joint)
        {
            if (joint.JointType == JointType.HandRight || joint.JointType == JointType.HandLeft || joint.JointType == JointType.FootLeft || joint.JointType == JointType.FootRight)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Scales the position.
        /// </summary>
        /// <param name="skeletonPoint">The skeleton point.</param>
        /// <returns></returns>
        private Point ScalePosition(SkeletonPoint skeletonPoint)
        {
            DepthImagePoint depthPoint = this.sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(skeletonPoint, DepthImageFormat.Resolution320x240Fps30);
            return new Point(depthPoint.X, depthPoint.Y);
        }

        /// <summary>
        /// Manages the auto adjust.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void ManageAutoAdjust(object sender, RoutedEventArgs e)
        {
            CheckBox cbox = sender as CheckBox;
            if (cbox.IsChecked == true)
            {
                this.AutoAdjust = true;

            }
            else if (cbox.IsChecked == false)
            {
                this.AutoAdjust = false;
                this.timer.Stop();
            }
        }

        /// <summary>
        /// Manages the seated skeleton tracking.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void ManageSeatedSkeletonTracking(object sender, RoutedEventArgs e)
        {
            CheckBox cbox = sender as CheckBox;
            if (cbox.IsChecked == true)
            {
                this.sensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated;
            }
            else if (cbox.IsChecked == false)
            {
                this.sensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Default;
            }
        }

        /// <summary>
        /// Timer_s the tick.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        public void Timer_Tick(object sender, object e)
        {
            this.TimerTickCount++;
        }

        /// <summary>
        /// Handles the Click event of the buttonResetSensor control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void buttonResetSensor_Click(object sender, RoutedEventArgs e)
        {
            if (this.sensor != null && this.sensor.IsRunning)
            {
                this.sensor.ElevationAngle = 0;
            }
        }
    }
}
