using UnityEngine;

public class GrassPlayerAgent : MonoBehaviour
{
    public PestControlController Controller;

    private void Start()
    {
        if (Controller == null)
        {
            Controller = FindAnyObjectByType<PestControlController>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out GrassPreyTarget preyTarget))
        {
            Controller?.TryCapturePrey(preyTarget);
        }
    }
}
