using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FrameRateTest : MonoBehaviour
{
    [SerializeField] private TMP_Text fpsDisplay;  // UI Text element for FPS display
    private float deltaTime = 0.0f;
    private int frameCount = 0;
    private float elapsedTime = 0.0f;
    private float fps = 0.0f;
    public int targetFps;  // Desired frame rate

    void Start()
    {
        // Set the target frame rate for the application
        Application.targetFrameRate = targetFps;
    }

    void Update()
    {
        // Update deltaTime for frame rate calculation
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        frameCount++;
        elapsedTime += Time.unscaledDeltaTime;

        // Calculate FPS every second
        if (elapsedTime > 1.0f)
        {
            fps = frameCount / elapsedTime;
            frameCount = 0;
            elapsedTime = 0.0f;

            // Update the FPS display
            if (fpsDisplay != null)
            {
                fpsDisplay.text = string.Format("{0:0.}", fps);
            }
        }
    }
}
