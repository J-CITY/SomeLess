using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FauxGravityBody : MonoBehaviour {

	private FauxGravityAttractor attractor;
	private Rigidbody rb;

	public bool placeOnSurface = false;

	

	void Start()
	{
		attractor = FauxGravityAttractor.instance;
		rb = GetComponent<Rigidbody>();
	}

	void FixedUpdate()
	{
		if (placeOnSurface) {
			if (rb != null) {
				attractor.PlaceOnSurface(rb);
			}
		} else {
			if (rb != null) {
				attractor.Attract(rb);
			}
		}
	}
	


}
