using UnityEngine;

[CreateAssetMenu(fileName = "InsectData", menuName = "Insect Data", order = 1)]
public class InsectData : ScriptableObject
{
    public NotebookEntryId EntryId;
    public string DisplayName;
    [TextArea(2, 4)]
    public string ShortDescription;
    public EcologicalRole EcologicalRole;
    [TextArea(2, 4)]
    public string EcologicalContribution;
    [TextArea(2, 4)]
    public string FunFact;
    [TextArea(2, 4)]
    public string ObservationSuggestion;
    public Sprite Image;
    public Sprite InformationImage;
}
