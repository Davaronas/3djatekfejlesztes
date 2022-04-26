using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityTemplateProjects;
using UnityEngine.EventSystems;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.SceneManagement;

// BUGS

// 


public class MC_Manager : MonoBehaviour
{
    private struct Directions
    {
        public int id;
        public Vector3 dir;
    }

    public enum MC_Tools {Select, MouseRaycastMove, Rotate, Scale}
    public MC_Tools selectedTool = MC_Tools.Select;

    public static Action<Transform> OnObjectSelected;
    public static Action<Transform> OnObjectMoveRelease;
    public static Action OnSwitchedFromMoveTool;

   
    private Camera mainCam = null;
    private SimpleCameraController scc = null;

    private Image selectedIcon = null;

    [SerializeField] private float rotateSpeed = 20f;
    [SerializeField] private float scaleSpeed = 20f;
    [Space]
    [SerializeField] private float cameraLerpSpeed = 20f;
    [Space]
    [SerializeField] private float ortographicCameraSize = 15f;
    [Space]
    [SerializeField] private Button selectToolButton = null;
    [SerializeField] private Button raycastMoveToolButton = null;
    [SerializeField] private Button rotateToolButton = null;
    [SerializeField] private Button scaleToolButton = null;
    [Space]
    [SerializeField] private GameObject selectedObjectPanel;
    [SerializeField] private Text selectedObjectNameText;
    [SerializeField] private InputField selectedObjectX;
    [SerializeField] private InputField selectedObjectY;
    [SerializeField] private InputField selectedObjectZ;
    [Space]
    [SerializeField] private GameObject selectedObjectAdditionInfoPanel;
    [SerializeField] private GameObject selectedObjectLampHeadRotInfos;
    [SerializeField] private InputField lampHeadRotX;
    [SerializeField] private InputField lampHeadRotY;
    [SerializeField] private InputField lampHeadRotZ;
    [Space]
    [SerializeField] private InputField lampId_InputField;
    [SerializeField] private InputField lampAttachedLightControlId_InputField;
    [Space]
    [Space]
    [Space]
    [SerializeField] private GameObject selectedObjectLightControlInfo;
    [Space]
    [SerializeField] private InputField lightControlId_InputField;
    [Space]
    [Space]
    [SerializeField] private GameObject selectedObjectInteractableInfo;
    [Space]
    [SerializeField] private InputField interactableId_InputField;
    [Space]
    [Space]
    [SerializeField] private GameObject selectedObjectBridgeInfo;
    [Space]
    [SerializeField] private InputField bridgeScaleToX;
    [SerializeField] private InputField bridgeScaleToY;
    [SerializeField] private InputField bridgeScaleToZ;
    [Space]
    [SerializeField] private InputField bridgeId_InputField;
    [SerializeField] private InputField bridgeAttachedInteractableId_InputField;
    [Space]
    [Space]
    [SerializeField] private GameObject selectedObjectDoorInfo;
    [Space]
    [SerializeField] private InputField doorId_InputField;
    [Space]
    [Space]
    [Space]
    [SerializeField] private GameObject prefabButtons_Statics;
    [SerializeField] private GameObject prefabButtons_Navigation;
    [SerializeField] private GameObject prefabButtons_Interactable;
    [SerializeField] private GameObject prefabButtons_Interacted;
    [SerializeField] private GameObject prefabButtons_Light;
    [Space]
    [SerializeField] private Button prefabButtons_Statics_showButton;
    [SerializeField] private Button prefabButtons_Navigation_showButton;
    [SerializeField] private Button prefabButtons_Interactable_showButton;
    [SerializeField] private Button prefabButtons_Interacted_showButton;
    [SerializeField] private Button prefabButtons_Light_showButton;


    private bool realView = false;
    [Space]
    [SerializeField] private GameObject postProcessing;
    [SerializeField] private Light dirLight;
    [SerializeField] private Material[] skyboxes;
    private int skyboxMatId = 0;



    private Vector3 targetPos_;
    private Vector3 screenPos_;

    private RaycastHit rh_;
    private Ray ray_;

    private Vector3[] lastSavedPos = new Vector3[3];
    private Transform lastTarget = null;


    private float horizontalMouse_ = 0f;
    private float verticalMouse_ = 0f;

    private Transform currentTarget = null;
    private bool targetUndeletable, targetUnscalable = false;
    private MC_Lamp targetLamp = null;
    private MC_LightControl targetLightControl = null;
    private MC_Bridge targetBridge = null;
    private MC_Interactable targetInteractable = null;
    private MC_Door targetDoor = null;
  //  private 
    // private Collider currentTargetCol = null;
    private Renderer currentTargetRenderer = null;

    Bounds currentTargetmeshBounds_;
    float[] moveNormalDots_;
    float maxMoveNormalDot_;
    Directions[] directions;

    private Vector3 objectPropertyVector_;

    void Start()
    {

        Cursor.lockState = CursorLockMode.None;

        directions = new Directions[6];
        directions[0].id = 0;
        directions[0].dir = Vector3.forward;
        directions[1].id = 1;
        directions[1].dir = -Vector3.forward;
        directions[2].id = 2;
        directions[2].dir = Vector3.up;
        directions[3].id = 3;
        directions[3].dir = -Vector3.up;
        directions[4].id = 4;
        directions[4].dir = Vector3.right;
        directions[5].id = 5;
        directions[5].dir = -Vector3.right;


        mainCam = Camera.main;
        scc = mainCam.GetComponent<SimpleCameraController>();
        selectedIcon = FindObjectOfType<Canvas>().transform.Find("SelectedIcon").GetComponent<Image>();

        OnObjectSelected += SetTargetWithSelectTool;


        selectToolButton.onClick.AddListener(() => SwitchToSelectTool());
        raycastMoveToolButton.onClick.AddListener(() => SwitchToRaycastMoveTool());
        rotateToolButton.onClick.AddListener(() => SwitchToRotateTool());
        scaleToolButton.onClick.AddListener(() => SwitchToScaleTool());





        SwitchToSelectTool();
        ActivatePrefabButtons_Statics();

        selectedObjectPanel.SetActive(false);
        selectedObjectAdditionInfoPanel.SetActive(false);
        selectedObjectLampHeadRotInfos.SetActive(false);
        selectedObjectLightControlInfo.SetActive(false);

     //   string asd = Application.persistentDataPath;
       // GUIUtility.systemCopyBuffer = asd;

        Application.targetFrameRate = 60;
      
    }

    private void OnDestroy()
    {
        OnObjectSelected -= SetTargetWithSelectTool;
    }

    private void FillSelectedObjectPanel_Pos(Transform _t)
    {
        if(currentTarget == null) { return; }

        if (selectedObjectX.isFocused || selectedObjectY.isFocused || selectedObjectZ.isFocused)
        {
            return;
        }

        selectedObjectNameText.text = _t.gameObject.name;
        selectedObjectX.text = Math.Round(_t.position.x,2).ToString();
        selectedObjectY.text = Math.Round(_t.position.y, 2).ToString();
        selectedObjectZ.text = Math.Round(_t.position.z, 2).ToString();

        if(!selectedObjectPanel.activeSelf)
        {
            selectedObjectPanel.SetActive(true);
        }
    }

    private void FillSelectedObjectPanel_Rot(Transform _t)
    {
        if (currentTarget == null) { return; }

        if (selectedObjectX.isFocused || selectedObjectY.isFocused || selectedObjectZ.isFocused)
        {
            return;
        }

        selectedObjectNameText.text = _t.gameObject.name;
        selectedObjectX.text = Math.Round(_t.rotation.eulerAngles.x, 2).ToString();
        selectedObjectY.text = Math.Round(_t.rotation.eulerAngles.y, 2).ToString();
        selectedObjectZ.text = Math.Round(_t.rotation.eulerAngles.z, 2).ToString();
    }

    private void FillSelectedObjectPanel_Scale(Transform _t)
    {
        if (currentTarget == null) { return; }

        if (selectedObjectX.isFocused || selectedObjectY.isFocused || selectedObjectZ.isFocused)
        {
            return;
        }

        selectedObjectNameText.text = _t.gameObject.name;
        selectedObjectX.text = Math.Round(_t.localScale.x, 2).ToString();
        selectedObjectY.text = Math.Round(_t.localScale.y, 2).ToString();
        selectedObjectZ.text = Math.Round(_t.localScale.z, 2).ToString();
    }

    public void ObjectPropertyEdited()
    {


        if(currentTarget == null) { return; }

        if(!float.TryParse(selectedObjectX.text, out float _fx)) { return; }
        if (!float.TryParse(selectedObjectY.text, out float _fy)) { return; }
        if (!float.TryParse(selectedObjectZ.text, out float _fz)) { return; }

        objectPropertyVector_ = new Vector3(_fx, _fy, _fz);

        switch (selectedTool)
        {
            case MC_Tools.Select:
                currentTarget.position = objectPropertyVector_;
                break;
            case MC_Tools.MouseRaycastMove:
                currentTarget.position = objectPropertyVector_;
                break;
            case MC_Tools.Rotate:
                currentTarget.rotation = Quaternion.Euler(objectPropertyVector_);
                break;
            case MC_Tools.Scale:
                if (targetUnscalable) { return; }
                currentTarget.localScale = objectPropertyVector_;
                break;
        }
    }

    public void LampHeadRotationEdited()
    {
        if (currentTarget == null) { return; }

        if (!float.TryParse(lampHeadRotX.text, out float _fx)) { return; }
        if (!float.TryParse(lampHeadRotY.text, out float _fy)) { return; }
        if (!float.TryParse(lampHeadRotZ.text, out float _fz)) { return; }

        objectPropertyVector_ = new Vector3(_fx, _fy, _fz);

        targetLamp.SetLampHeadRotation(objectPropertyVector_);

        lampHeadRotX.text = Math.Round(objectPropertyVector_.x, 2).ToString();
        lampHeadRotY.text = Math.Round(objectPropertyVector_.y, 2).ToString();
        lampHeadRotZ.text = Math.Round(objectPropertyVector_.z, 2).ToString();
    }

    public void BridgeScaleToEdited()
    {
        if (currentTarget == null) { return; }

        if (!float.TryParse(bridgeScaleToX.text, out float _fx)) { return; }
        if (!float.TryParse(bridgeScaleToY.text, out float _fy)) { return; }
        if (!float.TryParse(bridgeScaleToZ.text, out float _fz)) { return; }

        objectPropertyVector_ = new Vector3(_fx, _fy, _fz);

        targetBridge.SetScaleTo(objectPropertyVector_);

        bridgeScaleToX.text = Math.Round(objectPropertyVector_.x, 2).ToString();
        bridgeScaleToY.text = Math.Round(objectPropertyVector_.y, 2).ToString();
        bridgeScaleToZ.text = Math.Round(objectPropertyVector_.z, 2).ToString();
    }



    public void LampIdEdited()
    {
        if(int.TryParse(lampId_InputField.text,out int _id))
        {
            targetLamp.SetLampId(_id);
        }
        else
        {
            lampAttachedLightControlId_InputField.text = "0";
            targetLamp.SetLampId(0);
        }
        
    }

    public void LampAttachedLightControlEdited()
    {
        if(int.TryParse(lampAttachedLightControlId_InputField.text,out int _id))
        {
            targetLamp.SetLightControlId(_id);
        }
        else
        {
            lampAttachedLightControlId_InputField.text = "0";
            targetLamp.SetLightControlId(0);
        }
    }

    public void LightControlIdEdited()
    {
        if(int.TryParse(lightControlId_InputField.text,out int _id))
        {
            targetLightControl.SetLightControlId(_id);
        }
        else
        {
            lightControlId_InputField.text = "0";
            targetLightControl.SetLightControlId(0);
        }
    }


    public void InteractableIdEdited()
    {
        if (int.TryParse(interactableId_InputField.text, out int _id))
        {
            targetInteractable.SetInteractableId(_id);
        }
        else
        {
            interactableId_InputField.text = "0";
            targetInteractable.SetInteractableId(0);
        }
    }


    public void BridgeIdEdited()
    {
        if(int.TryParse(bridgeId_InputField.text,out int _id))
        {
            targetBridge.SetBridgeId(_id);
        }
        else
        {
            bridgeId_InputField.text = "0";
            targetBridge.SetBridgeId(0);
        }
    }

    public void BridgeAttachedInteractableIdEdited()
    {
        if (int.TryParse(bridgeAttachedInteractableId_InputField.text, out int _id))
        {
            targetBridge.SetAttachedInteractableId(_id);
        }
        else
        {
            bridgeAttachedInteractableId_InputField.text = "0";
            targetBridge.SetAttachedInteractableId(0);
        }
    }

    public void DoorIdEdited()
    {
        if(int.TryParse(doorId_InputField.text,out int _id))
        {
            targetDoor.SetAttachedInteractableId(_id);
        }
        else
        {
            doorId_InputField.text = "0";
            targetDoor.SetAttachedInteractableId(0);
        }
    }




    private void SetTargetWithSelectTool(Transform _t)
    {
        if(selectedTool != MC_Tools.Select) { return; }

        currentTarget = _t;
        lastTarget = _t;

        if(currentTarget.TryGetComponent<MC_PlayerPos>(out MC_PlayerPos _pp) ||
           currentTarget.TryGetComponent<MC_MapEnd>(out MC_MapEnd _me))
        {
            targetUndeletable = true;
            targetUnscalable = true;
        }
        else
        {
            targetUndeletable = false;
            targetUnscalable = false;
        }

        if (currentTarget.TryGetComponent<MC_Lamp>(out targetLamp))
        {
            targetUnscalable = true;
            selectedObjectAdditionInfoPanel.SetActive(true);

            objectPropertyVector_ = targetLamp.GetLampHeadRotation();



            lampHeadRotX.text = Math.Round(objectPropertyVector_.x, 2).ToString();
            lampHeadRotY.text = Math.Round(objectPropertyVector_.y, 2).ToString();
            lampHeadRotZ.text = Math.Round(objectPropertyVector_.z, 2).ToString();

            lampId_InputField.text = targetLamp.GetLampId().ToString();
            lampAttachedLightControlId_InputField.text = targetLamp.GetLightControlId().ToString();

            selectedObjectLampHeadRotInfos.SetActive(true);
        }
        else
        {
            targetLamp = null;
            targetUnscalable = false;
            selectedObjectAdditionInfoPanel.SetActive(false);
            selectedObjectLampHeadRotInfos.SetActive(false);
        }

        if (currentTarget.TryGetComponent<MC_LightControl>(out targetLightControl))
        {
            lightControlId_InputField.text = targetLightControl.GetLightControlId().ToString();

            selectedObjectAdditionInfoPanel.SetActive(true);
            selectedObjectLightControlInfo.SetActive(true);
        }
        else
        {
            targetLightControl = null;
            //selectedObjectAdditionInfoPanel.SetActive(false);
            selectedObjectLightControlInfo.SetActive(false);
        }

        if(currentTarget.TryGetComponent<MC_Interactable>(out targetInteractable))
        {
            interactableId_InputField.text = targetInteractable.GetInteractableId().ToString();

            selectedObjectAdditionInfoPanel.SetActive(true);
            selectedObjectInteractableInfo.SetActive(true);
        }
        else
        {
            targetInteractable = null;
            selectedObjectInteractableInfo.SetActive(false);
        }

        if (currentTarget.TryGetComponent<MC_Bridge>(out targetBridge))
        {
            bridgeId_InputField.text = targetBridge.GetBridgeId().ToString();
            bridgeAttachedInteractableId_InputField.text = targetBridge.GetattachedInteractableId().ToString();

            bridgeScaleToX.text = Math.Round(targetBridge.GetScaleTo().x,2).ToString();
            bridgeScaleToY.text = Math.Round(targetBridge.GetScaleTo().y, 2).ToString();
            bridgeScaleToZ.text = Math.Round(targetBridge.GetScaleTo().z, 2).ToString();

            selectedObjectAdditionInfoPanel.SetActive(true);
            selectedObjectBridgeInfo.SetActive(true);
        }
        else
        {
            targetBridge = null;
            selectedObjectBridgeInfo.SetActive(false);
        }

        if(currentTarget.TryGetComponent<MC_Door>(out targetDoor))
        {
            doorId_InputField.text = targetDoor.GetAttachedInteractableId().ToString();
            selectedObjectAdditionInfoPanel.SetActive(true);
            selectedObjectDoorInfo.SetActive(true);
        }
        else
        {
            targetDoor = null;
            selectedObjectDoorInfo.SetActive(false);
        }

        //currentTargetCol = currentTarget.GetComponent<Collider>();
        currentTargetRenderer = null;
        currentTargetmeshBounds_ = new Bounds(Vector3.zero, Vector3.zero);


        currentTargetRenderer = currentTarget.GetComponent<Renderer>();
        if (currentTargetRenderer != null)
        {
            currentTargetmeshBounds_ = currentTargetRenderer.bounds;
        }

        FillSelectedObjectPanel_Pos(currentTarget);

       // currentTargetmeshBounds_ = //currentTarget.GetComponent<MeshFilter>().mesh.bounds; //currentTarget.GetComponent<Collider>().bounds; //
    }

    public void SetTargetWithPrefabButton(Transform _t)
    {
        if(currentTarget != null)
        {
            currentTarget.GetComponent<Collider>().enabled = true;
        }

        currentTarget = _t;
        lastTarget = _t;

        if (currentTarget.TryGetComponent<MC_PlayerPos>(out MC_PlayerPos _pp) ||
         currentTarget.TryGetComponent<MC_MapEnd>(out MC_MapEnd _me))
        {
            targetUndeletable = true;
            targetUnscalable = true;
        }
        else
        {
            targetUndeletable = false;
            targetUnscalable = false;
        }



        if (currentTarget.TryGetComponent<MC_Lamp>(out targetLamp))
        {
            targetUnscalable = true;
            selectedObjectAdditionInfoPanel.SetActive(true);

            objectPropertyVector_ = targetLamp.GetLampHeadRotation();

            lampHeadRotX.text = Math.Round(objectPropertyVector_.x, 2).ToString();
            lampHeadRotY.text = Math.Round(objectPropertyVector_.y, 2).ToString();
            lampHeadRotZ.text = Math.Round(objectPropertyVector_.z, 2).ToString();

            lampId_InputField.text = targetLamp.GetLampId().ToString();
            lampAttachedLightControlId_InputField.text = targetLamp.GetLightControlId().ToString();

            selectedObjectLampHeadRotInfos.SetActive(true);
        }
        else
        {
            targetLamp = null;
            targetUnscalable = false;
            selectedObjectAdditionInfoPanel.SetActive(false);
            selectedObjectLampHeadRotInfos.SetActive(false);
        }

        if(currentTarget.TryGetComponent<MC_LightControl>(out targetLightControl))
        {
            lightControlId_InputField.text = targetLightControl.GetLightControlId().ToString();

            selectedObjectAdditionInfoPanel.SetActive(true);
            selectedObjectLightControlInfo.SetActive(true);
        }
        else
        {
            targetLightControl = null;
            //selectedObjectAdditionInfoPanel.SetActive(false);
            selectedObjectLightControlInfo.SetActive(false);
        }

        if (currentTarget.TryGetComponent<MC_Interactable>(out targetInteractable))
        {
            interactableId_InputField.text = targetInteractable.GetInteractableId().ToString();

            selectedObjectAdditionInfoPanel.SetActive(true);
            selectedObjectInteractableInfo.SetActive(true);
        }
        else
        {
            targetInteractable = null;
            selectedObjectInteractableInfo.SetActive(false);
        }

        if(currentTarget.TryGetComponent<MC_Bridge>(out targetBridge))
        {
            bridgeId_InputField.text = targetBridge.GetBridgeId().ToString();
            bridgeAttachedInteractableId_InputField.text = targetBridge.GetattachedInteractableId().ToString();

            bridgeScaleToX.text = Math.Round(targetBridge.GetScaleTo().x, 2).ToString();
            bridgeScaleToY.text = Math.Round(targetBridge.GetScaleTo().y, 2).ToString();
            bridgeScaleToZ.text = Math.Round(targetBridge.GetScaleTo().z, 2).ToString();

            selectedObjectAdditionInfoPanel.SetActive(true);
            selectedObjectBridgeInfo.SetActive(true);
        }
        else
        {
            targetBridge = null;
            selectedObjectBridgeInfo.SetActive(false);
        }

        if (currentTarget.TryGetComponent<MC_Door>(out targetDoor))
        {
            doorId_InputField.text = targetDoor.GetAttachedInteractableId().ToString();
            selectedObjectAdditionInfoPanel.SetActive(true);
            selectedObjectDoorInfo.SetActive(true);
        }
        else
        {
            targetDoor = null;
            selectedObjectDoorInfo.SetActive(false);
        }

        //currentTargetCol = currentTarget.GetComponent<Collider>();
        //  currentTargetmeshBounds_ = currentTargetCol.bounds;


        currentTargetRenderer = null;
        currentTargetmeshBounds_ = new Bounds(Vector3.zero, Vector3.zero);

        currentTargetRenderer = currentTarget.GetComponent<Renderer>();
        if (currentTargetRenderer != null)
        {
            currentTargetmeshBounds_ = currentTargetRenderer.bounds;
        }

         FillSelectedObjectPanel_Pos(currentTarget);

        // currentTargetmeshBounds_ = currentTarget.GetComponent<MeshFilter>().mesh.bounds;// currentTarget.GetComponent<Collider>().bounds;//
    }


    public void SwitchToSelectTool()
    {
        selectedTool = MC_Tools.Select;

        selectToolButton.interactable = false;
        raycastMoveToolButton.interactable = true;
        rotateToolButton.interactable = true;
        scaleToolButton.interactable = true;

        OnSwitchedFromMoveTool?.Invoke();

        FillSelectedObjectPanel_Pos(currentTarget);


    }

    public void SwitchToRaycastMoveTool()
    {
        selectedTool = MC_Tools.MouseRaycastMove;

        selectToolButton.interactable = true;
        raycastMoveToolButton.interactable = false;
        rotateToolButton.interactable = true;
        scaleToolButton.interactable = true;

        if(currentTarget != null)
        {
            currentTarget.GetComponent<Collider>().enabled = false;
        }

        FillSelectedObjectPanel_Pos(currentTarget);

    }

    public void SwitchToRotateTool()
    {
        selectedTool = MC_Tools.Rotate;

        selectToolButton.interactable = true;
        raycastMoveToolButton.interactable = true;
        rotateToolButton.interactable = false;
        scaleToolButton.interactable = true;

        OnSwitchedFromMoveTool?.Invoke();

        FillSelectedObjectPanel_Rot(currentTarget);


    }

    public void SwitchToScaleTool()
    {
        selectedTool = MC_Tools.Scale;

        selectToolButton.interactable = true;
        raycastMoveToolButton.interactable = true;
        rotateToolButton.interactable = true;
        scaleToolButton.interactable = false;

        OnSwitchedFromMoveTool?.Invoke();

        FillSelectedObjectPanel_Scale(currentTarget);

    }

    private void Update()
    {

        if (!(selectedObjectX.isFocused || selectedObjectY.isFocused || selectedObjectZ.isFocused) &&
            !(lampHeadRotX.isFocused || lampHeadRotY.isFocused || lampHeadRotZ.isFocused) &&
            !(lampId_InputField.isFocused || lampAttachedLightControlId_InputField.isFocused) && 
            !lightControlId_InputField.isFocused &&
            !interactableId_InputField.isFocused &&
            !(bridgeScaleToX.isFocused || bridgeScaleToY.isFocused || bridgeScaleToZ.isFocused) &&
            !bridgeId_InputField.isFocused &&
            !bridgeAttachedInteractableId_InputField.isFocused)
        {


            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                SwitchToSelectTool();
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                SwitchToRaycastMoveTool();
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                SwitchToRotateTool();
            }

            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                SwitchToScaleTool();
            }
        }

#if UNITY_EDITOR
        if(Input.GetKeyDown(KeyCode.P))
        {
            if (lastTarget != null)
            {
                lastTarget.position = lastSavedPos[0];
                lastTarget.rotation = Quaternion.Euler(lastSavedPos[1]);
                lastTarget.localScale = lastSavedPos[2];
            }
        }
#endif

#if !UNITY_EDITOR
        if(Input.GetKey(KeyCode.LeftControl))
        {
            if(Input.GetKeyDown(KeyCode.Z))
            {
                print("wtf");
                if(lastTarget != null)
                {
                    lastTarget.position = lastSavedPos[0];
                    lastTarget.rotation = Quaternion.Euler(lastSavedPos[1]);
                    lastTarget.localScale = lastSavedPos[2];
                }
            }
        }
#endif


        /*
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            ray_ = mainCam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray_, out rh_, 999f, Physics.AllLayers))
            {
                if(rh_.collider.gameObject.layer == 12) // MC_Grid layer
                {
                    currentTarget = null;
                }
            }
            else // infinite distance click
            {
                currentTarget = null;
            }
        }
        */

        if (currentTarget == null)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Delete))
        {
            if (targetUndeletable) { return; }

            Destroy(currentTarget.gameObject);
            selectedObjectPanel.SetActive(false);
            selectedObjectAdditionInfoPanel.SetActive(false);
            currentTarget = null;
        }

        if (currentTarget == null)
        {
            return;
        }

        if(EventSystem.current.IsPointerOverGameObject() == true)
        {
            return;
        }

        
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            lastSavedPos[0] = currentTarget.transform.position;
            lastSavedPos[1] = currentTarget.transform.rotation.eulerAngles;
            lastSavedPos[2] = currentTarget.transform.localScale;
        }

       
        


        if (selectedTool == MC_Tools.MouseRaycastMove)
        {
            if (currentTargetRenderer != null)
            {
                currentTargetmeshBounds_ = currentTargetRenderer.bounds;
            }


            if (Input.GetKey(KeyCode.Mouse0))
            {
                ray_ = mainCam.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray_, out rh_, 999f, Physics.AllLayers))
                {

                    //currentTarget.position = rh_.point;
                    moveNormalDots_ = new float[6];
                    moveNormalDots_[0] = Vector3.Dot(Vector3.forward, rh_.normal);
                    moveNormalDots_[1] = Vector3.Dot(-Vector3.forward, rh_.normal);
                    moveNormalDots_[2] = Vector3.Dot(Vector3.up, rh_.normal);
                    moveNormalDots_[3] = Vector3.Dot(-Vector3.up, rh_.normal);
                    moveNormalDots_[4] = Vector3.Dot(Vector3.right, rh_.normal);
                    moveNormalDots_[5] = Vector3.Dot(-Vector3.right, rh_.normal);

                    maxMoveNormalDot_ = Mathf.Max(moveNormalDots_);

                    for (int i = 0; i < moveNormalDots_.Length; i++)
                    {
                        if(moveNormalDots_[i] == maxMoveNormalDot_)
                        {

                            switch(i)
                            {
                                case 0:
                                    currentTarget.transform.position = rh_.point + new Vector3(0, 0, currentTargetmeshBounds_.size.z /* * currentTarget.transform.localScale.z*/ / 2);
                                    break;
                                case 1:
                                    currentTarget.transform.position = rh_.point - new Vector3(0, 0, currentTargetmeshBounds_.size.z /* * currentTarget.transform.localScale.z */ / 2);
                                    break;
                                case 2:
                                    currentTarget.transform.position = rh_.point + new Vector3(0, currentTargetmeshBounds_.size.y /* * currentTarget.transform.localScale.y */ / 2, 0);
                                    break;
                                case 3:
                                    currentTarget.transform.position = rh_.point - new Vector3(0, currentTargetmeshBounds_.size.y /* * currentTarget.transform.localScale.y */ / 2, 0);
                                    break;
                                case 4:
                                    currentTarget.transform.position = rh_.point + new Vector3(currentTargetmeshBounds_.size.x /* * currentTarget.transform.localScale.x */ / 2, 0, 0);
                                    break;
                                case 5:
                                    currentTarget.transform.position = rh_.point - new Vector3(currentTargetmeshBounds_.size.x /* * currentTarget.transform.localScale.x */ / 2 , 0, 0);
                                    break;

                            }

                          //  currentTarget.transform.position = rh_.normal + new Vector3(0, currentTargetmeshBounds_.size.y * currentTarget.transform.localScale.y / 2, 0);
                            break;
                        }
                    }


                        //
                }
            }

            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                OnObjectMoveRelease?.Invoke(currentTarget);
            }


            FillSelectedObjectPanel_Pos(currentTarget);
        }

        if (selectedTool == MC_Tools.Rotate)
        {
            verticalMouse_ = Input.GetAxis("Mouse X");
            horizontalMouse_ = Input.GetAxis("Mouse Y");

            if(Mathf.Abs(verticalMouse_) < 0.2 && Mathf.Abs(horizontalMouse_) < 0.2) { return; }

            print(verticalMouse_);

            if (Mathf.Abs(verticalMouse_) > Mathf.Abs(horizontalMouse_))
            {
                if (Input.GetKey(KeyCode.Mouse0))
                {
                    currentTarget.Rotate(Vector3.up * verticalMouse_ * Time.deltaTime * rotateSpeed,Space.World);
                }
            }
            else
            {
                if (Input.GetKey(KeyCode.Mouse0))
                {
                    if (Mathf.Abs(Vector3.Dot(mainCam.transform.forward, Vector3.forward)) > Mathf.Abs(Vector3.Dot(mainCam.transform.forward, Vector3.right)))
                    {
                        currentTarget.Rotate(Vector3.right * horizontalMouse_ * Time.deltaTime * rotateSpeed, Space.World);
                    }
                    else
                    {
                        currentTarget.Rotate(Vector3.forward * horizontalMouse_ * Time.deltaTime * rotateSpeed, Space.World);
                    }
                }
            }

            FillSelectedObjectPanel_Rot(currentTarget);
        }

        if (selectedTool == MC_Tools.Scale)
        {
            if (targetUnscalable) { return; }

            verticalMouse_ = Input.GetAxis("Mouse X");
            horizontalMouse_ = Input.GetAxis("Mouse Y");

            if (Mathf.Abs(verticalMouse_) < 0.2 && Mathf.Abs(horizontalMouse_) < 0.2) { return; }

            if(Input.GetKey(KeyCode.LeftShift))
            {
                verticalMouse_ *= 10;
                horizontalMouse_ *= 10;
            }

            print(verticalMouse_);

            if (Mathf.Abs(verticalMouse_) > Mathf.Abs(horizontalMouse_))
            {
                if (Input.GetKey(KeyCode.Mouse0))
                {
                    if (Mathf.Abs(Vector3.Dot(mainCam.transform.forward, Vector3.forward)) > Mathf.Abs(Vector3.Dot(mainCam.transform.forward, Vector3.right)))
                    {
                       // currentTarget.Rotate(Vector3.right * verticalMouse_ * Time.deltaTime * rotateSpeed, Space.World);
                        currentTarget.localScale += Vector3.right * verticalMouse_ * Time.deltaTime * scaleSpeed;

                    }
                    else
                    {
                       // currentTarget.Rotate(Vector3.forward * horizontalMouse_ * Time.deltaTime * rotateSpeed, Space.World);
                        currentTarget.localScale += Vector3.forward * verticalMouse_ * Time.deltaTime * scaleSpeed;
                    }
                }

               
            }
            else
            {
                if (Input.GetKey(KeyCode.Mouse0))
                {
                    //   currentTarget.Rotate(Vector3.up * verticalMouse_ * Time.deltaTime * rotateSpeed, Space.World);
                    currentTarget.localScale += Vector3.up * horizontalMouse_ * Time.deltaTime * scaleSpeed; //Vector3.Scale(currentTarget.localScale, Vector3.up * verticalMouse_ * Time.deltaTime * scaleSPeed);
                }

            }

            FillSelectedObjectPanel_Scale(currentTarget);
        }
    }

    void LateUpdate()
    {
        if (currentTarget == null)
        {
            if (selectedIcon.enabled)
            {
                selectedIcon.enabled = false;
            }
            return;
        }

        PositionSelectedIcon();
    }

    private void PositionSelectedIcon()
    {
        targetPos_ = currentTarget.position;
        screenPos_ = mainCam.WorldToScreenPoint(targetPos_);
        if (Vector3.Angle(mainCam.transform.forward, targetPos_ - mainCam.transform.position) < 90)
        {
            if (!selectedIcon.enabled)
            {
                selectedIcon.enabled = true;
            }
            selectedIcon.transform.position = screenPos_;
        }
        else
        {
            if (selectedIcon.enabled)
            {
                selectedIcon.enabled = false;
            }
        }
    }







    public void OrientateCamera(int _orientation)
    {
        scc.enabled = false;

        switch(_orientation)
        {
            case 0: // X
              //  mainCam.transform.rotation = Quaternion.Euler(new Vector3(0, -90f, 0));
                StartCoroutine(LerpCameraRot(Quaternion.Euler(new Vector3(0, -90f, 0))));
                break;
            case 1: // -X
               // mainCam.transform.rotation = Quaternion.Euler(new Vector3(0, 90f, 0));
                StartCoroutine(LerpCameraRot(Quaternion.Euler(new Vector3(0, 90f, 0))));
                break;
            case 2: // Y
             //   mainCam.transform.rotation = Quaternion.Euler(new Vector3(90f, 0f, 0));
                StartCoroutine(LerpCameraRot(Quaternion.Euler(new Vector3(90f, 0f, 0))));
                break;
            case 3: // -Y
               // mainCam.transform.rotation = Quaternion.Euler(new Vector3(-90f, 0f, 0));
                StartCoroutine(LerpCameraRot(Quaternion.Euler(new Vector3(-90f, 0f, 0))));
                break;
            case 4: // Z
             //   mainCam.transform.rotation = Quaternion.Euler(new Vector3(0f, 180f, 0));
                StartCoroutine(LerpCameraRot(Quaternion.Euler(new Vector3(0f, 180f, 0))));
                break;
            case 5: // -Z
            //    mainCam.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0));
                StartCoroutine(LerpCameraRot(Quaternion.Euler(new Vector3(0f, 0f, 0))));
                break;
        }

    }

    IEnumerator LerpCameraRot(Quaternion _l)
    {

        float _lerpProgress = 0;

        Quaternion _camRot = mainCam.transform.rotation;

        while (_lerpProgress < 1)
        {
            
            mainCam.transform.rotation = Quaternion.Lerp(_camRot, _l, _lerpProgress + (cameraLerpSpeed * Time.deltaTime));

            _lerpProgress += cameraLerpSpeed * Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }


        mainCam.transform.rotation = _l;
        scc.enabled = true;
        yield return null;
    }



    public void ChangeCameraProjection()
    {
        mainCam.orthographic = !mainCam.orthographic;
        mainCam.orthographicSize = ortographicCameraSize;
    }


    public void ActivatePrefabButtons_Statics()
    {
        prefabButtons_Statics_showButton.interactable = false;
        prefabButtons_Navigation_showButton.interactable = true;
        prefabButtons_Interactable_showButton.interactable = true;
        prefabButtons_Interacted_showButton.interactable = true;
        prefabButtons_Light_showButton.interactable = true;

        prefabButtons_Statics.SetActive(true);
        prefabButtons_Navigation.SetActive(false);
        prefabButtons_Interactable.SetActive(false);
        prefabButtons_Interacted.SetActive(false);
        prefabButtons_Light.SetActive(false);
    }

    public void ActivatePrefabButtons_Navigation()
    {
        prefabButtons_Statics_showButton.interactable = true;
        prefabButtons_Navigation_showButton.interactable = false;
        prefabButtons_Interactable_showButton.interactable = true;
        prefabButtons_Interacted_showButton.interactable = true;
        prefabButtons_Light_showButton.interactable = true;

        prefabButtons_Statics.SetActive(false);
        prefabButtons_Navigation.SetActive(true);
        prefabButtons_Interactable.SetActive(false);
        prefabButtons_Interacted.SetActive(false);
        prefabButtons_Light.SetActive(false);
    }

    public void ActivatePrefabButtons_Interactable()
    {
        prefabButtons_Statics_showButton.interactable = true;
        prefabButtons_Navigation_showButton.interactable = true;
        prefabButtons_Interactable_showButton.interactable = false;
        prefabButtons_Interacted_showButton.interactable = true;
        prefabButtons_Light_showButton.interactable = true;

        prefabButtons_Statics.SetActive(false);
        prefabButtons_Navigation.SetActive(false);
        prefabButtons_Interactable.SetActive(true);
        prefabButtons_Interacted.SetActive(false);
        prefabButtons_Light.SetActive(false);
    }

    public void ActivatePrefabButtons_Interacted()
    {
        prefabButtons_Statics_showButton.interactable = true;
        prefabButtons_Navigation_showButton.interactable = true;
        prefabButtons_Interactable_showButton.interactable = true;
        prefabButtons_Interacted_showButton.interactable = false;
        prefabButtons_Light_showButton.interactable = true;

        prefabButtons_Statics.SetActive(false);
        prefabButtons_Navigation.SetActive(false);
        prefabButtons_Interactable.SetActive(false);
        prefabButtons_Interacted.SetActive(true);
        prefabButtons_Light.SetActive(false);
    }

    public void ActivatePrefabButtons_Light()
    {
        prefabButtons_Statics_showButton.interactable = true;
        prefabButtons_Navigation_showButton.interactable = true;
        prefabButtons_Interactable_showButton.interactable = true;
        prefabButtons_Interacted_showButton.interactable = true;
        prefabButtons_Light_showButton.interactable = false;

        prefabButtons_Statics.SetActive(false);
        prefabButtons_Navigation.SetActive(false);
        prefabButtons_Interactable.SetActive(false);
        prefabButtons_Interacted.SetActive(false);
        prefabButtons_Light.SetActive(true);
    }




    public void SaveMap()
    {
        MC_MapData _mapData = new MC_MapData();
        _mapData.baseObjects = new List<MC_BasePrefabData>();
        _mapData.interactableObjects = new List<MC_InteractablePrefabData>();
        _mapData.bridgeObjects = new List<MC_BridgePrefabData>();
        _mapData.doorObjects = new List<MC_DoorPrefabData>();
        _mapData.lightObjects = new List<MC_LightPrefabData>();
        _mapData.lightControlObjects = new List<MC_LightControlPrefabData>();

        MC_PlayerPos _playerPos = FindObjectOfType<MC_PlayerPos>();
        MC_MapEnd _mapEnd = FindObjectOfType<MC_MapEnd>();

        List<MC_Prefab> _allPrefabs = new List<MC_Prefab>(FindObjectsOfType<MC_Prefab>());
        _allPrefabs.Remove(_playerPos.GetComponent<MC_Prefab>());

        List<MC_Door> _doors = new List<MC_Door>(FindObjectsOfType<MC_Door>());
        List<MC_Interactable> _interactables = new List<MC_Interactable>(FindObjectsOfType<MC_Interactable>());
        List<MC_Lamp> _lamps = new List<MC_Lamp>(FindObjectsOfType<MC_Lamp>());
        List<MC_LightControl> _lightControls = new List<MC_LightControl>(FindObjectsOfType<MC_LightControl>());
        List<MC_Bridge> _bridges = new List<MC_Bridge>(FindObjectsOfType<MC_Bridge>());

        print(_allPrefabs == null);
        print(_doors == null);
        print(_interactables == null);
        print(_lamps == null);
        print(_lightControls == null);
        print(_bridges == null);

        _mapData.playerData.prefabId = _playerPos.GetComponent<MC_Prefab>().prefabId;

        _mapData.playerData.positionX = _playerPos.transform.position.x;
        _mapData.playerData.positionY = _playerPos.transform.position.y;
        _mapData.playerData.positionZ = _playerPos.transform.position.z;

        _mapData.playerData.rotationX = _playerPos.transform.rotation.eulerAngles.x;
        _mapData.playerData.rotationY = _playerPos.transform.rotation.eulerAngles.y;
        _mapData.playerData.rotationZ = _playerPos.transform.rotation.eulerAngles.z;


        _mapData.mapEndData.prefabId = _mapEnd.GetComponent<MC_Prefab>().prefabId;

        _mapData.mapEndData.positionX = _mapEnd.transform.position.x;
        _mapData.mapEndData.positionY = _mapEnd.transform.position.y;
        _mapData.mapEndData.positionZ = _mapEnd.transform.position.z;

        _mapData.mapEndData.rotationX = _mapEnd.transform.rotation.eulerAngles.x;
        _mapData.mapEndData.rotationY = _mapEnd.transform.rotation.eulerAngles.y;
        _mapData.mapEndData.rotationZ = _mapEnd.transform.rotation.eulerAngles.z;



        for (int i = 0; i < _doors.Count; i++)
        {
            MC_DoorPrefabData _doorData = new MC_DoorPrefabData();

            MC_Prefab _p = _doors[i].GetComponent<MC_Prefab>();
            _allPrefabs.Remove(_p);

            _doorData.prefabId = _p.prefabId;
            _doorData.attachedInteractableId = _doors[i].GetAttachedInteractableId();

            _doorData.positionX = _doors[i].transform.position.x;
            _doorData.positionY = _doors[i].transform.position.y;
            _doorData.positionZ = _doors[i].transform.position.z;

            _doorData.rotationX = _doors[i].transform.rotation.eulerAngles.x;
            _doorData.rotationY = _doors[i].transform.rotation.eulerAngles.y;
            _doorData.rotationZ = _doors[i].transform.rotation.eulerAngles.z;

            _doorData.scaleX = _doors[i].transform.localScale.x;
            _doorData.scaleY = _doors[i].transform.localScale.y;
            _doorData.scaleZ = _doors[i].transform.localScale.z;

            _mapData.doorObjects.Add(_doorData);
        }

        for (int i = 0; i < _interactables.Count; i++)
        {
            MC_InteractablePrefabData _interactableData = new MC_InteractablePrefabData();

            MC_Prefab _p = _interactables[i].GetComponent<MC_Prefab>();
            _allPrefabs.Remove(_p);

            _interactableData.prefabId = _p.prefabId;
            _interactableData.interactableId= _interactables[i].GetInteractableId();

            _interactableData.positionX = _interactables[i].transform.position.x;
            _interactableData.positionY = _interactables[i].transform.position.y;
            _interactableData.positionZ = _interactables[i].transform.position.z;

            _interactableData.rotationX = _interactables[i].transform.rotation.eulerAngles.x;
            _interactableData.rotationY = _interactables[i].transform.rotation.eulerAngles.y;
            _interactableData.rotationZ = _interactables[i].transform.rotation.eulerAngles.z;

            _interactableData.scaleX = _interactables[i].transform.localScale.x;
            _interactableData.scaleY = _interactables[i].transform.localScale.y;
            _interactableData.scaleZ = _interactables[i].transform.localScale.z;

            _mapData.interactableObjects.Add(_interactableData);
        }

        for (int i = 0; i < _lamps.Count; i++)
        {
            MC_LightPrefabData _lampData = new MC_LightPrefabData();

            MC_Prefab _p = _lamps[i].GetComponent<MC_Prefab>();
            _allPrefabs.Remove(_p);


            _lampData.prefabId = _p.prefabId;
            _lampData.lightId = _lamps[i].GetLampId();
            _lampData.attachedLightControlId = _lamps[i].GetLightControlId();

            _lampData.positionX = _lamps[i].transform.position.x;
            _lampData.positionY = _lamps[i].transform.position.y;
            _lampData.positionZ = _lamps[i].transform.position.z;

            _lampData.rotationX = _lamps[i].transform.rotation.eulerAngles.x;
            _lampData.rotationY = _lamps[i].transform.rotation.eulerAngles.y;
            _lampData.rotationZ = _lamps[i].transform.rotation.eulerAngles.z;

            _lampData.scaleX = _lamps[i].transform.localScale.x;
            _lampData.scaleY = _lamps[i].transform.localScale.y;
            _lampData.scaleZ = _lamps[i].transform.localScale.z;

            _lampData.headRotationX = _lamps[i].GetLampHeadRotation().x;
            _lampData.headRotationY = _lamps[i].GetLampHeadRotation().y;
            _lampData.headRotationZ = _lamps[i].GetLampHeadRotation().z;

            _mapData.lightObjects.Add(_lampData);
        }


        for (int i = 0; i < _lightControls.Count; i++)
        {
            MC_LightControlPrefabData _lightControlData = new MC_LightControlPrefabData();

            MC_Prefab _p = _lightControls[i].GetComponent<MC_Prefab>();
            _allPrefabs.Remove(_p);

            _lightControlData.prefabId = _p.prefabId;
            _lightControlData.lightControlId = _lightControls[i].GetLightControlId();

            _lightControlData.positionX = _lightControls[i].transform.position.x;
            _lightControlData.positionY = _lightControls[i].transform.position.y;
            _lightControlData.positionZ = _lightControls[i].transform.position.z;

            _lightControlData.rotationX = _lightControls[i].transform.rotation.eulerAngles.x;
            _lightControlData.rotationY = _lightControls[i].transform.rotation.eulerAngles.y;
            _lightControlData.rotationZ = _lightControls[i].transform.rotation.eulerAngles.z;

            _lightControlData.scaleX = _lightControls[i].transform.localScale.x;
            _lightControlData.scaleY = _lightControls[i].transform.localScale.y;
            _lightControlData.scaleZ = _lightControls[i].transform.localScale.z;

            _mapData.lightControlObjects.Add(_lightControlData);
        }

        for (int i = 0; i < _bridges.Count; i++)
        {
            MC_BridgePrefabData _bridgeData = new MC_BridgePrefabData();


            MC_Prefab _p = _bridges[i].GetComponent<MC_Prefab>();
            _allPrefabs.Remove(_p);

            _bridgeData.prefabId = _p.prefabId;
            _bridgeData.bridgeId= _bridges[i].GetBridgeId();
            _bridgeData.attachedInteractableId = _bridges[i].GetattachedInteractableId();

            _bridgeData.positionX = _bridges[i].transform.position.x;
            _bridgeData.positionY = _bridges[i].transform.position.y;
            _bridgeData.positionZ = _bridges[i].transform.position.z;

            _bridgeData.rotationX = _bridges[i].transform.rotation.eulerAngles.x;
            _bridgeData.rotationY = _bridges[i].transform.rotation.eulerAngles.y;
            _bridgeData.rotationZ = _bridges[i].transform.rotation.eulerAngles.z;

            _bridgeData.scaleX = _bridges[i].transform.localScale.x;
            _bridgeData.scaleY = _bridges[i].transform.localScale.y;
            _bridgeData.scaleZ = _bridges[i].transform.localScale.z;

            _bridgeData.scaleToX = _bridges[i].GetScaleTo().x;
            _bridgeData.scaleToY = _bridges[i].GetScaleTo().y;
            _bridgeData.scaleToZ = _bridges[i].GetScaleTo().z;

            _mapData.bridgeObjects.Add(_bridgeData);
        }

        for (int i = 0; i < _allPrefabs.Count; i++)
        {
            MC_BasePrefabData _basePrefabData = new MC_BasePrefabData();

            MC_Prefab _p = _allPrefabs[i].GetComponent<MC_Prefab>();

            _basePrefabData.prefabId = _p.prefabId;

            _basePrefabData.positionX = _allPrefabs[i].transform.position.x;
            _basePrefabData.positionY = _allPrefabs[i].transform.position.y;
            _basePrefabData.positionZ = _allPrefabs[i].transform.position.z;

            _basePrefabData.rotationX = _allPrefabs[i].transform.rotation.eulerAngles.x;
            _basePrefabData.rotationY = _allPrefabs[i].transform.rotation.eulerAngles.y;
            _basePrefabData.rotationZ = _allPrefabs[i].transform.rotation.eulerAngles.z;

            _basePrefabData.scaleX = _allPrefabs[i].transform.localScale.x;
            _basePrefabData.scaleY = _allPrefabs[i].transform.localScale.y;
            _basePrefabData.scaleZ = _allPrefabs[i].transform.localScale.z;

            _mapData.baseObjects.Add(_basePrefabData);
        }


        
        _mapData.skyboxMatId = skyboxMatId;


        string _path = Application.persistentDataPath + "/CustomMaps/";
        if(!Directory.Exists(_path))
        {
            Directory.CreateDirectory(_path);
        }
        FileInfo fileInfo2 = new FileInfo(_path + "CustomMap1.mapdata");

        if (File.Exists(_path + "CustomMap1.mapdata"))
        {
            fileInfo2.IsReadOnly = false;
        }
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(_path + "CustomMap1.mapdata", FileMode.Create);
        formatter.Serialize(stream, _mapData);
        stream.Close();
        FileInfo fileInfo = new FileInfo(_path);
        fileInfo.IsReadOnly = true;
        fileInfo2.IsReadOnly = true;
    }



    public void ChangeViewType()
    {
        realView = !realView;

        if(realView)
        {
            mainCam.clearFlags = CameraClearFlags.Skybox;
            postProcessing.SetActive(true);
            dirLight.enabled = false;
        }
        else
        {
            mainCam.clearFlags = CameraClearFlags.SolidColor;
            postProcessing.SetActive(false);
            dirLight.enabled = true;

        }
    }


    public void SetSkybox(int _s)
    {

        RenderSettings.skybox = skyboxes[_s];
        skyboxMatId = _s;
    }





    public void GoToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void GoToCustomMap()
    {
        SceneManager.LoadScene(2);
    }

}
