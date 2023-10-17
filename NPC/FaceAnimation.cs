using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class FaceAnimation : SerializedMonoBehaviour
{
    [SerializeField] SkinnedMeshRenderer skinnedMeshRenderer;
    [SerializeField] NPCScriptableObject stat;

    public void Init(SkinnedMeshRenderer skinned, NPCScriptableObject stat)
    {
        skinnedMeshRenderer = skinned;
        this.stat = stat;

        if (stat.faceDicCustom.ContainsKey("Die"))
        {
            GetComponent<NPC>().onDeath.AddListener(() => ChangeFaceCustom("Die"));
        }
    }

    ///<summary>
    /// string으로 표정 가져와서 변경
    ///</summary>
    public void ChangeFaceCustom(string faceKey)
    {
        foreach (var dic in stat.faceDicCustom)
        {
            if (dic.Key == faceKey)
            {
                skinnedMeshRenderer.materials[1].SetTextureOffset("_BaseMap", dic.Value.mouthOffset);
                skinnedMeshRenderer.materials[2].SetTextureOffset("_BaseMap", dic.Value.eyeOffset);
                return;
            }
        }

        Debug.LogError(faceKey + " 키에 해당하는 표정 정보 Dic이 없습니다");
    }

    ///<summary>
    /// Ai_State에 따라서 자동으로 표정 변경
    ///</summary>
    public void ChangeFaceForAI_State(AI_STATE currentState)
    {
        foreach (var dic in stat.faceDicForAi_State)
        {
            if (dic.Key == currentState)
            {
                skinnedMeshRenderer.materials[1].SetTextureOffset("_BaseMap", dic.Value.mouthOffset);
                skinnedMeshRenderer.materials[2].SetTextureOffset("_BaseMap", dic.Value.eyeOffset);
                break;
            }
        }
    }
}
