using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class AnimationObserver : MonoBehaviour
{
  #region Inspector
  [SerializeField] string attackAnimationEndMessage = "Attack";
  [SerializeField] string deathAnimationEndMessage = "Death";
  #endregion

  #region Properties
  public UnityEvent OnAttackAnimationEnd;
  public UnityEvent OnDeathAnimationEnd;
  #endregion

  #region Methods
  /// <summary>
  /// Method called by the animation events from the model asset.
  /// </summary>
  /// <param name="message"></param>
  public void AnimationEnded(string message)
  {
    if (message == attackAnimationEndMessage)
    { OnAttackAnimationEnd?.Invoke(); }

    if (message == deathAnimationEndMessage)
    { OnDeathAnimationEnd?.Invoke(); }
  }
  #endregion

  #region Unity Methods
  private void OnDestroy()
  {
    OnAttackAnimationEnd.RemoveAllListeners();
    OnDeathAnimationEnd.RemoveAllListeners();
  }
  #endregion
}