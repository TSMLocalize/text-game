using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GridMap : MonoBehaviour
{
    public GameObject tile;
    public GameObject playerPrefab;
    public GameObject player;
    public List<GameObject> tiles;
    public List<GameObject> OpenNodes;
    public List<GameObject> ClosedNodes;
    public float width;
    public float height;
    public float startX;
    public float startY;
    public float endX;
    public float endY;
    public float XoffSet;
    public float YoffSet;
    public GameObject winningTile;
    public GameObject startTile;
    public float fCostToBeat = 9999f;
    public Vector2 target;
    public GameObject selectedTile;
    public bool isMoving;
    public List<GameObject> TestNodes;

    // Start is called before the first frame update
    void Start()        
    {        
        width = 16f;
        height = 7f;

        XoffSet = 0.8f;
        YoffSet = 0.5f;

        //Create Grid and set Tile Data
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject tileGO;

                if (x % 2 == 0)
                {
                    tileGO = Instantiate(tile, new Vector3((this.gameObject.transform.position.x + x) * XoffSet, (this.gameObject.transform.position.y + y) * YoffSet), Quaternion.identity, this.gameObject.transform);
                }
                else
                {
                    tileGO = Instantiate(tile, new Vector3((this.gameObject.transform.position.x + x) * XoffSet, ((this.gameObject.transform.position.y + y) + 0.5f) * YoffSet), Quaternion.identity, this.gameObject.transform);
                }

                tileGO.name = x + ", " + y;
                tileGO.GetComponent<Tile>().x = x;
                tileGO.GetComponent<Tile>().y = y;

                if (tileGO.GetComponent<Tile>().x == 6 && tileGO.GetComponent<Tile>().y == 3 ||
                    tileGO.GetComponent<Tile>().x == 6 && tileGO.GetComponent<Tile>().y == 4 ||
                    tileGO.GetComponent<Tile>().x == 6 && tileGO.GetComponent<Tile>().y == 5)
                {
                    Destroy(tileGO);
                }
                else
                {
                    tiles.Add(tileGO);
                }
            }
        }

        player = Instantiate(playerPrefab, new Vector3(0, 0), Quaternion.identity);

        //Add neighbours to each tile
        for (int i = 0; i < tiles.Count; i++)
        {
            for (int y = 0; y < tiles.Count; y++)
            {
                if (tiles[y].GetComponent<Tile>().x % 2 == 0)
                {
                    //Upper
                    if (tiles[y].GetComponent<Tile>().x == tiles[i].GetComponent<Tile>().x + 0 &&
                        tiles[y].GetComponent<Tile>().y == tiles[i].GetComponent<Tile>().y + 1)
                    {
                        tiles[i].GetComponent<Tile>().Neighbours.Add(tiles[y]);
                    }
                    //Upper Left
                    else if (tiles[y].GetComponent<Tile>().x == tiles[i].GetComponent<Tile>().x - 1 &&
                             tiles[y].GetComponent<Tile>().y == tiles[i].GetComponent<Tile>().y + 1)
                    {
                        tiles[i].GetComponent<Tile>().Neighbours.Add(tiles[y]);
                    }
                    //Upper Right
                    else if (tiles[y].GetComponent<Tile>().x == tiles[i].GetComponent<Tile>().x + 1 &&
                             tiles[y].GetComponent<Tile>().y == tiles[i].GetComponent<Tile>().y + 1)
                    {
                        tiles[i].GetComponent<Tile>().Neighbours.Add(tiles[y]);
                    }
                    //Lower
                    else if (tiles[y].GetComponent<Tile>().x == tiles[i].GetComponent<Tile>().x + 0 &&
                             tiles[y].GetComponent<Tile>().y == tiles[i].GetComponent<Tile>().y - 1)
                    {
                        tiles[i].GetComponent<Tile>().Neighbours.Add(tiles[y]);
                    }
                    //Lower Left
                    else if (tiles[y].GetComponent<Tile>().x == tiles[i].GetComponent<Tile>().x - 1 &&
                             tiles[y].GetComponent<Tile>().y == tiles[i].GetComponent<Tile>().y + 0)
                    {
                        tiles[i].GetComponent<Tile>().Neighbours.Add(tiles[y]);
                    }
                    //Lower Right
                    else if (tiles[y].GetComponent<Tile>().x == tiles[i].GetComponent<Tile>().x + 1 &&
                             tiles[y].GetComponent<Tile>().y == tiles[i].GetComponent<Tile>().y + 0)
                    {
                        tiles[i].GetComponent<Tile>().Neighbours.Add(tiles[y]);
                    }
                }
                else
                {
                    //Upper
                    if (tiles[y].GetComponent<Tile>().x == tiles[i].GetComponent<Tile>().x + 0 &&
                        tiles[y].GetComponent<Tile>().y == tiles[i].GetComponent<Tile>().y + 1)
                    {
                        tiles[i].GetComponent<Tile>().Neighbours.Add(tiles[y]);
                    }
                    //Upper Left
                    else if (tiles[y].GetComponent<Tile>().x == tiles[i].GetComponent<Tile>().x - 1 &&
                             tiles[y].GetComponent<Tile>().y == tiles[i].GetComponent<Tile>().y + 0)
                    {
                        tiles[i].GetComponent<Tile>().Neighbours.Add(tiles[y]);
                    }
                    //Upper Right
                    else if (tiles[y].GetComponent<Tile>().x == tiles[i].GetComponent<Tile>().x + 1 &&
                             tiles[y].GetComponent<Tile>().y == tiles[i].GetComponent<Tile>().y + 0)
                    {
                        tiles[i].GetComponent<Tile>().Neighbours.Add(tiles[y]);
                    }
                    //Lower
                    else if (tiles[y].GetComponent<Tile>().x == tiles[i].GetComponent<Tile>().x + 0 &&
                             tiles[y].GetComponent<Tile>().y == tiles[i].GetComponent<Tile>().y - 1)
                    {
                        tiles[i].GetComponent<Tile>().Neighbours.Add(tiles[y]);
                    }
                    //Lower Left
                    else if (tiles[y].GetComponent<Tile>().x == tiles[i].GetComponent<Tile>().x - 1 &&
                             tiles[y].GetComponent<Tile>().y == tiles[i].GetComponent<Tile>().y - 1)
                    {
                        tiles[i].GetComponent<Tile>().Neighbours.Add(tiles[y]);
                    }
                    //Lower Right
                    else if (tiles[y].GetComponent<Tile>().x == tiles[i].GetComponent<Tile>().x + 1 &&
                             tiles[y].GetComponent<Tile>().y == tiles[i].GetComponent<Tile>().y - 1)
                    {
                        tiles[i].GetComponent<Tile>().Neighbours.Add(tiles[y]);
                    }
                }

            }
        }

        for (int i = 0; i < tiles.Count; i++)
        {
            if (player.GetComponent<TestPlayer>().x == tiles[i].GetComponent<Tile>().x &&
                player.GetComponent<TestPlayer>().y == tiles[i].GetComponent<Tile>().y)
            {
                player.GetComponent<TestPlayer>().currentTile = tiles[i];
            }
        }

    }

    // Update is called once per frame
    void Update()
    {

        for (int i = 0; i < tiles.Count; i++)
        {
            tiles[i].GetComponentInChildren<TextMeshPro>().text =
                "g:" + tiles[i].GetComponent<Tile>().gCost + ", " + "\n" +
                "h:" + tiles[i].GetComponent<Tile>().hCost + ", " + "\n" +
                "f:" + (tiles[i].GetComponent<Tile>().gCost + tiles[i].GetComponent<Tile>().hCost);
        }


        if (selectedTile != null)
        {
            workOutPath(player, selectedTile);

            isMoving = true;

            StartCoroutine(LerpPosition(player.GetComponent<TestPlayer>().path, 0.3f));

            selectedTile = null;
        }

        if (isMoving == false)
        {
            player.GetComponent<TestPlayer>().path.Clear();
            startTile = null;
            OpenNodes.Clear();
            ClosedNodes.Clear();
            for (int i = 0; i < tiles.Count; i++)
            {
                tiles[i].GetComponent<Tile>().gCost = 0;
                tiles[i].GetComponent<Tile>().hCost = 0;
            }
        }
    }


    void workOutPath(GameObject playerToPath, GameObject destinationTile)
    {
        startTile = playerToPath.GetComponent<TestPlayer>().currentTile;

        OpenNodes.Add(startTile);

        while (OpenNodes.Count > 0)
        {
            GameObject currentNode = OpenNodes[0];


            for (int i = 0; i < OpenNodes.Count; i++)
            {
                if ((OpenNodes[i].GetComponent<Tile>().fCost == currentNode.GetComponent<Tile>().fCost &&
                    OpenNodes[i].GetComponent<Tile>().hCost < currentNode.GetComponent<Tile>().hCost) ||
                    OpenNodes[i].GetComponent<Tile>().fCost < currentNode.GetComponent<Tile>().fCost)
                {
                    currentNode = OpenNodes[i];
                }
                else if (OpenNodes[i].GetComponent<Tile>().fCost == currentNode.GetComponent<Tile>().fCost &&
                         OpenNodes[i].GetComponent<Tile>().hCost == currentNode.GetComponent<Tile>().hCost)
                {
                    float dist = Vector3.Distance(OpenNodes[i].transform.position, destinationTile.transform.position);
                    float currDist = Vector3.Distance(currentNode.transform.position, destinationTile.transform.position);

                    if (dist < currDist)
                    {
                        currentNode = OpenNodes[i];
                    }
                }
            }

            OpenNodes.Remove(currentNode);
            ClosedNodes.Add(currentNode);

            if (currentNode == destinationTile)
            {
                retracePath(startTile, destinationTile);
                return;
            }

            foreach (GameObject neighbour in currentNode.GetComponent<Tile>().Neighbours)
            {
                if (ClosedNodes.Contains(neighbour))
                {
                    continue;
                }

                float newMovementCostToNeighbour = currentNode.GetComponent<Tile>().gCost + GetDistance(currentNode, neighbour);

                if (newMovementCostToNeighbour < neighbour.GetComponent<Tile>().gCost || !OpenNodes.Contains(neighbour))
                {
                    neighbour.GetComponent<Tile>().gCost = newMovementCostToNeighbour;
                    neighbour.GetComponent<Tile>().hCost = GetDistance(neighbour, destinationTile);
                    neighbour.GetComponent<Tile>().parent = currentNode;

                    if (!OpenNodes.Contains(neighbour))
                    {
                        OpenNodes.Add(neighbour);
                    }
                }
            }
        }
    }

    public void retracePath(GameObject startNode, GameObject endNode)
    {
        GameObject currentNode = endNode;

        while (currentNode != startNode)
        {
            player.GetComponent<TestPlayer>().path.Add(currentNode);
            currentNode = currentNode.GetComponent<Tile>().parent;
        }

        player.GetComponent<TestPlayer>().path.Reverse();
    }

    float GetDistance(GameObject nodeA, GameObject nodeB)
    {
        float dstX = Mathf.Abs(nodeA.GetComponent<Tile>().x - nodeB.GetComponent<Tile>().x);
        float dstY = Mathf.Abs(nodeA.GetComponent<Tile>().y - nodeB.GetComponent<Tile>().y);

        return (dstX + dstY) * 10;

        /*
        if (dstX > dstY)
        {
            return 10 * (dstX - dstY);
        }
        else
        {
            return 10 * (dstY - dstX);
        }
        */
    }

    IEnumerator LerpPosition(List<GameObject> pathNodes, float duration)
    {
        while (isMoving == false)
        {
            yield return null;
        }
        while (isMoving == true)
        {
            foreach (GameObject node in pathNodes)
            {
                target = node.transform.position;

                float time = 0;
                Vector2 startPosition = player.transform.position;

                while (time < duration)
                {
                    player.transform.position = Vector2.Lerp(startPosition, target, time / duration);
                    time += Time.deltaTime;
                    yield return null;
                }

                player.transform.position = target;
                player.GetComponent<TestPlayer>().currentTile = node;

            }

            isMoving = false;
        }
    }
}
