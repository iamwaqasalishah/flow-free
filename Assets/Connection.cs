using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connection : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    public GridTile FromTile { get; private set; }
    public GridTile ToTile { get; private set; }

    public void SetConnection(GridTile fromTile, GridTile toTile)
    {
        FromTile = fromTile;
        ToTile = toTile;

        _spriteRenderer.color = ColorGroupSO.Default.GetColor(fromTile.Color);

        transform.position = (fromTile.transform.position + toTile.transform.position) / 2;
        transform.right = (toTile.transform.position - fromTile.transform.position).normalized;
        transform.localScale =
            new Vector3(Vector3.Distance(fromTile.transform.position, toTile.transform.position), 0.1f, 1);
    }
}