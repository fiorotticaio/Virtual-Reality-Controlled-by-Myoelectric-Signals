// TODO: Passar o código para ingles

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldObject : MonoBehaviour
{
    private bool isHoldingObject = false;
    private Transform objectToHold; // Objeto que será segurado pela mão

    void OnCollisionEnter(Collision collision)
    {
        // Verifica se a colisão envolve o objeto que deve ser segurado
        if (collision.transform.CompareTag("ObjectToHold"))
        {
            Debug.Log("Entrou");
            // Define o objeto a ser segurado como o objeto envolvido na colisão
            objectToHold = collision.transform;
            isHoldingObject = true;
        }
    }

    // void OnCollisionExit(Collision collision)
    // {
    //     // Verifica se a colisão envolve o objeto que estava sendo segurado
    //     if (collision.transform == objectToHold)
    //     {
    //         // Limpa o objeto que estava sendo segurado
    //         objectToHold = null;
    //         isHoldingObject = false;
    //     }
    // }

    void Update()
    {
        // Verifica se o objeto a ser segurado está definido e se a mão está segurando
        if (objectToHold != null && isHoldingObject)
        {
            // Define a posição e rotação da mão para coincidir com o objeto a ser segurado
            objectToHold.position = transform.position;
            objectToHold.rotation = transform.rotation;
        }
    }
}
