using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public Material textureAtlas;
    public static int chunkSize = 8;
    public static int columnHeight = 2;
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

    // Start is called before the first frame update
    void Start()
    {
        chunks = new Dictionary<string, Chunk>();
        this.transform.position = Vector3.zero;
        this.transform.rotation = Quaternion.identity;
        StartCoroutine(BuildChunkColumn());
    }
}
