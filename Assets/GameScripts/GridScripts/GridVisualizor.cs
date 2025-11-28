using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class GridVisualizor : MonoBehaviour
{
    public Material lineMaterial;
    public GridController gridController;
    public GameController gameController;
    public TacticsController tacticsController;
    public Camera gridCamera;
    public bool renderGrid = true;
    public void ToggleGridLines()
    {
        if (renderGrid)
        {
            renderGrid = false;
        }
        else
        {
            renderGrid = true;
        }
    }
    private void OnRenderObject()
    {

        if (!lineMaterial || gridController == null || gridController.gridMap == null || Camera.current != gridCamera || !renderGrid)
        {
            return;
        }

        lineMaterial.SetPass(0);

        GL.Begin(GL.TRIANGLES);

       
        foreach (var kvp in gridController.gridMap)
        {
            
            if(gameController.currentUnit.traversableTiles.Contains(kvp.Value))
            {
                GL.Color(new Color(0f, 0f, 1f, .3f));
            } else 
            {
                GL.Color(new Color(0f, 1f, 1f, kvp.Value.moveCost/3f));
            }
            if(tacticsController.highlightedTiles.Contains(kvp.Value))
            {
                GL.Color(new Color(1f, 1f, 0f, .3f));
            }

            if(gameController.currentlySelectedTile != null && gameController.currentlySelectedTile == kvp.Value)
            {
                GL.Color(new Color(1f, 0f, 1f, .3f));
            }


            Vector3 center = kvp.Value.tilePosition;
            float half = gridController.tileSize * 0.5f;
            float yOffset = 0.02f; // avoid z-fighting with terrain

            Vector3 bl = new Vector3(center.x - half, center.y + yOffset, center.z - half);
            Vector3 br = new Vector3(center.x + half, center.y + yOffset, center.z - half);
            Vector3 tr = new Vector3(center.x + half, center.y + yOffset, center.z + half);
            Vector3 tl = new Vector3(center.x - half, center.y + yOffset, center.z + half);

            // Two triangles
            GL.Vertex(bl); GL.Vertex(br); GL.Vertex(tr);
            GL.Vertex(bl); GL.Vertex(tr); GL.Vertex(tl);
        }

        GL.End();




        foreach (var kvp in gridController.gridMap)
        {
            GL.Begin(GL.LINES);
            GL.Color(Color.black);

            Vector3 center = kvp.Value.tilePosition;
            float half = gridController.tileSize * 0.5f;
            float y = center.y + 0.02f;

            Vector3 bl = new Vector3(center.x - half, y, center.z - half);
            Vector3 br = new Vector3(center.x + half, y, center.z - half);
            Vector3 tr = new Vector3(center.x + half, y, center.z + half);
            Vector3 tl = new Vector3(center.x - half, y, center.z + half);

            // Start with default edge connections
            Vector3 blC = bl;
            Vector3 brC = br;
            Vector3 trC = tr;
            Vector3 tlC = tl;

            Vector3 bllC = bl;
            Vector3 brrC = br;
            Vector3 trrC = tr;
            Vector3 tllC = tl;

            List<Tile> adjacentTiles = gameController.FindAdjacentTiles(kvp.Value);

            foreach (Tile neighbor in adjacentTiles)
            {
                if (Mathf.Abs(neighbor.tilePosition.y - kvp.Value.tilePosition.y) < 0.01f)
                    continue; // Skip flat neighbors

                float avgY = (neighbor.tilePosition.y + kvp.Value.tilePosition.y) * 0.5f;

                // North
                if (neighbor.tileCoords.Item3 == kvp.Value.tileCoords.Item3 + 1 &&
                    neighbor.tileCoords.Item1 == kvp.Value.tileCoords.Item1)
                {
                    trC = new Vector3(tr.x, avgY, tr.z);
                    tlC = new Vector3(tl.x, avgY, tl.z);
                }
                // South
                if (neighbor.tileCoords.Item3 == kvp.Value.tileCoords.Item3 - 1 &&
                    neighbor.tileCoords.Item1 == kvp.Value.tileCoords.Item1)
                {
                    blC = new Vector3(bl.x, avgY, bl.z);
                    brC = new Vector3(br.x, avgY, br.z);
                }
                // West
                if (neighbor.tileCoords.Item1 == kvp.Value.tileCoords.Item1 - 1 &&
                    neighbor.tileCoords.Item3 == kvp.Value.tileCoords.Item3)
                {
                    tllC = new Vector3(tl.x, avgY, tl.z);
                    bllC = new Vector3(bl.x, avgY, bl.z);
                }
                // East
                if (neighbor.tileCoords.Item1 == kvp.Value.tileCoords.Item1 + 1 &&
                    neighbor.tileCoords.Item3 == kvp.Value.tileCoords.Item3)
                {
                    trrC = new Vector3(tr.x, avgY, tr.z);
                    brrC = new Vector3(br.x, avgY, br.z);
                }
            }

            // Connect the top edge
            GL.Vertex(tl); GL.Vertex(tlC);
            GL.Vertex(tlC); GL.Vertex(trC);
            GL.Vertex(trC); GL.Vertex(tr);

            // Right edge
            GL.Vertex(tr); GL.Vertex(trrC);
            GL.Vertex(trrC); GL.Vertex(brrC);
            GL.Vertex(brrC); GL.Vertex(br);

            // Bottom edge
            GL.Vertex(br); GL.Vertex(brC);
            GL.Vertex(brC); GL.Vertex(blC);
            GL.Vertex(blC); GL.Vertex(bl);

            // Left edge
            GL.Vertex(bl); GL.Vertex(bllC);
            GL.Vertex(bllC); GL.Vertex(tllC);
            GL.Vertex(tllC); GL.Vertex(tl);

            GL.End();
        }



    }
}
