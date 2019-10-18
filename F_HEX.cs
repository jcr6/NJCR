// Lic:
// NJCR
// Lelijke Heks!
// 
// 
// 
// (c) nano license.mkl.gini, 
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
// Version: 19.10.18
// EndLic
using System;
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



