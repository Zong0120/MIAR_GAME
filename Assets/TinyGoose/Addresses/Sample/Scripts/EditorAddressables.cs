#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
#endif

using System.Diagnostics;
using UnityEngine;

namespace MyGame
{
    // -----------------------------------------------------------------------------------------------------------------------------
    //		You don't need this class in your game. It's only here to set up Addressables
    //      for the sample!!
    //
    //      Sadly Asset Store packages cannot easily contain Addressable groups, so we 
    //      set them up here just before we need them. 
    //      
    //      Again, this is NOT required in normal usage!!
    // -----------------------------------------------------------------------------------------------------------------------------
    public static class EditorAddressables
    {
        [Conditional("UNITY_EDITOR")]
        public static void SetUp()
        {
#if UNITY_EDITOR
            // No Addressables at all? Set up!
            if (!AddressableAssetSettingsDefaultObject.SettingsExists)
            {
                AddressableAssetSettingsDefaultObject.Settings = AddressableAssetSettings.Create(AddressableAssetSettingsDefaultObject.kDefaultConfigFolder, AddressableAssetSettingsDefaultObject.kDefaultConfigAssetName, true, true);
                EditorApplication.ExitPlaymode();
                EditorApplication.EnterPlaymode();
            }
            
            // Default group
            AddAddressable(null, "P_Light", "Assets/TinyGoose/Addresses/Sample/Prefabs/P_Light.prefab");
            AddAddressable(null, "M_Sample", "Assets/TinyGoose/Addresses/Sample/Materials/M_Sample.mat");

            // "Other Cool Group"
            AddAddressable("Other Cool Group", "M_Sample_InOtherGroup", "Assets/TinyGoose/Addresses/Sample/Materials/M_Sample_InOtherGroup.mat");

            // "Folders"
            AddAddressable("Folders", "MiscAddressables", "Assets/TinyGoose/Addresses/Sample/MiscAddressables");
#endif
        }

#if UNITY_EDITOR
        private static AddressableAssetGroup CreateOrFindGroup(string groupNameOrNull)
        {
            if (groupNameOrNull == null)
                return AddressableAssetSettingsDefaultObject.Settings.DefaultGroup;
            
            AddressableAssetGroup assetGroup = AddressableAssetSettingsDefaultObject.Settings.FindGroup(groupNameOrNull);
            if (!assetGroup)
            {
                assetGroup = AddressableAssetSettingsDefaultObject.Settings.CreateGroup(groupNameOrNull, false, false, true, AddressableAssetSettingsDefaultObject.Settings.DefaultGroup.Schemas, AddressableAssetSettingsDefaultObject.Settings.DefaultGroup.SchemaTypes.ToArray());
            }

            return assetGroup;

        }
        private static void AddAddressable(string groupNameOrNull, string stringAddress, string assetPath)
        {
            AddressableAssetGroup group = CreateOrFindGroup(groupNameOrNull);
            if (!group)
                return;
            
            string assetGuid = AssetDatabase.AssetPathToGUID(assetPath);
            
            AddressableAssetEntry entry = AddressableAssetSettingsDefaultObject.Settings.CreateOrMoveEntry(assetGuid, group);
            entry.address = stringAddress;
        }
#endif
    }
}