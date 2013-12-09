using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Microsoft.Kinect;

namespace KinectCam
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private bool isColorStreamEnabledValue;
        private int sensorAngleValue;
        private bool canStartValue;
        private bool canStopValue;
        private bool isFrameRateEnabledValue;
        private bool isFrameNumberEnabledValue;

        private bool isGrayScaleEnabledValue;
        private int frameRateValue;
        private int frameNumberValue;

        private bool isInvertColorEffectEnabledValue;
        private ObservableCollection<ColorImageFormat> colorImageFormatvalue;
        private bool isEffectsEnabledValue;
        private ColorImageFormat currentImageFormatValue;
        private ObservableCollection<Effects> colorEffectsValue;
        private Effects currentColorEffectValue;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsFrameRateEnabled
        {
            get
            {
                return this.isFrameRateEnabledValue;
            }

            set
            {
                if (this.isFrameRateEnabledValue != value)
                {
                    this.isFrameRateEnabledValue = value;
                    this.OnNotifyPropertyChange("IsFrameRateEnabled");
                }
            }
        }

        public int FrameRate
        {
            get
            {
                return this.frameRateValue;
            }

            set
            {
                if (this.frameRateValue != value)
                {
                    this.frameRateValue = value;
                    this.OnNotifyPropertyChange("FrameRate");
                }
            }
        }

        public bool IsFrameNumberEnabled
        {
            get
            {
                return this.isFrameNumberEnabledValue;
            }

            set
            {
                if (this.isFrameNumberEnabledValue != value)
                {
                    this.isFrameNumberEnabledValue = value;
                    this.OnNotifyPropertyChange("IsFrameNumberEnabled");
                }
            }
        }

        public int FrameNumber
        {
            get
            {
                return this.frameNumberValue;
            }

            set
            {
                if (this.frameNumberValue != value)
                {
                    this.frameNumberValue = value;
                    this.OnNotifyPropertyChange("FrameNumber");
                }
            }
        }

        public bool IsGrayScaleEnabled
        {
            get
            {
                return this.isGrayScaleEnabledValue;
            }

            set
            {
                if (this.isGrayScaleEnabledValue != value)
                {
                    this.isGrayScaleEnabledValue = value;
                    this.OnNotifyPropertyChange("IsGrayScaleEnabled");
                }
            }
        }

        public bool IsInvertColorEffectsEnabled
        {
            get
            {
                return this.isInvertColorEffectEnabledValue;
            }

            set
            {
                if (this.isInvertColorEffectEnabledValue != value)
                {
                    this.isInvertColorEffectEnabledValue = value;
                    this.OnNotifyPropertyChange("IsInvertColorEffectsEnabled");
                }
            }
        }

        public Effects CurrentColorEffect
        {
            get
            {
                return this.currentColorEffectValue;
            }

            set
            {
                if (this.currentColorEffectValue != value)
                {
                    this.currentColorEffectValue = value;
                    this.OnNotifyPropertyChange("CurrentColorEffect");
                }
            }
        }

        public ObservableCollection<Effects> ColorEffects
        {
            get
            {
                this.colorEffectsValue = new ObservableCollection<Effects>();
                foreach (Effects colorEffects in Enum.GetValues(typeof(Effects)))
                {
                    this.colorEffectsValue.Add(colorEffects);
                }

                return this.colorEffectsValue;
            }
        }

        public ObservableCollection<ColorImageFormat> ColorImageFormats
        {
            get
            {
                this.colorImageFormatvalue = new ObservableCollection<ColorImageFormat>();
                foreach (ColorImageFormat colorImageFormat in Enum.GetValues(typeof(ColorImageFormat)))
                {
                    this.colorImageFormatvalue.Add(colorImageFormat);
                }

                return this.colorImageFormatvalue;
            }
        }

        public ColorImageFormat CurrentImageFormat
        {
            get
            {
                return this.currentImageFormatValue;
            }

            set
            {
                if (this.currentImageFormatValue != value)
                {
                    this.currentImageFormatValue = value;
                    this.OnNotifyPropertyChange("CurrentImageFormat");
                }
            }
        }

        public bool IsColorStreamEnabled
        {
            get
            {
                return this.isColorStreamEnabledValue;
            }

            set
            {
                if (this.isColorStreamEnabledValue != value)
                {
                    this.isColorStreamEnabledValue = value;
                    this.OnNotifyPropertyChange("IsColorStreamEnabled");
                    this.OnNotifyPropertyChange("FrameNumber");
                }
            }
        }

        public int SensorAngle
        {
            get
            {
                return this.sensorAngleValue;
            }

            set
            {
                if (this.sensorAngleValue != value)
                {
                    this.sensorAngleValue = value;
                    this.OnNotifyPropertyChange("SensorAngle");
                }
            }
        }

        public bool CanStart
        {
            get
            {
                return this.canStartValue;
            }

            set
            {
                if (this.canStartValue != value)
                {
                    this.canStartValue = value;
                    this.OnNotifyPropertyChange("CanStart");
                }
            }
        }

        public bool CanStop
        {
            get
            {
                return this.canStopValue;
            }

            set
            {
                if (this.canStopValue != value)
                {
                    this.canStopValue = value;
                    this.OnNotifyPropertyChange("CanStop");
                }
            }
        }

        public bool IsEffectsEnabled
        {
            get
            {
                return this.isEffectsEnabledValue;
            }

            set
            {
                if (this.isEffectsEnabledValue != value)
                {
                    this.isEffectsEnabledValue = value;
                    this.OnNotifyPropertyChange("IsEffectsEnabled");
                }
            }
        }

        public void OnNotifyPropertyChange(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
