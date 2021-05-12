using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AugmentDataContainer : MonoBehaviour
{
    public Augment augmentInfo;

    public AugmentDataContainer(Augment augment)
    {
        this.augmentInfo = augment;
    }

    public void UpdateAugmentData(Augment augment)
    {
        this.augmentInfo = augment;
    }
}
