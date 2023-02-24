using UnityEngine;
using Unit.Managers;

/// <summary>
/// A sub-component used to check what kind of obstacle in front of the unit.
/// </summary>
public class ObstacleDetector
{
  private IManaged owner;

  public ObstacleDetector(IManaged owner)
  {
    this.owner = owner;
  }

  public ObstacleType CheckObstacle(RaycastHit hit)
  {
    if (hit.collider == null)
    { return ObstacleType.None; }

    if (!hit.collider.TryGetComponent(out UnitPropertiesManager otherProperties))
    { return ObstacleType.Obstacle; }

    if (otherProperties.CombatStats.IsDead)
    { return ObstacleType.Obstacle; }

    var isSameFaction = otherProperties.CombatStats.Faction == owner.UnitPropertiesManager.CombatStats.Faction;

    if (isSameFaction)
    { return ObstacleType.Friendly; }
    else
    { return ObstacleType.Enemy; }
  }
}