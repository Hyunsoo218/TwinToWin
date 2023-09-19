
using UnityEngine;
using UnityEngine.UI;

public class FeverGauge : MonoBehaviour
{
    public static FeverGauge instance;

    private StateMachine cStateMachine;
    private State cRedGauge = new State("RedGauge");
    private State cBlueGauge = new State("BlueGauge");

    public Image imgGaugeFillImage;
    private int fRedGauge = 0;
    private int fBlueGauge = 0;
    private void Awake()
    {
        Initialize();
        StateInitalizeOnStay();
    }

    private void Initialize()
    {
        cStateMachine = GetComponent<StateMachine>();
        cStateMachine.ChangeState(cBlueGauge);
    }

    private void StateInitalizeOnStay()
    {
        cRedGauge.onStay += () =>
        {
            imgGaugeFillImage.color = Color.red;
        };
        cBlueGauge.onStay += () =>
        {
            imgGaugeFillImage.color = Color.blue;
        };
    }

    public void IncreaseFeverGauge()
    {
        if (Player.instance.cCurrentCharacter == Player.instance.GetTwinSword())
        {

        }
    }
}
