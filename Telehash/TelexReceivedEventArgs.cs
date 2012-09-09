using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Telehash
{
    /// <summary>
    /// Container for TelexReceived event
    /// </summary>
    public class TelexReceivedEventArgs
    {
        /// <summary>
        /// The received telex
        /// </summary>
        public Telex TelexMessage { get; set; }

        public TelexReceivedEventArgs(Telex telex)
        {
            TelexMessage = telex;
        }
    }
}
