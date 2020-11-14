// Lic:
// NJCR
// Scripting engine
// 
// 
// 
// (c) Jeroen P. Broks, 2019
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
using System.IO;
using System.Text;
using System.Threading.Tasks;
using TrickyUnits;
using NLua;

namespace NJCR {

    class ScriptAPI {

        static F_Add Adder => F_Add.X;
        public bool Verbose = false;

        void V(string m) { if (Verbose) QCol.Magenta($"{m}\n"); }

        public int ArgCount => F_Script.ScriptArgs.Length;
        public string Arg(int i) {
            if (i < 0 || i >= ArgCount) throw new Exception("Invalid arg index");
            return F_Script.ScriptArgs[i];
        }

        public string OutputFile { set { F_Script.JLS_Data_Output = value; Adder.jcrfile = value; } get => F_Script.JLS_Data_Output; }
        public void Output(string arg) {
            var s = arg.Split(':');
            if (s.Length != 2) throw new Exception("Invalid JLS.Output string");
            F_Script.JLS_Data[s[0]] = s[1];
        }

        private TMap<string, string> AddMap = null;
        public void SetAdd(string key,string value) {
            try {
                if (key != "Start" && AddMap == null) throw new Exception("Cannot add data to add request when no request has been opened!");
                switch (key) {
                    case "Start":
                        AddMap = new TMap<string, string>();
                        break;
                    case "Close":
                        if (AddMap["Target"] == "") AddMap["Target"] = qstr.StripDir(AddMap["Source"]);
                        if (AddMap["Source"] == "") throw new Exception("Cannot add without source");
                        if (AddMap["Storage"] == "") AddMap["Storage"] = "Store";
                        if (!UseJCR6.JCR6.CompDrivers.ContainsKey(AddMap["Storage"])) throw new Exception($"Unknown compression method requested {AddMap["Storage"]}");
                        if (File.Exists(AddMap["Source"])) {
                            V($"F:Adding file: {AddMap["Source"]} as {AddMap["Target"]}");
                            Adder.Add2List(AddMap["Source"], AddMap["Target"], AddMap["Storage"], AddMap["Author"], AddMap["Notes"]);
                        } else if (Directory.Exists(AddMap["Source"])) {
                            var list = FileList.GetTree(AddMap["Source"]);
                            foreach (string f in list) {
                                V($"D:Adding file: {f} from directory {AddMap["Source"].Replace('\\', '/')} => {AddMap["Target"]}");
                                Adder.Add2List($"{AddMap["Source"].Replace('\\', '/')}/{f}", $"{AddMap["Target"]}/{f}", AddMap["Storage"], AddMap["Author"], AddMap["Notes"]);
                            }
                        } else {
                            throw new Exception($"Source {AddMap["Source"]} not found!");
                        }
                        AddMap = null;
                        break;
                    default:
                        AddMap[key] = value;
                        break;
                }
            } catch (Exception e) {
                QCol.QuickError($"{e.Message}");
                F_Script.State.DoString("print(debug.traceback())");
                throw new Exception("Error");
            }
        }

        public void AddAlias(string ori, string target) {
            V($"Alias: {ori} => {target}");
            Adder.AddAlias(ori, target);
            
        }

        public void AddImport(string d) => Adder.Imports.Add(d);
        public void AddRequire(string d) => Adder.Requires.Add(d);

        public void AddComment(string name, string content) => Adder.AddComment(name, content);
        
        public string GetDir(string dir) {
            var r = new StringBuilder();
            var gd = FileList.GetTree(dir);
            foreach(string f in gd) {
                if (r.Length > 0) r.Append(", ");
                r.Append($"\"{f}\"");
            }
            return r.ToString();
        }

    }

    class F_Script : FeatBase {

        static F_Add Adder => F_Add.X;

        static internal string[] ScriptArgs;
        static internal string ScriptFile;
        static internal string Script;
        static internal string ScriptType;        
        static internal Lua State;


        static public string JLS_Data_Output = "";
        static public TMap<string, string> JLS_Data = new TMap<string, string>();


        public F_Script() {
            Description = "Creates JCR file based on script files written in either Lua or NIL";
        }


        public override void Run(FlagParse fp) {
            if (fp.Args.Length==1) {
                QCol.Doing("Usage", "NJCR script <script> [args]");
                return;
            }
            ScriptFile = fp.Args[1];
            ScriptType = qstr.ExtractExt(ScriptFile).ToUpper();
            if (fp.Args.Length == 2)
                ScriptArgs = new string[0];
            else {
                ScriptArgs = new string[fp.Args.Length - 2];
                for(int i = 1; i < fp.Args.Length - 2; i++) {
                    ScriptArgs[i] = fp.Args[i + 2];
                }
            }
            try {
                QCol.Doing("Loading", ScriptFile);
                QCol.Doing("Type", ScriptType);
                Script = QuickStream.LoadString(ScriptFile);
                QCol.Doing("Configuring", "Default Data");
                JLS_Data_Output = $"{qstr.StripExt(ScriptFile)}.JCR";
                QCol.Doing("Creating", "Work state");
                State = new Lua();
                State["__NJCR"] = new ScriptAPI();
                QCol.Doing("Compiling", "Base Script");
                var BaseScript = QuickStream.StringFromEmbed("BaseScript.lua");
                State.DoString(BaseScript, "NJCR Base Script");
                QCol.Doing("Compiling", ScriptFile);
                switch (ScriptType) {
                    case "LUA":
                        State.DoString(Script, $"LUA:{qstr.StripDir(ScriptFile)}");
                        break;
                    case "NIL":
                        throw new Exception("NIL support has not yet been implemented. Please wait awhile");
                    default:
                        throw new Exception($"Unknown script type: {ScriptType}");
                }
                QCol.Doing("Starting", "Packer");
                QCol.Doing("Creating", Adder.jcrfile);
                Adder.updating = false;
                Adder.Go();
            } catch (Exception e) {
                QCol.QuickError(e.Message);                
                return;
            }
        }

        public override void Parse(FlagParse fp) {            
        }

    }
}