namespace WinTail
{
    using System.IO;
    using System.Text;

    using Akka.Actor;

    public class TailActor : UntypedActor
    {
        private readonly string filePath;
        private readonly IActorRef reporterActor;
        private FileObserver observer;
        private Stream fileStream;
        private StreamReader fileStreamReader;

        #region constructor
        public TailActor(IActorRef reporterActor, string filePath)
        {
            this.reporterActor = reporterActor;
            this.filePath = filePath;
        }
        #endregion

        #region methods
        #region protected override void OnReceive(object)
        protected override void OnReceive(object message)
        {
            if (message is FileWrite)
            {
                // move file cursor forward
                // pull results from cursor to end of file and write to output
                // (this is assuming a log file type format that is append-only)
                var text = fileStreamReader.ReadToEnd();
                if (!string.IsNullOrEmpty(text))
                {
                    reporterActor.Tell(text);
                }

            }
            else if (message is FileError)
            {
                var fe = message as FileError;
                reporterActor.Tell(string.Format("Tail error: {0}", fe.Reason));
            }
            else if (message is InitialRead)
            {
                var ir = message as InitialRead;
                reporterActor.Tell(ir.Text);
            }
        }
        #endregion

        #region protected override void PostStop()
        protected override void PostStop()
        {
            this.observer.Dispose();
            this.observer = null;
            this.fileStreamReader.Close();
            this.fileStreamReader.Dispose();
            base.PostStop();
        }
        #endregion

        #region protected override void PreStart()
        protected override void PreStart()
        {
            // start watching file for changes
            this.observer = new FileObserver(Self, Path.GetFullPath(this.filePath));
            this.observer.Start();

            // open the file stream with shared read/write permissions (so file can be written to while open)
            this.fileStream = new FileStream(Path.GetFullPath(this.filePath), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            this.fileStreamReader = new StreamReader(fileStream, Encoding.UTF8);

            // read the initial contents of the file and send it to console as first message
            var text = fileStreamReader.ReadToEnd();
            Self.Tell(new InitialRead(this.filePath, text));
        }
        #endregion
        #endregion

        #region classes
        #region public class FileWrite
        /// <summary>
        /// Signal that the file has changed, and we need to read the next line of the file.
        /// </summary>
        public class FileWrite
        {
            #region constructor
            public FileWrite(string fileName)
            {
                this.FileName = fileName;
            }
            #endregion

            #region properties
            #region public string FileName { get; private set; }
            public string FileName { get; private set; }
            #endregion
            #endregion
        }
        #endregion

        #region public class FileError
        /// <summary>
        /// Signal that the OS had an error accessing the file.
        /// </summary>
        public class FileError
        {
            #region constructor
            public FileError(string fileName, string reason)
            {
                this.FileName = fileName;
                this.Reason = reason;
            }
            #endregion

            #region properties
            #region public string FileName { get; private set; }
            public string FileName { get; private set; }
            #endregion

            #region public string Reason { get; private set; }
            public string Reason { get; private set; }
            #endregion
            #endregion
        }
        #endregion

        #region public class InitialRead
        /// <summary>
        /// Signal to read the initial contents of the file at actor startup.
        /// </summary>
        public class InitialRead
        {
            #region constructor
            public InitialRead(string fileName, string text)
            {
                this.FileName = fileName;
                this.Text = text;
            }
            #endregion

            #region properties
            #region public string FileName { get; private set; }
            public string FileName { get; private set; }
            #endregion

            #region public string Text { get; private set; }
            public string Text { get; private set; }
            #endregion
            #endregion
        }
        #endregion
        #endregion
    }
}
