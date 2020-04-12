﻿using System;
using Utils.Logging;
using Utils.Discord;
using System.Windows.Forms;
using System.Diagnostics;

namespace Utils.Settings
{
    public class ToolkitSettings
    {
        private static IniFile ini;

        //Directories keys;
        public static string M2Directory;

        //Discord keys;
        public static bool DiscordEnabled;
        public static bool DiscordElapsedTimeEnabled;
        public static bool DiscordStateEnabled;
        public static bool DiscordDetailsEnabled;
        public static string CustomStateText;

        //ModelViewer keys;
        public static bool VSync;
        public static float ScreenDepth;
        public static float ScreenNear;
        public static float CameraSpeed;
        public static string ShaderPath;
        public static string TexturePath;
        public static bool Experimental;
        public static bool UseMIPS;
        public static float FieldOfView;
        public const int Width = 1920;
        public const int Height = 1080;
        public static int Season;

        //Model Exporting keys;
        public static int Format;
        public static string ExportPath;

        //Material Library Keys:
        public static string MaterialLibs;

        //Misc vars;
        private static long ElapsedTime;
        private static DiscordController controller;
        public const string DiscordLibLocation = "libs/discord-rpc";
        public static bool LoggingEnabled;
        public static int Language;
        public static int SerializeSDSOption;
        public static bool DecompileLUA;
        public static bool AddTimeDataBackup;
        public static bool UseSDSToolFormat;
        public static bool CookCollisions;
        public static bool CheckForUpdates;
        public static readonly float Version = 2.04f;

        public static void ReadINI()
        {
            ini = new IniFile();
            ElapsedTime = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;

            M2Directory = ReadKey("MafiaII", "Directories");
            TexturePath = ReadKey("TexturePath", "ModelViewer");
            int.TryParse(ReadKey("Season", "ModelViewer", "0"), out Season);
            CustomStateText = ReadKey("CustomStateText", "Discord", "Developing mods.");
            bool.TryParse(ReadKey("Enabled", "Discord", "True"), out DiscordEnabled);
            bool.TryParse(ReadKey("ElapsedTimeEnabled", "Discord", "True"), out DiscordElapsedTimeEnabled);
            bool.TryParse(ReadKey("StateEmabled", "Discord", "True"), out DiscordStateEnabled);
            bool.TryParse(ReadKey("DetailsEnabled", "Discord", "True"), out DiscordDetailsEnabled);
            int.TryParse(ReadKey("SerializeOption", "SDS", "0"), out SerializeSDSOption);
            bool.TryParse(ReadKey("VSync", "ModelViewer", "True"), out VSync);
            bool.TryParse(ReadKey("UseMIPS", "ModelViewer", "True"), out UseMIPS);
            float.TryParse(ReadKey("ScreenDepth", "ModelViewer", "10000"), out ScreenDepth);
            float.TryParse(ReadKey("ScreenNear", "ModelViewer", "1"), out ScreenNear);
            float.TryParse(ReadKey("CameraSpeed", "ModelViewer", "1"), out CameraSpeed);
            bool.TryParse(ReadKey("EnableExperimental", "ModelViewer", "0"), out Experimental);
            float.TryParse(ReadKey("FieldOfView", "ModelViewer", "60"), out FieldOfView);
            bool.TryParse(ReadKey("Logging", "Misc", "True"), out LoggingEnabled);
            int.TryParse(ReadKey("Language", "Misc", "0"), out Language);
            int.TryParse(ReadKey("Format", "Exporting", "0"), out Format);
            bool.TryParse(ReadKey("AddTimeDataBackup", "SDS", "True"), out AddTimeDataBackup);
            bool.TryParse(ReadKey("DecompileLUA", "SDS", "False"), out DecompileLUA);
            bool.TryParse(ReadKey("UseSDSToolFormat", "SDS", "False"), out UseSDSToolFormat);
            bool.TryParse(ReadKey("CookCollisions", "SDS", "False"), out CookCollisions);
            bool.TryParse(ReadKey("CheckForUpdates", "Misc", "True"), out CheckForUpdates);
            ExportPath = ReadKey("ModelExportPath", "Directories", Application.StartupPath);
            MaterialLibs = ReadKey("MaterialLibs", "Materials", "");


            ShaderPath = @"Shaders\";
            Log.LoggingEnabled = LoggingEnabled;

            if (DiscordEnabled)
                InitRichPresence();
        }

        private static string ReadKey(string key, string section, string defaultValue = null)
        {
            if (!ini.KeyExists(key, section))
                ini.Write(key, defaultValue, section);
            else
                return ini.Read(key, section);

            return defaultValue;
        }

        public static void WriteKey(string key, string section, string defaultValue)
        {
            ini.Write(key, defaultValue, section);
        }

        private static void InitRichPresence()
        {
            controller = new DiscordController();
            controller.Initialize();
            controller.presence = new DiscordRPC.RichPresence()
            {
                smallImageKey = "",
                smallImageText = "",
                largeImageKey = "main_art",
                largeImageText = "",
                startTimestamp = ElapsedTime
            };
            UpdateRichPresence("Using the Game Explorer");
        }

        public static void UpdateRichPresence(string details = null)
        {
            if (!DiscordEnabled)
            {
                DiscordRPC.Shutdown();
                controller = null;
            }
            else
            {
                if (controller == null)
                    InitRichPresence();

                details = ""; //don't like current imp.
                string detailsLine = string.IsNullOrEmpty(details) ? ToolkitSettings.CustomStateText : details;
                controller.presence.state = DiscordStateEnabled ? detailsLine : null;
                string vString = Debugger.IsAttached ? "DEBUG " : "RELEASE ";
                vString += Version;
                controller.presence.details = DiscordDetailsEnabled ? vString : null;
                controller.presence.startTimestamp = DiscordElapsedTimeEnabled ? ElapsedTime : 0;

                DiscordRPC.UpdatePresence(ref controller.presence);
            }
        }
    }
}