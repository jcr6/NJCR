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
