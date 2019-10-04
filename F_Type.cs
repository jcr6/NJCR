using System;
using System.Collections.Generic;
using System.Text;
using Kitty;
using UseJCR6;
using TrickyUnits;

namespace NJCR {
    class F_Type:FeatBase {

        public F_Type() { Description = "Display content of JCR entry as text"; }

        public override void Parse(FlagParse fp) {
            fp.CrBool("b", false);
            fp.CrBool("nsh", false);
            fp.CrBool("nln", false);
        }

        void Show(TJCRDIR jcr, string entry,FlagParse fp) {
            QCol.Doing("Reading", entry);
            var kind = "OTHER";
            var highlight = !fp.GetBool("nsh");
            var linenums = !fp.GetBool("nln");
            var istext = !fp.GetBool("b");
            var src = jcr.LoadString(entry);
            if (JCR6.JERROR != "") {
                QCol.QuickError(JCR6.JERROR);
                return;
            }
            if (istext) {
                var i = src.IndexOf((char)26);
                if (i >= 0) src = src.Substring(0, i);
            }
            if (highlight) {
                var ext = qstr.ExtractExt(entry).ToLower();
                if (KittyHigh.Langs.ContainsKey(ext))
                    kind = ext;
                else if (ext == "bubbleproject")
                    kind = "gini";
            }
            var Viewer = KittyHigh.Langs[kind];
            QCol.Doing("Language", Viewer.Language);
            Console.WriteLine();
            Viewer.Show(src, linenums);
        }

        public override void Run(FlagParse fp) {
            if (fp.Args.Length == 1) {
                QCol.Green("Show content of an entry as text on the console:\n\n");
                QCol.Yellow("-b              "); QCol.Cyan("When not set the ^Z character will be seen as EOF (DOS setting still working in Windows). When -b is set, everything will always be shown\n");
                QCol.Yellow("-nsh            "); QCol.Cyan("When set JCR6 will NOT try to syntax highlight");
                QCol.Yellow("-nln            "); QCol.Cyan("When set line numbers will not be displayed");
                return;
            }
            KittyHigh.Init();
            new KittyHighCS();
            new KittyHighGINI();
            new KittyHighLua();
            new KittyHighNIL();
            var jcr = JCR6.Dir(fp.Args[1]);
            if (jcr==null) {
                QCol.QuickError(JCR6.JERROR);
                return;
            }
            if (fp.Args.Length == 2) {
                foreach (TJCREntry ent in jcr.Entries.Values) {
                    Show(jcr, ent.Entry, fp);
                }
            } else {
                for (int i = 2; i < fp.Args.Length; i++) Show(jcr, fp.Args[i], fp);
            }
        }

    }
}
