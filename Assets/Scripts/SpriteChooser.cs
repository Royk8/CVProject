using UnityEngine;

public class SpriteChooser : MonoBehaviour
{
    [SerializeField] private GameObject beam, concrete;

    public void SetBeam(bool isBeam)
    {
        beam.SetActive(isBeam);
        concrete.SetActive(!isBeam);
    }
}
