﻿using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Yueby.ModalWindow;
using Yueby.Utils;

namespace Yueby.PackageImporter
{
    public class PackageInfoGroupSelectDrawer : ModalEditorWindowDrawer<PackageImporterData>
    {
        public override string Title => "Group Select";

        private readonly ReorderableListDroppable _reorderable;
        public ReorderableList List => _reorderable.List;

        public PackageInfoGroupSelectDrawer(PackageImporterData data)
        {
            Data = data;

            position.height = 200;
            _reorderable = new ReorderableListDroppable(Data.PackageInfoGroups, typeof(PackageInfoGroup), EditorGUIUtility.singleLineHeight, null)
            {
                OnDraw = DrawList,
                OnAdd = OnAdd,
                // OnRemove = OnRemove
            };
        }


        private void OnAdd(ReorderableList list)
        {
            var groupEditorDrawer = new PackageInfoGroupEditorDrawer();
            ModalEditorWindow.Show(groupEditorDrawer, () =>
            {
                Data.PackageInfoGroups.Add(groupEditorDrawer.Data);
                // Data.SetCurrent(Data.PackageInfoGroups.Count - 1);
            });
            GUIUtility.ExitGUI();
        }

        // private void OnRemove(ReorderableList list)
        // {
        //     ModalEditorWindow.ShowTip("Are you sure to delete this group?", onOk: () =>
        //     {
        //         if (Data.PackageInfoGroups.Count <= 1) return;
        //
        //         Data.PackageInfoGroups.Remove(Data.Current);
        //
        //
        //         Data.SetCurrent(Data.CurrentIndex - 1);
        //     });
        //     GUIUtility.ExitGUI();
        // }

        private float DrawList(Rect rect, int index, bool arg3, bool arg4)
        {
            EditorGUI.LabelField(rect, Data.PackageInfoGroups[index].Name);

            if (GUI.Button(new Rect(rect.width - 15, rect.y+2, 40, EditorGUIUtility.singleLineHeight), "Edit"))
            {
                var groupEditorDrawer = new PackageInfoGroupEditorDrawer(Data.PackageInfoGroups[index]);
                ModalEditorWindow.Show(groupEditorDrawer, () => { Data.PackageInfoGroups[index] = groupEditorDrawer.Data; }, showFocusCenter: true);
                GUIUtility.ExitGUI();
            }

            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnDraw()
        {
            _reorderable.DoLayout("", new Vector2(0, position.height), false, false, false);
        }
    }
}