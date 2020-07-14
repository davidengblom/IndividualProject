using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public string unitName;
    public int unitLevel;

    public int damage;
    public int maxHP;
    public int currentHP;
    public float critChance;
    public float hitChance;

    private Vector3 originPosition;
    private Quaternion originRotation;
    public float shakeDecay = 0.002f;
    public float shakeIntensity = 0.3f;

    private float tempShakeIntensity = 0;

    public bool frozen = false;

    void Update()
    { 
        if (tempShakeIntensity > 0) //Shake logic @courtesy https://gist.github.com/GuilleUCM/d882e228d93c7f7d0820
        {
            transform.position = originPosition + Random.insideUnitSphere * tempShakeIntensity;
            transform.rotation = new Quaternion(
                originRotation.x + Random.Range(-tempShakeIntensity, tempShakeIntensity) * 0.2f,
                originRotation.y + Random.Range(-tempShakeIntensity, tempShakeIntensity) * 0.2f,
                originRotation.z + Random.Range(-tempShakeIntensity, tempShakeIntensity) * 0.2f,
                originRotation.w + Random.Range(-tempShakeIntensity, tempShakeIntensity) * 0.2f);
            tempShakeIntensity -= shakeDecay;
        }
    }

    public void Frozen()
    {
        if(!frozen)
        {
            GetComponentInChildren<SpriteRenderer>().color = Color.blue;
        }
        else
        {
            GetComponentInChildren<SpriteRenderer>().color = Color.white;
        }
    }

    public bool TakeDamage(int damage) 
    {
        currentHP -= damage;
        
        if(currentHP <= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void Shake() //Call when you need to shake
    {
        originPosition = transform.position;
        originRotation = transform.rotation;
        tempShakeIntensity = shakeIntensity;
    }
}
