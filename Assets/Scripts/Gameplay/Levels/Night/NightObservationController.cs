using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NightObservationController : LevelControllerBase
{
    public Camera GameplayCamera;
    public RectTransform FlashlightCursor;
    public RectTransform FlashlightCore;
    public RectTransform FlashlightHalo;
    public RectTransform PhotoFrame;
    public List<NightObservationTarget> Targets = new();
    public LayerMask TargetLayerMask = ~0;
    public float FocusDuration = 0.5f;
    public float IdleHintDelay = 8f;
    public float CursorFollowSpeed = 16f;
    public float FlashlightPulseSpeed = 2.2f;
    public float FlashlightPulseAmount = 0.08f;
    public int DungBeetleTargetCount = 2;
    public int FireflyTargetCount = 2;

    private readonly Dictionary<NotebookEntryId, int> photographedCounts = new();
    private NightObservationTarget focusedTarget;
    private float focusTimer;
    private float idleTimer;
    private Vector2 smoothedPointerPosition;
    private bool pointerInitialized;
    private bool beetleHintShown;
    private bool fireflyHintShown;

    protected override void Start()
    {
        base.Start();

        if (GameplayCamera == null)
        {
            GameplayCamera = Camera.main;
        }

        photographedCounts[NotebookEntryId.DungBeetle] = 0;
        photographedCounts[NotebookEntryId.Firefly] = 0;

        if (HUD != null)
        {
            HUD.SetTitle("Night Habitat: Search and Record");
        }

        SetPhotoFrameVisible(false);
        RefreshHUD();
        StartCoroutine(ShowIntroSequence());
    }

    private void Update()
    {
        if (IsCompleted || InputManager.Instance == null)
        {
            return;
        }

        idleTimer += Time.deltaTime;
        UpdateCursorVisuals();
        UpdateFocusedTarget();

        if (InputManager.Instance.IsPhotoPressed)
        {
            TryPhotographFocusedTarget();
        }

        if (idleTimer >= IdleHintDelay)
        {
            idleTimer = 0f;
            ShowHint("Move the light over the habitat, then press F to record what you find.");
        }
    }

    private void UpdateCursorVisuals()
    {
        Vector2 pointerPosition = InputManager.Instance.PointerScreenPosition;
        if (!pointerInitialized)
        {
            smoothedPointerPosition = pointerPosition;
            pointerInitialized = true;
        }
        else
        {
            float t = 1f - Mathf.Exp(-CursorFollowSpeed * Time.deltaTime);
            smoothedPointerPosition = Vector2.Lerp(smoothedPointerPosition, pointerPosition, t);
        }

        float pulse = 1f + Mathf.Sin(Time.time * FlashlightPulseSpeed) * FlashlightPulseAmount;

        if (FlashlightCursor != null)
        {
            FlashlightCursor.position = smoothedPointerPosition;
            FlashlightCursor.localScale = Vector3.one * pulse;
        }

        if (FlashlightCore != null)
        {
            FlashlightCore.position = smoothedPointerPosition;
            FlashlightCore.localScale = Vector3.one * (pulse * 0.94f);
        }

        if (FlashlightHalo != null)
        {
            FlashlightHalo.position = smoothedPointerPosition;
            FlashlightHalo.localScale = Vector3.one * (pulse * 1.08f);
        }

        if (PhotoFrame != null)
        {
            PhotoFrame.position = smoothedPointerPosition;
            bool showPhotoFrame = focusedTarget != null && focusTimer >= FocusDuration * 0.5f;
            SetPhotoFrameVisible(showPhotoFrame);
        }
    }

    private void UpdateFocusedTarget()
    {
        NightObservationTarget newTarget = null;

        if (GameplayCamera != null)
        {
            Ray ray = GameplayCamera.ScreenPointToRay(InputManager.Instance.PointerScreenPosition);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, 200f, TargetLayerMask))
            {
                hitInfo.collider.TryGetComponent(out newTarget);
            }
        }

        if (newTarget != focusedTarget)
        {
            if (focusedTarget != null)
            {
                focusedTarget.SetFocused(false, false);
            }

            focusedTarget = newTarget != null && !newTarget.IsPhotographed ? newTarget : null;
            focusTimer = 0f;
        }

        if (focusedTarget == null)
        {
            RefreshHUD();
            return;
        }

        focusTimer += Time.deltaTime;
        bool isReady = focusTimer >= FocusDuration;
        focusedTarget.SetFocused(true, isReady);
        RefreshHUD();
    }

    private void TryPhotographFocusedTarget()
    {
        idleTimer = 0f;

        if (focusedTarget == null)
        {
            ShowHint("Center the light on an insect first.");
            return;
        }

        if (focusTimer < FocusDuration)
        {
            ShowHint("Hold the light steady a little longer, then press F.");
            return;
        }

        focusedTarget.Photograph();
        RegisterPhoto(focusedTarget);
        focusedTarget = null;
        focusTimer = 0f;
        RefreshHUD();
        TryCompleteLevel();
    }

    private void RegisterPhoto(NightObservationTarget target)
    {
        if (!photographedCounts.ContainsKey(target.EntryId))
        {
            photographedCounts[target.EntryId] = 0;
        }

        photographedCounts[target.EntryId]++;

        if (GameProgressManager.Instance != null)
        {
            GameProgressManager.Instance.UnlockNotebookEntry(target.EntryId);
        }

        if (target.EntryId == NotebookEntryId.DungBeetle && !beetleHintShown)
        {
            beetleHintShown = true;
            ShowHint("Night-active beetles help recycle organic matter on the ground.");
            return;
        }

        if (target.EntryId == NotebookEntryId.Firefly && !fireflyHintShown)
        {
            fireflyHintShown = true;
            ShowHint("Fireflies are often treated as signs of a healthy habitat.");
            return;
        }

        ShowHint("Photo recorded. Your field notes were updated.");
    }

    private void RefreshHUD()
    {
        if (HUD == null)
        {
            return;
        }

        HUD.SetObjectives(
            $"Ground beetle records {GetCount(NotebookEntryId.DungBeetle)}/{DungBeetleTargetCount}",
            $"Firefly records {GetCount(NotebookEntryId.Firefly)}/{FireflyTargetCount}");
        HUD.SetState(focusedTarget == null ? "Status: Searching in the dark" : "Status: Target in focus");
    }

    private void TryCompleteLevel()
    {
        if (GetCount(NotebookEntryId.DungBeetle) < DungBeetleTargetCount || GetCount(NotebookEntryId.Firefly) < FireflyTargetCount)
        {
            return;
        }

        CompleteLevel(
            LevelSummary.Create(
                LevelId.Night,
                "Night Habitat Summary",
                "You documented nocturnal insects that stay active after sunset. Ground beetles help cycle matter on the ground, while fireflies signal habitat quality and nighttime biodiversity.",
                GetCount(NotebookEntryId.DungBeetle),
                GetCount(NotebookEntryId.Firefly)),
            "Ground beetle records",
            "Firefly records",
            NotebookEntryId.GroundBeetle,
            NotebookEntryId.Firefly);
    }

    private int GetCount(NotebookEntryId entryId)
    {
        return photographedCounts.TryGetValue(entryId, out int count) ? count : 0;
    }

    private IEnumerator ShowIntroSequence()
    {
        ShowHint("Move the flashlight across the scene.");
        yield return new WaitForSeconds(2.2f);
        ShowHint("Hold the light on an insect and press F to record it.");
        yield return new WaitForSeconds(2.2f);
        ShowHint("Nighttime habitats stay busy even after sunset.");
    }

    private void SetPhotoFrameVisible(bool isVisible)
    {
        if (PhotoFrame == null)
        {
            return;
        }

        Image image = PhotoFrame.GetComponent<Image>();
        if (image != null)
        {
            Color color = image.color;
            color.a = isVisible ? 0.14f : 0f;
            image.color = color;
        }

        Outline outline = PhotoFrame.GetComponent<Outline>();
        if (outline != null)
        {
            Color effectColor = outline.effectColor;
            effectColor.a = isVisible ? 0.55f : 0f;
            outline.effectColor = effectColor;
        }
    }
}
