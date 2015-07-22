namespace WinTail
{
    using System;
    using System.IO;

    using Akka.Actor;

    public partial class FileObserver
    {
        #region fields
        private readonly IActorRef tailActor;
        private readonly string absoluteFilePath;
        private FileSystemWatcher watcher;
        private readonly string fileDir;
        private readonly string fileNameOnly;
        #endregion

        #region constructor
        public FileObserver(IActorRef tailActor, string absoluteFilePath)
        {
            this.tailActor = tailActor;
            this.absoluteFilePath = absoluteFilePath;
            this.fileDir = Path.GetDirectoryName(absoluteFilePath);
            this.fileNameOnly = Path.GetFileName(absoluteFilePath);
        }
        #endregion

        #region methods
        #region public void Start()
        /// <summary>
        /// Begin monitoring file.
        /// </summary>
        public void Start()
        {
            // make watcher to observe our specific file
            this.watcher = new FileSystemWatcher(this.fileDir, this.fileNameOnly);

            // watch our file for changes to the file name, or new messages being written to file
            this.watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite;

            // assign callbacks for event types
            this.watcher.Changed += OnFileChanged;
            this.watcher.Error += OnFileError;

            // start watching
            this.watcher.EnableRaisingEvents = true;
        }
        #endregion

        #region private void OnFileError(object sender, ErrorEventArgs e)
        /// <summary>
        /// Callback for <see cref="FileSystemWatcher"/> file error events.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnFileError(object sender, ErrorEventArgs e)
        {
            this.tailActor.Tell(new TailActor.FileError(this.fileNameOnly, e.GetException().Message), ActorRefs.NoSender);
        }
        #endregion

        #region private void OnFileChanged(object, FileSystemEventArgs)
        /// <summary>
        /// Callback for <see cref="FileSystemWatcher"/> file change events.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Changed)
            {
                // here we use a special ActorRefs.NoSender
                // since this event can happen many times, this is a little microoptimization
                this.tailActor.Tell(new TailActor.FileWrite(e.Name), ActorRefs.NoSender);
            }
        }
        #endregion
        #endregion
    }

    public partial class FileObserver : IDisposable
    {
        #region methods
        #region  public void Dispose()
        public void Dispose()
        {
            this.watcher.Dispose();
        }
        #endregion
        #endregion
    }
}
