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
using Microsoft.Speech.Recognition;
using System.IO;
using Microsoft.Speech.AudioFormat;
using System.Threading;

namespace DrawWhatIWant
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Thread for Audio Start
        /// </summary>
        private Thread thread;

        /// <summary>
        /// Gets or sets the view model.
        /// </summary>
        /// <value>The view model.</value>
        public MainWindowViewModel ViewModel { get; set; }

        const string RecognizerId = "SR_MS_en-US_Kinect_11.0";

        /// <summary>
        /// Kinect Sensor
        /// </summary>
        KinectSensor sensor;
        KinectAudioSource source;
        bool keepRunning = true;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(MainWindow_Loaded);
            Closed += new EventHandler(MainWindow_Closed);
            this.ViewModel = new MainWindowViewModel();
            this.DataContext = this.ViewModel;
        }

        void MainWindow_Closed(object sender, EventArgs e)
        {
            // Abort the thread
            if (thread != null)
            {
                thread.Abort();
            }

            // Stop the sensor if it's running
            if (this.sensor != null && this.sensor.IsRunning)
            {
                this.sensor.AudioSource.Stop();
                this.sensor.Stop();
            }
        }


        void MainWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            if (sensor != null)
            {
                sensor.Stop();
            }
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (KinectSensor.KinectSensors.Count > 0)
            {
                sensor = KinectSensor.KinectSensors[0];
                sensor.Start();
                this.Start();
            }
            else
            {
                MessageBox.Show("No Device Connected !");
            }
        }

        public void Start()
        {
            if (sensor == null)
            {
                MessageBox.Show("No Device Found !");

                return;
            }
            StartKinectRecognizer();
        }

        private void BuildGrammarforRecognizer(object recognizerInfo)
        {
            EnableKinectAudioSource();

            var grammarBuilder = new GrammarBuilder { Culture = (recognizerInfo as RecognizerInfo).Culture };

            // first say Draw
            grammarBuilder.Append(new Choices("draw"));

            var colorObjects = new Choices();
            colorObjects.Add("red");
            colorObjects.Add("green");
            colorObjects.Add("blue");
            colorObjects.Add("yellow");
            colorObjects.Add("gray");


            // New Grammar builder for color
            grammarBuilder.Append(colorObjects);

            // Another Grammar Builder for object
            grammarBuilder.Append(new Choices("circle", "square", "triangle", "rectangle"));

            // Create Grammar from GrammarBuilder
            var grammar = new Grammar(grammarBuilder);

            // Creating another Grammar and load
            var newGrammarBuilder = new GrammarBuilder();
            newGrammarBuilder.Append("close the application");
            var grammarClose = new Grammar(newGrammarBuilder);

            int SamplesPerSecond = 16000;
            int bitsPerSample = 16;
            int channels = 1;
            int averageBytesPerSecond = 32000;
            int blockAlign = 2;

            using (var speechRecognizer = new SpeechRecognitionEngine((recognizerInfo as RecognizerInfo).Id))
            {
                speechRecognizer.LoadGrammar(grammar);
                speechRecognizer.LoadGrammar(grammarClose);

                speechRecognizer.SpeechRecognized += SreSpeechRecognized;
                speechRecognizer.SpeechHypothesized += SreSpeechHypothesized;
                speechRecognizer.SpeechRecognitionRejected += SreSpeechRecognitionRejected;

                using (Stream s = source.Start())
                {
                    speechRecognizer.SetInputToAudioStream(
                        s, new SpeechAudioFormatInfo(EncodingFormat.Pcm, SamplesPerSecond, bitsPerSample, channels, averageBytesPerSecond, blockAlign, null));


                    while (keepRunning)
                    {
                        RecognitionResult result = speechRecognizer.Recognize(new TimeSpan(0, 0, 5));
                    }

                    speechRecognizer.RecognizeAsyncStop();
                }
            }
        }

        /// <summary>
        /// Enables the kinect audio source.
        /// </summary>
        private void EnableKinectAudioSource()
        {
            source = sensor.AudioSource;
            source.BeamAngleChanged += new EventHandler<BeamAngleChangedEventArgs>(source_BeamAngleChanged);
            source.SoundSourceAngleChanged += new EventHandler<SoundSourceAngleChangedEventArgs>(source_SoundSourceAngleChanged);
            source.AutomaticGainControlEnabled = false;
            source.NoiseSuppression = true;
        }

        /// <summary>
        /// Handles the Closing event of the MainWindow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs" /> instance containing the event data.</param>
        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            keepRunning = false;
        }


        /// <summary>
        /// Handles the SoundSourceAngleChanged event of the source control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SoundSourceAngleChangedEventArgs" /> instance containing the event data.</param>
        void source_SoundSourceAngleChanged(object sender, SoundSourceAngleChangedEventArgs e)
        {
            ViewModel.SoundSourceAngle = e.Angle.ToString();
        }

        /// <summary>
        /// Handles the BeamAngleChanged event of the source control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="BeamAngleChangedEventArgs" /> instance containing the event data.</param>
        void source_BeamAngleChanged(object sender, BeamAngleChangedEventArgs e)
        {
            ViewModel.SoundBeamAngle = e.Angle.ToString();
        }

        /// <summary>
        /// Starts the kinect recognizer.
        /// </summary>
        private void StartKinectRecognizer()
        {
            RecognizerInfo recognizerInfo = SpeechRecognitionEngine.InstalledRecognizers().Where(r => r.Id == RecognizerId).FirstOrDefault();

            if (recognizerInfo == null)
            {
                MessageBox.Show("Could not find Kinect speech recognizer");
                return;
            }
            Thread.Sleep(1000);
            Thread newThread = new Thread(new ParameterizedThreadStart(BuildGrammarforRecognizer));
            newThread.Start(recognizerInfo);
        }

        /// <summary>
        /// Sres the speech recognition rejected.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="SpeechRecognitionRejectedEventArgs" /> instance containing the event data.</param>
        private void SreSpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            if (e.Result != null)
            {
            }
        }

        private void SreSpeechHypothesized(object sender, SpeechHypothesizedEventArgs e)
        {
            ViewModel.HypothesizedText = e.Result.Text;
        }

        private void SreSpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            ViewModel.SoundConfidenceLevel = e.Result.Confidence.ToString();
            float confidenceThreshold = 0.6f;
            if (e.Result.Confidence > confidenceThreshold)
            {
                Dispatcher.BeginInvoke(new Action<SpeechRecognizedEventArgs>(CommandsParser), e);
            }
        }


        private void CommandsParser(SpeechRecognizedEventArgs e)
        {
            var result = e.Result;
            Color objectColor;
            Shape drawObject;
            System.Collections.ObjectModel.ReadOnlyCollection<RecognizedWordUnit> words = e.Result.Words;
            DisplayWords(result);

            if (words[0].Text == "draw")
            {
                string colorObject = words[1].Text;
                switch (colorObject)
                {
                    case "red": objectColor = Colors.Red;
                        break;
                    case "green": objectColor = Colors.Green;
                        break;
                    case "blue": objectColor = Colors.Blue;
                        break;
                    case "yellow": objectColor = Colors.Yellow;
                        break;
                    case "gray": objectColor = Colors.Gray;
                        break;
                    default:
                        return;
                }

                var shapeString = words[2].Text;
                switch (shapeString)
                {
                    case "circle":
                        drawObject = new Ellipse();
                        drawObject.Width = 100;
                        drawObject.Height = 100;
                        break;
                    case "square":
                        drawObject = new Rectangle();
                        drawObject.Width = 100;
                        drawObject.Height = 100;
                        break;
                    case "rectangle":
                        drawObject = new Rectangle();
                        drawObject.Width = 100;
                        drawObject.Height = 60;
                        break;
                    case "triangle":
                        var polygon = new Polygon();
                        polygon.Points.Add(new Point(0, 0));
                        polygon.Points.Add(new Point(-169, 0));
                        polygon.Points.Add(new Point(60, -40));
                        drawObject = polygon;
                        break;
                    default:
                        return;
                }

                PlaceHolder.Children.Clear();
                drawObject.SetValue(Canvas.LeftProperty, 120.0);
                drawObject.SetValue(Canvas.TopProperty, 100.0);
                drawObject.Fill = new SolidColorBrush(objectColor);
                PlaceHolder.Children.Add(drawObject);
            }

            if (words[0].Text == "close" && words[1].Text == "the" && words[2].Text == "application")
            {
                this.Close();
            }
        }

        /// <summary>
        /// Displays the words.
        /// </summary>
        /// <param name="result">The result.</param>
        private void DisplayWords(RecognitionResult result)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var word in result.Words)
            {
                sb.Append(string.Format("[{0}]", word.Text));
            }
            ViewModel.Words = sb.ToString();
        }


    }


}
