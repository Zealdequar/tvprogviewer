using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ClearPluginAssemblies
{
    public class Program
    {
        protected const string FILES_TO_DELETE = "dotnet-bundle.exe;TvProgViewer.WebUI.pdb;TvProgViewer.WebUI.exe;TvProgViewer.WebUI.exe.config";

        protected static void Clear(string paths, IList<string> fileNames, bool saveLocalesFolders)
        {
            foreach (var pluginPath in paths.Split(';'))
            {
                try
                {
                    var pluginDirectoryInfo = new DirectoryInfo(pluginPath);
                    var allDirectoryInfo = new List<DirectoryInfo> { pluginDirectoryInfo };

                    if (!saveLocalesFolders)
                        allDirectoryInfo.AddRange(pluginDirectoryInfo.GetDirectories());

                    foreach (var directoryInfo in allDirectoryInfo)
                    {
                        foreach (var fileName in fileNames)
                        {
                            //delete dll file if it exists in current path
                            var dllfilePath = Path.Combine(directoryInfo.FullName, fileName + ".dll");
                            if (File.Exists(dllfilePath))
                                File.Delete(dllfilePath);
                            //delete pdb file if it exists in current path
                            var pdbfilePath = Path.Combine(directoryInfo.FullName, fileName + ".pdb");
                            if (File.Exists(pdbfilePath))
                                File.Delete(pdbfilePath);
                        }

                        foreach (var fileName in FILES_TO_DELETE.Split(';'))
                        {
                            //delete file if it exists in current path
                            var pdbfilePath = Path.Combine(directoryInfo.FullName, fileName);
                            if (File.Exists(pdbfilePath))
                                File.Delete(pdbfilePath);
                        }

                        if (!directoryInfo.GetFiles().Any() && !directoryInfo.GetDirectories().Any() && !saveLocalesFolders)
                            directoryInfo.Delete(true);
                    }
                }
                catch
                {
                    //do nothing
                }
            }
        }

        private static void Main(string[] args)
        {
            try
            {
                // Для отладки: записываем в файл что получили
                File.WriteAllText("debug.log", $"Args count: {args.Length}\n");
                for (int i = 0; i < args.Length; i++)
                {
                    File.AppendAllText("debug.log", $"Arg[{i}]: {args[i]}\n");
                }

                var outputPath = string.Empty;
                var pluginPaths = string.Empty;
                var saveLocalesFolders = true;

                // Собираем все аргументы в одну строку
                var allArgs = string.Join(" ", args);
                File.AppendAllText("debug.log", $"All args: {allArgs}\n");

                // Разбиваем по |, но учитываем что в путях может быть |
                var parts = new List<string>();
                var currentPart = new StringBuilder();
                bool inQuotes = false;

                for (int i = 0; i < allArgs.Length; i++)
                {
                    char c = allArgs[i];

                    if (c == '"')
                    {
                        inQuotes = !inQuotes;
                        currentPart.Append(c);
                    }
                    else if (c == '|' && !inQuotes)
                    {
                        parts.Add(currentPart.ToString().Trim());
                        currentPart.Clear();
                    }
                    else
                    {
                        currentPart.Append(c);
                    }
                }

                if (currentPart.Length > 0)
                {
                    parts.Add(currentPart.ToString().Trim());
                }

                File.AppendAllText("debug.log", $"Parts count: {parts.Count}\n");
                foreach (var part in parts)
                {
                    File.AppendAllText("debug.log", $"Part: {part}\n");
                }

                foreach (var part in parts)
                {
                    var equalIndex = part.IndexOf('=');
                    if (equalIndex <= 0)
                        continue;

                    var name = part.Substring(0, equalIndex).Trim();
                    var value = part.Substring(equalIndex + 1).Trim();

                    // Убираем кавычки если есть
                    if (value.StartsWith("\"") && value.EndsWith("\""))
                        value = value.Substring(1, value.Length - 2);

                    File.AppendAllText("debug.log", $"Name: '{name}', Value: '{value}'\n");

                    switch (name)
                    {
                        case "OutputPath":
                            outputPath = value;
                            // Убираем возможный разделитель в конце
                            if (outputPath.EndsWith("|"))
                                outputPath = outputPath.Substring(0, outputPath.Length - 1);
                            break;
                        case "PluginPath":
                            pluginPaths = value;
                            break;
                        case "SaveLocalesFolders":
                            bool.TryParse(value, out saveLocalesFolders);
                            break;
                    }
                }

                File.AppendAllText("debug.log", $"Parsed - OutputPath: '{outputPath}'\n");
                File.AppendAllText("debug.log", $"Parsed - PluginPaths: '{pluginPaths}'\n");
                File.AppendAllText("debug.log", $"Parsed - SaveLocalesFolders: {saveLocalesFolders}\n");

                if (!Directory.Exists(outputPath))
                {
                    File.AppendAllText("debug.log", $"OutputPath does not exist: {outputPath}\n");
                    return;
                }

                var di = new DirectoryInfo(outputPath);
                var separator = Path.DirectorySeparatorChar;
                var folderToIgnore = string.Concat(separator, "Plugins", separator);
                var fileNames = di.GetFiles("*.dll", SearchOption.AllDirectories)
                    .Where(fi => !fi.FullName.Contains(folderToIgnore))
                    .Select(fi => fi.Name.Replace(fi.Extension, "")).ToList();

                if (string.IsNullOrEmpty(pluginPaths) || !fileNames.Any())
                {
                    File.AppendAllText("debug.log", "No plugin paths or no dlls found\n");
                    return;
                }

                Clear(pluginPaths, fileNames, saveLocalesFolders);
                File.AppendAllText("debug.log", "Clear completed successfully\n");
            }
            catch (Exception ex)
            {
                File.WriteAllText("error.log", $"Exception: {ex.Message}\n{ex.StackTrace}");
                Environment.Exit(1);
            }
        }
    }
}
