// Lic:
// NJCR
// Show
// 
// 
// 
// (c) Jeroen P. Broks, 
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 
// Please note that some references to data like pictures or audio, do not automatically
// fall under this licenses. Mostly this is noted in the respective files.
// 
// Version: 19.10.19
// EndLic

using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using UseJCR6;
using TrickyUnits;

namespace NJCR {
    class F_Show:FeatBase {

        readonly string ShowConfig = Dirry.C("$AppSupport$/NJCR_Show.GINI");
        TGINI Config;
        string TempFolder => Dirry.AD(Config.C("Temp.Dir"));

        public override void Parse(FlagParse fp) {                    }

        public override void Run(FlagParse fp) {
            if (fp.Args.Length < 2) {
                QCol.Green("This feature allows you to show the content of a certain file.\n");
                QCol.Green("In order to do this, some additional configuration will be needed, as well as knowledge of the file system you are using.\n");
                QCol.Green("Don't worry about having to set up this file, manually. You will be prompted whenever NJCR requires more information.\n");
                QCol.Green($"The configuration can lateron be edited with your favority text editor (as long as it supports Unix LF line ends), as it will be saved as {ShowConfig}");
                return;
            }            
            QCol.Doing("Reading JCR", fp.Args[1].Replace("\\", "/"));
            var jcr = JCR6.Dir(fp.Args[1]);
            if (jcr==null) { QCol.QuickError(JCR6.JERROR); return; }
            if (!File.Exists(ShowConfig)) QuickStream.SaveString(ShowConfig, "[rem]\nNothint to see here yet\n");
            Config = GINI.ReadFromFile(ShowConfig);
            Ask("Temp.Dir", "I am in need of a temp-directory.\nYou can pick any directory you want for this, and the system will try to create the desired directory if it doesn't yet exist.\nThe files to show will be temporarily stored here, and IT'S VERY IMPORTANT THAT THIS DIRECTORY IS ONLY USED FOR THIS PURPOSE!!!", "Temp Folder");
            QCol.Doing("Temp dir", TempFolder);
            Directory.CreateDirectory(TempFolder);
            for (int i = 2; i < fp.Args.Length; i++) Show(jcr, fp.Args[i]);
        }

        void Ask(string Tag, string Uitleg,string Vraag) {
            if (Config.C(Tag) != "") return;
            QCol.Green($"{Uitleg}\n");
            var Antwoord = "";
            do {
                QCol.Yellow($"{Vraag}: ");
                Antwoord = Console.ReadLine().Replace("\\", "/").Trim();
            } while (Antwoord == "");
            Config.D(Tag, Antwoord);
            Config.SaveSource(ShowConfig);
        }


        void Show(TJCRDIR jcr,string entry) {
            try {
                var Ext = qstr.ExtractExt(entry).ToUpper(); if (Ext == "") throw new Exception($"Files without extension cannot be processed: {entry}");
                Ask($"APP.{Ext}", $"Which application should be used to show .{Ext} files?\nI need to know in order to view {entry}.\nJust a tag for the application, not yet a full line to execute", "Application");
                Ask($"EXE.{Config.C(Ext)}", $"Now I need the full line to execute the application {Config.C(Ext)}.\nPlease note I will visit the folder where the temp file is located, and you must add {'{'}file{'}'} in the line so NJCR can subsitute that with the file needed","Command line");
                QCol.Doing("Extracting", entry);
                var b = jcr.JCR_B(entry);
                var tent = qstr.StripDir(entry);
                var old = Directory.GetCurrentDirectory();
                Directory.SetCurrentDirectory(TempFolder);
                if (b == null) throw new Exception($"JCR ERROR: {JCR6.JERROR}");
                QuickStream.SaveBytes(tent,b);
                QuickStream.SaveString("NJCRSHOW.BAT",$"{Config.C($"EXE.{Config.C(Ext)}").Replace("{file}", tent)}");
                // Start the child process.
                QCol.Doing("Executing", $"{Config.C($"EXE.{Config.C(Ext)}").Replace("{file}", tent)}");
                Process p = new Process();                
                // Redirect the output stream of the child process.
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.FileName = "NJCRSHOW.BAT";
                p.Start();
                // Do not wait for the child process to exit before
                // reading to the end of its redirected stream.
                // p.WaitForExit();
                // Read the output stream first and then wait.
                string output = p.StandardOutput.ReadToEnd();
                QCol.Magenta(output);
                p.WaitForExit();
                QCol.Doing("Deleting temp", tent);
                File.Delete(tent);
                Directory.SetCurrentDirectory(old);
            } catch (Exception Mislukt) {
                QCol.QuickError(Mislukt.Message);
#if DEBUG
                QCol.Magenta($"{Mislukt.StackTrace}\n");
#endif
            }
        }
    }
}

