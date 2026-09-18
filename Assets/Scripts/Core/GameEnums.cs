public enum LevelId
{
    Flower = 0,
    Grass = 1,
    Night = 2
}

public enum NotebookEntryId
{
    Bee = 0,
    Hoverfly = 1,
    GroundBeetle = 2,
    GreenLacewing = 3,
    DungBeetle = 4,
    Firefly = 5,
    Ladybug = GroundBeetle,
    Dragonfly = GreenLacewing
}

public enum EcologicalRole
{
    Pollinator = 0,
    PestController = 1,
    Decomposer = 2,
    IndicatorSpecies = 3
}

public enum NotebookPageId
{
    Intro = 0,
    LevelSelect = 1,
    LevelResult = 2,
    FinalReview = 3,
    EndSequence = 4
}
