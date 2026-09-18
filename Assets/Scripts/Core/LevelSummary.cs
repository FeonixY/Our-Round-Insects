using System;

[Serializable]
public class LevelSummary
{
    public LevelId LevelId;
    public string Title;
    public string SummaryText;
    public int PrimaryScore;
    public int SecondaryScore;

    public static LevelSummary Create(LevelId levelId, string title, string summaryText, int primaryScore, int secondaryScore)
    {
        return new LevelSummary
        {
            LevelId = levelId,
            Title = title,
            SummaryText = summaryText,
            PrimaryScore = primaryScore,
            SecondaryScore = secondaryScore
        };
    }
}
