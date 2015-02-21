using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CrystalEmuLib.Extensions;

namespace CrystalEmuLib.IPC_Comms.Database
{
    public class IniFile
    {
        public double ReadDouble(string SectionName, string Key, double DefaultValue = 0)
        {
            var StringValue = ReadString(SectionName, Key, DefaultValue.ToString(CultureInfo.InvariantCulture));
            double Value;
            return double.TryParse(StringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out Value) ? Value : DefaultValue;
        }

        #region "Declarations"

        private string _FileName;
        public bool CacheModified;

        private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, string>> _Sections = new ConcurrentDictionary<string, ConcurrentDictionary<string, string>>();
        private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, string>> _Modified = new ConcurrentDictionary<string, ConcurrentDictionary<string, string>>();

        #endregion

        #region "Methods"

        public IniFile(string FileName)
        {
            Initialize(FileName);
        }

        private void Initialize(string FileName)
        {
            _FileName = FileName;
            Refresh();
        }

        private static string ParseSectionName(string Line)
        {
            if (!Line.StartsWith("[", StringComparison.Ordinal)) return null;
            if (!Line.EndsWith("]", StringComparison.Ordinal)) return null;
            return Line.Length < 3 ? null : Line.Substring(1, Line.Length - 2);
        }

        private static bool ParseKeyValuePair(string Line, ref string Key, ref string Value)
        {
            int I;
            if ((I = Line.IndexOf('=')) <= 0) return false;

            var J = Line.Length - I - 1;
            Key = Line.Substring(0, I).Trim();
            if (Key.Length <= 0) return false;

            Value = (J > 0) ? (Line.Substring(I + 1, J).Trim()) : ("");
            return true;
        }

        private void Refresh()
        {
            StreamReader Sr = null;
            try
            {
                _Sections.Clear();
                _Modified.Clear();
                try
                {
                    Sr = !File.Exists(_FileName) ? new StreamReader(File.Create(_FileName)) : new StreamReader(_FileName);
                }
                catch
                {
                    return;
                }

                ConcurrentDictionary<string, string> CurrentSection = null;
                string S;
                string Key = null;
                string Value = null;
                while ((S = Sr.ReadLine()) != null)
                {
                    S = S.Trim();
                    var SectionName = ParseSectionName(S);
                    if (SectionName != null)
                    {
                        if (_Sections.ContainsKey(SectionName))
                            CurrentSection = null;
                        else
                        {
                            CurrentSection = new ConcurrentDictionary<string, string>();
                            _Sections.TryAdd(SectionName, CurrentSection);
                        }
                    }
                    else if (CurrentSection != null)
                    {
                        if (!ParseKeyValuePair(S, ref Key, ref Value)) continue;

                        if (!CurrentSection.ContainsKey(Key))
                            CurrentSection.TryAdd(Key, Value);
                    }
                }
            }
            finally
            {
                Sr?.Close();
            }
        }

        public void Flush() => PerformFlush();

        private async void PerformFlush()
        {
            try
            {
                if (!CacheModified)
                    return;
                CacheModified = false;
                var OriginalFileExists = File.Exists(_FileName);
                var TmpFileName = Path.ChangeExtension(_FileName, "$n$");
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
                var Sw = new StreamWriter(TmpFileName) {AutoFlush = true};

                try
                {
                    ConcurrentDictionary<string, string> CurrentSection = null;
                    if (OriginalFileExists)
                    {
                        StreamReader Sr = null;
                        try
                        {
                            Sr = new StreamReader(_FileName);
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
                                        if (!_Modified.TryGetValue(SectionName, out CurrentSection))
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
                                            while (!CurrentSection.TryRemove(Key))
                                            {
                                                await Task.Delay(10);
                                            }

                                            Sw.Write(Key);
                                            Sw.Write('=');
                                            Sw.WriteLine(Value);
                                        }
                                    }
                                }

                                if (Unmodified)
                                    Sw.WriteLine(S);
                            }

                            Sr.Close();
                            Sr = null;
                        }
                        finally
                        {
                            Sr?.Close();
                        }
                    }

                    foreach (var SectionPair in _Modified)
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
                    _Modified.Clear();

                    Sw.Close();
                    Sw = null;
                    File.Copy(TmpFileName, _FileName, true);
                    File.Delete(TmpFileName);
                }
                finally
                {
                    Sw?.Close();
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
            ConcurrentDictionary<string, string> Section;
            if (!_Sections.TryGetValue(SectionName, out Section)) return DefaultValue;
            string Value;
            return !Section.TryGetValue(Key, out Value) ? DefaultValue : Value;
        }

        public string ReadString(string SectionName, string Key)
        {
            Refresh();

            // *** Check if the section exists ***
            ConcurrentDictionary<string, string> Section;
            if (!_Sections.TryGetValue(SectionName, out Section)) return "";

            string Value;
            return !Section.TryGetValue(Key, out Value) ? "nil" : Value;
        }

        public async void Write(string SectionName, string Key, object Value)
        {
            CacheModified = true;

            ConcurrentDictionary<string, string> Section;
            if (!_Sections.TryGetValue(SectionName, out Section))
            {
                Section = new ConcurrentDictionary<string, string>();
                _Sections.TryAdd(SectionName, Section);
            }

            if (Section.ContainsKey(Key))
            {
                while (!Section.TryRemove(Key))
                {
                    await Task.Delay(10);
                }
            }
            Section.TryAdd(Key, Convert.ToString(Value));

            if (!_Modified.TryGetValue(SectionName, out Section))
            {
                Section = new ConcurrentDictionary<string, string>();
                _Modified.TryAdd(SectionName, Section);
            }

            if (Section.ContainsKey(Key))
            {
                while (!Section.TryRemove(Key))
                {
                    await Task.Delay(10);
                }
            }
            Section.TryAdd(Key, Convert.ToString(Value));
        }

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
    }
}