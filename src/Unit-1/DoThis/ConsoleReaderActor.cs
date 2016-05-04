using System;
using Akka.Actor;

namespace WinTail
{
    /// <summary>
    /// Actor responsible for reading FROM the console. 
    /// Also responsible for calling <see cref="ActorSystem.Shutdown"/>.
    /// </summary>
    class ConsoleReaderActor : UntypedActor
    {
        #region fields
        public const string ExitCommand = "exit";
        public const string StartCommand = "start";
        #endregion

        #region methods
        #region protected override void OnReceive(object)
        protected override void OnReceive(object message)
        {
            if (message.Equals(StartCommand))
            {
                this.DoPrintInstructions();
            }
            
            this.GetAndValidateInput();
        }
        #endregion

        #region private void DoPrintInstructions()
        private void DoPrintInstructions()
        {
            Console.WriteLine("Please provide the URI of a log file on disk.\n");
        }
        #endregion

        #region private void GetAndValidateInput()
        /// <summary>
        /// Reads input from console, validates it, then signals appropriate response
        /// (continue processing, error, success, etc.).
        /// </summary>
        private void GetAndValidateInput()
        {
            var message = Console.ReadLine();
            
            if (!string.IsNullOrEmpty(message) && String.Equals(message, ExitCommand, StringComparison.OrdinalIgnoreCase))
            {
                // shut down the entire actor system (allows the process to exit)
                Context.System.Terminate();
                return;
            }

            // otherwise, just hand message off for validation
            Context.ActorSelection("akka://MyActorSystem/user/validationActor").Tell(message);
        }
        #endregion
        #endregion
    }
}