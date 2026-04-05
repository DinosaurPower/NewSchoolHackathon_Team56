using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Keeps a <see cref="Light"/> on the cursor ray used by <see cref="InteractionManager"/>
/// (when assigned), and flickers on left click.
/// </summary>
[RequireComponent(typeof(Light))]
public class FlashLight : MonoBehaviour
{
    [Tooltip("Uses the same corrected screen ray as interactions. If unset, falls back to local cursor math.")]
    [SerializeField] InteractionManager interactionSource;

    [SerializeField] Camera targetCamera;

    [Tooltip("Max ray length when looking for a surface to place the light on.")]
    [SerializeField] float raycastMaxDistance = 100f;

    [Tooltip("World distance along the ray when nothing is hit.")]
    [SerializeField] float fallbackDistance = 5f;

    [Tooltip("Nudge the light slightly off the surface along the hit normal.")]
    [SerializeField] float surfaceOffset = 0.02f;

    [SerializeField] LayerMask raycastLayers = ~0;

    [Tooltip("How long the click flicker lasts.")]
    [SerializeField] float flickerDuration = 0.35f;

    Light _light;
    float _baseIntensity;
    Coroutine _flickerRoutine;

    void Awake()
    {
        _light = GetComponent<Light>();
        _baseIntensity = _light.intensity;
    }

    void OnEnable()
    {
        _baseIntensity = _light.intensity;
    }

    void LateUpdate()
    {
        Ray ray = GetCursorRay();
        if (ray.direction.sqrMagnitude < 1e-6f)
            return;

        if (Physics.Raycast(ray, out RaycastHit hit, raycastMaxDistance, raycastLayers, QueryTriggerInteraction.Ignore))
            transform.position = hit.point + hit.normal * surfaceOffset;
        else
            transform.position = ray.GetPoint(fallbackDistance);

        transform.rotation = Quaternion.LookRotation(ray.direction);

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (_flickerRoutine != null)
                StopCoroutine(_flickerRoutine);
            _flickerRoutine = StartCoroutine(FlickerRoutine());
        }
    }

    Ray GetCursorRay()
    {
        if (interactionSource != null)
            return interactionSource.CurrentCursorRay;

        if (Mouse.current == null)
            return default;

        Camera cam = targetCamera != null ? targetCamera : Camera.main;
        if (cam == null)
            return default;

        Vector2 screenPos = Mouse.current.position.ReadValue();
        if (CursorCorrection.Instance != null)
            screenPos = CursorCorrection.Instance.Correct(screenPos);

        return cam.ScreenPointToRay(screenPos);
    }

    IEnumerator FlickerRoutine()
    {
        float endTime = Time.time + flickerDuration;
        while (Time.time < endTime)
        {
            _light.intensity = _baseIntensity * Random.Range(0.25f, 1.15f);
            yield return new WaitForSeconds(Random.Range(0.02f, 0.07f));
        }

        _light.intensity = _baseIntensity;
        _flickerRoutine = null;
    }
}
