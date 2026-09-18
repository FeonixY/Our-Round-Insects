using System;
using System.Collections.Generic;
using UnityEngine;

public class GameProgressManager : SingletonMonoBehaviour<GameProgressManager>
{
    public event Action ProgressChanged;

    public LevelSummary LastCompletedSummary { get; private set; }
    public NotebookPageId RequestedNotebookPage { get; private set; } = NotebookPageId.Intro;
    public bool OpenDictionaryOnNotebookOpen { get; private set; }

    private readonly HashSet<LevelId> completedLevels = new();
    private readonly HashSet<NotebookEntryId> unlockedEntries = new();

    protected override void Awake()
    {
        base.Awake();
        if (Instance != this)
        {
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    public void ResetProgress()
    {
        completedLevels.Clear();
        unlockedEntries.Clear();
        LastCompletedSummary = null;
        RequestedNotebookPage = NotebookPageId.Intro;
        OpenDictionaryOnNotebookOpen = false;
        RaiseProgressChanged();
    }

    public void CompleteLevel(LevelSummary summary, params NotebookEntryId[] entriesToUnlock)
    {
        if (summary == null)
        {
            return;
        }

        completedLevels.Add(summary.LevelId);
        LastCompletedSummary = summary;

        foreach (NotebookEntryId entryId in entriesToUnlock)
        {
            unlockedEntries.Add(NormalizeNotebookEntryId(entryId));
        }

        RaiseProgressChanged();
    }

    public bool IsLevelCompleted(LevelId levelId)
    {
        return completedLevels.Contains(levelId);
    }

    public int GetCompletedLevelCount()
    {
        return completedLevels.Count;
    }

    public bool AreAllLevelsCompleted()
    {
        return completedLevels.Count >= 3
            && completedLevels.Contains(LevelId.Flower)
            && completedLevels.Contains(LevelId.Grass)
            && completedLevels.Contains(LevelId.Night);
    }

    public void UnlockNotebookEntry(NotebookEntryId entryId)
    {
        if (unlockedEntries.Add(NormalizeNotebookEntryId(entryId)))
        {
            RaiseProgressChanged();
        }
    }

    public bool IsNotebookEntryUnlocked(NotebookEntryId entryId)
    {
        return unlockedEntries.Contains(NormalizeNotebookEntryId(entryId));
    }

    public int GetUnlockedNotebookCount()
    {
        return unlockedEntries.Count;
    }

    public void RequestNotebookPage(NotebookPageId pageId, bool openDictionary = false)
    {
        RequestedNotebookPage = pageId;
        OpenDictionaryOnNotebookOpen = openDictionary;
    }

    public void ClearNotebookOverlayRequest()
    {
        OpenDictionaryOnNotebookOpen = false;
    }

    private static NotebookEntryId NormalizeNotebookEntryId(NotebookEntryId entryId)
    {
        return entryId switch
        {
            NotebookEntryId.Ladybug => NotebookEntryId.GroundBeetle,
            NotebookEntryId.Dragonfly => NotebookEntryId.GreenLacewing,
            NotebookEntryId.DungBeetle => NotebookEntryId.GroundBeetle,
            _ => entryId
        };
    }

    private void RaiseProgressChanged()
    {
        ProgressChanged?.Invoke();
    }
}
