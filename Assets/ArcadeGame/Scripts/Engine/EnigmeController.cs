using UnityEngine;
using UnityEngine.Rendering.Universal;

public class EnigmeController : MonoBehaviour
{
   [SerializeField] private EnigmeDatas enigmeData;
   [SerializeField] private Light2D[] Lights;
   [SerializeField] private Light2D winlight;
   
   [SerializeField] private GameObject circularMovementTarget;
   [SerializeField] private float circularMovementRadius;

   private void Start()
   {
      for (int i =0; i < Lights.Length; i++)
      {
         Lights[i].enabled = enigmeData.LampsActivated[i];
      }
      winlight.enabled = enigmeData.IsAllActivated();
   }

   private void Update()
   {
      circularMovementTarget.transform.rotation = Quaternion.Euler(0, 180, 0);
   }
}
