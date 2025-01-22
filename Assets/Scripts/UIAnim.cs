using UnityEngine;

public class UIAnim : MonoBehaviour
{
    public Animator information;

    public void skip()
    {
        information.SetBool("SKIP", true);
    }
}
