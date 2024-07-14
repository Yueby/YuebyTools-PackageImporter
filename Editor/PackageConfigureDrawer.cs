using System.IO;
using UnityEditor;
using UnityEngine;
using Yueby.ModalWindow;
using Yueby.Utils;

namespace Yueby.PackageImporter
{
    public class PackageConfigureDrawer : ModalEditorWindowDrawer<PackageInfo>
    {
        public override string Title => "Package Configure";

        private const string _packagePathKey = "PackageImporter_LastOpenPackagePath";

        public PackageConfigureDrawer() : base()
        {
            Data = new PackageInfo();
        }

        public override void OnDraw()
        {
            const int labelWidth = 80;

            EditorUI.HorizontalEGL(() =>
            {
                Data.Path = EditorUI.TextField("Path", Data.Path, labelWidth);
                if (GUILayout.Button("Browse"))
                {
                    var lastOpenPackagePath = EditorPrefs.GetString(_packagePathKey, "Assets");

                    var result = EditorUtility.OpenFilePanel("Select Package", lastOpenPackagePath, "unitypackage");
                    if (!string.IsNullOrEmpty(result))
                    {
                        Data.Path = result;
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