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
