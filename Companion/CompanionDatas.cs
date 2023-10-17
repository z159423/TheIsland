using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Companion Data", menuName = "Game/CompanionData")]
public class CompanionDatas : ScriptableObject
{
    [SerializeField] GameObject companionPrefab;
    public GameObject GetCompanionPrefab() => companionPrefab;

    [SerializeField] SkinnedMeshRenderer[] companionSkins;
    public SkinnedMeshRenderer[] GetCompanionSkins() => companionSkins;
    public SkinnedMeshRenderer GetRandomCompanionSkins() => companionSkins.GetRandomElement();

}
