using System;
using System.Collections;
using UnityEngine;
using NaughtyAttributes;
using Unit.CombatSystem;

[RequireComponent(typeof(HealthBarManager))]
[DisallowMultipleComponent]
public class UnitPropertiesManager : MonoBehaviour
{
  [Tooltip("The general properties of the Unit.")]
  [Expandable][SerializeField] UnitProperties unitProperties;
  [Tooltip("The combat stats of the Unit.")]
  [Expandable][SerializeField] CombatStats combatStatsRef;

  private HealthBarManager _healthBarManager;
  private Animator _animator;
  public UnitProperties UnitProperties { get; private set; }
  public CombatStats CombatStats { get; private set; }

  /// <summary>
  /// Change the unit's faction.
  /// </summary>
  public void SetUnitFaction(UnitFaction faction) => CombatStats.Faction = faction;

  private void OnDeathHandler()
  {
    _animator.SetBool("IsDead", true);
    _animator.SetBool("HasEnemy", false);
    _animator.SetBool("HasFriendly", false);
    _animator.SetBool("HasObstacle", false);
  }

  public void DestroyUnit()
  {
    IEnumerator DestroyUnitCoroutine()
    {
      yield return new WaitForSeconds(1f);
      Destroy(gameObject);
    }

    StartCoroutine(DestroyUnitCoroutine());
  }

  private void Awake()
  {
    UnitProperties = Instantiate(unitProperties);
    CombatStats = Instantiate(combatStatsRef);

    _healthBarManager = GetComponent<HealthBarManager>();
    _healthBarManager.SetCombatStatsRef(CombatStats);

    _animator = GetComponentInChildren<Animator>();

    CombatStats.SubscribeToDeath(OnDeathHandler);
  }

  private void OnValidate()
  {
    if (!combatStatsRef)
    { throw new Exception("No combat stats assigned to the unit."); }
  }

  private void OnDestroy()
  {
    CombatStats.UnsubscribeFromDeath(OnDeathHandler);
  }
}