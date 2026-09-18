public static class GameSceneNames
{
    public const string MainMenu = "MainMenu";
    public const string Notebook = "NotebookScene";
    public const string Start = Notebook;
    public const string LevelSelect = Notebook;
    public const string Flower = "FlowerScene";
    public const string Grass = "GrassScene";
    public const string Night = "NightScene";
    public const string End = Notebook;

    public static string GetLevelSceneName(LevelId levelId)
    {
        return levelId switch
        {
            LevelId.Flower => Flower,
            LevelId.Grass => Grass,
            LevelId.Night => Night,
            _ => LevelSelect
        };
    }
}
