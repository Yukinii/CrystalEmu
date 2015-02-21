namespace CrystalEmuLib.IPC_Comms.Database
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;

    public class IniFile
    {

        #region "Declarations"

        // *** Lock for thread-safe access to file and local cache ***
        private readonly object _MLock = new object();

        // *** File name ***
        private string _MFileName;

        // *** Lazy loading flag ***
        private bool _MLazy;

        // *** Automatic flushing flag ***
        private bool _MAutoFlush;

        // *** Local cache ***
        private readonly Dictionary<string, Dictionary<string, string>> _MSections = new Dictionary<string, Dictionary<string, string>>();
        private readonly Dictionary<string, Dictionary<string, string>> _MModified = new Dictionary<string, Dictionary<string, string>>();

        // *** Local cache modified flag ***
        private bool _MCacheModified;

        #endregion

        #region "Methods"

        // *** Constructor ***
        public IniFile(string FileName)
        {
            Initialize(FileName, false);
        }

        // *** Initialization ***
        private void Initialize(string FileName, bool Lazy)
        {
            _MFileName = FileName;
            _MLazy = Lazy;
            _MAutoFlush = true;
            if (!_MLazy) Refresh();
        }

        // *** Parse section name ***
        private static string ParseSectionName(string Line)
        {
            if (!Line.StartsWith("[", StringComparison.Ordinal)) return null;
            if (!Line.EndsWith("]", StringComparison.Ordinal)) return null;
            return Line.Length < 3 ? null : Line.Substring(1, Line.Length - 2);
        }

        // *** Parse key+value pair ***
        private static bool ParseKeyValuePair(string Line, ref string Key, ref string Value)
        {
            // *** Check for key+value pair ***
            var I = 0;
            if (Line != null && (I = Line.IndexOf('=')) <= 0) return false;

            if (Line == null) return true;
            var J = Line.Length - I - 1;
            Key = Line.Substring(0, I).Trim();
            if (Key.Length <= 0) return false;

            Value = (J > 0) ? (Line.Substring(I + 1, J).Trim()) : ("");
            return true;
        }

        // *** Read file contents into local cache ***
        private void Refresh()
        {
            lock (_MLock)
            {
                StreamReader Sr = null;
                try
                {
                    // *** Clear local cache ***
                    _MSections.Clear();
                    _MModified.Clear();

                    // *** Open the INI file ***
                    try
                    {
                        Sr = !File.Exists(_MFileName) ? new StreamReader(File.Create(_MFileName)) : new StreamReader(_MFileName);
                    }
                    catch
                    {
                        return;
                    }

                    // *** Read up the file content ***
                    Dictionary<string, string> CurrentSection = null;
                    string S;
                    string Key = null;
                    string Value = null;
                    while ((S = Sr.ReadLine()) != null)
                    {
                        S = S.Trim();

                        // *** Check for section names ***
                        var SectionName = ParseSectionName(S);
                        if (SectionName != null)
                        {
                            // *** Only first occurrence of a section is loaded ***
                            if (_MSections.ContainsKey(SectionName))
                            {
                                CurrentSection = null;
                            }
                            else
                            {
                                CurrentSection = new Dictionary<string, string>();
                                _MSections.Add(SectionName, CurrentSection);
                            }
                        }
                        else if (CurrentSection != null)
                        {
                            // *** Check for key+value pair ***
                            if (!ParseKeyValuePair(S, ref Key, ref Value)) continue;
                            // *** Only first occurrence of a key is loaded ***
                            if (!CurrentSection.ContainsKey(Key))
                            {
                                CurrentSection.Add(Key, Value);
                            }
                        }
                    }
                }
                finally
                {
                    // *** Cleanup: close file ***
                    Sr?.Close();
                }
            }
        }

        // *** Flush local cache content ***
        public void Flush()
        {
            lock (_MLock)
            {
                PerformFlush();
            }
        }

        public void PerformFlush()
        {
            try
            {
                if (!_MCacheModified) return;
                _MCacheModified = false;
                var OriginalFileExists = File.Exists(_MFileName);
                var TmpFileName = Path.ChangeExtension(_MFileName, "$n$");
                if (!Directory.Exists(Path.GetDirectoryName(TmpFileName)))
                    Directory.CreateDirectory(Path.GetDirectoryName(TmpFileName));
                while (File.Exists(TmpFileName))
                {
                    try
                    {
                        File.Delete(TmpFileName);
                    }
                    catch
                    {
                        Console.WriteLine("Fail.");
                    }
                }
                var Sw = new StreamWriter(TmpFileName) { AutoFlush = true };

                try
                {
                    Dictionary<string, string> CurrentSection = null;
                    if (OriginalFileExists)
                    {
                        StreamReader Sr = null;
                        try
                        {
                            Sr = new StreamReader(_MFileName);
                            string Key = null;
                            string Value = null;
                            var Reading = true;
                            while (Reading)
                            {
                                var S = Sr.ReadLine();
                                Reading = (S != null);
                                bool Unmodified;
                                string SectionName;
                                if (Reading)
                                {
                                    Unmodified = true;
                                    S = S.Trim();
                                    SectionName = ParseSectionName(S);
                                }
                                else
                                {
                                    Unmodified = false;
                                    SectionName = null;
                                }
                                
                                if ((SectionName != null) || (!Reading))
                                {
                                    if (CurrentSection?.Count > 0)
                                    {
                                        var Section = CurrentSection;
                                        foreach (var Fkey in CurrentSection.Keys.Where(Fkey => Section.TryGetValue(Fkey, out Value)))
                                        {
                                            Sw.Write(Fkey);
                                            Sw.Write('=');
                                            Sw.WriteLine(Value);
                                        }
                                        CurrentSection.Clear();
                                    }

                                    if (Reading)
                                    {
                                        if (!_MModified.TryGetValue(SectionName, out CurrentSection))
                                        {
                                        }
                                    }
                                }
                                else if (CurrentSection != null)
                                {
                                    if (ParseKeyValuePair(S, ref Key, ref Value))
                                    {
                                        if (CurrentSection.TryGetValue(Key, out Value))
                                        {
                                            Unmodified = false;
                                            CurrentSection.Remove(Key);

                                            Sw.Write(Key);
                                            Sw.Write('=');
                                            Sw.WriteLine(Value);
                                        }
                                    }
                                }
                                
                                if (Unmodified)
                                {
                                    Sw.WriteLine(S);
                                }
                            }
                            
                            Sr.Close();
                            Sr = null;
                        }
                        finally
                        {             
                            Sr.Close();
                        }
                    }
                    
                    foreach (var SectionPair in _MModified)
                    {
                        CurrentSection = SectionPair.Value;
                        if (CurrentSection.Count <= 0)
                            continue;
                        Sw.Write('[');
                        Sw.Write(SectionPair.Key);
                        Sw.WriteLine(']');
                        foreach (var ValuePair in CurrentSection)
                        {
                            Sw.Write(ValuePair.Key);
                            Sw.Write('=');
                            Sw.WriteLine(ValuePair.Value);
                        }
                        CurrentSection.Clear();
                    }
                    _MModified.Clear();
                    
                    Sw.Close();
                    Sw = null;
                    File.Copy(TmpFileName, _MFileName, true);
                    File.Delete(TmpFileName);
                }
                finally
                {              
                    Sw.Close();
                }
            }
            catch (Exception E)
            {
                Console.WriteLine(E);
            }

        }

        // *** Read a value from local cache ***
        public string ReadString(string SectionName, string Key, string DefaultValue)
        {
            // *** Lazy loading ***
            if (_MLazy)
            {
                _MLazy = false;
                Refresh();
            }

            lock (_MLock)
            {
                // *** Check if the section exists ***
                Dictionary<string, string> Section;
                if (!_MSections.TryGetValue(SectionName, out Section)) return DefaultValue;

                // *** Check if the key exists ***
                string Value;
                return !Section.TryGetValue(Key, out Value) ? DefaultValue : Value;

                // *** Return the found value ***
            }
        }
        public string ReadString(string SectionName, string Key)
        {
            Refresh();
            // *** Lazy loading ***
            if (_MLazy)
            {
                _MLazy = false;
                Refresh();
            }

            lock (_MLock)
            {
                // *** Check if the section exists ***
                Dictionary<string, string> Section;
                if (!_MSections.TryGetValue(SectionName, out Section)) return "";

                // *** Check if the key exists ***
                string Value;
                return !Section.TryGetValue(Key, out Value) ? "nil" : Value;

                // *** Return the found value ***
            }
        }

        // *** Insert or modify a value in local cache ***
        public void Write(string SectionName, string Key, object Value)
        {
            // *** Lazy loading ***
            if (_MLazy)
            {
                _MLazy = false;
                Refresh();
            }

            lock (_MLock)
            {
                // *** Flag local cache modification ***
                _MCacheModified = true;

                // *** Check if the section exists ***
                Dictionary<string, string> Section = null;
                if (_MSections != null && !_MSections.TryGetValue(SectionName, out Section))
                {
                    // *** If it doesn't, add it ***
                    Section = new Dictionary<string, string>();
                    _MSections.Add(SectionName, Section);
                }

                // *** Modify the value ***
                if (Section != null && Section.ContainsKey(Key)) Section.Remove(Key);
                Section.Add(Key, Convert.ToString(Value));

                // *** Add the modified value to local modified values cache ***
                if (_MModified != null && !_MModified.TryGetValue(SectionName, out Section))
                {
                    Section = new Dictionary<string, string>();
                    _MModified.Add(SectionName, Section);
                }

                if (Section != null && Section.ContainsKey(Key)) Section.Remove(Key);
                Section.Add(Key, Convert.ToString(Value));

                // *** Automatic flushing : immediately write any modification to the file ***
                if (_MAutoFlush)
                    PerformFlush();
            }
        }

        // *** Encode byte array ***

        // *** Decode byte array ***

        // *** Getters for various types ***
        public bool GetValue(string SectionName, string Key, bool DefaultValue)
        {
            var StringValue = ReadString(SectionName, Key, DefaultValue.ToString(CultureInfo.InvariantCulture));
            int Value;
            if (int.TryParse(StringValue, out Value)) return (Value != 0);
            return DefaultValue;
        }

        public int ReadInt(string SectionName, string Key, int DefaultValue)
        {
            var StringValue = ReadString(SectionName, Key, DefaultValue.ToString(CultureInfo.InvariantCulture));
            int Value;
            return int.TryParse(StringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out Value) ? Value : DefaultValue;
        }
        public byte ReadByte(string SectionName, string Key, byte DefaultValue)
        {
            var StringValue = ReadString(SectionName, Key, DefaultValue.ToString(CultureInfo.InvariantCulture));
            byte Value;
            return byte.TryParse(StringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out Value) ? Value : DefaultValue;
        }
        public ushort ReadShort(string SectionName, string Key, ushort DefaultValue)
        {
            var StringValue = ReadString(SectionName, Key, DefaultValue.ToString(CultureInfo.InvariantCulture));
            ushort Value;
            return ushort.TryParse(StringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out Value) ? Value : DefaultValue;
        }

        public DateTime GetValue(string SectionName, string Key, DateTime DefaultValue)
        {
            var StringValue = ReadString(SectionName, Key, DefaultValue.ToString(CultureInfo.InvariantCulture));
            DateTime Value;
            return DateTime.TryParse(StringValue, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.NoCurrentDateDefault | DateTimeStyles.AssumeLocal, out Value) ? Value : DefaultValue;
        }

        // *** Setters for various types ***
        public void SetValue(string SectionName, string Key, bool Value) => Write(SectionName, Key, (Value) ? ("1") : ("0"));

        public void SetValue(string SectionName, string Key, int Value) => Write(SectionName, Key, Value.ToString(CultureInfo.InvariantCulture));

        public void SetValue(string SectionName, string Key, DateTime Value) => Write(SectionName, Key, Value.ToString(CultureInfo.InvariantCulture));

        #endregion

        public double ReadDouble(string SectionName, string Key, double DefaultValue = 0)
        {
            var StringValue = ReadString(SectionName, Key, DefaultValue.ToString(CultureInfo.InvariantCulture));
            double Value;
            return double.TryParse(StringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out Value) ? Value : DefaultValue;

        }
    }
}
