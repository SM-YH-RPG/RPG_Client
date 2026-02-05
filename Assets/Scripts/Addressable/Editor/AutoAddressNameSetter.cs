using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

[InitializeOnLoad]
public class AutoAddressNameSetter
{
    static AutoAddressNameSetter()
    {
        AddressableAssetSettingsDefaultObject.Settings.OnModification += OnSettingsModified;
    }

    private static void OnSettingsModified(AddressableAssetSettings settings, AddressableAssetSettings.ModificationEvent modificationEvent, object eventData)
    {
        if (modificationEvent != AddressableAssetSettings.ModificationEvent.EntryAdded &&
            modificationEvent != AddressableAssetSettings.ModificationEvent.EntryModified &&
            modificationEvent != AddressableAssetSettings.ModificationEvent.EntryCreated)
            return;

        foreach (AddressableAssetGroup group in settings.groups)
        {
            if (group.Name == "Default Local Group" || group.Name == "Built In Data")
                continue;

            foreach (AddressableAssetEntry entry in group.entries)
            {
                if (entry.address.Contains("/") || entry.address.Contains("\\"))
                {
                    string fileName = System.IO.Path.GetFileNameWithoutExtension(entry.AssetPath);

                    if (entry.address != fileName)
                    {
                        entry.address = fileName;
                        EditorUtility.SetDirty(settings);
                    }
                }
            }
        }
    }
}