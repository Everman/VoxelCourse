﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    //World Size
    public static int chunkSize = 16;
    public static int columnHeight = 16;                    // 16
    public static int worldSize = 2;                        // 2
    public static int maxHeight = 150;                      // 150
    //Air Layer
    public static float airLayerSM = 0.1f;                  // 0.1
    public static int airLayerOctaves = 2;                  // 3
    public static float airLayerCutoff = 0.38f;             // 0.39
    //Dirt Layer
    public static float dirt_smooth = 0.005f;               // 0.01f
    public static int dirt_octaves = 2;                     // 4
    public static float dirt_persistence = 0.3f;            // 0.5
    //Stone Layer
    public static float stone_smooth = dirt_smooth * 2;
    public static int stone_octaves = dirt_octaves + 3;
    public static float stone_persistence = 0.5f;
    //Diamond Layer
    public static float diamond_smooth = 0.01f;
    public static int diamond_octaves = 2;
    public static float diamond_persistence = 0.5f;
    public static int diamond_startDepth = 40;
    public static float diamond_fBMCutoff = 0.4f;
    //Redstone Layer
    public static float redstone_smooth = 0.03f;
    public static int redstone_octaves = 3;
    public static float redstone_persistence = 0.5f;
    public static int redstone_startDepth = 20;
    public static float redstone_fBMCutoff = 0.41f;


    public Material textureAtlas;
    public static Dictionary<string, Chunk> chunks;

    public static string BuildChunkName(Vector3 v) {
        return (int)v.x + "_" + (int)v.y + "_" + (int)v.z;
    }

    IEnumerator BuildChunkColumn() {
        for (int i = 0; i < columnHeight; i++) {
            Vector3 chunkPosition = new Vector3(this.transform.position.x, i * chunkSize, this.transform.position.z);
            Chunk chunk = new Chunk(chunkPosition, textureAtlas);
            chunk.chunk.transform.parent = this.transform;
            chunks.Add(chunk.chunk.name, chunk);
        }

        foreach (KeyValuePair<string, Chunk> chunk in chunks) {
            chunk.Value.DrawChunk();
            yield return null;
        }
    }

    IEnumerator BuildWorld() {
        for (int z = 0; z < worldSize; z++) {
            for (int x = 0; x < worldSize; x++) {
                for (int y = 0; y < columnHeight; y++) {
                    Vector3 chunkPosition = new Vector3(x * chunkSize, y * chunkSize, z * chunkSize );
                    Chunk chunk = new Chunk(chunkPosition, textureAtlas);
                    chunk.chunk.transform.parent = this.transform;
                    chunks.Add(chunk.chunk.name, chunk);
                }
            }
        }
        
        foreach(KeyValuePair<string, Chunk> chunk in chunks) {
            chunk.Value.DrawChunk();
            yield return null;
        }
        
    }

    // Start is called before the first frame update
    void Start()
    {
        chunks = new Dictionary<string, Chunk>();
        this.transform.position = Vector3.zero;
        this.transform.rotation = Quaternion.identity;
        StartCoroutine(BuildWorld());
    }
}
