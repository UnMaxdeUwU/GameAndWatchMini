using UnityEngine;
using UnityEngine.Rendering.Universal;

public class EnigmeController : MonoBehaviour
{
   [SerializeField] private EnigmeDatas enigmeData;
   [SerializeField] private Light2D[] Lights;
   [SerializeField] private Light2D winlight;

   private void Start()
   {
      for (int i =0; i < Lights.Length; i++)
      {
         Lights[i].enabled = enigmeData.LampsActivated[i];
      }
      winlight.enabled = enigmeData.IsAllActivated();
   }
}
