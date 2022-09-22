using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animation : MonoBehaviour
{
    [SerializeField] GameObject _effectPrefab;
    [SerializeField] GameObject _posObj;
    [SerializeField] GameObject _soundPrefab;


    public void AnimationEvent(AnimationEvent animationEvent)
    {
        if (animationEvent.stringParameter == "hit")
        {
            var effect = Instantiate(_effectPrefab, _posObj.transform) as GameObject;
            Destroy(effect, 1);

            if (animationEvent.objectReferenceParameter != null)
            {
                var obj = Instantiate(animationEvent.objectReferenceParameter) as GameObject;
                obj.transform.position = transform.position;
                obj.GetComponent<AudioSource>().Play();
            }

        }
    }
}
