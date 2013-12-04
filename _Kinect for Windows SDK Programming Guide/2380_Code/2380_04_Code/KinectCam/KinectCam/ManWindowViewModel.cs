// ***********************************************************************
// <copyright file="MainWindowViewModel.cs" company="">
//     . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace KinectCam
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using Microsoft.Kinect;

    /// <summary>
    /// Class MainWindowViewModel
    /// </summary>
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Is Color Stream Enabled
        /// </summary>
        private bool isColorStreamEnabledValue;

        /// <summary>
        /// sensor elevation angle
        /// </summary>
        private int sensorAngleValue;

        /// <summary>
        /// Can Start the sensor
        /// </summary>
        private bool canStartValue;

        /// <summary>
        /// Can Stop the sensor
        /// </summary>
        private bool canStopValue;

        /// <summary>
        /// Frame rate is enabled
        /// </summary>
        private bool isFrameRateEnabledValue;

        /// <summary>
        /// frame number is enabled
        /// </summary>
        private bool isFrameNumberEnabledValue;

        /// <summary>
        /// Gray scale is enabled
        /// </summary>
        private bool isGrayScaleEnabledValue;

        /// <summary>
        /// frame rate
        /// </summary>
        private int frameRateValue;

        /// <summary>
        /// The frame number value
        /// </summary>
        private int frameNumberValue;

        /// <summary>
        /// Invert color 
        /// </summary>
        private bool isInvertColorEffectEnabledValue;

        /// <summary>
        /// Collection of Color image format
        /// </summary>
        private ObservableCollection<ColorImageFormat> colorImageFormatvalue;

        /// <summary>
        /// Is effects enabled.
        /// </summary>
        private bool isEffectsEnabledValue;

        /// <summary>
        /// current image format
        /// </summary>
        private ColorImageFormat currentImageFormatValue;

        /// <summary>
        /// Colloection of color effects
        /// </summary>
        private ObservableCollection<Effects> colorEffectsValue;

        /// <summary>
        /// current color effect
        /// </summary>
        private Effects currentColorEffectValue;

        /// <summary>
        /// Occurs when [property changed].
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is frame rate enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is frame rate enabled; otherwise, <c>false</c>.
        /// </value>
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

        /// <summary>
        /// Gets or sets the frame rate.
        /// </summary>
        /// <value>
        /// The frame rate.
        /// </value>
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

        /// <summary>
        /// Gets or sets a value indicating whether this instance is frame number enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is frame number enabled; otherwise, <c>false</c>.
        /// </value>
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

        /// <summary>
        /// Gets or sets the frame number.
        /// </summary>
        /// <value>
        /// The frame number.
        /// </value>
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

        /// <summary>
        /// Gets or sets a value indicating whether this instance is gray scale enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is gray scale enabled; otherwise, <c>false</c>.
        /// </value>
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

        /// <summary>
        /// Gets or sets a value indicating whether this instance is invert color effects enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is invert color effects enabled; otherwise, <c>false</c>.
        /// </value>
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

        /// <summary>
        /// Gets or sets the current color effect.
        /// </summary>
        /// <value>
        /// The current color effect.
        /// </value>
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

        /// <summary>
        /// Gets the color effects.
        /// </summary>
        /// <value>
        /// The color effects.
        /// </value>
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

        /// <summary>
        /// Gets the color image formats.
        /// </summary>
        /// <value>
        /// The color image formats.
        /// </value>
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

        /// <summary>
        /// Gets or sets the current image format.
        /// </summary>
        /// <value>
        /// The current image format.
        /// </value>
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

        /// <summary>
        /// Gets or sets a value indicating whether this instance is color stream enabled.
        /// </summary>
        /// <value><c>true</c> if this instance is color stream enabled; otherwise, <c>false</c>.</value>
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

        /// <summary>
        /// Gets or sets the sensor angle.
        /// </summary>
        /// <value>The sensor angle.</value>
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

        /// <summary>
        /// Gets or sets a value indicating whether this instance can start.
        /// </summary>
        /// <value><c>true</c> if this instance can start; otherwise, <c>false</c>.</value>
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

        /// <summary>
        /// Gets or sets a value indicating whether this instance can stop.
        /// </summary>
        /// <value><c>true</c> if this instance can stop; otherwise, <c>false</c>.</value>
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

        /// <summary>
        /// Gets or sets a value indicating whether this instance is effects enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is effects enabled; otherwise, <c>false</c>.
        /// </value>
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

        /// <summary>
        /// Called when [notify property change].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        public void OnNotifyPropertyChange(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
