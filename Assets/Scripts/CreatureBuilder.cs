using System;
using System.Collections.Generic;
using EvolvingWilds;
using UnityEngine;

public class CreatureBuilder : MonoBehaviour {

    private Creature _creature;
    private Torso _torso;
    private List<Bodypart> _bodyparts = new List<Bodypart>();

    public void Clear() {
        Destroy(_torso.gameObject);
        _bodyparts.Clear();
    }

    public void SetTorso(GameObject torsoPrefab) {
        _creature = GetComponent<Creature>();
        
        if (_torso != null) {

            foreach (var bodypart in _bodyparts) {
                bodypart.transform.SetParent(null);
            }
            Destroy(_torso.gameObject);
        }

        _torso = Instantiate(torsoPrefab, this.transform).GetComponent<Torso>();
        _torso.SetColor(_creature.Species.Color);
        
        foreach (var part in _bodyparts) {
            CreatureJoint creatureJoint = _torso.GetJoint(part.Joint);
            part.Place(creatureJoint);
        }
    }

    public void SetBodypart(GameObject partPrefab) {

        Bodypart newPart = Instantiate(partPrefab).GetComponent<Bodypart>();
        CreatureJoint creatureJoint = _torso.GetJoint(newPart.Joint);
        newPart.SetColor(_creature.Species.Color);
        newPart.Place(creatureJoint);
    }
}
