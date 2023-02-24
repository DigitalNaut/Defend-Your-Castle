using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WalkingStats", menuName = "Units/WalkingStats", order = 1)]
public class UnitProperties : ScriptableObject
{
  #region Fields
  public float WalkSpeed = 1f;
  #endregion
}
