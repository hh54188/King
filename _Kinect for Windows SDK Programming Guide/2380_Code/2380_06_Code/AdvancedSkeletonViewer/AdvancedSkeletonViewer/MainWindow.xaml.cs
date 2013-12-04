/*Working Solution- Hand Tracking with Joint Position Display: 
•	Tracking the skeleton in both Default and Seated mode also you can enable near range.
•	Display the Tracked joints, joints name and drawing bones.
•	Visualizing different bone sequence for both tracking mode.
•	Visual indication for total tracked skeleton.
•	Record and Play a fixed set of skeleton collection.
•	Select specific frame and display.
*/
namespace AdvancedSkeletonViewer
{
    using System;
    using System.Collections.ObjectModel;
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
        /// The sensor
        /// </summary>
        KinectSensor sensor;

        /// <summary>
        /// Gets or sets a value indicating whether [show joints].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show joints]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowJoints { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is recording started.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is recording started; otherwise, <c>false</c>.
        /// </value>
        public bool IsRecordingStarted { get; set; }

        /// <summary>
        /// skeleton Collection
        /// </summary>
        ObservableCollection<SkeletonInfo> skeletonCollection = new ObservableCollection<SkeletonInfo>();

        /// <summary>
        /// The skeleton object
        /// </summary>
        Skeleton skeleton;

        /// <summary>
        /// Gets or sets a value indicating whether [show names].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show names]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowNames { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow" /> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(MainWindow_Loaded);
            this.DataContext = this;
            listBox1.ItemsSource = skeletonCollection;
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
                // get the reference of first connected sensor from collection
                this.sensor = KinectSensor.KinectSensors.Where(item => item.Status == KinectStatus.Connected).FirstOrDefault();

                // start the sensor.
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


            if (this.ShowNames)
            {
                Point mappedPoint1 = this.ScalePosition(trackedJoint1.Position);
                TextBlock textBlock = new TextBlock();
                textBlock.Text = trackedJoint1.JointType.ToString();
                textBlock.Foreground = Brushes.Black;
                Canvas.SetLeft(textBlock, mappedPoint1.X + 5);
                Canvas.SetTop(textBlock, mappedPoint1.Y + 5);
                myCanvas.Children.Add(textBlock);
            }

            if (this.ShowJoints)
            {
                Point mappedPoint1 = this.ScalePosition(trackedJoint1.Position);
                Rectangle r = new Rectangle(); r.Height = 10; r.Width = 10;
                r.Fill = Brushes.Red;
                Canvas.SetLeft(r, mappedPoint1.X - 2);
                Canvas.SetTop(r, mappedPoint1.Y - 2);
                myCanvas.Children.Add(r);
            }

            Point joint2 = this.ScalePosition(trackedJoint2.Position);
            bone.X2 = joint2.X;
            bone.Y2 = joint2.Y;

            Point mappedPoint2 = this.ScalePosition(trackedJoint2.Position);

            if (LeafJoint(trackedJoint2) && this.ShowJoints)
            {
                Rectangle r1 = new Rectangle(); r1.Height = 10; r1.Width = 10;
                r1.Fill = Brushes.Red;
                Canvas.SetLeft(r1, mappedPoint2.X - 2);
                Canvas.SetTop(r1, mappedPoint2.Y - 2);
                myCanvas.Children.Add(r1);
            }

            if (LeafJoint(trackedJoint2) && this.ShowJoints)
            {
                Point mappedPoint = this.ScalePosition(trackedJoint2.Position);
                TextBlock textBlock = new TextBlock();
                textBlock.Text = trackedJoint2.JointType.ToString();
                textBlock.Foreground = Brushes.Black;
                Canvas.SetLeft(textBlock, mappedPoint.X + 5);
                Canvas.SetTop(textBlock, mappedPoint.Y + 5);
                myCanvas.Children.Add(textBlock);
            }

            myCanvas.Children.Add(bone);
        }

        /// <summary>
        /// Leafs the joint.
        /// </summary>
        /// <param name="j2">The j2.</param>
        /// <returns></returns>
        private bool LeafJoint(Joint j2)
        {
            if (j2.JointType == JointType.HandRight || j2.JointType == JointType.HandLeft || j2.JointType == JointType.FootLeft || j2.JointType == JointType.FootRight)
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
        /// Handles the SkeletonFrameReady event of the sensor control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SkeletonFrameReadyEventArgs" /> instance containing the event data.</param>
        void sensor_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {

            progressBar1.Value = 0;
            myCanvas.Children.Clear();
            Brush brush = new SolidColorBrush(Colors.Red);
            Skeleton[] skeletons = null;
            SkeletonFrame frame;
            using (frame = e.OpenSkeletonFrame())
            {
                if (frame != null)
                {
                    skeletons = new Skeleton[frame.SkeletonArrayLength];
                    frame.CopySkeletonDataTo(skeletons);
                }
            }

            if (skeletons == null) return;

            skeleton = (from trackSkeleton in skeletons
                        where trackSkeleton.TrackingState == SkeletonTrackingState.Tracked
                        select trackSkeleton).FirstOrDefault();

            if (skeleton == null)
                return;

            if (this.IsRecordingStarted && skeletonCollection.Count <= 1000)
            {
                skeletonCollection.Add(new SkeletonInfo { FrameID = frame.FrameNumber, Skeleton = skeleton });
            }

            int trackedSkeleton = skeleton.Joints.Where(item => item.TrackingState == JointTrackingState.Tracked).Count();
            progressBar1.Value = trackedSkeleton;
            if (this.sensor.SkeletonStream.TrackingMode == SkeletonTrackingMode.Seated)
            {
                DrawSeatedSkeleton();
            }
            else
            {
                DrawDefaultSkeleton();
            }
        }

        /// <summary>
        /// Draws the default skeleton.
        /// </summary>
        private void DrawDefaultSkeleton()
        {
            if (radioHead.IsChecked == true)
            {
                DrawSpine(); ;
            }
            else if (radioLeftArm.IsChecked == true)
            {
                DrawLeftArm();
            }
            else if (radioRightArm.IsChecked == true)
            {
                DrawRightArm();
            }
            else if (radioLeftLeg.IsChecked == true)
            {
                DrawLeftLeg();
            }
            else if (radioRightLeg.IsChecked == true)
            {
                DrawRightLeg();
            }
            else
            {
                DrawSpine();
                DrawLeftArm();
                DrawRightArm();
                DrawLeftLeg();
                DrawRightLeg();
            }
        }

        /// <summary>
        /// Draws the seated skeleton.
        /// </summary>
        private void DrawSeatedSkeleton()
        {
            if (radioSeatedHead.IsChecked == true)
            {
                DrawHeadShoulder();
            }
            else if (radioSeatedLeftArm.IsChecked == true)
            {
                DrawLeftArm();
            }
            else if (radioSeatedRightArm.IsChecked == true)
            {
                DrawRightArm();
            }
            else
            {
                DrawHeadShoulder();
                DrawLeftArm();
                DrawRightArm();
            }
        }

        /// <summary>
        /// Draws the head shoulder.
        /// </summary>
        private void DrawHeadShoulder()
        {
            drawBone(skeleton.Joints[JointType.Head], skeleton.Joints[JointType.ShoulderCenter]);
        }

        /// <summary>
        /// Draws the spine.
        /// </summary>
        private void DrawSpine()
        {
            drawBone(skeleton.Joints[JointType.Head], skeleton.Joints[JointType.ShoulderCenter]);
            drawBone(skeleton.Joints[JointType.ShoulderCenter], skeleton.Joints[JointType.Spine]);
        }

        /// <summary>
        /// Draws the left arm.
        /// </summary>
        private void DrawLeftArm()
        {
            drawBone(skeleton.Joints[JointType.ShoulderCenter], skeleton.Joints[JointType.ShoulderLeft]);
            drawBone(skeleton.Joints[JointType.ShoulderLeft], skeleton.Joints[JointType.ElbowLeft]);
            drawBone(skeleton.Joints[JointType.ElbowLeft], skeleton.Joints[JointType.WristLeft]);
            drawBone(skeleton.Joints[JointType.WristLeft], skeleton.Joints[JointType.HandLeft]);

        }

        /// <summary>
        /// Draws the right arm.
        /// </summary>
        private void DrawRightArm()
        {
            drawBone(skeleton.Joints[JointType.ShoulderCenter], skeleton.Joints[JointType.ShoulderRight]);
            drawBone(skeleton.Joints[JointType.ShoulderRight], skeleton.Joints[JointType.ElbowRight]);
            drawBone(skeleton.Joints[JointType.ElbowRight], skeleton.Joints[JointType.WristRight]);
            drawBone(skeleton.Joints[JointType.WristRight], skeleton.Joints[JointType.HandRight]);
        }

        /// <summary>
        /// Draws the left leg.
        /// </summary>
        private void DrawLeftLeg()
        {
            drawBone(skeleton.Joints[JointType.Spine], skeleton.Joints[JointType.HipCenter]);
            drawBone(skeleton.Joints[JointType.HipCenter], skeleton.Joints[JointType.HipLeft]);
            drawBone(skeleton.Joints[JointType.HipLeft], skeleton.Joints[JointType.KneeLeft]);
            drawBone(skeleton.Joints[JointType.KneeLeft], skeleton.Joints[JointType.AnkleLeft]);
            drawBone(skeleton.Joints[JointType.AnkleLeft], skeleton.Joints[JointType.FootLeft]);
        }

        /// <summary>
        /// Draws the right leg.
        /// </summary>
        private void DrawRightLeg()
        {
            drawBone(skeleton.Joints[JointType.HipCenter], skeleton.Joints[JointType.HipRight]);
            drawBone(skeleton.Joints[JointType.HipRight], skeleton.Joints[JointType.KneeRight]);
            drawBone(skeleton.Joints[JointType.KneeRight], skeleton.Joints[JointType.AnkleRight]);
            drawBone(skeleton.Joints[JointType.AnkleRight], skeleton.Joints[JointType.FootRight]);
        }

        /// <summary>
        /// Shows the skeleton joints.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void ShowSkeletonJoints(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb.IsChecked == true)
            {
                this.ShowJoints = true;
            }
            if (cb.IsChecked == false)
            {
                this.ShowJoints = false;
            }
        }

        /// <summary>
        /// Shows the name of the skeleton joints.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void ShowSkeletonJointsName(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb.IsChecked == true)
            {
                this.ShowNames = true;
            }
            if (cb.IsChecked == false)
            {
                this.ShowNames = false;
            }
        }

        /// <summary>
        /// Trackings the in near mode.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void TrackingInNearMode(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb.IsChecked == true)
            {
                this.sensor.DepthStream.Range = DepthRange.Near;
                this.sensor.SkeletonStream.EnableTrackingInNearRange = true;
            }
            if (cb.IsChecked == false)
            {
                this.sensor.DepthStream.Range = DepthRange.Default;
                this.sensor.SkeletonStream.EnableTrackingInNearRange = false;
            }
        }

        /// <summary>
        /// Handles the seated skeleton.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void HandleSeatedSkeleton(object sender, RoutedEventArgs e)
        {

            CheckBox cb = sender as CheckBox;
            if (cb.IsChecked == true)
            {
                this.sensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated;
                groupBoneSequenceDefault.IsEnabled = false;
                groupBoneSequenceSeated.IsEnabled = true;
                radioSeatedComplete.IsChecked = true;
            }
            if (cb.IsChecked == false)
            {
                this.sensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Default;
                groupBoneSequenceDefault.IsEnabled = true;
                groupBoneSequenceSeated.IsEnabled = false;
                radioComplete.IsChecked = true;
            }
        }


        /// <summary>
        /// Handles the Click event of the buttonStopRecording control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void buttonStopRecording_Click(object sender, RoutedEventArgs e)
        {
            this.IsRecordingStarted = false;
            this.buttonRecordSketon.IsEnabled = true;
            this.buttonPlayRecording.IsEnabled = true;
            this.buttonStopRecording.IsEnabled = false;
        }

        /// <summary>
        /// Handles the Click event of the buttonRecordSketon control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void buttonRecordSketon_Click(object sender, RoutedEventArgs e)
        {
            this.IsRecordingStarted = true;
            this.buttonRecordSketon.IsEnabled = false;
            this.buttonStopRecording.IsEnabled = true;
        }



        /// <summary>
        /// Handles the SelectionChanged event of the listBox1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs" /> instance containing the event data.</param>
        private void listBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        /// <summary>
        /// Selects the frame.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void SelectFrame(object sender, RoutedEventArgs e)
        {
            SkeletonInfo cb = (SkeletonInfo)(sender as CheckBox).DataContext;
            if (cb != null)
            {
                this.StopTracking();
                myCanvas.Children.Clear();

                this.DrawSkeleton(cb.Skeleton);
            }
        }

        /// <summary>
        /// Draws the skeleton.
        /// </summary>
        /// <param name="skeleton">The skeleton.</param>
        private void DrawSkeleton(Skeleton skeleton)
        {

            drawBone(skeleton.Joints[JointType.Head], skeleton.Joints[JointType.ShoulderCenter]);
            drawBone(skeleton.Joints[JointType.ShoulderCenter], skeleton.Joints[JointType.Spine]);

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
        /// Handles the Click event of the buttonPlayRecording control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void buttonPlayRecording_Click(object sender, RoutedEventArgs e)
        {
            if (skeletonCollection.Count > 0)
            {
                this.StopTracking();
                this.buttonStartTracking.IsEnabled = false;

                foreach (var skeletonInfo in skeletonCollection)
                {
                    myCanvas.Children.Clear();

                    this.DrawSkeleton(skeletonInfo.Skeleton);
                    System.Threading.Thread.Sleep(100);
                }

            }
            this.buttonStartTracking.IsEnabled = true;
        }


        /// <summary>
        /// Handles the Click event of the buttonStartTracking control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void buttonStartTracking_Click(object sender, RoutedEventArgs e)
        {
            this.StartTracking();
            this.skeletonCollection.Clear();
            this.EnableRecordingControl();
        }

        /// <summary>
        /// Enables the recording control.
        /// </summary>
        private void EnableRecordingControl()
        {
            this.buttonRecordSketon.IsEnabled = true;

        }

        /// <summary>
        /// Stops the tracking.
        /// </summary>
        private void StopTracking()
        {
            if (this.sensor != null && this.sensor.SkeletonStream.IsEnabled)
            {
                this.sensor.SkeletonStream.Disable();
                this.sensor.SkeletonFrameReady -= new EventHandler<SkeletonFrameReadyEventArgs>(sensor_SkeletonFrameReady);
            }
            buttonStartTracking.IsEnabled = true;
        }

        /// <summary>
        /// Starts the tracking.
        /// </summary>
        private void StartTracking()
        {
            if (this.sensor != null)
            {
                this.sensor.SkeletonStream.Enable();
                this.sensor.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(sensor_SkeletonFrameReady);
            }
            buttonStartTracking.IsEnabled = false;
        }

    }
}
