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
  private float _interval;
  private Collider _collider;
  private Vector3 _forward;

  private RaycastHit RaycastObstacle(Vector3 origin, Vector3 direction)
  {
    RaycastHit[] hitResults = new RaycastHit[1];
    Physics.RaycastNonAlloc(origin, direction, hitResults, distance);

    if (debugging)
    { Debug.DrawRay(origin, direction * distance, hitResults[0].collider == null ? Color.green : Color.red, 0.01f); }

    return hitResults[0];
  }

  public RaycastHit CheckObstacleForward() => RaycastObstacle(_collider.bounds.center, _forward);

  public void StartCheckingObstacleOnInterval(UnityAction<RaycastHit> action, float customInterval)
  {
    _forward = transform.forward;
    _interval = customInterval;
    _intervalCoroutine ??= StartCoroutine(CheckObstacleOnIntervalCoroutine());

    OnHasObstacle.AddListener(action);
  }

  public void StopCheckingObstacleOnInterval(UnityAction<RaycastHit> action)
  {
    OnHasObstacle.RemoveListener(action);
  }

  public void StopIntervalCoroutine()
  {
    if (_intervalCoroutine == null) return;

    StopCoroutine(_intervalCoroutine);
    _intervalCoroutine = null;
  }

  private IEnumerator CheckObstacleOnIntervalCoroutine()
  {
    while (true)
    {
      yield return new WaitForSecondsRealtime(_interval);

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
    StopIntervalCoroutine();
  }

  private void OnDrawGizmos()
  {
    if (Application.isPlaying) return;
    if (!debugging) return;

    Gizmos.color = Color.yellow;
    Gizmos.DrawRay(transform.position, transform.forward * distance);
  }
}