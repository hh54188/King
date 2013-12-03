
namespace VirtualRopeWorkOut
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Shapes;
    using GestureRecognizer;
    using Microsoft.Kinect;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        KinectSensor sensor;
        TextBlock textBlock = new TextBlock();

        Skeleton[] totalSkeleton = new Skeleton[6];
        GestureRecognitionEngine recognitionEngine;

        Line distanceLiner = new Line();
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
            this.sensor = KinectSensor.KinectSensors[0];
            
            this.sensor.DepthStream.Enable(DepthImageFormat.Resolution320x240Fps30);
            sensor.DepthFrameReady += new EventHandler<DepthImageFrameReadyEventArgs>(sensor_DepthFrameReady);
            
            this.sensor.SkeletonStream.Enable();
            this.sensor.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(sensor_SkeletonFrameReady);

            recognitionEngine = new GestureRecognitionEngine();
            recognitionEngine.GestureRecognized += new EventHandler<GestureEventArgs>(recognitionEngine_GestureRecognized);
            recognitionEngine.GestureNotRecognized += new EventHandler<GestureEventArgs>(recognitionEngine_GestureNotRecognized);

            this.sensor.Start();
        }

        /// <summary>
        /// Handles the GestureNotRecognized event of the recognitionEngine control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="GestureEventArgs" /> instance containing the event data.</param>
        void recognitionEngine_GestureNotRecognized(object sender, GestureEventArgs e)
        {
            progressBar1.Value = e.Result == RecognitionResult.Failed ? 0 : 100;
        }

        /// <summary>
        /// Handles the GestureRecognized event of the recognitionEngine control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="GestureEventArgs" /> instance containing the event data.</param>
        void recognitionEngine_GestureRecognized(object sender, GestureEventArgs e)
        {
            progressBar1.Value = e.Result == RecognitionResult.Success ? 100 :0 ;
        }

        /// <summary>
        /// Handles the DepthFrameReady event of the sensor control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="DepthImageFrameReadyEventArgs" /> instance containing the event data.</param>
        void sensor_DepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            using (DepthImageFrame depthimageFrame = e.OpenDepthImageFrame())
            {
                if (depthimageFrame == null)
                {
                    return;
                }

                short[] pixelData = new short[depthimageFrame.PixelDataLength];
                int stride = depthimageFrame.Width * 2;
                depthimageFrame.CopyPixelDataTo(pixelData);

                depthImageControl.Source = BitmapSource.Create(
                    depthimageFrame.Width, depthimageFrame.Height, 96, 96, PixelFormats.Gray16, null, pixelData, stride);

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
                    myCanvas.Children.Remove(textBlock);
                    myCanvas.Children.Remove(distanceLiner);
                    return;
                }
                  progressBar1.Value=0;
                if (radioBothHand.IsChecked == true)
                {
                    if (firstSkeleton.Joints[JointType.HandRight].TrackingState == JointTrackingState.Tracked && firstSkeleton.Joints[JointType.HandLeft].TrackingState == JointTrackingState.Tracked)
                    {
                        this.MapJointsWithUIElement(firstSkeleton.Joints[JointType.HandRight], firstSkeleton.Joints[JointType.HandLeft]);
                        myCanvas.Children.Remove(distanceLiner);
                        myCanvas.Children.Remove(textBlock);
                        this.drawLineWithDistance(firstSkeleton.Joints[JointType.HandRight], firstSkeleton.Joints[JointType.HandLeft]);
                        recognitionEngine.Skeleton = firstSkeleton;
                        recognitionEngine.GestureType = GestureType.HandJointDistance;
                        try
                        {
                            recognitionEngine.RecognizeParameter = float.Parse(textBox1.Text);
                        }
                        catch (Exception)
                        {

                            recognitionEngine.RecognizeParameter = 1.2f;
                        }
                    
                        recognitionEngine.StartRecognize();
                    }
                }
                else if (radioRightHandFoot.IsChecked == true)
                {
                    if (firstSkeleton.Joints[JointType.HandRight].TrackingState == JointTrackingState.Tracked && firstSkeleton.Joints[JointType.FootRight].TrackingState == JointTrackingState.Tracked)
                    {
                        this.MapJointsWithUIElement(firstSkeleton.Joints[JointType.HandRight], firstSkeleton.Joints[JointType.FootRight]);
                        myCanvas.Children.Remove(distanceLiner);
                        myCanvas.Children.Remove(textBlock);
                        this.drawLineWithDistance(firstSkeleton.Joints[JointType.HandRight], firstSkeleton.Joints[JointType.FootRight]);
                        recognitionEngine.Skeleton = firstSkeleton;
                        recognitionEngine.GestureType = GestureType.RightHandAndFeet;
                        try
                        {
                            recognitionEngine.RecognizeParameter = float.Parse(textBox2.Text);
                        }
                        catch (Exception)
                        {

                            recognitionEngine.RecognizeParameter = 0.5f;
                        }

                        recognitionEngine.StartRecognize();
                    }
                }
                else
                {
                    if (firstSkeleton.Joints[JointType.HandLeft].TrackingState == JointTrackingState.Tracked && firstSkeleton.Joints[JointType.FootLeft].TrackingState == JointTrackingState.Tracked)
                    {
                        this.MapJointsWithUIElement(firstSkeleton.Joints[JointType.HandLeft], firstSkeleton.Joints[JointType.FootLeft]);
                        myCanvas.Children.Remove(distanceLiner);
                        myCanvas.Children.Remove(textBlock);
                        this.drawLineWithDistance(firstSkeleton.Joints[JointType.HandLeft], firstSkeleton.Joints[JointType.FootLeft]);
                        recognitionEngine.Skeleton = firstSkeleton;
                        recognitionEngine.GestureType = GestureType.LeftHandAndFeet;
                        try
                        {
                            recognitionEngine.RecognizeParameter = float.Parse(textBox3.Text);
                        }
                        catch (Exception)
                        {

                            recognitionEngine.RecognizeParameter = 0.5f;
                        }

                        recognitionEngine.StartRecognize();
                    }

                }


                
               
            }
        }

        /// <summary>
        /// Maps the joints with UI element.
        /// </summary>
        /// <param name="skeleton">The skeleton.</param>
        private void MapJointsWithUIElement(Joint joint1, Joint joint2)
        {
            // Get the Points in 2D Space to map in UI. for that call the Scale postion method.
            Point mappedPoint = this.ScalePosition(joint1.Position);
            Canvas.SetLeft(righthand, mappedPoint.X);
            Canvas.SetTop(righthand, mappedPoint.Y);

            Point mappedPoint2 = this.ScalePosition(joint2.Position);
            Canvas.SetLeft(Lefthand, mappedPoint2.X);
            Canvas.SetTop(Lefthand, mappedPoint2.Y);
            Canvas.SetZIndex(Lefthand, 100);
            Canvas.SetZIndex(righthand, 100);
        }

        /// <summary>
        /// Draws the line with distance.
        /// </summary>
        /// <param name="trackedJoint1">The tracked joint1.</param>
        /// <param name="trackedJoint2">The tracked joint2.</param>
        void drawLineWithDistance(Joint trackedJoint1, Joint trackedJoint2)
        {

            distanceLiner.Stroke = Brushes.Red;
            distanceLiner.StrokeThickness = 3;
            Point joint1 = this.ScalePosition(trackedJoint1.Position);
            distanceLiner.X1 = joint1.X;
            distanceLiner.Y1 = joint1.Y;


            Point joint2 = this.ScalePosition(trackedJoint2.Position);
            distanceLiner.X2 = joint2.X;
            distanceLiner.Y2 = joint2.Y;

            Point mappedPoint2 = this.ScalePosition(trackedJoint2.Position);


            myCanvas.Children.Add(distanceLiner);
            Canvas.SetZIndex(distanceLiner, 100);


            float distance = this.GetJointDistance(trackedJoint1, trackedJoint2);

            Point midPoint = this.GetMidPoint(joint1, joint2);


            textBlock.Text = distance.ToString("00.00");
            textBlock.FontSize = 24;
            textBlock.Foreground = Brushes.Yellow;
            Canvas.SetLeft(textBlock, midPoint.X + 5);
            Canvas.SetTop(textBlock, midPoint.Y + 5);
            myCanvas.Children.Add(textBlock);
            Canvas.SetZIndex(textBlock, 100);
        }

        /// <summary>
        /// Gets the mid point.
        /// </summary>
        /// <param name="joint1">The joint1.</param>
        /// <param name="joint2">The joint2.</param>
        /// <returns></returns>
        private Point GetMidPoint(Point joint1, Point joint2)
        {
            return new Point((joint1.X + joint1.Y) / 2, (joint1.Y + joint2.Y) / 2);
        }

        /// <summary>
        /// Gets the joint distance.
        /// </summary>
        /// <param name="firstJoint">The first joint.</param>
        /// <param name="secondJoint">The second joint.</param>
        /// <returns></returns>
        private float GetJointDistance(Joint firstJoint, Joint secondJoint)
        {
            float distanceX = firstJoint.Position.X - secondJoint.Position.X;
            float distanceY = firstJoint.Position.Y - secondJoint.Position.Y;
            float distanceZ = firstJoint.Position.Z - secondJoint.Position.Z;
            return (float)Math.Sqrt(Math.Pow(distanceX, 2) + Math.Pow(distanceY, 2) + Math.Pow(distanceZ, 2));
        }

        /// <summary>
        /// Scales the position.
        /// </summary>
        /// <param name="skeletonPoint">The skeltonpoint.</param>
        /// <returns></returns>
        private Point ScalePosition(SkeletonPoint skeletonPoint)
        {
            // return the depth points from the skeleton point
            DepthImagePoint depthPoint = this.sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(skeletonPoint, DepthImageFormat.Resolution320x240Fps30);
            return new Point(depthPoint.X, depthPoint.Y);
        }

        private void setHandToHandLimit_Click(object sender, RoutedEventArgs e)
        {

        }

        private void setRightHandFeetLimit_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SetLeftHandFeet_Click(object sender, RoutedEventArgs e)
        {

        }
    }

}
