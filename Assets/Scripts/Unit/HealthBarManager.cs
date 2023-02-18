using UnityEngine;
using UnityEngine.UI;
using System;

[RequireComponent(typeof(CombatBehavior))]
[DisallowMultipleComponent]
public class HealthBarManager : MonoBehaviour
{
  #region Inspector
  [Tooltip("The health bar prefab.")]
  [SerializeField] Slider healthBarRef;
  #endregion

  #region Properties
  public CombatStats CombatStats { get; private set; }
  #endregion

  #region Methods
  public void SetCombatStatsRef(CombatStats combatStats) => CombatStats = combatStats;
  public void UpdateHealthBar()
  {
    if (!healthBarRef) { Debug.LogError("Health bar prefab reference is null.", this); return; }

    healthBarRef.value = CombatStats.HealthPercentage;
  }
  #endregion

  #region Unity Methods
  private void OnValidate()
  {
    if (!healthBarRef) { throw new Exception("Health bar prefab reference is null."); }
  }
  private void OnDestroy()
  {
    CombatStats = null;
  }
  #endregion
}