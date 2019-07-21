using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrickyUnits;
using UseJCR6;

namespace NJCR {

    abstract class FeatBase {
        abstract public void Run(FlagParse fp);
    }

    class NJCR {

        static void Init() {
            MKL.Lic("", "");
            MKL.Version("", "");
            JCR6_lzma.Init();
            JCR6_zlib.Init();
            JCR6_jxsrcca.Init();
            Dirry.InitAltDrives();
        }

        static void Head() {
            QCol.Yellow("NJCR ");
            QCol.Cyan($"{MKL.Newest}\n");
            QCol.Magenta($"(c) Copyright Jeroen P. Broks {MKL.CYear(2019)}\n");
            QCol.Green("Released under the terms of the GPL3\n\n");
        }

        static void Main(string[] args) {
            Init();
            Head();
            TrickyDebug.AttachWait();

        }
    }
}
