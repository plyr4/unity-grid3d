using UnityEngine;
using System.Collections.Generic;

public class Grid3D
{
    private Vector3 _center;
    private float _cellSize;
    private HashSet<Vector3Int> _occupiedCells;
    private int _gizmoSize;
    private SortedSet<(float distance, Vector3Int position)> _availablePositions;

    public Grid3D(Vector3 center, float cellSize, int gizmoSize)
    {
        _center = center;
        _cellSize = cellSize;
        _gizmoSize = gizmoSize;
        _occupiedCells = new HashSet<Vector3Int>();
        _availablePositions = new SortedSet<(float, Vector3Int)>(Comparer<(float, Vector3Int)>.Create((a, b) =>
        {
            int result = a.Item1.CompareTo(b.Item1);
            return result == 0 ? a.Item2.GetHashCode().CompareTo(b.Item2.GetHashCode()) : result;
        }));

        InitializeAvailablePositions();
    }

    private void InitializeAvailablePositions()
    {
        int distance = 0;
        while (_availablePositions.Count < 1000)
        {
            for (int x = -distance; x <= distance; x++)
            {
                for (int y = -distance; y <= distance; y++)
                {
                    for (int z = -distance; z <= distance; z++)
                    {
                        Vector3Int gridPosition = new Vector3Int(x, y, z);
                        float dist = Vector3.Distance(GridToWorld(gridPosition), _center);
                        if (!_occupiedCells.Contains(gridPosition))
                        {
                            _availablePositions.Add((dist, gridPosition));
                        }
                    }
                }
            }

            distance++;
        }
    }

    public Vector3Int WorldToGrid(Vector3 worldPosition)
    {
        Vector3 localPosition = worldPosition - _center;
        int x = Mathf.RoundToInt(localPosition.x / _cellSize);
        int y = Mathf.RoundToInt(localPosition.y / _cellSize);
        int z = Mathf.RoundToInt(localPosition.z / _cellSize);
        return new Vector3Int(x, y, z);
    }

    public Vector3 GridToWorld(Vector3Int gridPosition)
    {
        float x = gridPosition.x * _cellSize + _center.x;
        float y = gridPosition.y * _cellSize + _center.y;
        float z = gridPosition.z * _cellSize + _center.z;
        return new Vector3(x, y, z);
    }

    public Vector3 AddChild(Vector3 worldPosition)
    {
        Vector3Int gridPosition = WorldToGrid(worldPosition);
        _occupiedCells.Add(gridPosition);
        UpdateAvailablePositions(gridPosition);
        return GridToWorld(gridPosition);
    }

    public Vector3 GetNextOpenPosition()
    {
        while (_availablePositions.Count > 0)
        {
            var nextPosition = _availablePositions.Min;
            _availablePositions.Remove(nextPosition);
            if (!_occupiedCells.Contains(nextPosition.position))
            {
                _occupiedCells.Add(nextPosition.position);
                UpdateAvailablePositions(nextPosition.position);
                return GridToWorld(nextPosition.position);
            }
        }

        return Vector3.zero; // Fallback if no position found
    }

    private void UpdateAvailablePositions(Vector3Int occupiedPosition)
    {
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                for (int z = -1; z <= 1; z++)
                {
                    Vector3Int neighbor = new Vector3Int(occupiedPosition.x + x, occupiedPosition.y + y,
                        occupiedPosition.z + z);
                    if (!_occupiedCells.Contains(neighbor))
                    {
                        float dist = Vector3.Distance(GridToWorld(neighbor), _center);
                        _availablePositions.Add((dist, neighbor));
                    }
                }
            }
        }
    }

    public void ClearCells()
    {
        _occupiedCells.Clear();
        _availablePositions.Clear();
        InitializeAvailablePositions();
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(_center,
            new Vector3(_gizmoSize * _cellSize, _gizmoSize * _cellSize, _gizmoSize * _cellSize));

        for (int x = -_gizmoSize / 2; x <= _gizmoSize / 2; x++)
        {
            for (int y = -_gizmoSize / 2; y <= _gizmoSize / 2; y++)
            {
                for (int z = -_gizmoSize / 2; z <= _gizmoSize / 2; z++)
                {
                    Vector3Int gridPosition = new Vector3Int(x, y, z);
                    Vector3 worldPosition = GridToWorld(gridPosition);

                    if (_occupiedCells.Contains(gridPosition))
                    {
                        Gizmos.color = new Color(1, 0, 0, 0.5f);
                    }
                    else
                    {
                        Gizmos.color = new Color(0, 1, 0, 0.5f);
                    }

                    Gizmos.DrawSphere(worldPosition, _cellSize * 0.3f);
                }
            }
        }
    }
}