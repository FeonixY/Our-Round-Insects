using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerPollinationController : LevelControllerBase
{
    public int NectarTarget = 2;
    public int PollinationTarget = 2;
    public float IdleHintDelay = 8f;

    private int pollenCollected;
    private int pollinationCount;
    private bool awaitingPollination;
    private float idleTimer;
    private bool firstPickupHintShown;
    private bool firstPollinationHintShown;
    private FlowerTarget currentSourceFlower;
    private FlowerTarget[] flowers = System.Array.Empty<FlowerTarget>();

    protected override void Start()
    {
        base.Start();

        if (HUD != null)
        {
            HUD.SetTitle("Flower Habitat: Pollination");
        }

        flowers = FindObjectsByType<FlowerTarget>(FindObjectsSortMode.None);
        PickNextSourceFlower(null);
        RefreshHUD();
        StartCoroutine(ShowIntroSequence());
    }

    private void Update()
    {
        if (IsCompleted)
        {
            return;
        }

        idleTimer += Time.deltaTime;

        if (idleTimer >= IdleHintDelay)
        {
            idleTimer = 0f;
            ShowHint(awaitingPollination
                ? "Touch one of the bright flowers to pollinate it."
                : "Touch the yellow flower to collect pollen.");
        }
    }

    public void TryVisitFlower(FlowerTarget flower)
    {
        if (flower == null || IsCompleted)
        {
            return;
        }

        idleTimer = 0f;

        if (!awaitingPollination)
        {
            TryCollectPollen(flower);
        }
        else
        {
            TryPollinateFlower(flower);
        }

        RefreshHUD();
        TryCompleteLevel();
    }

    private void TryCollectPollen(FlowerTarget flower)
    {
        if (flower != currentSourceFlower)
        {
            ShowHint("Touch the yellow flower to collect pollen.");
            return;
        }

        if (!flower.TryCollectNectar())
        {
            return;
        }

        pollenCollected++;
        awaitingPollination = true;
        ApplyFlowerStates();

        if (!firstPickupHintShown)
        {
            firstPickupHintShown = true;
            ShowHint("Pollen collected. Touch one of the bright flowers.");
            return;
        }

        ShowHint("Touch one of the bright flowers to pollinate it.");
    }

    private void TryPollinateFlower(FlowerTarget flower)
    {
        if (flower == currentSourceFlower)
        {
            ShowHint("That flower is empty. Touch one of the bright flowers.");
            return;
        }

        pollinationCount++;
        awaitingPollination = false;
        flower.PlayPollinationFeedback();

        if (!firstPollinationHintShown)
        {
            firstPollinationHintShown = true;
            ShowHint("Pollination complete. Another flower now carries pollen.");
        }

        PickNextSourceFlower(flower);
    }

    private void RefreshHUD()
    {
        if (HUD == null)
        {
            return;
        }

        HUD.SetObjectives(
            $"Pollen pickups {pollenCollected}/{NectarTarget}",
            $"Pollination links {pollinationCount}/{PollinationTarget}");
        HUD.SetState(awaitingPollination
            ? "Status: Touch a bright flower"
            : "Status: Touch the yellow flower");
    }

    private void TryCompleteLevel()
    {
        if (pollenCollected < NectarTarget || pollinationCount < PollinationTarget)
        {
            return;
        }

        CompleteLevel(
            LevelSummary.Create(
                LevelId.Flower,
                "Flower Habitat Summary",
                "You collected pollen from yellow flowers and delivered it to bright flowers. Repeating that cycle helps flowering plants reproduce.",
                pollenCollected,
                pollinationCount),
            "Pollen pickups",
            "Pollination links",
            NotebookEntryId.Bee,
            NotebookEntryId.Hoverfly);
    }

    private IEnumerator ShowIntroSequence()
    {
        ShowHint("Touch the yellow flower to collect pollen.");
        yield return new WaitForSeconds(2.4f);
        ShowHint("The yellow flower will turn gray after you collect it.");
        yield return new WaitForSeconds(2.4f);
        ShowHint("Touch one of the bright flowers to finish the pollination.");
    }

    private void PickNextSourceFlower(FlowerTarget preferredFlower)
    {
        List<FlowerTarget> availableFlowers = new();
        foreach (FlowerTarget flower in flowers)
        {
            if (flower != null && flower.isActiveAndEnabled)
            {
                availableFlowers.Add(flower);
            }
        }

        if (availableFlowers.Count == 0)
        {
            currentSourceFlower = null;
            return;
        }

        if (preferredFlower != null && availableFlowers.Contains(preferredFlower))
        {
            currentSourceFlower = preferredFlower;
        }
        else
        {
            int currentIndex = currentSourceFlower != null ? availableFlowers.IndexOf(currentSourceFlower) : -1;
            currentSourceFlower = availableFlowers[(currentIndex + 1 + availableFlowers.Count) % availableFlowers.Count];
        }

        ApplyFlowerStates();
    }

    private void ApplyFlowerStates()
    {
        foreach (FlowerTarget flower in flowers)
        {
            if (flower == null)
            {
                continue;
            }

            if (flower == currentSourceFlower)
            {
                flower.ApplyVisualState(awaitingPollination
                    ? FlowerTarget.VisualState.Used
                    : FlowerTarget.VisualState.PollenSource);
                continue;
            }

            flower.ApplyVisualState(awaitingPollination
                ? FlowerTarget.VisualState.PollinationTarget
                : FlowerTarget.VisualState.Dim);
        }
    }
}
