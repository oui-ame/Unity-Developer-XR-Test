using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
class LibraryElement
{
    public GameObject gameObject;
    public Sprite sprite;
}

public class LibraryLoader : MonoBehaviour
{
    [SerializeField]
    private List<LibraryElement> libraryElements;

    [SerializeField]
    private GameObject elementsParent;


    // Start is called before the first frame update
    void Start()
    {
        foreach (var elt in  libraryElements)
        {
            Debug.Log($"GameObject: {elt.gameObject.name}, Sprite: {elt.sprite.name}");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
