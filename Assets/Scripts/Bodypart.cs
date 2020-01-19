using UnityEngine;

public class Bodypart : MonoBehaviour {

    public JointType Joint;
    
    public void Place(CreatureJoint creatureJoint) {
        transform.SetParent(creatureJoint.Transform);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }
}