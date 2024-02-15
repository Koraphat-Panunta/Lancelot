using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{    
    [SerializeField] protected Animator animator;        
    [SerializeField] protected Rigidbody rb;
    
    protected SpriteRenderer CharRenderer;

    float StopDuration = 0;
    protected void LoadComponent() 
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();                
    }
    protected virtual void Update()
    {
        if (Time.fixedDeltaTime == 100)
        {
            if (StopDuration >= 1)
            {
               
                StopDuration = 0;
            }
            StopDuration += Time.deltaTime;
        }
    }               
    protected virtual void FixedUpdate()
    {
        
    }
    public void Stop_Frame() 
    {
       
    }
    
}
