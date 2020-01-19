using System;
using UnityEngine;

public class Torso : MonoBehaviour {

    public CreatureJoint[] creatureJoints;

    public CreatureJoint GetJoint(JointType jointType) {
        foreach (CreatureJoint joint in creatureJoints) {
            if (joint.Type == jointType) {
                return joint;
            }
        }
        
        throw new Exception(String.Format("Missing joint {0} on torso {1}", jointType, this.name));
    }
}