using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrickyUnits;

namespace NJCR {
    class F_Verbose : FeatBase {

        enum Just { Left, Right }

        public F_Verbose() { Description = "Verboses content of a JCR resource"; }

        public override void Parse(FlagParse fp) {
            fp.CrBool("x", false); // so extended stuff too like Author and Notes
            fp.CrBool("a", false); // show aliases
            fp.CrBool("xd", false); // Show all data
        }

        public override void Run(FlagParse fp) {
            var ShowXStuff = fp.GetBool("x");
            var ShowAlias = fp.GetBool("a");
            var ShowAlLDat = fp.GetBool("xd");
        }

        void XPrint(int size, ConsoleColor col, string text, Just align = Just.Left) {
            var x = Console.CursorLeft;
            Console.ForegroundColor = col;
            if (text.Length > size) {
                Console.WriteLine(text);
                while (Console.CursorLeft < x + size) Console.Write(" ");
            } else {
                switch (align) {
                    case Just.Left:
                        Console.Write(text);
                        for (int i = text.Length; i < size; i++) Console.Write(" ");
                        break;
                    case Just.Right:
                        for (int i = 0; i< size-text.Length;  i++) Console.Write(" ");
                        Console.Write(text);
                        break;
                }
            }
        }
        void XPrint(int size, ConsoleColor col, int whatever, Just align = Just.Right) => XPrint(size, col, $"{whatever}", align);
        void XPrint(int size, ConsoleColor col, object whatever, Just align = Just.Left) => XPrint(size, col, $"{whatever}", align);

        void WhiteSpace(int size) {
            for (int i = 0; i < size; i++) Console.Write(" ");
        }

    }
}
