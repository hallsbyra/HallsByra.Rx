using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HallsByra.Rx
{
    // TODO:
    // * Introduce an event with the type of change instead of just the file name.
    // * Throttle per unique file using GroupByUntil

    public static class FileSystem
    {
        public static IObservable<string> WhenFileChanges(string pathToFile)
        {
            return WhenPathChanges(Path.GetDirectoryName(pathToFile), Path.GetFileName(pathToFile));
        }

        public static IObservable<string> WhenPathChanges(string path, string filter)
        {
            return Observable.Create<string>(observer =>
            {
                var watcher = new FileSystemWatcher(path, filter);
                var changed = Observable.FromEventPattern<FileSystemEventHandler, FileSystemEventArgs>(h => watcher.Changed += h, h => watcher.Changed -= h);
                var deleted = Observable.FromEventPattern<FileSystemEventHandler, FileSystemEventArgs>(h => watcher.Deleted += h, h => watcher.Deleted -= h);
                var created = Observable.FromEventPattern<FileSystemEventHandler, FileSystemEventArgs>(h => watcher.Created += h, h => watcher.Created -= h);
                watcher.EnableRaisingEvents = true;
                return
                    changed
                    .Merge(deleted)
                    .Merge(created)
                    .Select(e => e.EventArgs.FullPath)
                    .Throttle(TimeSpan.FromMilliseconds(100))
                    .Finally(() => watcher.Dispose())
                    .Subscribe(observer);
            });
        }
    }
}
