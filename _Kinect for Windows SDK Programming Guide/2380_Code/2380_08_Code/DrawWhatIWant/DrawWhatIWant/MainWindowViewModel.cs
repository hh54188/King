using System;
using System.ComponentModel;

public class MainWindowViewModel : INotifyPropertyChanged
{

    public event PropertyChangedEventHandler PropertyChanged;

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
    /// Words for the speech
    /// </summary>
    private string words;

    /// <summary>
    /// Gets or sets the words.
    /// </summary>
    /// <value>The words.</value>
    public string Words
    {
        get { return words; }
        set
        {
            if (this.words != value)
            {
                this.words = value;
                this.OnNotifyPropertyChanged("Words");
            }
        }
    }

    /// <summary>
    /// Hypothesized Text
    /// </summary>
    private string hypText;


    /// <summary>
    /// Gets or sets the hypothesized text.
    /// </summary>
    /// <value>The hypothesized text.</value>
    public string HypothesizedText
    {
        get { return hypText; }
        set
        {
            if (this.hypText != value)
            {
                this.hypText = value;
                this.OnNotifyPropertyChanged("HypothesizedText");
            }
        }
    }

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
}
