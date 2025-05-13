﻿using Quest;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
public class DayManager : MonoBehaviour
{
    public static bool nextLoadIsNewDay = false;

    [Header("UI")]
    [SerializeField] private Button newDayButton;

    private void Awake()
    {
        if (newDayButton != null)
            newDayButton.onClick.AddListener(ShowEndOfDay);
        else
            Debug.LogWarning("DayManager: newDayButton не назначена!");

    }

    private void Update()
    {
        CheckNewDayConditions();
    }
    public void CheckNewDayConditions()
    {

        // Ищем главный Prime-квест, у которого ВСЕ задачи отмечены IsDone == true
        var completedPrimeQuest = QuestCollection.GetAllQuestGroups()
            .FirstOrDefault(q =>
                q.Prime
                && q.IsEneded()
            );

        newDayButton.gameObject.SetActive(completedPrimeQuest != null);
    }

    private void ShowEndOfDay()
    {
        LoadingScreenManager.Instance.ShowEndOfDayPanel();
    }


}
