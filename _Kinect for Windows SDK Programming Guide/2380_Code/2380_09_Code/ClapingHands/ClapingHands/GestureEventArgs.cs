using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClapingHands
{
    public abstract class GestureEventArgs : EventArgs
    {
        public RecognitionResult Result { get; }
    }
}
