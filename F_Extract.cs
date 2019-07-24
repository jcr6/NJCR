using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrickyUnits;

namespace NJCR {
    class F_Extract:FeatBase {





        public override void Run(FlagParse fp) {
            if (fp.Args.Length == 1 || fp.GetBool("h")) {
                QCol.Yellow("-nx             "); QCol.Cyan("No eXternals. This means that any file imported from external JCR files will be ignored\n");
                QCol.Yellow("-ow             "); QCol.Cyan("Overwrite existing files\n");
                QCol.Magenta("\n\nWhat is important to note is that JCR6 was never set up as a real archiver like ZIP, RAR and 7z.\nIt has therefore features that ZIP, RAR, 7z nor any other archiver has.\n\nIt also lacks features the others have.\n\nExtracting was never a full feature of JCR6, but was rather added for completeness sake.\nExtracting files from it can therefore have some funny results.\n\n");
                return;
            }
            throw new NotImplementedException();
        }

        public override void Parse(FlagParse fp) {
            fp.CrBool("h", false);
            fp.CrBool("nx", false);
            fp.CrBool("ow", false);
        }

        public F_Extract() {
            Description = "Extract from JCR file";
        }
    }
}
