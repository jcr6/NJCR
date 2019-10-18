using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UseJCR6;
using TrickyUnits;

namespace NJCR {
    class F_Verbose : FeatBase {

        enum Just { Left, Right }

        public F_Verbose() { Description = "Verboses content of a JCR resource"; }

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

        }

        void XPrint(int size, ConsoleColor col, string text, Just align = Just.Left) {
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
                        for (int i = 0; i< size-text.Length;  i++) Console.Write(" ");
                        Console.Write(text);
                        break;
                }
            }
        }
        void XPrint(int size, ConsoleColor col, int whatever, Just align = Just.Right) => XPrint(size, col, $"{whatever}", align);
        void XPrint(int size, ConsoleColor col, object whatever, Just align = Just.Left) => XPrint(size, col, $"{whatever}", align);

        void WhiteSpace(int size) {
            for (int i = 0; i < size; i++) Console.Write(" ");
        }

    }
}
