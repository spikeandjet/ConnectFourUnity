using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class inputField : MonoBehaviour
{
    public int column;
    public int AiMove;
    public gameManager gm;

    void OnMouseDown()
    {
        gm.SelectColumn(column);
     
            Debug.Log("This message will appear immediately");
            StartCoroutine(Delay());
    
    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(1); // Wait for 2 seconds
        gm.SelectColumn(AiMove);
        Debug.Log("This action will happen on spawn");
    }

}
