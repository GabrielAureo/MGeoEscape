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
    [SerializeField] GameObject m_numberPrefab = null;
    [SerializeField] Animator m_safeAnimator = null;
    private Renderer m_lockRenderer;
    private Renderer[] m_handleRenderer;
    private Color m_handleLockColor;
    [Header("Adjustments")]
    [SerializeField] private float radius = 0f;

    private Tween m_motion;
    private Coroutine m_trackerRoutine;
    private int angleClamp;

    private Plane m_gizmoPlane;

    public List<float> password;
     public List<float> input;

    private List<float> numbers;

    float? valueLooked;



    void SpawnNumbers(){
        var items = new List<PetrolCollection.PetrolItem>(GameResources.Instance.petrolCollection.items.Values);
        int n = items.Count;
        numbers = new List<float>();
        angleClamp = 360/(n * 4);
        float part = (2 * Mathf.PI)/n;
        for(int i = 0; i< n; i++){
            var angle = part * i;
            var pos = radius * new Vector3(-Mathf.Cos(angle), 0, -Mathf.Sin(angle));

            var g = GameObject.Instantiate(m_numberPrefab, m_wheelHandle.GetChild(0));
            g.transform.localPosition = pos;

            var rot = (Mathf.Atan2(pos.x, pos.z) * Mathf.Rad2Deg) - 90f;
            g.transform.localRotation = Quaternion.Euler(0, rot ,0);
            g.GetComponentInChildren<TMPro.TextMeshPro>().text = items[i].value.ToString();

            numbers.Add(items[i].value);
        }
    }


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

    public void ClearInput(){
        if(input != null){
            input.Clear();
        }else{
            input = new List<float>();
        }
        
    }


    void Start(){
        SpawnNumbers();
    }

    public void ToggleGizmo(){
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

    public void RotateLock(){
        m_trackerRoutine = StartCoroutine(RotationTracker());
    }
    public void ReleaseLock(){
        if(m_trackerRoutine != null) StopCoroutine(m_trackerRoutine);
        if(valueLooked != null){
            input.Add((float)valueLooked);
        }
        if(input.Count == password.Count){
            if(EvaluateInput()){
                m_safeAnimator.SetTrigger("UnlockSuccess");
            }else{
                m_safeAnimator.SetTrigger("UnlockFail");
            }
            input.Clear();
        }
    }
    private bool EvaluateInput(){
        //if both lists have the same values in same order, return true
        if(input == password) return true; 
        var temp = new List<float>(password);
        foreach(var val in input){
            foreach(var val2 in temp){
                if(val == val2){
                    temp.Remove(val2);
                    break;
                }
            }
        }
        return (temp.Count == 0);
    }
    IEnumerator RotationTracker(){
        var startRay = ARTouchController.touchData.ray;
        float startRayDist;
        m_gizmoPlane = new Plane(m_wheelHandle.right, m_wheelHandle.position);
        m_gizmoPlane.Raycast(startRay, out startRayDist);
        //get current rotation of wheel as offset, so the top of the object doesn't automatically end up facing the cursor
        float angleOffset = -Vector3.SignedAngle(m_wheelHandle.parent.up, m_wheelHandle.up, m_wheelHandle.right);

        float startAngle = CalculateAngle(startRayDist) + angleOffset;


        while(true){
            var ray = ARTouchController.touchData.ray;
            float rayDist;
            m_gizmoPlane = new Plane(m_wheelHandle.right, m_wheelHandle.position);
            if(!m_gizmoPlane.Raycast(ray, out rayDist)) yield return null;
            float angle = CalculateAngle(rayDist);
            var temp = angle;
            angle -= startAngle;

            angle = angle - Mathf.CeilToInt(angle/360f) * 360f;
            if(angle < 0) angle += 360f;

            var clampedAngle = ((int)angle/angleClamp);
            angle = clampedAngle * angleClamp;

            valueLooked = ValueLooked(clampedAngle);


            m_wheelHandle.transform.localRotation = Quaternion.Euler(angle, 0 ,0);

            yield return null;
        }
    }
    float? ValueLooked(float angle){
        var items = GameResources.Instance.petrolCollection.items.Values;
        var n = items.Count;
        if(angle % 4 != 0) return null;
        return numbers[(int)angle/4];
    }

    float CalculateAngle(float rayDist){
        var center = m_wheelHandle.position;        
        var ray = ARTouchController.touchData.ray;
        
        var grabPosition = ray.GetPoint(rayDist);
        Debug.DrawLine(center,grabPosition,Color.green,0);

        
    
        //direction = m_wheelHandle.InverseTransformDirection(direction);
        var dir = center - grabPosition;
        Debug.DrawLine(Vector3.zero, dir * 5, Color.yellow, 0);

        var rot = Quaternion.FromToRotation(m_wheelHandle.right, Vector3.right);

        dir = rot * dir;
        Debug.DrawLine(Vector3.zero, dir * 5, Color.blue, 0);
        float angle = ((Mathf.Atan2(dir.y, dir.z) * Mathf.Rad2Deg) + 90f);  
        return -angle;
    }
}
