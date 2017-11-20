using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class RingHabitats : MonoBehaviour {

    public int numInstances = 10;
    public int numRingHabitatsPerInstance = 10;
    public int numTubeSides = 8;
    public float tubeRadius = .0001f;
    public float habitatHeight = .00001f;
    public Material material;
	public GameObject habitat;
	public GameObject train;

    private float torusRadius;
	private Constants.Configuration config;									// Holds reference to config file
    private List<GameObject> ringHabitatObjects = new List<GameObject>();
	private List<GameObject> trams = new List<GameObject>();
	private int tramIndex;

	void Awake()
	{
		tramIndex = 0;
	}

    // Use this for initialization
    void Start()
    {
		config = Constants.Configuration.Instance;

		torusRadius = Mathf.Cos(config.RingLatitude * Mathf.PI / 180) / 2;
        RefreshRingHabitats();
    }

    GameObject createRingHabitatObject()
    {
        GameObject obj = new GameObject("ringHabitat " + ringHabitatObjects.Count);
        obj.AddComponent<MeshFilter>();
        MeshRenderer mr = obj.AddComponent<MeshRenderer>();

        obj.transform.SetParent(transform);
        obj.transform.localPosition = new Vector3();
        obj.transform.localRotation = Quaternion.identity;
        obj.transform.localScale = new Vector3(1, 1, 1);

        mr.sharedMaterial = material;

        ringHabitatObjects.Add(obj);
        return obj;
    }

    void destroyAllRingHabitatObjects()
    {
        foreach (GameObject obj in ringHabitatObjects)
        {
            Destroy(obj);
        }
        ringHabitatObjects.Clear();
    }

    public void RefreshRingHabitats()
    {
        int sectionIndex;

        destroyAllRingHabitatObjects();

        int numRingHabitats = numRingHabitatsPerInstance * numInstances;
        float ringHabitatSpacing = 2.0f * Mathf.PI / (float)numRingHabitats;

        int totalVertices = numRingHabitatsPerInstance * numTubeSides * 2;
        Vector3[] vertices = new Vector3[totalVertices];

        int totalPrimitives = numRingHabitatsPerInstance * numTubeSides * 2;
        int totalIndices = totalPrimitives * 3;
        int[] triangleIndices = new int[totalIndices];

        //for (int instance = 0; instance < numInstances; instance++)
        for (int instance = 0; instance < numInstances; instance++)
        {
            if ((instance * 4 / numInstances == 0 ) || (instance * 4 / numInstances == numInstances-1))
            {
                GameObject obj = createRingHabitatObject();
                Mesh mesh = new Mesh();

                sectionIndex = 0;
                for (int ringHabitatIndex = 0; ringHabitatIndex < numRingHabitatsPerInstance; ringHabitatIndex++)
                {
                    NewRingHabitat(
                        instance,
                        ringHabitatIndex,
                        ref sectionIndex,
                        ringHabitatSpacing,
                        vertices,
                        triangleIndices);
                }
            }
        }
    }

    public void NewRingHabitat(int instance, int ringHabitatIndex, ref int sectionIndex, float ringHabitatSpacing, Vector3[] vertices, int[] triangleIndices)
    {
        float theta;
        float phi0;
        float phi1;

        theta = (instance * numRingHabitatsPerInstance + ringHabitatIndex) * ringHabitatSpacing;
        phi0 = torusRadius;
		phi1 = torusRadius - habitatHeight * Mathf.Cos(config.RingLatitude * Mathf.Deg2Rad);
			
        // Find the current and next segments
		Vector3 habtop, habbot, hableft;

        habtop.x = phi0 * Mathf.Cos(theta) - torusRadius;
        habtop.z = phi0 * Mathf.Sin(theta);
        habtop.y = -Mathf.Pow(torusRadius - phi0, 1.7f) * 10;

        habbot.x = phi1 * Mathf.Cos(theta) - torusRadius;
        habbot.z = phi1 * Mathf.Sin(theta);
		habbot.y = -habitatHeight * Mathf.Sin(config.RingLatitude * Mathf.Deg2Rad);

		hableft.x = phi0 * Mathf.Cos(theta) - torusRadius;
		hableft.z = phi0 * Mathf.Sin(theta);
		hableft.y = -0.0075f * Mathf.Sin(config.RingLatitude * Mathf.Deg2Rad);

		GameObject cylinder = Instantiate(habitat, transform);
		cylinder.SetActive(true);
		cylinder.name = "Cylinder " + ringHabitatObjects.Count;
		cylinder.transform.localPosition = habbot;
		cylinder.transform.up = this.transform.TransformVector(habtop - habbot);

		ringHabitatObjects.Add(cylinder);

		// No longer needed because we are using prefabs for habitats
        //DrawCylinder(habtop, habbot, tubeRadius, vertices, triangleIndices, tubePrimitiveBaseOffset, tubeIndexBaseOffset);
    }

    public void DrawCylinder(Vector3 start, Vector3 end, float Radius, Vector3[] vertices, int[] triangleIndices, int tubePrimitiveBaseOffset, int tubeIndexBaseOffset)
    {
        Vector3 v1 = (end - start).normalized;
        Vector3 v2 = Vector3.Cross(v1, Vector3.up).normalized * Radius;
        Vector3 v3 = Vector3.Cross(v1, v2).normalized * Radius;
        float r;

        //Debug.DrawRay(transform.TransformPoint(start), transform.TransformDirection(v2 * 1000), Color.red, 100);
        //Debug.DrawRay(transform.TransformPoint(start), transform.TransformDirection(v3 * 1000), Color.green, 100);
        //Debug.DrawRay(transform.TransformPoint(end), transform.TransformDirection(v2 * 1000), Color.yellow, 100);
        //Debug.DrawRay(transform.TransformPoint(end), transform.TransformDirection(v3 * 1000), Color.blue, 100);

        for (int j = 0; j < numTubeSides; j++)
        {
            // Find next (or first) vertex offset
            int m = (j + 1) % numTubeSides; // changed currentTube.Count to numTubeSides

            // Find the 4 vertices that make up a quad
            int iv0 = tubePrimitiveBaseOffset + j * 2 + 0;
            int iv1 = tubePrimitiveBaseOffset + j * 2 + 1;
            int iv2 = tubePrimitiveBaseOffset + m * 2 + 0;
            int iv3 = tubePrimitiveBaseOffset + m * 2 + 1;

            r = j * 2.0f * Mathf.PI / numTubeSides;
            Vector3 v4 = Mathf.Cos(r) * v2 + Mathf.Sin(r) * v3;
            Vector3 v5 = start + v4;
            Vector3 v6 = end + v4;

            vertices[iv0] = v5;
            vertices[iv1] = v6;

            // As we itterate around the circumference of the tube, "Draw" the two triangles that make each tube face
            int ti = tubeIndexBaseOffset + j * 2 * 3;
            // Triangle 0
            triangleIndices[ti + 0] = iv0;
            triangleIndices[ti + 1] = iv2;
            triangleIndices[ti + 2] = iv1;
            // Triangle 1
            triangleIndices[ti + 3] = iv1;
            triangleIndices[ti + 4] = iv2;
            triangleIndices[ti + 5] = iv3;
        }
    }


    // Update is called once per frame
    void Update () {

    }
}
