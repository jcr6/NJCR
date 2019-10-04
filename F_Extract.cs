// Lic:
// NJCR
// Extract
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
// Version: 19.10.04
// EndLic
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UseJCR6;
using TrickyUnits;

namespace NJCR {
    class F_Extract:FeatBase {


        enum aliashanding { Ask, Skip, Ignore }


        public override void Run(FlagParse fp) {
            if (fp.Args.Length == 1 || fp.GetBool("h")) {
                QCol.Green("Extract files from a JCR file! Available switches are:\n\n");
                QCol.Yellow("-nx             "); QCol.Cyan("No eXternals. This means that any file imported from external JCR files will be ignored\n");
                QCol.Yellow("-ow             "); QCol.Cyan("Overwrite existing files\n");
                QCol.Yellow("-nodir          "); QCol.Cyan("Remove paths from file names\n");
                QCol.Yellow("-nac            "); QCol.Cyan("No auto-creation of new directories if needed\n");
                QCol.Yellow("-output <path>  "); QCol.Cyan("Define output path\n");
                QCol.Magenta("\n\nWhat is important to note is that JCR6 was never set up as a real archiver like ZIP, RAR and 7z.\nIt has therefore features that ZIP, RAR, 7z nor any other archiver has.\n\nIt also lacks features the others have.\n\nExtracting was never a full feature of JCR6, but was rather added for completeness sake.\nExtracting files from it can therefore have some funny results.\n\n");
                return;
            }
            var nx = fp.GetBool("nx");
            var ow = fp.GetBool("ow");
            var nodir = fp.GetBool("nodir");
            var autocreate = fp.GetBool("nac");
            var outdir = fp.GetString("output");
            if (outdir != "") {
                outdir = outdir.Replace("\\", "/");
                if (!qstr.Suffixed(outdir, "/")) outdir += "/";
            }
        QCol.Doing("Reading", fp.Args[1]);
            var jcr = JCR6.Dir(fp.Args[1]);
            if (JCR6.JERROR!="") {
                QCol.QuickError($"JCR6 Error: {JCR6.JERROR}");
                return;
            }
            try {
                var skipreason = "";
                var allow = true;
                var offsets = new Dictionary<string, List<TJCREntry>>();
                var alias = aliashanding.Ignore;
                void skip(string reason) {
                    if (skipreason != "") skipreason += "; ";
                    skipreason += reason;
                    allow = false;
                }
                var shared = false;
                foreach (TJCREntry Ent in jcr.Entries.Values) {
                    var tag = $"{Ent.MainFile}:{Ent.Offset}";
                    if (!offsets.ContainsKey(tag)) offsets[tag] = new List<TJCREntry>(); else shared = true;
                    offsets[tag].Add(Ent);
                }
                if (shared) {
                    Console.Beep();
                    QCol.Red("\n\nWARNING!\n");
                    QCol.Yellow("This resource has some shared-references or aliases as they are properly called in JCR6.\n");
                    QCol.Yellow("Extracting from this file can therefore create tons of duplicate files, as JCR6 cannot tell which entry is more relevant than others.\n");
                    QCol.Yellow("It's strongly recommended to extract files from this resource unless you are sure about what you are doing and what the consequences are and how to deal with them\n");
                    QCol.Cyan("1 = Continue, but ask about any files that has aliases, what to do with them\n");
                    QCol.Cyan("2 = Continue, but skip all files with aliases\n");
                    QCol.Cyan("3 = Continue, and just let the duplicate files come, I don't fear them!\n");
                    QCol.Cyan("Q = Cancel this operation\n");
                    QCol.Magenta("What do you want to do? "); QCol.Green("");
                    {
                        var loop = true;
                        do {
                            var ch = Console.ReadKey();
                            switch (ch.KeyChar) {
                                case '1': alias = aliashanding.Ask; loop = false; break;
                                case '2': alias = aliashanding.Skip; loop = false; break;
                                case '3': alias = aliashanding.Ignore; loop = false; break;
                                case 'q': case 'Q': return;
                            }
                        } while (loop);
                        Console.WriteLine($" -- {alias}");
                    }
                }
                foreach (TJCREntry Ent in jcr.Entries.Values) {
                    allow = true;
                    skipreason = "";
                    // can we do this?
                    var source = Ent.Entry;
                    var target = Ent.Entry;
                    var tag = $"{Ent.MainFile}:{Ent.Offset}";
                    if (!JCR6.CompDrivers.ContainsKey(Ent.Storage)) skip($"Unknown compression method ({Ent.Storage})");
                    if (offsets[tag].Count > 1) {
                        switch (alias) {
                            case aliashanding.Skip: skip($"Offset reference as {offsets[tag].Count} entries, and system has been set to skip those"); break;
                            case aliashanding.Ask: {
                                    Console.Beep();
                                    QCol.Red($"Entry {Ent.Entry} has been aliased.\n");
                                    foreach(TJCREntry aliasentry in offsets[tag]) {
                                        QCol.Magenta("= "); QCol.Cyan($"{aliasentry.Entry}\n");
                                    }
                                    QCol.Green("Extract ? <Y/N> ");
                                    do {
                                        var d = Console.ReadKey().KeyChar;
                                        if (d == 'Y' || d == 'y') break;
                                        if (d == 'N' || d == 'n') { skip("User decided not to extract"); break; }
                                    } while (true);
                                    Console.Write("\r");
                                    break;
                                }
                        }
                    }
                    if (Ent.MainFile != fp.Args[1] && nx) skip("No Externals");
                    if (nodir) target = qstr.StripDir(target);
                    target = $"{outdir}{target}";
                    if (!Directory.Exists(qstr.ExtractDir(target)) && !autocreate) {
                        Console.Beep();
                        QCol.Red($"{target}! ");
                        QCol.Yellow($"Create directory {qstr.ExtractDir(target)} ? <Y/N> ");
                        do {
                            var d = Console.ReadKey().KeyChar;
                            if (d == 'Y' || d == 'y') break;
                            if (d == 'N' || d == 'n') { skip("User decided not to extract"); break; }
                        } while (true);
                        Console.Write("\n");                        
                    }
                    if (File.Exists(target) && (!ow)) {
                        Console.Beep();
                        QCol.Red($"{target} exists! ");
                        QCol.Yellow($"Overwrite ? <Y/N> ");
                        do {
                            var d = Console.ReadKey().KeyChar;
                            if (d == 'Y' || d == 'y') break;
                            if (d == 'N' || d == 'n') { skip("User decided not to extract"); break; }
                        } while (true);
                        Console.Write("\n");
                    }                
                if (allow) {
                        QCol.Doing("Extracting", target, "\r");
                        Directory.CreateDirectory(qstr.ExtractDir(target));
                        var b = jcr.JCR_B(source);
                        QuickStream.SaveBytes(target, b);
                        QCol.Doing(" Extracted", target);
                    } else {
                        QCol.Doing("   Skipped", target, "\t"); QCol.Red($"{skipreason}\n");
                    }
                }
            } catch (Exception e) {
                QCol.QuickError($".NET Error: {e.Message}");
#if DEBUG
                QCol.Magenta(e.StackTrace);
#endif
            }
        }

        public override void Parse(FlagParse fp) {
            fp.CrBool("h", false);
            fp.CrBool("nx", false);
            fp.CrBool("ow", false);
            fp.CrBool("nodir", false);
            fp.CrBool("nac", true);
            fp.CrString("output", "");
        }

        public F_Extract() {
            Description = "Extract from JCR file";
        }
    }
}



