using System.IO;
using UnityEditor;
using UnityEngine;
using Yueby.ModalWindow;
using Yueby.Utils;

namespace Yueby.PackageImporter
{
    public class PackageInfoEditorDrawer : ModalEditorWindowDrawer<PackageInfo>
    {
        public override string Title => "PackageInfo Editor";

        private const string _packagePathKey = "PackageImporter_LastOpenPackagePath";

        public PackageInfoEditorDrawer()
        {
            Data = new PackageInfo();
        }

        public PackageInfoEditorDrawer(PackageInfo packageInfo)
        {
            Data = packageInfo;
        }

        public override void OnDraw()
        {
            const int labelWidth = 80;

            EditorUI.HorizontalEGL(() =>
            {
                EditorGUI.BeginChangeCheck();
                Data.Path = EditorUI.TextField("Path", Data.Path, labelWidth);

                if (EditorGUI.EndChangeCheck())
                    Data.Path = Data.Path.Trim();
                
                if (GUILayout.Button("Browse"))
                {
                    var lastOpenPackagePath = EditorPrefs.GetString(_packagePathKey, "Assets");

                    var result = EditorUtility.OpenFilePanel("Select Package", lastOpenPackagePath, "unitypackage");
                    if (!string.IsNullOrEmpty(result))
                    {
                        Data.Path = result.Trim();
                        var file = new FileInfo(result);
                        Data.Name = file.Name.Replace(file.Extension, "");

                        EditorPrefs.SetString(_packagePathKey, file.Directory.FullName);
                    }
                }
            });

            Data.Name = EditorUI.TextField("Name", Data.Name, labelWidth: labelWidth);
            Data.Description = EditorUI.TextField("Description", Data.Description, labelWidth);
        }
    }
}