// ***********************************************************************
// Assembly         : KinectStatusNotifier
// Author           : Abhijit Jana
// Chapter          : Chapter 03 - Kinect Status Change Notification Demo 
// ***********************************************************************
namespace KinectStatusNotifier
{
    using System.Drawing;
    using System.Windows.Forms;
    using Microsoft.Kinect;

    /// <summary>
    /// Status Notifier class
    /// </summary>
    public class StatusNotifier
    {
        /// <summary>
        /// The Notify Icon
        /// </summary>
        private NotifyIcon kinectNotifier = new NotifyIcon();

        /// <summary>
        /// Type of status
        /// </summary>
        private StatusType statusTypeValue;

        /// <summary>
        /// Kinect sensor collection
        /// </summary>
        private KinectSensorCollection sensorsValue;

        /// <summary>
        ///  Enable or disable the auto notification
        /// </summary>
        private bool autoNotificationValue;

        /// <summary>
        /// Title of the notification message
        /// </summary>
        private string notifierTitleValue;

        /// <summary>
        /// Notification message content
        /// </summary>
        private string notifierMessageValue;

        /// <summary>
        /// Sensor Status
        /// </summary>
        private KinectStatus sensorStatusValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="StatusNotifier" /> class.
        /// </summary>
        public StatusNotifier()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StatusNotifier" /> class.
        /// </summary>
        /// <param name="statusTitle">The status title.</param>
        /// <param name="statusMessage">The status message.</param>
        /// <param name="type">The type.</param>
        public StatusNotifier(string statusTitle, string statusMessage, StatusType type)
        {
          this.NotifierTitle = statusTitle;
          this.NotifierMessage = statusMessage;
          this.StatusType = type;
        }
        
        /// <summary>
        /// Gets or sets the sensors.
        /// </summary>
        /// <value>The sensors.</value>
        public KinectSensorCollection Sensors
        {
            get 
            {
                return this.sensorsValue;
            }

            set
            {
                this.sensorsValue = value;
                this.AutoNotification = true;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [auto notification].
        /// </summary>
        /// <value><c>true</c> if [auto notification]; otherwise, <c>false</c>.</value>
        public bool AutoNotification
        {
            get
            {
                return this.autoNotificationValue;
            }

            set
            {
                this.autoNotificationValue = value;
                if (value)
                {
                    this.Sensors.StatusChanged += this.Sensors_StatusChanged;
                }
                else
                {
                    this.sensorsValue.StatusChanged -= this.Sensors_StatusChanged;
                }
            }
        }

        /// <summary>
        /// Gets or sets the notifier title.
        /// </summary>
        /// <value>The notifier title.</value>
        public string NotifierTitle
        {
            get
            {
                return this.notifierTitleValue;
            }

            set
            {
                this.notifierTitleValue = value;
            }
        }

        /// <summary>
        /// Gets or sets the notifier message.
        /// </summary>
        /// <value>The notifier message.</value>
        public string NotifierMessage
        {
            get
            {
                return this.notifierMessageValue;
            }

            set
            {
                this.notifierMessageValue = value;
            }
        }

        /// <summary>
        /// Gets or sets the type of the status.
        /// </summary>
        /// <value>The type of the status.</value>
        public StatusType StatusType
        {
            get
            {
                return this.statusTypeValue;
            }

            set
            {
                this.statusTypeValue = value;
            }
        }

        /// <summary>
        /// Gets or sets the sensor status.
        /// </summary>
        /// <value>The sensor status.</value>
        private KinectStatus SensorStatus
        {
            get
            {
                return this.sensorStatusValue;
            }

            set
            {
                if (this.sensorStatusValue != value)
                {
                    this.sensorStatusValue = value;
                }
            }
        }

        /// <summary>
        /// Notifies the status.
        /// </summary>
        public void NotifyStatus()
        {      
            this.kinectNotifier.Icon = new Icon(this.GetIcon());
            this.kinectNotifier.Text = string.Format("Device Status : {0}", this.SensorStatus.ToString());
            this.kinectNotifier.Visible = true;
            this.kinectNotifier.ShowBalloonTip(3000, this.NotifierTitle, this.NotifierMessage, this.StatusType == StatusType.Information ? ToolTipIcon.Info : ToolTipIcon.Warning);
        }

        /// <summary>
        /// Gets the icon.
        /// </summary>
        /// <returns>The icon stream</returns>
        private System.IO.Stream GetIcon()
        {
            System.Reflection.Assembly assembly;
            assembly = System.Reflection.Assembly.GetExecutingAssembly();
            return assembly.GetManifestResourceStream("KinectStatusNotifier.kinect.ico");
        }

        /// <summary>
        /// Handles the StatusChanged event of the sensors control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="StatusChangedEventArgs" /> instance containing the event data.</param>
        protected void Sensors_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            this.SensorStatus = e.Status;
            this.NotifierTitle = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            this.NotifierMessage = string.Format("{0}\n{1}", this.SensorStatus.ToString(), e.Sensor.DeviceConnectionId);
            this.StatusType = StatusType.Information;
            this.NotifyStatus();
        }
    }
}