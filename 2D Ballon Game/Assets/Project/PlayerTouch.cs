using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTouch : MonoBehaviour
{
    // Assign the layer in the inspector or set it via code
    public LayerMask playerLayer; 
public GameObject ballonPrefab;

void Start()
{

    Vector2 pos = new Vector2(0,Random.Range(0,4));
    Instantiate(ballonPrefab,pos,Quaternion.identity);



}
    void Update()
    {
        Cast();  // Call the Cast function in Update to check for touch inputs
    }
    
    void Cast()																																		
    {
        for (int i = 0; i < Input.touchCount; ++i)
        {
            // Convert touch position to world point
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.GetTouch(i).position);
            
            // Check if the touch is stationary
            if (Input.GetTouch(i).phase == TouchPhase.Stationary)
            {
                // Cast a ray from the touch position into the scene, filtering by the playerLayer
                RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero, Mathf.Infinity, playerLayer);
                
                // Check if the ray hit a collider on the playerLayer
                if (hit.collider != null)
                {
                    // Do stuff when the object on the playerLayer is touched
                   hit.collider.gameObject.SetActive(false);
                }
            }
        }
    }


    
}
