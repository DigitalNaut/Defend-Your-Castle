using System;
using UnityEngine;
using Unit.Managers;

public class AttackState : StateMachineBehaviour, IManaged
{
  private AnimationObserver _animationObserver;
  private CombatManager _combatManager;
  public Animator Animator { get; set; }
  public SpatialDetector SpatialDetector { get; set; }
  public UnitPropertiesManager UnitPropertiesManager { get; private set; }

  private void CacheComponents(Animator animator)
  {
    Animator = animator;

    if (UnitPropertiesManager == null)
      UnitPropertiesManager = Animator.GetComponentInParent<UnitPropertiesManager>();

    if (SpatialDetector == null)
      SpatialDetector = Animator.GetComponentInParent<SpatialDetector>();

    if (_animationObserver == null)
      _animationObserver = Animator.GetComponent<AnimationObserver>();

    _combatManager ??= new CombatManager(this);
  }

  private void HandleAnimation(string message)
  {
    Action action = message switch
    {
      "Attack" => _combatManager.HandleAttack,
      // "AttackEnd" => _combatManager.HandleAttackEnd,
      _ => () => { Debug.Log("No action found for message: " + message); }
    };

    action.Invoke();
  }

  override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
  {
    CacheComponents(animator);

    _animationObserver.OnAnimationEvent.AddListener(HandleAnimation);
  }

  public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
  {
    _animationObserver.OnAnimationEvent.RemoveListener(HandleAnimation);
  }
}
