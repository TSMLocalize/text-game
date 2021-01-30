using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public float x;
    public float y;

    public GameObject parent;

    public List<GameObject> Neighbours;

    public float gCost;
    public float hCost;

    public bool tested;

    public GridMap map;

    void Update()
    {

    }

    private void Start()
    {
        map = FindObjectOfType<GridMap>();
    }

    void OnMouseDown()
    {
        if (map.isMoving == false)
        {
            map.selectedTile = this.gameObject;
        }
    }

    public float fCost
    {
        get
        {
            return gCost + hCost;
        }
    }
}
