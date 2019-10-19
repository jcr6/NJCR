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
// Version: 19.10.18
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


        /// <summary>
        /// Removes the first entry since we don't need that at all!
        /// </summary>
        /// <param name="fp"></param>
        /// <returns></returns>
        protected string[] NonFlags(FlagParse fp,int start=1) {
            var ret = new List<string>();
            for (int i = start; i < fp.Args.Length; i++) ret.Add(fp.Args[i]);
            return ret.ToArray();            
        }

        protected string[] Files(FlagParse fp) => NonFlags(fp,2);
        protected string JCRFile(FlagParse fp) {
            var tmp = NonFlags(fp, 1);
            if (tmp.Length<1) {
                QCol.QuickError("No JCR file present");
                Environment.Exit(2);                
            }
            return tmp[0];
        }
        
    }

    class NJCR {

        static SortedDictionary<string, FeatBase> Features = new SortedDictionary<string, FeatBase>();
        static public void Register(string name,FeatBase feature) {
            Features[name] = feature;
        }

        static void Init() {
            MKL.Lic    ("NJCR - NJCR.cs","GNU General Public License 3");
            MKL.Version("NJCR - NJCR.cs","19.10.18");
            JCR6_lzma.Init();
            JCR6_zlib.Init();
            JCR6_jxsrcca.Init();
            JCR_JCR5.Init();
            new JCR6_WAD();
            new JCR_QuakePack();
            Dirry.InitAltDrives();
            Register("ADD", new F_Add());
            Register("DELETE", new F_Delete());
            Register("EXTRACT", new F_Extract());
            Register("TYPE", new F_Type());
            Register("HEX", new F_HEX());
            {
                var V = new F_Verbose();
                Register("VIEW", V);
                Register("LIST", V);
                Register("VERBOSE", V);
            }
            Register("SHOW", new F_Show());
            //Register("QUHELP", new F_QU_Help());
            QCol.DoingTab = 20;
        }

        static void Head() {
            QCol.Yellow("NJCR ");
            QCol.Cyan($"{MKL.Newest}");
#if DEBUG
            QCol.Red("\tDEBUG BUILD!");
#endif
            Console.WriteLine();
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
                return;
            }
            var cmd = args[0].ToUpper();
            if (!Features.ContainsKey(cmd)) {
                QCol.QuickError($"Command '{cmd}' has not been understood");
                return;
            }
            Features[cmd].Parse(f);
            if (!f.Parse()) {
                QCol.QuickError($"Bad command line input");
                return;
            }
            Features[cmd].Run(f);
        }        

        static void Main(string[] args) {
            Init();
            Head();
            Run(args);
            TrickyDebug.AttachWait();
        }
    }
}




