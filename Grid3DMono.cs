using UnityEngine;
using System.Collections.Generic;

public class Grid3DMono : MonoBehaviour
{
    public Grid3D _grid;
    public bool _addChild;
    public bool _clearGrid;
    public float _cellSize;
    public int _gizmoSize;
    public bool _initializeGizmoGrid;
    private SortedSet<(float distance, Vector3Int position)> _availablePositions;

    void Start()
    {
        _grid = new Grid3D(Vector3.zero, _cellSize, _gizmoSize);
    }

    void Update()
    {
        if (_addChild)
        {
            _addChild = false;
            _grid.AddChild(_grid.GetNextOpenPosition());
        }

        if (_clearGrid)
        {
            _clearGrid = false;
            _grid.ClearCells();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (_initializeGizmoGrid)
        {
            _initializeGizmoGrid = false;
            _grid = new Grid3D(transform.position, _cellSize, _gizmoSize);
        }

        if (_grid != null)
        {
            _grid.OnDrawGizmosSelected();
        }
    }
}