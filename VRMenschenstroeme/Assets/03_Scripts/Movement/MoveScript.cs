using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveScript : MonoBehaviour {

    public List<Structs.PersonPosition> posistions;
    public CreateBlobs cb;
    private float time = 0f;
    public float moveTime;
    public ReadConfig conf;
    public MeshRenderer rend;
    public Color high_density;
    public Color low_density;
    void Start()
    {
        rend = gameObject.GetComponent<MeshRenderer>();
    }
    // Update is called once per frame
    void Update () {
        if (this.posistions != null)
        {
            if (time <= cb.maxTime - this.moveTime)
            {
                this.time += Time.deltaTime;
                this.transform.localPosition = Vector3.MoveTowards(this.transform.localPosition, posistions[(int)(this.time / this.moveTime/*conf_time*/) + 1].position * conf.Length + new Vector3(0, 1f, 0), this.moveTime/*conf_time*/ * Time.deltaTime);
                rend.material.color = Color.Lerp(low_density, high_density, posistions[(int)(this.time / this.moveTime/*conf_time*/)].density);
                if (!this.posistions[(int)(this.time / this.moveTime/*conf_time*/)].activated)
                    this.gameObject.SetActive(false);
            }
        }

    }
}
