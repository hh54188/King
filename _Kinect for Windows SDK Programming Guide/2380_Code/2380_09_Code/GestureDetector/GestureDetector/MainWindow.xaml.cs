
namespace GestureDetector
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Shapes;
    using GestureRecognizer;
    using Microsoft.Kinect;

    public partial class MainWindow : Window
    {
        KinectSensor sensor;
        TextBlock textBlock = new TextBlock();
        GestureRecognitionEngine recognitionEngine;
        Skeleton[] totalSkeleton = new Skeleton[6];

        public MainWindow()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(MainWindow_Loaded);
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.sensor = KinectSensor.KinectSensors[0];

            this.sensor.SkeletonStream.Enable();
            this.sensor.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(sensor_SkeletonFrameReady);

            recognitionEngine = new GestureRecognitionEngine();
            recognitionEngine.GestureRecognized += new EventHandler<GestureEventArgs>(recognitionEngine_GestureRecognized);

            this.sensor.Start();
        }


        void recognitionEngine_GestureRecognized(object sender, GestureEventArgs e)
        {
            listBox1.Items.Add(e.GestureType.ToString());
        }

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
                this.myCanvas.Children.Clear();
                this.DrawSkeleton(firstSkeleton);
                recognitionEngine.Skeleton = firstSkeleton;
                recognitionEngine.StartRecognize();

            }
        }


        void drawBone(Joint trackedJoint1, Joint trackedJoint2)
        {
            Line bone = new Line();
            bone.Stroke = Brushes.Red;
            bone.StrokeThickness = 3;
            Point joint1 = this.ScalePosition(trackedJoint1.Position);
            bone.X1 = joint1.X;
            bone.Y1 = joint1.Y;



            Point mappedPoint11 = this.ScalePosition(trackedJoint1.Position);
            TextBlock textBlock = new TextBlock();
            textBlock.Text = trackedJoint1.JointType.ToString();
            textBlock.Foreground = Brushes.Black;
            Canvas.SetLeft(textBlock, mappedPoint11.X + 5);
            Canvas.SetTop(textBlock, mappedPoint11.Y + 5);
            myCanvas.Children.Add(textBlock);

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

            if (LeafJoint(trackedJoint2))
            {
                Rectangle r1 = new Rectangle(); r1.Height = 10; r1.Width = 10;
                r1.Fill = Brushes.Red;
                Canvas.SetLeft(r1, mappedPoint2.X - 2);
                Canvas.SetTop(r1, mappedPoint2.Y - 2);
                myCanvas.Children.Add(r1);
            }

            if (LeafJoint(trackedJoint2))
            {
                Point mappedPoint = this.ScalePosition(trackedJoint2.Position);
                TextBlock textBlock1 = new TextBlock();
                textBlock1.Text = trackedJoint2.JointType.ToString();
                textBlock1.Foreground = Brushes.Black;
                Canvas.SetLeft(textBlock1, mappedPoint.X + 5);
                Canvas.SetTop(textBlock1, mappedPoint.Y + 5);
                myCanvas.Children.Add(textBlock1);
            }

            myCanvas.Children.Add(bone);
        }

        private Point ScalePosition(SkeletonPoint skeletonPoint)
        {
            DepthImagePoint depthPoint = this.sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(skeletonPoint, DepthImageFormat.Resolution320x240Fps30);
            return new Point(depthPoint.X, depthPoint.Y);
        }

        private bool LeafJoint(Joint j2)
        {
            if (j2.JointType == JointType.HandRight || j2.JointType == JointType.HandLeft || j2.JointType == JointType.FootLeft || j2.JointType == JointType.FootRight)
            {
                return true;
            }
            return false;
        }

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
    }
}
