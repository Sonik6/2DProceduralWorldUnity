using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType
{
  Dirt,
  Grass,
  Water
}

public class Tile : MonoBehaviour
{
  
  [System.NonSerialized]
  public Tile[] upNeighbours;

  [System.NonSerialized]
  public Tile[] downNeighbours;

  [System.NonSerialized]
  public Tile[] leftNeighbours;

  [System.NonSerialized]
  public Tile[] rightNeighbours;

  public int spawnRate = 1;
  
  public TileType topLeftVertexType;
  public TileType topRightVertexType;
  public TileType bottomLeftVertexType;
  public TileType bottomRightVertexType;
  
}
