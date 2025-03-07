using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGridTile
{
  Vector2Int Coordinate { get; }  
  ColorType Color { get; set; }   
  bool IsNode { get; }            
  List<IGridTile> Neighbors { get; set; } 
  void EnableCollider();
  void DisableCollider();
  void Highlight();
  void UnHighlight();
  
  
}
