namespace CombatSystem
{
  public enum UnitFaction { Player, Enemy }
  public enum CombatType { Melee, Ranged }
  
  /// <summary>
  /// The states of the unit animation state machine.
  /// </summary>
  public enum UnitAnimationState
  {
    Dead,
    Idle,
    Moving,
    Attacking,
  }
}