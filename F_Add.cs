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
// Version: 19.10.04
// EndLic

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using TrickyUnits;
using UseJCR6;

namespace NJCR {

    class Fil2Add {
        public string source ="";
        public string target ="";
        public string storage = "";
        public string author = "";
        public string notes = "";
        public string TARGET => target.ToUpper();
        public Dictionary<string, bool> xBool = new Dictionary<string, bool>();
        public Dictionary<string, int> xInt = new Dictionary<string, int>();
        public Dictionary<string, string> xString = new Dictionary<string, string>();
        
        public Fil2Add(string asource,string atarget,string astorage,string aauthor, string anotes, Dictionary<string,bool> axBool=null, Dictionary<string,int> axInt=null,Dictionary<string,string> axString = null) {
            source = asource.Replace("\\", "/");
            target = atarget.Replace("\\", "/");
            storage = astorage;
            author = aauthor;
            notes = anotes;
            if (axBool != null) xBool = axBool;
            if (axInt != null) xInt = axInt;
            if (axString != null) xString = axString;
        }
    }
    class F_Add : FeatBase {

        public static F_Add X; // Used to allow the scripter and the converter to run the adding routine internally.
        bool quick = false;
        bool updating = false;
        string jcrfile = "";
        string[] toadd = null;
        string filetablecompression = "";
        string compressionmethod = "";
        string sig = "";
        bool nomerge = false;
        bool puremerge = false;
        List<Fil2Add> Jiffy = new List<Fil2Add>();

        void ParseJIF(string jif) {
            throw new Exception("JIF not yet implemented");
        }

        void AfterAdd(TJCREntry E,Fil2Add aFile) {
            QCol.Doing("Configuring", "", "\r");
            var deferrors = new List<string>();
            foreach (string vars in aFile.xBool.Keys) {
                if (vars[0] == '_')
                    deferrors.Add($"Cannot define protected boolean variable {vars}");
                else
                    E.databool[vars] = aFile.xBool[vars];
            }
            foreach (string vars in aFile.xInt.Keys) {
                if (vars[0] == '_')
                    deferrors.Add($"Cannot define protected integer variable {vars}");
                else
                    E.dataint[vars] = aFile.xInt[vars];
            }
            foreach (string vars in aFile.xString.Keys) {
                if (vars[0] == '_')
                    deferrors.Add($"Cannot define protected string variable {vars}");
                else
                    E.datastring[vars] = aFile.xString[vars];
            }
            if (E.Storage == "Store") {
                //          12345
                QCol.White("     Stored:\n");
            } else {
                var deel = (decimal)E.CompressedSize;
                var geheel = (decimal)E.Size;
                var pureratio = deel / geheel;
                QCol.Blue($"{qstr.Right($"   {(int)(pureratio * 100)}", 3)}% ");
                QCol.Green($"Packed: {E.Storage}\n");
            }
            if (deferrors.Count > 0) {
                QCol.QuickError("Although the file has been succesfully added, there are configuration errors!");
                foreach(string de in deferrors) {
                    QCol.Red("\t=> "); QCol.Yellow(de);
                }
            }

        }

    public void Go() {
            // Create or update?
            TJCRCreate jout;
            var temp = $"{qstr.ExtractDir(jcrfile)}/{qstr.md5($"{jcrfile}.{DateTime.Now.ToString()}")}.$jcr";
            TJCRDIR jtmp = null;
            if (updating) {
                jout = new TJCRCreate(temp, filetablecompression, sig);
            } else {
                jout = new TJCRCreate(jcrfile, filetablecompression, sig);
            }

            // Add Comments
            // TODO: Add comments

            // Add files
            foreach(Fil2Add aFile in Jiffy) {
                try {                    
                    if (nomerge || JCR6.Recognize(aFile.source) == "NONE") {
                        QCol.Doing("Adding", aFile.source, "\r");
                        jout.AddFile(aFile.source, aFile.target, aFile.storage, aFile.author, aFile.notes);
                        var E = jout.Entries[aFile.TARGET];
                        AfterAdd(E, aFile);
                    } else {
                        QCol.Doing("Merging", aFile.source);
                        var merge = JCR6.Dir(aFile.source);
                        foreach (TJCREntry ent in merge.Entries.Values) {
                            QCol.Doing("Adding", "", "");
                            QCol.Magenta($"{aFile.source}/");
                            QCol.Cyan($"{ent.Entry}\r");
                            var tar = $"{aFile.target}/{ent.Entry}";
                            if (puremerge)
                                jout.JCRCopy(merge, ent.Entry, tar);
                            else {
                                var buf = merge.JCR_B(ent.Entry);
                                jout.AddBytes(buf, tar, aFile.storage, ent.Author, ent.Notes);
                            }
                            var E = jout.Entries[tar.ToUpper()];
                            AfterAdd(E, aFile);
                        }
                    }
                } catch (Exception crap) {
                    QCol.Red("     Failed:\n");
                    if (JCR6.JERROR != "") QCol.QuickError($"JCR6: {JCR6.JERROR}");
                    QCol.QuickError($".NET: {crap.Message}");
                }
            }

            // Process aliases
            // TODO: Aliases

            // Reorganize Files
            if (updating) {
                try {
                    QCol.Doing("Reorganizing", "Data");
                    jtmp = JCR6.Dir(jcrfile);
                    if (jtmp == null) throw new Exception($"JCR failed to analyse the old archive: {JCR6.JERROR}");
                    var indicator = 0;
                    foreach (TJCREntry entry in jtmp.Entries.Values) {
                        indicator++;
                        if (indicator % 5 == 0) QCol.Blue("\r                \r.\b"); else QCol.Blue(".\b");
                        if (!jout.Entries.ContainsKey(entry.Entry.ToUpper())) {
                            QCol.Green("O");
                            jout.JCRCopy(jtmp, entry.Entry);
                        } else {
                            QCol.Red("X");
                        }
                    }
                    Console.WriteLine();
                } catch (Exception well) {
                    QCol.QuickError(well.Message);
                }
            }

            // Add dependency requests
            // TODO: Dependency requests

            // Closure and overwrite old JCR file if applicable.
            QCol.Doing("Finalizing", jcrfile);
            jout.Close();
            if (updating) {
                File.Delete(jcrfile);
                File.Move(temp, jcrfile);
            }
        }

            public void Add2List(string ori, string tar, string sto, string aut, string notes) {
            if (File.Exists(ori)) {
                Jiffy.Add(new Fil2Add(ori, tar, sto, aut, notes));
            } else if (Directory.Exists(ori)) {
                var tree = FileList.GetTree(ori);
                foreach(string fil in tree) {
                    Jiffy.Add(new Fil2Add($"{ori}/{fil}", $"{tar}/{fil}", sto, aut, notes));
                }
            } else {
                QCol.QuickError($"Neither a file nor a directory named {ori} found! Will be ignored!");
            }
        }

        public F_Add() {
            Description = "Creates/Updates JCR file and add/replace files";
            X = this;
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
                QCol.Yellow("-nomerge         "); QCol.Cyan("When set, files detected as JCR6 compatible files will not be merged, but just be addd as a regular file!");
                QCol.Yellow("-puremerge       "); QCol.Cyan("When set, entries being added due to JCR-merging will not be repacked, but be directly copied");
                QCol.Yellow("                 "); QCol.Magenta("It goes without saying, but using -nomerge and -puremerge in one run is not a very clever thing to do.");
                //QCol.Yellow("-qu              "); QCol.Cyan("Quick update. (Type \"NJCR quhelp\" for more information)\n");
                QCol.Blue("\n\n\n");
                QCol.Green("JCR6 supports the next compression methods:\n");
                foreach(string name in JCR6.CompDrivers.Keys) {
                    QCol.Red("\t* "); QCol.Yellow($"{name}\n");
                }
                return;
            }
            quick = fp.GetBool("qu");
            jcrfile = fp.Args[1];
            toadd = Files(fp);
            filetablecompression = fp.GetString("fc");
            compressionmethod = fp.GetString("cm");
            puremerge = fp.GetBool("puremerge");
            nomerge = fp.GetBool("nomerge");
            if (fp.GetString("i") != "") {
                var l = new List<string>(toadd);
                l.Add(fp.GetString("i"));
                toadd = l.ToArray();
            }
            updating = File.Exists(jcrfile) && !fp.GetBool("doj");
            if (toadd.Length == 0)
                Add2List(Directory.GetCurrentDirectory(), "", compressionmethod, fp.GetString("author"), fp.GetString("notes"));
            else
                foreach (string fil in toadd) Add2List(fil, qstr.StripDir(fil), compressionmethod, fp.GetString("author"), fp.GetString("notes"));                
            if (fp.GetString("jif") != "") ParseJIF(fp.GetString("jif"));
            if (updating)
                QCol.Doing("Updating", jcrfile);
            else
                QCol.Doing("Creating", jcrfile);
            QCol.Doing("File Storage", compressionmethod);
            QCol.Doing("Table Storage", filetablecompression);
            QCol.Doing("No merge",$"{nomerge}");
            QCol.Doing("Pure merge",$"{puremerge}");
            QCol.Doing("Files", $"{Jiffy.Count}");
            QCol.Doing("NJCR", MKL.MyExe);
            QCol.Doing("PWD", Directory.GetCurrentDirectory());
            Console.WriteLine("\n\n");
            Go();
        }

        public override void Parse(FlagParse fp) {
            fp.CrBool("h", false);
            fp.CrBool("doj", false);
            fp.CrBool("nomerge", false);
            fp.CrBool("puremerge", false);
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



