using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerInputAction{
public class InheritanceSceneBox : MonoBehaviour
{
    private Animator BoxAnimator=>GetComponentInChildren<Animator>();
    private Vector3 PlayerPositionTarget;


    private void Start()
    {
        PlayerPositionTarget = new Vector3(transform.position.x, transform.position.y-7.35f,0);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().SetInheritanceSceneBox(this);
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().ClearInheritanceSceneBox();
        }
    }

    public Vector3 GetPlayerPositionTarget()
    {
        return PlayerPositionTarget;
    }
    
    public void OpenBox()
    {
        BoxAnimator.SetBool("BoxClose",false);
        BoxAnimator.CrossFade("BoxOpen", 0.1f);
    }

    public void CloseBox()
    {
        BoxAnimator.SetBool("BoxClose",true);
    }
}
}