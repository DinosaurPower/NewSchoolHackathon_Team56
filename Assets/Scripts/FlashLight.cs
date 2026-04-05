using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Keeps the light’s world position as you placed it; aims the spotlight along corrected mouse rays
/// (<see cref="CursorCorrection"/>). Aim point: raycast against the configured plane layer mask, then optional
/// transform-based plane, then distance fallback.
/// </summary>
public class FlashLight : MonoBehaviour
{
    [Tooltip("GameObject that has the Light to aim and flicker.")]
    [SerializeField] GameObject lightObject;

    [Tooltip("If set, mouse position is passed through this component’s Correct(). Otherwise uses CursorCorrection.Instance when present.")]
    [SerializeField] CursorCorrection cursorCorrection;

    [SerializeField] Camera targetCamera;

    [Tooltip("Raycast the mouse ray against these layers only (e.g. your Plane layer with a collider). Interaction can ignore this layer; the flashlight still hits it here.")]
    [SerializeField] LayerMask aimPlaneLayers;

    [Tooltip("Optional fallback: infinite plane through this transform if nothing on Aim Plane Layers is hit.")]
    [SerializeField] Transform aimSurface;

    [Tooltip("Max ray length for plane raycast / math plane.")]
    [SerializeField] float rayMaxDistance = 500f;

    [Tooltip("If the plane is missed, aim this far along the mouse ray from the camera.")]
    [SerializeField] float fallbackAimDistance = 15f;

    [Tooltip("How long the click flicker lasts.")]
    [SerializeField] float flickerDuration = 0.35f;

    [Tooltip("Upper intensity while flickering (lower bound is the Light’s intensity at enable).")]
    [SerializeField] float flickerPeakIntensity = 5f;

    Light _light;
    float _baseIntensity;
    Coroutine _flickerRoutine;

    void Awake()
    {
        CacheLight();
        if (_light != null)
            _baseIntensity = _light.intensity;
    }

    void OnEnable()
    {
        CacheLight();
        if (_light != null)
            _baseIntensity = _light.intensity;
    }

    void OnValidate()
    {
        CacheLight();
    }

    void CacheLight()
    {
        _light = lightObject != null ? lightObject.GetComponent<Light>() : null;
    }

    Vector2 CorrectScreen(Vector2 raw)
    {
        if (cursorCorrection != null)
            return cursorCorrection.Correct(raw);
        if (CursorCorrection.Instance != null)
            return CursorCorrection.Instance.Correct(raw);
        return raw;
    }

    void LateUpdate()
    {
        if (_light == null || Mouse.current == null)
            return;

        Camera cam = targetCamera != null ? targetCamera : Camera.main;
        if (cam == null)
            return;

        Vector2 screen = CorrectScreen(Mouse.current.position.ReadValue());
        Ray ray = cam.ScreenPointToRay(screen);

        Vector3 aimWorld = GetAimWorldPoint(ray);

        Vector3 origin = _light.transform.position;
        Vector3 dir = aimWorld - origin;
        if (dir.sqrMagnitude > 1e-10f)
            _light.transform.rotation = Quaternion.LookRotation(dir.normalized);

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (_flickerRoutine != null)
                StopCoroutine(_flickerRoutine);
            _flickerRoutine = StartCoroutine(FlickerRoutine());
        }
    }

    Vector3 GetAimWorldPoint(Ray ray)
    {
        if (aimPlaneLayers.value != 0 &&
            Physics.Raycast(ray, out RaycastHit hit, rayMaxDistance, aimPlaneLayers, QueryTriggerInteraction.Ignore))
            return hit.point;

        if (aimSurface != null)
        {
            var plane = new Plane(aimSurface.forward, aimSurface.position);
            if (plane.Raycast(ray, out float t) && t > 0f && t <= rayMaxDistance)
                return ray.GetPoint(t);
        }

        return ray.GetPoint(fallbackAimDistance);
    }

    IEnumerator FlickerRoutine()
    {
        if (_light == null)
        {
            _flickerRoutine = null;
            yield break;
        }

        float original = _baseIntensity;
        float endTime = Time.time + flickerDuration;
        while (Time.time < endTime)
        {
            _light.intensity = Random.value > 0.5f ? original : flickerPeakIntensity;
            yield return new WaitForSeconds(Random.Range(0.02f, 0.07f));
        }

        _light.intensity = original;
        _flickerRoutine = null;
    }
}
