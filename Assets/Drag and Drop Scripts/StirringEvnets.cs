using UnityEngine;
using UnityEngine.UI;
using static Stirring;

public class StirringEvnets : MonoBehaviour
{
    [SerializeField] Image Arrow;
    //[SerializeField] SpriteRenderer Arrow;
    [SerializeField] Sprite CounterClock;
    [SerializeField] Sprite Clock;
    [SerializeField] Stirring stirring;



    public void DirectionChange()
    {

        if (stirring.CurrentDirection == StirDirection.Clock)
        {
            Arrow.color = Color.red;
            //Arrow.sprite = Clock;
        }
        if(stirring.CurrentDirection == StirDirection.CounterClock)
        {
            Arrow.color = Color.blue;
            //Arrow.sprite = CounterClock;
        }
    }
}
