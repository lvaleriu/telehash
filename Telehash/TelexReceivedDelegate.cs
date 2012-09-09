using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Telehash
{
    /// <summary>
    /// Delegate used to represent a Telex message subscriber
    /// </summary>
    /// <param name="eventArgs">Representation of the Telex received</param>
    public delegate void TelexReceivedDelegate(TelexReceivedEventArgs eventArgs);
}
