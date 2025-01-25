using UnityEngine;

public class GameLayers : MonoBehaviour
{
    [SerializeField] LayerMask solidObjects;
    [SerializeField] LayerMask interactableLayer;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] LayerMask defaultLayer;

    public static GameLayers i { get; set; }

    private void Awake(){
        i = this;
    }

    public LayerMask SolidObjects{
        get => solidObjects;
    }

    public LayerMask InteractableLayer{
        get => interactableLayer;
    }

    public LayerMask PlayerLayer{
        get => playerLayer;
    }

    public LayerMask DefaultLayer{
        get => defaultLayer;
    }
}
