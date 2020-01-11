using UnityEngine;
using Mirror;

public class PlayerSetup : NetworkBehaviour
{

    [SerializeField] Behaviour[] componentsToDisable = null;
    Camera sceneCamera = null;

    // Start is called before the first frame update
    void Start()
    {
        //Disable components on other players (audio listeners, other controllers etc)
        if(hasAuthority == false)
        {
            for (int i = 0; i < componentsToDisable.Length; i++)
            {
                componentsToDisable[i].enabled = false;
            }
        }
        else
        {
            //Disable scene camera
            sceneCamera = Camera.main;
            if(sceneCamera != null)
            {
                sceneCamera.gameObject.SetActive(false);
            }
        }
    }

    //When object is removed from game
    void OnDisable()
    {
        //Re-enable scene camera
        if(sceneCamera != null)
        {
            sceneCamera.gameObject.SetActive(true);
        }
    }
}
