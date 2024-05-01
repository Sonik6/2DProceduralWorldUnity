using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class WaveFunction : MonoBehaviour
{
    public int dimensions;
    public Tile[] tileObjects;
    public List<Cell> gridComponents;
    public Cell cellObj;
    public static event Action OnGridFilled;


    int iterations = 0;

    void Awake()
    {
        gridComponents = new List<Cell>();
        AssignNeighbours();
        InitializeGrid();
    }

    void InitializeGrid()
    {
        
        for (int y = 0; y < dimensions; y++)
        {
            for (int x = 0; x < dimensions; x++)
            {
                Cell newCell = Instantiate(cellObj, new Vector2(x, y), Quaternion.identity);
                newCell.CreateCell(false, tileObjects);
                gridComponents.Add(newCell);
            }
        }
        
        StartCoroutine(CheckEntropy());
    }

    void AssignNeighbours()
    {
        foreach (Tile tile in tileObjects)
        {
            List<Tile> upNeighbours = new List<Tile>();
            List<Tile> downNeighbours = new List<Tile>();
            List<Tile> leftNeighbours = new List<Tile>();
            List<Tile> rightNeighbours = new List<Tile>();

            foreach (Tile possibleNeighbour in tileObjects)
            {
                if (possibleNeighbour.bottomLeftVertexType == tile.topLeftVertexType && possibleNeighbour.bottomRightVertexType == tile.topRightVertexType)
                {
                    upNeighbours.Add(possibleNeighbour);
                }

                if (possibleNeighbour.topLeftVertexType == tile.bottomLeftVertexType && possibleNeighbour.topRightVertexType == tile.bottomRightVertexType)
                {
                    downNeighbours.Add(possibleNeighbour);
                }

                if (possibleNeighbour.topRightVertexType == tile.topLeftVertexType && possibleNeighbour.bottomRightVertexType == tile.bottomLeftVertexType)
                {
                    leftNeighbours.Add(possibleNeighbour);
                }

                if (possibleNeighbour.topLeftVertexType == tile.topRightVertexType && possibleNeighbour.bottomLeftVertexType == tile.bottomRightVertexType)
                {
                    rightNeighbours.Add(possibleNeighbour);
                }
            }

            // Assign the neighbours
            tile.upNeighbours = upNeighbours.ToArray();
            tile.downNeighbours = downNeighbours.ToArray();
            tile.leftNeighbours = leftNeighbours.ToArray();
            tile.rightNeighbours = rightNeighbours.ToArray();
        }
    }


    IEnumerator CheckEntropy()
    {
        List<Cell> tempGrid = new List<Cell>(gridComponents);

        tempGrid.RemoveAll(c => c.collapsed);

        if (tempGrid.Count == 0)
        {
            yield break; // Exit the method if there are no elements in tempGrid
        }

        tempGrid.Sort((a, b) => { return a.tileOptions.Length - b.tileOptions.Length; });

        int arrLength = tempGrid[0].tileOptions.Length;
        int stopIndex = default;

        for (int i = 1; i < tempGrid.Count; i++)
        {
            if (tempGrid[i].tileOptions.Length > arrLength)
            {
                stopIndex = i;
                break;
            }
        }

        if (stopIndex > 0 && stopIndex < tempGrid.Count)
        {
            tempGrid.RemoveRange(stopIndex, tempGrid.Count - stopIndex);
        }

        yield return new WaitForSeconds(0.01f);

        CollapseCell(tempGrid);
    }
    
    void CollapseCell(List<Cell> tempGrid)
    {
        if (tempGrid.Count == 0)
        {
            //Debug.Log("tempGrid is empty");
            return;
        }
    
        int randIndex = UnityEngine.Random.Range(0, tempGrid.Count);
    
        Cell cellToCollapse = tempGrid[randIndex];
    
        cellToCollapse.collapsed = true;
    
        // Create a list where each tile appears a number of times equal to its weight
        List<Tile> weightedTileOptions = new List<Tile>();
        foreach (Tile tile in cellToCollapse.tileOptions)
        {
            for (int i = 0; i < tile.spawnRate; i++)
            {
                weightedTileOptions.Add(tile);
            }
        }
    
        if (weightedTileOptions.Count == 0)
        {
            //Debug.Log("weightedTileOptions is empty");
            return;
        }
    
        // Select a random tile from the weighted list
        Tile selectedTile = weightedTileOptions[UnityEngine.Random.Range(0, weightedTileOptions.Count)];
        cellToCollapse.tileOptions = new Tile[] { selectedTile };
    
        Tile foundTile = cellToCollapse.tileOptions[0];
        Instantiate(foundTile, cellToCollapse.transform.position, Quaternion.identity);
    
        UpdateGeneration();
    }

    void UpdateGeneration()
    {
        List<Cell> newGenerationCell = new List<Cell>(gridComponents);

        for (int y = 0; y < dimensions; y++)
        {
            for (int x = 0; x < dimensions; x++)
            {
                var index = x + y * dimensions;
                if (gridComponents[index].collapsed)
                {
                    //Debug.Log("called");
                    newGenerationCell[index] = gridComponents[index];
                }
                else
                {
                    List<Tile> options = new List<Tile>();
                    foreach (Tile t in tileObjects)
                    {
                        options.Add(t);
                    }

                    //update above
                    if (y > 0)
                    {
                        Cell up = gridComponents[x + (y - 1) * dimensions];
                        List<Tile> validOptions = new List<Tile>();

                        foreach (Tile possibleOptions in up.tileOptions)
                        {
                            var valOption = Array.FindIndex(tileObjects, obj => obj == possibleOptions);
                            var valid = tileObjects[valOption].upNeighbours;

                            validOptions = validOptions.Concat(valid).ToList();
                        }

                        CheckValidity(options, validOptions);
                    }

                    //update right
                    if (x < dimensions - 1)
                    {
                        Cell right = gridComponents[x + 1 + y * dimensions];
                        List<Tile> validOptions = new List<Tile>();

                        foreach (Tile possibleOptions in right.tileOptions)
                        {
                            var valOption = Array.FindIndex(tileObjects, obj => obj == possibleOptions);
                            var valid = tileObjects[valOption].leftNeighbours;

                            validOptions = validOptions.Concat(valid).ToList();
                        }

                        CheckValidity(options, validOptions);
                    }

                    //look down
                    if (y < dimensions - 1)
                    {
                        Cell down = gridComponents[x + (y + 1) * dimensions];
                        List<Tile> validOptions = new List<Tile>();

                        foreach (Tile possibleOptions in down.tileOptions)
                        {
                            var valOption = Array.FindIndex(tileObjects, obj => obj == possibleOptions);
                            var valid = tileObjects[valOption].downNeighbours;

                            validOptions = validOptions.Concat(valid).ToList();
                        }

                        CheckValidity(options, validOptions);
                    }

                    //look left
                    if (x > 0)
                    {
                        Cell left = gridComponents[x - 1 + y * dimensions];
                        List<Tile> validOptions = new List<Tile>();

                        foreach (Tile possibleOptions in left.tileOptions)
                        {
                            var valOption = Array.FindIndex(tileObjects, obj => obj == possibleOptions);
                            var valid = tileObjects[valOption].rightNeighbours;

                            validOptions = validOptions.Concat(valid).ToList();
                        }

                        CheckValidity(options, validOptions);
                    }

                    Tile[] newTileList = new Tile[options.Count];

                    for (int i = 0; i < options.Count; i++)
                    {
                        newTileList[i] = options[i];
                    }

                    newGenerationCell[index].RecreateCell(newTileList);
                }
            }
        }

        gridComponents = newGenerationCell;
        iterations++;

        if(iterations < dimensions * dimensions)
        {
            StartCoroutine(CheckEntropy());
        }
        else
        {
            Debug.Log("OnGridFilled event is about to be invoked");

            OnGridFilled?.Invoke();
        }

    }

    void CheckValidity(List<Tile> optionList, List<Tile> validOption)
    {
        for (int x = optionList.Count - 1; x >= 0; x--)
        {
            var element = optionList[x];
            if (!validOption.Contains(element))
            {
                optionList.RemoveAt(x);
            }
        }
    }
}