using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;


namespace Telehash
{
    /// <summary>
    /// A representation of a telehash message, called a Telex. Contains the message payload in JSON format, and one or
    /// more Commands, Signals or Headers. Also contains the source or destination
    /// </summary>
    public class Telex
    {

        #region Constants

        /// <summary>
        /// Character prefixes for special JSON entries
        /// </summary>
        private const char SIGNAL_PREFIX = '+';
        private const char COMMAND_PREFIX = '.';
        private const char HEADER_PREFIX = '_';
        
        #endregion Constants

        #region Public Members

        /// <summary>
        /// The endpoint of the telex
        /// </summary>
        public IPEndPoint End { get; private set; }

        /// <summary>
        /// The payload message
        /// </summary>
        public JObject Payload { get; private set; }

        /// <summary>
        /// All of the telex commands
        /// </summary>
        public List<JProperty> Commands { get; set; }

        /// <summary>
        /// All of the telex signals
        /// </summary>
        public List<JProperty> Signals { get; set; }

        /// <summary>
        /// All of the telex headers
        /// </summary>
        public List<JProperty> Headers { get; set; }

        #endregion Public Members

        #region Constructors

        /// <summary>
        /// The constructor designed for creating a new telex for purposes of sending
        /// </summary>
        /// <param name="payload">The message to be sent</param>
        /// <param name="end">The destination</param>
        private Telex(
            JObject payload, 
            IPEndPoint end)
        {
            Payload = payload;
            End = end;
        }

        /// <summary>
        /// The constructor designed for creating a new telex representing a received message
        /// </summary>
        /// <param name="payload">The JSON message sent</param>
        private Telex(string payload)
        {
            JObject TempMessage = JObject.Parse(payload);
            Commands = GetSpecialJson(
                TempMessage, 
                COMMAND_PREFIX);
            Signals = GetSpecialJson(
                TempMessage, 
                SIGNAL_PREFIX);
            Headers = GetSpecialJson(
                TempMessage, 
                HEADER_PREFIX);
        }

        #endregion Constructors

        #region Static Methods

        /// <summary>
        /// Yields a telex object representing a received message
        /// </summary>
        /// <param name="message">The raw JSON string representation of the message</param>
        /// <returns>A telex representing the received message</returns>
        public static Telex ParseReceivedMessage(string message)
        {
            return new Telex(message);
        }

        /// <summary>
        /// Yields a Telex object representing a message to be sent
        /// </summary>
        /// <param name="payload">The message to be sent</param>
        /// <param name="ipAddress">The destination public ip</param>
        /// <param name="port">The destination port</param>
        /// <returns>A telex representing the message</returns>
        public static Telex CreateNewMessage(
            string payload, 
            string ipAddress, 
            int port
            )
        {
            JObject Payload = JObject.Parse(payload);
            IPEndPoint End = new IPEndPoint(IPAddress.Parse(ipAddress), port);

            return new Telex(Payload, End);
        }

        #endregion Static Methods

        #region Private Static Methods

        /// <summary>
        /// Gets all of the special Json entries and removes those entries from the raw message
        /// </summary>
        /// <param name="rawMessage">The raw message with 0 or more special entries</param>
        /// <param name="specialCharacter">The leading character identifying the special entries</param>
        /// <returns>A list of all the special entries with the designated leading character</returns>
        private static List<JProperty> GetSpecialJson(
            JObject rawMessage, 
            char specialCharacter
            )
        {
            List<JProperty> SpecialJson = (from P in rawMessage.Properties()
                                           where P.Value.ToString().StartsWith(specialCharacter.ToString())
                                           select P).ToList();

            foreach (JProperty specialProperty in SpecialJson)
            {
                rawMessage.Remove(specialProperty.Name);
            }

            return SpecialJson;
        }

        #endregion Private Static Methods

        /// <summary>
        /// Creates a string representation of the JSON of the telex including the special entries
        /// </summary>
        /// <returns>A string representation of the JSON</returns>
        public override string ToString()
        {
            return Payload.ToString() + Commands.ToString() + Signals.ToString() + Headers.ToString();
        }
    }
}
