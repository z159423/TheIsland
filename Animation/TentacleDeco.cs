using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TentacleDeco : MonoBehaviour
{

    [SerializeField] private Animator animator;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!ES3.KeyExists("tentacledeco"))
            {
                ES3.Save("tentacledeco", true);
                animator.SetTrigger("Show");
            }
        }
    }
}
