using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using System.IO;

public class ContentUpdateCheckerWindow : EditorWindow
{
    private string stateFilePath = "";
    private List<AddressableAssetEntry> changedEntries = new List<AddressableAssetEntry>();
    private Vector2 scrollPosition;

    [MenuItem("Window/Asset Management/번들 업데이트 검사기")]
    public static void ShowWindow()
    {
        GetWindow<ContentUpdateCheckerWindow>("번들 업데이트 검사기");
    }

    private void OnGUI()
    {
        GUILayout.Label("번들 업데이트 검사기", EditorStyles.boldLabel);

        EditorGUILayout.Space();

        if (GUILayout.Button("이전 빌드파일 선택 (.bin)"))
        {
            stateFilePath = EditorUtility.OpenFilePanel("Select addressables_content_state.bin", "", "bin");
            if (!string.IsNullOrEmpty(stateFilePath))
            {
                CheckForChanges();
            }
        }

        EditorGUILayout.TextField("빌드파일 경로", stateFilePath);

        EditorGUILayout.Space();

        if (changedEntries.Count > 0)
        {
            EditorGUILayout.HelpBox($"총 {changedEntries.Count}개의 애셋이 마지막 빌드 이후 변경되었습니다.", MessageType.Warning);

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            for (int i = 0; i < changedEntries.Count; i++)
            {
                AddressableAssetEntry entry = changedEntries[i];
                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                EditorGUILayout.LabelField(entry.address, EditorStyles.miniLabel);

                if (GUILayout.Button("에셋 선택", GUILayout.Width(100)))
                {
                    Selection.activeObject = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(entry.AssetPath);
                    EditorGUIUtility.PingObject(Selection.activeObject);
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();
        }
        else if (string.IsNullOrEmpty(stateFilePath) == false)
        {
            EditorGUILayout.HelpBox("마지막 빌드 이후 변경된 애셋이 없습니다.", MessageType.Info);
        }
    }

    private void CheckForChanges()
    {
        changedEntries.Clear();

        if (string.IsNullOrEmpty(stateFilePath) || File.Exists(stateFilePath) == false)
        {
            return;
        }

        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null)
        {
            Debug.LogError("AddressableAssetSettings을 찾을 수 없습니다.");
            return;
        }

        List<AddressableAssetEntry> modifiedEntries = ContentUpdateScript.GatherModifiedEntries(settings, stateFilePath);
        if (modifiedEntries == null)
        {
            Debug.LogError("ContentUpdateScript.GatherModifiedEntries가 null을 반환했습니다. Addressables 설정이나 패키지 버전 문제일 수 있습니다.");
        }
        else
        {
            Debug.Log($"GatherModifiedEntries 호출 완료. 감지된 변경사항 수: {modifiedEntries.Count}");
            changedEntries = modifiedEntries;
        }

        Repaint();
    }
}