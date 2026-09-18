using UnityEngine;

public class FlowerTarget : MonoBehaviour
{
    public enum VisualState
    {
        Dim,
        PollenSource,
        Used,
        PollinationTarget
    }

    public GameObject HighlightMarker;
    public ParticleSystem CollectEffect;
    public ParticleSystem PollinationEffect;
    public float NectarCooldown = 1.5f;
    public SpriteRenderer VisualRenderer;

    private float nectarCooldownTimer;

    public bool CanProvideNectar => nectarCooldownTimer <= 0f;

    private void Awake()
    {
        if (VisualRenderer == null)
        {
            Transform visualTransform = transform.Find("Visual");
            VisualRenderer = visualTransform != null
                ? visualTransform.GetComponent<SpriteRenderer>()
                : GetComponentInChildren<SpriteRenderer>();
        }
    }

    private void Update()
    {
        if (nectarCooldownTimer > 0f)
        {
            nectarCooldownTimer -= Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out FlowerPlayerAgent playerAgent))
        {
            playerAgent.TryVisitFlower(this);
        }
    }

    public bool TryCollectNectar()
    {
        if (!CanProvideNectar)
        {
            return false;
        }

        nectarCooldownTimer = NectarCooldown;

        if (CollectEffect != null)
        {
            CollectEffect.Play();
        }

        return true;
    }

    public void PlayPollinationFeedback()
    {
        if (PollinationEffect != null)
        {
            PollinationEffect.Play();
        }
    }

    public void SetHighlighted(bool isHighlighted)
    {
        if (HighlightMarker != null)
        {
            HighlightMarker.SetActive(isHighlighted);
        }
    }

    public void ApplyVisualState(VisualState state)
    {
        if (VisualRenderer == null)
        {
            return;
        }

        VisualRenderer.color = state switch
        {
            VisualState.Dim => new Color(0.72f, 0.72f, 0.72f, 1f),
            VisualState.PollenSource => new Color(1f, 0.9f, 0.35f, 1f),
            VisualState.Used => new Color(0.55f, 0.55f, 0.55f, 1f),
            VisualState.PollinationTarget => Color.white,
            _ => Color.white
        };

        SetHighlighted(false);
    }
}
