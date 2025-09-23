using UnityEngine;

public class PosteDeLuz : MonoBehaviour
{
    [SerializeField] private Light[] luz;


    void Start()
    {

    }


    void Update()
    {
        AcendeApagarLuzes();
    }

    void AcendeApagarLuzes()
    {
        if (WorldTimeManager.instance.isNight)
        {
            foreach (var l in luz)
            {
                l.enabled = true;
            }
        }
        else
        {
            foreach (var l in luz)
            {
                l.enabled = false;
            }
        }
    }
}
