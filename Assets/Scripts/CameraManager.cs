using System.Collections;
using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }
     public CinemachineVirtualCamera mainCam;
    public float zoomedFOV = 30f; // Adjust as needed
    public float zoomedDeathFOV = 20f; // Adjust as needed
    public float originalFOV; // Will be set to the initial FOV on Start
    public float zoomInDuration = 0.5f;
    public float zoomOutDuration = 1.0f;
    public float zoomCooldown = 2.0f; // Time in seconds before another zoom can be initiated
    private bool isZooming = false;

    public CinemachineImpulseSource impulse;
    
    [Header("Witness Cams")]
    public CinemachineVirtualCamera witnessCam1;
    public CinemachineVirtualCamera witnessCam2;
    public CinemachineVirtualCamera witnessCam3;
    private CinemachineVirtualCamera[] _witnessCameras;
    [SerializeField] private float shakePower = 0.3f;
    [SerializeField] private float deathShakePower = 1f;


    void Start()
    {
        originalFOV = mainCam.m_Lens.FieldOfView;
        _witnessCameras  = new [] { witnessCam1, witnessCam2, witnessCam3};
    }
    

    IEnumerator ZoomCoroutine(float fov = 0f)
    {
        if (fov == 0f) fov = zoomedFOV;
        // Set flag to prevent spamming
        isZooming = true;

        // Zoom in
        yield return ZoomFOV(fov, zoomInDuration);


        // Zoom out to the original FOV
        yield return ZoomFOV(originalFOV, zoomOutDuration);

        // Optional: Perform any actions after the zoom is complete

        // Reset flag after cooldown period
        yield return new WaitForSeconds(zoomCooldown);
        isZooming = false;
    }

    IEnumerator ZoomFOV(float targetFOV, float duration)
    {
        float elapsed = 0f;
        float startFOV = mainCam.m_Lens.FieldOfView;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            // Calculate the interpolation factor between 0 and 1 based on the elapsed time and duration
            float t = Mathf.Clamp01(elapsed / duration);

            // Lerp the FOV
            mainCam.m_Lens.FieldOfView = Mathf.Lerp(startFOV, targetFOV, t);

            yield return null;
        }

        // Ensure the FOV is exactly the target value to avoid floating-point errors
        mainCam.m_Lens.FieldOfView = targetFOV;
    }
    public void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
        }

        DontDestroyOnLoad(this);
    }

    public void HitEffect()
    {
        if(!isZooming)
         StartCoroutine(ZoomCoroutine());
    }

    public void ExplosiveEffect()
    {
        impulse.GenerateImpulse(0.9f);

    }
    public void DeathEffect()
    {
        if (!isZooming)
        {
            StartCoroutine(ZoomCoroutine(zoomedDeathFOV));
        }
        impulse.GenerateImpulse(deathShakePower);
    }

    public void ScreenShake()
    {
        impulse.GenerateImpulse(shakePower);
    }
    
    
     public void ActivateWitness1()
    {
        ActivateCamera(witnessCam1);
    }

    public void ActivateWitness2()
    {
        ActivateCamera(witnessCam2);
    }

    public void ActivateWitness3()
    {
        ActivateCamera(witnessCam3);
    }
    
    // Activate Main Camera
    public void ActivateMainCamera()
    {
        ActivateCamera(mainCam);
    }

    // Activate a random Virtual Camera by setting priority
    public void ActivateRandomCamera()
    {

        // Shuffle the array to get a random order
        for (int i = 0; i < _witnessCameras.Length; i++)
        {
            int randomIndex = Random.Range(i, _witnessCameras.Length);
            CinemachineVirtualCamera temp = _witnessCameras[i];
            _witnessCameras[i] = _witnessCameras[randomIndex];
            _witnessCameras[randomIndex] = temp;
        }

        // Activate the cameras in the shuffled order with priorities
        for (int i = 0; i < _witnessCameras.Length; i++)
        {
            _witnessCameras[i].Priority = i + 1; // Set priority based on the shuffled order
        }
    }

    // Generic method to activate a specific virtual camera and deactivate others
    private void ActivateCamera(CinemachineVirtualCamera targetCamera)
    {
        // Reset priorities of all cameras
        ResetCameraPriorities();

        // Activate the target virtual camera by setting a higher priority
        targetCamera.Priority = 10; // You can adjust this priority value based on your needs
    }

    // Reset priorities of all virtual cameras
    private void ResetCameraPriorities()
    {
        witnessCam1.Priority = 0;
        witnessCam2.Priority = 0;
        witnessCam3.Priority = 0;
        mainCam.Priority = 0;
    }
    // private void OnGUI()
    // {
    //     // Set up GUI layout
        
    //     if (GUI.Button(new Rect(10, 140, 80, 30), "Hit effect"))
    //     {
    //         HitEffect();
    //     }
        
    //     if (GUI.Button(new Rect(10, 180, 80, 30), "Witness 1"))
    //     {   
    //         ActivateWitness1();
    //     }
        
    //     if (GUI.Button(new Rect(10, 220, 80, 30), "Witness 2"))
    //     {
    //         ActivateWitness2();
    //     }        
    //     if (GUI.Button(new Rect(90, 180, 80, 30), "Main Cam"))
    //     {
    //         ActivateMainCamera();
    //     }     
    //     if (GUI.Button(new Rect(90, 220, 80, 30), "Impulse"))
    //     {
    //         ScreenShake();
    //     }

    // }

}