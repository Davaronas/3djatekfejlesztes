using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.EventSystems;



#if UNITY_EDITOR
using UnityEditor;
#endif

//[ExecuteInEditMode]
public class MC_PrefabButton : MonoBehaviour, IPointerDownHandler
{
    private Image buttonImage;
    private MC_Manager mc_manager;

    [SerializeField] protected GameObject heldPrefab;

    private Transform p = null;

    

    public void OnPointerDown(PointerEventData eventData)
    {
        if(heldPrefab == null) { return; }


       GameObject _newObject = Instantiate(heldPrefab, Vector3.zero, Quaternion.identity);
     //   Bounds _meshBounds = _newObject.GetComponent<MeshFilter>().mesh.bounds;
        _newObject.transform.position += new Vector3(0, 0, 0);
        _newObject.transform.SetParent(p);
        _newObject.GetComponent<MC_Prefab>().Created();


        MC_Manager.OnSwitchedFromMoveTool?.Invoke();
        _newObject.GetComponent<Collider>().enabled = false;
        mc_manager.SwitchToRaycastMoveTool();
        mc_manager.SetTargetWithPrefabButton(_newObject.transform);

        MC_Manager.OnObjectSelected?.Invoke(_newObject.transform);


    }

    void Start()
    {

        mc_manager = FindObjectOfType<MC_Manager>();
        p = GameObject.Find("EnvMap").transform;

        /*
#if UNITY_EDITOR
    
        buttonImage = GetComponent<Image>();
        Texture2D _tW = AssetPreview.GetAssetPreview(heldPrefab);
        buttonImage.sprite = Sprite.Create(_tW, new Rect(0f, 0f, _tW.width, _tW.height), new Vector2(0.5f, 0.5f));
        byte[] _bytesW = _tW.EncodeToPNG();
        string _pathW = Application.persistentDataPath + "/Images/";
        if (!Directory.Exists(_pathW))
        {
            Directory.CreateDirectory(_pathW);
        }
        File.WriteAllBytes(_pathW + heldPrefab.name + ".png", _bytesW);
    
    
#endif
        */
        

        //#if !UNITY_EDITOR
        /*
                string _pathR = Application.persistentDataPath + "/Images/" + heldPrefab.name + ".png";
                if (File.Exists(_pathR))
                {
                    byte[] _bytesR = File.ReadAllBytes(_pathR);
                    Texture2D _tR = new Texture2D(128, 128);
                    _tR.LoadImage(_bytesR);
                }
        */

        //#endif


    }

    // Update is called once per frame
    void Update()
    {
        // Texture2D _t = AssetPreview.GetMiniThumbnail(heldPrefab);
       
    }
}


