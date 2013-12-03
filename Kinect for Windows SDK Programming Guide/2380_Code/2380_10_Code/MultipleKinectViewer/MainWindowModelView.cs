
namespace MultipleKinectViewer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.ComponentModel;

    /// <summary>
    /// Main Window Model View
    /// </summary>
    public class MainWindowModelView : INotifyPropertyChanged
    {
        /// <summary>
        /// The number of device
        /// </summary>
        private int numberOfDevice;

        /// <summary>
        /// Gets or sets the numberof device.
        /// </summary>
        /// <value>
        /// The numberof device.
        /// </value>
        public int NumberofDevice
        {
            get
            {
                return numberOfDevice;
            }
            set
            {
                numberOfDevice = value;
                this.OnPropertyChange("NumberOfDevice");
            }
        }

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
