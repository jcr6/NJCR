// Lic:
// JCR6
// Verbose
// 
// 
// 
// (c) Jeroen P. Broks, 2019, 2021
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
// Version: 21.07.05
// EndLic

using System;
using UseJCR6;
using TrickyUnits;

namespace NJCR {
    class F_Verbose : FeatBase {

        enum Just { Left, Right }
        TMap<string, string> FTypes = new TMap<string, string>();

        public F_Verbose() {
            Description = "Verboses content of a JCR resource";
            FTypes["png"] = "Image";
            FTypes["jpg"] = "Image";
            FTypes["jpeg"] = "Image";
            FTypes["gif"] = "Image";
            FTypes["txt"] = "Text";
            FTypes["md"] = "Markdown";
            FTypes["html"] = "HTML file";
            FTypes["htm"] = "HTML file";
            FTypes["js"] = "Script";
            FTypes["saskia"] = "Script";
            FTypes["lua"] = "Script";
            FTypes["nil"] = "Script";
            FTypes["neil"] = "Script";
            FTypes["py"] = "Script";
            FTypes["wav"] = "Audio";
            FTypes["mp3"] = "Audio";
            FTypes["ogg"] = "Audio";
            FTypes["voc"] = "Audio";
            FTypes["gini"] = "Config";
            FTypes["ini"] = "Config";
            FTypes["ttf"] = "Font";
            FTypes["mydata"] = "Database";
            FTypes["bubbleproject"] = "Project";
            FTypes["ps1"] = "PowerShell";
            FTypes["exe"] = "Executable";
            FTypes["dll"] = "Dyn. Lnk. Lib";
            FTypes["dylib"] = "Dyn. Lnk. Lib";
            FTypes["so"] = "Shared Object";
            FTypes["xml"] = "Ext. Mrkp Lng";
        }

        public override void Parse(FlagParse fp) {
            fp.CrBool("x", false); // so extended stuff too like Author and Notes
            fp.CrBool("a", false); // show aliases
            fp.CrBool("xd", false); // Show all data
        }

        public override void Run(FlagParse fp) {
            var ShowXStuff = fp.GetBool("x");
            var ShowAlias = fp.GetBool("a");
            var ShowAllDat = fp.GetBool("xd");
            if (fp.Args.Length==1) {
                QCol.Green("Verboses the files in a JCR resource:\n\n");
                QCol.Yellow("-x              "); QCol.Cyan("Show notes and author (if available)\n");
                QCol.Yellow("-a              "); QCol.Cyan("List out all aliases");
                QCol.Yellow("-xd             "); QCol.Cyan("Show all entry variable settings");
                return;
            }
            if (fp.Args.Length > 2) {
                QCol.QuickError("Only ONE file please!");
                return;
            }
            var jcr = JCR6.Dir(fp.Args[1]);
            if (jcr==null) { QCol.QuickError(JCR6.JERROR); return; }
            { // Resources
                var ResCount = new TMap<string, int>();
                var CmpCount = new TMap<string, int>();
                foreach (TJCREntry ent in jcr.Entries.Values) {
                    ResCount[ent.MainFile]++;
                    CmpCount[ent.Storage]++;
                }
                XPrint(15, ConsoleColor.White, "Type");
                XPrint(9, ConsoleColor.White, "Entries",Just.Right);
                XPrint(10, ConsoleColor.White, " Resource:"); Console.WriteLine();
                XPrint(15, ConsoleColor.White, "====");
                XPrint(9, ConsoleColor.White, " =======",Just.Right);
                XPrint(10, ConsoleColor.White, " ========="); Console.WriteLine();
                foreach(string k in ResCount.Keys) {
                    var rec = JCR6.Recognize(k);
                    if (rec != "NONE") {
                        XPrint(15, ConsoleColor.Blue, JCR6.FileDrivers[rec].name);
                        XPrint(9, ConsoleColor.Cyan, ResCount[k]);
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine($" {k}");
                    }
                }
                Console.WriteLine();
                XPrint(20, ConsoleColor.White, "Storage Method");
                XPrint(10, ConsoleColor.White, "Used",Just.Right); Console.WriteLine();
                XPrint(20, ConsoleColor.White, "==============");
                XPrint(10, ConsoleColor.White, "====",Just.Right); Console.WriteLine();
                foreach (string k in CmpCount.Keys) {
                    XPrint(20, ConsoleColor.Blue, k);
                    XPrint(10, ConsoleColor.Cyan, CmpCount[k],Just.Right);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine();
                }
            }
            Console.WriteLine();
            // Comments
            foreach (string name in jcr.Comments.Keys) {
                QCol.White($"{name}\n");
                for (int i = 0; i < name.Length; i++) QCol.White("=");
                QCol.Yellow($"\n{jcr.Comments[name]}\n\n");
            }
            // Blocks (if any)
            if (jcr.Blocks.Count>0) {
                Console.WriteLine();
                XPrint(5, ConsoleColor.White, "Block",Just.Right); WhiteSpace(2);
                XPrint(10, ConsoleColor.White, "Compressed", Just.Right); WhiteSpace(2);
                XPrint(10, ConsoleColor.White, "Size", Just.Right); WhiteSpace(2);
                XPrint(5, ConsoleColor.White, "Ratio", Just.Right); WhiteSpace(2);
                XPrint(7, ConsoleColor.White, "Storage"); Console.WriteLine();
                XPrint(5, ConsoleColor.White, "=====", Just.Right); WhiteSpace(2);
                XPrint(10, ConsoleColor.White, "==========", Just.Right); WhiteSpace(2);
                XPrint(10, ConsoleColor.White, "====", Just.Right); WhiteSpace(2);
                XPrint(5, ConsoleColor.White, "=====", Just.Right); WhiteSpace(2);
                XPrint(7, ConsoleColor.White, "======="); Console.WriteLine();
                foreach (var B in jcr.Blocks.Values) {
                    XPrint(5, ConsoleColor.Blue, B.ID); WhiteSpace(2);
                    XPrint(10, ConsoleColor.Green, B.CompressedSize); WhiteSpace(2);
                    XPrint(10, ConsoleColor.Red, B.Size); WhiteSpace(2);
                    XPrint(5, ConsoleColor.Magenta, $"{B.Ratio}%", Just.Right); WhiteSpace(2);
                    XPrint(7, ConsoleColor.Yellow, B.Storage); Console.WriteLine();
                }

            }
            // Entries
            Console.WriteLine();
            XPrint(15, ConsoleColor.White, "Kind"); WhiteSpace(2);
            XPrint(10, ConsoleColor.White, "Compressed", Just.Right); WhiteSpace(2);
            XPrint(10, ConsoleColor.White, "Size", Just.Right); WhiteSpace(2);
            XPrint(5, ConsoleColor.White, "Ratio", Just.Right); WhiteSpace(2);
            XPrint(7, ConsoleColor.White, "Storage"); WhiteSpace(2);
            Console.WriteLine("Entry");
            XPrint(15, ConsoleColor.White, "===="); WhiteSpace(2);
            XPrint(10, ConsoleColor.White, "==========", Just.Right); WhiteSpace(2);
            XPrint(10, ConsoleColor.White, "====", Just.Right); WhiteSpace(2);
            XPrint(5, ConsoleColor.White, "=====", Just.Right); WhiteSpace(2);
            XPrint(7, ConsoleColor.White, "======="); WhiteSpace(2);
            Console.WriteLine("=====");
            foreach (TJCREntry ent in jcr.Entries.Values) {
                Console.BackgroundColor = ConsoleColor.Black;
                if (ent.MainFile != fp.Args                    [1].Replace("\\", "/")) Console.BackgroundColor = ConsoleColor.DarkBlue;
                XPrint(15, ConsoleColor.Blue, FTypes[qstr.ExtractExt(ent.Entry).ToLower()]); WhiteSpace(2);
                if (ent.Block == 0)
                    XPrint(10, ConsoleColor.Green, ent.CompressedSize);
                else
                    XPrint(10, ConsoleColor.DarkGreen, $"Block: {ent.Block}");
                WhiteSpace(2);
                XPrint(10, ConsoleColor.Red, ent.Size); WhiteSpace(2);
                if (ent.Block == 0) XPrint(5, ConsoleColor.Magenta, ent.Ratio, Just.Right); else XPrint(5,ConsoleColor.Magenta, ""); WhiteSpace(2);
                XPrint(7, ConsoleColor.Yellow, ent.Storage); WhiteSpace(2);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(ent.Entry);
                if (ShowXStuff) {
                    if (ent.Author != "") QCol.Doing("\tAuthor", ent.Author);
                    if (ent.Notes != "") QCol.Green($"{ent.Notes}\n");
                }
                if (ShowAlias) {
                    foreach (string AlFile in jcr.Aliases(ent)) QCol.Doing("\tAlias", AlFile);
                }
                if (ShowAllDat) {
                    foreach(string k in ent.databool.Keys) {
                        QCol.Magenta("\tbool   "); QCol.Yellow(k); QCol.White(" = "); if (ent.databool[k]) QCol.Green("True\n"); else QCol.Red("False\n");
                    }
                    foreach (string k in ent.datastring.Keys) {
                        QCol.Magenta("\tstring "); QCol.Yellow(k); QCol.White(" = "); QCol.Green($"\"{ent.datastring[k]}\"\n");
                    }
                    foreach (string k in ent.dataint.Keys) {
                        QCol.Magenta("\tint    "); QCol.Yellow(k); QCol.White(" = "); QCol.Cyan($"{ent.dataint[k]}\n");
                    }

                }
            }
        }

        void XPrint(int size, ConsoleColor col, string text, Just align = Just.Left) {
            try {
                if (text == null) text = "";
                var x = Console.CursorLeft;
                Console.ForegroundColor = col;
                if (text.Length > size) {
                    Console.WriteLine(text);
                    while (Console.CursorLeft < x + size) Console.Write(" ");
                } else {
                    switch (align) {
                        case Just.Left:
                            Console.Write(text);
                            for (int i = text.Length; i < size; i++) Console.Write(" ");
                            break;
                        case Just.Right:
                            for (int i = 0; i < size - text.Length; i++) Console.Write(" ");
                            Console.Write(text);
                            break;
                    }
                }
            } catch (Exception crap) {
                QCol.QuickError(crap.Message);
                QCol.Magenta(crap.StackTrace);
                Console.Write("\n\n");
            }
        }
        void XPrint(int size, ConsoleColor col, int whatever, Just align = Just.Right) => XPrint(size, col, $"{whatever}", align);
        void XPrint(int size, ConsoleColor col, object whatever, Just align = Just.Left) => XPrint(size, col, $"{whatever}", align);

        void WhiteSpace(int size) {
            for (int i = 0; i < size; i++) Console.Write(" ");
        }

    }
}