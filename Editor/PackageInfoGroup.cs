using System;
using System.Collections.Generic;

namespace Yueby.PackageImporter
{
    [Serializable]
    public class PackageInfoGroup
    {
        public string Name;
        public List<PackageInfo> Packages;


        public PackageInfoGroup()
        {
            Packages = new List<PackageInfo>();
        }

        public PackageInfoGroup(string name, List<PackageInfo> packages)
        {
            Name = name;
            Packages = packages;
        }
    }
}