using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class UnitsSpawner : MonoBehaviour
{
  public WaveFunction waveFunction; 
  public GameObject logPrefab;
  public GameObject mushroomPrefab;
  public GameObject rockPrefab;
  
  // tree prefab is a combination of treeTop and treeBottom prefabs
  public GameObject treeTopPrefab;
  public GameObject treeBottomPrefab;
  
  private readonly HashSet<Vector2> _objectsSpawnedPositions = new HashSet<Vector2>();

  void Start()
  {
    WaveFunction.OnGridFilled += SpawnSprites;
  }
  
  void SpawnSprites()
  {
    for (int i = 0; i < waveFunction.gridComponents.Count; i++)
    {
      Cell cell = waveFunction.gridComponents[i];
      Tile tile = cell.tileOptions[0];

      if (tile.topLeftVertexType == TileType.Grass && tile.topRightVertexType == TileType.Grass && tile.bottomLeftVertexType == TileType.Grass && tile.bottomRightVertexType == TileType.Grass)
      {
        int randomNumber = UnityEngine.Random.Range(1, 31);

        if (randomNumber == 1 && !_objectsSpawnedPositions.Contains(cell.transform.position))
        {
          Instantiate(logPrefab, cell.transform.position, Quaternion.identity);

          _objectsSpawnedPositions.Add(cell.transform.position);
        }
      }

      if (tile.topLeftVertexType == TileType.Grass && tile.topRightVertexType == TileType.Grass && tile.bottomLeftVertexType == TileType.Grass && tile.bottomRightVertexType == TileType.Grass)
      {
        int randomNumber = UnityEngine.Random.Range(1, 16);

        if (randomNumber == 1 && !_objectsSpawnedPositions.Contains(cell.transform.position))
        {
          Instantiate(mushroomPrefab, cell.transform.position, Quaternion.identity);
          
          _objectsSpawnedPositions.Add(cell.transform.position);
        }
      }
      
      if (tile.topLeftVertexType == TileType.Dirt && tile.topRightVertexType == TileType.Dirt && tile
          .bottomLeftVertexType == TileType.Dirt && tile.bottomRightVertexType == TileType.Dirt)
      {
        int randomNumber = UnityEngine.Random.Range(1, 9);

        if (randomNumber == 1 && !_objectsSpawnedPositions.Contains(cell.transform.position))
        {
          Instantiate(rockPrefab, cell.transform.position, Quaternion.identity);
          
          _objectsSpawnedPositions.Add(cell.transform.position);
        }
      }
      
      if (tile.topLeftVertexType == TileType.Grass && tile.topRightVertexType == TileType.Grass && tile.bottomLeftVertexType == TileType.Grass && tile.bottomRightVertexType == TileType.Grass)
      {
        int randomNumber = UnityEngine.Random.Range(1, 8);

        if (randomNumber == 1 && !_objectsSpawnedPositions.Contains(cell.transform.position))
        {
          Instantiate(treeBottomPrefab, cell.transform.position, Quaternion.identity);

          _objectsSpawnedPositions.Add(cell.transform.position);

          int aboveCellIndex = i + waveFunction.dimensions;
          
          if (_objectsSpawnedPositions.Contains(cell.transform.position))
          {
            Cell aboveCell = waveFunction.gridComponents[aboveCellIndex];
            Instantiate(treeTopPrefab, aboveCell.transform.position, Quaternion.identity);
          }
        }
      }
    }
  }
}