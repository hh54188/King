
namespace ClappingHands
{
    using System;
    using System.Linq;
    using System.Media;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using GestureRecognizer;
    using Microsoft.Kinect;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        /// <summary>
        /// The Sound Player
        /// </summary>
        SoundPlayer kinectSoundPlayer;

        /// <summary>
        /// Total Skeleton
        /// </summary>
        Skeleton[] totalSkeleton = new Skeleton[6];

        /// <summary>
        /// Kinect sensor
        /// </summary>
        KinectSensor sensor;

        /// <summary>
        /// Gesture Recognition Engine
        /// </summary>
        GestureRecognitionEngine recognitionEngine;

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
            recognitionEngine.GestureType = GestureType.HandsClapping;
            recognitionEngine.GestureRecognized += new EventHandler<GestureEventArgs>(recognitionEngine_GestureRecognized);

            this.sensor.Start();
            kinectSoundPlayer = new SoundPlayer("Clap.wav");

        }

        /// <summary>
        /// Handles the GestureRecognized event of the recognitionEngine control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="GestureEventArgs" /> instance containing the event data.</param>
        void recognitionEngine_GestureRecognized(object sender, GestureEventArgs e)
        {
            kinectSoundPlayer.Play();
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
                    return;
                }

                recognitionEngine.Skeleton = firstSkeleton;
                recognitionEngine.StartRecognize();

            }
        }
    }
}
