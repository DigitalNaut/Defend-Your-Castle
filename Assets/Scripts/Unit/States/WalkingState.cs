using System;
using UnityEngine;
using Unit.Managers;

public class WalkState : StateMachineBehaviour, IManaged
{
  private Rigidbody _rigidbody;
  private ObstacleDetector _obstacleDetector;
  public Animator Animator { get; set; }
  public SpatialDetector SpatialDetector { get; set; }
  public UnitPropertiesManager UnitPropertiesManager { get; private set; }

  private void Walk()
  {
    _rigidbody.velocity = _rigidbody.transform.forward * UnitPropertiesManager.UnitProperties.WalkSpeed;
  }

  private void CacheComponents(Animator animator)
  {
    Animator = animator;

    if (UnitPropertiesManager == null)
      UnitPropertiesManager = Animator.GetComponentInParent<UnitPropertiesManager>();

    if (_rigidbody == null)
      _rigidbody = Animator.GetComponentInParent<Rigidbody>();

    if (SpatialDetector == null)
      SpatialDetector = Animator.GetComponentInParent<SpatialDetector>();

    _obstacleDetector ??= new ObstacleDetector(this);
  }

  private void CheckForObstacle(RaycastHit hit)
  {
    var obstacle = _obstacleDetector.CheckObstacle(hit);

    switch (obstacle)
    {
      case ObstacleType.None:
        Animator.SetBool("HasObstacle", false);
        Animator.SetBool("HasEnemy", false);
        Animator.SetBool("HasFriendly", false);
        break;
      case ObstacleType.Enemy:
      case ObstacleType.Friendly:
      case ObstacleType.Obstacle:
        Animator.SetBool("HasObstacle", true);
        break;
      default:
        throw new Exception($"Unexpected obstacle type {obstacle}");
    }
  }

  override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) => Walk();

  override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
  {
    CacheComponents(animator);

    SpatialDetector.StartCheckingObstacleOnInterval(CheckForObstacle, 0.1f, "WalkState");
  }

  public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
  {
    SpatialDetector.StopCheckingObstacleOnInterval(CheckForObstacle, "WalkState");
  }
}
