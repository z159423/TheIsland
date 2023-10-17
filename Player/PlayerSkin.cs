using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerSkin : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer playerSkin;

    private void Start()
    {
        ChangePlayerSkin(SaveManager.Instance.PlayerController.GetPlayerCurrentSkin());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            foreach (var skin in ResourceManager.Instance.GetplayerSkines())
            {
                if (!SaveManager.Instance.PlayerController.GetOwnedPlayerSkins().Contains(new DString(skin.Guid)))
                {
                    ChangePlayerSkin(skin.Guid);

                    var UI = UIManager.Instance.ShowScreen("UI/New Skin Acquire UI");

                    UI.GetComponentInChildren<NewSkinUIBase>().InitDummy(SaveManager.Instance.PlayerController.GetPlayerCurrentSkin());

                    break;
                }
            }
        }
    }

    public void ChangePlayerSkin(string guid)
    {
        playerSkin.sharedMesh = ResourceManager.Instance.GetPlayerSkin(guid)?.mesh;

        SaveManager.Instance.PlayerController.ChangePlayerSkin(guid);

        PlayerManager.Instance.ChangeDummySkin(guid);
    }
}
