﻿using System;
using System.IO;
using System.Diagnostics;

namespace ConHexView
{
    class Program
    {
        const string UPDATER_NAME = "0xdd_updater.exe";

        /// <summary>
        /// Get the current version of the project as a string object.
        /// </summary>
        static string ProjectVersionString
        {
            get
            {
                return
                    System.Reflection.Assembly
                    .GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        /// <summary>
        /// Gets the current version of the project as a <see cref="Version"/> object.
        /// </summary>
        static Version ProjectVersion
        {
            get
            {
                return
                    System.Reflection.Assembly
                    .GetExecutingAssembly().GetName().Version;
            }
        }

        /// <summary>
        /// Get the project's name.
        /// </summary>
        static string ProjectName
        {
            get
            {
                return
                    System.Reflection.Assembly
                    .GetExecutingAssembly().GetName().Name;
            }
        }
        
        /// <summary>
        /// Get the current executable's filename.
        /// </summary>
        static string ExecutableFilename
        {
            get
            {
                return
                    Path.GetFileName(
                        System.Diagnostics.Process
                        .GetCurrentProcess().MainModule.FileName
                    );
            }
        }

        static int Main(string[] args)
        {
#if DEBUG
            //args = new string[] { ExecutableFilename };
            //args = new string[] { "f" };
            //args = new string[] { "tt" };
            //args = new string[] { "-dump", "tt" };
            //args = new string[] { "gg.txt" };
#endif

            if (args.Length == 0)
            {
                // Future reminder:
                // New buffer in editing mode if no arguments
                ShowHelp();
                return 0;
            }
            
            string file = args[args.Length - 1];

            // Defaults
            int bytesInRow = 16;
            HexView.OffsetViewMode ovm = HexView.OffsetViewMode.Hexadecimal;

            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-v":
                    case "/v":
                        switch (args[i + 1])
                        {
                            // h is default.
                            case "d":
                                ovm = HexView.OffsetViewMode.Decimal;
                                break;
                            case "o":
                                ovm = HexView.OffsetViewMode.Octal;
                                break;
                            default:
                                Console.WriteLine($"Invalid parameter for -v: {args[i + 1]}");
                                return 1;
                        }
                        break;

                    case "-w":
                    case "/w":
                        if (!int.TryParse(args[i + 1], out bytesInRow))
                        {
                            Console.WriteLine($"Invalid parameter for -w: {args[i + 1]}");
                            return 1;
                        }
                        break;

                    case "-U":
                    case "/U":
                        return Update();

                    case "-dump":
                    case "/dump":
                        Console.WriteLine("Dumping file...");
                        int err = HexView.Dump(file, bytesInRow, ovm);
                        switch (err)
                        {
                            case 1:
                                Console.WriteLine("File not found, aborted.");
                                break;
                            case 0:
                                Console.WriteLine("Dumping done!");
                                break;
                            default:
                                Console.WriteLine("Unknown error, aborted.");
                                break;
                        }
                        return err;
                }
            }

            if (File.Exists(file))
            {
                Console.Clear();

#if RELEASE
                try
                {
                    HexView.Open(file, ovm);
                }
                catch (Exception e)
                {
                    Abort(e);
                }
#elif DEBUG
                // I want Visual Studio to catch the exceptions!
                HexView.Open(file, ovm, bytesInRow);
#endif
            }
            else
            {
                Console.WriteLine("File not found.");
                return 1;
            }

            return 0;
        }

        static int Update()
        {
            if (File.Exists(UPDATER_NAME))
            {
                //TODO: Fix updater output
                ProcessStartInfo updater = new ProcessStartInfo(UPDATER_NAME);
                updater.RedirectStandardError = true;
                updater.RedirectStandardInput = true;
                updater.RedirectStandardOutput = true;
                updater.UseShellExecute = false;
                Process.Start(updater);
                return 0;
            }
            else
            {
                Console.WriteLine("ABORTED: Updater not found. (0xdd_updater.exe)");
                return 1;
            }
        }
        
        static void Abort(Exception e)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine(" !! Fatal error !! ");
            Console.ResetColor();
            Console.WriteLine($"Exception: {e.GetType()}");
            Console.WriteLine($"Message: {e.Message}");
            Console.WriteLine($"Stack: {e.StackTrace}");
            Console.WriteLine();
        }

        static void ShowHelp()
        {
            //                 1       10        20        30        40        50        60        70        80
            //                 |--------|---------|---------|---------|---------|---------|---------|---------|
            Console.WriteLine(" Usage:");
            Console.WriteLine("  0xdd [-v {h|d|o}] [-w n] [-U] [-dump] <file>");
            Console.WriteLine();
            Console.WriteLine("  -v       Start with an offset view: Hex, Dec, Oct.          Default: Hex");
            Console.WriteLine("  -w       Start with a number of bytes to show in a row.     Default: 16");
            Console.WriteLine("  -U       Updates if necessary.");
            Console.WriteLine("  -dump    Dumps a data file as plain text.");
            Console.WriteLine();
            Console.WriteLine("  /help, /?   Shows this screen and exits.");
            Console.WriteLine("  /version    Shows version and exits.");
        }

        static void ShowVersion()
        {
            //         1       10        20        30        40        50        60        70        80
            //         |--------|---------|---------|---------|---------|---------|---------|---------|
            Console.WriteLine();
            Console.WriteLine($"0xDD - {ProjectVersion}");
            Console.WriteLine("Copyright (c) 2015 DD~!/guitarxhero");
            Console.WriteLine("License: MIT License <http://opensource.org/licenses/MIT>");
            Console.WriteLine("Project page: <https://github.com/guitarxhero/0xDD>");
            Console.WriteLine();
            Console.WriteLine(" -- Credits --");
            Console.WriteLine("DD~! (guitarxhero) - Original author");
        }
    }
}
