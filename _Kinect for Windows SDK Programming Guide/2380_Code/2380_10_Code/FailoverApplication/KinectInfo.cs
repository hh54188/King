
namespace FailoverApplication
{
    using System;
    using System.ComponentModel;

    /// <summary>
    /// Kinect Info
    /// </summary>
    public class KinectInfo : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>
        /// The ID.
        /// </value>
        public Guid ID { get; set; }

        /// <summary>
        /// Gets or sets the device ID.
        /// </summary>
        /// <value>
        /// The device ID.
        /// </value>
        public string DeviceID { get; set; }

        /// <summary>
        /// Gets or sets the started.
        /// </summary>
        /// <value>
        /// The started.
        /// </value>
        public string Started { get; set; }

        /// <summary>
        /// Gets or sets the connection ID.
        /// </summary>
        /// <value>
        /// The connection ID.
        /// </value>
        public string ConnectionID { get; set; }

        /// <summary>
        /// Occurs when [property changed].
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Called when [property change].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        public void OnPropertyChange(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
