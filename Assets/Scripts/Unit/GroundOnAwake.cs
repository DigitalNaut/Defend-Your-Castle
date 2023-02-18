using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(SpatialAwarenessManager))]
[DisallowMultipleComponent]
public class GroundOnAwake : MonoBehaviour
{
  #region Fields
  new private Collider collider;
  private float height;
  #endregion

  #region Methods
  /// <summary>
  /// Caches the components on the object.
  /// </summary>
  private void CacheComponents()
  {
    collider = GetComponent<Collider>();
    height = collider.bounds.extents.y;
  }

  /// <summary>
  /// Casts a ray down from the object's position and returns the point of impact.
  /// </summary>
  /// <returns>The point of impact if the raycast hits something, otherwise Vector3.zero.</returns>
  private Vector3 ProbeGround()
  {
    var origin = transform.position + Vector3.up * height;
    var direction = Vector3.down;
    var distance = height * 5f;
    var isGroundCollision = Physics.Raycast(origin, direction, out RaycastHit hit, maxDistance: distance);

    return isGroundCollision ? hit.point : transform.position;
  }

  /// <summary>
  /// Places the object on the ground.
  /// </summary>
  private void PlaceOnGround()
  {
    Vector3 groundPoint = ProbeGround();
    if (groundPoint != transform.position)
      transform.position = groundPoint + Vector3.up * height;
  }
  #endregion

  #region Unity Methods
  private void Awake()
  {
    CacheComponents();
    PlaceOnGround();
  }
  #endregion
}
