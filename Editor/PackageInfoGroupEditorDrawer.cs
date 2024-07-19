using System.IO;
using UnityEditor;
using UnityEngine;
using Yueby.ModalWindow;
using Yueby.Utils;

namespace Yueby.PackageImporter
{
    public class PackageInfoGroupEditorDrawer : ModalEditorWindowDrawer<PackageInfoGroup>
    {
        public override string Title => "PackageInfoGroup Editor";

        public PackageInfoGroupEditorDrawer()
        {
            Data = new PackageInfoGroup
            {
                Name = "New PackageInfoGroup"
            };
        }

        public PackageInfoGroupEditorDrawer(PackageInfoGroup data)
        {
            Data = data;
        }


        public override void OnDraw()
        {
            const int labelWidth = 80;

            EditorGUILayout.HelpBox("You can edit the package info group name here.", MessageType.Info);
            Data.Name = EditorUI.TextField("Name", Data.Name, labelWidth: labelWidth);
        }
    }
}