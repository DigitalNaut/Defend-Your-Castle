using UnityEngine;
using Unit.CombatSystem;
using System;

[CreateAssetMenu(fileName = "UnitCombatStats", menuName = "Units/CombatStats", order = 1)]
public class CombatStats : ScriptableObject
{
  /// <summary>
  /// Events fired when the unit takes damage or dies.
  /// </summary>
  private event Action OnDamageTaken;
  /// <summary>
  /// Events fired when the unit takes damage or dies.
  /// </summary>
  private event Action OnDeath;
  private event Action OnHealthChanged;

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

  // Event subscriptions
  public void SubscribeToDamageTaken(Action action) => OnDamageTaken += action;
  public void SubscribeToDeath(Action action) => OnDeath += action;
  public void UnsubscribeFromDamageTaken(Action action) => OnDamageTaken -= action;
  public void UnsubscribeFromDeath(Action action) => OnDeath -= action;
  public void SubscribeToHealthChanged(Action action) => OnHealthChanged += action;
  public void UnsubscribeFromHealthChanged(Action action) => OnHealthChanged -= action;

  /// <summary>
  /// Reduces the unit's health by the specified amount.
  /// Prevents the unit's health from going below 0.
  /// Fires the related events.
  /// </summary>
  public void TakeDamage(float damage)
  {
    if (CurrentHealth <= 0) { return; }

    CurrentHealth -= damage;

    if (damage > 0)
    {
      OnHealthChanged?.Invoke();
      OnDamageTaken?.Invoke();
    }

    if (CurrentHealth > 0) { return; }

    CurrentHealth = 0;
    OnDeath?.Invoke();
  }

  public void OnDestroy()
  {
    OnDamageTaken = null;
    OnHealthChanged = null;
    OnDeath = null;
  }
}