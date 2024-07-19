using System;
using System.Collections.Generic;

namespace Yueby.PackageImporter
{
    [Serializable]
    public class PackageImporterData
    {
        public List<PackageInfoGroup> PackageInfoGroups;
        // public bool IsInteractive = true;

        public PackageInfoGroup Current => PackageInfoGroups[CurrentIndex];
        public int CurrentIndex { get; private set; } = 0;

        public Action<int> OnIndexChanged;

        public void SetCurrent(int index)
        {
            CurrentIndex = index;
            OnIndexChanged?.Invoke(index);
        }


        public PackageImporterData()
        {
            PackageInfoGroups = new List<PackageInfoGroup> { new("Default", new List<PackageInfo>()) };
        }

        public PackageImporterData(List<PackageInfoGroup> packageInfoGroups)
        {
            PackageInfoGroups = packageInfoGroups;
        }

        public string[] GetPackageGroupNames()
        {
            var names = new string[PackageInfoGroups.Count];
            for (var i = 0; i < PackageInfoGroups.Count; i++)
                names[i] = PackageInfoGroups[i].Name;
            return names;
        }
    }
}