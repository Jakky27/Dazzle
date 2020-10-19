
using UnityEngine;

public class Scalable : MonoBehaviour
{
    //Public variables
    //Growth/shrinking time (MILLISECONDS)
    public float time;
    //Private variables
    private bool active = false;
    private Vector3 originalScale;
    private Vector3 prevScale;
    public Vector3 targetScale;
    private float timePassed = 0.0f;
    private float progress;

    // Use this for initialization
    void Start()
    {
        originalScale = transform.localScale;
        prevScale = transform.localScale;
    }

    // This function eases between initial and final values as progress goes from 0 to 1,
    // such that the speed is proportional to the value at each given time.
    private float ProportionalEasing(float progress, float initial, float final)
    {
        return 2 * (final - initial) / (final + initial) * (0.5f * (final - initial) * progress * progress + initial * progress) + initial;
    }

    private void FixedUpdate()
    {
        if (active)
        {
            timePassed += Time.deltaTime * 1000.0f;
            //0 - 1 with time
            progress = (timePassed / time);
        }
        var currentScale = new Vector3(
            ProportionalEasing(progress, prevScale.x, targetScale.x),
            ProportionalEasing(progress, prevScale.y, targetScale.y),
            ProportionalEasing(progress, prevScale.z, targetScale.z)
        );
        transform.localScale = currentScale;
        if (progress >= 1)
        {
            active = false;
            prevScale = currentScale;
        }
    }

    public void Scale(Vector3 scale)
    {
        targetScale = scale;
        active = true;
        timePassed = 0.0f;
    }

    public void Scale(float ratio)
    {
        Scale(ratio * originalScale);
    }

    public void OriginalSize()
    {
        Scale(originalScale);
    }
}
