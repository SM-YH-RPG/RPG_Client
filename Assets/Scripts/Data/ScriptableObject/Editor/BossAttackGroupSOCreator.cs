using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class BossAttackGroupSOCreator : EditorWindow
{
    private BossEnemyConfig _targetDataAsset;
    private string _newAttackName = "";
    private BossAttackGroupConfig _newConfigReference;    

    private Object _targetFolder;
    private string _targetFolderPath = "Assets/GameData/Attacks";

    [MenuItem("Tools/Boss AttackGroup Creator")]
    public static void ShowWindow()
    {
        GetWindow<BossAttackGroupSOCreator>("Boss공격 그룹 데이터 생성기");
    }

    private void OnEnable()
    {
        if (Selection.activeObject is BossEnemyConfig selectedAsset)
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


        _targetDataAsset = (BossEnemyConfig)EditorGUILayout.ObjectField(
            "메인 에셋",
            _targetDataAsset,
            typeof(BossEnemyConfig),
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
        _newConfigReference = (BossAttackGroupConfig)EditorGUILayout.ObjectField(
            "생성 에셋",
            _newConfigReference,
            typeof(BossAttackGroupConfig),
            false
        );
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.Space();
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
        BossAttackGroupConfig newConfig = ScriptableObject.CreateInstance<BossAttackGroupConfig>();
        newConfig.name = _newAttackName;
        AssetDatabase.CreateAsset(newConfig, finalAssetPath);

        AttackConfig biteAttack = ScriptableObject.CreateInstance<AttackConfig>();
        biteAttack.name = $"BiteAttack";
        AssetDatabase.AddObjectToAsset(biteAttack, newConfig);

        AttackConfig clawAttack = ScriptableObject.CreateInstance<AttackConfig>();
        clawAttack.name = $"ClawAttack";
        AssetDatabase.AddObjectToAsset(clawAttack, newConfig);

        AttackConfig headAttack = ScriptableObject.CreateInstance<AttackConfig>();
        headAttack.name = $"HeadAttack";
        AssetDatabase.AddObjectToAsset(headAttack, newConfig);        

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.SetDirty(_targetDataAsset);
        Debug.Log($"생성 성공: {_targetDataAsset.name}/{_newAttackName}");
    }
}
