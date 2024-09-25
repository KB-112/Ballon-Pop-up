using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FrameRateTest : MonoBehaviour
{
    [SerializeField] private TMP_Text fpsDisplay;  // A UI Text element to display FPS
    private float deltaTime = 0.0f;
    private int frameCount = 0;
    private float elapsedTime = 0.0f;
    private float fps = 0.0f;
     public int targetFps;

    void Start()
    {
        // Set target frame rate to 60 FPS
        Application.targetFrameRate =targetFps;
    }

    void Update()
    {
        // Calculate the deltaTime each frame (time between frames)
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        frameCount++;
        elapsedTime += Time.unscaledDeltaTime;

        // Update FPS every second
        if (elapsedTime > 1.0f)
        {
            fps = frameCount / elapsedTime;
            frameCount = 0;
            elapsedTime = 0.0f;

            // Display FPS on the screen
            if (fpsDisplay != null)
            {
                fpsDisplay.text = string.Format("{0:0.}", fps);
            }
        }
    }
}
