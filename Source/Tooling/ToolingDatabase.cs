using System;
using System.Collections.Generic;
using System.Text;
using KSP;
using UnityEngine;

namespace RP0
{
    public class ToolingEntry
    {
        public float Value { get; set; }
        public List<ToolingEntry> Children { get; } = new List<ToolingEntry>();

        public ToolingEntry(float value)
        {
            Value = value;
        }
    }
    public class ToolingDatabase
    {
        protected static float comparisonEpsilonHigh = 1.04f;
        protected static float comparisonEpsilonLow = 0.96f;

        protected static int EpsilonCompare(float a, float b)
        {
            if (a > b * comparisonEpsilonLow && a < b * comparisonEpsilonHigh)
                return 0;

            return a.CompareTo(b);
        }

        public static Dictionary<string, List<ToolingEntry>> toolings = new Dictionary<string, List<ToolingEntry>>();
<<<<<<< master

        protected static int GetEntryIndex(float value, List<ToolingEntry> list, out int min)
        {
            min = 0;
            int max = list.Count - 1;
            while (min <= max)
            {
                int mid = (min + max) / 2;
                switch (EpsilonCompare(value, list[mid].Value))
                {
                    case 0:
                        return mid;

                    case 1:
                        min = mid + 1;
                        break;

                    default:
                    case -1:
                        max = mid - 1;
                        break;
                }
            }
            return -1;
        }

=======

        protected static int GetEntryIndex(float value, List<ToolingEntry> list, out int min)
        {
            min = 0;
            int max = list.Count - 1;
            while (min <= max)
            {
                int mid = (min + max) / 2;
                switch (EpsilonCompare(value, list[mid].Value))
                {
                    case 0:
                        return mid;

                    case 1:
                        min = mid + 1;
                        break;

                    default:
                    case -1:
                        max = mid - 1;
                        break;
                }
            }
            return -1;
        }

>>>>>>> ProcAvionicsTooling
        /// <summary>
        /// Compares two tooling sizes and checks whether their diameter and length are within a predefined epsilon (currently 4%).
        /// </summary>
        /// <param name="diam1">Diameter of tooling 1</param>
        /// <param name="len1">Length of tooling 1</param>
        /// <param name="diam2">Diameter of tooling 2</param>
        /// <param name="len2">Length of tooling 2</param>
        /// <returns>True if the two tooling sizes are considered the same.</returns>
        public static bool IsSameSize(float diam1, float len1, float diam2, float len2)
        {
            return EpsilonCompare(diam1, diam2) == 0 && EpsilonCompare(len1, len2) == 0;
        }

        public static int GetToolingLevel(string type, params float[] parameters)
        {
<<<<<<< master
            if (!ToolingManager.Instance.toolingEnabled)
            {
                return parameters.Length;
            }

            List<ToolingEntry> entries;
            var level = 0;
            if (toolings.TryGetValue(type, out entries))
            {
                for (int i = 0; i < parameters.Length; ++i)
                {
                    var entryIndex = GetEntryIndex(parameters[i], entries, out _);
                    if (entryIndex == -1)
=======
            List<ToolingEntry> entries;
            var level = 0;
            if (toolings.TryGetValue(type, out entries))
            {
                for(int i = 0; i < parameters.Length; ++i)
                {
                    var entryIndex = GetEntryIndex(parameters[i], entries, out _);
                    if(entryIndex == -1)
>>>>>>> ProcAvionicsTooling
                    {
                        break;
                    }
                    level++;
                    entries = entries[entryIndex].Children;
                }
<<<<<<< master
            }

            return level;
        }

        public static bool UnlockTooling(string type, params float[] parameters)
        {
            var toolingUnlocked = false;
            if (!toolings.TryGetValue(type, out var entries))
            {
                entries = new List<ToolingEntry>();
                toolings[type] = entries;
            }

            for (int i = 0; i < parameters.Length; ++i)
            {
                var entryIndex = GetEntryIndex(parameters[i], entries, out var insertionIndex);
                if (entryIndex == -1)
                {
                    entries.Insert(insertionIndex, new ToolingEntry(parameters[i]));
                    entryIndex = insertionIndex;
                    toolingUnlocked = true;
                }
                entries = entries[entryIndex].Children;
            }

            return toolingUnlocked;
        }


        public static void Load(ConfigNode node)
        {
            toolings.Clear();

            if (node == null)
            {
                return;
            }

            LoadNewDbFormat(node);

            if (toolings.Count == 0)
            {
                LoadOldDbFormat(node);
            }
        }

        private static void LoadNewDbFormat(ConfigNode node)
        {
=======
            }

            return level;
        }

        public static bool UnlockTooling(string type, params float[] parameters)
        {
            var toolingUnlocked = false;
            if (!toolings.TryGetValue(type, out var entries))
            {
                entries = new List<ToolingEntry>();
                toolings[type] = entries;
            }

            for (int i = 0; i < parameters.Length; ++i)
            {
                var entryIndex = GetEntryIndex(parameters[i], entries, out var insertionIndex);
                if(entryIndex == -1)
                {
                    entries.Insert(insertionIndex, new ToolingEntry(parameters[i]));
                    entryIndex = insertionIndex;
                    toolingUnlocked = true;
                }
                entries = entries[entryIndex].Children;
            }

            return toolingUnlocked;
        }


        public static void Load(ConfigNode node)
        {
            toolings.Clear();

            if (node == null)
            {
                return;
            }

<<<<<<< master
>>>>>>> ProcAvionicsTooling
=======
            LoadNewDbFormat(node);

            if (toolings.Count == 0)
            {
                LoadOldDbFormat(node);
            }
        }

        private static void LoadNewDbFormat(ConfigNode node)
        {
>>>>>>> Restore loading of legacy tooling DBs
            foreach (var typeNode in node.GetNodes("TYPE"))
            {
                string type = typeNode.GetValue("type");
                if (string.IsNullOrEmpty(type))
                {
                    continue;
                }

                var entries = new List<ToolingEntry>();
                LoadEntries(typeNode, entries);
<<<<<<< master
<<<<<<< master
=======
>>>>>>> Restore loading of legacy tooling DBs
                if (entries.Count > 0)
                {
                    toolings[type] = entries;
                }
<<<<<<< master
=======
                toolings[type] = entries;
>>>>>>> ProcAvionicsTooling
=======
>>>>>>> Restore loading of legacy tooling DBs
            }
        }

        private static void LoadEntries(ConfigNode node, List<ToolingEntry> entries)
        {
            foreach (var entryNode in node.GetNodes("ENTRY"))
            {
                var tmp = 0f;
                if (!entryNode.TryGetValue("value", ref tmp))
                {
                    continue;
                }

                var entry = new ToolingEntry(tmp);
                LoadEntries(entryNode, entry.Children);
                entries.Add(entry);
<<<<<<< master
            }
        }

        public static void Save(ConfigNode node)
=======
            }
        }

        public static void Save(ConfigNode node)
        {
            foreach (var typeToEntries in toolings)
            {
                var typeNode = node.AddNode("TYPE");
                typeNode.AddValue("type", typeToEntries.Key);

                var entries = typeToEntries.Value;
                SaveEntries(typeNode, entries);
            }
        }

        private static void SaveEntries(ConfigNode typeNode, List<ToolingEntry> entries)
        {
            foreach (var entry in entries)
            {
                var entryNode = typeNode.AddNode("ENTRY");
                entryNode.AddValue("value", entry.Value.ToString("G17"));
                SaveEntries(entryNode, entry.Children);
            }
        }

<<<<<<< master
        public static void LoadOldFormat(ConfigNode node)
>>>>>>> ProcAvionicsTooling
        {
            foreach (var typeToEntries in toolings)
            {
                var typeNode = node.AddNode("TYPE");
                typeNode.AddValue("type", typeToEntries.Key);

                var entries = typeToEntries.Value;
                SaveEntries(typeNode, entries);
            }
        }

        private static void SaveEntries(ConfigNode typeNode, List<ToolingEntry> entries)
        {
            foreach (var entry in entries)
            {
                var entryNode = typeNode.AddNode("ENTRY");
                entryNode.AddValue("value", entry.Value.ToString("G17"));
                SaveEntries(entryNode, entry.Children);
            }
        }

        public static void LoadOldDbFormat(ConfigNode node)
        {
=======
        public static void LoadOldDbFormat(ConfigNode node)
        {
>>>>>>> Restore loading of legacy tooling DBs
            foreach (var c in node.GetNodes("TYPE"))
            {
                string type = c.GetValue("type");
                if (string.IsNullOrEmpty(type))
                    continue;

<<<<<<< master
<<<<<<< master
                var entries = new List<ToolingEntry>();
=======
                List<ToolingEntry> lst = new List<ToolingEntry>();
>>>>>>> ProcAvionicsTooling
=======
                var entries = new List<ToolingEntry>();
>>>>>>> Restore loading of legacy tooling DBs

                foreach (var n in c.GetNodes("DIAMETER"))
                {
                    float tmp = 0f;
                    if (!n.TryGetValue("diameter", ref tmp))
                        continue;

<<<<<<< master
<<<<<<< master
                    var diameter = new ToolingEntry(tmp);
=======
                    ToolingEntry d = new ToolingEntry(tmp);
>>>>>>> ProcAvionicsTooling
=======
                    var diameter = new ToolingEntry(tmp);
>>>>>>> Restore loading of legacy tooling DBs

                    var length = n.GetNode("LENGTHS");
                    if (length != null)
                    {
                        foreach (ConfigNode.Value v in length.values)
                        {
                            if (float.TryParse(v.value, out tmp))
<<<<<<< master
<<<<<<< master
=======
>>>>>>> Restore loading of legacy tooling DBs
                            {
                                diameter.Children.Add(new ToolingEntry(tmp));
                            }
                        }
                    }
<<<<<<< master
=======
                                //d.lengths.Add(tmp);
>>>>>>> ProcAvionicsTooling
=======
>>>>>>> Restore loading of legacy tooling DBs

                    entries.Add(diameter);
                }

<<<<<<< master
<<<<<<< master
                toolings[type] = entries;
=======
                toolings[type] = lst;
>>>>>>> ProcAvionicsTooling
=======
                toolings[type] = entries;
>>>>>>> Restore loading of legacy tooling DBs
            }
        }
    }
}
