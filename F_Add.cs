// Lic:
// NJCR
// Add files
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
// Version: 19.07.22
// EndLic


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrickyUnits;
using UseJCR6;

namespace NJCR {
    class F_Add : FeatBase {

        public F_Add() {
            Description = "Creates/Updates JCR file and add/replace files";
        }

        public override void Run(FlagParse fp) {
            if (fp.Args.Length==1 || fp.GetBool("h" )) {
                QCol.Green("Add files to a JCR file! Available switches are:\n\n");
                QCol.Yellow("-doj             "); QCol.Cyan("Destroy original JCR file, so begin completely fresh, always\n");
                QCol.Yellow("-i               "); QCol.Cyan("Input file(s) or directory/directories. (deprecated)\n");
                QCol.Yellow("-cm <method>     "); QCol.Cyan("Compression method for the files inside the JCR file (default is lzma)\n");
                QCol.Yellow("-fc <method>     "); QCol.Cyan("Compression method for the file table inside the JCR file (default is lzma)\n");
                QCol.Yellow("-jif <file>      "); QCol.Cyan("Read a JCR Instruction File to see how files must be stored and under which conditions\n");
                QCol.Yellow("-author <author> "); QCol.Cyan("Add an author to the files added (jif files ignore this flag)\n");
                QCol.Yellow("-notes <notes>   "); QCol.Cyan("Add notes to the files added (jif files ignore this flag)\n");
                QCol.Yellow("-qu              "); QCol.Cyan("Quick update. (Type \"NJCR quhelp\" for more information)\n");
                QCol.Blue("\n\n\n");
                QCol.Green("JCR6 supports the next compression methods:\n");
                foreach(string name in JCR6.CompDrivers.Keys) {
                    QCol.Red("\t* "); QCol.Yellow($"{name}\n");
                }
                return;
            }
            throw new NotImplementedException();
        }

        public override void Parse(FlagParse fp) {
            fp.CrBool("h", false);
            fp.CrBool("doj", false);
            fp.CrString("i", ""); // This feature will be declared "deprecated" from the start, but has only been put in to prevent confusion with the Go version
            fp.CrString("cm", "lzma");
            fp.CrString("fc", "lzma");
            fp.CrString("jif", "");
            fp.CrString("author", "");
            fp.CrString("notes");
            fp.CrBool("qu", false);
        }
    }
}


