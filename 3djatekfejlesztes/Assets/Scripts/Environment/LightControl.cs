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

    // ezt azért csináltam mert ha a lights-ot átirom List-re akkor az összes eddigi referenciát amit az eddig elkészített pályákon
    // csináltam újra be kéne húzni editorból és nincs kedvem hozzá
    private List<Light> lightListTemp = new List<Light>();

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer != playerLayer) { return; }

        if (lights != null) 
            foreach(Light _l in lights)
            {
                _l.enabled = true;


                // ez borzasztó csúnya, viszont gyors megoldás, ebben a projektben úgy döntöttem belefér
                // string keresés
                // nem helyettesíthetõ más fényforrásokra a kód, mivel specifikus hierarchiára mûködik csak
                // mindig ugyanazoknak a lámpáknak változtatjuk a materialját, mégsem cacheljük el a Renderer komponenst

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
