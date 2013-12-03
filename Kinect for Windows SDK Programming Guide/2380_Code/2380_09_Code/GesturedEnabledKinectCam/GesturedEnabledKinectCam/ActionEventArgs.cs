using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GesturedEnabledKinectCam
{
    public class ActionEventArgs : EventArgs
    {
        public ActionStatus Status { get; internal set; }
        public ActionEventArgs(ActionStatus status)
        {
            this.Status = status;
        }

    }
}
