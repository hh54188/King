using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace DepthCam
{
    class MainWindowViewModel :INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private int xValue;

        public int X
        {
            get { return xValue; }
            set {
                if (xValue != value)
                {
                    xValue = value;
                    this.OnNotifyPropertyChange("X");
                }
            }
        }

        private int yValue;

        public int Y
        {
            get { return yValue; }
            set
            {
                if (yValue != value)
                {
                    yValue = value;
                    this.OnNotifyPropertyChange("Y");
                }
            }
        }


        private int depthValue;

        public int Depth
        {
            get { return depthValue; }
            set
            {
                if (depthValue != value)
                {
                    depthValue = value;
                    this.OnNotifyPropertyChange("Depth");
                }
            }
        }


        private string distanceValue;

        public string Distance
        {
            get { return distanceValue; }
            set
            {
                if (distanceValue != value)
                {
                    distanceValue = value;
                    this.OnNotifyPropertyChange("Distance");
                }
            }
        }

        private int maxDepthValue;

        public int MaxDepth
        {
            get { return maxDepthValue; }
            set
            {
                if (maxDepthValue != value)
                {
                    maxDepthValue = value;
                    this.OnNotifyPropertyChange("MaxDepth");
                }
            }
        }

        private int minDepthValue;

        public int MinDepth
        {
            get { return minDepthValue; }
            set
            {
                if (minDepthValue != value)
                {
                    minDepthValue = value;
                    this.OnNotifyPropertyChange("MinDepth");
                }
            }
        }

        private int frameWidthValue;

        public int FrameWidth
        {
            get { return frameWidthValue; }
            set
            {
                if (frameWidthValue != value)
                {
                    frameWidthValue = value;
                    this.OnNotifyPropertyChange("FrameWidth");
                }
            }
        }

        private int frameHeightValue;

        public int FrameHeight
        {
            get { return frameHeightValue; }
            set
            {
                if (frameHeightValue != value)
                {
                    frameHeightValue = value;
                    this.OnNotifyPropertyChange("FrameHeight");
                }
            }
        }

        private int startRangeValue;

        public int StartRange
        {
            get { return startRangeValue; }
            set
            {
                if (startRangeValue != value)
                {
                    startRangeValue = value;
                    this.OnNotifyPropertyChange("StartRange");
                }
            }
        }

        private int endRangeValue;

        public int EndRange
        {
            get { return endRangeValue; }
            set
            {
                if (endRangeValue != value)
                {
                    endRangeValue = value;
                    this.OnNotifyPropertyChange("EndRange");
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
