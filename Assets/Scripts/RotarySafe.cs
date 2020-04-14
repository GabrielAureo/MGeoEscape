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
        while(true){
            var oldPosition = m_wheelHandle.transform.position;
            var ray = ARTouchController.touchData.ray;
            float rayDist;
            if(!m_gizmoPlane.Raycast(ray, out rayDist)) yield return null;
            
            var newPosition = ray.GetPoint(rayDist);
            Debug.DrawLine(oldPosition,newPosition,Color.green,0);

            var direction = oldPosition - newPosition;
    

            float angle = ((Mathf.Atan2(direction.y, direction.z) * Mathf.Rad2Deg + 90));
            angle = ((int)angle/90) * 90f;

            Quaternion rotation = Quaternion.AngleAxis(-angle, m_wheelHandle.right);
            m_wheelHandle.transform.rotation = rotation;    

            yield return null;
        }
    }
}
