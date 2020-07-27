using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Task_2_FileVisitor.Events;

namespace Task_2_FileVisitor
{
    public class FileVisitor : IEnumerable<string>
    {
        private readonly string _root;
        private readonly Predicate<string> _filter = (x) => true;

        private event EventHandler<EventArgs> VisitStarted;
        private event EventHandler<EventArgs> VisitFinished;
        private event EventHandler<DirectoryEventArgs> FilteredFileFound;
        private event EventHandler<DirectoryEventArgs> FilteredDirectoryFound;
        private event EventHandler<DirectoryEventArgs> FileFound;
        private event EventHandler<DirectoryEventArgs> DirectoryFound;

        public FileVisitor(string root)
        {
            if (root == null)
            {
                throw new ArgumentNullException(nameof(root));
            }
            if (!Directory.Exists(root))
            {
                throw new DirectoryNotFoundException();
            }

            _root = root;
        }

        public FileVisitor(string root, Predicate<string> filter) : this(root)
        {
            _filter = filter ?? throw new ArgumentNullException(nameof(filter));
        }

        public void ShowEventsInfo()
        {
            VisitStarted += (sender, e) => { Console.WriteLine("\nEVENT VisitStarted\n"); };
            VisitFinished += (sender, e) => { Console.WriteLine("\nEVENT VisitFinished"); };
            DirectoryFound += (sender, e) => { Console.WriteLine($"\n__________________________________________________\nEVENT DirectoryFound\nFOLDER: {e.Path}"); };
            FileFound += (sender, e) => { Console.WriteLine($"EVENT FileFound FILE: {e.Path}"); };
            FilteredDirectoryFound += (sender, e) => { Console.WriteLine($"EVENT FilteredDirectoryFound: {e.Path}"); };
            FilteredFileFound += (sender, e) => { Console.WriteLine($"EVENT FilteredFileFound: {e.Path}"); };
        }

        public void ShowFileInfo()
        {
            var result = new List<string>();

            foreach (var item in this)
            {
                result.Add(item);
            }

            Console.WriteLine("All filtered files: ");
            foreach (var item in result)
            {
                Console.WriteLine(item);
            }
        }

        private IEnumerable<string> VisitFile(string root)
        {
            if (root == null)
            {
                throw new ArgumentNullException($"{nameof(root)}");
            }

            OnVisitStarted(new EventArgs());

            foreach (var item in VisitDirectory(root))
            {
                yield return item;
            }

            OnVisitFinished(new EventArgs());
        }

        private IEnumerable<string> VisitDirectory(string root)
        {
            foreach (var file in VisitFiles(root))
            {
                yield return file;
            }

            foreach (var subDirectory in Directory.EnumerateDirectories(root))
            {
                var directoryEventArgs = new DirectoryEventArgs { Path = subDirectory };

                OnDirectoryFound(directoryEventArgs);

                if (directoryEventArgs.Stop)
                {
                    yield break;
                }

                if (!directoryEventArgs.Exclude && _filter(subDirectory))
                {
                    var filteredDirectoryEventArgs = new DirectoryEventArgs { Path = subDirectory };

                    OnFilteredDirectoryFound(directoryEventArgs);

                    if (filteredDirectoryEventArgs.Stop)
                    {
                        yield break;
                    }

                    if (!filteredDirectoryEventArgs.Exclude)
                    {
                        yield return subDirectory;
                    }
                }

                foreach (var item in VisitDirectory(subDirectory))
                {
                    yield return item;
                }
            }
        }

        private IEnumerable<string> VisitFiles(string root)
        {
            foreach (var file in Directory.EnumerateFiles(root))
            {
                var fileEventArgs = new DirectoryEventArgs { Path = file };

                OnFileFound(fileEventArgs);

                if (fileEventArgs.Stop)
                {
                    yield break;
                }

                if (!fileEventArgs.Exclude && _filter(file))
                {
                    var filteredFileEventArgs = new DirectoryEventArgs { Path = file };
                    OnFilteredFileFound(filteredFileEventArgs);

                    if (filteredFileEventArgs.Stop)
                    {
                        yield break;
                    }

                    if (!filteredFileEventArgs.Exclude)
                    {
                        yield return file;
                    }
                }
            }
        }

        public IEnumerator<string> GetEnumerator()
        {
            return VisitFile(_root).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void OnVisitStarted(EventArgs args)
        {
            VisitStarted?.Invoke(this, args);
        }

        private void OnVisitFinished(EventArgs args)
        {
            VisitFinished?.Invoke(this, args);
        }

        private void OnFileFound(DirectoryEventArgs args)
        {
            FileFound?.Invoke(this, args);
        }

        private void OnDirectoryFound(DirectoryEventArgs args)
        {
            DirectoryFound?.Invoke(this, args);
        }

        private void OnFilteredFileFound(DirectoryEventArgs args)
        {
            FilteredFileFound?.Invoke(this, args);
        }

        private void OnFilteredDirectoryFound(DirectoryEventArgs args)
        {
            FilteredDirectoryFound?.Invoke(this, args);
        }
    }
}
