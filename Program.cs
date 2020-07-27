using System;
using System.IO;

namespace Task_2_FileVisitor
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Please enter path directory: ");
            var path = Console.ReadLine();

            Console.WriteLine("Please enter filter for files:");
            var filterValue = Console.ReadLine();
            Predicate<string> filter = info => info.Contains(filterValue);

            FileVisitor fyleSystemVisitor;
            
            try
            {
                fyleSystemVisitor = new FileVisitor(path, filter);
                fyleSystemVisitor.ShowEventsInfo();
            }
            catch (DirectoryNotFoundException e)
            {
                throw new DirectoryNotFoundException($"Directory not found: {e}");
            }
            catch (ArgumentNullException e)
            {
                throw new ArgumentNullException($"Argument null: {e}");
            }
            catch (Exception)
            {
                throw;
            }

            fyleSystemVisitor.ShowFileInfo();

            Console.ReadLine();
        }
    }
}
