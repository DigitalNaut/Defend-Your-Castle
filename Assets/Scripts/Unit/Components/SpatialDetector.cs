using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A component that handles raycasts for the Unit.
/// </summary>
[RequireComponent(typeof(Collider))]
[DisallowMultipleComponent]
public class SpatialDetector : MonoBehaviour
{
  public bool debugging = false;

  [Tooltip("The distance to cast the ray.")]
  [SerializeField, Range(0f, 5f)] private float distance = 1f;
  [Tooltip("The layer mask to use for the raycast.")]
  [SerializeField] private LayerMask layerMask;

  public UnityEvent<RaycastHit> OnHasObstacle;
  private Coroutine _intervalCoroutine;
  private Collider _collider;
  private Transform _transform;
  private Vector3 _forward;

  private RaycastHit RaycastObstacle(Vector3 origin, Vector3 direction)
  {
    RaycastHit[] hitResults = new RaycastHit[1];
    Physics.RaycastNonAlloc(origin, direction, hitResults, distance);

    if (debugging)
      Debug.DrawRay(origin, direction * distance, hitResults[0].collider == null ? Color.green : Color.red, 0.1f);

    return hitResults[0];
  }

  public RaycastHit CheckObstacleForward() => RaycastObstacle(_collider.bounds.center, _forward);

  public void StartCheckingObstacleOnInterval(UnityAction<RaycastHit> action, float customInterval, string msg = "default")
  {
    if (msg != "default" && debugging)
      Debug.Log($"{gameObject.name} :: {msg} :: start {_intervalCoroutine}");

    _intervalCoroutine = StartCoroutine(CheckObstacleOnIntervalCoroutine(customInterval));

    OnHasObstacle.AddListener(action);

    if (_transform == null)
    { _transform = transform; }

    _forward = _transform.forward;
  }

  public void StopCheckingObstacleOnInterval(UnityAction<RaycastHit> action, string msg = "default")
  {
    OnHasObstacle.RemoveListener(action);
    StopCheckingObstacleOnInterval(msg);
  }

  public void StopCheckingObstacleOnInterval(string msg = "default")
  {
    if (msg != "default" && debugging)
      Debug.Log($"{gameObject.name} :: {msg} :: end {_intervalCoroutine}");

    if (_intervalCoroutine == null) return;

    StopCoroutine(_intervalCoroutine);
    _intervalCoroutine = null;
  }

  private IEnumerator CheckObstacleOnIntervalCoroutine(float customInterval)
  {
    while (true)
    {
      yield return new WaitForSeconds(customInterval);

      var hit = CheckObstacleForward();

      OnHasObstacle?.Invoke(hit);
    }
  }

  private void Awake()
  {
    _collider = GetComponent<Collider>();
  }

  private void OnDestroy()
  {
    OnHasObstacle.RemoveAllListeners();
    StopCheckingObstacleOnInterval();
  }

  private void OnDrawGizmos()
  {
    if (Application.isPlaying) return;
    if (!debugging) return;

    Gizmos.color = Color.yellow;
    Gizmos.DrawRay(transform.position, transform.forward * distance);
  }
}