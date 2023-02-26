using UnityEngine.Events;
using NaughtyAttributes;
using UnityEngine;
using System.Threading.Tasks;
using System.Threading;

public class AsyncTimer : MonoBehaviour
{
  [Tooltip("Shows debug logs.")]
  [SerializeField] bool debug = false;
  [Tooltip("The time in milliseconds before the timer ends.")]
  [SerializeField][Range(minTime, maxTime)] int time = 1000;
  [Tooltip("Starts the timer when the game starts.")]
  [SerializeField] bool startOnAwake = false;
  [Tooltip("Loops the timer.")]
  [SerializeField] bool loop = false;
  [Tooltip("Invoked when the timer ends.")]
  [SerializeField] UnityEvent onTimerEnd;

  private const int minTime = 100;
  private const int maxTime = int.MaxValue / 1000;
  private CancellationTokenSource cancellationTokenSource;

  /// <summary>
  /// Starts the timer with the specified delay.
  /// </summary>
  /// <param name="delay"></param>
  private async Task SetTimer(int delay)
  {
    if (delay <= 0)
    { throw new System.ArgumentException("Delay cannot be equals or less than 0."); }

    cancellationTokenSource?.Cancel();
    cancellationTokenSource = new CancellationTokenSource();

    do
    {
      try
      {
        await Task.Delay(delay, cancellationTokenSource.Token);

        if (debug)
        { Debug.Log("Ding!"); }
      }
      catch (TaskCanceledException)
      { break; }

      onTimerEnd?.Invoke();
    }
    while (loop);
  }

  /// <summary>
  /// Starts the timer with the specified delay.
  /// </summary>
  [Button] public Task StartTimer() => SetTimer(time);

  /// <summary>
  /// Stops the current timer.
  /// </summary>
  [Button] public void StopTimer() => cancellationTokenSource?.Cancel();

  private void Start()
  {
    if (startOnAwake)
    { StartTimer(); }
  }
  private void OnValidate() => time = Mathf.Clamp(Mathf.Abs(time), minTime, maxTime);
  private void OnDestroy()
  {
    StopTimer();
    onTimerEnd.RemoveAllListeners();
    onTimerEnd = null;
  }
}
