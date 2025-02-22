﻿using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RSDiagnostics
{
    public class Log
    {
        readonly static string outputFile = "output.log";

        /// <summary>
        /// Create a log file to be easily sharable to volunteers to look over.
        /// </summary>
        public Log()
        {
            Init();
            CheckForRedFlags_RocksmithSettings();
            DumpRocksmithINI();
            DumpASIO();
            Songs();
        }

        /// <summary>
        /// Log very basic information on their version of Rocksmith to the outputFile.
        /// </summary>
        void Init()
        {
            using (StreamWriter sw = File.CreateText(outputFile))
            {
                sw.WriteLine("Rocksmith Location: " + Settings.Settings.RocksmithLocation);
                sw.WriteLine("Valid CDLC DLL: " + MainForm.ValidCdlcDLL().ToString());
                sw.WriteLine("DLL Type: " + MainForm.DLLType);
                sw.WriteLine("Valid Game: " + MainForm.ValidGame.ToString());

                sw.WriteLine('\n');
            }
        }

        /// <summary>
        /// Look for common settings that can cause issues in Rocksmith's settings file.
        /// </summary>
        void CheckForRedFlags_RocksmithSettings()
        {
            using (StreamWriter sw = File.AppendText(outputFile))
            {
                sw.WriteLine("Potential Red Flags:");

                if ((int)Settings.Rocksmith.Settings.Where("Win32UltraLowLatencyMode").Value == 0)
                    sw.WriteLine("Win32UltraLowLatencyMode: OFF");

                if ((int)Settings.Rocksmith.Settings.Where("ForceWDM").Value == 1)
                    sw.WriteLine("ForceWDM: ON");

                if ((int)Settings.Rocksmith.Settings.Where("ForceDirectXSink").Value == 1)
                    sw.WriteLine("ForceDirectXSink: ON");

                if ((int)Settings.Rocksmith.Settings.Where("DumpAudioLog").Value == 1)
                    sw.WriteLine("DumpAudioLog: ON");

                sw.WriteLine("End Potential Red Flags!");

                sw.WriteLine('\n');
            }
        }

        /// <summary>
        /// Dump every Rocksmith.ini setting into the outputFile.
        /// </summary>
        void DumpRocksmithINI()
        {
            using (StreamWriter sw = File.AppendText(outputFile))
            {
                sw.WriteLine("Rocksmith.ini is as follows");
                string section = string.Empty;
                foreach(Settings.Rocksmith.Settings setting in Settings.Rocksmith.LoadSettings.LoadedSettings)
                {
                    if (setting.Section != section)
                    {
                        section = setting.Section;
                        sw.WriteLine("  " + "[" + section + "]");
                    }

                    sw.WriteLine("  " + setting.SettingName + "=" + setting.Value);
                }

                sw.WriteLine('\n');
            }
        }

        /// <summary>
        /// Dump every RS_ASIO.ini setting into the outputFile.
        /// </summary>
        void DumpASIO()
        {
            using (StreamWriter sw = File.AppendText(outputFile))
            {
                sw.WriteLine("RS_ASIO.ini is as follows");
                string section = string.Empty;
                foreach (Settings.Asio.Settings setting in Settings.Asio.LoadSettings.LoadedSettings)
                {
                    if (setting.Section != section)
                    {
                        section = setting.Section;
                        sw.WriteLine("  " + "[" + section + "]");
                    }

                    sw.WriteLine("  " + setting.SettingName + "=" + setting.Value);
                }

                sw.WriteLine('\n');
            }
        }

        /// <summary>
        /// Log information about the user's songs to the outputFile.
        /// </summary>
        void Songs()
        {
            List<SongData> ODLC = SongManager.Songs.Where(song => song.Value.ODLC == true && song.Value.RS1AppID == 0).Select(pair => pair.Value).ToList();
            using (StreamWriter sw = File.AppendText(outputFile))
            {
                sw.WriteLine("Total Songs: " + SongManager.Songs.Count);
                sw.WriteLine("Non-Authentic ODLC: " + SongManager.Validate(ODLC));

                sw.WriteLine('\n');
            }
        }
    }
}
