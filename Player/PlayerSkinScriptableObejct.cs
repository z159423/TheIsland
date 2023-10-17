using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Player Skin", menuName = "Game/PlayerSkin")]
public class PlayerSkinScriptableObejct : ScriptableObject
{
    [HorizontalGroup("Guid")][ReadOnly][SerializeField] string guid;
    public string Guid { get => guid; private set => guid = value; }
    [field: SerializeField] public Mesh mesh { get; private set; }
    [field: SerializeField] public Sprite skinIcon { get; private set; }


    [field: SerializeField] public string skinNameKey { get; private set; }


    [HorizontalGroup("Guid")]
    [Button(SdfIconType.ArrowClockwise)]
    virtual protected void NewGuid()
    {
        Guid = System.Guid.NewGuid().ToString();
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }
}
