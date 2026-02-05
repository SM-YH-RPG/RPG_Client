using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class AttackGroupSOCreator : EditorWindow
{
    private CharacterConfig _targetDataAsset;
    private string _newAttackName = "";
    private AttackGroupConfig _newConfigReference;
    private AttackGroupConfig _selectedConfigToDelete;

    private int _weakComboCount = 1;
    private int _strongComboCount = 1;
    
    private bool _includeFallingAttack = false;
    private bool _includeAirAttack = false;

    private Object _targetFolder;
    private string _targetFolderPath = "Assets/GameData/Attacks";

    [MenuItem("Tools/Sub-Asset Creator")]
    public static void ShowWindow()
    {
        GetWindow<AttackGroupSOCreator>("공격 그룹 데이터 생성기");
    }

    private void OnEnable()
    {
        if (Selection.activeObject is CharacterConfig selectedAsset)
        {
            _targetDataAsset = selectedAsset;
        }
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("저장 위치", EditorStyles.boldLabel);
        _targetFolder = EditorGUILayout.ObjectField("저장 폴더", _targetFolder, typeof(DefaultAsset), false);
        if (_targetFolder != null)
        {
            _targetFolderPath = AssetDatabase.GetAssetPath(_targetFolder);
            if (Path.GetExtension(_targetFolderPath) != "")
            {
                _targetFolderPath = Path.GetDirectoryName(_targetFolderPath);
            }
        }


        _targetDataAsset = (CharacterConfig)EditorGUILayout.ObjectField(
            "메인 에셋",
            _targetDataAsset,
            typeof(CharacterConfig),
            false
        );


        if (_targetDataAsset == null)
        {
            EditorGUILayout.HelpBox("메인 에셋을 선택해 주세요.", MessageType.Info);
            return;
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("새 데이터 에셋 생성", EditorStyles.boldLabel);
        _newAttackName = EditorGUILayout.TextField("에셋 이름", _newAttackName);

        if (GUILayout.Button("에셋 생성 및 저장"))
        {
            if (ValidateInputs())
            {
                CreateEmbeddedAsset();
            }
        }


        EditorGUI.BeginDisabledGroup(true);
        _newConfigReference = (AttackGroupConfig)EditorGUILayout.ObjectField(
            "생성 에셋",
            _newConfigReference,
            typeof(AttackGroupConfig),
            false
        );
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("공격 설정", EditorStyles.boldLabel);

        _weakComboCount = EditorGUILayout.IntField("약한 공격 콤보 수", _weakComboCount);
        _strongComboCount = EditorGUILayout.IntField("강한 공격 콤보 수", _strongComboCount);
        _includeFallingAttack = EditorGUILayout.Toggle("낙하 공격 포함", _includeFallingAttack);
        _includeAirAttack = EditorGUILayout.Toggle("공중 공격 포함", _includeAirAttack);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("기존 에셋 삭제", EditorStyles.boldLabel);

        _selectedConfigToDelete = (AttackGroupConfig)EditorGUILayout.ObjectField(
            "삭제할 에셋 선택",
            _selectedConfigToDelete,
            typeof(AttackGroupConfig),
            false
        );

        if (GUILayout.Button("선택한 에셋 삭제"))
        {
            if (_selectedConfigToDelete != null)
            {
                DeleteEmbeddedAsset(_selectedConfigToDelete);
            }
            else
            {
                EditorUtility.DisplayDialog("경고", "삭제할 에셋을 선택해주세요.", "확인");
            }
        }
    }

    private bool ValidateInputs()
    {
        if (_targetDataAsset == null)
        {
            EditorUtility.DisplayDialog("경고", "메인 에셋 (CharacterStateData)을 선택해주세요.", "확인");
            return false;
        }

        if (string.IsNullOrEmpty(_newAttackName))
        {
            EditorUtility.DisplayDialog("경고", "유효한 이름을 입력해주세요.", "확인");
            return false;
        }

        Object[] existingSubAssets = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(_targetDataAsset));
        if (existingSubAssets.Any(asset => asset.name == _newAttackName && asset is AttackConfig))
        {
            EditorUtility.DisplayDialog("경고", $"'{_newAttackName}' 이름의 에셋이 이미 존재합니다.", "확인");
            return false;
        }

        return true;
    }

    private void CreateEmbeddedAsset()
    {
        string finalAssetPath = Path.Combine(_targetFolderPath, _newAttackName + ".asset");
        AttackGroupConfig newConfig = ScriptableObject.CreateInstance<AttackGroupConfig>();
        newConfig.name = _newAttackName;
        AssetDatabase.CreateAsset(newConfig, finalAssetPath);

        for(int i = 0; i < _weakComboCount; i++)
        {
            AttackConfig weakAttack = ScriptableObject.CreateInstance<AttackConfig>();
            weakAttack.name = $"WeakAttack{i + 1}";
            AssetDatabase.AddObjectToAsset(weakAttack, newConfig);
        }

        for(int i = 0; i < _strongComboCount; i++)
        {
            AttackConfig strongAttack = ScriptableObject.CreateInstance<AttackConfig>();
            strongAttack.name = $"StrongAttack{i + 1}";
            AssetDatabase.AddObjectToAsset(strongAttack, newConfig);
        }

        if(_includeFallingAttack)
        {
            AttackConfig fallingAttack = ScriptableObject.CreateInstance<AttackConfig>();
            fallingAttack.name = $"FallingAttack";
            AssetDatabase.AddObjectToAsset(fallingAttack, newConfig);
        }

        if(_includeAirAttack)
        {
            AttackConfig airAttack = ScriptableObject.CreateInstance<AttackConfig>();
            airAttack.name = $"AirAttack";
            AssetDatabase.AddObjectToAsset(airAttack, newConfig);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.SetDirty(_targetDataAsset);
        Debug.Log($"생성 성공: {_targetDataAsset.name}/{_newAttackName}");
    }

    private void DeleteEmbeddedAsset(AttackGroupConfig configToDelete)
    {
        string assetPath = AssetDatabase.GetAssetPath(_targetDataAsset);
        if (AssetDatabase.GetAssetPath(configToDelete) != assetPath)
        {
            EditorUtility.DisplayDialog("오류", "선택한 에셋이 메인 에셋 파일에 속해있지 않습니다.", "확인");
            _selectedConfigToDelete = null;
            return;
        }

        // Ask for confirmation
        if (!EditorUtility.DisplayDialog("확인 필요",
            $"정말로 '{configToDelete.name}' 에셋을 삭제하시겠습니까?\n\n" +
            "이 작업은 CharacterStateData 내의 참조도 자동으로 끊습니다.", "삭제", "취소"))
        {
            return;
        }

        SerializedObject so = new SerializedObject(_targetDataAsset);

        RemoveReferenceFromList(so.FindProperty("_weakAttack"), configToDelete);

        so.ApplyModifiedProperties();

        AssetDatabase.RemoveObjectFromAsset(configToDelete);
        DestroyImmediate(configToDelete, true);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        _selectedConfigToDelete = null;
    }

    private void RemoveReferenceFromList(SerializedProperty listProperty, AttackGroupConfig configToRemove)
    {
        if (listProperty != null && listProperty.isArray)
        {
            for (int i = 0; i < listProperty.arraySize; i++)
            {
                SerializedProperty element = listProperty.GetArrayElementAtIndex(i);
                if (element.objectReferenceValue == configToRemove)
                {
                    element.objectReferenceValue = null;
                    listProperty.DeleteArrayElementAtIndex(i);
                    i--;
                }
            }
        }
    }
}