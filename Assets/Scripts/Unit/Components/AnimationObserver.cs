using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class AnimationObserver : MonoBehaviour
{
  #region Properties
  public UnityEvent<string> OnAnimationEvent;
  #endregion

  #region Methods
  /// <summary>
  /// Method called by the animation events from the model asset.
  /// </summary>
  /// <param name="message"></param>
  public void AnimationEvent(string message) => OnAnimationEvent?.Invoke(message);
  #endregion

  #region Unity Methods
  private void OnDestroy() => OnAnimationEvent.RemoveAllListeners();
  #endregion
}