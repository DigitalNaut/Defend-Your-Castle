using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(SpatialAwarenessManager))]
[RequireComponent(typeof(UnitAnimationsManager))]
[DisallowMultipleComponent]
public class MarchingMovement : MonoBehaviour
{
  #region Inspector
  [SerializeField] float velocity = 1f;
  #endregion

  #region Fields
  new private Rigidbody rigidbody;
  private SpatialAwarenessManager spatialAwarenessManager;
  private UnitAnimationsManager unitAnimationsManager;
  #endregion

  #region Methods
  /// <summary>
  /// Caches the components on the object.
  /// </summary>
  private void CacheComponents()
  {
    rigidbody = GetComponent<Rigidbody>();
    if (!rigidbody) Debug.LogError("Rigidbody reference is null.", this);

    spatialAwarenessManager = GetComponent<SpatialAwarenessManager>();
    if (!spatialAwarenessManager) Debug.LogError("Spatial awareness manager reference is null.", this);

    unitAnimationsManager = GetComponent<UnitAnimationsManager>();
    if (!unitAnimationsManager) Debug.LogError("Unit animations manager reference is null.", this);
  }

  /// <summary>
  /// Moves the rigidbody forward at the velocity specified in the inspector.
  /// </summary>
  private void Move()
  {
    if (rigidbody)
    {
      rigidbody.velocity = new Vector3(
        transform.forward.x * velocity,
        rigidbody.velocity.y,
        transform.forward.z * velocity);

      unitAnimationsManager.SetMoving();
    }
  }

  /// <summary>
  /// Stops the rigidbody by setting its velocity to zero.
  /// </summary>
  private void Stop()
  {
    if (rigidbody)
    {
      if (rigidbody.velocity.magnitude > 0.1f)
      {
        rigidbody.velocity = new Vector3(
        0,
        rigidbody.velocity.y,
        0);

        unitAnimationsManager.SetIdle();
      }
    }
  }
  #endregion

  #region Unity Methods
  private void Awake() => CacheComponents();

  private void Update()
  {
    if (spatialAwarenessManager.CanMoveForward)
    { Move(); }
    else
    { Stop(); }
  }
  #endregion
}
