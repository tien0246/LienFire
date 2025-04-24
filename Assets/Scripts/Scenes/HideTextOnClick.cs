using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideTextOnClick : MonoBehaviour
{
    public GameObject text;
    // Start is called before the first frame update
    public void HideText()
    {
        text.SetActive(false);
    }
}
