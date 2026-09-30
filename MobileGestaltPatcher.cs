using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using plist_dict = System.Collections.Generic.Dictionary<string, object>;

namespace Milo_Activator
{
    public static class GestaltPatcher
    {
        public static readonly string Signature = "LC BYPASS = ABDOU = MILO";

        private static readonly Dictionary<string, string> IosBuildMap = new Dictionary<string, string>
        {
            { "19A341", "15.0" }, { "19A346", "15.0" }, { "19A348", "15.0.1" }, { "19A404", "15.0.2" },
            { "19B74", "15.1" }, { "19B81", "15.1.1" }, { "19C56", "15.2" }, { "19C63", "15.2.1" },
            { "19D50", "15.3" }, { "19D52", "15.3.1" }, { "19E241", "15.4" }, { "19E258", "15.4.1" },
            { "19F77", "15.5" }, { "19G71", "15.6" }, { "19G82", "15.6.1" }, { "19H12", "15.7" },
            { "19H117", "15.7.1" }, { "19H218", "15.7.2" }, { "19H307", "15.7.3" }, { "19H321", "15.7.4" },
            { "19H332", "15.7.5" }, { "19H349", "15.7.6" }, { "19H357", "15.7.7" }, { "19H364", "15.7.8" },
            { "19H365", "15.7.9" }, { "19H370", "15.8" }, { "19H380", "15.8.1" },
            { "20A362", "16.0" }, { "20A371", "16.0.1" }, { "20A380", "16.0.2" }, { "20B82", "16.1" },
            { "20B101", "16.1.1" }, { "20C65", "16.2" }, { "20D47", "16.3" }, { "20D67", "16.3.1" },
            { "20E247", "16.4" }, { "20E252", "16.4.1" }, { "20F66", "16.5" }, { "20F75", "16.5.1" },
            { "20G75", "16.6" }, { "20G81", "16.6.1" }, { "20H19", "16.7" }, { "20H30", "16.7.1" },
            { "20H115", "16.7.2" }, { "20H232", "16.7.3" },
            { "21A326", "17.0" }, { "21A327", "17.0" }, { "21A329", "17.0" }, { "21A340", "17.0.1" },
            { "21A350", "17.0.2" }, { "21A360", "17.0.3" }, { "21B74", "17.1" }, { "21B80", "17.1" },
            { "21B91", "17.1.1" }, { "21B101", "17.1.2" }, { "21C62", "17.2" }, { "21C66", "17.2.1" },
            { "21D50", "17.3" }, { "21D61", "17.3.1" }, { "21E219", "17.4" }, { "21E236", "17.4.1" },
            { "21E237", "17.4.1" }, { "21F79", "17.5" }, { "21F90", "17.5.1" }, { "21G80", "17.6" },
            { "21G93", "17.6.1" }, { "21H16", "17.7" }, { "21H216", "17.7.1" }, { "21H221", "17.7.2" },
            { "22A3351", "18.0" }, { "22A3354", "18.0" }, { "22A3370", "18.0.1" }, { "22B83", "18.1" },
            { "22B91", "18.1.1" }, { "22C152", "18.2" }, { "22C161", "18.2.1" }, { "22D63", "18.3" },
            { "22D64", "18.3" }, { "22D72", "18.3.1" }, { "22D82", "18.3.2" }, { "22E240", "18.4" },
            { "22E252", "18.4.1" }, { "22F76", "18.5" }, { "22G86", "18.6" }, { "22G90", "18.6.1" },
            { "22G100", "18.6.2" }, { "22H20", "18.7" }, { "22H31", "18.7.1" }, { "22H124", "18.7.2" },
            { "23A341", "26.0" }, { "23A355", "26.0.1" }, { "23B85", "26.1" }
        };

        private static readonly Dictionary<string, KeyValuePair<int, byte>> OffsetPatches = new Dictionary<string, KeyValuePair<int, byte>>
        {
            { "15", new KeyValuePair<int, byte>(0x15C0, 0x01) },
            { "15.0", new KeyValuePair<int, byte>(0x15C0, 0x01) },
            { "15.1", new KeyValuePair<int, byte>(0x15C0, 0x01) },
            { "15.2", new KeyValuePair<int, byte>(0x15C0, 0x01) },
            { "15.3", new KeyValuePair<int, byte>(0x15C0, 0x01) },
            { "15.4", new KeyValuePair<int, byte>(0x15C0, 0x01) },
            { "15.5", new KeyValuePair<int, byte>(0x15C0, 0x01) },
            { "15.6", new KeyValuePair<int, byte>(0x15C0, 0x01) },
            { "15.7", new KeyValuePair<int, byte>(0x15C0, 0x01) },
            { "15.8", new KeyValuePair<int, byte>(0x15C0, 0x01) },
            { "16", new KeyValuePair<int, byte>(0x15C0, 0x01) },
            { "16.0", new KeyValuePair<int, byte>(0x15C0, 0x01) },
            { "16.1", new KeyValuePair<int, byte>(0x15C0, 0x01) },
            { "16.2", new KeyValuePair<int, byte>(0x15C0, 0x01) },
            { "16.3", new KeyValuePair<int, byte>(0x15C0, 0x01) },
            { "16.4", new KeyValuePair<int, byte>(0x15C0, 0x01) },
            { "16.5", new KeyValuePair<int, byte>(0x15C0, 0x01) },
            { "16.6", new KeyValuePair<int, byte>(0x15C0, 0x01) },
            { "16.7", new KeyValuePair<int, byte>(0x15C0, 0x01) },
            { "17", new KeyValuePair<int, byte>(0x15C0, 0x01) },
            { "17.0", new KeyValuePair<int, byte>(0x15C0, 0x01) },
            { "17.1", new KeyValuePair<int, byte>(0x15C0, 0x01) },
            { "17.2", new KeyValuePair<int, byte>(0x15C0, 0x01) },
            { "17.3", new KeyValuePair<int, byte>(0x15C0, 0x01) },
            { "17.4", new KeyValuePair<int, byte>(0x15C0, 0x01) },
            { "17.5", new KeyValuePair<int, byte>(0x15C0, 0x01) },
            { "17.6", new KeyValuePair<int, byte>(0x15C0, 0x01) },
            { "17.7", new KeyValuePair<int, byte>(0x15C0, 0x01) },
            { "18.6", new KeyValuePair<int, byte>(0x15C0, 0x01) },
            { "18.6.0", new KeyValuePair<int, byte>(0x15C0, 0x01) },
            { "18.6.1", new KeyValuePair<int, byte>(0x15C0, 0x01) },
            { "18.6.2", new KeyValuePair<int, byte>(0x15C0, 0x01) },
            { "18.7", new KeyValuePair<int, byte>(0x15C0, 0x01) },
            { "18.7.0", new KeyValuePair<int, byte>(0x15C0, 0x01) },
            { "18.7.1", new KeyValuePair<int, byte>(0x15C0, 0x01) },
            { "18.7.2", new KeyValuePair<int, byte>(0x15C0, 0x01) },
            { "26", new KeyValuePair<int, byte>(0x16CB, 0x01) },
            { "26.0", new KeyValuePair<int, byte>(0x16CB, 0x01) },
            { "26.0.1", new KeyValuePair<int, byte>(0x16CB, 0x01) },
            { "26.1", new KeyValuePair<int, byte>(0x16CB, 0x01) }
        };

        public static string NormalizeVersion(string version)
        {
            if (string.IsNullOrEmpty(version)) return "";
            string v = version.Trim().ToLower();
            v = Regex.Replace(v, @"\.0+(?=\.|$)", "");
            return v.TrimEnd('.');
        }

        public static KeyValuePair<bool, string> IsSupportedOffsetVersion(string version)
        {
            if (string.IsNullOrEmpty(version)) return new KeyValuePair<bool, string>(false, null);
            string v = NormalizeVersion(version);

            var sortedKeys = new List<string>(OffsetPatches.Keys);
            sortedKeys.Sort((a, b) => NormalizeVersion(b).Length.CompareTo(NormalizeVersion(a).Length));

            foreach (var key in sortedKeys)
            {
                string normKey = NormalizeVersion(key);
                if (v == normKey || v.StartsWith(normKey + "."))
                {
                    return new KeyValuePair<bool, string>(true, key);
                }
            }
            return new KeyValuePair<bool, string>(false, null);
        }

        public static Dictionary<string, object> ExtractDeviceInfo(byte[] content, string filename)
        {
            var info = new Dictionary<string, object>
            {
                { "device_model", null },
                { "ios_version", null },
                { "build_version", null },
                { "device_type", "iPhone" }
            };

            string contentStr = Encoding.GetEncoding("latin1").GetString(content);

            Match modelMatch = Regex.Match(contentStr, @"(iPhone|iPad)\d{1,2},\d{1,2}");
            if (modelMatch.Success)
            {
                info["device_model"] = modelMatch.Value;
                if (modelMatch.Value.ToLower().Contains("ipad"))
                {
                    info["device_type"] = "iPad";
                }
            }

            foreach (var pair in IosBuildMap)
            {
                if (contentStr.Contains(pair.Key))
                {
                    info["build_version"] = pair.Key;
                    info["ios_version"] = pair.Value;
                    break;
                }
            }

            if (info["ios_version"] == null)
            {
                Match verMatch = Regex.Match(contentStr, @"\b(1[5-9]|2[0-9])\.(0|[1-9]|1[0-9])(?:\.(0|[1-9]|1[0-9]))?(?:[ab]\d)?\b");
                if (verMatch.Success)
                {
                    info["ios_version"] = verMatch.Value;
                }
            }

            if (info["device_model"] == null)
            {
                Match fnameMatch = Regex.Match(filename, @"(iPhone|iPad|iPod)(\d+,\d+)", RegexOptions.IgnoreCase);
                if (fnameMatch.Success)
                {
                    info["device_model"] = fnameMatch.Groups[1].Value + fnameMatch.Groups[2].Value;
                }
            }

            return info;
        }

        public static bool PatchCacheData(byte[] data, int offset, byte value)
        {
            if (offset >= data.Length) return false;
            if (data[offset] == value) return true;
            data[offset] = value;
            return true;
        }

        public static Tuple<bool, string, byte[]> PatchPlistOffset(byte[] content, string versionKey)
        {
            if (!OffsetPatches.TryGetValue(versionKey, out var patchInfo))
            {
                return new Tuple<bool, string, byte[]>(false, $"iOS version '{versionKey}' is outside offset-patching range.", content);
            }

            int offset = patchInfo.Key;
            byte value = patchInfo.Value;

            try
            {
                string contentStr = Encoding.GetEncoding("latin1").GetString(content);
                string pattern = @"(<key>CacheData</key>\s*<data>\s*)([A-Za-z0-9+/=\s]*)(\s*</data>)";
                Match match = Regex.Match(contentStr, pattern, RegexOptions.Singleline);

                if (match.Success)
                {
                    string prefix = match.Groups[1].Value;
                    string b64Data = match.Groups[2].Value;
                    string suffix = match.Groups[3].Value;

                    string cleanB64 = Regex.Replace(b64Data, @"\s+", "");
                    byte[] decoded = Convert.FromBase64String(cleanB64);

                    if (offset >= decoded.Length)
                    {
                        return new Tuple<bool, string, byte[]>(false, $"✗ Offset 0x{offset:X} out of bounds", content);
                    }

                    if (PatchCacheData(decoded, offset, value))
                    {
                        string reencoded = Convert.ToBase64String(decoded);
                        StringBuilder sb = new StringBuilder();
                        sb.Append("\n\t");
                        for (int i = 0; i < reencoded.Length; i += 76)
                        {
                            int length = Math.Min(76, reencoded.Length - i);
                            sb.Append(reencoded.Substring(i, length)).Append("\n\t");
                        }

                        int startIdx = match.Index;
                        int lenIdx = match.Length;
                        byte[] prefixBytes = Encoding.ASCII.GetBytes(prefix);
                        byte[] middleBytes = Encoding.ASCII.GetBytes(sb.ToString().TrimEnd('\t'));
                        byte[] suffixBytes = Encoding.ASCII.GetBytes(suffix);

                        MemoryStream ms = new MemoryStream();
                        ms.Write(content, 0, startIdx);
                        ms.Write(prefixBytes, 0, prefixBytes.Length);
                        ms.Write(middleBytes, 0, middleBytes.Length);
                        ms.Write(suffixBytes, 0, suffixBytes.Length);
                        ms.Write(content, startIdx + lenIdx, content.Length - (startIdx + lenIdx));

                        return new Tuple<bool, string, byte[]>(true, $"✓ Patched offset 0x{offset:X} → 0x{value:X2} in CacheData", ms.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                return new Tuple<bool, string, byte[]>(false, $"✗ Error patching XML: {ex.Message}", content);
            }

            return new Tuple<bool, string, byte[]>(false, "✗ Could not locate or patch CacheData section", content);
        }
    }
}