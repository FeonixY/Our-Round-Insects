using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PestControlController : LevelControllerBase
{
    public Transform BeetlePlayer;
    public List<GrassPreyTarget> PreyTargets = new();
    public int CaptureTarget = 3;
    public float CaptureDistance = 1.35f;
    public float IdleHintDelay = 7f;

    private int capturedCount;
    private float idleTimer;

    protected override void Start()
    {
        base.Start();

        PreyTargets ??= new List<GrassPreyTarget>();

        if (BeetlePlayer == null && Player.Instance != null)
        {
            BeetlePlayer = Player.Instance.transform;
        }

        if (PreyTargets.Count == 0)
        {
            PreyTargets.AddRange(FindObjectsByType<GrassPreyTarget>(FindObjectsSortMode.None));
        }

        if (HUD != null)
        {
            HUD.SetTitle("Grass Habitat: Beetle Hunt");
        }

        RefreshHUD();
        StartCoroutine(ShowIntroSequence());
    }

    private void Update()
    {
        if (IsCompleted)
        {
            return;
        }

        TryCaptureNearbyPrey();

        idleTimer += Time.deltaTime;
        if (idleTimer >= IdleHintDelay)
        {
            idleTimer = 0f;
            ShowHint("Move into a green lacewing to eat it.");
        }
    }

    public void TryCapturePrey(GrassPreyTarget prey)
    {
        if (prey == null || prey.IsCaptured || IsCompleted)
        {
            return;
        }

        idleTimer = 0f;
        prey.Capture();
        capturedCount++;

        ShowHint(capturedCount >= GetRequiredCaptures()
            ? "All green lacewings were eaten."
            : "Good catch. Keep hunting the remaining lacewings.");

        RefreshHUD();
        TryCompleteLevel();
    }

    private int GetRequiredCaptures()
    {
        return CaptureTarget;
    }

    private void RefreshHUD()
    {
        if (HUD == null)
        {
            return;
        }

        HUD.SetObjectives(
            $"Green lacewings eaten {capturedCount}/{GetRequiredCaptures()}",
            "Rule: touch each lacewing to eat it");
        HUD.SetState("Phase: Ground beetle patrol");
    }

    private void TryCompleteLevel()
    {
        if (capturedCount < GetRequiredCaptures())
        {
            return;
        }

        CompleteLevel(
            LevelSummary.Create(
                LevelId.Grass,
                "Grass Habitat Summary",
                "You guided the ground beetle to eat all three green lacewings. Predatory insects help keep the grass habitat balanced.",
                capturedCount,
                GetRequiredCaptures()),
            "Lacewings eaten",
            "Target",
            NotebookEntryId.GroundBeetle,
            NotebookEntryId.GreenLacewing);
    }

    private void TryCaptureNearbyPrey()
    {
        if (BeetlePlayer == null || PreyTargets == null)
        {
            return;
        }

        foreach (GrassPreyTarget prey in PreyTargets)
        {
            if (prey == null || prey.IsCaptured || !prey.gameObject.activeInHierarchy)
            {
                continue;
            }

            if (Vector3.Distance(BeetlePlayer.position, prey.transform.position) <= CaptureDistance)
            {
                TryCapturePrey(prey);
                return;
            }
        }
    }

    private IEnumerator ShowIntroSequence()
    {
        ShowHint("Control the beetle and hunt the green lacewings.");
        yield return new WaitForSeconds(2.2f);
        ShowHint("A question mark appears above each lacewing from time to time.");
        yield return new WaitForSeconds(2.2f);
        ShowHint("Touch all three lacewings to clear the level.");
    }
}
