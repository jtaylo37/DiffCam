using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject prefab;
    public int count;

    const float half = 0.5f;

    void Start()
    {
        System.Random r = new System.Random();

        while (count-- > 0)
        {
            Vector3 pos = 
                new Vector3(
                    (float)r.NextDouble() - half,
                    (float)r.NextDouble() - half,
                    -1);

            Instantiate(prefab, pos, Quaternion.identity);
        }
    }
}
