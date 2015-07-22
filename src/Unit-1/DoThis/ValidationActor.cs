namespace WinTail
{
    using Akka.Actor;

    public class ValidationActor : UntypedActor
    {
        #region fields
        private readonly IActorRef consoleWriterActor;
        #endregion

        #region constructor
        public ValidationActor(IActorRef consoleWriterActor)
        {
            this.consoleWriterActor = consoleWriterActor;
        }
        #endregion

        #region methods
        #region protected override void OnReceive(object)
        protected override void OnReceive(object message)
        {
            var msg = message as string;
            if (string.IsNullOrEmpty(msg))
            {
                this.consoleWriterActor.Tell(new Messages.NullInputError("No input received."));
            }
            else
            {
                var valid = ValidationActor.IsValid(msg);
                if (valid)
                {
                    // send success to console writer
                    this.consoleWriterActor.Tell(new Messages.InputSuccess("Thank you! Message was valid."));
                }
                else
                {
                    // signal that input was bad
                    this.consoleWriterActor.Tell(new Messages.ValidationError("Invalid: input had odd number of characters."));
                }
            }

            // tell sender to continue doing its thing (whatever that may be, this actor doesn't care)
            Sender.Tell(new Messages.ContinueProcessing());
        }
        #endregion

        #region private static bool IsValid(string)
        /// <summary>
        /// Determines if the message received is valid.
        /// Currently, arbitrarily checks if number of chars in message received is even.
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        private static bool IsValid(string msg)
        {
            var valid = msg.Length % 2 == 0;
            return valid;
        }
        #endregion
        #endregion
    }
}
