
namespace KinectSoundRecorder
{
    using System;
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Threading;
    using Microsoft.Kinect;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Kinect sensor
        /// </summary>
        private KinectSensor sensor;

        /// <summary>
        /// Dispatcher timer for kinect sensor
        /// </summary>
        DispatcherTimer dispatcherTimer = new DispatcherTimer();

        /// <summary>
        /// Audio source for kinect
        /// </summary>
        KinectAudioSource audioSource;

        /// <summary>
        /// Status Message
        /// </summary>
        public string StatusMessage { get; set; }

        /// <summary>
        /// Current Time
        /// </summary>
        int currenttime = 0;

        /// <summary>
        /// Gets or sets the view model.
        /// </summary>
        /// <value>The view model.</value>
        public MainWindowViewModel ViewModel { get; set; }

        /// <summary>
        /// Check if Playing is in progress
        /// </summary>
        bool playinginProgress;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(MainWindow_Loaded);
            this.ViewModel = new MainWindowViewModel();
            this.DataContext = this.ViewModel;

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
                this.sensor = KinectSensor.KinectSensors[0];
                this.audioSource = this.sensor.AudioSource;
                this.audioSource.NoiseSuppression = ViewModel.IsNoiseSuppression;
                this.audioSource.AutomaticGainControlEnabled = ViewModel.IsGainControl;
                this.sensor.AudioSource.SoundSourceAngleChanged += new EventHandler<SoundSourceAngleChangedEventArgs>(AudioSource_SoundSourceAngleChanged);
                this.sensor.AudioSource.BeamAngleChanged += new EventHandler<BeamAngleChangedEventArgs>(AudioSource_BeamAngleChanged);
                if (ViewModel.IsEchoCancelation)
                {
                    this.audioSource.EchoCancellationMode = EchoCancellationMode.CancellationOnly;
                    this.audioSource.EchoCancellationSpeakerIndex = 0;
                }

                this.sensor.Start();
                labelStatusMessage.Content = "Press Start for Recording.";
            }
            else
            {
                labelStatusMessage.Content = "Recording Completed.";
            }
        }

        /// <summary>
        /// Handles the Click event of the buttonStart control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void buttonStart_Click(object sender, RoutedEventArgs e)
        {
            this.ViewModel.StartRecording();
            this.PlugDispatcher();
            var audioThread = new Thread(new ThreadStart(this.RecordAudio));
            audioThread.SetApartmentState(ApartmentState.MTA);
            audioThread.Start();
        }

        /// <summary>
        /// Plugs the dispatcher.
        /// </summary>
        private void PlugDispatcher()
        {
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            currenttime = 0;
            dispatcherTimer.Start();

        }

        /// <summary>
        /// Handles the Click event of the buttonStop control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void buttonStop_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new ThreadStart(this.ViewModel.StopRecording));
            labelStatusMessage.Content = "Recording Stopped";
        }

        /// <summary>
        /// Handles the Click event of the buttonPlay control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void buttonPlay_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty("d:\\kinectAudio.wav") && File.Exists("d:\\kinectAudio.wav"))
            {
                this.PlugDispatcher();
                kinectaudioPlayer.Source = new Uri("d:\\kinectAudio.wav", UriKind.RelativeOrAbsolute);
                kinectaudioPlayer.LoadedBehavior = MediaState.Play;
                kinectaudioPlayer.UnloadedBehavior = MediaState.Close;
                labelStatusMessage.Content = "Playing in Progress";
                playinginProgress = true;
            }
        }

        /// <summary>
        /// Handles the Tick event of the dispatcherTimer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            this.Dispatcher.BeginInvoke((Action)delegate()
            {
                this.audioProgress.Value = currenttime++;
                if (!playinginProgress)
                {
                    labelStatusMessage.Content = string.Format("Recording in Progress : {0} Sec. ", this.audioProgress.Value);
                }
                else
                {
                    labelStatusMessage.Content = string.Format("Playing in Progress : {0} Sec. ", this.dispatcherTimer.Interval);
                }
            });
            if (currenttime == 10)
            {
                this.dispatcherTimer.Stop();
                currenttime = 0;
                Dispatcher.BeginInvoke(new ThreadStart(this.ViewModel.RecordingCompleted));
                this.Dispatcher.BeginInvoke((Action)delegate()
                {
                    labelStatusMessage.Content = "Recording Completed.";
                });
            }

        }

        /// <summary>
        /// WAVEFORMATEX
        /// </summary>
        struct WAVEFORMATEX
        {
            public ushort wFormatTag;
            public ushort nChannels;
            public uint nSamplesPerSec;
            public uint nAvgBytesPerSec;
            public ushort nBlockAlign;
            public ushort wBitsPerSample;
            public ushort cbSize;
        }


        /// <summary>
        /// Writes the wav header.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="dataLength">Length of the data.</param>
        static void WriteWavHeader(Stream stream, int dataLength)
        {
            using (var memStream = new MemoryStream(64))
            {
                int cbFormat = 18; //sizeof(WAVEFORMATEX)
                WAVEFORMATEX format = new WAVEFORMATEX()
                {
                    wFormatTag = 1,
                    nChannels = 1,
                    nSamplesPerSec = 16000,
                    nAvgBytesPerSec = 32000,
                    nBlockAlign = 2,
                    wBitsPerSample = 16,
                    cbSize = 0
                };

                using (var binarywriter = new BinaryWriter(memStream))
                {
                    //RIFF header
                    WriteString(memStream, "RIFF");
                    binarywriter.Write(dataLength + cbFormat + 4); //File size - 8
                    WriteString(memStream, "WAVE");
                    WriteString(memStream, "fmt ");
                    binarywriter.Write(cbFormat);

                    //WAVEFORMATEX
                    binarywriter.Write(format.wFormatTag);
                    binarywriter.Write(format.nChannels);
                    binarywriter.Write(format.nSamplesPerSec);
                    binarywriter.Write(format.nAvgBytesPerSec);
                    binarywriter.Write(format.nBlockAlign);
                    binarywriter.Write(format.wBitsPerSample);
                    binarywriter.Write(format.cbSize);

                    //data header
                    WriteString(memStream, "data");
                    binarywriter.Write(dataLength);
                    memStream.WriteTo(stream);
                }
            }
        }

        /// <summary>
        /// Writes the string.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="s">The s.</param>
        static void WriteString(Stream stream, string s)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(s);
            stream.Write(bytes, 0, bytes.Length);
        }
        /// <summary>
        /// Records the audio.
        /// </summary>
        public void RecordAudio()
        {
            int recordingLength = (int)5 * 2 * 16000;
            byte[] buffer = new byte[1024];

            using (FileStream _fileStream = new FileStream("d:\\kinectAudio.wav", FileMode.Create))
            {
                WriteWavHeader(_fileStream, recordingLength);

                //Start capturing audio                               
                using (Stream audioStream = this.sensor.AudioSource.Start())
                {
                    int count, totalCount = 0;
                    while ((count = audioStream.Read(buffer, 0, buffer.Length)) > 0 && totalCount < recordingLength)
                    {
                        _fileStream.Write(buffer, 0, count);
                        totalCount += count;
                        this.ViewModel.SoundSourceAngle = this.sensor.AudioSource.SoundSourceAngle.ToString();
                        this.ViewModel.SoundConfidenceLevel = this.sensor.AudioSource.SoundSourceAngleConfidence.ToString();
                        this.ViewModel.SoundBeamAngle = this.sensor.AudioSource.BeamAngle.ToString();

                    }
                }
            }

        }

        /// <summary>
        /// Handles the BeamAngleChanged event of the AudioSource control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Microsoft.Kinect.BeamAngleChangedEventArgs"/> instance containing the event data.</param>
        void AudioSource_BeamAngleChanged(object sender, BeamAngleChangedEventArgs e)
        {
            this.ViewModel.SoundBeamAngle = e.Angle.ToString();
        }

        /// <summary>
        /// Handles the SoundSourceAngleChanged event of the AudioSource control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Microsoft.Kinect.SoundSourceAngleChangedEventArgs"/> instance containing the event data.</param>
        void AudioSource_SoundSourceAngleChanged(object sender, SoundSourceAngleChangedEventArgs e)
        {
            this.ViewModel.SoundSourceAngle = e.Angle.ToString();
            this.ViewModel.SoundConfidenceLevel = e.ConfidenceLevel.ToString();
        }

    }
}