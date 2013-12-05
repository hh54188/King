
using System.ComponentModel;
/* 
 * 需要使用继承，继承自INotifyPropertyChanged
 * 才能调用PropertyChanged方法
*/
public class MainWindowViewModel : INotifyPropertyChanged
{
    private string sensorStatusValue;
    private string connectionIDValue;
    private string deviceIdValue;
    private bool isColorStreamEnabledValue;
    private bool isDepthStreamEnabledValue;
    private bool isSkeletonStreamEnabledValue;
    private int sensorAngleValue;
    private bool canStartValue;
    private bool canStopValue;
    public event PropertyChangedEventHandler PropertyChanged;

    public void OnNotifyPropertyChange(string propertyName)
    {
        if (this.PropertyChanged != null)
        {   
            // 相当于Javascript中的 fn.call / fn.apply
            this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public string ConnectionID
    {
        get
        {
            return this.connectionIDValue;
        }
        set
        {
            if (this.connectionIDValue != value)
            {
                this.connectionIDValue = value;
                // 如果属性有改变，则通知视图更新
                this.OnNotifyPropertyChange("ConnectionID");
            }
        }
    }
    public string DeviceID
    {
        get
        {
            return this.deviceIdValue;
        }

        set
        {
            if (this.deviceIdValue != value)
            {
                this.deviceIdValue = value;
                this.OnNotifyPropertyChange("DeviceID");
            }
        }
    }
    public string SensorStatus
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
                this.OnNotifyPropertyChange("SensorStatus");
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
            }
        }
    }
    public bool IsDepthStreamEnabled
    {
        get
        {
            return this.isDepthStreamEnabledValue;
        }

        set
        {
            if (this.isDepthStreamEnabledValue != value)
            {
                this.isDepthStreamEnabledValue = value;
                this.OnNotifyPropertyChange("IsDepthStreamEnabled");
            }
        }
    }
    public bool IsSkeletonStreamEnabled
    {
        get
        {
            return this.isSkeletonStreamEnabledValue;
        }

        set
        {
            if (this.isSkeletonStreamEnabledValue != value)
            {
                this.isSkeletonStreamEnabledValue = value;
                this.OnNotifyPropertyChange("IsSkeletonStreamEnabled");
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
    
}
