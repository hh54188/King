using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;


namespace AdvancedSkeletonViewer
{
    /// <summary>
    /// Main window view model
    /// </summary>
    class MainWindowViewModel : INotifyPropertyChanged
    {

        /// <summary>
        /// Occurs when [property changed].
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Ons the property change.
        /// </summary>
        /// <param name="properyName">Name of the propery.</param>
        public void onPropertyChange(string properyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs(properyName));
            }
        }
    }
}
