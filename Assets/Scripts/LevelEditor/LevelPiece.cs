using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelPiece : MonoBehaviour
{
    // public ParticleSystem hit;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}

   /*  void OnTriggerEnter(Collider other){
        

        if(hit != null){
            if (other.gameObject.tag == "Bullet Player" && gameObject.tag != "Life" && gameObject.tag != "Indestructible"){
                Instantiate(hit, transform.position, Quaternion.identity);
                hit.Play();
            }
        }
        
        if (gameObject.tag == "Life"){
            if(other.gameObject.tag == "Player"){
                GameManager.instance.health ++;
                GameManager.instance.life.text = "Life : " + GameManager.instance.health.ToString();
                Destroy(gameObject);
                if(hit != null){
                    Instantiate(hit, transform.position, Quaternion.identity);
                    hit.Play();         
                }
            }    
        }
        else
        {
            if(other.gameObject.tag == "Player"){
                GameManager.instance.health--;
                GameManager.instance.life.text = "Life : " + GameManager.instance.health.ToString();
                Destroy(gameObject);
                if(hit != null){
                    Instantiate(hit, transform.position, Quaternion.identity);
                    hit.Play();         
                }   
            }
        }   
    }
    */

