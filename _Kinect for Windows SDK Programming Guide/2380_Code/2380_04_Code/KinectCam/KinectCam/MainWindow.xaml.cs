namespace KinectCam
{
    using System;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Threading;
    using KinectStatusNotifier;
    using Microsoft.Kinect;
    using System.Linq;
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Kinect Sensor Instance
        /// </summary>
       private KinectSensor sensor;

       /// <summary>
       /// Kinect Status Notifer
       /// </summary>
       private StatusNotifier statusNotifier = new StatusNotifier();

        /// <summary>
        /// The Dispatched Timer
        /// </summary>
        private DispatcherTimer timer = new DispatcherTimer();

        /// <summary>
        /// View Model 
        /// </summary>
        private MainWindowViewModel viewModel;

        /// <summary>
        /// Color Image Wrapper
        /// </summary>
        private ColorImageWrapper imageFramevalue;

        /// <summary>
        /// Current frame rate
        /// </summary>
        private int currentFrameRate = 0;

        /// <summary>
        /// last count time
        /// </summary>
        private DateTime lastTime = DateTime.MaxValue;

        /// <summary>
        /// Gets or sets the pixel data.
        /// </summary>
        /// <value>The pixel data.</value>
        private byte[] pixelData;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow" /> class.
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();
            this.viewModel = new MainWindowViewModel();
            this.DataContext = this.viewModel;
            this.Loaded += this.MainWindow_Loaded;
        }

        /// <summary>
        /// Gets or sets the total frames.
        /// </summary>
        /// <value>
        /// The total frames.
        /// </value>
        private int TotalFrames { get; set; }

        /// <summary>
        /// Gets or sets the last frames.
        /// </summary>
        /// <value>
        /// The last frames.
        /// </value>
        private int LastFrames { get; set; }

        /// <summary>
        /// Gets or sets the image frame.
        /// </summary>
        /// <value>
        /// The image frame.
        /// </value>
        private ColorImageWrapper ImageFrame
        {
            get
            {
                return this.imageFramevalue;
            }

            set
            {
                if (this.imageFramevalue != null && this.imageFramevalue.NeedDispose)
                {
                    this.imageFramevalue.Dispose();
                }

                this.imageFramevalue = value;
            }
        }

        /// <summary>
        /// Starts the Timer
        /// </summary>
        public void StartTimer()
        {
            this.timer.Interval = new TimeSpan(0, 0, 10);
            this.timer.Start();
            this.timer.Tick += this.Timer_Tick;
        }

        /// <summary>
        /// Stops the timer.
        /// </summary>
        public void StopTimer()
        {
            this.timer.Stop();
            this.timer.Tick -= this.Timer_Tick;
        }

        /// <summary>
        /// Timer Tick Event
        /// </summary>
        /// <param name="sender">Objects as sender</param>
        /// <param name="e">Event Args</param>
        public void Timer_Tick(object sender, object e)
        {
            if (this.sensor.IsRunning && this.sensor.ColorStream.IsEnabled)
            {
                this.SaveImage();
            }
        }

        /// <summary>
        /// Handles the Loaded event of the MainWindow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        /// <exception cref="System.NotImplementedException">Not Implemented Exception</exception>
        protected void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.StartKinectCam();
        }

        /// <summary>
        /// Handles the ColorFrameReady event of the sensor control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ColorImageFrameReadyEventArgs" /> instance containing the event data.</param>
        protected void sensor_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            ColorImageFrame imageFrame = e.OpenColorImageFrame();      
            
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

                //// uncomments this when need to save images from image frame.
                //// this.ImageFrame = new ColorImageWrapper(imageFrame);

                if (this.viewModel.IsFrameNumberEnabled)
                {
                    this.viewModel.FrameNumber = this.GetCurrentFrameNumber(imageFrame);
                }

                if (this.viewModel.IsFrameRateEnabled)
                {
                    this.viewModel.FrameRate = this.GetCurrentFrameRate();
                }

                if (this.viewModel.CurrentColorEffect != Effects.None)
                {
                    switch (this.viewModel.CurrentColorEffect)
                    {
                        case Effects.Red:
                            for (int i = 0; i < this.pixelData.Length; i += imageFrame.BytesPerPixel)
                            {
                                this.pixelData[i] = 0;
                                this.pixelData[i + 1] = 0;
                            }

                            break;
                        case Effects.Green:
                            for (int i = 0; i < this.pixelData.Length; i += imageFrame.BytesPerPixel)
                            {
                                this.pixelData[i] = 0;
                                this.pixelData[i + 2] = 0;
                            }

                            break;
                        case Effects.Blue:
                            for (int i = 0; i < this.pixelData.Length; i += imageFrame.BytesPerPixel)
                            {
                                this.pixelData[i + 1] = 0;
                                this.pixelData[i + 2] = 0;
                            }

                            break;
                        default:
                            break;
                    }
                }

                if (this.viewModel.IsInvertColorEffectsEnabled)
                {
                    for (int i = 0; i < this.pixelData.Length; i += imageFrame.BytesPerPixel)
                    {
                        this.pixelData[i] = (byte)~this.pixelData[i];
                        this.pixelData[i + 1] = (byte)~this.pixelData[i + 1];
                        this.pixelData[i + 2] = (byte)~this.pixelData[i + 2];
                    }
                }

                if (this.viewModel.IsGrayScaleEnabled)
                {
                    for (int i = 0; i < this.pixelData.Length; i += imageFrame.BytesPerPixel)
                    {
                        var data = Math.Max(Math.Max(this.pixelData[i], this.pixelData[i + 1]), this.pixelData[i + 2]);
                        this.pixelData[i] = data;
                        this.pixelData[i + 1] = data;
                        this.pixelData[i + 2] = data;
                    }
                }

                // Calculate the stride
                int stride = imageFrame.Width * imageFrame.BytesPerPixel;

                // As per needed, here we have to apply alogritham to convert Bayer to RGB.
                if (imageFrame.Format == ColorImageFormat.RawBayerResolution640x480Fps30 ||
                       imageFrame.Format == ColorImageFormat.RawBayerResolution1280x960Fps12 || imageFrame.Format == ColorImageFormat.RawYuvResolution640x480Fps15)
                {
                    return;
                }
              
                // assign the bitmap image source into image control
                if (imageFrame.Format == ColorImageFormat.InfraredResolution640x480Fps30)
                {
                    this.VideoControl.Source = BitmapSource.Create(
               imageFrame.Width,
               imageFrame.Height,
               96,
               96,
               PixelFormats.Gray16,
               null,
               this.pixelData,
               stride);
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
                this.pixelData,
                stride);
                }


               
            }
        }

        /// <summary>
        /// Starts the kinect cam.
        /// </summary>
        private void StartKinectCam()
        {
            if (KinectSensor.KinectSensors.Count > 0)
            {
                this.sensor = KinectSensor.KinectSensors.FirstOrDefault(sensorItem => sensorItem.Status == KinectStatus.Connected);
                this.statusNotifier.Sensors = KinectSensor.KinectSensors;
                this.StartSensor();
                this.sensor.ColorStream.Enable();
                this.sensor.ColorFrameReady += this.sensor_ColorFrameReady;
                this.ColorImageFormatSelection.SelectedIndex = 1;
                this.EffectsCombo.SelectedIndex = 0;
            }
            else
            {
                MessageBox.Show("No device is connected with system!");
                this.Close();
            }
        }

        /// <summary>
        /// Gets the current frame rate.
        /// </summary>
        /// <returns>return the frame rate</returns>
        private int GetCurrentFrameRate()
        {
            ++this.TotalFrames;
            DateTime currentTime = DateTime.Now;
            var timeSpan = currentTime.Subtract(this.lastTime);

            if (this.lastTime == DateTime.MaxValue || timeSpan >= TimeSpan.FromSeconds(1))
            {
                this.currentFrameRate = (int)Math.Round((this.TotalFrames - this.LastFrames) / timeSpan.TotalSeconds);
                this.LastFrames = this.TotalFrames;
                this.lastTime = currentTime;
            }

            return this.currentFrameRate;
        }

        /// <summary>
        /// Gets the current frame number.
        /// </summary>
        /// <param name="imageFrame">The image frame.</param>
        /// <returns>return the frame number</returns>
        private int GetCurrentFrameNumber(ColorImageFrame imageFrame)
        {
            return imageFrame.FrameNumber;
        }

        /// <summary>
        /// Handles the Click event of the ButtonStart control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void ButtonStart_Click(object sender, RoutedEventArgs e)
        {
            this.StartSensor();
        }

        /// <summary>
        /// Handles the Click event of the ButtonStop control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void ButtonStop_Click(object sender, RoutedEventArgs e)
        {
            this.StopSensor();
        }

        /// <summary>
        /// Handles the Click event of the ButtonExit control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            this.StopSensor();
            this.Close();
        }

        /// <summary>
        /// Starts the sensor.
        /// </summary>
        private void StartSensor()
        {
            if (this.sensor != null && !this.sensor.IsRunning)
            {
                this.sensor.Start();
                this.viewModel.CanStart = false;
                this.viewModel.CanStop = true;
            }
        }

        /// <summary>
        /// Stops the sensor.
        /// </summary>
        private void StopSensor()
        {
            if (this.sensor != null && this.sensor.IsRunning)
            {
                this.sensor.Stop();
                this.viewModel.CanStart = true;
                this.viewModel.CanStop = false;
            }
        }

        /// <summary>
        /// Handles the Click event of the ButtonSave control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            // Check if sensor is running and color stream is enabled.
            if (this.sensor.IsRunning && this.sensor.ColorStream.IsEnabled)
            {
                // Uncomment this for saving images directly from image frame
                ////using (this.ImageFrame)
                ////{
                ////    ColorImageFrame imageFrame = this.ImageFrame.ImageFrame;
                ////    this.SaveImage(imageFrame);
                ////    this.LoadImages();
                ////}

                this.SaveImage();
                this.LoadImages();
            }
        }

        /// <summary>
        /// Saves the image.
        /// </summary>
        /// <param name="imageFrame">The image frame.</param>
        private void SaveImage(ColorImageFrame imageFrame)
        {
            // Check if incoming frame is null
            if (imageFrame == null)
            {
                return;
            }

            // Create a new Stream with file name of image frame number.
            using (FileStream fileStream = new FileStream(string.Format("{0}.Jpg", imageFrame.FrameNumber.ToString()), System.IO.FileMode.Create))
            {
                // Get byte[] of image frame and create the bitmap image source
                byte[] pixelData1 = new byte[imageFrame.PixelDataLength];
                imageFrame.CopyPixelDataTo(pixelData1);
                BitmapSource imageSource = BitmapSource.Create(imageFrame.Width, imageFrame.Height, 96, 96, PixelFormats.Bgr32, null, pixelData1, imageFrame.Width * 4);
                JpegBitmapEncoder jpegEncoder = new JpegBitmapEncoder();
                jpegEncoder.Frames.Add(BitmapFrame.Create(imageSource));
                jpegEncoder.Save(fileStream);
                fileStream.Close();
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
        /// Loads the images.
        /// </summary>
        private void LoadImages()
        {
            ObservableCollection<BitmapImage> images = new ObservableCollection<BitmapImage>();
            DirectoryInfo dinfo = new DirectoryInfo(Directory.GetCurrentDirectory());
            foreach (FileInfo finfo in dinfo.GetFiles("*.jpg"))
            {
                BitmapImage bimg = new BitmapImage(new Uri(finfo.FullName));
                images.Add(bimg);
            }

            this.imgImages.DataContext = images;
        }

        /// <summary>
        /// Handles the SelectionChanged event of the ComboBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs" /> instance containing the event data.</param>
        private void ComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            this.ChangeColorImageFormat();
        }

        /// <summary>
        /// Changes the color image format.
        /// </summary>
        private void ChangeColorImageFormat()
        {
            if (this.sensor.IsRunning)
            {
                this.viewModel.CurrentImageFormat = (ColorImageFormat)this.ColorImageFormatSelection.SelectedItem;
                this.sensor.ColorStream.Enable(this.viewModel.CurrentImageFormat == ColorImageFormat.Undefined ? ColorImageFormat.RgbResolution640x480Fps30 : this.viewModel.CurrentImageFormat);
            }
        }

        /// <summary>
        /// Handles the ValueChanged event of the Slider control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedPropertyChangedEventArgs" /> instance containing the event data.</param>
        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.SetSensorAngle(int.Parse(e.NewValue.ToString()));
        }

        /// <summary>
        /// Sets the sensor angle.
        /// </summary>
        /// <param name="angleValue">The angle value.</param>
        private void SetSensorAngle(int angleValue)
        {
            if (angleValue > this.sensor.MinElevationAngle || angleValue < this.sensor.MaxElevationAngle)
            {
                this.viewModel.SensorAngle = angleValue;
                this.sensor.ElevationAngle = this.viewModel.SensorAngle;
            }
        }

        /// <summary>
        /// Handles the 1 event of the CheckBox_Checked control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void CheckBox_Checked_1(object sender, RoutedEventArgs e)
        {
            if (AutoFrameCapture.IsChecked == true)
            {
                this.StartTimer();
            }
            else
            {
                this.StopTimer();
            }
        }

        /// <summary>
        /// Handles the SelectionChanged event of the EffectsCombo control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs" /> instance containing the event data.</param>
        private void EffectsCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.sensor.IsRunning)
            {
                this.viewModel.CurrentColorEffect = (Effects)this.EffectsCombo.SelectedItem;
            }
        }

        /// <summary>
        /// Handles the checking or unchecking of the invert color selection
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void InvertColorCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (InvertColorCheckBox.IsChecked == true)
            {
                this.viewModel.IsInvertColorEffectsEnabled = true;
                this.viewModel.IsGrayScaleEnabled = false;
            }
            else
            {
                this.viewModel.IsInvertColorEffectsEnabled = false;
            }
        }

        /// <summary>
        /// Handles the checking or unchecking of the gray scale selection
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void GrayScaleChecked(object sender, RoutedEventArgs e)
        {
            if (GrayScaleCheckBox.IsChecked == true)
            {
                this.viewModel.IsGrayScaleEnabled = true;
                this.viewModel.IsInvertColorEffectsEnabled = false;
            }
            else
            {
                this.viewModel.IsGrayScaleEnabled = false;
            }
        }

        /// <summary>
        /// Manages the frame rate display.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void ManageFrameRateDisplay(object sender, RoutedEventArgs e)
        {
            if (FrameRateCheckBox.IsChecked == true)
            {
                this.viewModel.IsFrameRateEnabled = true;
            }
            else
            {
                this.viewModel.IsFrameRateEnabled = false;
            }
        }

        /// <summary>
        /// Manageds the frame number display.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void ManagedFrameNumberDisplay(object sender, RoutedEventArgs e)
        {
            if (FrameNumberCheckBox.IsChecked == true)
            {
                this.viewModel.IsFrameNumberEnabled = true;
            }
            else
            {
                this.viewModel.IsFrameNumberEnabled = false;
            }
        }
    }
}
