// Lic:
// NJCR
// Show
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
// Version: 19.10.19
// EndLic

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UseJCR6;
using TrickyUnits;

namespace NJCR {
    class F_Show:FeatBase {

        readonly string ShowConfig = Dirry.C("$AppSupport$/NJCR_Show.GINI");

        public override void Run(FlagParse fp) {
            if (fp.Args.Length < 2) {
                QCol.Green("This feature allows you to show the content of a certain file.\n");
                QCol.Green("In order to do this, some additional configuration will be needed, as well as knowledge of the file system you are using.\n");
                QCol.Green("Don't worry about having to set up this file, manually. You will be prompted whenever NJCR requires more information.\n");
                QCol.Green($"The configuration can lateron be edited with your favority text editor, as it will be saved as {ShowConfig}");
                return;
            }
            var jcr = JCR6.Dir(fp.Args[1]);
            if (jcr==null) { QCol.QuickError(JCR6.JERROR); return; }
            for (int i = 2; i < fp.Args.Length; i++) Show(jcr, fp.Args[i]);
        }


        public override void Parse(FlagParse fp) {            
        }

        void Show(TJCRDIR jcr,string entry) { }
    }
}

