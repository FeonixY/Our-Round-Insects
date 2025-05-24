using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InputHintUIManager : SingletonMonoBehaviour<InputHintUIManager>
{
    public GameObject LayoutGroup;
    public GameObject InputHintPrefab;
    [ShowInInspector]
    public Dictionary<SceneType, List<string>> InputHints = new();

    private void Start()
    {
        ShowInputHint(SceneType.MainScene);
    }

    public void ShowInputHint(SceneType sceneType)
    {
        foreach (string hint in InputHints[sceneType])
        {
            GameObject inputHint = Instantiate(InputHintPrefab, LayoutGroup.transform);
            inputHint.GetComponent<TMP_Text>().SetText(hint);
        }
    }

    public enum SceneType
    {
        MainScene,
        PhotoScene,
        OtherScene
    }
}
