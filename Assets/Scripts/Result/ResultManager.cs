using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultManager : MonoBehaviour
{
    [SerializeField] private Image resultImage; 
    [SerializeField] private Sprite winSprite; 
    [SerializeField] private Sprite loseSprite;
    
    // Start is called before the first frame update
    void Start()
    {
        if (GameManager.isWin) 
        { 
            resultImage.sprite = winSprite; 
        } 
        else 
        { 
            resultImage.sprite = loseSprite; 
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
