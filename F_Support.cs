// Lic:
// NJCR F_Support
// .
// 
// 
// 
// (c) Jeroen P. Broks, 2020
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
// Version: 20.11.10
// EndLic
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UseJCR6;
using TrickyUnits;

namespace NJCR {
    class F_Support:FeatBase {
        public F_Support() {
            Description = "List of all supported file types and compression algorithms";
        }

        public override void Parse(FlagParse fp) {
            
        }

        public override void Run(FlagParse fp) {
            QCol.Magenta("Supported compression algoritms\n");
            foreach (string k in JCR6.CompDrivers.Keys) {
                QCol.Red("= ");
                QCol.Cyan($"{k}\n");
            }
            QCol.Magenta("Resource file readers supported\n");
            foreach (string k in JCR6.FileDrivers.Keys) {
                QCol.Red("= ");
                QCol.Cyan($"{k}\n");
            }
        }

    }
}