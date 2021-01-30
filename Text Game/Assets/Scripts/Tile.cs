using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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
        for (int i = 0; i < map.tiles.Count; i++)
        {
            map.tiles[i].GetComponentInChildren<SpriteRenderer>().color = Color.white;
        }


        if (map.isMoving == false)
        {
            map.selectedTile = this.gameObject;
        }

        for (int i = 0; i < this.Neighbours.Count; i++)
        {
            this.Neighbours[i].GetComponentInChildren<SpriteRenderer>().color = Color.red;
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
