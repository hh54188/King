 /*Working Solution- Intrusion Detector Camera. The application will capture images and store it 
  * the application execution directory (/bin) when a new skeleton is tracked. The application will keep continue tracking
  * that user, unless sensor loses the tracking for that particular id.
  */
namespace IntrusionDetectorCamera
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Microsoft.Kinect;
    using System.IO;
    using System.Windows.Controls;

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
        /// check if night vision enabled.
        /// </summary>
        bool nightVision = false;

        /// <summary>
        /// Gets or sets the pixel data.
        /// </summary>
        /// <value>
        /// The pixel data.
        /// </value>
        private byte[] pixelData { get; set; }

        /// <summary>
        /// Total Skeleton Array
        /// </summary>
        Skeleton[] totalSkeleton = new Skeleton[6];

        /// <summary>
        /// Gets or sets the current skeleton ID.
        /// </summary>
        /// <value>
        /// The current skeleton ID.
        /// </value>
        public int CurrentSkeletonID { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(MainWindow_Loaded);
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (KinectSensor.KinectSensors.Count > 0)
            {
                // get the reference of first connected sensor from collection
                this.sensor = KinectSensor.KinectSensors.Where(item => item.Status == KinectStatus.Connected).FirstOrDefault();

                // check if the skeleton stream is enabled. if yes, then just start the sensor, else enable and attach the event handler.

                this.sensor.ColorStream.Enable();
                this.sensor.SkeletonStream.Enable();
                this.sensor.ColorFrameReady += sensor_ColorFrameReady;
                this.sensor.SkeletonFrameReady += sensor_SkeletonFrameReady;


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
        /// Handles the ColorFrameReady event of the sensor control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ColorImageFrameReadyEventArgs" /> instance containing the event data.</param>
        void sensor_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (ColorImageFrame imageFrame = e.OpenColorImageFrame())
            {
                // Check if the incoming frame is not null
                if (imageFrame == null)
                {
                    return;
                }
                else
                {
                    // Get the pixel data in byte array
                    this.pixelData = new byte[imageFrame.PixelDataLength];

                    // Copy the pixel data
                    imageFrame.CopyPixelDataTo(this.pixelData);

                    // Calculate the stride
                    int stride = imageFrame.Width * imageFrame.BytesPerPixel;

                    if (nightVision)
                    {
                        this.VideoControl.Source = BitmapSource.Create(
                        imageFrame.Width, imageFrame.Height, 96, 96,
                   PixelFormats.Gray16,
                   null, pixelData, stride);
                    }
                    else
                    {
                        this.VideoControl.Source = BitmapSource.Create(
                                         imageFrame.Width,
                                         imageFrame.Height,
                                         96,
                                         96,
                                         PixelFormats.Bgr32,
                                         null,
                                         pixelData,
                                         stride);
                    }




                }
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

                skeletonFrame.CopySkeletonDataTo(totalSkeleton);
                Skeleton skeleton;

                if (CurrentSkeletonID != 0)
                {
                    skeleton = (from trackSkeleton in totalSkeleton
                                where trackSkeleton.TrackingState == SkeletonTrackingState.Tracked &&
                                trackSkeleton.TrackingId == CurrentSkeletonID
                                select trackSkeleton).FirstOrDefault();
                    if (skeleton == null)
                    {
                        CurrentSkeletonID = 0;
                        this.sensor.SkeletonStream.AppChoosesSkeletons = false;
                    }
                }
                else
                {
                    skeleton = (from trackSkeleton in totalSkeleton
                                where trackSkeleton.TrackingState == SkeletonTrackingState.Tracked
                                select trackSkeleton).FirstOrDefault();

                    if (skeleton == null)
                    {
                        return;
                    }
                    else
                    {
                        CurrentSkeletonID = skeleton.TrackingId;
                        this.sensor.SkeletonStream.AppChoosesSkeletons = true;
                        this.sensor.SkeletonStream.ChooseSkeletons(CurrentSkeletonID);
                    }
                    if (skeleton.Joints[JointType.Head].TrackingState == JointTrackingState.Tracked)
                    {
                        this.SaveImage();
                    }
                }
            }
        }

        /// <summary>
        /// Saves the image.
        /// </summary>
        private void SaveImage()
        {
            using (FileStream fileStream = new FileStream(string.Format("{0}.Jpg", Guid.NewGuid().ToString()), System.IO.FileMode.Create))
            {
                BitmapSource imageSource = (BitmapSource)VideoControl.Source;
                JpegBitmapEncoder jpegEncoder = new JpegBitmapEncoder();
                jpegEncoder.Frames.Add(BitmapFrame.Create(imageSource));
                jpegEncoder.Save(fileStream);
                fileStream.Close();
            }
        }

        /// <summary>
        /// Controls the camera stream.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void ControlCameraStream(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb.IsChecked == true)
            {
                this.sensor.ColorStream.Enable(ColorImageFormat.InfraredResolution640x480Fps30);
                nightVision = true;
            }
            else if (cb.IsChecked == false)
            {
                this.sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
                nightVision = false;

            }
        }

    }
}
