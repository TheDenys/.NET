using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using CompressLib;
using CompressLib.Exceptions;

namespace CompressConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var compressionArguments = GetValidArguments(args);

            try
            {
                MultithreadGZip.ProcessFile(compressionArguments.CompressionMode, compressionArguments.OriginFileName, compressionArguments.ResultFileName);
                Console.WriteLine("Operation succeeded.");
            }
            catch (CompressLibCompoundException ex)
            {
                var firstInnerException = ex.InnerExceptions.FirstOrDefault();

                if (firstInnerException is FileNotFoundException)
                {
                    var fne = firstInnerException as FileNotFoundException;
                    OnError($"Can't find file: {fne.FileName}. Error: {fne.Message}");
                }

                OnError($"Can't compress/decompress file. Error: {ex.Message}");
            }
            catch (EmptyFileException ex)
            {
                OnError($"Can't compress/decompress empty file. Error: {ex.Message}");
            }
            catch (InvalidDataException ex)
            {
                OnError($"Original file is probably not a GZipped archive. Error: {ex.Message}");
            }
            catch (DirectoryNotFoundException ex)
            {
                OnError($"Target folder not found. Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                OnError($"Error: {ex.Message}");
            }
        }

        static void OnError(string message)
        {
            Console.Error.WriteLine(message);
            Environment.Exit(1);
        }

        static ArgumentsContainer GetValidArguments(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine(Usage);
                Environment.Exit(0);
            }
            else if (args.Length != 3)
            {
                Console.Error.WriteLine(WrongArguments);
                Environment.Exit(1);
            }

            // parse and validate mode
            var modeStr = args[0];
            CompressionMode mode = 0;
            try
            {
                mode = (CompressionMode)Enum.Parse(typeof(CompressionMode), modeStr, true);

                switch (mode)
                {
                    case CompressionMode.Compress:
                    case CompressionMode.Decompress:
                        break;
                    default:
                        throw new ArgumentException($"Not supported mode [{mode}].");
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Not supported mode value [{modeStr}]. Allowed values are: [compress, decompress]\n\r{Usage}");
                Environment.Exit(3);
            }

            string originFileName = args[1];

            if (!File.Exists(originFileName))
            {
                Console.Error.WriteLine($"Original file [{originFileName}] was not found. Check if it exists and accessible by user.");
                Environment.Exit(4);
            }

            string resultFileName = args[2];

            return new ArgumentsContainer(mode, originFileName, resultFileName);
        }

        private static string WrongArguments => $@"Wrong amount of arguments
{Usage}";

        static string Usage => $@"Usage:
{Assembly.GetEntryAssembly().GetName().Name}.exe compress|decompress origin_file_name result_file_name";
    }
}
