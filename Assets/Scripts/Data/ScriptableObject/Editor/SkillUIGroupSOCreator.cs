using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class SkillUIGroupSOCreator : EditorWindow
{
    private CharacterConfig _targetDataAsset;
    private string _newSkillName = "";
    private SkillUIGroupConfig _newConfigReference;

    private bool _includeWeeakAttack = true;
    private bool _includeStrongAttack = true;
    private bool _includeGroundedSkill = false;
    private bool _includeSpecialSkill = false;
    private bool _includeUltimateSkill = false;
    private bool _includeAirSkill = false;

    private Object _targetFolder;
    private string _targetFolderPath = "Assets/GameData/Attacks";

    [MenuItem("Tools/Skill UI Sub-Asset Creator")]
    public static void ShowWindow()
    {
        GetWindow<SkillUIGroupSOCreator>("스킬 UI 그룹 데이터 생성기");
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
        _newSkillName = EditorGUILayout.TextField("에셋 이름", _newSkillName);

        if (GUILayout.Button("에셋 생성 및 저장"))
        {
            if (ValidateInputs())
            {
                CreateEmbeddedAsset();
            }
        }


        EditorGUI.BeginDisabledGroup(true);
        _newConfigReference = (SkillUIGroupConfig)EditorGUILayout.ObjectField(
            "생성 에셋",
            _newConfigReference,
            typeof(SkillUIGroupConfig),
            false
        );
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("스킬 설정", EditorStyles.boldLabel);

        _includeWeeakAttack = EditorGUILayout.Toggle("기본 공격 포함", _includeWeeakAttack);
        _includeStrongAttack = EditorGUILayout.Toggle("강공격 포함", _includeStrongAttack);
        _includeGroundedSkill = EditorGUILayout.Toggle("기본 스킬 포함", _includeGroundedSkill);
        _includeSpecialSkill = EditorGUILayout.Toggle("특수 스킬 포함", _includeSpecialSkill);
        _includeUltimateSkill = EditorGUILayout.Toggle("궁극 스킬 포함", _includeUltimateSkill);
        _includeAirSkill = EditorGUILayout.Toggle("공중 스킬 포함", _includeAirSkill);
    }

    private bool ValidateInputs()
    {
        if (_targetDataAsset == null)
        {
            EditorUtility.DisplayDialog("경고", "메인 에셋 (CharacterStateData)을 선택해주세요.", "확인");
            return false;
        }

        if (string.IsNullOrEmpty(_newSkillName))
        {
            EditorUtility.DisplayDialog("경고", "유효한 이름을 입력해주세요.", "확인");
            return false;
        }

        Object[] existingSubAssets = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(_targetDataAsset));
        if (existingSubAssets.Any(asset => asset.name == _newSkillName && asset is AttackConfig))
        {
            EditorUtility.DisplayDialog("경고", $"'{_newSkillName}' 이름의 에셋이 이미 존재합니다.", "확인");
            return false;
        }

        return true;
    }

    private void CreateEmbeddedAsset()
    {
        string finalAssetPath = Path.Combine(_targetFolderPath, _newSkillName + ".asset");
        SkillUIGroupConfig newConfig = ScriptableObject.CreateInstance<SkillUIGroupConfig>();
        newConfig.name = _newSkillName;
        AssetDatabase.CreateAsset(newConfig, finalAssetPath);

        if (_includeWeeakAttack)
        {
            SkillUIConfig weeakAttack = ScriptableObject.CreateInstance<SkillUIConfig>();
            weeakAttack.name = $"WeekAttackSKill";
            AssetDatabase.AddObjectToAsset(weeakAttack, newConfig);
        }

        if (_includeStrongAttack)
        {
            SkillUIConfig strongAttack = ScriptableObject.CreateInstance<SkillUIConfig>();
            strongAttack.name = $"strongAttackSKill";
            AssetDatabase.AddObjectToAsset(strongAttack, newConfig);
        }

        if (_includeGroundedSkill)
        {
            SkillUIConfig groundedSkill = ScriptableObject.CreateInstance<SkillUIConfig>();
            groundedSkill.name = $"Skill";
            AssetDatabase.AddObjectToAsset(groundedSkill, newConfig);
        }

        if (_includeSpecialSkill)
        {
            SkillUIConfig specialSkill = ScriptableObject.CreateInstance<SkillUIConfig>();
            specialSkill.name = $"SpecialSkill";
            AssetDatabase.AddObjectToAsset(specialSkill, newConfig);
        }

        if (_includeUltimateSkill)
        {
            SkillUIConfig ultimateSkill = ScriptableObject.CreateInstance<SkillUIConfig>();
            ultimateSkill.name = $"UltimateSkill";
            AssetDatabase.AddObjectToAsset(ultimateSkill, newConfig);
        }

        if (_includeAirSkill)
        {
            SkillUIConfig airSkill = ScriptableObject.CreateInstance<SkillUIConfig>();
            airSkill.name = $"AirSkill";
            AssetDatabase.AddObjectToAsset(airSkill, newConfig);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.SetDirty(_targetDataAsset);
        Debug.Log($"생성 성공: {_targetDataAsset.name}/{_newSkillName}");        
    }
}
