using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightOffset : MonoBehaviour {

    [SerializeField] private float amount;                      //Variable creada para ver cuanto queremos que se mueva el arma para la derecha/izquierda
    [SerializeField] private float maxAmount;                   //Maxima cantidad que va a oscilar
    [SerializeField] private float smoothAmount;                //Velocidad de que va a oscilar

    private Vector3 initialPosition;

    void Start()
    {
        initialPosition = transform.localPosition;              //Respeta la posicion del parent
    }

    void Update()
    {

        float movementX = -Input.GetAxis("Mouse X") * amount;   //Si me muevo para la derecha el arma se va a ir para la izquierda
        float movementY = -Input.GetAxis("Mouse Y") * amount;
        movementX = Mathf.Clamp(movementX, -maxAmount, maxAmount); //Aca estamos limitando movementX para que no oscile demasiado
        movementY = Mathf.Clamp(movementY, -maxAmount, maxAmount);

        Vector3 finalPosition = new Vector3(movementX, movementY, 0); //Calculamos la nueva posicion donde va a estar el arma, etc
        transform.localPosition = Vector3.Lerp(transform.localPosition, finalPosition + initialPosition, Time.deltaTime * smoothAmount);
    }

}
