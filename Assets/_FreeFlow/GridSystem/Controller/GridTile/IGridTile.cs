using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGridTile
{
  public Vector2Int Coordinate { get; set; }
  public void EnableCollider();
  public void DisableCollider();
  public void SelectTile();
  public void DeselectTile();
  
}
