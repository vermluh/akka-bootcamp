namespace WinTail
{
    using System;
    using Akka.Actor;

    public class TailCoordinatorActor : UntypedActor
    {
        #region methods
        #region protected override void OnReceive(object)
        protected override void OnReceive(object message)
        {
            if (message is StartTail)
            {
                var msg = message as StartTail;
                // here we are creating our first parent/child relationship!
                // the TailActor instance created here is a child
                // of this instance of TailCoordinatorActor
                Context.ActorOf(Props.Create(() => new TailActor(msg.ReporterActor, msg.FilePath)));
            }

        }
        #endregion

        #region protected override SupervisorStrategy SupervisorStrategy()
        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new OneForOneStrategy(
                10, // maxNumberOfRetries
                TimeSpan.FromSeconds(30), // withinTimeRange
                x => // localOnlyDecider
                {
                    //Maybe we consider ArithmeticException to not be application critical
                    //so we just ignore the error and keep going.
                    if (x is ArithmeticException) return Directive.Resume;

                    //Error that we cannot recover from, stop the failing actor
                    else if (x is NotSupportedException) return Directive.Stop;

                    //In all other cases, just restart the failing actor
                    else return Directive.Restart;
                });
        }
        #endregion
        #endregion

        #region classes
        #region StartTail
        /// <summary>
        /// Start tailing the file at user-specified path.
        /// </summary>
        public class StartTail
        {
            #region constructor
            public StartTail(string filePath, IActorRef reporterActor)
            {
                this.FilePath = filePath;
                this.ReporterActor = reporterActor;
            }
            #endregion

            #region properties
            #region public string FilePath { get; private set; }
            public string FilePath { get; private set; }
            #endregion

            #region public IActorRef ReporterActor { get; private set; }
            public IActorRef ReporterActor { get; private set; }
            #endregion
            #endregion
        }
        #endregion

        #region StopTail
        /// <summary>
        /// Stop tailing the file at user-specified path.
        /// </summary>
        public class StopTail
        {
            #region constructor
            public StopTail(string filePath)
            {
                this.FilePath = filePath;
            }
            #endregion

            #region properties
            #region public string FilePath { get; private set; }
            public string FilePath { get; private set; }
            #endregion
            #endregion
        }
        #endregion
        #endregion
    }
}
