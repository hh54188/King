using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Microsoft.Kinect;

namespace KinectStatusNotifier
{
    public class StatusNotifier
    {   
        private NotifyIcon kinectNotifier = new NotifyIcon();
        private KinectSensorCollection sensorsValue;
        private bool autoNotificationValue;

        private StatusType statusTypeValue;
        private KinectStatus sensorStatusValue;
        private string notifierTitleValue;
        private string notifierMessageValue;
        public KinectStatus SensorStatus
        {
            get
            {
                return this.sensorStatusValue;
            }
            set
            {
                this.sensorStatusValue = value;
            }
        }
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
        public KinectSensorCollection Sensors
        {
            get
            {
                return this.sensorsValue;
            }
            set
            {
                this.sensorsValue = value;
                
            }
            
        }
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
                    this.Sensors.StatusChanged += Sensors_StatusChanged;
                }
                else
                {
                    this.Sensors.StatusChanged -= Sensors_StatusChanged;
                }
            }
        }

        void Sensors_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            this.SensorStatus = e.Status;
            this.NotifierTitle = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            this.NotifierMessage = string.Format("{0}\n{1}", 
                                    this.SensorStatus.ToString(), 
                                    e.Sensor.DeviceConnectionId);
            this.StatusType = StatusType.Information;
            this.NotifyStatus();
        }

        private void NotifyStatus()
        {
            this.kinectNotifier.Icon = new Icon(this.GetIcon());
            this.kinectNotifier.Text = string.Format("Device Status:{0}", this.SensorStatus.ToString());
            this.kinectNotifier.Visible = true;

            this.kinectNotifier.ShowBalloonTip(3000, 
                                            this.NotifierTitle, 
                                            this.NotifierMessage, 
                                            this.StatusType == StatusType.Information ? ToolTipIcon.Info : ToolTipIcon.Warning);
        }
        private System.IO.Stream GetIcon()
        {
            System.Reflection.Assembly assembly;
            assembly = System.Reflection.Assembly.GetExecutingAssembly();
            return assembly.GetManifestResourceStream("KinectStatusNotifier.kinect.ico");
        }
    }
}
