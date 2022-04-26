using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class MC_MapBuilder : MonoBehaviour
{

    [SerializeField] private GameObject[] prefabs;
    [Space]
    [SerializeField] private Material[] skyboxes;

    private bool editor = false;

    void Start()
    {
        string _path = Application.persistentDataPath + "/CustomMaps/";
        if (!Directory.Exists(_path))
        {
            Directory.CreateDirectory(_path);
        }

        if(FindObjectOfType<MC_Manager>() != null)
        {
            editor = true;
        }

        _path = _path + "CustomMap1.mapdata";
        if(!File.Exists(_path))
        {
            return;
        }

        Transform _EnvMap = GameObject.Find("EnvMap").transform;

        FileInfo fileInfo = new FileInfo(_path);
        fileInfo.IsReadOnly = false;
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(_path, FileMode.Open);
        MC_MapData  _mapData = (MC_MapData)formatter.Deserialize(stream);
        stream.Close();
        fileInfo.IsReadOnly = true;

        Transform _player;
        if (FindObjectOfType<PlayerMovement>() == null)
        {
            _player = FindObjectOfType<MC_PlayerPos>().transform;
        }
        else
        {
            _player = FindObjectOfType<PlayerMovement>().transform;
        }
        _player.position = new Vector3(_mapData.playerData.positionX, _mapData.playerData.positionY, _mapData.playerData.positionZ);
        _player.rotation = Quaternion.Euler(new Vector3(0, _mapData.playerData.rotationY, 0));

        RenderSettings.skybox = skyboxes[_mapData.skyboxMatId];




        if(_mapData.baseObjects != null)
        for (int i = 0; i < _mapData.baseObjects.Count; i++)
        {
            for (int j = 0; j < prefabs.Length; j++)
            {
                if(_mapData.baseObjects[i].prefabId == prefabs[j].GetComponent<MC_Prefab>().prefabId)
                {
                    Transform _newObject = Instantiate(prefabs[j], Vector3.zero, Quaternion.identity, _EnvMap).transform;

                    _newObject.position = new Vector3(_mapData.baseObjects[i].positionX, _mapData.baseObjects[i].positionY, _mapData.baseObjects[i].positionZ);
                    _newObject.rotation = Quaternion.Euler(new Vector3(_mapData.baseObjects[i].rotationX, _mapData.baseObjects[i].rotationY, _mapData.baseObjects[i].rotationZ));
                    _newObject.localScale = new Vector3(_mapData.baseObjects[i].scaleX, _mapData.baseObjects[i].scaleY, _mapData.baseObjects[i].scaleZ);

                    break;
                }
            }
        }

        if (_mapData.interactableObjects != null)
            for (int i = 0; i < _mapData.interactableObjects.Count; i++)
            {
                for (int j = 0; j < prefabs.Length; j++)
                {
                    if (_mapData.interactableObjects[i].prefabId == prefabs[j].GetComponent<MC_Prefab>().prefabId)
                    {
                        Transform _newObject = Instantiate(prefabs[j], Vector3.zero, Quaternion.identity, _EnvMap).transform;

                        _newObject.position = new Vector3(_mapData.interactableObjects[i].positionX, _mapData.interactableObjects[i].positionY, _mapData.interactableObjects[i].positionZ);
                        _newObject.rotation = Quaternion.Euler(new Vector3(_mapData.interactableObjects[i].rotationX, _mapData.interactableObjects[i].rotationY, _mapData.interactableObjects[i].rotationZ));
                        _newObject.localScale = new Vector3(_mapData.interactableObjects[i].scaleX, _mapData.interactableObjects[i].scaleY, _mapData.interactableObjects[i].scaleZ);

                        _newObject.GetComponent<Interactable>().interactableId = _mapData.interactableObjects[i].interactableId;

                        if(editor)
                        {
                            _newObject.GetComponent<MC_Interactable>().SetInteractableId(_mapData.interactableObjects[i].interactableId);
                        }

                        break;
                    }
                }
            }

        Interactable[] _interactables = FindObjectsOfType<Interactable>();

        if (_mapData.doorObjects != null)
            for (int i = 0; i < _mapData.doorObjects.Count; i++)
        {
            for (int j = 0; j < prefabs.Length; j++)
            {
                if (_mapData.doorObjects[i].prefabId == prefabs[j].GetComponent<MC_Prefab>().prefabId)
                {
                    Transform _newObject = Instantiate(prefabs[j], Vector3.zero, Quaternion.identity, _EnvMap).transform;

                    _newObject.position = new Vector3(_mapData.doorObjects[i].positionX, _mapData.doorObjects[i].positionY, _mapData.doorObjects[i].positionZ);
                    _newObject.rotation = Quaternion.Euler(new Vector3(_mapData.doorObjects[i].rotationX, _mapData.doorObjects[i].rotationY, _mapData.doorObjects[i].rotationZ));
                    _newObject.localScale = new Vector3(_mapData.doorObjects[i].scaleX, _mapData.doorObjects[i].scaleY, _mapData.doorObjects[i].scaleZ);

                       
                        for (int k = 0; k < _interactables.Length; k++)
                        {
                            if(_interactables[k].interactableId == _mapData.doorObjects[i].attachedInteractableId)
                            {
                                _newObject.GetComponent<GridDoor>().SetInteractable(_interactables[k]);
                                break;
                            }
                        }

                        if(editor)
                        {
                            MC_Door _d = _newObject.GetComponent<MC_Door>();
                            _d.SetAttachedInteractableId(_mapData.doorObjects[i].attachedInteractableId);
                         //   _d.Att(_mapData.doorObjects[i].id)
                        }

                    break;
                }
            }
        }

        if (_mapData.bridgeObjects != null)
            for (int i = 0; i < _mapData.bridgeObjects.Count; i++)
            {
                for (int j = 0; j < prefabs.Length; j++)
                {
                    if (_mapData.bridgeObjects[i].prefabId == prefabs[j].GetComponent<MC_Prefab>().prefabId)
                    {
                        Transform _newObject = Instantiate(prefabs[j], Vector3.zero, Quaternion.identity, _EnvMap).transform;

                        _newObject.position = new Vector3(_mapData.bridgeObjects[i].positionX, _mapData.bridgeObjects[i].positionY, _mapData.bridgeObjects[i].positionZ);
                        _newObject.rotation = Quaternion.Euler(new Vector3(_mapData.bridgeObjects[i].rotationX, _mapData.bridgeObjects[i].rotationY, _mapData.bridgeObjects[i].rotationZ));
                        _newObject.localScale = new Vector3(_mapData.bridgeObjects[i].scaleX, _mapData.bridgeObjects[i].scaleY, _mapData.bridgeObjects[i].scaleZ);

                        Bridge _bridge = _newObject.GetComponent<Bridge>();
                        _bridge.SetScaleTo(new Vector3(_mapData.bridgeObjects[i].scaleToX, _mapData.bridgeObjects[i].scaleToY, _mapData.bridgeObjects[i].scaleToZ));

                        for (int k = 0; k < _interactables.Length; k++)
                        {
                            if (_interactables[k].interactableId == _mapData.doorObjects[i].attachedInteractableId)
                            {
                                _newObject.GetComponent<Bridge>().SetInteractable(_interactables[k]);
                                break;
                            }
                        }

                        if(editor)
                        {
                            MC_Bridge _b = _newObject.GetComponent<MC_Bridge>();
                            _b.SetAttachedInteractableId(_mapData.bridgeObjects[i].attachedInteractableId);

                            _b.SetScaleTo(new Vector3(_mapData.bridgeObjects[i].scaleToX, _mapData.bridgeObjects[i].scaleToY, _mapData.bridgeObjects[i].scaleToZ));
                        }

                        break;
                    }
                }
            }



        if (_mapData.lightControlObjects != null)
            for (int i = 0; i < _mapData.lightControlObjects.Count; i++)
            {
                for (int j = 0; j < prefabs.Length; j++)
                {
                    if (_mapData.lightControlObjects[i].prefabId == prefabs[j].GetComponent<MC_Prefab>().prefabId)
                    {
                        Transform _newObject = Instantiate(prefabs[j], Vector3.zero, Quaternion.identity, _EnvMap).transform;

                        _newObject.position = new Vector3(_mapData.lightControlObjects[i].positionX, _mapData.lightControlObjects[i].positionY, _mapData.lightControlObjects[i].positionZ);
                        _newObject.rotation = Quaternion.Euler(new Vector3(_mapData.lightControlObjects[i].rotationX, _mapData.lightControlObjects[i].rotationY, _mapData.lightControlObjects[i].rotationZ));
                        _newObject.localScale = new Vector3(_mapData.lightControlObjects[i].scaleX, _mapData.lightControlObjects[i].scaleY, _mapData.lightControlObjects[i].scaleZ);

                        _newObject.GetComponent<LightControl>().lightControlId = _mapData.lightControlObjects[i].lightControlId;

                        if(editor)
                        {
                            MC_LightControl _lc = _newObject.GetComponent<MC_LightControl>();
                            _lc.SetLightControlId(_mapData.lightControlObjects[i].lightControlId);
                        }

                        break;
                    }
                }
            }

        LightControl[] _lightControls = FindObjectsOfType<LightControl>();

        if (_mapData.lightObjects != null)
            for (int i = 0; i < _mapData.lightObjects.Count; i++)
        {
            for (int j = 0; j < prefabs.Length; j++)
            {
                if (_mapData.lightObjects[i].prefabId == prefabs[j].GetComponent<MC_Prefab>().prefabId)
                {
                    Transform _newObject = Instantiate(prefabs[j], Vector3.zero, Quaternion.identity, _EnvMap).transform;

                    _newObject.position = new Vector3(_mapData.lightObjects[i].positionX, _mapData.lightObjects[i].positionY, _mapData.lightObjects[i].positionZ);
                    _newObject.rotation = Quaternion.Euler(new Vector3(_mapData.lightObjects[i].rotationX, _mapData.lightObjects[i].rotationY, _mapData.lightObjects[i].rotationZ));
                    _newObject.localScale = new Vector3(_mapData.lightObjects[i].scaleX, _mapData.lightObjects[i].scaleY, _mapData.lightObjects[i].scaleZ);

                        Transform _lampHead = _newObject.Find("LampHeadPivot");
                        _lampHead.rotation = Quaternion.Euler(new Vector3(_mapData.lightObjects[i].headRotationX, _mapData.lightObjects[i].headRotationY, _mapData.lightObjects[i].headRotationZ));

                        for (int k = 0; k < _lightControls.Length; k++)
                        {
                            if(_lightControls[k].lightControlId == _mapData.lightObjects[i].attachedLightControlId)
                            {
                                _lightControls[k].AddLight(_newObject.GetComponentInChildren<Light>());
                            }
                        }

                        if(editor)
                        {
                            MC_Lamp _l = _newObject.GetComponent<MC_Lamp>();
                            _l.SetLightControlId(_mapData.lightObjects[i].attachedLightControlId);
                        }

                    break;
                }
            }
        }

        for (int i = 0; i < _lightControls.Length; i++)
        {
            _lightControls[i].FinalizeLights();
        }

      

     

     
    }


    void Update()
    {
        
    }
}
