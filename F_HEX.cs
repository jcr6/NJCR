using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TrickyUnits;
using UseJCR6;

namespace NJCR {
    class F_HEX : FeatBase {

        public F_HEX() {
            Description = "Shows JCR6 entries in HEX codes";
        }

        void Show(TJCRDIR jcr,string entry) {
            QCol.Doing("Reading", entry);
            var b = jcr.JCR_B(entry);
            var c = ConsoleColor.Black;
            var stuff = "";
            if (b == null) { QCol.QuickError(JCR6.JERROR); return; }
            QCol.Green("................ 00 01 02 03  04 05 06 07  08 09 0A 0B  0C 0D 0E 0F");
            for (int i = 0; i < b.Length; i++) {
                switch (i % 16) {
                    case 0x00:
                        QCol.ColWrite(ConsoleColor.Yellow,$"  {stuff}\n"); stuff = "";
                        QCol.ColWrite(ConsoleColor.DarkGray, $"{i.ToString("X16")} ");
                        c = ConsoleColor.Blue;
                        break;
                    case 0x04:
                    case 0x0C:
                        c = ConsoleColor.Cyan;
                        Console.Write(" ");
                        break;
                    case 0x08:
                        c = ConsoleColor.Blue;
                        Console.Write(" ");
                        break;
                }
                if (b[i] > 31 && b[i] < 0x80) stuff += $"{(char)b[i]}"; else stuff += ".";
                QCol.ColWrite(c, $"{b[i].ToString("X2")} ");
            }
            if (b.Length % 16 != 0) {
                for (int i = b.Length % 16; i < 16; i++) {
                    switch (i) {
                        case 0x04:
                        case 0x0C:
                        case 0x08:
                            Console.Write(" ");
                            break;
                    }
                    Console.Write("   ");
                }
            }
            QCol.ColWrite(ConsoleColor.Yellow, $"  {stuff}\n");
            Console.WriteLine("\n");
        }

        public override void Parse(FlagParse fp) {            
        }

        public override void Run(FlagParse fp) {
            if (fp.Args.Length == 1) {
                QCol.Green("Displays a HEX output of a JCR entry");
                return;
            }
            QCol.Doing("Reading", fp.Args[1]);
            var jcr = JCR6.Dir(fp.Args[1]);
            if (jcr == null) { QCol.QuickError(JCR6.JERROR); return; }
            if (fp.Args.Length==2) {
                foreach (TJCREntry file in jcr.Entries.Values) Show(jcr,file.Entry);
            } else {
                for (int i = 2; i < fp.Args.Length; ++i) Show(jcr, fp.Args[i]);
            }
        }
    }
}
