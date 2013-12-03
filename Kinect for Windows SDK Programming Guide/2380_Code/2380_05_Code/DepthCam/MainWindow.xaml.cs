// Working with Raw Depth Data and Reversion data based in distance
namespace DepthCam
{
    using Microsoft.Kinect;
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Kinect Sensor
        /// </summary>
        KinectSensor sensor;

        MainWindowViewModel ViewModel;

        byte[] depth32;

        short[] pixelData;
        DepthImageFrame frame;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(MainWindow_Loaded);
            Closed += new EventHandler(MainWindow_Closed);
            this.ViewModel = new MainWindowViewModel();
            this.DataContext = this.ViewModel;

            foreach (DepthRange ranges in Enum.GetValues(typeof(DepthRange)))
            {
                rangeCombo.Items.Add(ranges);
            }

            this.rangeCombo.SelectedIndex = 0;
        }

        /// <summary>
        /// Handles the Closed event of the MainWindow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void MainWindow_Closed(object sender, EventArgs e)
        {
            if (this.sensor != null && this.sensor.IsRunning)
            {
                this.sensor.Stop();
            }
        }

        /// <summary>
        /// Handles the Loaded event of the MainWindow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (KinectSensor.KinectSensors.Count > 0)
            {
                sensor = KinectSensor.KinectSensors[0];
                sensor.Start();
                sensor.DepthStream.Enable(DepthImageFormat.Resolution320x240Fps30);
                this.ViewModel.FrameHeight = sensor.DepthStream.FrameHeight;
                this.ViewModel.FrameWidth = sensor.DepthStream.FrameWidth;
                sensor.SkeletonStream.Enable();
                sensor.DepthFrameReady += new EventHandler<DepthImageFrameReadyEventArgs>(sensor_DepthFrameReady);
            }
            else
            {
                MessageBox.Show("No Device Connected !! ");
            }
        }

        /// <summary>
        /// Handles the DepthFrameReady event of the sensor control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Microsoft.Kinect.DepthImageFrameReadyEventArgs"/> instance containing the event data.</param>
        void sensor_DepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            using (DepthImageFrame depthimageFrame = e.OpenDepthImageFrame())
            {
                if (depthimageFrame == null)
                {
                    return;
                }
                frame = depthimageFrame;
                pixelData = new short[depthimageFrame.PixelDataLength];

                this.ViewModel.MaxDepth = depthimageFrame.MaxDepth;
                this.ViewModel.MinDepth = depthimageFrame.MinDepth;

                depthimageFrame.CopyPixelDataTo(pixelData);

                //short[] reversePixelData = new short[depthimageFrame.PixelDataLength];
                //reversePixelData = this.ReversingBitValueWithDistance(depthimageFrame, pixelData);

                if (trackPlayer.IsChecked == true)
                {
                    depth32 = new byte[depthimageFrame.PixelDataLength * 4];
                    this.TrackPlayer(pixelData);
                    depthImageControl.Source = BitmapSource.Create(
              depthimageFrame.Width, depthimageFrame.Height, 96, 96, PixelFormats.Bgr32, null, depth32, depthimageFrame.Width * 4
              );
                }
                else if (colorizedCheck.IsChecked == true)
                {
                    depth32 = new byte[depthimageFrame.PixelDataLength * 4];
                    this.GetColorPixelDataWithDistance(pixelData);
                    depthImageControl.Source = BitmapSource.Create(
                depthimageFrame.Width, depthimageFrame.Height, 96, 96, PixelFormats.Bgr32, null, depth32, depthimageFrame.Width * 4
                );
                }
                else
                {
                    depthImageControl.Source = BitmapSource.Create(
                       depthimageFrame.Width, depthimageFrame.Height, 96, 96, PixelFormats.Gray16, null, pixelData, depthimageFrame.Width * depthimageFrame.BytesPerPixel
                       );
                }
                //depthImageControl.Source = BitmapSource.Create(
                // depthimageFrame.Width, depthimageFrame.Height, 96, 96, PixelFormats.Gray16, null, reversePixelData, depthimageFrame.Width * depthimageFrame.BytesPerPixel
                // );

            }
        }

        /// <summary>
        /// Reversings the bit value with distance.
        /// </summary>
        /// <param name="depthImageFrame">The depth image frame.</param>
        /// <param name="pixelData">The pixel data.</param>
        /// <returns></returns>
        private short[] ReversingBitValueWithDistance(DepthImageFrame depthImageFrame, short[] pixelData)
        {
            short[] reverseBitPixelData = new short[depthImageFrame.PixelDataLength];
            int depth;
            for (int index = 0; index < pixelData.Length; index++)
            {
                // Caculate the distance
                depth = pixelData[index] >> DepthImageFrame.PlayerIndexBitmaskWidth;


                int player = pixelData[index] & DepthImageFrame.PlayerIndexBitmask;

                // Change the pixel value 
                if (depth < 1500 || depth > 3500)
                {
                    reverseBitPixelData[index] = (short)~pixelData[index]; ;
                }
                else
                {
                    reverseBitPixelData[index] = pixelData[index];
                }
            }

            return reverseBitPixelData;
        }

        /// <summary>
        /// Handles the MouseDown event of the depthImageControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs" /> instance containing the event data.</param>
        private void depthImageControl_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Point currentPoint = e.GetPosition(depthImageControl);
         
            // get the index postion from the moused clicked position and the pixel width and height
            int pixelIndex = (int)(currentPoint.X + ((int)currentPoint.Y * this.frame.Width));
          
            int distancemm = this.pixelData[pixelIndex] >> DepthImageFrame.PlayerIndexBitmaskWidth;
         
            // 1m is equal to 1000 mm, or 39.370078 inches
            const decimal convert = 0.039370078m;
            int distanceinc = (int)(distancemm * convert);
            int distanceft = distanceinc / 12;
            
            // Get only feet distance 
            //double distancefeet = (double)(distancemm * 0.00328);
            
            ViewModel.Depth = distancemm;
            ViewModel.X = (int)currentPoint.X;
            ViewModel.Y = (int)currentPoint.Y;
            ViewModel.Distance = string.Format("{0}'{1}\"", distanceft, distanceinc);
            
        }

        /// <summary>
        /// Handles the Checked event of the CheckBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }


        /// <summary>
        /// Tracks the player.
        /// </summary>
        /// <param name="depthFrame">The depth frame.</param>
        private void TrackPlayer(short[] depthFrame)
        {
            for (int depthIndex = 0, colorIndex = 0; depthIndex < depthFrame.Length && colorIndex < this.depth32.Length; depthIndex++, colorIndex += 4)
            {
                // Get the player
                int player = depthFrame[depthIndex] & DepthImageFrame.PlayerIndexBitmask;
                // Color the all pixels associated with a player
                if (player > 0)
                {
                    depth32[colorIndex + 2] = 169;
                    depth32[colorIndex + 1] = 62;
                    depth32[colorIndex + 0] = 9;
                }
            }
        }

        /// <summary>
        /// Gets the color pixel data with distance.
        /// </summary>
        /// <param name="depthFrame">The depth frame.</param>
        private void GetColorPixelDataWithDistance(short[] depthFrame)
        {
            for (int depthIndex = 0, colorIndex = 0; depthIndex < depthFrame.Length && colorIndex < this.depth32.Length; depthIndex++, colorIndex += 4)
            {
                // Calculate the depth distance
                int distance = depthFrame[depthIndex] >> DepthImageFrame.PlayerIndexBitmaskWidth;
                // Colorize pixels for a range of distance
                if (distance <= 1000)
                {
                    depth32[colorIndex + 2] = 115; // red
                    depth32[colorIndex + 1] = 169;  // green
                    depth32[colorIndex + 0] = 9; // blue

                }
                else if (distance > 1000 && distance <= 2500)
                {
                    depth32[colorIndex + 2] = 255;
                    depth32[colorIndex + 1] = 61;
                    depth32[colorIndex + 0] = 0;
                }
                else if (distance > 2500)
                {
                    depth32[colorIndex + 2] = 169;
                    depth32[colorIndex + 1] = 9;
                    depth32[colorIndex + 0] = 115;
                }
            }
        }

        /// <summary>
        /// Handles the SelectionChanged event of the rangeCombo control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs" /> instance containing the event data.</param>
        private void rangeCombo_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (sensor != null && sensor.IsRunning && sensor.DepthStream.IsEnabled)
            {
                this.sensor.DepthStream.Range = (DepthRange)(Enum.Parse(typeof(DepthRange), (sender as ComboBox).SelectedItem.ToString()));
            }

        }

        /// <summary>
        /// Handles the Checked event of the TrackPlayer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void TrackPlayer_Checked(object sender, RoutedEventArgs e)
        {

        }



    }
}