using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{    
    [SerializeField] protected Animator animator;        
    [SerializeField] protected Rigidbody rb;
    
    protected SpriteRenderer CharRenderer;

   
    protected void LoadComponent() 
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();                
    }
    
}
