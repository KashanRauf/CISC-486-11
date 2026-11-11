
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] private Animator animator;

    
    void Start()
    {
        if (animator == null)
        {
            Debug.LogError("Animator component not found!");
            return;
        }

        if (animator.runtimeAnimatorController == null)
        {
            Debug.LogError("No Animator Controller assigned!");
            return;
        }

        bool parameterExists = false;

        foreach (AnimatorControllerParameter parameter
                in animator.parameters)
        {
            Debug.Log("Animator Parameter: " +
                parameter.name + " | Type: " + parameter.type);

            if (parameter.name == "isRunning" &&
                parameter.type == AnimatorControllerParameterType.Bool)
            {
                parameterExists = true;
            }
        }

        if (!parameterExists)
        {
            Debug.LogError("Missing Boolean parameter: isRunning");
        }
        else
        {
            Debug.Log("isRunning parameter successfully found!");
        }
    }


    void Update()
    {
        if (animator == null || Keyboard.current == null)
            return;

        bool isRunning =
            Keyboard.current.wKey.isPressed ||
            Keyboard.current.aKey.isPressed ||
            Keyboard.current.sKey.isPressed ||
            Keyboard.current.dKey.isPressed;

        Debug.Log("Animator: " + animator.gameObject.name);
        Debug.Log("Controller: " + animator.runtimeAnimatorController.name);

        foreach (var p in animator.parameters)
        {
            Debug.Log("Parameter found: " + p.name + " | " + p.type);
        }

        animator.SetBool("isRunning", isRunning);
    }
}
