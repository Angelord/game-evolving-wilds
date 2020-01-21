using System;
using System.Collections.Generic;
using System.Linq;
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
        _creature = GetComponent<Creature>();
        
        Bodypart newPart = Instantiate(partPrefab).GetComponent<Bodypart>();

        if (_torso != null) {
            CreatureJoint creatureJoint = _torso.GetJoint(newPart.Joint);
            newPart.Place(creatureJoint);
        }

        newPart.SetColor(_creature.Species.Color);
        _bodyparts.Add(newPart);
    }

    public void RemoveBodypart(JointType jointType) {
        for(int i = _bodyparts.Count - 1; i >= 0; i--) {
            if (_bodyparts[i].Joint == jointType) {
                Destroy(_bodyparts[i].gameObject);
                _bodyparts.RemoveAt(i);
                return;
            }
        }
    }
}
