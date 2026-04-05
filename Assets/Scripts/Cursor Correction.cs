using UnityEngine;

/// <summary>
/// Maps raw OS/air-mouse screen coordinates into the space Unity uses for
/// <see cref="Camera.ScreenPointToRay"/> when the projected image does not match
/// the logical desktop 1:1 (letterboxing, overscan, scaled duplicate display, etc.).
/// Tune scale and offset in Play mode, then copy values for a build preset.
/// </summary>
public class CursorCorrection : MonoBehaviour
{
    [Tooltip("Applied as: corrected = raw * scale + offset. Use >1 if the cursor moves too little on the wall, <1 if it moves too much.")]
    public Vector2 scale = Vector2.one;

    [Tooltip("Pixels added after scale (Unity screen space, origin bottom-left).")]
    public Vector2 offset = Vector2.zero;

    public static CursorCorrection Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public Vector2 Correct(Vector2 rawScreen)
    {
        var v = new Vector2(rawScreen.x * scale.x + offset.x, rawScreen.y * scale.y + offset.y);
        v.x = Mathf.Clamp(v.x, 0f, Mathf.Max(0f, Screen.width - 1f));
        v.y = Mathf.Clamp(v.y, 0f, Mathf.Max(0f, Screen.height - 1f));
        return v;
    }
}
