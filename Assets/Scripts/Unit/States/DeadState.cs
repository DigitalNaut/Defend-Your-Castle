using System;
using System.Collections;
using UnityEngine;

public class DeadState : StateMachineBehaviour
{
  private SpatialDetector _spatialDetector;
  private UnitPropertiesManager _unitPropertiesManager;

  private void CacheComponents(Animator animator)
  {
    if (_spatialDetector == null)
      _spatialDetector = animator.GetComponentInParent<SpatialDetector>();

    if (_unitPropertiesManager == null)
      _unitPropertiesManager = animator.GetComponentInParent<UnitPropertiesManager>();
  }

  override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
  {
    CacheComponents(animator);

    _spatialDetector.StopCheckingObstacleOnInterval("Stop coroutine DeadState");

    _unitPropertiesManager.DestroyUnit();
  }
}