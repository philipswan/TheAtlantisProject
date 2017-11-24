using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class RingHabitats : MonoBehaviour {

    public int numInstances = 10;
    public int numRingHabitatsPerInstance = 10;
    public int numTubeSides = 8;
    public float tubeRadius = 0.0001f;
    public float habitatHeight = 0.00001f;
	public GameObject habitat;

	private Vector3 prevUp;
    private float torusRadius;
	private Constants.Configuration config;									// Holds reference to config file
    private List<GameObject> ringHabitatObjects = new List<GameObject>();

    // Use this for initialization
    void Start()
    {
		config = Constants.Configuration.Instance;

		torusRadius = Mathf.Cos(config.RingLatitude * Mathf.PI / 180) / 2;
        RefreshRingHabitats();
		UpdatePositions();
    }

    public void RefreshRingHabitats()
    {
        int sectionIndex;

        int numRingHabitats = numRingHabitatsPerInstance * numInstances;
        float ringHabitatSpacing = 2.0f * Mathf.PI / (float)numRingHabitats;

        int totalVertices = numRingHabitatsPerInstance * numTubeSides * 2;
        Vector3[] vertices = new Vector3[totalVertices];

        int totalPrimitives = numRingHabitatsPerInstance * numTubeSides * 2;
        int totalIndices = totalPrimitives * 3;
        int[] triangleIndices = new int[totalIndices];

        for (int instance = 0; instance < numInstances; instance++)
        {
			if ((instance >= 0 && instance <= 74) || (instance >= 263 && instance <= numInstances))
            {
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
		cylinder.name = "Habitat " + ringHabitatObjects.Count;
		cylinder.transform.localPosition = habbot;

		if (ringHabitatObjects.Count == 0)
		{
			prevUp = habtop - habbot;
		}
		else if (ringHabitatObjects.Count > 0)
		{
			cylinder.transform.LookAt(ringHabitatObjects[ringHabitatObjects.Count-1].transform.position, transform.TransformVector(habtop - habbot));
		}
		//cylinder.transform.up = this.transform.TransformVector(habtop - habbot);
		//cylinder.transform.localPosition -= cylinder.transform.up * 1e-4f;

		ringHabitatObjects.Add(cylinder);

		if (ringHabitatObjects.Count == 1120)
		{
			ringHabitatObjects[0].transform.LookAt(ringHabitatObjects[ringHabitatObjects.Count-1].transform.position, transform.TransformVector(prevUp));
		}
	}

	/// <summary>
	/// Move habitats to be adjacent to transit ring
	/// </summary>
	private void UpdatePositions()
	{
		foreach (GameObject r in ringHabitatObjects)
		{
			r.transform.localPosition -= r.transform.up * 1e-4f;
			r.transform.localPosition -= r.transform.right * 1e-5f;
		}
	}

}
