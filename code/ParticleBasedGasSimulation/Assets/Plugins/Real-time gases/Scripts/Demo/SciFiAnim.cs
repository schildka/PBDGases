using UnityEngine;


public class SciFiAnim : MonoBehaviour
{
    public Animator animator;
    public Animator animatorSecond;


    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            animator.SetBool("character_nearby", true);
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            animatorSecond.SetBool("character_nearby", true);
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            animatorSecond.SetBool("character_nearby", false);
        }
    }
}