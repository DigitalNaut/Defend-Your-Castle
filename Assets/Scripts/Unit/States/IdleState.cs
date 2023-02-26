using System;
using UnityEngine;
using Unit.Managers;

public class IdleState : StateMachineBehaviour, IManaged
{
  private ObstacleDetector _obstacleDetector;
  public Animator Animator { get; set; }
  public SpatialDetector SpatialDetector { get; set; }
  public UnitPropertiesManager UnitPropertiesManager { get; private set; }

  private void CacheComponents(Animator animator)
  {
    Animator = animator;

    if (SpatialDetector == null)
      SpatialDetector = Animator.GetComponentInParent<SpatialDetector>();

    if (UnitPropertiesManager == null)
      UnitPropertiesManager = Animator.GetComponentInParent<UnitPropertiesManager>();

    _obstacleDetector ??= new ObstacleDetector(this);
  }

  private void HandleObstacleCheck(RaycastHit hit)
  {
    var obstacle = _obstacleDetector.CheckObstacle(hit);

    switch (obstacle)
    {
      case ObstacleType.None:
        Animator.SetBool("HasObstacle", false);
        Animator.SetBool("HasEnemy", false);
        Animator.SetBool("HasFriendly", false);
        break;
      case ObstacleType.Friendly:
      case ObstacleType.Obstacle:
        Animator.SetBool("HasObstacle", true);
        break;
      case ObstacleType.Enemy:
        Animator.SetBool("HasEnemy", true);
        break;
      default:
        throw new Exception($"Unexpected obstacle type {obstacle}");
    }
  }

  override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
  {
    CacheComponents(animator);

    SpatialDetector.StartCheckingObstacleOnInterval(HandleObstacleCheck, 1f);
  }

  public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
  {
    SpatialDetector.StopCheckingObstacleOnInterval(HandleObstacleCheck);
  }
}
