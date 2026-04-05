using UnityEngine;

/// <summary>
/// Per-object story data. <see cref="IsHovered"/> and <see cref="IsClicked"/> are updated by
/// <see cref="InteractionManager"/> using the same screen ray as interactions (with cursor correction).
/// Other systems (e.g. overlay materials) can read these on this component via <c>GetComponent&lt;StoryObject&gt;()</c>.
/// Temporary console logs (hover enter / click) run in <see cref="LateUpdate"/> after flags are set.
/// </summary>
public class StoryObject : MonoBehaviour
{
    public string ObjectName;

    [Tooltip("True this frame while the interaction ray hits this object's collider.")]
    public bool IsHovered;

    [Tooltip("True for the single frame when this object was clicked (same ray as hover).")]
    public bool IsClicked;

    bool _debugWasHoveredLastFrame;

    void LateUpdate()
    {
        if (IsHovered && !_debugWasHoveredLastFrame)
            Debug.Log($"[StoryObject] is hovered: {ObjectName}");
        _debugWasHoveredLastFrame = IsHovered;

        if (IsClicked)
            Debug.Log($"[StoryObject] clicked: {ObjectName}");
    }
}
