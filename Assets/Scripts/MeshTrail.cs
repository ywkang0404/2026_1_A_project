using System.Collections;
using UnityEngine;

public class MeshTrail : MonoBehaviour
{

    public float activeTime = 2.0f;
    public MovementInput moveScript;
    public float speedBoost = 6;
    public Animator animator;
    public float animSpeedBoost = 1.5f;

    [Header("mesh Releted")]
    public float meshRefreshRate = 1.0f;
    public float meshDestroyDelay = 3.0f;
    public Transform positionToSpawn;

    [Header("Shader Related")]
    public Material mat;
    public string ShaderVerRef;
    public float shaderVerRef;
    public float ShaderVarRate = 0.1f;
    public float shadervarRefreshRate = 0.05f;

    private SkinnedMeshRenderer[] skinnedRenderer;
    private bool istrailActive;

    private float normalSpeed;
    private float normalAnimSpeed;


    IEnumerator AnimatermaterialFloat(Material m, float valuGoal, float rate, float refreshRate)
    {
        float valuToAnimate = m.GetFloat(ShaderVerRef);

        while(valuToAnimate > valuGoal)
        {
            valuToAnimate -= rate;
            m.SetFloat(ShaderVerRef, valuToAnimate);
            yield return new WaitForSeconds(refreshRate);
        }
    }

    IEnumerator ActivateTrail(float timeActivated)
    {
        normalSpeed = moveScript.movementSpeed;
        moveScript.movementSpeed = speedBoost;

        normalAnimSpeed = animator.GetFloat("animSpeed");
        animator.SetFloat("animSpeed", animSpeedBoost);

        while(timeActivated > 0)
        {
            if (skinnedRenderer == null)
                skinnedRenderer = positionToSpawn.GetComponentsInChildren<SkinnedMeshRenderer>();
            for(int i = 0; i <skinnedRenderer.Length; i++)
            {
                GameObject g0bj = new GameObject();
                g0bj.transform.SetPositionAndRotation(positionToSpawn.position, positionToSpawn.rotation);
                MeshRenderer mr = g0bj.AddComponent<MeshRenderer>();
                MeshFilter mf = g0bj.AddComponent<MeshFilter>();

                Mesh m = new Mesh();
                skinnedRenderer[i].BakeMesh(m);
                mf.mesh = m;
                mr.material = mat;

                StartCoroutine(AnimatermaterialFloat(mr.material, 0, ShaderVarRate, shadervarRefreshRate));

                Destroy(g0bj, meshDestroyDelay);
            }
            yield return new WaitForSeconds(meshRefreshRate);


        }

        moveScript.movementSpeed = normalSpeed;
        animator.SetFloat("animSpeed", normalAnimSpeed);
        istrailActive = false;

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) && !istrailActive )
        {
            istrailActive = true;
            StartCoroutine(ActivateTrail(activeTime));
        }
    }
}
