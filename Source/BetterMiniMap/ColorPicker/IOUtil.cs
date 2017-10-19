using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using Verse;

namespace ColorPicker.Dialog
{
    static class IOUtil
    {
        public static ColorPresets LoadColorPresets()
        {
            ColorPresets presets = CreateDefaultColorPresets();
            try
            {
                if (TryGetFileName(out string fileName) &&
                    File.Exists(fileName))
                {
                    // Load Data
                    using (StreamReader sr = new StreamReader(fileName))
                    {
                        string version = sr.ReadLine();
                        if (version.Equals("Version 1"))
                        {
                            for (int i = 0; i < presets.Count; ++i)
                            {
                                try
                                {
                                    string[] s = sr.ReadLine().Split(new Char[] { ':' });
                                    Color c = Color.white;
                                    c.r = float.Parse(s[0]);
                                    c.g = float.Parse(s[1]);
                                    c.b = float.Parse(s[2]);
                                    presets[i] = c;
                                }
                                catch
                                {
                                    presets[i] = Color.white;
                                }
                            }
                        }
                        else
                        {
                            presets = new ColorPresets();
                            {
                                for (int i = 0; i < presets.Count; ++i)
                                    presets[i] = Color.white;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Warning(
                    "Problem while loading ReColor Color Presets:\n" +
                    e.GetType().Name + " " + e.Message + "\n" + e.StackTrace);
                presets = CreateDefaultColorPresets();
            }
            presets.IsModified = false;
            return presets;
        }

        private static ColorPresets CreateDefaultColorPresets()
        {
            ColorPresets presets = new ColorPresets();
            for (int i = 0; i < presets.Count; ++i)
                presets[i] = Color.white;
            return presets;
        }

        public static void SaveColorPresets(ColorPresets presets)
        {
            try
            {
                if (!TryGetFileName(out string fileName))
                {
                    throw new Exception("Unable to get file name.");
                }

                // Write Data
                using (FileStream fileStream = File.Open(fileName, FileMode.Create, FileAccess.Write))
                {
                    using (StreamWriter sw = new StreamWriter(fileStream))
                    {
                        sw.WriteLine("Version 1");
                        for (int i = 0; i < presets.Count; ++i)
                        {
                            Color c = presets[i];
                            sw.WriteLine(c.r + ":" + c.g + ":" + c.b);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(
                    "Problem while saving ReColor Color Presets:\n" +
                    e.GetType().Name + " " + e.Message + "\n" + e.StackTrace);
            }
        }

        private static bool TryGetFileName(out string fileName)
        {
            if (TryGetDirectoryPath(out fileName))
            {
                fileName = Path.Combine(fileName, "presets.xml");
                return true;
            }
            return false;
        }

        private static bool TryGetDirectoryPath(out string path)
        {
            if (TryGetDirectoryName(out path))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(path);
                if (!directoryInfo.Exists)
                {
                    directoryInfo.Create();
                }
                return true;
            }
            return false;
        }

        private static bool TryGetDirectoryName(out string path)
        {
            try
            {
                path = (string)typeof(GenFilePaths).GetMethod("FolderUnderSaveData", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, new object[]
                {
                    "ReColorStockpile"
                });
                return true;
            }
            catch (Exception ex)
            {
                Log.Error("ReColorStockpile: Failed to get folder name - " + ex);
                path = null;
                return false;
            }
        }
    }
}