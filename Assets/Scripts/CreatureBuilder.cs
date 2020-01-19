using System.Collections.Generic;
using UnityEngine;

public class CreatureBuilder : MonoBehaviour {

    private Torso _torso;
    private List<Bodypart> _bodyparts = new List<Bodypart>();
    
    public void SetTorso(GameObject torsoPrefab) {
        
        if (_torso != null) {

            foreach (var bodypart in _bodyparts) {
                bodypart.transform.SetParent(null);
            }
            Destroy(_torso.gameObject);
        }

        _torso = Instantiate(torsoPrefab, this.transform).GetComponent<Torso>();

        foreach (var part in _bodyparts) {
            CreatureJoint creatureJoint = _torso.GetJoint(part.Joint);
            part.Place(creatureJoint);
        }
    }

    public void SetBodypart(GameObject partPrefab) {

        Bodypart newPart = Instantiate(partPrefab).GetComponent<Bodypart>();
        CreatureJoint creatureJoint = _torso.GetJoint(newPart.Joint);
        newPart.Place(creatureJoint);
    }
}
