// Lic:
// NJCR
// Main
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
// Version: 19.07.21
// EndLic

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrickyUnits;
using UseJCR6;

namespace NJCR {

    abstract class FeatBase {
        public string Description { get; protected set; }
        abstract public void Run(FlagParse fp);
        abstract public void Parse(FlagParse fp);
    }

    class NJCR {

        static SortedDictionary<string, FeatBase> Features = new SortedDictionary<string, FeatBase>();
        static public void Register(string name,FeatBase feature) {
            Features[name] = feature;
        }

        static void Init() {
            MKL.Lic    ("NJCR - NJCR.cs","GNU General Public License 3");
            MKL.Version("NJCR - NJCR.cs","19.07.21");
            JCR6_lzma.Init();
            JCR6_zlib.Init();
            JCR6_jxsrcca.Init();
            Dirry.InitAltDrives();
            Register("ADD", new F_Add());
        }

        static void Head() {
            QCol.Yellow("NJCR ");
            QCol.Cyan($"{MKL.Newest}\n");
            QCol.Magenta($"(c) Copyright Jeroen P. Broks {MKL.CYear(2019)}\n");
            QCol.Green("Released under the terms of the GPL3\n\n");
        }

        static void Run(string[] args) {
            var f = new FlagParse(args);
            if (args.Length == 0) {
                QCol.White("\tUsage: ");
                QCol.Cyan($"{qstr.StripDir(MKL.MyExe)} ");
                QCol.Green("<command> ");
                QCol.Yellow("<parameters>\n\n");
                var tabx = 5;
                foreach (string k in Features.Keys) { if (k.Length > tabx) tabx = k.Length; }
                foreach (string k in Features.Keys) {
                    QCol.Green(k);
                    for (int i = k.Length; i < tabx + 2; ++i) Console.Write(" ");
                    QCol.Yellow($"{Features[k].Description}\n");
                }
                
            }
        }        

        static void Main(string[] args) {
            Init();
            Head();
            Run(args);
            TrickyDebug.AttachWait();
        }
    }
}

