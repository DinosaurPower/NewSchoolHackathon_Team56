using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Per-object story data. <see cref="IsHovered"/> and <see cref="IsClicked"/> are updated by
/// <see cref="InteractionManager"/> using the same screen ray as interactions (with cursor correction).
/// While the mouse button stays down after a click on this object, renderers use the shared press overlay
/// material (see <see cref="SharedPressOverlayTemplate"/> / Resources) with <c>_Is_Selected</c> enabled
/// and transparency driven by <see cref="SharedPressTransparency"/> / <see cref="SharedPressSelectionColor"/>.
/// </summary>
public class StoryObject : MonoBehaviour
{
    public const string DefaultPressOverlayResourcesName = "PressOverlayMaterial";

    /// <summary>Optional per-object material; if null, <see cref="SharedPressOverlayTemplate"/> or Resources <see cref="DefaultPressOverlayResourcesName"/> is used.</summary>
    public static Material SharedPressOverlayTemplate { get; set; }

    /// <summary>Shader <c>_Transparency</c> for all press overlay instances (0–1).</summary>
    public static float SharedPressTransparency = 0.5f;

    /// <summary>Shader <c>_Selection_Color</c>; use alpha to tune overlay strength where the graph uses it.</summary>
    public static Color SharedPressSelectionColor = new Color(0.27727836f, 0.4747538f, 0.8773585f, 0.85f);

    public string ObjectName;

    [Tooltip("True this frame while the interaction ray hits this object's collider.")]
    public bool IsHovered;

    [Tooltip("True for the single frame when this object was clicked (same ray as hover).")]
    public bool IsClicked;

    [Tooltip("If set, this object only uses this template instead of the shared default.")]
    [SerializeField] Material pressOverlayMaterialOverride;

    static Material _resourcesTemplate;
    static bool _warnedMissingTemplate;

    Renderer[] _renderers;
    Material[][] _savedSharedMaterials;
    Material[][] _pressOverlayInstances;
    bool _pressOverlayActive;
    bool _debugWasHoveredLastFrame;

    void Awake()
    {
        _renderers = GetComponentsInChildren<Renderer>(true);
    }

    void LateUpdate()
    {
        if (IsHovered && !_debugWasHoveredLastFrame)
         //   Debug.Log($"[StoryObject] is hovered: {ObjectName}");
        _debugWasHoveredLastFrame = IsHovered;

        if (IsClicked)
            Debug.Log($"[StoryObject] clicked: {ObjectName}");

        bool pointerHeld = Mouse.current != null && Mouse.current.leftButton.isPressed;

        if (IsClicked && ResolveOverlayTemplate() != null && !_pressOverlayActive)
            ApplyPressOverlay();

        if (_pressOverlayActive && !pointerHeld)
            RemovePressOverlay();
    }

    Material ResolveOverlayTemplate()
    {
        if (pressOverlayMaterialOverride != null)
            return pressOverlayMaterialOverride;
        if (SharedPressOverlayTemplate != null)
            return SharedPressOverlayTemplate;
        if (_resourcesTemplate == null)
            _resourcesTemplate = Resources.Load<Material>(DefaultPressOverlayResourcesName);
        if (_resourcesTemplate == null && !_warnedMissingTemplate)
        {
            Debug.LogWarning(
                $"[StoryObject] No press overlay material. Assign {nameof(SharedPressOverlayTemplate)}, place a material at Resources/{DefaultPressOverlayResourcesName}, or set {nameof(pressOverlayMaterialOverride)} on an instance.");
            _warnedMissingTemplate = true;
        }

        return _resourcesTemplate;
    }

    static void ConfigurePressOverlayInstance(Material instance)
    {
        // Shader Graph booleans are exposed as float 0/1 on the material.
        instance.SetFloat("_Is_Selected", 1f);
        instance.SetFloat("_Is_Hovered", 0f);
        instance.SetFloat("_Transparency", Mathf.Clamp01(SharedPressTransparency));
        instance.SetColor("_Selection_Color", SharedPressSelectionColor);
    }

    void ApplyPressOverlay()
    {
        Material template = ResolveOverlayTemplate();
        if (template == null || _renderers == null || _renderers.Length == 0)
            return;

        _savedSharedMaterials = new Material[_renderers.Length][];
        _pressOverlayInstances = new Material[_renderers.Length][];
        for (int i = 0; i < _renderers.Length; i++)
        {
            Renderer r = _renderers[i];
            Material[] src = r.sharedMaterials;
            _savedSharedMaterials[i] = new Material[src.Length];
            for (int j = 0; j < src.Length; j++)
                _savedSharedMaterials[i][j] = src[j];

            var overlay = new Material[src.Length];
            for (int j = 0; j < src.Length; j++)
            {
                overlay[j] = new Material(template);
                ConfigurePressOverlayInstance(overlay[j]);
            }

            r.materials = overlay;
            _pressOverlayInstances[i] = overlay;
        }

        _pressOverlayActive = true;
    }

    void RemovePressOverlay()
    {
        if (!_pressOverlayActive || _savedSharedMaterials == null)
            return;

        for (int i = 0; i < _renderers.Length; i++)
        {
            Renderer r = _renderers[i];
            if (r == null)
                continue;
            r.sharedMaterials = _savedSharedMaterials[i];
        }

        if (_pressOverlayInstances != null)
        {
            for (int i = 0; i < _pressOverlayInstances.Length; i++)
            {
                if (_pressOverlayInstances[i] == null)
                    continue;
                for (int j = 0; j < _pressOverlayInstances[i].Length; j++)
                {
                    if (_pressOverlayInstances[i][j] != null)
                        Destroy(_pressOverlayInstances[i][j]);
                }
            }
        }

        _savedSharedMaterials = null;
        _pressOverlayInstances = null;
        _pressOverlayActive = false;
    }
}
