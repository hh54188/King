using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Kinect;

namespace SkeletonHand
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private KinectSensor sensor;
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
            this.Closing += MainWindow_Closing;
        }

        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.sensor != null && this.sensor.IsRunning)
            {
                this.sensor.Stop();
            }
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (KinectSensor.KinectSensors.Count > 0)
            {
                this.sensor = KinectSensor.KinectSensors[0];

                // Enable Seated Mode:
                this.sensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated;
                // Enable Near Mode:
                this.sensor.DepthStream.Range = DepthRange.Near;
                this.sensor.SkeletonStream.EnableTrackingInNearRange = true;

                this.sensor.SkeletonStream.Enable();
                this.sensor.DepthStream.Enable();
                this.sensor.SkeletonFrameReady += sensor_SkeletonFrameReady;

                this.sensor.Start();
            }
        }

        void sensor_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame == null)
                {
                    return;
                }

                Skeleton[] totalSkeleton = new Skeleton[skeletonFrame.SkeletonArrayLength];
                skeletonFrame.CopySkeletonDataTo(totalSkeleton);

                // 如果不用 LINQ表达式应该是怎么写？
                Skeleton firstSkeleton = (from trackskeleton in totalSkeleton
                                          where trackskeleton.TrackingState == SkeletonTrackingState.
                                          Tracked
                                          select trackskeleton).FirstOrDefault();
                if (firstSkeleton == null)
                {
                    return;
                }

                if (firstSkeleton.Joints[JointType.HandRight].TrackingState == JointTrackingState.Tracked)
                {
                    this.MapJointsWithUIElement(firstSkeleton);
                }

            }
        }

        private void MapJointsWithUIElement(Skeleton firstSkeleton)
        {
            Point mappedPoint = this.ScalePosition(firstSkeleton.Joints[JointType.HandRight].Position);
            Canvas.SetLeft(righthand, mappedPoint.X);
            Canvas.SetTop(righthand, mappedPoint.Y);
           
        }

        private Point ScalePosition(SkeletonPoint skeletonPoint)
        {
            DepthImagePoint depthPoint = this.sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(skeletonPoint, DepthImageFormat.Resolution640x480Fps30);
            return new Point(depthPoint.X, depthPoint.Y);
        }
    }
}
