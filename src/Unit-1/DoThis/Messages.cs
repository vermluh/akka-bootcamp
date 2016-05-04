namespace WinTail
{
    class Messages
    {
        #region Neutral/system messages
        /// <summary>
        /// Marker class to continue processing.
        /// </summary>
        public class ContinueProcessing { }
        #endregion

        #region Success messages
        // in Messages.cs
        /// <summary>
        /// Base class for signalling that user input was valid.
        /// </summary>
        public class InputSuccess
        {
            #region constructor
            public InputSuccess(string reason)
            {
                this.Reason = reason;
            }
            #endregion

            #region properties
            #region public string Reason { get; private set; }
            public string Reason { get; private set; }
            #endregion
            #endregion
        }
        #endregion

        #region Input error message
        /// <summary>
        /// Base class for signalling that user input was invalid.
        /// </summary>
        public class InputError
        {
            #region constructor
            public InputError(string reason)
            {
                Reason = reason;
            }
            #endregion

            #region properties
            #region public string Reason { get; private set; }
            public string Reason { get; private set; }
            #endregion
            #endregion
        }
        #endregion

        #region NullInput error message
        /// <summary>
        /// User provided blank input.
        /// </summary>
        public class NullInputError : InputError
        {
            #region constructor
            public NullInputError(string reason) 
                : base(reason)
            {
            }
            #endregion
        }
        #endregion

        #region Validation error message
        /// <summary>
        /// User provided invalid input (currently, input w/ odd # chars)
        /// </summary>
        public class ValidationError : InputError
        {
            #region constructor
            public ValidationError(string reason) 
                : base(reason)
            {
            }
            #endregion
        }
        #endregion
    }
}
