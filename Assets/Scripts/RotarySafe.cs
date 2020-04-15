using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RotarySafe : MonoBehaviour
{
    private bool m_gizmoToggle = false;
    [Header("Components")]

    [SerializeField] Transform m_gizmoTransform = null;
    [SerializeField] Transform m_lockHandle = null;
    [SerializeField] Animation m_lockGlowAnimation = null;
    [SerializeField] Transform m_wheelHandle = null;
    private Renderer m_lockRenderer;
    private Renderer[] m_handleRenderer;
    private Color m_handleLockColor;

    private Tween m_motion;
    private Coroutine m_trackerRoutine;

    private Plane m_gizmoPlane;

    // Start is called before the first frame update
    void Awake()
    {   
        m_handleRenderer = new Renderer[2];
        m_gizmoToggle = false;
        if(m_gizmoTransform != null){
            m_gizmoTransform.localScale = Vector3.zero;
            m_gizmoTransform.gameObject.SetActive(m_gizmoToggle);
            m_handleRenderer[0] = m_gizmoTransform.GetChild(0).GetComponent<Renderer>();
            m_handleRenderer[1] = m_gizmoTransform.GetChild(1).GetComponent<Renderer>();
        }
        m_lockRenderer = m_lockHandle?.GetComponent<Renderer>();
        var color = m_lockRenderer.material.GetColor("_EmissionColor");
        m_handleLockColor = color;
        m_gizmoPlane = new Plane(m_wheelHandle.right, m_wheelHandle.position);
    }

    void Start(){
        var items = Resources.Instance.petrolCollection.items;
    }

    public void ToggleGizmo(ARTouchData data){
        m_motion.Kill();
        if(m_gizmoToggle){
            m_lockGlowAnimation["LockGlow"].speed = 1;
            
            m_motion = m_gizmoTransform.DOScale(Vector3.zero, .8f);
            m_motion.onComplete += ()=> m_gizmoTransform.gameObject.SetActive(false);
            m_gizmoToggle = false;
        }else{
            var state = m_lockGlowAnimation["LockGlow"];
            state.time = 0;
            state.speed = 0;
            
            m_gizmoTransform.localScale = Vector3.zero;
            m_gizmoTransform.gameObject.SetActive(true);
            m_motion = m_gizmoTransform.DOScale(Vector3.one, .8f);
            m_gizmoToggle = true;
        }
    }

    public void RotateLock(ARTouchData data){
        m_trackerRoutine = StartCoroutine(RotationTracker());
    }
    public void StopTracker(ARTouchData data){
        if(m_trackerRoutine != null) StopCoroutine(m_trackerRoutine);
    }
    IEnumerator RotationTracker(){
        // var startRay = ARTouchController.touchData.ray;
        // float startRayDist;
        // m_gizmoPlane.SetNormalAndPosition(m_wheelHandle.right, m_wheelHandle.position);
        // m_gizmoPlane.Raycast(startRay, out startRayDist);
        // //get current rotation of wheel as offset, so the top of the object doesn't automatically end up facing the cursor
        // float angleOffset = Vector3.SignedAngle(m_wheelHandle.parent.up, m_wheelHandle.up,m_wheelHandle.right); 

        // float startAngle = CalculateAngle(startRayDist) + angleOffset;

        while(true){
            var ray = ARTouchController.touchData.ray;
            float rayDist;
            m_gizmoPlane = new Plane(m_wheelHandle.right, m_wheelHandle.position);
            if(!m_gizmoPlane.Raycast(ray, out rayDist)) yield return null;
            float angle = CalculateAngle(rayDist);
            //angle -= startAngle;
            //angle = ((int)angle/90) * 90f;
            var orient = Quaternion.FromToRotation(Vector3.right, m_wheelHandle.right);
            Quaternion rotation = Quaternion.AngleAxis(-angle, m_wheelHandle.right);

            m_wheelHandle.transform.rotation = rotation * orient;

            yield return null;
        }
    }

    float CalculateAngle(float rayDist){
        var oldPosition = m_wheelHandle.position;
        var ray = ARTouchController.touchData.ray;
        
        var newPosition = ray.GetPoint(rayDist);
        Debug.DrawLine(oldPosition,newPosition,Color.green,0);

        var direction = oldPosition - newPosition;
        direction.Normalize();
        print(direction);
        direction = Vector3.ProjectOnPlane(direction, Vector3.right);
        //direction = Vector3.Project(direction, Vector3.right);

        print(direction);
        


        float angle = ((Mathf.Atan2(direction.y, direction.z) * Mathf.Rad2Deg) + 90f);
        return angle;
    }
}
