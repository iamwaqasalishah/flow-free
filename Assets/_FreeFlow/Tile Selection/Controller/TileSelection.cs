using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSelection : MonoBehaviour
{
  
        Vector2[] directions;

        Selection selection;

        public TileSelection()
        {
            this.selection = new Selection();
            this.selection.Clear();

            directions = new Vector2[]
            {
                Vector2.up,
                Vector2.right,
                Vector2.down,
                Vector2.left 
            };
        }

        /// <summary>
        /// Returns the number of all GameTiles which are selectable from the given one
        /// </summary>
        /// <param name="gameTile"></param>
        /// <returns></returns>
        public int Count(GridTile gameTile)
        {
            selection.Clear();

            if (gameTile != null )
            {
                AddToSelection(gameTile);
            }

            return selection.Count;
        }

        public void AddToSelection(GridTile gameTile)
        {
            if (selection.Contains(gameTile))
            {
                return;
            }

            selection.Add(gameTile);

            gameTile.DisableCollider();

            foreach (var direction in directions)
            {
                TrySelect(gameTile, direction);
            }

            gameTile.EnableCollider();
        }

        public void TrySelect(GridTile gameTile, Vector2 direction)
        {
            float maxDistance = 2;

            var raycast = new GridTileRaycast(gameTile.transform.position, direction, maxDistance);

            if (raycast.Perform(out var neighbor))
            {
                if (true)
                {
                    AddToSelection(neighbor);
                }
            }
        }
}
