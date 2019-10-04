// Lic:
// NJCR
// QU information
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrickyUnits;

namespace NJCR {
    class F_QU_Help:FeatBase {
        public F_QU_Help() {
            MKL.Lic    ("NJCR - F_QU_Help.cs","GNU General Public License 3");
            MKL.Version("NJCR - F_QU_Help.cs","19.10.04");
            Description = "Provides help about the -qu switch some features use";
        }
        public override void Parse(FlagParse fp) {        }
        public override void Run(FlagParse fp) {
            foreach (string lin in new string[] {
                "'qu' or Quick Update is a feature which can be used in features actually modifying a JCR file",
                "Normally when files get overridden by new files or if files are removed, JCR6 needs to move",
                "tons of data inside the JCR file in order to get everything right again.","",
                "When 'qu' has been set, JCR won't do that, but only unlink the removed or overridden files",
                "This means the data itself still exists, but no more pointers are set to it","",
                "This will cause the update to be much faster, downside is that since the data itself still exists,",
                "but is no longer accessible will eventually lead to ridiculously large JCR files","",
                "This feature is therefore handy for quick updates only (debugging comes to mind),","",
                "For serious archiving or for JCR files tied to games or utilites being prepared for",
                "distribution I'd strongly recommend against using the '-qu' switch"
            }) QCol.ColWrite(ConsoleColor.DarkGreen,$"\t{lin}\n");
        }
    }
}



