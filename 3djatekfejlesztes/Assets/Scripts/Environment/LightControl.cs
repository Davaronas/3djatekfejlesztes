using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightControl : MonoBehaviour
{
    [SerializeField] private Light[] lights = null;

    [SerializeField] private Material turnedOnMat = null;
    [SerializeField] private Material turnedOffMat = null;

    private const int playerLayer = 6;

    public int lightControlId;

    // ezt az�rt csin�ltam mert ha a lights-ot �tirom List-re akkor az �sszes eddigi referenci�t amit az eddig elk�sz�tett p�ly�kon
    // csin�ltam �jra be k�ne h�zni editorb�l �s nincs kedvem hozz�
    private List<Light> lightListTemp = new List<Light>();

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer != playerLayer) { return; }

        if (lights != null) 
            foreach(Light _l in lights)
            {
                _l.enabled = true;


                // ez borzaszt� cs�nya, viszont gyors megold�s, ebben a projektben �gy d�nt�ttem belef�r
                // string keres�s
                // nem helyettes�thet� m�s f�nyforr�sokra a k�d, mivel specifikus hierarchi�ra m�k�dik csak
                // mindig ugyanazoknak a l�mp�knak v�ltoztatjuk a materialj�t, m�gsem cachelj�k el a Renderer komponenst

                Material[] _newMats = new Material[2];
                _newMats[0] = turnedOffMat;
                _newMats[1] = turnedOnMat;
                _l.transform.parent.parent.GetComponent<AudioSource>().Play();
                _l.transform.parent.Find("LampHead").GetComponent<Renderer>().materials = _newMats;
            }


    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer != playerLayer) { return; }

        if (lights != null)
            foreach (Light _l in lights)
            {
                _l.enabled = false;

                Material[] _newMats = new Material[2];
                _newMats[0] = turnedOffMat;
                _newMats[1] = turnedOffMat;
                _l.transform.parent.Find("LampHead").GetComponent<Renderer>().materials = _newMats;
            }
    }

  

    public void AddLight(Light _l)
    {
        lightListTemp.Add(_l);
    }

    public void FinalizeLights()
    {
        lights = lightListTemp.ToArray();
    }

}
