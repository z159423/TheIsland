using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Sirenix.OdinInspector;
using System.Linq;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    [FoldoutGroup("참조")][SerializeField] private FixedTouchField touchField;
    [FoldoutGroup("참조")][SerializeField] private Rigidbody rigid;
    [FoldoutGroup("참조")][SerializeField] private Animator animator;
    [FoldoutGroup("참조")][SerializeField] private Player player;
    [FoldoutGroup("참조")][SerializeField] private NavMeshAgent agent;

    [Space]
    private Vector3 moveDir;
    private float angle;
    public float moveSpeed;
    float movespeedBonus = 0;
    public bool canMove = true;

    bool isTransporting;

    TaskUtil.WhileTaskMethod playerPositionSaveTask;

    private void Start()
    {
        if (SaveManager.Instance.PlayerController.GetRevive())
        {
            agent.Warp(SaveManager.Instance.PlayerController.GetRespawnData().Position);
            this.TaskDelay(Time.deltaTime, () => SaveManager.Instance.PlayerController.SetRevive(false));
        }
        else
        {
            if (!string.IsNullOrEmpty(SaveManager.Instance.PlayerController.GetPortal()))
            {
                var find = FindObjectsOfType<WorldPortal>().FirstOrDefault(f => f.GetPortalName().Equals(SaveManager.Instance.PlayerController.GetPortal()));
                if (find != null)
                {
                    agent.Warp(find.GetWorldPosition());
                    SaveManager.Instance.PlayerController.SetPortal(null);
                }
                else
                {
                    Debug.LogError($"Can't find {SaveManager.Instance.PlayerController.GetPortal()} in {SceneManager.GetActiveScene().name}");
                }
            }
            else
            {
                agent.Warp(SaveManager.Instance.PlayerController.GetLastestSavedPositon(SceneManager.GetActiveScene().name));

            }
        }

        if (SaveManager.Instance.IAPController.GetPurchaseAssistantPack())
            player.SpawnPermanentCompanion();

        playerPositionSaveTask = this.TaskWhile(5f, 0, SavePlayerPosition);

        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed + movespeedBonus;
        agent.acceleration = agent.speed * 20f;

        if (SaveManager.Instance.IAPController.GetPurchaseAssistantPack())
            PlayerManager.Instance.player.SpawnPermanentCompanion();
    }
    // Update is called once per frame
    
    void Update()
    {
        if (player.isDie || !canMove)
            return;

        if (isTransporting)
        {
            animator.SetBool("Moving", false);
            animator.SetFloat("MoveAnimationSpeed", 1f);
        }
        else
        {
            moveDir = touchField.joystickDir.normalized * touchField.distBetweenJoystickBodyToHandle;

            float nomalizeMoveSpeed = touchField.distBetweenJoystickBodyToHandle;

            var delta = new Vector3(moveDir.x, 0, moveDir.y) * (moveSpeed + movespeedBonus) * Time.deltaTime;
            agent.Move(delta);

            //RaycastHit hit;

            //LayerMask mask = LayerMask.GetMask("Ground");

            //for (int i = 1; i <= 3; i++)
            //{
            //    var deltaIter = delta * 10 * i;
            //    var pos = transform.position + deltaIter + (Vector3.up * 100);
            //    if (Physics.Raycast(pos, Vector3.down, out hit, float.MaxValue, mask))
            //    {
            //        agent.SetDestination(hit.point);
            //        break;
            //    }
            //} 

            if (nomalizeMoveSpeed == 0)
            {
                animator.SetBool("Moving", false);
            }
            else
            {
                animator.SetBool("Moving", true);
                animator.SetFloat("MoveAnimationSpeed", nomalizeMoveSpeed);
            }

            if (Mathf.Abs(touchField.joystickDir.normalized.x) > 0 || Mathf.Abs(touchField.joystickDir.normalized.y) > 0)
            {
                angle = Mathf.Atan2((touchField.joystickDir.normalized.y + transform.position.y) - transform.position.y,
                (touchField.joystickDir.normalized.x + transform.position.x) - transform.position.x) * Mathf.Rad2Deg;
            }

            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.AngleAxis(angle - 90, Vector3.down), 10 * Time.deltaTime);
        }

    }

    public void AnimationTrigger(string value) => animator.SetTrigger(value);

    public bool IsTransporting() => isTransporting;
    public void SetTransporting(bool value)
    {
        isTransporting = value;
        agent.enabled = !value;
    }

    public void Warp(Vector3 pos) => agent.Warp(pos);

    void SavePlayerPosition()
    {
        if (!isTransporting)
            SaveManager.Instance.PlayerController.SavePlayerPositon(transform.position);
    }

    public void TouchInit()
    {
        touchField.OnPointerUp(null);
    }

    public void StopSavePlayerPosition() => playerPositionSaveTask?.Kill();

    private GameObject doubleSpeedParticle;

    public void DoubleSpeed(float speed = 3f)
    {
        movespeedBonus = speed;
    }

    public void DoubleSpeedEnd()
    {
        movespeedBonus = 0;
    }
}