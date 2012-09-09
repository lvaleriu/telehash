using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Threading;

namespace Telehash
{
    /// <summary>
    /// A telehash endpoint. Only one instance will exist per application. It is identified by its public IP:PORT and placed
    /// within the DHT by the SHA1 has of its IP:PORT. Switches communicate by sending UDP packets containing JSON
    /// </summary>
    public sealed class Switch : IDisposable
    {

        #region Constants

        private const int MAXIMUM_MESSAGE_SIZE_IN_BYTES = 1400;

        #endregion Constants

        #region Private Members

        /// <summary>
        /// Self instance to implement singleton pattern, volatile to ensure assignment occurs before use
        /// </summary>
        private static volatile Switch _Instance;

        /// <summary>
        /// Used for thread locking access to singleton instance of Switch
        /// </summary>
        private static readonly object _SyncRoot = new object();

        /// <summary>
        /// The GUID of the current switch; will be an SHA1 hash of its public IP:PORT
        /// </summary>
        private readonly string _GUID;

        /// <summary>
        /// The client used only for listening; only one listener per switch
        /// </summary>
        private volatile UdpClient _Listener;

        private volatile int _Port = 0;

        #endregion Private Members

        #region Public Members

        /// <summary>
        /// Event indicating that a Telex has been received; contains the respective telex object
        /// </summary>
        public event TelexReceivedDelegate TelexReceived;

        #endregion Public Members

        #region Constructors

        /// <summary>
        /// Protected constructor to implement singleton pattern
        /// </summary>
        protected Switch()
        {
            _GUID = GenerateGuid();
        }

        #endregion Constructors

        #region Static Methods

        /// <summary>
        /// Represents the current static instance of a Switch. If the instance is null uses double-check
        /// locking to create a static instance
        /// </summary>
        public static Switch Current
        {
            get
            {
                if (_Instance == null)
                {
                    lock (_SyncRoot)
                    {
                        if (_Instance == null)
                        {
                            _Instance = new Switch();
                        }
                    }
                }

                return _Instance;
            }
        }

        #endregion Static Methods

        #region Private Methods

        /// <summary>
        /// The Switches GUID should be an SHA1 hash of its public IP:PORT. To get this try sending a message to a known
        /// peer and observing the "_to" value in the response. For purposes of simplicity let the known peer be
        /// "telehash.org:42424"
        /// </summary>
        /// <returns>A string representation of the SHA1 hash of this instances public IP:PORT</returns>
        private string GenerateGuid()
        {
            //throw new NotImplementedException();
            return string.Empty;
        }

        /// <summary>
        /// Get the IP Address of the local machine for purposes of listening
        /// </summary>
        /// <returns>The local IP Address</returns>
        private IPAddress GetLocalIpAddress()
        {
            IPAddress LocalIpAddress = null;

            #if (SIM)
            LocalIPAddress = IPAddress.Parse("127.0.0.1");
            #else
            string HostName = Dns.GetHostName() + ".local";
            IPHostEntry HostEntry = Dns.GetHostEntry(HostName);
            LocalIpAddress = HostEntry.AddressList[0];
            #endif

            return LocalIpAddress;
        }

        /// <summary>
        /// Handles a callback from the listener socket and passes it along to any subscribers
        /// </summary>
        /// <param name="result">Callback result</param>
        private void ReceiveCompletedCallback(IAsyncResult result)
        {
            //Try-finally block will ensure that listener is "rebooted" no matter what happens
            try
            {
                IPEndPoint Endpoint = new IPEndPoint(IPAddress.Any, _Port);
                byte[] Result = _Listener.EndReceive(result, ref Endpoint);
                string ResultAsString = Encoding.UTF8.GetString(Result, 0, Result.Length);
                ResultAsString = ResultAsString.Trim('\0');

                if (TelexReceived != null)
                {
                    Telex Message = Telex.ParseReceivedMessage(ResultAsString);
                    TelexReceivedEventArgs EventArgs = new TelexReceivedEventArgs(Message);
                    TelexReceived(EventArgs);
                }
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
            finally
            {
                _Listener.BeginReceive(ReceiveCompletedCallback, null);
            }
        }

        #endregion Private Methods

        #region Public Methods

        /// <summary>
        /// Encodes the given string into UTF8 and then sends to the destination in UDP format Throws an exception if
        /// the message is to large
        /// </summary>
        /// <param name="data">The data to be sent on to the next node</param>
        /// <param name="endpoint">The endpoint of the telex</param>
        public void SendTelex(string data, IPEndPoint endpoint)
        {
            byte[] Data = Encoding.UTF8.GetBytes(data);

            if (Data.Length > MAXIMUM_MESSAGE_SIZE_IN_BYTES)
            {
                throw new ProtocolViolationException("Message to large");
            }

            Socket SendingSocket = new Socket(
                AddressFamily.InterNetwork, 
                SocketType.Dgram, 
                ProtocolType.Udp);

            try
            {
                SendingSocket.SendTo(Data, endpoint);
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }

        }

        /// <summary>
        /// An attempt to start listening on the specified port and an indication if this fails
        /// </summary>
        /// <param name="port">The port to listen on</param>
        /// <returns>False if the port was opened in this call for listening, true otherwise</returns>
        public bool StartListening(int port)
        {
            bool WasAlreadyStarted = true;

            if (_Listener == null)
            {
                lock(_SyncRoot)
                {
                    if (_Listener == null)
                    {
                        UdpClient TempListener = new UdpClient(port);
                        TempListener.BeginReceive(ReceiveCompletedCallback, null);
                        _Listener = TempListener;
                        _Port = port;

                        WasAlreadyStarted = false;
                    }
                }
            }

            return WasAlreadyStarted;
        }

        /// <summary>
        /// Stop the switch from listening and detach all current listeners
        /// </summary>
        public void StopListening()
        {
            _Listener.Close();
            _Listener = null;
            _Port = 0;

            //By setting the event to null all of the listeners drop off
            TelexReceived = null;
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            StopListening();
        }

        #endregion IDisposable
    }
}
