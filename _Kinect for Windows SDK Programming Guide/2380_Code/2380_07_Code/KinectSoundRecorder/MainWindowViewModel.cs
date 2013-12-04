using System.ComponentModel;
/// <summary>
/// The ViewModel for MainWindow
/// </summary>
public class MainWindowViewModel : INotifyPropertyChanged
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
    /// </summary>
    public MainWindowViewModel()
    {
        this.CanPlayback = false;
        this.CanStartRecording = true;
        this.CanStopRecording = false;
    }

    /// <summary>
    /// instance can start recording
    /// </summary>
    private bool canStartRecordingValue;

    /// <summary>
    /// Gets or sets a value indicating whether this instance can start recording.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance can start recording; otherwise, <c>false</c>.
    /// </value>
    public bool CanStartRecording
    {
        get { return canStartRecordingValue; }
        set
        {
            if (this.canStartRecordingValue != value)
            {
                canStartRecordingValue = value;
                this.OnNotifyPropertyChanged("CanStartRecording");
            }
        }
    }

    /// <summary>
    /// whether this instance can stop recording
    /// </summary>
    private bool canStopRecordingValue;

    /// <summary>
    /// Gets or sets a value indicating whether this instance can stop recording.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance can stop recording; otherwise, <c>false</c>.
    /// </value>
    public bool CanStopRecording
    {
        get { return canStopRecordingValue; }
        set
        {

            if (this.canStopRecordingValue != value)
            {
                canStopRecordingValue = value;
                this.OnNotifyPropertyChanged("CanStopRecording");
            }
        }
    }

    /// <summary>
    /// indicating whether this instance can playback
    /// </summary>
    private bool canPlaybackValue;

    /// <summary>
    /// Gets or sets a value indicating whether this instance can playback.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance can playback; otherwise, <c>false</c>.
    /// </value>
    public bool CanPlayback
    {
        get { return canPlaybackValue; }
        set
        {
            if (this.canPlaybackValue != value)
            {
                canPlaybackValue = value;
                this.OnNotifyPropertyChanged("CanPlayback");
            }
        }
    }


    /// <summary>
    /// check if noise suppressio is enabled or not
    /// </summary>
    private bool isNoiseSuppression;

    /// <summary>
    /// Gets or sets a value indicating whether this instance is noise suppression.
    /// </summary>
    /// <value>
    /// <c>true</c> if this instance is noise suppression; otherwise, <c>false</c>.
    /// </value>
    public bool IsNoiseSuppression
    {
        get
        {
            return isNoiseSuppression;
        }
        set 
        {
            if (this.isNoiseSuppression != value)
            {
                isNoiseSuppression = value;
                this.OnNotifyPropertyChanged("IsNoiseSuppression");
            }
        }
    }

    /// <summary>
    /// Check if echo cancelation is enabled or not.
    /// </summary>
    private bool isEchoCancelation;

    /// <summary>
    /// Gets or sets a value indicating whether this instance is echo cancelation.
    /// </summary>
    /// <value>
    /// <c>true</c> if this instance is echo cancelation; otherwise, <c>false</c>.
    /// </value>
    public bool IsEchoCancelation
    {
        get
        {
            return isEchoCancelation;
        }
        set
        {
            if (this.isEchoCancelation != value)
            {
                isEchoCancelation = value;
                this.OnNotifyPropertyChanged("IsEchoCancelation");
            }
        }
    }

    /// <summary>
    /// Check if gain control is enabled or not
    /// </summary>
    private bool isGainControl;

    /// <summary>
    /// Gets or sets a value indicating whether this instance is gain control.
    /// </summary>
    /// <value>
    /// <c>true</c> if this instance is gain control; otherwise, <c>false</c>.
    /// </value>
    public bool IsGainControl
    {
        get
        {
            return isGainControl;
        }
        set
        {
            if (this.isGainControl != value)
            {
                isGainControl = value;
                this.OnNotifyPropertyChanged("IsGainControl");
            }
        }
    }

    /// <summary>
    /// sound source angle.
    /// </summary>
    private string soundSourceAngle;

    /// <summary>
    /// Gets or sets the sound source angle.
    /// </summary>
    /// <value>The sound source angle.</value>
    public string SoundSourceAngle
    {
        get { return soundSourceAngle; }
        set
        {
            if (this.soundSourceAngle != value)
            {
                soundSourceAngle = value;
                this.OnNotifyPropertyChanged("SoundSourceAngle");
            }
        }
    }
    /// <summary>
    /// sound confidence level.
    /// </summary>
    private string soundConfidenceLevel;

    /// <summary>
    /// Gets or sets the sound confidence level.
    /// </summary>
    /// <value>The sound confidence level.</value>
    public string SoundConfidenceLevel
    {
        get { return soundConfidenceLevel; }
        set
        {
            if (this.soundConfidenceLevel != value)
            {
                this.soundConfidenceLevel = value;
                this.OnNotifyPropertyChanged("SoundConfidenceLevel");
            }
        }
    }


    /// <summary>
    /// Sound Beam Angle
    /// </summary>
    private string soundBeamAngle;

    /// <summary>
    /// Gets or sets the sound beam angle.
    /// </summary>
    /// <value>The sound beam angle.</value>
    public string SoundBeamAngle
    {
        get { return soundBeamAngle; }
        set
        {
            if (this.soundBeamAngle != value)
            {
                this.soundBeamAngle = value;
                this.OnNotifyPropertyChanged("SoundBeamAngle");
            }
        }
    }

    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Raises the notify property changed.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    private void OnNotifyPropertyChanged(string propertyName)
    {
        if (this.PropertyChanged != null)
        {
            this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    /// Starts the recording.
    /// </summary>
    public void StartRecording()
    {
        this.CanStartRecording = false;
        this.CanStopRecording = true;
        this.CanPlayback = false;
    }

    /// <summary>
    /// Stops the recording.
    /// </summary>
    public void StopRecording()
    {
        this.CanStartRecording = true;
        this.CanStopRecording = false;
        this.CanPlayback = true;
    }

    /// <summary>
    /// Starts the playback.
    /// </summary>
    public void StartPlayback()
    {
        this.CanStartRecording = false;
        this.CanStopRecording = false;
        this.CanPlayback = false;
    }

    /// <summary>
    /// Starts the playback.
    /// </summary>
    public void RecordingCompleted()
    {
        this.CanStartRecording = true;
        this.CanStopRecording = false;
        this.CanPlayback = true;
    }
}