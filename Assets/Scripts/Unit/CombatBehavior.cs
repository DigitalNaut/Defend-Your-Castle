using UnityEngine;
using NaughtyAttributes;
using CombatSystem;

[RequireComponent(typeof(SpatialAwarenessManager))]
[RequireComponent(typeof(UnitAnimationsManager))]
[RequireComponent(typeof(HealthBarManager))]
[DisallowMultipleComponent]
public class CombatBehavior : MonoBehaviour
{
  #region Inspector
  [SerializeField] bool debugging = false;

  [Tooltip("The combat stats of the unit.")]
  [Expandable][SerializeField] CombatStats combatStatsRef;

  [ShowNativeProperty] public float Health => combatStats ? combatStats.CurrentHealth : -1f;
  #endregion

  #region Fields
  internal CombatStats combatStats;
  private SpatialAwarenessManager spatialAwarenessManager;
  [ShowNonSerializedField] private UnitAnimationsManager unitAnimationsManager;
  private HealthBarManager healthBarManager;
  #endregion

  #region Properties
  public bool IsDead => combatStats.IsDead;
  public float AttackDamage => combatStats.AttackDamage;
  public UnitFaction Faction => combatStats.Faction;
  #endregion

  #region Methods
  /// <summary>
  /// Caches the components on the object.
  /// </summary> 
  private void CacheComponents()
  {
    spatialAwarenessManager = GetComponent<SpatialAwarenessManager>();
    unitAnimationsManager = GetComponent<UnitAnimationsManager>();
    healthBarManager = GetComponent<HealthBarManager>();
  }

  /// <summary>
  /// Subscribes to the necessary events.
  /// </summary>
  private void SubscribeToEvents()
  {
    // Subscribe to unit animation events.
    unitAnimationsManager.OnAttackAnimationEnd.AddListener(SetIdle);
    unitAnimationsManager.OnDeathAnimationEnd.AddListener(DestroyUnit);

    // Subscribe to combat stats events.
    combatStats.OnDeath.AddListener(Die);
    combatStats.OnDamageTaken.AddListener(healthBarManager.UpdateHealthBar);
  }

  /// <summary>
  /// Unsubscribes from all events.
  /// </summary>
  private void UnsubscribeFromAllEvents()
  {
    UnsubscribeFromNonDeathEvents();
    UnsubscribeFromDeathEvents();
  }

  /// <summary>
  /// Unsubscribes from events not related to death.
  /// </summary>
  private void UnsubscribeFromNonDeathEvents()
  {
    combatStats.OnDamageTaken.RemoveListener(healthBarManager.UpdateHealthBar);
    unitAnimationsManager.OnAttackAnimationEnd.RemoveListener(SetIdle);
  }

  /// <summary>
  /// Unsubscribes from events related to death.
  /// </summary>
  private void UnsubscribeFromDeathEvents()
  {
    combatStats.OnDeath.RemoveListener(Die);
    unitAnimationsManager.OnDeathAnimationEnd.RemoveListener(DestroyUnit);
  }

  /// <summary>
  /// Ready the combat stats.
  /// </summary>
  private void InitCombatStats()
  {
    if (!combatStatsRef)
    {
      Debug.LogError("Combat stats reference is null.");
      return;
    }

    combatStats = Instantiate(combatStatsRef);
    healthBarManager.SetCombatStatsRef(combatStats);
  }

  /// <summary>
  /// Caches and initializes the necessary components.
  /// </summary>
  private void CacheAndInitComponents()
  {
    CacheComponents();
    InitCombatStats();
  }

  /// <summary>
  /// Changes the unit's faction.
  /// </summary>
  /// <param name="faction"></param>
  public void SetFaction(UnitFaction faction)
  {
    if (!combatStats) return;

    combatStats.Faction = faction;
  }

  /// <summary>
  /// Attacks the first enemy in the spatial awareness manager's hits array.
  /// </summary>
  /// <remarks>This method is meant to be called by a Unity event.</remarks>
  public void Attack()
  {
    if (IsDead) return;
    if (unitAnimationsManager.IsAttacking) return;
    if (!spatialAwarenessManager) return;
    if (spatialAwarenessManager.Hits.Length <= 0) return;

    var raycastHit = spatialAwarenessManager.Hits[0];
    raycastHit.collider.TryGetComponent(out CombatBehavior enemy);

    if (!enemy) return;
    if (enemy.Faction == Faction) return;
    if (enemy.IsDead) return;

    if (debugging) { Debug.Log($"{gameObject.name} is attacking {enemy.name}.", gameObject); }

    unitAnimationsManager.SetAttacking();
    enemy.TakeDamage(AttackDamage);
  }

  /// <summary>
  /// Reduces the unit's health by the specified amount.
  /// </summary>
  /// <param name="damage">The amount of damage to be dealt.</param>
  public void TakeDamage(float damage) => combatStats.TakeDamage(damage);

  /// <summary>
  /// Destroys the game object upon death.
  /// </summary>
  private void Die()
  {
    Debug.Log($"{gameObject.name} is dying.", gameObject);

    unitAnimationsManager.SetDying();
    spatialAwarenessManager.StopUpdating();
    UnsubscribeFromNonDeathEvents();
  }

  /// <summary>
  /// Destroys the game object upon death.
  /// </summary>
  private void DestroyUnit()
  {
    Debug.Log($"{gameObject.name} is dead.");

    Destroy(gameObject);
  }

  [Button] public void SetIdle() => unitAnimationsManager.SetIdle();
  [Button] public void SetAttacking() => unitAnimationsManager.SetAttacking();
  [Button] public void SetMoving() => unitAnimationsManager.SetMoving();
  [Button] public void SetDying() => unitAnimationsManager.SetDying();
  #endregion

  #region Unity Methods
  private void OnValidate()
  {
    if (!combatStatsRef)
    { Debug.LogWarning("No combat stats assigned to the unit.", gameObject); }
  }

  private void Awake() => CacheAndInitComponents();
  private void OnEnable() => SubscribeToEvents();
  private void OnDisable() => UnsubscribeFromAllEvents();
  private void OnDestroy() => UnsubscribeFromAllEvents();
  #endregion
}