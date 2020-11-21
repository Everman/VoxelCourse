using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    public Material cubeMaterial = null;
    public Block[,,] chunkData;
    public GameObject chunk;

    void BuildChunk() {
        chunkData = new Block[World.chunkSize, World.chunkSize, World.chunkSize];

        //create blocks
        for (int z = 0; z < World.chunkSize; z++) {
            for (int y = 0; y < World.chunkSize; y++) {
                for (int x = 0; x < World.chunkSize; x++) {
                    Vector3 pos = new Vector3(x, y, z);
                    int worldX = (int)(x + chunk.transform.position.x);
                    int worldY = (int)(y + chunk.transform.position.y);
                    int worldZ = (int)(z + chunk.transform.position.z);

                    Block.BlockType bt = new Block.BlockType();

                    if (worldY == 0) { bt = Block.BlockType.BEDROCK; }
                    else if (Utils.FractalBrownianMotion3D(worldX, worldY, worldZ, World.airLayerSM, World.airLayerOctaves) < World.airLayerCutoff) { bt = Block.BlockType.AIR; } 
                    else if (worldY <= Utils.GenerateStoneHeight(worldX, worldZ)) {
                        if (Utils.FractalBrownianMotion3D(worldX, worldY, worldZ, World.diamond_smooth, World.diamond_octaves) < World.diamond_fBMCutoff && worldY < World.diamond_startDepth) { bt = Block.BlockType.DIAMOND; } 
                        else if (Utils.FractalBrownianMotion3D(worldX, worldY, worldZ, World.redstone_smooth, World.redstone_octaves) < World.redstone_fBMCutoff && worldY < World.redstone_startDepth) { bt = Block.BlockType.REDSTONE; } 
                        else { bt = Block.BlockType.STONE; }
                    } 
                    else if (worldY < Utils.GenerateHeight(worldX, worldZ)) { bt = Block.BlockType.DIRT; } 
                    else if (worldY == Utils.GenerateHeight(worldX, worldZ)) { bt = Block.BlockType.GRASS; } 
                    else { bt = Block.BlockType.AIR; }

                    chunkData[x, y, z] = new Block(bt, pos, chunk.gameObject, this);
                }
            }
        }
    }

    public void DrawChunk() {
        for (int z = 0; z < World.chunkSize; z++) {
            for (int y = 0; y < World.chunkSize; y++) {
                for (int x = 0; x < World.chunkSize; x++) {
                    chunkData[x, y, z].Draw();
                }
            }
        }
        CombineQuads();
        MeshCollider collider = chunk.gameObject.AddComponent(typeof(MeshCollider)) as MeshCollider;
        collider.sharedMesh = chunk.transform.GetComponent<MeshFilter>().mesh;
    }

    public Chunk(Vector3 position, Material c) {
        chunk = new GameObject(World.BuildChunkName(position));
        chunk.transform.position = position;
        cubeMaterial = c;
        BuildChunk();
    }

    private void CombineQuads() {
        //1. Combine all children meshes
        MeshFilter[] meshFilters = chunk.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        int i = 0;
        while (i < meshFilters.Length) {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            i++;
        }

        //2. Create a new mesh on the parent object
        MeshFilter meshFilter = (MeshFilter)chunk.gameObject.AddComponent(typeof(MeshFilter));
        meshFilter.mesh = new Mesh();

        //3 Add combined meshes on children as the parent's mesh
        meshFilter.mesh.CombineMeshes(combine);

        //4. Create a renderer for the parent
        MeshRenderer renderer = chunk.gameObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        renderer.material = cubeMaterial;

        //5. Delete all uncombined children
        foreach (Transform quad in chunk.transform) {
            GameObject.Destroy(quad.gameObject);
        }
    }
}
