using System.Text;
using TMPro;
using UnityEngine;

public class MissionUIManager : SingletonMonoBehaviour<MissionUIManager>
{
    public string Header = "Current Missions:\n";
    public TMP_Text MissionText;

    public void AddMission(string mission)
    {
        var sb = new StringBuilder();
        sb.Append(Header);
        sb.Append("  - ");
        sb.Append(mission);
        sb.Append('\n');
        MissionText.SetText(sb.ToString());
    }
}
