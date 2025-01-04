using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject balanced;
    [SerializeField] private GameObject attack;
    [SerializeField] private GameObject speed;
    [SerializeField] private GameObject lifeSupport;

    [SerializeField] private GameObject modeWheel;
    private GameObject curMode;
    private GameObject queuedMode;

    [SerializeField] private ProgressBar rightCannonBar;
    [SerializeField] private ProgressBar leftCannonBar;

    public void QueueBalancedImage()
    {
        queuedMode = balanced;
    }
    public void QueueAttackImage()
    {
        queuedMode = attack;
    }
    public void QueueSpeedImage()
    {
        queuedMode = speed;
    }
    public void QueueLifeSupportImage()
    {
        queuedMode = lifeSupport;
    }
    public void ChangeModeImage()
    {
        if(curMode != null) curMode.SetActive(false);
        curMode = queuedMode;
        curMode.SetActive(true);
    }
    public void ToggleModeWheel(Vector3 mousePos, bool newState)
    {
        modeWheel.GetComponent<RectTransform>().position = mousePos;
        modeWheel.SetActive(newState);
    }

    public void InitializeCannonBars(int cannonAmountPerSide)
    {
        rightCannonBar.InitializeBar(cannonAmountPerSide);
        leftCannonBar.InitializeBar(cannonAmountPerSide);
    }
    public ProgressBar GetRightCannonBar()
    { 
        return rightCannonBar;
    }
    public ProgressBar GetLeftCannonBar()
    {
        return leftCannonBar;
    }

    public void ToggleUI(bool newState)
    {
        rightCannonBar.gameObject.SetActive(newState);
        leftCannonBar.gameObject.SetActive(newState);
    }
}
