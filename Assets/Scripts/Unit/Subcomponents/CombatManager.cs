using UnityEngine;
using Unit.Managers;

/// <summary>
/// A sub-component used to manage the combat behavior of the unit.
/// </summary>
public class CombatManager
{
  private IManaged owner;

  public CombatManager(IManaged owner)
  {
    this.owner = owner;
  }

  public void HandleAttack()
  {
    var hit = owner.SpatialDetector.CheckObstacleForward();

    if (hit.collider == null)
    {
      owner.Animator.SetBool("HasEnemy", false);
      return;
    }

    ApplyAttack(hit);
  }

  private void ApplyAttack(RaycastHit hit)
  {
    var enemyStats = hit.collider.GetComponent<UnitPropertiesManager>().CombatStats;
    var enemyFaction = enemyStats.Faction;

    if (enemyStats.IsDead)
    {
      owner.Animator.SetBool("HasEnemy", false);
      return;
    }

    if (enemyFaction != owner.UnitPropertiesManager.CombatStats.Faction)
    {
      enemyStats.TakeDamage(owner.UnitPropertiesManager.CombatStats.AttackDamage);

      return;
    }
  }
}