
namespace CameraSettings
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Threading;
    using Microsoft.Kinect;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        /// <summary>
        ///  Define the kinect sensor
        /// </summary>
        private KinectSensor sensor;

        /// <summary>
        /// Gets or sets the pixel data.
        /// </summary>
        /// <value>The pixel data.</value>
        private byte[] pixelData { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(this.MainWindow_Loaded);
            Unloaded += new RoutedEventHandler(MainWindow_Unloaded);
        }

        /// <summary>
        /// Loads the basic camera settings.
        /// </summary>
        private void LoadBasicCameraSettings()
        {
            this.sliderBrightness.Maximum = this.sensor.ColorStream.CameraSettings.MaxBrightness;
            this.sliderBrightness.Minimum = this.sensor.ColorStream.CameraSettings.MinBrightness;
            this.sliderBrightness.Value = this.sensor.ColorStream.CameraSettings.Brightness;

            this.sliderContrast.Maximum = this.sensor.ColorStream.CameraSettings.MaxContrast;
            this.sliderContrast.Minimum = this.sensor.ColorStream.CameraSettings.MinContrast;
            this.sliderContrast.Value = this.sensor.ColorStream.CameraSettings.Contrast;

            this.sliderSaturation.Maximum = this.sensor.ColorStream.CameraSettings.MaxSaturation;
            this.sliderSaturation.Minimum = this.sensor.ColorStream.CameraSettings.MinSaturation;
            this.sliderSaturation.Value = this.sensor.ColorStream.CameraSettings.Saturation;

            this.sliderGamma.Maximum = this.sensor.ColorStream.CameraSettings.MaxGamma;
            this.sliderGamma.Minimum = this.sensor.ColorStream.CameraSettings.MinGamma;
            this.sliderGamma.Value = this.sensor.ColorStream.CameraSettings.Gamma;

            this.sliderHue.Maximum = this.sensor.ColorStream.CameraSettings.MaxHue;
            this.sliderHue.Minimum = this.sensor.ColorStream.CameraSettings.MinHue;
            this.sliderHue.Value = this.sensor.ColorStream.CameraSettings.Hue;

            this.sliderSharpness.Maximum = this.sensor.ColorStream.CameraSettings.MaxSharpness;
            this.sliderSharpness.Minimum = this.sensor.ColorStream.CameraSettings.MinSharpness;
            this.sliderSharpness.Value = this.sensor.ColorStream.CameraSettings.Sharpness;

            this.sliderGain.Maximum = this.sensor.ColorStream.CameraSettings.MaxGain;
            this.sliderGain.Minimum = this.sensor.ColorStream.CameraSettings.MinGain;
            this.sliderGain.Value = this.sensor.ColorStream.CameraSettings.Gain;

            this.sliderWhiteBalance.Maximum = this.sensor.ColorStream.CameraSettings.MaxWhiteBalance;
            this.sliderWhiteBalance.Minimum = this.sensor.ColorStream.CameraSettings.MinWhiteBalance;
            this.sliderWhiteBalance.Value = this.sensor.ColorStream.CameraSettings.WhiteBalance;
          
            this.sensor.ColorStream.CameraSettings.AutoWhiteBalance=false;

            this.autoExpCheck.IsChecked = this.sensor.ColorStream.CameraSettings.AutoExposure;
            this.radioAverageBrightness.IsChecked = this.sensor.ColorStream.CameraSettings.AutoExposure;

            this.SliderframeInterval.Maximum = this.sensor.ColorStream.CameraSettings.MaxFrameInterval;
            this.SliderframeInterval.Minimum = this.sensor.ColorStream.CameraSettings.MinFrameInterval;
            this.SliderframeInterval.Value = this.sensor.ColorStream.CameraSettings.FrameInterval;
            
        }

        /// <summary>
        /// Handles the Unloaded event of the MainWindow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void MainWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            this.sensor.Stop();
        }

        /// <summary>
        /// Handles the Loaded event of the MainWindow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        protected void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {

            // Check if there kinect connected
            if (KinectSensor.KinectSensors.Count > 0)
            {
                // Get the first kinect sensor
                this.sensor = KinectSensor.KinectSensors[0];

                // start the sensor
                this.sensor.Start();
                LoadBasicCameraSettings();

                if (!this.sensor.ColorStream.IsEnabled)
                {
                    this.sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
                    this.sensor.ColorFrameReady += new EventHandler<ColorImageFrameReadyEventArgs>(sensor_ColorFrameReady);
                }
            }
            else
            {
                MessageBox.Show("No Device Connected");
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
                if (imageFrame != null)
                {
                    // Get the pixel data in byte array
                    this.pixelData = new byte[imageFrame.PixelDataLength];
                    // Copy the pixel data
                    imageFrame.CopyPixelDataTo(this.pixelData);


                    int stride = imageFrame.Width * imageFrame.BytesPerPixel;

                    // assign the bitmap image source into image control
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

        /// <summary>
        /// Sliders the brightness_ value changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void sliderBrightness_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal,
                 new Action(
                     delegate()
                     {
                         this.sensor.ColorStream.CameraSettings.Brightness = e.NewValue;
                     }
                 ));
        }

        /// <summary>
        /// Sliders the contrast_ value changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void sliderContrast_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal,
                new Action(
                    delegate()
                    {
                        this.sensor.ColorStream.CameraSettings.Contrast = e.NewValue;
                    }
             ));
        }

        /// <summary>
        /// Sliders the saturation_ value changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void sliderSaturation_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal,
                new Action(
                    delegate()
                    {
                        this.sensor.ColorStream.CameraSettings.Saturation = e.NewValue;
                    }
                ));
        }

        /// <summary>
        /// Sliders the hue_ value changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void sliderHue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal,
                new Action(
                    delegate()
                    {
                        this.sensor.ColorStream.CameraSettings.Hue = e.NewValue;
                    }
                ));
        }

        /// <summary>
        /// Sliders the gamma_ value changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void sliderGamma_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal,
                new Action(
                    delegate()
                    {
                        this.sensor.ColorStream.CameraSettings.Gamma = e.NewValue;
                    }
                ));
        }

        /// <summary>
        /// Sliders the sharpness_ value changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void sliderSharpness_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal,
                new Action(
                    delegate()
                    {
                        this.sensor.ColorStream.CameraSettings.Sharpness = e.NewValue;
                    }
                ));
        }

        /// <summary>
        /// Sliders the gain_ value changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void sliderGain_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal,
               new Action(
                   delegate()
                   {
                       this.sensor.ColorStream.CameraSettings.Gain = e.NewValue;
                   }
               ));
        }

        /// <summary>
        /// Sliders the white balance_ value changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void sliderWhiteBalance_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal,
               new Action(
                   delegate()
                   {
                       this.sensor.ColorStream.CameraSettings.WhiteBalance = Int32.Parse(e.NewValue.ToString());
                   }
               ));
        }

        /// <summary>
        /// Handles the Checked event of the RadioButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            this.sensor.ColorStream.CameraSettings.BacklightCompensationMode = BacklightCompensationMode.AverageBrightness;
        }

        /// <summary>
        /// Handles the 1 event of the RadioButton_Checked control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void RadioButton_Checked_1(object sender, RoutedEventArgs e)
        {
            this.sensor.ColorStream.CameraSettings.BacklightCompensationMode = BacklightCompensationMode.CenterOnly;
        }

        /// <summary>
        /// Handles the 2 event of the RadioButton_Checked control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void RadioButton_Checked_2(object sender, RoutedEventArgs e)
        {
            this.sensor.ColorStream.CameraSettings.BacklightCompensationMode = BacklightCompensationMode.CenterPriority;
        }

        /// <summary>
        /// Handles the 3 event of the RadioButton_Checked control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void RadioButton_Checked_3(object sender, RoutedEventArgs e)
        {
            this.sensor.ColorStream.CameraSettings.BacklightCompensationMode = BacklightCompensationMode.LowlightsPriority;
        }

        /// <summary>
        /// Exits the click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void ExitClick(object sender, RoutedEventArgs e)
        {
            if (this.sensor != null && this.sensor.IsRunning)
            {
                this.sensor.Stop();
            }
        }

        /// <summary>
        /// Frames the interval changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void frameIntervalChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal,
             new Action(
                 delegate()
                 {
                     this.sensor.ColorStream.CameraSettings.FrameInterval = e.NewValue;
                 }
             ));
        }

        /// <summary>
        /// Handles the Checked event of the CheckBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox chkBox = sender as CheckBox;

            if (chkBox.IsChecked == true)
            {
                this.sensor.ColorStream.CameraSettings.AutoExposure = true;
                this.AutoExposerSettings.IsEnabled = true;
                SliderframeInterval.IsEnabled = false;
            }
            else
            {
                this.sensor.ColorStream.CameraSettings.AutoExposure = false;
                this.AutoExposerSettings.IsEnabled = false;
                SliderframeInterval.IsEnabled = true;
            }
        }




    }
}
