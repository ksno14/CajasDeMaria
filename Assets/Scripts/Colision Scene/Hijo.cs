using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hijo : Colision
{
    void OnCollisionEnter(Collision collision){
        if(collision.gameObject.name!="Caja"){
        listaAñadir(gameObject.name);
        }
    }

    void OnCollisionExit(Collision collision) {
       if(collision.gameObject.name!="Caja"){
        listaRemover(gameObject.name);
        }
    }
}

