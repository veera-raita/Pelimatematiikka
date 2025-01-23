using System;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static EasingFunction;

public class LerpMath : MonoBehaviour
{
    [SerializeField] private BasicMath basics;
    [SerializeField] private Transform objA;
    [SerializeField] private Transform objB;
    [SerializeField] private Transform myObj;
    [Range(0f, 1f)][SerializeField] float t = 0f;
    [Range(0f, 10f)][SerializeField] float LerpTime = 5f;
    [SerializeField] EasingFunction.Ease ease = EasingFunction.Ease.EaseInOutQuad;
    float timer = 0f;
    bool reverse = false;

    private void Update()
    {
        if (!reverse)
        {
            timer += Time.deltaTime;
        }
        else
        {
            timer -= Time.deltaTime;
        }

        if (timer > LerpTime && !reverse) reverse = true;
        else if (timer <= 0 && reverse) reverse = false;

        t = timer / LerpTime;

        EasingFunction.Function func = GetEasingFunction(ease);

        t = func(0f, 1f, t);

        Vector3 pos = (1 - t) * (objA.position) + t * (objB.position);
        myObj.position = pos;
    }

    private void OnDrawGizmos()
    {
        //origin to objects
        basics.DrawVector(transform.position, objA.position - transform.position, Color.blue, 3f);
        basics.DrawVector(transform.position, objB.position - transform.position, Color.red, 3f);

        //between objects
        Handles.color = Color.white;
        Handles.DrawLine(objA.position, objB.position, 3f);

        //draw segments of previous line
        Vector3 partial1 = (1 - t) * (objA.position - transform.position);
        Vector3 partial2 = t * (objB.position - transform.position);
        basics.DrawVector(transform.position, partial1, Color.magenta, 3f);
        basics.DrawVector(transform.position + partial1, partial2, Color.magenta, 3f);

        //Vector3 posX = partial1 + partial2;
        //myObj.position = posX;
    }

    //thought i'd need this for more complex lerping but then we grabbed easing functions from the web
    public double CheapPow(double num, int exp)
    {
        //account for negative exponent
        if (exp < 0) return (1 / CheapPow(num, Math.Abs(exp)));

        double result = 1.0;

        while (exp > 0)
        {
            if ((exp & 1) != 0)
            {
                result *= num;
            }
            exp >>= 1;
            num *= num;
        }

        return result;
    }
}