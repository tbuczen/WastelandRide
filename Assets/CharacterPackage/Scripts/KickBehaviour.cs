using UnityEngine;
using System.Collections;
using CharacterPackage.Scripts;

public class KickBehaviour : MonoBehaviour
{

    static Animator anim;
    private UserController uc;

    void Start()
    {
        anim = GetComponent<Animator>();
        uc = gameObject.GetComponent<UserController>();
    }

    // Update is called once per frame
    void Update()
    {
        /**
        * KICK
        */
        if (Input.GetButtonDown("Kick") && uc.GetUserBaseTranslation() == 0)
        {
            anim.SetTrigger("isKicking");
        }
    }
}
