using UnityEngine;
using System.Collections;

public class Shield : MonoBehaviour {
    [Header("Set in Inspector")]
    public float rotationsPerSecond = 0.1f;

    [Header("Set Dynamically")]
    public int levelShown = 0;

    Material mat;

    // Use this for initialization
    void Start () {
        mat = GetComponent<Renderer>().material;
    }
	
	// Update is called once per frame
	void Update () {
		int currlevel = Mathf.FloorToInt (Hero.S.shieldLevel);
		if (levelShown != currlevel) {
			levelShown = currlevel;
			Material mat = this.GetComponent<Renderer>().material;
			mat.mainTextureOffset = new Vector2(0.2f*levelShown, 0f);
		}

		float rZ = (rotationsPerSecond * Time.time * 360) % 360f;
		transform.rotation = Quaternion.Euler (0, 0, rZ);

	}
}
