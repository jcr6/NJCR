using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            FTypes["py"] = "Script";
            FTypes["wav"] = "Audio";
            FTypes["mp3"] = "Audio";
            FTypes["ogg"] = "Audio";
            FTypes["gini"] = "Config";
            FTypes["ini"] = "Config";
        }

        public override void Parse(FlagParse fp) {
            fp.CrBool("x", false); // so extended stuff too like Author and Notes
            fp.CrBool("a", false); // show aliases
            fp.CrBool("xd", false); // Show all data
        }

        public override void Run(FlagParse fp) {
            var ShowXStuff = fp.GetBool("x");
            var ShowAlias = fp.GetBool("a");
            var ShowAlLDat = fp.GetBool("xd");
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
                    XPrint(15, ConsoleColor.Blue, JCR6.FileDrivers[rec].name);
                    XPrint(9, ConsoleColor.Cyan, ResCount[k]);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($" {k}");
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
            // Comments
            foreach(string name in jcr.Comments.Keys) {
                QCol.White($"{name}\n");
                for (int i = 0; i < name.Length; i++) QCol.White("=");
                QCol.Yellow($"\n{jcr.Comments[name]}\n\n");
            }

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
                XPrint(10, ConsoleColor.Green, ent.CompressedSize); WhiteSpace(2);
                XPrint(10, ConsoleColor.Red, ent.Size); WhiteSpace(2);
                XPrint(5, ConsoleColor.Magenta, ent.Ratio, Just.Right); WhiteSpace(2);
                XPrint(7, ConsoleColor.Yellow, ent.Storage); WhiteSpace(2);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(ent.Entry);
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
