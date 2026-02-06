using System;
using System.Collections.Generic;
using UnityEngine;

public class TargetSelector : MonoBehaviour
{
    public static TargetSelector Instance;

    private List<CharacterBattle> availableTargets;
    private Action<CharacterBattle> onTargetSelected;

    [Header("UI")]
    public GameObject targetIndicatorPrefab; 
    private List<GameObject> targetIndicators = new List<GameObject>();

    void Awake()
    {
        Instance = this;
    }

    public void StartTargetSelection(List<CharacterBattle> targets, Action<CharacterBattle> callback)
    {
        availableTargets = targets;
        onTargetSelected = callback;

        ShowTargetIndicators();
    }

    void ShowTargetIndicators()
    {
        ClearIndicators();

        foreach (var target in availableTargets)
        {
            if (targetIndicatorPrefab != null)
            {
                GameObject indicator = Instantiate(targetIndicatorPrefab, target.transform.position + Vector3.up * 2f, Quaternion.identity);
                indicator.transform.SetParent(target.transform);
                targetIndicators.Add(indicator);

                var button = indicator.AddComponent<UnityEngine.UI.Button>();
                CharacterBattle capturedTarget = target; 
                button.onClick.AddListener(() => SelectTarget(capturedTarget));
            }
        }
    }

    void SelectTarget(CharacterBattle target)
    {
        ClearIndicators();
        onTargetSelected?.Invoke(target);
    }

    void ClearIndicators()
    {
        foreach (var indicator in targetIndicators)
        {
            Destroy(indicator);
        }
        targetIndicators.Clear();
    }
}
