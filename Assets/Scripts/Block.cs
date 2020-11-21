using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block
{
    public enum CubeSide { BOTTOM, TOP, LEFT, RIGHT, FRONT, BACK };
    public enum BlockType { GRASS, DIRT, STONE, DIAMOND, REDSTONE, BEDROCK, AIR };

    public bool isSolid;

    Chunk owner;
    BlockType blockType;
    GameObject parent;
    Vector3 position;

    Vector2[,] blockUVs = {
        /*GRASS TOP*/       {new Vector2(0.125f, 0.375f), new Vector2(0.1875f, 0.375f), new Vector2(0.125f, 0.4375f), new Vector2(0.1875f, 0.4375f) },
        /*GRASS SIDE*/      {new Vector2(0.1875f, 0.9375f), new Vector2(0.25f, 0.9375f), new Vector2(0.1875f, 1.0f), new Vector2(0.25f, 1.0f) },
        /*DIRT*/            {new Vector2(0.125f, 0.9375f), new Vector2(0.1875f, 0.9375f), new Vector2(0.125f, 1.0f), new Vector2(0.1875f, 1.0f) },
        /*STONE*/           {new Vector2(0f, 0.875f), new Vector2(0.0625f, 0.875f), new Vector2(0f, 0.9375f), new Vector2(0.0625f, 0.9375f) },
        /*DIAMOND*/         {new Vector2(0.125f, 0.75f), new Vector2(0.1875f, 0.750f), new Vector2(0.125f, 0.8125f), new Vector2(0.1825f, 0.8125f) },
        /*REDSTONE*/        {new Vector2(0.0625f, 0.4375f), new Vector2(0.125f, 0.4375f), new Vector2(0.0625f, 0.5f), new Vector2(0.125f, 0.5f) },
        /*BEDROCK*/         {new Vector2(0.5f, 0.5625f), new Vector2(0.5625f, 0.5625f), new Vector2(0.5f, 0.625f), new Vector2(0.5625f, 0.625f) }

    };

    public Block(BlockType blockType, Vector3 position, GameObject parent, Chunk owner) {
        this.blockType = blockType;
        this.position = position;
        this.parent = parent;
        this.owner = owner;

        if(this.blockType == BlockType.AIR) {
            isSolid = false;
        } else {
            isSolid = true;
        }
    }

    private void CreateQuad(CubeSide side) {
        Mesh mesh = new Mesh();
        mesh.name = "ScriptedMesh" + side.ToString();

        Vector3[] vertices = new Vector3[4];
        Vector3[] normals = new Vector3[4];
        Vector2[] uvs = new Vector2[4];
        int[] triangles = new int[6];

        //all possinble UVs
        Vector2 uv00;
        Vector2 uv10;
        Vector2 uv01;
        Vector2 uv11;

        if (blockType == BlockType.GRASS && side == CubeSide.TOP) {
            uv00 = blockUVs[0, 0];
            uv10 = blockUVs[0, 1];
            uv01 = blockUVs[0, 2];
            uv11 = blockUVs[0, 3];
        } else if (blockType == BlockType.GRASS && side == CubeSide.BOTTOM) {
            uv00 = blockUVs[(int)(BlockType.DIRT + 1), 0];
            uv10 = blockUVs[(int)(BlockType.DIRT + 1), 1];
            uv01 = blockUVs[(int)(BlockType.DIRT + 1), 2];
            uv11 = blockUVs[(int)(BlockType.DIRT + 1), 3];
        } else {
            uv00 = blockUVs[(int)(blockType + 1), 0];
            uv10 = blockUVs[(int)(blockType + 1), 1];
            uv01 = blockUVs[(int)(blockType + 1), 2];
            uv11 = blockUVs[(int)(blockType + 1), 3];
        }

        //all possible vertices
        Vector3 p0 = new Vector3(-0.5f, -0.5f, 0.5f);
        Vector3 p1 = new Vector3(0.5f, -0.5f, 0.5f);
        Vector3 p2 = new Vector3(0.5f, -0.5f, -0.5f);
        Vector3 p3 = new Vector3(-0.5f, -0.5f, -0.5f);
        Vector3 p4 = new Vector3(-0.5f, 0.5f, 0.5f);
        Vector3 p5 = new Vector3(0.5f, 0.5f, 0.5f);
        Vector3 p6 = new Vector3(0.5f, 0.5f, -0.5f);
        Vector3 p7 = new Vector3(-0.5f, 0.5f, -0.5f);

        switch (side) {
            case CubeSide.BOTTOM:
                vertices = new Vector3[] { p0, p1, p2, p3 };
                normals = new Vector3[] { Vector3.down, Vector3.down, Vector3.down, Vector3.down };
            break;
            case CubeSide.TOP:
                vertices = new Vector3[] { p7, p6, p5, p4 };
                normals = new Vector3[] { Vector3.up, Vector3.up, Vector3.up, Vector3.up };
            break;
            case CubeSide.LEFT:
                vertices = new Vector3[] { p7, p4, p0, p3 };
                normals = new Vector3[] { Vector3.left, Vector3.left, Vector3.left, Vector3.left };
            break;
            case CubeSide.RIGHT:
                vertices = new Vector3[] { p5, p6, p2, p1 };
                normals = new Vector3[] { Vector3.right, Vector3.right, Vector3.right, Vector3.right };
            break;
            case CubeSide.FRONT:
                vertices = new Vector3[] { p4, p5, p1, p0 };
                normals = new Vector3[] { Vector3.forward, Vector3.forward, Vector3.forward, Vector3.forward };
            break;
            case CubeSide.BACK:
                vertices = new Vector3[] { p6, p7, p3, p2 };
                normals = new Vector3[] { Vector3.back, Vector3.back, Vector3.back, Vector3.back };
            break;
        }

        uvs = new Vector2[] { uv11, uv01, uv00, uv10 };
        triangles = new int[] { 3, 1, 0, 3, 2, 1 };

        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.uv = uvs;
        mesh.triangles = triangles;

        mesh.RecalculateBounds();

        GameObject quad = new GameObject("Quad");
        
        quad.transform.position = position;
        quad.transform.parent = this.parent.transform;
        
        MeshFilter meshFilter = (MeshFilter)quad.AddComponent(typeof(MeshFilter));
        meshFilter.mesh = mesh;
    }

    private int ConvertBlockIndexToLocal(int i) {
        if (i == -1) { 
            i = World.chunkSize - 1; 
        } else if (i == World.chunkSize) { 
            i = 0; 
        }
        
        return i;
    }

    public bool HasSolidNeighbour(int x, int y, int z) {
        Block[,,] chunks;

        if( x < 0 || x >= World.chunkSize || 
            y < 0 || y >= World.chunkSize || 
            z < 0 || z >= World.chunkSize ) {
            
            //block in a neighbouring chunk
            Vector3 neighbourChunkPosition = this.parent.transform.position + new Vector3(  (x - (int)position.x) * World.chunkSize, (y - (int)position.y) * World.chunkSize, (z - (int)position.z) * World.chunkSize);

            string neighborName = World.BuildChunkName(neighbourChunkPosition);

            x = ConvertBlockIndexToLocal(x);
            y = ConvertBlockIndexToLocal(y);
            z = ConvertBlockIndexToLocal(z);

            Chunk neighborChunk;
            if(World.chunks.TryGetValue(neighborName, out neighborChunk)) {
                chunks = neighborChunk.chunkData;
            } else {
                return false;
            }
        } else {
            //block in this chunk
            chunks = owner.chunkData;
        }
        
        try {
            return chunks[x, y, z].isSolid;
        } catch { }
        
        return false;
    }


    public void Draw() {
        if (blockType == BlockType.AIR) { return; }

        if (!HasSolidNeighbour((int)position.x, (int)position.y, (int)position.z + 1)) { CreateQuad(CubeSide.FRONT); }
        if (!HasSolidNeighbour((int)position.x, (int)position.y, (int)position.z - 1)) { CreateQuad(CubeSide.BACK); }
        if (!HasSolidNeighbour((int)position.x, (int)position.y + 1, (int)position.z)) { CreateQuad(CubeSide.TOP); }
        if (!HasSolidNeighbour((int)position.x, (int)position.y - 1, (int)position.z)) { CreateQuad(CubeSide.BOTTOM); }
        if (!HasSolidNeighbour((int)position.x + 1, (int)position.y, (int)position.z)) { CreateQuad(CubeSide.RIGHT); }
        if (!HasSolidNeighbour((int)position.x - 1, (int)position.y, (int)position.z)) { CreateQuad(CubeSide.LEFT); }
    }
}
