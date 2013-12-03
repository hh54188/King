

namespace KinectStatusNotifier
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Microsoft.Kinect;

    /// <summary>
    /// The Kinect 
    /// </summary>
    public class StatusNotifier
    {
        /// <summary>
        /// The Notify Icon
        /// </summary>
        NotifyIcon kinectNotifier = new NotifyIcon();

        /// <summary>
        /// The Dispatched Timer
        /// </summary>
        private Timer timer = new Timer();

        /// <summary>
        /// Kinect sensor collection
        /// </summary>
        private KinectSensorCollection sensors;

        /// <summary>
        /// Gets or set the 
        /// </summary>
        public KinectSensorCollection Sensors
        {
            get { return sensors; }
            set
            {
                this.sensors = value;
                this.sensors.StatusChanged += sensors_StatusChanged;
            }
        }

        void sensors_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            this.SensorStatus = e.Status;
            this.NotifierTitle = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            this.NotiferMessage = string.Format("{Device ID - {0} \n Device Status - {1}", e.Sensor.UniqueKinectId, e.Status.ToString());
            this.StatusType = StatusNotifier.StatusType.Information;
            this.NotifyStatus();

        }

        private KinectSensor sensor;

        public KinectSensor Sensor
        {
            get
            {
                return sensor;
            }
            set
            {
                if (this.sensor != value)
                {
                    this.sensor = value;

                    this.NotifierTitle = "Sensor Test";
                    this.NotiferMessage = this.sensor.Status.ToString();
                    this.StatusType = StatusNotifier.StatusType.Information;
                    this.NotifyStatus();
                }
            }
        }

        private KinectStatus sensorStatus;

        public KinectStatus SensorStatus
        {
            get { return sensorStatus; }
            set
            {
                if (this.sensorStatus != value)
                {
                    this.sensorStatus = value;
                }
            }
        }


        private string notifierTitle;

        public string NotifierTitle
        {
            get { return notifierTitle; }
            set { notifierTitle = value; }
        }

        private string notifierMessage;

        public string NotiferMessage
        {
            get { return notifierMessage; }
            set { notifierMessage = value; }
        }


        private StatusType statusType;

        public StatusType StatusType
        {
            get { return statusType; }
            set { statusType = value; }
        }

        public KinectStatusNotifier()
        {

        }

        public KinectStatusNotifier(string statusTitle, string statusMessage, StatusType type)
        {

            this.kinectNotifier.ShowBalloonTip(5000, statusTitle, statusMessage, type == StatusNotifier.StatusType.Information ? ToolTipIcon.Info : ToolTipIcon.Warning);
        }

        public void NotifyStatus()
        {

            this.kinectNotifier.Icon = new Icon(@"Kinect.ico");
            this.kinectNotifier.Text = "Kinect Status Notifier";
            this.kinectNotifier.Visible = true;
            this.kinectNotifier.ShowBalloonTip(3000, this.NotifierTitle, this.NotiferMessage, this.StatusType == StatusNotifier.StatusType.Information ? ToolTipIcon.Info : ToolTipIcon.Warning);
        }



    }

}
