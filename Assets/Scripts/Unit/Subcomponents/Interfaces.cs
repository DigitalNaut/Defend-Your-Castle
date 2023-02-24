using UnityEngine;

namespace Unit.Managers
{
  public enum ObstacleType
  {
    None,
    Friendly,
    Obstacle,
    Enemy
  }

  public interface IManaged
  {
    UnitPropertiesManager UnitPropertiesManager { get; }
    Animator Animator { get; }
    SpatialDetector SpatialDetector { get; }
  }
}
