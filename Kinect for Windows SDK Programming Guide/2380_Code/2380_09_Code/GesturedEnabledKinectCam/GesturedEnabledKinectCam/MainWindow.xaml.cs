

namespace GesturedEnabledKinectCam
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media.Animation;
    using Microsoft.Kinect;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public ActionStatus Status { get; set; }

        /// <summary>
        /// Kinect Sensor
        /// </summary>
        KinectSensor sensor = null;

        /// <summary>
        /// Total Skeleton
        /// </summary>
        private Skeleton[] skeletons = new Skeleton[6];


        /// <summary>
        /// Gets or sets a value indicating whether [infrared stream enabled].
        /// </summary>
        /// <value>
        /// <c>true</c> if [infrared stream enabled]; otherwise, <c>false</c>.
        /// </value>
        public bool InfraredStreamEnabled { get; set; }


        /// <summary>
        /// Gets or sets a value indicating whether [red effects enabled].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [red effects enabled]; otherwise, <c>false</c>.
        /// </value>
        public bool RedEffectsEnabled { get; set; }


        /// <summary>
        /// Button Range
        /// </summary>
        ButtonPosition buttonPoint;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(MainWindow_Loaded);
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            sensor = KinectSensor.KinectSensors[0];

            var parameters = new TransformSmoothParameters
            {
                Smoothing = 0.75f,
                Correction = 0.0f,
                Prediction = 0.0f,
                JitterRadius = 0.05f,
                MaxDeviationRadius = 0.04f
            };
            sensor.SkeletonStream.Enable();
            this.sensor.ColorFrameReady += new EventHandler<ColorImageFrameReadyEventArgs>(sensor_ColorFrameReady);
            this.sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
            sensor.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(sensor_SkeletonFrameReady);

            sensor.Start();
        }

        void sensor_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame == null)
                {
                    return;
                }

                skeletonFrame.CopySkeletonDataTo(this.skeletons);

                // Get the first Tracked skeleton
                Skeleton skeleton = (from trackskeleton in this.skeletons
                                     where trackskeleton.TrackingState == SkeletonTrackingState.Tracked
                                     select trackskeleton).FirstOrDefault();

                if (skeleton == null)
                {
                    return;
                }

                var rightHand = skeleton.Joints[JointType.HandRight];
                handCursor.SetPosition(this.sensor, rightHand);
                this.infraredButton.ValidatePoisition(handCursor);
                this.RedEffectsButton.ValidatePoisition(handCursor);
                this.ResetButton.ValidatePoisition(handCursor);

            }
        }

        /// <summary>
        /// Handles the ColorFrameReady event of the sensor control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ColorImageFrameReadyEventArgs" /> instance containing the event data.</param>
        void sensor_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            var colorImageFrame = e.OpenColorImageFrame();

            if (colorImageFrame != null)
            {
                byte[] pixelData = new byte[colorImageFrame.PixelDataLength];

                colorImageFrame.CopyPixelDataTo(pixelData);

                int stride = colorImageFrame.Width * 4;
                PixelFormat pformats = PixelFormats.Bgr32;

                if (this.InfraredStreamEnabled)
                {
                    pformats = PixelFormats.Gray16;
                    stride = colorImageFrame.Width * 2;
                }

                if (this.RedEffectsEnabled)
                {
                    for (int i = 0; i < pixelData.Length; i += colorImageFrame.BytesPerPixel)
                    {
                        pixelData[i] = 0; //Blue
                        pixelData[i + 1] = 0; //Green
                    }

                }
                this.VideoImageControl.Source = BitmapSource.Create(
                               colorImageFrame.Width,
                               colorImageFrame.Height,
                               96,
                               96,
                              pformats,
                               null,
                               pixelData,
                               stride);

            }
        }

        /// <summary>
        /// Resets to color.
        /// </summary>
        private void ResetToColor()
        {
            this.InfraredStreamEnabled = false;
            this.sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
        }


        /// <summary>
        /// Handles the Click event of the KinectButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void KinectButton_Click(object sender, RoutedEventArgs e)
        {

            if (this.InfraredStreamEnabled)
            {
                this.InfraredStreamEnabled = false;
            }
            this.sensor.ColorStream.Enable(ColorImageFormat.InfraredResolution640x480Fps30);
            this.InfraredStreamEnabled = true;
        }

        /// <summary>
        /// Handles the Click event of the RedEffectsButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void RedEffectsButton_Click(object sender, RoutedEventArgs e)
        {

            if (this.InfraredStreamEnabled)
            {
                ResetToColor();
            }
            this.RedEffectsEnabled = true;
        }

        /// <summary>
        /// Handles the Click event of the ResetButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {

            if (this.RedEffectsEnabled)
            {
                this.RedEffectsEnabled = false;
            }
            if (this.InfraredStreamEnabled)
            {
                ResetToColor();
            }
        }
     }


  

}
