using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct MC_MapData 
{
    public MC_PlayerData playerData;
    public MC_MapEndData mapEndData;
    public List<MC_DoorPrefabData> doorObjects;
    public List<MC_InteractablePrefabData> interactableObjects;
    public List<MC_LightPrefabData> lightObjects;
    public List<MC_LightControlPrefabData> lightControlObjects;
    public List<MC_Bridge> bridgeObjects;
    public List<MC_BasePrefabData> baseObjects;
}

[System.Serializable]
public struct MC_PlayerData
{
    public int prefabId;
    [Space]
    public float positionX;
    public float positionY;
    public float positionZ;
    [Space]
    public float rotationX;
    public float rotationY;
    public float rotationZ;
}

[System.Serializable]
public struct MC_MapEndData
{
    public int prefabId;
    [Space]
    public float positionX;
    public float positionY;
    public float positionZ;
    [Space]
    public float rotationX;
    public float rotationY;
    public float rotationZ;
}



[System.Serializable]
public struct MC_BasePrefabData
{
    public int prefabId;
    [Space]
    public float positionX;
    public float positionY;
    public float positionZ;
    [Space]
    public float rotationX;
    public float rotationY;
    public float rotationZ;
    [Space]
    public float scaleX;
    public float scaleY;
    public float scaleZ;
}

[System.Serializable]
public struct MC_DoorPrefabData
{
    public int prefabId;
    public int attachedInteractableId;
    [Space]
    public float positionX;
    public float positionY;
    public float positionZ;
    [Space]
    public float rotationX;
    public float rotationY;
    public float rotationZ;
    [Space]
    public float scaleX;
    public float scaleY;
    public float scaleZ;
}

[System.Serializable]
public struct MC_InteractablePrefabData
{
    public int prefabId;
    public int interactableId;
    [Space]
    public float positionX;
    public float positionY;
    public float positionZ;
    [Space]
    public float rotationX;
    public float rotationY;
    public float rotationZ;
    [Space]
    public float scaleX;
    public float scaleY;
    public float scaleZ;
}

[System.Serializable]
public struct MC_LightPrefabData
{
    public int prefabId;
    public int lightId;
    public int attachedLightControlId;
    [Space]
    public float positionX;
    public float positionY;
    public float positionZ;
    [Space]
    public float rotationX;
    public float rotationY;
    public float rotationZ;
    [Space]
    public float scaleX;
    public float scaleY;
    public float scaleZ;
    [Space]
    public float headRotationX;
    public float headRotationY;
    public float headRotationZ;
}

public struct MC_LightControlPrefabData
{
    public int prefabId;
    public int lightControlId;
    [Space]
    public float positionX;
    public float positionY;
    public float positionZ;
    [Space]
    public float rotationX;
    public float rotationY;
    public float rotationZ;
    [Space]
    public float scaleX;
    public float scaleY;
    public float scaleZ;
}


[System.Serializable]
public struct MC_BridgePrefabData
{
    public int prefabId;
    public int bridgeId;
    public int attachedInteractableId;
    [Space]
    public float positionX;
    public float positionY;
    public float positionZ;
    [Space]
    public float rotationX;
    public float rotationY;
    public float rotationZ;
    [Space]
    public float scaleX;
    public float scaleY;
    public float scaleZ;
    [Space]
    public float scaleToX;
    public float scaleToY;
    public float scaleToZ;
}

