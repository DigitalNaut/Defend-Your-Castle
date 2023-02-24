using UnityEngine;
using UnityEngine.UI;
using System;

[DisallowMultipleComponent]
public class HealthBarManager : MonoBehaviour
{
  [Tooltip("The health bar prefab.")]
  [SerializeField] private Slider _healthBarRef;
  public CombatStats CombatStats { get; private set; }

  private void OnValidate()
  {
    if (!_healthBarRef) { throw new Exception("Health bar prefab reference is null."); }
  }

  private void OnDestroy()
  {
    CombatStats.UnsubscribeFromHealthChanged(UpdateHealthBar);
    CombatStats = null;
  }

  public void SetCombatStatsRef(CombatStats combatStats)
  {
    CombatStats = combatStats;
    CombatStats.SubscribeToHealthChanged(UpdateHealthBar);
  }

  public void UpdateHealthBar()
  {
    _healthBarRef.value = CombatStats.HealthPercentage;
  }
}