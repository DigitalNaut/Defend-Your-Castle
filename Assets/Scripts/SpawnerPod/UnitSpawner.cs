
using UnityEngine;
using NaughtyAttributes;
using CombatSystem;
using System;

[RequireComponent(typeof(MeshRenderer))]
[DisallowMultipleComponent]
public class UnitSpawner : MonoBehaviour
{
  #region Fields
  [Tooltip("The unit prefab to spawn.")]
  [ShowAssetPreview][SerializeField] GameObject unitPrefabRef;
  [Tooltip("The offset of the unit's position from the spawner pod's position.")]
  [SerializeField] float vOffset = 0;
  [Tooltip("The faction to assign to the spawned unit.")]
  [SerializeField] UnitFaction faction;
  [Tooltip("The color of the spawner pod when the unit cannot be spawned.")]
  [SerializeField] Color blockedColor = Color.red;
  [Tooltip("The color of the spawner pod when the unit can be spawned.")]
  [SerializeField] Color readyColor = Color.green;
  #endregion

  #region Fields
  private MeshRenderer meshRenderer;
  private Vector3 UnitColliderExtents;
  private CapsuleCollider UnitCollider;
  private Mesh UnitMesh;
  private Vector3 UnitPosition => transform.position + new Vector3(0, vOffset, 0);
  private Vector3 UnitCenter => UnitPosition + Vector3.up * UnitColliderExtents.y;
  private Quaternion UnitRotation => transform.rotation;
  #endregion

  #region Component Preparation
  /// <summary>
  /// Evaluates the unit's collider bounds.
  /// </summary>
  /// <returns>The unit's capsule collider bounds.</returns>
  private Vector3 EvaluateColliderExtents() => new Vector3(UnitCollider.radius, UnitCollider.height * 0.5f, UnitCollider.radius);

  /// <summary>
  /// Caches the unit's components and evaluates the necessary properties.
  /// </summary>
  private void CacheAndEvaluate()
  {
    meshRenderer = GetComponent<MeshRenderer>();

    if (!unitPrefabRef) throw new Exception("No unit prefab assigned to the spawner pod.");

    UnitCollider = unitPrefabRef.GetComponentInChildren<CapsuleCollider>();
    if (!UnitCollider) throw new Exception("No capsule collider found on the unit prefab.");

    var skinnedMeshRenderer = unitPrefabRef.GetComponentInChildren<SkinnedMeshRenderer>();
    if (!skinnedMeshRenderer) throw new Exception("No skinned mesh renderer found on the unit prefab.");

    UnitMesh = skinnedMeshRenderer.sharedMesh;
    if (!UnitMesh) throw new Exception("No mesh assigned to the unit prefab.");

    UnitColliderExtents = EvaluateColliderExtents();
    if (UnitColliderExtents == Vector3.zero) throw new Exception("The capsule collider's bounds are zero.");
  }
  #endregion

  #region Methods
  /// <summary>
  /// Spawns a unit at the spawner pod's position if the space is free.
  /// </summary>
  [Button]
  public void SpawnUnit()
  {
    var isSpaceFree = EvaluateFreeSpace();

    ChangePodColor(isSpaceFree ? readyColor : blockedColor);

    if (isSpaceFree)
    {
      var newUnit = Instantiate(unitPrefabRef, UnitPosition, UnitRotation);
      ConfigureUnit(newUnit);
    }
  }

  private void ConfigureUnit(GameObject unit)
  {
    var combatBehavior = unit.GetComponent<CombatBehavior>();
    if (!combatBehavior) throw new Exception("No unit controller found on the unit prefab.");

    unit.name = $"{faction} {unit.name}";
    combatBehavior.SetFaction(faction);
  }

  /// <summary>
  /// Evaluates the free space at the spawner pod's position.
  /// </summary>
  /// <returns>True if the space is free, false otherwise.</returns>
  private bool EvaluateFreeSpace()
  {
    var collider = new Collider[1];
    var collision = Physics.OverlapBoxNonAlloc(UnitCenter, UnitColliderExtents, collider, UnitRotation);
    var isSpaceFree = collision == 0;

    return isSpaceFree;
  }

  /// <summary>
  /// Changes the color of the spawner pod.
  /// </summary>
  /// <param name="color">The color to change to.</param>
  private void ChangePodColor(Color color) => meshRenderer.material.color = color;
  #endregion

  #region Unity Methods
  private void OnValidate() => CacheAndEvaluate();

  private void Awake() => CacheAndEvaluate();
  #endregion

  #region Gizmos
  public void OnDrawGizmos()
  {
    CacheAndEvaluate();

    Gizmos.color = Color.white;
    Gizmos.DrawWireMesh(UnitMesh, UnitPosition, UnitRotation);

    Gizmos.color = Color.blue;
    Gizmos.DrawWireCube(UnitCenter, UnitColliderExtents * 2);

    Gizmos.color = Color.red;
    Gizmos.DrawWireSphere(UnitPosition, 0.1f);

    Gizmos.color = Color.green;
    Gizmos.DrawWireSphere(UnitCenter, 0.1f);
  }
  #endregion
}