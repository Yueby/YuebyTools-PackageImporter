using UnityEditor;
using UnityEngine;
using Yueby.ModalWindow;

namespace Yueby.PackageImporter
{
    public class PackageInfoTransferDrawer : ModalEditorWindowDrawer<string[]>
    {
        public override string Title => "Package Info Transfer";
        
        public int Index { get; set; } = 0;

        public PackageInfoTransferDrawer(string[] data)
        {
            Data = data;
        }

        public override void OnDraw()
        {
            EditorGUILayout.HelpBox("Please select the package group to transfer the package info to.", MessageType.Info);
            Index = EditorGUILayout.Popup("Package Group", Index, Data);
        }
    }
}