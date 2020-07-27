using System;

namespace Task_2_FileVisitor.Events
{
    public class DirectoryEventArgs : EventArgs
    {
        public string Path { get; set; }

        public bool Stop { get; set; }

        public bool Exclude { get; set; }
    }
}
