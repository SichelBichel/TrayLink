﻿using Microsoft.Win32;
using System;
using System.IO;
using System.Text;

namespace TrayLink.Helper
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage:\n  TrayLinkHelper.exe --register\n  TrayLinkHelper.exe <path-to-file-or-folder>");
                return;
            }

            if (args[0] == "--register")
            {
                RegisterContextMenu();
            }
            else
            {
                AddEntryToConfig(args[0]);
            }
        }

        private static void RegisterContextMenu()
        {
            try
            {
                string exePath = Path.Combine(AppContext.BaseDirectory, "TrayLinkHelper.exe");

                void AddRegistry(string shellType)
                {
                 
                    using var key = Registry.ClassesRoot.CreateSubKey($@"{shellType}\shell\AddToTrayLink");
                    key.SetValue("", "Add to TrayLink");
                    key.SetValue("Icon", exePath);      

                   
                    using var commandKey = key.CreateSubKey("command");

              
                    commandKey.SetValue("", $"\"{exePath}\" \"%1\"");
                }

                AddRegistry("*");

                AddRegistry("Directory");

                Console.WriteLine("TrayLink context menu registered successfully.");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error registering context menu: {ex.Message}");
            }
        }


        private static void AddEntryToConfig(string selectedPath)
        {
            try
            {
                Console.WriteLine($"Received path: {selectedPath}");

                if (!File.Exists(selectedPath) && !Directory.Exists(selectedPath))
                {
                    Console.Error.WriteLine("Error: Path does not exist.");
                    return;
                }

                string actionType = File.Exists(selectedPath) ? "FILE" : "PATH";
                string actionName = Path.GetFileName(selectedPath);

                if (Directory.Exists(selectedPath))
                {
                    actionName += " (Folder)";
                }

                string configPath = Path.Combine(AppContext.BaseDirectory, "actions.ini");
                Console.WriteLine($"Appending to: {configPath}");

                var sb = new StringBuilder();
                sb.AppendLine($"[Action{DateTime.Now.Ticks}]");
                sb.AppendLine($"ActionName=Open {actionName}");
                sb.AppendLine($"ActionType={actionType}");
                sb.AppendLine($"PathOrUrl={selectedPath}");
                sb.AppendLine();

                File.AppendAllText(configPath, sb.ToString());

                Console.WriteLine("Entry added successfully.");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error adding to config: {ex.Message}");
            }
        }
    }
}