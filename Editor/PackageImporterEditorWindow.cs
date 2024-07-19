using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Serialization;
using Yueby.ModalWindow;
using Yueby.Utils;

namespace Yueby.PackageImporter
{
    public class PackageImporterEditorWindow : EditorWindow
    {
        private static PackageImporterEditorWindow _window;
        private const string _key = "PackageImporterData";

        private PackageImporterData _packageImporterData;
        private ReorderableListDroppable _reorderableList;

        [MenuItem("Tools/YuebyTools/Utils/Package Importer")]
        public static void ShowWindow()
        {
            _window = GetWindow<PackageImporterEditorWindow>();
            _window.titleContent = new GUIContent("Package Importer");
        }

        private void OnEnable()
        {
            // EditorPrefs.DeleteKey(_key);
            // Debug.Log("OnEnable");
            var packageData = EditorPrefs.GetString(_key);
            _packageImporterData = new PackageImporterData();
            if (!string.IsNullOrEmpty(packageData))
                JsonUtility.FromJsonOverwrite(packageData, _packageImporterData);

            _packageImporterData.OnIndexChanged = InitReorderableList;
            InitReorderableList(0);
        }

        private void OnDisable()
        {
            // Save data
            SaveData();
        }

        private void InitReorderableList(int index)
        {
            _reorderableList = new ReorderableListDroppable(_packageImporterData.Current.Packages, typeof(PackageInfo), EditorGUIUtility.singleLineHeight, null, true, true, Repaint)
            {
                OnAdd = OnAdd,
                OnDraw = OnDraw,
                OnRemove = OnRemove,
            };

            // Search Package
            foreach (var package in _packageImporterData.Current.Packages.Where(package => package.IsWeb))
            {
                package.ListPackage();
            }
        }

        private float OnDraw(Rect rect, int index, bool arg3, bool arg4)
        {
            var height = EditorGUIUtility.singleLineHeight;
            var currentPackage = _packageImporterData.Current.Packages[index];
            var isWeb = currentPackage.IsWeb;

            GUI.Box(new Rect(rect.x, rect.y + 2, 45, height), isWeb ? "Web" : "Local", "Badge");

            var labelRect = new Rect(rect.x + 50 + 2, rect.y, rect.width, height);
            EditorGUI.LabelField(labelRect, new GUIContent(currentPackage.Name), EditorStyles.boldLabel);

            var style = new GUIStyle(EditorStyles.miniLabel)
            {
                normal =
                {
                    textColor = Color.gray
                }
            };
            var pathRect = new Rect(rect.x, labelRect.y + height + 2, rect.width - 22, height);
            EditorGUI.LabelField(pathRect, new GUIContent(currentPackage.Path), style);

            var descriptionRect = new Rect(rect.x, pathRect.y + height, rect.width - 22, height);
            EditorGUI.LabelField(descriptionRect, new GUIContent(currentPackage.Description), EditorStyles.miniLabel);

            var result = descriptionRect.y + descriptionRect.height - labelRect.y;

            // Edit
            if (GUI.Button(new Rect(rect.width - 61, rect.y + 2, 40, height), "Edit"))
            {
                var configure = new PackageInfoEditorDrawer(currentPackage);
                ModalEditorWindow.Show(configure, value =>
                {
                    if (!value) return;
                    _packageImporterData.Current.Packages[index] = configure.Data;
                    SaveData();
                });
                GUIUtility.ExitGUI();
            }

            // Move
            if (GUI.Button(new Rect(rect.width - 21, rect.y + 2, 20, height), "▲"))
            {
                var transferDrawer = new PackageInfoTransferDrawer(_packageImporterData.GetPackageGroupNames());
                transferDrawer.Index = _packageImporterData.CurrentIndex;
                ModalEditorWindow.Show(transferDrawer, () =>
                {
                    if (transferDrawer.Index == _packageImporterData.CurrentIndex) return;
                    _packageImporterData.PackageInfoGroups[transferDrawer.Index].Packages.Add(currentPackage);
                    _packageImporterData.Current.Packages.Remove(currentPackage);
                });
                GUIUtility.ExitGUI();
            }


            var label = isWeb ? currentPackage.IsInPackage ? "-" : "+" : "+";

            if (GUI.Button(new Rect(rect.width, rect.y + 2, 20, height), label))
            {
                if (!isWeb)
                    ImportPackage(currentPackage.Path);
                else
                {
                    if (currentPackage.IsInPackage)
                    {
                        ModalEditorWindow.ShowTip("Are you sure to remove this package?", onOk: () =>
                        {
                            currentPackage.RemoveFromPackage();
                            Debug.Log("Removing package: " + currentPackage.Name);
                        });
                    }
                    else
                    {
                        ModalEditorWindow.ShowTip("Are you sure to add this package?", onOk: () =>
                        {
                            currentPackage.AddInPackage();
                            Debug.Log("Adding package: " + currentPackage.Name);
                        });
                    }

                    GUIUtility.ExitGUI();
                }

                // Debug.Log("Import" + _packageImporterData.Packages[index].Name);
            }


            return result;
        }


        private void OnAdd(ReorderableList list)
        {
            var packageConfigureDrawer = new PackageInfoEditorDrawer();
            ModalEditorWindow.Show(packageConfigureDrawer, value =>
            {
                if (!value) return;
                var data = packageConfigureDrawer.Data;
                if (!string.IsNullOrEmpty(data.Path))
                {
                    // Debug.Log(data.Path);
                    if (string.IsNullOrEmpty(data.Description))
                        data.Description = "No description";

                    list.list.Add(data);

                    // _packageImporterData.Packages.Add(data);

                    SaveData();
                }
            });
        }

        private void OnRemove(ReorderableList list)
        {
            SaveData();
        }

        private void SaveData()
        {
            var json = JsonUtility.ToJson(_packageImporterData);
            // Debug.Log(json);
            EditorPrefs.SetString(_key, json);
        }

        private void OnGUI()
        {
            EditorUI.DrawEditorTitle("Package Importer");

            EditorUI.HorizontalEGL(() =>
            {
                EditorGUILayout.LabelField("Group", GUILayout.Width(60));
                // EditorGUI.BeginChangeCheck();
                // var index = EditorGUILayout.Popup(_packageImporterData.CurrentIndex, _packageImporterData.GetPackageGroupNames());
                // if (EditorGUI.EndChangeCheck())
                // {
                //     if (index != _packageImporterData.CurrentIndex)
                //         _packageImporterData.SetCurrent(index);
                // }

                if (GUILayout.Button(_packageImporterData.Current.Name))
                {
                    var groupSelectDrawer = new PackageInfoGroupSelectDrawer(_packageImporterData);
                    groupSelectDrawer.List.index = _packageImporterData.CurrentIndex;
                    ModalEditorWindow.Show(groupSelectDrawer, () =>
                    {
                        _packageImporterData.SetCurrent(groupSelectDrawer.List.index);
                        SaveData();
                    });
                    GUIUtility.ExitGUI();
                }

                // if (GUILayout.Button("Edit", GUILayout.Width(40)))
                // {
                //     var groupEditorDrawer = new PackageInfoGroupEditorDrawer(_packageImporterData.Current);
                //     ModalEditorWindow.Show(groupEditorDrawer, () =>
                //     {
                //         _packageImporterData.PackageInfoGroups[_packageImporterData.CurrentIndex] = groupEditorDrawer.Data;
                //         SaveData();
                //     });
                //     GUIUtility.ExitGUI();
                // }

                // if (GUILayout.Button("+", GUILayout.Width(20), GUILayout.Height(EditorGUIUtility.singleLineHeight)))
                // {
                //     var groupEditorDrawer = new PackageInfoGroupEditorDrawer();
                //     ModalEditorWindow.Show(groupEditorDrawer, () =>
                //     {
                //         _packageImporterData.PackageInfoGroups.Add(groupEditorDrawer.Data);
                //         _packageImporterData.SetCurrent(_packageImporterData.PackageInfoGroups.Count - 1);
                //         SaveData();
                //     });
                //     GUIUtility.ExitGUI();
                // }
                //
                // EditorGUI.BeginDisabledGroup(_packageImporterData.PackageInfoGroups.Count <= 1);
                // if (GUILayout.Button("-", GUILayout.Width(20), GUILayout.Height(EditorGUIUtility.singleLineHeight)))
                // {
                //     ModalEditorWindow.ShowTip("Are you sure to delete this group?", onOk: () =>
                //     {
                //         if (_packageImporterData.PackageInfoGroups.Count <= 1) return;
                //
                //         _packageImporterData.PackageInfoGroups.Remove(_packageImporterData.Current);
                //         _packageImporterData.SetCurrent(_packageImporterData.CurrentIndex - 1);
                //         SaveData();
                //     });
                //     GUIUtility.ExitGUI();
                // }
                //
                // EditorGUI.EndDisabledGroup();
            });

            // EditorGUILayout.Space();
            // _packageImporterData.IsInteractive = EditorUI.Radio(_packageImporterData.IsInteractive, "Interactive");
            // EditorGUILayout.Space();


            _reorderableList.DoLayoutList("Packages", new Vector2(0, 0), false, false, false);


            // if (GUILayout.Button("Import All", GUILayout.Height(EditorGUIUtility.singleLineHeight * 2)))
            //     ImportAllAssets();
        }

        private void ImportAllAssets()
        {
            if (_packageImporterData.Current.Packages.Count == 0)
            {
                Debug.Log("No package to import");
                return;
            }

            var unityPackages = new string[_packageImporterData.PackageInfoGroups.Count];
            for (var i = 0; i < _packageImporterData.PackageInfoGroups.Count; i++)
                unityPackages[i] = _packageImporterData.Current.Packages[i].Path;

            for (int i = 0; i < unityPackages.Length; i++)
            {
                if (unityPackages[i].EndsWith(".unitypackage"))
                {
                    // AssetDatabase.ImportPackage(unityPackages[i], _packageImporterData.IsInteractive);
                    AssetDatabase.ImportPackage(unityPackages[i], true);
                }
            }

            Debug.Log("Assets have been imported");
        }

        private void ImportPackage(string path)
        {
            if (path.EndsWith(".unitypackage"))
            {
                // AssetDatabase.ImportPackage(path, _packageImporterData.IsInteractive);
                AssetDatabase.ImportPackage(path, true);

                // Debug.Log("Package has been imported: " + path);
            }
        }
    }
}