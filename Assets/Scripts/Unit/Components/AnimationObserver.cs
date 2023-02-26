using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class AnimationObserver : MonoBehaviour
{
  public UnityEvent<string> OnAnimationEvent;

  /// <summary>
  /// Method called by the animation events from the model asset.
  /// </summary>
  /// <param name="message"></param>
  public void AnimationEvent(string message) => OnAnimationEvent?.Invoke(message);

  private void OnDestroy() => OnAnimationEvent.RemoveAllListeners();
}