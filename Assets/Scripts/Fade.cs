using UnityEngine;

public class Fade : MonoBehaviour
{
    public Animator animator;
    void Start()
    {

    }

    public void FadeOut()
    {
        animator.Play("FadeOut");
    }
}
