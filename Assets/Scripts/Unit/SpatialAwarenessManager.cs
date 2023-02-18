using UnityEngine;
using UnityEngine.Events;
using NaughtyAttributes;

[DisallowMultipleComponent]
public class SpatialAwarenessManager : MonoBehaviour
{
  #region Inspector
  [Tooltip("Whether to draw debug lines or not.")]
  [SerializeField] bool debugging = false;

  [Tooltip("The distance to cast the ray.")]
  [SerializeField][Range(minDistance, max: maxDistance)] float distance = 1f;
  
  [Tooltip("The layer mask to use for the raycast.")]
  [SerializeField] LayerMask layerMask;
  #endregion

  #region Fields
  private const float minDistance = 0f;
  private const float maxDistance = 500f;

  /// <summary>
  /// Whether the object can move forward or not.
  /// </summary>
  public bool CanMoveForward { get; private set; }
  /// <summary>
  /// The array of collisions returned by the raycast.
  /// </summary>
  public RaycastHit[] Hits { get; private set; } = new RaycastHit[1];
  [SerializeField] UnityEvent OnHasObstacle;
  #endregion

  #region Methods
  /// <summary>
  /// Casts a ray forward from the object's position and returns true if the raycast hits nothing. 
  /// The collisions are stored in the Collisions array.
  /// </summary>
  /// <returns>True if the raycast hits nothing, otherwise false.</returns>
  /// <remarks>Note: This method is meant to be called from an interval timer to avoid performance issues.</remarks>
  public void CheckForward()
  {
    // Set the parameters for the raycast.
    var origin = transform.position;
    var direction = transform.forward;

    if (debugging) { Debug.DrawRay(origin, direction * distance, CanMoveForward ? Color.green : Color.red, 0.1f); }

    // Cast the ray and store the collisions in the Collisions array.
    var collisions = Physics.RaycastNonAlloc(origin, direction, Hits, distance, layerMask);
    CanMoveForward = collisions == 0;

    if (!CanMoveForward) { OnHasObstacle?.Invoke(); }
  }

  /// <summary>
  /// Unsubscribes from all events.
  /// </summary>
  private void UnsubscribeFromEvents()
  {
    OnHasObstacle.RemoveAllListeners();
  }

  /// <summary>
  /// Stops updating the object's spatial awareness.
  /// </summary>
  public void StopUpdating() => UnsubscribeFromEvents();
  #endregion

  #region Unity Methods
  private void OnValidate()
  {
    distance = Mathf.Clamp(distance, minDistance, maxDistance);

    if (layerMask == 0 && gameObject.layer != 0)
    {
      Debug.LogWarning($"The layer mask is set to 'Nothing'. Setting it to match the parent object's: '{LayerMask.LayerToName(gameObject.layer)}''.", gameObject);
      layerMask = 1 << gameObject.layer;
    }
  }
  private void OnDestroy() => UnsubscribeFromEvents();
  #endregion
}