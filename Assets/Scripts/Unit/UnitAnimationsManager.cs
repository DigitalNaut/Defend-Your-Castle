using UnityEngine;
using UnityEngine.Events;
using CombatSystem;

/// <summary>
/// Manages the animations of the unit using a state machine and an animation observer.
/// It essentially acts as a bridge between the unit and the animator through functions.
/// It also subscribes to the animation observer events.
/// </summary>
[DisallowMultipleComponent]
public class UnitAnimationsManager : MonoBehaviour
{
  #region Inspector
  [SerializeField] Animator animator;
  [SerializeField] AnimationObserver animationObserver;
  #endregion

  #region Fields
  public UnityEvent OnDeathAnimationEnd => animationObserver.OnDeathAnimationEnd;
  public UnityEvent OnAttackAnimationEnd => animationObserver.OnAttackAnimationEnd;
  #endregion

  #region Properties
  public int CurrentState => animator.GetInteger("State");
  public bool IsState(UnitAnimationState state) => CurrentState == (int)state;
  public bool IsDead => IsState(UnitAnimationState.Dead);
  public bool IsAttacking => IsState(UnitAnimationState.Attacking);
  #endregion

  #region Methods
  /// <summary>
  /// Updates the animation state if it is different from the current state.
  /// </summary>
  /// <param name="newState"></param>
  private void SetAnimationState(UnitAnimationState newState)
  {
    if (IsDead) return;
    if (CurrentState == (int)newState) return;

    animator.SetInteger("State", (int)newState);
  }

  /// <summary>
  /// Set state to idle.
  /// </summary>
  public void SetIdle() => SetAnimationState(UnitAnimationState.Idle);
  /// <summary>
  /// Set state to moving.
  /// </summary>
  public void SetMoving() => SetAnimationState(UnitAnimationState.Moving);
  /// <summary>
  /// Set state to attacking.
  /// </summary>
  public void SetAttacking() => SetAnimationState(UnitAnimationState.Attacking);
  /// <summary>
  /// Set state to dying.
  /// </summary>
  public void SetDying() => SetAnimationState(UnitAnimationState.Dead);
  #endregion

  #region Unity Methods
  private void Start() => SetIdle();
  private void OnValidate()
  {
    if (!animator) { Debug.LogError("Animator reference is null.", this); }
    if (!animationObserver) { Debug.LogError("Animation observer reference is null.", this); }
  }
  #endregion
}