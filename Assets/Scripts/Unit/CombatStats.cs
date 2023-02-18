using UnityEngine;
using UnityEngine.Events;
using CombatSystem;

[CreateAssetMenu(fileName = "UnitCombatStats", menuName = "Units/CombatStats", order = 1)]
public class CombatStats : ScriptableObject
{
  /// <summary>
  /// Events fired when the unit takes damage or dies.
  /// </summary>
  public UnityEvent OnDamageTaken;
  /// <summary>
  /// Events fired when the unit takes damage or dies.
  /// </summary>
  public UnityEvent OnDeath;

  /// <summary>
  /// The type of combat the unit is capable of.
  /// </summary>
  [Tooltip("The type of combat the unit is capable of.")]
  public CombatType CombatStyle = CombatType.Melee;
  [Tooltip("The attack stat of the unit.")]
  public float AttackDamage = 10f;
  [Tooltip("The defense stat of the unit.")]
  public float Defense = 5f;
  [Tooltip("The health stat of the unit.")]
  public float MaxHealth = 100f;
  [Tooltip("The health stat of the unit.")]
  public float CurrentHealth { get; private set; } = 100f;
  [Tooltip("The attack speed stat of the unit in attacks per second.")]
  public float AttackSpeed = 1f;
  [Tooltip("Faction of the unit.")]
  public UnitFaction Faction = UnitFaction.Player;

  /// <summary>
  /// Returns the unit's health as a percentage of its max health.
  /// </summary>
  public float HealthPercentage => CurrentHealth / MaxHealth;

  /// <summary>
  /// Returns true if the unit's health is 0 or less.
  /// </summary>
  public bool IsDead => CurrentHealth <= 0;

  /// <summary>
  /// Reduces the unit's health by the specified amount.
  /// Prevents the unit's health from going below 0.
  /// Fires the related events.
  /// </summary>
  public void TakeDamage(float damage)
  {
    CurrentHealth -= damage;

    // if (damage > 0) { OnDamageTaken?.Invoke(); }

    if (CurrentHealth > 0) return;

    CurrentHealth = 0;
    OnDeath?.Invoke();
  }

  public void OnDestroy()
  {
    OnDamageTaken.RemoveAllListeners();
    OnDeath.RemoveAllListeners();
  }
}