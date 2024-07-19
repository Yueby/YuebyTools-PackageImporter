using System;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.Serialization;

namespace Yueby.PackageImporter
{
    [Serializable]
    public class PackageInfo
    {
        public string Name;
        public string Path;
        public string Description;

        public string PackageId;

        private string _searchedPackageName;

        public bool IsWeb => Path.StartsWith("http://") || Path.StartsWith("https://");

        public bool IsInPackage => !string.IsNullOrEmpty(_searchedPackageName);

        public PackageInfo()
        {
        }

        public PackageInfo(string name, string path, string description)
        {
            Name = name;
            Path = path;
            Description = description;
        }

        private AddRequest _request;

        public void AddInPackage()
        {
            _request = Client.Add(Path);
            EditorApplication.update += AddProgress;
        }

        private void AddProgress()
        {
            if (!_request.IsCompleted) return;
            switch (_request.Status)
            {
                case StatusCode.Success:
                    PackageId = _request.Result.packageId;
                    Debug.Log("Added package: " + PackageId);
                    ListPackage();
                    break;
                case >= StatusCode.Failure:
                    Debug.Log(_request.Error.message);
                    PackageId = "";
                    break;
            }

            EditorApplication.update -= AddProgress;
            _request = null;
        }

        private ListRequest _listRequest;

        public void ListPackage()
        {
            if (string.IsNullOrEmpty(PackageId))
            {
                PackageId = Name;
            }

            _listRequest = Client.List(true);
            EditorApplication.update += ListProgress;
        }

        private void ListProgress()
        {
            // Debug.Log("Listing packages: " + PackageId);
            if (!_listRequest.IsCompleted) return;
            switch (_listRequest.Status)
            {
                case StatusCode.Success:
                    // Debug.Log("Successfully listed packages: " + PackageId);
                    foreach (var package in _listRequest.Result)
                    {
                        // Debug.Log(package.packageId);
                        if (package.packageId == PackageId)
                        {
                            _searchedPackageName = package.name;
                            Debug.Log("Found package: " + _searchedPackageName);
                            break;
                        }
                    }

                    break;
                case >= StatusCode.Failure:
                    Debug.Log(_listRequest.Error.message);
                    _searchedPackageName = "";
                    break;
            }

            EditorApplication.update -= ListProgress;
            _listRequest = null;
        }

        private RemoveRequest _removeRequest;

        public void RemoveFromPackage()
        {
            _removeRequest = Client.Remove(PackageId.Split('@')[0]);
            EditorApplication.update += RemoveProgress;
        }

        private void RemoveProgress()
        {
            if (!_removeRequest.IsCompleted) return;
            switch (_removeRequest.Status)
            {
                case StatusCode.Success:
                    Debug.Log("Removed package: " + PackageId);
                    PackageId = "";
                    break;
                case >= StatusCode.Failure:
                    Debug.Log(_removeRequest.Error.message);
                    break;
            }

            EditorApplication.update -= RemoveProgress;
            _removeRequest = null;
        }
    }
}