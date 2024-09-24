using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class BalloonPath : MonoBehaviour
{
    public List<GameObject> objectPooled;
    public List<ListContianer> listContianers = new List<ListContianer>();

    void Start()
    {
        objectPooled = BalloonManager.Instance.GetPooledObject();
        InitializeListContianers();
        Categorize();
        StartCoroutine(InitalizePool());
    }

    void InitializeListContianers()
    {
        int balloonConfigCount = BalloonManager.Instance.balloonConfigs.Count;
        listContianers = new List<ListContianer>(balloonConfigCount);

        // Initialize ListContianers
        for (int i = 0; i < balloonConfigCount; i++)
        {
            listContianers.Add(new ListContianer());
        }
    }

 IEnumerator InitalizePool()
{
    int store = 0; // Initialize the store outside the loop
    
    for (int i = 0; i < listContianers.Count; i++)
    {
        for (int j = 0; j < BalloonManager.Instance.balloonConfigs[i].balloonCount; j++)
        {
            objectPooled[j + store].SetActive(true); // Use the store value to ensure correct index

            // Apply movement pattern and change sprite for the current balloon
            BalloonManager.Instance.PathMovementPattern(objectPooled[j + store].transform, BalloonManager.Instance.balloonConfigs[i]);
            BalloonManager.Instance.ChangeSprite(objectPooled[j + store].GetComponent<SpriteRenderer>(), BalloonManager.Instance.balloonConfigs[i].sprites);
        }

        // Update the store value with the total number of pooled objects processed so far
        store += BalloonManager.Instance.balloonConfigs[i].balloonCount;
        if(store>=objectPooled.Count)
                   {
                    store=0;
                   }

        // Wait before proceeding to the next configuration
      // Check if the duration is 10, and wait for half the time, otherwise wait for the full duration
float waitTime = (BalloonManager.Instance.balloonConfigs[i].duration >= 10) 
                 ? BalloonManager.Instance.balloonConfigs[i].duration -3 
                 : BalloonManager.Instance.balloonConfigs[i].duration;

// Wait for the specified amount of time
yield return new WaitForSeconds(waitTime);

    }
}

    

    void Categorize()
    {
        // Ensure listContianers has enough capacity
        while (listContianers.Count < BalloonManager.Instance.balloonConfigs.Count)
        {
            listContianers.Add(new ListContianer());
        }

        // Iterate through each balloon configuration
        for (int i = 0; i < BalloonManager.Instance.balloonConfigs.Count; i++)
        {
            var balloonConfig = BalloonManager.Instance.balloonConfigs[i];
            var balloonType = balloonConfig.type; // Assuming balloonType is a property of BalloonConfig

            // Ensure we don’t go out of range for the objectPooled list
            if (i >= objectPooled.Count)
            {
                break; // Exit early if out of range
            }

            // Add balloons to the corresponding container
            for (int j = 0; j < balloonConfig.balloonCount; j++)
            {
                if (j >= objectPooled.Count)
                {
                    break; // Exit early if out of range
                }

                // Store the balloon object and type in the container
                listContianers[i].balloonObjects.Add(objectPooled[j]);
                listContianers[i].balloonType = balloonType; // Set the balloon type
            }
        }
    }
}

[System.Serializable]
public class ListContianer
{
    public List<GameObject> balloonObjects = new List<GameObject>();
    public BalloonType balloonType; // Store balloon type
}
