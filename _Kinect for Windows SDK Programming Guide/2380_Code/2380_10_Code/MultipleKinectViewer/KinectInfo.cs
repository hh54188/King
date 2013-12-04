

namespace MultipleKinectViewer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.ComponentModel;
    using System.Windows.Media.Imaging;

    /// <summary>
    /// Kinect Info
    /// </summary>
    public class KinectInfo : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets or sets the device count.
        /// </summary>
        /// <value>
        /// The device count.
        /// </value>
        public string deviceCount { get; set; }

        /// <summary>
        /// Gets or sets the device ID.
        /// </summary>
        /// <value>
        /// The device ID.
        /// </value>
        public string DeviceID { get; set; }

        /// <summary>
        /// The status
        /// </summary>
        private string status;

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public string Status
        {
            get
            {
                return this.status;
            }
            set
            {
                this.status = value;
                this.OnPropertyChange("Status");
            }
        }

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
